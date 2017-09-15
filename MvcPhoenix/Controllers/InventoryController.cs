using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using Neodynamic.SDK.Web;
using PagedList;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class InventoryController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RecentBulkReceived()   // build partial for index, same as receiving Index
        {
            List<BulkContainerViewModel> bulkContainers = ReceivingService.fnIndexList();

            return PartialView("~/Views/Inventory/_RecentBulkReceived.cshtml", bulkContainers);
        }

        public ActionResult Edit(int id)    // Entry point for ProductDetailID from Search Results
        {
            var inventory = InventoryService.FillInventory(id);

            return View("~/Views/Inventory/Edit.cshtml", inventory);
        }

        [HttpPost]
        public ActionResult Save(Inventory inventory)
        {
            InventoryService.SaveInventory(inventory);

            return RedirectToAction("Edit", new { id = inventory.PP.productdetailid });
        }

        public ActionResult EditBulk(int id)
        {
            var bulk = BulkService.FillBulkContainer(id);

            return View("~/Views/Bulk/Edit.cshtml", bulk);
        }

        public ActionResult EditBulkOrder(int bulkorderid)
        {
            return RedirectToAction("Edit", "Replenishments", new { id = bulkorderid });
        }

        public ActionResult BulkStockList(int productmasterid, int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkContainers = new List<BulkContainerViewModel>();

                var bulkProducts = (from t in db.tblBulk
                                    where t.ProductMasterID == productmasterid
                                    select t).ToList();

                foreach (var row in bulkProducts)
                {
                    bulkContainers.Add(BulkService.FillBulkContainer(row.BulkID));
                }

                TempData["productdetailid"] = productdetailid;

                return PartialView("~/Views/Inventory/_BulkStock.cshtml", bulkContainers);
            }
        }

        public ActionResult ShelfStockList(int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var stocks = new List<StockViewModel>();

                var stockProducts = (from stock in db.tblStock
                                     join shelf in db.tblShelfMaster on stock.ShelfID equals shelf.ShelfID
                                     join productDetail in db.tblProductDetail on shelf.ProductDetailID equals productDetail.ProductDetailID
                                     join bulk in db.tblBulk on stock.BulkID equals bulk.BulkID
                                     where productDetail.ProductDetailID == productdetailid
                                     && stock.QtyOnHand > 0
                                     select stock).ToList();

                var productReference = (from productMaster in db.tblProductMaster
                                        join productDetail in db.tblProductDetail on productMaster.ProductMasterID equals productDetail.ProductMasterID
                                        where productDetail.ProductDetailID == productdetailid
                                        select productMaster).FirstOrDefault();

                foreach (var row in stockProducts)
                {
                    stocks.Add(InventoryService.FillStock(row.StockID));
                }

                ViewBag.ParentID = productdetailid;
                ViewBag.ShelfLife = productReference.ShelfLife;
                ViewBag.CeaseShipDays = productReference.CeaseShipDifferential;

                return PartialView("~/Views/Inventory/_ShelfStock.cshtml", stocks);
            }
        }

        public ActionResult EditStock(int shelfstockid)
        {
            var stock = InventoryService.FillStock(shelfstockid);

            return PartialView("~/Views/Inventory/_ShelfStockModal.cshtml", stock);
        }

        public ActionResult ShelfStockConvertToBulk(int shelfstockid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var shelfStock = db.tblStock.Find(shelfstockid);
                ViewBag.StockItemId = shelfStock.StockID;
                ViewBag.StockQty = shelfStock.QtyAvailable;

                return PartialView("~/Views/Inventory/_ShelfStockConvertToBulkModal.cshtml");
            }
        }

        [HttpPost]
        public ActionResult ShelfStockConvertToBulk(int stockItemId, int stockQuantity)
        {
            using (var db = new CMCSQL03Entities())
            {
                try
                {
                    var shelfStock = db.tblStock.Find(stockItemId);

                    if (shelfStock.QtyAvailable < stockQuantity || shelfStock.QtyAvailable < 1 || shelfStock.QtyAvailable == null)
                    {
                        return null;
                    }

                    shelfStock.QtyAvailable = shelfStock.QtyAvailable - stockQuantity;
                    shelfStock.QtyOnHand = shelfStock.QtyOnHand - stockQuantity;
                    shelfStock.UpdateDate = DateTime.UtcNow;
                    shelfStock.UpdateUser = HttpContext.User.Identity.Name;

                    int? bulkid = shelfStock.BulkID;
                    int? shelfid = shelfStock.ShelfID;

                    var getunitweight = db.tblShelfMaster.Find(shelfid);
                    decimal? unitweight = getunitweight.UnitWeight;

                    var bulkstock = db.tblBulk.Find(bulkid);
                    if (bulkstock.BulkStatus == "Virtual" || bulkstock.BulkStatus == "BF")
                    {
                        bulkstock.Bin = shelfStock.Bin;
                        bulkstock.BulkStatus = shelfStock.ShelfStatus;
                    }

                    decimal? shelfstockweight = stockQuantity * unitweight;
                    bulkstock.CurrentWeight = bulkstock.CurrentWeight + shelfstockweight;
                    bulkstock.UpdateDate = DateTime.UtcNow;
                    bulkstock.UpdateUser = HttpContext.User.Identity.Name;

                    db.SaveChanges();

                    // Write to inventory log
                    InventoryLogNote inventoryLogNote = new InventoryLogNote();

                    inventoryLogNote.productnoteid = -1;
                    inventoryLogNote.productmasterid = bulkstock.ProductMasterID;
                    inventoryLogNote.notes = String.Format("Converted ({0}) x {1} from shelf stock id: {2} to bulk stock id: {3}.",
                                                stockQuantity, getunitweight.Size, shelfStock.StockID, bulkstock.BulkID);
                    inventoryLogNote.reasoncode = "Change Packaging";

                    InventoryService.SaveInventoryLogNote(inventoryLogNote);

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public ActionResult CreatePrePackStock(int productDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(productDetailId);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);
                var client = db.tblClient.Find(productMaster.ClientID);

                PrePackStock prePackStock = new PrePackStock();

                prePackStock.ProductDetailID = productDetailId;
                prePackStock.BulkContainer = new BulkContainerViewModel();
                prePackStock.BulkContainer.bulkid = -1;
                prePackStock.BulkContainer.receivedate = DateTime.UtcNow;
                prePackStock.BulkContainer.warehouse = client.CMCLocation;
                prePackStock.BulkContainer.lotnumber = "lotnumber";
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

                return View("~/Views/Inventory/PrePackStock.cshtml", prePackStock);
            }
        }

        [HttpPost]
        public ActionResult SavePrePackStock(PrePackStock prePackStock, FormCollection form)
        {
            InventoryService.SavePrePackStock(prePackStock, form);

            return RedirectToAction("Edit", new { id = prePackStock.ProductDetailID });
        }

        [HttpPost]
        public ActionResult SaveStock(StockViewModel stock)
        {
            InventoryService.SaveStock(stock);

            return RedirectToAction("Edit", new { id = stock.ProductDetailID });
        }

        public ActionResult BulkOrdersList(int productDetailId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(productDetailId);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);
                var bulkOrderItem = (from items in db.tblBulkOrderItem
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

                ViewBag.ProductDetailID = productDetailId;

                return PartialView("~/Views/Inventory/_ReplenishOrders.cshtml", bulkOrderItem);
            }
        }

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new CMCSQL03Entities())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.CodeSortParm = String.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                if (searchString != null)
                {
                    page = 1;
                }
                else { searchString = currentFilter; }

                ViewBag.CurrentFilter = searchString;
                ViewBag.SearchString = searchString;

                var productCodes = from p in db.tblProductDetail select p;

                if (!String.IsNullOrEmpty(searchString))
                {
                    productCodes = productCodes.Where(p => p.ProductCode.Contains(searchString)
                        || p.ProductName.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "name":
                        productCodes = productCodes.OrderBy(p => p.ProductName);
                        break;

                    case "name_desc":
                        productCodes = productCodes.OrderByDescending(p => p.ProductName);
                        break;

                    case "code_desc":
                        productCodes = productCodes.OrderByDescending(p => p.ProductCode);
                        break;

                    default:
                        productCodes = productCodes.OrderBy(p => p.ProductCode);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(productCodes.ToPagedList(pageNumber, pageSize));
            }
        }

        #region Inventory Product Master Log Notes

        /// <summary>
        /// Takes in productdetailid but returns obj on master level.
        /// Changes reflect in related equivalents.
        /// id comes in as detail key; find master id equiv
        /// </summary>
        public ActionResult InventoryLogList(int productDetailId)
        {
            int? productMasterId = ProductsService.GetProductMasterId(productDetailId);
            var inventoryLogNotes = InventoryService.ListInvPMLogNotes(productMasterId);
            ViewBag.ParentKey = productDetailId;

            return PartialView("~/Views/Inventory/_InventoryLogNotes.cshtml", inventoryLogNotes);
        }

        /// <summary>
        /// Take in product detail id.
        /// Get product master id and create record.
        /// </summary>
        [HttpGet]
        public ActionResult CreateInventoryLogNote(int productDetailId)
        {
            int? productMasterId = ProductsService.GetProductMasterId(productDetailId);
            var inventoryLogNote = InventoryService.CreateInventoryLogNote(productMasterId);

            return PartialView("~/Views/Inventory/_InventoryLogNotesModal.cshtml", inventoryLogNote);
        }

        [HttpGet]
        public ActionResult EditInventoryLogNote(int logNoteId)
        {
            var inventoryLogNote = InventoryService.GetInventoryNote(logNoteId);

            return PartialView("~/Views/Inventory/_InventoryLogNotesModal.cshtml", inventoryLogNote);
        }

        [HttpPost]
        public ActionResult SaveInventoryLogNote(InventoryLogNote inventoryLogNote)
        {
            InventoryService.SaveInventoryLogNote(inventoryLogNote);

            return null;
        }

        [HttpGet]
        public ActionResult DeleteInventoryLogNote(int logNoteId)
        {
            InventoryService.DeleteProductNote(logNoteId);

            return null;
        }

        /// <summary>
        /// Displays log notes from product profile as readonly.
        /// Notes are distinct to the detail level.
        /// </summary>
        public ActionResult ProductLogList(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productLogNotes = (from t in db.tblPPPDLogNote
                                       where t.ProductDetailID == id
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

                ViewBag.ParentKey = id;

                return PartialView("~/Views/Inventory/_ProductLogNotes.cshtml", productLogNotes);
            }
        }

        #endregion Inventory Product Master Log Notes

        #region Label Printing

        [AllowAnonymous]
        public ActionResult PrintLabel()
        {
            return View();                                                      // Generate plain html label
        }

        // Anonymous access is REQUIRED for the callback from client print.
        [AllowAnonymous]
        public void LabelPrint()
        {
            int pagecopies = 1;
            string printerName = @"\\cmcnmps2\ZebraIT";                         // @"\\CMCNMPS2\RcvShelf"; @"AThermalZebraNet";

            var actionPDF = new Rotativa.ActionAsPdf("PrintLabel")
            {
                PageMargins = new Margins(10, 2, 2, 10),
                PageWidth = 200,
                PageHeight = 75,
                CustomSwitches = "--disable-smart-shrinking --load-error-handling ignore --copies " + pagecopies + ""
            };

            byte[] pdfContent = actionPDF.BuildPdf(ControllerContext);

            string fileName = "thermallabel.pdf";                               // Create a temp file name for our PDF report...
            PrintFile file = new PrintFile(pdfContent, fileName);               // Create a PrintFile object with the pdf report

            ClientPrintJob printJob = new ClientPrintJob();                     // Create a ClientPrintJob and send it back to the client!
            printJob.PrintFile = file;                                          // Set file to print...
            printJob.ClientPrinter = new InstalledPrinter(printerName);         // Set client printer...//cpj.ClientPrinter = new NetworkPrinter("10.0.0.8", 9100);

            printJob.SendToClient(System.Web.HttpContext.Current.Response);     // Send it...
        }

        // Web client call to ShelfStockPrint generates pdf and sends pdf stream to printer.
        // Stream pdf to printer.
        [AllowAnonymous]
        public void ShelfStockPrint(int shelfStockId, string pageCopies)
        {
            string printerName = @"\\cmcnmps2\ZebraIT";                         // Set printer name as installed on local pc or UNC eg; @"\\CMCNMPS2\RcvShelf"; @"AThermalZebraNet"; 

            var actionPDF = new Rotativa.ActionAsPdf("PrintShelfStockLabel", new { shelfstockid = shelfStockId })
            {
                PageMargins = new Margins(10, 2, 2, 10),
                PageWidth = 200,
                PageHeight = 75,
                CustomSwitches = "--disable-smart-shrinking --load-error-handling ignore --copies " + pageCopies + ""
            };

            byte[] pdfContent = actionPDF.BuildPdf(ControllerContext);          // PDF stream content

            string fileName = shelfStockId + ".pdf";                            // Set file and extension name
            PrintFile file = new PrintFile(pdfContent, fileName);               // Build file

            ClientPrintJob printJob = new ClientPrintJob();                     // Create a ClientPrintJob and send it back to the client!
            printJob.PrintFile = file;                                          // Set file to print
            printJob.ClientPrinter = new InstalledPrinter(printerName);         // Set client printer; new NetworkPrinter("192.168.0.60", 9100); Set IP printer: ipaddress, port

            printJob.SendToClient(System.Web.HttpContext.Current.Response);     // Send it
        }

        // Generate label as html view.
        // Used as template for pdf stream in ShelfStockPrint.
        [AllowAnonymous]
        public ActionResult PrintShelfStockLabel(int shelfstockid)
        {
            var stock = InventoryService.FillStock(shelfstockid);

            return View(stock);
        }

        // Generate PDF.
        public ActionResult ShelfStockLabel(int shelfstockid)
        {
            var stock = InventoryService.FillStock(shelfstockid);

            return new ViewAsPdf(stock)
            {
                // FileName = vm.ProductCode + vm.LotNumber + vm.Size + ".pdf",     // download using filename
                PageMargins = new Margins(2, 2, 0, 2),
                PageWidth = 200,
                PageHeight = 75,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }

        #endregion Label Printing
    }
}