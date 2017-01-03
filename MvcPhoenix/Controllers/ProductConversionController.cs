using MvcPhoenix.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ProductConversionController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public ActionResult DetailToMaster()
        {
            ViewBag.ListOfClients = fnListOfClientIDs();
            return View("~/Views/ProductConversion/DetailToMaster.cshtml");
        }

        public ActionResult LoadDetailPartial(int productdetailid)
        {
            using (db)
            {
                ProductProfile vm = new ProductProfile();
                vm.productdetailid = productdetailid;
                vm = ProductsService.FillFromPD(vm);
                vm = ProductsService.FillFromPM(vm);
                vm = ProductsService.fnFillOtherPMProps(vm);
                var vb = db.tblDivision.Find(vm.divisionid);
                ViewBag.Division = vb.DivisionName;    // not in PP
                return PartialView("~/Views/ProductConversion/_ProductDetail.cshtml", vm);
            }
            return RedirectToAction("Index");
        }

        public ActionResult ConvertDetailToMaster(int id) // id=productdetailid
        {
            using (db)
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
            // return Product codes to the view
            using (db)
            {
                var qry = (from t in db.tblProductDetail
                           join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                           where pm.ClientID == id && t.ProductCode != pm.MasterCode
                           orderby t.ProductCode
                           select new { t.ProductDetailID, t.ProductCode, t.ProductName }).ToList();

                string s = "<option value='0'>Select Product Code</option>";
                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    {
                        s = s + "<option value=" + item.ProductDetailID.ToString() + ">" + item.ProductCode + " - " + item.ProductName + "</option>";
                    }
                }
                s = s + "</select>";
                return Content(s);
            }
        }

        private static List<SelectListItem> fnListOfClientIDs()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblClient
                          join pm in db.tblProductMaster on t.ClientID equals pm.ClientID
                          join pd in db.tblProductDetail on pm.ProductMasterID equals pd.ProductDetailID
                          orderby t.ClientName
                          select new SelectListItem { Value = t.ClientID.ToString(), Text = t.CMCLocation + " - " + t.ClientName }).Distinct().ToList();
                mylist = (from t in mylist orderby t.Text select t).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });
                return mylist;
            }
        }

        private static List<SelectListItem> fnListOfProductCodes(int clientid)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();
                mylist = (from t in db.tblProductDetail
                          join pm in db.tblProductMaster on t.ProductMasterID equals pm.ProductMasterID
                          where pm.ClientID == clientid
                          orderby t.ProductCode
                          select new SelectListItem { Value = t.ProductDetailID.ToString(), Text = t.ProductName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Product Code" });
                return mylist;
            }
        }
    }
}