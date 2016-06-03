using MvcPhoenix.Models;
using MvcPhoenix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class BulkController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        public ActionResult Index()
        {
            // The index view is the landing page to search for a container for edit
            ViewBag.ListOfClients = BulkService.fnClientIDs();
            ViewBag.ListOfBulkStatus = BulkService.fnBulkStatusIDs();
            ViewBag.ListOfWarehouses = BulkService.fnWarehouseIDs();
            return View("~/Views/Bulk/Index.cshtml");
        }

        public ActionResult BuildProductMasterDropDown(int id)
        {
            // id=clientid .. return the <option> values for the <select> tag
            return Content(BulkService.fnBuildProductMasterDropDown(id));
        }

        [HttpPost]
        public ActionResult Search1()
        {
            int ClientID = Convert.ToInt32(Request.Form["clientid"]);
            int PK = Convert.ToInt32(Request.Form["productmasterid"]);
            if (ClientID == 0 || PK == 0)
            {
                return Content("<strong>Please Select a Client and Mastercode</strong>");
            }
            else
            {
                List<BulkContainerViewModel> mylist = BulkService.fnBulkContainerList();
                mylist = (from t in mylist where t.clientid == ClientID && t.productmasterid == PK select t).ToList();
                return PartialView("~/Views/Bulk/_SearchResults.cshtml", mylist);
            }
        }

        [HttpPost]
        public ActionResult Search2()
        {
            string BulkStatus = Request.Form["BulkStatus"];
            string Warehouse = Request.Form["Warehouse"];
            List<BulkContainerViewModel> mylist = BulkService.fnBulkContainerList();
            if (String.IsNullOrEmpty(BulkStatus) && String.IsNullOrEmpty(Warehouse))
            {
                return Content("<strong>Please Select a Bulk Status and/or a Warehouse</strong>");
            }
            if (!String.IsNullOrEmpty(BulkStatus))
            {
                mylist = (from t in mylist where t.bulkstatus == BulkStatus select t).ToList();
            }
            if (!String.IsNullOrEmpty(Warehouse))
            {
                mylist = (from t in mylist where t.warehouse == Warehouse select t).ToList();
            }
            return PartialView("~/Views/Bulk/_SearchResults.cshtml", mylist);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = BulkService.fnFillBulkContainerFromDB(id);
            return View("~/Views/Bulk/Edit.cshtml", obj);
        }

        [HttpPost]
        public ActionResult Save(BulkContainerViewModel obj)
        {
            // WIP
            bool bUpdate = BulkService.fnSaveBulk(obj);
            TempData["SaveResult"] = "Bulk Container Saved at " + System.DateTime.Now.ToString();
            return RedirectToAction("Edit", new { id = obj.bulkid });
        }
    }
}