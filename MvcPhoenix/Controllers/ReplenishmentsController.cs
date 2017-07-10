﻿using MvcPhoenix.EF;
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
            // Build list of BulkOrderItems for use in partial
            List<BulkOrderItem> mylist = ReplenishmentsService.fnItemsList(id);
            ViewBag.ParentKey = id;

            return PartialView("~/Views/Replenishments/_BulkOrderItems.cshtml", mylist);
        }

        #region SearchActions ------------------------------------------------------

        [HttpPost]
        public ActionResult SearchResultsUserCriteria(FormCollection fc, string mode)
        {
            var mylist = ReplenishmentsService.fnSearchResults(fc, "User");

            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", mylist);
        }

        [HttpGet]
        public ActionResult SearchResults(FormCollection fc, string mode)
        {
            var mylist = ReplenishmentsService.fnSearchResults(fc, mode);

            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", mylist);
        }

        #endregion SearchActions ------------------------------------------------------

        #region OrderEdit ---------------------------------------------------------

        public ActionResult Edit(int id)
        {
            BulkOrder vm = new BulkOrder();
            vm = ReplenishmentsService.fnFillBulkOrderFromDB(id);

            return View("~/Views/Replenishments/Edit.cshtml", vm);
        }

        [HttpPost]
        public ActionResult Save(BulkOrder obj)
        {
            // Always save the Bulk Order then maybe re-direct to SendEmail
            string btn = Request.Form["btnSave"];
            int pk = ReplenishmentsService.fnSaveBulkOrder(obj);

            if (btn == "Send Email")
            {
                return RedirectToAction("Email", obj);
            }

            TempData["SaveResult"] = "Order Saved on " + DateTime.UtcNow.ToString("R");

            return RedirectToAction("Edit", new { id = pk });
        }

        public ActionResult GetSupplyIDEmail(int clientid, string supplyid)
        {
            // called from view to insert value into view
            return Content(ReplenishmentsService.fnGetSupplyIDEmail(clientid, supplyid));
        }

        #endregion OrderEdit ---------------------------------------------------------

        #region Email

        public ActionResult Email(BulkOrder obj)
        {
            var EmailModel = ReplenishmentsService.fnCreateEmail(obj);

            return View("Email", EmailModel);
        }

        [HttpPost]
        public ActionResult Email(BulkOrderEmailViewModel obj)
        {
            ReplenishmentsService.fnSendEmail(obj);

            return RedirectToAction("Index");
        }

        #endregion Email

        #region Replenishment Order Items

        [HttpGet]
        public ActionResult CreateItem(int id)
        {
            // id = bulkorderid ..create a new line item to an existing bulkorderid
            BulkOrderItem obj = new BulkOrderItem();
            obj = ReplenishmentsService.fnCreateItem(id);

            return PartialView("~/Views/Replenishments/_BulkOrderItemModal.cshtml", obj);
        }

        [HttpGet]
        public ActionResult EditItem(int id)
        {
            BulkOrderItem obj = new BulkOrderItem();
            obj = ReplenishmentsService.fnFillItemFromDB(id);

            return PartialView("~/Views/Replenishments/_BulkOrderItemModal.cshtml", obj);
        }

        [HttpPost]
        public ActionResult SaveItem(BulkOrderItem obj)
        {
            // Catch form if no master code is selected
            if (obj.productmasterid == 0)
            {
                return Content("Please Select Master Code");
            }
            else
            {
                int pk = ReplenishmentsService.fnSaveItem(obj);

                return Content("Item Saved on " + DateTime.UtcNow.ToString("R"));
            }
        }

        [HttpGet]
        public ActionResult DeleteItem(int id)
        {
            int pk = ReplenishmentsService.fnDeleteItem(id);

            return Content("Deleted");
        }

        #endregion Replenishment Order Items

        #region SuggestedBulkOrder

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ListOfClients = ApplicationService.ddlClientIDs();

            return View("~/Views/Replenishments/Create.cshtml");
        }

        [HttpGet]
        public ActionResult CreateFromInventory(int productdetailid) // id=productdetailid
        {
            using (var db = new CMCSQL03Entities())
            {
                var pd = db.tblProductDetail.Find(productdetailid);
                var pm = db.tblProductMaster.Find(pd.ProductMasterID);
                ViewBag.ProductDetailID = productdetailid;
                Session["InventoryProductDetailID"] = productdetailid;                           // so CreateBulkOrders Action below can redirect back to Inventory

                return View("~/Views/Replenishments/Create.cshtml");
            }
        }

        public ActionResult BuildDivisionDropDown(int clientid)
        {
            return Content(ApplicationService.ddlBuildDivisionDropDown(clientid));
        }

        [HttpPost]
        public ActionResult CreateSuggestedOrder(FormCollection fc)
        {
            // Generate the suggested items, let the view refresh the partial after the ajax POST
            int clientid = Convert.ToInt32(fc["clientid"]);
            int divisionid = Convert.ToInt32(fc["divisionid"]);
            int ItemsCreated = ReplenishmentsService.fnGenerateSuggestedOrder(clientid, divisionid);
            Session["SuggestedBulkOrderItemClientID"] = clientid;

            return null;
        }

        public ActionResult SuggestedItemsList()
        {
            List<SuggestedBulkOrderItem> suggesteditems = ReplenishmentsService.fnSuggestedItemsList();

            return PartialView("~/Views/Replenishments/_SuggestedItems.cshtml", suggesteditems);
        }

        [HttpGet]
        public ActionResult DeleteSuggestedItem(int id)
        {
            ReplenishmentsService.fnDeleteSuggestedItem(id);

            return null;
        }

        [HttpGet]
        public ActionResult EditSuggestedItem(int id)
        {
            SuggestedBulkOrderItem vm = new SuggestedBulkOrderItem();
            vm = ReplenishmentsService.fnFillSuggestedItemfromDB(id);

            return PartialView("~/Views/Replenishments/_SuggestedItemModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult CreateSuggestedItem(int clientid)
        {
            SuggestedBulkOrderItem vm = new SuggestedBulkOrderItem();
            vm = ReplenishmentsService.fnCreateSuggestedBulkOrderItem(clientid);

            return PartialView("~/Views/Replenishments/_SuggestedItemModal.cshtml", vm);
        }

        public ActionResult SaveSuggestedItem(SuggestedBulkOrderItem obj)
        {
            int pk = ReplenishmentsService.fnSaveSuggestedItem(obj);
            
            return Content("Item Saved on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult PrintSuggested()
        {
            List<SuggestedBulkOrderItem> vm = ReplenishmentsService.fnSuggestedItemsList();

            return View("~/Views/Replenishments/PrintSuggested.cshtml", vm);
        }

        [HttpGet]
        public ActionResult CreateBulkOrders()
        {
            int OrderCount = ReplenishmentsService.fnCreateBulkOrders();
            int iSession = Convert.ToInt32(Session["InventoryProductDetailID"]);

            if (iSession > 0)
            {                
                int pdid = Convert.ToInt32(Session["InventoryProductDetailID"]);                // reroute back to Inventory
                Session["InventoryProductDetailID"] = null;                                     // reset

                return RedirectToAction("Edit", "Inventory", new { id = pdid });
            }

            return RedirectToAction("Index");
        }

        #endregion SuggestedBulkOrder
    }
}