using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{
    public class ReceivingService
    {
        public static List<BulkContainerViewModel> fnIndexList()
        {
            // List for the Index View
            using (var db = new EF.CMCSQL03Entities())
            {
                var obj = (from t in db.tblBulk
                           join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                           join cn in db.tblClient on pm.ClientID equals cn.ClientID
                           where t.BulkStatus == "RECD"
                           orderby t.BulkID descending
                           select new BulkContainerViewModel
                           {
                               clientid = cn.ClientID,
                               clientname = cn.ClientName,
                               bulkid = t.BulkID,
                               warehouse = t.Warehouse,
                               receivedate = t.ReceiveDate,
                               carrier = t.Carrier,
                               MasterName = pm.MasterName,
                               MasterCode = pm.MasterCode,
                               receiveweight = t.ReceiveWeight,
                               lotnumber = t.LotNumber,
                               expirationdate = t.ExpirationDate,
                               bulkstatus = t.BulkStatus,
                               um = t.UM
                           }).ToList();
                return obj;
            }
        }

        public static string fnBuildProductMasterDropDown(int clientid)
        {
            // This returns ONLY the <option> portion of the <select> tag, thus allowing the <select> tag to
            // be propering decorated with onchange= etc..
            using (var db = new CMCSQL03Entities())
            {
                var qry = (from t in db.tblProductMaster where t.ClientID == clientid orderby t.MasterCode, t.MasterName select t);
                string s = "<option value='0' selected=true>Select Master Code</option>";
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

        public static string fnBuildProductCodeDropDown(int clientid)
        {
            // This returns ONLY the <option> portion of the <select> tag, thus allowing the <select> tag to
            // be propering decorated with onchange= etc..
            using (var db = new CMCSQL03Entities())
            {
                var qry = (from t in db.tblProductDetail
                           join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                           where pm.ClientID == clientid
                           orderby t.ProductCode
                           select t);
                string s = "<option value='0' selected=true>Select Product Code</option>";
                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    { s = s + "<option value=" + item.ProductDetailID.ToString() + ">" + item.ProductCode + " - " + item.ProductName + "</option>"; }
                }
                else
                { s = s + "<option value=0>No Products Found</option>"; }
                s = s + "</select>";
                return s;
            }
        }

        public static string fnAlertMessage(string message)
        {
            var h2 = new System.Web.HtmlString("<span class='alert alert-success'><a class='close' data-dismiss='alert'>&times;</a><strong style='width:12px'>" + message + "</strong></span>");
            return h2.ToHtmlString();
        }

        public static List<OpenBulkOrderItems> fnOpenBulkOrderItems(int id)
        {
            // reset the flag on all order items first
            using (var db = new EF.CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("Update tblBulkOrderItem set ToBeClosed=null where ProductMasterID=" + id);
            }

            // build a list of Open Bulk Order Items for the Partial View
            List<OpenBulkOrderItems> mylist = new List<OpenBulkOrderItems>();
            using (var db = new EF.CMCSQL03Entities())
            {
                mylist = (from t in db.tblBulkOrderItem
                          join t2 in db.tblBulkOrder on t.BulkOrderID equals t2.BulkOrderID
                          where t.ProductMasterID == id
                          && t.Status == "OP"
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
                              orderdate = t2.OrderDate,
                              ToBeClosed = t.ToBeClosed
                          }).ToList();
                return mylist;
            }
        }

        public static void fnTagItemToBeClosed(int id, bool ischecked)
        {
            // update the ToBeClosed flag on the bulk order item record
            string s = "";
            using (var db = new CMCSQL03Entities())
            {
                // clear it always
                s = "Update tblBulkOrderItem set ToBeClosed=0 where BulkOrderItemID=" + id;
                db.Database.ExecuteSqlCommand(s);
            }
            if (ischecked == true)
            {
                using (var db = new CMCSQL03Entities())
                {
                    // tag it
                    s = "Update tblBulkOrderItem set ToBeClosed=1 where BulkOrderItemID=" + id;
                    db.Database.ExecuteSqlCommand(s);
                }
            }
        }

        // ************** TODO ***************************

        // Finish adding all properties to next 2

        // ************** TODO ***************************

        public static BulkContainerViewModel fnNewBulkContainer(int id)
        {
            // id=productmasterid
            BulkContainerViewModel obj = new BulkContainerViewModel();
            using (var db = new CMCSQL03Entities())
            {
                obj.isknownmaterial = true;

                var x = (from t in db.tblBulk where t.ProductMasterID == id select t).ToList();
                obj.pm_sumofcurrentweight = 0;
                foreach (var row in x)
                {
                    obj.pm_sumofcurrentweight = obj.pm_sumofcurrentweight + row.CurrentWeight;
                }

                var dbPM = db.tblProductMaster.Find(id);
                // assign ProductMaster fields for R/O
                obj.pm_MasterNotes = dbPM.MasterNotes;
                obj.pm_HandlingOther = dbPM.HandlingOther;
                obj.pm_OtherHandlingInstr = dbPM.OtherHandlingInstr;
                obj.pm_refrigerate = dbPM.Refrigerate;
                obj.pm_flammablestorageroom = dbPM.FlammableStorageRoom;
                obj.pm_freezablelist = dbPM.FreezableList;
                obj.pm_refrigeratedlist = dbPM.RefrigeratedList;
				
				if (!String.IsNullOrEmpty(dbPM.AlertNotesReceiving))
                {
                    obj.pm_alertnotesreceiving = dbPM.AlertNotesReceiving;
                }
                else
                { 
                    obj.pm_alertnotesreceiving = "No receiving alert for this product";
                } 

                var dbClient = db.tblClient.Find(dbPM.ClientID);

                obj.bulkid = -1;    // for insert later
                obj.clientid = dbClient.ClientID;
                obj.warehouse = dbClient.CMCLocation;
                obj.clientname = dbClient.ClientName;
                obj.productmasterid = id;
                obj.receivedate = DateTime.Now;
                obj.bulkstatus = "RECD";

                obj.ListOfWareHouses = fnWarehouseIDs();
                obj.ListOfProductMasters = fnProductMasterIDs(obj.clientid, id);
                obj.ListOfBulkStatusIDs = fnBulkStatusIDs();
                obj.ListOfContainerTypeIDs = fnContainerTypeIDs();
                obj.ListOfCarriers = fnCarriers();

                // R/O fields from PM
                obj.MasterCode = dbPM.MasterCode;
                obj.MasterName = dbPM.MasterName;
                obj.flammable = dbPM.Flammable;
                obj.freezer = dbPM.FREEZERSTORAGE;
                obj.refrigerated = dbPM.Refrigerate;
                obj.packout = dbPM.PackOutOnReceipt;

                return obj;
            }
        }

        public static BulkContainerViewModel fnNewBulkContainerUnKnown()
        {
            // id=productmasterid
            BulkContainerViewModel obj = new BulkContainerViewModel();
            using (var db = new CMCSQL03Entities())
            {
                obj.isknownmaterial = false;
                obj.productmasterid = null;
                obj.bulkid = -1;    // for insert later
                obj.clientid = null;
                obj.clientname = null;
                obj.productmasterid = null;
                obj.receivedate = DateTime.Now;
                obj.bulkstatus = "RECD";
                obj.MasterCode = null;
                obj.MasterName = null;
                obj.ListOfWareHouses = fnWarehouseIDs();
                obj.ListOfProductMasters = fnProductMasterIDs(obj.clientid, null);
                obj.ListOfBulkStatusIDs = fnBulkStatusIDs();
                obj.ListOfContainerTypeIDs = fnContainerTypeIDs();
                obj.ListOfCarriers = fnCarriers();
                return obj;
            }
        }

        public static BulkContainerViewModel fnFillBulkContainerFromDB(int id)
        {
            // id=bulkid
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblBulk
                           where t.BulkID == id
                           select new BulkContainerViewModel
                               {
                                   pm_sumofcurrentweight = (from x in db.tblBulk where x.ProductMasterID == t.ProductMasterID select x.CurrentWeight).Sum(),
                                   isknownmaterial = true,
                                   bulkid = t.BulkID,
                                   productmasterid = t.ProductMasterID,
                                   warehouse = t.Warehouse,
                                   receivedate = t.ReceiveDate,
                                   carrier = t.Carrier,
                                   receivedby = t.ReceivedBy,
                                   enteredby = t.EnteredBy,
                                   receiveweight = t.ReceiveWeight,
                                   lotnumber = t.LotNumber,
                                   mfgdate = t.MfgDate,
                                   expirationdate = t.ExpirationDate,
                                   ceaseshipdate = t.CeaseShipDate,
                                   bulkstatus = t.BulkStatus,
                                   qty = "1",
                                   um = t.UM,
                                   containercolor = t.ContainerColor,
                                   bin = t.Bin,
                                   containertype = t.ContainerType,
                                   coaincluded = t.COAIncluded,
                                   msdsincluded = t.MSDSIncluded,
                                   currentweight = t.CurrentWeight,
                                   qcdate = t.QCDate,
                                   returnlocation = t.ReturnLocation,
                                   noticedate = t.NoticeDate,
                                   bulklabelnote = t.BulkLabelNote,
                                   receivedascode = t.ReceivedAsCode,
                                   receivedasname = t.ReceivedAsName,
                                   containernotes = t.ContainerNotes,
                                   otherstorage = t.OtherStorage
                               }).FirstOrDefault();

                var qPM = (from t in db.tblProductMaster where t.ProductMasterID == obj.productmasterid select t).FirstOrDefault();
                var qCL = (from t in db.tblClient where t.ClientID == qPM.ClientID select t).FirstOrDefault();
                obj.clientid = qPM.ClientID;
                obj.clientname = qCL.ClientName;
                obj.ListOfWareHouses = fnWarehouseIDs();
                obj.ListOfProductMasters = fnProductMasterIDs(qPM.ClientID, qPM.ProductMasterID);
                obj.ListOfBulkStatusIDs = fnBulkStatusIDs();
                obj.ListOfContainerTypeIDs = fnContainerTypeIDs();
                obj.ListOfCarriers = fnCarriers();

                obj.MasterCode = qPM.MasterCode;
                obj.MasterName = qPM.MasterName;
                obj.flammable = qPM.Flammable;
                obj.freezer = qPM.FREEZERSTORAGE;
                obj.refrigerated = qPM.Refrigerate;
                obj.packout = qPM.PackOutOnReceipt;

                return obj;
            }
        }

        public static PrePackViewModel fnNewBulkContainerForPrePack(int clientid, int productdetailid)
        {
            PrePackViewModel obj = new PrePackViewModel();
            using (var db = new CMCSQL03Entities())
            {
                var dbClient = db.tblClient.Find(clientid);
                var dbProductDetail = db.tblProductDetail.Find(productdetailid);

                obj.productmasterid = dbProductDetail.ProductMasterID;
                obj.isknownmaterial = true;
                obj.clientid = clientid;
                obj.clientname = dbClient.ClientName;
                obj.bulkid = -1;    // for insert later
                obj.receivedate = DateTime.Now;
                obj.carrier = null;
                obj.ListOfCarriers = fnCarriers();
                obj.warehouse = null;
                obj.ListOfWareHouses = fnWarehouseIDs();
                obj.enteredby = null;
                obj.lotnumber = null;
                obj.receivedby = null;
                obj.mfgdate = null;
                obj.expirationdate = null;
                obj.ceaseshipdate = null;
                obj.qcdate = null;
                obj.msdsincluded = null;
                obj.coaincluded = null;
                obj.productcode = dbProductDetail.ProductCode;
                obj.productname = dbProductDetail.ProductName;

                obj.ListOfShelfMasters = (from t in db.tblShelfMaster
                                          orderby t.ShelfID
                                          where t.ProductDetailID == productdetailid
                                          select new ItemForPrePackViewModel
                                          {
                                              shelfid = t.ShelfID,
                                              size = t.Size,
                                              bin = t.Bin
                                          }).ToList();

                obj.ItemsCount = obj.ListOfShelfMasters.Count();

                return obj;
            }
        }

        public static List<SelectListItem> fnCarriers()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblCarrier
                          orderby t.CarrierName
                          select new SelectListItem
                          {
                              Value = t.CarrierName,
                              Text = t.CarrierName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "", Text = "Select Carrier" });

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
                          select new SelectListItem
                          {
                              Value = t.ClientID.ToString(),
                              Text = t.ClientName
                          }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });

                return mylist;
            }
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

        public static List<SelectListItem> fnProductMasterIDs(int? id, int? PmID = null)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                if (PmID == null)
                {
                    mylist = (from t in db.tblProductMaster
                              where t.ClientID == id
                              orderby t.MasterCode, t.MasterName
                              select new SelectListItem
                              {
                                  Value = t.ProductMasterID.ToString(),
                                  Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25)
                              }).ToList();

                    mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });
                }
                else
                {
                    mylist = (from t in db.tblProductMaster
                              where t.ClientID == id && t.ProductMasterID == PmID
                              orderby t.MasterCode, t.MasterName
                              select new SelectListItem
                              {
                                  Value = t.ProductMasterID.ToString(),
                                  Text = t.MasterCode + " - " + t.MasterName.Substring(0, 25)
                              }).ToList();
                }

                return mylist;
            }
        }

        public static List<SelectListItem> fnBulkStatusIDs()
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblBulk
                          orderby t.BulkStatus
                          select new SelectListItem
                          {
                              Value = t.BulkStatus,
                              Text = t.BulkStatus
                          }).Distinct().ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        public static List<SelectListItem> fnContainerTypeIDs()
        {
            List<SelectListItem> mylist = new List<SelectListItem>();
            mylist.Add(new SelectListItem { Value = "", Text = "" });
            mylist.Add(new SelectListItem { Value = "S", Text = "Steel" });
            mylist.Add(new SelectListItem { Value = "P", Text = "Plastic" });
            mylist.Add(new SelectListItem { Value = "F", Text = "Fiber" });
            mylist.Add(new SelectListItem { Value = "O", Text = "Other" });
            return mylist;
        }

        public static bool fnSavePrePack(PrePackViewModel vm, FormCollection fc)
        {
            bool retval = true;
            try
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    var newbulk = new EF.tblBulk();

                    newbulk.ProductMasterID = vm.productmasterid;
                    newbulk.ReceiveDate = vm.receivedate;
                    newbulk.LotNumber = vm.lotnumber;
                    newbulk.CeaseShipDate = vm.ceaseshipdate;
                    newbulk.Carrier = vm.carrier;
                    newbulk.ReceivedBy = vm.receivedby;
                    newbulk.QCDate = vm.qcdate;
                    newbulk.Warehouse = vm.warehouse;
                    newbulk.MfgDate = vm.mfgdate;
                    newbulk.COAIncluded = vm.coaincluded;
                    newbulk.EnteredBy = vm.enteredby;
                    newbulk.ExpirationDate = vm.expirationdate;
                    newbulk.MSDSIncluded = vm.msdsincluded;
                    newbulk.BulkStatus = "PP";

                    db.tblBulk.Add(newbulk);
                    db.SaveChanges();

                    int newBulkID = newbulk.BulkID;

                    for (int i = 1; i <= vm.ItemsCount; i++)
                    {
                        string sThisShelfID = fc["Key" + i.ToString()];
                        int ThisShelfID = Convert.ToInt32(sThisShelfID);

                        string sThisQty = fc["Value" + i.ToString()];
                        int ThisQty = Convert.ToInt32(sThisQty);

                        var newstock = new EF.tblStock();

                        newstock.BulkID = newBulkID; 
                        newstock.ShelfID = ThisShelfID; 
                        newstock.CreateDate = DateTime.Now;
                        newstock.Warehouse = vm.warehouse; 
                        newstock.QtyOnHand = ThisQty;
                        newstock.ShelfStatus = "PP";

                        db.tblStock.Add(newstock);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                retval = false;
                throw new Exception("Error occurred saving Pre Packs");
            }

            return retval;
        }

        public static bool fnSaveBulkContainerKnown(BulkContainerViewModel incoming)
        {
            bool retval = true;

            try
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    int pk = incoming.bulkid;
                    if (incoming.bulkid == -1)
                    {
                        var newrec = new EF.tblBulk 
                        { 
                            ProductMasterID = incoming.productmasterid 
                        };

                        newrec.CreateDate = System.DateTime.Now;
                        newrec.CreateUser = HttpContext.Current.User.Identity.Name;
                        
                        db.tblBulk.Add(newrec);
                        db.SaveChanges();

                        pk = newrec.BulkID;
                    }

                    var qry = (from t in db.tblBulk 
                               where t.BulkID == pk 
                               select t).FirstOrDefault();
                    
                    qry.Warehouse = incoming.warehouse;
                    qry.ReceiveDate = incoming.receivedate;
                    qry.Carrier = incoming.carrier;
                    qry.ReceivedBy = incoming.receivedby;
                    qry.EnteredBy = incoming.enteredby;
                    qry.ProductMasterID = incoming.productmasterid;
                    qry.ReceiveWeight = incoming.receiveweight;
                    qry.LotNumber = incoming.lotnumber;
                    qry.MfgDate = incoming.mfgdate;
                    qry.ExpirationDate = incoming.expirationdate;
                    qry.CeaseShipDate = incoming.ceaseshipdate;
                    qry.BulkStatus = incoming.bulkstatus;
                    qry.UM = incoming.um;
                    qry.ContainerColor = incoming.containercolor;
                    qry.Bin = incoming.bin;
                    qry.ContainerType = incoming.containertype;
                    qry.COAIncluded = incoming.coaincluded;
                    qry.MSDSIncluded = incoming.msdsincluded;
                    qry.ContainerNotes = incoming.containernotes;
                    qry.CurrentWeight = incoming.receiveweight;
                    qry.QCDate = incoming.qcdate;
                    qry.ReturnLocation = incoming.returnlocation;
                    qry.NoticeDate = incoming.noticedate;
                    qry.BulkLabelNote = incoming.bulklabelnote;
                    qry.ReceivedAsCode = incoming.receivedascode;
                    qry.ReceivedAsName = incoming.receivedasname;
                    qry.OtherStorage = incoming.otherstorage;
                    qry.UpdateDate = System.DateTime.Now;
                    qry.UpdateUser = HttpContext.Current.User.Identity.Name;
                    
                    db.SaveChanges();

                    // Close items tagged to be closed for this productmasterid (would be better to pass a comma delimited list of PKs)
                    db.Database.ExecuteSqlCommand("Update tblBulkOrderItem set Status='CL' where ToBeClosed=1 and productmasterid=" + incoming.productmasterid);
                    
                    retval = true;
                }
            }
            catch
            {
                retval = false;
                throw new Exception("Error occurred saving Bulk Container");
            }
            return retval;
        }

        public static bool fnSaveBulkContainerUnKnown(BulkContainerViewModel incoming)
        {
            // creation of tblBulkUnKnown // This is an INSERT only routine
            bool retval = true;
            try
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    var newitem = new EF.tblBulkUnKnown
                    {
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