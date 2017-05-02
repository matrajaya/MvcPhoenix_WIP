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
            using (var db = new CMCSQL03Entities())
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
            using (var db = new CMCSQL03Entities())
            {
                db.Database.ExecuteSqlCommand("UPDATE tblBulkOrderItem SET ToBeClosed=null WHERE ProductMasterID=" + id);
            }

            // build a list of Open Bulk Order Items for the Partial View
            List<OpenBulkOrderItems> mylist = new List<OpenBulkOrderItems>();
            using (var db = new CMCSQL03Entities())
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
                obj.pm_shelflife = qPM.ShelfLife;
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

                //obj.ListOfProductMasters = ApplicationService.ddlProductMasterIDs(obj.clientid, productmasterid);

                return obj;
            }
        }

        public static bool fnSaveBulkContainerKnown(BulkContainerViewModel incoming)
        {
            bool retval = true;
            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    int pk = incoming.bulkid;
                    if (incoming.bulkid == -1)
                    {
                        var newrec = new tblBulk
                        {
                            ProductMasterID = incoming.productmasterid
                        };

                        newrec.CreateDate = DateTime.UtcNow;
                        newrec.CreateUser = HttpContext.Current.User.Identity.Name;

                        db.tblBulk.Add(newrec);
                        db.SaveChanges();

                        pk = newrec.BulkID;
                    }

                    var bulk = (from t in db.tblBulk
                                where t.BulkID == pk
                                select t).FirstOrDefault();

                    bulk.Warehouse = incoming.warehouse;
                    bulk.ReceiveDate = incoming.receivedate;
                    bulk.Carrier = incoming.carrier;
                    bulk.ReceivedBy = incoming.receivedby;
                    bulk.EnteredBy = incoming.enteredby;
                    bulk.ProductMasterID = incoming.productmasterid;
                    bulk.ReceiveWeight = incoming.receiveweight;
                    bulk.LotNumber = incoming.lotnumber;
                    bulk.MfgDate = incoming.mfgdate;
                    bulk.ExpirationDate = incoming.expirationdate;
                    bulk.CeaseShipDate = incoming.ceaseshipdate;
                    bulk.BulkStatus = incoming.bulkstatus;
                    bulk.UM = incoming.um;
                    bulk.ContainerColor = incoming.containercolor;
                    bulk.Bin = incoming.bin;
                    bulk.ContainerType = incoming.containertype;
                    bulk.COAIncluded = incoming.coaincluded;
                    bulk.MSDSIncluded = incoming.msdsincluded;
                    bulk.ContainerNotes = incoming.containernotes;
                    bulk.CurrentWeight = incoming.receiveweight;
                    bulk.QCDate = incoming.qcdate;
                    bulk.NoticeDate = incoming.noticedate;
                    bulk.BulkLabelNote = incoming.bulklabelnote;
                    bulk.ReceivedAsCode = incoming.receivedascode;
                    bulk.ReceivedAsName = incoming.receivedasname;
                    bulk.UpdateDate = DateTime.UtcNow;
                    bulk.UpdateUser = HttpContext.Current.User.Identity.Name;

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
                obj.pm_shelflife = qPM.ShelfLife;
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

                return obj;
            }
        }

        public static bool fnSaveBulkContainerUnKnown(BulkContainerViewModel incoming)
        {
            bool retval = true;
            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    var newitem = new tblBulkUnKnown
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
                obj.warehouse = null;
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
                using (var db = new CMCSQL03Entities())
                {
                    var newbulk = new tblBulk();

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

                        var newstock = new tblStock();

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
    }
}