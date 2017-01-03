using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class InvoiceService
    {
        public static InvoiceViewModel FillInvoice(int id)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                InvoiceViewModel IVM = new InvoiceViewModel();

                var q = (from t in db.tblInvoice
                         where t.InvoiceID == id
                         select t).FirstOrDefault();

                IVM.invoiceid = q.InvoiceID;
                IVM.invoicenumber = q.InvoiceNumber;
                IVM.billinggroup = q.BillingGroup;
                IVM.warehouselocation = q.WarehouseLocation;
                IVM.clientid = q.ClientID;
                IVM.clientname = q.ClientName;
                IVM.createdby = q.CreatedBy;
                IVM.createdate = q.CreateDate;
                IVM.updatedby = q.UpdatedBy;
                IVM.updatedate = q.UpdateDate;
                IVM.verifiedaccuracy = q.VerifiedAccuracy;
                IVM.verifiedby = q.VerifiedBy;
                IVM.verifieddate = q.VerifyDate;
                IVM.status = q.Status;
                IVM.invoicedate = q.InvoiceDate;
                IVM.invoiceperiod = q.InvoicePeriod;        //extract month and year from invoice date convert to string
                IVM.invoicestartdate = q.InvoiceStartDate;
                IVM.invoiceenddate = q.InvoiceEndDate;
                IVM.ponumber = q.PONumber;
                IVM.netterm = q.NetTerm;
                IVM.billto = q.BillTo;
                IVM.remitto = q.RemitTo;
                IVM.currency = q.Currency;                  // USD / EUR / CNY
                IVM.tier = q.Tier;
                IVM.ordertype = q.OrderType;                // Sample/International/Revenue does not apply to all clients

                // Shipping Performance
                IVM.sampleshipsameday = q.SampleShipSameDay;
                IVM.sampleshipnextday = q.SampleShipNextDay;
                IVM.sampleshipsecondday = q.SampleShipSecondDay;
                IVM.sampleshipother = q.SampleShipOther;

                // Invoice Summary
                IVM.totalsamples = q.TotalSamples;
                IVM.totalcostsamples = q.TotalCostSamples;
                IVM.totalfreight = q.TotalFreight;
                IVM.totalfrtHzdSchg = q.TotalFrtHzdSchg;
                IVM.totaladmincharge = q.TotalAdminCharge;
                IVM.totaldue = q.TotalDue;

                // Billing Worksheet
                IVM.grandtotal = q.GrandTotal;

                IVM.samplesqty = q.SamplesQty;
                IVM.samplesrate = q.SamplesRate;
                IVM.samplescharge = q.SamplesCharge;

                IVM.revenueqty = q.RevenueQty;
                IVM.revenuerate = q.RevenueRate;
                IVM.revenuecharge = q.RevenueCharge;

                IVM.nonrevenueqty = q.NonRevenueQty;
                IVM.nonrevenuerate = q.NonRevenueRate;
                IVM.nonrevenuecharge = q.NonRevenueCharge;

                IVM.manualentryqty = q.ManualEntryQty;
                IVM.manualentryrate = q.ManualEntryRate;
                IVM.manualentrycharge = q.ManualEntryCharge;

                IVM.followupqty = q.FollowUpQty;
                IVM.followuprate = q.FollowUpRate;
                IVM.followupcharge = q.FollowUpCharge;

                IVM.labelprtqty = q.LabelPrtQty;
                IVM.labelprtrate = q.LabelPrtRate;
                IVM.labelprtcharge = q.LabelPrtCharge;

                IVM.labelstock = q.LabelStockCharge;

                IVM.relabelprtqty = q.ReLabelPrtQty;
                IVM.relabelprtrate = q.ReLabelPrtRate;
                IVM.relabelprtcharge = q.ReLabelPrtCharge;

                IVM.relabelfeeqty = q.ReLabelFeeQty;
                IVM.relabelfeerate = q.ReLabelFeeRate;
                IVM.relabelfeecharge = q.ReLabelFeeCharge;

                IVM.productsetupqty = q.ProductSetupQty;
                IVM.productsetuprate = q.ProductSetupRate;
                IVM.productsetupcharge = q.ProductSetupCharge;

                IVM.ccprocessqty = q.CCProcessQty;
                IVM.ccprocessrate = q.CCProcessRate;
                IVM.ccprocesscharge = q.CCProcessCharge;

                IVM.cccredit = q.CCCredit;
                IVM.globalprocesscharge = q.GlobalProcessCharge;
                IVM.misccreditcharge = q.MiscCreditCharge;
                IVM.itsupportcharge = q.ITSupportCharge;
                IVM.emptydrumcharge = q.EmptyDrumCharge;

                IVM.rushshipqty = q.RushShipQty;
                IVM.rushshiprate = q.RushShipRate;
                IVM.rushshipcharge = q.RushShipCharge;

                IVM.emptypailsqty = q.EmptyPailsQty;
                IVM.emptypailsrate = q.EmptyPailsRate;
                IVM.emptypailscharge = q.EmptyPailsCharge;

                IVM.emptypailsfgtcharge = q.EmptyPailsFgtCharge;

                IVM.inactivestockqty = q.InactiveStockQty;
                IVM.inactivestockrate = q.InactiveStockRate;
                IVM.inactivestockcharge = q.InactiveStockCharge;

                IVM.hm181pkgcharge = q.HM181PkgCharge;
                IVM.dochandlingcharge = q.DocHandlingCharge;
                IVM.minimalsamplecharge = q.MinimalSampleCharge;

                return IVM;
            }
        }

        public static int SaveInvoice(InvoiceViewModel vm)
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                if (vm.invoiceid == -1)
                {
                    vm.invoiceid = NewInvoiceID();
                    vm.createdate = System.DateTime.Now;
                    vm.createdby = HttpContext.Current.User.Identity.Name;
                    vm.invoicedate = System.DateTimeOffset.UtcNow;
                    vm.status = "NEW";
                }

                var confirmVerify = false;
                if (vm.verifiedaccuracy == true)
                {
                    ///Add logic to check invoice status if not new or verified already
                    ///check if verified by user == created user. should be different
                    ///capture verified datetime and user identity and save to db
                    ///verified invoices should be locked from further edits..
                    vm.status = "VERIFIED";
                    confirmVerify = true;
                }

                // Capture user info in viewmodel
                vm.updatedby = HttpContext.Current.User.Identity.Name;
                vm.updatedate = System.DateTime.Now;

                var q = (from t in db.tblInvoice 
                         where t.InvoiceID == vm.invoiceid 
                         select t).FirstOrDefault();

                q.InvoiceNumber = vm.invoiceid;                                    //there can be multiple invoice numbers which can be used to group different order types for an invoice
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
                q.TotalAdminCharge = vm.totaladmincharge;
                q.TotalDue = vm.totaldue;
                q.GrandTotal = vm.grandtotal;
                q.SamplesQty = vm.samplesqty;
                q.SamplesRate = vm.samplesrate;
                q.SamplesCharge = vm.samplescharge;
                q.RevenueQty = vm.revenueqty;
                q.RevenueRate = vm.revenuerate;
                q.RevenueCharge = vm.revenuecharge;
                q.NonRevenueQty = vm.nonrevenueqty;
                q.NonRevenueRate = vm.nonrevenuerate;
                q.NonRevenueCharge = vm.nonrevenuecharge;
                q.ManualEntryQty = vm.manualentryqty;
                q.ManualEntryRate = vm.manualentryrate;
                q.ManualEntryCharge = vm.manualentrycharge;
                q.FollowUpQty = vm.followupqty;
                q.FollowUpRate = vm.followuprate;
                q.FollowUpCharge = vm.followupcharge;
                q.LabelPrtQty = vm.labelprtqty;
                q.LabelPrtRate = vm.labelprtrate;
                q.LabelPrtCharge = vm.labelprtcharge;
                q.LabelStockCharge = vm.labelstock;
                q.ReLabelPrtQty = vm.relabelprtqty;
                q.ReLabelPrtRate = vm.relabelprtrate;
                q.ReLabelPrtCharge = vm.relabelprtcharge;
                q.ReLabelFeeQty = vm.relabelfeeqty;
                q.ReLabelFeeRate = vm.relabelfeerate;
                q.ReLabelFeeCharge = vm.relabelfeecharge;
                q.ProductSetupQty = vm.productsetupqty;
                q.ProductSetupRate = vm.productsetuprate;
                q.ProductSetupCharge = vm.productsetupcharge;
                q.CCProcessQty = vm.ccprocessqty;
                q.CCProcessRate = vm.ccprocessrate;
                q.CCProcessCharge = vm.ccprocesscharge;
                q.CCCredit = vm.cccredit;
                q.GlobalProcessCharge = vm.globalprocesscharge;
                q.MiscCreditCharge = vm.misccreditcharge;
                q.ITSupportCharge = vm.itsupportcharge;
                q.EmptyDrumCharge = vm.emptydrumcharge;
                q.RushShipQty = vm.rushshipqty;
                q.RushShipRate = vm.rushshiprate;
                q.RushShipCharge = vm.rushshipcharge;
                q.EmptyPailsQty = vm.emptypailsqty;
                q.EmptyPailsRate = vm.emptypailsrate;
                q.EmptyPailsCharge = vm.emptypailscharge;
                q.EmptyPailsFgtCharge = vm.emptypailsfgtcharge;
                q.InactiveStockQty = vm.inactivestockqty;
                q.InactiveStockRate = vm.inactivestockrate;
                q.InactiveStockCharge = vm.inactivestockcharge;
                q.HM181PkgCharge = vm.hm181pkgcharge;
                q.DocHandlingCharge = vm.dochandlingcharge;
                q.MinimalSampleCharge = vm.minimalsamplecharge;

                db.SaveChanges();

                return vm.invoiceid;
            }
        }

        public static int NewInvoiceID()
        {
            using (var db = new MvcPhoenix.EF.CMCSQL03Entities())
            {
                MvcPhoenix.EF.tblInvoice nr = new MvcPhoenix.EF.tblInvoice();
                db.tblInvoice.Add(nr);

                db.SaveChanges();

                return nr.InvoiceID;
            }
        }

        public static InvoiceViewModel CreateInvoice(int client, int division, string period, DateTime startdate, DateTime enddate)
        {
            InvoiceViewModel obj = new InvoiceViewModel();

            using (var db = new EF.CMCSQL03Entities())
            {
                obj.invoiceid = -1;
                obj.invoicedate = DateTime.UtcNow;
                obj.status = "New";

                if (division > 0)
                {
                    var div = db.tblDivision.Find(division);
                    obj.billinggroup = div.DivisionName;
                }
                else { 
                    obj.billinggroup = "All"; 
                }

                // Pull default values from Client table
                var cl = db.tblClient.Find(client);
                obj.clientid = cl.ClientID;
                obj.clientname = cl.ClientName;
                obj.warehouselocation = cl.CMCLocation;
                obj.billto = cl.InvoiceAddress;
                obj.netterm = String.IsNullOrEmpty(cl.ClientNetTerm) ? "Net 30 Days" : cl.ClientNetTerm;
                obj.currency = cl.ClientCurrency;
                obj.ponumber = "Enter PO Number";
                obj.tier = 1;
                obj.invoiceperiod = period;
                obj.invoicestartdate = startdate;
                obj.invoiceenddate = enddate;
                obj.remitto = "<p>Chemical Marketing Concepts, LLC<br />c/o Odyssey Logistics &amp; Technology Corp<br />39 Old Ridgebury Road, N-1<br />Danbury, CT 06810</p>";

                // Pull default values from Rates table
                var rate = (from t in db.tblRates
                            where t.ClientID == 1
                            select t).FirstOrDefault();

                obj.revenuerate = rate.RevenueRate;
                obj.nonrevenuerate = rate.NonRevenueRate;
                obj.manualentryrate = rate.ManualEntryRate;
                obj.followuprate = rate.FollowUpRate;
                obj.labelprtrate = rate.LabelPrtRate;
                obj.relabelprtrate = rate.ReLabelPrtRate;
                obj.relabelfeerate = rate.ReLabelFeeRate;
                obj.productsetuprate = rate.ProductSetupRate;
                obj.ccprocessrate = rate.CCProcessRate;
                obj.rushshiprate = rate.RushShipRate;
                obj.emptypailsrate = rate.EmptyPailsRate;
                obj.inactivestockrate = rate.InactiveStockRate;
                obj.minimalsamplecharge = rate.MinimalSampleCharge;

                UpdateTierPricing(obj, startdate, enddate);                         // go update tier pricing

                obj = GetOrderTransCharges(obj, startdate, enddate);                // now go read tblOrderTrans fill in some properties for the system generated trans

                return obj;
            }
        }

        private static void UpdateTierPricing(InvoiceViewModel obj, DateTime StartDate, DateTime EndDate)
        {
            // Look at each tier record
            // Go get shipped items for this invoice client and ship date range
            // add up qty shipped
            // adjust tblOrderTrans.TransAmount accordingly
            using (var db = new EF.CMCSQL03Entities())
            {
                // collect all Tier records for a client
                var qTiers = (from t in db.tblTier 
                              where t.ClientID == obj.clientid && t.Tier != "1" 
                              orderby t.Tier 
                              select t).ToList();

                // For each Tier, go see if shipments in the date range qualify for Tier change
                foreach (var row in qTiers)
                {
                    var qitems = (from t in db.tblOrderItem
                                  join order in db.tblOrderMaster on t.OrderID equals order.OrderID
                                  where order.ClientID == obj.clientid
                                  && t.ShipDate >= StartDate && t.ShipDate <= EndDate
                                  select new { t.Qty, t.Size, t.ShipDate, order.ClientID, order.BillingGroup }).ToList();

                    if (obj.billinggroup != "All")
                    {
                        qitems = (from t in qitems 
                                  where t.BillingGroup == obj.billinggroup 
                                  select t).ToList();
                    }

                    var qitemsSum = (from t in qitems 
                                     where t.Size == row.Size 
                                     select t.Qty).Sum();

                    if (qitemsSum.Value >= row.LoSampAmt)
                    {
                        // TODO confirm date formats coincede
                        var qUpdate = (from t in db.tblOrderTrans
                                       join oi in db.tblOrderItem on t.OrderItemID equals oi.ItemID
                                       where t.ClientID == row.ClientID
                                       && oi.ShipDate >= StartDate
                                       && oi.ShipDate <= EndDate
                                       && oi.Size == row.Size
                                       select new { t.OrderTransID, t.OrderItemID, t.TransAmount }).ToList();

                        foreach (var trans in qUpdate)
                        {
                            // Do the price update
                            string s = String.Format("Update tblOrderTrans Set TransAmount = {0},UpdateDate=getdate(),updateUser='System' where OrderTransID={1}", row.Price, trans.OrderTransID);
                            db.Database.ExecuteSqlCommand(s);
                        }
                    }

                }
            }
        }

        private static InvoiceViewModel GetOrderTransCharges(InvoiceViewModel obj, DateTime StartDate, DateTime EndDate)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                //init
                obj.totalcostsamples = 0;
                obj.totalfreight = 0;
                obj.totalfrtHzdSchg = 0;
                obj.totaladmincharge = 0;
                obj.totaldue = 0;
                obj.sampleshipsameday = 0;
                obj.sampleshipnextday = 0;
                obj.sampleshipsecondday = 0;
                obj.sampleshipother = 0;

                // number of samples shipped - need to look in tblOrderItem, but would prefer a TransType=SHIP ???
                var qShippedCount = (from t in db.tblOrderItem
                                     join order in db.tblOrderMaster on t.OrderID equals order.OrderID
                                     where order.ClientID == obj.clientid
                                     && t.ShipDate != null
                                     && (t.ShipDate >= StartDate && t.ShipDate <= EndDate)
                                     select new { t.Qty, order.BillingGroup }).ToList();
                
                if (obj.billinggroup != "All")
                {
                    qShippedCount = (from t in qShippedCount 
                                     where t.BillingGroup == obj.billinggroup 
                                     select t).ToList();
                }

                obj.totalsamples = (from t in qShippedCount 
                                    select t.Qty).Sum();
                
                // shipped same, next, second, etc
                var qShippedWhichDay = (from t in db.tblOrderItem
                                        join trans in db.tblOrderTrans on t.ItemID equals trans.OrderItemID
                                        join order in db.tblOrderMaster on t.OrderID equals order.OrderID
                                        where order.ClientID == obj.clientid
                                        select new { t.ShipDate, trans.ChargeDate, t.Qty, order.BillingGroup }).ToList();
                
                if (obj.billinggroup != "All")
                {
                    qShippedWhichDay = (from t in qShippedWhichDay 
                                        where t.BillingGroup == obj.billinggroup 
                                        select t).ToList();
                }

                obj.sampleshipsameday = (from t in qShippedWhichDay 
                                         where t.ShipDate == t.ChargeDate 
                                         select t.Qty).Sum();

                // TODO Confirm that following 3 lines work correctly
                obj.sampleshipnextday = (from t in qShippedWhichDay 
                                         where t.ShipDate.Value.AddDays(1) == t.ChargeDate 
                                         select t.Qty).Sum();

                obj.sampleshipsecondday = (from t in qShippedWhichDay 
                                           where t.ShipDate.Value.AddDays(2) == t.ChargeDate 
                                           select t.Qty).Sum();

                obj.sampleshipother = (from t in qShippedWhichDay 
                                       where t.ShipDate != null & (t.ShipDate != t.ChargeDate) 
                                       select t.Qty).Sum();

                // Sample level
                string[] sSampleTransType = { "SAMP", "HAZD", "FLAM", "HEAT", "REFR", "FREZ", "CLEN", "BLND", "NALG", "NITR", "BIOC", "KOSH", "LABL" };

                var qSampleCharges = (from t in db.tblOrderTrans
                                      where t.OrderID != null && t.OrderItemID != null
                                      && t.ChargeDate >= StartDate && t.ChargeDate <= EndDate
                                      && t.ClientID == obj.clientid
                                      && sSampleTransType.Contains(t.TransType)
                                      select new { t.TransType, t.TransDate, t.TransQty, t.TransAmount, t.BillingGroup }).ToList();

                if (obj.billinggroup != "All")
                {
                    qSampleCharges = (from t in qSampleCharges 
                                      where t.BillingGroup == obj.billinggroup 
                                      select t).ToList();
                }

                var qSampleChargesSum = (from t in qSampleCharges 
                                         select (t.TransQty * t.TransAmount)).Sum();
                
                obj.totalcostsamples = obj.totalcostsamples + qSampleChargesSum;
                
                // TODO (Carrier FRT$ - these trans will come from ship verify)
                // Note - ship verify will need to update ChargeDate
                string[] sFreightTransType = { "CFRT" };

                var qCarrierCharges = (from t in db.tblOrderTrans
                                       where t.OrderID != null && t.OrderItemID != null
                                       && t.ChargeDate >= StartDate && t.ChargeDate <= EndDate
                                       && t.ClientID == obj.clientid
                                       && sFreightTransType.Contains(t.TransType)
                                       select new { t.TransType, t.TransQty, t.TransAmount, t.BillingGroup }).ToList();

                if (obj.billinggroup != "All")
                {
                    qCarrierCharges = (from t in qCarrierCharges 
                                       where t.BillingGroup == obj.billinggroup 
                                       select t).ToList();
                }

                var qCarrierChargesSum = (from t in qCarrierCharges 
                                          select (t.TransQty * t.TransAmount)).Sum();

                obj.totalfreight = obj.totalfreight + qCarrierChargesSum;
                
                // frt_surcharge
                string[] sFreightSurchargeTransType = { "SFRT" };

                var qFreightSurcharges = (from t in db.tblOrderTrans
                                          where t.OrderID != null && t.OrderItemID != null
                                          && t.ChargeDate >= StartDate && t.ChargeDate <= EndDate
                                          && t.ClientID == obj.clientid
                                          && sFreightSurchargeTransType.Contains(t.TransType)
                                          select new { t.TransType, t.TransQty, t.TransAmount, t.BillingGroup }).ToList();

                if (obj.billinggroup != "All")
                {
                    qFreightSurcharges = (from t in qFreightSurcharges 
                                          where t.BillingGroup == obj.billinggroup 
                                          select t).ToList();
                }

                var qFreightSurchargesSum = (from t in qFreightSurcharges 
                                             select (t.TransQty * t.TransAmount)).Sum();
                                
                obj.totalfrtHzdSchg = obj.totalfrtHzdSchg + qFreightSurchargesSum;
                
                // All other charges not considered above
                string[] sAdminTransType = { "SAMP", "CFRT", "SFRT", "SAMP", "HAZD", "FLAM", "HEAT", "REFR", "FREZ", "CLEN", "BLND", "NALG", "NITR", "BIOC", "KOSH", "LABL" };

                var qAdminCharges = (from t in db.tblOrderTrans
                                     where
                                         t.ChargeDate >= StartDate && t.ChargeDate <= EndDate
                                         && t.ClientID == obj.clientid
                                         && !sAdminTransType.Contains(t.TransType)
                                     select new { t.TransType, t.TransQty, t.TransAmount, t.BillingGroup }).ToList();

                if (obj.billinggroup != "All")
                {
                    qAdminCharges = (from t in qAdminCharges 
                                     where t.BillingGroup == obj.billinggroup 
                                     select t).ToList();
                }

                var qAdminChargesSum = (from t in qAdminCharges 
                                        select (t.TransQty * t.TransAmount)).Sum();

                obj.totaladmincharge = obj.totaladmincharge + qAdminChargesSum;
                
                // add in clientid level trans
                var qClientTrans = (from t in db.tblOrderTrans
                                    where t.OrderID == null && t.OrderItemID == null
                                    && t.ChargeDate >= StartDate && t.ChargeDate <= EndDate
                                    && t.ClientID == obj.clientid
                                    select new { t.ClientID, t.OrderID, t.OrderItemID, t.TransType, t.TransDate, t.TransQty, t.TransAmount, t.BillingGroup }).ToList();

                if (obj.billinggroup != "All")
                {
                    qClientTrans = (from t in qClientTrans 
                                    where t.BillingGroup == obj.billinggroup 
                                    select t).ToList();
                }

                var qClientTransSum = (from t in qClientTrans 
                                       select (t.TransQty * t.TransAmount)).Sum();

                obj.totaladmincharge = obj.totaladmincharge + qClientTransSum;
                
                // add in order level trans
                var qOrderLevelTrans = (from t in db.tblOrderTrans
                                        where t.OrderID != null
                                        && t.OrderItemID == null
                                        && t.ChargeDate >= StartDate && t.ChargeDate <= EndDate
                                        && t.ClientID == obj.clientid
                                        select new { t.ClientID, t.OrderID, t.OrderItemID, t.TransType, t.TransDate, t.TransQty, t.TransAmount, t.BillingGroup }).ToList();

                if (obj.billinggroup != "All")
                {
                    qOrderLevelTrans = (from t in qOrderLevelTrans 
                                        where t.BillingGroup == obj.billinggroup 
                                        select t).ToList();
                }

                var qOrderLevelTransSum = (from t in qOrderLevelTrans 
                                           select (t.TransQty * t.TransAmount)).Sum();

                obj.totaladmincharge = obj.totaladmincharge + qOrderLevelTransSum;
                
                // return the passed object with more properties filled in
                return obj;
            }
        }

        /// List Creation
        public static List<InvoiceViewModel> IndexList()
        {
            // List for the Index View
            using (var db = new EF.CMCSQL03Entities())
            {
                var obj = (from t in db.tblInvoice
                           //where t.Status == "NEW" ||
                           //     t.Status == "PENDING"
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

        public static List<SelectListItem> ListOfClientIDs()
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                List<SelectListItem> mylist = new List<SelectListItem>();

                mylist = (from t in db.tblClient
                          orderby t.ClientName
                          select new SelectListItem { Value = t.ClientID.ToString(), Text = t.ClientName }).ToList();
                mylist.Insert(0, new SelectListItem { Value = "0", Text = "Select Client" });

                return mylist;
            }
        }

        public static string fnBuildBillingGroupDDL(int clientid)
        {
            using (var db = new EF.CMCSQL03Entities())
            {
                var qry = (from t in db.tblDivision
                           where t.ClientID == clientid
                           orderby t.DivisionID, t.DivisionName
                           select t);

                string s = "<option value='0' selected=true>Select Billing Group</option>";

                if (qry.Count() > 0)
                {
                    foreach (var item in qry)
                    {
                        s = s + "<option value=" + item.DivisionID.ToString() + ">" + item.DivisionName + "</option>";
                    }
                }
                else
                {
                    s = s + "<option value=0>No Billing Group</option>";
                }

                s = s + "</select>";

                return s;
            }
        }
    }
}