using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//pc add
using System.Web.Mvc;
using MvcPhoenix.EF;
using MvcPhoenix.Models;


namespace MvcPhoenix.Services
{
    public class ReceivingService
    {

        // Return a class for use by the Partial inside of Index.cshtml
        // Can be expanded later for more Receiving-Only Quick Searches
        public static List<BulkContainerSearchResults> fnReceivingDefaultSearchResults()
        {
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
                    mylist = mylist.Where(x => x.bulkstatus == "RECD").ToList();
                db.Dispose();

            return mylist;
        }

        public static List<OpenBulkOrderItems> fnOpenBulkOrderItems(int id)
        {
            // build a list of Open Bulk Order Items for the Partial View
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
                return mylist;
            }
        }

    public static List<SelectListItem> fnClientIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblClient
                          orderby t.ClientName
                          select new SelectListItem { Value = t.ClientID.ToString(), Text = t.ClientName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });
                return mylist;
            }
        }

        public static string fnClientName(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblClient where t.ClientID == id select t).FirstOrDefault();
                return qry.ClientName;
            }
        }
        

        public static string fnProductCodesDropDown(int clientid, string change)
    {
            //MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            using (var db = new CMCSQL03Entities())
            {
                var qry = (from t in db.tblProductMaster
                    where t.ClientID == clientid orderby t.MasterCode, t.MasterName select t);
                string s = "<label>Master Code</label><select onchange='$onchange$()' name='$name$' id='$id$' class='form-control'>";
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
                return s;
            }

    }


        public static BulkContainer fnFillOtherBulkContainerProps(BulkContainer BC)
        {

            BC.ListOfWareHouses = fnWarehouseIDs();
            BC.ListOfProductMasters = fnProductMasterIDs(BC.clientid);
            BC.ListOfBulkStatusIDs = fnBulkStatusIDs();
            BC.ListOfContainerTypeIDs = fnContainerTypeIDs();
            BC.ListOfClients = fnClientIDs();
            // ClientName?????
            return BC;
        }

        private static List<SelectListItem> fnWarehouseIDs()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist.Add(new SelectListItem { Value = null, Text = "" });
            mylist.Add(new SelectListItem { Value = "AP", Text = "AP" });
            mylist.Add(new SelectListItem { Value = "CT", Text = "CT" });
            mylist.Add(new SelectListItem { Value = "CO", Text = "CO" });
            mylist.Add(new SelectListItem { Value = "EU", Text = "EU" });
            return mylist;
        }

        private static List<SelectListItem> fnProductMasterIDs(int? id)
        {
            using (var db=new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblProductMaster
                          where t.ClientID == id
                          orderby t.MasterCode, t.MasterName
                          select new SelectListItem { Value = t.ProductMasterID.ToString(), Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25) }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
                return mylist;
            }
        }



        private static List<SelectListItem> fnBulkStatusIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblBulk
                          orderby t.BulkStatus
                          select new SelectListItem { Value = t.BulkStatus, Text = t.BulkStatus }).Distinct().ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
                return mylist;
            }

        }

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



        public static bool fnSaveBulkContainer(BulkContainer incoming)
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
                        //var newknownrecord = new EF.tblBulk { ProductMasterID = incoming.productmasterid, IsKnownMaterial = incoming.isknownmaterial };
                        var newknownrecord = new EF.tblBulk { ProductMasterID = incoming.productmasterid };
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
                    qry.COAIncluded = incoming.coaincluded; qry.MSDSIncluded = incoming.msdsincluded;
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


        public static bool fnSaveBulkContainerUnKnownMaterial(BulkContainer incoming)
        {
            // creation of tblBulkUnKnown // This is an INSERT only routine
            bool retval = true;
            try
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    var newitem = new EF.tblBulkUnKnown
                    {

                        //IsKnownMaterial = incoming.isknownmaterial,
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
                    db.tblBulkUnKnown.Add(newitem);
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

    }
}