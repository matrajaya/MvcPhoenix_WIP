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
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Edit(int id)
        {
            var vm = InvoiceService.FillInvoice(id);
            
            return View(vm);
        }
                
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SaveInvoice(InvoiceViewModel vm)
        {
            int pk = vm.invoiceid;

            if (ModelState.IsValid)
            {
                pk = InvoiceService.SaveInvoice(vm);
                //todo: insert success messsage
            }

            return RedirectToAction("Edit", new { id = pk });
        }

        [AllowAnonymous]
        public ActionResult View(int id)
        {
            var vm = InvoiceService.FillInvoice(id);
            return View(vm);
        }

        [AllowAnonymous]
        public ActionResult GenerateInvoice(string client, string division)
        {
            /// Assign invoice period based on current date: Get: ClientID, BillingGroup, Period
            /// Automated business rules from service
            /// Assign Invoice ID = -1 and go to edit action
            /// Ephemeral until the user executes save action 
            
            return View();
        }

    }
}