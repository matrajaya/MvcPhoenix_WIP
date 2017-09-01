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

        public static ClientProfile FillClientProfile(ClientProfile clientProfile)
        {
            using (var db = new CMCSQL03Entities())
            {
                var client = (from t in db.tblClient
                              where t.ClientID == clientProfile.ClientID
                              select t).FirstOrDefault();

                clientProfile.ClientID = client.ClientID;
                clientProfile.LegacyID = client.LegacyID;
                clientProfile.GlobalClientID = client.GlobalClientID;
                clientProfile.ClientCode = client.ClientCode;
                clientProfile.ClientName = client.ClientName;
                clientProfile.CMCLocation = client.CMCLocation;
                clientProfile.ClientReference = client.ClientReference;
                clientProfile.ClientEntityName = client.ClientEntityName;
                clientProfile.ClientCurrency = client.ClientCurrency;
                clientProfile.ClientUM = client.ClientUM;
                clientProfile.ClientNetTerm = String.IsNullOrEmpty(client.ClientNetTerm) ? "Net 30 Days" : client.ClientNetTerm;
                clientProfile.InvoiceAddress = client.InvoiceAddress;
                clientProfile.InvoiceEmailTo = client.InvoiceEmailTo;
                clientProfile.KeyContactDir = client.KeyContactDir;
                clientProfile.ActiveProfile = client.ActiveProfile;
                clientProfile.ActiveDate = client.ActiveDate;
                clientProfile.LogoFile = client.LogoFile;

                return clientProfile;
            }
        }

        public static int SaveClientProfile(ClientProfile clientProfile)
        {
            int clientid = Convert.ToInt32(clientProfile.ClientID);

            if (clientid == -1)
            {
                clientProfile.ClientID = NewClientId();
            }

            ClientService.SaveClient(clientProfile);

            return clientProfile.ClientID;
        }

        public static int NewClientId()
        {
            using (var db = new CMCSQL03Entities())
            {
                var newClient = new tblClient { };

                db.tblClient.Add(newClient);
                db.SaveChanges();

                int clientId = newClient.ClientID;

                return clientId;
            }
        }

        public static void CreateClient(int clientId, string clientName, string clientCode, string whLocation)
        {
            using (var db = new CMCSQL03Entities())
            {
                var client = db.tblClient.Find(clientId);
                client.ClientName = clientName;
                client.ClientCode = clientCode;
                client.CMCLocation = whLocation;

                client.ClientReference = clientCode;
                client.ClientEntityName = clientName;

                switch (whLocation)
                {
                    case "AP":
                        client.ClientCurrency = "CNY";
                        client.ClientUM = "KG";
                        break;

                    case "EU":
                        client.ClientCurrency = "EUR";
                        client.ClientUM = "KG";
                        break;

                    default:
                        client.ClientCurrency = "USD";
                        client.ClientUM = "LB";
                        break;
                }

                db.SaveChanges();
            }
        }

        public static void SaveClient(ClientProfile clientProfile)
        {
            using (var db = new CMCSQL03Entities())
            {
                var client = db.tblClient.Find(clientProfile.ClientID);

                client.ClientID = clientProfile.ClientID;
                client.LegacyID = clientProfile.LegacyID;
                client.GlobalClientID = clientProfile.GlobalClientID;
                client.ClientCode = clientProfile.ClientCode;
                client.ClientName = clientProfile.ClientName;
                client.CMCLocation = clientProfile.CMCLocation;
                client.ClientCurrency = clientProfile.ClientCurrency;
                client.ClientUM = clientProfile.ClientUM;
                client.ClientNetTerm = clientProfile.ClientNetTerm;
                client.InvoiceAddress = clientProfile.InvoiceAddress;
                client.InvoiceEmailTo = clientProfile.InvoiceEmailTo;
                client.KeyContactDir = clientProfile.KeyContactDir;
                client.ActiveDate = clientProfile.ActiveDate;
                client.ActiveProfile = clientProfile.ActiveProfile;

                db.SaveChanges();
            }
        }

        #endregion Client Information Services

        #region Division Services

        public static Division FillDivisionDetails(int divisionId)
        {
            using (var db = new CMCSQL03Entities())
            {
                Division division = new Division();

                var result = (from t in db.tblDivision
                              where t.DivisionID == divisionId
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
                division.ERProvider = result.ERProvider;
                division.ERRegistrant = result.ERRegistrant;
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

        public static Supplier FillSupplierDetails(int bulkSupplierId)
        {
            using (var db = new CMCSQL03Entities())
            {
                Supplier supplier = new Supplier();

                var result = (from t in db.tblBulkSupplier
                              where t.BulkSupplierID == bulkSupplierId
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

        public static Contact FillContactDetails(int clientContactId)
        {
            using (var db = new CMCSQL03Entities())
            {
                Contact contact = new Contact();

                var result = (from t in db.tblClientContact
                              where t.ClientContactID == clientContactId
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

        public static Tier FillTierDetails(int tierId)
        {
            using (var db = new CMCSQL03Entities())
            {
                Tier tier = new Tier();

                var result = (from t in db.tblTier
                              where t.TierID == tierId
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