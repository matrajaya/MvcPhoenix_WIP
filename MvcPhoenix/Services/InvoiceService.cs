using MvcPhoenix.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    public class InvoiceService
    {
        public static List<InvoiceViewModel> IndexList()
        {
            using (var db = new CMCSQL03Entities())
            {
                var obj = (from t in db.tblInvoice
                           orderby t.CreateDate descending
                           select new InvoiceViewModel
                           {
                               InvoiceId = t.InvoiceID,
                               InvoiceNumber = t.InvoiceNumber,
                               ClientName = t.ClientName,
                               BillingGroup = t.BillingGroup,
                               InvoiceDate = t.InvoiceDate,
                               InvoicePeriod = t.InvoicePeriod,
                               WarehouseLocation = t.WarehouseLocation,
                               Status = t.Status,
                               Comments = t.Comments,
                               UpdateDate = t.UpdateDate,
                               UpdatedBy = t.UpdatedBy,
                               CreateDate = t.CreateDate,
                               CreatedBy = t.CreatedBy
                           }).ToList();

                return obj;
            }
        }

        public static InvoiceViewModel FillInvoice(int id)
        {
            using (var db = new CMCSQL03Entities())
            {
                InvoiceViewModel obj = new InvoiceViewModel();

                var q = (from t in db.tblInvoice
                         where t.InvoiceID == id
                         select t).FirstOrDefault();

                obj.InvoiceId = q.InvoiceID;
                obj.InvoiceNumber = q.InvoiceNumber;
                obj.BillingGroup = q.BillingGroup;
                obj.WarehouseLocation = q.WarehouseLocation;
                obj.ClientId = q.ClientID;
                obj.ClientName = q.ClientName;
                obj.CreatedBy = q.CreatedBy;
                obj.CreateDate = q.CreateDate;
                obj.UpdatedBy = q.UpdatedBy;
                obj.UpdateDate = q.UpdateDate;
                obj.VerifiedAccuracy = q.VerifiedAccuracy;
                obj.VerifiedBy = q.VerifiedBy;
                obj.VerifiedDate = q.VerifyDate;
                obj.Status = q.Status;
                obj.Comments = q.Comments;
                obj.InvoiceDate = q.InvoiceDate;
                obj.InvoicePeriod = q.InvoicePeriod;
                obj.InvoiceStartDate = q.InvoiceStartDate;
                obj.InvoiceEndDate = q.InvoiceEndDate;
                obj.PONumber = q.PONumber;
                obj.NetTerm = q.NetTerm;
                obj.BillTo = q.BillTo;
                obj.RemitTo = q.RemitTo;
                obj.Currency = q.Currency;
                obj.Tier = q.Tier;
                obj.OrderType = q.OrderType;                // Sample/International/Revenue - ignore for now

                // Shipping Performance
                obj.SampleShipSameDay = q.SampleShipSameDay;
                obj.SampleShipNextDay = q.SampleShipNextDay;
                obj.SampleShipSecondDay = q.SampleShipSecondDay;
                obj.SampleShipOther = q.SampleShipOther;

                // Invoice Summary
                obj.TotalSamples = q.TotalSamples;
                obj.TotalCostSamples = q.TotalCostSamples;
                obj.TotalFreight = q.TotalFreight;
                obj.TotalFrtHzdSchg = q.TotalFrtHzdSchg;
                obj.TotalServiceCharge = q.TotalServiceCharge;
                obj.TotalDue = q.TotalDue;

                // Billing Worksheet
                obj.GrandTotalCharge = q.GrandTotal;

                // Quantity
                obj.AirHzdOnlyQuantity = q.AirHzdOnlyQuantity;
                obj.CertificateOfOriginQuantity = q.CertificateOfOriginQuantity;
                obj.CMCPackQuantity = q.CMCPackQuantity;
                obj.CoolPackQuantity = q.CoolPackQuantity;
                obj.CreditCardFeeQuantity = q.CreditCardFeeQuantity;
                obj.CreditCardOrderQuantity = q.CreditCardOrderQuantity;
                obj.DocumentationHandlingQuantity = q.DocumentationHandlingQuantity;
                obj.EmptyPackagingQuantity = q.EmptyPackagingQuantity;
                obj.ExternalSystemQuantity = q.ExternalSystemQuantity;
                obj.FollowUpOrderQuantity = q.FollowUpOrderQuantity;
                obj.FreezerPackQuantity = q.FreezerPackQuantity;
                obj.GHSLabelsQuantity = q.GHSLabelsQuantity;
                obj.InactiveProductsQuantity = q.InactiveProductsQuantity;
                obj.IsolationQuantity = q.IsolationQuantity;
                obj.IsolationBoxQuantity = q.IsolationBoxQuantity;
                obj.ITFeeQuantity = q.ITFeeQuantity;
                obj.LabelMaintainanceQuantity = q.LabelMaintainanceQuantity;
                obj.LabelStockQuantity = q.LabelStockQuantity;
                obj.LabelsPrintedQuantity = q.LabelsPrintedQuantity;
                obj.LaborRelabelQuantity = q.LaborRelabelQuantity;
                obj.LiteratureFeeQuantity = q.LiteratureFeeQuantity;
                obj.LimitedQtyQuantity = q.LimitedQtyQuantity;
                obj.ManualHandlingQuantity = q.ManualHandlingQuantity;
                obj.MSDSPrintsQuantity = q.MSDSPrintsQuantity;
                obj.NewLabelSetupQuantity = q.NewLabelSetupQuantity;
                obj.NewProductSetupQuantity = q.NewProductSetupQuantity;
                obj.OberkPackQuantity = q.OberkPackQuantity;
                obj.OrderEntryQuantity = q.OrderEntryQuantity;
                obj.OverPackQuantity = q.OverPackQuantity;
                obj.PalletReturnQuantity = q.PalletReturnQuantity;
                obj.PoisonPackQuantity = q.PoisonPackQuantity;
                obj.ProductSetupChangesQuantity = q.ProductSetupChangesQuantity;
                obj.QCStorageQuantity = q.QCStorageQuantity;
                obj.RDHandlingADRQuantity = q.RDHandlingADRQuantity;
                obj.RDHandlingIATAQuantity = q.RDHandlingIATAQuantity;
                obj.RDHandlingLQQuantity = q.RDHandlingLQQuantity;
                obj.RDHandlingNonHzdQuantity = q.RDHandlingNonHzdQuantity;
                obj.RefrigeratorStorageQuantity = q.RefrigeratorStorageQuantity;
                obj.RelabelsQuantity = q.RelabelsQuantity;
                obj.RushShipmentQuantity = q.RushShipmentQuantity;
                obj.SPA197AppliedQuantity = q.SPA197AppliedQuantity;
                obj.SPSPaidOrderQuantity = q.SPSPaidOrderQuantity;
                obj.UNBoxQuantity = q.UNBoxQuantity;
                obj.WarehouseStorageQuantity = q.WarehouseStorageQuantity;
                obj.WHMISLabelsQuantity = q.WHMISLabelsQuantity;

                // Rates
                obj.AirHzdOnlyRate = q.AirHzdOnlyRate;
                obj.CertificateOfOriginRate = q.CertificateOfOriginRate;
                obj.CMCPackRate = q.CMCPackRate;
                obj.CoolPackRate = q.CoolPackRate;
                obj.CreditCardFeeRate = q.CreditCardFeeRate;
                obj.CreditCardOrderRate = q.CreditCardOrderRate;
                obj.DocumentationHandlingRate = q.DocumentationHandlingRate;
                obj.EmptyPackagingRate = q.EmptyPackagingRate;
                obj.ExternalSystemRate = q.ExternalSystemRate;
                obj.FollowUpOrderRate = q.FollowUpOrderRate;
                obj.FreezerPackRate = q.FreezerPackRate;
                obj.GHSLabelsRate = q.GHSLabelsRate;
                obj.InactiveProductsRate = q.InactiveProductsRate;
                obj.IsolationRate = q.IsolationRate;
                obj.IsolationBoxRate = q.IsolationBoxRate;
                obj.ITFeeRate = q.ITFeeRate;
                obj.LabelMaintainanceRate = q.LabelMaintainanceRate;
                obj.LabelStockRate = q.LabelStockRate;
                obj.LabelsPrintedRate = q.LabelsPrintedRate;
                obj.LaborRelabelRate = q.LaborRelabelRate;
                obj.LiteratureFeeRate = q.LiteratureFeeRate;
                obj.LimitedQtyRate = q.LimitedQtyRate;
                obj.ManualHandlingRate = q.ManualHandlingRate;
                obj.MSDSPrintsRate = q.MSDSPrintsRate;
                obj.NewLabelSetupRate = q.NewLabelSetupRate;
                obj.NewProductSetupRate = q.NewProductSetupRate;
                obj.OberkPackRate = q.OberkPackRate;
                obj.OrderEntryRate = q.OrderEntryRate;
                obj.OverPackRate = q.OverPackRate;
                obj.PalletReturnRate = q.PalletReturnRate;
                obj.PoisonPackRate = q.PoisonPackRate;
                obj.ProductSetupChangesRate = q.ProductSetupChangesRate;
                obj.QCStorageRate = q.QCStorageRate;
                obj.RDHandlingADRRate = q.RDHandlingADRRate;
                obj.RDHandlingIATARate = q.RDHandlingIATARate;
                obj.RDHandlingLQRate = q.RDHandlingLQRate;
                obj.RDHandlingNonHzdRate = q.RDHandlingNonHzdRate;
                obj.RefrigeratorStorageRate = q.RefrigeratorStorageRate;
                obj.RelabelsRate = q.RelabelsRate;
                obj.RushShipmentRate = q.RushShipmentRate;
                obj.SPA197AppliedRate = q.SPA197AppliedRate;
                obj.SPSPaidOrderRate = q.SPSPaidOrderRate;
                obj.UNBoxRate = q.UNBoxRate;
                obj.WarehouseStorageRate = q.WarehouseStorageRate;
                obj.WHMISLabelsRate = q.WHMISLabelsRate;

                // Charges
                obj.AirHzdOnlyCharge = q.AirHzdOnlyCharge;
                obj.CertificateOfOriginCharge = q.CertificateOfOriginCharge;
                obj.CMCPackCharge = q.CMCPackCharge;
                obj.CoolPackCharge = q.CoolPackCharge;
                obj.CreditCardFeeCharge = q.CreditCardFeeCharge;
                obj.CreditCardOrderCharge = q.CreditCardOrderCharge;
                obj.DocumentationHandlingCharge = q.DocumentationHandlingCharge;
                obj.EmptyPackagingCharge = q.EmptyPackagingCharge;
                obj.ExternalSystemCharge = q.ExternalSystemCharge;
                obj.FollowUpOrderCharge = q.FollowUpOrderCharge;
                obj.FreezerPackCharge = q.FreezerPackCharge;
                obj.GHSLabelsCharge = q.GHSLabelsCharge;
                obj.InactiveProductsCharge = q.InactiveProductsCharge;
                obj.IsolationCharge = q.IsolationCharge;
                obj.IsolationBoxCharge = q.IsolationBoxCharge;
                obj.ITFeeCharge = q.ITFeeCharge;
                obj.LabelMaintainanceCharge = q.LabelMaintainanceCharge;
                obj.LabelStockCharge = q.LabelStockCharge;
                obj.LabelsPrintedCharge = q.LabelsPrintedCharge;
                obj.LaborRelabelCharge = q.LaborRelabelCharge;
                obj.LiteratureFeeCharge = q.LiteratureFeeCharge;
                obj.LimitedQtyCharge = q.LimitedQtyCharge;
                obj.ManualHandlingCharge = q.ManualHandlingCharge;
                obj.MSDSPrintsCharge = q.MSDSPrintsCharge;
                obj.NewLabelSetupCharge = q.NewLabelSetupCharge;
                obj.NewProductSetupCharge = q.NewProductSetupCharge;
                obj.OberkPackCharge = q.OberkPackCharge;
                obj.OrderEntryCharge = q.OrderEntryCharge;
                obj.OverPackCharge = q.OverPackCharge;
                obj.PalletReturnCharge = q.PalletReturnCharge;
                obj.PoisonPackCharge = q.PoisonPackCharge;
                obj.ProductSetupChangesCharge = q.ProductSetupChangesCharge;
                obj.QCStorageCharge = q.QCStorageCharge;
                obj.RDHandlingADRCharge = q.RDHandlingADRCharge;
                obj.RDHandlingIATACharge = q.RDHandlingIATACharge;
                obj.RDHandlingLQCharge = q.RDHandlingLQCharge;
                obj.RDHandlingNonHzdCharge = q.RDHandlingNonHzdCharge;
                obj.RefrigeratorStorageCharge = q.RefrigeratorStorageCharge;
                obj.RelabelsCharge = q.RelabelsCharge;
                obj.RushShipmentCharge = q.RushShipmentCharge;
                obj.SPA197AppliedCharge = q.SPA197AppliedCharge;
                obj.SPSPaidOrderCharge = q.SPSPaidOrderCharge;
                obj.UNBoxCharge = q.UNBoxCharge;
                obj.WarehouseStorageCharge = q.WarehouseStorageCharge;
                obj.WHMISLabelsCharge = q.WHMISLabelsCharge;

                // Variable Charges
                obj.AdministrativeWasteFeeCharge = q.AdministrativeWasteFeeCharge;
                obj.CreditCharge = q.CreditCharge;
                obj.CustomsDocumentsCharge = q.CustomsDocumentsCharge;
                obj.DeliveryDutiesTaxesCharge = q.DeliveryDutiesTaxesCharge;
                obj.DocumentsCharge = q.DocumentsCharge;
                obj.HandlingCharge = q.HandlingCharge;
                obj.MautFuel = q.MautFuel;
                obj.MiscellaneousLaborCharge = q.MiscellaneousLaborCharge;
                obj.OtherCharge = q.OtherCharge;
                obj.WasteProcessingCharge = q.WasteProcessingCharge;

                return obj;
            }
        }

        public static int CreateInvoice(int client, string billinggroup, DateTime startdate, DateTime enddate)
        {
            using (var db = new CMCSQL03Entities())
            {
                var getclient = db.tblClient.Find(client);
                string period = startdate.ToString("MMMM") + ", " + startdate.Year;
                int invoiceid = NewInvoiceID();

                var obj = (from t in db.tblInvoice
                           where t.InvoiceID == invoiceid
                           select t).FirstOrDefault();

                obj.InvoiceDate = DateTime.UtcNow;
                obj.Status = "New";
                obj.CreateDate = DateTime.UtcNow;
                obj.CreatedBy = HttpContext.Current.User.Identity.Name;
                obj.Status = "NEW";
                obj.UpdateDate = obj.CreateDate;
                obj.UpdatedBy = obj.CreatedBy;
                obj.ClientID = getclient.ClientID;
                obj.ClientName = getclient.ClientName;
                obj.BillingGroup = billinggroup;                                    // division, business unit, enduse are available entries
                obj.WarehouseLocation = getclient.CMCLocation;
                obj.BillTo = getclient.InvoiceAddress;
                obj.NetTerm = String.IsNullOrEmpty(getclient.ClientNetTerm) ? "Net 30 Days" : getclient.ClientNetTerm;
                obj.Currency = getclient.ClientCurrency;
                obj.PONumber = "Enter PO Number";
                obj.Tier = 1;
                obj.InvoicePeriod = period;
                obj.InvoiceStartDate = startdate;
                obj.InvoiceEndDate = enddate;
                obj.RemitTo = "<p><b>Chemical Marketing Concepts, LLC</b><br />c/o Odyssey Logistics &amp; Technology Corp<br />39 Old Ridgebury Road, N-1<br />Danbury, CT 06810</p>";

                db.SaveChanges();

                return obj.InvoiceID;
            }
        }

        public static int SaveInvoice(InvoiceViewModel vm)
        {
            using (var db = new CMCSQL03Entities())
            {
                var confirmVerify = false;
                if (vm.VerifiedAccuracy == true)
                {
                    /// Add logic to check invoice status if not new or verified already
                    /// check if verified by user == created user. should be different
                    /// capture verified datetime and user identity and save to db
                    /// verified invoices should be locked from further edits.
                    vm.Status = "VERIFIED";
                    confirmVerify = true;
                }

                // Capture user info in viewmodel
                vm.UpdatedBy = HttpContext.Current.User.Identity.Name;
                vm.UpdateDate = DateTime.UtcNow;

                var q = (from t in db.tblInvoice
                         where t.InvoiceID == vm.InvoiceId
                         select t).FirstOrDefault();

                q.InvoiceNumber = vm.InvoiceId;
                q.BillingGroup = vm.BillingGroup;
                q.ClientID = vm.ClientId;
                q.ClientName = vm.ClientName;
                q.CreatedBy = vm.CreatedBy;
                q.CreateDate = vm.CreateDate;
                q.UpdatedBy = vm.UpdatedBy;
                q.UpdateDate = vm.UpdateDate;
                q.InvoiceStartDate = vm.InvoiceStartDate;
                q.InvoiceEndDate = vm.InvoiceEndDate;

                if (confirmVerify == true)
                {
                    q.VerifiedAccuracy = vm.VerifiedAccuracy;
                    q.VerifiedBy = HttpContext.Current.User.Identity.Name;
                    q.VerifyDate = System.DateTimeOffset.UtcNow;
                }

                q.Status = vm.Status;
                q.Comments = vm.Comments;
                q.InvoiceDate = vm.InvoiceDate;
                q.InvoicePeriod = vm.InvoicePeriod;
                q.PONumber = vm.PONumber;
                q.NetTerm = vm.NetTerm;
                q.BillTo = vm.BillTo;
                q.RemitTo = vm.RemitTo;
                q.OrderType = vm.OrderType;
                q.SampleShipSameDay = vm.SampleShipSameDay;
                q.SampleShipNextDay = vm.SampleShipNextDay;
                q.SampleShipSecondDay = vm.SampleShipSecondDay;
                q.SampleShipOther = vm.SampleShipOther;
                q.TotalSamples = vm.TotalSamples;

                decimal? grandtotal = 0;

                // Quantities
                q.AirHzdOnlyQuantity = vm.AirHzdOnlyQuantity;
                q.CertificateOfOriginQuantity = vm.CertificateOfOriginQuantity;
                q.CMCPackQuantity = vm.CMCPackQuantity;
                q.CoolPackQuantity = vm.CoolPackQuantity;
                q.CreditCardFeeQuantity = vm.CreditCardFeeQuantity;
                q.CreditCardOrderQuantity = vm.CreditCardOrderQuantity;
                q.DocumentationHandlingQuantity = vm.DocumentationHandlingQuantity;
                q.EmptyPackagingQuantity = vm.EmptyPackagingQuantity;
                q.ExternalSystemQuantity = vm.ExternalSystemQuantity;
                q.FollowUpOrderQuantity = vm.FollowUpOrderQuantity;
                q.FreezerPackQuantity = vm.FreezerPackQuantity;
                q.GHSLabelsQuantity = vm.GHSLabelsQuantity;
                q.InactiveProductsQuantity = vm.InactiveProductsQuantity;
                q.IsolationQuantity = vm.IsolationQuantity;
                q.IsolationBoxQuantity = vm.IsolationBoxQuantity;
                q.ITFeeQuantity = vm.ITFeeQuantity;
                q.LabelMaintainanceQuantity = vm.LabelMaintainanceQuantity;
                q.LabelStockQuantity = vm.LabelStockQuantity;
                q.LabelsPrintedQuantity = vm.LabelsPrintedQuantity;
                q.LaborRelabelQuantity = vm.LaborRelabelQuantity;
                q.LiteratureFeeQuantity = vm.LiteratureFeeQuantity;
                q.LimitedQtyQuantity = vm.LimitedQtyQuantity;
                q.ManualHandlingQuantity = vm.ManualHandlingQuantity;
                q.MSDSPrintsQuantity = vm.MSDSPrintsQuantity;
                q.NewLabelSetupQuantity = vm.NewLabelSetupQuantity;
                q.NewProductSetupQuantity = vm.NewProductSetupQuantity;
                q.OberkPackQuantity = vm.OberkPackQuantity;
                q.OrderEntryQuantity = vm.OrderEntryQuantity;
                q.OverPackQuantity = vm.OverPackQuantity;
                q.PalletReturnQuantity = vm.PalletReturnQuantity;
                q.PoisonPackQuantity = vm.PoisonPackQuantity;
                q.ProductSetupChangesQuantity = vm.ProductSetupChangesQuantity;
                q.QCStorageQuantity = vm.QCStorageQuantity;
                q.RDHandlingADRQuantity = vm.RDHandlingADRQuantity;
                q.RDHandlingIATAQuantity = vm.RDHandlingIATAQuantity;
                q.RDHandlingLQQuantity = vm.RDHandlingLQQuantity;
                q.RDHandlingNonHzdQuantity = vm.RDHandlingNonHzdQuantity;
                q.RefrigeratorStorageQuantity = vm.RefrigeratorStorageQuantity;
                q.RelabelsQuantity = vm.RelabelsQuantity;
                q.RushShipmentQuantity = vm.RushShipmentQuantity;
                q.SPA197AppliedQuantity = vm.SPA197AppliedQuantity;
                q.SPSPaidOrderQuantity = vm.SPSPaidOrderQuantity;
                q.UNBoxQuantity = vm.UNBoxQuantity;
                q.WarehouseStorageQuantity = vm.WarehouseStorageQuantity;
                q.WHMISLabelsQuantity = vm.WHMISLabelsQuantity;

                // Rates
                q.AirHzdOnlyRate = vm.AirHzdOnlyRate;
                q.CertificateOfOriginRate = vm.CertificateOfOriginRate;
                q.CMCPackRate = vm.CMCPackRate;
                q.CoolPackRate = vm.CoolPackRate;
                q.CreditCardFeeRate = vm.CreditCardFeeRate;
                q.CreditCardOrderRate = vm.CreditCardOrderRate;
                q.DocumentationHandlingRate = vm.DocumentationHandlingRate;
                q.EmptyPackagingRate = vm.EmptyPackagingRate;
                q.ExternalSystemRate = vm.ExternalSystemRate;
                q.FollowUpOrderRate = vm.FollowUpOrderRate;
                q.FreezerPackRate = vm.FreezerPackRate;
                q.GHSLabelsRate = vm.GHSLabelsRate;
                q.InactiveProductsRate = vm.InactiveProductsRate;
                q.IsolationRate = vm.IsolationRate;
                q.IsolationBoxRate = vm.IsolationBoxRate;
                q.ITFeeRate = vm.ITFeeRate;
                q.LabelMaintainanceRate = vm.LabelMaintainanceRate;
                q.LabelStockRate = vm.LabelStockRate;
                q.LabelsPrintedRate = vm.LabelsPrintedRate;
                q.LaborRelabelRate = vm.LaborRelabelRate;
                q.LiteratureFeeRate = vm.LiteratureFeeRate;
                q.LimitedQtyRate = vm.LimitedQtyRate;
                q.ManualHandlingRate = vm.ManualHandlingRate;
                q.MSDSPrintsRate = vm.MSDSPrintsRate;
                q.NewLabelSetupRate = vm.NewLabelSetupRate;
                q.NewProductSetupRate = vm.NewProductSetupRate;
                q.OberkPackRate = vm.OberkPackRate;
                q.OrderEntryRate = vm.OrderEntryRate;
                q.OverPackRate = vm.OverPackRate;
                q.PalletReturnRate = vm.PalletReturnRate;
                q.PoisonPackRate = vm.PoisonPackRate;
                q.ProductSetupChangesRate = vm.ProductSetupChangesRate;
                q.QCStorageRate = vm.QCStorageRate;
                q.RDHandlingADRRate = vm.RDHandlingADRRate;
                q.RDHandlingIATARate = vm.RDHandlingIATARate;
                q.RDHandlingLQRate = vm.RDHandlingLQRate;
                q.RDHandlingNonHzdRate = vm.RDHandlingNonHzdRate;
                q.RefrigeratorStorageRate = vm.RefrigeratorStorageRate;
                q.RelabelsRate = vm.RelabelsRate;
                q.RushShipmentRate = vm.RushShipmentRate;
                q.SPA197AppliedRate = vm.SPA197AppliedRate;
                q.SPSPaidOrderRate = vm.SPSPaidOrderRate;
                q.UNBoxRate = vm.UNBoxRate;
                q.WarehouseStorageRate = vm.WarehouseStorageRate;
                q.WHMISLabelsRate = vm.WHMISLabelsRate;

                // Calulated Charges
                grandtotal += q.AirHzdOnlyCharge = vm.AirHzdOnlyQuantity * vm.AirHzdOnlyRate;
                grandtotal += q.CertificateOfOriginCharge = vm.CertificateOfOriginQuantity * vm.CertificateOfOriginRate;
                grandtotal += q.CMCPackCharge = vm.CMCPackQuantity * vm.CMCPackRate;
                grandtotal += q.CoolPackCharge = vm.CoolPackQuantity * vm.CoolPackRate;
                grandtotal += q.CreditCardFeeCharge = vm.CreditCardFeeQuantity * vm.CreditCardFeeRate;
                grandtotal += q.CreditCardOrderCharge = vm.CreditCardOrderQuantity * vm.CreditCardOrderRate;
                grandtotal += q.DocumentationHandlingCharge = vm.DocumentationHandlingQuantity * vm.DocumentationHandlingRate;
                grandtotal += q.EmptyPackagingCharge = vm.EmptyPackagingQuantity * vm.EmptyPackagingRate;
                grandtotal += q.ExternalSystemCharge = vm.ExternalSystemQuantity * vm.ExternalSystemRate;
                grandtotal += q.FollowUpOrderCharge = vm.FollowUpOrderQuantity * vm.FollowUpOrderRate;
                grandtotal += q.FreezerPackCharge = vm.FreezerPackQuantity * vm.FreezerPackRate;
                grandtotal += q.GHSLabelsCharge = vm.GHSLabelsQuantity * vm.GHSLabelsRate;
                grandtotal += q.InactiveProductsCharge = vm.InactiveProductsQuantity * vm.InactiveProductsRate;
                grandtotal += q.IsolationCharge = vm.IsolationQuantity * vm.IsolationRate;
                grandtotal += q.IsolationBoxCharge = vm.IsolationBoxQuantity * vm.IsolationBoxRate;
                grandtotal += q.ITFeeCharge = vm.ITFeeQuantity * vm.ITFeeRate;
                grandtotal += q.LabelMaintainanceCharge = vm.LabelMaintainanceQuantity * vm.LabelMaintainanceRate;
                grandtotal += q.LabelStockCharge = vm.LabelStockQuantity * vm.LabelStockRate;
                grandtotal += q.LabelsPrintedCharge = vm.LabelsPrintedQuantity * vm.LabelsPrintedRate;
                grandtotal += q.LaborRelabelCharge = vm.LaborRelabelQuantity * vm.LaborRelabelRate;
                grandtotal += q.LiteratureFeeCharge = vm.LiteratureFeeQuantity * vm.LiteratureFeeRate;
                grandtotal += q.LimitedQtyCharge = vm.LimitedQtyQuantity * vm.LimitedQtyRate;
                grandtotal += q.ManualHandlingCharge = vm.ManualHandlingQuantity * vm.ManualHandlingRate;
                grandtotal += q.MSDSPrintsCharge = vm.MSDSPrintsQuantity * vm.MSDSPrintsRate;
                grandtotal += q.NewLabelSetupCharge = vm.NewLabelSetupQuantity * vm.NewLabelSetupRate;
                grandtotal += q.NewProductSetupCharge = vm.NewProductSetupQuantity * vm.NewProductSetupRate;
                grandtotal += q.OberkPackCharge = vm.OberkPackQuantity * vm.OberkPackRate;
                grandtotal += q.OrderEntryCharge = vm.OrderEntryQuantity * vm.OrderEntryRate;
                grandtotal += q.OverPackCharge = vm.OverPackQuantity * vm.OverPackRate;
                grandtotal += q.PalletReturnCharge = vm.PalletReturnQuantity * vm.PalletReturnRate;
                grandtotal += q.PoisonPackCharge = vm.PoisonPackQuantity * vm.PoisonPackRate;
                grandtotal += q.ProductSetupChangesCharge = vm.ProductSetupChangesQuantity * vm.ProductSetupChangesRate;
                grandtotal += q.QCStorageCharge = vm.QCStorageQuantity * vm.QCStorageRate;
                grandtotal += q.RDHandlingADRCharge = vm.RDHandlingADRQuantity * vm.RDHandlingADRRate;
                grandtotal += q.RDHandlingIATACharge = vm.RDHandlingIATAQuantity * vm.RDHandlingIATARate;
                grandtotal += q.RDHandlingLQCharge = vm.RDHandlingLQQuantity * vm.RDHandlingLQRate;
                grandtotal += q.RDHandlingNonHzdCharge = vm.RDHandlingNonHzdQuantity * vm.RDHandlingNonHzdRate;
                grandtotal += q.RefrigeratorStorageCharge = vm.RefrigeratorStorageQuantity * vm.RefrigeratorStorageRate;
                grandtotal += q.RelabelsCharge = vm.RelabelsQuantity * vm.RelabelsRate;
                grandtotal += q.RushShipmentCharge = vm.RushShipmentQuantity * vm.RushShipmentRate;
                grandtotal += q.SPA197AppliedCharge = vm.SPA197AppliedQuantity * vm.SPA197AppliedRate;
                grandtotal += q.SPSPaidOrderCharge = vm.SPSPaidOrderQuantity * vm.SPSPaidOrderRate;
                grandtotal += q.UNBoxCharge = vm.UNBoxQuantity * vm.UNBoxRate;
                grandtotal += q.WarehouseStorageCharge = vm.WarehouseStorageQuantity * vm.WarehouseStorageRate;
                grandtotal += q.WHMISLabelsCharge = vm.WHMISLabelsQuantity * vm.WHMISLabelsRate;

                // Variables Charges
                grandtotal += q.AdministrativeWasteFeeCharge = vm.AdministrativeWasteFeeCharge;
                grandtotal += q.CreditCharge = vm.CreditCharge;
                grandtotal += q.CustomsDocumentsCharge = vm.CustomsDocumentsCharge;
                grandtotal += q.DeliveryDutiesTaxesCharge = vm.DeliveryDutiesTaxesCharge;
                grandtotal += q.DocumentsCharge = vm.DocumentsCharge;
                grandtotal += q.HandlingCharge = vm.HandlingCharge;
                grandtotal += q.MautFuel = vm.MautFuel;
                grandtotal += q.MiscellaneousLaborCharge = vm.MiscellaneousLaborCharge;
                grandtotal += q.OtherCharge = vm.OtherCharge;
                grandtotal += q.WasteProcessingCharge = vm.WasteProcessingCharge;

                q.GrandTotal = grandtotal;
                q.TotalServiceCharge = grandtotal;

                q.TotalCostSamples = vm.TotalCostSamples;
                q.TotalFreight = vm.TotalFreight;
                q.TotalFrtHzdSchg = vm.TotalFrtHzdSchg;
                q.TotalDue = vm.TotalCostSamples + vm.TotalFreight + vm.TotalFrtHzdSchg + grandtotal;

                db.SaveChanges();

                return vm.InvoiceId;
            }
        }

        public static int GenerateInvoice(int invoiceid)
        {
            int clientid = 0;
            int divisionid = 0;
            string billinggroup;
            DateTime startdate = DateTime.UtcNow;
            DateTime enddate = DateTime.UtcNow;
            int? sampleitemscount = 0;
            string[] sampletranstypes = { "SAMP", "HAZD", "FLAM", "HEAT", "REFR", "FREZ", "CLEN", "BLND", "NALG", "NITR", "BIOC", "KOSH", "LABL" };
            Random random = new Random();
            decimal? grandtotal = 0;
            decimal? sumsamplecharges;
            decimal? calcfreightcharges;
            decimal? calcservicecharges;
            decimal? sumchargesdue;

            using (var db = new CMCSQL03Entities())
            {
                // Get invoice info
                var invoice = (from t in db.tblInvoice
                               where t.InvoiceID == invoiceid
                               select t).FirstOrDefault();

                clientid = Convert.ToInt32(invoice.ClientID);
                startdate = Convert.ToDateTime(invoice.InvoiceStartDate);
                enddate = Convert.ToDateTime(invoice.InvoiceEndDate);

                // Check input for type and determine whether billing group is division or enduse
                int n;
                bool isDivision = int.TryParse(invoice.BillingGroup, out n);

                if (isDivision == true)
                {
                    divisionid = Convert.ToInt32(invoice.BillingGroup);
                    var getDivision = (from t in db.tblDivision
                                       where t.DivisionID == divisionid
                                       select t).FirstOrDefault();

                    billinggroup = getDivision.DivisionName + " / " + getDivision.BusinessUnit;
                }
                else if (isDivision == false)
                {
                    billinggroup = invoice.BillingGroup;                        // Could be "All" or End Use
                }
                else
                {
                    billinggroup = "ALL";
                }

                // Get client info
                var client = (from t in db.tblClient
                              where t.ClientID == clientid
                              select t).FirstOrDefault();

                // Get predetermined rates set for client.
                var servicerates = (from t in db.tblRates
                                    where t.ClientID == clientid
                                    select t).FirstOrDefault();

                // Get order items for client within date range.
                var orderitems = (from t in db.tblOrderItem
                                  join o in db.tblOrderMaster on t.OrderID equals o.OrderID
                                  where o.ClientID == clientid
                                  && (t.ShipDate >= startdate && t.ShipDate <= enddate)
                                  select t).ToList();

                sampleitemscount = (from t in orderitems
                                   select (t.Qty)).Sum();

                // Get order transactions within date range.
                var transactions = (from t in db.tblOrderTrans
                                    where t.ClientID == clientid
                                    && (t.TransDate >= startdate && t.TransDate <= enddate)
                                    select t).ToList();

                var samplechargetrans = transactions.Where(x => sampletranstypes.Contains(x.TransType)).ToList();

                sumsamplecharges = (from t in samplechargetrans
                                    select (t.TransQty * t.TransRate)).Sum();

                // Bypass business rules within condition if 'ALL' is selected
                if (billinggroup != "ALL")
                {
                    // Filter transactions by billing group if division/businessunit is selected
                    if (isDivision == true)
                    {
                        var billinggrouptransactions = transactions.Where(x => x.DivisionID == divisionid).ToList();

                        transactions = billinggrouptransactions;

                        // Filter order items by divisionid, count, and sum up the sample charges
                        var sampleitems = (from orderitem in orderitems
                                           join order in db.tblOrderMaster on orderitem.OrderID equals order.OrderID
                                           where order.DivisionID == divisionid
                                           select orderitem).ToList();

                        sampleitemscount = (from t in sampleitems
                                            select (t.Qty)).Sum();

                        samplechargetrans = (from trans in transactions
                                             join orderitem in orderitems on trans.OrderItemID equals orderitem.ItemID
                                             where sampletranstypes.Contains(trans.TransType)
                                             select trans).ToList();

                        sumsamplecharges = (from t in samplechargetrans
                                            select (t.TransQty * t.BillingRate)).Sum();             // Calculate sample charges using billing rate by adjusted tiers
                    }

                    // Filter orders by enduse if enduse is selected and match orders to transactions
                    if (isDivision == false)
                    {
                        string enduse = billinggroup;

                        var enduseorders = (from t in db.tblOrderMaster
                                            where t.ClientID == clientid
                                            && t.EndUse == enduse
                                            select t).ToList();

                        var endusetransactions = (from trans in transactions
                                                  join order in enduseorders on trans.OrderID equals order.OrderID
                                                  select trans).ToList();

                        transactions = endusetransactions;

                        // Filter order items by enduse, count, and sum up the sample charges
                        var sampleitems = (from orderitem in orderitems
                                           join order in db.tblOrderMaster on orderitem.OrderID equals order.OrderID
                                           where order.EndUse == enduse
                                           select orderitem).ToList();

                        sampleitemscount = (from t in sampleitems
                                            select (t.Qty)).Sum();

                        samplechargetrans = transactions.Where(x => sampletranstypes.Contains(x.TransType)).ToList();

                        sumsamplecharges = (from t in samplechargetrans
                                            select (t.TransQty * t.BillingRate)).Sum();             // Calculate sample charges using billing rate by adjusted tiers
                    }
                }

                // TODO: Calculate Freight Charges
                calcfreightcharges = 0;                                                                         // random.Next(100, 999);

                // TODO: Calculate Freight Hazard Surcharge
                var calcfreighthazdsurcharges = 0;                                                              // random.Next(100, 999);

                // TODO: Calculate shipping performance.
                // Is shipping calculated per sample or per order?
                int? countorderitems = sampleitemscount;
                invoice.SampleShipSameDay = sampleitemscount;                                                   // (int)(sampleitemscount * 0.5);
                invoice.SampleShipNextDay = 0;                                                                  // (int)(sampleitemscount * 0.25);
                invoice.SampleShipSecondDay = 0;                                                                // (int)(sampleitemscount * 0.15);
                invoice.SampleShipOther = 0;                                                                    // (int)(sampleitemscount * 0.1);

                // Group and sum transaction quantities for fixed charges; and amounts for variable charges.
                var getservicetransactions = from transaction in transactions
                                             group transaction by transaction.TransType into transactiongroup
                                             select new
                                             {
                                                 TransType = transactiongroup.Key,
                                                 TransQty = transactiongroup.Sum(x => x.TransQty),
                                                 TransAmount = transactiongroup.Sum(x => x.TransAmount),
                                             };

                // Populate corresponding quantities
                // Assign sum of quantities from order transaction to corresponding transaction type fields
                invoice.AirHzdOnlyQuantity = getservicetransactions.Where(j => j.TransType == "Air Hazard Only").Select(i => i.TransQty).Sum();
                invoice.CertificateOfOriginQuantity = getservicetransactions.Where(j => j.TransType == "Certificate Of Origin").Select(i => i.TransQty).Sum();
                invoice.CMCPackQuantity = getservicetransactions.Where(j => j.TransType == "CMC Pack").Select(i => i.TransQty).Sum();
                invoice.CoolPackQuantity = getservicetransactions.Where(j => j.TransType == "Cool Pack").Select(i => i.TransQty).Sum();
                invoice.CreditCardFeeQuantity = getservicetransactions.Where(j => j.TransType == "Credit Card Fee").Select(i => i.TransQty).Sum();
                invoice.CreditCardOrderQuantity = getservicetransactions.Where(j => j.TransType == "Credit Card Order").Select(i => i.TransQty).Sum();
                invoice.DocumentationHandlingQuantity = getservicetransactions.Where(j => j.TransType == "Document Handling").Select(i => i.TransQty).Sum();
                invoice.EmptyPackagingQuantity = getservicetransactions.Where(j => j.TransType == "Empty Packaging").Select(i => i.TransQty).Sum();
                invoice.ExternalSystemQuantity = getservicetransactions.Where(j => j.TransType == "External System").Select(i => i.TransQty).Sum();
                invoice.FollowUpOrderQuantity = getservicetransactions.Where(j => j.TransType == "Follow Up Order").Select(i => i.TransQty).Sum();
                invoice.FreezerPackQuantity = getservicetransactions.Where(j => j.TransType == "Freezer Pack").Select(i => i.TransQty).Sum();
                invoice.GHSLabelsQuantity = getservicetransactions.Where(j => j.TransType == "GHS Labels").Select(i => i.TransQty).Sum();
                invoice.InactiveProductsQuantity = getservicetransactions.Where(j => j.TransType == "Inactive Products").Select(i => i.TransQty).Sum();
                invoice.IsolationQuantity = getservicetransactions.Where(j => j.TransType == "Isolation").Select(i => i.TransQty).Sum();
                invoice.IsolationBoxQuantity = getservicetransactions.Where(j => j.TransType == "Isolation Box").Select(i => i.TransQty).Sum();
                invoice.ITFeeQuantity = getservicetransactions.Where(j => j.TransType == "IT Fee").Select(i => i.TransQty).Sum();
                invoice.LabelMaintainanceQuantity = getservicetransactions.Where(j => j.TransType == "Label Maintainance").Select(i => i.TransQty).Sum();
                invoice.LabelStockQuantity = getservicetransactions.Where(j => j.TransType == "Label Stock").Select(i => i.TransQty).Sum();
                invoice.LabelsPrintedQuantity = getservicetransactions.Where(j => j.TransType == "Labels Printed").Select(i => i.TransQty).Sum();
                invoice.LaborRelabelQuantity = getservicetransactions.Where(j => j.TransType == "Labor Relabel").Select(i => i.TransQty).Sum();
                invoice.LiteratureFeeQuantity = getservicetransactions.Where(j => j.TransType == "Literature Fee").Select(i => i.TransQty).Sum();
                invoice.LimitedQtyQuantity = getservicetransactions.Where(j => j.TransType == "Limited Quantity").Select(i => i.TransQty).Sum();
                invoice.ManualHandlingQuantity = getservicetransactions.Where(j => j.TransType == "Manual Handling").Select(i => i.TransQty).Sum();
                invoice.MSDSPrintsQuantity = getservicetransactions.Where(j => j.TransType == "MSDS Prints").Select(i => i.TransQty).Sum();
                invoice.NewLabelSetupQuantity = getservicetransactions.Where(j => j.TransType == "New Label Setup").Select(i => i.TransQty).Sum();
                invoice.NewProductSetupQuantity = getservicetransactions.Where(j => j.TransType == "New Product Setup").Select(i => i.TransQty).Sum();
                invoice.OberkPackQuantity = getservicetransactions.Where(j => j.TransType == "Oberk Pack").Select(i => i.TransQty).Sum();
                invoice.OrderEntryQuantity = getservicetransactions.Where(j => j.TransType == "Order Entry").Select(i => i.TransQty).Sum();
                invoice.OverPackQuantity = getservicetransactions.Where(j => j.TransType == "Over Pack").Select(i => i.TransQty).Sum();
                invoice.PalletReturnQuantity = getservicetransactions.Where(j => j.TransType == "Pallet Return").Select(i => i.TransQty).Sum();
                invoice.PoisonPackQuantity = getservicetransactions.Where(j => j.TransType == "Poison Pack").Select(i => i.TransQty).Sum();
                invoice.ProductSetupChangesQuantity = getservicetransactions.Where(j => j.TransType == "Product Setup Changes").Select(i => i.TransQty).Sum();
                invoice.QCStorageQuantity = getservicetransactions.Where(j => j.TransType == "QC Storage").Select(i => i.TransQty).Sum();
                invoice.RDHandlingADRQuantity = getservicetransactions.Where(j => j.TransType == "RD Handling ADR").Select(i => i.TransQty).Sum();
                invoice.RDHandlingIATAQuantity = getservicetransactions.Where(j => j.TransType == "RD Handling IATA").Select(i => i.TransQty).Sum();
                invoice.RDHandlingLQQuantity = getservicetransactions.Where(j => j.TransType == "RD Handling LQ").Select(i => i.TransQty).Sum();
                invoice.RDHandlingNonHzdQuantity = getservicetransactions.Where(j => j.TransType == "RD Handling Non Hazard").Select(i => i.TransQty).Sum();
                invoice.RefrigeratorStorageQuantity = getservicetransactions.Where(j => j.TransType == "Refrigerator Storage").Select(i => i.TransQty).Sum();
                invoice.RelabelsQuantity = getservicetransactions.Where(j => j.TransType == "Relabels").Select(i => i.TransQty).Sum();
                invoice.RushShipmentQuantity = getservicetransactions.Where(j => j.TransType == "Rush Shipment").Select(i => i.TransQty).Sum();
                invoice.SPA197AppliedQuantity = getservicetransactions.Where(j => j.TransType == "SPA 197 Applied").Select(i => i.TransQty).Sum();
                invoice.SPSPaidOrderQuantity = getservicetransactions.Where(j => j.TransType == "SPS Paid Order").Select(i => i.TransQty).Sum();
                invoice.UNBoxQuantity = getservicetransactions.Where(j => j.TransType == "UN Box").Select(i => i.TransQty).Sum();
                invoice.WarehouseStorageQuantity = getservicetransactions.Where(j => j.TransType == "Warehouse Storage").Select(i => i.TransQty).Sum();
                invoice.WHMISLabelsQuantity = getservicetransactions.Where(j => j.TransType == "WHMIS Labels").Select(i => i.TransQty).Sum();

                // Populate corresponding rates.
                // Assign set rates established in client profile with 1 as default
                invoice.AirHzdOnlyRate = servicerates.AirHazardOnly ?? 1;
                invoice.CertificateOfOriginRate = servicerates.CertificateOfOrigin ?? 1;
                invoice.CMCPackRate = servicerates.CMCPack ?? 1;
                invoice.CoolPackRate = servicerates.CoolPack ?? 1;
                invoice.CreditCardFeeRate = servicerates.CreditCardFee ?? 1;
                invoice.CreditCardOrderRate = servicerates.CreditCardOrder ?? 1;
                invoice.DocumentationHandlingRate = servicerates.DocumentHandling ?? 1;
                invoice.EmptyPackagingRate = servicerates.EmptyPackaging ?? 1;
                invoice.ExternalSystemRate = servicerates.ExternalSystem ?? 1;
                invoice.FollowUpOrderRate = servicerates.FollowUpOrder ?? 1;
                invoice.FreezerPackRate = servicerates.FreezerPack ?? 1;
                invoice.GHSLabelsRate = servicerates.GHSLabels ?? 1;
                invoice.InactiveProductsRate = servicerates.InactiveProducts ?? 1;
                invoice.IsolationRate = servicerates.Isolation ?? 1;
                invoice.IsolationBoxRate = servicerates.IsolationBox ?? 1;
                invoice.ITFeeRate = servicerates.ITFee ?? 1;
                invoice.LabelMaintainanceRate = servicerates.LabelMaintainance ?? 1;
                invoice.LabelStockRate = servicerates.LabelStock ?? 1;
                invoice.LabelsPrintedRate = servicerates.LabelsPrinted ?? 1;
                invoice.LaborRelabelRate = servicerates.LaborRelabel ?? 1;
                invoice.LiteratureFeeRate = servicerates.LiteratureFee ?? 1;
                invoice.LimitedQtyRate = servicerates.LimitedQuantity ?? 1;
                invoice.ManualHandlingRate = servicerates.ManualHandling ?? 1;
                invoice.MSDSPrintsRate = servicerates.MSDSPrints ?? 1;
                invoice.NewLabelSetupRate = servicerates.NewLabelSetup ?? 1;
                invoice.NewProductSetupRate = servicerates.NewProductSetup ?? 1;
                invoice.OberkPackRate = servicerates.OberkPack ?? 1;
                invoice.OrderEntryRate = servicerates.OrderEntry ?? 1;
                invoice.OverPackRate = servicerates.OverPack ?? 1;
                invoice.PalletReturnRate = servicerates.PalletReturn ?? 1;
                invoice.PoisonPackRate = servicerates.PoisonPack ?? 1;
                invoice.ProductSetupChangesRate = servicerates.ProductSetupChanges ?? 1;
                invoice.QCStorageRate = servicerates.QCStorage ?? 1;
                invoice.RDHandlingADRRate = servicerates.RDHandlingADR ?? 1;
                invoice.RDHandlingIATARate = servicerates.RDHandlingIATA ?? 1;
                invoice.RDHandlingLQRate = servicerates.RDHandlingLQ ?? 1;
                invoice.RDHandlingNonHzdRate = servicerates.RDHandlingNonHazard ?? 1;
                invoice.RefrigeratorStorageRate = servicerates.RefrigeratorStorage ?? 1;
                invoice.RelabelsRate = servicerates.Relabels ?? 1;
                invoice.RushShipmentRate = servicerates.RushShipment ?? 1;
                invoice.SPA197AppliedRate = servicerates.SPA197Applied ?? 1;
                invoice.SPSPaidOrderRate = servicerates.SPSPaidOrder ?? 1;
                invoice.UNBoxRate = servicerates.UNBox ?? 1;
                invoice.WarehouseStorageRate = servicerates.WarehouseStorage ?? 1;
                invoice.WHMISLabelsRate = servicerates.WHMISLabels ?? 1;

                // Calculate charges by multiplying quantities with corresponding rates.
                // Aggregate sum into grandtotal.
                grandtotal += invoice.AirHzdOnlyCharge = invoice.AirHzdOnlyQuantity * invoice.AirHzdOnlyRate;
                grandtotal += invoice.CertificateOfOriginCharge = invoice.CertificateOfOriginQuantity * invoice.CertificateOfOriginRate;
                grandtotal += invoice.CMCPackCharge = invoice.CMCPackQuantity * invoice.CMCPackRate;
                grandtotal += invoice.CoolPackCharge = invoice.CoolPackQuantity * invoice.CoolPackRate;
                grandtotal += invoice.CreditCardFeeCharge = invoice.CreditCardFeeQuantity * invoice.CreditCardFeeRate;
                grandtotal += invoice.CreditCardOrderCharge = invoice.CreditCardOrderQuantity * invoice.CreditCardOrderRate;
                grandtotal += invoice.DocumentationHandlingCharge = invoice.DocumentationHandlingQuantity * invoice.DocumentationHandlingRate;
                grandtotal += invoice.EmptyPackagingCharge = invoice.EmptyPackagingQuantity * invoice.EmptyPackagingRate;
                grandtotal += invoice.ExternalSystemCharge = invoice.ExternalSystemQuantity * invoice.ExternalSystemRate;
                grandtotal += invoice.FollowUpOrderCharge = invoice.FollowUpOrderQuantity * invoice.FollowUpOrderRate;
                grandtotal += invoice.FreezerPackCharge = invoice.FreezerPackQuantity * invoice.FreezerPackRate;
                grandtotal += invoice.GHSLabelsCharge = invoice.GHSLabelsQuantity * invoice.GHSLabelsRate;
                grandtotal += invoice.InactiveProductsCharge = invoice.InactiveProductsQuantity * invoice.InactiveProductsRate;
                grandtotal += invoice.IsolationCharge = invoice.IsolationQuantity * invoice.IsolationRate;
                grandtotal += invoice.IsolationBoxCharge = invoice.IsolationBoxQuantity * invoice.IsolationBoxRate;
                grandtotal += invoice.ITFeeCharge = invoice.ITFeeQuantity * invoice.ITFeeRate;
                grandtotal += invoice.LabelMaintainanceCharge = invoice.LabelMaintainanceQuantity * invoice.LabelMaintainanceRate;
                grandtotal += invoice.LabelStockCharge = invoice.LabelStockQuantity * invoice.LabelStockRate;
                grandtotal += invoice.LabelsPrintedCharge = invoice.LabelsPrintedQuantity * invoice.LabelsPrintedRate;
                grandtotal += invoice.LaborRelabelCharge = invoice.LaborRelabelQuantity * invoice.LaborRelabelRate;
                grandtotal += invoice.LiteratureFeeCharge = invoice.LiteratureFeeQuantity * invoice.LiteratureFeeRate;
                grandtotal += invoice.LimitedQtyCharge = invoice.LimitedQtyQuantity * invoice.LimitedQtyRate;
                grandtotal += invoice.ManualHandlingCharge = invoice.ManualHandlingQuantity * invoice.ManualHandlingRate;
                grandtotal += invoice.MSDSPrintsCharge = invoice.MSDSPrintsQuantity * invoice.MSDSPrintsRate;
                grandtotal += invoice.NewLabelSetupCharge = invoice.NewLabelSetupQuantity * invoice.NewLabelSetupRate;
                grandtotal += invoice.NewProductSetupCharge = invoice.NewProductSetupQuantity * invoice.NewProductSetupRate;
                grandtotal += invoice.OberkPackCharge = invoice.OberkPackQuantity * invoice.OberkPackRate;
                grandtotal += invoice.OrderEntryCharge = invoice.OrderEntryQuantity * invoice.OrderEntryRate;
                grandtotal += invoice.OverPackCharge = invoice.OverPackQuantity * invoice.OverPackRate;
                grandtotal += invoice.PalletReturnCharge = invoice.PalletReturnQuantity * invoice.PalletReturnRate;
                grandtotal += invoice.PoisonPackCharge = invoice.PoisonPackQuantity * invoice.PoisonPackRate;
                grandtotal += invoice.ProductSetupChangesCharge = invoice.ProductSetupChangesQuantity * invoice.ProductSetupChangesRate;
                grandtotal += invoice.QCStorageCharge = invoice.QCStorageQuantity * invoice.QCStorageRate;
                grandtotal += invoice.RDHandlingADRCharge = invoice.RDHandlingADRQuantity * invoice.RDHandlingADRRate;
                grandtotal += invoice.RDHandlingIATACharge = invoice.RDHandlingIATAQuantity * invoice.RDHandlingIATARate;
                grandtotal += invoice.RDHandlingLQCharge = invoice.RDHandlingLQQuantity * invoice.RDHandlingLQRate;
                grandtotal += invoice.RDHandlingNonHzdCharge = invoice.RDHandlingNonHzdQuantity * invoice.RDHandlingNonHzdRate;
                grandtotal += invoice.RefrigeratorStorageCharge = invoice.RefrigeratorStorageQuantity * invoice.RefrigeratorStorageRate;
                grandtotal += invoice.RelabelsCharge = invoice.RelabelsQuantity * invoice.RelabelsRate;
                grandtotal += invoice.RushShipmentCharge = invoice.RushShipmentQuantity * invoice.RushShipmentRate;
                grandtotal += invoice.SPA197AppliedCharge = invoice.SPA197AppliedQuantity * invoice.SPA197AppliedRate;
                grandtotal += invoice.SPSPaidOrderCharge = invoice.SPSPaidOrderQuantity * invoice.SPSPaidOrderRate;
                grandtotal += invoice.UNBoxCharge = invoice.UNBoxQuantity * invoice.UNBoxRate;
                grandtotal += invoice.WarehouseStorageCharge = invoice.WarehouseStorageQuantity * invoice.WarehouseStorageRate;
                grandtotal += invoice.WHMISLabelsCharge = invoice.WHMISLabelsQuantity * invoice.WHMISLabelsRate;

                // Variables charges by group summing amounts with corresponding transaction types.
                // Aggregate sum into grandtotal.
                grandtotal += invoice.AdministrativeWasteFeeCharge = getservicetransactions.Where(j => j.TransType == "Administrative Waste Fee").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.CreditCharge = getservicetransactions.Where(j => j.TransType == "Credit").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.CustomsDocumentsCharge = getservicetransactions.Where(j => j.TransType == "Customs Documents").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.DeliveryDutiesTaxesCharge = getservicetransactions.Where(j => j.TransType == "Delivery Duties Taxes").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.DocumentsCharge = getservicetransactions.Where(j => j.TransType == "Documents").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.HandlingCharge = getservicetransactions.Where(j => j.TransType == "Handling").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.MautFuel = getservicetransactions.Where(j => j.TransType == "MautFuel").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.MiscellaneousLaborCharge = getservicetransactions.Where(j => j.TransType == "Miscellaneous Labor").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.OtherCharge = getservicetransactions.Where(j => j.TransType == "Other").Select(i => i.TransAmount).Sum();
                grandtotal += invoice.WasteProcessingCharge = getservicetransactions.Where(j => j.TransType == "Waste Processing").Select(i => i.TransAmount).Sum();

                invoice.GrandTotal = grandtotal;

                // Calculate all charges due.
                sumchargesdue = sumsamplecharges + calcfreightcharges + calcfreighthazdsurcharges + grandtotal;

                // Fill summary fields.
                invoice.TotalSamples = sampleitemscount;
                invoice.TotalCostSamples = sumsamplecharges;
                invoice.TotalFreight = calcfreightcharges;                                                  // TODO: Random for now
                invoice.TotalFrtHzdSchg = calcfreighthazdsurcharges;                                        // TODO: Random for now
                invoice.TotalServiceCharge = grandtotal;
                invoice.TotalDue = sumchargesdue;

                // Revisit
                invoice.BillingGroup = billinggroup;

                db.SaveChanges();

                return invoiceid;
            }
        }

        public static int NewInvoiceID()
        {
            using (var db = new CMCSQL03Entities())
            {
                tblInvoice nr = new tblInvoice();
                db.tblInvoice.Add(nr);

                db.SaveChanges();

                return nr.InvoiceID;
            }
        }

        public static void SampleRateTierAdjustment(int? tierclient, int? tierlevel, DateTime tierstartdate, DateTime tierenddate)
        {
            using (var db = new CMCSQL03Entities())
            {
                var tiers = (from t in db.tblTier
                             where t.ClientID == tierclient
                             && t.TierLevel == tierlevel
                             select t).ToList();

                if (tiers.Count != 0)
                {
                    var transactions = (from transaction in db.tblOrderTrans
                                        join orderitem in db.tblOrderItem on transaction.OrderItemID equals orderitem.ItemID
                                        where transaction.ClientID == tierclient
                                        && transaction.TransType == "SAMP"
                                        && (transaction.TransDate >= tierstartdate && transaction.TransDate <= tierenddate)
                                        select new { transaction, orderitem }).ToList();

                    for (int i = 0; i < transactions.Count; i++)
                    {
                        try
                        {
                            int xtransid = transactions[i].transaction.OrderTransID;
                            int? xitemid = transactions[i].transaction.OrderItemID;
                            string xsize = transactions[i].orderitem.Size;

                            var gettierlevel = tiers.Where(x => x.Size == xsize).FirstOrDefault();
                            int? xtier = gettierlevel.TierLevel;
                            decimal? xrate = gettierlevel.Price;
                            decimal? xcharge = transactions[i].transaction.TransQty * xrate;

                            var updatebilling = (from t in db.tblOrderTrans
                                                 where t.OrderTransID == xtransid
                                                 && t.OrderItemID == xitemid
                                                 select t).SingleOrDefault();

                            updatebilling.BillingTier = xtier;
                            updatebilling.BillingRate = xrate;
                            updatebilling.BillingCharge = xcharge;
                            var xdate = DateTime.UtcNow.ToString("dd/MM/yyyy");
                            updatebilling.Comments = updatebilling.Comments + ". " + String.Format("Billing rate updated to {0} per tier {1} pricing on {2}.", xrate, xtier, xdate);
                            updatebilling.UpdateDate = DateTime.UtcNow;
                            updatebilling.UpdateUser = HttpContext.Current.User.Identity.Name;

                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        }
                    }
                }
            }
        }
    }
}