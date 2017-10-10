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
        #region Search

        public ActionResult OpenOrders(int page = 0)
        {
            var orders = Session["openOrders"] as List<OrderMasterFull>;
            TempData["SearchResultsMessage"] = "Open Orders";

            // If session store is empty go fetch new open orders
            if (orders == null)
            {
                orders = OrderService.GetAssignedOpenOrders();
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

            orders = orders.Where(t => t.NeedAllocationCount > 0)
                           .OrderByDescending(t => t.OrderID)
                           .ToList();

            TempData["SearchResultsMessage"] = "Orders Needing Allocation";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpGet]
        public ActionResult OrdersToday()
        {
            DateTime today = DateTime.Today.AddDays(0);
            var orders = OrderService.GetOrders();

            orders = orders.Where(t => t.OrderDate.Value.Date == today.Date)
                           .OrderByDescending(t => t.OrderID)
                           .ToList();

            TempData["SearchResultsMessage"] = "Orders Created Today";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        [HttpGet]
        public ActionResult OrdersYesterday()
        {
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            var orders = OrderService.GetOrders();

            orders = orders.Where(t => t.OrderDate.Value.Date == yesterday.Date)
                           .OrderByDescending(t => t.OrderID)
                           .ToList();

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

            orders = orders.Where(t => t.CreateUser == User.Identity.Name ||
                                       t.UpdateUser == User.Identity.Name)
                           .OrderByDescending(t => t.OrderID)
                           .Take(20).ToList();

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

            orders = orders.Where(t => t.ClientId == clientId)
                           .OrderByDescending(t => t.OrderID)
                           .Take(100).ToList();

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

            orders = orders.Where(t => t.OrderDate.Value.Date == orderDate.Date)
                           .OrderByDescending(t => t.OrderID)
                           .ToList();

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

            orders = orders.Where(t => t.Company != null &&
                                       t.Company.ToLower().Contains(company.ToLower()))
                           .ToList();

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

            orders = orders.Where(t => t.Zip != null && 
                                       t.Zip.Contains(zipCode))
                           .ToList();

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

            orders = orders.Where(t => t.SalesRepName != null && 
                                       t.SalesRepName.ToLower().Contains(salesRep.ToLower()))
                           .ToList();

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

        #endregion Search

        #region Order

        public ActionResult Index()
        {
            // Preload open orders in session data store
            var openOrders = OrderService.GetAssignedOpenOrders();
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

        #endregion Order

        #region Order Item

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
            var orderItem = OrderService.GetOrderItems(orderItemId);

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

        #endregion Order Item

        #region Order Transaction

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
            var orderTransaction = OrderService.GetOrderTransaction(orderTransactionId);
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

        #endregion Order Transaction

        #region Allocate Order Item

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

        #endregion Allocate Order Item

        #region SPS Billing

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

        #endregion SPS Billing

        #region Order Import

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

        #endregion Order Import

        #region Print

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
            var orderItems = OrderService.OrderItems(orderid);

            if (orderItems.Count > 0)
            {
                orderItems = orderItems.OrderBy(x => x.ProductCode)
                                       .Where(x => x.ShipDate == null &&
                                                   x.AllocateStatus == "A" ||
                                                   x.RDTransfer == true).ToList();

                return PartialView("~/Views/Orders/_PrintPickOrderItems.cshtml", orderItems);
            }

            return null;
        }

        [HttpGet]
        public ActionResult PrintPackOrderItems(int orderid)
        {
            var orderItems = OrderService.OrderItems(orderid);

            if (orderItems.Count > 0)
            {
                orderItems = orderItems.OrderBy(x => x.ProductCode)
                                       .Where(x => x.ShipDate == null &&
                                                   x.AllocateStatus == "A" ||
                                                   x.RDTransfer == true).ToList();

                return PartialView("~/Views/Orders/_PrintPackOrderItems.cshtml", orderItems);
            }

            return null;
        }

        [HttpGet]
        public ActionResult PrintRemainingItems(int orderid)
        {
            var orderItems = OrderService.OrderItems(orderid);

            if (orderItems.Count > 0)
            {
                orderItems = orderItems.OrderBy(x => x.ProductCode)
                                       .Where(x => x.ShipDate != null ||
                                                   x.AllocateStatus != "A").ToList();

                return PartialView("~/Views/Orders/_PrintRemainingItems.cshtml", orderItems);
            }

            return null;
        }

        public ActionResult PrintPreferredCarrierMatrix(string Country)
        {
            var preferredCarrier = OrderService.GetPreferredCarrier(Country);

            if (preferredCarrier != null)
            {
                return PartialView("~/Views/Orders/_PrintPreferredCarrierMatrix.cshtml", preferredCarrier);
            }

            return PartialView("~/Views/Orders/_PrintPreferredCarrierMatrix.cshtml");
        }

        private static string DocumentFooter()
        {
            string footer = "--footer-left \"Printed on: " +
                DateTime.UtcNow.ToString("R") +
                "                                                                                                                                    " +
                " Page: [page]/[toPage]\"" +
                " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

            return footer;
        }

        #endregion Print
    }
}