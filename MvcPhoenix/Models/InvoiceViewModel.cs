using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class InvoiceViewModel
    {
        [Display(Name = "Invoice ID")]
        public int invoiceid { get; set; }

        [Display(Name = "Invoice Number")]
        public int? invoicenumber { get; set; }

        [Display(Name = "Billing Group")]
        public string billinggroup { get; set; }

        [Display(Name = "Warehouse Location")]
        public string warehouselocation { get; set; }

        public enum WHChoice
        {   
            CT,
            CO,
            EU,
            AP
        }

        [Display(Name = "Client ID")]
        public int? clientid { get; set; }

        [Display(Name = "Client Name")]
        public string clientname { get; set; }
        
        [Display(Name = "Created By")]
        public string createdby { get; set; }
        
        [Display(Name = "Create Date")]
        public DateTimeOffset? createdate { get; set; }

        [Display(Name = "Updated By")]
        public string updatedby { get; set; }

        [Display(Name = "Update Date")]
        public DateTimeOffset? updatedate { get; set; }

        [Display(Name = "Verified Accuracy")]
        public bool? verifiedaccuracy { get; set; }

        [Display(Name = "Verified By")]
        public string verifiedby { get; set; }

        [Display(Name = "Verified Date")]
        public DateTimeOffset? verifieddate { get; set; }

        [Display(Name = "Invoice Status")]
        public string status { get; set; }          // choices: NEW / REVIEWED / VERIFIED / EMAILED / CLOSED

        /// InvoiceStatusChoice is not being used for UX atm.
        /// Will leave here in enum as a reference of options. 
        /// Most choices are used in services and controllers.
        /// Might use choice when creating admin ux for invoice filter
        public enum InvoiceStatusChoice
        {
            NEW,
            REVIEWED,
            VERIFIED,
            EMAILED,
            CLOSED
        }

        [Display(Name = "Invoice Date")]
        public DateTimeOffset? invoicedate { get; set; }

        [Display(Name = "Invoice Period")]
        public string invoiceperiod { get; set; }
        
        [Display(Name = "PO Number")]
        public string ponumber { get; set; }

        [Display(Name = "Net Term")]
        public string netterm { get; set; }

        [AllowHtml]
        [Display(Name = "BILL TO")]
        public string billto { get; set; }

        [AllowHtml]
        [Display(Name = "REMIT TO")]
        public string remitto { get; set; }

        [Display(Name = "Currency")]
        public string currency { get; set; }

        public enum CurrencyChoice
        {
            USD,
            EUR,
            CNY
        }

        [Display(Name = "Tier")]
        public int? tier { get; set; }

        [Display(Name = "Order Type")]
        public string ordertype { get; set; }       //Sample(domestic)/international/Revenue does not apply to all clients

        /////////////////////////////////////////////
        // Shipping Performance
        /////////////////////////////////////////////
        [Display(Name = "Same Day")]
        public int? sampleshipsameday { get; set; }

        [Display(Name = "Next Day")]
        public int? sampleshipnextday { get; set; }
        
        [Display(Name = "Second Day")]
        public int? sampleshipsecondday { get; set; }
        
        [Display(Name = "Other")]
        public int? sampleshipother { get; set; }

        /////////////////////////////////////////////
        // Master Invoice Summary
        /////////////////////////////////////////////
        [Display(Name = "Number of Samples")]
        public int? totalsamples { get; set; }

        [Display(Name = "Total Cost of Processing Samples")]
        public decimal? totalcostsamples { get; set; }
        
        [Display(Name = "Total Freight")]
        public decimal? totalfreight { get; set; }
        
        [Display(Name = "Total Freight Hazardous Surcharge")]
        public decimal? totalfrtHzdSchg { get; set; }

        [Display(Name = "Total Administrative Charges")]
        public decimal? totaladmincharge { get; set; }

        [Display(Name = "Total Due")]
        public decimal? totaldue { get; set; }

        /////////////////////////////////////////////
        // Billing Worksheet
        /////////////////////////////////////////////
        [Display(Name = "Grand Total")]
        public decimal? grandtotal { get; set; }

        [Display(Name = "Samples")]
        public int? samplesqty { get; set; }
        public decimal? samplesrate { get; set; }
        public decimal? samplescharge { get; set; }

        [Display(Name = "Revenue")]
        public int? revenueqty { get; set; }
        public decimal? revenuerate { get; set; }
        public decimal? revenuecharge { get; set; }

        [Display(Name = "Non-Revenue")]
        public int? nonrevenueqty { get; set; }
        public decimal? nonrevenuerate { get; set; }
        public decimal? nonrevenuecharge { get; set; }

        [Display(Name = "Manual Entry")]
        public int? manualentryqty { get; set; }
        public decimal? manualentryrate { get; set; }
        public decimal? manualentrycharge { get; set; }

        [Display(Name = "Follow Up Orders")]
        public int? followupqty { get; set; }
        public decimal? followuprate { get; set; }
        public decimal? followupcharge { get; set; }

        [Display(Name = "Label Printed")]
        public int? labelprtqty { get; set; }
        public decimal? labelprtrate { get; set; }
        public decimal? labelprtcharge { get; set; }

        [Display(Name = "Re-Label Printed")]
        public int? relabelprtqty { get; set; }
        public decimal? relabelprtrate { get; set; }
        public decimal? relabelprtcharge { get; set; }

        [Display(Name = "Re-Label Labor Fee")]
        public int? relabelfeeqty { get; set; }
        public decimal? relabelfeerate { get; set; }
        public decimal? relabelfeecharge { get; set; }

        [Display(Name = "Product Set-Up Changes")]
        public int? productsetupqty { get; set; }
        public decimal? productsetuprate { get; set; }
        public decimal? productsetupcharge { get; set; }

        [Display(Name = "Credit Card Order Processing Fee")]
        public int? ccprocessqty { get; set; }
        public decimal? ccprocessrate { get; set; }
        public decimal? ccprocesscharge { get; set; }

        [Display(Name = "Credit Card Credit")]
        public decimal? cccredit { get; set; }

        [Display(Name = "Global Processing Fee")]
        public decimal? globalprocesscharge { get; set; }

        [Display(Name = "Misc Credits For Failure")]
        public decimal? misccreditcharge { get; set; }

        [Display(Name = "IT Project Support")]
        public decimal? itsupportcharge { get; set; }

        [Display(Name = "Label Stock")]
        public decimal? labelstock { get; set; }

        [Display(Name = "Empty Drum Disposal")]
        public decimal? emptydrumcharge { get; set; }

        [Display(Name = "Same Day Rush Shipment")]
        public int? rushshipqty { get; set; }
        public decimal? rushshiprate { get; set; }
        public decimal? rushshipcharge { get; set; }

        [Display(Name = "Empty Pails")]
        public int? emptypailsqty { get; set; }
        public decimal? emptypailsrate { get; set; }
        public decimal? emptypailscharge { get; set; }

        [Display(Name = "Empty Pails Freight")]
        public decimal? emptypailsfgtcharge { get; set; }

        [Display(Name = "Inactive Stock Storage Surcharge")]
        public int? inactivestockqty { get; set; }
        public decimal? inactivestockrate { get; set; }
        public decimal? inactivestockcharge { get; set; }

        [Display(Name = "Other HM 181 Packaging")]
        public decimal? hm181pkgcharge { get; set; }

        [Display(Name = "Documentation & Handling Fee")]
        public decimal? dochandlingcharge { get; set; }

        [Display(Name = "Minimal Sample Charge")]
        public decimal? minimalsamplecharge { get; set; }

        // Consider month choice for period or possible code magic and date manipulation
    }
}