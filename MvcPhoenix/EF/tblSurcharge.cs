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
    
    public partial class tblSurcharge
    {
        public int SurchargeID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<decimal> Haz { get; set; }
        public Nullable<decimal> Flam { get; set; }
        public Nullable<decimal> Clean { get; set; }
        public Nullable<decimal> Heat { get; set; }
        public Nullable<decimal> Refrig { get; set; }
        public Nullable<decimal> Freezer { get; set; }
        public Nullable<decimal> Nalgene { get; set; }
        public Nullable<decimal> LabelFee { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public string Company_MDB { get; set; }
        public string Location_MDB { get; set; }
    }
}