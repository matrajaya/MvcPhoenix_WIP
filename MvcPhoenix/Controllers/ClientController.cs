using MvcPhoenix.Models;
using MvcPhoenix.Services;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcPhoenix.Controllers
{
    public class ClientController : Controller
    {
        private MvcPhoenix.EF.CMCSQL03Entities db = new MvcPhoenix.EF.CMCSQL03Entities();

        // GET: Client/Index
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
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

        // POST: Client/Create
        [HttpPost]
        public ActionResult Create(FormCollection fc)
        {
            int ClientID = ClientService.NewClientId();
            string ClientName = fc["clientname"];
            string ClientCode = fc["clientcode"].ToUpper();
            string WHLocation = fc["whlocation"];

            ClientService.CreateClient(ClientID, ClientName, ClientCode, WHLocation);

            return RedirectToAction("Edit", new { id = ClientID });
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

        [HttpPost]
        public ActionResult SaveClientProfile(ClientProfile CPVM)
        {
            int pk = ClientService.fnSaveClientProfile(CPVM);

            return RedirectToAction("Edit", new { id = pk });
        }

        #region Division Methods

        public ActionResult ListDivisions(int id)
        {
            var qry = (from t in db.tblDivision
                       where t.ClientID == id
                       orderby t.DivisionName
                       select new MvcPhoenix.Models.Division
                       {
                           DivisionID = t.DivisionID,
                           ClientID = t.ClientID,
                           DivisionName = t.DivisionName,
                           BusinessUnit = t.BusinessUnit,
                           Abbr = t.Abbr,
                           WasteRateOffSpec = t.WasteRate_OffSpec,
                           WasteRateEmpty = t.WasteRate_Empty,
                           Inactive = t.Inactive,
                           LegacyID = t.LegacyID,
                           Location_MDB = t.Location_MDB,
                           Company_MDB = t.Company_MDB,
                           ContactLabelName = t.ContactLabelName,
                           ContactLabelPhone = t.ContactLabelPhone,
                           ContactMSDSName = t.ContactMSDSName,
                           ContactMSDSPhone = t.ContactMSDSPhone,
                           EmergencyNumber = t.EmergencyNumber,
                           UPSHazBook = t.UPSHazBook,
                           ExtMSDS = t.ExtMSDS,
                           ExtLabel = t.ExtLabel,
                           MainContactName = t.MainContactName,
                           MainContactNumber = t.MainContactNumber,
                           CompanyName = t.CompanyName,
                           CompanyStreet1 = t.CompanyStreet1,
                           CompanyStreet2 = t.CompanyStreet2,
                           CompanyStreet3 = t.CompanyStreet3,
                           CompanyPostalCode = t.CompanyPostalCode,
                           CompanyCity = t.CompanyCity,
                           CompanyCountry = t.CompanyCountry,
                           CompanyTelephone = t.CompanyTelephone,
                           CompanyFax = t.CompanyFax,
                           CompanyEmergencyTelephone = t.CompanyEmergencyTelephone,
                           CompanyEmail = t.CompanyEmail,
                           CompanyWebsite = t.CompanyWebsite,
                           CompanyLogo = t.CompanyLogo,
                           CompanyLogo2 = t.CompanyLogo2,
                           IncludeExpDtOnLabel = t.IncludeExpDtOnLabel
                       }).ToList();

            if (qry.Count > 0)
            {
                return PartialView("~/Views/Client/_DivisionListing.cshtml", qry);
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditDivision(int id)
        {
            Division vm = new Division();
            vm = ClientService.FillDivisionDetails(id);
            return PartialView("~/Views/Client/_DivisionEditModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult CreateDivision(int id)
        {
            // id = clientid
            var vm = EmptyDivision(id);
            return PartialView("~/Views/Client/_DivisionEditModal.cshtml", vm);
        }

        public static Division EmptyDivision(int id)
        {
            Division vm = new Division();
            vm.DivisionID = -1;
            vm.ClientID = id;

            return vm;
        }

        public ActionResult SaveDivision(Division obj)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = db.tblDivision.Find(obj.DivisionID);

                if (q != null)
                {
                    q.DivisionName = obj.DivisionName;
                    q.BusinessUnit = obj.BusinessUnit;
                    q.EmergencyNumber = obj.EmergencyNumber;
                    q.Inactive = obj.Inactive;
                    q.MainContactName = obj.MainContactName;
                    q.MainContactNumber = obj.MainContactNumber;
                    q.ContactLabelName = obj.ContactLabelName;
                    q.ContactLabelPhone = obj.ContactLabelPhone;
                    q.ContactMSDSName = obj.ContactMSDSName;
                    q.ContactMSDSPhone = obj.ContactMSDSPhone;
                    q.WasteRate_OffSpec = obj.WasteRateOffSpec;
                    q.WasteRate_Empty = obj.WasteRateEmpty;

                    db.SaveChanges();
                }
                else
                {
                    var newrecord = new EF.tblDivision
                    {
                        DivisionID = Convert.ToInt32(obj.DivisionID),
                        ClientID = Convert.ToInt32(obj.ClientID),
                        DivisionName = obj.DivisionName,
                        BusinessUnit = obj.BusinessUnit,
                        EmergencyNumber = obj.EmergencyNumber,
                        Inactive = obj.Inactive,
                        MainContactName = obj.MainContactName,
                        MainContactNumber = obj.MainContactNumber,
                        ContactLabelName = obj.ContactLabelName,
                        ContactLabelPhone = obj.ContactLabelPhone,
                        ContactMSDSName = obj.ContactMSDSName,
                        ContactMSDSPhone = obj.ContactMSDSPhone,
                        WasteRate_OffSpec = obj.WasteRateOffSpec,
                        WasteRate_Empty = obj.WasteRateEmpty,
                    };

                    db.tblDivision.Add(newrecord);
                    db.SaveChanges();
                }
            }

            return null;
        }

        #endregion Division Methods

        #region Supplier Methods

        public ActionResult ListSuppliers(int id)
        {
            var qry = (from t in db.tblSupplier
                       where t.ClientID == id
                       orderby t.SupplierName
                       select new MvcPhoenix.Models.Supplier
                       {
                           SupplierID = t.SupplierID,
                           ClientID = t.ClientID,
                           SupplierCode = t.SupplierCode,
                           SupplierName = t.SupplierName,
                           ContactName = t.ContactName,
                           Address1 = t.Address1,
                           Address2 = t.Address2,
                           City = t.City,
                           State = t.State,
                           PostalCode = t.PostalCode,
                           Country = t.Country,
                           Email = t.Email,
                           Phone = t.Phone,
                           Fax = t.Fax
                       }).ToList();

            if (qry.Count > 0)
            {
                return PartialView("~/Views/Client/_Suppliers.cshtml", qry);
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditSupplier(int id)
        {
            Supplier vm = new Supplier();
            vm = ClientService.FillSupplierDetails(id);
            return PartialView("~/Views/Client/_SuppliersModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult CreateSupplier(int id)
        {
            // id = clientid
            var vm = EmptySupplier(id);
            return PartialView("~/Views/Client/_SuppliersModal.cshtml", vm);
        }

        public static Supplier EmptySupplier(int id)
        {
            Supplier vm = new Supplier();
            vm.SupplierID = -1;
            vm.ListOfCountries = ClientService.fnListOfCountries();
            vm.ClientID = id;

            return vm;
        }

        public ActionResult SaveSupplier(Supplier obj)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = db.tblSupplier.Find(obj.SupplierID);

                if (q != null)
                {
                    q.SupplierCode = obj.SupplierCode;
                    q.SupplierName = obj.SupplierName;
                    q.ContactName = obj.ContactName;
                    q.Address1 = obj.Address1;
                    q.Address2 = obj.Address2;
                    q.City = obj.City;
                    q.State = obj.State;
                    q.PostalCode = obj.PostalCode;
                    q.Country = obj.Country;
                    q.Email = obj.Email;
                    q.Phone = obj.Phone;
                    q.Fax = obj.Fax;

                    db.SaveChanges();
                }
                else
                {
                    var newrecord = new EF.tblSupplier
                    {
                        SupplierID = Convert.ToInt32(obj.SupplierID),
                        ClientID = Convert.ToInt32(obj.ClientID),
                        SupplierCode = obj.SupplierCode,
                        SupplierName = obj.SupplierName,
                        ContactName = obj.ContactName,
                        Address1 = obj.Address1,
                        Address2 = obj.Address2,
                        City = obj.City,
                        State = obj.State,
                        PostalCode = obj.PostalCode,
                        Country = obj.Country,
                        Email = obj.Email,
                        Phone = obj.Phone,
                        Fax = obj.Fax
                    };

                    db.tblSupplier.Add(newrecord);
                    db.SaveChanges();
                }
            }

            return null;
        }

        #endregion Supplier Methods

        #region Tier Methods

        public ActionResult ListTiers(int id)
        {
            var qry = (from t in db.tblTier
                       where t.ClientID == id
                       orderby t.Tier
                       select new MvcPhoenix.Models.Tier
                       {
                           TierID = t.TierID,
                           ClientID = t.ClientID,
                           TierLevel = t.Tier,
                           Size = t.Size,
                           LoSampAmt = t.LoSampAmt,
                           HiSampAmt = t.HiSampAmt,
                           Price = t.Price
                       }).ToList();

            if (qry.Count > 0)
            {
                return PartialView("~/Views/Client/_Tiers.cshtml", qry);
            }

            return null;
        }

        [HttpGet]
        public ActionResult EditTier(int id)
        {
            Tier vm = new Tier();
            vm = ClientService.FillTierDetails(id);
            return PartialView("~/Views/Client/_TiersModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult CreateTier(int id)
        {
            // id = clientid
            var vm = EmptyTier(id);
            return PartialView("~/Views/Client/_TiersModal.cshtml", vm);
        }

        public static Tier EmptyTier(int id)
        {
            Tier vm = new Tier();
            vm.TierID = -1;
            vm.ClientID = id;

            return vm;
        }

        public ActionResult SaveTier(Tier obj)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = db.tblTier.Find(obj.TierID);

                if (q != null)
                {
                    q.Tier = obj.TierLevel;
                    q.Size = obj.Size;
                    q.LoSampAmt = obj.LoSampAmt;
                    q.HiSampAmt = obj.HiSampAmt;
                    q.Price = obj.Price;

                    db.SaveChanges();
                }
                else
                {
                    var newrecord = new EF.tblTier
                    {
                        TierID = Convert.ToInt32(obj.TierID),
                        ClientID = Convert.ToInt32(obj.ClientID),
                        Tier = obj.TierLevel,
                        Size = obj.Size,
                        LoSampAmt = obj.LoSampAmt,
                        HiSampAmt = obj.HiSampAmt,
                        Price = obj.Price
                    };

                    db.tblTier.Add(newrecord);
                    db.SaveChanges();
                }
            }

            return null;
        }

        #endregion Tier Methods

        #region Surcharge methods

        public ActionResult DisplaySurcharges(int? id)
        {
            ViewBag.ClientKey = id;

            var qry = (from t in db.tblSurcharge
                       where t.ClientID == id
                       orderby t.SurchargeID
                       select new MvcPhoenix.Models.Surcharge
                       {
                           SurchargeID = t.SurchargeID,
                           ClientID = id,
                           Haz = t.Haz,
                           Flam = t.Flam,
                           Clean = t.Clean,
                           Heat = t.Heat,
                           Refrig = t.Refrig,
                           Freezer = t.Freezer,
                           Nalgene = t.Nalgene,
                           LabelFee = t.LabelFee
                       }).FirstOrDefault();

            if (qry != null)
            {
                return PartialView("~/Views/Client/_Surcharges.cshtml", qry);
            }

            return PartialView("~/Views/Client/_Surcharges.cshtml");
        }

        public ActionResult SaveSurcharges(Surcharge obj, int clientkey)
        {
            var q = db.tblSurcharge.Find(obj.SurchargeID);
            var msg = "Save Successful!";

            try
            {

                if (q != null)
                {
                    q.Haz = obj.Haz;
                    q.Flam = obj.Flam;
                    q.Clean = obj.Clean;
                    q.Heat = obj.Heat;
                    q.Refrig = obj.Refrig;
                    q.Freezer = obj.Freezer;
                    q.Nalgene = obj.Nalgene;
                    q.LabelFee = obj.LabelFee;

                    db.SaveChanges();
                }
                else
                {
                    var newrecord = new EF.tblSurcharge
                    {
                        ClientID = Convert.ToInt32(clientkey),
                        Haz = obj.Haz,
                        Flam = obj.Flam,
                        Clean = obj.Clean,
                        Heat = obj.Heat,
                        Refrig = obj.Refrig,
                        Freezer = obj.Freezer,
                        Nalgene = obj.Nalgene,
                        LabelFee = obj.LabelFee
                    };

                    db.tblSurcharge.Add(newrecord);
                    db.SaveChanges();

                    msg = "New record added";
                }
            }
            catch (Exception)
            {
                // catch if null or beyond reasonable bounds. Unforseen errors.
                msg = "Something is wrong with your input..."; 
            }

            ViewBag.Message = "<label class='text-success'>" + msg + "</label>";
            return Content(ViewBag.Message);
        }

        #endregion Surcharge methods
    }
}