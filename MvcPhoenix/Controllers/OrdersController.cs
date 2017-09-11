using MvcPhoenix.EF;
using MvcPhoenix.Models;
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
        #region Order Master Methods

        public ActionResult Index()
        {
            // Preload open orders in session data store
            var ordersopen = OrderService.OpenOrdersAssigned();
            Session["ordersopen"] = ordersopen;

            return View("~/Views/Orders/Index.cshtml");
        }

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            int ClientID = Convert.ToInt32(fc["NewClientID"]);
            var vm = OrderService.CreateOrder(ClientID);

            return View("~/Views/Orders/Edit.cshtml", vm);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var vm = OrderService.FillOrder(id);

            return View("~/Views/Orders/Edit.cshtml", vm);
        }

        [HttpPost]
        public ActionResult Save(OrderMasterFull vm)
        {
            int pk = OrderService.SaveOrder(vm);
            TempData["SaveResult"] = "Order Information updated on " + DateTime.UtcNow.ToString("R");

            return RedirectToAction("Edit", new { id = pk });
        }

        #endregion Order Master Methods

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
            var vm = OrderService.FillOrder(id);

            return new ViewAsPdf(vm) { CustomSwitches = footer };
        }

        [HttpGet]
        public ActionResult PrintPickOrderItems(int orderid)
        {
            var orderitems = PrintListOrderItems(orderid);

            if (orderitems.Count > 0)
            {
                return PartialView("~/Views/Orders/_PrintPickOrderItems.cshtml", orderitems);
            }

            return null;
        }

        [HttpGet]
        public ActionResult PrintPackOrderItems(int orderid)
        {
            var orderitems = PrintListOrderItems(orderid);

            if (orderitems.Count > 0)
            {
                return PartialView("~/Views/Orders/_PrintPackOrderItems.cshtml", orderitems);
            }

            return null;
        }

        public List<OrderItem> PrintListOrderItems(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderitems = (from t in db.tblOrderItem
                                  where t.OrderID == id
                                  && t.AllocateStatus == "A" | (t.RDTransfer != null && t.RDTransfer != false)
                                  && t.ShipDate == null
                                  orderby t.ProductCode
                                  select new OrderItem
                                  {
                                      OrderID = t.OrderID,
                                      ItemID = t.ItemID,
                                      ProductDetailID = t.ProductDetailID,
                                      ProductCode = t.ProductCode,
                                      ProductName = t.ProductName,
                                      Qty = t.Qty,
                                      Size = t.Size,
                                      ShipDate = t.ShipDate,
                                      CeaseShipDate = t.CeaseShipDate,
                                      LotNumber = t.LotNumber,
                                      Bin = t.Bin,
                                      AirUnNumber = t.AirUnNumber,
                                      GrnUnNumber = t.GrnUnNumber,
                                      SeaUnNumber = t.GrnUnNumber,                         //Note: Add SeaUnNumber field to tblOrderItem
                                      BackOrdered = t.BackOrdered,
                                      AllocateStatus = t.AllocateStatus,
                                      NonCMCDelay = t.NonCMCDelay,
                                      RDTransfer = t.RDTransfer,
                                      DelayReason = t.DelayReason,
                                      FreezableList = (from q in db.tblProductMaster
                                                       where (q.MasterCode == t.ProductCode)
                                                       select q.FreezableList).FirstOrDefault(),
                                      HarmonizedCode = (from q in db.tblProductDetail
                                                        where (q.ProductDetailID == t.ProductDetailID)
                                                        select q.HarmonizedCode).FirstOrDefault(),
                                      QtyAvailable = (from q in db.tblStock
                                                      where q.ShelfID == t.ShelfID
                                                      && (q.QtyOnHand - q.QtyAllocated >= t.Qty)
                                                      && q.ShelfStatus == "AVAIL"
                                                      select q).Count(),
                                      AlertNotesOrderEntry = (from q in db.tblProductDetail
                                                              where q.ProductDetailID == t.ProductDetailID
                                                              select q.AlertNotesOrderEntry).FirstOrDefault(),
                                      AlertNotesShipping = (from q in db.tblProductDetail
                                                            where q.ProductDetailID == t.ProductDetailID
                                                            select q.AlertNotesShipping).FirstOrDefault()
                                  }).ToList();

                return orderitems;
            }
        }

        [HttpGet]
        public ActionResult PrintRemainingItems(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderitems = (from t in db.tblOrderItem
                                  where t.OrderID == orderid
                                  && (t.ShipDate != null || t.AllocateStatus != "A")
                                  orderby t.ProductCode
                                  select new OrderItem
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

                if (orderitems.Count > 0)
                {
                    return PartialView("~/Views/Orders/_PrintRemainingItems.cshtml", orderitems);
                }

                return null;
            }
        }

        public ActionResult PrintPreferredCarrierMatrix(string Country)
        {
            using (var db = new CMCSQL03Entities())
            {
                PreferredCarrierViewModel preferredcarrier = new PreferredCarrierViewModel();
                var result = (from t in db.tblPreferredCarrierList
                              where t.CountryName.Contains(Country)
                              select t).FirstOrDefault();

                preferredcarrier.CountryCode = result.CountryCode;
                preferredcarrier.CountryName = result.CountryName;
                preferredcarrier.CommInvoiceReq = result.CommInvoiceReq;
                preferredcarrier.NonHazSm = result.NonHaz_Sm;
                preferredcarrier.NonHazLg = result.NonHaz_Lg;
                preferredcarrier.NonHazIncoTerms = result.NonHaz_IncoTerms;
                preferredcarrier.HazIATASm = result.HazIATA_Sm;
                preferredcarrier.HazIATALg = result.HazIATA_Lg;
                preferredcarrier.HazGroundLQ = result.HazGround_LQ;
                preferredcarrier.HazGround = result.HazGround;
                preferredcarrier.HazIncoterms = result.Haz_Incoterms;
                preferredcarrier.IncotermsAlt = result.Incoterms_Alt;
                preferredcarrier.NotesGeneral = result.Notes_General;
                preferredcarrier.NotesIATAADR = result.Notes_IATA_ADR;
                preferredcarrier.NonHazIncotermsAlt = result.NonHazIncoterms_Alt;
                preferredcarrier.HazIncotermsAlt = result.HazIncoterms_Alt;

                if (result == null)
                {
                    return null;
                }

                return PartialView("~/Views/Orders/_PrintPreferredCarrierMatrix.cshtml", preferredcarrier);
            }
        }

        #endregion Printing Actions

        #region Order SPS Billing Details

        public ActionResult EditSPSBilling(int id)
        {
            int orderid = id;
            var vm = OrderService.SPSBilling(orderid);

            return PartialView("~/Views/Orders/_SPSBillingModal.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SaveSPSBilling(OrderSPSBilling vm)
        {
            OrderService.SaveSPSBillingDetails(vm);

            return null;
        }

        #endregion Order SPS Billing Details

        #region Order Item Methods

        [HttpGet]
        public ActionResult fnOrderItemsList(int orderid)
        {
            var orderitems = OrderService.OrderItems(orderid);

            if (orderitems.Count > 0)
            {
                return PartialView("~/Views/Orders/_OrderItems.cshtml", orderitems);
            }

            return null;
        }

        [HttpGet]
        public ActionResult CreateItem(int id)
        {
            var vm = OrderService.CreateOrderItem(id);                             // id=orderid

            return PartialView("~/Views/Orders/_OrderItemModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult EditItem(int id)
        {
            OrderItem vm = new OrderItem();
            vm = OrderService.FillOrderItemDetails(id);

            return PartialView("~/Views/Orders/_OrderItemModal.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SaveItem(OrderItem incoming, string productdetailid)
        {
            if (productdetailid != "0" && productdetailid != "")
            {
                if (incoming.ShelfID != null)
                {
                    int pk = OrderService.SaveOrderItem(incoming);

                    return null;
                }
            }

            return null;
        }

        public ActionResult DeleteItem(int id)
        {
            OrderService.DeleteOrderItem(id);

            return Content("Item Deleted");
        }

        public ActionResult BuildSizeDropDown(int id)
        {
            return Content(ApplicationService.ddlBuildSizeDropDown(id));                 // id=clientid / return a <select> for <div>
        }

        public ActionResult CheckProductForAlert(int? id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productorderalert = (from t in db.tblProductDetail
                                         where t.ProductDetailID == id
                                         select t.AlertNotesOrderEntry).FirstOrDefault();

                return Json(productorderalert, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Order Item Methods

        #region Order Import Actions

        [HttpGet]
        public ActionResult OrdersImport()
        {
            using (var db = new CMCSQL03Entities())
            {
                var failedimports = (from t in db.tblOrderImport
                                     where t.ImportStatus == "FAIL"
                                     && t.Location_MDB == "EU"
                                     && t.ImportError != null
                                     orderby t.OrderDate, t.GUID
                                     select t).ToList();

                return View("~/Views/Orders/Import.cshtml", failedimports);
            }
        }

        [HttpGet]
        public ActionResult OrdersImportProcess()
        {
            int ImportCount = OrderService.ImportOrders();        // Actual import
            ViewBag.ImportCount = ImportCount;

            return RedirectToAction("OrdersImport");
        }

        #endregion Order Import Actions

        [HttpGet]
        public ActionResult PullContactDetails(int id)
        {
            Contact obj = new Contact();
            obj = OrderService.GetClientContacts(id);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #region Allocate Methods

        public ActionResult AllocateFromShelf(int id, bool IncludeQCStock)
        {
            int AllocationCount = OrderService.AllocateShelf(id, IncludeQCStock);

            return Content(AllocationCount.ToString() + " item(s) allocated");
        }

        public ActionResult ReverseAllocatedItem(int orderitemid)
        {
            OrderService.ReverseAllocatedItem(orderitemid);

            return Content("Item allocation reversed");
        }

        #endregion Allocate Methods

        #region Order Transaction Methods

        [HttpGet]
        public ActionResult fnOrderTransList(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderitemtransactions = (from t in db.tblOrderTrans
                                             join items in db.tblOrderItem on t.OrderItemID equals items.ItemID into a
                                             from p in a.DefaultIfEmpty()
                                             where t.OrderID == orderid
                                             orderby t.OrderItemID
                                             select new OrderTrans
                                             {
                                                 OrderTransID = t.OrderTransID,
                                                 OrderId = t.OrderID,
                                                 OrderItemId = t.OrderItemID,
                                                 ProductCode = p.ProductCode,
                                                 ClientId = t.ClientID,
                                                 TransDate = t.TransDate,
                                                 TransType = t.TransType,
                                                 TransQty = t.TransQty,
                                                 TransRate = t.TransRate,
                                                 TransAmount = t.TransAmount,
                                                 Comments = t.Comments,
                                                 CreateDate = t.CreateDate,
                                                 CreateUser = t.CreateUser,
                                                 UpdateDate = t.UpdateDate,
                                                 UpdateUser = t.UpdateUser
                                             }).ToList();

                return PartialView("~/Views/Orders/_OrderTrans.cshtml", orderitemtransactions);
            }
        }

        [HttpGet]
        public ActionResult CreateTrans(int id)
        {
            var vm = OrderService.CreateOrderTransaction(id);
            ViewBag.ListOrderItemIDs = ApplicationService.ddlOrderItemIDs(vm.OrderId);

            return PartialView("~/Views/Orders/_OrderTransModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult EditTrans(int id)
        {
            var vm = OrderService.FillOrderTransaction(id);
            ViewBag.ListOrderItemIDs = ApplicationService.ddlOrderItemIDs(vm.OrderId);

            return PartialView("~/Views/Orders/_OrderTransModal.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SaveTrans(OrderTrans vm)
        {
            int pk = OrderService.SaveOrderTransaction(vm);

            return Content("Transaction Saved on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult DeleteTrans(int id)
        {
            OrderService.DeleteTransaction(id);

            return Content("Transaction Deleted on " + DateTime.UtcNow.ToString("R"));
        }

        #endregion Order Transaction Methods

        #region Index Order Search and Filters

        public ActionResult OpenOrders(int page = 0)
        {
            var orderslist = Session["ordersopen"] as List<OrderMasterFull>;
            TempData["SearchResultsMessage"] = "Open Orders";

            // If session store is empty go fetch new open orders
            if (orderslist == null)
            {
                orderslist = OrderService.OpenOrdersAssigned();
            }
            
            const int PageSize = 20;
            int count = orderslist.Count();
            var data = orderslist.Skip(page * PageSize).Take(PageSize).ToList();
            int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;
            
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        [HttpPost]
        public ActionResult LookupOrderID(FormCollection fc)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (String.IsNullOrEmpty(fc["orderid"]))
                {
                    return RedirectToAction("Index");
                }

                int inputorderid = Convert.ToInt32(fc["orderid"]);
                var result = (from t in db.tblOrderMaster
                              where t.OrderID == inputorderid
                              select t).FirstOrDefault();

                if (result != null)
                {
                    return RedirectToAction("Edit", new { id = inputorderid });
                }

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult OrdersNeedAllocation()
        {
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "Orders Needing Allocation";

            orderslist = (from t in orderslist
                          where t.NeedAllocationCount > 0
                          orderby t.OrderID ascending
                          select t).ToList();
            
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersToday()
        {
            DateTime dateToday = DateTime.Today.AddDays(0);
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "Orders Created Today";

            orderslist = (from t in orderslist
                          orderby t.OrderID descending
                          where t.OrderDate.Value.Date == dateToday.Date
                          select t).ToList();
            
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersYesterday()
        {
            DateTime dateToday = DateTime.Today.AddDays(-1);
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "Orders Created Yesterday";

            orderslist = (from t in orderslist
                          orderby t.OrderID descending
                          where t.OrderDate.Value.Date == dateToday.Date
                          select t).ToList();
            
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersLastTen()
        {
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "Last 10 Orders";

            orderslist = (from t in orderslist
                          orderby t.OrderID descending
                          select t).Take(10).ToList();
            
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersMineLastTen()
        {
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "My Last 10 Orders";

            orderslist = (from t in orderslist
                          where t.CreateUser == User.Identity.Name
                          orderby t.OrderID descending
                          select t).Take(10).ToList();
            
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
            int inputclientid = Convert.ToInt32(fc["ClientID"]);
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "No Results Found";

            orderslist = (from t in orderslist
                          where t.ClientId == inputclientid
                          orderby t.OrderID descending
                          select t).Take(100).ToList();

            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Last 100 Orders - " + orderslist[0].ClientName;
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupOrderDate(FormCollection fc)
        {
            DateTime inputdate = Convert.ToDateTime(fc["searchorderdate"]);
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "No Results Found";

            orderslist = (from t in orderslist
                          where t.OrderDate.Value.Date == inputdate.Date
                          orderby t.OrderID descending
                          select t).ToList();

            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For " + inputdate.ToShortDateString();
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupCompany(FormCollection fc)
        {
            var inputcompany = fc["searchcompany"];
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "No Results Found";

            orderslist = (from t in orderslist
                          where t.Company != null
                          && t.Company.ToLower().Contains(inputcompany.ToLower())
                          select t).ToList();

            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For Ship To " + inputcompany;
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupZipCode(FormCollection fc)
        {
            var inputzipcode = fc["searchzipcode"];
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "No Results Found";

            orderslist = (from t in orderslist
                          where t.Zip != null
                          && t.Zip.Contains(inputzipcode)
                          select t).ToList();

            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For Zip Code " + inputzipcode;
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupSalesRep(FormCollection fc)
        {
            var inputsalesrep = fc["searchsalesrep"];
            var orderslist = OrderService.SearchOrders();
            TempData["SearchResultsMessage"] = "No Results Found";

            orderslist = (from t in orderslist
                          where t.SalesRepName != null
                          && t.SalesRepName.ToLower().Contains(inputsalesrep.ToLower())
                          select t).ToList();

            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For sales Rep " + inputsalesrep;
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        #endregion Index Order Search and Filters
    }
}