using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ReceivingController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            List<BulkContainerViewModel> mylist = ReceivingService.fnIndexList();
            return View("~/Views/Receiving/Index.cshtml", mylist);
        }

        [HttpGet]
        public ActionResult UnkownBulkList()
        {
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblBulkUnKnown
                           orderby t.BulkID descending
                           select t).ToList();

                return PartialView("~/Views/Receiving/_UnknownBulkList.cshtml", obj);
            }
        }

        public ActionResult DeleteUnknownBulk(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                string s = @"DELETE FROM tblBulkUnKnown WHERE BulkID=" + id.ToString();
                db.Database.ExecuteSqlCommand(s);
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
                else { searchString = currentFilter; }

                ViewBag.CurrentFilter = searchString;

                var productCodes = (from pd in db.tblProductDetail
                                    join pm in db.tblProductMaster on pd.ProductMasterID equals pm.ProductMasterID
                                    join c in db.tblClient on pm.ClientID equals c.ClientID
                                    select new ProductProfile
                                    {
                                        clientid = c.ClientID,
                                        clientname = c.ClientName,
                                        productmasterid = pm.ProductMasterID,
                                        mastercode = pm.MasterCode,
                                        mastername = pm.MasterName,
                                        productdetailid = pd.ProductDetailID,
                                        productcode = pd.ProductCode,
                                        productname = pd.ProductName
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

        // TODO: Repurpose for open orders check - Iffy
        //public ActionResult SetUpReceiveKnown()
        //{
        //    return View("~/Views/Receiving/SelectProduct.cshtml");
        //}

        // TODO: Repurpose for open orders check - Iffy
        //public ActionResult SetUpReceivePrePack()
        //{
        //    return View("~/Views/Receiving/SelectPrePack.cshtml");
        //}

        [HttpGet]
        public ActionResult SetupReceiveUnKnown()
        {
            // Load view to receive unknown bulk material
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = ReceivingService.fnNewBulkContainerUnKnown();
            return View("~/Views/Receiving/Edit.cshtml", obj);
        }

        public ActionResult BuildProductMasterDropDown(int clientId)
        {
            // id=clientid .. return the <option> values for the <select> tag
            return Content(ApplicationService.ddlBuildProductMasterDropDown(clientId));
        }

        // TODO: Move to inventory - Iffy
        // parse productmasterid to method instead of formcollection
        public ActionResult CheckForOpenOrderItems(FormCollection fc)
        {
            // Build list of open order items and return a partial
            int pk = Convert.ToInt32(fc["productmasterid"]);
            List<OpenBulkOrderItems> result = ReceivingService.fnOpenBulkOrderItems(pk);
            if (result.Count() > 0)
            {
                return PartialView("~/Views/Receiving/_OpenOrderItems.cshtml", result);
            }
            else
            {
                return Content("<br><label>No open orders found for the selected Master Product</label>");
            }
        }

        public void TagItemToBeClosed(int id, bool ischecked)
        {
            // Called from the onchange event of the checkbox on the view
            // Tag an open order item to be closed later on Save
            ReceivingService.fnTagItemToBeClosed(id, ischecked);
        }

        public ActionResult BuildProductCodeDropDown(int clientid)
        {
            return Content(ApplicationService.ddlBuildProductCodeDropDown(clientid));
        }

        public ActionResult EnterPrePack(int clientid, int productdetailid)
        {
            PrePackViewModel obj = new PrePackViewModel();
            obj = ReceivingService.fnNewBulkContainerForPrePack(clientid, productdetailid);
            return View("~/Views/Receiving/EnterPrePack.cshtml", obj);
        }

        [HttpPost]
        public ActionResult SavePrePack(PrePackViewModel vm, FormCollection fc)
        {
            bool bSave = ReceivingService.fnSavePrePack(vm, fc);
            return Content("Items Added to Shelf Stock on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult CreateContainerReceipt(int productmasterid, int? productdetailid)
        {
            // id = productmasterid .. Build a new viewmodel of a bulk container and load the edit view
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = ReceivingService.fnNewBulkContainer(productmasterid, productdetailid);
            return View("~/Views/Receiving/Edit.cshtml", obj);
        }

        [HttpGet]
        public ActionResult EditContainerReceipt(int id)
        {
            // id = bulkid .. Lookup row in DB and load the edit view
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = ReceivingService.fnFillBulkContainerFromDB(id);
            return View("~/Views/Receiving/Edit.cshtml", obj);
        }
        
        [HttpPost]
        public ActionResult SaveContainer(BulkContainerViewModel bc)
        {
            // Update a tblBulk record from a BulkContainerViewModel
            bool bUpdate;
            switch (bc.isknownmaterial)
            {
                case true:
                    bUpdate = ReceivingService.fnSaveBulkContainerKnown(bc);
                    break;

                case false:
                    bUpdate = ReceivingService.fnSaveBulkContainerUnKnown(bc);
                    break;

                default:
                    return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}