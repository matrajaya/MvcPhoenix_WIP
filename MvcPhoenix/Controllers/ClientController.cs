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
        #region Client Information Methods

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
        [AllowAnonymous]
        public ActionResult LogoFile(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var fileToRetrieve = db.tblClient.Find(id);
                return File(fileToRetrieve.LogoFile, "gif");
            }
        }

        [HttpPost]
        public ActionResult SaveClientProfile(ClientProfile CPVM, HttpPostedFileBase logoupload)
        {
            using (var db = new CMCSQL03Entities())
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
        }

        #endregion Client Information Methods

        #region Division Methods

        public ActionResult ListDivisions(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var divisions = (from t in db.tblDivision
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

                if (divisions.Count > 0)
                {
                    return PartialView("~/Views/Client/_DivisionListing.cshtml", divisions);
                }

                return null;
            }
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

        public ActionResult SaveDivision(Division obj, HttpPostedFileBase logouploaddivision)
        {
            using (var db = new CMCSQL03Entities())
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
                    var newrecord = new tblDivision
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

                    obj.DivisionID = newrecord.DivisionID;      // needed for logo upload in new division
                }

                // Save logo file to division to be used as label logo
                if (logouploaddivision != null && logouploaddivision.ContentLength > 0)
                {
                    var qimg = db.tblDivision.Find(obj.DivisionID);

                    using (var reader = new System.IO.BinaryReader(logouploaddivision.InputStream))
                    {
                        qimg.LogoFile = reader.ReadBytes(logouploaddivision.ContentLength);
                    }

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Edit", new { id = obj.ClientID });
        }

        // GET: Client/DivisionLogoFile/id
        [AllowAnonymous]
        public ActionResult DivisionLogoFile(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var fileToRetrieve = db.tblDivision.Find(id);
                return File(fileToRetrieve.LogoFile, "gif");
            }
        }

        #endregion Division Methods

        #region Supplier Methods

        public ActionResult ListSuppliers(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var suppliers = (from t in db.tblBulkSupplier
                                 where t.ClientID == id
                                 orderby t.CompanyName
                                 select new MvcPhoenix.Models.Supplier
                                 {
                                     BulkSupplierID = t.BulkSupplierID,
                                     ClientID = t.ClientID,
                                     SupplyID = t.SupplyID,
                                     SupplierCode = t.ShortName,
                                     SupplierName = t.CompanyName,
                                     ContactName = t.ContactName,
                                     Address1 = t.Address1,
                                     Address2 = t.Address2,
                                     Address3 = t.Address3,
                                     City = t.City,
                                     State = t.State,
                                     PostalCode = t.Zip,
                                     Country = t.Country,
                                     Email = t.Email,
                                     Phone = t.Phone,
                                     Fax = t.Fax
                                 }).ToList();

                if (suppliers.Count > 0)
                {
                    return PartialView("~/Views/Client/_Suppliers.cshtml", suppliers);
                }

                return null;
            }
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
            vm.BulkSupplierID = -1;
            vm.ClientID = id;

            return vm;
        }

        public ActionResult SaveSupplier(Supplier obj)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = db.tblBulkSupplier.Find(obj.BulkSupplierID);

                // Check for duplicate SupplyID related to the Client
                var supplyidcheck = (from c in db.tblBulkSupplier
                                     where c.SupplyID == obj.SupplyID && c.ClientID == obj.ClientID
                                     select c).Count();
                try
                {
                    if (q.SupplyID != obj.SupplyID)
                    {
                        if (supplyidcheck > 0)
                        {
                            obj.SupplyID += " - Duplicate";
                        }
                    }
                }
                catch (Exception)
                {
                    // Catch NullReferenceException which is thrown when adding new supplier
                    if (supplyidcheck > 0)
                    {
                        obj.SupplyID += " - Duplicate";
                    }
                }

                if (q != null)
                {
                    q.SupplyID = obj.SupplyID;
                    q.ShortName = obj.SupplierCode;
                    q.CompanyName = obj.SupplierName;
                    q.ContactName = obj.ContactName;
                    q.Address1 = obj.Address1;
                    q.Address2 = obj.Address2;
                    q.Address3 = obj.Address3;
                    q.City = obj.City;
                    q.State = obj.State;
                    q.Zip = obj.PostalCode;
                    q.Country = obj.Country;
                    q.Email = obj.Email;
                    q.Phone = obj.Phone;
                    q.Fax = obj.Fax;

                    db.SaveChanges();
                }
                else
                {
                    var newrecord = new tblBulkSupplier
                    {
                        BulkSupplierID = Convert.ToInt32(obj.BulkSupplierID),
                        ClientID = Convert.ToInt32(obj.ClientID),
                        SupplyID = obj.SupplyID,
                        ShortName = obj.SupplierCode,
                        CompanyName = obj.SupplierName,
                        ContactName = obj.ContactName,
                        Address1 = obj.Address1,
                        Address2 = obj.Address2,
                        Address3 = obj.Address3,
                        City = obj.City,
                        State = obj.State,
                        Zip = obj.PostalCode,
                        Country = obj.Country,
                        Email = obj.Email,
                        Phone = obj.Phone,
                        Fax = obj.Fax
                    };

                    db.tblBulkSupplier.Add(newrecord);
                    db.SaveChanges();
                }

                return null;
            }
        }

        #endregion Supplier Methods

        #region Client Contact Methods

        public ActionResult ListContacts(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var contacts = (from t in db.tblClientContact
                                where t.ClientID == id
                                orderby t.ContactType, t.FullName
                                select new MvcPhoenix.Models.Contact
                                {
                                    ClientContactID = t.ClientContactID,
                                    ClientID = t.ClientID,
                                    ContactType = t.ContactType,
                                    Account = t.Account,
                                    FullName = t.FullName,
                                    Email = t.Email,
                                    Phone = t.Phone,
                                    Fax = t.Fax,
                                    Company = t.Company,
                                    DistributorName = t.DistributorName,
                                    Address1 = t.Address1,
                                    Address2 = t.Address2,
                                    City = t.City,
                                    State = t.State,
                                    Zip = t.Zip,
                                    Country = t.Country
                                }).ToList();

                if (contacts.Count > 0)
                {
                    return PartialView("~/Views/Client/_Contacts.cshtml", contacts);
                }

                return null;
            }
        }

        [HttpGet]
        public ActionResult EditContact(int id)
        {
            Contact vm = new Contact();
            vm = ClientService.FillContactDetails(id);
            return PartialView("~/Views/Client/_ContactsModal.cshtml", vm);
        }

        [HttpGet]
        public ActionResult CreateContact(int id)
        {
            // id = clientid
            var vm = EmptyContact(id);
            return PartialView("~/Views/Client/_ContactsModal.cshtml", vm);
        }

        public static Contact EmptyContact(int id)
        {
            Contact vm = new Contact();
            vm.ClientContactID = -1;
            vm.ClientID = id;

            return vm;
        }

        public ActionResult SaveContact(Contact obj)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = db.tblClientContact.Find(obj.ClientContactID);

                if (q != null)
                {
                    q.ContactType = obj.ContactType;
                    q.Account = obj.Account;
                    q.FullName = obj.FullName;
                    q.Email = obj.Email;
                    q.Phone = obj.Phone;
                    q.Fax = obj.Fax;
                    q.Company = obj.Company;
                    q.DistributorName = obj.DistributorName;
                    q.Address1 = obj.Address1;
                    q.Address2 = obj.Address2;
                    q.City = obj.City;
                    q.State = obj.State;
                    q.Zip = obj.Zip;
                    q.Country = obj.Country;

                    db.SaveChanges();
                }
                else
                {
                    var newrecord = new tblClientContact
                    {
                        ClientContactID = Convert.ToInt32(obj.ClientContactID),
                        ClientID = Convert.ToInt32(obj.ClientID),
                        ContactType = obj.ContactType,
                        Account = obj.Account,
                        FullName = obj.FullName,
                        Email = obj.Email,
                        Phone = obj.Phone,
                        Fax = obj.Fax,
                        Company = obj.Company,
                        DistributorName = obj.DistributorName,
                        Address1 = obj.Address1,
                        Address2 = obj.Address2,
                        City = obj.City,
                        State = obj.State,
                        Zip = obj.Zip,
                        Country = obj.Country
                    };

                    db.tblClientContact.Add(newrecord);
                    db.SaveChanges();
                }

                return null;
            }
        }

        #endregion Client Contact Methods

        #region Tier Methods

        public ActionResult ListTiers(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                var tiers = (from t in db.tblTier
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

                if (tiers.Count > 0)
                {
                    return PartialView("~/Views/Client/_Tiers.cshtml", tiers);
                }

                return null;
            }
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
            using (var db = new CMCSQL03Entities())
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
                    var newrecord = new tblTier
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

        #region End Use Methods

        public ActionResult ListEndUses(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                // id = clientid;
                ViewBag.ClientKey = id;
                var obj = (from t in db.tblEndUse
                           where t.ClientID == id
                           orderby t.EndUse
                           select new MvcPhoenix.Models.EndUse
                           {
                               EndUseID = t.EndUseID,
                               ClientID = t.ClientID,
                               EndUseString = t.EndUse
                           }).ToList();

                if (obj.Count > 0)
                {
                    return PartialView("~/Views/Client/_EndUses.cshtml", obj);
                }

                return null;
            }
        }

        public ActionResult AddEndUse(int clientid, string endusestring)
        {
            using (var db = new CMCSQL03Entities())
            {
                var newrow = new tblEndUse { };
                newrow.ClientID = clientid;
                newrow.EndUse = endusestring;

                db.tblEndUse.Add(newrow);
                db.SaveChanges();
            }

            return RedirectToAction("ListEndUses", new { id = clientid });
        }

        public ActionResult DeleteEndUse(int id, int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                string s = @"DELETE FROM tblEndUse WHERE EndUseID=" + id.ToString();
                db.Database.ExecuteSqlCommand(s);
            }

            return RedirectToAction("ListEndUses", new { id = clientid });
        }

        #endregion End Use Methods

        #region Surcharge methods

        public ActionResult DisplaySurcharges(int? id)
        {
            using (var db = new CMCSQL03Entities())
            {
                ViewBag.ClientKey = id;

                var surcharges = (from t in db.tblSurcharge
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

                if (surcharges != null)
                {
                    return PartialView("~/Views/Client/_Surcharges.cshtml", surcharges);
                }

                return PartialView("~/Views/Client/_Surcharges.cshtml");
            }
        }

        public ActionResult SaveSurcharges(Surcharge obj, int clientkey)
        {
            using (var db = new CMCSQL03Entities())
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
                        var newrecord = new tblSurcharge
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
        }

        #endregion Surcharge methods

        #region Administrative Rates methods

        public ActionResult DisplayServiceChargeRates(int? id)
        {
            using (var db = new CMCSQL03Entities())
            {
                ViewBag.ClientKey = id;

                var servicechargerates = (from t in db.tblRates
                                           where t.ClientID == id
                                           select new MvcPhoenix.Models.ServiceChargeRates
                                           {
                                               RatesID = t.RatesID,
                                               ClientID = t.ClientID,
                                               AirHzdOnlyRate = t.AirHzdOnlyRate,
                                               CertificateOfOriginRate = t.CertificateOfOriginRate,
                                               CMCPackRate = t.CMCPackRate,
                                               CoolPackRate = t.CoolPackRate,
                                               CreditCardFeeRate = t.CreditCardFeeRate,
                                               CreditCardOrderRate = t.CreditCardOrderRate,
                                               DocumentationHandlingRate = t.DocumentationHandlingRate,
                                               EmptyPackagingRate = t.EmptyPackagingRate,
                                               ExternalSystemRate = t.ExternalSystemRate,
                                               FollowUpOrderRate = t.FollowUpOrderRate,
                                               FreezerPackRate = t.FreezerPackRate,
                                               GHSLabelsRate = t.GHSLabelsRate,
                                               InactiveProductsRate = t.InactiveProductsRate,
                                               IsolationRate = t.IsolationRate,
                                               IsolationBoxRate = t.IsolationBoxRate,
                                               ITFeeRate = t.ITFeeRate,
                                               LabelMaintainanceRate = t.LabelMaintainanceRate,
                                               LabelStockRate = t.LabelStockRate,
                                               LabelsPrintedRate = t.LabelsPrintedRate,
                                               LaborRelabelRate = t.LaborRelabelRate,
                                               LiteratureFeeRate = t.LiteratureFeeRate,
                                               LimitedQtyRate = t.LimitedQtyRate,
                                               ManualHandlingRate = t.ManualHandlingRate,
                                               MSDSPrintsRate = t.MSDSPrintsRate,
                                               NewLabelSetupRate = t.NewLabelSetupRate,
                                               NewProductSetupRate = t.NewProductSetupRate,
                                               OberkPackRate = t.OberkPackRate,
                                               OrderEntryRate = t.OrderEntryRate,
                                               OverPackRate = t.OverPackRate,
                                               PalletReturnRate = t.PalletReturnRate,
                                               PoisonPackRate = t.PoisonPackRate,
                                               ProductSetupChangesRate = t.ProductSetupChangesRate,
                                               QCStorageRate = t.QCStorageRate,
                                               RDHandlingADRRate = t.RDHandlingADRRate,
                                               RDHandlingIATARate = t.RDHandlingIATARate,
                                               RDHandlingLQRate = t.RDHandlingLQRate,
                                               RDHandlingNonHzdRate = t.RDHandlingNonHzdRate,
                                               RefrigeratorStorageRate = t.RefrigeratorStorageRate,
                                               RelabelsRate = t.RelabelsRate,
                                               RushShipmentRate = t.RushShipmentRate,
                                               SPA197AppliedRate = t.SPA197AppliedRate,
                                               SPSPaidOrderRate = t.SPSPaidOrderRate,
                                               UNBoxRate = t.UNBoxRate,
                                               WarehouseStorageRate = t.WarehouseStorageRate,
                                               WHMISLabelsRate = t.WHMISLabelsRate,
                                           }).FirstOrDefault();

                if (servicechargerates != null)
                {
                    return PartialView("~/Views/Client/_ServiceChargeRates.cshtml", servicechargerates);
                }

                return PartialView("~/Views/Client/_ServiceChargeRates.cshtml");
            }
        }

        public ActionResult SaveServiceChargeRates(ServiceChargeRates obj, int clientkey)
        {
            using (var db = new CMCSQL03Entities())
            {
                var q = db.tblRates.Find(obj.RatesID);
                var msg = "Save Successful!";

                try
                {
                    if (q != null)
                    {
                        q.RatesID = obj.RatesID;
                        q.AirHzdOnlyRate = obj.AirHzdOnlyRate;
                        q.CertificateOfOriginRate = obj.CertificateOfOriginRate;
                        q.CMCPackRate = obj.CMCPackRate;
                        q.CoolPackRate = obj.CoolPackRate;
                        q.CreditCardFeeRate = obj.CreditCardFeeRate;
                        q.CreditCardOrderRate = obj.CreditCardOrderRate;
                        q.DocumentationHandlingRate = obj.DocumentationHandlingRate;
                        q.EmptyPackagingRate = obj.EmptyPackagingRate;
                        q.ExternalSystemRate = obj.ExternalSystemRate;
                        q.FollowUpOrderRate = obj.FollowUpOrderRate;
                        q.FreezerPackRate = obj.FreezerPackRate;
                        q.GHSLabelsRate = obj.GHSLabelsRate;
                        q.InactiveProductsRate = obj.InactiveProductsRate;
                        q.IsolationRate = obj.IsolationRate;
                        q.IsolationBoxRate = obj.IsolationBoxRate;
                        q.ITFeeRate = obj.ITFeeRate;
                        q.LabelMaintainanceRate = obj.LabelMaintainanceRate;
                        q.LabelStockRate = obj.LabelStockRate;
                        q.LabelsPrintedRate = obj.LabelsPrintedRate;
                        q.LaborRelabelRate = obj.LaborRelabelRate;
                        q.LiteratureFeeRate = obj.LiteratureFeeRate;
                        q.LimitedQtyRate = obj.LimitedQtyRate;
                        q.ManualHandlingRate = obj.ManualHandlingRate;
                        q.MSDSPrintsRate = obj.MSDSPrintsRate;
                        q.NewLabelSetupRate = obj.NewLabelSetupRate;
                        q.NewProductSetupRate = obj.NewProductSetupRate;
                        q.OberkPackRate = obj.OberkPackRate;
                        q.OrderEntryRate = obj.OrderEntryRate;
                        q.OverPackRate = obj.OverPackRate;
                        q.PalletReturnRate = obj.PalletReturnRate;
                        q.PoisonPackRate = obj.PoisonPackRate;
                        q.ProductSetupChangesRate = obj.ProductSetupChangesRate;
                        q.QCStorageRate = obj.QCStorageRate;
                        q.RDHandlingADRRate = obj.RDHandlingADRRate;
                        q.RDHandlingIATARate = obj.RDHandlingIATARate;
                        q.RDHandlingLQRate = obj.RDHandlingLQRate;
                        q.RDHandlingNonHzdRate = obj.RDHandlingNonHzdRate;
                        q.RefrigeratorStorageRate = obj.RefrigeratorStorageRate;
                        q.RelabelsRate = obj.RelabelsRate;
                        q.RushShipmentRate = obj.RushShipmentRate;
                        q.SPA197AppliedRate = obj.SPA197AppliedRate;
                        q.SPSPaidOrderRate = obj.SPSPaidOrderRate;
                        q.UNBoxRate = obj.UNBoxRate;
                        q.WarehouseStorageRate = obj.WarehouseStorageRate;
                        q.WHMISLabelsRate = obj.WHMISLabelsRate;

                        db.SaveChanges();
                    }
                    else
                    {
                        var newrecord = new tblRates
                        {
                            ClientID = Convert.ToInt32(clientkey),
                            AirHzdOnlyRate = obj.AirHzdOnlyRate,
                            CertificateOfOriginRate = obj.CertificateOfOriginRate,
                            CMCPackRate = obj.CMCPackRate,
                            CoolPackRate = obj.CoolPackRate,
                            CreditCardFeeRate = obj.CreditCardFeeRate,
                            CreditCardOrderRate = obj.CreditCardOrderRate,
                            DocumentationHandlingRate = obj.DocumentationHandlingRate,
                            EmptyPackagingRate = obj.EmptyPackagingRate,
                            ExternalSystemRate = obj.ExternalSystemRate,
                            FollowUpOrderRate = obj.FollowUpOrderRate,
                            FreezerPackRate = obj.FreezerPackRate,
                            GHSLabelsRate = obj.GHSLabelsRate,
                            InactiveProductsRate = obj.InactiveProductsRate,
                            IsolationRate = obj.IsolationRate,
                            IsolationBoxRate = obj.IsolationBoxRate,
                            ITFeeRate = obj.ITFeeRate,
                            LabelMaintainanceRate = obj.LabelMaintainanceRate,
                            LabelStockRate = obj.LabelStockRate,
                            LabelsPrintedRate = obj.LabelsPrintedRate,
                            LaborRelabelRate = obj.LaborRelabelRate,
                            LiteratureFeeRate = obj.LiteratureFeeRate,
                            LimitedQtyRate = obj.LimitedQtyRate,
                            ManualHandlingRate = obj.ManualHandlingRate,
                            MSDSPrintsRate = obj.MSDSPrintsRate,
                            NewLabelSetupRate = obj.NewLabelSetupRate,
                            NewProductSetupRate = obj.NewProductSetupRate,
                            OberkPackRate = obj.OberkPackRate,
                            OrderEntryRate = obj.OrderEntryRate,
                            OverPackRate = obj.OverPackRate,
                            PalletReturnRate = obj.PalletReturnRate,
                            PoisonPackRate = obj.PoisonPackRate,
                            ProductSetupChangesRate = obj.ProductSetupChangesRate,
                            QCStorageRate = obj.QCStorageRate,
                            RDHandlingADRRate = obj.RDHandlingADRRate,
                            RDHandlingIATARate = obj.RDHandlingIATARate,
                            RDHandlingLQRate = obj.RDHandlingLQRate,
                            RDHandlingNonHzdRate = obj.RDHandlingNonHzdRate,
                            RefrigeratorStorageRate = obj.RefrigeratorStorageRate,
                            RelabelsRate = obj.RelabelsRate,
                            RushShipmentRate = obj.RushShipmentRate,
                            SPA197AppliedRate = obj.SPA197AppliedRate,
                            SPSPaidOrderRate = obj.SPSPaidOrderRate,
                            UNBoxRate = obj.UNBoxRate,
                            WarehouseStorageRate = obj.WarehouseStorageRate,
                            WHMISLabelsRate = obj.WHMISLabelsRate,
                        };

                        db.tblRates.Add(newrecord);
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
        }

        #endregion Administrative Rates methods
    }
}