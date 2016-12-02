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
    
    public partial class tblProductDetail
    {
        public int ProductDetailID { get; set; }
        public Nullable<int> ProductMasterID { get; set; }
        public Nullable<int> SGLegacyID { get; set; }
        public Nullable<int> SDLegacyID { get; set; }
        public Nullable<int> DivisionID { get; set; }
        public string BusArea_MDB { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CustCode { get; set; }
        public Nullable<bool> MultiLotReq { get; set; }
        public Nullable<bool> ExtendableExpDt { get; set; }
        public string HarmonizedCode { get; set; }
        public string EndUse { get; set; }
        public Nullable<System.DateTime> SGRevisionDate { get; set; }
        public Nullable<System.DateTime> MSDSRevisionDate { get; set; }
        public string MSDSRevisionNumber { get; set; }
        public Nullable<System.DateTime> LabelRevisionDate { get; set; }
        public string LabelNumber { get; set; }
        public Nullable<bool> ProductChecked { get; set; }
        public string CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckedWhen { get; set; }
        public Nullable<bool> EPABiocide { get; set; }
        public string LabelInfo { get; set; }
        public Nullable<bool> GHSReady { get; set; }
        public Nullable<decimal> CustomsValue { get; set; }
        public string CustomsValueUnit { get; set; }
        public Nullable<bool> GlobalProduct { get; set; }
        public Nullable<bool> AccuracyVerified { get; set; }
        public string AccuracyVerifiedBy { get; set; }
        public Nullable<bool> PolymerizationHazard { get; set; }
        public string SDSContactName { get; set; }
        public string SDSContactPhone { get; set; }
        public Nullable<System.DateTime> ChinaCertificationDate { get; set; }
        public string LabelContactName { get; set; }
        public string LabelContactPhone { get; set; }
        public Nullable<bool> TechnicalSheet { get; set; }
        public Nullable<System.DateTime> TechnicalSheetRevisionDate { get; set; }
        public string EmergencyContactNumber { get; set; }
        public Nullable<bool> EPAHazardousWaste { get; set; }
        public Nullable<bool> NonRCRAWaste { get; set; }
        public string WasteProfileNumber { get; set; }
        public string ShippingChemicalName { get; set; }
        public string LabelNotesEPA { get; set; }
        public string GRNUNNUMBER { get; set; }
        public string GRNPKGRP { get; set; }
        public string GRNHAZSUBCL { get; set; }
        public string GRNLABEL { get; set; }
        public string GRNSUBLABEL { get; set; }
        public string GRNHAZCL { get; set; }
        public string GRNSHIPNAME { get; set; }
        public string GRNOSNAME { get; set; }
        public string GRNSHIPNAMED { get; set; }
        public string GRNTREMACARD { get; set; }
        public string AIRUNNUMBER { get; set; }
        public string AIRPKGRP { get; set; }
        public string AIRHAZSUBCL { get; set; }
        public string AIRLABEL { get; set; }
        public string AIRSUBLABEL { get; set; }
        public string AIRHAZCL { get; set; }
        public string AIRSHIPNAME { get; set; }
        public string AIRNOSNAME { get; set; }
        public string SEAUNNUM { get; set; }
        public string SEAPKGRP { get; set; }
        public string SEAHAZSUBCL { get; set; }
        public string SEALABEL { get; set; }
        public string SEASUBLABEL { get; set; }
        public string SEAHAZCL { get; set; }
        public string SEASHIPNAME { get; set; }
        public string SEANOSNAME { get; set; }
        public string SEASHIPNAMED { get; set; }
        public string SEAHAZMAT { get; set; }
        public string SEAEMSNO { get; set; }
        public string SEAMFAGNO { get; set; }
        public string RCRAUNNumber { get; set; }
        public string RCRAPKGRP { get; set; }
        public string RCRAHAZSUBCL { get; set; }
        public string RCRALABEL { get; set; }
        public string RCRASUBLABEL { get; set; }
        public string RCRAHAZCL { get; set; }
        public string RCRASHIPNAME { get; set; }
        public string RCRANOSNAME { get; set; }
        public Nullable<System.DateTime> ActiveDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string AlertNotesShipping { get; set; }
        public string AlertNotesOrderEntry { get; set; }
        public string Company_MDB { get; set; }
        public string MasterCode_MDB { get; set; }
        public string Division_MDB { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
