using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
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

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            /// string client, string division, 
            /// Assign invoice period based on current date: Get: ClientID, BillingGroup, Period
            /// Automated business rules from service
            /// Assign Invoice ID = -1 and go to edit action
            /// Ephemeral until the user executes save action
            int ClientID = Convert.ToInt32(fc["ClientID"]);
            int DivisionID = Convert.ToInt32(fc["billinggroupid"]);

            InvoiceViewModel obj = new InvoiceViewModel();
            obj = InvoiceService.CreateInvoice(ClientID, DivisionID);
            
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
        
    }
}