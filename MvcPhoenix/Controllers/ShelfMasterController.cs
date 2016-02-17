using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcPhoenix.Models;
using MvcPhoenix.Services;

namespace MvcPhoenix.Controllers
{
    public class ShelfMasterController : Controller
    {

        public ActionResult Index(int id)
        {
            // build the landing page for shelf masters belonging to a productdetailid
            ShelfMasterViewModel obj = new ShelfMasterViewModel();
            var mylist = ShelfMasterService.fnListOfShelfMasters(id);
            return View("~/Views/ShelfMaster/Index.cshtml", mylist);
        }

        public ActionResult ShelfMasterList(int id)
        {
            // for the partial in Products Edit
            var mylist = ShelfMasterService.fnListOfShelfMasters(id);
            return PartialView("~/Views/Products/_ShelfSize.cshtml", mylist);
        }


        [HttpGet]
        public ActionResult Create(int id)
        {
            ShelfMasterViewModel obj = new ShelfMasterViewModel();
            obj = ShelfMasterService.fnCreateNewShelfMaster(id);
            return PartialView("~/Views/ShelfMaster/_Edit.cshtml", obj);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            ShelfMasterViewModel obj = new ShelfMasterViewModel();
            obj = ShelfMasterService.fnFillShelfMasterFromDB(id);
            return PartialView("~/Views/ShelfMaster/_Edit.cshtml", obj);
        }

        [HttpGet]
        public ActionResult CloneShelfMaster(int id)
        {
            // id=shelfid
            int PDid = ShelfMasterService.fnCloneShelfMaster(id);
            return RedirectToAction("Index", new { id = PDid });
        }

        public ActionResult BuildShelfMasterPackagesDropDown(string id)
        {
            // id=clientid .. return the <option> values for the <select> tag
            return Content(ShelfMasterService.fnBuildShelfMasterPackagesDropDown(id));
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
                //if(ShelfMasterService.isValidShelfMaster(obj))
                //{
                    ShelfMasterService.fnSaveShelfMaster(obj);
                //}
                //else
                //{
                //    return Content("dede");
                //}
                
            }
            if (UserChoice == "Delete")
            {
                // Need server side validation for Delete - no tblStock records
                ShelfMasterService.fnDeleteShelfMaster(obj.shelfid);
            }
            return RedirectToAction("Index", new { id = obj.productdetailid });
        }





    }
}
