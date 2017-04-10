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

        public static List<OpenBulkOrderItems> fnOpenBulkOrderItems(int id)
        {
            // reset the flag on all order items first
            using (var db = new EF.CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("UPDATE tblBulkOrderItem SET ToBeClosed=null WHERE ProductMasterID=" + id);
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
                s = "UPDATE tblBulkOrderItem SET ToBeClosed=0 WHERE BulkOrderItemID=" + id;
                db.Database.ExecuteSqlCommand(s);
            }
            if (ischecked == true)
            {
                using (var db = new CMCSQL03Entities())
                {
                    // tag it
                    s = "UPDATE tblBulkOrderItem SET ToBeClosed=1 WHERE BulkOrderItemID=" + id;
                    db.Database.ExecuteSqlCommand(s);
                }
            }
        }

        #region Known Bulk Receiving Methods

        public static BulkContainerViewModel fnNewBulkContainer(int productmasterid, int? productdetailid)
        {
            // id=productmasterid
            BulkContainerViewModel obj = new BulkContainerViewModel();
            using (var db = new CMCSQL03Entities())
            {
                obj.isknownmaterial = true;
                obj.pm_sumofcurrentweight = 0;

                var x = (from t in db.tblBulk
                         where t.ProductMasterID == productmasterid
                         select t).ToList();

                foreach (var row in x)
                {
                    obj.pm_sumofcurrentweight = obj.pm_sumofcurrentweight + row.CurrentWeight;
                }

                // R/O fields from PM
                var qPM = db.tblProductMaster.Find(productmasterid);
                obj.MasterCode = qPM.MasterCode;
                obj.MasterName = qPM.MasterName;

                obj.pm_alertnotesreceiving = qPM.AlertNotesReceiving;
                obj.pm_OtherHandlingInstr = qPM.OtherHandlingInstr;
                obj.pm_refrigerate = qPM.Refrigerate;
                obj.pm_flammablestorageroom = qPM.FlammableStorageRoom;
                obj.pm_freezerstorage = qPM.FREEZERSTORAGE;
                obj.pm_otherstorage = qPM.OtherStorage;
                obj.pm_cleanroomgmp = qPM.CleanRoomGMP;
                obj.pm_alertnotesreceiving = qPM.AlertNotesReceiving;
                obj.pm_restrictedtoamount = qPM.RestrictedToAmount;
                obj.pm_tempraturecontrolledstorage = qPM.TemperatureControlledStorage;
                obj.pm_shelflife = qPM.ShlfLife;
                obj.pm_packoutonreceipt = qPM.PackOutOnReceipt;
                obj.pm_ceaseshipdifferential = qPM.CeaseShipDifferential;

                // R/O from related PD
                var qPD = (from t in db.tblProductDetail
                           where t.ProductMasterID == obj.productmasterid
                           select t).FirstOrDefault();

                obj.pd_groundunnum = qPD.GRNUNNUMBER;
                obj.pd_groundpackinggrp = qPD.GRNPKGRP;
                obj.pd_groundhazardclass = qPD.GRNHAZCL;
                obj.pd_groundhazardsubclass = qPD.GRNHAZSUBCL;
                obj.pd_epabiocide = qPD.EPABiocide;

                try
                {
                    var qGHS = (from t in db.tblGHS
                                join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                                where pd.ProductMasterID == obj.productmasterid
                                select t).FirstOrDefault();

                    obj.ghs_signalword = qGHS.SignalWord;
                    obj.ghs_symbol1 = qGHS.Symbol1;
                    obj.ghs_symbol2 = qGHS.Symbol2;
                    obj.ghs_symbol3 = qGHS.Symbol3;
                    obj.ghs_symbol4 = qGHS.Symbol4;
                    obj.ghs_symbol5 = qGHS.Symbol5;
                }
                catch (Exception)
                {
                    // if no ghs entry is found assign NONE to fields
                    obj.ghs_signalword = "NONE";
                    obj.ghs_symbol1 = "NONE";
                    obj.ghs_symbol2 = "NONE";
                    obj.ghs_symbol3 = "NONE";
                    obj.ghs_symbol4 = "NONE";
                    obj.ghs_symbol5 = "NONE";
                }

                var dbClient = db.tblClient.Find(qPM.ClientID);
                obj.clientid = dbClient.ClientID;
                obj.warehouse = dbClient.CMCLocation;
                obj.clientname = dbClient.ClientName;

                obj.bulkid = -1;    // for insert later
                obj.productmasterid = productmasterid;
                obj.receivedate = DateTime.UtcNow;
                obj.bulkstatus = "RECD";
                obj.enteredby = HttpContext.Current.User.Identity.Name;

                obj.ListOfWareHouses = fnWarehouseIDs();
                obj.ListOfProductMasters = fnProductMasterIDs(obj.clientid, productmasterid);
                obj.ListOfBulkStatusIDs = fnBulkStatusIDs();
                obj.ListOfContainerTypeIDs = fnContainerTypeIDs();
                obj.ListOfUMs = fnUnitMeasure(obj.clientid);
                obj.ListOfCarriers = fnCarriers();

                return obj;
            }
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

                        newrec.CreateDate = DateTime.UtcNow;
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
                    qry.NoticeDate = incoming.noticedate;
                    qry.BulkLabelNote = incoming.bulklabelnote;
                    qry.ReceivedAsCode = incoming.receivedascode;
                    qry.ReceivedAsName = incoming.receivedasname;
                    qry.UpdateDate = DateTime.UtcNow;
                    qry.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();

                    // Close items tagged to be closed for this productmasterid (would be better to pass a comma delimited list of PKs)
                    db.Database.ExecuteSqlCommand("UPDATE tblBulkOrderItem SET Status='CL' WHERE ToBeClosed=1 AND productmasterid=" + incoming.productmasterid);

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

        public static BulkContainerViewModel fnFillBulkContainerFromDB(int id)
        {
            // id=bulkid
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblBulk
                           where t.BulkID == id
                           select new BulkContainerViewModel
                               {
                                   pm_sumofcurrentweight = (from x in db.tblBulk
                                                            where x.ProductMasterID == t.ProductMasterID
                                                            select x.CurrentWeight).Sum(),
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
                                   noticedate = t.NoticeDate,
                                   bulklabelnote = t.BulkLabelNote,
                                   receivedascode = t.ReceivedAsCode,
                                   receivedasname = t.ReceivedAsName,
                                   containernotes = t.ContainerNotes
                               }).FirstOrDefault();

                var qPM = db.tblProductMaster.Find(obj.productmasterid);
                var qCL = (from t in db.tblClient
                           where t.ClientID == qPM.ClientID
                           select t).FirstOrDefault();

                obj.clientid = qPM.ClientID;
                obj.clientname = qCL.ClientName;
                obj.ListOfWareHouses = fnWarehouseIDs();
                obj.ListOfProductMasters = fnProductMasterIDs(qPM.ClientID, qPM.ProductMasterID);
                obj.ListOfBulkStatusIDs = fnBulkStatusIDs();
                obj.ListOfContainerTypeIDs = fnContainerTypeIDs();
                obj.ListOfUMs = fnUnitMeasure(obj.clientid);
                obj.ListOfCarriers = fnCarriers();

                // R/O fields from PM
                obj.MasterCode = qPM.MasterCode;
                obj.MasterName = qPM.MasterName;
                obj.pm_alertnotesreceiving = qPM.AlertNotesReceiving;
                obj.pm_OtherHandlingInstr = qPM.OtherHandlingInstr;
                obj.pm_refrigerate = qPM.Refrigerate;
                obj.pm_flammablestorageroom = qPM.FlammableStorageRoom;
                obj.pm_freezerstorage = qPM.FREEZERSTORAGE;
                obj.pm_otherstorage = qPM.OtherStorage;
                obj.pm_cleanroomgmp = qPM.CleanRoomGMP;
                obj.pm_alertnotesreceiving = qPM.AlertNotesReceiving;
                obj.pm_restrictedtoamount = qPM.RestrictedToAmount;
                obj.pm_tempraturecontrolledstorage = qPM.TemperatureControlledStorage;
                obj.pm_shelflife = qPM.ShlfLife;
                obj.pm_packoutonreceipt = qPM.PackOutOnReceipt;

                // R/O from related PD
                var qPD = (from t in db.tblProductDetail
                           where t.ProductMasterID == obj.productmasterid
                           select t).FirstOrDefault();

                obj.pd_groundunnum = qPD.GRNUNNUMBER;
                obj.pd_groundpackinggrp = qPD.GRNPKGRP;
                obj.pd_groundhazardclass = qPD.GRNHAZCL;
                obj.pd_groundhazardsubclass = qPD.GRNHAZSUBCL;
                obj.pd_epabiocide = qPD.EPABiocide;

                try
                {
                    var qGHS = (from t in db.tblGHS
                                join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                                where pd.ProductMasterID == obj.productmasterid
                                select t).FirstOrDefault();

                    obj.ghs_signalword = qGHS.SignalWord;
                    obj.ghs_symbol1 = qGHS.Symbol1;
                    obj.ghs_symbol2 = qGHS.Symbol2;
                    obj.ghs_symbol3 = qGHS.Symbol3;
                    obj.ghs_symbol4 = qGHS.Symbol4;
                    obj.ghs_symbol5 = qGHS.Symbol5;
                }
                catch (Exception)
                {
                    // if no ghs entry is found assign NONE to fields
                    obj.ghs_signalword = "NONE";
                    obj.ghs_symbol1 = "NONE";
                    obj.ghs_symbol2 = "NONE";
                    obj.ghs_symbol3 = "NONE";
                    obj.ghs_symbol4 = "NONE";
                    obj.ghs_symbol5 = "NONE";
                }

                return obj;
            }
        }

        #endregion Known Bulk Receiving Methods

        #region Unknown Bulk Receiving Methods

        public static BulkContainerViewModel fnNewBulkContainerUnKnown()
        {
            BulkContainerViewModel obj = new BulkContainerViewModel();
            using (var db = new CMCSQL03Entities())
            {
                obj.isknownmaterial = false;
                obj.bulkid = -1;
                obj.receivedate = DateTime.UtcNow;
                obj.enteredby = HttpContext.Current.User.Identity.Name;
                obj.receivedby = HttpContext.Current.User.Identity.Name;
                obj.ListOfWareHouses = fnWarehouseIDs();
                obj.ListOfContainerTypeIDs = fnContainerTypeIDs();
                obj.ListOfCarriers = fnCarriers();
                return obj;
            }
        }

        public static bool fnSaveBulkContainerUnKnown(BulkContainerViewModel incoming)
        {
            bool retval = true;
            try
            {
                using (var db = new EF.CMCSQL03Entities())
                {
                    var newitem = new EF.tblBulkUnKnown
                    {
                        ReceivedAsCode = incoming.receivedascode,
                        ReceivedAsName = incoming.receivedasname,
                        ReceiveDate = incoming.receivedate,
                        Warehouse = incoming.warehouse,
                        LotNumber = incoming.lotnumber,
                        Carrier = incoming.carrier,
                        ReceiveWeight = incoming.receiveweight,
                        CurrentWeight = incoming.receiveweight,
                        EnteredBy = incoming.enteredby,
                        ReceivedBy = incoming.receivedby,
                        COAIncluded = incoming.coaincluded,
                        MSDSIncluded = incoming.msdsincluded,
                        MfgDate = incoming.mfgdate,
                        ExpirationDate = incoming.expirationdate,
                        CeaseShipDate = incoming.ceaseshipdate,
                        QCDate = incoming.qcdate,
                        ContainerNotes = incoming.containernotes,
                        Bin = incoming.bin,
                        ContainerColor = incoming.containercolor,
                        ReturnLocation = incoming.returnlocation,
                        ContainerType = incoming.containertype,
                        BulkLabelNote = incoming.bulklabelnote,
                        UM = incoming.um,
                        NoticeDate = DateTime.UtcNow,
                        LogNotes = "Unknown bulk stock received by " + HttpContext.Current.User.Identity.Name + " on " + DateTime.UtcNow.ToString("R"),
                        BulkStatus = "HOLD"
                    };

                    db.tblBulkUnKnown.Add(newitem);
                    db.SaveChanges();

                    retval = true;
                }
            }
            catch
            {
                retval = false;
                throw new Exception("Error occurred saving Unknown Bulk Container");
            }

            return retval;
        }

        #endregion Unknown Bulk Receiving Methods

        #region Prepack Receiving Methods

        public static PrePackViewModel fnNewBulkContainerForPrePack(int clientid, int productdetailid)
        {
            PrePackViewModel obj = new PrePackViewModel();
            using (var db = new CMCSQL03Entities())
            {
                var dbClient = db.tblClient.Find(clientid);
                var dbProductDetail = db.tblProductDetail.Find(productdetailid);
                var qPM = db.tblProductMaster.Find(dbProductDetail.ProductMasterID);

                obj.productmasterid = dbProductDetail.ProductMasterID;
                obj.isknownmaterial = true;
                obj.clientid = clientid;
                obj.clientname = dbClient.ClientName;
                obj.bulkid = -1;
                obj.receivedate = DateTime.UtcNow;
                obj.carrier = null;
                obj.ListOfCarriers = fnCarriers();
                obj.warehouse = null;
                obj.ListOfWareHouses = fnWarehouseIDs();
                obj.enteredby = HttpContext.Current.User.Identity.Name;
                obj.lotnumber = null;
                obj.receivedby = HttpContext.Current.User.Identity.Name;
                obj.mfgdate = null;
                obj.expirationdate = null;
                obj.ceaseshipdate = null;
                obj.qcdate = null;
                obj.msdsincluded = null;
                obj.coaincluded = null;
                obj.productcode = dbProductDetail.ProductCode;
                obj.productname = dbProductDetail.ProductName;
                obj.pm_ceaseshipdifferential = qPM.CeaseShipDifferential;

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
                    newbulk.Bin = "PREPACK";

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
                        newstock.CreateDate = DateTime.UtcNow;
                        newstock.CreateUser = HttpContext.Current.User.Identity.Name;
                        newstock.Warehouse = vm.warehouse;
                        newstock.QtyOnHand = ThisQty;
                        newstock.ShelfStatus = "RECD";
                        newstock.Bin = (from t in db.tblShelfMaster
                                        where t.ShelfID == newstock.ShelfID
                                        select t.Bin).FirstOrDefault();

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

        #endregion Prepack Receiving Methods

        #region Dropdownlist methods

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

        public static List<SelectListItem> fnUnitMeasure(int? clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblTier
                          orderby t.ClientID
                          where t.ClientID == clientid
                          select new SelectListItem
                          {
                              Value = t.Size,
                              Text = t.Size
                          }).Distinct().ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }

        #endregion Dropdownlist methods
    }
}