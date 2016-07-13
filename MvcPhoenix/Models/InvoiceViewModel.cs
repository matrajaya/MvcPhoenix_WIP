using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    public class InvoiceViewModel
    {
        [Display(Name = "Created Date")]
        public DateTime? createddate { get; set; }
        
        [Display(Name = "Created By")]
        public string createdby { get; set; }
        
        [Display(Name = "Verified Date")]
        public DateTime? verifieddate { get; set; }

        [Display(Name = "Invoice Status")]
        public string status { get; set; }

        [Display(Name = "Client")]
        public string clientname { get; set; }
        
        [Display(Name = "Billing Group")]
        public string billinggroup { get; set; }

        public string periodmonth { get; set; }
        public string periodyear { get; set; }

        [Display(Name = "Invoice Number")]
        public int invoiceid { get; set; }

        [Display(Name = "Invoice Date")]
        public DateTime? invoicedate { get; set; }

        [Display(Name = "PO Number")]
        public int ponumber { get; set; }

        [Display(Name = "Net")]
        public int netterm { get; set; }

        [Display(Name = "BILL TO")]
        public string billtoaddress { get; set; }
        public string billtoattention { get; set; }

        [Display(Name = "REMIT TO")]
        public string remitto { get; set; }
        
        // Master Invoice Summary
        [Display(Name = "Number of Samples")]
        public int totalsamples { get; set; }

        [Display(Name = "Total Cost of Processing Samples")]
        public decimal totalcostsampleprocessing { get; set; }
        
        [Display(Name = "Total Freight")]
        public decimal totalfreight { get; set; }
        
        [Display(Name = "Freight Hazardous Surcharge")]
        public decimal totalfreightHzdSchg { get; set; }

        [Display(Name = "Administrative Charges")]
        public decimal totaladmincharge { get; set; }

        [Display(Name = "Total Due")]
        public decimal totaldue { get; set; }

        // Total Shipping Performance
        [Display(Name = "Same Day")]
        public int sampleshipsameday { get; set; }

        [Display(Name = "Next Day")]
        public int sampleshipnextday { get; set; }
        
        [Display(Name = "Second Day")]
        public int sampleshipsecondday { get; set; }
        
        [Display(Name = "Other")]
        public int sampleshipother { get; set; }

        // public string invoicefilelocation { get; set; }

        // Billing Worksheet
        [Display(Name = "Grand Total")]
        public decimal grandtotal { get; set; }

        [Display(Name = "Manual Order Entry")]
        public int manualentryqty { get; set; }
        public decimal manualentryrate { get; set; }
        public decimal manualentrytotal { get; set; }

        [Display(Name = "Follow Up Orders")]
        public int followuporderqty { get; set; }
        public decimal followuporderrate { get; set; }
        public decimal followupordertotal { get; set; }

        [Display(Name = "Re-Label Labor Fee")]
        public int relabelfeeqty { get; set; }
        public decimal relabelfeerate { get; set; }
        public decimal relabelfeetotal { get; set; }

        [Display(Name = "Change in Product Set-Up")]
        public int productsetupqty { get; set; }
        public decimal productsetuprate { get; set; }
        public decimal productsetuptotal { get; set; }

        [Display(Name = "Credit Card Order Processing Fee")]
        public int ccorderprocessqty { get; set; }
        public decimal ccorderprocessrate { get; set; }
        public decimal ccorderprocesstotal { get; set; }

        [Display(Name = "Credit Card Credit")]
        public decimal cccredittotal { get; set; }

        [Display(Name = "Global Processing Fee")]
        public decimal globalprocessingfeetotal { get; set; }

        [Display(Name = "Misc Credits")]
        public int misccreditqty { get; set; }
        public decimal misccreditrate { get; set; }
        public decimal misccredittotal { get; set; }

        [Display(Name = "Same Day Rush Shipment")]
        public int samedayrushqty { get; set; }
        public decimal samedayrushrate { get; set; }
        public decimal samedayrushtotal { get; set; }

        [Display(Name = "Empty Drum Disposal")]
        public decimal emptydrumdisposaltotal { get; set; }

        [Display(Name = "Empty Pails")]
        public int emptypailsqty { get; set; }
        public decimal emptypailsrate { get; set; }
        public decimal emptypailstotal { get; set; }

        [Display(Name = "Empty Pails Freight")]
        public decimal emptypailsfgttotal { get; set; }

        [Display(Name = "Inactive Stock Storage Surcharge")]
        public int inactivestksurchqty { get; set; }
        public decimal inactivestksurchrate { get; set; }
        public decimal inactivestksurchtotal { get; set; }
    }
}