using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Services
{
    public class BulkService
    {
        public static BulkContainerViewModel FillBulkContainer(int bulkId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkRecord = (from bulk in db.tblBulk
                                  where bulk.BulkID == bulkId
                                  select new BulkContainerViewModel
                                  {
                                      isknownmaterial = true,
                                      bulkid = bulk.BulkID,
                                      productmasterid = bulk.ProductMasterID,
                                      warehouse = bulk.Warehouse,
                                      receivedate = bulk.ReceiveDate,
                                      carrier = bulk.Carrier,
                                      receivedby = bulk.ReceivedBy,
                                      enteredby = bulk.EnteredBy,
                                      receiveweight = bulk.ReceiveWeight,
                                      lotnumber = bulk.LotNumber,
                                      mfgdate = bulk.MfgDate,
                                      expirationdate = bulk.ExpirationDate,
                                      ceaseshipdate = bulk.CeaseShipDate,
                                      bulkstatus = bulk.BulkStatus,
                                      qty = "1",
                                      um = bulk.UM,
                                      containercolor = bulk.ContainerColor,
                                      bin = bulk.Bin,
                                      containertype = bulk.ContainerType,
                                      coaincluded = bulk.COAIncluded,
                                      msdsincluded = bulk.MSDSIncluded,
                                      currentweight = bulk.CurrentWeight,
                                      qcdate = bulk.QCDate,
                                      noticedate = bulk.NoticeDate,
                                      bulklabelnote = bulk.BulkLabelNote,
                                      receivedascode = bulk.ReceivedAsCode,
                                      receivedasname = bulk.ReceivedAsName,
                                      containernotes = bulk.ContainerNotes,
                                      CreateUser = bulk.CreateUser,
                                      CreateDate = bulk.CreateDate,
                                      UpdateUser = bulk.UpdateUser,
                                      UpdateDate = bulk.UpdateDate
                                  }).FirstOrDefault();

                var productMaster = (from t in db.tblProductMaster
                                     where t.ProductMasterID == bulkRecord.productmasterid
                                     select t).FirstOrDefault();

                var client = (from t in db.tblClient
                              where t.ClientID == productMaster.ClientID
                              select t).FirstOrDefault();

                bulkRecord.clientid = productMaster.ClientID;
                bulkRecord.clientname = client.ClientName;
                bulkRecord.MasterCode = productMaster.MasterCode;
                bulkRecord.MasterName = productMaster.MasterName;
                bulkRecord.pm_ceaseshipdifferential = productMaster.CeaseShipDifferential;

                return bulkRecord;
            }
        }

        public static bool SaveBulkContainer(BulkContainerViewModel model)
        {
            bool isConfirmSaved = false;
            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    int pk = model.bulkid;
                    if (model.bulkid == -1)
                    {
                        var newRecord = new tblBulk { ProductMasterID = model.productmasterid };

                        db.tblBulk.Add(newRecord);

                        newRecord.CreateDate = DateTime.UtcNow;
                        newRecord.CreateUser = HttpContext.Current.User.Identity.Name;
                        newRecord.UpdateDate = DateTime.UtcNow;
                        newRecord.UpdateUser = HttpContext.Current.User.Identity.Name;

                        db.SaveChanges();

                        pk = newRecord.BulkID;
                    }

                    var bulkRecord = (from t in db.tblBulk
                                      where t.BulkID == pk
                                      select t).FirstOrDefault();

                    bulkRecord.Warehouse = model.warehouse;
                    bulkRecord.ReceiveDate = model.receivedate;
                    bulkRecord.Carrier = model.carrier;
                    bulkRecord.ReceivedBy = model.receivedby;
                    bulkRecord.EnteredBy = model.enteredby;
                    bulkRecord.ProductMasterID = model.productmasterid;
                    bulkRecord.ReceiveWeight = model.receiveweight;
                    bulkRecord.LotNumber = model.lotnumber;
                    bulkRecord.MfgDate = model.mfgdate;
                    bulkRecord.ExpirationDate = model.expirationdate;
                    bulkRecord.CeaseShipDate = model.ceaseshipdate;
                    bulkRecord.BulkStatus = model.bulkstatus;
                    bulkRecord.UM = model.um;
                    bulkRecord.ContainerColor = model.containercolor;
                    bulkRecord.Bin = model.bin;
                    bulkRecord.ContainerType = model.containertype;
                    bulkRecord.COAIncluded = model.coaincluded;
                    bulkRecord.MSDSIncluded = model.msdsincluded;
                    bulkRecord.ContainerNotes = model.containernotes;
                    bulkRecord.CurrentWeight = model.currentweight;
                    bulkRecord.QCDate = model.qcdate;
                    bulkRecord.NoticeDate = model.noticedate;
                    bulkRecord.BulkLabelNote = model.bulklabelnote;
                    bulkRecord.ReceivedAsCode = model.receivedascode;
                    bulkRecord.ReceivedAsName = model.receivedasname;
                    bulkRecord.UpdateDate = DateTime.UtcNow;
                    bulkRecord.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();

                    isConfirmSaved = true;
                }
            }
            catch
            {
                isConfirmSaved = false;
                throw new Exception("Error occurred while saving bulk container changes to database.");
            }

            return isConfirmSaved;
        }

        #region SupportMethods

        public static List<BulkContainerViewModel> BulkContainers()
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkRecords = (from bulk in db.tblBulk
                                   join productMaster in db.tblProductMaster on bulk.ProductMasterID equals productMaster.ProductMasterID
                                   join client in db.tblClient on productMaster.ClientID equals client.ClientID
                                   orderby client.ClientName, productMaster.MasterCode
                                   select new BulkContainerViewModel
                                   {
                                       bulkid = bulk.BulkID,
                                       warehouse = bulk.Warehouse,
                                       bin = bulk.Bin,
                                       clientname = client.ClientName,
                                       MasterCode = productMaster.MasterCode,
                                       MasterName = productMaster.MasterName,
                                       bulkstatus = bulk.BulkStatus,
                                       productmasterid = bulk.ProductMasterID,
                                       clientid = client.ClientID
                                   }).ToList();

                return bulkRecords;
            }
        }

        #endregion SupportMethods
    }
}