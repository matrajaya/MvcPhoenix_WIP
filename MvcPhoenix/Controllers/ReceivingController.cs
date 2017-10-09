using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ReceivingController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var bulkContainers = ReceivingService.GetBulkContainers();

            return View("~/Views/Receiving/Index.cshtml", bulkContainers);
        }

        [HttpGet]
        public ActionResult UnkownBulkList()
        {
            using (var db = new CMCSQL03Entities())
            {
                var UnknownBulkItems = db.tblBulkUnKnown
                                         .OrderByDescending(x => x.BulkID)
                                         .ToList();

                return PartialView("~/Views/Receiving/_UnknownBulkList.cshtml", UnknownBulkItems);
            }
        }

        public ActionResult DeleteUnknownBulk(int id)
        {
            int bulkId = id;

            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = @"DELETE FROM tblBulkUnKnown WHERE BulkID=" + bulkId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }

            return null;
        }

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new CMCSQL03Entities())
            {
                ViewBag.CurrentSort = sortOrder;

                if (searchString != null)
                {
                    page = 1;
                }
                else 
                { 
                    searchString = currentFilter; 
                }

                ViewBag.CurrentFilter = searchString;

                var productCodes = (from productdetail in db.tblProductDetail
                                    join productmaster in db.tblProductMaster on productdetail.ProductMasterID equals productmaster.ProductMasterID
                                    join client in db.tblClient on productmaster.ClientID equals client.ClientID
                                    select new ProductProfile
                                    {
                                        clientid = client.ClientID,
                                        clientname = client.ClientName,
                                        productmasterid = productmaster.ProductMasterID,
                                        mastercode = productmaster.MasterCode,
                                        mastername = productmaster.MasterName,
                                        productdetailid = productdetail.ProductDetailID,
                                        productcode = productdetail.ProductCode,
                                        productname = productdetail.ProductName
                                    });

                if (!String.IsNullOrEmpty(searchString))
                {
                    productCodes = productCodes.Where(
                        p => p.mastercode.Contains(searchString)
                          || p.mastername.Contains(searchString)
                          || p.productcode.Contains(searchString)
                          || p.productname.Contains(searchString))
                              .OrderBy(p => p.mastercode);
                }

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                return PartialView(productCodes.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult SetupReceiveUnKnown()
        {
            var bulkContainerUnknown = ReceivingService.NewBulkContainerUnKnown();

            return View("~/Views/Receiving/Edit.cshtml", bulkContainerUnknown);
        }

        public ActionResult BuildProductMasterDropDown(int clientId)
        {
            return Content(ApplicationService.ddlBuildProductMaster(clientId));
        }

        public ActionResult CheckForOpenOrderItems(FormCollection form)
        {
            int productMasterId = Convert.ToInt32(form["productmasterid"]);
            var bulkOrderItems = BulkService.GetOpenBulkOrderItems(productMasterId);

            if (bulkOrderItems.Count() > 0)
            {
                return PartialView("~/Views/Receiving/_OpenOrderItems.cshtml", bulkOrderItems);
            }
            else
            {
                return Content("<br><label>No open orders found for the selected Master Product</label>");
            }
        }

        public void TagItemToBeClosed(int id, bool ischecked)
        {
            int bulkOrderItemId = id;

            BulkService.CloseBulkOrderItem(bulkOrderItemId, ischecked);
        }

        public ActionResult BuildProductCodeDropDown(int clientid)
        {
            return Content(ApplicationService.ddlBuildProductCode(clientid));
        }

        public ActionResult EnterPrePack(int clientid, int productdetailid)
        {
            var prePack = ReceivingService.NewBulkContainerPrePack(clientid, productdetailid);

            return View("~/Views/Receiving/EnterPrePack.cshtml", prePack);
        }

        [HttpPost]
        public ActionResult SavePrePack(PrePackViewModel prepack, FormCollection form)
        {
            ReceivingService.SavePrePack(prepack, form);

            return Content("Items Added to Shelf Stock on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult CreateContainerReceipt(int productmasterid, int? productdetailid)
        {
            var bulkContainer = ReceivingService.NewBulkContainer(productmasterid, productdetailid);

            return View("~/Views/Receiving/Edit.cshtml", bulkContainer);
        }

        [HttpGet]
        public ActionResult EditContainerReceipt(int id)
        {
            int bulkId = id;
            var bulkContainer = ReceivingService.GetBulkContainer(bulkId);

            return View("~/Views/Receiving/Edit.cshtml", bulkContainer);
        }

        [HttpPost]
        public ActionResult SaveContainer(BulkContainerViewModel bulkContainer)
        {
            switch (bulkContainer.isknownmaterial)
            {
                case true:
                    ReceivingService.SaveBulkContainerKnown(bulkContainer);
                    break;

                case false:
                    ReceivingService.SaveBulkContainerUnKnown(bulkContainer);
                    break;

                default:
                    return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}