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
        public static Inventory GetProductInventory(int productdetailid)
        {
            var inventory = new Inventory();
            var productProfile = new ProductProfile();
            productProfile.productdetailid = productdetailid;

            productProfile = ProductsService.GetProductDetail(productProfile);
            productProfile = ProductsService.GetProductMaster(productProfile);
            productProfile = ProductsService.GetProductExtendedProps(productProfile);
            inventory.ProductProfile = productProfile;

            var client = ClientService.GetClient(productProfile.clientid);
            inventory.ClientCode = client.ClientCode;
            inventory.ClientUM = client.ClientUM;
            inventory.Division = ClientService.GetDivision(productProfile.divisionid).DivisionName;
            int? productMasterId = inventory.ProductProfile.productmasterid;

            using (var db = new CMCSQL03Entities())
            {
                var openBulkOrderItem = (from t in db.tblBulkOrderItem
                                         where t.ProductMasterID == productMasterId
                                         && t.Status == "OP"
                                         select t).FirstOrDefault();

                inventory.BackOrderPending = openBulkOrderItem == null ? false : true;

                var bulkOrderItemWeights = (from t in db.tblBulkOrderItem
                                            where t.ProductMasterID == productdetailid
                                            && t.Status == "OP"
                                            select new { total = t.Qty * t.Weight }).ToList();

                decimal bulkWeightTotal = Convert.ToDecimal(bulkOrderItemWeights.Select(x => x.total).Sum());

                inventory.BulkWeightCurrentlyOnOrder = bulkWeightTotal;

                inventory.ShelfLevelTotal = StatusLevelShelf(productdetailid, "TOTAL");
                inventory.ShelfLevelAvail = StatusLevelShelf(productdetailid, "AVAIL");
                inventory.ShelfLevelTest = StatusLevelShelf(productdetailid, "TEST");
                inventory.ShelfLevelHold = StatusLevelShelf(productdetailid, "HOLD");
                inventory.ShelfLevelQC = StatusLevelShelf(productdetailid, "QC");
                inventory.ShelfLevelReturn = StatusLevelShelf(productdetailid, "RETURN");
                inventory.ShelfLevelRecd = StatusLevelShelf(productdetailid, "RECD");
                inventory.ShelfLevelOther = StatusLevelShelf(productdetailid, "OTHER");
                inventory.BulkLevelTotal = StatusLevelBulk(productdetailid, "TOTAL");
                inventory.BulkLevelAvail = StatusLevelBulk(productdetailid, "AVAIL");
                inventory.BulkLevelTest = StatusLevelBulk(productdetailid, "TEST");
                inventory.BulkLevelHold = StatusLevelBulk(productdetailid, "HOLD");
                inventory.BulkLevelQC = StatusLevelBulk(productdetailid, "QC");
                inventory.BulkLevelReturn = StatusLevelBulk(productdetailid, "RETURN");
                inventory.BulkLevelRecd = StatusLevelBulk(productdetailid, "RECD");
                inventory.BulkLevelOther = StatusLevelBulk(productdetailid, "OTHER");
                inventory.TotalLevelTotal = StatusLevelTotal(productdetailid, "TOTAL");
                inventory.TotalLevelAvail = StatusLevelTotal(productdetailid, "AVAIL");
                inventory.TotalLevelTest = StatusLevelTotal(productdetailid, "TEST");
                inventory.TotalLevelHold = StatusLevelTotal(productdetailid, "HOLD");
                inventory.TotalLevelQC = StatusLevelTotal(productdetailid, "QC");
                inventory.TotalLevelReturn = StatusLevelTotal(productdetailid, "RETURN");
                inventory.TotalLevelRecd = StatusLevelTotal(productdetailid, "RECD");
                inventory.TotalLevelOther = StatusLevelTotal(productdetailid, "OTHER");
            }

            return inventory;
        }

        public static void SaveInventory(Inventory inventory)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(inventory.ProductProfile.productdetailid);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);

                productMaster.AlertNotesPackout = inventory.ProductProfile.alertnotespackout;
                productMaster.AlertNotesReceiving = inventory.ProductProfile.alertnotesreceiving;
                productDetail.AlertNotesOrderEntry = inventory.ProductProfile.alertnotesorderentry;
                productDetail.AlertNotesShipping = inventory.ProductProfile.alertnotesshipping;

                db.SaveChanges();
            }
        }

        public static decimal StatusLevelShelf(int? productDetailId, string status)
        {
            decimal shelfStatusTotal = 0;

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
                        shelfStatusTotal = (from stock in productStock
                                            select stock.total).Sum() ?? 0;
                        break;

                    case "OTHER":
                        var ListOfStatus = (from stock in db.tblStock
                                            select stock.ShelfStatus).Distinct().ToList();

                        shelfStatusTotal = (from stock in productStock
                                            where !ListOfStatus.Contains(stock.status)
                                            select stock.total).Sum() ?? 0;
                        break;

                    default:
                        shelfStatusTotal = (from stock in productStock
                                            where stock.status == status
                                            select stock.total).Sum() ?? 0;
                        break;
                }

                shelfStatusTotal = Math.Round(shelfStatusTotal, 0);

                return shelfStatusTotal;
            }
        }

        public static decimal StatusLevelBulk(int productDetailId, string status)
        {
            decimal bulkStatusTotal = 0;

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
                        bulkStatusTotal = (from bulk in productBulk
                                           select bulk.total).Sum() ?? 0;
                        break;

                    case "OTHER":
                        bulkStatusTotal = (from bulk in productBulk
                                           where !(bulk.status == "AVAIL"
                                           || bulk.status == "TEST"
                                           || bulk.status == "HOLD"
                                           || bulk.status == "QC"
                                           || bulk.status == "RETURN"
                                           || bulk.status == "RECD")
                                           select bulk.total).Sum() ?? 0;
                        break;

                    default:
                        bulkStatusTotal = (from bulk in productBulk
                                           where bulk.status == status
                                           select bulk.total).Sum() ?? 0;
                        break;
                }

                bulkStatusTotal = Math.Round(bulkStatusTotal, 0);
            }

            return bulkStatusTotal;
        }

        public static decimal StatusLevelTotal(int productDetailId, string status)
        {
            decimal shelf = StatusLevelShelf(productDetailId, status);
            decimal bulk = StatusLevelBulk(productDetailId, status);

            var total = Math.Round((shelf + bulk), 1);

            return total;
        }

        public static StockViewModel GetStock(int stockId)
        {
            var productStock = new StockViewModel();

            using (var db = new CMCSQL03Entities())
            {
                productStock = (from stock in db.tblStock
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

                int productDetailId = Convert.ToInt32(productStock.ProductDetailID);
                productStock.ListOfShelfMasterIDs = ApplicationService.ddlShelfMasterIDs(productDetailId);
            }

            return productStock;
        }

        public static List<ReturnStockViewModel> GetStockItems(int clientid)
        {
            var stockItems = new List<ReturnStockViewModel>();

            using (var db = new CMCSQL03Entities())
            {
                stockItems = (from stock in db.tblStock
                              join shelfmaster in db.tblShelfMaster on stock.ShelfID equals shelfmaster.ShelfID
                              join productdetail in db.tblProductDetail on shelfmaster.ProductDetailID equals productdetail.ProductDetailID
                              join productmaster in db.tblProductMaster on productdetail.ProductMasterID equals productmaster.ProductMasterID
                              join bulk in db.tblBulk on stock.BulkID equals bulk.BulkID
                              join division in db.tblDivision on productdetail.DivisionID equals division.DivisionID
                              where productmaster.ClientID == clientid
                              orderby productdetail.ProductCode
                              select new ReturnStockViewModel
                              {
                                  ClientID = productmaster.ClientID,
                                  StockID = stock.StockID,
                                  LotNumber = bulk.LotNumber,
                                  ProductCode = productdetail.ProductCode,
                                  ProductName = productdetail.ProductName,
                                  Warehouse = stock.Warehouse,
                                  Bin = stock.Bin,
                                  QtyOnHand = stock.QtyOnHand,
                                  Size = shelfmaster.Size,
                                  CurrentWeight = shelfmaster.UnitWeight * stock.QtyOnHand,
                                  ExpirationDate = bulk.ExpirationDate,
                                  ShelfStatus = stock.ShelfStatus,
                                  markedforreturn = stock.MarkedForReturn,
                                  divisionid = productdetail.DivisionID,
                                  divisionname = division.DivisionName
                              }).ToList();
            }

            return stockItems;
        }

        public static void SaveStock(StockViewModel stock)
        {
            using (var db = new CMCSQL03Entities())
            {
                var stockRecord = db.tblStock.Find(stock.StockID);

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
                var userName = HttpContext.Current.User.Identity.Name;
                var bulk = db.tblBulk.Find(prePackStock.BulkContainer.bulkid);
                var productDetail = db.tblProductDetail.Find(prePackStock.ProductDetailID);

                bulk.ProductMasterID = productDetail.ProductMasterID;
                bulk.ReceiveDate = prePackStock.BulkContainer.receivedate;
                bulk.LotNumber = prePackStock.BulkContainer.lotnumber;
                bulk.MfgDate = prePackStock.BulkContainer.mfgdate;
                bulk.ExpirationDate = prePackStock.BulkContainer.expirationdate;
                bulk.CeaseShipDate = prePackStock.BulkContainer.ceaseshipdate;
                bulk.QCDate = prePackStock.BulkContainer.qcdate;
                bulk.BulkStatus = prePackStock.BulkContainer.bulkstatus;
                bulk.Bin = prePackStock.BulkContainer.bin;
                bulk.Warehouse = prePackStock.BulkContainer.warehouse;
                bulk.ReceiveWeight = 0;
                bulk.CurrentWeight = 0;
                bulk.Qty = 1;
                bulk.CreateDate = DateTime.UtcNow;
                bulk.CreateUser = userName;

                for (int i = 1; i <= prePackStock.ShelfMasterCount; i++)
                {
                    int shelfId = Convert.ToInt32(form["Key" + i.ToString()]);
                    int shelfQty = Convert.ToInt32(form["Value" + i.ToString()]);

                    if (shelfQty > 0)
                    {
                        var shelf = db.tblShelfMaster.Find(shelfId);

                        var newStock = new tblStock();

                        newStock.ShelfID = shelfId;
                        newStock.BulkID = prePackStock.BulkContainer.bulkid;
                        newStock.Warehouse = prePackStock.BulkContainer.warehouse;
                        newStock.QtyOnHand = shelfQty;
                        bulk.ReceiveWeight += bulk.ReceiveWeight + (shelfQty * shelf.UnitWeight);
                        bulk.CurrentWeight += bulk.ReceiveWeight;
                        newStock.Bin = shelf.Bin;
                        newStock.ShelfStatus = "AVAIL";
                        newStock.CreateDate = DateTime.UtcNow;
                        newStock.CreateUser = userName;
                        newStock.UpdateDate = DateTime.UtcNow;
                        newStock.UpdateUser = userName;

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

        /// <summary>
        /// Get inventory log notes
        /// </summary>
        /// <param name="masterid"></param>
        /// <returns>List of log notes</returns>
        public static List<InventoryLogNote> ListInvPMLogNotes(int? masterid)
        {
            var logNotes = new List<InventoryLogNote>();

            using (var db = new CMCSQL03Entities())
            {
                logNotes = (from t in db.tblInvPMLogNote
                            where t.ProductMasterID == masterid
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
            }

            return logNotes;
        }

        public static InventoryLogNote GetInventoryNote(int inventorylognoteid)
        {
            var inventoryLogNote = new InventoryLogNote();

            using (var db = new CMCSQL03Entities())
            {
                var productLogNote = db.tblInvPMLogNote.Find(inventorylognoteid);

                inventoryLogNote.productnoteid = productLogNote.InvPMLogNoteIDID;
                inventoryLogNote.productmasterid = productLogNote.ProductMasterID;
                inventoryLogNote.reasoncode = productLogNote.Comment;
                inventoryLogNote.notedate = productLogNote.NoteDate;
                inventoryLogNote.notes = productLogNote.Notes;
                inventoryLogNote.UpdateDate = productLogNote.UpdateDate;
                inventoryLogNote.UpdateUser = productLogNote.UpdateUser;
                inventoryLogNote.CreateDate = productLogNote.CreateDate;
                inventoryLogNote.CreateUser = productLogNote.CreateUser;
            }

            return inventoryLogNote;
        }

        /// <summary>
        /// Create new empty model for product master inventory note
        /// </summary>
        /// <param name="productMasterId"></param>
        /// <returns>placeholder inventory log note</returns>
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

        /// <summary>
        /// Save product master inventory log note
        /// </summary>
        /// <param name="inventoryLogNote"></param>
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

        /// <summary>
        /// Delete product master inventory log note
        /// </summary>
        /// <param name="inventoryProductLogNoteId"></param>
        public static void DeleteProductNote(int inventoryProductLogNoteId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = "Delete from tblInvPMLogNote Where InvPMLogNoteIDID=" + inventoryProductLogNoteId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        /// <summary>
        /// Deduct quantity on hand and allocated from inventory when shelf stock item is shipped.
        /// Special request and bulk sizes get taken out when allocated/processed.
        /// </summary>
        public static void UpdateInventoryShipped(int orderitemid)
        {
            int orderItemId = orderitemid;

            using (var db = new CMCSQL03Entities())
            {
                var orderItem = db.tblOrderItem.Find(orderItemId);
                var stock = db.tblStock.Find(orderItem.AllocatedStockID);

                int? clientId = db.tblOrderMaster
                                  .Where(x => x.OrderID == orderItem.OrderID)
                                  .Select(x => x.ClientID)
                                  .FirstOrDefault();

                var bulk = db.tblBulk
                             .Where(x => x.BulkID == stock.BulkID)
                             .Select(x => new { x.Bin, x.ReceiveDate })
                             .FirstOrDefault();

                decimal unitWeight = ShelfMasterService.GetUnitWeight(orderItem.ShelfID);

                if (orderItem.ShipDate != null && stock != null)
                {
                    stock.QtyAllocated -= orderItem.Qty;
                    stock.QtyOnHand -= orderItem.Qty;

                    db.SaveChanges();

                    // Insert inventory log record
                    var log = new InventoryLog();

                    log.LogType = "SS-SHP";
                    log.BulkId = stock.BulkID;
                    log.StockId = stock.StockID;
                    log.ProductDetailId = orderItem.ProductDetailID;
                    log.Size = orderItem.Size;
                    log.LogQty = orderItem.Qty;
                    log.LogAmount = orderItem.Qty * unitWeight;
                    log.ClientId = clientId;
                    log.LogNotes = "Shipped shelf stock item";
                    log.Status = stock.ShelfStatus;
                    log.Warehouse = orderItem.Warehouse;
                    log.LotNumber = orderItem.LotNumber;
                    log.BulkBin = bulk.Bin;
                    log.ShelfBin = stock.Bin;
                    log.ShipDate = orderItem.ShipDate;
                    log.OrderNumber = orderItem.OrderID;
                    log.CurrentQtyAvailable = stock.QtyAvailable;
                    log.CurrentWeightAvailable = stock.QtyAvailable * unitWeight;
                    log.ExpirationDate = orderItem.ExpirationDate;
                    log.CeaseShipDate = orderItem.CeaseShipDate;
                    log.DateReceived = bulk.ReceiveDate;
                    log.PackOutId = orderItem.PackID;

                    InventoryService.LogInventoryUpdates(log);
                }
            }
        }

        /// <summary>
        /// Update inventory when product is received.
        /// </summary>
        public static void UpdateInventoryReceived()
        {
        }

        /// <summary>
        /// Update inventory when product is packed out for special requests.
        /// </summary>
        public static void UpdateInventoryOnPackout()
        {
        }

        /// <summary>
        /// Record inventory log entries
        /// </summary>
        /// <param name="log"></param>
        public static void LogInventoryUpdates(InventoryLog log)
        {
            int clientId = Convert.ToInt32(log.ClientId);
            var client = ClientService.GetClient(clientId);

            using (var db = new CMCSQL03Entities())
            {
                var InventoryLog = new tblInvLog();

                InventoryLog.LogType = log.LogType;
                InventoryLog.LogDate = DateTime.UtcNow;
                InventoryLog.BulkID = log.BulkId;
                InventoryLog.StockID = log.StockId;
                InventoryLog.ProductDetailID = log.ProductDetailId;
                InventoryLog.ProductCode = log.ProductCode;
                InventoryLog.ProductName = log.ProductName;
                InventoryLog.ProductMasterID = log.ProductMasterId;
                InventoryLog.MasterCode = log.MasterCode;
                InventoryLog.MasterName = log.MasterName;
                InventoryLog.Size = log.Size;
                InventoryLog.LogQty = log.LogQty;
                InventoryLog.LogAmount = log.LogAmount;
                InventoryLog.ClientID = client.ClientID;
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