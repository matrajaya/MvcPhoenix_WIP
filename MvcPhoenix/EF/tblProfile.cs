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
    
    public partial class tblProfile
    {
        public int ProfileID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<int> DivisionID { get; set; }
        public string Division_MDB { get; set; }
        public string BusArea { get; set; }
        public string ProfArea { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Mnemonic { get; set; }
        public string GroundHazard { get; set; }
        public string AirHazard { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> Discontinued { get; set; }
        public string Alert { get; set; }
        public string CMCLocation { get; set; }
        public string CustCode { get; set; }
        public string BillingCode { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public string Company_MDB { get; set; }
        public string Location_MDB { get; set; }
    }
}
