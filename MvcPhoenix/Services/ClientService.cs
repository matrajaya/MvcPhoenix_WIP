using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// ************** ClientService.cs ********************
// This class contains Client Management service methods
// ******************************************************

namespace MvcPhoenix.Models
{
    public class ClientService
    {
        public static ClientProfile FillFromDB(ClientProfile CP)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = (from t in db.tblClient2 where t.ClientID == CP.ClientID select t).FirstOrDefault();
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
                CP.MSDS = q.MSDS ?? false;
                CP.MSDSFileDir = q.MSDSFileDir;
                CP.TDS = q.TDS ?? false;
                CP.TDSFileDir = q.TDSFileDir;
                CP.COA = q.COA ?? false;
                CP.COAFileDir = q.COAFileDir;
                CP.BOLComment = q.BOLComment;
                //CP.LogoFileDir = q.LogoFileDir;
                CP.LogoFileDir = q.ClientCode + ".gif";
                CP.InvoiceAddress = q.InvoiceAddress;
                CP.InvoiceEmailTo = q.InvoiceEmailTo;
                CP.KeyContactDir = q.KeyContactDir;
                CP.ClientContact = q.ClientContact;
                CP.CMCContact = q.CMCContact;
                CP.ClientUrl = q.ClientUrl;
                CP.ProductSetupDetails = q.ProductSetupDetails;
                CP.SDSLanguage = q.SDSLanguage;
                CP.SDSUpdateMethod = q.SDSUpdateMethod;
                CP.LabelLanguage = q.LabelLanguage;
                CP.SDSRequired = q.SDSRequired ?? false;
                CP.COARequired = q.COARequired ?? false;
                CP.TDSRequired = q.TDSRequired ?? false;
                CP.StdCoverLetterRequired = q.StdCoverLetterRequired ?? false;
                CP.InventoryReports = q.InventoryReports;
                CP.CeaseShipPeriod = q.CeaseShipPeriod;
                CP.ReplenishmentLeadDays = q.ReplenishmentLeadDays;
                CP.ExpDateOnLabel = q.ExpDateOnLabel ?? false;
                CP.ExpDateRules = q.ExpDateRules;
                CP.WasteProcedure = q.WasteProcedure;
                CP.InventoryBusinessRules = q.InventoryBusinessRules;
                CP.SpecialReqAllowed = q.SpecialReqAllowed;
                CP.CommercialInvoiceValue = q.CommercialInvoiceValue;
                CP.FreezableProcedure = q.FreezableProcedure;
                CP.SurveyUsed = q.SurveyUsed ?? false;
                CP.CSBusinessRules = q.CSBusinessRules;
                CP.ShipConfirmEmail = q.ShipConfirmEmail;
                CP.DelayConfirmEmail = q.DelayConfirmEmail;
                CP.OrderConfirmEmail = q.OrderConfirmEmail;
                CP.InvoiceSegregation = q.InvoiceSegregation;
                CP.ChargesSummary = q.ChargesSummary;
                CP.PartialDeliveryAllowed = q.PartialDeliveryAllowed;
                CP.ClientStatus = q.ClientStatus;
                CP.ActiveDate = q.ActiveDate;
                CP.SurchageHazard = q.SurchageHazard;
                CP.SurchargeFlammable = q.SurchargeFlammable;
                CP.SurchargeHeat = q.SurchargeHeat;
                CP.SurchargeRefrigerate = q.SurchargeRefrigerate;
                CP.SurchargeFreeze = q.SurchargeFreeze;
                CP.SurchargeClean = q.SurchargeClean;
                CP.SurchargeNalgene = q.SurchargeNalgene;
                CP.SurchargeNitrogen = q.SurchargeNitrogen;
                CP.SurchargeBiocide = q.SurchargeBiocide;
                CP.SurchargeBlend = q.SurchargeBlend;
                CP.SurchargeKosher = q.SurchargeKosher;
                CP.HazDHServLvl1 = q.HazDHServLvl1;
                CP.HazDHServLvl2 = q.HazDHServLvl2;
                CP.HazDHServLvl3 = q.HazDHServLvl3;
                CP.HazDHServLvl4 = q.HazDHServLvl4;
                CP.NonHazDHServLvl1 = q.NonHazDHServLvl1;
                CP.NonHazDHServLvl2 = q.NonHazDHServLvl2;
                CP.NonHazDHServLvl3 = q.NonHazDHServLvl3;
                CP.NonHazDHServLvl4 = q.NonHazDHServLvl4;

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
                var newrow = new EF.tblClient2{};
                db.tblClient2.Add(newrow);
                db.SaveChanges();
                int clientkey = newrow.ClientID;

