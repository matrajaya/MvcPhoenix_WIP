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

        // Charges
        public decimal? EmptyPackagingCharge { get; set; }
        public decimal? HandlingCharge { get; set; }
        public decimal? InactiveProductsCharge { get; set; }
        public decimal? ProductSetupChangesCharge { get; set; }
        public decimal? MiscellaneousLaborCharge { get; set; }
        public decimal? FollowUpOrderCharge { get; set; }
        public decimal? RefrigeratorStorageCharge { get; set; }
        public decimal? GHSLabelsCharge { get; set; }
        public decimal? ITFeeCharge { get; set; }
        public decimal? LabelsPrintedCharge { get; set; }
        public decimal? LaborRelabelCharge { get; set; }
        public decimal? LiteratureCharge { get; set; }
        public decimal? LabelStockCharge { get; set; }
        public decimal? LabelMaintainanceCharge { get; set; }
        public decimal? MSDSPrintsCharge { get; set; }
        public decimal? NewLabelSetupCharge { get; set; }
        public decimal? NewProductSetupCharge { get; set; }
        public decimal? OtherCharge { get; set; }
        public decimal? PalletReturnCharge { get; set; }
        public decimal? QCStorageCharge { get; set; }
        public decimal? RelabelsCharge { get; set; }
        public decimal? WarehouseStorageCharge { get; set; }
        public decimal? WHMISLabelsCharge { get; set; }
        public decimal? WasteProcessingCharge { get; set; }
        public decimal? MinimumInvoiceCharge { get; set; }                     // TODO: Add to tblInvoice later: hold for now
        
        // Rate
        public decimal? EmptyPackagingRate  { get; set; }
        public decimal? HandlingRate  { get; set; }
        public decimal? InactiveProductsRate  { get; set; }
        public decimal? ProductSetupChangesRate  { get; set; }
        public decimal? MiscellaneousLaborRate  { get; set; }
        public decimal? FollowUpOrderRate  { get; set; }
        public decimal? RefrigeratorStorageRate  { get; set; }
        public decimal? GHSLabelsRate  { get; set; }
        public decimal? ITFeeRate  { get; set; }
        public decimal? LabelsPrintedRate  { get; set; }
        public decimal? LaborRelabelRate  { get; set; }
        public decimal? LiteratureRate  { get; set; }
        public decimal? LabelStockRate  { get; set; }
        public decimal? LabelMaintainanceRate  { get; set; }
        public decimal? MSDSPrintsRate  { get; set; }
        public decimal? NewLabelSetupRate  { get; set; }
        public decimal? NewProductSetupRate  { get; set; }
        public decimal? OtherRate  { get; set; }
        public decimal? PalletReturnRate  { get; set; }
        public decimal? QCStorageRate  { get; set; }
        public decimal? RelabelsRate  { get; set; }
        public decimal? WarehouseStorageRate  { get; set; }
        public decimal? WHMISLabelsRate  { get; set; }
        public decimal? WasteProcessingRate  { get; set; }

        // Quantity
        public int? EmptyPackagingQuantity  { get; set; }
        public int? HandlingQuantity  { get; set; }
        public int? InactiveProductsQuantity  { get; set; }
        public int? ProductSetupChangesQuantity  { get; set; }
        public int? MiscellaneousLaborQuantity  { get; set; }
        public int? FollowUpOrderQuantity  { get; set; }
        public int? RefrigeratorStorageQuantity  { get; set; }
        public int? GHSLabelsQuantity  { get; set; }
        public int? ITFeeQuantity  { get; set; }
        public int? LabelsPrintedQuantity  { get; set; }
        public int? LaborRelabelQuantity  { get; set; }
        public int? LiteratureQuantity  { get; set; }
        public int? LabelStockQuantity  { get; set; }
        public int? LabelMaintainanceQuantity  { get; set; }
        public int? MSDSPrintsQuantity  { get; set; }
        public int? NewLabelSetupQuantity  { get; set; }
        public int? NewProductSetupQuantity  { get; set; }
        public int? OtherQuantity  { get; set; }
        public int? PalletReturnQuantity  { get; set; }
        public int? QCStorageQuantity  { get; set; }
        public int? RelabelsQuantity  { get; set; }
        public int? WarehouseStorageQuantity  { get; set; }
        public int? WHMISLabelsQuantity  { get; set; }
        public int? WasteProcessingQuantity  { get; set; }
                
        #endregion
    }
}