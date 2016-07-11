using MvcPhoenix.Models;
using PagedList;
using Rotativa;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ProductsController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/Products/Index.cshtml");
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

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                //return View(productCodes.ToPagedList(pageNumber, pageSize));
                return PartialView(productCodes.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult ProductCodesDropDown(int id, string divid)
        {
            // AJAX call to return a <select> tag string into a <div>
            return Content(ProductsService.fnProductCodesDropDown(id, divid));
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ProductProfile PP = new ProductProfile();
            PP.productdetailid = id;
            PP = ProductsService.FillFromPD(PP);
            PP = ProductsService.FillFromPM(PP);
            PP = ProductsService.fnFillOtherPMProps(PP);
            return View(PP);
        }

        public ActionResult PrintProfile(int id)
        {
            string footer = "--footer-left \"Printed on: " +
                DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") +
                "                                                                                                                                    " +
                " Page: [page]/[toPage]\"" +
                " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

            ProductProfile PP = new ProductProfile();
            PP.productdetailid = id;
            PP = ProductsService.FillFromPD(PP);
            PP = ProductsService.FillFromPM(PP);
            PP = ProductsService.fnFillOtherPMProps(PP);
            return new ViewAsPdf(PP) { CustomSwitches = footer };
        }

        [HttpPost]
        public ActionResult Create(int clientid2)
        {
            ProductProfile PP = new ProductProfile();
            PP.clientid = clientid2;
            PP.productmasterid = -1;
            PP.productdetailid = -1;
            PP = ProductsService.fnFillOtherPMProps(PP);
            return View(PP);
        }

        [HttpPost]
        public ActionResult Equiv(int productdetailid3)
        {
            ProductProfile PP = new ProductProfile();
            PP.productdetailid = -1;
            PP.productmasterid = ProductsService.fnProductMasterID(productdetailid3);
            PP = ProductsService.FillFromPM(PP);
            PP = ProductsService.fnFillOtherPMProps(PP);
            return View("~/Views/Products/Create.cshtml", PP);
        }

        [HttpPost]
        public ActionResult SaveProductProfile(ProductProfile PPVM)
        {
            System.Threading.Thread.Sleep(500);    // dev, remove later
            // the call to the service always returns a ProductDetailID
            int pk = ProductsService.fnSaveProductProfile(PPVM);
            return RedirectToAction("Edit", new { id = pk });
        }

        [HttpGet]
        public ActionResult DeActivateProductMaster(int id)
        {
            ProductsService.fnDeActivateProductMaster(id);
            // The returned string can be pushed into a message <div>
            return Content("Product De-Activated");
        }

        [HttpGet]
        public ActionResult LookupUNGround(string id)
        {
            UN obj = new UN();
            obj = ProductsService.fnGetUN(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LookupUNAir(string id)
        {
            UN obj = new UN();
            obj = ProductsService.fnGetUN(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LookupUNSea(string id)
        {
            UN obj = new UN();
            obj = ProductsService.fnGetUN(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LookupUNRCRA(string id)
        {
            UN obj = new UN();
            obj = ProductsService.fnGetUN(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #region LogNotes - ProductNotes

        public ActionResult LogNotesList(int id)
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
                return PartialView("~/Views/Products/_LogNotes.cshtml", obj);
            }
        }

        [HttpGet]
        public ActionResult CreateProductNote(int id)
        {
            // id=ProductDetailID as the FK
            var PN = ProductsService.fnCreateProductNote(id);
            return PartialView("~/Views/Products/_LogNotesModal.cshtml", PN);
        }

        [HttpGet]
        public ActionResult EditProductNote(int id)
        {
            // id=ProductNoteID as the PK
            var PN = ProductsService.fnGetProductNote(id);
            return PartialView("~/Views/Products/_LogNotesModal.cshtml", PN);
        }

        [HttpPost]
        public ActionResult SaveProductNote(ProductNote PN)
        {
            int pk = ProductsService.fnSaveProductNoteToDB(PN);
            return Content("Data Updated at " + DateTime.Now);
        }

        [HttpGet]
        public ActionResult DeleteProductNote(int id, int ParentID)
        {
            int pk = ProductsService.fnDeleteProductNote(id);
            return null;
        }

        #endregion LogNotes - ProductNotes

        #region CAS

        [HttpGet]
        public ActionResult CasList(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var obj = (from t in db.tblCAS
                           where t.ProductDetailID == id
                           select new Cas
                           {
                               casid = t.CASID,
                               productdetailid = t.ProductDetailID,
                               casnumber = t.CasNumber,
                               chemicalname = t.ChemicalName,
                               percentage = t.Percentage,
                               restrictedqty = t.RestrictedQty,
                               restrictedamount = t.RestrictedAmount,
                               packonreceipt = t.PackOnReceipt,
                               reportableqty = t.ReportableQty,
                               reportableamount = t.ReportableAmount,
                               lessthan = t.LessThan,
                               excludefromlabel = t.ExcludeFromLabel
                           }).ToList();
                ViewBag.ParentKey = id;
                return PartialView("~/Views/Products/_Cas.cshtml", obj);
            }
        }

        [HttpGet]
        public ActionResult CreateCAS(int id)
        {
            // id = ProductDetailID
            var CS = ProductsService.fnCreateCAS(id);
            return PartialView("~/Views/Products/_CasModal.cshtml", CS);
        }

        [HttpGet]
        public ActionResult EditCAS(int id)
        {
            // id= CASID
            var CS = ProductsService.fnGetCAS(id);
            return PartialView("~/Views/Products/_CasModal.cshtml", CS);
        }

        [HttpPost]
        public ActionResult SaveCAS(Cas CS)
        {
            int pk = ProductsService.fnSaveCASToDB(CS);
            return Content("Data Updated at " + DateTime.Now);
        }

        public ActionResult DeleteCAS(int id)
        {
            int pk = ProductsService.fnDeleteCAS(id);
            return null;
        }

        #endregion CAS
    }
}