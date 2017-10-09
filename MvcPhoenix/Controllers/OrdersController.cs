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
            var openOrders = OrderService.OpenOrdersAssigned();
            Session["openOrders"] = openOrders;

            return View("~/Views/Orders/Index.cshtml");
        }

        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            int clientId = Convert.ToInt32(form["NewClientID"]);
            if (clientId < 1)
            {
                return RedirectToAction("Index");
            }
            var order = OrderService.CreateOrder(clientId);

            return View("~/Views/Orders/Edit.cshtml", order);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            int orderId = id;
            var order = OrderService.FillOrder(orderId);

            return View("~/Views/Orders/Edit.cshtml", order);
        }

        [HttpPost]
        public ActionResult Save(OrderMasterFull order)
        {
            int orderId = OrderService.SaveOrder(order);
            TempData["SaveResult"] = "Order updated on " + DateTime.UtcNow.ToString("R");

            return RedirectToAction("Edit", new { id = orderId });
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
            int orderId = id;
            var order = OrderService.FillOrder(orderId);
            var footer = DocumentFooter();

            return new ViewAsPdf(order) { CustomSwitches = footer };
        }

        [HttpGet]
        public ActionResult PrintPickOrderItems(int orderid)
        {
            var orderItems = PrintListOrderItems(orderid);

            if (orderItems.Count > 0)
            {
                return PartialView("~/Views/Orders/_PrintPickOrderItems.cshtml", orderItems);
            }

            return null;
        }

        [HttpGet]
        public ActionResult PrintPackOrderItems(int orderid)
        {
            var orderItems = PrintListOrderItems(orderid);

            if (orderItems.Count > 0)
            {
                return PartialView("~/Views/Orders/_PrintPackOrderItems.cshtml", orderItems);
            }

            return null;
        }

        public List<OrderItem> PrintListOrderItems(int id)
        {
            int orderId = id;

            using (var db = new CMCSQL03Entities())
            {
                var orderItems = (from t in db.tblOrderItem
                                  where t.OrderID == orderId
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
                                      SeaUnNumber = t.GrnUnNumber,
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

                return orderItems;
            }
        }

        [HttpGet]
        public ActionResult PrintRemainingItems(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderItems = (from t in db.tblOrderItem
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

                if (orderItems.Count > 0)
                {
                    return PartialView("~/Views/Orders/_PrintRemainingItems.cshtml", orderItems);
                }

                return null;
            }
        }

        public ActionResult PrintPreferredCarrierMatrix(string Country)
        {
            using (var db = new CMCSQL03Entities())
            {
                var preferredCarrier = new PreferredCarrierViewModel();
                var suggestedCarrier = db.tblPreferredCarrierList
                                         .Where(x => x.CountryName.Contains(Country))
                                         .FirstOrDefault();

                preferredCarrier.CountryCode = suggestedCarrier.CountryCode;
                preferredCarrier.CountryName = suggestedCarrier.CountryName;
                preferredCarrier.CommInvoiceReq = suggestedCarrier.CommInvoiceReq;
                preferredCarrier.NonHazSm = suggestedCarrier.NonHaz_Sm;
                preferredCarrier.NonHazLg = suggestedCarrier.NonHaz_Lg;
                preferredCarrier.NonHazIncoTerms = suggestedCarrier.NonHaz_IncoTerms;
                preferredCarrier.HazIATASm = suggestedCarrier.HazIATA_Sm;
                preferredCarrier.HazIATALg = suggestedCarrier.HazIATA_Lg;
                preferredCarrier.HazGroundLQ = suggestedCarrier.HazGround_LQ;
                preferredCarrier.HazGround = suggestedCarrier.HazGround;
                preferredCarrier.HazIncoterms = suggestedCarrier.Haz_Incoterms;
                preferredCarrier.IncotermsAlt = suggestedCarrier.Incoterms_Alt;
                preferredCarrier.NotesGeneral = suggestedCarrier.Notes_General;
                preferredCarrier.NotesIATAADR = suggestedCarrier.Notes_IATA_ADR;
                preferredCarrier.NonHazIncotermsAlt = suggestedCarrier.NonHazIncoterms_Alt;
                preferredCarrier.HazIncotermsAlt = suggestedCarrier.HazIncoterms_Alt;

                if (suggestedCarrier == null)
                {
                    return null;
                }

                return PartialView("~/Views/Orders/_PrintPreferredCarrierMatrix.cshtml", preferredCarrier);
            }
        }

        #endregion Printing Actions

        #region Order SPS Billing Details

        public ActionResult EditSPSBilling(int id)
        {
            int orderId = id;
            var orderSPSBilling = OrderService.SPSBilling(orderId);

            return PartialView("~/Views/Orders/_SPSBillingModal.cshtml", orderSPSBilling);
        }

        [HttpPost]
        public ActionResult SaveSPSBilling(OrderSPSBilling orderSPSBilling)
        {
            OrderService.SaveSPSBillingDetails(orderSPSBilling);

            return null;
        }

        #endregion Order SPS Billing Details

        #region Order Item Methods

        [HttpGet]
        public ActionResult OrderItemsList(int orderid)
        {
            var orderItems = OrderService.OrderItems(orderid);

            if (orderItems.Count > 0)
            {
                return PartialView("~/Views/Orders/_OrderItems.cshtml", orderItems);
            }

            return null;
        }

        [HttpGet]
        public ActionResult CreateItem(int id)
        {
            int orderItemId = id;
            var orderItem = OrderService.CreateOrderItem(orderItemId);

            return PartialView("~/Views/Orders/_OrderItemModal.cshtml", orderItem);
        }

        [HttpGet]
        public ActionResult EditItem(int id)
        {
            int orderItemId = id;
            var orderItem = OrderService.GetOrderItemDetails(orderItemId);

            return PartialView("~/Views/Orders/_OrderItemModal.cshtml", orderItem);
        }

        [HttpPost]
        public ActionResult SaveItem(OrderItem orderItem, int productdetailid)
        {
            int productDetailId = productdetailid;
            if (productDetailId != 0)
            {
                if (orderItem.ShelfID != null)
                {
                    int orderItemId = OrderService.SaveOrderItem(orderItem);

                    return null;
                }
            }

            return null;
        }

        public ActionResult DeleteItem(int id)
        {
            int orderItemId = id;
            OrderService.DeleteOrderItem(orderItemId);

            return Content("Item Deleted");
        }

        public ActionResult BuildSizeDropDown(int productdetailid)
        {
            var productShelfSizes = ApplicationService.ddlBuildSize(productdetailid);

            return Content(productShelfSizes);
        }

        public ActionResult CheckProductForAlert(int? id)
        {
            int? productDetailId = id;
            using (var db = new CMCSQL03Entities())
            {
                var productAlert = db.tblProductDetail
                                     .Where(t => t.ProductDetailID == productDetailId)
                                     .Select(t => t.AlertNotesOrderEntry)
                                     .FirstOrDefault();

                return Json(productAlert, JsonRequestBehavior.AllowGet);
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

        #region Allocate Methods

        public ActionResult AllocateFromShelf(int id, bool IncludeQCStock)
        {
            int orderId = id;
            int allocationCount = OrderService.AllocateShelf(orderId, IncludeQCStock);

            return Content(allocationCount.ToString() + " item(s) allocated");
        }

        public ActionResult ReverseAllocatedItem(int orderitemid)
        {
            OrderService.ReverseAllocatedItem(orderitemid);

            return Content("Item allocation reversed");
        }

        #endregion Allocate Methods

        #region Order Transaction Methods

        [HttpGet]
        public ActionResult OrderTransList(int orderid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var orderItemTransactions = (from t in db.tblOrderTrans
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

                return PartialView("~/Views/Orders/_OrderTrans.cshtml", orderItemTransactions);
            }
        }

        [HttpGet]
        public ActionResult CreateTrans(int id)
        {
            int orderTransactionId = id;
            var orderTransaction = OrderService.CreateOrderTransaction(orderTransactionId);
            ViewBag.ListOrderItemIDs = ApplicationService.ddlOrderItemIDs(Convert.ToInt32(orderTransaction.OrderId));

            return PartialView("~/Views/Orders/_OrderTransModal.cshtml", orderTransaction);
        }

        [HttpGet]
        public ActionResult EditTrans(int id)
        {
            int orderTransactionId = id;
            var orderTransaction = OrderService.FillOrderTransaction(orderTransactionId);
            ViewBag.ListOrderItemIDs = ApplicationService.ddlOrderItemIDs(Convert.ToInt32(orderTransaction.OrderId));

            return PartialView("~/Views/Orders/_OrderTransModal.cshtml", orderTransaction);
        }

        [HttpPost]
        public ActionResult SaveTrans(OrderTrans orderTransaction)
        {
            int orderTransactionId = OrderService.SaveOrderTransaction(orderTransaction);

            return Content("Transaction Saved on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult DeleteTrans(int id)
        {
            int orderTransactionId = id;
            OrderService.DeleteTransaction(orderTransactionId);

            return Content("Transaction Deleted on " + DateTime.UtcNow.ToString("R"));
        }

        #endregion Order Transaction Methods

        #region Index Order Search and Filters

        public ActionResult OpenOrders(int page = 0)
        {
            var orders = Session["openOrders"] as List<OrderMasterFull>;
            TempData["SearchResultsMessage"] = "Open Orders";

            // If session store is empty go fetch new open orders
            if (orders == null)
            {
                orders = OrderService.OpenOrdersAssigned();
            }

            const int PageSize = 20;
            int count = orders.Count();
            var data = orders.Skip(page * PageSize).Take(PageSize).ToList();
            int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        [HttpPost]
        public ActionResult LookupOrderID(FormCollection form)
        {
            int orderId = Convert.ToInt32(form["orderid"]);

            if (orderId == 0)
            {
                return RedirectToAction("Index");
            }

            using (var db = new CMCSQL03Entities())
            {
                var order = db.tblOrderMaster.Find(orderId);

                if (order != null)
                {
                    return RedirectToAction("Edit", new { id = orderId });
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult OrdersNeedAllocation()
        {
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      where t.NeedAllocationCount > 0
                      orderby t.OrderID descending
                      select t).ToList();

            TempData["SearchResultsMessage"] = "Orders Needing Allocation";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpGet]
        public ActionResult OrdersToday()
        {
            DateTime today = DateTime.Today.AddDays(0);
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      orderby t.OrderID descending
                      where t.OrderDate.Value.Date == today.Date
                      select t).ToList();

            TempData["SearchResultsMessage"] = "Orders Created Today";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpGet]
        public ActionResult OrdersYesterday()
        {
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      orderby t.OrderID descending
                      where t.OrderDate.Value.Date == yesterday.Date
                      select t).ToList();

            TempData["SearchResultsMessage"] = "Orders Created Yesterday";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpGet]
        public ActionResult OrdersRecentAll()
        {
            var orders = OrderService.GetOrders();

            orders = orders.OrderByDescending(x => x.OrderID).Take(100).ToList();

            TempData["SearchResultsMessage"] = "All Recent Orders";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpGet]
        public ActionResult OrdersRecentUser()
        {
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      where t.CreateUser == User.Identity.Name
                      || t.UpdateUser == User.Identity.Name
                      orderby t.OrderID descending
                      select t).Take(20).ToList();

            TempData["SearchResultsMessage"] = "My Recent Orders";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpGet]
        public ActionResult AdvancedSearch()
        {
            return PartialView("~/Views/Orders/_AdvancedSearchModal.cshtml");
        }

        [HttpPost]
        public ActionResult LookupClientID(FormCollection form)
        {
            int clientId = Convert.ToInt32(form["ClientID"]);
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      where t.ClientId == clientId
                      orderby t.OrderID descending
                      select t).Take(100).ToList();

            if (orders.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Last 100 Orders - " + orders[0].ClientName;
            }
            else
            {
                TempData["SearchResultsMessage"] = "No Results Found";
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpPost]
        public ActionResult LookupOrderDate(FormCollection form)
        {
            DateTime orderDate = Convert.ToDateTime(form["searchorderdate"]);
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      where t.OrderDate.Value.Date == orderDate.Date
                      orderby t.OrderID descending
                      select t).ToList();

            if (orders.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For " + orderDate.ToShortDateString();
            }
            else
            {
                TempData["SearchResultsMessage"] = "No Results Found";
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpPost]
        public ActionResult LookupCompany(FormCollection form)
        {
            var company = form["searchcompany"];
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      where t.Company != null
                      && t.Company.ToLower().Contains(company.ToLower())
                      select t).ToList();

            if (orders.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For Ship To " + company;
            }
            else
            {
                TempData["SearchResultsMessage"] = "No Results Found";
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpPost]
        public ActionResult LookupZipCode(FormCollection form)
        {
            var zipCode = form["searchzipcode"];
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      where t.Zip != null
                      && t.Zip.Contains(zipCode)
                      select t).ToList();

            if (orders.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For Zip Code " + zipCode;
            }
            else
            {
                TempData["SearchResultsMessage"] = "No Results Found";
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpPost]
        public ActionResult LookupSalesRep(FormCollection form)
        {
            var salesRep = form["searchsalesrep"];
            var orders = OrderService.GetOrders();

            orders = (from t in orders
                      where t.SalesRepName != null
                      && t.SalesRepName.ToLower().Contains(salesRep.ToLower())
                      select t).ToList();

            if (orders.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For sales Rep " + salesRep;
            }
            else
            {
                TempData["SearchResultsMessage"] = "No Results Found";
            }

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpGet]
        public ActionResult PullContactDetails(int id)
        {
            int clientContactId = id;
            var contact = ClientService.GetClientContact(clientContactId);

            return Json(contact, JsonRequestBehavior.AllowGet);
        }

        #endregion Index Order Search and Filters
    }
}