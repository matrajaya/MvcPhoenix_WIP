using MvcPhoenix.Models;
using MvcPhoenix.Services;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class InvoiceController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            int ClientID = Convert.ToInt32(fc["NewClientID"]);
            var vm = InvoiceService.CreateInvoice(ClientID);

            /// string client, string division, 
            /// Assign invoice period based on current date: Get: ClientID, BillingGroup, Period
            /// Automated business rules from service
            /// Assign Invoice ID = -1 and go to edit action
            /// Ephemeral until the user executes save action

            return View("~/Views/Orders/Edit.cshtml", vm);
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
                // todo: insert success messsage
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