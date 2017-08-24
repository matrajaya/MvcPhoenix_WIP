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
        public ActionResult Index()
        {
            return View("~/Views/Bulk/Index.cshtml");
        }

        public ActionResult BuildProductMasterDropDown(int clientId)
        {
            // id=clientid .. return the <option> values for the <select> tag
            return Content(ApplicationService.ddlBuildProductMasterDropDown(clientId));
        }

        [HttpPost]
        public ActionResult Search1()
        {
            int ClientID = Convert.ToInt32(Request.Form["clientid"]);
            int ProductMasterID = Convert.ToInt32(Request.Form["productmasterid"]);

            if (ClientID == 0 || ProductMasterID == 0)
            {
                return Content("<strong>Please Select a Client and Mastercode</strong>");
            }
            else
            {
                List<BulkContainerViewModel> result = BulkService.BulkContainers();
                result = (from t in result
                          where t.clientid == ClientID
                          && t.productmasterid == ProductMasterID
                          select t).ToList();

                return PartialView("~/Views/Bulk/_SearchResults.cshtml", result);
            }
        }

        [HttpPost]
        public ActionResult Search2()
        {
            string BulkStatus = Request.Form["BulkStatus"];
            string Warehouse = Request.Form["Warehouse"];
            List<BulkContainerViewModel> result = BulkService.BulkContainers();

            if (String.IsNullOrEmpty(BulkStatus) && String.IsNullOrEmpty(Warehouse))
            {
                return Content("<strong>Please Select a Bulk Status and/or a Warehouse</strong>");
            }

            if (!String.IsNullOrEmpty(BulkStatus))
            {
                result = (from t in result
                          where t.bulkstatus == BulkStatus
                          select t).ToList();
            }

            if (!String.IsNullOrEmpty(Warehouse))
            {
                result = (from t in result
                          where t.warehouse == Warehouse
                          select t).ToList();
            }

            return PartialView("~/Views/Bulk/_SearchResults.cshtml", result);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            BulkContainerViewModel obj = new BulkContainerViewModel();
            obj = BulkService.FillBulkContainer(id);

            return View("~/Views/Bulk/Edit.cshtml", obj);
        }

        [HttpPost]
        public ActionResult Save(BulkContainerViewModel model)
        {
            bool isSaved = BulkService.SaveBulkContainer(model);

            if (isSaved == true)
            {
                TempData["SaveResult"] = "Bulk container saved on " + DateTime.UtcNow.ToString();
            }
            else
            {
                TempData["SaveResult"] = "Something went wrong. Bulk container was not saved.";
            }
            
            return RedirectToAction("Edit", new { id = model.bulkid });
        }
    }
}