﻿using MvcPhoenix.Models;
using MvcPhoenix.EF;
using MvcPhoenix.Services;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class OrdersController : Controller
    {
        //private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        #region Order Master Methods

        public ActionResult Index()
        {
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          orderby t.orderdate descending
                          select t).Take(20).ToList();  // refine for here

            TempData["SearchResultsMessage"] = "Last 20 Orders";
            List<SelectListItem> clientlist = new List<SelectListItem>();
            clientlist = OrderService.fnListOfClientIDs();
            ViewBag.NewClientID = clientlist;

            return View("~/Views/Orders/Index.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            int ClientID = Convert.ToInt32(fc["NewClientID"]);
            var vm = OrderService.fnCreateOrder(ClientID);
            return View("~/Views/Orders/Edit.cshtml", vm);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var vm = OrderService.fnFillOrder(id);
            return View("~/Views/Orders/Edit.cshtml", vm);
        }

        [HttpPost]
        public ActionResult Save(OrderMasterFull vm)
        {
            int pk = OrderService.fnSaveOrder(vm);
            TempData["SaveResult"] = "Order Information updated on " + DateTime.UtcNow.ToString("R");
            return RedirectToAction("Edit", new { id = pk });
        }

        #endregion

        #region Printing Actions

        private static string DocumentFooter()
        {
            string footer = "--footer-left \"Printed on: " +
                DateTime.UtcNow.ToString("R") +
                "                                                                                                                                    " +
                " Page: [page]/[toPage]\"" +
                " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
            return footer;
        }

        public ActionResult PrintPickPack(int id)
        {
            string footer = DocumentFooter();
            var vm = OrderService.fnFillOrder(id);

            return new ViewAsPdf(vm) { CustomSwitches = footer };
        }

        [HttpGet]
        public ActionResult PrintPickOrderItems(int id)
        {
            var qry = PrintListOrderItems(id);
            if (qry.Count > 0)
            {
                return PartialView("~/Views/Orders/_PrintPickOrderItems.cshtml", qry);
            }

            return null;
        }

        [HttpGet]
        public ActionResult PrintPackOrderItems(int id)
        {
            var qry = PrintListOrderItems(id);
            if (qry.Count > 0)
            {
                return PartialView("~/Views/Orders/_PrintPackOrderItems.cshtml", qry);
            }

            return null;
        }

        private List<OrderItem> PrintListOrderItems(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderItem
                           where t.OrderID == id && t.AllocateStatus == "A" && t.ShipDate == null
                           orderby t.ProductCode
                           select new MvcPhoenix.Models.OrderItem
                           {
                               OrderID = t.OrderID,
                               ItemID = t.ItemID,
                               ProductDetailID = t.ProductDetailID,
                               ProductCode = t.ProductCode,
                               ProductName = t.ProductName,
                               Qty = t.Qty,
                               Size = t.Size,
                               ShipDate = t.ShipDate,
                               CeaseShipDate = t.ShipDate, //Note TBD: ceaseshipdate = (stock:expiration_date) minus (profile:cease_ship_differential)
                               LotNumber = t.LotNumber,
                               Bin = t.Bin,
                               AirUnNumber = t.AirUnNumber,
                               GrnUnNumber = t.GrnUnNumber,
                               SeaUnNumber = t.GrnUnNumber, //Note: Add SeaUnNumber field to tblOrderItem
                               BackOrdered = t.BackOrdered,
                               AllocateStatus = t.AllocateStatus,
                               NonCMCDelay = t.NonCMCDelay,
                               DelayReason = t.DelayReason,
                               FreezableList = (from q in db.tblProductMaster
                                                where (q.MasterCode == t.ProductCode)
                                                select q.FreezableList).FirstOrDefault(),
                               HarmonizedCode = (from q in db.tblProductDetail
                                                 where (q.ProductDetailID == t.ProductDetailID)
                                                 select q.HarmonizedCode).FirstOrDefault(),
                               QtyAvailable = (from q in db.tblStock
                                               where q.ShelfID == t.ShelfID && (q.QtyOnHand - q.QtyAllocated >= t.Qty) && q.ShelfStatus == "AVAIL"
                                               select q).Count(),
                               AlertNotesOrderEntry = (from q in db.tblProductDetail
                                                       where q.ProductDetailID == t.ProductDetailID
                                                       select q.AlertNotesOrderEntry).FirstOrDefault(),
                               AlertNotesShipping = (from q in db.tblProductDetail
                                                     where q.ProductDetailID == t.ProductDetailID
                                                     select q.AlertNotesShipping).FirstOrDefault()
                           }).ToList();

                return qry;
            }
        }

        [HttpGet]
        public ActionResult PrintRemainingItems(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderItem
                           where t.OrderID == id && (t.ShipDate != null || t.AllocateStatus != "A")
                           orderby t.ProductCode
                           select new MvcPhoenix.Models.OrderItem
                           {
                               OrderID = t.OrderID,
                               ItemID = t.ItemID,
                               ProductDetailID = t.ProductDetailID,
                               ProductCode = t.ProductCode,
                               ProductName = t.ProductName,
                               Qty = t.Qty,
                               Size = t.Size,
                               LotNumber = t.LotNumber,
                               ShipDate = t.ShipDate,
                               BackOrdered = t.BackOrdered,
                               AllocateStatus = t.AllocateStatus,
                               AlertNotesOrderEntry = (from q in db.tblProductDetail
                                                       where q.ProductDetailID == t.ProductDetailID
                                                       select q.AlertNotesOrderEntry).FirstOrDefault(),
                               AlertNotesShipping = (from q in db.tblProductDetail
                                                     where q.ProductDetailID == t.ProductDetailID
                                                     select q.AlertNotesShipping).FirstOrDefault()
                           }).ToList();

                if (qry.Count > 0)
                {
                    return PartialView("~/Views/Orders/_PrintRemainingItems.cshtml", qry);
                }

                return null;
            }
        }

        public ActionResult PrintPreferredCarrierMatrix(string Country)
        {
            using (var db = new CMCSQL03Entities())
            {
                PreferredCarrierViewModel modelobj = new PreferredCarrierViewModel();
                var qry = (from t in db.tblPreferredCarrierList
                           where t.CountryName.Contains(Country)
                           select t).FirstOrDefault();

                modelobj.CountryCode = qry.CountryCode;
                modelobj.CountryName = qry.CountryName;
                modelobj.CommInvoiceReq = qry.CommInvoiceReq;
                modelobj.NonHazSm = qry.NonHaz_Sm;
                modelobj.NonHazLg = qry.NonHaz_Lg;
                modelobj.NonHazIncoTerms = qry.NonHaz_IncoTerms;
                modelobj.HazIATASm = qry.HazIATA_Sm;
                modelobj.HazIATALg = qry.HazIATA_Lg;
                modelobj.HazGroundLQ = qry.HazGround_LQ;
                modelobj.HazGround = qry.HazGround;
                modelobj.HazIncoterms = qry.Haz_Incoterms;
                modelobj.IncotermsAlt = qry.Incoterms_Alt;
                modelobj.NotesGeneral = qry.Notes_General;
                modelobj.NotesIATAADR = qry.Notes_IATA_ADR;
                modelobj.NonHazIncotermsAlt = qry.NonHazIncoterms_Alt;
                modelobj.HazIncotermsAlt = qry.HazIncoterms_Alt;

                if (qry == null) { return null; }

                return PartialView("~/Views/Orders/_PrintPreferredCarrierMatrix.cshtml", modelobj);
            }
        }

        #endregion Printing Actions

        #region Order SPS Billing Details

        public ActionResult EditSPSBilling(int id)
        {
            int orderid = id;
            var vm = OrderService.fnSPSBilling(orderid);
            ViewBag.ListOfCountries = OrderService.fnListOfCountries();

            return PartialView("~/Views/Orders/_SPSBillingModal.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SaveSPSBilling(OrderSPSBilling vm)
        {
            OrderService.fnSaveSPSBillingDetails(vm);
            return null;
        }

        #endregion

        #region Order Item Methods

        [HttpGet]
        public ActionResult fnOrderItemsList(int id)
        {
            // list of items for a given order
            using (var db = new CMCSQL03Entities())
            {
                var qry = (from t in db.tblOrderItem
                           where t.OrderID == id
                           orderby t.ProductCode
                           select new MvcPhoenix.Models.OrderItem
                           {
                               OrderID = t.OrderID,
                               ItemID = t.ItemID,
                               ShelfID = t.ShelfID,
                               BulkID = t.BulkID,
                               ProductDetailID = t.ProductDetailID,
                               ProductCode = t.ProductCode,
                               Mnemonic = t.Mnemonic,
                               ProductName = t.ProductName,
                               Size = t.Size,
                               SRSize = t.SRSize,
                               LotNumber = t.LotNumber,
                               Qty = t.Qty,
                               ShipDate = t.ShipDate,
                               Via = t.Via,
                               BackOrdered = t.BackOrdered,
                               AllocateStatus = t.AllocateStatus,
                               CSAllocate = t.CSAllocate,
                               NonCMCDelay = t.NonCMCDelay,
                               QtyAvailable = (from x in db.tblStock
                                               where (x.ShelfID != null) && (x.ShelfID == t.ShelfID) && (x.ShelfStatus == "AVAIL")
                                               select (x.QtyOnHand - x.QtyAllocated)).Sum(),
                               AllocatedDate = t.AllocatedDate,
                               GrnUnNumber = (from a in db.tblProductDetail
                                              where (a.ProductDetailID == t.ProductDetailID)
                                              select a.GRNUNNUMBER).FirstOrDefault(),
                               GrnPkGroup = (from b in db.tblProductDetail
                                             where (b.ProductDetailID == t.ProductDetailID)
                                             select b.GRNPKGRP).FirstOrDefault(),
                               AirUnNumber = (from c in db.tblProductDetail
                                              where (c.ProductDetailID == t.ProductDetailID)
                                              select c.AIRUNNUMBER).FirstOrDefault(),
                               AirPkGroup = (from d in db.tblProductDetail
                                             where (d.ProductDetailID == t.ProductDetailID)
                                             select d.AIRPKGRP).FirstOrDefault(),
                               CreateDate = t.CreateDate,
                               CreateUser = t.CreateUser,
                               UpdateDate = t.UpdateDate,
                               UpdateUser = t.UpdateUser
                           }).ToList();

                if (qry.Count > 0)
                {
                    return PartialView("~/Views/Orders/_OrderItems.cshtml", qry);
                }

                return null;
            }
        }

        [HttpGet]
        public ActionResult CreateItem(int id)
        {
            // id=orderid
            var vm = OrderService.fnCreateItem(id);
            ViewBag.ListOfDelayReasons = OrderService.fnListOfDelayReasons();

            return PartialView("~/Views/Orders/_OrderItemModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult EditItem(int id)
        {
            OrderItem vm = new OrderItem();
            vm = OrderService.fnFillOrderItem(id);
            ViewBag.ListOfDelayReasons = OrderService.fnListOfDelayReasons();

            return PartialView("~/Views/Orders/_OrderItemModal.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SaveItem(OrderItem incoming, string productdetailid)
        {
            if (productdetailid == "0" || productdetailid == "")
            {
                return Content("Please Select a Product");
            }

            int pk = OrderService.fnSaveItem(incoming);

            return Content("Item Saved at " + DateTime.UtcNow.ToString("R"));
        }

        public ActionResult DeleteItem(int id)
        {
            OrderService.fnDeleteOrderItem(id);
            return Content("Item Deleted");
        }
        
        public ActionResult BuildSizeDropDown(int id)
        {
            // id=clientid / return a <select> for <div>
            return Content(OrderService.fnBuildSizeDropDown(id));
        }

        public ActionResult CheckProductForAlert(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblProductDetail
                           where t.ProductDetailID == id
                           select t.AlertNotesOrderEntry).FirstOrDefault();

                return Content(obj);
            }
        }

        #endregion Order Item Methods

        #region Import Actions - Pull data from samplecenter db after transform in Access

        [HttpGet]
        public ActionResult OrdersImport()
        {
            using (var db = new CMCSQL03Entities())
            {
                var vm = (from t in db.tblOrderImport
                          where t.ImportStatus == "FAIL" && t.Location_MDB == "EU" && t.ImportError != null
                          orderby t.OrderDate, t.GUID
                          select t).ToList();

                return View("~/Views/Orders/Import.cshtml", vm);
            }
        }

        [HttpGet]
        public ActionResult OrdersImportProcess()
        {
            int ImportCount = OrderService.ImportOrders();        // Actual import
            TempData["ImportCount"] = ImportCount;

            return RedirectToAction("OrdersImport");
        }

        #endregion 

        [HttpGet]
        public ActionResult PullContactDetails(int id)
        {
            Contact obj = new Contact();
            obj = OrderService.fnGetClientContacts(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #region Allocate Methods

        public ActionResult AllocateFromShelf(int id, bool IncludeQCStock)
        {
            // The view handles the partial updates
            int AllocationCount = OrderService.fnAllocateShelf(id, IncludeQCStock);
            return Content(AllocationCount.ToString() + " item(s) allocated");
        }

        public ActionResult AllocateFromBulk(int id, bool IncludeQCStock)
        {
            int AllocationCount = OrderService.fnAllocateBulk(id, IncludeQCStock);
            return Content(AllocationCount.ToString() + " item(s) allocated");
        }

        #endregion Allocate Methods

        #region Order Transaction Methods

        [HttpGet]
        public ActionResult fnOrderTransList(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                // id=orderid
                var qry = (from t in db.tblOrderTrans
                           join items in db.tblOrderItem on t.OrderItemID equals items.ItemID into a
                           from qry2 in a.DefaultIfEmpty()
                           where t.OrderID == id
                           orderby t.UpdateDate descending
                           select new OrderTrans
                           {
                               ordertransid = t.OrderTransID,
                               orderid = t.OrderID,
                               orderitemid = t.OrderItemID,
                               productcode = qry2.ProductCode,
                               clientid = t.ClientID,
                               transdate = t.TransDate,
                               transtype = t.TransType,
                               transqty = t.TransQty,
                               transamount = t.TransAmount,
                               comments = t.Comments,
                               createdate = t.CreateDate,
                               createuser = t.CreateUser,
                               updatedate = t.UpdateDate,
                               updateuser = t.UpdateUser
                           }).ToList();

                return PartialView("~/Views/Orders/_OrderTrans.cshtml", qry);
            }
        }

        [HttpGet]
        public ActionResult CreateTrans(int id)
        {
            var vm = OrderService.fnCreateTrans(id);
            ViewBag.ListOrderItemIDs = OrderService.fnListOfOrderItemIDs(vm.orderid);
            return PartialView("~/Views/Orders/_OrderTransModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult EditTrans(int id)
        {
            var vm = OrderService.fnFillTrans(id);
            ViewBag.ListOrderItemIDs = OrderService.fnListOfOrderItemIDs(vm.orderid);
            return PartialView("~/Views/Orders/_OrderTransModal.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SaveTrans(OrderTrans vm)
        {
            int pk = OrderService.fnSaveTrans(vm);
            return Content("Transaction Saved on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult DeleteTrans(int id)
        {
            OrderService.fnDeleteTrans(id);
            return Content("Transaction Deleted on " + DateTime.UtcNow.ToString("R"));
        }

        #endregion Order Transaction Methods

        #region Index Order Search and Filters

        [HttpPost]
        public ActionResult LookupOrderID(FormCollection fc)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (String.IsNullOrEmpty(fc["orderid"]))
                {
                    return RedirectToAction("Index");
                }
                int id = Convert.ToInt32(fc["orderid"]);
                var qry = (from t in db.tblOrderMaster
                           where t.OrderID == id
                           select t).FirstOrDefault();

                if (qry != null)
                {
                    return RedirectToAction("Edit", new { id = id });
                }
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult OrdersNeedAllocation()
        {
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          where t.needallocationcount > 0
                          orderby t.orderid ascending
                          select t).ToList();
            TempData["SearchResultsMessage"] = "Orders Needing Allocation";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersToday()
        {
            DateTime datToday = DateTime.Today.AddDays(0);
            var orderslist = OrderService.fnOrdersSearchResults();
            //List<OrderMasterFull> orderslist = new List<OrderMasterFull>();
            //orderslist = fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          orderby t.orderid descending
                          where t.orderdate.Value.Date == datToday.Date
                          select t).ToList();
            TempData["SearchResultsMessage"] = "Orders Created Today";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersYesterday()
        {
            //TODO change to calc last Business day
            DateTime dateToday = DateTime.Today.AddDays(-1);
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          orderby t.orderid descending
                          where t.orderdate.Value.Date == dateToday.Date
                          select t).ToList();
            TempData["SearchResultsMessage"] = "Orders Created Yesterday";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersLastTen()
        {
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          orderby t.orderid descending
                          select t).Take(10).ToList();
            TempData["SearchResultsMessage"] = "Last 10 Orders";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersMineLastTen()
        {
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          where t.CreateUser == User.Identity.Name
                          orderby t.orderid descending
                          select t).Take(10).ToList();
            TempData["SearchResultsMessage"] = "My Last 10 Orders";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult AdvancedSearch()
        {
            return PartialView("~/Views/Orders/_AdvancedSearchModal.cshtml");
        }

        [HttpPost]
        public ActionResult LookupClientID(FormCollection fc)
        {
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            int pk = Convert.ToInt32(fc["ClientID"]);
            orderslist = (from t in orderslist
                          where t.clientid == pk
                          orderby t.orderid descending
                          select t).Take(20).ToList();

            TempData["SearchResultsMessage"] = "No Results Found";
            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Last 20 Orders - " + orderslist[0].clientname;
            }
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupOrderDate(FormCollection fc)
        {
            DateTime mydate = Convert.ToDateTime(fc["searchorderdate"]);
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          where t.orderdate.Value.Date == mydate.Date
                          orderby t.orderid descending
                          select t).ToList();

            TempData["SearchResultsMessage"] = "No Results Found";
            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For " + mydate.ToShortDateString();
            }
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupCompany(FormCollection fc)
        {
            var mycompany = fc["searchcompany"];
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          where t.company != null && t.company.ToLower().Contains(mycompany.ToLower())
                          select t).ToList();

            TempData["SearchResultsMessage"] = "No Results Found";
            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For Ship To " + mycompany;
            }
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupZipCode(FormCollection fc)
        {
            var myzipcode = fc["searchzipcode"];
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          where t.Zip != null && t.Zip.Contains(myzipcode)
                          select t).ToList();

            TempData["SearchResultsMessage"] = "No Results Found";
            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For Zip Code " + myzipcode;
            }
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupSalesRep(FormCollection fc)
        {
            var mysalesrep = fc["searchsalesrep"];
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          where t.salesrep != null && t.salesrep.Contains(mysalesrep)
                          select t).ToList();

            TempData["SearchResultsMessage"] = "No Results Found";
            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For sales Rep " + mysalesrep;
            }
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        #endregion Index Order Search and Filters
    }
}