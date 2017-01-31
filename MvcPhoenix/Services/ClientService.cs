using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

// ************** ClientService.cs ********************
// This class contains Client Management service methods
// ******************************************************

namespace MvcPhoenix.Services
{
    public class ClientService
    {
        #region Client Information Services

        public static ClientProfile FillFromDB(ClientProfile CP)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblClient
                         where t.ClientID == CP.ClientID
                         select t).FirstOrDefault();

                CP.ClientID = q.ClientID;
                CP.LegacyID = q.LegacyID;
                CP.GlobalClientID = q.GlobalClientID;
                CP.ClientCode = q.ClientCode;
                CP.ClientName = q.ClientName;
                CP.CMCLocation = q.CMCLocation;
                CP.ClientReference = q.ClientReference;
                CP.ClientEntityName = q.ClientEntityName;
                CP.ClientCurrency = q.ClientCurrency;
                CP.ClientUM = q.ClientUM;
                CP.ClientNetTerm = String.IsNullOrEmpty(q.ClientNetTerm) ? "Net 30 Days" : q.ClientNetTerm;
                CP.InvoiceAddress = q.InvoiceAddress;
                CP.InvoiceEmailTo = q.InvoiceEmailTo;
                CP.KeyContactDir = q.KeyContactDir;
                CP.ActiveProfile = q.ActiveProfile;
                CP.ActiveDate = q.ActiveDate;
                CP.LogoFile = q.LogoFile;

