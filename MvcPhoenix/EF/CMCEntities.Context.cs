﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CMCSQL03Entities : DbContext
    {
        public CMCSQL03Entities()
            : base("name=CMCSQL03Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblBulk> tblBulk { get; set; }
        public virtual DbSet<tblBulkOrder> tblBulkOrder { get; set; }
        public virtual DbSet<tblBulkOrderItem> tblBulkOrderItem { get; set; }
        public virtual DbSet<tblBulkSupplier> tblBulkSupplier { get; set; }
        public virtual DbSet<tblBulkUnKnown> tblBulkUnKnown { get; set; }
        public virtual DbSet<tblCarrier> tblCarrier { get; set; }
        public virtual DbSet<tblCAS> tblCAS { get; set; }
        public virtual DbSet<tblClient> tblClient { get; set; }
        public virtual DbSet<tblClient2> tblClient2 { get; set; }
        public virtual DbSet<tblClientContact> tblClientContact { get; set; }
        public virtual DbSet<tblClientIncidentalRate> tblClientIncidentalRate { get; set; }
        public virtual DbSet<tblClientInvoiceTrans> tblClientInvoiceTrans { get; set; }
        public virtual DbSet<tblCountry> tblCountry { get; set; }
        public virtual DbSet<tblCustomer> tblCustomer { get; set; }
        public virtual DbSet<tblDhRate> tblDhRate { get; set; }
        public virtual DbSet<tblDivision> tblDivision { get; set; }
        public virtual DbSet<tblEndUse> tblEndUse { get; set; }
        public virtual DbSet<tblEndUseForCustoms> tblEndUseForCustoms { get; set; }
        public virtual DbSet<tblHSCode> tblHSCode { get; set; }
        public virtual DbSet<tblInvLog> tblInvLog { get; set; }
        public virtual DbSet<tblInvoiceDetail> tblInvoiceDetail { get; set; }
        public virtual DbSet<tblInvoiceMaster> tblInvoiceMaster { get; set; }
        public virtual DbSet<tblInvTrans> tblInvTrans { get; set; }
        public virtual DbSet<tblLog> tblLog { get; set; }
        public virtual DbSet<tblOrderImport> tblOrderImport { get; set; }
        public virtual DbSet<tblOrderItem> tblOrderItem { get; set; }
        public virtual DbSet<tblOrderMaster> tblOrderMaster { get; set; }
        public virtual DbSet<tblOrderSource> tblOrderSource { get; set; }
        public virtual DbSet<tblOrderTrans> tblOrderTrans { get; set; }
        public virtual DbSet<tblOrderType> tblOrderType { get; set; }
        public virtual DbSet<tblPackage> tblPackage { get; set; }
        public virtual DbSet<tblPackageType> tblPackageType { get; set; }
        public virtual DbSet<tblPMNote> tblPMNote { get; set; }
        public virtual DbSet<tblPreferredCarrierList> tblPreferredCarrierList { get; set; }
        public virtual DbSet<tblProductDetail> tblProductDetail { get; set; }
        public virtual DbSet<tblProductMaster> tblProductMaster { get; set; }
        public virtual DbSet<tblProductNotes> tblProductNotes { get; set; }
        public virtual DbSet<tblProductXRef> tblProductXRef { get; set; }
        public virtual DbSet<tblProfile_MDB> tblProfile_MDB { get; set; }
        public virtual DbSet<tblReasonCode> tblReasonCode { get; set; }
        public virtual DbSet<tblReportCriteria> tblReportCriteria { get; set; }
        public virtual DbSet<tblShelfMaster> tblShelfMaster { get; set; }
        public virtual DbSet<tblSQlTable> tblSQlTable { get; set; }
        public virtual DbSet<tblState> tblState { get; set; }
        public virtual DbSet<tblStatusNotes> tblStatusNotes { get; set; }
        public virtual DbSet<tblStock> tblStock { get; set; }
        public virtual DbSet<tblSuggestedBulk> tblSuggestedBulk { get; set; }
        public virtual DbSet<tblSurcharge> tblSurcharge { get; set; }
        public virtual DbSet<tblSystem> tblSystem { get; set; }
        public virtual DbSet<tblTier> tblTier { get; set; }
        public virtual DbSet<tblTransType> tblTransType { get; set; }
        public virtual DbSet<tblUN> tblUN { get; set; }
        public virtual DbSet<tblUser> tblUser { get; set; }
        public virtual DbSet<tblWasteCode> tblWasteCode { get; set; }
        public virtual DbSet<vwBulkTransForReplenishment> vwBulkTransForReplenishment { get; set; }
        public virtual DbSet<vwProductsInfo> vwProductsInfo { get; set; }
        public virtual DbSet<vwShelfStockForReplenishment> vwShelfStockForReplenishment { get; set; }
        public virtual DbSet<vwShelfTransForReplenishment> vwShelfTransForReplenishment { get; set; }
        public virtual DbSet<tblInvoice> tblInvoice { get; set; }
    }
}
