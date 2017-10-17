using MvcPhoenix.EF;
using MvcPhoenix.Models;
using PagedList;
using Rotativa;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (String.IsNullOrWhiteSpace(searchString)
                && String.IsNullOrWhiteSpace(currentFilter))
            {
                return new EmptyResult();
            }

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
            ViewBag.SearchString = searchString;

            using (var db = new CMCSQL03Entities())
            {
                var productCodes = db.tblProductDetail.AsQueryable();

                if (!String.IsNullOrEmpty(searchString))
                {
                    productCodes = productCodes.Where(p => p.ProductCode.Contains(searchString)
                                                        || p.ProductName.Contains(searchString));
                }

                productCodes = productCodes.OrderBy(p => p.ProductCode);

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                return PartialView(productCodes.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            int productDetailId = id;
            var productProfile = new ProductProfile();
            productProfile.productdetailid = productDetailId;

            productProfile = ProductService.GetProductDetail(productProfile);
            productProfile = ProductService.GetProductMaster(productProfile);
            productProfile = ProductService.GetProductExtendedProps(productProfile);

            return View(productProfile);
        }

        [HttpPost]
        public ActionResult Create(int clientid)
        {
            var productProfile = new ProductProfile();
            productProfile.clientid = clientid;
            productProfile.productmasterid = -1;
            productProfile.productdetailid = -1;

            return View("~/Views/Product/Edit.cshtml", productProfile);
        }

        [HttpPost]
        public ActionResult Equivalent()
        {
            int productDetailId = Convert.ToInt32(Request.Form["productdetailid"]);
            int newProductDetailId = ProductService.CreateEquivalent(productDetailId);

            return RedirectToAction("Edit", new { id = newProductDetailId });
        }

        [HttpPost]
        public ActionResult SaveProductProfile(ProductProfile productProfile)
        {
            if (String.IsNullOrEmpty(productProfile.productcode)
                || String.IsNullOrEmpty(productProfile.productname)
                || String.IsNullOrEmpty(productProfile.mastercode)
                || String.IsNullOrEmpty(productProfile.mastername))
            {
                TempData["SaveResult"] = "Product Profile not saved. Missing product code/name";

                return View("Edit", productProfile);
            }

            int productDetailId = ProductService.SaveProductProfile(productProfile);
            TempData["SaveResult"] = "Product Profile updated on " + DateTime.UtcNow.ToString("R");

            return RedirectToAction("Edit", new { id = productDetailId });
        }

        [HttpGet]
        public ActionResult DeActivateProductMaster(int id)
        {
            int productMasterId = id;
            ProductService.DeActivateProductMaster(productMasterId);

            return Content("Product De-Activated");
        }

        public ActionResult PrintProfile(int id)
        {
            int productDetailId = id;
            var productProfile = new ProductProfile();
            productProfile.productdetailid = productDetailId;

            productProfile = ProductService.GetProductDetail(productProfile);
            productProfile = ProductService.GetProductMaster(productProfile);
            productProfile = ProductService.GetProductExtendedProps(productProfile);

            string footer = "--footer-left \"Printed on: " +
                DateTime.UtcNow.ToString("R") +
                "                                                                                                                                    " +
                " Page: [page]/[toPage]\"" +
                " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

            return new ViewAsPdf(productProfile) { CustomSwitches = footer };
        }

        #region Lookup Information

        public ActionResult ProductCodesDropDown(int clientid)
        {
            return Content(ApplicationService.ddlBuildProductCode(clientid));
        }

        [HttpGet]
        public ActionResult GetUN(string id)
        {
            string unNumber = id;
            var un = ProductService.GetUN(unNumber);

            return Json(un, JsonRequestBehavior.AllowGet);
        }

        #endregion Lookup Information

        #region Product Note

        public ActionResult ProductLogList(int id)
        {
            int productDetailId = id;
            var productLogs = ProductService.GetProductNotes(productDetailId);

            ViewBag.ParentKey = productDetailId;
            return PartialView("~/Views/Product/_LogNotes.cshtml", productLogs);
        }

        [HttpGet]
        public ActionResult CreateProductNote(int id)
        {
            int productDetailId = id;
            var productNote = ProductService.CreateProductNote(productDetailId);

            return PartialView("~/Views/Product/_LogNotesModal.cshtml", productNote);
        }

        [HttpGet]
        public ActionResult EditProductNote(int id)
        {
            int productDetailLogNoteId = id;
            var productNote = ProductService.GetProductNote(productDetailLogNoteId);

            return PartialView("~/Views/Product/_LogNotesModal.cshtml", productNote);
        }

        [HttpPost]
        public ActionResult SaveProductNote(ProductNote productnote)
        {
            ProductService.SaveProductNote(productnote);

            return Content("Data Updated on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult DeleteProductNote(int id, int ParentID)
        {
            int productDetailLogNoteId = id;
            ProductService.DeleteProductNote(productDetailLogNoteId);

            return null;
        }

        #endregion Product Note

        #region CAS

        [HttpGet]
        public ActionResult CasList(int id)
        {
            int productDetailId = id;
            var casItems = ProductService.GetCasItems(productDetailId);

            ViewBag.ParentKey = productDetailId;

            return PartialView("~/Views/Product/_Cas.cshtml", casItems);
        }

        [HttpGet]
        public ActionResult CreateCAS(int id)
        {
            int productDetailId = id;
            var cas = ProductService.CreateCAS(productDetailId);

            return PartialView("~/Views/Product/_CasModal.cshtml", cas);
        }

        [HttpGet]
        public ActionResult EditCAS(int id)
        {
            int casId = id;
            var cas = ProductService.GetCAS(casId);

            return PartialView("~/Views/Product/_CasModal.cshtml", cas);
        }

        [HttpPost]
        public ActionResult SaveCAS(Cas cas)
        {
            ProductService.SaveCAS(cas);

            return Content("Data Updated on " + DateTime.UtcNow.ToString("R"));
        }

        public ActionResult DeleteCAS(int id)
        {
            int casId = id;
            ProductService.DeleteCAS(casId);

            return null;
        }

        #endregion CAS

        #region Client Product Cross Reference

        [HttpGet]
        public ActionResult XRefList(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new CMCSQL03Entities())
            {
                var productXRef = (from t in db.tblProductXRef
                                   let client = db.tblClient
                                                  .Where(x => x.ClientID == t.ClientID)
                                                  .FirstOrDefault()
                                   let product = db.tblProductDetail
                                                   .Where(x => x.ProductCode == t.CMCProductCode)
                                                   .FirstOrDefault()
                                   select new ClientProductXRef
                                   {
                                       ProductXRefID = t.ProductXRefID,
                                       ClientID = t.ClientID,
                                       ClientName = client.ClientName,
                                       ProductID = product.ProductDetailID,
                                       ProductName = product.ProductName,
                                       CMCProductCode = t.CMCProductCode,
                                       CMCSize = t.CMCSize,
                                       ClientProductCode = t.CustProductCode,
                                       ClientProductName = t.CustProductName,
                                       ClientSize = t.CustSize
                                   }).AsQueryable();

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                if (!String.IsNullOrEmpty(searchString))
                {
                    productXRef = productXRef.Where(x => x.ClientName.Contains(searchString)
                                                      || x.ProductName.Contains(searchString)
                                                      || x.CMCProductCode.Contains(searchString));
                }

                productXRef = productXRef.OrderBy(x => x.ClientName);

                int pageSize = 15;
                int pageNumber = (page ?? 1);

                return View("~/Views/Product/ClientXRef.cshtml", productXRef.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult CreateXRef()
        {
            var clientProductXRef = new ClientProductXRef();
            clientProductXRef.ProductXRefID = -1;

            return PartialView("~/Views/Product/_ClientXRefModal.cshtml", clientProductXRef);
        }

        [HttpGet]
        public ActionResult EditXRef(int id)
        {
            int productXRefId = id;
            var clientProductXRef = ProductService.GetClientProductXRef(productXRefId);

            return PartialView("~/Views/Product/_ClientXRefModal.cshtml", clientProductXRef);
        }

        [HttpPost]
        public ActionResult SaveXRef(ClientProductXRef clientProductXRef)
        {
            ProductService.SaveClientProductXRef(clientProductXRef);

            return null;
        }

        public ActionResult DeleteXRef(int id)
        {
            int productXRefId = id;
            ProductService.DeleteProductXRef(productXRefId);

            return null;
        }

        #endregion Client Product Cross Reference
    }
}