using MvcPhoenix.DataLayer;
using MvcPhoenix.EF;
using MvcPhoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Services
{
    public class ClientService
    {
        #region Client

        public static ClientProfile GetClient(int clientid)
        {
            var client = new ClientProfile();

            using (var db = new CMCSQL03Entities())
            {
                var getClient = db.tblClient.Find(clientid);

                client.ClientID = getClient.ClientID;
                client.LegacyID = getClient.LegacyID;
                client.GlobalClientID = getClient.GlobalClientID;
                client.ClientCode = getClient.ClientCode;
                client.ClientName = getClient.ClientName;
                client.CMCLocation = getClient.CMCLocation;
                client.ClientReference = getClient.ClientReference;
                client.ClientEntityName = getClient.ClientEntityName;
                client.ClientCurrency = getClient.ClientCurrency;
                client.ClientUM = getClient.ClientUM;
                client.ClientNetTerm = String.IsNullOrEmpty(getClient.ClientNetTerm) ? "Net 30 Days" : getClient.ClientNetTerm;
                client.InvoiceAddress = getClient.InvoiceAddress;
                client.InvoiceEmailTo = getClient.InvoiceEmailTo;
                client.KeyContactDir = getClient.KeyContactDir;
                client.ActiveProfile = getClient.ActiveProfile;
                client.ActiveDate = getClient.ActiveDate;
                client.LogoFile = getClient.LogoFile;
            }

            return client;
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

        public static int SaveClient(ClientProfile clientProfile)
        {
            int clientId = Convert.ToInt32(clientProfile.ClientID);

            if (clientId < 1)
            {
                clientProfile.ClientID = ClientService.NewClientId();
            }

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

            return clientProfile.ClientID;
        }

        public static void UploadClientLogo(int clientId, HttpPostedFileBase logoclient)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (logoclient != null && logoclient.ContentLength > 0)
                {
                    var client = db.tblClient.Find(clientId);

                    using (var reader = new System.IO.BinaryReader(logoclient.InputStream))
                    {
                        client.LogoFile = reader.ReadBytes(logoclient.ContentLength);
                    }

                    db.SaveChanges();
                }
            }
        }

        public static byte[] GetClientLogo(int clientId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var client = db.tblClient.Find(clientId);

                return client.LogoFile;
            }
        }

        #endregion Client

        #region Division

        public static List<Division> GetDivisions(int clientId)
        {
            var divisions = new List<Division>();

            using (var db = new CMCSQL03Entities())
            {
                divisions = (from t in db.tblDivision
                             where t.ClientID == clientId
                             orderby t.DivisionName
                             select new Division
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
                                 ERProvider = t.ERProvider,
                                 ERRegistrant = t.ERRegistrant,
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
            }

            return divisions;
        }

        public static Division GetDivision(int? divisionId)
        {
            var division = new Division();

            using (var db = new CMCSQL03Entities())
            {
                var getDivision = db.tblDivision.Find(divisionId);

                division.DivisionID = getDivision.DivisionID;
                division.ClientID = getDivision.ClientID;
                division.DivisionName = getDivision.DivisionName;
                division.BusinessUnit = getDivision.BusinessUnit;
                division.WasteRateOffSpec = getDivision.WasteRate_OffSpec;
                division.WasteRateEmpty = getDivision.WasteRate_Empty;
                division.Inactive = getDivision.Inactive;
                division.ContactLabelName = getDivision.ContactLabelName;
                division.ContactLabelPhone = getDivision.ContactLabelPhone;
                division.ContactMSDSName = getDivision.ContactMSDSName;
                division.ContactMSDSPhone = getDivision.ContactMSDSPhone;
                division.EmergencyNumber = getDivision.EmergencyNumber;
                division.ERProvider = getDivision.ERProvider;
                division.ERRegistrant = getDivision.ERRegistrant;
                division.MainContactName = getDivision.MainContactName;
                division.MainContactNumber = getDivision.MainContactNumber;
                division.Abbr = getDivision.Abbr;
                division.UPSHazBook = getDivision.UPSHazBook;
                division.ExtMSDS = getDivision.ExtMSDS;
                division.ExtLabel = getDivision.ExtLabel;
                division.CompanyName = getDivision.CompanyName;
                division.CompanyStreet1 = getDivision.CompanyStreet1;
                division.CompanyStreet2 = getDivision.CompanyStreet2;
                division.CompanyStreet3 = getDivision.CompanyStreet3;
                division.CompanyPostalCode = getDivision.CompanyPostalCode;
                division.CompanyCity = getDivision.CompanyCity;
                division.CompanyCountry = getDivision.CompanyCountry;
                division.CompanyTelephone = getDivision.CompanyTelephone;
                division.CompanyFax = getDivision.CompanyFax;
                division.CompanyEmergencyTelephone = getDivision.CompanyEmergencyTelephone;
                division.CompanyEmail = getDivision.CompanyEmail;
                division.CompanyWebsite = getDivision.CompanyWebsite;
                division.IncludeExpDtOnLabel = getDivision.IncludeExpDtOnLabel;
            }

            return division;
        }

        public static Division EmptyDivision(int clientId)
        {
            var division = new Division();
            division.DivisionID = -1;
            division.ClientID = clientId;

            return division;
        }

        public static int SaveDivision(Division division)
        {
            int? clientId = division.ClientID;
            int divisionId = division.DivisionID;

            using (var db = new CMCSQL03Entities())
            {
                var divisionRecord = db.tblDivision.Find(divisionId);

                if (divisionRecord != null)
                {
                    divisionRecord.DivisionName = division.DivisionName;
                    divisionRecord.BusinessUnit = division.BusinessUnit;
                    divisionRecord.EmergencyNumber = division.EmergencyNumber;
                    divisionRecord.ERProvider = division.ERProvider;
                    divisionRecord.ERRegistrant = division.ERRegistrant;
                    divisionRecord.Inactive = division.Inactive;
                    divisionRecord.MainContactName = division.MainContactName;
                    divisionRecord.MainContactNumber = division.MainContactNumber;
                    divisionRecord.ContactLabelName = division.ContactLabelName;
                    divisionRecord.ContactLabelPhone = division.ContactLabelPhone;
                    divisionRecord.ContactMSDSName = division.ContactMSDSName;
                    divisionRecord.ContactMSDSPhone = division.ContactMSDSPhone;
                    divisionRecord.WasteRate_OffSpec = division.WasteRateOffSpec;
                    divisionRecord.WasteRate_Empty = division.WasteRateEmpty;
                    divisionRecord.Abbr = division.Abbr;
                    divisionRecord.UPSHazBook = division.UPSHazBook;
                    divisionRecord.ExtMSDS = division.ExtMSDS;
                    divisionRecord.ExtLabel = division.ExtLabel;
                    divisionRecord.CompanyName = division.CompanyName;
                    divisionRecord.CompanyStreet1 = division.CompanyStreet1;
                    divisionRecord.CompanyStreet2 = division.CompanyStreet2;
                    divisionRecord.CompanyStreet3 = division.CompanyStreet3;
                    divisionRecord.CompanyPostalCode = division.CompanyPostalCode;
                    divisionRecord.CompanyCity = division.CompanyCity;
                    divisionRecord.CompanyCountry = division.CompanyCountry;
                    divisionRecord.CompanyTelephone = division.CompanyTelephone;
                    divisionRecord.CompanyFax = division.CompanyFax;
                    divisionRecord.CompanyEmergencyTelephone = division.CompanyEmergencyTelephone;
                    divisionRecord.CompanyEmail = division.CompanyEmail;
                    divisionRecord.CompanyWebsite = division.CompanyWebsite;
                    divisionRecord.IncludeExpDtOnLabel = division.IncludeExpDtOnLabel;

                    db.SaveChanges();
                }
                else
                {
                    var newDivisionRecord = new tblDivision
                    {
                        ClientID = clientId,
                        DivisionName = division.DivisionName,
                        BusinessUnit = division.BusinessUnit,
                        EmergencyNumber = division.EmergencyNumber,
                        ERProvider = division.ERProvider,
                        ERRegistrant = division.ERRegistrant,
                        Inactive = division.Inactive,
                        MainContactName = division.MainContactName,
                        MainContactNumber = division.MainContactNumber,
                        ContactLabelName = division.ContactLabelName,
                        ContactLabelPhone = division.ContactLabelPhone,
                        ContactMSDSName = division.ContactMSDSName,
                        ContactMSDSPhone = division.ContactMSDSPhone,
                        WasteRate_OffSpec = division.WasteRateOffSpec,
                        WasteRate_Empty = division.WasteRateEmpty,
                        Abbr = division.Abbr,
                        UPSHazBook = division.UPSHazBook,
                        ExtMSDS = division.ExtMSDS,
                        ExtLabel = division.ExtLabel,
                        CompanyName = division.CompanyName,
                        CompanyStreet1 = division.CompanyStreet1,
                        CompanyStreet2 = division.CompanyStreet2,
                        CompanyStreet3 = division.CompanyStreet3,
                        CompanyPostalCode = division.CompanyPostalCode,
                        CompanyCity = division.CompanyCity,
                        CompanyCountry = division.CompanyCountry,
                        CompanyTelephone = division.CompanyTelephone,
                        CompanyFax = division.CompanyFax,
                        CompanyEmergencyTelephone = division.CompanyEmergencyTelephone,
                        CompanyEmail = division.CompanyEmail,
                        CompanyWebsite = division.CompanyWebsite,
                        IncludeExpDtOnLabel = division.IncludeExpDtOnLabel
                    };

                    db.tblDivision.Add(newDivisionRecord);
                    db.SaveChanges();

                    divisionId = newDivisionRecord.DivisionID;
                }
            }

            return divisionId;
        }

        public static void UploadDivisionLogo(int divisionId, HttpPostedFileBase logo)
        {
            using (var db = new CMCSQL03Entities())
            {
                if (logo != null && logo.ContentLength > 0)
                {
                    var division = db.tblDivision.Find(divisionId);

                    using (var reader = new System.IO.BinaryReader(logo.InputStream))
                    {
                        division.LogoFile = reader.ReadBytes(logo.ContentLength);
                    }

                    db.SaveChanges();
                }
            }
        }

        public static byte[] GetDivisionLogo(int divisionId)
        {
            using (var db = new CMCSQL03Entities())
            {
                var division = db.tblDivision.Find(divisionId);

                if (division.LogoFile == null)
                {
                    division.LogoFile = db.tblClient
                                          .Where(x => x.ClientID == division.ClientID)
                                          .Select(x => x.LogoFile)
                                          .FirstOrDefault();
                }

                return division.LogoFile;
            }
        }

        #endregion Division

        #region Supplier

        public static List<Supplier> GetSuppliers(int clientId)
        {
            var suppliers = new List<Supplier>();

            using (var db = new CMCSQL03Entities())
            {
                suppliers = (from t in db.tblBulkSupplier
                             where t.ClientID == clientId
                             orderby t.CompanyName
                             select new Supplier
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
            }

            return suppliers;
        }

        public static Supplier GetSupplier(int bulkSupplierId)
        {
            var supplier = new Supplier();

            using (var db = new CMCSQL03Entities())
            {
                var getSupplier = db.tblBulkSupplier.Find(bulkSupplierId);

                supplier.BulkSupplierID = getSupplier.BulkSupplierID;
                supplier.ClientID = getSupplier.ClientID;
                supplier.SupplyID = getSupplier.SupplyID;
                supplier.SupplierCode = getSupplier.ShortName;
                supplier.SupplierName = getSupplier.CompanyName;
                supplier.ContactName = getSupplier.ContactName;
                supplier.Address1 = getSupplier.Address1;
                supplier.Address2 = getSupplier.Address2;
                supplier.Address3 = getSupplier.Address3;
                supplier.City = getSupplier.City;
                supplier.State = getSupplier.State;
                supplier.PostalCode = getSupplier.Zip;
                supplier.Country = getSupplier.Country;
                supplier.Email = getSupplier.Email;
                supplier.Phone = getSupplier.Phone;
                supplier.Fax = getSupplier.Fax;
            }

            return supplier;
        }

        public static Supplier EmptySupplier(int clientId)
        {
            var supplier = new Supplier();
            supplier.BulkSupplierID = -1;
            supplier.ClientID = clientId;

            return supplier;
        }

        public static void SaveSupplierRecord(Supplier supplier)
        {
            using (var db = new CMCSQL03Entities())
            {
                var supplierRecord = db.tblBulkSupplier.Find(supplier.BulkSupplierID);

                // Check for duplicate SupplyID related to the Client
                int supplyDuplicateCheck = db.tblBulkSupplier
                                             .Where(x => x.SupplyID == supplier.SupplyID
                                                      && x.ClientID == supplier.ClientID)
                                             .Count();

                try
                {
                    if (supplierRecord.SupplyID != supplier.SupplyID)
                    {
                        if (supplyDuplicateCheck > 0)
                        {
                            supplier.SupplyID += " - Duplicate";
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    // Catch NullReferenceException which is thrown when adding new supplier
                    if (supplyDuplicateCheck > 0)
                    {
                        supplier.SupplyID += " - Duplicate";
                    }
                }

                if (supplierRecord != null)
                {
                    supplierRecord.SupplyID = supplier.SupplyID;
                    supplierRecord.ShortName = supplier.SupplierCode;
                    supplierRecord.CompanyName = supplier.SupplierName;
                    supplierRecord.ContactName = supplier.ContactName;
                    supplierRecord.Address1 = supplier.Address1;
                    supplierRecord.Address2 = supplier.Address2;
                    supplierRecord.Address3 = supplier.Address3;
                    supplierRecord.City = supplier.City;
                    supplierRecord.State = supplier.State;
                    supplierRecord.Zip = supplier.PostalCode;
                    supplierRecord.Country = supplier.Country;
                    supplierRecord.Email = supplier.Email;
                    supplierRecord.Phone = supplier.Phone;
                    supplierRecord.Fax = supplier.Fax;

                    db.SaveChanges();
                }
                else
                {
                    var newSupplierRecord = new tblBulkSupplier
                    {
                        BulkSupplierID = supplier.BulkSupplierID,
                        ClientID = supplier.ClientID,
                        SupplyID = supplier.SupplyID,
                        ShortName = supplier.SupplierCode,
                        CompanyName = supplier.SupplierName,
                        ContactName = supplier.ContactName,
                        Address1 = supplier.Address1,
                        Address2 = supplier.Address2,
                        Address3 = supplier.Address3,
                        City = supplier.City,
                        State = supplier.State,
                        Zip = supplier.PostalCode,
                        Country = supplier.Country,
                        Email = supplier.Email,
                        Phone = supplier.Phone,
                        Fax = supplier.Fax
                    };

                    db.tblBulkSupplier.Add(newSupplierRecord);
                    db.SaveChanges();
                }
            }
        }

        #endregion Supplier

        #region Client Contact

        public static List<Contact> GetClientContacts(int clientId)
        {
            var clientContacts = new List<Contact>();

            using (var db = new CMCSQL03Entities())
            {
                clientContacts = (from t in db.tblClientContact
                                  where t.ClientID == clientId
                                  orderby t.ContactType, t.FullName
                                  select new Contact
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
            }

            return clientContacts;
        }

        public static Contact GetClientContact(int clientContactId)
        {
            var contact = new Contact();

            using (var db = new CMCSQL03Entities())
            {
                var getContact = db.tblClientContact.Find(clientContactId);

                contact.ClientContactID = getContact.ClientContactID;
                contact.ClientID = getContact.ClientID;
                contact.ContactType = getContact.ContactType;
                contact.Account = getContact.Account;
                contact.FullName = getContact.FullName;
                contact.Email = getContact.Email;
                contact.Phone = getContact.Phone;
                contact.Fax = getContact.Fax;
                contact.Company = getContact.Company;
                contact.DistributorName = getContact.DistributorName;
                contact.Address1 = getContact.Address1;
                contact.Address2 = getContact.Address2;
                contact.City = getContact.City;
                contact.State = getContact.State;
                contact.Zip = getContact.Zip;
                contact.Country = getContact.Country;
            }

            return contact;
        }

        public static Contact EmptyClientContact(int clientId)
        {
            var clientContact = new Contact();
            clientContact.ClientContactID = -1;
            clientContact.ClientID = clientId;

            return clientContact;
        }

        public static void SaveClientContact(Contact clientcontact)
        {
            using (var db = new CMCSQL03Entities())
            {
                var clientContact = db.tblClientContact.Find(clientcontact.ClientContactID);

                if (clientContact != null)
                {
                    clientContact.ContactType = clientcontact.ContactType;
                    clientContact.Account = clientcontact.Account;
                    clientContact.FullName = clientcontact.FullName;
                    clientContact.Email = clientcontact.Email;
                    clientContact.Phone = clientcontact.Phone;
                    clientContact.Fax = clientcontact.Fax;
                    clientContact.Company = clientcontact.Company;
                    clientContact.DistributorName = clientcontact.DistributorName;
                    clientContact.Address1 = clientcontact.Address1;
                    clientContact.Address2 = clientcontact.Address2;
                    clientContact.City = clientcontact.City;
                    clientContact.State = clientcontact.State;
                    clientContact.Zip = clientcontact.Zip;
                    clientContact.Country = clientcontact.Country;

                    db.SaveChanges();
                }
                else
                {
                    var newClientContact = new tblClientContact
                    {
                        ClientContactID = Convert.ToInt32(clientcontact.ClientContactID),
                        ClientID = Convert.ToInt32(clientcontact.ClientID),
                        ContactType = clientcontact.ContactType,
                        Account = clientcontact.Account,
                        FullName = clientcontact.FullName,
                        Email = clientcontact.Email,
                        Phone = clientcontact.Phone,
                        Fax = clientcontact.Fax,
                        Company = clientcontact.Company,
                        DistributorName = clientcontact.DistributorName,
                        Address1 = clientcontact.Address1,
                        Address2 = clientcontact.Address2,
                        City = clientcontact.City,
                        State = clientcontact.State,
                        Zip = clientcontact.Zip,
                        Country = clientcontact.Country
                    };

                    db.tblClientContact.Add(newClientContact);
                    db.SaveChanges();
                }
            }
        }

        #endregion Client Contact

        #region Tier

        public static List<Tier> GetTiers(int clientId)
        {
            var tiers = new List<Tier>();

            using (var db = new CMCSQL03Entities())
            {
                tiers = (from t in db.tblTier
                         where t.ClientID == clientId
                         orderby t.TierLevel
                         select new Tier
                         {
                             TierID = t.TierID,
                             ClientID = t.ClientID,
                             TierLevel = t.TierLevel,
                             Size = t.Size,
                             LoSampQty = t.LoSampAmt,
                             HiSampQty = t.HiSampAmt,
                             Price = t.Price
                         }).ToList();
            }

            return tiers;
        }

        public static Tier GetTier(int tierId)
        {
            var tier = new Tier();

            using (var db = new CMCSQL03Entities())
            {
                var result = db.tblTier.Find(tierId);

                tier.TierID = result.TierID;
                tier.ClientID = result.ClientID;
                tier.TierLevel = result.TierLevel;
                tier.Size = result.Size;
                tier.LoSampQty = result.LoSampAmt;
                tier.HiSampQty = result.HiSampAmt;
                tier.Price = result.Price;
            }

            return tier;
        }

        public static Tier EmptyTier(int clientid)
        {
            var tier = new Tier();
            tier.TierID = -1;
            tier.ClientID = clientid;

            return tier;
        }

        public static void SaveTier(Tier tier)
        {
            using (var db = new CMCSQL03Entities())
            {
                var tierRecord = db.tblTier.Find(tier.TierID);

                if (tierRecord != null)
                {
                    tierRecord.TierLevel = tier.TierLevel;
                    tierRecord.Size = tier.Size;
                    tierRecord.LoSampAmt = tier.LoSampQty;
                    tierRecord.HiSampAmt = tier.HiSampQty;
                    tierRecord.Price = tier.Price;

                    db.SaveChanges();
                }
                else
                {
                    var newTier = new tblTier
                    {
                        TierID = Convert.ToInt32(tier.TierID),
                        ClientID = Convert.ToInt32(tier.ClientID),
                        TierLevel = tier.TierLevel,
                        Size = tier.Size,
                        LoSampAmt = tier.LoSampQty,
                        HiSampAmt = tier.HiSampQty,
                        Price = tier.Price
                    };

                    db.tblTier.Add(newTier);
                    db.SaveChanges();
                }
            }
        }

        #endregion Tier

        #region End Use

        public static List<EndUse> GetEndUses(int clientId)
        {
            var endUses = new List<EndUse>();

            using (var db = new CMCSQL03Entities())
            {
                endUses = (from t in db.tblEndUse
                           where t.ClientID == clientId
                           orderby t.EndUse
                           select new EndUse
                           {
                               EndUseID = t.EndUseID,
                               ClientID = t.ClientID,
                               EndUseString = t.EndUse
                           }).ToList();
            }

            return endUses;
        }

        public static int AddEndUse(int clientid, string endusestring)
        {
            int clientId = clientid;

            using (var db = new CMCSQL03Entities())
            {
                var newrow = new tblEndUse { };
                newrow.ClientID = clientId;
                newrow.EndUse = endusestring;

                db.tblEndUse.Add(newrow);
                db.SaveChanges();
            }

            return clientId;
        }

        public static void DeleteEnduse(int endUseId)
        {
            using (var db = new CMCSQL03Entities())
            {
                string deleteQuery = @"DELETE FROM tblEndUse WHERE EndUseID=" + endUseId;
                db.Database.ExecuteSqlCommand(deleteQuery);
            }
        }

        #endregion End Use

        #region Surcharge Rates

        public static Surcharge GetSurcharges(int clientId)
        {
            var surcharges = new Surcharge();

            using (var db = new CMCSQL03Entities())
            {
                surcharges = (from t in db.tblSurcharge
                              where t.ClientID == clientId
                              orderby t.SurchargeID
                              select new Surcharge
                              {
                                  SurchargeID = t.SurchargeID,
                                  ClientID = clientId,
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
            }

            return surcharges;
        }

        public static string SaveSurcharge(Surcharge surcharge, int clientId)
        {
            string message;

            using (var db = new CMCSQL03Entities())
            {
                var surchargeRecord = db.tblSurcharge.Find(surcharge.SurchargeID);

                try
                {
                    if (surchargeRecord != null)
                    {
                        surchargeRecord.Haz = surcharge.Haz;
                        surchargeRecord.Flam = surcharge.Flam;
                        surchargeRecord.Clean = surcharge.Clean;
                        surchargeRecord.Heat = surcharge.Heat;
                        surchargeRecord.Refrig = surcharge.Refrig;
                        surchargeRecord.Freezer = surcharge.Freezer;
                        surchargeRecord.Nalgene = surcharge.Nalgene;
                        surchargeRecord.Nitrogen = surcharge.Nitrogen;
                        surchargeRecord.Biocide = surcharge.Biocide;
                        surchargeRecord.Blend = surcharge.Blend;
                        surchargeRecord.Kosher = surcharge.Kosher;

                        db.SaveChanges();

                        message = "Save Successful!";
                    }
                    else
                    {
                        var newSurchargeRecord = new tblSurcharge
                        {
                            ClientID = clientId,
                            Haz = surcharge.Haz,
                            Flam = surcharge.Flam,
                            Clean = surcharge.Clean,
                            Heat = surcharge.Heat,
                            Refrig = surcharge.Refrig,
                            Freezer = surcharge.Freezer,
                            Nalgene = surcharge.Nalgene,
                            Nitrogen = surcharge.Nitrogen,
                            Biocide = surcharge.Biocide,
                            Blend = surcharge.Blend,
                            Kosher = surcharge.Kosher
                        };

                        db.tblSurcharge.Add(newSurchargeRecord);
                        db.SaveChanges();

                        message = "New record added";
                    }
                }
                catch (Exception)
                {
                    message = "Something is wrong with your input...";
                }
            }

            return message;
        }

        #endregion Surcharge Rates

        #region Service Charge Rates

        public static ServiceChargeRates GetServiceChargeRates(int clientId)
        {
            var serviceChargeRates = new ServiceChargeRates();

            using (var db = new CMCSQL03Entities())
            {
                serviceChargeRates = (from t in db.tblRates
                                      where t.ClientID == clientId
                                      select new ServiceChargeRates
                                      {
                                          RatesID = t.RatesID,
                                          ClientID = t.ClientID,
                                          AirHazardOnly = t.AirHazardOnly,
                                          CertificateOfOrigin = t.CertificateOfOrigin,
                                          CMCPack = t.CMCPack,
                                          CoolPack = t.CoolPack,
                                          CreditCardFee = t.CreditCardFee,
                                          CreditCardOrder = t.CreditCardOrder,
                                          DocumentHandling = t.DocumentHandling,
                                          EmptyPackaging = t.EmptyPackaging,
                                          ExternalSystem = t.ExternalSystem,
                                          FollowUpOrder = t.FollowUpOrder,
                                          FreezerPack = t.FreezerPack,
                                          GHSLabels = t.GHSLabels,
                                          InactiveProducts = t.InactiveProducts,
                                          Isolation = t.Isolation,
                                          IsolationBox = t.IsolationBox,
                                          ITFee = t.ITFee,
                                          LabelMaintainance = t.LabelMaintainance,
                                          LabelStock = t.LabelStock,
                                          LabelsPrinted = t.LabelsPrinted,
                                          LaborRelabel = t.LaborRelabel,
                                          LiteratureFee = t.LiteratureFee,
                                          LimitedQuantity = t.LimitedQuantity,
                                          ManualHandling = t.ManualHandling,
                                          MSDSPrints = t.MSDSPrints,
                                          NewLabelSetup = t.NewLabelSetup,
                                          NewProductSetup = t.NewProductSetup,
                                          OberkPack = t.OberkPack,
                                          OrderEntry = t.OrderEntry,
                                          OverPack = t.OverPack,
                                          PalletReturn = t.PalletReturn,
                                          PoisonPack = t.PoisonPack,
                                          ProductSetupChanges = t.ProductSetupChanges,
                                          QCStorage = t.QCStorage,
                                          RDHandlingADR = t.RDHandlingADR,
                                          RDHandlingIATA = t.RDHandlingIATA,
                                          RDHandlingLQ = t.RDHandlingLQ,
                                          RDHandlingNonHazard = t.RDHandlingNonHazard,
                                          RefrigeratorStorage = t.RefrigeratorStorage,
                                          Relabels = t.Relabels,
                                          RushShipment = t.RushShipment,
                                          SPA197Applied = t.SPA197Applied,
                                          SPSPaidOrder = t.SPSPaidOrder,
                                          UNBox = t.UNBox,
                                          WarehouseStorage = t.WarehouseStorage,
                                          WHMISLabels = t.WHMISLabels,
                                      }).FirstOrDefault();
            }

            return serviceChargeRates;
        }

        public static string SaveChargeRates(ServiceChargeRates serviceChargeRates, int clientId)
        {
            string message;

            using (var db = new CMCSQL03Entities())
            {
                var rates = db.tblRates.Find(serviceChargeRates.RatesID);

                try
                {
                    if (rates != null)
                    {
                        rates.RatesID = serviceChargeRates.RatesID;
                        rates.AirHazardOnly = serviceChargeRates.AirHazardOnly;
                        rates.CertificateOfOrigin = serviceChargeRates.CertificateOfOrigin;
                        rates.CMCPack = serviceChargeRates.CMCPack;
                        rates.CoolPack = serviceChargeRates.CoolPack;
                        rates.CreditCardFee = serviceChargeRates.CreditCardFee;
                        rates.CreditCardOrder = serviceChargeRates.CreditCardOrder;
                        rates.DocumentHandling = serviceChargeRates.DocumentHandling;
                        rates.EmptyPackaging = serviceChargeRates.EmptyPackaging;
                        rates.ExternalSystem = serviceChargeRates.ExternalSystem;
                        rates.FollowUpOrder = serviceChargeRates.FollowUpOrder;
                        rates.FreezerPack = serviceChargeRates.FreezerPack;
                        rates.GHSLabels = serviceChargeRates.GHSLabels;
                        rates.InactiveProducts = serviceChargeRates.InactiveProducts;
                        rates.Isolation = serviceChargeRates.Isolation;
                        rates.IsolationBox = serviceChargeRates.IsolationBox;
                        rates.ITFee = serviceChargeRates.ITFee;
                        rates.LabelMaintainance = serviceChargeRates.LabelMaintainance;
                        rates.LabelStock = serviceChargeRates.LabelStock;
                        rates.LabelsPrinted = serviceChargeRates.LabelsPrinted;
                        rates.LaborRelabel = serviceChargeRates.LaborRelabel;
                        rates.LiteratureFee = serviceChargeRates.LiteratureFee;
                        rates.LimitedQuantity = serviceChargeRates.LimitedQuantity;
                        rates.ManualHandling = serviceChargeRates.ManualHandling;
                        rates.MSDSPrints = serviceChargeRates.MSDSPrints;
                        rates.NewLabelSetup = serviceChargeRates.NewLabelSetup;
                        rates.NewProductSetup = serviceChargeRates.NewProductSetup;
                        rates.OberkPack = serviceChargeRates.OberkPack;
                        rates.OrderEntry = serviceChargeRates.OrderEntry;
                        rates.OverPack = serviceChargeRates.OverPack;
                        rates.PalletReturn = serviceChargeRates.PalletReturn;
                        rates.PoisonPack = serviceChargeRates.PoisonPack;
                        rates.ProductSetupChanges = serviceChargeRates.ProductSetupChanges;
                        rates.QCStorage = serviceChargeRates.QCStorage;
                        rates.RDHandlingADR = serviceChargeRates.RDHandlingADR;
                        rates.RDHandlingIATA = serviceChargeRates.RDHandlingIATA;
                        rates.RDHandlingLQ = serviceChargeRates.RDHandlingLQ;
                        rates.RDHandlingNonHazard = serviceChargeRates.RDHandlingNonHazard;
                        rates.RefrigeratorStorage = serviceChargeRates.RefrigeratorStorage;
                        rates.Relabels = serviceChargeRates.Relabels;
                        rates.RushShipment = serviceChargeRates.RushShipment;
                        rates.SPA197Applied = serviceChargeRates.SPA197Applied;
                        rates.SPSPaidOrder = serviceChargeRates.SPSPaidOrder;
                        rates.UNBox = serviceChargeRates.UNBox;
                        rates.WarehouseStorage = serviceChargeRates.WarehouseStorage;
                        rates.WHMISLabels = serviceChargeRates.WHMISLabels;

                        db.SaveChanges();

                        message = "Save Successful!";
                    }
                    else
                    {
                        var newRates = new tblRates
                        {
                            ClientID = clientId,
                            AirHazardOnly = serviceChargeRates.AirHazardOnly,
                            CertificateOfOrigin = serviceChargeRates.CertificateOfOrigin,
                            CMCPack = serviceChargeRates.CMCPack,
                            CoolPack = serviceChargeRates.CoolPack,
                            CreditCardFee = serviceChargeRates.CreditCardFee,
                            CreditCardOrder = serviceChargeRates.CreditCardOrder,
                            DocumentHandling = serviceChargeRates.DocumentHandling,
                            EmptyPackaging = serviceChargeRates.EmptyPackaging,
                            ExternalSystem = serviceChargeRates.ExternalSystem,
                            FollowUpOrder = serviceChargeRates.FollowUpOrder,
                            FreezerPack = serviceChargeRates.FreezerPack,
                            GHSLabels = serviceChargeRates.GHSLabels,
                            InactiveProducts = serviceChargeRates.InactiveProducts,
                            Isolation = serviceChargeRates.Isolation,
                            IsolationBox = serviceChargeRates.IsolationBox,
                            ITFee = serviceChargeRates.ITFee,
                            LabelMaintainance = serviceChargeRates.LabelMaintainance,
                            LabelStock = serviceChargeRates.LabelStock,
                            LabelsPrinted = serviceChargeRates.LabelsPrinted,
                            LaborRelabel = serviceChargeRates.LaborRelabel,
                            LiteratureFee = serviceChargeRates.LiteratureFee,
                            LimitedQuantity = serviceChargeRates.LimitedQuantity,
                            ManualHandling = serviceChargeRates.ManualHandling,
                            MSDSPrints = serviceChargeRates.MSDSPrints,
                            NewLabelSetup = serviceChargeRates.NewLabelSetup,
                            NewProductSetup = serviceChargeRates.NewProductSetup,
                            OberkPack = serviceChargeRates.OberkPack,
                            OrderEntry = serviceChargeRates.OrderEntry,
                            OverPack = serviceChargeRates.OverPack,
                            PalletReturn = serviceChargeRates.PalletReturn,
                            PoisonPack = serviceChargeRates.PoisonPack,
                            ProductSetupChanges = serviceChargeRates.ProductSetupChanges,
                            QCStorage = serviceChargeRates.QCStorage,
                            RDHandlingADR = serviceChargeRates.RDHandlingADR,
                            RDHandlingIATA = serviceChargeRates.RDHandlingIATA,
                            RDHandlingLQ = serviceChargeRates.RDHandlingLQ,
                            RDHandlingNonHazard = serviceChargeRates.RDHandlingNonHazard,
                            RefrigeratorStorage = serviceChargeRates.RefrigeratorStorage,
                            Relabels = serviceChargeRates.Relabels,
                            RushShipment = serviceChargeRates.RushShipment,
                            SPA197Applied = serviceChargeRates.SPA197Applied,
                            SPSPaidOrder = serviceChargeRates.SPSPaidOrder,
                            UNBox = serviceChargeRates.UNBox,
                            WarehouseStorage = serviceChargeRates.WarehouseStorage,
                            WHMISLabels = serviceChargeRates.WHMISLabels,
                        };

                        db.tblRates.Add(newRates);
                        db.SaveChanges();

                        message = "New record added";
                    }
                }
                catch (Exception)
                {
                    message = "Something is wrong with your input...";
                }
            }

            return message;
        }
        
        #endregion

        #region Account Representative

        public static List<int> GetAccountRepClientIds(string user)
        {
            var clientids = new List<int>();

            using(var db = new CMCSQL03Entities())
            { 
                clientids = db.tblClientAccountRep
                              .Where(x => x.AccountRepEmail == user)
                              .Select(x => x.ClientID)
                              .ToList();
            }

            return clientids;
        }

        public static List<AccountRep> GetAccountReps(int clientid)
        {
            var accountReps = new List<AccountRep>();

            using (var db = new CMCSQL03Entities())
            {
                accountReps = (from t in db.tblClientAccountRep
                               where t.ClientID == clientid
                               select new AccountRep
                               {
                                   AccountRepID = t.AccountRepID,
                                   ClientID = t.ClientID,
                                   AccountRepUserID = t.AccountRepUserID,
                                   AccountRepName = t.AccountRepName,
                                   AccountRepEmail = t.AccountRepEmail
                               }).ToList();
            }

            return accountReps;
        }

        public static int AddAccountRep(int clientid, string accountrepemail)
        {
            string repUserId;
            string repUserFullName;
            int clientId = clientid;

            // Get user id and full name
            using (var auth = new ApplicationDbContext())
            {
                var getUser = auth.Users
                                  .Where(t => t.Email == accountrepemail)
                                  .FirstOrDefault();

                repUserId = getUser.Id;
                repUserFullName = getUser.FirstName + " " + getUser.LastName;
            }

            using (var db = new CMCSQL03Entities())
            {
                // Check if user was assigned already
                bool isUserExists = db.tblClientAccountRep
                                      .Any(c => c.ClientID == clientId
                                             && c.AccountRepUserID == repUserId);

                // Assign user to client account
                if (isUserExists == false)
                {
                    var newrow = new tblClientAccountRep { };
                    newrow.ClientID = clientId;
                    newrow.AccountRepUserID = repUserId;
                    newrow.AccountRepName = repUserFullName;
                    newrow.AccountRepEmail = accountrepemail;

                    db.tblClientAccountRep.Add(newrow);
                    db.SaveChanges();
                }
            }

            return clientId;
        }

        public static void DeleteAccountRep(string accountrepemail, int clientid)
        {
            using (var db = new CMCSQL03Entities())
            {
                int accountRepId = db.tblClientAccountRep
                                        .Where(t => t.ClientID == clientid
                                                 && t.AccountRepEmail == accountrepemail)
                                        .Select(t => t.AccountRepID).FirstOrDefault();

                if (accountRepId > 0)
                {
                    string deleteQuery = @"DELETE FROM tblClientAccountRep WHERE AccountRepID=" + accountRepId;
                    db.Database.ExecuteSqlCommand(deleteQuery);
                }
            }
        }

        #endregion Account Representative
    }
}