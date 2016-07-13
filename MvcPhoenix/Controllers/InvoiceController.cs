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
            InvoiceViewModel CI = new InvoiceViewModel();
            
            CI.invoiceid = id;
            CI = InvoiceService.FillInvoice(CI);
            
            return View(CI);
        }

        public ActionResult View(int id)
        {
            InvoiceViewModel CI = new InvoiceViewModel();

            CI.invoiceid = id;
            CI = InvoiceService.FillInvoice(CI);

            return View(CI);
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