                return CP;
            }
        }

        public static int fnSaveClientProfile(ClientProfile obj)
        {
            int clientid = Convert.ToInt32(obj.ClientID);

            if (clientid == -1)
            {
                obj.ClientID = NewClientId();
            }

            ClientService.SaveClient(obj);

            return obj.ClientID;
        }

        public static int NewClientId()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var newrow = new EF.tblClient { };

                db.tblClient.Add(newrow);
                db.SaveChanges();

                int clientkey = newrow.ClientID;

                return clientkey;
            }
        }

        public static void CreateClient(int clientid, string clientname, string clientcode, string whlocation)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = db.tblClient.Find(clientid);
                q.ClientName = clientname;
                q.ClientCode = clientcode;
                q.CMCLocation = whlocation;

                q.ClientReference = clientcode;
                q.ClientEntityName = clientname;

                switch (whlocation)
                {
                    case "AP":
                        q.ClientCurrency = "CNY";
                        q.ClientUM = "KG";
                        break;

                    case "EU":
                        q.ClientCurrency = "EUR";
                        q.ClientUM = "KG";
                        break;

                    default:
                        q.ClientCurrency = "USD";
                        q.ClientUM = "LB";
                        break;
                }

                db.SaveChanges();
            }
        }

        public static void SaveClient(ClientProfile CP)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = db.tblClient.Find(CP.ClientID);
                q.ClientID = CP.ClientID;
                q.LegacyID = CP.LegacyID;
                q.GlobalClientID = CP.GlobalClientID;
                q.ClientCode = CP.ClientCode;
                q.ClientName = CP.ClientName;
                q.CMCLocation = CP.CMCLocation;
                q.ClientCurrency = CP.ClientCurrency;
                q.ClientUM = CP.ClientUM;
                q.ClientNetTerm = CP.ClientNetTerm;
                q.InvoiceAddress = CP.InvoiceAddress;
                q.InvoiceEmailTo = CP.InvoiceEmailTo;
                q.KeyContactDir = CP.KeyContactDir;
                q.ActiveDate = CP.ActiveDate;
                q.ActiveProfile = CP.ActiveProfile;

                db.SaveChanges();
            }
        }

        #endregion Client Information Services

        #region Division Services

        public static Division FillDivisionDetails(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                Division vm = new Division();

                var qry = (from t in db.tblDivision
                           where t.DivisionID == id
                           select t).FirstOrDefault();

                vm.DivisionID = qry.DivisionID;
                vm.ClientID = qry.ClientID;
                vm.DivisionName = qry.DivisionName;
                vm.BusinessUnit = qry.BusinessUnit;
                vm.WasteRateOffSpec = qry.WasteRate_OffSpec;
                vm.WasteRateEmpty = qry.WasteRate_Empty;
                vm.Inactive = qry.Inactive;
                vm.ContactLabelName = qry.ContactLabelName;
                vm.ContactLabelPhone = qry.ContactLabelPhone;
                vm.ContactMSDSName = qry.ContactMSDSName;
                vm.ContactMSDSPhone = qry.ContactMSDSPhone;
                vm.EmergencyNumber = qry.EmergencyNumber;
                vm.MainContactName = qry.MainContactName;
                vm.MainContactNumber = qry.MainContactNumber;
                vm.Abbr = qry.Abbr;
                vm.UPSHazBook = qry.UPSHazBook;
                vm.ExtMSDS = qry.ExtMSDS;
                vm.ExtLabel = qry.ExtLabel;
                vm.CompanyName = qry.CompanyName;
                vm.CompanyStreet1 = qry.CompanyStreet1;
                vm.CompanyStreet2 = qry.CompanyStreet2;
                vm.CompanyStreet3 = qry.CompanyStreet3;
                vm.CompanyPostalCode = qry.CompanyPostalCode;
                vm.CompanyCity = qry.CompanyCity;
                vm.CompanyCountry = qry.CompanyCountry;
                vm.ListOfCountries = fnListOfCountries();
                vm.CompanyTelephone = qry.CompanyTelephone;
                vm.CompanyFax = qry.CompanyFax;
                vm.CompanyEmergencyTelephone = qry.CompanyEmergencyTelephone;
                vm.CompanyEmail = qry.CompanyEmail;
                vm.CompanyWebsite = qry.CompanyWebsite;
                vm.IncludeExpDtOnLabel = qry.IncludeExpDtOnLabel;

                return vm;
            }
        }

        #endregion Division Services

        #region Supplier Services

        public static Supplier FillSupplierDetails(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                Supplier vm = new Supplier();

                var qry = (from t in db.tblBulkSupplier
                           where t.BulkSupplierID == id
                           select t).FirstOrDefault();

                vm.BulkSupplierID = qry.BulkSupplierID;
                vm.ClientID = qry.ClientID;
                vm.SupplyID = qry.SupplyID;
                vm.SupplierCode = qry.ShortName;
                vm.SupplierName = qry.CompanyName;
                vm.ContactName = qry.ContactName;
                vm.Address1 = qry.Address1;
                vm.Address2 = qry.Address2;
                vm.Address3 = qry.Address3;
                vm.City = qry.City;
                vm.State = qry.State;
                vm.PostalCode = qry.Zip;
                vm.Country = qry.Country;
                vm.ListOfCountries = fnListOfCountries();
                vm.Email = qry.Email;
                vm.Phone = qry.Phone;
                vm.Fax = qry.Fax;

                return vm;
            }
        }

        #endregion Supplier Services

        #region Client Contact Services

        public static Contact FillContactDetails(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                Contact vm = new Contact();

                var qry = (from t in db.tblClientContact
                           where t.ClientContactID == id
                           select t).FirstOrDefault();

                vm.ClientContactID = qry.ClientContactID;
                vm.ClientID = qry.ClientID;
                vm.ContactType = qry.ContactType;
                vm.Account = qry.Account;
                vm.FullName = qry.FullName;
                vm.Email = qry.Email;
                vm.Phone = qry.Phone;
                vm.Fax = qry.Fax;
                vm.Company = qry.Company;
                vm.DistributorName = qry.DistributorName;
                vm.Address1 = qry.Address1;
                vm.Address2 = qry.Address2;
                vm.City = qry.City;
                vm.State = qry.State;
                vm.Zip = qry.Zip;
                vm.Country = qry.Country;
                vm.ListOfCountries = fnListOfCountries();

                return vm;
            }
        }

        #endregion Supplier Services

        #region Tier Services

        public static Tier FillTierDetails(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                Tier vm = new Tier();

                var qry = (from t in db.tblTier
                           where t.TierID == id
                           select t).FirstOrDefault();

                vm.TierID = qry.TierID;
                vm.ClientID = qry.ClientID;
                vm.TierLevel = qry.TierLevel;
                vm.Size = qry.Size;
                vm.LoSampQty = qry.LoSampAmt;
                vm.HiSampQty = qry.HiSampAmt;
                vm.Price = qry.Price;

                return vm;
            }
        }

        #endregion Tier Services

        public static List<SelectListItem> fnListOfCountries()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblCountry
                          orderby t.Country
                          select new SelectListItem { Value = t.Country, Text = t.Country }).ToList();

                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Please Select" });

                return mylist;
            }
        }
    }
}