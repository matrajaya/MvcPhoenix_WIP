using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ShelfMasterController : Controller
    {
        public ActionResult Index(int id)
        {
            // build the landing page for shelf masters belonging to a productdetailid
            ShelfMasterViewModel obj = new ShelfMasterViewModel();
            var mylist = ShelfMasterService.GetShelfs(id);
            FillIndexViewBag(id);
            return View("~/Views/ShelfMaster/Index.cshtml", mylist);
        }

        public ActionResult ShelfMasterList(int id)
        {
            // for the partial in Products Edit
            var mylist = ShelfMasterService.GetShelfs(id);

            // Exclude inactive sizes from shared list
            var mylistx = (from x in mylist
                           where x.inactivesize == false
                           select x).ToList();

            ViewBag.ParentID = id;
            return PartialView("~/Views/Product/_ShelfSize.cshtml", mylistx);
        }

        public ActionResult FillIndexViewBag(int id)
        {
            // This will change later (after some DB changes)
            using (var db = new CMCSQL03Entities())
            {
                var dbProductDetail = db.tblProductDetail.Find(id);
                var dbProductMaster = db.tblProductMaster.Find(dbProductDetail.ProductMasterID);
                var dbClient = db.tblClient.Find(dbProductMaster.ClientID);
                ViewBag.ParentID = id;
                ViewBag.ClientID = dbClient.ClientID;
                ViewBag.ClientName = dbClient.ClientName;
                ViewBag.ProductCode = dbProductDetail.ProductCode;
                ViewBag.ProductName = dbProductDetail.ProductName;
                return null;
            }
        }

        [HttpGet]
        public ActionResult Create(int id)
        {
            ShelfMasterViewModel obj = new ShelfMasterViewModel();
            obj = ShelfMasterService.CreateShelf(id);
            return PartialView("~/Views/ShelfMaster/_Edit.cshtml", obj);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var shelf = ShelfMasterService.GetShelf(id);

            return PartialView("~/Views/ShelfMaster/_Edit.cshtml", shelf);
        }

        [HttpGet]
        public ActionResult CloneShelfMaster(int id)
        {
            // id=shelfid
            int productid = ShelfMasterService.CloneShelf(id);

            if (productid == 0)
            {
                return null;
            }

            return RedirectToAction("Index", new { id = productid });
        }

        public ActionResult BuildShelfMasterPackagesDropDown()
        {
            return Content(ApplicationService.ddlBuildShelfMasterPackage());
        }

        [HttpPost]
        public ActionResult Save(ShelfMasterViewModel obj)
        {
            // do the work based on the button clicked in the Save action of the form

            string UserChoice = Request.Form["submitbutton"];

            if (UserChoice == "Cancel")
            {
                // just fall thru
            }
            if (UserChoice == "Save")
            {
                ShelfMasterService.SaveShelf(obj);
            }
            if (UserChoice == "Delete")
            {
                // Need server side validation for Delete - no tblStock records
                ShelfMasterService.DeleteShelf(obj.shelfid);
            }

            return RedirectToAction("Index", new { id = obj.productdetailid });
        }
    }
}