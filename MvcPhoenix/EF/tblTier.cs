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
    
    public partial class tblTier
    {
        public int TierID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<int> TierLevel { get; set; }
        public string Size { get; set; }
        public Nullable<int> LoSampAmt { get; set; }
        public Nullable<int> HiSampAmt { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public string Company_MDB { get; set; }
        public string Location_MDB { get; set; }
    }
}
