using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
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
            if (productdetailid < 1)
            {
                return RedirectToAction("Index");
            }

            var productProfile = new ProductProfile();
            productProfile.productdetailid = productdetailid;

            productProfile = ProductService.GetProductDetail(productProfile);
            productProfile = ProductService.GetProductMaster(productProfile);
            productProfile = ProductService.GetProductExtendedProps(productProfile);
            ViewBag.Division = ClientService.GetDivision(productProfile.divisionid).DivisionName;                                             // not in PP

            return PartialView("~/Views/ProductConversion/_ProductDetail.cshtml", productProfile);
        }

        public ActionResult ConvertDetailToMaster(int id)
        {
            int productDetailId = id;

            using (var db = new CMCSQL03Entities())
            {
                var productDetail = db.tblProductDetail.Find(id);
                var productMaster = db.tblProductMaster.Find(productDetail.ProductMasterID);

                productMaster.MasterCode = productDetail.ProductCode;
                productMaster.MasterName = productDetail.ProductName;
                db.SaveChanges();
            }
            
            return RedirectToAction("Edit", "Product", new { id = productDetailId });
        }

        public ActionResult BuildProductCodeDropDown(int id)
        {
            int clientid = id;
            var productCodes = ApplicationService.ddlBuildProductEquivalent(clientid);

            return Content(productCodes);
        }
    }
}