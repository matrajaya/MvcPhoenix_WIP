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
    
    public partial class tblStock
    {
        public int StockID { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public Nullable<int> ShelfID { get; set; }
        public Nullable<int> BulkID { get; set; }
        public string Warehouse { get; set; }
        public Nullable<int> QtyOnHand { get; set; }
        public Nullable<int> QtyAvailable { get; set; }
        public Nullable<int> QtyAllocated { get; set; }
        public string Bin { get; set; }
        public string ShelfStatus { get; set; }
        public string Company_MDB { get; set; }
        public string Division_MDB { get; set; }
        public string ProdCode_MDB { get; set; }
        public string UM_MDB { get; set; }
        public string LotNumber_MDB { get; set; }
        public string MasterCode_MDB { get; set; }
        public Nullable<System.DateTime> WasteAccumStartDate { get; set; }
        public Nullable<bool> MarkedForReturn { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string MigrationNotes { get; set; }
    }
}
