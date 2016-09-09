using MvcPhoenix.Models;
using PagedList;
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
            List<BulkContainerViewModel> mylist = MvcPhoenix.Services.ReceivingService.fnIndexList();
            return PartialView("~/Views/Inventory/_RecentBulkReceived.cshtml", mylist);
        }

        public ActionResult Edit(int id)    // Entry point for ProductDetailID from Search Results
        {
            var vm = MvcPhoenix.Services.InventoryService.fnFillInventoryVM(id);
            return View("~/Views/Inventory/Edit.cshtml", vm);
        }

        [HttpPost]
        public ActionResult Save(Inventory vm)
        {
            MvcPhoenix.Services.InventoryService.fnSaveInventory(vm);
            return RedirectToAction("Edit", new { id = vm.PP.productdetailid });
        }

        public ActionResult EditBulk(int id)   // redirect to bulk edit
        {
            //ViewBag.SearchName = null;
            var vm = MvcPhoenix.Services.BulkService.fnFillBulkContainerFromDB(id);
            return View("~/Views/Bulk/Edit.cshtml", vm);
        }

        public ActionResult EditBulkOrder(int id)
        {
            return RedirectToAction("Edit", "Replenishments", new { id = id });
        }

        public ActionResult BulkStockList(int id)  // return partial
        {
            // fill a list<BulkContainerViewModel>
            var vm = new List<BulkContainerViewModel>();

            var q = (from t in db.tblBulk
                     where t.ProductMasterID == id
                     select t).ToList();

            foreach (var row in q)
            {
                vm.Add(MvcPhoenix.Services.BulkService.fnFillBulkContainerFromDB(row.BulkID));
            }

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
                vm.Add(Services.InventoryService.fnFillStockViewModel(row.StockID));
            }

            ViewBag.ParentID = id;

            return PartialView("~/Views/Inventory/_ShelfStock.cshtml", vm);
        }

        public ActionResult EditStock(int id)
        {
            var vm = Services.InventoryService.fnFillStockViewModel(id);
            return View("~/Views/Inventory/_ShelfStockModal.cshtml", vm);
        }

        public ActionResult CreatePrePackStock(int id) //id=productdetailid
        {
            var pd = db.tblProductDetail.Find(id);
            var pm = db.tblProductMaster.Find(pd.ProductMasterID);
            var cl = db.tblClient.Find(pm.ClientID);
            PrePackStock vm = new PrePackStock();
            vm.ProductDetailID = id;
            vm.BulkContainer = new BulkContainerViewModel();
            vm.BulkContainer.logofilename = cl.LogoFileName;
            vm.BulkContainer.bulkid = -1;
            vm.BulkContainer.receivedate = System.DateTime.Now;
            vm.BulkContainer.warehouse = cl.CMCLocation;
            vm.BulkContainer.lotnumber = "lotnumber";
            vm.BulkContainer.mfgdate = System.DateTime.Now;
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
            Services.InventoryService.fnSavePrePackStock(vm, fc);

            return RedirectToAction("Edit", new { id = vm.ProductDetailID });
        }

        [HttpPost]
        public ActionResult SaveStock(StockViewModel vm)
        {
            Services.InventoryService.fnSaveStock(vm);

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

        public ActionResult ProductLogList(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var obj = (from t in db.tblProductNotes
                           where t.ProductDetailID == id
                           select new ProductNote
                           {
                               productnoteid = t.ProductNoteID,
                               productdetailid = t.ProductDetailID,
                               notedate = t.NoteDate,
                               notes = t.Notes,
                               reasoncode = t.ReasonCode,
                               ListOfReasonCodes = (from r in db.tblReasonCode orderby r.Reason select new SelectListItem { Value = r.Reason, Text = r.Reason }).ToList()
                           }).ToList();
                ViewBag.ParentKey = id;

                return PartialView("~/Views/Inventory/_ProductLogNotes.cshtml", obj);
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
    }
}