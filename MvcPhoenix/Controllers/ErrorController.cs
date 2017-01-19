using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error/NotFound
        [AllowAnonymous]
        public ActionResult NotFound()
        {
            //return a status code for proper seo
            Response.StatusCode = 404;
            return View();
        }

        // GET: Error/Error
        [AllowAnonymous]
        public ActionResult Error()
        {
            // Error handled in the global.asax.cs code.
            // return a status code for proper seo
            Response.StatusCode = 500;
            return View();
        }
    }
}