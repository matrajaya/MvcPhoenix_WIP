using MvcPhoenix.EF;
using MvcPhoenix.Extensions;
using MvcPhoenix.Models;
using PagedList;
using Rotativa;
using System;
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

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                return PartialView(productCodes.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult ProductCodesDropDown(int clientid)
        {
            return Content(ApplicationService.ddlBuildProductCodeDropDown(clientid));
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
                DateTime.UtcNow.ToString("R") +
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
        public ActionResult Create(int clientid)
        {
            ProductProfile PP = new ProductProfile();
            PP.clientid = clientid;
            PP.productmasterid = -1;
            PP.productdetailid = -1;
            PP = ProductsService.fnFillOtherPMProps(PP);

            return View("~/Views/Products/Edit.cshtml", PP);
        }

        [HttpPost]
        public ActionResult Equivalent()
        {
            int productDetailId = Convert.ToInt32(Request.Form["productdetailid"].ToString());

            // fill model to be replicated
            ProductProfile PP = new ProductProfile();
            PP.productdetailid = productDetailId;
            PP = ProductsService.FillFromPD(PP);
            PP = ProductsService.FillFromPM(PP);
            PP = ProductsService.fnFillOtherPMProps(PP);

            // create new record and clear select values for manual entry
            PP.productdetailid = ProductsService.fnNewProductDetailID();
            PP.productcode = PP.productcode + " Clone";
            PP.productname = PP.productname + " Clone";
            PP.sgrevisiondate = DateTime.UtcNow;
            PP.UpdateUserDetail = HttpContext.User.Identity.Name;
            PP.UpdateDateDetail = DateTime.UtcNow;

            // save model held in memory to db
            int pk = ProductsService.fnSaveProductProfile(PP);

            // find entries for shelfsize info, ghs, cas where id = productdetailid3
            // clone these entries where id = PP.productdetailid
            using (var db = new CMCSQL03Entities())
            {
                // Shelf
                var shelf = (from s in db.tblShelfMaster
                             where s.ProductDetailID == productDetailId
                             select s).ToList();

                for (int i = 0; i < shelf.Count; i++)
                {
                    var newShelf = shelf[i].Clone();
                    newShelf.ProductDetailID = PP.productdetailid;

                    db.tblShelfMaster.Add(newShelf);
                    db.SaveChanges();
                }

                // GHS
                var ghs = (from g in db.tblGHS
                           where g.ProductDetailID == productDetailId
                           select g).ToList();

                for (int i = 0; i < ghs.Count; i++)
                {
                    var newGhs = ghs[i].Clone();
                    newGhs.ProductDetailID = PP.productdetailid;

                    db.tblGHS.Add(newGhs);
                    db.SaveChanges();
                }

                // PH Detail
                var ph = (from p in db.tblGHSPHDetail
                          where p.ProductDetailID == productDetailId
                          select p).ToList();

                for (int i = 0; i < ph.Count; i++)
                {
                    var newPh = ph[i].Clone();
                    newPh.ProductDetailID = PP.productdetailid;

                    db.tblGHSPHDetail.Add(newPh);
                    db.SaveChanges();
                }

                // CAS
                var cas = (from c in db.tblCAS
                           where c.ProductDetailID == productDetailId
                           select c).ToList();

                for (int i = 0; i < cas.Count; i++)
                {
                    var newCas = cas[i].Clone();
                    newCas.ProductDetailID = PP.productdetailid;

                    db.tblCAS.Add(newCas);
                    db.SaveChanges();
                }

                // Product Notes Log
                var pdln = new tblPPPDLogNote();

                pdln.ProductDetailID = PP.productdetailid;
                pdln.NoteDate = DateTime.UtcNow;
                pdln.Notes = "Equivalent created from product id: " + productDetailId;
                pdln.ReasonCode = "New";
                pdln.CreateDate = DateTime.UtcNow;
                pdln.CreateUser = HttpContext.User.Identity.Name;
                pdln.UpdateDate = DateTime.UtcNow;
                pdln.UpdateUser = HttpContext.User.Identity.Name;

                db.tblPPPDLogNote.Add(pdln);
                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = pk });
        }

        [HttpPost]
        public ActionResult SaveProductProfile(ProductProfile PPVM)
        {
            if (String.IsNullOrEmpty(PPVM.productcode) || String.IsNullOrEmpty(PPVM.productname) || String.IsNullOrEmpty(PPVM.mastercode) || String.IsNullOrEmpty(PPVM.mastername))
            {
                TempData["SaveResult"] = "Product Profile not saved. Missing product code/name";

                return View("Edit", PPVM);
            }

            int pk = ProductsService.fnSaveProductProfile(PPVM);
            TempData["SaveResult"] = "Product Profile updated on " + DateTime.UtcNow.ToString("R");

            return RedirectToAction("Edit", new { id = pk });
        }

        [HttpGet]
        public ActionResult DeActivateProductMaster(int id)
        {
            ProductsService.fnDeActivateProductMaster(id);
            // The returned string can be pushed into a message <div>
            return Content("Product De-Activated");
        }

        #region Lookup UN Information

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

        #endregion Lookup UN Information

        #region LogNotes - ProductNotes

        public ActionResult ProductLogList(int id)
        {
            using (var db = new CMCSQL03Entities())
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

            return Content("Data Updated on " + DateTime.UtcNow.ToString("R"));
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
            using (var db = new CMCSQL03Entities())
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

            return Content("Data Updated on " + DateTime.UtcNow.ToString("R"));
        }

        public ActionResult DeleteCAS(int id)
        {
            int pk = ProductsService.fnDeleteCAS(id);

            return null;
        }

        #endregion CAS

        #region Client Product Cross Reference

        [HttpGet]
        public ActionResult XRefList(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblProductXRef
                           orderby t.ProductXRefID
                           select new ClientProductXRef
                           {
                               ProductXRefID = t.ProductXRefID,
                               ClientID = t.ClientID,
                               ClientName = (from cn in db.tblClient
                                             where cn.ClientID == t.ClientID
                                             select cn.ClientName).FirstOrDefault(),

                               ProductID = (from pd in db.tblProductDetail
                                            where pd.ProductCode == t.CMCProductCode
                                            select pd.ProductDetailID).FirstOrDefault(),

                               ProductName = (from pd in db.tblProductDetail
                                              where pd.ProductCode == t.CMCProductCode
                                              select pd.ProductName).FirstOrDefault(),

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
                    obj = obj.Where(x => x.ClientName.Contains(searchString)
                        || x.ProductName.Contains(searchString)
                        || x.CMCProductCode.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        obj = obj.OrderByDescending(x => x.ClientName);
                        break;

                    default:
                        obj = obj.OrderBy(x => x.ClientName);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View("~/Views/Products/ClientXRef.cshtml", obj.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult CreateXRef()
        {
            ClientProductXRef CXRef = new ClientProductXRef();
            CXRef.ProductXRefID = -1;

            return PartialView("~/Views/Products/_ClientXRefModal.cshtml", CXRef);
        }

        [HttpGet]
        public ActionResult EditXRef(int id)
        {
            // id = ProductXRefID
            var CXRef = ProductsService.fnGetXRef(id);

            return PartialView("~/Views/Products/_ClientXRefModal.cshtml", CXRef);
        }

        [HttpPost]
        public ActionResult SaveXRef(ClientProductXRef CXRef)
        {
            int pk = ProductsService.fnSaveXRefToDB(CXRef);

            return null;
        }

        public ActionResult DeleteXRef(int id)
        {
            int pk = ProductsService.fnDeleteXRef(id);

            return null;
        }

        #endregion Client Product Cross Reference
    }
}