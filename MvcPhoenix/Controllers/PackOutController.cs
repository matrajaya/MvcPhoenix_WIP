using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class PackOutController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public ActionResult Index(int id, int productdetailid)
        {
            using (db)
            {
                TempData["productdetailid"] = productdetailid;    // to stash calling parameter into form for read on postback cause PDid not part of Bulk vm
                var vm = BulkService.fnFillBulkContainerFromDB(id);

                return View("~/Views/PackOut/Index.cshtml", vm);
            }
        }

        public ActionResult CreateNewPackout(BulkContainerViewModel vm, FormCollection fc)
        {
            int ReturnProductDetailID = Convert.ToInt32(fc["productdetailid"]);
            int Priority = Convert.ToInt32(fc["priority"]);

            if (String.IsNullOrEmpty(fc["priority"]))
            {
                TempData["ResultMessage"] = "Please select a Priority value";
                return RedirectToAction("Index", new { id = vm.bulkid, productdetailid = ReturnProductDetailID });
            }

            int PackOutResult = Packout.fnCreatePackOutOrder(vm.bulkid, Priority);

            if (PackOutResult == -1)
            {
                TempData["ResultMessage"] = "There is already an existing Pack Out order for the selected Bulk item";
                return RedirectToAction("Index", new { id = vm.bulkid, productdetailid = ReturnProductDetailID });
            }
            else if (PackOutResult == 0)
            {
                TempData["ResultMessage"] = "An error occurred trying to create a Pack Out on " + DateTime.UtcNow.ToString("R");
            }
            else
            {
                TempData["ResultMessage"] = String.Format("New packout order number {0} successfully created on {1}", PackOutResult.ToString(), DateTime.UtcNow.ToString("R"));
                return RedirectToAction("Index", new { id = vm.bulkid, productdetailid = ReturnProductDetailID });
            }

            return RedirectToAction("Edit", "Inventory", new { id = ReturnProductDetailID });
        }

        public ActionResult ListPackouts()
        {
            return null;
        }
    }
}