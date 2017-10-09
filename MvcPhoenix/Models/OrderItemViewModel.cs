using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OrderItem
    {
        public int? BulkID { get; set; } // for return orders
        public DateTime? AllocatedDate { get; set; } // updated via allocation
        public string CrudMode { get; set; }
        public int? QtyAvailable { get; set; }  // QtyOnHand - QtyAllocated for display purposes only

        [Required(ErrorMessage = "Select Product Code")]
        public int? ProductDetailID { get; set; }
        
        // dragged from tblProductDetail after insert/update
        public string ProductCode { get; set; }

        // holds the shelfid selected by the user
        public int? ShelfID { get; set; }
        public int ItemID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public int? ClientID { get; set; }
        public string ProductName { get; set; }
        public string Mnemonic { get; set; }
        public int? AllocatedBulkID { get; set; }
        public int? AllocatedStockID { get; set; }
        public string ImportItemID { get; set; }
        public string LotNumber { get; set; }
        [Required(ErrorMessage = "Enter Quantity")]
        public Nullable<int> Qty { get; set; }
        [Required(ErrorMessage = "Size Required")]
        public string Size { get; set; }
        public decimal? SRSize { get; set; }
        public Nullable<DateTime> ShipDate { get; set; }
        public string ItemShipVia { get; set; }
        public bool? NonCMCDelay { get; set; }
        public bool? RDTransfer { get; set; }                   // EU needs this for pass through orders that are handled virutally
        public string DelayReason { get; set; }
        public Nullable<bool> BackOrdered { get; set; }
        public string Status { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string AllocateStatus { get; set; }
        public bool? FreezableList { get; set; }
        public Nullable<bool> CSAllocate { get; set; }
        public string Bin { get; set; }
        public string CustProdCode { get; set; }
        public string CustProdName { get; set; }
        public string CustSize { get; set; }
        public Nullable<bool> EmailSent { get; set; }
        public Nullable<bool> BackorderEmailSent { get; set; }
        public decimal? Weight { get; set; }
        public string Warehouse { get; set; }
        public Nullable<int> PackID { get; set; }
        public Nullable<int> LineItem { get; set; }
        public decimal? SPSCharge { get; set; }
        public Nullable<bool> CarrierInvoiceRcvd { get; set; }

        [Display(Name = "Grn UN")]
        public string GrnUnNumber { get; set; }
        [Display(Name = "Grn PckGrp")]
        public string GrnPkGroup { get; set; }
        public string GrnHzdClass { get; set; }
        [Display(Name = "Air UN")]
        public string AirUnNumber { get; set; }
        [Display(Name = "Air PckGrp")]
        public string AirPkGroup { get; set; }
        public string AirHzdClass { get; set; }
        [Display(Name = "Sea UN")]
        public string SeaUnNumber { get; set; }
        [Display(Name = "Sea PckGrp")]
        public string SeaPkGroup { get; set; }
        public string SeaHzdClass { get; set; }


        public string HarmonizedCode { get; set; }
        public string Via { get; set; } // Carrier
        public Nullable<decimal> ShipWt { get; set; }
        public Nullable<decimal> ShipDimWt { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateResult { get; set; }
        public DateTime? CeaseShipDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ItemNotes { get; set; }
        public decimal? WasteOrderTotalWeight { get; set; }
        public string AlertNotesOrderEntry { get; set; }
        public string AlertNotesShipping { get; set; }
        public string AlertNotesOther { get; set; }
        public string AlertNotesPackOut { get; set; }
    }
}