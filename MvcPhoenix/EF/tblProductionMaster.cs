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
    
    public partial class tblProductionMaster
    {
        public int ID { get; set; }
        public Nullable<int> BulkID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public string CMCOrderNumber { get; set; }
        public string Company { get; set; }
        public string Division { get; set; }
        public string MasterCode { get; set; }
        public string ProdName { get; set; }
        public Nullable<decimal> Shelf__Life { get; set; }
        public string Lot_Number { get; set; }
        public string Bulk_Location { get; set; }
        public Nullable<decimal> Contents_Weight { get; set; }
        public Nullable<System.DateTime> ExpDt { get; set; }
        public Nullable<System.DateTime> RecDate { get; set; }
        public Nullable<bool> Packout { get; set; }
        public string Status { get; set; }
        public Nullable<int> Priority { get; set; }
        public Nullable<int> ProductionStage { get; set; }
        public Nullable<bool> InvQtyLockedOut { get; set; }
        public Nullable<bool> BulkContainerEmpty { get; set; }
        public Nullable<decimal> FinalDrumWeight { get; set; }
        public string ProductionUserID { get; set; }
        public string ContainerCheckID { get; set; }
        public string LabelerID { get; set; }
        public Nullable<System.DateTime> LabelCheckDate { get; set; }
        public Nullable<int> LabelPageCount { get; set; }
        public string LotNumberUsed { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Special { get; set; }
        public Nullable<System.DateTime> AddedToShelfStock { get; set; }
        public Nullable<bool> Heat_Prior_To_Filling { get; set; }
        public string AirUN { get; set; }
        public Nullable<bool> CleanRoom { get; set; }
        public Nullable<bool> Moisture { get; set; }
        public string CMCUser { get; set; }
        public Nullable<System.DateTime> CeaseShipDate { get; set; }
        public Nullable<System.DateTime> ProdmastCreateDate { get; set; }
        public string BulkContainerNote { get; set; }
        public byte[] TS { get; set; }
    }
}
