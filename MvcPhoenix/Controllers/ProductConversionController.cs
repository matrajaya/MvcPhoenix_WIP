using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ProductConversionController : Controller
    {
        public ActionResult DetailToMaster()
        {
            return View("~/Views/ProductConversion/DetailToMaster.cshtml");
        }

        public ActionResult LoadDetailPartial(int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                ProductProfile vm = new ProductProfile();
                vm.productdetailid = productdetailid;
                vm = ProductsService.FillFromPD(vm);
                vm = ProductsService.FillFromPM(vm);
                vm = ProductsService.fnFillOtherPMProps(vm);
                var vb = db.tblDivision.Find(vm.divisionid);
                ViewBag.Division = vb.DivisionName;                                             // not in PP

                return PartialView("~/Views/ProductConversion/_ProductDetail.cshtml", vm);
            }

            return RedirectToAction("Index");
        }

        public ActionResult ConvertDetailToMaster(int id)                                   // id=productdetailid
        {
            using (var db = new CMCSQL03Entities())
            {
                var pd = db.tblProductDetail.Find(id);
                var pm = db.tblProductMaster.Find(pd.ProductMasterID);

                pm.MasterCode = pd.ProductCode;
                pm.MasterName = pd.ProductName;
                db.SaveChanges();

                return RedirectToAction("Edit", "Products", new { id = id });
            }
        }

        public ActionResult BuildProductCodeDropDown(int id)
        {
            var ddlproductcode = ApplicationService.ddlBuildProductEquivalentDropdown(id);

            return Content(ddlproductcode);
        }

        // Shows after buildproductcodedropdown is used once upon page load
        public static List<SelectListItem> fnListOfProductCodes(int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblProductDetail
                          join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                          where pm.ClientID == clientid
                          orderby t.ProductCode
                          select new SelectListItem { Value = t.ProductDetailID.ToString(), Text = t.ProductName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "" });

                return mylist;
            }
        }
    }
}