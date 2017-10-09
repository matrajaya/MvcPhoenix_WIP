using MvcPhoenix.EF;
using MvcPhoenix.Models;
using MvcPhoenix.Services;
using PagedList;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ClientController : Controller
    {
        #region Client

        // GET: Client/Index
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            using (var db = new CMCSQL03Entities())
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

                var clients = from c in db.tblClient
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
        }

        // GET: Client/View/id
        [AllowAnonymous]
        public ActionResult View(int id)
        {
            int clientId = id;

            var clientProfile = ClientService.GetClient(clientId);

            return View(clientProfile);
        }

        // GET: Client/Edit/id
        [AllowAnonymous]
        public ActionResult Edit(int id)
        {
            int clientId = id;

            var clientProfile = ClientService.GetClient(clientId);

            return View(clientProfile);
        }

        // POST: Client/Create
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            string clientName = form["clientname"];
            string clientCode = form["clientcode"].ToUpper();
            string whLocation = form["whlocation"];

            int clientId = ClientService.NewClientId();

            ClientService.CreateClient(clientId, clientName, clientCode, whLocation);

            return RedirectToAction("Edit", new { id = clientId });
        }

        [HttpPost]
        public ActionResult SaveClientProfile(ClientProfile clientProfile, HttpPostedFileBase logoclient)
        {
            int clientId = ClientService.SaveClient(clientProfile);
            ClientService.UploadClientLogo(clientId, logoclient);

            return RedirectToAction("Edit", new { id = clientId });
        }

        // GET: Client/ClientLogoFile/id
        [AllowAnonymous]
        public ActionResult ClientLogoFile(int id)
        {
            int clientId = id;
            
            return File(ClientService.GetClientLogo(clientId), "gif");
        }

        #endregion Client

        #region Division

        public ActionResult ListDivisions(int id)
        {
            int clientId = id;

            var divisions = ClientService.GetDivisions(clientId);

            if (divisions.Count > 0)
            {
                return PartialView("~/Views/Client/_DivisionListing.cshtml", divisions);
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditDivision(int id)
        {
            int divisionId = id;

            var division = ClientService.GetDivision(divisionId);

            return PartialView("~/Views/Client/_DivisionEditModal.cshtml", division);
        }

        [HttpGet]
        public ActionResult CreateDivision(int id)
        {
            int clientId = id;

            var division = ClientService.EmptyDivision(clientId);

            return PartialView("~/Views/Client/_DivisionEditModal.cshtml", division);
        }

        public ActionResult SaveDivision(Division division, HttpPostedFileBase logodivision)
        {
            int? clientId = division.ClientID;

            int divisionId = ClientService.SaveDivision(division);

            ClientService.UploadDivisionLogo(divisionId, logodivision);

            return RedirectToAction("Edit", new { id = clientId });
        }

        // GET: Client/DivisionLogoFile/id
        [AllowAnonymous]
        public ActionResult DivisionLogoFile(int id)
        {
            int divisionId = id;

            return File(ClientService.GetDivisionLogo(divisionId), "gif");
        }

        #endregion Division

        #region Supplier

        public ActionResult ListSuppliers(int id)
        {
            int clientId = id;

            var suppliers = ClientService.GetSuppliers(clientId);

            if (suppliers.Count > 0)
            {
                return PartialView("~/Views/Client/_Suppliers.cshtml", suppliers);
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditSupplier(int id)
        {
            int bulkSupplierId = id;

            var supplier = ClientService.GetSupplier(bulkSupplierId);

            return PartialView("~/Views/Client/_SuppliersModal.cshtml", supplier);
        }

        [HttpGet]
        public ActionResult CreateSupplier(int id)
        {
            int clientId = id;

            var supplier = ClientService.EmptySupplier(clientId);

            return PartialView("~/Views/Client/_SuppliersModal.cshtml", supplier);
        }

        public ActionResult SaveSupplier(Supplier supplier)
        {
            ClientService.SaveSupplierRecord(supplier);

            return null;
        }

        #endregion Supplier

        #region Client Contact

        public ActionResult ListContacts(int id)
        {
            int clientId = id;
            var clientContacts = ClientService.GetClientContacts(clientId);

            if (clientContacts.Count > 0)
            {
                return PartialView("~/Views/Client/_Contacts.cshtml", clientContacts);
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditContact(int id)
        {
            int clientContactId = id;
            var clientContact = ClientService.GetClientContact(clientContactId);

            return PartialView("~/Views/Client/_ContactsModal.cshtml", clientContact);
        }

        [HttpGet]
        public ActionResult CreateContact(int id)
        {
            int clientContactId = id;
            var clientContact = ClientService.EmptyClientContact(clientContactId);

            return PartialView("~/Views/Client/_ContactsModal.cshtml", clientContact);
        }

        public ActionResult SaveContact(Contact clientContact)
        {
            ClientService.SaveClientContact(clientContact);

            return null;
        }

        #endregion Client Contact

        #region Tier

        public ActionResult ListTiers(int id)
        {
            int clientId = id;
            var tiers = ClientService.GetTiers(clientId);

            if (tiers.Count > 0)
            {
                return PartialView("~/Views/Client/_Tiers.cshtml", tiers);
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditTier(int id)
        {
            int tierId = id;
            var tier = ClientService.GetTier(tierId);

            return PartialView("~/Views/Client/_TiersModal.cshtml", tier);
        }

        [HttpGet]
        public ActionResult CreateTier(int id)
        {
            int clientId = id;
            var tier = ClientService.EmptyTier(clientId);

            return PartialView("~/Views/Client/_TiersModal.cshtml", tier);
        }

        public ActionResult SaveTier(Tier tier)
        {
            ClientService.SaveTier(tier);

            return null;
        }

        #endregion Tier

        #region End Use

        public ActionResult ListEndUses(int id)
        {
            int clientId = id;
            ViewBag.ClientKey = clientId;

            var endUses = ClientService.GetEndUses(clientId);

            if (endUses.Count > 0)
            {
                return PartialView("~/Views/Client/_EndUses.cshtml", endUses);
            }

            return null;
        }

        public ActionResult AddEndUse(int clientid, string endusestring)
        {
            int clientId = ClientService.AddEndUse(clientid, endusestring);

            return RedirectToAction("ListEndUses", new { id = clientId });
        }

        public ActionResult DeleteEndUse(int id, int clientid)
        {
            int endUseId = id;

            ClientService.DeleteEnduse(endUseId);

            return RedirectToAction("ListEndUses", new { id = clientid });
        }

        #endregion End Use

        #region Surcharge

        public ActionResult DisplaySurcharges(int? id)
        {
            int clientId = id ?? 0;
            ViewBag.ClientKey = clientId;

            var surcharges = ClientService.GetSurcharges(clientId);

            if (surcharges != null)
            {
                return PartialView("~/Views/Client/_Surcharges.cshtml", surcharges);
            }

            return PartialView("~/Views/Client/_Surcharges.cshtml");
        }

        public ActionResult SaveSurcharges(Surcharge surcharge, int clientkey)
        {
            int clientId = Convert.ToInt32(clientkey);
            string message = ClientService.SaveSurcharge(surcharge, clientId);

            ViewBag.Message = "<label class='text-success'>" + message + "</label>";
            return Content(ViewBag.Message);
        }

        #endregion Surcharge

        #region Service Charge Rates

        public ActionResult DisplayServiceChargeRates(int? id)
        {
            int clientId = id ?? 0;
            ViewBag.ClientKey = clientId;

            var serviceChargeRates = ClientService.GetServiceChargeRates(clientId);

            if (serviceChargeRates != null)
            {
                return PartialView("~/Views/Client/_ServiceChargeRates.cshtml", serviceChargeRates);
            }

            return PartialView("~/Views/Client/_ServiceChargeRates.cshtml");
        }

        public ActionResult SaveServiceChargeRates(ServiceChargeRates serviceChargeRates, int clientkey)
        {
            int clientId = clientkey;
            string message = ClientService.SaveChargeRates(serviceChargeRates, clientId);

            ViewBag.Message = "<label class='text-success'>" + message + "</label>";
            return Content(ViewBag.Message);
        }

        #endregion Service Charge Rates

        #region Account Representative

        public ActionResult ListAccountReps(int clientid)
        {
            ViewBag.ClientKey = clientid;

            var accountReps = ClientService.GetAccountReps(clientid);

            return PartialView("~/Views/Client/_AccountReps.cshtml", accountReps);
        }

        public ActionResult AddAccountRep(string accountrepemail, int clientid)
        {
            int clientId = ClientService.AddAccountRep(clientid, accountrepemail);

            return RedirectToAction("ListAccountReps", new { clientid = clientId });
        }

        public ActionResult DeleteAccountRep(string accountrepemail, int clientid)
        {
            ClientService.DeleteAccountRep(accountrepemail, clientid);

            return RedirectToAction("ListAccountReps", new { clientid = clientid });
        }

        #endregion Account Representative
    }
}