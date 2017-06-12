using MvcPhoenix.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    public class InvoiceService
    {
        // List for the Index View
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
                obj.InvoicePeriod = q.InvoicePeriod;        //extract month and year from invoice date convert to string
                obj.InvoiceStartDate = q.InvoiceStartDate;
                obj.InvoiceEndDate = q.InvoiceEndDate;
                obj.PONumber = q.PONumber;
                obj.NetTerm = q.NetTerm;
                obj.BillTo = q.BillTo;
                obj.RemitTo = q.RemitTo;
                obj.Currency = q.Currency;                  // USD / EUR / CNY
                obj.Tier = q.Tier;
                obj.OrderType = q.OrderType;                // Sample/International/Revenue does not apply to all clients

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
                obj.TotalAdminCharge = q.TotalAdminCharge;
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

        public static int CreateInvoice(int client, int division, DateTime startdate, DateTime enddate)
        {
            using (var db = new CMCSQL03Entities())
            {
                var cl = db.tblClient.Find(client);
                var div = db.tblDivision.Find(division);
                string period = startdate.ToString("MMMM") + ", " + startdate.Year;

                int invoiceid = NewInvoiceID();
                var obj = (from t in db.tblInvoice
                           where t.InvoiceID == invoiceid
                           select t).FirstOrDefault();

                obj.InvoiceDate = DateTime.UtcNow;
                obj.Status = "New";

                if (division > 0)
                {
                    obj.BillingGroup = div.DivisionName;
                }
                else
                {
                    obj.BillingGroup = "All";                                   // TODO: Address this condition later in GenerateInvoice method
                }

                obj.CreateDate = DateTime.UtcNow;
                obj.CreatedBy = HttpContext.Current.User.Identity.Name;
                obj.Status = "NEW";
                obj.UpdateDate = obj.CreateDate;
                obj.UpdatedBy = obj.CreatedBy;

                obj.ClientID = cl.ClientID;
                obj.ClientName = cl.ClientName;
                obj.WarehouseLocation = cl.CMCLocation;
                obj.BillTo = cl.InvoiceAddress;
                obj.NetTerm = String.IsNullOrEmpty(cl.ClientNetTerm) ? "Net 30 Days" : cl.ClientNetTerm;
                obj.Currency = cl.ClientCurrency;
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
                q.WarehouseLocation = vm.WarehouseLocation;
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
                q.Currency = vm.Currency;
                q.Tier = vm.Tier;
                q.OrderType = vm.OrderType;
                q.SampleShipSameDay = vm.SampleShipSameDay;
                q.SampleShipNextDay = vm.SampleShipNextDay;
                q.SampleShipSecondDay = vm.SampleShipSecondDay;
                q.SampleShipOther = vm.SampleShipOther;
                q.TotalSamples = vm.TotalSamples;
                q.TotalCostSamples = vm.TotalCostSamples;
                q.TotalFreight = vm.TotalFreight;
                q.TotalFrtHzdSchg = vm.TotalFrtHzdSchg;
                //q.TotalAdminCharge = vm.totaladmincharge;
                q.TotalDue = vm.TotalDue;

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
                q.TotalAdminCharge = grandtotal;

                db.SaveChanges();

                return vm.InvoiceId;
            }
        }

        public static int GenerateInvoice(int invoiceid)
        {
            int clientid = 0;
            DateTime startdate = DateTime.UtcNow;
            DateTime enddate = DateTime.UtcNow;
            string[] sampletranstypes = { "SAMP", "HAZD", "FLAM", "HEAT", "REFR", "FREZ", "CLEN", "BLND", "NALG", "NITR", "BIOC", "KOSH", "LABL" };
            Random random = new Random();
            decimal? grandtotal = 0;

            using (var db = new CMCSQL03Entities())
            {
                var invoice = (from t in db.tblInvoice
                               where t.InvoiceID == invoiceid
                               select t).FirstOrDefault();

                clientid = Convert.ToInt32(invoice.ClientID);
                startdate = Convert.ToDateTime(invoice.InvoiceStartDate);
                enddate = Convert.ToDateTime(invoice.InvoiceEndDate);

                var client = (from t in db.tblClient
                              where t.ClientID == clientid
                              select t).FirstOrDefault();

                var orderitems = (from t in db.tblOrderItem
                                  join o in db.tblOrderMaster on t.OrderID equals o.OrderID
                                  where o.ClientID == clientid
                                  && (t.ShipDate >= startdate && t.ShipDate <= enddate)
                                  select t).ToList();

                var adminrate = (from t in db.tblRates
                                 where t.ClientID == clientid
                                 select t).FirstOrDefault();

                var transactions = (from t in db.tblOrderTrans
                                    where t.ClientID == clientid
                                    && (t.TransDate >= startdate && t.TransDate <= enddate)
                                    select t).ToList();

                // Calculate Sample Related Charges
                var samplecharges = (from t in transactions
                                     where sampletranstypes.Contains(t.TransType)
                                     select new
                                     {
                                         t.TransQty,
                                         t.TransRate
                                     }).ToList();

                var calcsamplecharges = (from t in samplecharges
                                         select (t.TransQty * t.TransRate)).Sum();

                // Calculate Freight Charges
                // TODO
                var calcfreightcharges = random.Next(100, 999);

                // Calculate Freight Hazard Surcharge
                // TODO
                var calcfreighthazdsurcharges = random.Next(100, 999);

                // Calculate Administrative Charges
                var servicecharges = (from t in transactions
                                      where !sampletranstypes.Contains(t.TransType)
                                      select new
                                      {
                                          t.TransQty,
                                          t.TransRate
                                      }).ToList();

                var calcservicecharges = (from t in servicecharges
                                          select (t.TransQty * t.TransRate)).Sum();

                // Calculate All Charges
                var calcsumcharges = calcsamplecharges + calcfreightcharges + calcfreighthazdsurcharges + calcservicecharges;

                // Calculate Shipping Performance
                // TODO
                // Is shipping calculated per sample or per order?
                int countorderitems = orderitems.Count();
                var calcsamedayshipping = (int)(countorderitems * 0.70);
                var calcnextdayshipping = (int)(countorderitems * 0.25);
                var calcseconddayshipping = (int)(countorderitems * 0.04);
                var calcothershipping = (int)(countorderitems * 0.01);

                // Fill table fields
                invoice.TotalSamples = orderitems.Count();
                invoice.TotalCostSamples = calcsamplecharges;
                invoice.TotalFreight = calcfreightcharges;                          // TODO: Random for now
                invoice.TotalFrtHzdSchg = calcfreighthazdsurcharges;                // TODO: Random for now
                invoice.TotalAdminCharge = calcservicecharges;
                invoice.TotalDue = calcsumcharges;
                invoice.SampleShipSameDay = calcsamedayshipping;
                invoice.SampleShipNextDay = calcnextdayshipping;
                invoice.SampleShipSecondDay = calcseconddayshipping;
                invoice.SampleShipOther = calcothershipping;

                // Quantities
                invoice.AirHzdOnlyQuantity = 0;
                invoice.CertificateOfOriginQuantity = 0;
                invoice.CMCPackQuantity = 0;
                invoice.CoolPackQuantity = 0;
                invoice.CreditCardFeeQuantity = 0;
                invoice.CreditCardOrderQuantity = 0;
                invoice.DocumentationHandlingQuantity = 0;
                invoice.EmptyPackagingQuantity = 0;
                invoice.ExternalSystemQuantity = 0;
                invoice.FollowUpOrderQuantity = 0;
                invoice.FreezerPackQuantity = 0;
                invoice.GHSLabelsQuantity = 0;
                invoice.InactiveProductsQuantity = 0;
                invoice.IsolationQuantity = 0;
                invoice.IsolationBoxQuantity = 0;
                invoice.ITFeeQuantity = 0;
                invoice.LabelMaintainanceQuantity = 0;
                invoice.LabelStockQuantity = 0;
                invoice.LabelsPrintedQuantity = 0;
                invoice.LaborRelabelQuantity = 0;
                invoice.LiteratureFeeQuantity = 0;
                invoice.LimitedQtyQuantity = 0;
                invoice.ManualHandlingQuantity = 0;
                invoice.MSDSPrintsQuantity = 0;
                invoice.NewLabelSetupQuantity = 0;
                invoice.NewProductSetupQuantity = 0;
                invoice.OberkPackQuantity = 0;
                invoice.OrderEntryQuantity = 0;
                invoice.OverPackQuantity = 0;
                invoice.PalletReturnQuantity = 0;
                invoice.PoisonPackQuantity = 0;
                invoice.ProductSetupChangesQuantity = 0;
                invoice.QCStorageQuantity = 0;
                invoice.RDHandlingADRQuantity = 0;
                invoice.RDHandlingIATAQuantity = 0;
                invoice.RDHandlingLQQuantity = 0;
                invoice.RDHandlingNonHzdQuantity = 0;
                invoice.RefrigeratorStorageQuantity = 0;
                invoice.RelabelsQuantity = 0;
                invoice.RushShipmentQuantity = 0;
                invoice.SPA197AppliedQuantity = 0;
                invoice.SPSPaidOrderQuantity = 0;
                invoice.UNBoxQuantity = 0;
                invoice.WarehouseStorageQuantity = 0;
                invoice.WHMISLabelsQuantity = 0;

                // Rates
                invoice.AirHzdOnlyRate = adminrate.AirHzdOnlyRate ?? 1;
                invoice.CertificateOfOriginRate = adminrate.CertificateOfOriginRate ?? 1;
                invoice.CMCPackRate = adminrate.CMCPackRate ?? 1;
                invoice.CoolPackRate = adminrate.CoolPackRate ?? 1;
                invoice.CreditCardFeeRate = adminrate.CreditCardFeeRate ?? 1;
                invoice.CreditCardOrderRate = adminrate.CreditCardOrderRate ?? 1;
                invoice.DocumentationHandlingRate = adminrate.DocumentationHandlingRate ?? 1;
                invoice.EmptyPackagingRate = adminrate.EmptyPackagingRate ?? 1;
                invoice.ExternalSystemRate = adminrate.ExternalSystemRate ?? 1;
                invoice.FollowUpOrderRate = adminrate.FollowUpOrderRate ?? 1;
                invoice.FreezerPackRate = adminrate.FreezerPackRate ?? 1;
                invoice.GHSLabelsRate = adminrate.GHSLabelsRate ?? 1;
                invoice.InactiveProductsRate = adminrate.InactiveProductsRate ?? 1;
                invoice.IsolationRate = adminrate.IsolationRate ?? 1;
                invoice.IsolationBoxRate = adminrate.IsolationBoxRate ?? 1;
                invoice.ITFeeRate = adminrate.ITFeeRate ?? 1;
                invoice.LabelMaintainanceRate = adminrate.LabelMaintainanceRate ?? 1;
                invoice.LabelStockRate = adminrate.LabelStockRate ?? 1;
                invoice.LabelsPrintedRate = adminrate.LabelsPrintedRate ?? 1;
                invoice.LaborRelabelRate = adminrate.LaborRelabelRate ?? 1;
                invoice.LiteratureFeeRate = adminrate.LiteratureFeeRate ?? 1;
                invoice.LimitedQtyRate = adminrate.LimitedQtyRate ?? 1;
                invoice.ManualHandlingRate = adminrate.ManualHandlingRate ?? 1;
                invoice.MSDSPrintsRate = adminrate.MSDSPrintsRate ?? 1;
                invoice.NewLabelSetupRate = adminrate.NewLabelSetupRate ?? 1;
                invoice.NewProductSetupRate = adminrate.NewProductSetupRate ?? 1;
                invoice.OberkPackRate = adminrate.OberkPackRate ?? 1;
                invoice.OrderEntryRate = adminrate.OrderEntryRate ?? 1;
                invoice.OverPackRate = adminrate.OverPackRate ?? 1;
                invoice.PalletReturnRate = adminrate.PalletReturnRate ?? 1;
                invoice.PoisonPackRate = adminrate.PoisonPackRate ?? 1;
                invoice.ProductSetupChangesRate = adminrate.ProductSetupChangesRate ?? 1;
                invoice.QCStorageRate = adminrate.QCStorageRate ?? 1;
                invoice.RDHandlingADRRate = adminrate.RDHandlingADRRate ?? 1;
                invoice.RDHandlingIATARate = adminrate.RDHandlingIATARate ?? 1;
                invoice.RDHandlingLQRate = adminrate.RDHandlingLQRate ?? 1;
                invoice.RDHandlingNonHzdRate = adminrate.RDHandlingNonHzdRate ?? 1;
                invoice.RefrigeratorStorageRate = adminrate.RefrigeratorStorageRate ?? 1;
                invoice.RelabelsRate = adminrate.RelabelsRate ?? 1;
                invoice.RushShipmentRate = adminrate.RushShipmentRate ?? 1;
                invoice.SPA197AppliedRate = adminrate.SPA197AppliedRate ?? 1;
                invoice.SPSPaidOrderRate = adminrate.SPSPaidOrderRate ?? 1;
                invoice.UNBoxRate = adminrate.UNBoxRate ?? 1;
                invoice.WarehouseStorageRate = adminrate.WarehouseStorageRate ?? 1;
                invoice.WHMISLabelsRate = adminrate.WHMISLabelsRate ?? 1;

                // Calulated Charges
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

                // Variables Charges
                grandtotal += invoice.AdministrativeWasteFeeCharge = 0;
                grandtotal += invoice.CreditCharge = 0;
                grandtotal += invoice.CustomsDocumentsCharge = 0;
                grandtotal += invoice.DeliveryDutiesTaxesCharge = 0;
                grandtotal += invoice.DocumentsCharge = 0;
                grandtotal += invoice.HandlingCharge = 0;
                grandtotal += invoice.MautFuel = 0;
                grandtotal += invoice.MiscellaneousLaborCharge = 0;
                grandtotal += invoice.OtherCharge = 0;
                grandtotal += invoice.WasteProcessingCharge = 0;

                invoice.GrandTotal = grandtotal;
                invoice.TotalAdminCharge = grandtotal;

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

        # region Review Tier Pricing before deleting

        //public static InvoiceViewModel CreateInvoice(int client, int division, string period, DateTime startdate, DateTime enddate)
        //{
        //    InvoiceViewModel obj = new InvoiceViewModel();

        //    using (var db = new CMCSQL03Entities())
        //    {
        //        obj.invoiceid = NewInvoiceID();
        //        obj.invoicedate = DateTime.UtcNow;
        //        obj.status = "New";

        //        if (division > 0)
        //        {
        //            var div = db.tblDivision.Find(division);
        //            obj.billinggroup = div.DivisionName;
        //        }
        //        else
        //        {
        //            obj.billinggroup = "All";
        //        }

        //        // Pull default values from Client table
        //        var cl = db.tblClient.Find(client);
        //        obj.clientid = cl.ClientID;
        //        obj.clientname = cl.ClientName;
        //        obj.warehouselocation = cl.CMCLocation;
        //        obj.billto = cl.InvoiceAddress;
        //        obj.netterm = String.IsNullOrEmpty(cl.ClientNetTerm) ? "Net 30 Days" : cl.ClientNetTerm;
        //        obj.currency = cl.ClientCurrency;
        //        obj.ponumber = "Enter PO Number";
        //        obj.tier = 1;
        //        obj.invoiceperiod = period;
        //        obj.invoicestartdate = startdate;
        //        obj.invoiceenddate = enddate;
        //        obj.remitto = "<p>Chemical Marketing Concepts, LLC<br />c/o Odyssey Logistics &amp; Technology Corp<br />39 Old Ridgebury Road, N-1<br />Danbury, CT 06810</p>";

        //        // Pull default values from Rates table
        //        //var rate = (from t in db.tblRates
        //        //            where t.ClientID == 1
        //        //            select t).FirstOrDefault();

        //        //obj.revenuerate = rate.RevenueRate;
        //        //obj.nonrevenuerate = rate.NonRevenueRate;
        //        //obj.manualentryrate = rate.ManualEntryRate;
        //        //obj.followuprate = rate.FollowUpRate;
        //        //obj.labelprtrate = rate.LabelPrtRate;
        //        //obj.relabelprtrate = rate.ReLabelPrtRate;
        //        //obj.relabelfeerate = rate.ReLabelFeeRate;
        //        //obj.productsetuprate = rate.ProductSetupRate;
        //        //obj.ccprocessrate = rate.CCProcessRate;
        //        //obj.rushshiprate = rate.RushShipRate;
        //        //obj.emptypailsrate = rate.EmptyPailsRate;
        //        //obj.inactivestockrate = rate.InactiveStockRate;
        //        //obj.minimalsamplecharge = rate.MinimalSampleCharge;

        //        //UpdateTierPricing(obj, startdate, enddate);                         // go update tier pricing
        //        //obj = GetOrderTransCharges(obj, startdate, enddate);                // now go read tblOrderTrans fill in some properties for the system generated trans

        //        GenerateInvoice(obj.invoiceid);

        //        return obj;
        //    }
        //}

        //public static void UpdateTierPricing(InvoiceViewModel obj, DateTime startdate, DateTime enddate)
        //{
        //    // Look at each tier record
        //    // Go get shipped items for this invoice client and ship date range
        //    // add up qty shipped
        //    // adjust tblOrderTrans.TransRate accordingly
        //    using (var db = new CMCSQL03Entities())
        //    {
        //        // collect all Tier records for a client
        //        var qTiers = (from t in db.tblTier
        //                      where t.ClientID == obj.clientid
        //                      && t.TierLevel != 1
        //                      orderby t.TierLevel
        //                      select t).ToList();

        //        // For each Tier, go see if shipments in the date range qualify for Tier change
        //        foreach (var row in qTiers)
        //        {
        //            var qitems = (from t in db.tblOrderItem
        //                          join order in db.tblOrderMaster on t.OrderID equals order.OrderID
        //                          where order.ClientID == obj.clientid
        //                          && t.ShipDate >= startdate
        //                          && t.ShipDate <= enddate
        //                          select new
        //                          {
        //                              t.Qty,
        //                              t.Size,
        //                              t.ShipDate,
        //                              order.ClientID,
        //                              order.BillingGroup
        //                          }).ToList();

        //            if (obj.billinggroup != "All")
        //            {
        //                qitems = (from t in qitems
        //                          where t.BillingGroup == obj.billinggroup
        //                          select t).ToList();
        //            }

        //            var qitemsSum = (from t in qitems
        //                             where t.Size == row.Size
        //                             select t.Qty).Sum();

        //            if (qitemsSum.Value >= row.LoSampAmt)
        //            {
        //                // TODO confirm date formats coincede
        //                var qUpdate = (from t in db.tblOrderTrans
        //                               join oi in db.tblOrderItem on t.OrderItemID equals oi.ItemID
        //                               where t.ClientID == row.ClientID
        //                               && oi.ShipDate >= startdate
        //                               && oi.ShipDate <= enddate
        //                               && oi.Size == row.Size
        //                               select new
        //                               {
        //                                   t.OrderTransID,
        //                                   t.OrderItemID,
        //                                   t.TransRate
        //                               }).ToList();

        //                foreach (var trans in qUpdate)
        //                {
        //                    // Do the price update
        //                    string s = String.Format("Update tblOrderTrans Set TransRate = {0},UpdateDate=getdate(),updateUser='System' where OrderTransID={1}", row.Price, trans.OrderTransID);
        //                    db.Database.ExecuteSqlCommand(s);
        //                }
        //            }
        //        }
        //    }
        //}

        //public static InvoiceViewModel GetOrderTransCharges(InvoiceViewModel obj, DateTime startdate, DateTime enddate)
        //{
        //    using (var db = new CMCSQL03Entities())
        //    {
        //        //init
        //        obj.totalcostsamples = 0;
        //        obj.totalfreight = 0;
        //        obj.totalfrtHzdSchg = 0;
        //        obj.totaladmincharge = 0;
        //        obj.totaldue = 0;
        //        obj.sampleshipsameday = 0;
        //        obj.sampleshipnextday = 0;
        //        obj.sampleshipsecondday = 0;
        //        obj.sampleshipother = 0;

        //        // number of samples shipped - need to look in tblOrderItem, but would prefer a TransType=SHIP ???
        //        var qShippedCount = (from t in db.tblOrderItem
        //                             join order in db.tblOrderMaster on t.OrderID equals order.OrderID
        //                             where order.ClientID == obj.clientid
        //                             && t.ShipDate != null
        //                             && (t.ShipDate >= startdate
        //                             && t.ShipDate <= enddate)
        //                             select new
        //                             {
        //                                 t.Qty,
        //                                 order.BillingGroup
        //                             }).ToList();

        //        if (obj.billinggroup != "All")
        //        {
        //            qShippedCount = (from t in qShippedCount
        //                             where t.BillingGroup == obj.billinggroup
        //                             select t).ToList();
        //        }

        //        obj.totalsamples = (from t in qShippedCount
        //                            select t.Qty).Sum();

        //        // shipped same, next, second, etc
        //        var qShippedWhichDay = (from t in db.tblOrderItem
        //                                join trans in db.tblOrderTrans on t.ItemID equals trans.OrderItemID
        //                                join order in db.tblOrderMaster on t.OrderID equals order.OrderID
        //                                where order.ClientID == obj.clientid
        //                                select new
        //                                {
        //                                    t.ShipDate,
        //                                    trans.ChargeDate,
        //                                    t.Qty,
        //                                    order.BillingGroup
        //                                }).ToList();

        //        if (obj.billinggroup != "All")
        //        {
        //            qShippedWhichDay = (from t in qShippedWhichDay
        //                                where t.BillingGroup == obj.billinggroup
        //                                select t).ToList();
        //        }

        //        obj.sampleshipsameday = (from t in qShippedWhichDay
        //                                 where t.ShipDate == t.ChargeDate
        //                                 select t.Qty).Sum();

        //        // TODO Confirm that following 3 lines work correctly
        //        obj.sampleshipnextday = (from t in qShippedWhichDay
        //                                 where t.ShipDate.Value.AddDays(1) == t.ChargeDate
        //                                 select t.Qty).Sum();

        //        obj.sampleshipsecondday = (from t in qShippedWhichDay
        //                                   where t.ShipDate.Value.AddDays(2) == t.ChargeDate
        //                                   select t.Qty).Sum();

        //        obj.sampleshipother = (from t in qShippedWhichDay
        //                               where t.ShipDate != null
        //                               & (t.ShipDate != t.ChargeDate)
        //                               select t.Qty).Sum();

        //        // Sample level
        //        string[] sSampleTransType = { "SAMP", "HAZD", "FLAM", "HEAT", "REFR", "FREZ", "CLEN", "BLND", "NALG", "NITR", "BIOC", "KOSH", "LABL" };

        //        var qSampleCharges = (from t in db.tblOrderTrans
        //                              where t.OrderID != null
        //                              && t.OrderItemID != null
        //                              && t.ChargeDate >= startdate
        //                              && t.ChargeDate <= enddate
        //                              && t.ClientID == obj.clientid
        //                              && sSampleTransType.Contains(t.TransType)
        //                              select new
        //                              {
        //                                  t.TransType,
        //                                  t.TransDate,
        //                                  t.TransQty,
        //                                  t.TransRate,
        //                                  t.BillingGroup
        //                              }).ToList();

        //        if (obj.billinggroup != "All")
        //        {
        //            qSampleCharges = (from t in qSampleCharges
        //                              where t.BillingGroup == obj.billinggroup
        //                              select t).ToList();
        //        }

        //        var qSampleChargesSum = (from t in qSampleCharges
        //                                 select (t.TransQty * t.TransRate)).Sum();

        //        obj.totalcostsamples = obj.totalcostsamples + qSampleChargesSum;

        //        // TODO (Carrier FRT$ - these trans will come from ship verify)
        //        // Note - ship verify will need to update ChargeDate
        //        string[] sFreightTransType = { "CFRT" };

        //        var qCarrierCharges = (from t in db.tblOrderTrans
        //                               where t.OrderID != null
        //                               && t.OrderItemID != null
        //                               && t.ChargeDate >= startdate
        //                               && t.ChargeDate <= enddate
        //                               && t.ClientID == obj.clientid
        //                               && sFreightTransType.Contains(t.TransType)
        //                               select new
        //                               {
        //                                   t.TransType,
        //                                   t.TransQty,
        //                                   t.TransRate,
        //                                   t.BillingGroup
        //                               }).ToList();

        //        if (obj.billinggroup != "All")
        //        {
        //            qCarrierCharges = (from t in qCarrierCharges
        //                               where t.BillingGroup == obj.billinggroup
        //                               select t).ToList();
        //        }

        //        var qCarrierChargesSum = (from t in qCarrierCharges
        //                                  select (t.TransQty * t.TransRate)).Sum();

        //        obj.totalfreight = obj.totalfreight + qCarrierChargesSum;

        //        // frt_surcharge
        //        string[] sFreightSurchargeTransType = { "SFRT" };

        //        var qFreightSurcharges = (from t in db.tblOrderTrans
        //                                  where t.OrderID != null
        //                                  && t.OrderItemID != null
        //                                  && t.ChargeDate >= startdate
        //                                  && t.ChargeDate <= enddate
        //                                  && t.ClientID == obj.clientid
        //                                  && sFreightSurchargeTransType.Contains(t.TransType)
        //                                  select new
        //                                  {
        //                                      t.TransType,
        //                                      t.TransQty,
        //                                      t.TransRate,
        //                                      t.BillingGroup
        //                                  }).ToList();

        //        if (obj.billinggroup != "All")
        //        {
        //            qFreightSurcharges = (from t in qFreightSurcharges
        //                                  where t.BillingGroup == obj.billinggroup
        //                                  select t).ToList();
        //        }

        //        var qFreightSurchargesSum = (from t in qFreightSurcharges
        //                                     select (t.TransQty * t.TransRate)).Sum();

        //        obj.totalfrtHzdSchg = obj.totalfrtHzdSchg + qFreightSurchargesSum;

        //        // All other charges not considered above
        //        string[] sAdminTransType = { "SAMP", "CFRT", "SFRT", "SAMP", "HAZD", "FLAM", "HEAT", "REFR", "FREZ", "CLEN", "BLND", "NALG", "NITR", "BIOC", "KOSH", "LABL" };

        //        var qAdminCharges = (from t in db.tblOrderTrans
        //                             where
        //                                 t.ChargeDate >= startdate
        //                                 && t.ChargeDate <= enddate
        //                                 && t.ClientID == obj.clientid
        //                                 && !sAdminTransType.Contains(t.TransType)
        //                             select new
        //                             {
        //                                 t.TransType,
        //                                 t.TransQty,
        //                                 t.TransRate,
        //                                 t.BillingGroup
        //                             }).ToList();

        //        if (obj.billinggroup != "All")
        //        {
        //            qAdminCharges = (from t in qAdminCharges
        //                             where t.BillingGroup == obj.billinggroup
        //                             select t).ToList();
        //        }

        //        var qAdminChargesSum = (from t in qAdminCharges
        //                                select (t.TransQty * t.TransRate)).Sum();

        //        obj.totaladmincharge = obj.totaladmincharge + qAdminChargesSum;

        //        // add in clientid level trans
        //        var qClientTrans = (from t in db.tblOrderTrans
        //                            where t.OrderID == null
        //                            && t.OrderItemID == null
        //                            && t.ChargeDate >= startdate
        //                            && t.ChargeDate <= enddate
        //                            && t.ClientID == obj.clientid
        //                            select new
        //                            {
        //                                t.ClientID,
        //                                t.OrderID,
        //                                t.OrderItemID,
        //                                t.TransType,
        //                                t.TransDate,
        //                                t.TransQty,
        //                                t.TransRate,
        //                                t.BillingGroup
        //                            }).ToList();

        //        if (obj.billinggroup != "All")
        //        {
        //            qClientTrans = (from t in qClientTrans
        //                            where t.BillingGroup == obj.billinggroup
        //                            select t).ToList();
        //        }

        //        var qClientTransSum = (from t in qClientTrans
        //                               select (t.TransQty * t.TransRate)).Sum();

        //        obj.totaladmincharge = obj.totaladmincharge + qClientTransSum;

        //        // add in order level trans
        //        var qOrderLevelTrans = (from t in db.tblOrderTrans
        //                                where t.OrderID != null
        //                                && t.OrderItemID == null
        //                                && t.ChargeDate >= startdate
        //                                && t.ChargeDate <= enddate
        //                                && t.ClientID == obj.clientid
        //                                select new
        //                                {
        //                                    t.ClientID,
        //                                    t.OrderID,
        //                                    t.OrderItemID,
        //                                    t.TransType,
        //                                    t.TransDate,
        //                                    t.TransQty,
        //                                    t.TransRate,
        //                                    t.BillingGroup
        //                                }).ToList();

        //        if (obj.billinggroup != "All")
        //        {
        //            qOrderLevelTrans = (from t in qOrderLevelTrans
        //                                where t.BillingGroup == obj.billinggroup
        //                                select t).ToList();
        //        }

        //        var qOrderLevelTransSum = (from t in qOrderLevelTrans
        //                                   select (t.TransQty * t.TransRate)).Sum();

        //        obj.totaladmincharge = obj.totaladmincharge + qOrderLevelTransSum;

        //        // return the passed object with more properties filled in
        //        return obj;
        //    }
        //}
        #endregion
    }
}