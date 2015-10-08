using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MvcPhoenix.Models
{
    public class OrderItem
    {
        [Display(Name = "Item ID")]
        public int ItemID { get; set; }

        [Display(Name = "Client ID")]
        public int ClientID { get; set; }

        [Display(Name = "Order ID")]
        public Nullable<int> OrderID { get; set; }

        [Display(Name = "Product Code")]
        [Required(ErrorMessage = "Select Product Code")]
        public Nullable<int> ProfileID { get; set; }

        public List<SelectListItem> ListOfProfileIDs { get; set; }

        [Display(Name = "Lot Number")]
        public string LotNumber { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Enter Quantity")]
        public Nullable<int> Qty { get; set; }

        [Display(Name = "Size")]
        [Required(ErrorMessage = "Size Required")]
        public string Size { get; set; }

        public List<SelectListItem> ListOfSizes { get; set; }

        [Display(Name = "SRSize")]
        public string SRSize { get; set; }

        [Display(Name = "Non CMC Delay?")]
        public Nullable<bool> NonCMCDelay { get; set; }

        [Display(Name = "Invoice Rec'd")]
        public Nullable<bool> CarrierInvoiceRcvd { get; set; }

        [Display(Name = "Add a Status")]
        public Nullable<int> StatusID { get; set; }

        public List<SelectListItem> ListOfStatuses { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Update Result")]
        public string UpdateResult { get; set; }

        [Display(Name = "Mode")]
        public string PartialMode { get; set; }

        [Display(Name = "Delay Reason")]
        public string DelayReason { get; set; }

        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "MNEMONIC")]
        public string MNEMONIC { get; set; }

        [Display(Name = "Ship Date")]
        public Nullable<System.DateTime> ShipDate { get; set; }

        [Display(Name = "Ship Via")]
        public string Via { get; set; }

        [Display(Name = "Backordered")]
        public Nullable<bool> BackOrdered { get; set; }

        public Nullable<int> CMCOrder { get; set; }

        public string LegacyCMCOrder { get; set; }

        public string CustOrdNum { get; set; }

        public string BusArea { get; set; }

        public string ImportItemID { get; set; }

        public Nullable<decimal> FRT { get; set; }

        public Nullable<decimal> FrtSurcharge { get; set; }

        public Nullable<decimal> DH { get; set; }

        public Nullable<decimal> Other { get; set; }

        public Nullable<decimal> TotalSurcharge { get; set; }

        [Display(Name = "Special Request")]
        public string SpecialRequest { get; set; }

        [Display(Name = "Air Hazard")]
        public string AirHazard { get; set; }

        [Display(Name = "Ground Hazard")]
        public string GroundHazard { get; set; }

        public string ProfitArea { get; set; }

        public string AllocateStatus { get; set; }

        public Nullable<bool> CSAllocate { get; set; }

        public string Location { get; set; }

        [Display(Name = "Expiry Date")]
        public Nullable<System.DateTime> ExpDate { get; set; }

        public string CustProdCode { get; set; }

        public string CustProdName { get; set; }

        public string CustSize { get; set; }

        public string Currency { get; set; }

        public string UnitPrice { get; set; }

        public Nullable<bool> EmailSent { get; set; }

        public Nullable<bool> BackorderEmailSent { get; set; }

        public string CustCode { get; set; }

        public string Weight { get; set; }

        public string CMCLocation { get; set; }

        public Nullable<int> LineItem { get; set; }

        public Nullable<bool> SAPClosed { get; set; }

        public Nullable<int> PackID { get; set; }

        public Nullable<decimal> ShipWt { get; set; }

        public Nullable<decimal> ShipDimWt { get; set; }

        public string SPSCost { get; set; }

        public Nullable<decimal> WasteOrderTotalWeight { get; set; }

        public string CustStatus { get; set; }

        public Nullable<int> LegacyID { get; set; }
        
        public OrderItem()
        {
            NonCMCDelay = false;
            BackOrdered = false;
            CSAllocate = false;
            EmailSent = false;
            BackorderEmailSent = false;
            SAPClosed = false;
            CarrierInvoiceRcvd = false;
        }
    }
}