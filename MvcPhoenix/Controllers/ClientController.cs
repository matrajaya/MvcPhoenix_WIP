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

        // GET: Client/LogoFile/id
        public ActionResult LogoFile(int id)
        {
            var fileToRetrieve = db.tblClient.Find(id);
            return File(fileToRetrieve.LogoFile, "gif");
        }

        [HttpPost]
        public ActionResult SaveClientProfile(ClientProfile CPVM, HttpPostedFileBase logoupload)
        {
            int pk = ClientService.fnSaveClientProfile(CPVM);

            if (logoupload != null && logoupload.ContentLength > 0)
            {
                var qimg = db.tblClient.Find(CPVM.ClientID);

                using (var reader = new System.IO.BinaryReader(logoupload.InputStream))
                {
                    qimg.LogoFile = reader.ReadBytes(logoupload.ContentLength);
                }

                db.SaveChanges();
            }

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

                    q.Abbr = obj.Abbr;
                    q.UPSHazBook = obj.UPSHazBook;
                    q.ExtMSDS = obj.ExtMSDS;
                    q.ExtLabel = obj.ExtLabel;
                    q.CompanyName = obj.CompanyName;
                    q.CompanyStreet1 = obj.CompanyStreet1;
                    q.CompanyStreet2 = obj.CompanyStreet2;
                    q.CompanyStreet3 = obj.CompanyStreet3;
                    q.CompanyPostalCode = obj.CompanyPostalCode;
                    q.CompanyCity = obj.CompanyCity;
                    q.CompanyCountry = obj.CompanyCountry;
                    q.CompanyTelephone = obj.CompanyTelephone;
                    q.CompanyFax = obj.CompanyFax;
                    q.CompanyEmergencyTelephone = obj.CompanyEmergencyTelephone;
                    q.CompanyEmail = obj.CompanyEmail;
                    q.CompanyWebsite = obj.CompanyWebsite;
                    q.IncludeExpDtOnLabel = obj.IncludeExpDtOnLabel;

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

                        Abbr = obj.Abbr,
                        UPSHazBook = obj.UPSHazBook,
                        ExtMSDS = obj.ExtMSDS,
                        ExtLabel = obj.ExtLabel,
                        CompanyName = obj.CompanyName,
                        CompanyStreet1 = obj.CompanyStreet1,
                        CompanyStreet2 = obj.CompanyStreet2,
                        CompanyStreet3 = obj.CompanyStreet3,
                        CompanyPostalCode = obj.CompanyPostalCode,
                        CompanyCity = obj.CompanyCity,
                        CompanyCountry = obj.CompanyCountry,
                        CompanyTelephone = obj.CompanyTelephone,
                        CompanyFax = obj.CompanyFax,
                        CompanyEmergencyTelephone = obj.CompanyEmergencyTelephone,
                        CompanyEmail = obj.CompanyEmail,
                        CompanyWebsite = obj.CompanyWebsite,
                        IncludeExpDtOnLabel = obj.IncludeExpDtOnLabel
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
                       orderby t.TierLevel
                       select new MvcPhoenix.Models.Tier
                       {
                           TierID = t.TierID,
                           ClientID = t.ClientID,
                           TierLevel = t.TierLevel,
                           Size = t.Size,
                           LoSampQty = t.LoSampAmt,
                           HiSampQty = t.HiSampAmt,
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
                    q.TierLevel = obj.TierLevel;
                    q.Size = obj.Size;
                    q.LoSampAmt = obj.LoSampQty;
                    q.HiSampAmt = obj.HiSampQty;
                    q.Price = obj.Price;

                    db.SaveChanges();
                }
                else
                {
                    var newrecord = new EF.tblTier
                    {
                        TierID = Convert.ToInt32(obj.TierID),
                        ClientID = Convert.ToInt32(obj.ClientID),
                        TierLevel = obj.TierLevel,
                        Size = obj.Size,
                        LoSampAmt = obj.LoSampQty,
                        HiSampAmt = obj.HiSampQty,
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
                           Nitrogen = t.Nitrogen,
                           Biocide = t.Biocide,
                           Blend = t.Blend,
                           Kosher = t.Kosher
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
                    q.Nitrogen = obj.Nitrogen;
                    q.Biocide = obj.Biocide;
                    q.Blend = obj.Blend;
                    q.Kosher = obj.Kosher;

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
                        Nitrogen = obj.Nitrogen,
                        Biocide = obj.Biocide,
                        Blend = obj.Blend,
                        Kosher = obj.Kosher
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