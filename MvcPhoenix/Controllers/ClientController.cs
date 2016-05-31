using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using PagedList;

namespace MvcPhoenix.Controllers
{
    public class ClientController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        // GET: Client/Index
        [AllowAnonymous]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.LocationSortParm = sortOrder == "location" ? "location_desc" : "location";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var clients = from c in db.tblClient2
                        select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                clients = clients.Where(c => c.ClientName.Contains(searchString)
                    || c.ClientCode.Contains(searchString)
                    || c.CMCLocation.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    clients = clients.OrderByDescending(c => c.ClientName);
                    break;

                case "location":
                    clients = clients.OrderBy(c => c.CMCLocation);
                    break;

                case "location_desc":
                    clients = clients.OrderByDescending(c => c.CMCLocation);
                    break;

                default:
                    clients = clients.OrderBy(c => c.ClientName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(clients.ToPagedList(pageNumber, pageSize));
        }

        #region TODO

        // GET: Client/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            //// POST: Client/Create
            //[HttpPost]
            //[AllowAnonymous]
            //public ActionResult Create(FormCollection collection)
            //{
            //    try
            //    {
            //        // TODO: Add insert logic here
            //        return RedirectToAction("Index");
            //    }
            //    catch
            //    {
            //        return View();
            //    }
            //}

            return View();
        }

        
        // GET: Client/Edit/id
        [AllowAnonymous]
        public ActionResult Edit(int id)
        {
            ClientProfile CP = new ClientProfile();
            CP.ClientID = id;
            CP = ClientService.FillFromDB(CP);

            return View(CP);
        }

        // GET: Client/View/id
        [AllowAnonymous]
        public ActionResult View(int id)
        {
            ClientProfile obj = new ClientProfile();
            obj.ClientID = id;
            obj = ClientService.FillFromDB(obj);

            return View(obj);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SaveClientProfile(ClientProfile CPVM)
        {
            int pk = ClientService.fnSaveClientProfile(CPVM);

            return RedirectToAction("Edit", new { id = pk });
        }

        #endregion
    }
}