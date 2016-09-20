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
    }
}