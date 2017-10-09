using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ReplenishmentsController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ListOfClients = ApplicationService.ddlClientIDs();

            return View();
        }

        public ActionResult BulkItemsList(int id)
        {
            int bulkOrderId = id;
            ViewBag.ParentKey = bulkOrderId;

            var bulkOrderItems = ReplenishmentsService.BulkOrderItems(bulkOrderId);

            return PartialView("~/Views/Replenishments/_BulkOrderItems.cshtml", bulkOrderItems);
        }

        [HttpPost]
        public ActionResult SearchResultsUserCriteria(FormCollection form, string mode)
        {
            var bulkOrders = ReplenishmentsService.GetBulkOrders(form, "User");

            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", bulkOrders);
        }

        [HttpGet]
        public ActionResult SearchResults(FormCollection form, string mode)
        {
            var bulkOrders = ReplenishmentsService.GetBulkOrders(form, mode);

            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", bulkOrders);
        }

        
        public ActionResult Edit(int id)
        {
            int bulkOrderId = id;
            var bulkOrder = ReplenishmentsService.GetBulkOrder(bulkOrderId);

            return View("~/Views/Replenishments/Edit.cshtml", bulkOrder);
        }

        [HttpPost]
        public ActionResult Save(BulkOrder bulkOrder)
        {
            // Always save the Bulk Order then maybe re-direct to SendEmail
            string btn = Request.Form["btnSave"];
            int bulkOrderId = ReplenishmentsService.SaveBulkOrder(bulkOrder);

            if (btn == "Send Email")
            {
                return RedirectToAction("Email", bulkOrder);
            }

            TempData["SaveResult"] = "Order Saved on " + DateTime.UtcNow.ToString("R");

            return RedirectToAction("Edit", new { id = bulkOrderId });
        }

        public ActionResult GetSupplyIDEmail(int clientid, string supplyid)
        {
            // called from view to insert value into view
            return Content(ReplenishmentsService.GetSupplierEmail(clientid, supplyid));
        }

        public ActionResult Email(BulkOrder bulkOrder)
        {
            var email = ReplenishmentsService.BuildBulkOrderEmail(bulkOrder);

            return View("Email", email);
        }

        [HttpPost]
        public ActionResult Email(BulkOrderEmailViewModel bulkOrderEmail)
        {
            ReplenishmentsService.BuildSendEmail(bulkOrderEmail);

            return RedirectToAction("Index");
        }
                
        [HttpGet]
        public ActionResult CreateItem(int id)
        {
            int bulkOrderId = id;
            var bulkOrderItem = ReplenishmentsService.CreateBulkOrderItem(bulkOrderId);

            return PartialView("~/Views/Replenishments/_BulkOrderItemModal.cshtml", bulkOrderItem);
        }

        [HttpGet]
        public ActionResult EditItem(int id)
        {
            int bulkOrderItemId = id;
            var bulkOrderItem = ReplenishmentsService.GetBulkOrderItem(bulkOrderItemId);

            return PartialView("~/Views/Replenishments/_BulkOrderItemModal.cshtml", bulkOrderItem);
        }

        [HttpPost]
        public ActionResult SaveItem(BulkOrderItem bulkOrderItem)
        {
            // Catch form if no master code is selected
            if (bulkOrderItem.productmasterid == 0)
            {
                return Content("Please Select Master Code");
            }
            else
            {
                int bulkOrderItemId = ReplenishmentsService.SaveBulkOrderItem(bulkOrderItem);

                return Content("Item Saved on " + DateTime.UtcNow.ToString("R"));
            }
        }

        [HttpGet]
        public ActionResult DeleteItem(int id)
        {
            int bulkOrderItemId = id;
            int bulkItemId = ReplenishmentsService.DeleteBulkOrderItem(bulkOrderItemId);

            return Content("Deleted");
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ListOfClients = ApplicationService.ddlClientIDs();

            return View("~/Views/Replenishments/Create.cshtml");
        }

        [HttpGet]
        public ActionResult CreateFromInventory(int productDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(productDetailId);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);
                ViewBag.ProductDetailID = productDetailId;
                Session["InventoryProductDetailID"] = productDetailId;
            }
            
            return View("~/Views/Replenishments/Create.cshtml");
        }

        public ActionResult BuildDivisionDropDown(int clientid)
        {
            return Content(ApplicationService.ddlBuildDivision(clientid));
        }

        [HttpPost]
        public ActionResult CreateSuggestedOrder(FormCollection form)
        {
            // Generate the suggested items, let the view refresh the partial after the ajax POST
            int clientId = Convert.ToInt32(form["clientid"]);
            int divisionId = Convert.ToInt32(form["divisionid"]);
            int itemsCreated = ReplenishmentsService.GenerateSuggestedBulkOrder(clientId, divisionId);
            Session["SuggestedBulkOrderItemClientID"] = clientId;

            return null;
        }

        public ActionResult SuggestedItemsList()
        {
            var suggestedItems = ReplenishmentsService.GetSuggestedBulkItems();

            return PartialView("~/Views/Replenishments/_SuggestedItems.cshtml", suggestedItems);
        }

        [HttpGet]
        public ActionResult DeleteSuggestedItem(int id)
        {
            int bulkItemId = id;
            ReplenishmentsService.DeleteSuggestedBulkItem(bulkItemId);

            return null;
        }

        [HttpGet]
        public ActionResult EditSuggestedItem(int id)
        {
            int bulkItemId = id;
            var bulkOrderItem = ReplenishmentsService.GetSuggestedBulkItem(bulkItemId);

            return PartialView("~/Views/Replenishments/_SuggestedItemModal.cshtml", bulkOrderItem);
        }

        [HttpGet]
        public ActionResult AddBulkItem(int clientid)
        {
            var bulkOrderItem = new SuggestedBulkOrderItem();
            bulkOrderItem.id = -1;
            bulkOrderItem.clientid = clientid;

            return PartialView("~/Views/Replenishments/_SuggestedItemModal.cshtml", bulkOrderItem);
        }

        public ActionResult SaveSuggestedItem(SuggestedBulkOrderItem bulkOrderItem)
        {
            int bulkItemId = ReplenishmentsService.SaveSuggestedBulkItem(bulkOrderItem);
            
            return Content("Item Saved on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult PrintSuggested()
        {
            var bulkOrderItems = ReplenishmentsService.GetSuggestedBulkItems();

            return View("~/Views/Replenishments/PrintSuggested.cshtml", bulkOrderItems);
        }

        [HttpGet]
        public ActionResult CreateBulkOrders()
        {
            int orderItemCount = ReplenishmentsService.CreateSuggestedBulkOrder();
            int productDetailId = Convert.ToInt32(Session["InventoryProductDetailID"]);

            if (productDetailId > 0)
            {   
                Session["InventoryProductDetailID"] = null;

                return RedirectToAction("Edit", "Inventory", new { id = productDetailId });
            }

            return RedirectToAction("Index");
        }
    }
}