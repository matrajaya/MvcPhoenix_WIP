using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OrderMasterFull
    {
        public int itemscount { get; set; }
        public int transcount { get; set; }
        public int needallocationcount { get; set; }

        [Display(Name = "Order ID")]
        public int orderid { get; set; }

        [Display(Name = "Client")]
        public int? clientid { get; set; }

        [Display(Name = "Division ID")]
        public int? divisionid { get; set; }

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

        // Legacy; remove later
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

        [Display(Name = "Customs Reference Number")]
        public string CustomsRefNum { get; set; }

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

        [Display(Name = "Billing Group")]                       // Propose changing to standard divisionid instead
        public string billinggroup { get; set; }

        [Display(Name = "Total Order Weight")]
        public int? totalorderweight { get; set; }
        
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

    public class OrderSPSBilling
    {
        public int SPSBillingID { get; set; }
        public int OrderId { get; set; }
        public string Type { get; set; }                            // CC or Invoice
        public string TaxId { get; set; }
        public string Currency { get; set; }                        // USD or EUR or CNY ...necessary?
        public decimal? PriceCost { get; set; }                     // Sum of SPSCharge in Order Item
        public decimal? FreightCost { get; set; }
        public decimal? ShippedWeight { get; set; }
        public string InvoiceTitle { get; set; }
        public string InvoiceFirstName { get; set; }
        public string InvoiceLastName { get; set; }
        public string InvoiceCompany { get; set; }
        public string InvoiceAddress1 { get; set; }
        public string InvoiceAddress2 { get; set; }
        public string InvoiceAddress3 { get; set; }
        public string InvoiceCity { get; set; }
        public string InvoiceState { get; set; }
        public string InvoicePostalCode { get; set; }
        public string InvoiceCountry { get; set; }
        public string InvoicePhone { get; set; }
        public string InvoiceEmail { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class OrderImportFile
    {
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
        public string Client { get; set; }
        public IEnumerable<SelectListItem> Clients { get; set; }
    }

    public class PreferredCarrierViewModel
    {
        public int ID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public bool? CommInvoiceReq { get; set; }
        public string NonHazSm { get; set; }
        public string NonHazLg { get; set; }
        public string NonHazIncoTerms { get; set; }
        public string HazIATASm { get; set; }
        public string HazIATALg { get; set; }
        public string HazGroundLQ { get; set; }
        public string HazGround { get; set; }
        public string HazIncoterms { get; set; }
        public string IncotermsAlt { get; set; }
        public string NotesGeneral { get; set; }
        public string NotesIATAADR { get; set; }
        public string NonHazIncotermsAlt { get; set; }
        public string HazIncotermsAlt { get; set; }
    }
}