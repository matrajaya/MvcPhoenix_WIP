using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcPhoenix.Models;

namespace MvcPhoenix.Controllers
{
    public class PackOutController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public ActionResult Index(int id, int productdetailid)
        {
            using(db)
            {
                TempData["productdetailid"] = productdetailid;    // to stash calling parameter into form for read on postback cause PDid not part of Bulk vm
                var vm = Services.BulkService.fnFillBulkContainerFromDB(id);

                return View("~/Views/PackOut/Index.cshtml",vm);
            }

        }


        public ActionResult CreateNewPackout(BulkContainerViewModel vm, FormCollection fc)
        {
            int ReturnProductDetailID = Convert.ToInt32(fc["productdetailid"]);
            int Priority = Convert.ToInt32(fc["priority"]);

            if(String.IsNullOrEmpty(fc["priority"]))
            {
                TempData["ResultMessage"] = "Please select a Priority value";
                return RedirectToAction("Index", new { id = vm.bulkid, productdetailid = ReturnProductDetailID });
            }
            
            string PackOutResult = Services.Packout.fnCreatePackOutOrder(vm.bulkid,Priority);

            if (PackOutResult == "PackOutExists")
            {
                TempData["ResultMessage"] = "There is already an existing Pack Out order for the selected Bulk item";
                return RedirectToAction("Index", new { id = vm.bulkid, productdetailid = ReturnProductDetailID });
            }
            //else if (PackOutResult == "BulkIsInQCStatus")
            //{
            //    TempData["ResultMessage"] = "The product you've selected is in QC status. You cannot create a packout";
            //    return RedirectToAction("Index", new { id = vm.bulkid, productdetailid = ReturnProductDetailID });
            //}
            //else if (PackOutResult == "BulkIsInOtherStatus")
            //{
            //    TempData["ResultMessage"] = "The product you've selected is in HOLD,TEST, or RETURN status. You cannot create a packout";
            //    return RedirectToAction("Index", new { id = vm.bulkid, productdetailid = ReturnProductDetailID });
            //}
            
            else if(PackOutResult == "Success")
            {
                TempData["ResultMessage"] = "New packout order successfully created: " + System.DateTime.Now.ToString();
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