using MvcPhoenix.Models;
using MvcPhoenix.Services;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class OrdersController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public ActionResult Index()
        {
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist orderby t.orderdate descending select t).Take(10).ToList();  // refine for here
            TempData["SearchResultsMessage"] = "Last 10 Orders";
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

        public ActionResult PrintOrder(int id)
        {
            string footer = "--footer-left \"Printed on: " +
                DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") +
                "                                                                                                                                    " +
                " Page: [page]/[toPage]\"" +
                " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

            var vm = OrderService.fnFillOrder(id);
            return new ViewAsPdf(vm) { CustomSwitches = footer };
            //return View(vm);
        }

        [HttpPost]
        public ActionResult Save(OrderMasterFull vm)
        {
            int pk = OrderService.fnSaveOrder(vm);
            TempData["SaveResult"] = "Order Information updated at " + DateTime.Now;
            return RedirectToAction("Edit", new { id = pk });
        }

        [HttpGet]
        public ActionResult fnOrderItemsList(int id)
        {
            // list of items for a given order
            using (db)
            {
                var qry = (from t in db.tblOrderItem
                           where t.OrderID == id
                           orderby t.ProductCode
                           select new MvcPhoenix.Models.OrderItem
                           {
                               OrderID = t.OrderID,
                               ItemID = t.ItemID,
                               ProductDetailID = t.ProductDetailID,
                               ProductCode = t.ProductCode,
                               Mnemonic = t.Mnemonic,
                               ProductName = t.ProductName,
                               Size = t.Size,
                               LotNumber = t.LotNumber,
                               Qty = t.Qty,
                               ShipDate = t.ShipDate,
                               BackOrdered = t.BackOrdered,
                               AllocateStatus = t.AllocateStatus,
                               NonCMCDelay = t.NonCMCDelay,
                               QtyAvailable = (from q in db.tblStock where q.ShelfID == t.ShelfID && (q.QtyOnHand - q.QtyAllocated >= t.Qty) && q.ShelfStatus == "AVAIL" select q).Count()
                           }).ToList();
                if (qry.Count > 0)
                { return PartialView("~/Views/Orders/_OrderItems.cshtml", qry); }
                return null;
            }
        }

        [HttpGet]
        public ActionResult CreateItem(int id)
        {
            // id=orderid
            var vm = OrderService.fnCreateItem(id);
            return PartialView("~/Views/Orders/_OrderItemModal.cshtml", vm);
        }

        public ActionResult BuildSizeDropDown(int id)
        {
            // id=clientid / return a <select> for <div>
            return Content(OrderService.fnBuildSizeDropDown(id));
        }

        [HttpGet]
        public ActionResult EditItem(int id)
        {
            OrderItem vm = new OrderItem();
            vm = OrderService.fnFillOrderItem(id);
            return PartialView("~/Views/Orders/_OrderItemModal.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SaveItem(OrderItem incoming)
        {
            int pk = OrderService.fnSaveItem(incoming);
            return Content("Item Saved at " + System.DateTime.Now.ToString());
        }

        public ActionResult DeleteItem(int id)
        {
            OrderService.fnDeleteOrderItem(id);
            return Content("Item Deleted");
        }

        public ActionResult AllocateFromShelf(int id, bool IncludeExpiredStock)
        {
            // The view handles the partial updates
            int AllocationCount = OrderService.fnAllocateShelf(id, IncludeExpiredStock);
            return Content(AllocationCount.ToString() + " item(s) allocated");
        }

        public ActionResult AllocateFromBulk(int id, bool IncludeExpiredStock)
        {
            int AllocationCount = OrderService.fnAllocateBulk(id, IncludeExpiredStock);
            return Content(AllocationCount.ToString() + " item(s) allocated");
        }

        [HttpGet]
        public ActionResult fnOrderTransList(int id)
        {
            // id=orderid
            var qry = (from t in db.tblOrderTrans
                       join items in db.tblOrderItem on t.OrderItemID equals items.ItemID into a
                       from qry2 in a.DefaultIfEmpty()
                       where t.OrderID == id
                       orderby t.OrderItemID, t.TransType
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
                       }).ToList();
            return PartialView("~/Views/Orders/_OrderTrans.cshtml", qry);
        }

        [HttpGet]
        public ActionResult CreateTrans(int id)
        {
            var vm = OrderService.fnCreateTrans(id);
            return PartialView("~/Views/Orders/_OrderTransModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult EditTrans(int id)
        {
            var vm = OrderService.fnFillTrans(id);
            return PartialView("~/Views/Orders/_OrderTransModal.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SaveTrans(OrderTrans vm)
        {
            int pk = OrderService.fnSaveTrans(vm);
            return Content("Trans Saved at " + System.DateTime.Now.ToString());
        }

        [HttpGet]
        public ActionResult DeleteTrans(int id)
        {
            OrderService.fnDeleteTrans(id);
            return Content("Trans Deleted at " + System.DateTime.Now.ToString());
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
            DateTime datToday = DateTime.Today.AddDays(-1);
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist
                          orderby t.orderid descending
                          where t.orderdate.Value.Date == datToday.Date
                          select t).ToList();
            TempData["SearchResultsMessage"] = "Orders Created Yesterday";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpGet]
        public ActionResult OrdersLastTen()
        {
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist orderby t.orderid descending select t).Take(10).ToList();
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

        [HttpPost]
        public ActionResult LookupOrderID(FormCollection fc)
        {
            if (String.IsNullOrEmpty(fc["orderid"]))
            {
                return RedirectToAction("Index");
            }
            int id = Convert.ToInt32(fc["orderid"]);
            var qry = (from t in db.tblOrderMaster where t.OrderID == id select t).FirstOrDefault();
            if (qry != null)
            {
                return RedirectToAction("Edit", new { id = id });
            }
            return RedirectToAction("Index");
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
            orderslist = (from t in orderslist where t.clientid == pk orderby t.orderid descending select t).Take(20).ToList();
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
            orderslist = (from t in orderslist where t.orderdate.Value.Date == mydate.Date orderby t.orderid descending select t).ToList();
            TempData["SearchResultsMessage"] = "No Results Found";
            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Order For " + mydate.ToShortDateString();
            }
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        [HttpPost]
        public ActionResult LookupCompany(FormCollection fc)
        {
            var mycompany = fc["searchcompany"];
            //var orderslist = fnOrdersSearchResults();
            var orderslist = OrderService.fnOrdersSearchResults();
            orderslist = (from t in orderslist where t.company != null && t.company.Contains(mycompany) select t).ToList();
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
            orderslist = (from t in orderslist where t.Zip != null && t.Zip.Contains(myzipcode) select t).ToList();
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
            orderslist = (from t in orderslist where t.salesrep != null && t.salesrep.Contains(mysalesrep) select t).ToList();
            TempData["SearchResultsMessage"] = "No Results Found";
            if (orderslist.Count() > 0)
            {
                TempData["SearchResultsMessage"] = "Orders For sales Rep " + mysalesrep;
            }
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", orderslist);
        }

        //public ActionResult PrintOrder()
        //{
        //    // The data view will need to more complicated
        //    OrderMasterFull obj = new OrderMasterFull();
        //    obj = OrderService.fnFillOrder(Convert.ToInt32(Session["OrderID"]));
        //    return View("PrintOrder", obj);
        //}

        //public ActionResult Notification()
        //{
        //    // The data view will need to more complicated
        //    OrderMasterFull obj = new OrderMasterFull();
        //    obj = OrderService.fnFillOrder(Convert.ToInt32(Session["OrderID"]));
        //    return View("Notification", obj);
        //}

        public ActionResult Import()
        {
            return View();
        }

        // Order Import Action

        #region Order Import Action

        [HttpGet]
        public ActionResult OrderImport()
        {
            var clients = GetClients(); // Let's get all clients that we need for a DropDownList
            var model = new OrderImportFile();
            model.Clients = GetSelectListItems(clients);  // Create a list of SelectListItems so these can be rendered on the page

            return View(model);
        }

        [HttpPost]
        public ActionResult OrderImport(OrderImportFile model)
        {
            var clientfolder = ""; // initialize
            var clients = GetClients();
            model.Clients = GetSelectListItems(clients);
            var client = model.Client;

            // Change folders based on client dropdown selection.
            switch (client)
            {
                case "Akzo Nobel":
                    clientfolder = "AkzoNobel";
                    break;

                case "Archroma":
                    clientfolder = "Archroma";
                    break;

                case "Clariant":
                    clientfolder = "Clariant";
                    break;

                case "Cytec":
                    clientfolder = "Cytec";
                    break;

                case "Dow Chemical":
                    clientfolder = "DowChem";
                    break;

                case "DSM":
                    clientfolder = "DSM";
                    break;

                case "Eastman":
                    clientfolder = "Eastman";
                    break;

                case "Elementis":
                    clientfolder = "Elementis";
                    break;

                case "Honeywell":
                    clientfolder = "Honeywell";
                    break;

                case "Lonza":
                    clientfolder = "Lonza";
                    break;

                case "Momentive":
                    clientfolder = "Momentive";
                    break;

                case "OMG":
                    clientfolder = "OMG";
                    break;

                case "Reichhold":
                    clientfolder = "Reichhold";
                    break;

                case "Stepan":
                    clientfolder = "Stepan";
                    break;

                case "Sun Chemical":
                    clientfolder = "SunChem";
                    break;

                default:
                    TempData["message"] = "Please select a client from the dropdown list";

                    return RedirectToAction("Index");
                //return RedirectToAction("OrderImport");
            }

            try
            {
                foreach (var file in model.Files)
                {
                    if (file.ContentLength != 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/OrderImportFiles/" + clientfolder), fileName);
                        file.SaveAs(path);
                        TempData["message"] = String.Format("The {0} order import files uploaded successfully", client);
                    }
                }
            }
            catch (Exception)
            {
                TempData["message"] = "Please make sure you browse and select atleast a file";
            }
            return RedirectToAction("Index");
            //return RedirectToAction("OrderImport");
        }

        // Return a list of clients that send files and have folders in Content/OrderImport
        // Consider call to data access layer to retrieve clients from database.
        private IEnumerable<string> GetClients()
        {
            return new List<string>
            {
                "Akzo Nobel",
                "Archroma",
                "Clariant",
                "Cytec",
                "Dow Chemical",
                "DSM",
                "Eastman",
                "Elementis",
                "Honeywell",
                "Lonza",
                "Momentive",
                "OMG",
                "Reichhold",
                "Stepan",
                "Sun Chemical",
            };
        }

        //Generic Helper For Creating lists from enums. Consider moving it to relevant Helper in Phoenix.
        //
        // Create an empty list to hold result of the operation.
        //
        // For each string in the 'elements' variable, create a new SelectListItem object
        // that has both its Value and Text properties set to a particular value.
        // This will result in MVC rendering each item as: <option value="State Name">State Name</option>
        //
        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
        {
            var selectList = new List<SelectListItem>();

            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element,
                    Text = element
                });
            }
            return selectList;
        }

        #endregion Order Import Action
    }
}