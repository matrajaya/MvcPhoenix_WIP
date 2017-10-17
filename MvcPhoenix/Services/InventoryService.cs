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

            productProfile = ProductService.GetProductDetail(productProfile);
            productProfile = ProductService.GetProductMaster(productProfile);
            productProfile = ProductService.GetProductExtendedProps(productProfile);
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

        public static List<BulkOrderItemForInventory> GetBulkOrderItems(int productDetailId)
        {
            var bulkOrderItems = new List<BulkOrderItemForInventory>();

            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(productDetailId);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);

                bulkOrderItems = (from items in db.tblBulkOrderItem
                                  join orders in db.tblBulkOrder on items.BulkOrderID equals orders.BulkOrderID
                                  join pm in db.tblProductMaster on items.ProductMasterID equals pm.ProductMasterID
                                  where items.ProductMasterID == productMaster.ProductMasterID && items.Status == "OP"
                                  orderby orders.OrderDate
                                  select new BulkOrderItemForInventory
                                  {
                                      bulkorderitemid = items.BulkOrderItemID,
                                      bulkorderid = items.BulkOrderID,
                                      productmasterid = items.ProductMasterID,
                                      mastercode = pm.MasterCode,
                                      mastername = pm.MasterName,
                                      weight = items.Weight,
                                      itemstatus = items.Status,
                                      eta = items.ETA,
                                      datereceived = items.DateReceived,
                                      itemnotes = items.ItemNotes,
                                      OrderDate = orders.OrderDate,
                                      SupplyID = orders.SupplyID,
                                      OrderStatus = orders.Status,
                                      OrderComment = orders.Comment
                                  }).ToList();
            }
            return bulkOrderItems;
        }

        public static PrePackStock CreatePrepackedStock(int productDetailId)
        {
            var prePackStock = new PrePackStock();

            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(productDetailId);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);
                var client = db.tblClient.Find(productMaster.ClientID);

                prePackStock.ProductDetailID = productDetailId;
                prePackStock.BulkContainer = new BulkContainerViewModel();
                prePackStock.BulkContainer.bulkid = -1;
                prePackStock.BulkContainer.receivedate = DateTime.UtcNow;
                prePackStock.BulkContainer.warehouse = client.CMCLocation;
                prePackStock.BulkContainer.lotnumber = "";
                prePackStock.BulkContainer.mfgdate = DateTime.UtcNow;
                prePackStock.BulkContainer.clientid = productMaster.ClientID;
                prePackStock.BulkContainer.productmasterid = productMaster.ProductMasterID;
                prePackStock.BulkContainer.bulkstatus = "AVAIL";
                prePackStock.ProductCode = productDetail.ProductCode;
                prePackStock.ProductName = productDetail.ProductName;
                prePackStock.BulkContainer.bin = "PREPACK";
                prePackStock.ListOfShelfMasterIDs = (from t in db.tblShelfMaster
                                                     where t.ProductDetailID == productDetailId
                                                     && t.Discontinued == false
                                                     select new ShelfMasterViewModel
                                                     {
                                                         shelfid = t.ShelfID,
                                                         productdetailid = t.ProductDetailID,
                                                         bin = t.Bin,
                                                         size = t.Size
                                                     }).ToList();

                prePackStock.ShelfMasterCount = prePackStock.ListOfShelfMasterIDs.Count();
                prePackStock.BulkContainer.pm_ceaseshipdifferential = productMaster.CeaseShipDifferential;
                prePackStock.BulkContainer.pm_shelflife = productMaster.ShelfLife;
            }

            return prePackStock;
        }

        public static bool ConvertStockToBulk(int stockItemId, int stockQuantity)
        {
            using (var db = new CMCSQL03Entities())
            {
                try
                {
                    var shelfStock = db.tblStock.Find(stockItemId);

                    if (shelfStock.QtyAvailable < stockQuantity
                        || shelfStock.QtyAvailable < 1
                        || shelfStock.QtyAvailable == null)
                    {
                        return false;
                    }

                    shelfStock.QtyAvailable = shelfStock.QtyAvailable - stockQuantity;
                    shelfStock.QtyOnHand = shelfStock.QtyOnHand - stockQuantity;
                    shelfStock.UpdateDate = DateTime.UtcNow;
                    shelfStock.UpdateUser = HttpContext.Current.User.Identity.Name;

                    int? bulkid = shelfStock.BulkID;
                    var bulkstock = db.tblBulk.Find(bulkid);
                    if (bulkstock.BulkStatus == "Virtual" || bulkstock.BulkStatus == "BF")
                    {
                        bulkstock.Bin = shelfStock.Bin;
                        bulkstock.BulkStatus = shelfStock.ShelfStatus;
                    }

                    int? shelfid = shelfStock.ShelfID;
                    decimal? unitWeight = ShelfMasterService.GetUnitWeight(shelfid);
                    decimal? shelfstockweight = stockQuantity * unitWeight;
                    bulkstock.CurrentWeight = bulkstock.CurrentWeight + shelfstockweight;
                    bulkstock.UpdateDate = DateTime.UtcNow;
                    bulkstock.UpdateUser = HttpContext.Current.User.Identity.Name;

                    db.SaveChanges();

                    // Write to inventory log
                    InventoryLogNote inventoryLogNote = new InventoryLogNote();

                    inventoryLogNote.productnoteid = -1;
                    inventoryLogNote.productmasterid = bulkstock.ProductMasterID;
                    inventoryLogNote.notes = "Converted " + stockQuantity + " from shelf stock id: " + shelfStock.StockID + " to bulk stock id: " + bulkstock.BulkID + ".\n";
                    inventoryLogNote.reasoncode = "Change Packaging";

                    InventoryService.SaveInventoryLogNote(inventoryLogNote);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static List<BulkContainerViewModel> GetBulkStocks(int productmasterid)
        {
            var bulkContainers = new List<BulkContainerViewModel>();

            using (var db = new CMCSQL03Entities())
            {
                var bulkProducts = (from t in db.tblBulk
                                    where t.ProductMasterID == productmasterid
                                    select t).ToList();

                foreach (var row in bulkProducts)
                {
                    bulkContainers.Add(BulkService.GetBulkContainer(row.BulkID));
                }
            }

            return bulkContainers;
        }

        public static List<StockViewModel> GetShelfStocks(int productdetailid)
        {
            var stocks = new List<StockViewModel>();

            using (var db = new CMCSQL03Entities())
            {
                var stockProducts = (from stock in db.tblStock
                                     join shelf in db.tblShelfMaster on stock.ShelfID equals shelf.ShelfID
                                     join productDetail in db.tblProductDetail on shelf.ProductDetailID equals productDetail.ProductDetailID
                                     join bulk in db.tblBulk on stock.BulkID equals bulk.BulkID
                                     where productDetail.ProductDetailID == productdetailid
                                     && stock.QtyOnHand > 0
                                     select stock).ToList();

                foreach (var row in stockProducts)
                {
                    stocks.Add(InventoryService.GetStock(row.StockID));
                }
            }

            return stocks;
        }

        /// <summary>
        /// Get inventory log notes
        /// </summary>
        /// <param name="masterid"></param>
        /// <returns>List of log notes</returns>
        public static List<InventoryLogNote> GetProductMasterNotes(int? masterid)
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

        public static List<ProductNote> GetProductDetailNotes(int productDetailId)
        {
            var productLogNotes = new List<ProductNote>();

            using (var db = new CMCSQL03Entities())
            {
                productLogNotes = (from t in db.tblPPPDLogNote
                                   where t.ProductDetailID == productDetailId
                                   orderby t.NoteDate descending
                                   select new ProductNote
                                   {
                                       productnoteid = t.PPPDLogNoteID,
                                       productdetailid = t.ProductDetailID,
                                       notedate = t.NoteDate,
                                       notes = t.Notes,
                                       reasoncode = t.ReasonCode,
                                       UpdateDate = t.UpdateDate,
                                       UpdateUser = t.UpdateUser,
                                       CreateDate = t.CreateDate,
                                       CreateUser = t.CreateUser
                                   }).ToList();
            }
            return productLogNotes;
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
        
        public static int CreatePackOutOrder(int bulkid, int priority)
        {
            int packoutId = 0;

            using (var db = new CMCSQL03Entities())
            {
                var bulk = db.tblBulk.Find(bulkid);
                var productMaster = db.tblProductMaster.Find(bulk.ProductMasterID);
                var client = db.tblClient.Find(productMaster.ClientID);

                // check to see if there is an open packout already
                var packout = (from t in db.tblProductionMaster
                               where t.Company == client.CMCLongCustomer
                               && t.MasterCode == productMaster.MasterCode
                               && (t.ProductionStage == 10 | t.ProductionStage == 20)
                               select t).FirstOrDefault();

                if (packout != null)
                {
                    packoutId = -1;
                    return packoutId;
                }

                // Get a list of what we need to inform packout
                var packoutBulk = (from bulks in db.tblBulk
                                   join productmaster in db.tblProductMaster on bulks.ProductMasterID equals productmaster.ProductMasterID
                                   join productdetail in db.tblProductDetail on productmaster.ProductMasterID equals productdetail.ProductMasterID
                                   join shelfmaster in db.tblShelfMaster on productdetail.ProductDetailID equals shelfmaster.ProductDetailID
                                   where bulks.BulkID == bulkid
                                   select new { bulks, productdetail, productmaster, shelfmaster }).ToList();

                if (priority == 0)
                {
                    priority = ColorPriorityToday();
                }

                // Create new production master
                var newProductionMaster = new tblProductionMaster();
                db.tblProductionMaster.Add(newProductionMaster);
                db.SaveChanges();

                int newPackOutID = newProductionMaster.ID;

                // Insert production master
                var productionMaster = db.tblProductionMaster.Find(newPackOutID);

                productionMaster.BulkID = bulk.BulkID;
                productionMaster.ClientID = client.ClientID;
                productionMaster.CreateDate = DateTime.UtcNow;
                productionMaster.ProdmastCreateDate = DateTime.UtcNow;
                productionMaster.Company = client.CMCLongCustomer;
                productionMaster.Division = "N/A";
                productionMaster.MasterCode = productMaster.MasterCode;
                productionMaster.ProdName = productMaster.MasterName;
                productionMaster.Lot_Number = bulk.LotNumber;
                productionMaster.Bulk_Location = bulk.Bin;
                productionMaster.Contents_Weight = bulk.CurrentWeight;
                productionMaster.Shelf__Life = productMaster.ShelfLife;
                productionMaster.ExpDt = bulk.ExpirationDate;
                productionMaster.CeaseShipDate = bulk.CeaseShipDate;
                productionMaster.RecDate = bulk.ReceiveDate;
                productionMaster.Packout = true;
                productionMaster.Priority = priority;
                productionMaster.ProductionStage = 10;
                productionMaster.AirUN = "N/A";
                productionMaster.Status = bulk.BulkStatus;
                productionMaster.CMCUser = HttpContext.Current.User.Identity.Name;
                productionMaster.Heat_Prior_To_Filling = productMaster.HeatPriorToFilling;
                productionMaster.Moisture = productMaster.MoistureSensitive;
                productionMaster.CleanRoom = productMaster.CleanRoomEquipment;

                db.SaveChanges();

                packoutId = newPackOutID;

                // Insert production details
                foreach (var row in packoutBulk)
                {
                    var newProductionDetail = new tblProductionDetail();

                    newProductionDetail.MasterID = productionMaster.ID;
                    newProductionDetail.ShelfID = row.shelfmaster.ShelfID;
                    newProductionDetail.InvRequestedQty = 0;
                    newProductionDetail.ProdActualQty = 0;
                    newProductionDetail.LabelQty = 0;
                    newProductionDetail.UM = row.shelfmaster.Size;
                    newProductionDetail.Unit_Weight = row.shelfmaster.UnitWeight;

                    DateTime? oneYearAgo = DateTime.UtcNow.AddDays(-365);
                    DateTime? fourMonthsAgo = DateTime.UtcNow.AddDays(-120);

                    var log = (from t in db.tblInvLog
                               where t.LogType == "SS-SHP"
                               && t.ProductDetailID == row.productdetail.ProductDetailID
                               select t).ToList();

                    var shippedPastYear = log.Where(x => x.ShipDate >= oneYearAgo);
                    var shippedPastFourMonths = log.Where(x => x.ShipDate >= fourMonthsAgo);

                    int? totalShippedPastYear = shippedPastYear.Sum(x => x.LogQty);
                    int? totalShippedPastFourMonths = shippedPastFourMonths.Sum(x => x.LogQty);

                    decimal? reOrderMin = 0;

                    if (row.productmaster.ProductSetupDate <= DateTime.UtcNow.AddDays(-365))
                    {
                        reOrderMin = ((totalShippedPastYear / 12) * 2);
                    }
                    else
                    {
                        reOrderMin = (totalShippedPastFourMonths / 2);
                    }

                    newProductionDetail.SS_REORDMIN = reOrderMin;
                    newProductionDetail.SS_REORDMAX = (reOrderMin * 2);

                    var stock = db.tblStock.Where(x => x.ShelfID == row.shelfmaster.ShelfID
                                                    && x.ShelfStatus == "AVAIL");

                    newProductionDetail.OnHand = stock.Sum(x => x.QtyOnHand);
                    newProductionDetail.RecQty = newProductionDetail.SS_REORDMAX - newProductionDetail.OnHand;
                    newProductionDetail.Status = row.bulks.BulkStatus;
                    newProductionDetail.ProdCode = row.productdetail.ProductCode;
                    newProductionDetail.ProductName = row.productdetail.ProductName;
                    newProductionDetail.ShelfStockLocation = row.shelfmaster.Bin;

                    var package = db.tblPackage.Find(row.shelfmaster.PackageID);
                    newProductionDetail.PackagePartNumber = package.PartNumber;

                    db.tblProductionDetail.Add(newProductionDetail);
                    db.SaveChanges();
                }
            }

            return packoutId;
        }

        private static int ColorPriorityToday()
        {
            switch (System.DateTime.Today.DayOfWeek.ToString())
            {
                case "Monday":
                    return 2;
                    break;

                case "Tuesday":
                    return 3;
                    break;

                case "Wednesday":
                    return 3;
                    break;

                case "Thursday":
                    return 4;
                    break;

                case "Friday":
                    return 6;
                    break;

                default:
                    return 1;
            }
        }
    }
}