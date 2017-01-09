using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ClientProfile
    {
        [Display(Name = "Client ID")]
        public int ClientID { get; set; }

        [Display(Name = "Legacy")]
        public int? LegacyID { get; set; }

        [Display(Name = "Global Client ID")]
        public int? GlobalClientID { get; set; }

        [Display(Name = "Client Code")]
        public string ClientCode { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "CMC Location")]
        public string CMCLocation { get; set; }

        public enum CMCLocationChoice
        {
            CT,
            CO,
            EU,
            AP
        }

        [Display(Name = "Client Reference")]
        public string ClientReference { get; set; }

        [Display(Name = "Client Entity Name")]
        public string ClientEntityName { get; set; }

        [Display(Name = "Client Currency")]
        public string ClientCurrency { get; set; }

        public enum CurrencyChoice
        {
            USD,
            EUR,
            CNY
        }

        [Display(Name = "Client UM")]
        public string ClientUM { get; set; }

        public enum UMChoice
        {
            KG,
            LB,
        }

        [Display(Name = "Client Net Term")]
        public string ClientNetTerm { get; set; }

        [Display(Name = "MSDS")]
        public bool MSDS { get; set; }

        [Display(Name = "MSDS File Directory")]
        public string MSDSFileDir { get; set; }

        [Display(Name = "TDS")]
        public bool TDS { get; set; }

        [Display(Name = "TDS File Directory")]
        public string TDSFileDir { get; set; }

        [Display(Name = "COA")]
        public bool COA { get; set; }

        [Display(Name = "COA File Dir")]
        public string COAFileDir { get; set; }

        [Display(Name = "BOL Comment")]
        public string BOLComment { get; set; }

        [Display(Name = "Logo File Directory")]
        public string LogoFileDir { get; set; }

        [AllowHtml]
        [Display(Name = "Invoice Address")]
        public string InvoiceAddress { get; set; }

        [AllowHtml]
        [Display(Name = "Email Invoice To")]
        public string InvoiceEmailTo { get; set; }

        [AllowHtml]
        [Display(Name = "Key Contact Directory")]
        public string KeyContactDir { get; set; }

        [AllowHtml]
        [Display(Name = "Client Contact Information")]
        public string ClientContact { get; set; }

        [AllowHtml]
        [Display(Name = "CMC Contact Information")]
        public string CMCContact { get; set; }

        [Display(Name = "Client Website")]
        public string ClientUrl { get; set; }

        [AllowHtml]
        [Display(Name = "Product Setup Details")]
        public string ProductSetupDetails { get; set; }

        [AllowHtml]
        [Display(Name = "SDS Language(s)")]
        public string SDSLanguage { get; set; }

        [AllowHtml]
        [Display(Name = "SDS Update Method")]
        public string SDSUpdateMethod { get; set; }

        [Display(Name = "Label Language(s)")]
        public string LabelLanguage { get; set; }

        [Display(Name = "SDS Required")]
        public bool SDSRequired { get; set; }

        [Display(Name = "COA Required")]
        public bool COARequired { get; set; }

        [Display(Name = "TDS Required")]
        public bool TDSRequired { get; set; }

        [Display(Name = "Std Cover Letter Required")]
        public bool StdCoverLetterRequired { get; set; }

        [AllowHtml]
        [Display(Name = "Inventory Reports")]
        public string InventoryReports { get; set; }

        [Display(Name = "Cease Ship Period")]
        public string CeaseShipPeriod { get; set; }

        [Display(Name = "Replenishment Lead Days")]
        public string ReplenishmentLeadDays { get; set; }

        [Display(Name = "Expiration Date On Label")]
        public bool ExpDateOnLabel { get; set; }

        [AllowHtml]
        [Display(Name = "Expiry Date Rules")]
        public string ExpDateRules { get; set; }

        [AllowHtml]
        [Display(Name = "Waste Procedure")]
        public string WasteProcedure { get; set; }

        [AllowHtml]
        [Display(Name = "Inventory Business Rules")]
        public string InventoryBusinessRules { get; set; }

        [AllowHtml]
        [Display(Name = "Special Request Allowed")]
        public string SpecialReqAllowed { get; set; }

        [Display(Name = "Commercial Invoice Value")]
        public string CommercialInvoiceValue { get; set; }

        [AllowHtml]
        [Display(Name = "Freezable Procedure")]
        public string FreezableProcedure { get; set; }

        [Display(Name = "Survey Used")]
        public bool SurveyUsed { get; set; }

        [AllowHtml]
        [Display(Name = "Business Rules")]
        public string CSBusinessRules { get; set; }

        [AllowHtml]
        [Display(Name = "Shipping Rules")]
        public string ShippingRules { get; set; }

        [Display(Name = "Ship Confirm Email")]
        public string ShipConfirmEmail { get; set; }

        [Display(Name = "Delay Confirm Email")]
        public string DelayConfirmEmail { get; set; }

        [Display(Name = "Order Confirm Email")]
        public string OrderConfirmEmail { get; set; }

        [Display(Name = "Invoice Segregation")]
        public string InvoiceSegregation { get; set; }

        [AllowHtml]
        [Display(Name = "Charges Summary")]
        public string ChargesSummary { get; set; }

        [Display(Name = "Partial Delivery Allowed")]
        public string PartialDeliveryAllowed { get; set; }

        [Display(Name = "Client Status")]
        public string ClientStatus { get; set; }

        [Display(Name = "Active Date")]
        public DateTimeOffset? ActiveDate { get; set; }

        [Display(Name = "Surchage Hazard")]
        public int? SurchageHazard { get; set; }

        [Display(Name = "Surcharge Flammable")]
        public int? SurchargeFlammable { get; set; }

        [Display(Name = "Surcharge Heat")]
        public int? SurchargeHeat { get; set; }

        [Display(Name = "Surcharge Refrigerate")]
        public int? SurchargeRefrigerate { get; set; }

        [Display(Name = "Surcharge Freeze")]
        public int? SurchargeFreeze { get; set; }

        [Display(Name = "Surcharge Clean")]
        public int? SurchargeClean { get; set; }

        [Display(Name = "Surcharge Nalgene")]
        public int? SurchargeNalgene { get; set; }

        [Display(Name = "Surcharge Nitrogen")]
        public int? SurchargeNitrogen { get; set; }

        [Display(Name = "Surcharge Biocide")]
        public int? SurchargeBiocide { get; set; }

        [Display(Name = "Surcharge Blend")]
        public int? SurchargeBlend { get; set; }

        [Display(Name = "Surcharge Kosher")]
        public int? SurchargeKosher { get; set; }

        [Display(Name = "Hazard D&H Service Level 1")]
        public int? HazDHServLvl1 { get; set; }

        [Display(Name = "Hazard D&H Service Level 2")]
        public int? HazDHServLvl2 { get; set; }

        [Display(Name = "Hazard D&H Service Level 3")]
        public int? HazDHServLvl3 { get; set; }

        [Display(Name = "Hazard D&H Service Level 4")]
        public int? HazDHServLvl4 { get; set; }

        [Display(Name = "Non-Hazard D&H Service Level 1")]
        public int? NonHazDHServLvl1 { get; set; }

        [Display(Name = "Non-Hazard D&H Service Level 2")]
        public int? NonHazDHServLvl2 { get; set; }

        [Display(Name = "Non-Hazard D&H Service Level 3")]
        public int? NonHazDHServLvl3 { get; set; }

        [Display(Name = "Non-Hazard D&H Service Level 4")]
        public int? NonHazDHServLvl4 { get; set; }
    }

    public class Division
    {
        [Display(Name = "Division ID")]
        public int DivisionID { get; set; }

        [Display(Name = "Client ID")]
        public int? ClientID { get; set; }

        [Display(Name = "Division Name")]
        public string DivisionName { get; set; }

        [Display(Name = "Business Unit")]
        public string BusinessUnit { get; set; }

        [Display(Name = "Short Name")]
        public string Abbr { get; set; }

        [Display(Name = "Waste Rate Off Spec")]
        public decimal? WasteRateOffSpec { get; set; }

        [Display(Name = "Waste Rate Empty")]
        public decimal? WasteRateEmpty { get; set; }

        [Display(Name = "Inactive")]
        public bool? Inactive { get; set; }

        [Display(Name = "Legacy Code")]
        public int? LegacyID { get; set; }

        [Display(Name = "Label Contact Name")]
        public string ContactLabelName { get; set; }

        [Display(Name = "Label Contact Phone")]
        public string ContactLabelPhone { get; set; }

        [Display(Name = "MSDS Contact Name")]
        public string ContactMSDSName { get; set; }

        [Display(Name = "MSDS Contact Phone")]
        public string ContactMSDSPhone { get; set; }

        [Display(Name = "Emergency Phone")]
        public string EmergencyNumber { get; set; }

        [Display(Name = "UPS Hazard Book")]
        public string UPSHazBook { get; set; }

        [Display(Name = "Ext MSDS")]
        public string ExtMSDS { get; set; }

        [Display(Name = "External Label")]
        public string ExtLabel { get; set; }

        [Display(Name = "Main Contact Name")]
        public string MainContactName { get; set; }

        [Display(Name = "Main Contact Phone")]
        public string MainContactNumber { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Street 1")]
        public string CompanyStreet1 { get; set; }

        [Display(Name = "Street 2")]
        public string CompanyStreet2 { get; set; }

        [Display(Name = "Street 3")]
        public string CompanyStreet3 { get; set; }

        [Display(Name = "Postal Code")]
        public string CompanyPostalCode { get; set; }

        [Display(Name = "City")]
        public string CompanyCity { get; set; }

        [Display(Name = "Country")]
        public string CompanyCountry { get; set; }

        [Display(Name = "Phone")]
        public string CompanyTelephone { get; set; }

        [Display(Name = "Fax")]
        public string CompanyFax { get; set; }

        [Display(Name = "Emergency Phone")]
        public string CompanyEmergencyTelephone { get; set; }

        [Display(Name = "Email")]
        public string CompanyEmail { get; set; }

        [Display(Name = "Website")]
        public string CompanyWebsite { get; set; }

        [Display(Name = "Logo")]
        public string CompanyLogo { get; set; }

        [Display(Name = "Logo 2")]
        public string CompanyLogo2 { get; set; }

        [Display(Name = "Include on Label")]
        public bool? IncludeExpDtOnLabel { get; set; }

        public string Location_MDB { get; set; }
        public string Company_MDB { get; set; }
    }

    public class Supplier
    {
        [Display(Name = "Supplier ID")]
        public int SupplierID { get; set; }

        [Display(Name = "Client ID")]
        public int? ClientID { get; set; }

        [Display(Name = "Supplier Code")]
        public string SupplierCode { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        public List<SelectListItem> ListOfCountries { get; set; }
    }

    public class Tier
    {
        public int TierID { get; set; }
        public int? ClientID { get; set; }
        public string TierLevel { get; set; }
        public string Size { get; set; }
        public decimal? LoSampAmt { get; set; }
        public decimal? HiSampAmt { get; set; }
        public decimal? Price { get; set; }
    }
}