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

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CodeSortParm = String.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            using (var db = new CMCSQL03Entities())
            {
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

        public ActionResult Edit(int id)
        {
            int productDetailId = id;
            var inventory = InventoryService.GetProductInventory(productDetailId);

            return View("~/Views/Inventory/Edit.cshtml", inventory);
        }

        [HttpPost]
        public ActionResult Save(Inventory inventory)
        {
            InventoryService.SaveInventory(inventory);
            int productDetailId = inventory.ProductProfile.productdetailid;

            return RedirectToAction("Edit", new { id = productDetailId });
        }

        #region Bulk Stock

        public ActionResult BulkStockList(int productmasterid, int productdetailid)
        {
            var bulkContainers = InventoryService.GetBulkStocks(productmasterid);

            TempData["productdetailid"] = productdetailid;

            return PartialView("~/Views/Inventory/_BulkStock.cshtml", bulkContainers);
        }
        
        public ActionResult EditBulk(int id)
        {
            int bulkId = id;
            var bulk = BulkService.GetBulkContainer(bulkId);

            return View("~/Views/Bulk/Edit.cshtml", bulk);
        }



        #endregion
        
        #region Shelf Stock

        public ActionResult ShelfStockList(int productdetailid)
        {
            var stocks = InventoryService.GetShelfStocks(productdetailid);

            var productReference = ProductService.GetProductMasterReference(productdetailid);

            ViewBag.ParentID = productdetailid;
            ViewBag.ShelfLife = productReference.ShelfLife + " months";
            ViewBag.CeaseShipDays = productReference.CeaseShipDifferential + " days";

            return PartialView("~/Views/Inventory/_ShelfStock.cshtml", stocks);
        }

        public ActionResult EditStock(int shelfstockid)
        {
            var stock = InventoryService.GetStock(shelfstockid);

            return PartialView("~/Views/Inventory/_ShelfStockModal.cshtml", stock);
        }

        [HttpPost]
        public ActionResult SaveStock(StockViewModel stock)
        {
            InventoryService.SaveStock(stock);

            int? productDetailId = stock.ProductDetailID;

            if (productDetailId < 1)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit", new { id = productDetailId });
        }
                
        #endregion

        #region Convert Stock

        public ActionResult Packout(int id, int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkId = id;
                var bulkContainer = BulkService.GetBulkContainer(bulkId);

                TempData["productdetailid"] = productdetailid;

                return View("~/Views/Inventory/_Packout.cshtml", bulkContainer);
            }
        }
        
        public ActionResult CreatePackout(BulkContainerViewModel bulkcontainer, FormCollection form)
        {
            int productDetailId = Convert.ToInt32(form["productdetailid"]);
            int priority = Convert.ToInt32(form["priority"]);

            if (String.IsNullOrEmpty(form["priority"]))
            {
                TempData["ResultMessage"] = "Please select a Priority value";
                return RedirectToAction("Packout", new { id = bulkcontainer.bulkid, productdetailid = productDetailId });
            }

            int PackOutResult = InventoryService.CreatePackOutOrder(bulkcontainer.bulkid, priority);
            
            if (PackOutResult > 0)
            {
                TempData["ResultMessage"] = "New packout order number " + PackOutResult.ToString() + " successfully created on " + DateTime.UtcNow.ToString("R");
                return RedirectToAction("Packout", new { id = bulkcontainer.bulkid, productdetailid = productDetailId });
            }

            if (PackOutResult == -1)
            {
                TempData["ResultMessage"] = "There is already an existing Pack Out order for the selected Bulk item";
                return RedirectToAction("Packout", new { id = bulkcontainer.bulkid, productdetailid = productDetailId });
            }

            if (PackOutResult == 0)
            {
                TempData["ResultMessage"] = "An error occurred trying to create a Pack Out on " + DateTime.UtcNow.ToString("R");
            }

            return RedirectToAction("Edit", "Inventory", new { id = productDetailId });
        }

        public ActionResult ShelfStockConvertToBulk(int shelfstockid)
        {
            var shelfStock = InventoryService.GetStock(shelfstockid);

            ViewBag.StockItemId = shelfStock.StockID;
            ViewBag.StockQty = shelfStock.QtyAvailable;

            return PartialView("~/Views/Inventory/_ShelfStockConvertToBulkModal.cshtml");
        }

        [HttpPost]
        public ActionResult ShelfStockConvertToBulk(int stockItemId, int stockQuantity)
        {
            bool result = InventoryService.ConvertStockToBulk(stockItemId, stockQuantity);

            return null;
        }

        #endregion

        public ActionResult CreatePrePackStock(int productDetailId)
        {
            var prePackStock = InventoryService.CreatePrepackedStock(productDetailId);

            return View("~/Views/Inventory/PrePackStock.cshtml", prePackStock);
        }

        [HttpPost]
        public ActionResult SavePrePackStock(PrePackStock prePackStock, FormCollection form)
        {
            InventoryService.SavePrePackStock(prePackStock, form);

            return RedirectToAction("Edit", new { id = prePackStock.ProductDetailID });
        }

        #region Bulk Order Replenishment

        public ActionResult BulkOrdersList(int productDetailId)
        {
            var bulkOrderItems = InventoryService.GetBulkOrderItems(productDetailId);

            ViewBag.ProductDetailID = productDetailId;

            return PartialView("~/Views/Inventory/_ReplenishOrders.cshtml", bulkOrderItems);
        }

        public ActionResult EditBulkOrder(int bulkorderid)
        {
            return RedirectToAction("Edit", "Replenishments", new { id = bulkorderid });
        }

        #endregion

        #region Product Log Notes

        /// <summary>
        /// Takes in productdetailid but returns obj on master level. Product master notes.
        /// Changes reflect in related equivalents.
        /// id comes in as detail key; find master id equiv
        /// </summary>
        public ActionResult InventoryLogList(int productDetailId)
        {
            int? productMasterId = ProductService.GetProductMasterId(productDetailId);
            var inventoryLogNotes = InventoryService.GetProductMasterNotes(productMasterId);

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
            int? productMasterId = ProductService.GetProductMasterId(productDetailId);
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
            int productDetailId = id;
            var productLogNotes = InventoryService.GetProductDetailNotes(productDetailId);

            ViewBag.ParentKey = productDetailId;

            return PartialView("~/Views/Inventory/_ProductLogNotes.cshtml", productLogNotes);
        }

        #endregion Product Log Notes

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
            var stock = InventoryService.GetStock(shelfstockid);

            return View(stock);
        }

        // Generate PDF.
        public ActionResult ShelfStockLabel(int shelfstockid)
        {
            var stock = InventoryService.GetStock(shelfstockid);

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