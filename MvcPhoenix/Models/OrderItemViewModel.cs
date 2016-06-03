using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OrderItem
    {
        // from tblShelfMaster where ProductDetailID=xxx
        //public List<SelectListItem> ListOfSizes { get; set; }

        public string CrudMode { get; set; }
        public int QtyAvailable { get; set; }

        // select ProductDetailID,ProductCode from tblProductDetail where ClientID=xx
        [Display(Name = "Product Code")]
        [Required(ErrorMessage = "Select Product Code")]
        public int? ProductDetailID { get; set; }

        public List<SelectListItem> ListOfProductDetailIDs { get; set; }

        // dragged from tblProductDetail after insert/update
        public string ProductCode { get; set; }

        // holds the shelfid selected by the user
        public int? ShelfID { get; set; }

        // select ShelfID,Size from tblShelfMaster where ProductDetailID=xxx
        public List<SelectListItem> ListOfShelfIDs { get; set; }

        [Display(Name = "Item ID")]
        public int ItemID { get; set; }

        [Display(Name = "Order ID")]
        public Nullable<int> OrderID { get; set; }

        // Viewmodel only
        [Display(Name = "Client ID")]
        public int? ClientID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Mnemonic")]
        public string Mnemonic { get; set; }

        public int? AllocatedBulkID { get; set; }
        public int? AllocatedStockID { get; set; }

        public string ImportItemID { get; set; }

        // Remove later
        //public Nullable<int> ProfileID { get; set; }
        //public List<SelectListItem> ListOfProfileIDs { get; set; }

        [Display(Name = "Lot Number")]
        public string LotNumber { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Enter Quantity")]
        public Nullable<int> Qty { get; set; }

        [Display(Name = "Size")]
        [Required(ErrorMessage = "Size Required")]
        public string Size { get; set; }

        [Display(Name = "SRSize")]
        public string SRSize { get; set; }

        [Display(Name = "Ship Date")]
        public Nullable<System.DateTime> ShipDate { get; set; }

        [Display(Name = "Non CMC Delay?")]
        public bool? NonCMCDelay { get; set; }

        [Display(Name = "Delay Reason")]
        public string DelayReason { get; set; }

        [Display(Name = "Backordered")]
        public Nullable<bool> BackOrdered { get; set; }

        // Status is a text field !!!
        [Display(Name = "Status")]
        public string Status { get; set; }

        //[Display(Name = "Add a Status")]
        public Nullable<int> StatusID { get; set; }

        [Display(Name = "A-Status")]
        public string AllocateStatus { get; set; }

        public Nullable<bool> CSAllocate { get; set; }

        public string Bin { get; set; }

        public string CustProdCode { get; set; }

        public string CustProdName { get; set; }

        public string CustSize { get; set; }

        public Nullable<bool> EmailSent { get; set; }

        public Nullable<bool> BackorderEmailSent { get; set; }

        //public string Weight { get; set; }
        public decimal? Weight { get; set; }

        public string Warehouse { get; set; }

        public Nullable<int> PackID { get; set; }

        public Nullable<int> LineItem { get; set; }

        public string SPSCharge { get; set; }

        [Display(Name = "Carrier Inv Rec'd")]
        public Nullable<bool> CarrierInvoiceRcvd { get; set; }

        [Display(Name = "Grn UN")]
        public string GrnUnNumber { get; set; }

        [Display(Name = "Grn PckGrp")]
        public string GrnPkGroup { get; set; }

        [Display(Name = "Air UN")]
        public string AirUnNumber { get; set; }

        [Display(Name = "Air PckGrp")]
        public string AirPkGroup { get; set; }

        [Display(Name = "Ship Via")]
        public string Via { get; set; }

        public Nullable<decimal> ShipWt { get; set; }

        public Nullable<decimal> ShipDimWt { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }

        public List<SelectListItem> ListOfStatusNotesIDs { get; set; }
        public string UpdateResult { get; set; }

        // Remove later
        public string PartialMode { get; set; }

        //public Nullable<int> CMCOrder { get; set; }
        //public string LegacyCMCOrder { get; set; }
        //public string CustOrdNum { get; set; }
        //public string BusArea { get; set; }
        //public Nullable<decimal> FRT { get; set; }
        //public Nullable<decimal> FrtSurcharge { get; set; }
        //public Nullable<decimal> DH { get; set; }
        //public Nullable<decimal> Other { get; set; }
        //public Nullable<decimal> TotalSurcharge { get; set; }
        //public string SpecialRequest { get; set; }
        //public string ProfitArea { get; set; }
        //public Nullable<System.DateTime> ExpDate { get; set; }
        //public string Currency { get; set; }
        //public string UnitPrice { get; set; }
        //public string CustCode { get; set; }
        //public Nullable<bool> SAPClosed { get; set; }
        //public Nullable<decimal> WasteOrderTotalWeight { get; set; }
        //public string CustStatus { get; set; }
        //public Nullable<int> LegacyID { get; set; }

        // 5/20/2016
        public string AlertNotesOrderEntry { get; set; }

        public string AlertNotesShipping { get; set; }
        public string AlertNotesOther { get; set; }
        public string AlertNotesPackOut { get; set; }
    }
}