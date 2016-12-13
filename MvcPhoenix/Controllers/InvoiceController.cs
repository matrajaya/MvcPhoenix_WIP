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
            ViewBag.ListOfClients = InvoiceService.ListOfClientIDs();
            List<InvoiceViewModel> mylist = InvoiceService.IndexList();

            return View(mylist);
        }

        public ActionResult BuildBillingGroupDDL(int id)
        {
            return Content(InvoiceService.fnBuildBillingGroupDDL(id));
        }

        public ActionResult Search(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new EF.CMCSQL03Entities())
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
            /// string client, string division,
            /// Assign invoice period based on current date: Get: ClientID, BillingGroup, Period
            /// Automated business rules from service
            /// Assign Invoice ID = -1 ansd go to edit action
            /// Ephemeral until the user executes save action
            int ClientID = Convert.ToInt32(fc["ClientID"]);
            int DivisionID = Convert.ToInt32(fc["billinggroupid"]);
            string InvoicePeriod = fc["invoiceperiod"];
            DateTime StartDate = Convert.ToDateTime(fc["startdate"]);
            DateTime EndDate = Convert.ToDateTime(fc["enddate"]);

            InvoiceViewModel obj = new InvoiceViewModel();
            obj = InvoiceService.CreateInvoice(ClientID, DivisionID, InvoicePeriod, StartDate, EndDate);

            return View("Edit", obj);
        }

        public ActionResult Edit(int id)
        {
            var vm = InvoiceService.FillInvoice(id);

            return View(vm);
        }

        [HttpPost]
        public ActionResult SaveInvoice(InvoiceViewModel vm)
        {
            int pk = vm.invoiceid;

            if (ModelState.IsValid)
            {
                pk = InvoiceService.SaveInvoice(vm);
                TempData["SaveResult"] = "Invoice updated at " + DateTime.Now;
            }

            return RedirectToAction("Edit", new { id = pk });
        }

        public ActionResult View(int id)
        {
            var vm = InvoiceService.FillInvoice(id);
            return View(vm);
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
                DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") +
                "                                                                                                                                    " +
                " Page: [page]/[toPage]\"" +
                " --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
            return footer;
        }
    }
}