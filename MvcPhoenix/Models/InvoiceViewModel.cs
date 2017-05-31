using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class InvoiceViewModel
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? invoicestartdate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? invoiceenddate { get; set; }
		
        public int invoiceid { get; set; }
        public int? invoicenumber { get; set; }
        public string billinggroup { get; set; }
        public string warehouselocation { get; set; }
        public int? clientid { get; set; }
        public string clientname { get; set; }
        public string createdby { get; set; }
        public DateTimeOffset? createdate { get; set; }
        public string updatedby { get; set; }
        public DateTimeOffset? updatedate { get; set; }
        public bool? verifiedaccuracy { get; set; }
        public string verifiedby { get; set; }
        public DateTimeOffset? verifieddate { get; set; }
        public string status { get; set; }                          // choices: NEW / REVIEWED / VERIFIED / EMAILED / CLOSED
        public DateTimeOffset? invoicedate { get; set; }
        public string invoiceperiod { get; set; }
        public string ponumber { get; set; }
        public string netterm { get; set; }

        [AllowHtml]
        public string billto { get; set; }

        [AllowHtml]
        public string remitto { get; set; }

        public string currency { get; set; }
        public int? tier { get; set; }
        public string ordertype { get; set; }                       //Sample(domestic)/international/Revenue does not apply to all clients

        /////////////////////////////////////////////
        // Shipping Performance
        /////////////////////////////////////////////
        
        public int? sampleshipsameday { get; set; }
        public int? sampleshipnextday { get; set; }
        public int? sampleshipsecondday { get; set; }
        public int? sampleshipother { get; set; }

        /////////////////////////////////////////////
        // Master Invoice Summary
        /////////////////////////////////////////////
        
        public int? totalsamples { get; set; }
        public decimal? totalcostsamples { get; set; }
        public decimal? totalfreight { get; set; }
        public decimal? totalfrtHzdSchg { get; set; }
        public decimal? totaladmincharge { get; set; }
        public decimal? totaldue { get; set; }

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