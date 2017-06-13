using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class InvoiceViewModel
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? InvoiceStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? InvoiceEndDate { get; set; }
		
        public int InvoiceId { get; set; }
        public int? InvoiceNumber { get; set; }
        public string BillingGroup { get; set; }
        public string WarehouseLocation { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool? VerifiedAccuracy { get; set; }
        public string VerifiedBy { get; set; }
        public DateTimeOffset? VerifiedDate { get; set; }
        public string Status { get; set; }                          // choices: NEW / REVIEWED / VERIFIED / EMAILED / CLOSED
        public DateTimeOffset? InvoiceDate { get; set; }
        public string InvoicePeriod { get; set; }
        public string PONumber { get; set; }
        public string NetTerm { get; set; }
        public string Comments { get; set; }

        [AllowHtml]
        public string BillTo { get; set; }

        [AllowHtml]
        public string RemitTo { get; set; }

        public string Currency { get; set; }
        public int? Tier { get; set; }
        public string OrderType { get; set; }                       //Sample(domestic)/international/Revenue does not apply to all clients

        /////////////////////////////////////////////
        // Shipping Performance
        /////////////////////////////////////////////
        
        public int? SampleShipSameDay { get; set; }
        public int? SampleShipNextDay { get; set; }
        public int? SampleShipSecondDay { get; set; }
        public int? SampleShipOther { get; set; }

        /////////////////////////////////////////////
        // Master Invoice Summary
        /////////////////////////////////////////////
        
        public int? TotalSamples { get; set; }
        public decimal? TotalCostSamples { get; set; }
        public decimal? TotalFreight { get; set; }
        public decimal? TotalFrtHzdSchg { get; set; }
        public decimal? TotalAdminCharge { get; set; }
        public decimal? TotalDue { get; set; }

        /////////////////////////////////////////////
        #region// Billing Worksheet
        /////////////////////////////////////////////
        
        public decimal? GrandTotalCharge { get; set; }

        // Quantity
        public int? AirHzdOnlyQuantity { get; set; }
        public int? CertificateOfOriginQuantity { get; set; }
        public int? CMCPackQuantity { get; set; }
        public int? CoolPackQuantity { get; set; }
        public int? CreditCardFeeQuantity { get; set; }
        public int? CreditCardOrderQuantity { get; set; }
        public int? DocumentationHandlingQuantity { get; set; }
        public int? EmptyPackagingQuantity { get; set; }
        public int? ExternalSystemQuantity { get; set; }
        public int? FollowUpOrderQuantity { get; set; }
        public int? FreezerPackQuantity { get; set; }
        public int? GHSLabelsQuantity { get; set; }
        public int? InactiveProductsQuantity { get; set; }
        public int? IsolationQuantity { get; set; }
        public int? IsolationBoxQuantity { get; set; }
        public int? ITFeeQuantity { get; set; }
        public int? LabelMaintainanceQuantity { get; set; }
        public int? LabelStockQuantity { get; set; }
        public int? LabelsPrintedQuantity { get; set; }
        public int? LaborRelabelQuantity { get; set; }
        public int? LiteratureFeeQuantity { get; set; }
        public int? LimitedQtyQuantity { get; set; }
        public int? ManualHandlingQuantity { get; set; }
        public int? MSDSPrintsQuantity { get; set; }
        public int? NewLabelSetupQuantity { get; set; }
        public int? NewProductSetupQuantity { get; set; }
        public int? OberkPackQuantity { get; set; }
        public int? OrderEntryQuantity { get; set; }
        public int? OverPackQuantity { get; set; }
        public int? PalletReturnQuantity { get; set; }
        public int? PoisonPackQuantity { get; set; }
        public int? ProductSetupChangesQuantity { get; set; }
        public int? QCStorageQuantity { get; set; }
        public int? RDHandlingADRQuantity { get; set; }
        public int? RDHandlingIATAQuantity { get; set; }
        public int? RDHandlingLQQuantity { get; set; }
        public int? RDHandlingNonHzdQuantity { get; set; }
        public int? RefrigeratorStorageQuantity { get; set; }
        public int? RelabelsQuantity { get; set; }
        public int? RushShipmentQuantity { get; set; }
        public int? SPA197AppliedQuantity { get; set; }
        public int? SPSPaidOrderQuantity { get; set; }
        public int? UNBoxQuantity { get; set; }
        public int? WarehouseStorageQuantity { get; set; }
        public int? WHMISLabelsQuantity { get; set; }
        
        // Rate
        public decimal? AirHzdOnlyRate { get; set; }
        public decimal? CertificateOfOriginRate { get; set; }
        public decimal? CMCPackRate { get; set; }
        public decimal? CoolPackRate { get; set; }
        public decimal? CreditCardFeeRate { get; set; }
        public decimal? CreditCardOrderRate { get; set; }
        public decimal? DocumentationHandlingRate { get; set; }
        public decimal? EmptyPackagingRate { get; set; }
        public decimal? ExternalSystemRate { get; set; }
        public decimal? FollowUpOrderRate { get; set; }
        public decimal? FreezerPackRate { get; set; }
        public decimal? GHSLabelsRate { get; set; }
        public decimal? InactiveProductsRate { get; set; }
        public decimal? IsolationRate { get; set; }
        public decimal? IsolationBoxRate { get; set; }
        public decimal? ITFeeRate { get; set; }
        public decimal? LabelMaintainanceRate { get; set; }
        public decimal? LabelStockRate { get; set; }
        public decimal? LabelsPrintedRate { get; set; }
        public decimal? LaborRelabelRate { get; set; }
        public decimal? LiteratureFeeRate { get; set; }
        public decimal? LimitedQtyRate { get; set; }
        public decimal? ManualHandlingRate { get; set; }
        public decimal? MSDSPrintsRate { get; set; }
        public decimal? NewLabelSetupRate { get; set; }
        public decimal? NewProductSetupRate { get; set; }
        public decimal? OberkPackRate { get; set; }
        public decimal? OrderEntryRate { get; set; }
        public decimal? OverPackRate { get; set; }
        public decimal? PalletReturnRate { get; set; }
        public decimal? PoisonPackRate { get; set; }
        public decimal? ProductSetupChangesRate { get; set; }
        public decimal? QCStorageRate { get; set; }
        public decimal? RDHandlingADRRate { get; set; }
        public decimal? RDHandlingIATARate { get; set; }
        public decimal? RDHandlingLQRate { get; set; }
        public decimal? RDHandlingNonHzdRate { get; set; }
        public decimal? RefrigeratorStorageRate { get; set; }
        public decimal? RelabelsRate { get; set; }
        public decimal? RushShipmentRate { get; set; }
        public decimal? SPA197AppliedRate { get; set; }
        public decimal? SPSPaidOrderRate { get; set; }
        public decimal? UNBoxRate { get; set; }
        public decimal? WarehouseStorageRate { get; set; }
        public decimal? WHMISLabelsRate { get; set; }

        // Charges
        public decimal? AirHzdOnlyCharge { get; set; }
        public decimal? CertificateOfOriginCharge { get; set; }
        public decimal? CMCPackCharge { get; set; }
        public decimal? CoolPackCharge { get; set; }
        public decimal? CreditCardFeeCharge { get; set; }
        public decimal? CreditCardOrderCharge { get; set; }
        public decimal? DocumentationHandlingCharge { get; set; }
        public decimal? EmptyPackagingCharge { get; set; }
        public decimal? ExternalSystemCharge { get; set; }
        public decimal? FollowUpOrderCharge { get; set; }
        public decimal? FreezerPackCharge { get; set; }
        public decimal? GHSLabelsCharge { get; set; }
        public decimal? InactiveProductsCharge { get; set; }
        public decimal? IsolationCharge { get; set; }
        public decimal? IsolationBoxCharge { get; set; }
        public decimal? ITFeeCharge { get; set; }
        public decimal? LabelMaintainanceCharge { get; set; }
        public decimal? LabelStockCharge { get; set; }
        public decimal? LabelsPrintedCharge { get; set; }
        public decimal? LaborRelabelCharge { get; set; }
        public decimal? LiteratureFeeCharge { get; set; }
        public decimal? LimitedQtyCharge { get; set; }
        public decimal? ManualHandlingCharge { get; set; }
        public decimal? MSDSPrintsCharge { get; set; }
        public decimal? NewLabelSetupCharge { get; set; }
        public decimal? NewProductSetupCharge { get; set; }
        public decimal? OberkPackCharge { get; set; }
        public decimal? OrderEntryCharge { get; set; }
        public decimal? OverPackCharge { get; set; }
        public decimal? PalletReturnCharge { get; set; }
        public decimal? PoisonPackCharge { get; set; }
        public decimal? ProductSetupChangesCharge { get; set; }
        public decimal? QCStorageCharge { get; set; }
        public decimal? RDHandlingADRCharge { get; set; }
        public decimal? RDHandlingIATACharge { get; set; }
        public decimal? RDHandlingLQCharge { get; set; }
        public decimal? RDHandlingNonHzdCharge { get; set; }
        public decimal? RefrigeratorStorageCharge { get; set; }
        public decimal? RelabelsCharge { get; set; }
        public decimal? RushShipmentCharge { get; set; }
        public decimal? SPA197AppliedCharge { get; set; }
        public decimal? SPSPaidOrderCharge { get; set; }
        public decimal? UNBoxCharge { get; set; }
        public decimal? WarehouseStorageCharge { get; set; }
        public decimal? WHMISLabelsCharge { get; set; }
        
        // Variable Charges manually entered with no prior rates in system
        public decimal? AdministrativeWasteFeeCharge { get; set; }
        public decimal? CreditCharge { get; set; }
        public decimal? CustomsDocumentsCharge { get; set; }
        public decimal? DeliveryDutiesTaxesCharge { get; set; }
        public decimal? DocumentsCharge { get; set; }
        public decimal? HandlingCharge { get; set; }
        public decimal? MautFuel { get; set; }
        public decimal? MiscellaneousLaborCharge { get; set; }
        public decimal? OtherCharge { get; set; }
        public decimal? WasteProcessingCharge { get; set; }

        #endregion
    }
}