//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

using MvcPhoenix.Models;
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
                var q = (from t in db.tblInvoice where t.InvoiceID == id select t).FirstOrDefault();

                IVM.invoiceid = q.InvoiceID;
                IVM.billinggroup = q.BillingGroup;
                IVM.warehouselocation = q.WarehouseLocation;
                IVM.clientid = q.ClientID;
                IVM.clientname = q.ClientName;
                IVM.createdby = q.CreatedBy;
                IVM.createdate = q.CreateDate;
                IVM.updatedby = q.UpdatedBy;
                IVM.updatedate = q.UpdateDate;
                IVM.isverified = q.IsVerified;
                IVM.verifiedby = q.VerifiedBy;
                IVM.verifieddate = q.VerifyDate;
                IVM.status = q.Status;
                IVM.invoicedate = q.InvoiceDate;
                IVM.invoiceperiod = q.InvoicePeriod;        //extract month and year from invoice date conver tto string
                IVM.periodmonth = q.PeriodMonth;
                IVM.periodyear = q.PeriodYear;
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
                }

                // Capture user info in viewmodel
                vm.updatedby = HttpContext.Current.User.Identity.Name;
                vm.updatedate = System.DateTime.Now;

                var q = (from t in db.tblInvoice where t.InvoiceID == vm.invoiceid select t).FirstOrDefault();

                q.BillingGroup = vm.billinggroup;
                q.WarehouseLocation = vm.warehouselocation;
                q.ClientID = vm.clientid;
                q.ClientName = vm.clientname;
                q.CreatedBy = vm.createdby;
                q.CreateDate = vm.createdate;
                q.UpdatedBy = vm.updatedby;
                q.UpdateDate = vm.updatedate;
                q.IsVerified = vm.isverified;
                q.VerifiedBy = vm.verifiedby;
                q.VerifyDate = vm.verifieddate;
                q.Status = vm.status;
                q.InvoiceDate = vm.invoicedate;
                q.InvoicePeriod = vm.invoiceperiod;
                q.PeriodMonth = vm.periodmonth;
                q.PeriodYear = vm.periodyear;
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
    }
}