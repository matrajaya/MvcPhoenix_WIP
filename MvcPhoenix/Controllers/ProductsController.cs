using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoenix.Models;
using MvcPhoenix.Services;

namespace MvcPhoenix.Controllers
{
    public class ProductsController : Controller
    {
        
        public ActionResult Index()
        {
            return View("~/Views/Products/Index.cshtml");
        }

        [HttpGet]
        public ActionResult ProductCodesDropDown(int id,string divid)
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
            //return View("~/Views/Products/ProductProfileEdit.cshtml", PP);  - Iffy
            return View(PP);
        }

        //[HttpGet]
        //public ActionResult New(int id) - Iffy
        [HttpPost]
        public ActionResult Create(int clientid2)
        {
            ProductProfile PP = new ProductProfile();
            //PP.clientid = id; - Iffy
            PP.clientid = clientid2;
            PP.productmasterid = -1;
            PP.productdetailid = -1;
            PP = ProductsService.fnFillOtherPMProps(PP);
            //return View("~/Views/Products/ProductProfileEdit.cshtml", PP);  - Iffy
            return View(PP);
        }
        
        //[HttpGet]
        //public ActionResult Equiv(int id) - Iffy
        [HttpPost]
        public ActionResult Equiv(int productdetailid3)
        {
            ProductProfile PP = new ProductProfile();
            PP.productdetailid = -1;
            //PP.productmasterid = ProductsService.fnProductMasterID(id);  - Iffy
            PP.productmasterid = ProductsService.fnProductMasterID(productdetailid3);
            PP = ProductsService.FillFromPM(PP);
            PP = ProductsService.fnFillOtherPMProps(PP);
            //return View("~/Views/Products/ProductProfileEdit.cshtml", PP);  - Iffy
            return View("~/Views/Products/Create.cshtml", PP);
        }


        [HttpPost]
        public ActionResult SaveProductProfile(ProductProfile PPVM)
        {
            System.Threading.Thread.Sleep(1000);    // dev
            ProductProfile obj = new ProductProfile();

            if (TryUpdateModel(obj))
            {
                if (ProductsService.fnSaveProductProfile(obj)==true)
                {
                    return Content("Profile Updated At " + DateTime.Now.ToString());
                }
                else
                {
                    return Content("DB Backend Error At " + DateTime.Now.ToString());
                }
            }
            else
            {
                return Content("Model Invalid At " + DateTime.Now.ToString());
            }
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

        #region LogNotes - ProductNotes
        public ActionResult LogNotesList(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var obj = (from t in db.tblProductNotes
                           where t.ProductDetailID == id
                           select new ProductNote{
                productnoteid = t.ProductNoteID,
                productdetailid = t.ProductDetailID,
                notedate =t.NoteDate,
                notes = t.Notes,
                reasoncode = t.ReasonCode,
                ListOfReasonCodes = (from r in db.tblReasonCode orderby r.Reason select new SelectListItem { Value = r.Reason, Text = r.Reason }).ToList()
                }).ToList();
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
        #endregion

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




        #endregion




        #region PC Testing

        // *******************************************************************
        // Older POST versions - used by pc for testing 
        [HttpPost]
        public ActionResult SetUpProductProfileEdit(int productdetailid1)
        {
            return RedirectToAction("Edit", new { id = productdetailid1 });
        }

        [HttpPost]
        public ActionResult SetUpProductProfileNew(int clientid2)
        {
            return RedirectToAction("Edit", new { id = clientid2 });
        }

        [HttpPost]
        public ActionResult SetUpProductProfileEquiv(int productdetailid3)
        {
            return RedirectToAction("Equiv", new { id = productdetailid3 });
        }
        // *******************************************************************

        #endregion
    }
}
