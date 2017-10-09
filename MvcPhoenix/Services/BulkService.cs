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
        #region Bulk Container

        /// <summary>
        /// Get list of bulk containers
        /// </summary>
        /// <returns>bulk records</returns>
        public static List<BulkContainerViewModel> GetBulkContainers()
        {
            var bulkContainers = new List<BulkContainerViewModel>();

            using (var db = new CMCSQL03Entities())
            {
                bulkContainers = (from bulk in db.tblBulk
                                  join productmaster in db.tblProductMaster on bulk.ProductMasterID equals productmaster.ProductMasterID
                                  join client in db.tblClient on productmaster.ClientID equals client.ClientID
                                  orderby client.ClientName, productmaster.MasterCode
                                  select new BulkContainerViewModel
                                  {
                                      clientid = client.ClientID,
                                      clientname = client.ClientName,
                                      bulkid = bulk.BulkID,
                                      warehouse = bulk.Warehouse,
                                      bin = bulk.Bin,
                                      productmasterid = bulk.ProductMasterID,
                                      MasterCode = productmaster.MasterCode,
                                      MasterName = productmaster.MasterName,
                                      bulkstatus = bulk.BulkStatus,
                                      receivedate = bulk.ReceiveDate,
                                      carrier = bulk.Carrier,
                                      receiveweight = bulk.ReceiveWeight,
                                      lotnumber = bulk.LotNumber,
                                      expirationdate = bulk.ExpirationDate,
                                      um = bulk.UM
                                  }).ToList();
            }

            return bulkContainers;
        }

        /// <summary>
        /// Get bulk container details
        /// </summary>
        /// <param name="bulkId"></param>
        /// <returns>bulkContainer</returns>
        public static BulkContainerViewModel GetBulkContainer(int bulkId)
        {
            var bulkContainer = new BulkContainerViewModel();

            using (var db = new CMCSQL03Entities())
            {
                bulkContainer = (from bulk in db.tblBulk
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

                var productMaster = db.tblProductMaster.Find(bulkContainer.productmasterid);
                var client = db.tblClient.Find(productMaster.ClientID);

                bulkContainer.clientid = productMaster.ClientID;
                bulkContainer.clientname = client.ClientName;
                bulkContainer.MasterCode = productMaster.MasterCode;
                bulkContainer.MasterName = productMaster.MasterName;
                bulkContainer.pm_ceaseshipdifferential = productMaster.CeaseShipDifferential;
            }

            return bulkContainer;
        }

        /// <summary>
        /// Save bulk container record
        /// </summary>
        /// <param name="bulk"></param>
        /// <returns>true or false</returns>
        public static bool SaveBulkContainer(BulkContainerViewModel bulk)
        {
            bool isSaved = false;
            int bulkId = bulk.bulkid;

            using (var db = new CMCSQL03Entities())
            {
                if (bulk.bulkid < 1)
                {
                    var newBulkRecord = new tblBulk { ProductMasterID = bulk.productmasterid };

                    db.tblBulk.Add(newBulkRecord);

                    newBulkRecord.CreateDate = DateTime.UtcNow;
                    newBulkRecord.CreateUser = HttpContext.Current.User.Identity.Name;
                    newBulkRecord.UpdateDate = DateTime.UtcNow;
                    newBulkRecord.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();

                    bulkId = newBulkRecord.BulkID;
                }

                var bulkRecord = db.tblBulk.Find(bulkId);

                bulkRecord.Warehouse = bulk.warehouse;
                bulkRecord.ReceiveDate = bulk.receivedate;
                bulkRecord.Carrier = bulk.carrier;
                bulkRecord.ReceivedBy = bulk.receivedby;
                bulkRecord.EnteredBy = bulk.enteredby;
                bulkRecord.ProductMasterID = bulk.productmasterid;
                bulkRecord.ReceiveWeight = bulk.receiveweight;
                bulkRecord.LotNumber = bulk.lotnumber;
                bulkRecord.MfgDate = bulk.mfgdate;
                bulkRecord.ExpirationDate = bulk.expirationdate;
                bulkRecord.CeaseShipDate = bulk.ceaseshipdate;
                bulkRecord.BulkStatus = bulk.bulkstatus;
                bulkRecord.UM = bulk.um;
                bulkRecord.ContainerColor = bulk.containercolor;
                bulkRecord.Bin = bulk.bin;
                bulkRecord.ContainerType = bulk.containertype;
                bulkRecord.COAIncluded = bulk.coaincluded;
                bulkRecord.MSDSIncluded = bulk.msdsincluded;
                bulkRecord.ContainerNotes = bulk.containernotes;
                bulkRecord.CurrentWeight = bulk.currentweight;
                bulkRecord.QCDate = bulk.qcdate;
                bulkRecord.NoticeDate = bulk.noticedate;
                bulkRecord.BulkLabelNote = bulk.bulklabelnote;
                bulkRecord.ReceivedAsCode = bulk.receivedascode;
                bulkRecord.ReceivedAsName = bulk.receivedasname;
                bulkRecord.UpdateDate = DateTime.UtcNow;
                bulkRecord.UpdateUser = HttpContext.Current.User.Identity.Name;

                try
                {
                    db.SaveChanges();
                    isSaved = true;
                }
                catch (Exception ex)
                {
                    isSaved = false;
                    throw new Exception("Error occurred while saving bulk container changes to database. " + ex);
                }
            }

            return isSaved;
        }

        #endregion Bulk Container

        /// <summary>
        /// Get list of open bulk order items
        /// </summary>
        /// <param name="productmasterid"></param>
        /// <returns>OpenBulkOrderItems</returns>
        public static List<OpenBulkOrderItems> GetOpenBulkOrderItems(int productmasterid)
        {
            var bulkOrderItems = new List<OpenBulkOrderItems>();

            using (var db = new CMCSQL03Entities())
            {
                string updateQuery = "UPDATE tblBulkOrderItem SET ToBeClosed=null WHERE ProductMasterID=" + productmasterid;
                db.Database.ExecuteSqlCommand(updateQuery);

                bulkOrderItems = (from bulkorderitem in db.tblBulkOrderItem
                                  join bulkorder in db.tblBulkOrder on bulkorderitem.BulkOrderID equals bulkorder.BulkOrderID
                                  where bulkorderitem.ProductMasterID == productmasterid
                                  && bulkorderitem.Status == "OP"
                                  select new OpenBulkOrderItems
                                  {
                                      bulkorderitemid = bulkorderitem.BulkOrderItemID,
                                      bulkorderid = bulkorderitem.BulkOrderID,
                                      productmasterid = bulkorderitem.ProductMasterID,
                                      weight = bulkorderitem.Weight,
                                      status = bulkorderitem.Status,
                                      eta = bulkorderitem.ETA,
                                      supplyid = bulkorderitem.SupplyID,
                                      itemnotes = bulkorderitem.ItemNotes,
                                      orderdate = bulkorder.OrderDate,
                                      ToBeClosed = bulkorderitem.ToBeClosed
                                  }).ToList();
            }

            return bulkOrderItems;
        }

        public static void CloseBulkOrderItem(int bulkorderitemid, bool ischecked)
        {
            int closeValue = 0;

            if (ischecked == true)
            {
                closeValue = 1;
            }

            using (var db = new CMCSQL03Entities())
            {
                string updateQuery = "UPDATE tblBulkOrderItem SET ToBeClosed=" + closeValue + " WHERE BulkOrderItemID=" + bulkorderitemid;
                db.Database.ExecuteSqlCommand(updateQuery);
            }
        }
    }
}