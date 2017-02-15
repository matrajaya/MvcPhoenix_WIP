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
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RecentBulkReceived()   // build partial for index, same as receiving Index
        {
            List<BulkContainerViewModel> mylist = ReceivingService.fnIndexList();
            return PartialView("~/Views/Inventory/_RecentBulkReceived.cshtml", mylist);
        }

        public ActionResult Edit(int id)    // Entry point for ProductDetailID from Search Results
        {
            var vm = InventoryService.fnFillInventoryVM(id);
            return View("~/Views/Inventory/Edit.cshtml", vm);
        }

        [HttpPost]
        public ActionResult Save(Inventory vm)
        {
            InventoryService.fnSaveInventory(vm);
            return RedirectToAction("Edit", new { id = vm.PP.productdetailid });
        }

        public ActionResult EditBulk(int id)   // redirect to bulk edit
        {
            var vm = BulkService.fnFillBulkContainerFromDB(id);
            return View("~/Views/Bulk/Edit.cshtml", vm);
        }

        public ActionResult EditBulkOrder(int id)
        {
            return RedirectToAction("Edit", "Replenishments", new { id = id });
        }

        public ActionResult BulkStockList(int id, int productdetailid)  // return partial
        {
            // fill a list<BulkContainerViewModel>
            var vm = new List<BulkContainerViewModel>();

            var q = (from t in db.tblBulk
                     where t.ProductMasterID == id
                     select t).ToList();

            foreach (var row in q)
            {
                vm.Add(BulkService.fnFillBulkContainerFromDB(row.BulkID));
            }
			
			// pc 12/27 added for use by CreateNewPackout so can return to Inv Edit
            TempData["productdetailid"] = productdetailid;
            return PartialView("~/Views/Inventory/_BulkStock.cshtml", vm);
        }

        public ActionResult ShelfStockList(int id)  // return partial
        {
            var vm = new List<StockViewModel>();
            var q = (from t in db.tblStock
                     join sm in db.tblShelfMaster on t.ShelfID equals sm.ShelfID
                     join pd in db.tblProductDetail on sm.ProductDetailID equals pd.ProductDetailID
                     join bk in db.tblBulk on t.BulkID equals bk.BulkID
                     where pd.ProductDetailID == id && t.QtyOnHand > 0
                     select t).ToList();

            foreach (var row in q)
            {
                vm.Add(InventoryService.fnFillStockViewModel(row.StockID));
            }

            ViewBag.ParentID = id;

            return PartialView("~/Views/Inventory/_ShelfStock.cshtml", vm);
        }

        public ActionResult EditStock(int id)
        {
            var vm = InventoryService.fnFillStockViewModel(id);
            return PartialView("~/Views/Inventory/_ShelfStockModal.cshtml", vm);
        }

        public ActionResult CreatePrePackStock(int id) //id=productdetailid
        {
            var pd = db.tblProductDetail.Find(id);
            var pm = db.tblProductMaster.Find(pd.ProductMasterID);
            var cl = db.tblClient.Find(pm.ClientID);

            PrePackStock vm = new PrePackStock();

            vm.ProductDetailID = id;
            vm.BulkContainer = new BulkContainerViewModel();
            vm.BulkContainer.bulkid = -1;
            vm.BulkContainer.receivedate = DateTime.UtcNow;
            vm.BulkContainer.warehouse = cl.CMCLocation;
            vm.BulkContainer.lotnumber = "lotnumber";
            vm.BulkContainer.mfgdate = DateTime.UtcNow;
            vm.BulkContainer.clientid = pm.ClientID;
            vm.BulkContainer.productmasterid = pm.ProductMasterID;
            vm.BulkContainer.bulkstatus = "AVAIL";
            vm.ProductCode = pd.ProductCode;
            vm.ProductName = pd.ProductName;
            vm.BulkContainer.bin = "PREPACK";
            vm.ListOfShelfMasterIDs = (from t in db.tblShelfMaster
                                       where t.ProductDetailID == id && t.Discontinued == false
                                       select new ShelfMasterViewModel
                                       {
                                           shelfid = t.ShelfID,
                                           productdetailid = t.ProductDetailID,
                                           bin = t.Bin,
                                           size = t.Size
                                       }).ToList();

            vm.ShelfMasterCount = vm.ListOfShelfMasterIDs.Count();

            return View("~/Views/Inventory/PrePackStock.cshtml", vm);
        }

        [HttpPost]
        public ActionResult SavePrePackStock(PrePackStock vm, FormCollection fc)
        {
            InventoryService.fnSavePrePackStock(vm, fc);

            return RedirectToAction("Edit", new { id = vm.ProductDetailID });
        }

        [HttpPost]
        public ActionResult SaveStock(StockViewModel vm)
        {
            InventoryService.fnSaveStock(vm);

            return RedirectToAction("Edit", new { id = vm.ProductDetailID });
        }

        public ActionResult BulkOrdersList(int id)  // id=productdetailid
        {
            using (db)
            {
                var pd = db.tblProductDetail.Find(id);
                var pmx = db.tblProductMaster.Find(pd.ProductMasterID);
                var vm = (from items in db.tblBulkOrderItem
                          join orders in db.tblBulkOrder on items.BulkOrderID equals orders.BulkOrderID
                          join pm in db.tblProductMaster on items.ProductMasterID equals pm.ProductMasterID
                          //where items.ProductMasterID == id && items.Status == "OP"
                          where items.ProductMasterID == pmx.ProductMasterID && items.Status == "OP"
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

                // 09/02 add viewbag
                // we need the clientid to pass to replenishments controller if user wants to create a new bulk order
                var pm2 = db.tblProductMaster.Find(id);
                ViewBag.ProductDetailID = id;

                return PartialView("~/Views/Inventory/_ReplenishOrders.cshtml", vm);
            }
        }

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new EF.CMCSQL03Entities())
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
        /// </summary>
        public ActionResult InventoryLogList(int id)
        {
            //id comes in as detail key
            //find master id equiv
            var masterid = GetProductMasterId(id);
            var obj = InventoryService.ListInvPMLogNotes(masterid);
            ViewBag.ParentKey = id;

            return PartialView("~/Views/Inventory/_InventoryLogNotes.cshtml", obj);
        }

        private int? GetProductMasterId(int id)
        {
            var masterid = (from t in db.tblProductDetail
                            where t.ProductDetailID == id
                            select t.ProductMasterID).FirstOrDefault();

            return masterid;
        }

        /// <summary>
        /// Take in product detail id
        /// Get product master id and create record
        /// </summary>
        [HttpGet]
        public ActionResult CreateInventoryLogNote(int id)
        {
            var masterid = Convert.ToInt32(GetProductMasterId(id));
            var obj = InventoryService.fnCreateInventoryLogNote(masterid);

            return PartialView("~/Views/Inventory/_InventoryLogNotesModal.cshtml", obj);
        }

        [HttpGet]
        public ActionResult EditInventoryLogNote(int id)
        {
            var obj = InventoryService.fnGetInventoryNote(id);
            return PartialView("~/Views/Inventory/_InventoryLogNotesModal.cshtml", obj);
        }

        [HttpPost]
        public ActionResult SaveInventoryLogNote(InventoryLogNote obj)
        {
            int pk = InventoryService.fnSaveInventoryLogNote(obj);
            return null;
        }

        [HttpGet]
        public ActionResult DeleteInventoryLogNote(int id, int ParentID)
        {
            int pk = InventoryService.fnDeleteProductNote(id);
            return null;
        }

        /// <summary>
        /// Displays log notes from product profile as readonly.
        /// Notes are distinct to the detail level.
        /// </summary>
        public ActionResult ProductLogList(int id)
        {
            var obj = (from t in db.tblPPPDLogNote
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

            return PartialView("~/Views/Inventory/_ProductLogNotes.cshtml", obj);
        }

        #endregion Inventory Product Master Log Notes

        #region Label Printing

        /// Generates Label as PDF
        //public ActionResult PrintLabel()
        //{
        //    var pagecopies = 2;

        //    return new ViewAsPdf()
        //    {
        //        //FileName = "Label.pdf",
        //        PageMargins = new Margins(2, 2, 0, 2),
        //        PageWidth = 200,
        //        PageHeight = 75,
        //        CustomSwitches = "--disable-smart-shrinking --copies "+pagecopies+""
        //    };
        //}

        [AllowAnonymous]
        public ActionResult PrintLabel()
        {
            // Generate plain html label
            return View();
        }

        // Anonymous access is REQUIRED for the callback from client print
        [AllowAnonymous]
        public void LabelPrint()
        {
            int pagecopies = 1;
            string printerName = @"AThermalZebraNet";                           // @"\\CMCNMPS2\RcvShelf";

            var actionPDF = new Rotativa.ActionAsPdf("PrintLabel")
            {
                PageMargins = new Margins(2, 2, 0, 2),
                PageWidth = 200,
                PageHeight = 75,
                CustomSwitches = "--disable-smart-shrinking --load-error-handling ignore --copies " + pagecopies + ""
            };

            byte[] pdfContent = actionPDF.BuildPdf(ControllerContext);

            string fileName = "thermallabel.pdf";                               // Create a temp file name for our PDF report...
            PrintFile file = new PrintFile(pdfContent, fileName);               // Create a PrintFile object with the pdf report

            ClientPrintJob cpj = new ClientPrintJob();                          // Create a ClientPrintJob and send it back to the client!
            cpj.PrintFile = file;                                               // Set file to print...
            cpj.ClientPrinter = new InstalledPrinter(printerName);              // Set client printer...//cpj.ClientPrinter = new NetworkPrinter("10.0.0.8", 9100);
            cpj.SendToClient(System.Web.HttpContext.Current.Response);          // Send it...
        }

        // Web client call to ShelfStockPrint generates pdf and sends pdf stream to printer
        // stream pdf to printer
        [AllowAnonymous]
        public void ShelfStockPrint(string Shelfstockid, string pagecopies)
        {
            int id = Convert.ToInt32(Shelfstockid);
            string printerName = @"AThermalZebraNet";                           // Set printer name as installed on local pc or UNC eg; @"\\CMCNMPS2\RcvShelf"

            var actionPDF = new Rotativa.ActionAsPdf("PrintShelfStockLabel", new { id })
            {
                PageMargins = new Margins(2, 2, 0, 2),
                PageWidth = 200,
                PageHeight = 75,
                CustomSwitches = "--disable-smart-shrinking --load-error-handling ignore --copies " + pagecopies + ""
            };

            byte[] pdfContent = actionPDF.BuildPdf(ControllerContext);          // PDF stream content

            string fileName = Shelfstockid + ".pdf";                            // Set file and extension name
            PrintFile file = new PrintFile(pdfContent, fileName);               // Build file

            ClientPrintJob cpj = new ClientPrintJob();                          // Create a ClientPrintJob and send it back to the client!
            cpj.PrintFile = file;                                               // Set file to print
            cpj.ClientPrinter = new InstalledPrinter(printerName);              // Set client printer
            //cpj.ClientPrinter = new NetworkPrinter("192.168.0.60", 9100);     // Set IP printer: ipaddress, port
            cpj.SendToClient(System.Web.HttpContext.Current.Response);          // Send it
        }

        // Generate label as html view
        // Used as template for pdf stream in ShelfStockPrint
        [AllowAnonymous]
        public ActionResult PrintShelfStockLabel(int id)
        {
            var vm = InventoryService.fnFillStockViewModel(id);

            return View(vm);
        }

        // Generate PDF
        public ActionResult ShelfStockLabel(int id)
        {
            var vm = InventoryService.fnFillStockViewModel(id);

            return new ViewAsPdf(vm)
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