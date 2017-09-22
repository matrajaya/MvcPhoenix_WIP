using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{
    public class InventoryService
    {
        public static Inventory FillInventory(int productDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var inventory = new Inventory();
                var productProfile = new ProductProfile();

                productProfile.productdetailid = productDetailId;
                inventory.PP = ProductsService.FillFromPD(productProfile);
                productProfile = ProductsService.FillFromPM(productProfile);
                productProfile = ProductsService.FillOtherPMProps(productProfile);

                inventory.ClientCode = (from t in db.tblClient
                                        where t.ClientID == productProfile.clientid
                                        select t.ClientCode).FirstOrDefault();

                inventory.ClientUM = (from t in db.tblClient
                                      where t.ClientID == productProfile.clientid
                                      select t.ClientUM).FirstOrDefault();

                inventory.Division = (from t in db.tblDivision
                                      where t.DivisionID == productProfile.divisionid
                                      select t.DivisionName).FirstOrDefault();

                var openBulkOrderItem = (from t in db.tblBulkOrderItem
                                         where t.ProductMasterID == productDetailId
                                         && t.Status == "OP"
                                         select t).FirstOrDefault();

                inventory.BackOrderPending = openBulkOrderItem == null ? false : true;

                var bulkOrderItemWeights = (from t in db.tblBulkOrderItem
                                            where t.ProductMasterID == productDetailId
                                            && t.Status == "OP"
                                            select new
                                            {
                                                total = t.Qty * t.Weight
                                            }).ToList();

                var bulkWeightTotal = (from t in bulkOrderItemWeights
                                       select t.total).Sum();

                inventory.BulkWeightCurrentlyOnOrder = Convert.ToDecimal(bulkWeightTotal);

                inventory.ShelfLevelTotal = StatusLevelShelf(productDetailId, "TOTAL");
                inventory.ShelfLevelAvail = StatusLevelShelf(productDetailId, "AVAIL");
                inventory.ShelfLevelTest = StatusLevelShelf(productDetailId, "TEST");
                inventory.ShelfLevelHold = StatusLevelShelf(productDetailId, "HOLD");
                inventory.ShelfLevelQC = StatusLevelShelf(productDetailId, "QC");
                inventory.ShelfLevelReturn = StatusLevelShelf(productDetailId, "RETURN");
                inventory.ShelfLevelRecd = StatusLevelShelf(productDetailId, "RECD");
                inventory.ShelfLevelOther = StatusLevelShelf(productDetailId, "OTHER");
                inventory.BulkLevelTotal = StatusLevelBulk(productDetailId, "TOTAL");
                inventory.BulkLevelAvail = StatusLevelBulk(productDetailId, "AVAIL");
                inventory.BulkLevelTest = StatusLevelBulk(productDetailId, "TEST");
                inventory.BulkLevelHold = StatusLevelBulk(productDetailId, "HOLD");
                inventory.BulkLevelQC = StatusLevelBulk(productDetailId, "QC");
                inventory.BulkLevelReturn = StatusLevelBulk(productDetailId, "RETURN");
                inventory.BulkLevelRecd = StatusLevelBulk(productDetailId, "RECD");
                inventory.BulkLevelOther = StatusLevelBulk(productDetailId, "OTHER");
                inventory.TotalLevelTotal = StatusLevelTotal(productDetailId, "TOTAL");
                inventory.TotalLevelAvail = StatusLevelTotal(productDetailId, "AVAIL");
                inventory.TotalLevelTest = StatusLevelTotal(productDetailId, "TEST");
                inventory.TotalLevelHold = StatusLevelTotal(productDetailId, "HOLD");
                inventory.TotalLevelQC = StatusLevelTotal(productDetailId, "QC");
                inventory.TotalLevelReturn = StatusLevelTotal(productDetailId, "RETURN");
                inventory.TotalLevelRecd = StatusLevelTotal(productDetailId, "RECD");
                inventory.TotalLevelOther = StatusLevelTotal(productDetailId, "OTHER");

                return inventory;
            }
        }

        public static void SaveInventory(Inventory inventory)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(inventory.PP.productdetailid);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);

                productMaster.AlertNotesPackout = inventory.PP.alertnotespackout;
                productMaster.AlertNotesReceiving = inventory.PP.alertnotesreceiving;
                productDetail.AlertNotesOrderEntry = inventory.PP.alertnotesorderentry;
                productDetail.AlertNotesShipping = inventory.PP.alertnotesshipping;

                db.SaveChanges();
            }
        }

        public static decimal StatusLevelShelf(int? productDetailId, string status)
        {
            decimal shelfStatusTotal;

            using (var db = new CMCSQL03Entities())
            {
                var productStock = (from stock in db.tblStock
                                    join shelf in db.tblShelfMaster on stock.ShelfID equals shelf.ShelfID
                                    where shelf.ProductDetailID == productDetailId
                                    && stock.StockID != null
                                    select new
                                    {
                                        status = stock.ShelfStatus,
                                        total = (stock.QtyOnHand == null ? 0 : stock.QtyOnHand) * (shelf.UnitWeight == null ? 0 : shelf.UnitWeight)
                                    }).ToList();

                switch (status)
                {
                    case "TOTAL":
                        var stockTotal = (from stock in productStock
                                          select stock.total).Sum();

                        shelfStatusTotal = Convert.ToDecimal(stockTotal);
                        break;

                    case "OTHER":
                        var ListOfStatus = (from stock in db.tblStock
                                            select stock.ShelfStatus).Distinct().ToList();

                        var otherStockTotal = (from stock in productStock
                                               where !ListOfStatus.Contains(stock.status)
                                               select stock.total).Sum();

                        shelfStatusTotal = Convert.ToDecimal(otherStockTotal);
                        break;

                    default:
                        var statusStockTotal = (from stock in productStock
                                                where stock.status == status
                                                select stock.total).Sum();

                        shelfStatusTotal = Convert.ToDecimal(statusStockTotal);
                        break;
                }

                shelfStatusTotal = Math.Round(shelfStatusTotal, 0);

                return shelfStatusTotal;
            }
        }

        public static decimal StatusLevelBulk(int productDetailId, string status)
        {
            decimal bulkStatusTotal;

            using (var db = new CMCSQL03Entities())
            {
                var productBulk = (from productDetail in db.tblProductDetail
                                   join productMaster in db.tblProductMaster on productDetail.ProductMasterID equals productMaster.ProductMasterID
                                   join bulk in db.tblBulk on productMaster.ProductMasterID equals bulk.ProductMasterID
                                   where productDetail.ProductDetailID == productDetailId
                                   select new
                                   {
                                       status = bulk.BulkStatus,
                                       total = (bulk.Qty == null ? 1 : bulk.Qty) * (bulk.CurrentWeight == null ? 0 : bulk.CurrentWeight)
                                   });

                switch (status)
                {
                    case "TOTAL":
                        var bulkTotal = (from bulk in productBulk
                                         select bulk.total).Sum();

                        bulkStatusTotal = Convert.ToDecimal(bulkTotal);
                        break;

                    case "OTHER":
                        var otherBulkTotal = (from bulk in productBulk
                                              where !(bulk.status == "AVAIL"
                                              || bulk.status == "TEST"
                                              || bulk.status == "HOLD"
                                              || bulk.status == "QC"
                                              || bulk.status == "RETURN"
                                              || bulk.status == "RECD")
                                              select bulk.total).Sum();

                        bulkStatusTotal = Convert.ToDecimal(otherBulkTotal);
                        break;

                    default:
                        var statusBulkTotal = (from bulk in productBulk
                                               where bulk.status == status
                                               select bulk.total).Sum();

                        bulkStatusTotal = Convert.ToDecimal(statusBulkTotal);
                        break;
                }

                bulkStatusTotal = Math.Round(bulkStatusTotal, 0);

                return bulkStatusTotal;
            }
        }

        public static decimal StatusLevelTotal(int productDetailId, string status)
        {
            decimal shelf = StatusLevelShelf(productDetailId, status);
            decimal bulk = StatusLevelBulk(productDetailId, status);

            var total = Math.Round((shelf + bulk), 1);

            return total;
        }

        public static StockViewModel FillStock(int stockId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productStock = (from stock in db.tblStock
                                    join shelf in db.tblShelfMaster on stock.ShelfID equals shelf.ShelfID
                                    join productDetail in db.tblProductDetail on shelf.ProductDetailID equals productDetail.ProductDetailID
                                    join bulk in db.tblBulk on stock.BulkID equals bulk.BulkID
                                    where stock.StockID == stockId
                                    select new StockViewModel
                                    {
                                        StockID = stock.StockID,
                                        ShelfID = stock.ShelfID,
                                        BulkID = stock.BulkID,
                                        Warehouse = stock.Warehouse,
                                        QtyOnHand = stock.QtyOnHand,
                                        QtyAvailable = stock.QtyAvailable,
                                        QtyAllocated = stock.QtyAllocated,
                                        Bin = stock.Bin,
                                        ShelfStatus = stock.ShelfStatus,
                                        WasteAccumStartDate = stock.WasteAccumStartDate,
                                        CreateDate = stock.CreateDate,
                                        CreateUser = stock.CreateUser,
                                        ProductDetailID = productDetail.ProductDetailID,
                                        ProductCode = productDetail.ProductCode,
                                        ProductName = productDetail.ProductName,
                                        LotNumber = bulk.LotNumber,
                                        ExpirationDate = bulk.ExpirationDate,
                                        MfgDate = bulk.MfgDate,
                                        QCDate = bulk.QCDate,
                                        CeaseShipDate = bulk.CeaseShipDate,
                                        Size = shelf.Size,
                                        UnitWeight = shelf.UnitWeight,
                                        UpdateDate = stock.UpdateDate,
                                        UpdateUser = stock.UpdateUser
                                    }).FirstOrDefault();

                productStock.ListOfShelfMasterIDs = ApplicationService.ddlShelfMasterIDs(productStock.ProductDetailID);

                return productStock;
            }
        }

        public static void SaveStock(StockViewModel stock)
        {
            using (var db = new CMCSQL03Entities())
            {
                var stockRecord = (from t in db.tblStock
                                   where t.StockID == stock.StockID
                                   select t).FirstOrDefault();

                stockRecord.Warehouse = stock.Warehouse;
                stockRecord.QtyOnHand = stock.QtyOnHand;
                stockRecord.Bin = stock.Bin;
                stockRecord.ShelfStatus = stock.ShelfStatus;
                stockRecord.WasteAccumStartDate = stock.WasteAccumStartDate;
                stockRecord.UpdateDate = DateTime.UtcNow;
                stockRecord.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }
        }

        public static void SavePrePackStock(PrePackStock prePackStock, FormCollection form)
        {
            // For each shelf master, insert record into tblStock
            prePackStock.BulkContainer.bulkid = NewBulkId();

            using (var db = new CMCSQL03Entities())
            {
                var bulk = db.tblBulk.Find(prePackStock.BulkContainer.bulkid);
                var productDetail = db.tblProductDetail.Find(prePackStock.ProductDetailID);

                bulk.ProductMasterID = productDetail.ProductMasterID;
                bulk.Qty = 1;
                bulk.ReceiveDate = prePackStock.BulkContainer.receivedate;
                bulk.LotNumber = prePackStock.BulkContainer.lotnumber;
                bulk.MfgDate = prePackStock.BulkContainer.mfgdate;
                bulk.ExpirationDate = prePackStock.BulkContainer.expirationdate;
                bulk.CeaseShipDate = prePackStock.BulkContainer.ceaseshipdate;
                bulk.QCDate = prePackStock.BulkContainer.qcdate;
                bulk.BulkStatus = prePackStock.BulkContainer.bulkstatus;
                bulk.Bin = prePackStock.BulkContainer.bin;
                bulk.Warehouse = prePackStock.BulkContainer.warehouse;
                bulk.CreateDate = DateTime.UtcNow;
                bulk.CreateUser = HttpContext.Current.User.Identity.Name;
                bulk.ReceiveWeight = 0;
                bulk.CurrentWeight = 0;

                for (int i = 1; i <= prePackStock.ShelfMasterCount; i++)
                {
                    int shelfId = Convert.ToInt32(form["Key" + i.ToString()]);
                    int shelfQty = Convert.ToInt32(form["Value" + i.ToString()]);

                    if (shelfQty > 0)
                    {
                        var newStock = new tblStock();

                        var shelf = (from t in db.tblShelfMaster
                                     where t.ShelfID == shelfId
                                     select t).FirstOrDefault();

                        newStock.ShelfID = shelfId;
                        newStock.BulkID = prePackStock.BulkContainer.bulkid;
                        newStock.Warehouse = prePackStock.BulkContainer.warehouse;
                        newStock.QtyOnHand = shelfQty;
                        bulk.ReceiveWeight = bulk.ReceiveWeight + (shelfQty * shelf.UnitWeight);
                        newStock.Bin = shelf.Bin;
                        newStock.ShelfStatus = "AVAIL";
                        newStock.CreateDate = DateTime.UtcNow;
                        newStock.CreateUser = HttpContext.Current.User.Identity.Name;

                        db.tblStock.Add(newStock);

                        db.SaveChanges();
                    }
                }
            }
        }

        public static int NewBulkId()
        {
            using (var db = new CMCSQL03Entities())
            {
                tblBulk bulk = new tblBulk();
                db.tblBulk.Add(bulk);
                db.SaveChanges();

                return bulk.BulkID;
            }
        }

        public static List<InventoryLogNote> ListInvPMLogNotes(int? masterId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var inventoryLogNotes = (from t in db.tblInvPMLogNote
                                         where t.ProductMasterID == masterId
                                         orderby t.InvPMLogNoteIDID descending
                                         select new InventoryLogNote
                                         {
                                             productnoteid = t.InvPMLogNoteIDID,
                                             productmasterid = t.ProductMasterID,
                                             notedate = t.NoteDate,
                                             notes = t.Notes,
                                             reasoncode = t.Comment,
                                             UpdateDate = t.UpdateDate,
                                             UpdateUser = t.UpdateUser,
                                             CreateDate = t.CreateDate,
                                             CreateUser = t.CreateUser
                                         }).ToList();

                return inventoryLogNotes;
            }
        }

        public static InventoryLogNote GetInventoryNote(int inventoryLogNoteId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var inventoryLogNote = new InventoryLogNote();

                var productLogNote = db.tblInvPMLogNote.Find(inventoryLogNoteId);

                inventoryLogNote.productnoteid = productLogNote.InvPMLogNoteIDID;
                inventoryLogNote.productmasterid = productLogNote.ProductMasterID;
                inventoryLogNote.reasoncode = productLogNote.Comment;
                inventoryLogNote.notedate = productLogNote.NoteDate;
                inventoryLogNote.notes = productLogNote.Notes;
                inventoryLogNote.UpdateDate = productLogNote.UpdateDate;
                inventoryLogNote.UpdateUser = productLogNote.UpdateUser;
                inventoryLogNote.CreateDate = productLogNote.CreateDate;
                inventoryLogNote.CreateUser = productLogNote.CreateUser;

                return inventoryLogNote;
            }
        }

        public static InventoryLogNote CreateInventoryLogNote(int? productMasterId)
        {
            var inventoryLogNote = new InventoryLogNote();

            inventoryLogNote.productnoteid = -1;
            inventoryLogNote.productmasterid = productMasterId;
            inventoryLogNote.reasoncode = null;
            inventoryLogNote.notedate = DateTime.UtcNow;
            inventoryLogNote.notes = null;
            inventoryLogNote.UpdateDate = DateTime.UtcNow;
            inventoryLogNote.UpdateUser = HttpContext.Current.User.Identity.Name;
            inventoryLogNote.CreateDate = DateTime.UtcNow;
            inventoryLogNote.CreateUser = HttpContext.Current.User.Identity.Name;

            return inventoryLogNote;
        }

        public static void SaveInventoryLogNote(InventoryLogNote inventoryLogNote)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (inventoryLogNote.productnoteid == -1)
                {
                    var newLogNote = new tblInvPMLogNote();
                    newLogNote.CreateDate = DateTime.UtcNow;
                    newLogNote.CreateUser = HttpContext.Current.User.Identity.Name;
                    db.tblInvPMLogNote.Add(newLogNote);

                    db.SaveChanges();

                    inventoryLogNote.productnoteid = newLogNote.InvPMLogNoteIDID;
                }

                var logNote = db.tblInvPMLogNote.Find(inventoryLogNote.productnoteid);

                logNote.ProductMasterID = inventoryLogNote.productmasterid;
                logNote.NoteDate = DateTime.UtcNow;
                logNote.Notes = inventoryLogNote.notes;
                logNote.Comment = inventoryLogNote.reasoncode;
                logNote.UpdateDate = DateTime.UtcNow;
                logNote.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.SaveChanges();
            }
        }

        public static void DeleteProductNote(int inventoryProductLogNoteId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "Delete from tblInvPMLogNote Where InvPMLogNoteIDID=" + inventoryProductLogNoteId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        /// <summary>
        /// Deduct item shipped from inventory (bulk/shelf).
        /// Reduce qty on hand and qty allocated.
        /// Possible that SR  items are deducted once production clears. Ignore SR for now.
        /// Log inventory changes
        /// </summary>
        public static void UpdateInventoryShipped(int orderitemid)
        {
            int orderItemId = orderitemid;
            var log = new InventoryLog();

            using (var db = new CMCSQL03Entities())
            {
                var orderItem = db.tblOrderItem.Find(orderItemId);
                var stock = db.tblStock.Find(orderItem.AllocatedStockID);

                if (true)
                {
                    stock.QtyAllocated = stock.QtyAllocated - orderItem.Qty;
                    db.SaveChanges();
                    
                    log.LogType = "SS-SHP";
                    InventoryService.LogInventoryUpdates(log);
                }
            }
        }

        /// <summary>
        /// Update inventory when product is received.
        /// Log inventory changes.
        /// </summary>
        public static void UpdateInventoryReceived()
        {

        }

        public static void LogInventoryUpdates(InventoryLog log)
        {
            using (var db = new CMCSQL03Entities())
            {
                var client = db.tblClient
                               .Where(x => x.ClientID == log.ClientId)
                               .Select(x => new { x.ClientName, x.ClientUM })
                               .FirstOrDefault();

                var InventoryLog = new tblInvLog();

                InventoryLog.LogType = log.LogType;
                InventoryLog.LogDate = DateTime.UtcNow;
                InventoryLog.BulkID = log.BulkId;
                InventoryLog.StockID = log.StockId;
                InventoryLog.ProductMasterID = log.ProductMasterId;
                InventoryLog.ProductDetailID = log.ProductDetailId;
                InventoryLog.ProductCode = log.ProductCode;
                InventoryLog.ProductName = log.ProductName;
                InventoryLog.MasterCode = log.MasterCode;
                InventoryLog.MasterName = log.MasterName;
                InventoryLog.Size = log.Size;
                InventoryLog.LogQty = log.LogQty;
                InventoryLog.LogAmount = log.LogAmount;
                InventoryLog.ClientID = log.ClientId;
                InventoryLog.ClientName = client.ClientName;
                InventoryLog.UM = client.ClientUM;
                InventoryLog.LogNotes = log.LogNotes;
                InventoryLog.Status = log.Status;
                InventoryLog.Warehouse = log.Warehouse;
                InventoryLog.Lotnumber = log.LotNumber;
                InventoryLog.BulkBin = log.BulkBin;
                InventoryLog.ShelfBin = log.ShelfBin;
                InventoryLog.ShipDate = log.ShipDate;
                InventoryLog.OrderNumber = log.OrderNumber;
                InventoryLog.CurrentQtyAvailable = log.CurrentQtyAvailable;
                InventoryLog.CurrentWeightAvailable = log.CurrentWeightAvailable;
                InventoryLog.ExpirationDate = log.ExpirationDate;
                InventoryLog.CeaseShipDate = log.CeaseShipDate;
                InventoryLog.DateReceived = log.DateReceived;
                InventoryLog.QCDate = log.QCDate;
                InventoryLog.PackOutID = log.PackOutId;
                InventoryLog.CreateDate = DateTime.UtcNow;
                InventoryLog.CreateUser = HttpContext.Current.User.Identity.Name;
                InventoryLog.UpdateDate = DateTime.UtcNow;
                InventoryLog.UpdateUser = HttpContext.Current.User.Identity.Name;

                db.tblInvLog.Add(InventoryLog);
                db.SaveChanges();
            }
        }
    }
}