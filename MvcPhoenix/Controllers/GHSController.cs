using MvcPhoenix.Models;
using PagedList;
using Rotativa;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class GHSController : Controller
    {
        public ActionResult Index(int id)
        {
            ViewBag.ParentID = id;
            return View("Index");
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

                var phCodes = from p in db.tblGHSPHSource select p;

                if (!String.IsNullOrEmpty(searchString))
                {
                    phCodes = phCodes.Where(p => p.PHNumber.Contains(searchString)
                        || p.PHStatement.Contains(searchString));
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

        public ActionResult GHSDisplay(int id)
        {
            ViewBag.ParentID = id;
            return PartialView("~/Views/Products/_GHSInfo.cshtml");
        }
        
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //GHSViewModel obj = new GHSViewModel();
            //obj = GHSService.fnFillShelfMasterFromDB(id);
            //return PartialView("~/Views/ShelfMaster/_Edit.cshtml", obj);

            return null;
        }

        [HttpGet]
        public ActionResult CloneShelfMaster(int id)
        {
            // id=shelfid
            //int PDid = GHSService.fnCloneShelfMaster(id);
           // return RedirectToAction("Index", new { id = PDid });

            return null;
        }

        [HttpPost]
        public ActionResult Save(ShelfMasterViewModel obj)
        {
            // do the work based on the button clicked in the Save action of the form

            //string UserChoice = Request.Form["submitbutton"];

            //if (UserChoice == "Cancel")
            //{
            //    // just fall thru
            //}
            //if (UserChoice == "Save")
            //{
            //    ShelfMasterService.fnSaveShelfMaster(obj);
            //}
            //if (UserChoice == "Delete")
            //{
            //    // Need server side validation for Delete - no tblStock records
            //    ShelfMasterService.fnDeleteShelfMaster(obj.shelfid);
            //}

            //return RedirectToAction("Index", new { id = obj.productdetailid });

            return null;
        }

    }
}