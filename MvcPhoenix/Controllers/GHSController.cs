using MvcPhoenix.EF;
using MvcPhoenix.Extensions;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class GHSController : Controller
    {
        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else { searchString = currentFilter; }

            ViewBag.CurrentFilter = searchString;
            ViewBag.SearchString = searchString;

            using (var db = new CMCSQL03Entities())
            {
                var phCodes = db.tblGHSPHSource.AsQueryable();

                if (!String.IsNullOrEmpty(searchString))
                {
                    phCodes = phCodes.Where(p => p.PHNumber.Contains(searchString)
                                              && p.Language == "EN"
                                              || p.PHStatement.Contains(searchString)
                                              && p.Language == "EN");
                }

                phCodes = phCodes.OrderBy(p => p.PHNumber);

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                return PartialView(phCodes.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Index(int id)
        {
            int productDetailId = id;
            var GHS = new GHSViewModel();
            ViewBag.ParentID = productDetailId;
            TempData["ProductDetId"] = productDetailId;

            var product = ProductService.GetProductDetail(productDetailId);

            ViewBag.productcode = product.productcode;
            ViewBag.productname = product.productname;
            ViewBag.ProductDetailID = product.productdetailid;

            try
            {
                GHS = GHSService.GetGHS(productDetailId);

                return View("Index", GHS);
            }
            catch (Exception)
            {
                return View("Index", GHS);
            }
        }

        /// <summary>
        /// Displays readonly GHS information in Product detail.
        /// Returns blank view if model is empty.
        /// </summary>
        public ActionResult GHSDisplay(int? id)
        {
            int productDetailId = id ?? 0;
            var GHS = new GHSViewModel();
            ViewBag.ParentID = productDetailId;

            var product = ProductService.GetProductDetail(productDetailId);

            ViewBag.productcode = product.productcode;
            ViewBag.productname = product.productname;
            ViewBag.ProductDetailID = product.productdetailid;

            try
            {
                GHS = GHSService.GetGHS(productDetailId);

                return PartialView("~/Views/Product/_GHSInfo.cshtml", GHS);
            }
            catch (Exception)
            {
                return PartialView("~/Views/Product/_GHSInfo.cshtml", GHS);
            }
        }

        public ActionResult Detail(int id)
        {
            int productDetailId = id;

            var ghsDetail = GHSService.GetGHSDetails(productDetailId);

            return PartialView(ghsDetail);
        }

        public ActionResult Save(GHSViewModel ghs)
        {
            GHSService.SaveGHS(ghs);

            return null;
        }

        public ActionResult AddPHDetail(int id)
        {
            int phSourceId = id;
            int productDetailId = Convert.ToInt32(TempData["ProductDetId"]);

            GHSService.CreatePHDetail(phSourceId, productDetailId);

            return null;
        }

        [HttpPost]
        public ActionResult ExcludePH(int id, bool isChecked)
        {
            int phDetailId = id;

            GHSService.UpdatePHDetailExclude(phDetailId, isChecked);

            return null;
        }

        public ActionResult DeletePHDetail(int id)
        {
            int phDetailId = id;

            GHSService.DeletePH(phDetailId);

            return null;
        }

        [HttpGet]
        public ActionResult EditPHSource(string phnumber, string lang)
        {
            var phSource = GHSService.EditPHSource(phnumber, lang);

            return PartialView("~/Views/GHS/EditPHSource.cshtml", phSource);
        }

        public ActionResult SavePHSource(GHSPHSource phsource)
        {
            GHSService.UpdatePHSource(phsource);

            return null;
        }

        // Generates temporary model values in clone modal
        public ActionResult Clone(int id)
        {
            int phSourceId = id;
            var PHSource = new GHSPHSource();
            var ClonePHSource = GHSService.CloneAltPHSource(phSourceId);

            PHSource = ClonePHSource.Item1;
            ViewBag.OriginalPHNumber = ClonePHSource.Item2;

            return PartialView("CloneEdit", PHSource);
        }

        public ActionResult SaveClone(GHSPHSource phsource, string originalPHNumber)
        {
            int phSourceId = GHSService.SavePHSourceClone(phsource, originalPHNumber);

            return RedirectToAction("AddPHDetail", new { id = phSourceId });
        }
    }
}