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

        public ActionResult LookupOrderID(FormCollection form)
        {
            int orderId = Convert.ToInt32(form["orderid"]);

            if (orderId > 0)
            {
                var order = OrderService.FillOrder(orderId);

                if (order != null)
                {
                    return RedirectToAction("Edit", new { id = order.OrderID });
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult OpenOrdersClientAccounts(string filter, int page = 0)
        {
            string storeName = "openOrdersClientAcct";

            var orders = Session[storeName] as List<OrderMasterFull>;

            // If session store is empty go fetch new open orders
            if (String.IsNullOrWhiteSpace(filter))
            {
                orders = OrderService.GetClientAccountOpenOrders();
                Session[storeName] = orders;
            }

            const int PageSize = 20;
            int count = orders.Count();
            var data = orders.Skip(page * PageSize).Take(PageSize).ToList();
            int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;
            ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewBag.FilterKey = "OpenOrders";

            TempData["SearchResultsMessage"] = "Open Orders For Client Accounts";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult OpenOrdersAssignedSelf(string filter, int page = 0)
        {
            string storeName = "openOrdersAssignedSelf";
            string user = HttpContext.User.Identity.Name;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (String.IsNullOrWhiteSpace(filter))
            {
                orders = OrderService.GetAssignedOpenOrders(user);
                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            const int PageSize = 20;
            int count = orders.Count();
            data = orders.Skip(page * PageSize).Take(PageSize).ToList();
            int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;
            ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewBag.FilterKey = "OpenOrdersAssignedSelf";

            TempData["SearchResultsMessage"] = "Open Orders Assigned";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }
        
        public ActionResult OrdersToday(string filter, int page = 0)
        {
            DateTime today = DateTime.Today.AddDays(0);
            string storeName = "ordersToday";

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (String.IsNullOrWhiteSpace(filter))
            {
                orders = OrderService.GetClientAccountOpenOrders();

                orders = orders.Where(t => t.OrderDate.Value.Date == today.Date)
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            const int PageSize = 20;
            int count = orders.Count();
            data = orders.Skip(page * PageSize).Take(PageSize).ToList();
            int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;
            ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewBag.FilterKey = "Today";

            TempData["SearchResultsMessage"] = "Orders Created Today";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult OrdersYesterday(string filter, int page = 0)
        {
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            string storeName = "ordersYesterday";

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (String.IsNullOrWhiteSpace(filter))
            {
                orders = OrderService.GetClientAccountOpenOrders();

                orders = orders.Where(t => t.OrderDate.Value.Date == yesterday.Date)
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            const int PageSize = 20;
            int count = orders.Count();
            data = orders.Skip(page * PageSize).Take(PageSize).ToList();
            int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;
            ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewBag.FilterKey = "Yesterday";

            TempData["SearchResultsMessage"] = "Orders Created Yesterday";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult OrdersNeedAttention(string filter, int page = 0)
        {
            string storeName = "ordersNeedAttention";
            var orders = Session[storeName] as List<OrderMasterFull>;

            if (String.IsNullOrWhiteSpace(filter))
            {
                orders = OrderService.GetClientAccountOpenOrders();

                orders = orders.Where(t => t.NeedAllocationCount > 0)
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            const int PageSize = 20;
            int count = orders.Count();
            data = orders.Skip(page * PageSize).Take(PageSize).ToList();
            int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;
            ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewBag.FilterKey = "NeedAttention";

            TempData["SearchResultsMessage"] = "Orders Needing Attention";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }
        
        public ActionResult OrdersWithBackorderItem(string filter, int page = 0)
        {
            string storeName = "ordersWithBackorderItem";
            var orders = Session[storeName] as List<OrderMasterFull>;

            if (String.IsNullOrWhiteSpace(filter))
            {
                orders = OrderService.GetOrdersBackorderItems();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            const int PageSize = 20;
            int count = orders.Count();
            data = orders.Skip(page * PageSize).Take(PageSize).ToList();
            int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;
            ViewBag.MaxPage = maxpage;
            ViewBag.DisplayActivePage = page + 1;
            ViewBag.DisplayLastPage = maxpage + 1;
            ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewBag.FilterKey = "BackorderItem";

            TempData["SearchResultsMessage"] = "Orders With Backorder Item";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }
        
        public ActionResult AdvancedSearch()
        {
            return PartialView("~/Views/Orders/_AdvancedSearchModal.cshtml");
        }

        public ActionResult LookupClientID(FormCollection form, string filter, int page = 0)
        {
            int? clientId = Convert.ToInt32(form["searchclient"]);
            string storeName = "clientOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                clientId = Convert.ToInt32(filter);
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.ClientId == clientId)
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = clientId.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders For '" + orders[0].ClientName + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupOrderDate(FormCollection form, string filter, int page = 0)
        {
            DateTime orderDate = Convert.ToDateTime(form["searchorderdate"]);
            string storeName = "dateOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                orderDate = Convert.ToDateTime(filter);
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.OrderDate.Value.Date == orderDate.Date)
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = orderDate.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders On " + orders[0].OrderDate.Value.ToShortDateString();

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupCompany(FormCollection form, string filter, int page = 0)
        {
            string company = form["searchcompany"];
            string storeName = "companyOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                company = filter;
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.Company != null &&
                                           t.Company.ToLower().Contains(company.ToLower()))
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = company.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders With Ship To '" + company + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupAttention(FormCollection form, string filter, int page = 0)
        {
            string attention = form["searchattention"];
            string storeName = "attentionOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                attention = filter;
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.Attention != null &&
                                       t.Attention.ToLower().Contains(attention.ToLower()))
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = attention.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders With Attention Matching '" + attention + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupZipCode(FormCollection form, string filter, int page = 0)
        {
            string zipCode = form["searchzipcode"];
            string storeName = "zipcodeOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                zipCode = filter;
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.Zip != null &&
                                           t.Zip.Contains(zipCode))
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = zipCode.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders For Zip Code '" + zipCode + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupSalesRep(FormCollection form, string filter, int page = 0)
        {
            string salesRep = form["searchsalesrep"];
            string storeName = "salesrepOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                salesRep = filter;
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.SalesRepName != null &&
                                           t.SalesRepName.ToLower().Contains(salesRep.ToLower()))
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = salesRep.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders For Sales Rep '" + salesRep + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupWebOrderId(FormCollection form)
        {
            int? webOrderId = Convert.ToInt32(form["searchweborderid"]);
            var orders = OrderService.GetOrders();

            orders = orders.Where(t => t.WebOrderId != 0 &&
                                       t.WebOrderId == webOrderId)
                           .OrderByDescending(t => t.OrderID)
                           .ToList();

            TempData["SearchResultsMessage"] = "Orders With Web Order Id Matching '" + webOrderId + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orders);
        }

        public ActionResult LookupClientReference(FormCollection form, string filter, int page = 0)
        {
            string clientReference = form["searchclientreference"];
            string storeName = "clientRefOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                clientReference = filter;
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.ClientRefNumber != null &&
                                           t.ClientRefNumber.ToLower().Contains(clientReference.ToLower()))
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = clientReference.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders With Client Reference Matching '" + clientReference + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupClientOrderNumber(FormCollection form, string filter, int page = 0)
        {
            string clientOrderNumber = form["searchclientordernumber"];
            string storeName = "clientnumOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                clientOrderNumber = filter;
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.ClientOrderNumber != null &&
                                           t.ClientOrderNumber.ToLower().Contains(clientOrderNumber.ToLower()))
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = clientOrderNumber.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders With Client Order Number Matching '" + clientOrderNumber + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupTrackingNumber(FormCollection form, string filter, int page = 0)
        {
            string trackingNumber = form["searchtrackingnumber"];
            string storeName = "trackingOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                trackingNumber = filter;
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => t.Tracking != null &&
                                       t.Tracking.ToLower().Contains(trackingNumber.ToLower()))
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = trackingNumber.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders With Tracking Number Matching '" + trackingNumber + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        public ActionResult LookupInstructions(FormCollection form, string filter, int page = 0)
        {
            string specialInstructions = form["searchinstructions"];
            string storeName = "specialInstructionOrders";
            bool isStale = false;

            var orders = Session[storeName] as List<OrderMasterFull>;

            if (filter != null)
            {
                specialInstructions = filter;
            }
            else
            {
                isStale = true;
            }

            if (orders == null)
            {
                isStale = true;
            }

            if (isStale == true)
            {
                orders = OrderService.GetOrders();

                orders = orders.Where(t => (t.SpecialInternal != null &&
                                            t.SpecialInternal.ToLower().Contains(specialInstructions.ToLower())) ||
                                           (t.Special != null &&
                                            t.Special.ToLower().Contains(specialInstructions.ToLower())))
                               .OrderByDescending(t => t.OrderID)
                               .ToList();

                Session[storeName] = orders;
            }

            List<OrderMasterFull> data = null;

            if (orders.Count() > 0)
            {
                const int PageSize = 20;
                int count = orders.Count();
                data = orders.Skip(page * PageSize).Take(PageSize).ToList();
                int maxpage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

                ViewBag.Page = page;
                ViewBag.MaxPage = maxpage;
                ViewBag.DisplayActivePage = page + 1;
                ViewBag.DisplayLastPage = maxpage + 1;

                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["action"].ToString();
                ViewBag.FilterKey = specialInstructions.ToString();
            }

            TempData["SearchResultsMessage"] = "Orders With Special Instructions Matching '" + specialInstructions + "'";

            return PartialView("~/Views/Orders/_IndexPartial.cshtml", data);
        }

        [HttpGet]
        public ActionResult PullContactDetails(int id)
        {
            int clientContactId = id;
            var contact = ClientService.GetClientContact(clientContactId);

            return Json(contact, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssignOrderToSelf(bool check, int orderid)
        {
            string user = HttpContext.User.Identity.Name;

            if (check)
            {
                OrderService.AssignOrderOwner(orderid, user);
            }

            return new EmptyResult();
        }

        #endregion Search

        #region Order

        public ActionResult Index()
        {
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

            if (order == null)
            {
                return RedirectToAction("Index");
            }

            return View("~/Views/Orders/Edit.cshtml", order);
        }

        [HttpPost]
        public ActionResult Save(OrderMasterFull order)
        {
            order.CreateUser = HttpContext.User.Identity.Name;

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
            var orderItem = OrderService.GetOrderItem(orderItemId);

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
        public ActionResult OrdersImport(int? count, TimeSpan? time)
        {
            using (var db = new CMCSQL03Entities())
            {
                var failedimports = (from t in db.tblOrderImport
                                     where t.ImportStatus == "FAIL"
                                     && t.Location_MDB == "EU"
                                     && t.ImportError != null
                                     orderby t.OrderDate, t.GUID
                                     select t).ToList();

                ViewBag.OrdersImportedCount = count;
                ViewBag.RunTime = String.Format("{0:mm\\:ss\\:ff}", time);

                return View("~/Views/Orders/Import.cshtml", failedimports);
            }
        }

        [HttpGet]
        public ActionResult OrdersImportProcess()
        {
            int OrdersImportedCount;
            TimeSpan runTime;

            OrderService.ImportOrders(out OrdersImportedCount, out runTime);

            return RedirectToAction("OrdersImport", new { count = OrdersImportedCount, time = runTime });
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