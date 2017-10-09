using MvcPhoenix.EF;
using MvcPhoenix.Extensions;
using MvcPhoenix.Models;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ProductsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (String.IsNullOrWhiteSpace(searchString) && String.IsNullOrWhiteSpace(currentFilter))
            {
                return new EmptyResult();
            }

            using (var db = new CMCSQL03Entities())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.CodeSortParm = String.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

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

                var productCodes = from p in db.tblProductDetail select p;

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

        public ActionResult ProductCodesDropDown(int clientid)
        {
            return Content(ApplicationService.ddlBuildProductCode(clientid));
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            int productDetailId = id;
            var productProfile = new ProductProfile();
            productProfile.productdetailid = productDetailId;

            productProfile = ProductsService.GetProductDetail(productProfile);
            productProfile = ProductsService.GetProductMaster(productProfile);
            productProfile = ProductsService.GetProductExtendedProps(productProfile);
            
            return View(productProfile);
        }

        public ActionResult PrintProfile(int id)
        {
            int productDetailId = id;
            var productProfile = new ProductProfile();
            productProfile.productdetailid = productDetailId;

            productProfile = ProductsService.GetProductDetail(productProfile);
            productProfile = ProductsService.GetProductMaster(productProfile);
            productProfile = ProductsService.GetProductExtendedProps(productProfile);

            string footer = "--footer-left \"Printed on: " +
                DateTime.UtcNow.ToString("R") +
                "                                                                                                                                    " +
                " Page: [page]/[toPage]\"" +
                " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

            return new ViewAsPdf(productProfile) { CustomSwitches = footer };
        }

        [HttpPost]
        public ActionResult Create(int clientid)
        {
            var productProfile = new ProductProfile();
            productProfile.clientid = clientid;
            productProfile.productmasterid = -1;
            productProfile.productdetailid = -1;

            return View("~/Views/Products/Edit.cshtml", productProfile);
        }

        [HttpPost]
        public ActionResult Equivalent()
        {
            int productDetailId = Convert.ToInt32(Request.Form["productdetailid"]);
            int newProductDetailId = ProductsService.CreateEquivalent(productDetailId);

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

            int productDetailId = ProductsService.SaveProductProfile(productProfile);
            TempData["SaveResult"] = "Product Profile updated on " + DateTime.UtcNow.ToString("R");

            return RedirectToAction("Edit", new { id = productDetailId });
        }

        [HttpGet]
        public ActionResult DeActivateProductMaster(int id)
        {
            int productMasterId = id;
            ProductsService.DeActivateProductMaster(productMasterId);

            return Content("Product De-Activated");
        }

        #region Lookup UN Information

        [HttpGet]
        public ActionResult LookupUNGround(string id)
        {
            string unNumber = id;
            var un = new UN();
            un = ProductsService.GetUN(unNumber);

            return Json(un, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LookupUNAir(string id)
        {
            string unNumber = id;
            var un = new UN();
            un = ProductsService.GetUN(unNumber);

            return Json(un, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LookupUNSea(string id)
        {
            string unNumber = id;
            var un = new UN();
            un = ProductsService.GetUN(unNumber);

            return Json(un, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LookupUNRCRA(string id)
        {
            string unNumber = id;
            var un = new UN();
            un = ProductsService.GetUN(unNumber);

            return Json(un, JsonRequestBehavior.AllowGet);
        }

        #endregion Lookup UN Information

        #region LogNotes - ProductNotes

        public ActionResult ProductLogList(int id)
        {
            int productDetailId = id;
            var productLogs = new List<ProductNote>();

            using (var db = new CMCSQL03Entities())
            {
                productLogs = (from t in db.tblPPPDLogNote
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

                ViewBag.ParentKey = productDetailId;
            }

            return PartialView("~/Views/Products/_LogNotes.cshtml", productLogs);
        }

        [HttpGet]
        public ActionResult CreateProductNote(int id)
        {
            int productDetailId = id;
            var productNote = ProductsService.CreateProductNote(productDetailId);

            return PartialView("~/Views/Products/_LogNotesModal.cshtml", productNote);
        }

        [HttpGet]
        public ActionResult EditProductNote(int id)
        {
            int productDetailLogNoteId = id;
            var productNote = ProductsService.GetProductNote(productDetailLogNoteId);

            return PartialView("~/Views/Products/_LogNotesModal.cshtml", productNote);
        }

        [HttpPost]
        public ActionResult SaveProductNote(ProductNote productnote)
        {
            ProductsService.SaveProductNote(productnote);

            return Content("Data Updated on " + DateTime.UtcNow.ToString("R"));
        }

        [HttpGet]
        public ActionResult DeleteProductNote(int id, int ParentID)
        {
            int productDetailLogNoteId = id;
            ProductsService.DeleteProductNote(productDetailLogNoteId);

            return null;
        }

        #endregion LogNotes - ProductNotes

        #region CAS

        [HttpGet]
        public ActionResult CasList(int id)
        {
            int productDetailId = id;
            var casItems = new List<Cas>();

            using (var db = new CMCSQL03Entities())
            {
                casItems = (from cas in db.tblCAS
                            where cas.ProductDetailID == productDetailId
                            select new Cas
                            {
                                casid = cas.CASID,
                                productdetailid = cas.ProductDetailID,
                                casnumber = cas.CasNumber,
                                chemicalname = cas.ChemicalName,
                                percentage = cas.Percentage,
                                restrictedqty = cas.RestrictedQty,
                                restrictedamount = cas.RestrictedAmount,
                                reportableqty = cas.ReportableQty,
                                reportableamount = cas.ReportableAmount,
                                lessthan = cas.LessThan,
                                excludefromlabel = cas.ExcludeFromLabel
                            }).ToList();

                ViewBag.ParentKey = productDetailId;
            }

            return PartialView("~/Views/Products/_Cas.cshtml", casItems);
        }

        [HttpGet]
        public ActionResult CreateCAS(int id)
        {
            int productDetailId = id;
            var cas = ProductsService.CreateCAS(productDetailId);

            return PartialView("~/Views/Products/_CasModal.cshtml", cas);
        }

        [HttpGet]
        public ActionResult EditCAS(int id)
        {
            int casId = id;
            var cas = ProductsService.GetCAS(casId);

            return PartialView("~/Views/Products/_CasModal.cshtml", cas);
        }

        [HttpPost]
        public ActionResult SaveCAS(Cas cas)
        {
            ProductsService.SaveCAS(cas);

            return Content("Data Updated on " + DateTime.UtcNow.ToString("R"));
        }

        public ActionResult DeleteCAS(int id)
        {
            int casId = id;
            ProductsService.DeleteCAS(casId);

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
                                   orderby t.ProductXRefID
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

                switch (sortOrder)
                {
                    case "name_desc":
                        productXRef = productXRef.OrderByDescending(x => x.ClientName);
                        break;

                    default:
                        productXRef = productXRef.OrderBy(x => x.ClientName);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View("~/Views/Products/ClientXRef.cshtml", productXRef.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult CreateXRef()
        {
            var clientProductXRef = new ClientProductXRef();
            clientProductXRef.ProductXRefID = -1;

            return PartialView("~/Views/Products/_ClientXRefModal.cshtml", clientProductXRef);
        }

        [HttpGet]
        public ActionResult EditXRef(int id)
        {
            int productXRefId = id;
            var clientProductXRef = ProductsService.GetClientProductXRef(productXRefId);

            return PartialView("~/Views/Products/_ClientXRefModal.cshtml", clientProductXRef);
        }

        [HttpPost]
        public ActionResult SaveXRef(ClientProductXRef clientProductXRef)
        {
            ProductsService.SaveClientProductXRef(clientProductXRef);

            return null;
        }

        public ActionResult DeleteXRef(int id)
        {
            int productXRefId = id;
            ProductsService.DeleteProductXRef(productXRefId);

            return null;
        }

        #endregion Client Product Cross Reference
    }
}