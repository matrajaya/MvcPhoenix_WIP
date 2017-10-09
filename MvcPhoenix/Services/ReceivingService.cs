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
        /// <summary>
        /// Get bulk containers received
        /// </summary>
        /// <returns>bulkContainers</returns>
        public static List<BulkContainerViewModel> GetBulkContainers()
        {
            var bulkContainers = BulkService.GetBulkContainers()
                                            .Where(t => t.bulkstatus == "RECD")
                                            .OrderBy(x => x.bulkid).ToList();

            return bulkContainers;
        }

        #region Receive Known Bulk

        public static BulkContainerViewModel GetBulkContainer(int bulkid)
        {
            var bulkContainer = new BulkContainerViewModel();

            using (var db = new CMCSQL03Entities())
            {
                bulkContainer = (from t in db.tblBulk
                                 where t.BulkID == bulkid
                                 select new BulkContainerViewModel
                                 {
                                     pm_sumofcurrentweight = db.tblBulk
                                                               .Where(x => x.ProductMasterID == t.ProductMasterID)
                                                               .Sum(x => x.CurrentWeight),
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

                var productMaster = db.tblProductMaster.Find(bulkContainer.productmasterid);
                var productDetail = db.tblProductDetail.FirstOrDefault(x => x.ProductMasterID == bulkContainer.productmasterid);
                int clientId = Convert.ToInt32(productMaster.ClientID);
                var client = ClientService.GetClient(clientId);

                bulkContainer.clientid = productMaster.ClientID;
                bulkContainer.clientname = client.ClientName;

                bulkContainer.MasterCode = productMaster.MasterCode;
                bulkContainer.MasterName = productMaster.MasterName;
                bulkContainer.pm_alertnotesreceiving = productMaster.AlertNotesReceiving;
                bulkContainer.pm_OtherHandlingInstr = productMaster.OtherHandlingInstr;
                bulkContainer.pm_refrigerate = productMaster.Refrigerate;
                bulkContainer.pm_flammablestorageroom = productMaster.FlammableStorageRoom;
                bulkContainer.pm_freezerstorage = productMaster.FREEZERSTORAGE;
                bulkContainer.pm_otherstorage = productMaster.OtherStorage;
                bulkContainer.pm_cleanroomgmp = productMaster.CleanRoomGMP;
                bulkContainer.pm_alertnotesreceiving = productMaster.AlertNotesReceiving;
                bulkContainer.pm_restrictedtoamount = productMaster.RestrictedToAmount;
                bulkContainer.pm_tempraturecontrolledstorage = productMaster.TemperatureControlledStorage;
                bulkContainer.pm_shelflife = productMaster.ShelfLife;
                bulkContainer.pm_packoutonreceipt = productMaster.PackOutOnReceipt;

                bulkContainer.pd_groundunnum = productDetail.GRNUNNUMBER;
                bulkContainer.pd_groundpackinggrp = productDetail.GRNPKGRP;
                bulkContainer.pd_groundhazardclass = productDetail.GRNHAZCL;
                bulkContainer.pd_groundhazardsubclass = productDetail.GRNHAZSUBCL;
                bulkContainer.pd_epabiocide = productDetail.EPABiocide;

                try
                {
                    var ghs = (from t in db.tblGHS
                               join productdetail in db.tblProductDetail on t.ProductDetailID equals productdetail.ProductDetailID
                               where productdetail.ProductMasterID == bulkContainer.productmasterid
                               select t).FirstOrDefault();

                    bulkContainer.ghs_signalword = ghs.SignalWord;
                    bulkContainer.ghs_symbol1 = ghs.Symbol1;
                    bulkContainer.ghs_symbol2 = ghs.Symbol2;
                    bulkContainer.ghs_symbol3 = ghs.Symbol3;
                    bulkContainer.ghs_symbol4 = ghs.Symbol4;
                    bulkContainer.ghs_symbol5 = ghs.Symbol5;
                }
                catch (Exception)
                {
                    // if no ghs entry is found assign NONE to fields
                    bulkContainer.ghs_signalword = "NONE";
                    bulkContainer.ghs_symbol1 = "NONE";
                    bulkContainer.ghs_symbol2 = "NONE";
                    bulkContainer.ghs_symbol3 = "NONE";
                    bulkContainer.ghs_symbol4 = "NONE";
                    bulkContainer.ghs_symbol5 = "NONE";
                }
            }

            return bulkContainer;
        }

        public static BulkContainerViewModel NewBulkContainer(int productmasterid, int? productdetailid)
        {
            var bulkContainer = new BulkContainerViewModel();

            using (var db = new CMCSQL03Entities())
            {
                bulkContainer.isknownmaterial = true;
                bulkContainer.pm_sumofcurrentweight = 0;

                var bulk = db.tblBulk.Where(x => x.ProductMasterID == productmasterid);

                foreach (var row in bulk)
                {
                    bulkContainer.pm_sumofcurrentweight = bulkContainer.pm_sumofcurrentweight + row.CurrentWeight;
                }

                var productMaster = db.tblProductMaster.Find(productmasterid);

                bulkContainer.MasterCode = productMaster.MasterCode;
                bulkContainer.MasterName = productMaster.MasterName;
                bulkContainer.pm_alertnotesreceiving = productMaster.AlertNotesReceiving;
                bulkContainer.pm_OtherHandlingInstr = productMaster.OtherHandlingInstr;
                bulkContainer.pm_refrigerate = productMaster.Refrigerate;
                bulkContainer.pm_flammablestorageroom = productMaster.FlammableStorageRoom;
                bulkContainer.pm_freezerstorage = productMaster.FREEZERSTORAGE;
                bulkContainer.pm_otherstorage = productMaster.OtherStorage;
                bulkContainer.pm_cleanroomgmp = productMaster.CleanRoomGMP;
                bulkContainer.pm_alertnotesreceiving = productMaster.AlertNotesReceiving;
                bulkContainer.pm_restrictedtoamount = productMaster.RestrictedToAmount;
                bulkContainer.pm_tempraturecontrolledstorage = productMaster.TemperatureControlledStorage;
                bulkContainer.pm_shelflife = productMaster.ShelfLife;
                bulkContainer.pm_packoutonreceipt = productMaster.PackOutOnReceipt;
                bulkContainer.pm_ceaseshipdifferential = productMaster.CeaseShipDifferential;

                var productDetail = db.tblProductDetail
                                      .Where(x => x.ProductMasterID == bulkContainer.productmasterid)
                                      .FirstOrDefault();

                bulkContainer.pd_groundunnum = productDetail.GRNUNNUMBER;
                bulkContainer.pd_groundpackinggrp = productDetail.GRNPKGRP;
                bulkContainer.pd_groundhazardclass = productDetail.GRNHAZCL;
                bulkContainer.pd_groundhazardsubclass = productDetail.GRNHAZSUBCL;
                bulkContainer.pd_epabiocide = productDetail.EPABiocide;

                try
                {
                    var ghs = (from t in db.tblGHS
                               join pd in db.tblProductDetail on t.ProductDetailID equals pd.ProductDetailID
                               where pd.ProductMasterID == bulkContainer.productmasterid
                               select t).FirstOrDefault();

                    bulkContainer.ghs_signalword = ghs.SignalWord;
                    bulkContainer.ghs_symbol1 = ghs.Symbol1;
                    bulkContainer.ghs_symbol2 = ghs.Symbol2;
                    bulkContainer.ghs_symbol3 = ghs.Symbol3;
                    bulkContainer.ghs_symbol4 = ghs.Symbol4;
                    bulkContainer.ghs_symbol5 = ghs.Symbol5;
                }
                catch (Exception)
                {
                    // if no ghs entry is found assign NONE to fields
                    bulkContainer.ghs_signalword = "NONE";
                    bulkContainer.ghs_symbol1 = "NONE";
                    bulkContainer.ghs_symbol2 = "NONE";
                    bulkContainer.ghs_symbol3 = "NONE";
                    bulkContainer.ghs_symbol4 = "NONE";
                    bulkContainer.ghs_symbol5 = "NONE";
                }

                var client = db.tblClient.Find(productMaster.ClientID);

                bulkContainer.clientid = client.ClientID;
                bulkContainer.warehouse = client.CMCLocation;
                bulkContainer.clientname = client.ClientName;

                bulkContainer.bulkid = -1;
                bulkContainer.productmasterid = productmasterid;
                bulkContainer.receivedate = DateTime.UtcNow;
                bulkContainer.bulkstatus = "RECD";
                bulkContainer.enteredby = HttpContext.Current.User.Identity.Name;
            }

            return bulkContainer;
        }

        public static void SaveBulkContainerKnown(BulkContainerViewModel bulkcontainer)
        {
            using (var db = new CMCSQL03Entities())
            {
                int bulkId = bulkcontainer.bulkid;

                if (bulkcontainer.bulkid == -1)
                {
                    var newBulk = new tblBulk
                    {
                        ProductMasterID = bulkcontainer.productmasterid
                    };

                    newBulk.CreateDate = DateTime.UtcNow;
                    newBulk.CreateUser = HttpContext.Current.User.Identity.Name;

                    db.tblBulk.Add(newBulk);
                    db.SaveChanges();

                    bulkId = newBulk.BulkID;
                }

                var bulk = db.tblBulk.Find(bulkId);

                bulk.Warehouse = bulkcontainer.warehouse;
                bulk.ReceiveDate = bulkcontainer.receivedate;
                bulk.Carrier = bulkcontainer.carrier;
                bulk.ReceivedBy = bulkcontainer.receivedby;
                bulk.EnteredBy = bulkcontainer.enteredby;
                bulk.ProductMasterID = bulkcontainer.productmasterid;
                bulk.ReceiveWeight = bulkcontainer.receiveweight;
                bulk.LotNumber = bulkcontainer.lotnumber;
                bulk.MfgDate = bulkcontainer.mfgdate;
                bulk.ExpirationDate = bulkcontainer.expirationdate;
                bulk.CeaseShipDate = bulkcontainer.ceaseshipdate;
                bulk.BulkStatus = bulkcontainer.bulkstatus;
                bulk.UM = bulkcontainer.um;
                bulk.ContainerColor = bulkcontainer.containercolor;
                bulk.Bin = bulkcontainer.bin;
                bulk.ContainerType = bulkcontainer.containertype;
                bulk.COAIncluded = bulkcontainer.coaincluded;
                bulk.MSDSIncluded = bulkcontainer.msdsincluded;
                bulk.ContainerNotes = bulkcontainer.containernotes;
                bulk.CurrentWeight = bulkcontainer.receiveweight;
                bulk.QCDate = bulkcontainer.qcdate;
                bulk.NoticeDate = bulkcontainer.noticedate;
                bulk.BulkLabelNote = bulkcontainer.bulklabelnote;
                bulk.ReceivedAsCode = bulkcontainer.receivedascode;
                bulk.ReceivedAsName = bulkcontainer.receivedasname;
                bulk.UpdateDate = DateTime.UtcNow;
                bulk.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();

                // Close items tagged to be closed for this productmasterid
                int? productMasterId = bulkcontainer.productmasterid;

                string updateQuery = "UPDATE tblBulkOrderItem SET Status='CL' WHERE ToBeClosed=1 AND productmasterid=" + productMasterId;
                db.Database.ExecuteSqlCommand(updateQuery);
            }
        }

        #endregion Receive Known Bulk

        #region Receive Unknown Bulk

        public static BulkContainerViewModel NewBulkContainerUnKnown()
        {
            var bulkContainer = new BulkContainerViewModel();

            using (var db = new CMCSQL03Entities())
            {
                bulkContainer.isknownmaterial = false;
                bulkContainer.bulkid = -1;
                bulkContainer.receivedate = DateTime.UtcNow;
                bulkContainer.enteredby = HttpContext.Current.User.Identity.Name;
                bulkContainer.receivedby = HttpContext.Current.User.Identity.Name;
            }

            return bulkContainer;
        }

        public static void SaveBulkContainerUnKnown(BulkContainerViewModel bulkcontainer)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkUnknown = new tblBulkUnKnown
                {
                    ReceivedAsCode = bulkcontainer.receivedascode,
                    ReceivedAsName = bulkcontainer.receivedasname,
                    ReceiveDate = bulkcontainer.receivedate,
                    Warehouse = bulkcontainer.warehouse,
                    LotNumber = bulkcontainer.lotnumber,
                    Carrier = bulkcontainer.carrier,
                    ReceiveWeight = bulkcontainer.receiveweight,
                    CurrentWeight = bulkcontainer.receiveweight,
                    EnteredBy = bulkcontainer.enteredby,
                    ReceivedBy = bulkcontainer.receivedby,
                    COAIncluded = bulkcontainer.coaincluded,
                    MSDSIncluded = bulkcontainer.msdsincluded,
                    MfgDate = bulkcontainer.mfgdate,
                    ExpirationDate = bulkcontainer.expirationdate,
                    CeaseShipDate = bulkcontainer.ceaseshipdate,
                    QCDate = bulkcontainer.qcdate,
                    ContainerNotes = bulkcontainer.containernotes,
                    Bin = bulkcontainer.bin,
                    ContainerColor = bulkcontainer.containercolor,
                    ReturnLocation = bulkcontainer.returnlocation,
                    ContainerType = bulkcontainer.containertype,
                    BulkLabelNote = bulkcontainer.bulklabelnote,
                    UM = bulkcontainer.um,
                    NoticeDate = DateTime.UtcNow,
                    LogNotes = "Unknown bulk stock received by " + HttpContext.Current.User.Identity.Name + " on " + DateTime.UtcNow.ToString("R"),
                    BulkStatus = "HOLD"
                };

                db.tblBulkUnKnown.Add(bulkUnknown);
                db.SaveChanges();
            }
        }

        #endregion Receive Unknown Bulk

        #region Receive Prepack

        public static PrePackViewModel NewBulkContainerPrePack(int clientid, int productdetailid)
        {
            var prePack = new PrePackViewModel();

            using (var db = new CMCSQL03Entities())
            {
                var client = db.tblClient.Find(clientid);
                var productDetail = db.tblProductDetail.Find(productdetailid);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);
                var userName = HttpContext.Current.User.Identity.Name;

                prePack.productmasterid = productDetail.ProductMasterID;
                prePack.isknownmaterial = true;
                prePack.clientid = clientid;
                prePack.clientname = client.ClientName;
                prePack.bulkid = -1;
                prePack.receivedate = DateTime.UtcNow;
                prePack.carrier = null;
                prePack.warehouse = null;
                prePack.enteredby = userName;
                prePack.lotnumber = null;
                prePack.receivedby = userName;
                prePack.mfgdate = null;
                prePack.expirationdate = null;
                prePack.ceaseshipdate = null;
                prePack.qcdate = null;
                prePack.msdsincluded = null;
                prePack.coaincluded = null;
                prePack.productcode = productDetail.ProductCode;
                prePack.productname = productDetail.ProductName;
                prePack.pm_ceaseshipdifferential = productMaster.CeaseShipDifferential;
                prePack.ListOfShelfMasters = (from t in db.tblShelfMaster
                                              orderby t.ShelfID
                                              where t.ProductDetailID == productdetailid
                                              select new ItemForPrePackViewModel
                                              {
                                                  shelfid = t.ShelfID,
                                                  size = t.Size,
                                                  bin = t.Bin
                                              }).ToList();
                prePack.ItemsCount = prePack.ListOfShelfMasters.Count();
            }

            return prePack;
        }

        public static void SavePrePack(PrePackViewModel prepack, FormCollection form)
        {
            using (var db = new CMCSQL03Entities())
            {
                var newBulk = new tblBulk();

                newBulk.ProductMasterID = prepack.productmasterid;
                newBulk.ReceiveDate = prepack.receivedate;
                newBulk.LotNumber = prepack.lotnumber;
                newBulk.CeaseShipDate = prepack.ceaseshipdate;
                newBulk.Carrier = prepack.carrier;
                newBulk.ReceivedBy = prepack.receivedby;
                newBulk.QCDate = prepack.qcdate;
                newBulk.Warehouse = prepack.warehouse;
                newBulk.MfgDate = prepack.mfgdate;
                newBulk.COAIncluded = prepack.coaincluded;
                newBulk.EnteredBy = prepack.enteredby;
                newBulk.ExpirationDate = prepack.expirationdate;
                newBulk.MSDSIncluded = prepack.msdsincluded;
                newBulk.BulkStatus = "PP";
                newBulk.Bin = "PREPACK";

                db.tblBulk.Add(newBulk);
                db.SaveChanges();

                int newBulkID = newBulk.BulkID;

                for (int i = 1; i <= prepack.ItemsCount; i++)
                {
                    int ThisShelfID = Convert.ToInt32(form["Key" + i.ToString()]);
                    int ThisQty = Convert.ToInt32(form["Value" + i.ToString()]);

                    var newstock = new tblStock();

                    newstock.BulkID = newBulkID;
                    newstock.ShelfID = ThisShelfID;
                    newstock.CreateDate = DateTime.UtcNow;
                    newstock.CreateUser = HttpContext.Current.User.Identity.Name;
                    newstock.Warehouse = prepack.warehouse;
                    newstock.QtyOnHand = ThisQty;
                    newstock.ShelfStatus = "RECD";
                    newstock.Bin = db.tblShelfMaster
                                     .Where(x => x.ShelfID == newstock.ShelfID)
                                     .Select(x => x.Bin).FirstOrDefault();

                    db.tblStock.Add(newstock);
                    db.SaveChanges();
                }
            }
        }

        #endregion Receive Prepack
    }
}