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
    
    public partial class tblBulkOrderItem
    {
        public int BulkOrderItemID { get; set; }
        public Nullable<int> BulkOrderID { get; set; }
        public Nullable<int> ProductMasterID { get; set; }
        public Nullable<bool> ToBeClosed { get; set; }
        public Nullable<int> LegacyOrderNumber { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CustCode { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> ETA { get; set; }
        public Nullable<System.DateTime> DateReceived { get; set; }
        public Nullable<int> AmountReceived { get; set; }
        public string SupplyID { get; set; }
        public string ItemNotes { get; set; }
        public Nullable<decimal> ShelfLife { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public string Size_MDB { get; set; }
        public string Location_MDB { get; set; }
    }
}
