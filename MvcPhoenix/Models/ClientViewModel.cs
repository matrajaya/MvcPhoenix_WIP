using System;
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
        public string ClientReference { get; set; }
        public string ClientEntityName { get; set; }
        public string ClientCurrency { get; set; }
        public string ClientUM { get; set; }
        public string ClientNetTerm { get; set; }

        [AllowHtml]
        public string InvoiceAddress { get; set; }

        [AllowHtml]
        public string InvoiceEmailTo { get; set; }

        [AllowHtml]
        public string KeyContactDir { get; set; }

        public byte[] LogoFile { get; set; }
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
        public bool? IncludeExpDtOnLabel { get; set; }
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
    }

    public class Supplier
    {
        public int BulkSupplierID { get; set; }
        public int? ClientID { get; set; }
        public string SupplyID { get; set; }
        public string SupplierCode { get; set; }                    // ShortName
        public string SupplierName { get; set; }                    // CompanyName
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }                      // Zip
        public string Country { get; set; }
    }

    public class Contact
    {
        public int ClientContactID { get; set; }
        public int? ClientID { get; set; }
        public string ContactType { get; set; }
        public string Account { get; set; }
        public string FullName { get; set; }
        public string DistributorName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
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

    public class EndUse
    {
        public int EndUseID { get; set; }
        public int? ClientID { get; set; }
        public string EndUseString { get; set; }
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
        public decimal? OrderChargeCertificateOfOrigin { get; set; }
        public decimal? OrderChargeDocumentsHandling { get; set; }
        public decimal? OrderChargeManualHandling { get; set; }
        public decimal? OrderChargeRDHandlingNone { get; set; }
        public decimal? OrderChargeRDHandlingLQ { get; set; }
        public decimal? OrderChargeRDHandlingADR { get; set; }
        public decimal? OrderChargeRDHandlingIATA { get; set; }
        public decimal? OrderChargeSPSPaidOrder { get; set; }
        public decimal? OrderChargeBPCSChange { get; set; }
        public decimal? OrderChargeCreditCardFee { get; set; }
        public decimal? OrderChargeCreditCardOrder { get; set; }
        public decimal? OrderChargeOrderEntry { get; set; }
        public decimal? PPSChargeCoolPack { get; set; }
        public decimal? PPSChargeIsolation { get; set; }
        public decimal? PPSChargeIsolationBox { get; set; }
        public decimal? PPSChargeUNBox { get; set; }
        public decimal? CarrierChargeRushShipment { get; set; }
    }

    public class AdministativeRates
    {
        public int RatesID { get; set; }
        public int ClientID { get; set; }
        public decimal? EmptyPackagingRate { get; set; }
        public decimal? HandlingRate { get; set; }
        public decimal? InactiveProductsRate { get; set; }
        public decimal? ProductSetupChangesRate { get; set; }
        public decimal? MiscellaneousLaborRate { get; set; }
        public decimal? FollowUpOrderRate { get; set; }
        public decimal? RefrigeratorStorageRate { get; set; }
        public decimal? GHSLabelsRate { get; set; }
        public decimal? ITFeeRate { get; set; }
        public decimal? LabelsPrintedRate { get; set; }
        public decimal? LaborRelabelRate { get; set; }
        public decimal? LiteratureRate { get; set; }
        public decimal? LabelStockRate { get; set; }
        public decimal? LabelMaintainanceRate { get; set; }
        public decimal? MSDSPrintsRate { get; set; }
        public decimal? NewLabelSetupRate { get; set; }
        public decimal? NewProductSetupRate { get; set; }
        public decimal? OtherRate { get; set; }
        public decimal? PalletReturnRate { get; set; }
        public decimal? QCStorageRate { get; set; }
        public decimal? RelabelsRate { get; set; }
        public decimal? WarehouseStorageRate { get; set; }
        public decimal? WHMISLabelsRate { get; set; }
        public decimal? WasteProcessingRate { get; set; }
    }
}