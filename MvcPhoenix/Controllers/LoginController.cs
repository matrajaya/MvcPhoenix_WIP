using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcPhoenix.Models;

namespace MvcPhoenix.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LiftOff()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return View("~/Views/Login/Logout.cshtml");
        }

        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            //if (User.Identity.IsAuthenticated) {
            // if good, redirect to landing Orders Landing
            bool isValidUser = true;
            isValidUser = fnIsValidUser(fc["UserName"], fc["Password"]);
            if (isValidUser == true)
            {
                FormsAuthentication.SetAuthCookie(fc["UserName"], true);
                return RedirectToAction("Index", "Orders");
            }
            else
            {
                ViewData["LoginResults"] = "Invalid Login";
                return RedirectToAction("Index", "Orders");
                //return View("~/Views/Login/Index.cshtml");
            }
        }

        private static bool fnIsValidUser(string myusername, string mypassword)
        {
            // Controller will set the auth cookie and redirect
            bool retvalue = false;
            
            MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();
            
            //Insert login log wedge here

            // Must return only one row
            int recs = (from t in db.tblUser
                        where t.UserName == myusername
                        where t.Password == mypassword
                        select t).Count();
            if (recs == 1)
            {
                retvalue = true;
            }
            else
            {
                retvalue = false;
            }
            return retvalue;
        }
    }
}