                return clientkey;
            }
        }

        public static void SaveClient(ClientProfile CP)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var q = db.tblClient2.Find(CP.ClientID);
                q.ClientID = CP.ClientID;
                q.LegacyID = CP.LegacyID;
                q.GlobalClientID = CP.GlobalClientID;
                q.ClientCode = CP.ClientCode;
                q.ClientName = CP.ClientName;
                q.CMCLocation = CP.CMCLocation;
                //q.ClientReference = CP.ClientReference;
                //q.ClientEntityName = CP.ClientEntityName;
                q.ClientCurrency = CP.ClientCurrency;
                q.ClientUM = CP.ClientUM;
                q.MSDS = CP.MSDS;
                q.MSDSFileDir = CP.MSDSFileDir;
                q.TDS = CP.TDS;
                q.TDSFileDir = CP.TDSFileDir;
                q.COA = CP.COA;
                q.COAFileDir = CP.COAFileDir;
                q.BOLComment = CP.BOLComment;
                //q.LogoFileDir = CP.LogoFileDir;
                q.InvoiceAddress = CP.InvoiceAddress;
                q.InvoiceEmailTo = CP.InvoiceEmailTo;
                q.KeyContactDir = CP.KeyContactDir;
                q.ClientContact = CP.ClientContact;
                q.CMCContact = CP.CMCContact;
                q.ClientUrl = CP.ClientUrl;
                q.ProductSetupDetails = CP.ProductSetupDetails;
                q.SDSLanguage = CP.SDSLanguage;
                q.SDSUpdateMethod = CP.SDSUpdateMethod;
                q.LabelLanguage = CP.LabelLanguage;
                q.SDSRequired = CP.SDSRequired;
                q.COARequired = CP.COARequired;
                q.TDSRequired = CP.TDSRequired;
                q.StdCoverLetterRequired = CP.StdCoverLetterRequired;
                q.InventoryReports = CP.InventoryReports;
                q.CeaseShipPeriod = CP.CeaseShipPeriod;
                q.ReplenishmentLeadDays = CP.ReplenishmentLeadDays;
                q.ExpDateOnLabel = CP.ExpDateOnLabel;
                q.ExpDateRules = CP.ExpDateRules;
                q.WasteProcedure = CP.WasteProcedure;
                q.InventoryBusinessRules = CP.InventoryBusinessRules;
                q.SpecialReqAllowed = CP.SpecialReqAllowed;
                q.CommercialInvoiceValue = CP.CommercialInvoiceValue;
                q.FreezableProcedure = CP.FreezableProcedure;
                q.SurveyUsed = CP.SurveyUsed;
                q.CSBusinessRules = CP.CSBusinessRules;
                q.ShipConfirmEmail = CP.ShipConfirmEmail;
                q.DelayConfirmEmail = CP.DelayConfirmEmail;
                q.OrderConfirmEmail = CP.OrderConfirmEmail;
                q.InvoiceSegregation = CP.InvoiceSegregation;
                q.ChargesSummary = CP.ChargesSummary;
                q.PartialDeliveryAllowed = CP.PartialDeliveryAllowed;
                q.ClientStatus = CP.ClientStatus;
                q.ActiveDate = CP.ActiveDate;
                q.SurchageHazard = CP.SurchageHazard;
                q.SurchargeFlammable = CP.SurchargeFlammable;
                q.SurchargeHeat = CP.SurchargeHeat;
                q.SurchargeRefrigerate = CP.SurchargeRefrigerate;
                q.SurchargeFreeze = CP.SurchargeFreeze;
                q.SurchargeClean = CP.SurchargeClean;
                q.SurchargeNalgene = CP.SurchargeNalgene;
                q.SurchargeNitrogen = CP.SurchargeNitrogen;
                q.SurchargeBiocide = CP.SurchargeBiocide;
                q.SurchargeBlend = CP.SurchargeBlend;
                q.SurchargeKosher = CP.SurchargeKosher;
                q.HazDHServLvl1 = CP.HazDHServLvl1;
                q.HazDHServLvl2 = CP.HazDHServLvl2;
                q.HazDHServLvl3 = CP.HazDHServLvl3;
                q.HazDHServLvl4 = CP.HazDHServLvl4;
                q.NonHazDHServLvl1 = CP.NonHazDHServLvl1;
                q.NonHazDHServLvl2 = CP.NonHazDHServLvl2;
                q.NonHazDHServLvl3 = CP.NonHazDHServLvl3;
                q.NonHazDHServLvl4 = CP.NonHazDHServLvl4;

                db.SaveChanges();
            }
        }

    }
}