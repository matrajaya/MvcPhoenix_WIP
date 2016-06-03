using Postal;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class HomeController : Controller
    {
        //[AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Email()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Email(FormCollection fc)
        {
            string emailto = fc["email"];
            string urllink = fc["link"];
            return RedirectToAction("EmailTest", new { emailto, urllink });
        }

        public ActionResult EmailTest(string emailTo, string urlLink)
        {
            dynamic email = new Email("Example");

            email.From = User.Identity.Name;
            email.To = emailTo;
            email.LinkUrl = urlLink;
            //email.To = "ifeanyiigbo@chemicalmarketing.com";
            //email.LinkUrl = "https://dev_serv/Products/PrintProfile/451";

            email.Send();

            return new EmailViewResult(email);
            //return RedirectToAction("Index");
        }
    }
}