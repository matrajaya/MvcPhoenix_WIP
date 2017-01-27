using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ClientProfile
    {
        public int ClientID { get; set; }
        public int? LegacyID { get; set; }
        public int? GlobalClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public bool? ActiveProfile { get; set; }
        public DateTimeOffset? ActiveDate { get; set; }
        public string CMCLocation { get; set; }
        public enum CMCLocationChoice
        {
            CT,
            CO,
            EU,
            AP
        }
        public string ClientReference { get; set; }
        public string ClientEntityName { get; set; }
        public string ClientCurrency { get; set; }
        public enum CurrencyChoice
        {
            USD,
            EUR,
            CNY
        }
        public string ClientUM { get; set; }
        public enum UMChoice
        {
            KG,
            LB,
        }
        public string ClientNetTerm { get; set; }
        [AllowHtml]
        public string InvoiceAddress { get; set; }
        [AllowHtml]
        public string InvoiceEmailTo { get; set; }
        [AllowHtml]
        public string KeyContactDir { get; set; }
        public byte[] LogoFile { get; set; }

        #region Client Profile Fields Shelfed - Consider Later

        //[Display(Name = "MSDS")]
        //public bool MSDS { get; set; }

        //[Display(Name = "MSDS File Directory")]
        //public string MSDSFileDir { get; set; }

        //[Display(Name = "TDS")]
        //public bool TDS { get; set; }

        //[Display(Name = "TDS File Directory")]
        //public string TDSFileDir { get; set; }

        //[Display(Name = "COA")]
        //public bool COA { get; set; }

        //[Display(Name = "COA File Dir")]
        //public string COAFileDir { get; set; }

        //[Display(Name = "BOL Comment")]
        //public string BOLComment { get; set; }

        //[Display(Name = "Logo File Directory")]
        //public string LogoFileDir { get; set; }

        //[AllowHtml]
        //[Display(Name = "Client Contact Information")]
        //public string ClientContact { get; set; }

        //[AllowHtml]
        //[Display(Name = "CMC Contact Information")]
        //public string CMCContact { get; set; }

        //[Display(Name = "Client Website")]
        //public string ClientUrl { get; set; }

        //[AllowHtml]
        //[Display(Name = "Product Setup Details")]
        //public string ProductSetupDetails { get; set; }

        //[AllowHtml]
        //[Display(Name = "SDS Language(s)")]
        //public string SDSLanguage { get; set; }

        //[AllowHtml]
        //[Display(Name = "SDS Update Method")]
        //public string SDSUpdateMethod { get; set; }

        //[Display(Name = "Label Language(s)")]
        //public string LabelLanguage { get; set; }

        //[Display(Name = "SDS Required")]
        //public bool SDSRequired { get; set; }

        //[Display(Name = "COA Required")]
        //public bool COARequired { get; set; }

        //[Display(Name = "TDS Required")]
        //public bool TDSRequired { get; set; }

        //[Display(Name = "Std Cover Letter Required")]
        //public bool StdCoverLetterRequired { get; set; }

        //[AllowHtml]
        //[Display(Name = "Inventory Reports")]
        //public string InventoryReports { get; set; }

        //[Display(Name = "Cease Ship Period")]
        //public string CeaseShipPeriod { get; set; }

        //[Display(Name = "Replenishment Lead Days")]
        //public string ReplenishmentLeadDays { get; set; }

        //[Display(Name = "Expiration Date On Label")]
        //public bool ExpDateOnLabel { get; set; }

        //[AllowHtml]
        //[Display(Name = "Expiry Date Rules")]
        //public string ExpDateRules { get; set; }

        //[AllowHtml]
        //[Display(Name = "Waste Procedure")]
        //public string WasteProcedure { get; set; }

        //[AllowHtml]
        //[Display(Name = "Inventory Business Rules")]
        //public string InventoryBusinessRules { get; set; }

        //[AllowHtml]
        //[Display(Name = "Special Request Allowed")]
        //public string SpecialReqAllowed { get; set; }

        //[Display(Name = "Commercial Invoice Value")]
        //public string CommercialInvoiceValue { get; set; }

        //[AllowHtml]
        //[Display(Name = "Freezable Procedure")]
        //public string FreezableProcedure { get; set; }

        //[Display(Name = "Survey Used")]
        //public bool SurveyUsed { get; set; }

        //[AllowHtml]
        //[Display(Name = "Business Rules")]
        //public string CSBusinessRules { get; set; }

        //[AllowHtml]
        //[Display(Name = "Shipping Rules")]
        //public string ShippingRules { get; set; }

        //[Display(Name = "Ship Confirm Email")]
        //public string ShipConfirmEmail { get; set; }

        //[Display(Name = "Delay Confirm Email")]
        //public string DelayConfirmEmail { get; set; }

        //[Display(Name = "Order Confirm Email")]
        //public string OrderConfirmEmail { get; set; }

        //[Display(Name = "Invoice Segregation")]
        //public string InvoiceSegregation { get; set; }

        //[AllowHtml]
        //[Display(Name = "Charges Summary")]
        //public string ChargesSummary { get; set; }

        //[Display(Name = "Partial Delivery Allowed")]
        //public string PartialDeliveryAllowed { get; set; }

        //[Display(Name = "Client Status")]
        //public string ClientStatus { get; set; }
        
        //[Display(Name = "Surchage Hazard")]
        //public int? SurchageHazard { get; set; }

        //[Display(Name = "Surcharge Flammable")]
        //public int? SurchargeFlammable { get; set; }

        //[Display(Name = "Surcharge Heat")]
        //public int? SurchargeHeat { get; set; }

        //[Display(Name = "Surcharge Refrigerate")]
        //public int? SurchargeRefrigerate { get; set; }

        //[Display(Name = "Surcharge Freeze")]
        //public int? SurchargeFreeze { get; set; }

        //[Display(Name = "Surcharge Clean")]
        //public int? SurchargeClean { get; set; }

        //[Display(Name = "Surcharge Nalgene")]
        //public int? SurchargeNalgene { get; set; }

        //[Display(Name = "Surcharge Nitrogen")]
        //public int? SurchargeNitrogen { get; set; }

        //[Display(Name = "Surcharge Biocide")]
        //public int? SurchargeBiocide { get; set; }

        //[Display(Name = "Surcharge Blend")]
        //public int? SurchargeBlend { get; set; }

        //[Display(Name = "Surcharge Kosher")]
        //public int? SurchargeKosher { get; set; }

        //[Display(Name = "Hazard D&H Service Level 1")]
        //public int? HazDHServLvl1 { get; set; }

        //[Display(Name = "Hazard D&H Service Level 2")]
        //public int? HazDHServLvl2 { get; set; }

        //[Display(Name = "Hazard D&H Service Level 3")]
        //public int? HazDHServLvl3 { get; set; }

        //[Display(Name = "Hazard D&H Service Level 4")]
        //public int? HazDHServLvl4 { get; set; }

        //[Display(Name = "Non-Hazard D&H Service Level 1")]
        //public int? NonHazDHServLvl1 { get; set; }

        //[Display(Name = "Non-Hazard D&H Service Level 2")]
        //public int? NonHazDHServLvl2 { get; set; }

        //[Display(Name = "Non-Hazard D&H Service Level 3")]
        //public int? NonHazDHServLvl3 { get; set; }

        //[Display(Name = "Non-Hazard D&H Service Level 4")]
        //public int? NonHazDHServLvl4 { get; set; }
        #endregion
        
    }

    public class Division
    {
        public int DivisionID { get; set; }
        public int? ClientID { get; set; }
        public string DivisionName { get; set; }
        public string BusinessUnit { get; set; }
        public string Abbr { get; set; }
        public decimal? WasteRateOffSpec { get; set; }
        public decimal? WasteRateEmpty { get; set; }
        public bool? Inactive { get; set; }
        public string ContactLabelName { get; set; }
        public string ContactLabelPhone { get; set; }
        public string ContactMSDSName { get; set; }
        public string ContactMSDSPhone { get; set; }
        public string EmergencyNumber { get; set; }
        public string UPSHazBook { get; set; }
        public string ExtMSDS { get; set; }
        public string ExtLabel { get; set; }
        public string MainContactName { get; set; }
        public string MainContactNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyStreet1 { get; set; }
        public string CompanyStreet2 { get; set; }
        public string CompanyStreet3 { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyTelephone { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyEmergencyTelephone { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyWebsite { get; set; }
        public bool? IncludeExpDtOnLabel { get; set; }

        public List<SelectListItem> ListOfCountries { get; set; }
    }

    public class Supplier
    {
        public int BulkSupplierID { get; set; }
        public int? ClientID { get; set; }
        public string SupplyID { get; set; }
        public string SupplierCode { get; set; } // ShortName
        public string SupplierName { get; set; } // CompanyName
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; } // Zip
        public string Country { get; set; }

        public List<SelectListItem> ListOfCountries { get; set; }
    }

    public class Tier
    {
        public int TierID { get; set; }
        public int? ClientID { get; set; }
        public int? TierLevel { get; set; }
        public string Size { get; set; }
        public int? LoSampQty { get; set; }
        public int? HiSampQty { get; set; }
        public decimal? Price { get; set; }
    }

    public class Surcharge
    {
        public int SurchargeID { get; set; }
        public int? ClientID { get; set; }
        public decimal? Haz { get; set; }
        public decimal? Flam { get; set; }
        public decimal? Heat { get; set; }
        public decimal? Refrig { get; set; }
        public decimal? Freezer { get; set; }
        public decimal? Clean { get; set; }
        public decimal? Nalgene { get; set; }
        public decimal? Nitrogen { get; set; }
        public decimal? Biocide { get; set; }
        public decimal? Blend { get; set; }
        public decimal? Kosher { get; set; }
    }
}