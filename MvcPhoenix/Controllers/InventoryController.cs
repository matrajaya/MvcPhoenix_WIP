using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPhoenix.Models;

namespace MvcPhoenix.Controllers
{
    public class InventoryController : Controller
    {
        //
        // GET: /Inventory/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult ShelfStock()
        {
            return View();
        }

        public ActionResult BulkStock()
        {
            return View();
        }
    }
}
