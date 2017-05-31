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
        public int orderid { get; set; }
        public int? clientid { get; set; }
        public int? divisionid { get; set; }
        public string clientname { get; set; }
        public string clientcode { get; set; }
        public string logofilename { get; set; }
        public string orderstatus { get; set; }
        public string customer { get; set; }
        public int cmcorder { get; set; }                              // Legacy; remove later
        public int weborderid { get; set; }                            // Legacy; remove later
        public string cmclegacynumber { get; set; }
        public string custordnum { get; set; }
        public string custsapnum { get; set; }
        public string custrefnum { get; set; }
        public string ordertype { get; set; }
        public DateTime? orderdate { get; set; }
        [Required(ErrorMessage = "Ship To Name is required")]
        public string company { get; set; }
        public string street { get; set; }
        public string street2 { get; set; }
        public string street3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string Zip { get; set; }
        public string country { get; set; }
        public string attention { get; set; }
        public string email { get; set; }
        public string CustomsRefNum { get; set; }
        public string salesrep { get; set; }
        public string sales_email { get; set; }
        public string req { get; set; }
        public string reqphone { get; set; }
        public string reqfax { get; set; }
        public string reqemail { get; set; }
        public string enduse { get; set; }
        public string shipvia { get; set; }
        public string shipacct { get; set; }
        public string phone { get; set; }
        public string source { get; set; }                              // Change "source" to ordersource and add "clientsource" to tblOrderMaster.
        public string ordersource { get; set; }
        public string clientsource { get; set; }                        // Transfer legacy data from source to client source
        public string fax { get; set; }
        public string tracking { get; set; }                            // Legacy Only
        public string special { get; set; }
        public string specialinternal { get; set; }
        public bool lit { get; set; }
        public string region { get; set; }
        public bool coa { get; set; }
        public bool tds { get; set; }
        public string cid { get; set; }
        public string custacct { get; set; }
        public string acode { get; set; }
        public string importfile { get; set; }
        public DateTime? importdateline { get; set; }
        public string timing { get; set; }
        public string volume { get; set; }
        public bool samplerack { get; set; }
        public string cmcuser { get; set; }
        public string customerreference { get; set; }
        public string billinggroup { get; set; }                       // Propose changing to standard divisionid instead
        public int? totalorderweight { get; set; }
        public string custordertype { get; set; }
        public DateTime? custrequestdate { get; set; }
        public DateTime? approvaldate { get; set; }
        public DateTime? requesteddeliverydate { get; set; }
        public int custtotalitems { get; set; }
        public string custrequestedcarrier { get; set; }
        public int legacyid { get; set; }
        public string salesrepphone { get; set; }
        public string salesrepterritory { get; set; }
        public string marketingrep { get; set; }
        public string marketingrepemail { get; set; }
        public string distributor { get; set; }
        public string preferredcarrier { get; set; }
        public bool approvalneeded { get; set; }
        public bool? IsSDN { get; set; }
        public bool? IsSDNOverride { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class OrderSPSBilling
    {
        public int SPSBillingID { get; set; }
        public int OrderId { get; set; }
        public string Type { get; set; }                                // CC or Invoice
        public string TaxId { get; set; }
        public string Currency { get; set; }                            // USD or EUR or CNY ...necessary?
        public decimal? PriceCost { get; set; }                         // Sum of SPSCharge in Order Item
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