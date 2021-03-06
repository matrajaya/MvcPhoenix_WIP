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
    
    public partial class tblInvLog
    {
        public int LogID { get; set; }
        public string LogType { get; set; }
        public Nullable<System.DateTime> LogDate { get; set; }
        public Nullable<int> BulkID { get; set; }
        public Nullable<int> StockID { get; set; }
        public Nullable<int> ProductMasterID { get; set; }
        public Nullable<int> ProductDetailID { get; set; }
        public Nullable<int> LogQty { get; set; }
        public Nullable<decimal> LogAmount { get; set; }
        public string UM { get; set; }
        public string LogNotes { get; set; }
        public Nullable<int> ClientID { get; set; }
        public string ClientName { get; set; }
        public string Status { get; set; }
        public string Warehouse { get; set; }
        public string Lotnumber { get; set; }
        public string BulkBin { get; set; }
        public string ShelfBin { get; set; }
        public string Size { get; set; }
        public string MasterCode { get; set; }
        public string MasterName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Nullable<System.DateTime> ShipDate { get; set; }
        public Nullable<int> CurrentQtyAvailable { get; set; }
        public Nullable<decimal> CurrentWeightAvailable { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<System.DateTime> CeaseShipDate { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public Nullable<System.DateTime> DateReceived { get; set; }
        public Nullable<System.DateTime> QCDate { get; set; }
        public Nullable<int> PackOutID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public byte[] TS { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public string LogRecordStatus_MDB { get; set; }
        public string Company_MDB { get; set; }
        public string Division_MDB { get; set; }
        public string ProductCode_MDB { get; set; }
        public string MasterCode_MDB { get; set; }
        public string LotNumber_MDB { get; set; }
        public string Size_MDB { get; set; }
        public string Source_MDB { get; set; }
        public string Source_Table { get; set; }
    }
}
