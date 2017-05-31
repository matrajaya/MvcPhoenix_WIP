//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcPhoenix.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblOrderMaster
    {
        public int OrderID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<int> DivisionID { get; set; }
        public string Customer { get; set; }
        public Nullable<int> CMCOrder { get; set; }
        public Nullable<int> WebOrderID { get; set; }
        public string CMCLegacyNum { get; set; }
        public string CustOrdNum { get; set; }
        public string CustSapNum { get; set; }
        public string CustRefNum { get; set; }
        public string ShipRef { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string Company { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Attention { get; set; }
        public string Email { get; set; }
        public string CustomsRefNum { get; set; }
        public string SalesRep { get; set; }
        public string SalesEmail { get; set; }
        public string Req { get; set; }
        public string ReqPhone { get; set; }
        public string ReqFax { get; set; }
        public string ReqEmail { get; set; }
        public string EndUse { get; set; }
        public string ShipVia { get; set; }
        public string ShipAcct { get; set; }
        public string Phone { get; set; }
        public string Source { get; set; }
        public string Fax { get; set; }
        public string Tracking { get; set; }
        public string Special { get; set; }
        public string SpecialInternal { get; set; }
        public Nullable<bool> Lit { get; set; }
        public string Region { get; set; }
        public Nullable<bool> COA { get; set; }
        public Nullable<bool> TDS { get; set; }
        public string CID { get; set; }
        public string CustAcct { get; set; }
        public string ACode { get; set; }
        public string ImportFile { get; set; }
        public Nullable<System.DateTime> ImportDateLine { get; set; }
        public string Timing { get; set; }
        public string Volume { get; set; }
        public Nullable<bool> SampleRack { get; set; }
        public string CMCUser { get; set; }
        public string CustomerReference { get; set; }
        public string Division_MDB { get; set; }
        public string BillingGroup { get; set; }
        public Nullable<int> TotalOrderWeight { get; set; }
        public string SPSTaxID { get; set; }
        public string SPSCurrency { get; set; }
        public string SPSShippedWt { get; set; }
        public string SPSFreightCost { get; set; }
        public string InvoiceCompany { get; set; }
        public string InvoiceTitle { get; set; }
        public string InvoiceFirstName { get; set; }
        public string InvoiceLastName { get; set; }
        public string InvoiceAddress1 { get; set; }
        public string InvoiceAddress2 { get; set; }
        public string InvoiceAddress3 { get; set; }
        public string InvoiceCity { get; set; }
        public string InvoiceStateProv { get; set; }
        public string InvoicePostalCode { get; set; }
        public string InvoiceCountry { get; set; }
        public string InvoicePhone { get; set; }
        public string CustOrderType { get; set; }
        public Nullable<System.DateTime> CustRequestDate { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        public Nullable<System.DateTime> RequestedDeliveryDate { get; set; }
        public Nullable<int> CustTotalItems { get; set; }
        public string CustRequestedCarrier { get; set; }
        public string SalesRepPhone { get; set; }
        public string SalesRepTerritory { get; set; }
        public string MarketingRep { get; set; }
        public string MarketingRepEmail { get; set; }
        public string Distributor { get; set; }
        public string PreferredCarrier { get; set; }
        public Nullable<bool> ApprovalNeeded { get; set; }
        public Nullable<bool> IsSDN { get; set; }
        public Nullable<bool> IsSDNOverride { get; set; }
        public Nullable<int> CeaseShipOffset { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> LegacyID { get; set; }
    }
}
