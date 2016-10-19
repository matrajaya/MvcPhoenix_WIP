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
    
    public partial class tblOrderItem
    {
        public int ItemID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<int> ShelfID { get; set; }
        public Nullable<int> ProductDetailID { get; set; }
        public Nullable<int> AllocatedBulkID { get; set; }
        public Nullable<int> AllocatedStockID { get; set; }
        public string ImportItemID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Mnemonic { get; set; }
        public string LotNumber { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Size { get; set; }
        public string SRSize { get; set; }
        public Nullable<System.DateTime> ShipDate { get; set; }
        public Nullable<bool> NonCMCDelay { get; set; }
        public string DelayReason { get; set; }
        public Nullable<bool> BackOrdered { get; set; }
        public string Status { get; set; }
        public string AllocateStatus { get; set; }
        public Nullable<bool> CSAllocate { get; set; }
        public string Bin { get; set; }
        public string CustProdCode { get; set; }
        public string CustProdName { get; set; }
        public string CustSize { get; set; }
        public Nullable<bool> EmailSent { get; set; }
        public Nullable<bool> BackorderEmailSent { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string Warehouse { get; set; }
        public Nullable<int> LineItem { get; set; }
        public Nullable<int> PackID { get; set; }
        public string SPSCharge { get; set; }
        public Nullable<bool> CarrierInvoiceRcvd { get; set; }
        public string GrnUnNumber { get; set; }
        public string GrnPkGroup { get; set; }
        public string AirUnNumber { get; set; }
        public string AirPkGroup { get; set; }
        public string Via { get; set; }
        public Nullable<decimal> ShipWt { get; set; }
        public Nullable<decimal> ShipDimWt { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<System.DateTime> CeaseShipDate { get; set; }
        public string ItemNotes { get; set; }
        public string AlertNotesShipping { get; set; }
        public string AlertNotesPackout { get; set; }
        public string AlertNotesOrderEntry { get; set; }
        public string AlertNotesOther { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string CustOrdNum_MDB { get; set; }
        public string BusArea_MDB { get; set; }
        public Nullable<int> CMCOrder_MDB { get; set; }
        public string LegacyCMCOrder_MDB { get; set; }
        public Nullable<decimal> TotalSurcharge_MDB { get; set; }
        public string SpecialRequest_MDB { get; set; }
        public string ProfitArea_MDB { get; set; }
        public string Currency_MDB { get; set; }
        public string UnitPrice_MDB { get; set; }
        public string CustCode_MDB { get; set; }
        public Nullable<bool> SAPClosed_MDB { get; set; }
        public Nullable<decimal> WasteOrderTotalWeight_MDB { get; set; }
        public string CustStatus_MDB { get; set; }
        public Nullable<int> ProfileID_MDB { get; set; }
        public Nullable<int> LegacyID_MDB { get; set; }
        public string Company_MDB { get; set; }
        public Nullable<decimal> FRT_MDB { get; set; }
        public Nullable<decimal> FrtSurcharge_MDB { get; set; }
        public Nullable<decimal> DH_MDB { get; set; }
        public Nullable<decimal> Other_MDB { get; set; }
    }
}
