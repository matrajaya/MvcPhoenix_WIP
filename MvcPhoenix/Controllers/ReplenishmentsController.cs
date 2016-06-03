using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ReplenishmentsController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public ActionResult Index()
        {
            ViewBag.ListOfClients = ReplenishmentsService.fnClientIDs();
            return View();
        }

        public ActionResult ItemsList(int id)
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
            List<BulkOrderSearchResults> mylist = ReplenishmentsService.fnSearchResults(fc, "User");
            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", mylist);
        }

        [HttpGet]
        public ActionResult SearchResults(FormCollection fc, string mode)
        {
            List<BulkOrderSearchResults> mylist = ReplenishmentsService.fnSearchResults(fc, mode);
            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", mylist);
        }

        #endregion SearchActions ------------------------------------------------------

        #region OrderEdit ---------------------------------------------------------

        public ActionResult Edit(int id)
        {
            BulkOrder obj = new BulkOrder();
            obj = ReplenishmentsService.fnFillBulkOrderFromDB(id);
            return View("~/Views/Replenishments/Edit.cshtml", obj);
        }

        [HttpPost]
        public ActionResult Save(BulkOrder obj)
        {
            string btn = Request.Form["btnSave"];
            int pk = ReplenishmentsService.fnSaveBulkOrder(obj);
            // Trap the Send Email action
            if (btn == "Send Email")
            {
                var message = ReplenishmentsService.fnCreateEmail(obj);
                return View("~/Views/Replenishments/Email.cshtml", message);
            }
            TempData["SaveResult"] = "Order Saved at " + DateTime.Now.ToString();
            return RedirectToAction("Edit", new { id = pk });
        }

        public ActionResult GetSupplyIDEmail(int clientid, string supplyid)
        {
            return Content(ReplenishmentsService.fnGetSupplyIDEmail(clientid, supplyid));
        }

        #endregion OrderEdit ---------------------------------------------------------

        #region Email -----------------------------------------------------------

        [HttpPost]
        public ActionResult SendEmail(BulkOrderEmailViewModel obj)
        {
            ReplenishmentsService.fnSendEmail(obj);
            return RedirectToAction("Edit", new { id = obj.bulkorderid });
        }

        #endregion Email -----------------------------------------------------------

        #region Items --------------------------------------------------------------------

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
            int pk = ReplenishmentsService.fnSaveItem(obj);
            return Content("Item Saved at " + DateTime.Now.ToString());
        }

        [HttpGet]
        public ActionResult DeleteItem(int id)
        {
            int pk = ReplenishmentsService.fnDeleteItem(id);
            //return PartialView("~/Views/Replenishments/_BulkOrderItems.cshtml", obj);
            return Content("Deleted");
        }

        #endregion Items --------------------------------------------------------------------

        #region SuggestedBulkOrder ------------------------------------------------

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ListOfClients = ReplenishmentsService.fnClientIDs();
            return View("~/Views/Replenishments/Create.cshtml");
        }

        public ActionResult BuildDivisionDropDown(int id)
        {
            // id=clientid
            return Content(ReplenishmentsService.fnBuildDivisionDropDown(id));
        }

        [HttpPost]
        public ActionResult CreateSuggestedOrder(SuggestedBulkOrder obj)
        {
            // Generate the suggested items and return partial
            int ItemsCreated = ReplenishmentsService.fnGenerateSuggestedOrder(obj);
            List<SuggestedBulkOrderItem> mylist = ReplenishmentsService.fnSuggestedItemsList();
            Session["SuggestedBulkOrderItemClientID"] = obj.clientid;
            return PartialView("~/Views/Replenishments/_SuggestedItems.cshtml", mylist);
        }

        public ActionResult SuggestedItemsList()
        {
            // for the partial
            List<SuggestedBulkOrderItem> mylist = ReplenishmentsService.fnSuggestedItemsList();
            return PartialView("~/Views/Replenishments/_SuggestedItems.cshtml", mylist);
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
            SuggestedBulkOrderItem obj = new SuggestedBulkOrderItem();
            obj = ReplenishmentsService.fnFillSuggestedItemfromDB(id);
            return PartialView("~/Views/Replenishments/_SuggestedItemModal.cshtml", obj);
        }

        [HttpGet]
        public ActionResult CreateSuggestedItem(int ClientID)
        {
            SuggestedBulkOrderItem obj = new SuggestedBulkOrderItem();
            obj = ReplenishmentsService.fnCreateSuggestedBulkOrderItem(ClientID);
            return PartialView("~/Views/Replenishments/_SuggestedItemModal.cshtml", obj);
        }

        public ActionResult SaveSuggestedItem(SuggestedBulkOrderItem obj)
        {
            int pk = ReplenishmentsService.fnSaveSuggestedItem(obj);
            return Content("Item Saved at " + DateTime.Now.ToString());
        }

        [HttpGet]
        public ActionResult CreateBulkOrders()
        {
            int OrderCount = ReplenishmentsService.fnCreateBulkOrders();
            return RedirectToAction("Index");
            //return Content("New Orders Created = " + OrderCount.ToString());
        }

        #endregion SuggestedBulkOrder ------------------------------------------------
    }
}