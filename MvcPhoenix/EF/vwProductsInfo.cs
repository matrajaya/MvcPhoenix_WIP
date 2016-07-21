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
    
    public partial class vwProductsInfo
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string CMCLocation { get; set; }
        public Nullable<int> DivisionID { get; set; }
        public string Division { get; set; }
        public string CompanyCountry { get; set; }
        public int ProductMasterID { get; set; }
        public string MasterName { get; set; }
        public string MasterCode { get; set; }
        public string ProductName { get; set; }
        public string EndUse { get; set; }
        public Nullable<bool> GHSReady { get; set; }
        public Nullable<int> ProductDetailID { get; set; }
        public string Notes { get; set; }
        public string ReasonCode { get; set; }
        public string Warehouse { get; set; }
        public string Size { get; set; }
        public Nullable<decimal> UnitWeight { get; set; }
        public Nullable<decimal> ReorderMin { get; set; }
        public Nullable<decimal> ReorderMax { get; set; }
        public Nullable<bool> HazardSurcharge { get; set; }
        public Nullable<bool> FlammableSurcharge { get; set; }
        public Nullable<bool> HeatSurcharge { get; set; }
        public Nullable<bool> RefrigSurcharge { get; set; }
        public Nullable<bool> FreezerSurcharge { get; set; }
        public Nullable<bool> CleanSurcharge { get; set; }
        public Nullable<bool> BlendSurcharge { get; set; }
        public Nullable<bool> NalgeneSurcharge { get; set; }
        public Nullable<bool> NitrogenSurcharge { get; set; }
        public Nullable<bool> BiocideSurcharge { get; set; }
        public Nullable<bool> KosherSurcharge { get; set; }
        public Nullable<bool> LabelSurcharge { get; set; }
        public Nullable<bool> OtherSurcharge { get; set; }
        public Nullable<decimal> OtherSurchargeAmt { get; set; }
        public Nullable<bool> WebOEInclude { get; set; }
        public string PackagePartNumber_MDB { get; set; }
    }
}
