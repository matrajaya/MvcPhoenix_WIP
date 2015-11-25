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
