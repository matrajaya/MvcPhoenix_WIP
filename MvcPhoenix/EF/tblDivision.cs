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
    
    public partial class tblDivision
    {
        public int DivisionID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public string Division { get; set; }
        public string DivisionName { get; set; }
        public string BusinessUnit { get; set; }
        public string Abbr { get; set; }
        public Nullable<decimal> WasteRate_OffSpec { get; set; }
        public Nullable<decimal> WasteRate_Empty { get; set; }
        public Nullable<bool> Inactive { get; set; }
        public string ContactLabelName { get; set; }
        public string ContactLabelPhone { get; set; }
        public string ContactMSDSName { get; set; }
        public string ContactMSDSPhone { get; set; }
        public string EmergencyNumber { get; set; }
        public string ERProvider { get; set; }
        public string ERRegistrant { get; set; }
        public string UPSHazBook { get; set; }
        public string ExtMSDS { get; set; }
        public string ExtLabel { get; set; }
        public string MainContactName { get; set; }
        public string MainContactNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyStreet1 { get; set; }
        public string CompanyStreet2 { get; set; }
        public string CompanyStreet3 { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyTelephone { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyEmergencyTelephone { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyLogo2 { get; set; }
        public byte[] LogoFile { get; set; }
        public Nullable<bool> IncludeExpDtOnLabel { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public string Location_MDB { get; set; }
        public string Company_MDB { get; set; }
    }
}
