using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Linq;

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
            using (var db = new CMCSQL03Entities())
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
            using (var db = new CMCSQL03Entities())
            {
                var newrow = new tblClient { };

                db.tblClient.Add(newrow);
                db.SaveChanges();

                int clientkey = newrow.ClientID;

                return clientkey;
            }
        }

        public static void CreateClient(int clientid, string clientname, string clientcode, string whlocation)
        {
            using (var db = new CMCSQL03Entities())
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
            using (var db = new CMCSQL03Entities())
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
            using (var db = new CMCSQL03Entities())
            {
                Division division = new Division();

                var result = (from t in db.tblDivision
                              where t.DivisionID == id
                              select t).FirstOrDefault();

                division.DivisionID = result.DivisionID;
                division.ClientID = result.ClientID;
                division.DivisionName = result.DivisionName;
                division.BusinessUnit = result.BusinessUnit;
                division.WasteRateOffSpec = result.WasteRate_OffSpec;
                division.WasteRateEmpty = result.WasteRate_Empty;
                division.Inactive = result.Inactive;
                division.ContactLabelName = result.ContactLabelName;
                division.ContactLabelPhone = result.ContactLabelPhone;
                division.ContactMSDSName = result.ContactMSDSName;
                division.ContactMSDSPhone = result.ContactMSDSPhone;
                division.EmergencyNumber = result.EmergencyNumber;
                division.MainContactName = result.MainContactName;
                division.MainContactNumber = result.MainContactNumber;
                division.Abbr = result.Abbr;
                division.UPSHazBook = result.UPSHazBook;
                division.ExtMSDS = result.ExtMSDS;
                division.ExtLabel = result.ExtLabel;
                division.CompanyName = result.CompanyName;
                division.CompanyStreet1 = result.CompanyStreet1;
                division.CompanyStreet2 = result.CompanyStreet2;
                division.CompanyStreet3 = result.CompanyStreet3;
                division.CompanyPostalCode = result.CompanyPostalCode;
                division.CompanyCity = result.CompanyCity;
                division.CompanyCountry = result.CompanyCountry;
                division.CompanyTelephone = result.CompanyTelephone;
                division.CompanyFax = result.CompanyFax;
                division.CompanyEmergencyTelephone = result.CompanyEmergencyTelephone;
                division.CompanyEmail = result.CompanyEmail;
                division.CompanyWebsite = result.CompanyWebsite;
                division.IncludeExpDtOnLabel = result.IncludeExpDtOnLabel;

                return division;
            }
        }

        #endregion Division Services

        #region Supplier Services

        public static Supplier FillSupplierDetails(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                Supplier supplier = new Supplier();

                var result = (from t in db.tblBulkSupplier
                              where t.BulkSupplierID == id
                              select t).FirstOrDefault();

                supplier.BulkSupplierID = result.BulkSupplierID;
                supplier.ClientID = result.ClientID;
                supplier.SupplyID = result.SupplyID;
                supplier.SupplierCode = result.ShortName;
                supplier.SupplierName = result.CompanyName;
                supplier.ContactName = result.ContactName;
                supplier.Address1 = result.Address1;
                supplier.Address2 = result.Address2;
                supplier.Address3 = result.Address3;
                supplier.City = result.City;
                supplier.State = result.State;
                supplier.PostalCode = result.Zip;
                supplier.Country = result.Country;
                supplier.Email = result.Email;
                supplier.Phone = result.Phone;
                supplier.Fax = result.Fax;

                return supplier;
            }
        }

        #endregion Supplier Services

        #region Client Contact Services

        public static Contact FillContactDetails(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                Contact contact = new Contact();

                var result = (from t in db.tblClientContact
                              where t.ClientContactID == id
                              select t).FirstOrDefault();

                contact.ClientContactID = result.ClientContactID;
                contact.ClientID = result.ClientID;
                contact.ContactType = result.ContactType;
                contact.Account = result.Account;
                contact.FullName = result.FullName;
                contact.Email = result.Email;
                contact.Phone = result.Phone;
                contact.Fax = result.Fax;
                contact.Company = result.Company;
                contact.DistributorName = result.DistributorName;
                contact.Address1 = result.Address1;
                contact.Address2 = result.Address2;
                contact.City = result.City;
                contact.State = result.State;
                contact.Zip = result.Zip;
                contact.Country = result.Country;

                return contact;
            }
        }

        #endregion Client Contact Services

        #region Tier Services

        public static Tier FillTierDetails(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                Tier tier = new Tier();

                var result = (from t in db.tblTier
                              where t.TierID == id
                              select t).FirstOrDefault();

                tier.TierID = result.TierID;
                tier.ClientID = result.ClientID;
                tier.TierLevel = result.TierLevel;
                tier.Size = result.Size;
                tier.LoSampQty = result.LoSampAmt;
                tier.HiSampQty = result.HiSampAmt;
                tier.Price = result.Price;

                return tier;
            }
        }

        #endregion Tier Services
    }
}