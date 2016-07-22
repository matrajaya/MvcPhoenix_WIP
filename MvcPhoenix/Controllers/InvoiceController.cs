using System.Web.Mvc;
using MvcPhoenix.Models;

namespace MvcPhoenix.Controllers
{
    public class InvoiceController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            var vm = InvoiceService.FillInvoice(id);
            
            return View(vm);
        }

        public ActionResult View(int id)
        {
            
            return View();
        }

        public ActionResult GenerateInvoice(string client, string division)
        {
            return View();
        }

        public ActionResult SaveInvoice(int id)
        {
            return View(Edit(id));
        }
    }
}