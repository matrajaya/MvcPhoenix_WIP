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
                var invoice = (from t in db.tblInvoice
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

                return invoice;
            }
        }

        public static InvoiceViewModel FillInvoice(int id)
        {
            int invoiceId = id;
            InvoiceViewModel invoice = new InvoiceViewModel();

            using (var db = new CMCSQL03Entities())
            {
                var getInvoice = (from t in db.tblInvoice
                                  where t.InvoiceID == invoiceId
                                  select t).FirstOrDefault();

                invoice.InvoiceId = getInvoice.InvoiceID;
                invoice.InvoiceNumber = getInvoice.InvoiceNumber;
                invoice.BillingGroup = getInvoice.BillingGroup;
                invoice.WarehouseLocation = getInvoice.WarehouseLocation;
                invoice.ClientId = getInvoice.ClientID;
                invoice.ClientName = getInvoice.ClientName;
                invoice.CreatedBy = getInvoice.CreatedBy;
                invoice.CreateDate = getInvoice.CreateDate;
                invoice.UpdatedBy = getInvoice.UpdatedBy;
                invoice.UpdateDate = getInvoice.UpdateDate;
                invoice.VerifiedAccuracy = getInvoice.VerifiedAccuracy;
                invoice.VerifiedBy = getInvoice.VerifiedBy;
                invoice.VerifiedDate = getInvoice.VerifyDate;
                invoice.Status = getInvoice.Status;
                invoice.Comments = getInvoice.Comments;
                invoice.InvoiceDate = getInvoice.InvoiceDate;
                invoice.InvoicePeriod = getInvoice.InvoicePeriod;
                invoice.InvoiceStartDate = getInvoice.InvoiceStartDate;
                invoice.InvoiceEndDate = getInvoice.InvoiceEndDate;
                invoice.PONumber = getInvoice.PONumber;
                invoice.NetTerm = getInvoice.NetTerm;
                invoice.BillTo = getInvoice.BillTo;
                invoice.RemitTo = getInvoice.RemitTo;
                invoice.Currency = getInvoice.Currency;
                invoice.Tier = getInvoice.Tier;
                invoice.OrderType = getInvoice.OrderType;                // Sample/International/Revenue - ignore for now

                // Shipping Performance
                invoice.SampleShipSameDay = getInvoice.SampleShipSameDay;
                invoice.SampleShipNextDay = getInvoice.SampleShipNextDay;
                invoice.SampleShipSecondDay = getInvoice.SampleShipSecondDay;
                invoice.SampleShipOther = getInvoice.SampleShipOther;

                // Invoice Summary
                invoice.TotalSamples = getInvoice.TotalSamples;
                invoice.TotalCostSamples = getInvoice.TotalCostSamples;
                invoice.TotalFreight = getInvoice.TotalFreight;
                invoice.TotalFrtHzdSchg = getInvoice.TotalFrtHzdSchg;
                invoice.TotalServiceCharge = getInvoice.TotalServiceCharge;
                invoice.TotalDue = getInvoice.TotalDue;

                // Billing Worksheet
                invoice.GrandTotalCharge = getInvoice.GrandTotal;

                // Quantity
                invoice.AirHzdOnlyQuantity = getInvoice.AirHzdOnlyQuantity;
                invoice.CertificateOfOriginQuantity = getInvoice.CertificateOfOriginQuantity;
                invoice.CMCPackQuantity = getInvoice.CMCPackQuantity;
                invoice.CoolPackQuantity = getInvoice.CoolPackQuantity;
                invoice.CreditCardFeeQuantity = getInvoice.CreditCardFeeQuantity;
                invoice.CreditCardOrderQuantity = getInvoice.CreditCardOrderQuantity;
                invoice.DocumentationHandlingQuantity = getInvoice.DocumentationHandlingQuantity;
                invoice.EmptyPackagingQuantity = getInvoice.EmptyPackagingQuantity;
                invoice.ExternalSystemQuantity = getInvoice.ExternalSystemQuantity;
                invoice.FollowUpOrderQuantity = getInvoice.FollowUpOrderQuantity;
                invoice.FreezerPackQuantity = getInvoice.FreezerPackQuantity;
                invoice.GHSLabelsQuantity = getInvoice.GHSLabelsQuantity;
                invoice.InactiveProductsQuantity = getInvoice.InactiveProductsQuantity;
                invoice.IsolationQuantity = getInvoice.IsolationQuantity;
                invoice.IsolationBoxQuantity = getInvoice.IsolationBoxQuantity;
                invoice.ITFeeQuantity = getInvoice.ITFeeQuantity;
                invoice.LabelMaintainanceQuantity = getInvoice.LabelMaintainanceQuantity;
                invoice.LabelStockQuantity = getInvoice.LabelStockQuantity;
                invoice.LabelsPrintedQuantity = getInvoice.LabelsPrintedQuantity;
                invoice.LaborRelabelQuantity = getInvoice.LaborRelabelQuantity;
                invoice.LiteratureFeeQuantity = getInvoice.LiteratureFeeQuantity;
                invoice.LimitedQtyQuantity = getInvoice.LimitedQtyQuantity;
                invoice.ManualHandlingQuantity = getInvoice.ManualHandlingQuantity;
                invoice.MSDSPrintsQuantity = getInvoice.MSDSPrintsQuantity;
                invoice.NewLabelSetupQuantity = getInvoice.NewLabelSetupQuantity;
                invoice.NewProductSetupQuantity = getInvoice.NewProductSetupQuantity;
                invoice.OberkPackQuantity = getInvoice.OberkPackQuantity;
                invoice.OrderEntryQuantity = getInvoice.OrderEntryQuantity;
                invoice.OverPackQuantity = getInvoice.OverPackQuantity;
                invoice.PalletReturnQuantity = getInvoice.PalletReturnQuantity;
                invoice.PoisonPackQuantity = getInvoice.PoisonPackQuantity;
                invoice.ProductSetupChangesQuantity = getInvoice.ProductSetupChangesQuantity;
                invoice.QCStorageQuantity = getInvoice.QCStorageQuantity;
                invoice.RDHandlingADRQuantity = getInvoice.RDHandlingADRQuantity;
                invoice.RDHandlingIATAQuantity = getInvoice.RDHandlingIATAQuantity;
                invoice.RDHandlingLQQuantity = getInvoice.RDHandlingLQQuantity;
                invoice.RDHandlingNonHzdQuantity = getInvoice.RDHandlingNonHzdQuantity;
                invoice.RefrigeratorStorageQuantity = getInvoice.RefrigeratorStorageQuantity;
                invoice.RelabelsQuantity = getInvoice.RelabelsQuantity;
                invoice.RushShipmentQuantity = getInvoice.RushShipmentQuantity;
                invoice.SPA197AppliedQuantity = getInvoice.SPA197AppliedQuantity;
                invoice.SPSPaidOrderQuantity = getInvoice.SPSPaidOrderQuantity;
                invoice.UNBoxQuantity = getInvoice.UNBoxQuantity;
                invoice.WarehouseStorageQuantity = getInvoice.WarehouseStorageQuantity;
                invoice.WHMISLabelsQuantity = getInvoice.WHMISLabelsQuantity;

                // Rates
                invoice.AirHzdOnlyRate = getInvoice.AirHzdOnlyRate;
                invoice.CertificateOfOriginRate = getInvoice.CertificateOfOriginRate;
                invoice.CMCPackRate = getInvoice.CMCPackRate;
                invoice.CoolPackRate = getInvoice.CoolPackRate;
                invoice.CreditCardFeeRate = getInvoice.CreditCardFeeRate;
                invoice.CreditCardOrderRate = getInvoice.CreditCardOrderRate;
                invoice.DocumentationHandlingRate = getInvoice.DocumentationHandlingRate;
                invoice.EmptyPackagingRate = getInvoice.EmptyPackagingRate;
                invoice.ExternalSystemRate = getInvoice.ExternalSystemRate;
                invoice.FollowUpOrderRate = getInvoice.FollowUpOrderRate;
                invoice.FreezerPackRate = getInvoice.FreezerPackRate;
                invoice.GHSLabelsRate = getInvoice.GHSLabelsRate;
                invoice.InactiveProductsRate = getInvoice.InactiveProductsRate;
                invoice.IsolationRate = getInvoice.IsolationRate;
                invoice.IsolationBoxRate = getInvoice.IsolationBoxRate;
                invoice.ITFeeRate = getInvoice.ITFeeRate;
                invoice.LabelMaintainanceRate = getInvoice.LabelMaintainanceRate;
                invoice.LabelStockRate = getInvoice.LabelStockRate;
                invoice.LabelsPrintedRate = getInvoice.LabelsPrintedRate;
                invoice.LaborRelabelRate = getInvoice.LaborRelabelRate;
                invoice.LiteratureFeeRate = getInvoice.LiteratureFeeRate;
                invoice.LimitedQtyRate = getInvoice.LimitedQtyRate;
                invoice.ManualHandlingRate = getInvoice.ManualHandlingRate;
                invoice.MSDSPrintsRate = getInvoice.MSDSPrintsRate;
                invoice.NewLabelSetupRate = getInvoice.NewLabelSetupRate;
                invoice.NewProductSetupRate = getInvoice.NewProductSetupRate;
                invoice.OberkPackRate = getInvoice.OberkPackRate;
                invoice.OrderEntryRate = getInvoice.OrderEntryRate;
                invoice.OverPackRate = getInvoice.OverPackRate;
                invoice.PalletReturnRate = getInvoice.PalletReturnRate;
                invoice.PoisonPackRate = getInvoice.PoisonPackRate;
                invoice.ProductSetupChangesRate = getInvoice.ProductSetupChangesRate;
                invoice.QCStorageRate = getInvoice.QCStorageRate;
                invoice.RDHandlingADRRate = getInvoice.RDHandlingADRRate;
                invoice.RDHandlingIATARate = getInvoice.RDHandlingIATARate;
                invoice.RDHandlingLQRate = getInvoice.RDHandlingLQRate;
                invoice.RDHandlingNonHzdRate = getInvoice.RDHandlingNonHzdRate;
                invoice.RefrigeratorStorageRate = getInvoice.RefrigeratorStorageRate;
                invoice.RelabelsRate = getInvoice.RelabelsRate;
                invoice.RushShipmentRate = getInvoice.RushShipmentRate;
                invoice.SPA197AppliedRate = getInvoice.SPA197AppliedRate;
                invoice.SPSPaidOrderRate = getInvoice.SPSPaidOrderRate;
                invoice.UNBoxRate = getInvoice.UNBoxRate;
                invoice.WarehouseStorageRate = getInvoice.WarehouseStorageRate;
                invoice.WHMISLabelsRate = getInvoice.WHMISLabelsRate;

                // Charges
                invoice.AirHzdOnlyCharge = getInvoice.AirHzdOnlyCharge;
                invoice.CertificateOfOriginCharge = getInvoice.CertificateOfOriginCharge;
                invoice.CMCPackCharge = getInvoice.CMCPackCharge;
                invoice.CoolPackCharge = getInvoice.CoolPackCharge;
                invoice.CreditCardFeeCharge = getInvoice.CreditCardFeeCharge;
                invoice.CreditCardOrderCharge = getInvoice.CreditCardOrderCharge;
                invoice.DocumentationHandlingCharge = getInvoice.DocumentationHandlingCharge;
                invoice.EmptyPackagingCharge = getInvoice.EmptyPackagingCharge;
                invoice.ExternalSystemCharge = getInvoice.ExternalSystemCharge;
                invoice.FollowUpOrderCharge = getInvoice.FollowUpOrderCharge;
                invoice.FreezerPackCharge = getInvoice.FreezerPackCharge;
                invoice.GHSLabelsCharge = getInvoice.GHSLabelsCharge;
                invoice.InactiveProductsCharge = getInvoice.InactiveProductsCharge;
                invoice.IsolationCharge = getInvoice.IsolationCharge;
                invoice.IsolationBoxCharge = getInvoice.IsolationBoxCharge;
                invoice.ITFeeCharge = getInvoice.ITFeeCharge;
                invoice.LabelMaintainanceCharge = getInvoice.LabelMaintainanceCharge;
                invoice.LabelStockCharge = getInvoice.LabelStockCharge;
                invoice.LabelsPrintedCharge = getInvoice.LabelsPrintedCharge;
                invoice.LaborRelabelCharge = getInvoice.LaborRelabelCharge;
                invoice.LiteratureFeeCharge = getInvoice.LiteratureFeeCharge;
                invoice.LimitedQtyCharge = getInvoice.LimitedQtyCharge;
                invoice.ManualHandlingCharge = getInvoice.ManualHandlingCharge;
                invoice.MSDSPrintsCharge = getInvoice.MSDSPrintsCharge;
                invoice.NewLabelSetupCharge = getInvoice.NewLabelSetupCharge;
                invoice.NewProductSetupCharge = getInvoice.NewProductSetupCharge;
                invoice.OberkPackCharge = getInvoice.OberkPackCharge;
                invoice.OrderEntryCharge = getInvoice.OrderEntryCharge;
                invoice.OverPackCharge = getInvoice.OverPackCharge;
                invoice.PalletReturnCharge = getInvoice.PalletReturnCharge;
                invoice.PoisonPackCharge = getInvoice.PoisonPackCharge;
                invoice.ProductSetupChangesCharge = getInvoice.ProductSetupChangesCharge;
                invoice.QCStorageCharge = getInvoice.QCStorageCharge;
                invoice.RDHandlingADRCharge = getInvoice.RDHandlingADRCharge;
                invoice.RDHandlingIATACharge = getInvoice.RDHandlingIATACharge;
                invoice.RDHandlingLQCharge = getInvoice.RDHandlingLQCharge;
                invoice.RDHandlingNonHzdCharge = getInvoice.RDHandlingNonHzdCharge;
                invoice.RefrigeratorStorageCharge = getInvoice.RefrigeratorStorageCharge;
                invoice.RelabelsCharge = getInvoice.RelabelsCharge;
                invoice.RushShipmentCharge = getInvoice.RushShipmentCharge;
                invoice.SPA197AppliedCharge = getInvoice.SPA197AppliedCharge;
                invoice.SPSPaidOrderCharge = getInvoice.SPSPaidOrderCharge;
                invoice.UNBoxCharge = getInvoice.UNBoxCharge;
                invoice.WarehouseStorageCharge = getInvoice.WarehouseStorageCharge;
                invoice.WHMISLabelsCharge = getInvoice.WHMISLabelsCharge;

                // Variable Charges
                invoice.AdministrativeWasteFeeCharge = getInvoice.AdministrativeWasteFeeCharge;
                invoice.CreditCharge = getInvoice.CreditCharge;
                invoice.CustomsDocumentsCharge = getInvoice.CustomsDocumentsCharge;
                invoice.DeliveryDutiesTaxesCharge = getInvoice.DeliveryDutiesTaxesCharge;
                invoice.DocumentsCharge = getInvoice.DocumentsCharge;
                invoice.HandlingCharge = getInvoice.HandlingCharge;
                invoice.MautFuel = getInvoice.MautFuel;
                invoice.MiscellaneousLaborCharge = getInvoice.MiscellaneousLaborCharge;
                invoice.OtherCharge = getInvoice.OtherCharge;
                invoice.WasteProcessingCharge = getInvoice.WasteProcessingCharge;
            }
            return invoice;
        }

        public static int CreateInvoice(int client, string billinggroup, DateTime startdate, DateTime enddate)
        {
                string period = startdate.ToString("MMMM") + ", " + startdate.Year;

            using (var db = new CMCSQL03Entities())
            {
                var getclient = db.tblClient.Find(client);
                int invoiceid = NewInvoiceID();

                var invoice = db.tblInvoice.Find(invoiceid);

                invoice.InvoiceDate = DateTime.UtcNow;
                invoice.Status = "New";
                invoice.CreateDate = DateTime.UtcNow;
                invoice.CreatedBy = HttpContext.Current.User.Identity.Name;
                invoice.Status = "NEW";
                invoice.UpdateDate = invoice.CreateDate;
                invoice.UpdatedBy = invoice.CreatedBy;
                invoice.ClientID = getclient.ClientID;
                invoice.ClientName = getclient.ClientName;
                invoice.BillingGroup = billinggroup;                                    // division, business unit, enduse are available entries
                invoice.WarehouseLocation = getclient.CMCLocation;
                invoice.BillTo = getclient.InvoiceAddress;
                invoice.NetTerm = String.IsNullOrEmpty(getclient.ClientNetTerm) ? "Net 30 Days" : getclient.ClientNetTerm;
                invoice.Currency = getclient.ClientCurrency;
                invoice.PONumber = "Enter PO Number";
                invoice.Tier = 1;
                invoice.InvoicePeriod = period;
                invoice.InvoiceStartDate = startdate;
                invoice.InvoiceEndDate = enddate;
                invoice.RemitTo = "<p><b>Chemical Marketing Concepts, LLC</b><br />c/o Odyssey Logistics &amp; Technology Corp<br />39 Old Ridgebury Road, N-1<br />Danbury, CT 06810</p>";

                db.SaveChanges();

                return invoice.InvoiceID;
            }
        }

        public static int SaveInvoice(InvoiceViewModel invoice)
        {
            using (var db = new CMCSQL03Entities())
            {
                var confirmVerify = false;
                if (invoice.VerifiedAccuracy == true)
                {
                    /// Add logic to check invoice status if not new or verified already
                    /// check if verified by user == created user. should be different
                    /// capture verified datetime and user identity and save to db
                    /// verified invoices should be locked from further edits.
                    invoice.Status = "VERIFIED";
                    confirmVerify = true;
                }

                // Capture user info in viewmodel
                invoice.UpdatedBy = HttpContext.Current.User.Identity.Name;
                invoice.UpdateDate = DateTime.UtcNow;

                var Invoice = db.tblInvoice.Find(invoice.InvoiceId);

                Invoice.InvoiceNumber = invoice.InvoiceId;
                Invoice.BillingGroup = invoice.BillingGroup;
                Invoice.ClientID = invoice.ClientId;
                Invoice.ClientName = invoice.ClientName;
                Invoice.CreatedBy = invoice.CreatedBy;
                Invoice.CreateDate = invoice.CreateDate;
                Invoice.UpdatedBy = invoice.UpdatedBy;
                Invoice.UpdateDate = invoice.UpdateDate;
                Invoice.InvoiceStartDate = invoice.InvoiceStartDate;
                Invoice.InvoiceEndDate = invoice.InvoiceEndDate;

                if (confirmVerify == true)
                {
                    Invoice.VerifiedAccuracy = invoice.VerifiedAccuracy;
                    Invoice.VerifiedBy = HttpContext.Current.User.Identity.Name;
                    Invoice.VerifyDate = System.DateTimeOffset.UtcNow;
                }

                Invoice.Status = invoice.Status;
                Invoice.Comments = invoice.Comments;
                Invoice.InvoiceDate = invoice.InvoiceDate;
                Invoice.InvoicePeriod = invoice.InvoicePeriod;
                Invoice.PONumber = invoice.PONumber;
                Invoice.NetTerm = invoice.NetTerm;
                Invoice.BillTo = invoice.BillTo;
                Invoice.RemitTo = invoice.RemitTo;
                Invoice.OrderType = invoice.OrderType;
                Invoice.SampleShipSameDay = invoice.SampleShipSameDay;
                Invoice.SampleShipNextDay = invoice.SampleShipNextDay;
                Invoice.SampleShipSecondDay = invoice.SampleShipSecondDay;
                Invoice.SampleShipOther = invoice.SampleShipOther;
                Invoice.TotalSamples = invoice.TotalSamples;

                decimal? grandtotal = 0;

                // Quantities
                Invoice.AirHzdOnlyQuantity = invoice.AirHzdOnlyQuantity;
                Invoice.CertificateOfOriginQuantity = invoice.CertificateOfOriginQuantity;
                Invoice.CMCPackQuantity = invoice.CMCPackQuantity;
                Invoice.CoolPackQuantity = invoice.CoolPackQuantity;
                Invoice.CreditCardFeeQuantity = invoice.CreditCardFeeQuantity;
                Invoice.CreditCardOrderQuantity = invoice.CreditCardOrderQuantity;
                Invoice.DocumentationHandlingQuantity = invoice.DocumentationHandlingQuantity;
                Invoice.EmptyPackagingQuantity = invoice.EmptyPackagingQuantity;
                Invoice.ExternalSystemQuantity = invoice.ExternalSystemQuantity;
                Invoice.FollowUpOrderQuantity = invoice.FollowUpOrderQuantity;
                Invoice.FreezerPackQuantity = invoice.FreezerPackQuantity;
                Invoice.GHSLabelsQuantity = invoice.GHSLabelsQuantity;
                Invoice.InactiveProductsQuantity = invoice.InactiveProductsQuantity;
                Invoice.IsolationQuantity = invoice.IsolationQuantity;
                Invoice.IsolationBoxQuantity = invoice.IsolationBoxQuantity;
                Invoice.ITFeeQuantity = invoice.ITFeeQuantity;
                Invoice.LabelMaintainanceQuantity = invoice.LabelMaintainanceQuantity;
                Invoice.LabelStockQuantity = invoice.LabelStockQuantity;
                Invoice.LabelsPrintedQuantity = invoice.LabelsPrintedQuantity;
                Invoice.LaborRelabelQuantity = invoice.LaborRelabelQuantity;
                Invoice.LiteratureFeeQuantity = invoice.LiteratureFeeQuantity;
                Invoice.LimitedQtyQuantity = invoice.LimitedQtyQuantity;
                Invoice.ManualHandlingQuantity = invoice.ManualHandlingQuantity;
                Invoice.MSDSPrintsQuantity = invoice.MSDSPrintsQuantity;
                Invoice.NewLabelSetupQuantity = invoice.NewLabelSetupQuantity;
                Invoice.NewProductSetupQuantity = invoice.NewProductSetupQuantity;
                Invoice.OberkPackQuantity = invoice.OberkPackQuantity;
                Invoice.OrderEntryQuantity = invoice.OrderEntryQuantity;
                Invoice.OverPackQuantity = invoice.OverPackQuantity;
                Invoice.PalletReturnQuantity = invoice.PalletReturnQuantity;
                Invoice.PoisonPackQuantity = invoice.PoisonPackQuantity;
                Invoice.ProductSetupChangesQuantity = invoice.ProductSetupChangesQuantity;
                Invoice.QCStorageQuantity = invoice.QCStorageQuantity;
                Invoice.RDHandlingADRQuantity = invoice.RDHandlingADRQuantity;
                Invoice.RDHandlingIATAQuantity = invoice.RDHandlingIATAQuantity;
                Invoice.RDHandlingLQQuantity = invoice.RDHandlingLQQuantity;
                Invoice.RDHandlingNonHzdQuantity = invoice.RDHandlingNonHzdQuantity;
                Invoice.RefrigeratorStorageQuantity = invoice.RefrigeratorStorageQuantity;
                Invoice.RelabelsQuantity = invoice.RelabelsQuantity;
                Invoice.RushShipmentQuantity = invoice.RushShipmentQuantity;
                Invoice.SPA197AppliedQuantity = invoice.SPA197AppliedQuantity;
                Invoice.SPSPaidOrderQuantity = invoice.SPSPaidOrderQuantity;
                Invoice.UNBoxQuantity = invoice.UNBoxQuantity;
                Invoice.WarehouseStorageQuantity = invoice.WarehouseStorageQuantity;
                Invoice.WHMISLabelsQuantity = invoice.WHMISLabelsQuantity;

                // Rates
                Invoice.AirHzdOnlyRate = invoice.AirHzdOnlyRate;
                Invoice.CertificateOfOriginRate = invoice.CertificateOfOriginRate;
                Invoice.CMCPackRate = invoice.CMCPackRate;
                Invoice.CoolPackRate = invoice.CoolPackRate;
                Invoice.CreditCardFeeRate = invoice.CreditCardFeeRate;
                Invoice.CreditCardOrderRate = invoice.CreditCardOrderRate;
                Invoice.DocumentationHandlingRate = invoice.DocumentationHandlingRate;
                Invoice.EmptyPackagingRate = invoice.EmptyPackagingRate;
                Invoice.ExternalSystemRate = invoice.ExternalSystemRate;
                Invoice.FollowUpOrderRate = invoice.FollowUpOrderRate;
                Invoice.FreezerPackRate = invoice.FreezerPackRate;
                Invoice.GHSLabelsRate = invoice.GHSLabelsRate;
                Invoice.InactiveProductsRate = invoice.InactiveProductsRate;
                Invoice.IsolationRate = invoice.IsolationRate;
                Invoice.IsolationBoxRate = invoice.IsolationBoxRate;
                Invoice.ITFeeRate = invoice.ITFeeRate;
                Invoice.LabelMaintainanceRate = invoice.LabelMaintainanceRate;
                Invoice.LabelStockRate = invoice.LabelStockRate;
                Invoice.LabelsPrintedRate = invoice.LabelsPrintedRate;
                Invoice.LaborRelabelRate = invoice.LaborRelabelRate;
                Invoice.LiteratureFeeRate = invoice.LiteratureFeeRate;
                Invoice.LimitedQtyRate = invoice.LimitedQtyRate;
                Invoice.ManualHandlingRate = invoice.ManualHandlingRate;
                Invoice.MSDSPrintsRate = invoice.MSDSPrintsRate;
                Invoice.NewLabelSetupRate = invoice.NewLabelSetupRate;
                Invoice.NewProductSetupRate = invoice.NewProductSetupRate;
                Invoice.OberkPackRate = invoice.OberkPackRate;
                Invoice.OrderEntryRate = invoice.OrderEntryRate;
                Invoice.OverPackRate = invoice.OverPackRate;
                Invoice.PalletReturnRate = invoice.PalletReturnRate;
                Invoice.PoisonPackRate = invoice.PoisonPackRate;
                Invoice.ProductSetupChangesRate = invoice.ProductSetupChangesRate;
                Invoice.QCStorageRate = invoice.QCStorageRate;
                Invoice.RDHandlingADRRate = invoice.RDHandlingADRRate;
                Invoice.RDHandlingIATARate = invoice.RDHandlingIATARate;
                Invoice.RDHandlingLQRate = invoice.RDHandlingLQRate;
                Invoice.RDHandlingNonHzdRate = invoice.RDHandlingNonHzdRate;
                Invoice.RefrigeratorStorageRate = invoice.RefrigeratorStorageRate;
                Invoice.RelabelsRate = invoice.RelabelsRate;
                Invoice.RushShipmentRate = invoice.RushShipmentRate;
                Invoice.SPA197AppliedRate = invoice.SPA197AppliedRate;
                Invoice.SPSPaidOrderRate = invoice.SPSPaidOrderRate;
                Invoice.UNBoxRate = invoice.UNBoxRate;
                Invoice.WarehouseStorageRate = invoice.WarehouseStorageRate;
                Invoice.WHMISLabelsRate = invoice.WHMISLabelsRate;

                // Calulated Charges
                grandtotal += Invoice.AirHzdOnlyCharge = invoice.AirHzdOnlyQuantity * invoice.AirHzdOnlyRate;
                grandtotal += Invoice.CertificateOfOriginCharge = invoice.CertificateOfOriginQuantity * invoice.CertificateOfOriginRate;
                grandtotal += Invoice.CMCPackCharge = invoice.CMCPackQuantity * invoice.CMCPackRate;
                grandtotal += Invoice.CoolPackCharge = invoice.CoolPackQuantity * invoice.CoolPackRate;
                grandtotal += Invoice.CreditCardFeeCharge = invoice.CreditCardFeeQuantity * invoice.CreditCardFeeRate;
                grandtotal += Invoice.CreditCardOrderCharge = invoice.CreditCardOrderQuantity * invoice.CreditCardOrderRate;
                grandtotal += Invoice.DocumentationHandlingCharge = invoice.DocumentationHandlingQuantity * invoice.DocumentationHandlingRate;
                grandtotal += Invoice.EmptyPackagingCharge = invoice.EmptyPackagingQuantity * invoice.EmptyPackagingRate;
                grandtotal += Invoice.ExternalSystemCharge = invoice.ExternalSystemQuantity * invoice.ExternalSystemRate;
                grandtotal += Invoice.FollowUpOrderCharge = invoice.FollowUpOrderQuantity * invoice.FollowUpOrderRate;
                grandtotal += Invoice.FreezerPackCharge = invoice.FreezerPackQuantity * invoice.FreezerPackRate;
                grandtotal += Invoice.GHSLabelsCharge = invoice.GHSLabelsQuantity * invoice.GHSLabelsRate;
                grandtotal += Invoice.InactiveProductsCharge = invoice.InactiveProductsQuantity * invoice.InactiveProductsRate;
                grandtotal += Invoice.IsolationCharge = invoice.IsolationQuantity * invoice.IsolationRate;
                grandtotal += Invoice.IsolationBoxCharge = invoice.IsolationBoxQuantity * invoice.IsolationBoxRate;
                grandtotal += Invoice.ITFeeCharge = invoice.ITFeeQuantity * invoice.ITFeeRate;
                grandtotal += Invoice.LabelMaintainanceCharge = invoice.LabelMaintainanceQuantity * invoice.LabelMaintainanceRate;
                grandtotal += Invoice.LabelStockCharge = invoice.LabelStockQuantity * invoice.LabelStockRate;
                grandtotal += Invoice.LabelsPrintedCharge = invoice.LabelsPrintedQuantity * invoice.LabelsPrintedRate;
                grandtotal += Invoice.LaborRelabelCharge = invoice.LaborRelabelQuantity * invoice.LaborRelabelRate;
                grandtotal += Invoice.LiteratureFeeCharge = invoice.LiteratureFeeQuantity * invoice.LiteratureFeeRate;
                grandtotal += Invoice.LimitedQtyCharge = invoice.LimitedQtyQuantity * invoice.LimitedQtyRate;
                grandtotal += Invoice.ManualHandlingCharge = invoice.ManualHandlingQuantity * invoice.ManualHandlingRate;
                grandtotal += Invoice.MSDSPrintsCharge = invoice.MSDSPrintsQuantity * invoice.MSDSPrintsRate;
                grandtotal += Invoice.NewLabelSetupCharge = invoice.NewLabelSetupQuantity * invoice.NewLabelSetupRate;
                grandtotal += Invoice.NewProductSetupCharge = invoice.NewProductSetupQuantity * invoice.NewProductSetupRate;
                grandtotal += Invoice.OberkPackCharge = invoice.OberkPackQuantity * invoice.OberkPackRate;
                grandtotal += Invoice.OrderEntryCharge = invoice.OrderEntryQuantity * invoice.OrderEntryRate;
                grandtotal += Invoice.OverPackCharge = invoice.OverPackQuantity * invoice.OverPackRate;
                grandtotal += Invoice.PalletReturnCharge = invoice.PalletReturnQuantity * invoice.PalletReturnRate;
                grandtotal += Invoice.PoisonPackCharge = invoice.PoisonPackQuantity * invoice.PoisonPackRate;
                grandtotal += Invoice.ProductSetupChangesCharge = invoice.ProductSetupChangesQuantity * invoice.ProductSetupChangesRate;
                grandtotal += Invoice.QCStorageCharge = invoice.QCStorageQuantity * invoice.QCStorageRate;
                grandtotal += Invoice.RDHandlingADRCharge = invoice.RDHandlingADRQuantity * invoice.RDHandlingADRRate;
                grandtotal += Invoice.RDHandlingIATACharge = invoice.RDHandlingIATAQuantity * invoice.RDHandlingIATARate;
                grandtotal += Invoice.RDHandlingLQCharge = invoice.RDHandlingLQQuantity * invoice.RDHandlingLQRate;
                grandtotal += Invoice.RDHandlingNonHzdCharge = invoice.RDHandlingNonHzdQuantity * invoice.RDHandlingNonHzdRate;
                grandtotal += Invoice.RefrigeratorStorageCharge = invoice.RefrigeratorStorageQuantity * invoice.RefrigeratorStorageRate;
                grandtotal += Invoice.RelabelsCharge = invoice.RelabelsQuantity * invoice.RelabelsRate;
                grandtotal += Invoice.RushShipmentCharge = invoice.RushShipmentQuantity * invoice.RushShipmentRate;
                grandtotal += Invoice.SPA197AppliedCharge = invoice.SPA197AppliedQuantity * invoice.SPA197AppliedRate;
                grandtotal += Invoice.SPSPaidOrderCharge = invoice.SPSPaidOrderQuantity * invoice.SPSPaidOrderRate;
                grandtotal += Invoice.UNBoxCharge = invoice.UNBoxQuantity * invoice.UNBoxRate;
                grandtotal += Invoice.WarehouseStorageCharge = invoice.WarehouseStorageQuantity * invoice.WarehouseStorageRate;
                grandtotal += Invoice.WHMISLabelsCharge = invoice.WHMISLabelsQuantity * invoice.WHMISLabelsRate;

                // Variables Charges
                grandtotal += Invoice.AdministrativeWasteFeeCharge = invoice.AdministrativeWasteFeeCharge;
                grandtotal += Invoice.CreditCharge = invoice.CreditCharge;
                grandtotal += Invoice.CustomsDocumentsCharge = invoice.CustomsDocumentsCharge;
                grandtotal += Invoice.DeliveryDutiesTaxesCharge = invoice.DeliveryDutiesTaxesCharge;
                grandtotal += Invoice.DocumentsCharge = invoice.DocumentsCharge;
                grandtotal += Invoice.HandlingCharge = invoice.HandlingCharge;
                grandtotal += Invoice.MautFuel = invoice.MautFuel;
                grandtotal += Invoice.MiscellaneousLaborCharge = invoice.MiscellaneousLaborCharge;
                grandtotal += Invoice.OtherCharge = invoice.OtherCharge;
                grandtotal += Invoice.WasteProcessingCharge = invoice.WasteProcessingCharge;

                Invoice.GrandTotal = grandtotal;
                Invoice.TotalServiceCharge = grandtotal;

                Invoice.TotalCostSamples = invoice.TotalCostSamples;
                Invoice.TotalFreight = invoice.TotalFreight;
                Invoice.TotalFrtHzdSchg = invoice.TotalFrtHzdSchg;
                Invoice.TotalDue = invoice.TotalCostSamples + invoice.TotalFreight + invoice.TotalFrtHzdSchg + grandtotal;

                db.SaveChanges();
            }
            
            return invoice.InvoiceId;
        }

        public static int GenerateInvoice(int invoiceid)
        {
            int clientId = 0;
            int divisionId = 0;
            string billingGroup;
            int? sampleItemsCount = 0;
            string[] sampleTransTypes = SampleTransTypes();
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = DateTime.UtcNow;
            Random random = new Random();
            decimal? grandTotal = 0;
            decimal? sumSampleCharges;
            decimal? calcFreightCharges;
            decimal? calcServiceCharges;
            decimal? sumChargesDue;

            using (var db = new CMCSQL03Entities())
            {
                // Get invoice info
                var invoice = db.tblInvoice.Find(invoiceid);

                clientId = Convert.ToInt32(invoice.ClientID);
                startDate = Convert.ToDateTime(invoice.InvoiceStartDate);
                endDate = Convert.ToDateTime(invoice.InvoiceEndDate);

                // Check input for type and determine whether billing group is division or enduse
                int n;
                bool isDivision = int.TryParse(invoice.BillingGroup, out n);
                
                billingGroup = "ALL";

                if (isDivision == true)
                {
                    divisionId = Convert.ToInt32(invoice.BillingGroup);
                    var getDivision = db.tblDivision.Find(divisionId);

                    billingGroup = getDivision.DivisionName + " / " + getDivision.BusinessUnit;
                }

                if (isDivision == false)
                {
                    billingGroup = invoice.BillingGroup;
                }

                // Get client info
                var client = db.tblClient.Find(clientId);

                // Get predetermined rates set for client.
                var servicerates = (from t in db.tblRates
                                    where t.ClientID == clientId
                                    select t).FirstOrDefault();

                // Get order items for client within date range.
                var orderitems = (from t in db.tblOrderItem
                                  join o in db.tblOrderMaster on t.OrderID equals o.OrderID
                                  where o.ClientID == clientId
                                  && (t.ShipDate >= startDate && t.ShipDate <= endDate)
                                  select t).ToList();

                sampleItemsCount = orderitems.Sum(x => x.Qty);

                // Get order transactions within date range.
                var transactions = (from t in db.tblOrderTrans
                                    where t.ClientID == clientId
                                    && (t.TransDate >= startDate && t.TransDate <= endDate)
                                    select t).ToList();

                var samplechargetrans = transactions.Where(x => sampleTransTypes.Contains(x.TransType)).ToList();

                sumSampleCharges = samplechargetrans.Sum(x => x.TransQty * x.TransRate);

                // Bypass business rules within condition if 'ALL' is selected
                if (billingGroup != "ALL")
                {
                    // Filter transactions by billing group if division/businessunit is selected
                    if (isDivision == true)
                    {
                        var billinggrouptransactions = transactions.Where(x => x.DivisionID == divisionId).ToList();

                        transactions = billinggrouptransactions;

                        // Filter order items by divisionid, count, and sum up the sample charges
                        var sampleitems = (from orderitem in orderitems
                                           join order in db.tblOrderMaster on orderitem.OrderID equals order.OrderID
                                           where order.DivisionID == divisionId
                                           select orderitem).ToList();

                        sampleItemsCount = sampleitems.Sum(x => x.Qty);

                        samplechargetrans = (from trans in transactions
                                             join orderitem in orderitems on trans.OrderItemID equals orderitem.ItemID
                                             where sampleTransTypes.Contains(trans.TransType)
                                             select trans).ToList();
                        
                        // Calculate sample charges using billing rate by adjusted tiers
                        sumSampleCharges = samplechargetrans.Sum(x => x.TransQty * x.BillingRate);
                    }

                    // Filter orders by enduse if enduse is selected and match orders to transactions
                    if (isDivision == false)
                    {
                        string enduse = billingGroup;

                        var enduseorders = (from t in db.tblOrderMaster
                                            where t.ClientID == clientId
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

                        sampleItemsCount = sampleitems.Sum(x => x.Qty);

                        samplechargetrans = transactions.Where(x => sampleTransTypes.Contains(x.TransType)).ToList();
                        
                        // Calculate sample charges using billing rate by adjusted tiers
                        sumSampleCharges = samplechargetrans.Sum(x => x.TransQty * x.BillingRate);
                    }
                }

                // TODO: Calculate Freight Charges
                calcFreightCharges = 0;                                                                         // random.Next(100, 999);

                // TODO: Calculate Freight Hazard Surcharge
                var calcfreighthazdsurcharges = 0;                                                              // random.Next(100, 999);

                // TODO: Calculate shipping performance.
                // Is shipping calculated per sample or per order?
                int? countorderitems = sampleItemsCount;
                invoice.SampleShipSameDay = sampleItemsCount;                                                   // (int)(sampleitemscount * 0.5);
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
                grandTotal += invoice.AirHzdOnlyCharge = invoice.AirHzdOnlyQuantity * invoice.AirHzdOnlyRate;
                grandTotal += invoice.CertificateOfOriginCharge = invoice.CertificateOfOriginQuantity * invoice.CertificateOfOriginRate;
                grandTotal += invoice.CMCPackCharge = invoice.CMCPackQuantity * invoice.CMCPackRate;
                grandTotal += invoice.CoolPackCharge = invoice.CoolPackQuantity * invoice.CoolPackRate;
                grandTotal += invoice.CreditCardFeeCharge = invoice.CreditCardFeeQuantity * invoice.CreditCardFeeRate;
                grandTotal += invoice.CreditCardOrderCharge = invoice.CreditCardOrderQuantity * invoice.CreditCardOrderRate;
                grandTotal += invoice.DocumentationHandlingCharge = invoice.DocumentationHandlingQuantity * invoice.DocumentationHandlingRate;
                grandTotal += invoice.EmptyPackagingCharge = invoice.EmptyPackagingQuantity * invoice.EmptyPackagingRate;
                grandTotal += invoice.ExternalSystemCharge = invoice.ExternalSystemQuantity * invoice.ExternalSystemRate;
                grandTotal += invoice.FollowUpOrderCharge = invoice.FollowUpOrderQuantity * invoice.FollowUpOrderRate;
                grandTotal += invoice.FreezerPackCharge = invoice.FreezerPackQuantity * invoice.FreezerPackRate;
                grandTotal += invoice.GHSLabelsCharge = invoice.GHSLabelsQuantity * invoice.GHSLabelsRate;
                grandTotal += invoice.InactiveProductsCharge = invoice.InactiveProductsQuantity * invoice.InactiveProductsRate;
                grandTotal += invoice.IsolationCharge = invoice.IsolationQuantity * invoice.IsolationRate;
                grandTotal += invoice.IsolationBoxCharge = invoice.IsolationBoxQuantity * invoice.IsolationBoxRate;
                grandTotal += invoice.ITFeeCharge = invoice.ITFeeQuantity * invoice.ITFeeRate;
                grandTotal += invoice.LabelMaintainanceCharge = invoice.LabelMaintainanceQuantity * invoice.LabelMaintainanceRate;
                grandTotal += invoice.LabelStockCharge = invoice.LabelStockQuantity * invoice.LabelStockRate;
                grandTotal += invoice.LabelsPrintedCharge = invoice.LabelsPrintedQuantity * invoice.LabelsPrintedRate;
                grandTotal += invoice.LaborRelabelCharge = invoice.LaborRelabelQuantity * invoice.LaborRelabelRate;
                grandTotal += invoice.LiteratureFeeCharge = invoice.LiteratureFeeQuantity * invoice.LiteratureFeeRate;
                grandTotal += invoice.LimitedQtyCharge = invoice.LimitedQtyQuantity * invoice.LimitedQtyRate;
                grandTotal += invoice.ManualHandlingCharge = invoice.ManualHandlingQuantity * invoice.ManualHandlingRate;
                grandTotal += invoice.MSDSPrintsCharge = invoice.MSDSPrintsQuantity * invoice.MSDSPrintsRate;
                grandTotal += invoice.NewLabelSetupCharge = invoice.NewLabelSetupQuantity * invoice.NewLabelSetupRate;
                grandTotal += invoice.NewProductSetupCharge = invoice.NewProductSetupQuantity * invoice.NewProductSetupRate;
                grandTotal += invoice.OberkPackCharge = invoice.OberkPackQuantity * invoice.OberkPackRate;
                grandTotal += invoice.OrderEntryCharge = invoice.OrderEntryQuantity * invoice.OrderEntryRate;
                grandTotal += invoice.OverPackCharge = invoice.OverPackQuantity * invoice.OverPackRate;
                grandTotal += invoice.PalletReturnCharge = invoice.PalletReturnQuantity * invoice.PalletReturnRate;
                grandTotal += invoice.PoisonPackCharge = invoice.PoisonPackQuantity * invoice.PoisonPackRate;
                grandTotal += invoice.ProductSetupChangesCharge = invoice.ProductSetupChangesQuantity * invoice.ProductSetupChangesRate;
                grandTotal += invoice.QCStorageCharge = invoice.QCStorageQuantity * invoice.QCStorageRate;
                grandTotal += invoice.RDHandlingADRCharge = invoice.RDHandlingADRQuantity * invoice.RDHandlingADRRate;
                grandTotal += invoice.RDHandlingIATACharge = invoice.RDHandlingIATAQuantity * invoice.RDHandlingIATARate;
                grandTotal += invoice.RDHandlingLQCharge = invoice.RDHandlingLQQuantity * invoice.RDHandlingLQRate;
                grandTotal += invoice.RDHandlingNonHzdCharge = invoice.RDHandlingNonHzdQuantity * invoice.RDHandlingNonHzdRate;
                grandTotal += invoice.RefrigeratorStorageCharge = invoice.RefrigeratorStorageQuantity * invoice.RefrigeratorStorageRate;
                grandTotal += invoice.RelabelsCharge = invoice.RelabelsQuantity * invoice.RelabelsRate;
                grandTotal += invoice.RushShipmentCharge = invoice.RushShipmentQuantity * invoice.RushShipmentRate;
                grandTotal += invoice.SPA197AppliedCharge = invoice.SPA197AppliedQuantity * invoice.SPA197AppliedRate;
                grandTotal += invoice.SPSPaidOrderCharge = invoice.SPSPaidOrderQuantity * invoice.SPSPaidOrderRate;
                grandTotal += invoice.UNBoxCharge = invoice.UNBoxQuantity * invoice.UNBoxRate;
                grandTotal += invoice.WarehouseStorageCharge = invoice.WarehouseStorageQuantity * invoice.WarehouseStorageRate;
                grandTotal += invoice.WHMISLabelsCharge = invoice.WHMISLabelsQuantity * invoice.WHMISLabelsRate;

                // Variables charges by group summing amounts with corresponding transaction types.
                // Aggregate sum into grandtotal.
                grandTotal += invoice.AdministrativeWasteFeeCharge = getservicetransactions.Where(j => j.TransType == "Administrative Waste Fee").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.CreditCharge = getservicetransactions.Where(j => j.TransType == "Credit").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.CustomsDocumentsCharge = getservicetransactions.Where(j => j.TransType == "Customs Documents").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.DeliveryDutiesTaxesCharge = getservicetransactions.Where(j => j.TransType == "Delivery Duties Taxes").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.DocumentsCharge = getservicetransactions.Where(j => j.TransType == "Documents").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.HandlingCharge = getservicetransactions.Where(j => j.TransType == "Handling").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.MautFuel = getservicetransactions.Where(j => j.TransType == "MautFuel").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.MiscellaneousLaborCharge = getservicetransactions.Where(j => j.TransType == "Miscellaneous Labor").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.OtherCharge = getservicetransactions.Where(j => j.TransType == "Other").Select(i => i.TransAmount).Sum();
                grandTotal += invoice.WasteProcessingCharge = getservicetransactions.Where(j => j.TransType == "Waste Processing").Select(i => i.TransAmount).Sum();

                invoice.GrandTotal = grandTotal;

                // Calculate all charges due.
                sumChargesDue = sumSampleCharges + calcFreightCharges + calcfreighthazdsurcharges + grandTotal;

                // Fill summary fields.
                invoice.TotalSamples = sampleItemsCount;
                invoice.TotalCostSamples = sumSampleCharges;
                invoice.TotalFreight = calcFreightCharges;                                                  // TODO: Random for now
                invoice.TotalFrtHzdSchg = calcfreighthazdsurcharges;                                        // TODO: Random for now
                invoice.TotalServiceCharge = grandTotal;
                invoice.TotalDue = sumChargesDue;

                // Revisit
                invoice.BillingGroup = billingGroup;

                db.SaveChanges();

                return invoiceid;
            }
        }

        private static string[] SampleTransTypes()
        {
            string[] types = { "SAMP",
                               "HAZD", 
                               "FLAM", 
                               "HEAT", 
                               "REFR", 
                               "FREZ", 
                               "CLEN", 
                               "BLND", 
                               "NALG", 
                               "NITR", 
                               "BIOC", 
                               "KOSH", 
                               "LABL" };
            return types;
        }

        public static int NewInvoiceID()
        {
            using (var db = new CMCSQL03Entities())
            {
                tblInvoice invoice = new tblInvoice();
                db.tblInvoice.Add(invoice);

                db.SaveChanges();

                return invoice.InvoiceID;
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
                            var xdate = DateTime.UtcNow.ToString("dd/MM/yyyy");

                            var updatebilling = db.tblOrderTrans.Find(xtransid);

                            updatebilling.BillingTier = xtier;
                            updatebilling.BillingRate = xrate;
                            updatebilling.BillingCharge = xcharge;
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