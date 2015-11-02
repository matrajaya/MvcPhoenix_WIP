using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoenix.Models;

namespace MvcPhoenix.Controllers
{
    public class ReceivingController : Controller
    {
        //
        // GET: /Receiving/
        //
        [HttpGet]
        public ActionResult Index()
        {
            // Bulk Container Landing Page - returns a view for New/Search/Edit
            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = fnDefaultResults();
                mylist = mylist.Where(x => x.bulkstatus == "RECD").ToList();
                return View(mylist);
            }
        }

        #region Search Related Actions
        [HttpPost]
        public ActionResult SearchBulkContainerUserCriteria(FormCollection fc)
        {
            // Reads posted form inputs and builds a custom qry, then returns a partial for grid
            System.Threading.Thread.Sleep(1000);
            ViewBag.SearchName = "User Criteria:";
            int rowcount = Convert.ToInt32(fc["rowcount"]);
            int clientid = Convert.ToInt32(fc["clientid"]);
            string mywarehouse = fc["warehouse"];

            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = new List<BulkContainerSearchResults>();
                mylist = (from t in db.tblBulk
                          join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                          let cname = (from c in db.tblClient where c.ClientID == t2.ClientID select c.ClientName).FirstOrDefault()
                          orderby t.ReceiveDate descending
                          where t2.ClientID == clientid
                          select new BulkContainerSearchResults
                          {
                              bulkid = t.BulkID,
                              warehouse = t.Warehouse,
                              receivedate = t.ReceiveDate,
                              carrier = t.Carrier,
                              productmasterid = t.ProductMasterID,
                              mastercode = t2.MasterCode,
                              mastername = t2.MasterName,
                              lotnumber = t.LotNumber,
                              bulkstatus = t.BulkStatus,
                              um = t.UM,
                              bin = t.Bin,
                              containertype = t.ContainerType,
                              clientname = cname,
                              expirationdate = t.ExpirationDate,
                              currentweight = t.CurrentWeight
                          }).Take(rowcount).ToList();

                if (mywarehouse != "")
                {
                    mylist = mylist.Where(x => x.warehouse == mywarehouse).ToList();
                }

                return PartialView("~/Views/Receiving/_BulkContainerSearchResults.cshtml", mylist);
            }
        }

        private List<BulkContainerSearchResults> fnDefaultResults()
        {
            // This method is the base qry for the Qucik Search links on the Landing page
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<BulkContainerSearchResults> mylist = new List<BulkContainerSearchResults>();
            mylist = (from t in db.tblBulk
                      join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                      let cname = (from c in db.tblClient where c.ClientID == t2.ClientID select c.ClientName).FirstOrDefault()
                      orderby t.ReceiveDate descending
                      select new BulkContainerSearchResults
                      {
                          bulkid = t.BulkID,
                          warehouse = t.Warehouse,
                          receivedate = t.ReceiveDate,
                          carrier = t.Carrier,
                          productmasterid = t.ProductMasterID,
                          mastercode = t2.MasterCode,
                          mastername = t2.MasterName,
                          lotnumber = t.LotNumber,
                          bulkstatus = t.BulkStatus,
                          um = t.UM,
                          bin = t.Bin,
                          containertype = t.ContainerType,
                          clientname = cname,
                          expirationdate = t.ExpirationDate,
                          currentweight = t.CurrentWeight
                      }).ToList();
            db.Dispose();

            return mylist;
        }

        [HttpGet]
        public ActionResult SearchInReceiving()
        {
            ViewBag.SearchName = "In Receiving:";
            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = fnDefaultResults();
                mylist = mylist.Where(x => x.bulkstatus == "RECD").ToList();
                return PartialView("~/Views/Receiving/_BulkContainerSearchResults.cshtml", mylist);
            }
        }

        [HttpGet]
        public ActionResult SearchNonAvailable()
        {
            ViewBag.SearchName = "Status = Not Available:";
            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = fnDefaultResults();
                mylist = mylist.Where(x => x.bulkstatus != "AVAIL").ToList();
                return PartialView("~/Views/Receiving/_BulkContainerSearchResults.cshtml", mylist);
            }
        }

        [HttpGet]
        public ActionResult SearchExpired()
        {
            ViewBag.SearchName = "Expired Containers:";
            using (var db = new EF.CMCSQL03Entities())
            {
                List<BulkContainerSearchResults> mylist = fnDefaultResults();
                mylist = mylist.Where(x => x.expirationdate <= DateTime.Now).ToList();
                return PartialView("~/Views/Receiving/_BulkContainerSearchResults.cshtml", mylist);
            }
        }
        #endregion

        [HttpGet]
        public ActionResult SetUpEdit(int id)
        {
            // Called with a parameter BulkID, build an obj and return edit view
            ViewBag.SearchName = null;
            BulkContainer obj = new BulkContainer();
            obj = fnFillBulkContainer(id);
            return View("~/Views/Receiving/BulkContainerEdit.cshtml", obj);
        }

        // *****************************************************************
        // Begin Bulk Container Edit Actions
        //
        public ActionResult SetUpReceiptKnown()
        {
            // Called from Landing page, build new BulkContainer obj and return the view for drill down, data entry
            BulkContainer obj = new BulkContainer();
            Session["ListOfBulkOrderItemsToClose"] = null;
            obj.isknownmaterial = true;
            obj.bulkid = -1;
            obj.clientid = 0;
            obj.productmasterid = 0;
            obj.ListOfWareHouses = fnWarehouseIDs();
            obj.ListOfProductMasters = fnProductMasterIDs(obj.clientid);
            obj.ListOfBulkStatusIDs = fnBulkStatusIDs();
            obj.ListOfContainerTypeIDs = fnContainerTypeIDs();
            return View("~/Views/Receiving/BulkReceipt.cshtml", obj);
        }

        public ActionResult SetUpReceiptUnKnown()
        {
            // Called from Landing page, build new BulkContainer obj and return the view
            BulkContainer obj = new BulkContainer();
            Session["ListOfBulkOrderItemsToClose"] = null;
            obj.isknownmaterial = false;
            obj.bulkid = -1;
            obj.clientid = 0;
            obj.productmasterid = 0;
            obj.ListOfWareHouses = fnWarehouseIDs();
            obj.ListOfProductMasters = fnProductMasterIDs(obj.clientid);
            obj.ListOfBulkStatusIDs = fnBulkStatusIDs();
            obj.ListOfContainerTypeIDs = fnContainerTypeIDs();
            return View("~/Views/Receiving/BulkReceiptUnKnown.cshtml", obj);
        }

        public string fnProductMasterDD(int clientid, string change)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var qry = (from t in db.tblProductMaster
                       where t.ClientID == clientid
                       orderby t.MasterCode, t.MasterName
                       select t);
            string s = "<select onchange='$onchange$()' name='$name$' id='$id$' class='$class$' style='$style$' >";
            s = s.Replace("$onchange$", change);
            s = s.Replace("$name$", "productmasterid");
            s = s.Replace("$id$", "productmasterid");
            s = s.Replace("$class$", "form-control");
            s = s.Replace("$style$", "width:60%;");
            //System.Diagnostics.Debug.WriteLine(s);
            s = s + "<option value='0' selected=true>Master Code</option>";
            if (qry.Count() > 0)
            {
                foreach (var item in qry)
                { s = s + "<option value=" + item.ProductMasterID.ToString() + ">" + item.MasterCode + " - " + item.MasterName + "</option>"; }
            }
            else
            { s = s + "<option value=0>No Products Found</option>"; }
            s = s + "</select>";

            db.Dispose();
            return s;
        }

        public ActionResult fnFillBulkOpenOrderItems(int id)
        {
            // called from bulkreceipt.cshtml onchange event of Master code DD to build a view and LOAD into a div
            System.Threading.Thread.Sleep(1500);
            List<OpenBulkOrderItems> mylist = new List<OpenBulkOrderItems>();
            using (var db = new EF.CMCSQL03Entities())
            {
                mylist = (from t in db.tblBulkOrderItem
                          join t2 in db.tblBulkOrder on t.BulkOrderID equals t2.BulkOrderID
                          where t.ProductMasterID == id
                          where t.Status == "OP"
                          select
                          new OpenBulkOrderItems
                          {
                              bulkorderitemid = t.BulkOrderItemID,
                              bulkorderid = t.BulkOrderID,
                              productmasterid = t.ProductMasterID,
                              weight = t.Weight,
                              status = t.Status,
                              eta = t.ETA,
                              supplyid = t.SupplyID,
                              itemnotes = t.ItemNotes,
                              orderdate = t2.OrderDate
                          }).ToList();
            }
            System.Diagnostics.Debug.WriteLine("Row Count= " + mylist.Count().ToString());
            if (mylist.Count() > 0)
            { return PartialView("~/Views/Receiving/_BulkReceiptOpenOrderItems.cshtml", mylist); }
            else
            { return Content("&nbsp;&nbsp;No open orders found at  " + DateTime.Now.ToString()); }
        }

        //
        // Begin Support Actions for this controller
        //
        private string fnClientName(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblClient where t.ClientID == id select t).FirstOrDefault();
                return qry.ClientName;
            }
        }

        [HttpPost]
        public ActionResult SavePostData(BulkContainer incoming)
        {
            string messageroot = "";

            if (incoming.bulkid == -1)
            { messageroot = "New Container created at "; }
            else
            { messageroot = "Container updated at "; }

            // 

            if (fnSaveBulkContainer(incoming) == true)
            {
                return Content(messageroot + DateTime.Now.ToString());
            }
            else
            {
                return Content("<font color=red>Error updating database at " + DateTime.Now.ToString() + "</font>");
            }
        }

        [HttpPost]
        public ActionResult SavePostDataUnKnownMaterial(BulkContainer incoming)
        {
            string messageroot = "";
            if (incoming.bulkid == -1)
            { messageroot = "New Container created at "; }
            else
            { messageroot = "Container updated at "; }

            if (fnSaveBulkContainerUnKnownMaterial(incoming) == true)
            {
                return Content(messageroot + DateTime.Now.ToString());
            }
            else
            {
                return Content("<font color=red>Error updating database at " + DateTime.Now.ToString() + "</font>");
            }
        }

        public void BuildCloseList(int id, bool ischecked)
        {
            // persist the item to be closed in the row
            string s = "";
            System.Diagnostics.Debug.WriteLine(s);
            using (var db = new EF.CMCSQL03Entities())
            {
                s = "Update tblBulkOrderItem set ToBeClosed=0 where BulkOrderItemID=" + id;
                db.Database.ExecuteSqlCommand(s);
            }

            if (ischecked == true)
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    s = "Update tblBulkOrderItem set ToBeClosed=1 where BulkOrderItemID=" + id;
                    db.Database.ExecuteSqlCommand(s);
                }
            }
        }

        private BulkContainer fnFillBulkContainer(int id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            var qry = (from t in db.tblBulk
                       join t2 in db.tblProductMaster on t.ProductMasterID equals t2.ProductMasterID
                       let cname = (from c in db.tblClient where c.ClientID == t2.ClientID select c.ClientName).FirstOrDefault()
                       where t.BulkID == id
                       select new BulkContainer
                       {
                           clientid = t2.ClientID,
                           clientname = cname,
                           bulkid = t.BulkID,
                           warehouse = t.Warehouse,
                           receivedate = t.ReceiveDate,
                           carrier = t.Carrier,
                           receivedby = t.ReceivedBy,
                           enteredby = t.EnteredBy,

                           productmasterid = t.ProductMasterID,
                           receiveweight = t.ReceiveWeight,

                           lotnumber = t.LotNumber,
                           mfgdate = t.MfgDate,
                           expirationdate = t.ExpirationDate,
                           ceaseshipdate = t.CeaseShipDate,
                           bulkstatus = t.BulkStatus,

                           um = t.UM,
                           containercolor = t.ContainerColor,
                           bin = t.Bin,
                           containertype = t.ContainerType,

                           //flammable=t.flammable, freezer=t.freezer, refrigerator = t.refrigerator, restrictedqty = t.RestrictedQty,
                           //otherstorage=t.OtherStorage, restrictedamt=t.RestrictedAmt, packout=t.PackOut,

                           coaincluded = t.COAIncluded,
                           msdsincluded = t.MSDSIncluded,

                           //percentsolids=t.PercentSolids,nco=t.NCO, tew=t.TEW, percentoh=t.PercentOH,

                           containernotes = t.ContainerNotes,
                           currentweight = t.CurrentWeight,
                           qcdate = t.QCDate,
                           returnlocation = t.ReturnLocation,
                           noticedate = t.NoticeDate,
                           bulklabelnote = t.BulkLabelNote,
                           receivedascode = t.ReceivedAsCode,
                           receivedasname = t.ReceivedAsName

                       }).FirstOrDefault();
            // Have to fill list items after object is built  

            qry.ListOfWareHouses = fnWarehouseIDs();
            qry.ListOfProductMasters = fnProductMasterIDs(qry.clientid);
            qry.ListOfBulkStatusIDs = fnBulkStatusIDs();
            qry.ListOfContainerTypeIDs = fnContainerTypeIDs();
            db.Dispose();
            return qry;
        }

        private bool fnSaveBulkContainer(BulkContainer incoming)
        {
            bool retval = true;
            try
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    int pk = incoming.bulkid;
                    if (incoming.bulkid == -1)
                    {
                        //insert a record then edit it
                        var newknownrecord = new EF.tblBulk { ProductMasterID = incoming.productmasterid, IsKnownMaterial = incoming.isknownmaterial };
                        db.tblBulk.Add(newknownrecord);
                        db.SaveChanges(); pk = newknownrecord.BulkID;
                    }

                    var qry = (from t in db.tblBulk where t.BulkID == pk select t).FirstOrDefault();
                    qry.Warehouse = incoming.warehouse; qry.ReceiveDate = incoming.receivedate; qry.Carrier = incoming.carrier;
                    qry.ReceivedBy = incoming.receivedby; qry.EnteredBy = incoming.enteredby;

                    qry.ProductMasterID = incoming.productmasterid; qry.ReceiveWeight = incoming.receiveweight;

                    qry.LotNumber = incoming.lotnumber; qry.MfgDate = incoming.mfgdate; qry.ExpirationDate = incoming.expirationdate;
                    qry.CeaseShipDate = incoming.ceaseshipdate; qry.BulkStatus = incoming.bulkstatus;

                    qry.UM = incoming.um; qry.ContainerColor = incoming.containercolor; qry.Bin = incoming.bin;
                    qry.ContainerType = incoming.containertype;

                    //qry.flammable=incoming.flammable; qry.freezer=incoming.freezer;  qry.refrigerator=incoming.refrigerator;
                    //qry.OtherStorage=incoming.otherstorage; qry.RestrictedQty=incoming.restrictedqty; qry.RestrictedAmt=incoming.restrictedamt; qry.PackOut=incoming.packout;        

                    qry.COAIncluded = incoming.coaincluded; qry.MSDSIncluded = incoming.msdsincluded;

                    //qry.PercentSolids=incoming.percentsolids;qry.NCO=incoming.nco; qry.TEW = incoming.tew; qry.PercentOH = incoming.percentoh;

                    qry.ContainerNotes = incoming.containernotes; qry.CurrentWeight = incoming.currentweight; qry.QCDate = incoming.qcdate; qry.ReturnLocation = qry.ReturnLocation;
                    qry.NoticeDate = incoming.noticedate; qry.BulkLabelNote = incoming.bulklabelnote; qry.ReceivedAsCode = incoming.receivedascode; qry.ReceivedAsName = incoming.receivedasname;

                    // force an error due to bad data
                    //qry.NoticeDate = Convert.ToDateTime("DD");

                    db.SaveChanges();

                    // Close items tagged to be closed for this productmasterid (would be better to pass a comma delimited list of PKs)
                    db.Database.ExecuteSqlCommand("Update tblBulkOrderItem set Status='CL' where ToBeClosed=1 and productmasterid=" + incoming.productmasterid);
                    db.Database.ExecuteSqlCommand("Update tblBulkOrderItem set ToBeClosed=null where Status='CL' and ToBeClosed=1 and productmasterid=" + incoming.productmasterid);
                    retval = true;
                }

            }
            catch
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                retval = false;
            }
            return retval;
        }

        private bool fnSaveBulkContainerUnKnownMaterial(BulkContainer incoming)
        {
            // Still using tblBulk / After creation of tblBulkUnKnown change table reference
            // This is an INSERT only routine
            bool retval = true;
            try
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    var newitem = new EF.tblBulk
                    {
                        IsKnownMaterial = incoming.isknownmaterial,
                        ProductMasterID = incoming.productmasterid,
                        Warehouse = incoming.warehouse,
                        ReceiveDate = incoming.receivedate,
                        Carrier = incoming.carrier,
                        ReceivedBy = incoming.receivedby,
                        EnteredBy = incoming.enteredby,
                        ReceiveWeight = incoming.receiveweight,
                        LotNumber = incoming.lotnumber,
                        MfgDate = incoming.mfgdate,
                        ExpirationDate = incoming.expirationdate,
                        CeaseShipDate = incoming.ceaseshipdate,
                        BulkStatus = incoming.bulkstatus,
                        UM = incoming.um,
                        ContainerColor = incoming.containercolor,
                        Bin = incoming.bin,
                        ContainerType = incoming.containertype,
                        COAIncluded = incoming.coaincluded,
                        MSDSIncluded = incoming.msdsincluded,
                        ContainerNotes = incoming.containernotes,
                        CurrentWeight = incoming.currentweight,
                        QCDate = incoming.qcdate,
                        ReturnLocation = incoming.returnlocation,
                        NoticeDate = incoming.noticedate,
                        BulkLabelNote = incoming.bulklabelnote,
                        ReceivedAsCode = incoming.receivedascode,
                        ReceivedAsName = incoming.receivedasname
                    };
                    db.tblBulk.Add(newitem);
                    db.SaveChanges();
                    retval = true;
                }
            }
            catch
            {
                retval = false;
            }
            return retval;
        }

        #region DDLs for Views

        private static List<SelectListItem> fnContainerTypeIDs()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist.Add(new SelectListItem { Value = "", Text = "" });
            mylist.Add(new SelectListItem { Value = "S", Text = "Steel" });
            mylist.Add(new SelectListItem { Value = "P", Text = "Plastic" });
            mylist.Add(new SelectListItem { Value = "F", Text = "Fiber" });
            mylist.Add(new SelectListItem { Value = "O", Text = "Other" });

            return mylist;
        }

        private static List<SelectListItem> fnBulkStatusIDs()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblBulk
                      orderby t.BulkStatus
                      select new SelectListItem { Value = t.BulkStatus, Text = t.BulkStatus }).Distinct().ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

            return mylist;
        }

        private static List<SelectListItem> fnProductMasterIDs(int? id)
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist = (from t in db.tblProductMaster
                      where t.ClientID == id
                      orderby t.MasterCode, t.MasterName
                      select new SelectListItem { Value = t.ProductMasterID.ToString(), Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25) }).ToList();
            mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

            return mylist;
        }

        private static List<SelectListItem> fnWarehouseIDs()
        {
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist.Add(new SelectListItem { Value = null, Text = "" });
            mylist.Add(new SelectListItem { Value = "AP", Text = "AP" });
            mylist.Add(new SelectListItem { Value = "CT", Text = "CT" });
            mylist.Add(new SelectListItem { Value = "CO", Text = "CO" });
            mylist.Add(new SelectListItem { Value = "EU", Text = "EU" });

            return mylist;
        }

        #endregion

    }
}
