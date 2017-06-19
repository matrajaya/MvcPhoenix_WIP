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
        public string ERProvider { get; set; }
        public string ERRegistrant { get; set; }
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
    }

    public class ServiceChargeRates
    {
        public int RatesID { get; set; }
        public int ClientID { get; set; }
        public decimal? AirHazardOnly { get; set; }
        public decimal? CertificateOfOrigin { get; set; }
        public decimal? CMCPack { get; set; }
        public decimal? CoolPack { get; set; }
        public decimal? CreditCardFee { get; set; }
        public decimal? CreditCardOrder { get; set; }
        public decimal? DocumentHandling { get; set; }
        public decimal? EmptyPackaging { get; set; }
        public decimal? ExternalSystem { get; set; }
        public decimal? FollowUpOrder { get; set; }
        public decimal? FreezerPack { get; set; }
        public decimal? GHSLabels { get; set; }
        public decimal? InactiveProducts { get; set; }
        public decimal? Isolation { get; set; }
        public decimal? IsolationBox { get; set; }
        public decimal? ITFee { get; set; }
        public decimal? LabelMaintainance { get; set; }
        public decimal? LabelStock { get; set; }
        public decimal? LabelsPrinted { get; set; }
        public decimal? LaborRelabel { get; set; }
        public decimal? LiteratureFee { get; set; }
        public decimal? LimitedQuantity { get; set; }
        public decimal? ManualHandling { get; set; }
        public decimal? MSDSPrints { get; set; }
        public decimal? NewLabelSetup { get; set; }
        public decimal? NewProductSetup { get; set; }
        public decimal? OberkPack { get; set; }
        public decimal? OrderEntry { get; set; }
        public decimal? OverPack { get; set; }
        public decimal? PalletReturn { get; set; }
        public decimal? PoisonPack { get; set; }
        public decimal? ProductSetupChanges { get; set; }
        public decimal? QCStorage { get; set; }
        public decimal? RDHandlingADR { get; set; }
        public decimal? RDHandlingIATA { get; set; }
        public decimal? RDHandlingLQ { get; set; }
        public decimal? RDHandlingNonHazard { get; set; }
        public decimal? RefrigeratorStorage { get; set; }
        public decimal? Relabels { get; set; }
        public decimal? RushShipment { get; set; }
        public decimal? SPA197Applied { get; set; }
        public decimal? SPSPaidOrder { get; set; }
        public decimal? UNBox { get; set; }
        public decimal? WarehouseStorage { get; set; }
        public decimal? WHMISLabels { get; set; }
    }
}