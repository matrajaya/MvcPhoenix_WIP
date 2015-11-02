using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ReplenishmentsController : Controller
    {
        //
        // GET: /Replenishments/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //public ActionResult NewBulkOrder()
        public ActionResult Create()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            SuggestedBulkOrder bulkorder = new SuggestedBulkOrder();
            bulkorder.clientid = 0;
            bulkorder.divisionid = 0;
            bulkorder.ListOfClients = fnClientIDs();
            db.Dispose();

            //return View("~/Views/Replenishments/BulkOrderNew.cshtml", bulkorder);
            return View("~/Views/Replenishments/Create.cshtml", bulkorder);
        }

        // ************************************************************
        // Build a html <select> tag
        // ************************************************************
        public string BuildDivisionDropDown(int id)
        {
            string s = "";
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var qry = (from t in db.tblDivision
                       where t.ClientID == id
                       orderby t.Division, t.BusinessUnit
                       select t);
            // IMPORTANT: set the id and the name 
            s = "<select name='DivisionID' id='DivisionID' class='form-control' style='width:40%;' >";
            s = s + "<option value='0' selected=true>All Divisions</option>";
            foreach (var item in qry)
            {
                s = s + "<option value=" + item.DivisionID.ToString() + ">" + item.Division + " - " + item.BusinessUnit + "</option>";
            }
            s = s + "</select>";
            db.Dispose();
            return s;
        }

        // ************************************************************
        // Create suggested items into tblSuggestedBulk
        // ************************************************************
        [HttpPost]
        public ActionResult CreateSuggestedOrder(SuggestedBulkOrder incoming)
        {
            // Take ClientID and optional Division
            if (GenerateSuggestedOrderMain(incoming) == true)
            {
                MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
                var ItemCount = (from t in db.tblSuggestedBulk select t).Count();
                db.Dispose();
                if (ItemCount > 0)
                {
                    return PartialView("~/Views/Replenishments/_SuggestedItems.cshtml");
                }
                else
                {
                    db.Dispose(); return Content("<br>No Order Recomendations as of " + DateTime.Now.ToString() + "<br>");
                }
            }
            else
            // The call to the external messy method failed...
            { return Content("Error creating order - Contact IT support"); }
        }

        // ************************************************************
        // read a record, build an obj, return partial
        // ************************************************************
        [HttpGet]
        public ActionResult SetUpEditItem(int id)
        {
            // fill an obj and pass it to a partial for update
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            SuggestedBulkOrderItem obj = new SuggestedBulkOrderItem();
            var qry =
               (from t in db.tblSuggestedBulk
                where t.id == id
                orderby t.ProductMasterID
                select t).FirstOrDefault();
            obj.id = qry.id; obj.clientid = qry.ClientID; obj.ListOfProductMasters = fnProductMasterIDs(Convert.ToInt32(qry.ClientID));
            obj.productmasterid = Convert.ToInt32(qry.ProductMasterID); obj.supplyid = qry.SupplyID;
            obj.reorderweight = qry.ReorderWeight; obj.notes = qry.ReorderNotes;
            db.Dispose();
            return PartialView("~/Views/Replenishments/_SuggestedItemEdit.cshtml", obj);
        }

        // ************************************************************************
        // build a new obj, return a view
        // ************************************************************************
        [HttpGet]
        public ActionResult SetUpNewItem(int clientid)
        {
            // create a new empty obj and pass it to a partial for update
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            SuggestedBulkOrderItem newobj = new SuggestedBulkOrderItem();
            newobj.id = -1;     // used in save to determine INSERT vs UPDATE
            //newobj.clientid = Convert.ToInt32(Request.QueryString["clientid"]);
            newobj.clientid = clientid;
            newobj.productmasterid = 0;
            //newobj.ListOfProductMasters = fnProductMasterIDs(Convert.ToInt32(newobj.clientid));
            newobj.ListOfProductMasters = fnProductMasterIDs(clientid);
            newobj.supplyid = "";
            newobj.reorderweight = 0;
            newobj.notes = "";
            db.Dispose();
            return PartialView("~/Views/Replenishments/_SuggestedItemEdit.cshtml", newobj);
        }

        // ************************************************************
        // INSERT or UPDATE object
        // ************************************************************
        [HttpPost]
        public ActionResult SaveSuggItem(SuggestedBulkOrderItem incoming)
        {
            string s = "";
            if (fnisValidSuggItem(incoming) == false)
            {   // failed server-side validation
                return Content("Invalid data entered - please check your input" + " (" + DateTime.Now.ToString() + ")");
            }
            if (incoming.id == -1)  //insert
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    var newitem = new EF.tblSuggestedBulk   //this is an EF object (like a recordset)
                    {
                        ClientID = incoming.clientid,
                        ProductMasterID = incoming.productmasterid,
                        ReorderWeight = incoming.reorderweight,
                        ReorderNotes = incoming.notes
                    };
                    db.tblSuggestedBulk.Add(newitem);
                    db.SaveChanges();
                    int newpk = newitem.id;
                    s = "Update tblSuggestedBulk set supplyid=(Select supplyid from tblProductMaster where ProductMasterid=" + incoming.productmasterid + ") where id=" + newpk;
                    db.Database.ExecuteSqlCommand(s);
                }
                return Content("Data inserted at " + DateTime.Now.ToString());
            }

            // Update only
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblSuggestedBulk
                           where t.id == incoming.id
                           select t).FirstOrDefault();
                qry.ProductMasterID = incoming.productmasterid;
                qry.ReorderWeight = incoming.reorderweight;
                qry.ReorderNotes = incoming.notes;
                // supplyid is already there
                db.SaveChanges();
            }
            return Content("Data updated at " + DateTime.Now.ToString());
        }

        // ************************************************************
        // Just re-cycle the partial
        // ************************************************************
        [HttpGet]
        public ActionResult UpdateGrid()
        {
            return PartialView("~/Views/Replenishments/_SuggestedItems.cshtml");
        }

        // ************************************************************
        // Delete a table row
        // ************************************************************
        [HttpGet]
        public ActionResult DeleteItemFromSuggestedOrder(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            { db.Database.ExecuteSqlCommand("Delete From tblSuggestedBulk Where ID=" + id); }
            return PartialView("~/Views/Replenishments/_SuggestedItems.cshtml");
        }

        // ************************************************************
        // Begin Actions for the search / edit views
        // ************************************************************
        #region SearchActions

        // Read FormCollection and build a list
        [HttpPost]
        public ActionResult SearchBulkOrdersUserCriteria(FormCollection fc)
        {
            System.Threading.Thread.Sleep(1000);    // help AJAX
            int ordercount = Convert.ToInt32(fc["ordercount"]);
            int clientid = Convert.ToInt32(fc["clientid"]);
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<BulkOrderSearchResults> obj = new List<BulkOrderSearchResults>();
            obj = (from t in db.tblBulkOrder
                   join t2 in db.tblClient on t.ClientID equals t2.ClientID
                   let itemscount = (from items in db.tblBulkOrderItem where items.BulkOrderID == t.BulkOrderID select items).Count()
                   where t.ClientID == clientid
                   orderby t.BulkOrderID descending, t.SupplyID
                   select new BulkOrderSearchResults
                   {
                       bulkorderid = t.BulkOrderID,
                       clientid = t.ClientID,
                       supplyid = t.SupplyID,
                       orderdate = t.OrderDate,
                       comment = t.Comment,
                       bulksupplieremail = t.BulkSupplierEmail,
                       emailsent = t.EmailSent,
                       clientname = t2.ClientName,
                       itemcount = itemscount
                   }).Take(ordercount).ToList();
            db.Dispose();
            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", obj);
        }

        // ************************************************************
        // Search for unconfirmed bulk orders
        // ************************************************************
        [HttpGet]
        public ActionResult SearchUnConfirmedBulkOrders()
        {
            //System.Threading.Thread.Sleep(1000);
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<BulkOrderSearchResults> mylist = new List<BulkOrderSearchResults>();
            mylist = (from t in db.tblBulkOrder
                      join t2 in db.tblClient on t.ClientID equals t2.ClientID
                      let itemscount = (from items in db.tblBulkOrderItem where items.BulkOrderID == t.BulkOrderID select items).Count()
                      where (t.EmailSent == null)
                      orderby t.BulkOrderID descending
                      select new BulkOrderSearchResults
                      {
                          bulkorderid = t.BulkOrderID,
                          clientid = t.ClientID,
                          supplyid = t.SupplyID,
                          orderdate = t.OrderDate,
                          comment = t.Comment,
                          bulksupplieremail = t.BulkSupplierEmail,
                          emailsent = t.EmailSent,
                          clientname = t2.ClientName,
                          itemcount = itemscount
                      }).ToList();
            db.Dispose();
            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", mylist);
        }

        // ************************************************************
        // Search for last 10 bulk orders
        // ************************************************************
        [HttpGet]
        public ActionResult SearchLastTenBulkOrders()
        {
            //System.Threading.Thread.Sleep(1000);
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<BulkOrderSearchResults> mylist = new List<BulkOrderSearchResults>();
            mylist = (from t in db.tblBulkOrder
                      join t2 in db.tblClient on t.ClientID equals t2.ClientID
                      let itemscount = (from items in db.tblBulkOrderItem where items.BulkOrderID == t.BulkOrderID select items).Count()
                      orderby t.BulkOrderID descending
                      select new BulkOrderSearchResults
                      {
                          bulkorderid = t.BulkOrderID,
                          clientid = t.ClientID,
                          supplyid = t.SupplyID,
                          orderdate = t.OrderDate,
                          comment = t.Comment,
                          bulksupplieremail = t.BulkSupplierEmail,
                          emailsent = t.EmailSent,
                          clientname = t2.ClientName,
                          itemcount = itemscount
                      }).Take(10).ToList();
            db.Dispose();
            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", mylist);
        }

        // ************************************************************
        // Search for orders with OP items
        // ************************************************************
        [HttpGet]
        public ActionResult SearchOpenBulkOrders()
        {
            // Change this to start with a basic qry and add criteria
            //System.Threading.Thread.Sleep(1000);
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<BulkOrderSearchResults> mylist = new List<BulkOrderSearchResults>();
            mylist = (from t in db.tblBulkOrder
                      join t2 in db.tblClient on t.ClientID equals t2.ClientID
                      let itemscount = (from items in db.tblBulkOrderItem where items.BulkOrderID == t.BulkOrderID select items).Count()
                      let opencount = (from items in db.tblBulkOrderItem where (items.BulkOrderID == t.BulkOrderID) && (items.Status == "OP") select items).Count()
                      where opencount > 0
                      orderby t.BulkOrderID descending
                      select new BulkOrderSearchResults
                      {
                          bulkorderid = t.BulkOrderID,
                          clientid = t.ClientID,
                          supplyid = t.SupplyID,
                          orderdate = t.OrderDate,
                          comment = t.Comment,
                          bulksupplieremail = t.BulkSupplierEmail,
                          emailsent = t.EmailSent,
                          clientname = t2.ClientName,
                          itemcount = itemscount
                      }).Distinct().ToList();
            db.Dispose();
            return PartialView("~/Views/Replenishments/_SearchResults.cshtml", mylist);
        }

        // ************************************************************
        // Bulk Order Edit Landing Page
        // ************************************************************
        //public ActionResult LoadBulkOrderEdit(int id)
        public ActionResult Edit(int id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            BulkOrderHeader obj = new BulkOrderHeader();
            var qry = (from t in db.tblBulkOrder
                       where t.BulkOrderID == id
                       select t).FirstOrDefault();
            obj = fnFillBulkOrderHeader(id);
            //obj.ListOfOrderItemsForDisplay = fnFillBulkOrderItesmForDisplay(obj.bulkorderid);

            db.Dispose();

            //return View("~/Views/Replenishments/BulkOrderEdit.cshtml", obj);
            return View(obj);
        }

        // Save Order Header to DB, return a string
        [HttpPost]
        public ActionResult SaveBulkOrderHeader(BulkOrderHeader incoming)
        {
            if (ModelState.IsValid)
            {
                System.Threading.Thread.Sleep(1000);
                using (var db = new EF.CMCSQL03Entities())
                {
                    var qry = (from t in db.tblBulkOrder where t.BulkOrderID == incoming.bulkorderid select t).FirstOrDefault();
                    qry.SupplyID = incoming.supplyid;
                    qry.BulkSupplierEmail = incoming.bulksupplieremail;
                    qry.EmailSent = incoming.emailsent;
                    qry.Comment = incoming.ordercomment;
                    db.SaveChanges();
                    return Content("Saved at " + DateTime.Now.ToString());
                }
            }
            else
            { return Content("Validation error"); }
        }

        public ActionResult RefreshItemsGrid(int id)
        {
            BulkOrderHeader obj = new BulkOrderHeader();
            obj = fnFillBulkOrderHeader(id);
            return PartialView("~/Views/Replenishments/_BulkOrderItems.cshtml", obj);
        }

        [HttpGet]
        public ActionResult DeleteBulkOrderItem(int id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var q = (from t in db.tblBulkOrderItem
                     where t.BulkOrderItemID == id
                     select t).FirstOrDefault();
            int intBulkOrderID = Convert.ToInt32(q.BulkOrderID);
            string s = "Delete from tblBulkOrderItem where BulkOrderItemID=" + id;
            db.Database.ExecuteSqlCommand(s); db.Dispose();
            BulkOrderHeader obj = new BulkOrderHeader();
            obj = fnFillBulkOrderHeader(intBulkOrderID);
            return PartialView("~/Views/Replenishments/_BulkOrderItems.cshtml", obj);
        }

        [HttpGet]
        public ActionResult SetupBulkOrderItemInsert(int id, int clientid)
        {
            // id = bulkorderid
            BulkOrderItem obj = new BulkOrderItem();
            obj.bulkorderitemid = -1; obj.bulkorderid = id; obj.productmasterid = null;
            obj.weight = null; obj.itemstatus = "OP";
            obj.eta = null;
            obj.datereceived = null;
            obj.itemnotes = null;
            obj.ListOfProductMasters = fnProductMasterIDs(clientid);
            obj.ListOfItemStatusIDs = fnOrderItemStatusIDs();
            return PartialView("~/Views/Replenishments/_BulkOrderItemEdit.cshtml", obj);
        }

        [HttpGet]
        public ActionResult SetupBulkOrderItemEdit(int id, int clientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            BulkOrderItem obj = new BulkOrderItem();
            obj = fnFillBulkOrderItem(id);
            obj.ListOfProductMasters = fnProductMasterIDs(clientid);
            obj.ListOfItemStatusIDs = fnOrderItemStatusIDs();
            return PartialView("~/Views/Replenishments/_BulkOrderItemEdit.cshtml", obj);
        }

        private string InsertBulkOrderItem(BulkOrderItem incoming)
        {
            System.Threading.Thread.Sleep(1500);
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var newitem = new EF.tblBulkOrderItem
            {
                BulkOrderID = incoming.bulkorderid,
                ProductMasterID = incoming.productmasterid,
                Weight = incoming.weight,
                Status = incoming.itemstatus,
                ETA = incoming.eta
            };
            db.tblBulkOrderItem.Add(newitem);
            db.SaveChanges(); db.Dispose();
            int newpk = newitem.BulkOrderItemID;
            return "Added at " + DateTime.Now.ToString();
        }

        private string UpdateBulkOrderItem(BulkOrderItem incoming)
        {
            System.Threading.Thread.Sleep(1000);
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            BulkOrderItem obj = new BulkOrderItem();
            var q = (from t in db.tblBulkOrderItem where t.BulkOrderItemID == incoming.bulkorderitemid select t).FirstOrDefault();
            q.ProductMasterID = incoming.productmasterid;
            q.Weight = incoming.weight;
            q.Status = incoming.itemstatus;
            q.ETA = incoming.eta;
            // q.DateReceived =incoming.datereceived;
            q.ItemNotes = incoming.itemnotes;
            db.SaveChanges();
            db.Dispose();
            return "Updated at " + DateTime.Now.ToString();
        }

        [HttpPost]
        public ActionResult ProcessBulkOrderItemUpdate(BulkOrderItem incoming)
        {
            if (incoming.bulkorderitemid == -1)
            {
                return Content(InsertBulkOrderItem(incoming));
            }
            else
            {
                return Content(UpdateBulkOrderItem(incoming));
            }


        }

        [HttpPost]
        public string SendConfirmationEmail(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblBulkOrder where t.BulkOrderID == id select t).FirstOrDefault();
                string vtimestamp = DateTime.Now.ToString();
                qry.EmailSent = vtimestamp;
                db.SaveChanges();

                return vtimestamp;
            }
        }

        private BulkOrderHeader fnFillBulkOrderHeader(int id)
        {
            // FIll OrderHeader and List of otems
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            BulkOrderHeader obj = new BulkOrderHeader();
            var qry = (from t in db.tblBulkOrder where t.BulkOrderID == id select t).FirstOrDefault();
            obj.bulkorderid = qry.BulkOrderID; obj.clientid = qry.ClientID; obj.clientname = fnClientName(qry.ClientID);
            obj.orderdate = qry.OrderDate; obj.ordercomment = qry.Comment; obj.supplyid = qry.SupplyID;
            obj.ListOfOrderHeaderSupplyIDs = fnOrderHeaderSupplyIDs(Convert.ToInt32(qry.ClientID));
            obj.bulksupplieremail = qry.BulkSupplierEmail; obj.emailsent = qry.EmailSent;
            obj.ListOfOrderItemsForDisplay = fnFillBulkOrderItemsForDisplay(id);
            db.Dispose();

            return obj;
        }

        private static List<BulkOrderItemsForDisplay> fnFillBulkOrderItemsForDisplay(int bulkorderid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var mylist = (from t in db.tblBulkOrderItem
                          join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID into result
                          from temp in result.DefaultIfEmpty()
                          where t.BulkOrderID == bulkorderid
                          orderby temp.MasterCode
                          select new BulkOrderItemsForDisplay
                          {
                              bulkorderitemid = t.BulkOrderItemID,
                              bulkorderid = t.BulkOrderID,
                              productmasterid = t.ProductMasterID,
                              weight = t.Weight,
                              eta = t.ETA,
                              datereceived = t.DateReceived,
                              itemnotes = t.ItemNotes,
                              itemstatus = t.Status,
                              mastercode = temp.MasterCode,
                              mastername = temp.MasterName
                          }).ToList();

            return mylist;
        }

        private BulkOrderItem fnFillBulkOrderItem(int id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            BulkOrderItem obj = new BulkOrderItem();
            var qry = (from t in db.tblBulkOrderItem
                       where t.BulkOrderItemID == id
                       select t).FirstOrDefault();
            obj.bulkorderitemid = qry.BulkOrderItemID;
            obj.bulkorderid = qry.BulkOrderID;
            obj.productmasterid = qry.ProductMasterID;
            obj.weight = qry.Weight;
            obj.itemstatus = qry.Status;
            obj.eta = qry.ETA;
            obj.datereceived = qry.DateReceived;
            obj.itemnotes = qry.ItemNotes;
            // these list should be built here
            //obj.ListOfProductMasters = fnProductMasterIDs(Convert.ToInt32(qry2.ClientID));
            //obj.ListOfItemStatusIDs = fnOrderItemStatusIDs();
            db.Dispose();

            return obj;
        }

        #endregion


        // ************************************************************
        // This Action reads the temp records and creates real data 
        // ************************************************************
        [HttpGet]
        public ActionResult CreateBulkOrders()
        {
            // Create a new tblBulkOrder from tblSuggestedBulk for each distinct ClientID-SupplyID
            // Empty the table when doe
            // TODO: Modify to allow multiuser use by adding a userid to tblSuggestedBulk

            string s; string fnTempTable = "tblSuggestedBulk";
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            s = "Update " + fnTempTable + " set SupplyID='n/a' where Supplyid is null";
            db.Database.ExecuteSqlCommand(s);
            // how do I pass fnTempTable into the LINQ query?
            var qry = (from t in db.tblSuggestedBulk
                       select new { t.ClientID, t.SupplyID }).Distinct();
            int SupplyIDCount = qry.Count();
            DateTime myOrderDate = DateTime.Now;
            string BatchNumber = myOrderDate.ToString();
            string sSessionID = Session.SessionID;
            MvcPhoenix.EF.CMCSQL03Entities db1 = new MvcPhoenix.EF.CMCSQL03Entities();

            // For each distinct supplyid
            foreach (var item in qry)
            {
                // build a new bulk order
                var newitem = new EF.tblBulkOrder
                {
                    ClientID = item.ClientID,
                    OrderDate = myOrderDate,
                    Status = "OP",
                    SupplyID = item.SupplyID
                };
                db1.tblBulkOrder.Add(newitem);
                db1.SaveChanges();
                int newpk = newitem.BulkOrderID;
                System.Diagnostics.Debug.WriteLine("New OrderID= " + newpk.ToString());
                //// now create order items records
                s = "Insert into tblBulkOrderItem (BulkOrderID,ProductMasterID,Qty,Weight,Status,SupplyID,ItemNotes)";
                s = s + " Select " + newpk + ",ProductMasterID,1,ReorderWeight,'OP',SupplyID,ReorderNotes from " + fnTempTable;
                s = s + " Where Supplyid='" + item.SupplyID + "'";
                System.Diagnostics.Debug.WriteLine(s);
                db1.Database.ExecuteSqlCommand(s);
            }

            db1.Database.ExecuteSqlCommand("Delete From tblSuggestedBulk");
            db1.Dispose(); db.Dispose();

            return View("~/Views/Replenishments/Index.cshtml");
        }

        // ****************************************************************
        // Support methods for this controller
        // ****************************************************************

        private string fnClientName(int? id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var qry = (from t in db.tblClient where t.ClientID == id select t).FirstOrDefault();
            db.Dispose();
            return qry.ClientName.ToString();
        }

        private static bool fnisValidSuggItem(SuggestedBulkOrderItem incoming)
        {
            // server side validation
            bool retvalue = true;
            //string errormessage = "";
            if (incoming.reorderweight < 0 || incoming.reorderweight > 1000)
            { retvalue = false; }
            return retvalue;
        }

        // *******************************************************************
        // Fill temp table - WARNING: needs to be modified for multi-user use
        // *******************************************************************
        private bool GenerateSuggestedOrderMain(SuggestedBulkOrder obj)
        {
            // TODO: Put some of the hard-coded values into the ViewModel for user to edit (per cd)
            //System.Diagnostics.Debug.WriteLine(s);
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            bool result = true;
            try
            {
                string s;
                string fnTempTable = "tblSuggestedBulk";
                string sSessionID = Session["MySessionID"].ToString();

                s = "Delete from " + fnTempTable;
                //s = "Delete from " + fnTempTable + " where SessionID='" + sSessionID + "'";
                db.Database.ExecuteSqlCommand(s);

                s = "Insert into " + fnTempTable + "(ClientID, ProductMasterID,SUPPLYID, ShelfLife, CreateDate)";
                s = s + " SELECT ClientID, ProductMasterID,SUPPLYID,ShlfLife,CreateDate from tblProductMaster";
                s = s + " Where ClientID=" + obj.clientid;
                if (obj.divisionid > 0)
                { s = s + " and MasterDivisionID='" + obj.divisionid + "'"; }
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set SessionID='" + sSessionID + "'";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkOnOrder=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ProductMasterAge=DateDiff(day,createdate,getdate())";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkCurrentavailable=(Select isnull(Sum(  isnull(qty,1) *CurrentWeight),0) from tblBulk where ProductMasterID=" + fnTempTable + ".ProductMasterID and BulkStatus not in('QC','TEST','WASTE'))";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfCurrentAvailable=(Select isnull(Sum(TotalShelfWeight),0) from vwStockWeightsForReOrder where ProductMasterID=" + fnTempTable + ".ProductMasterID and ShelfStatus not in('QC','TEST','WASTE'))";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set CurrentAvailable=IsNull(BulkCurrentAvailable,0)+IsNull(ShelfCurrentAvailable,0)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPastYear=(Select sum(TransQty*TransAmount) from vwInvTransBulk";
                s = s + " Where TransType='B02' and Status not in('QC','TEST','WASTE') and transdate>DateAdd(day,-365,getdate())";
                s = s + " And ProductMasterID=" + fnTempTable + ".ProductMasterID)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPastYear=0 where BulkShippedPastYear is Null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPerDay=bulkShippedPastYear/365 where BulkShippedPastYear>0 and ProductMasterAge>365";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPerDay=bulkShippedPastYear/ProductMasterAge where BulkShippedpastYear>0 and ProductMasterAge<=365";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkShippedPerDay=0 Where BulkShippedPerDay is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPastYear=(Select isnull(Sum(TransQty*TransAmount),0) from vwInvTransShelf";
                s = s + " Where Status not in('QC','TEST','WASTE') and TransType IN('S04') and transdate>DateAdd(day,-365,getdate()) And ProductMasterID=" + fnTempTable + ".ProductMasterID)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPastYear=0 where ShelfShippedPastYear is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPerDay=ShelfShippedPastYear/365 Where ShelfShippedPastYear>0 and ProductMasterAge>365";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPerDay=ShelfShippedPastYear/ProductMasterAge Where ShelfShippedPastYear>0 and ProductMasterAge<=365";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfShippedPerDay=0 Where ShelfShippedPerDay Is Null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkLatestExpirationDate=(Select top 1 ExpirationDate from tblBulk where ProductMasterID=" + fnTempTable + ".ProductMasterID order by ExpirationDate Desc)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkDaysTilExpiration=DateDiff(day, Getdate(), BulkLatestExpirationDate)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfLatestExpirationDate=(Select Max(expirationdate) from vwExpirationForReorder where ProductMasterid=" + fnTempTable + ".ProductMasterID)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ShelfDaysTilExpiration=DateDiff(day, Getdate(), ShelfLatestExpirationDate)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisExpirationDate=IIF(BulkLatestExpirationDate>ShelfLatestExpirationDate,BulkLatestExpirationDate,ShelfLatestExpirationDate)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisExpirationDate=BulkLatestExpirationDate where UseThisExpirationDate is null and ShelfLatestExpirationDate is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisExpirationDate=ShelfLatestExpirationDate where UseThisExpirationDate is null and BulkLatestExpirationDate is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisExpirationDate='2099-01-01' where UseThisExpirationDate is null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisDaysTilExpiration=datediff(day,getdate(),UseThisExpirationDate)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set UseThisDaysTilExpiration=999 where UseThisDaysTilExpiration>998";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set DaysSupplyLeft= CurrentAvailable / (ShelfShippedPerDay+BulkShippedPerDay) Where (ShelfShippedPerDay+BulkShippedPerDay>0)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set DaysSupplyLeft=0 Where DaysSupplyLeft Is Null";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set AverageLeadTime=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=0,ReorderWeight=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set BulkOnOrder=(Select Case When (Select Count(*) from tblBulkOrderItem Where ProductMasterID=" + fnTempTable + ".ProductMasterID and Status in('OP','OPEN'))>0 then 1 Else 0 End)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=1 Where DaysSupplyLeft<65";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=1 Where UseThisDaysTilExpiration<65";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=0 Where (ShelfShippedPerDay)+(BulkShippedPerDay)=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderThis=0 Where BulkOnOrder=1";
                db.Database.ExecuteSqlCommand(s);

                s = "Delete from " + fnTempTable + " where ReorderThis=0";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderWeight=Round((ShelfShippedPerDay+BulkShippedPerDay) * 120,0) Where ShelfLife<13 and ReorderThis=1";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderWeight=Round((ShelfShippedPerDay+BulkShippedPerDay) * 180,0) Where (ShelfLife>=13 or ShelfLife is null) and (ReorderThis=1)";
                db.Database.ExecuteSqlCommand(s);

                s = "Update " + fnTempTable + " Set ReorderWeight=1 Where (ReorderWeight<1)  and (ReorderThis=1)";
                db.Database.ExecuteSqlCommand(s);

                // success
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                db.Dispose();
            }

            return result;
        }

        private static List<SelectListItem> fnOrderItemStatusIDs()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            //mylist = (from t in db.tblBulkOrderItem
            //          orderby t.Status
            //          select
            //          new SelectListItem { Value = t.Status, Text = t.Status }).Distinct().ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            mylist.Insert(1, new SelectListItem { Value = "CL", Text = "Closed" });
            mylist.Insert(2, new SelectListItem { Value = "OP", Text = "Open" });
            mylist.Insert(3, new SelectListItem { Value = "CN", Text = "Cancelled" });
            return mylist;
        }

        private static List<SelectListItem> fnOrderHeaderSupplyIDs(int clientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            //mylist = (from t in db.tblBulkSupplier
            //          where t.ClientID==clientid
            //          orderby t.SupplyID
            //          select
            //          new SelectListItem { Value = t.SupplyID, Text = t.SupplyID }).Distinct().ToList();
            //mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
            // 10/06 update
            mylist = (from t in db.tblProductMaster
                      where t.ClientID == clientid
                      orderby t.SUPPLYID
                      select
                      new SelectListItem { Value = t.SUPPLYID, Text = t.SUPPLYID }).Distinct().ToList();

            return mylist;
        }

        private static List<SelectListItem> fnProductMasterIDs(int clientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblProductMaster
                      where t.ClientID == clientid
                      orderby t.MasterCode
                      select
                          new SelectListItem { Value = t.ProductMasterID.ToString(), Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25) }).ToList();

            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Master Code" });
            db.Dispose();
            return mylist;
        }

        private static List<SelectListItem> fnClientIDs()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblClient
                      orderby t.ClientName
                      select
                          new SelectListItem { Value = t.ClientID.ToString(), Text = t.ClientName }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });
            db.Dispose();
            return mylist;
        }

        private static List<SelectListItem> fnDivisionIDs(int clientid)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from c in db.tblDivision where c.ClientID == clientid select new SelectListItem { Value = c.DivisionID.ToString(), Text = c.Division }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Division" });
            db.Dispose();
            return mylist;
        }

    }
}
