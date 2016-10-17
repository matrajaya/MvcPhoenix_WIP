using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OrderMasterFull
    {
        public List<SelectListItem> ListOfDivisions { get; set; }

        // public List<SelectListItem> ListOfOrderTypes { get; set; } Discussion on streamlining and trimming current selections. Enum list below
        public List<SelectListItem> ListOfSalesReps { get; set; }

        // public List<SelectListItem> ListOfOrderSources { get; set; } Discussion on streamlining and trimming current selections. Enum list below
        public List<SelectListItem> ListOfCountries { get; set; }

        public List<SelectListItem> ListOfEndUses { get; set; }
        public List<SelectListItem> ListOfShipVias { get; set; }

        // pc 04/28/16 new dd bound per cd, ii
        public List<SelectListItem> ListOfBillingGroups { get; set; }

        public int itemscount { get; set; }
        public int transcount { get; set; }
        public int needallocationcount { get; set; }

        [Display(Name = "Order ID")]
        public int orderid { get; set; }

        [Display(Name = "Client")]
        public int? clientid { get; set; }

        public string clientname { get; set; }
        public string clientcode { get; set; }
        public string logofilename { get; set; }

        [Display(Name = "Ord Status")]
        public string orderstatus { get; set; }

        [Display(Name = "Customer")]
        public string customer { get; set; }

        // Legacy
        // remove later
        //[Display(Name = "CMC Order No.")]
        public int cmcorder { get; set; }

        [Display(Name = "Web Order ID")]
        public int weborderid { get; set; }

        // Legacy
        // remove later
        //[Display(Name = "CMC Legacy Number")]
        public string cmclegacynumber { get; set; }

        [Display(Name = "Cust Order No.")]
        public string custordnum { get; set; }

        [Display(Name = "Cust SAP Number")]
        public string custsapnum { get; set; }

        [Display(Name = "Ref No.")]
        public string custrefnum { get; set; }

        [Display(Name = "Order Type")]
        public string ordertype { get; set; }

        public enum OrderTypeChoice
        {
            [Display(Name = "Sample")]
            S,

            [Display(Name = "Dormant")]
            D,

            [Display(Name = "Return")]
            R,

            [Display(Name = "Commercial")]
            C
        }

        [Display(Name = "Order Date")]
        public DateTime? orderdate { get; set; }

        [Display(Name = "Company")]
        [Required(ErrorMessage = "Ship To Name is required")]
        public string company { get; set; }

        [Display(Name = "Street")]
        public string street { get; set; }

        [Display(Name = "Street2")]
        public string street2 { get; set; }

        [Display(Name = "Street3")]
        public string street3 { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Display(Name = "Country")]
        public string country { get; set; }

        [Display(Name = "Attention")]
        public string attention { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Sales Rep")]
        public string salesrep { get; set; }

        [Display(Name = "Sales Email")]
        public string sales_email { get; set; }

        [Display(Name = "Requestor")]
        public string req { get; set; }

        [Display(Name = "Req Phone")]
        public string reqphone { get; set; }

        [Display(Name = "Req Fax")]
        public string reqfax { get; set; }

        [Display(Name = "Req Email")]
        public string reqemail { get; set; }

        [Display(Name = "End Use")]
        public string enduse { get; set; }

        [Display(Name = "Ship Via")]
        public string shipvia { get; set; }

        [Display(Name = "Ship Account")]
        public string shipacct { get; set; }

        [Display(Name = "Phone")]
        public string phone { get; set; }

        // Change "source" to ordersource and add "clientsource" to tblOrderMaster.
        [Display(Name = "Source")]
        public string source { get; set; }

        [Display(Name = "Order Source")]
        public string ordersource { get; set; }

        public enum OrderSourceChoice
        {
            Manual,
            Web,
            Download,
            Transfer
        }

        // Transfer legacy data from source to client source
        [Display(Name = "Client Source")]
        public string clientsource { get; set; }

        [Display(Name = "Fax")]
        public string fax { get; set; }

        // Legacy Only
        [Display(Name = "Tracking")]
        public string tracking { get; set; }

        [Display(Name = "Special Instr")]
        public string special { get; set; }

        [Display(Name = "Special Int")]
        public string specialinternal { get; set; }

        [Display(Name = "Literature")]
        public bool lit { get; set; }

        [Display(Name = "Region")]
        public string region { get; set; }

        [Display(Name = "COA")]
        public bool coa { get; set; }

        [Display(Name = "TDS")]
        public bool tds { get; set; }

        [Display(Name = "CID")]
        public string cid { get; set; }

        [Display(Name = "Customer Account")]
        public string custacct { get; set; }

        [Display(Name = "ACode")]
        public string acode { get; set; }

        [Display(Name = "Import File Name")]
        public string importfile { get; set; }

        [Display(Name = "Import Date Line")]
        public DateTime? importdateline { get; set; }

        [Display(Name = "Timing")]
        public string timing { get; set; }

        [Display(Name = "Volume")]
        public string volume { get; set; }

        [Display(Name = "Sample Rack")]
        public bool samplerack { get; set; }

        [Display(Name = "CMC User")]
        public string cmcuser { get; set; }

        [Display(Name = "Customer Ref")]
        public string customerreference { get; set; }

        //[Display(Name = "Division")]
        //public string division { get; set; }

        //[Display(Name = "Bus Area")]
        //public string busarea { get; set; }

        // 04/28/2016 Above fields replaced with new field / per cd, ii
        [Display(Name = "Billing Group")]
        public string billinggroup { get; set; }

        [Display(Name = "Total Order Weight")]
        public int? totalorderweight { get; set; }

        [Display(Name = "SPS Tax ID")]
        public string spstaxid { get; set; }

        [Display(Name = "SPS Currency")]
        public string spscurrency { get; set; }

        [Display(Name = "SPS Shipped Wt")]
        public string spsshippedwt { get; set; }

        [Display(Name = "SPS Freight Cost")]
        public string spsfreightcost { get; set; }

        [Display(Name = "Invoice Company")]
        public string invoicecompany { get; set; }

        [Display(Name = "Invoice Title")]
        public string invoicetitle { get; set; }

        [Display(Name = "Invoice First Name")]
        public string invoicefirstname { get; set; }

        [Display(Name = "Invoice Last Name")]
        public string invoicelastname { get; set; }

        [Display(Name = "Invoice Address 1")]
        public string invoiceaddress1 { get; set; }

        [Display(Name = "Invoice Address 2")]
        public string invoiceaddress2 { get; set; }

        [Display(Name = "Invoice Address 3")]
        public string invoiceaddress3 { get; set; }

        [Display(Name = "Invoice City")]
        public string invoicecity { get; set; }

        [Display(Name = "Invoice State")]
        public string invoicestateprov { get; set; }

        [Display(Name = "Invoice Postal Code")]
        public string invoicepostalcode { get; set; }

        [Display(Name = "Invoice Country")]
        public string invoicecountry { get; set; }

        [Display(Name = "Invoice Phone")]
        public string invoicephone { get; set; }

        [Display(Name = "Cust Order Type")]
        public string custordertype { get; set; }

        [Display(Name = "Cust Req Date")]
        public DateTime? custrequestdate { get; set; }

        [Display(Name = "Approval Date")]
        public DateTime? approvaldate { get; set; }

        [Display(Name = "Requested Delivery Date")]
        public DateTime? requesteddeliverydate { get; set; }

        [Display(Name = "Cust Total Items")]
        public int custtotalitems { get; set; }

        [Display(Name = "Cust Requested Carrier")]
        public string custrequestedcarrier { get; set; }

        [Display(Name = "Legacy ID")]
        public int legacyid { get; set; }

        [Display(Name = "Sales Rep Phone")]
        public string salesrepphone { get; set; }

        [Display(Name = "Sales Rep Terr")]
        public string salesrepterritory { get; set; }

        [Display(Name = "Marketing Rep")]
        public string marketingrep { get; set; }

        [Display(Name = "Marketing Rep Email")]
        public string marketingrepemail { get; set; }

        [Display(Name = "Distributor")]
        public string distributor { get; set; }

        [Display(Name = "Preferred Carrier")]
        public string preferredcarrier { get; set; }

        [Display(Name = "Approval Needed")]
        public bool approvalneeded { get; set; }

        public bool? IsSDN { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class OrderImportFile
    {
        public IEnumerable<HttpPostedFileBase> Files { get; set; }

        public string Client { get; set; }

        public IEnumerable<SelectListItem> Clients { get; set; }
    }
}