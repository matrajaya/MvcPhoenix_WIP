using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OrderMasterFull
    {
        public int OrderID { get; set; }

        [Required]
        public int? ClientId { get; set; }

        [Required]
        public int? DivisionId { get; set; }

        public string ClientName { get; set; }
        public string ClientCode { get; set; }
        public string LogoFileName { get; set; }
        public string OrderStatus { get; set; }
        public string Customer { get; set; }
        public int CMCOrder { get; set; }                              // Legacy; remove later
        public int WebOrderId { get; set; }                            // Legacy; remove later
        public string CMCLegacyNumber { get; set; }
        public string ClientOrderNumber { get; set; }
        public string ClientSAPNumber { get; set; }
        public string ClientRefNumber { get; set; }
        public string OrderType { get; set; }
        public DateTime? OrderDate { get; set; }

        [Required(ErrorMessage = "Ship To Name is required")]
        public string Company { get; set; }

        public string Street { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Attention { get; set; }
        public string Email { get; set; }
        public string CustomsRefNumber { get; set; }
        public string RequestorName { get; set; }
        public string RequestorPhone { get; set; }
        public string RequestorFax { get; set; }
        public string RequestorEmail { get; set; }
        public string SalesRepName { get; set; }
        public string SalesRepEmail { get; set; }
        public string SalesRepPhone { get; set; }
        public string SalesRepTerritory { get; set; }
        public string MarketingRep { get; set; }
        public string MarketingRepEmail { get; set; }
        public string Distributor { get; set; }
        public string EndUse { get; set; }
        public string ShipVia { get; set; }
        public string ShipAcct { get; set; }
        public string Phone { get; set; }
        public string Source { get; set; }                              // Change "source" to ordersource and add "clientsource" to tblOrderMaster.
        public string OrderSource { get; set; }
        public string ClientSource { get; set; }                        // Transfer legacy data from source to client source
        public string Fax { get; set; }
        public string Tracking { get; set; }                            // Legacy Only
        public string Special { get; set; }
        public string SpecialInternal { get; set; }
        public bool IsLiterature { get; set; }
        public string Region { get; set; }
        public bool COA { get; set; }
        public bool TDS { get; set; }
        public string CID { get; set; }
        public string ClientAcct { get; set; }
        public string ACode { get; set; }
        public string ImportFile { get; set; }
        public DateTime? ImportDateLine { get; set; }
        public string Timing { get; set; }
        public string Volume { get; set; }
        public bool SampleRack { get; set; }
        public string CMCUser { get; set; }
        public string ClientReference { get; set; }
        public int? TotalOrderWeight { get; set; }
        public string ClientOrderType { get; set; }
        public DateTime? ClientRequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? RequestedDeliveryDate { get; set; }
        public int ClientTotalItems { get; set; }
        public string ClientRequestedCarrier { get; set; }
        public int LegacyId { get; set; }
        public string PreferredCarrier { get; set; }
        public bool ApprovalNeeded { get; set; }
        public bool? IsSDN { get; set; }
        public bool? IsSDNOverride { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }

        public int ItemsCount { get; set; }
        public int TransCount { get; set; }
        public int NeedAllocationCount { get; set; }
    }

    public class OrderSPSBilling
    {
        public int SPSBillingID { get; set; }
        public int OrderId { get; set; }
        public string Type { get; set; }                                // CC or Invoice
        public string TaxId { get; set; }
        public string Currency { get; set; }                            // USD or EUR or CNY ...necessary?
        public decimal? PriceCost { get; set; }                         // Sum of SPSCharge in Order Item
        public decimal? FreightCost { get; set; }
        public decimal? ShippedWeight { get; set; }
        public string InvoiceTitle { get; set; }
        public string InvoiceFirstName { get; set; }
        public string InvoiceLastName { get; set; }
        public string InvoiceCompany { get; set; }
        public string InvoiceAddress1 { get; set; }
        public string InvoiceAddress2 { get; set; }
        public string InvoiceAddress3 { get; set; }
        public string InvoiceCity { get; set; }
        public string InvoiceState { get; set; }
        public string InvoicePostalCode { get; set; }
        public string InvoiceCountry { get; set; }
        public string InvoicePhone { get; set; }
        public string InvoiceEmail { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class OrderImportFile
    {
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
        public string Client { get; set; }
        public IEnumerable<SelectListItem> Clients { get; set; }
    }

    public class PreferredCarrierViewModel
    {
        public int ID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public bool? CommInvoiceReq { get; set; }
        public string NonHazSm { get; set; }
        public string NonHazLg { get; set; }
        public string NonHazIncoTerms { get; set; }
        public string HazIATASm { get; set; }
        public string HazIATALg { get; set; }
        public string HazGroundLQ { get; set; }
        public string HazGround { get; set; }
        public string HazIncoterms { get; set; }
        public string IncotermsAlt { get; set; }
        public string NotesGeneral { get; set; }
        public string NotesIATAADR { get; set; }
        public string NonHazIncotermsAlt { get; set; }
        public string HazIncotermsAlt { get; set; }
    }

    public class OrderInventoryLog
    {
        public string LogType { get; set; }
        public int? BulkId { get; set; }
        public int? StockId { get; set; }
        public int? ProductMasterId { get; set; }
        public int? ProductDetailId { get; set; }
        public int? LogQty { get; set; }
        public decimal? LogAmount { get; set; }
        public string UM { get; set; }
        public string LogNotes { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public string Status { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string MasterCode { get; set; }
        public string MasterName { get; set; }
        public string Warehouse { get; set; }
        public string Size { get; set; }
        public string BulkBin { get; set; }
        public string ShelfBin { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ShipDate { get; set; }
        public int? OrderNumber { get; set; }
        public int? CurrentQtyAvailable { get; set; }
        public decimal? CurrentWeightAvailable { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CeaseShipDate { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? QCDate { get; set; }
        public int? PackOutId { get; set; }
    }
}