using MvcPhoenix.EF;
using MvcPhoenix.Extensions;
using MvcPhoenix.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class GHSController : Controller
    {
        public ActionResult Index(int id)
        {
            ViewBag.ParentID = id;
            TempData["ProductDetId"] = id;

            GHSViewModel GHS = new GHSViewModel();

            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    var pd = (from t in db.tblProductDetail
                              where t.ProductDetailID == id
                              select t).FirstOrDefault();

                    ViewBag.productcode = pd.ProductCode;
                    ViewBag.productname = pd.ProductName;
                    ViewBag.ProductDetailID = pd.ProductDetailID;

                    var ghs = (from t in db.tblGHS
                               where t.ProductDetailID == id
                               select t).FirstOrDefault();

                    GHS.GHSID = ghs.GHSID;
                    GHS.ProductDetailID = ghs.ProductDetailID;
                    GHS.GHSSignalWord = ghs.SignalWord;
                    GHS.GHSSymbol1 = ghs.Symbol1;
                    GHS.GHSSymbol2 = ghs.Symbol2;
                    GHS.GHSSymbol3 = ghs.Symbol3;
                    GHS.GHSSymbol4 = ghs.Symbol4;
                    GHS.GHSSymbol5 = ghs.Symbol5;
                    GHS.OtherLabelInfo = ghs.OtherLabelInfo;
                }

                return View("Index", GHS);
            }
            catch (Exception)
            {
                return View("Index", GHS);
            }
        }

        public ActionResult Detail(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                List<GHSDetail> model = new List<GHSDetail>();

                var phdetail = (from t in db.tblGHSPHDetail
                                join tsrc in db.tblGHSPHSource on t.PHNumber equals tsrc.PHNumber
                                where t.ProductDetailID == id
                                orderby t.PHNumber ascending
                                select new
                                {
                                    t.PHDetailID,
                                    t.ProductDetailID,
                                    t.ExcludeFromLabel,
                                    t.PHNumber,
                                    tsrc.Language,
                                    tsrc.PHStatement,
                                    tsrc.CreateUser
                                }).ToList();

                foreach (var item in phdetail)
                {
                    model.Add(new GHSDetail()
                    {
                        PHDetailID = item.PHDetailID,
                        ProductDetailID = Convert.ToInt32(item.ProductDetailID),
                        ExcludeFromLabel = item.ExcludeFromLabel,
                        PHNumber = item.PHNumber,
                        Language = item.Language,
                        PHStatement = item.PHStatement,
                        CreateUser = item.CreateUser
                    });
                }

                return PartialView(model);
            }
        }

        public ActionResult Save(GHSViewModel obj)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = (from t in db.tblGHS 
                         where t.ProductDetailID == obj.ProductDetailID 
                         select t).FirstOrDefault();

                if (q != null)
                {
                    q.SignalWord = obj.GHSSignalWord;
                    q.Symbol1 = obj.GHSSymbol1;
                    q.Symbol2 = obj.GHSSymbol2;
                    q.Symbol3 = obj.GHSSymbol3;
                    q.Symbol4 = obj.GHSSymbol4;
                    q.Symbol5 = obj.GHSSymbol5;
                    q.OtherLabelInfo = obj.OtherLabelInfo;

                    db.SaveChanges();
                }
                else
                {
                    var newrecord = new tblGHS
                    {
                        ProductDetailID = obj.ProductDetailID,
                        SignalWord = obj.GHSSignalWord,
                        Symbol1 = obj.GHSSymbol1,
                        Symbol2 = obj.GHSSymbol2,
                        Symbol3 = obj.GHSSymbol3,
                        Symbol4 = obj.GHSSymbol4,
                        Symbol5 = obj.GHSSymbol5,
                        OtherLabelInfo = obj.OtherLabelInfo
                    };

                    db.tblGHS.Add(newrecord);
                    db.SaveChanges();
                }
            }

            return null;
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

                var phCodes = from p in db.tblGHSPHSource
                              select p;

                if (!String.IsNullOrEmpty(searchString))
                {
                    phCodes = phCodes.Where(p => p.PHNumber.Contains(searchString) && p.Language == "EN"
                        || p.PHStatement.Contains(searchString) && p.Language == "EN");
                }

                switch (sortOrder)
                {
                    case "name":
                        phCodes = phCodes.OrderBy(p => p.PHStatement);
                        break;

                    case "name_desc":
                        phCodes = phCodes.OrderByDescending(p => p.PHStatement);
                        break;

                    case "code_desc":
                        phCodes = phCodes.OrderByDescending(p => p.PHNumber);
                        break;

                    default:
                        phCodes = phCodes.OrderBy(p => p.PHNumber);
                        break;
                }

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                return PartialView(phCodes.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult AddPHDetail(int id)
        {
            int proddetid = Convert.ToInt32(TempData["ProductDetId"]);

            using (var db = new CMCSQL03Entities())
            {
                var srcrcd = db.tblGHSPHSource.Find(id);

                var newrecord = new tblGHSPHDetail
                {
                    PHNumber = srcrcd.PHNumber,
                    ProductDetailID = proddetid,
                    CreateDate = DateTime.UtcNow,
                    CreateUser = HttpContext.User.Identity.Name
                };

                db.tblGHSPHDetail.Add(newrecord);
                db.SaveChanges();
            }

            return null;
        }

        [HttpPost]
        public ActionResult ExcludePH(int id, bool isChecked)
        {
            using (var db = new CMCSQL03Entities())
            {
                var dtrcd = db.tblGHSPHDetail.Find(id);

                dtrcd.ExcludeFromLabel = isChecked;
                db.SaveChanges();
            }

            return null;
        }

        public ActionResult DeletePHDetail(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                string s = @"DELETE FROM tblGHSPHDetail WHERE PHDetailID=" + id.ToString();
                db.Database.ExecuteSqlCommand(s);
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditPHSource(string phnumber, string lang)
        {
            GHSPHSource PHSrc = new GHSPHSource();
            
            using (var db = new CMCSQL03Entities())
            {
                var phsrc = (from t in db.tblGHSPHSource
                             where t.PHNumber == phnumber 
                             && t.Language == lang
                             select t).FirstOrDefault();

                if (phsrc != null)
                {
                    PHSrc.PHSourceID = phsrc.PHsourceID;
                    PHSrc.PHNumber = phsrc.PHNumber;
                    PHSrc.Language = phsrc.Language;
                    PHSrc.PHStatement = phsrc.PHStatement;
                }                
            }

            return PartialView("~/Views/GHS/EditPHSource.cshtml", PHSrc);
        }

        public ActionResult SavePHSource(GHSPHSource obj)
        {
            using (var db = new CMCSQL03Entities())
            {
                var PH = (from t in db.tblGHSPHSource 
                                    where t.PHsourceID == obj.PHSourceID 
                                    select t).SingleOrDefault();
                
                PH.PHStatement = obj.PHStatement;
                PH.UpdateDate = DateTime.UtcNow;
                PH.UpdateUser = HttpContext.User.Identity.Name;

                db.SaveChanges();
            }

            return null;
        }

        /// <summary>
        /// Generates temporary model values in clone modal
        /// </summary>
        public ActionResult Clone(int id)
        {
            GHSPHSource PHSrc = new GHSPHSource();
            string[] Suffix = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            using (var db = new CMCSQL03Entities())
            {
                var phsrc = (from t in db.tblGHSPHSource
                             where t.PHsourceID == id
                             select t).FirstOrDefault();

                ViewBag.OldPhNum = phsrc.PHNumber;

                int i = 0;
                PHSrc.PHNumber = phsrc.PHNumber + "-" + Suffix[i];
                var isExists = db.tblGHSPHSource.Any(r => r.PHNumber.Equals(PHSrc.PHNumber));

                while (isExists)
                {
                    ++i;
                    PHSrc.PHNumber = phsrc.PHNumber + "-" + Suffix[i];
                    isExists = db.tblGHSPHSource.Any(r => r.PHNumber.Equals(PHSrc.PHNumber));
                }

                PHSrc.PHSourceID = phsrc.PHsourceID;
                PHSrc.Language = phsrc.Language;
                PHSrc.PHStatement = phsrc.PHStatement;
            }

            return PartialView("CloneEdit", PHSrc);
        }

        public ActionResult SaveClone(GHSPHSource obj, string oldPHNumber)
        {
            using (var db = new CMCSQL03Entities())
            {
                var ph = (from p in db.tblGHSPHSource
                          where p.PHNumber == oldPHNumber
                          select p).ToList();

                for (int i = 0; i < ph.Count; i++)
                {
                    var newPh = ph[i].Clone();
                    newPh.PHNumber = obj.PHNumber;
                    newPh.Language = ph[i].Language;
                    newPh.PHStatement = ph[i].PHStatement;
                    newPh.CreateDate = DateTime.UtcNow;
                    newPh.CreateUser = HttpContext.User.Identity.Name;

                    if (newPh.Language == obj.Language)
                    {
                        newPh.PHStatement = obj.PHStatement;
                    }

                    db.tblGHSPHSource.Add(newPh);
                    db.SaveChanges();
                }

                // Get new id to pass to AddPhDetail action
                var pd = (from t in db.tblGHSPHSource
                          where t.PHNumber == obj.PHNumber
                          select t).FirstOrDefault();

                obj.PHSourceID = pd.PHsourceID;
            }

            return RedirectToAction("AddPHDetail", new { id = obj.PHSourceID });
        }

        /// <summary>
        /// Displays readonly GHS information in Product detail.
        /// Returns blank view if model is empty.
        /// </summary>
        public ActionResult GHSDisplay(int? id)
        {
            ViewBag.ParentID = id;
            GHSViewModel GHS = new GHSViewModel();

            try
            {
                using (var db = new CMCSQL03Entities())
                {
                    var pd = (from t in db.tblProductDetail
                              where t.ProductDetailID == id
                              select t).FirstOrDefault();

                    ViewBag.productcode = pd.ProductCode;
                    ViewBag.productname = pd.ProductName;
                    ViewBag.ProductDetailID = pd.ProductDetailID;

                    var ghs = (from t in db.tblGHS
                               where t.ProductDetailID == id
                               select t).FirstOrDefault();

                    GHS.GHSSignalWord = ghs.SignalWord;
                    GHS.GHSSymbol1 = ghs.Symbol1;
                    GHS.GHSSymbol2 = ghs.Symbol2;
                    GHS.GHSSymbol3 = ghs.Symbol3;
                    GHS.GHSSymbol4 = ghs.Symbol4;
                    GHS.GHSSymbol5 = ghs.Symbol5;
                    GHS.OtherLabelInfo = ghs.OtherLabelInfo;
                }

                return PartialView("~/Views/Products/_GHSInfo.cshtml", GHS);
            }
            catch (Exception)
            {
                return PartialView("~/Views/Products/_GHSInfo.cshtml", GHS);
            }
        }
    }
}