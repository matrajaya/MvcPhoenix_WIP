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
    
        public virtual DbSet<tblCarrier> tblCarrier { get; set; }
        public virtual DbSet<tblClient> tblClient { get; set; }
        public virtual DbSet<tblClientContact> tblClientContact { get; set; }
        public virtual DbSet<tblCountry> tblCountry { get; set; }
        public virtual DbSet<tblCustomer> tblCustomer { get; set; }
        public virtual DbSet<tblDhRate> tblDhRate { get; set; }
        public virtual DbSet<tblDivision> tblDivision { get; set; }
        public virtual DbSet<tblEndUse> tblEndUse { get; set; }
        public virtual DbSet<tblOrderItem> tblOrderItem { get; set; }
        public virtual DbSet<tblOrderMaster> tblOrderMaster { get; set; }
        public virtual DbSet<tblOrderSource> tblOrderSource { get; set; }
        public virtual DbSet<tblOrderTrans> tblOrderTrans { get; set; }
        public virtual DbSet<tblOrderType> tblOrderType { get; set; }
        public virtual DbSet<tblPreferredCarrierList> tblPreferredCarrierList { get; set; }
        public virtual DbSet<tblProductXRef> tblProductXRef { get; set; }
        public virtual DbSet<tblProfile> tblProfile { get; set; }
        public virtual DbSet<tblSampSize> tblSampSize { get; set; }
        public virtual DbSet<tblState> tblState { get; set; }
        public virtual DbSet<tblStatusNotes> tblStatusNotes { get; set; }
        public virtual DbSet<tblSurcharge> tblSurcharge { get; set; }
        public virtual DbSet<tblTier> tblTier { get; set; }
        public virtual DbSet<tblUser> tblUser { get; set; }
        public virtual DbSet<tblClientIncidentalRate> tblClientIncidentalRate { get; set; }
        public virtual DbSet<tblClientInvoiceTrans> tblClientInvoiceTrans { get; set; }
    }
}
