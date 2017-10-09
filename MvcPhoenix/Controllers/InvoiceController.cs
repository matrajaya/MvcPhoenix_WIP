using MvcPhoenix.EF;
using MvcPhoenix.Models;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class InvoiceController : Controller
    {
        public ActionResult Index()
        {
            List<InvoiceViewModel> invoicelist = InvoiceService.IndexList();

            return View(invoicelist);
        }

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new CMCSQL03Entities())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.LocationSortParm = sortOrder == "location" ? "location_desc" : "location";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var clients = from c in db.tblInvoice
                              select c;

                if (!String.IsNullOrEmpty(searchString))
                {
                    clients = clients.Where(c => c.ClientName.Contains(searchString)
                        || c.BillingGroup.Contains(searchString)
                        || c.WarehouseLocation.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        clients = clients.OrderByDescending(c => c.ClientName);
                        break;

                    case "location":
                        clients = clients.OrderBy(c => c.WarehouseLocation);
                        break;

                    case "location_desc":
                        clients = clients.OrderByDescending(c => c.WarehouseLocation);
                        break;

                    default:
                        clients = clients.OrderBy(c => c.ClientName);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(clients.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            int ClientID = Convert.ToInt32(fc["ClientID"]);
            string BillingGroup = (fc["billinggroup"]);
            DateTime StartDate = Convert.ToDateTime(fc["startdate"]);
            DateTime EndDate = Convert.ToDateTime(fc["enddate"]);

            int invoiceid = InvoiceService.CreateInvoice(ClientID, BillingGroup, StartDate, EndDate);

            InvoiceService.GenerateInvoice(invoiceid);

            return RedirectToAction("Edit", new { id = invoiceid });
        }

        public ActionResult Edit(int id)
        {
            var vm = InvoiceService.FillInvoice(id);

            return View(vm);
        }

        [HttpPost]
        public ActionResult SaveInvoice(InvoiceViewModel vm)
        {
            int pk = vm.InvoiceId;

            if (ModelState.IsValid)
            {
                pk = InvoiceService.SaveInvoice(vm);
                TempData["SaveResult"] = "Invoice updated on " + DateTime.UtcNow.ToString("R");
            }

            return RedirectToAction("Edit", new { id = pk });
        }

        public ActionResult DeleteInvoice(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                string s = @"DELETE FROM tblInvoice WHERE InvoiceID=" + id;
                db.Database.ExecuteSqlCommand(s);
            }

            return null;
        }

        public ActionResult PrintInvoice(int id)
        {
            string footer = DocumentFooter();
            var vm = InvoiceService.FillInvoice(id);

            return new ViewAsPdf(vm) { CustomSwitches = footer };
        }

        private static string DocumentFooter()
        {
            string footer = "--footer-left \"Printed on: " +
                DateTime.UtcNow.ToString("R") +
                "                                                                                                                                    " +
                " Page: [page]/[toPage]\"" +
                " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

            return footer;
        }

        public ActionResult BuildBillingGroupDDL(int id)
        {
            return Content(ApplicationService.ddlBuildBillingGroup(id));
        }

        public ActionResult TierRateAdjustment(int? tierclient, int? tierlevel, DateTime tierstartdate, DateTime tierenddate)
        {
            InvoiceService.SampleRateTierAdjustment(tierclient, tierlevel, tierstartdate, tierenddate);
            
            return null;
        }        
    }
}