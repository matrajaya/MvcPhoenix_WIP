using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace MvcPhoenix.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public ActionResult Index()
        {
            // Index returns last 10 orders for the Landing Page
            // build order list and return to view
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();
            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()

                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()

                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          CMCUser = t.CMCUser,
                          ItemsCount = count
                      }).Take(10).ToList();
            ViewData["SearchResultsMessage"] = "Last 10 Orders";
            return View(mylist);
        }

        // Edit Order Content
        #region

        public ActionResult Edit(int id)
        {
            OrderMasterFull obj = new OrderMasterFull();
            obj = OrderService.fillOrderMasterObject(id);
            return View(obj);
        }

        public string GetSomeContent(int id)
        {
            string s = "<select>";
            var qry = (from t in db.tblClient where t.ClientID == id orderby t.ClientName select t);
            foreach (var item in qry)
            {
                s = s + "<option>" + item.ClientName + "</option>";
            }
            s = s + "</select>";
            return s;
        }

        [HttpGet]
        public ActionResult SetUpNewItem(int ItemID, string Mode, int myOrderID)
        {
            // Called from partial view to setup Item for insert
            OrderItem obj = new OrderItem();
            obj.OrderID = myOrderID;
            obj.ItemID = -1;
            obj.ProfileID = 0;
            obj.PartialMode = "Add";
            obj.Size = "";
            obj.SRSize = "";
            obj.Qty = 0;
            obj.LotNumber = "";
            obj.NonCMCDelay = false; obj.CarrierInvoiceRcvd = false;
            obj.StatusID = 0;
            obj.Status = "";
            return PartialView("~/Views/Orders/_ItemEditModal.cshtml", obj);
        }

        [HttpGet]
        public ActionResult SetUpEditItem(int ItemID, string Mode)
        {
            // Called from partial view to setup Item for edit
            OrderItem obj = new OrderItem();
            obj = OrderService.fnFillOrderItem(ItemID);
            
            obj.PartialMode = "Edit";
            obj.StatusID = 0;
            return PartialView("~/Views/Orders/_ItemEditModal.cshtml", obj);
        }

        [HttpGet]
        public string GrabString(int ItemID)
        {
            return "<br>Build the U/I for ItemID=" + ItemID.ToString();
        }

        public ActionResult DeleteItemFromOrder(int id)
        {
            System.Threading.Thread.Sleep(1500);
            OrderService.fnDeleteOrderItem(id);
            OrderItem obj = new OrderItem();
            return PartialView("~/Views/Orders/_OrderItems.cshtml", obj);
        }

        [HttpPost]
        public ActionResult GetSizesForProfileID(OrderItem incoming)
        {
            // Take an orderitem and rebuild it with a different ProfileID
            // so that the DD renders sizes
            OrderItem obj = new OrderItem();
            obj.ClientID = incoming.ClientID;
            obj.OrderID = incoming.OrderID;
            obj.ItemID = incoming.ItemID;
            obj.PartialMode = incoming.PartialMode;
            obj.ProfileID = incoming.ProfileID;
            obj.Size = incoming.Size;
            obj.SRSize = incoming.Size;
            obj.Qty = incoming.Qty;
            obj.LotNumber = incoming.LotNumber;
            obj.NonCMCDelay = incoming.NonCMCDelay;
            obj.CarrierInvoiceRcvd = incoming.CarrierInvoiceRcvd;
            obj.Status = incoming.Status;
            return PartialView("~/Views/Orders/_ItemEditModal.cshtml", obj);
        }

        public ActionResult RefreshItemsTable()
        {
            return PartialView("~/Views/Orders/_OrderItems.cshtml");
        }

        [HttpPost]
        public ActionResult UpdateOrderItemJson(OrderItem incoming)
        {
            int myid = incoming.ItemID;
            int dbpk = 0;

            OrderItem validateagainst = new OrderItem();
            if (TryUpdateModel(validateagainst) == false)
            {
                incoming.UpdateResult = "Validation error - Update failed";
                return PartialView("~/Views/Orders/_ItemEditModal.cshtml", incoming);
            }

            if (OrderService.IsValidOrderItem(incoming))
            {
                // continue
            }
            else
            {
                incoming.UpdateResult = "Data Entry error - Item NOT added";
                return PartialView("~/Views/Orders/_ItemEditModal.cshtml", incoming);
            }

            if (incoming.ItemID == 0)
            {
                OrderItem emptyobj = new OrderItem();
                emptyobj.PartialMode = "Add";
                emptyobj.UpdateResult = "No item selected - No work was done..";
                return PartialView("~/Views/Orders/_ItemEditModal.cshtml", emptyobj);
            }
            else if (incoming.ItemID == -1)
            { dbpk = OrderService.fnInsertOrderItem(incoming); }
            else
            { dbpk = OrderService.fnUpdateOrderItem(incoming); }

            // Check DB success
            if (dbpk > 0)
            {
                OrderItem newobj = new OrderItem();
                newobj = OrderService.fnFillOrderItem(dbpk);
                newobj.StatusID = 0;    // Force the Status DD to 0
                newobj.PartialMode = "Edit";
                newobj.UpdateResult = "Changes Saved at " + System.DateTime.Now.ToString();
                return PartialView("~/Views/Orders/_ItemEditModal.cshtml", newobj);
            }
            else
            {
                incoming.UpdateResult = "DB ERROR - Update failed at " + System.DateTime.Now.ToString();
                return PartialView("~/Views/Orders/_ItemEditModal.cshtml", incoming);
            }
        }

        public ActionResult RefreshTransView()
        {
            return PartialView("~/Views/Orders/_OrderTrans.cshtml", new MvcPhoenix.Models.OrderTrans());
        }

        public ActionResult QuickDeleteOrderTrans(int OrderTransID)
        {
            string s = @"Delete from tblOrderTrans Where OrderTransID=" + OrderTransID;
            db.Database.ExecuteSqlCommand(s);
            db.Dispose();
            OrderTrans obj = new OrderTrans();
            return PartialView("~/Views/Orders/_OrderTrans.cshtml", obj);
        }

        #endregion

        // Index Search Actions 
        #region 

        [HttpGet]
        public ActionResult OrdersToday()
        {
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();
            DateTime datToday = DateTime.Today.AddDays(0);
            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      where t.OrderDate == datToday
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          CMCUser = t.CMCUser,
                          ItemsCount = count
                      }).ToList();

            ViewData["SearchResultsMessage"] = "Showing Orders Created Today";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
        }

        public ActionResult OrdersYesterday()
        {
            // TODO: fix to be previous business day
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();

            DateTime datYesterday = DateTime.Today.AddDays(-1);
            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      where t.OrderDate == datYesterday
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          CMCUser = t.CMCUser,
                          ItemsCount = count
                      }).ToList();

            ViewData["SearchResultsMessage"] = "Showing Orders Created Yesterday";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
        }

        [HttpGet]
        public ActionResult OrdersLastTen()
        {
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();
            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          CMCUser = t.CMCUser,
                          ItemsCount = count
                      }).Take(10).ToList();

            ViewData["SearchResultsMessage"] = "Showing Last 10 Orders";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
        }

        [HttpGet]
        public ActionResult OrdersMineLastTen()
        {
            string myusername = "JBrok";  // bogus value
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();
            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      where t.CMCUser == myusername
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          //CMCUser = t.CMCUser,
                          CMCUser = "John Smith",
                          ItemsCount = count
                      }).Take(10).ToList();
            ViewData["SearchResultsMessage"] = "Showing Last 10 Orders For " + "John Smith";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
        }

        [HttpGet]
        public ActionResult OrdersProblems()
        {
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();
            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          CMCUser = t.CMCUser,
                          ItemsCount = count
                      }).Take(10).ToList();


            ViewData["SearchResultsMessage"] = " Orders With Problems";
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
        }

        [HttpPost]
        public ActionResult LookupClientID(FormCollection fc)
        {
            int myint = Convert.ToInt32(fc["DDClientID"]);
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();
            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      where t.ClientID == myint
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          CMCUser = t.CMCUser,
                          ItemsCount = count
                      }).Take(20).ToList();

            var qClientName = mylist.Select(x => x.ClientName).FirstOrDefault();
            ViewData["SearchResultsMessage"] = "Showing Last 20 Orders For " + qClientName;
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
        }

        [HttpPost]
        public ActionResult LookupOrderID(FormCollection fc)
        {
            // TODO: If lookup is a hit, redirec to Edit
            int myint = Convert.ToInt32(fc["orderid"]);
            var qry =
                (from t in db.tblOrderMaster
                 where t.OrderID == myint
                 select t.OrderID).Count();

            if (qry == 1)
            {
                OrderMasterFull obj = new OrderMasterFull();
                obj = OrderService.fillOrderMasterObject(myint);
                return View("~/Views/Orders/Edit.cshtml", obj);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult LookupOrderDate(FormCollection fc)
        {
            DateTime mydate = Convert.ToDateTime(fc["searchorderdate"]);
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();

            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      where t.OrderDate == mydate
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          CMCUser = t.CMCUser,
                          ItemsCount = count
                      }).ToList();

            ViewData["SearchResultsMessage"] = "Showing Orders Created On " + mydate.ToShortDateString();
            return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
        }

        [HttpPost]
        public ActionResult LookupCompany(FormCollection fc)
        {
            List<OrdersListForLandingPage> mylist = new List<OrdersListForLandingPage>();
            string mycompany = fc["searchcompany"];
            //var qry =
            //(from t in db.tblOrderMaster
            // orderby t.Company
            // where t.Company.Contains(mycompany)
            // select t).ToList();
            var intcount = (from t in db.tblOrderMaster
                            orderby t.Company
                            where t.Company.Contains(mycompany)
                            select t).Count();

            mylist = (from t in db.tblOrderMaster
                      orderby t.OrderID descending
                      where t.Company.Contains(mycompany)
                      let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                      let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                      select new OrdersListForLandingPage
                      {
                          OrderID = t.OrderID,
                          Customer = t.Customer,
                          ClientName = clientname,
                          OrderDate = t.OrderDate,
                          Company = t.Company,
                          OrderType = t.OrderType,
                          CMCUser = t.CMCUser,
                          ItemsCount = count
                      }).ToList();

            if (intcount <= 100)
            {
                ViewData["SearchResultsMessage"] = "Showing Orders " + mycompany;
                return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
            }
            else
            {
                // mylist back to default
                mylist = (from t in db.tblOrderMaster
                          orderby t.OrderID descending
                          where t.OrderDate == DateTime.Today
                          let count = (from items in db.tblOrderItem where items.OrderID == t.OrderID select items).Count()
                          let clientname = (from c in db.tblClient where c.ClientID == t.ClientID select c.ClientName).FirstOrDefault()
                          select new OrdersListForLandingPage
                          {
                              OrderID = t.OrderID,
                              Customer = t.Customer,
                              ClientName = clientname,
                              OrderDate = t.OrderDate,
                              Company = t.Company,
                              OrderType = t.OrderType,
                              CMCUser = t.CMCUser,
                              ItemsCount = count
                          }).ToList();
                return PartialView("~/Views/Orders/_IndexPartial.cshtml", mylist);
            }
        }

        [HttpPost]
        public ActionResult LookupMisc(FormCollection fc)
        {
            string myfield = (fc["DDMisc"]);
            string myfieldvalue = fc["miscfieldvalue"];

            switch (myfield)
            {
                case "ShipRef": var qry1 = (from t in db.tblOrderMaster
                                            where t.ShipRef == myfieldvalue
                                            orderby t.Company
                                            select t).ToList();
                    ViewBag.qryforgrid = qry1;
                    break;

                case "CustOrderNum": var qry2 = (from t in db.tblOrderMaster
                                                 where t.CustOrdNum == myfieldvalue
                                                 orderby t.Company
                                                 select t).ToList();
                    ViewBag.qryforgrid = qry2;
                    break;

                case "ZipCode": var qry3 = (from t in db.tblOrderMaster
                                            where t.Zip == myfieldvalue
                                            orderby t.Company
                                            select t).ToList();
                    ViewBag.qryforgrid = qry3;
                    break;

                case "SalesRep":
                    var qry4 = (from t in db.tblOrderMaster
                                orderby t.Company
                                where t.SalesRep.Contains(myfieldvalue)
                                select t).ToList().Take(50);
                    ViewBag.qryforgrid = qry4;
                    break;

                default: var qry0 =
                        (from t in db.tblOrderMaster
                         orderby t.OrderID descending, t.Company ascending
                         select t).ToList().Take(20);
                    ViewBag.qryforgrid = qry0;
                    break;
            }
            return PartialView("~/Views/Orders/_IndexPartial.cshtml");
        }
        #endregion

        // Order Transaction Actions
        #region

        public ActionResult SetUpNewOrderTrans()
        {
            OrderTrans obj = new OrderTrans();
            // Default values:
            obj.ordertransid = -1;
            obj.orderitemid = 0;
            obj.transtype = null;
            obj.transqty = 1;
            //obj.transdate = DateTime.Today; -- Iffy
            obj.transdate = DateTime.Now;
            obj.transamount = 0;
            obj.comments = "";
            obj.pagemode = "New Transaction";
            return PartialView("~/Views/Orders/_TransEditModal.cshtml", obj);
        }

        [HttpGet]
        public ActionResult SetTransFilter(int intfilter)
        {
            OrderTrans obj = new OrderTrans();
            if (intfilter == 1)
            {
                obj.qryfilter = "All";
            }
            else if (intfilter == 2)
            {
                obj.qryfilter = "OrderOnly";
            }
            return PartialView("~/Views/Orders/_OrderTrans.cshtml", obj);
        }

        public ActionResult SetUpEditOrderTrans(int OrderTransID)
        {
            OrderTrans obj = new OrderTrans();
            obj = OrderService.fnFillOrderTrans(OrderTransID);
            obj.pagemode = "Edit Transaction";
            return PartialView("~/Views/Orders/_TransEditModal.cshtml", obj);
        }

        public ActionResult SaveOrderTrans(OrderTrans incoming)
        {
            if (incoming.ordertransid == 0 || incoming.transqty == 0)
            {
                OrderTrans emptyobj = new OrderTrans();
                emptyobj.pagemode = "";
                emptyobj.updateresult = "Please Enter Quantity";
                return PartialView("~/Views/Orders/_TransEditModal.cshtml", emptyobj);
            }
            if (incoming.ordertransid == -1)
            {
                // New record
                int newpk = OrderService.fnAddOrderTrans(incoming);
                OrderTrans newobj = new OrderTrans();
                newobj = OrderService.fnFillOrderTrans(newpk);
                newobj.pagemode = "";
                newobj.updateresult = "New record added";
                return PartialView("~/Views/Orders/_TransEditModal.cshtml", newobj);
            }
            else
            {
                OrderService.fnUpdateOrderTrans(incoming);
                OrderTrans obj = new OrderTrans();
                obj = OrderService.fnFillOrderTrans(incoming.ordertransid);
                obj.pagemode = "Edit Transaction";
                obj.updateresult = "Record updated at " + DateTime.Now.ToString();
                return PartialView("~/Views/Orders/_TransEditModal.cshtml", obj);
            }
        }

        #endregion

        // New Order Actions
        #region

        [HttpPost]
        public ActionResult SetupNewOrder(FormCollection fc)
        {
            int ClientID = Convert.ToInt32(fc["NewClientID"]);
            // Build a barebones order and flip to edit mode
            if (ClientID > 0)
            {
                OrderMasterFull newobj = new OrderMasterFull(ClientID); // constructor can set initial values
                newobj.orderdate = DateTime.Now;
                newobj.clientid = ClientID;
                newobj.orderid = -1;    // important to do an insert
                int newpk = OrderService.SaveOrderMaster(newobj);
                OrderMasterFull obj = new OrderMasterFull();
                obj = OrderService.fillOrderMasterObject(newpk);
                return View("~/Views/Orders/Edit.cshtml", obj);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult SavePostData(OrderMasterFull incoming)
        {
            int pk = incoming.orderid;
            int myclientid = Convert.ToInt32(Request.Form["hiddenclientid"]);
            incoming.clientid = myclientid;
            OrderMasterFull newobj = new OrderMasterFull();

            // The following statement attempts to bind the incoming object to a new model object
            if (TryUpdateModel(newobj))
            {
                // At this point the model appears to be good, now lets try a DB update
                int RecordSavedResult = OrderService.SaveOrderMaster(incoming);
                if (RecordSavedResult == pk)    // Backend returned the same PK we sent it
                {
                    // refill the object in case some work was done on the back end that may affect display values
                    OrderMasterFull obj = new OrderMasterFull();
                    obj = OrderService.fillOrderMasterObject(pk);
                    obj.UpdateResult = "<span style='color:green;font-weight:bold;'>Order Information has been successfully updated at " + DateTime.Now + "</span>";
                    return View("Edit", obj);
                }
                else
                {
                    // return incoming for corrections
                    incoming.UpdateResult = "<span style='color:red;font-size:1.5em;font-weight:bold;'>Database ERROR: Unable to update order</span>";
                    return View("Edit", incoming);
                }
            }
            else
            {
                // return incoming for corrections
                incoming.UpdateResult = "<span style='color:red;font-size:1.5em;font-weight:bold;'>Model Validation Failed</span>";
                return View("Edit", incoming);
            }
        }

        #endregion

        public ActionResult PrintOrder()
        {
            // The data view will need to more complicated
            OrderMasterFull obj = new OrderMasterFull();
            obj = OrderService.fillOrderMasterObject(Convert.ToInt32(Session["OrderID"]));
            return View("PrintOrder", obj);
        }

        public ActionResult Notification()
        {
            // The data view will need to more complicated
            OrderMasterFull obj = new OrderMasterFull();
            obj = OrderService.fillOrderMasterObject(Convert.ToInt32(Session["OrderID"]));
            return View("Notification", obj);
        }

        public ActionResult Import()
        {
            return View();
        }

        // Order Import Action
        #region

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
                    return RedirectToAction("OrderImport");
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
            return RedirectToAction("OrderImport");
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

        #endregion
    }
}