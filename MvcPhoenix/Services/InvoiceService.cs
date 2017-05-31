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
                           orderby t.InvoiceNumber ascending
                           select new InvoiceViewModel
                           {
                               invoiceid = t.InvoiceID,
                               invoicenumber = t.InvoiceNumber,
                               clientname = t.ClientName,
                               billinggroup = t.BillingGroup,
                               invoicedate = t.InvoiceDate,
                               invoiceperiod = t.InvoicePeriod,
                               warehouselocation = t.WarehouseLocation,
                               status = t.Status
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

                obj.invoiceid = q.InvoiceID;
                obj.invoicenumber = q.InvoiceNumber;
                obj.billinggroup = q.BillingGroup;
                obj.warehouselocation = q.WarehouseLocation;
                obj.clientid = q.ClientID;
                obj.clientname = q.ClientName;
                obj.createdby = q.CreatedBy;
                obj.createdate = q.CreateDate;
                obj.updatedby = q.UpdatedBy;
                obj.updatedate = q.UpdateDate;
                obj.verifiedaccuracy = q.VerifiedAccuracy;
                obj.verifiedby = q.VerifiedBy;
                obj.verifieddate = q.VerifyDate;
                obj.status = q.Status;
                obj.invoicedate = q.InvoiceDate;
                obj.invoiceperiod = q.InvoicePeriod;        //extract month and year from invoice date convert to string
                obj.invoicestartdate = q.InvoiceStartDate;
                obj.invoiceenddate = q.InvoiceEndDate;
                obj.ponumber = q.PONumber;
                obj.netterm = q.NetTerm;
                obj.billto = q.BillTo;
                obj.remitto = q.RemitTo;
                obj.currency = q.Currency;                  // USD / EUR / CNY
                obj.tier = q.Tier;
                obj.ordertype = q.OrderType;                // Sample/International/Revenue does not apply to all clients

                // Shipping Performance
                obj.sampleshipsameday = q.SampleShipSameDay;
                obj.sampleshipnextday = q.SampleShipNextDay;
                obj.sampleshipsecondday = q.SampleShipSecondDay;
                obj.sampleshipother = q.SampleShipOther;

                // Invoice Summary
                obj.totalsamples = q.TotalSamples;
                obj.totalcostsamples = q.TotalCostSamples;
                obj.totalfreight = q.TotalFreight;
                obj.totalfrtHzdSchg = q.TotalFrtHzdSchg;
                obj.totaladmincharge = q.TotalAdminCharge;
                obj.totaldue = q.TotalDue;

                // Billing Worksheet
                obj.GrandTotalCharge = q.GrandTotal;

                // Charges
                obj.EmptyPackagingCharge = q.EmptyPackagingCharge;
                obj.HandlingCharge = q.HandlingCharge;
                obj.InactiveProductsCharge = q.InactiveProductsCharge;
                obj.ProductSetupChangesCharge = q.ProductSetupChangesCharge;
                obj.MiscellaneousLaborCharge = q.MiscellaneousLaborCharge;
                obj.FollowUpOrderCharge = q.FollowUpOrderCharge;
                obj.RefrigeratorStorageCharge = q.RefrigeratorStorageCharge;
                obj.GHSLabelsCharge = q.GHSLabelsCharge;
                obj.ITFeeCharge = q.ITFeeCharge;
                obj.LabelsPrintedCharge = q.LabelsPrintedCharge;
                obj.LaborRelabelCharge = q.LaborRelabelCharge;
                obj.LiteratureCharge = q.LiteratureCharge;
                obj.LabelStockCharge = q.LabelStockCharge;
                obj.LabelMaintainanceCharge = q.LabelMaintainanceCharge;
                obj.MSDSPrintsCharge = q.MSDSPrintsCharge;
                obj.NewLabelSetupCharge = q.NewLabelSetupCharge;
                obj.NewProductSetupCharge = q.NewProductSetupCharge;
                obj.OtherCharge = q.OtherCharge;
                obj.PalletReturnCharge = q.PalletReturnCharge;
                obj.QCStorageCharge = q.QCStorageCharge;
                obj.RelabelsCharge = q.RelabelsCharge;
                obj.WarehouseStorageCharge = q.WarehouseStorageCharge;
                obj.WHMISLabelsCharge = q.WHMISLabelsCharge;
                obj.WasteProcessingCharge = q.WasteProcessingCharge;

                // Rate
                obj.EmptyPackagingRate = q.EmptyPackagingRate;
                obj.HandlingRate = q.HandlingRate;
                obj.InactiveProductsRate = q.InactiveProductsRate;
                obj.ProductSetupChangesRate = q.ProductSetupChangesRate;
                obj.MiscellaneousLaborRate = q.MiscellaneousLaborRate;
                obj.FollowUpOrderRate = q.FollowUpOrderRate;
                obj.RefrigeratorStorageRate = q.RefrigeratorStorageRate;
                obj.GHSLabelsRate = q.GHSLabelsRate;
                obj.ITFeeRate = q.ITFeeRate;
                obj.LabelsPrintedRate = q.LabelsPrintedRate;
                obj.LaborRelabelRate = q.LaborRelabelRate;
                obj.LiteratureRate = q.LiteratureRate;
                obj.LabelStockRate = q.LabelStockRate;
                obj.LabelMaintainanceRate = q.LabelMaintainanceRate;
                obj.MSDSPrintsRate = q.MSDSPrintsRate;
                obj.NewLabelSetupRate = q.NewLabelSetupRate;
                obj.NewProductSetupRate = q.NewProductSetupRate;
                obj.OtherRate = q.OtherRate;
                obj.PalletReturnRate = q.PalletReturnRate;
                obj.QCStorageRate = q.QCStorageRate;
                obj.RelabelsRate = q.RelabelsRate;
                obj.WarehouseStorageRate = q.WarehouseStorageRate;
                obj.WHMISLabelsRate = q.WHMISLabelsRate;
                obj.WasteProcessingRate = q.WasteProcessingRate;

                // Quantity
                obj.EmptyPackagingQuantity = q.EmptyPackagingQuantity;
                obj.HandlingQuantity = q.HandlingQuantity;
                obj.InactiveProductsQuantity = q.InactiveProductsQuantity;
                obj.ProductSetupChangesQuantity = q.ProductSetupChangesQuantity;
                obj.MiscellaneousLaborQuantity = q.MiscellaneousLaborQuantity;
                obj.FollowUpOrderQuantity = q.FollowUpOrderQuantity;
                obj.RefrigeratorStorageQuantity = q.RefrigeratorStorageQuantity;
                obj.GHSLabelsQuantity = q.GHSLabelsQuantity;
                obj.ITFeeQuantity = q.ITFeeQuantity;
                obj.LabelsPrintedQuantity = q.LabelsPrintedQuantity;
                obj.LaborRelabelQuantity = q.LaborRelabelQuantity;
                obj.LiteratureQuantity = q.LiteratureQuantity;
                obj.LabelStockQuantity = q.LabelStockQuantity;
                obj.LabelMaintainanceQuantity = q.LabelMaintainanceQuantity;
                obj.MSDSPrintsQuantity = q.MSDSPrintsQuantity;
                obj.NewLabelSetupQuantity = q.NewLabelSetupQuantity;
                obj.NewProductSetupQuantity = q.NewProductSetupQuantity;
                obj.OtherQuantity = q.OtherQuantity;
                obj.PalletReturnQuantity = q.PalletReturnQuantity;
                obj.QCStorageQuantity = q.QCStorageQuantity;
                obj.RelabelsQuantity = q.RelabelsQuantity;
                obj.WarehouseStorageQuantity = q.WarehouseStorageQuantity;
                obj.WHMISLabelsQuantity = q.WHMISLabelsQuantity;
                obj.WasteProcessingQuantity = q.WasteProcessingQuantity;

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
                if (vm.verifiedaccuracy == true)
                {
                    /// Add logic to check invoice status if not new or verified already
                    /// check if verified by user == created user. should be different
                    /// capture verified datetime and user identity and save to db
                    /// verified invoices should be locked from further edits.
                    vm.status = "VERIFIED";
                    confirmVerify = true;
                }

                // Capture user info in viewmodel
                vm.updatedby = HttpContext.Current.User.Identity.Name;
                vm.updatedate = DateTime.UtcNow;

                var q = (from t in db.tblInvoice
                         where t.InvoiceID == vm.invoiceid
                         select t).FirstOrDefault();

                q.InvoiceNumber = vm.invoiceid;
                q.BillingGroup = vm.billinggroup;
                q.WarehouseLocation = vm.warehouselocation;
                q.ClientID = vm.clientid;
                q.ClientName = vm.clientname;
                q.CreatedBy = vm.createdby;
                q.CreateDate = vm.createdate;
                q.UpdatedBy = vm.updatedby;
                q.UpdateDate = vm.updatedate;
                q.InvoiceStartDate = vm.invoicestartdate;
                q.InvoiceEndDate = vm.invoiceenddate;

                if (confirmVerify == true)
                {
                    q.VerifiedAccuracy = vm.verifiedaccuracy;
                    q.VerifiedBy = HttpContext.Current.User.Identity.Name;
                    q.VerifyDate = System.DateTimeOffset.UtcNow;
                }

                q.Status = vm.status;
                q.InvoiceDate = vm.invoicedate;
                q.InvoicePeriod = vm.invoiceperiod;
                q.PONumber = vm.ponumber;
                q.NetTerm = vm.netterm;
                q.BillTo = vm.billto;
                q.RemitTo = vm.remitto;
                q.Currency = vm.currency;
                q.Tier = vm.tier;
                q.OrderType = vm.ordertype;
                q.SampleShipSameDay = vm.sampleshipsameday;
                q.SampleShipNextDay = vm.sampleshipnextday;
                q.SampleShipSecondDay = vm.sampleshipsecondday;
                q.SampleShipOther = vm.sampleshipother;
                q.TotalSamples = vm.totalsamples;
                q.TotalCostSamples = vm.totalcostsamples;
                q.TotalFreight = vm.totalfreight;
                q.TotalFrtHzdSchg = vm.totalfrtHzdSchg;
                //q.TotalAdminCharge = vm.totaladmincharge;
                q.TotalDue = vm.totaldue;

                decimal? grandtotal = 0;

                // Quantities
                q.EmptyPackagingQuantity = vm.EmptyPackagingQuantity;
                q.HandlingQuantity = vm.HandlingQuantity;
                q.InactiveProductsQuantity = vm.InactiveProductsQuantity;
                q.ProductSetupChangesQuantity = vm.ProductSetupChangesQuantity;
                q.MiscellaneousLaborQuantity = vm.MiscellaneousLaborQuantity;
                q.FollowUpOrderQuantity = vm.FollowUpOrderQuantity;
                q.RefrigeratorStorageQuantity = vm.RefrigeratorStorageQuantity;
                q.GHSLabelsQuantity = vm.GHSLabelsQuantity;
                q.ITFeeQuantity = vm.ITFeeQuantity;
                q.LabelsPrintedQuantity = vm.LabelsPrintedQuantity;
                q.LaborRelabelQuantity = vm.LaborRelabelQuantity;
                q.LiteratureQuantity = vm.LiteratureQuantity;
                q.LabelStockQuantity = vm.LabelStockQuantity;
                q.LabelMaintainanceQuantity = vm.LabelMaintainanceQuantity;
                q.MSDSPrintsQuantity = vm.MSDSPrintsQuantity;
                q.NewLabelSetupQuantity = vm.NewLabelSetupQuantity;
                q.NewProductSetupQuantity = vm.NewProductSetupQuantity;
                q.OtherQuantity = vm.OtherQuantity;
                q.PalletReturnQuantity = vm.PalletReturnQuantity;
                q.QCStorageQuantity = vm.QCStorageQuantity;
                q.RelabelsQuantity = vm.RelabelsQuantity;
                q.WarehouseStorageQuantity = vm.WarehouseStorageQuantity;
                q.WHMISLabelsQuantity = vm.WHMISLabelsQuantity;
                q.WasteProcessingQuantity = vm.WasteProcessingQuantity;

                // Rates
                q.EmptyPackagingRate = vm.EmptyPackagingRate;
                q.HandlingRate = vm.HandlingRate;
                q.InactiveProductsRate = vm.InactiveProductsRate;
                q.ProductSetupChangesRate = vm.ProductSetupChangesRate;
                q.MiscellaneousLaborRate = vm.MiscellaneousLaborRate;
                q.FollowUpOrderRate = vm.FollowUpOrderRate;
                q.RefrigeratorStorageRate = vm.RefrigeratorStorageRate;
                q.GHSLabelsRate = vm.GHSLabelsRate;
                q.ITFeeRate = vm.ITFeeRate;
                q.LabelsPrintedRate = vm.LabelsPrintedRate;
                q.LaborRelabelRate = vm.LaborRelabelRate;
                q.LiteratureRate = vm.LiteratureRate;
                q.LabelStockRate = vm.LabelStockRate;
                q.LabelMaintainanceRate = vm.LabelMaintainanceRate;
                q.MSDSPrintsRate = vm.MSDSPrintsRate;
                q.NewLabelSetupRate = vm.NewLabelSetupRate;
                q.NewProductSetupRate = vm.NewProductSetupRate;
                q.OtherRate = vm.OtherRate;
                q.PalletReturnRate = vm.PalletReturnRate;
                q.QCStorageRate = vm.QCStorageRate;
                q.RelabelsRate = vm.RelabelsRate;
                q.WarehouseStorageRate = vm.WarehouseStorageRate;
                q.WHMISLabelsRate = vm.WHMISLabelsRate;
                q.WasteProcessingRate = vm.WasteProcessingRate;

                // Calulated Charges
                grandtotal += q.EmptyPackagingCharge = vm.EmptyPackagingQuantity * vm.EmptyPackagingRate;
                grandtotal += q.HandlingCharge = vm.HandlingQuantity * vm.HandlingRate;
                grandtotal += q.InactiveProductsCharge = vm.InactiveProductsQuantity * vm.InactiveProductsRate;
                grandtotal += q.ProductSetupChangesCharge = vm.ProductSetupChangesQuantity * vm.ProductSetupChangesRate;
                grandtotal += q.MiscellaneousLaborCharge = vm.MiscellaneousLaborQuantity * vm.MiscellaneousLaborRate;
                grandtotal += q.FollowUpOrderCharge = vm.FollowUpOrderQuantity * vm.FollowUpOrderRate;
                grandtotal += q.RefrigeratorStorageCharge = vm.RefrigeratorStorageQuantity * vm.RefrigeratorStorageRate;
                grandtotal += q.GHSLabelsCharge = vm.GHSLabelsQuantity * vm.GHSLabelsRate;
                grandtotal += q.ITFeeCharge = vm.ITFeeQuantity * vm.ITFeeRate;
                grandtotal += q.LabelsPrintedCharge = vm.LabelsPrintedQuantity * vm.LabelsPrintedRate;
                grandtotal += q.LaborRelabelCharge = vm.LaborRelabelQuantity * vm.LaborRelabelRate;
                grandtotal += q.LiteratureCharge = vm.LiteratureQuantity * vm.LiteratureRate;
                grandtotal += q.LabelStockCharge = vm.LabelStockQuantity * vm.LabelStockRate;
                grandtotal += q.LabelMaintainanceCharge = vm.LabelMaintainanceQuantity * vm.LabelMaintainanceRate;
                grandtotal += q.MSDSPrintsCharge = vm.MSDSPrintsQuantity * vm.MSDSPrintsRate;
                grandtotal += q.NewLabelSetupCharge = vm.NewLabelSetupQuantity * vm.NewLabelSetupRate;
                grandtotal += q.NewProductSetupCharge = vm.NewProductSetupQuantity * vm.NewProductSetupRate;
                grandtotal += q.OtherCharge = vm.OtherQuantity * vm.OtherRate;
                grandtotal += q.PalletReturnCharge = vm.PalletReturnQuantity * vm.PalletReturnRate;
                grandtotal += q.QCStorageCharge = vm.QCStorageQuantity * vm.QCStorageRate;
                grandtotal += q.RelabelsCharge = vm.RelabelsQuantity * vm.RelabelsRate;
                grandtotal += q.WarehouseStorageCharge = vm.WarehouseStorageQuantity * vm.WarehouseStorageRate;
                grandtotal += q.WHMISLabelsCharge = vm.WHMISLabelsQuantity * vm.WHMISLabelsRate;
                grandtotal += q.WasteProcessingCharge = vm.WasteProcessingQuantity * vm.WasteProcessingRate;

                q.GrandTotal = grandtotal;
                q.TotalAdminCharge = grandtotal;

                db.SaveChanges();

                return vm.invoiceid;
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
                var administrativecharges = (from t in transactions
                                             where !sampletranstypes.Contains(t.TransType)
                                             select new
                                             {
                                                 t.TransQty,
                                                 t.TransRate
                                             }).ToList();

                var calcadministrativecharges = (from t in administrativecharges
                                                 select (t.TransQty * t.TransRate)).Sum();

                // Calculate All Charges
                var calcsumcharges = calcsamplecharges + calcfreightcharges + calcfreighthazdsurcharges + calcadministrativecharges;

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
                invoice.TotalAdminCharge = calcadministrativecharges;
                invoice.TotalDue = calcsumcharges;
                invoice.SampleShipSameDay = calcsamedayshipping;
                invoice.SampleShipNextDay = calcnextdayshipping;
                invoice.SampleShipSecondDay = calcseconddayshipping;
                invoice.SampleShipOther = calcothershipping;

                // Quantities
                invoice.EmptyPackagingQuantity = 1;
                invoice.HandlingQuantity = 1;
                invoice.InactiveProductsQuantity = 1;
                invoice.ProductSetupChangesQuantity = 1;
                invoice.MiscellaneousLaborQuantity = 1;
                invoice.FollowUpOrderQuantity = 1;
                invoice.RefrigeratorStorageQuantity = 1;
                invoice.GHSLabelsQuantity = 1;
                invoice.ITFeeQuantity = 1;
                invoice.LabelsPrintedQuantity = 1;
                invoice.LaborRelabelQuantity = 1;
                invoice.LiteratureQuantity = 1;
                invoice.LabelStockQuantity = 1;
                invoice.LabelMaintainanceQuantity = 1;
                invoice.MSDSPrintsQuantity = 1;
                invoice.NewLabelSetupQuantity = 1;
                invoice.NewProductSetupQuantity = 1;
                invoice.OtherQuantity = 1;
                invoice.PalletReturnQuantity = 1;
                invoice.QCStorageQuantity = 1;
                invoice.RelabelsQuantity = 1;
                invoice.WarehouseStorageQuantity = 1;
                invoice.WHMISLabelsQuantity = 1;
                invoice.WasteProcessingQuantity = 1;

                // Rates
                invoice.EmptyPackagingRate = adminrate.EmptyPackagingRate ?? 1;
                invoice.HandlingRate = adminrate.HandlingRate ?? 1;
                invoice.InactiveProductsRate = adminrate.InactiveProductRate ?? 1;
                invoice.ProductSetupChangesRate = adminrate.ProductSetupChangesRate ?? 1;
                invoice.MiscellaneousLaborRate = adminrate.MiscellaneousLaborRate ?? 1;
                invoice.FollowUpOrderRate = adminrate.FollowUpOrderRate ?? 1;
                invoice.RefrigeratorStorageRate = adminrate.RefrigeratorStorageRate ?? 1;
                invoice.GHSLabelsRate = adminrate.GHSLabelsRate ?? 1;
                invoice.ITFeeRate = adminrate.ITFeeRate ?? 1;
                invoice.LabelsPrintedRate = adminrate.LabelsPrintedRate ?? 1;
                invoice.LaborRelabelRate = adminrate.LaborRelabelRate ?? 1;
                invoice.LiteratureRate = adminrate.LiteratureRate ?? 1;
                invoice.LabelStockRate = adminrate.LabelStockRate ?? 1;
                invoice.LabelMaintainanceRate = adminrate.LabelMaintainanceRate ?? 1;
                invoice.MSDSPrintsRate = adminrate.MSDSPrintsRate ?? 1;
                invoice.NewLabelSetupRate = adminrate.NewLabelSetupRate ?? 1;
                invoice.NewProductSetupRate = adminrate.NewProductSetupRate ?? 1;
                invoice.OtherRate = adminrate.OtherRate ?? 1;
                invoice.PalletReturnRate = adminrate.PalletReturnRate ?? 1;
                invoice.QCStorageRate = adminrate.QCStorageRate ?? 1;
                invoice.RelabelsRate = adminrate.RelabelsRate ?? 1;
                invoice.WarehouseStorageRate = adminrate.WarehouseStorageRate ?? 1;
                invoice.WHMISLabelsRate = adminrate.WHMISLabelsRate ?? 1;
                invoice.WasteProcessingRate = adminrate.WasteProcessingRate ?? 1;

                // Calulated Charges
                grandtotal += invoice.EmptyPackagingCharge = invoice.EmptyPackagingQuantity * invoice.EmptyPackagingRate;
                grandtotal += invoice.HandlingCharge = invoice.HandlingQuantity * invoice.HandlingRate;
                grandtotal += invoice.InactiveProductsCharge = invoice.InactiveProductsQuantity * invoice.InactiveProductsRate;
                grandtotal += invoice.ProductSetupChangesCharge = invoice.ProductSetupChangesQuantity * invoice.ProductSetupChangesRate;
                grandtotal += invoice.MiscellaneousLaborCharge = invoice.MiscellaneousLaborQuantity * invoice.MiscellaneousLaborRate;
                grandtotal += invoice.FollowUpOrderCharge = invoice.FollowUpOrderQuantity * invoice.FollowUpOrderRate;
                grandtotal += invoice.RefrigeratorStorageCharge = invoice.RefrigeratorStorageQuantity * invoice.RefrigeratorStorageRate;
                grandtotal += invoice.GHSLabelsCharge = invoice.GHSLabelsQuantity * invoice.GHSLabelsRate;
                grandtotal += invoice.ITFeeCharge = invoice.ITFeeQuantity * invoice.ITFeeRate;
                grandtotal += invoice.LabelsPrintedCharge = invoice.LabelsPrintedQuantity * invoice.LabelsPrintedRate;
                grandtotal += invoice.LaborRelabelCharge = invoice.LaborRelabelQuantity * invoice.LaborRelabelRate;
                grandtotal += invoice.LiteratureCharge = invoice.LiteratureQuantity * invoice.LiteratureRate;
                grandtotal += invoice.LabelStockCharge = invoice.LabelStockQuantity * invoice.LabelStockRate;
                grandtotal += invoice.LabelMaintainanceCharge = invoice.LabelMaintainanceQuantity * invoice.LabelMaintainanceRate;
                grandtotal += invoice.MSDSPrintsCharge = invoice.MSDSPrintsQuantity * invoice.MSDSPrintsRate;
                grandtotal += invoice.NewLabelSetupCharge = invoice.NewLabelSetupQuantity * invoice.NewLabelSetupRate;
                grandtotal += invoice.NewProductSetupCharge = invoice.NewProductSetupQuantity * invoice.NewProductSetupRate;
                grandtotal += invoice.OtherCharge = invoice.OtherQuantity * invoice.OtherRate;
                grandtotal += invoice.PalletReturnCharge = invoice.PalletReturnQuantity * invoice.PalletReturnRate;
                grandtotal += invoice.QCStorageCharge = invoice.QCStorageQuantity * invoice.QCStorageRate;
                grandtotal += invoice.RelabelsCharge = invoice.RelabelsQuantity * invoice.RelabelsRate;
                grandtotal += invoice.WarehouseStorageCharge = invoice.WarehouseStorageQuantity * invoice.WarehouseStorageRate;
                grandtotal += invoice.WHMISLabelsCharge = invoice.WHMISLabelsQuantity * invoice.WHMISLabelsRate;
                grandtotal += invoice.WasteProcessingCharge = invoice.WasteProcessingQuantity * invoice.WasteProcessingRate;

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