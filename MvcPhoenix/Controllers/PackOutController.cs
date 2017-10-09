using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class PackOutController : Controller
    {
        public ActionResult Index(int id, int productdetailid)
        {
            using (var db = new CMCSQL03Entities())
            {
                var bulkId = id;
                var bulkContainer = BulkService.GetBulkContainer(bulkId);

                TempData["productdetailid"] = productdetailid;

                return View("~/Views/PackOut/Index.cshtml", bulkContainer);
            }
        }

        public ActionResult CreateNewPackout(BulkContainerViewModel bulkcontainer, FormCollection form)
        {
            int ReturnProductDetailID = Convert.ToInt32(form["productdetailid"]);
            int Priority = Convert.ToInt32(form["priority"]);

            if (String.IsNullOrEmpty(form["priority"]))
            {
                TempData["ResultMessage"] = "Please select a Priority value";
                return RedirectToAction("Index", new { id = bulkcontainer.bulkid, productdetailid = ReturnProductDetailID });
            }

            int PackOutResult = PackoutService.CreatePackOutOrder(bulkcontainer.bulkid, Priority);

            if (PackOutResult == -1)
            {
                TempData["ResultMessage"] = "There is already an existing Pack Out order for the selected Bulk item";
                return RedirectToAction("Index", new { id = bulkcontainer.bulkid, productdetailid = ReturnProductDetailID });
            }

            if (PackOutResult == 0)
            {
                TempData["ResultMessage"] = "An error occurred trying to create a Pack Out on " + DateTime.UtcNow.ToString("R");
            }

            if (PackOutResult > 0)
            {
                TempData["ResultMessage"] = "New packout order number " + PackOutResult.ToString() + " successfully created on " + DateTime.UtcNow.ToString("R");
                return RedirectToAction("Index", new { id = bulkcontainer.bulkid, productdetailid = ReturnProductDetailID });
            }

            return RedirectToAction("Edit", "Inventory", new { id = ReturnProductDetailID });
        }
    }
}