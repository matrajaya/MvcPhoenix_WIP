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
    
    public partial class tblBulkOrder
    {
        public int BulkOrderID { get; set; }
        public Nullable<int> LegacyOrderNumber { get; set; }
        public Nullable<int> LegacyOrderNumber_Acc2 { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public string SupplyID { get; set; }
        public string BulkSupplierEmail { get; set; }
        public string EmailSent { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string Company_MDB { get; set; }
        public string Division_MDB { get; set; }
        public string Location_MDB { get; set; }
        public string MigrationNotes { get; set; }
    }
}
