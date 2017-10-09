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
            return Content(ApplicationService.ddlBuildProductMaster(clientId));
        }

        [HttpPost]
        public ActionResult Search1()
        {
            int clientId = Convert.ToInt32(Request.Form["clientid"]);
            int productMasterId = Convert.ToInt32(Request.Form["productmasterid"]);

            if (clientId < 1 || productMasterId < 1)
            {
                return Content("<strong>Please Select a Client and Mastercode</strong>");
            }
            else
            {
                var bulkContainers = BulkService.GetBulkContainers();
                bulkContainers = bulkContainers.Where(t => t.clientid == clientId 
                                                        && t.productmasterid == productMasterId).ToList();

                return PartialView("~/Views/Bulk/_SearchResults.cshtml", bulkContainers);
            }
        }

        [HttpPost]
        public ActionResult Search2()
        {
            string bulkStatus = Request.Form["BulkStatus"];
            string warehouse = Request.Form["Warehouse"];
            var bulkContainers = BulkService.GetBulkContainers();

            if (String.IsNullOrEmpty(bulkStatus) && String.IsNullOrEmpty(warehouse))
            {
                return Content("<strong>Please Select a Bulk Status and/or a Warehouse</strong>");
            }

            if (!String.IsNullOrEmpty(bulkStatus))
            {
                bulkContainers = bulkContainers.Where(t => t.bulkstatus == bulkStatus).ToList();
            }

            if (!String.IsNullOrEmpty(warehouse))
            {
                bulkContainers = bulkContainers.Where(t => t.warehouse == warehouse).ToList();
            }

            return PartialView("~/Views/Bulk/_SearchResults.cshtml", bulkContainers);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            int bulkId = id;
            var bulkContainer = BulkService.GetBulkContainer(bulkId);

            return View("~/Views/Bulk/Edit.cshtml", bulkContainer);
        }

        [HttpPost]
        public ActionResult Save(BulkContainerViewModel bulkContainer)
        {
            bool isSaved = BulkService.SaveBulkContainer(bulkContainer);

            if (isSaved == true)
            {
                TempData["SaveResult"] = "Bulk container saved on " + DateTime.UtcNow.ToString("R");
            }
            else
            {
                TempData["SaveResult"] = "Something went wrong. Bulk container was not saved.";
            }
            
            return RedirectToAction("Edit", new { id = bulkContainer.bulkid });
        }
    }
}