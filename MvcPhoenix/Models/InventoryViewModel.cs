using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class Inventory
    {
        // build this as a composite view model
		public bool? vmMasterNotesAlert { get; set; }
		
        public ProductProfile PP { get; set; }
        public string ClientCode { get; set; }
        public string Division { get; set; }
        public string ClientUM { get; set; }
        public bool BackOrderPending { get; set; }
        public decimal BulkWeightCurrentlyOnOrder { get; set; }
        public decimal ShelfLevelTotal { get; set; }
        public decimal ShelfLevelAvail { get; set; }
        public decimal ShelfLevelTest { get; set; }
        public decimal ShelfLevelHold { get; set; }
        public decimal ShelfLevelQC { get; set; }
        public decimal ShelfLevelReturn { get; set; }
        public decimal ShelfLevelRecd { get; set; }
        public decimal ShelfLevelOther { get; set; }

        public decimal BulkLevelTotal { get; set; }
        public decimal BulkLevelAvail { get; set; }
        public decimal BulkLevelTest { get; set; }
        public decimal BulkLevelHold { get; set; }
        public decimal BulkLevelQC { get; set; }
        public decimal BulkLevelReturn { get; set; }
        public decimal BulkLevelRecd { get; set; }
        public decimal BulkLevelOther { get; set; }

        public decimal TotalLevelTotal { get; set; }
        public decimal TotalLevelAvail { get; set; }
        public decimal TotalLevelTest { get; set; }
        public decimal TotalLevelHold { get; set; }
        public decimal TotalLevelQC { get; set; }
        public decimal TotalLevelReturn { get; set; }
        public decimal TotalLevelRecd { get; set; }
        public decimal TotalLevelOther { get; set; }
    }

    public class BulkOrderItemForInventory
    {
        // detail
        public int bulkorderitemid { get; set; }
        public int? bulkorderid { get; set; }
        public int? productmasterid { get; set; }
        public string mastercode { get; set; }
        public string mastername { get; set; }
        public decimal? weight { get; set; }
        public string itemstatus { get; set; }
        public DateTime? eta { get; set; }
        public DateTime? datereceived { get; set; }
        public string itemnotes { get; set; }

        // master
        public DateTime? OrderDate { get; set; }
        public string SupplyID { get; set; }
        public string OrderStatus { get; set; }
        public string OrderComment { get; set; }
    }

    public class StockViewModel
    {
        // All fields from tblStock so this class can be used by CRUD
        public int? StockID { get; set; }
        public int? BulkID { get; set; }
        public int? ShelfID { get; set; }

        public List<SelectListItem> ListOfShelfMasterIDs { get; set; }
        public List<SelectListItem> ListOfBulkIDs { get; set; }
        public List<SelectListItem> ListOfShelfStatusIDs { get; set; }
        public List<SelectListItem> ListOfWareHouseIDs { get; set; }

        public string Warehouse { get; set; }
        public int? QtyOnHand { get; set; }
        public int? QtyAllocated { get; set; }
        public string Bin { get; set; }
        public string ShelfStatus { get; set; }
        public DateTime? WasteAccumStartDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public int? ProductDetailID { get; set; }
        public string Size { get; set; }
        public decimal? UnitWeight { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? MfgDate { get; set; }
        public DateTime? QCDate { get; set; }
    }

    public class PrePackStock
    {
        public BulkContainerViewModel BulkContainer { get; set; }
        public List<ShelfMasterViewModel> ListOfShelfMasterIDs { get; set; }
        public int ProductDetailID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int ShelfMasterCount { get; set; }
    }

    public class BulkContainerViewModel
    {
        public decimal? pm_sumofcurrentweight { get; set; } // hold sum(CurrentWeight) for ProductMasterID
        public string pm_MasterNotes { get; set; }
        public string pm_HandlingOther { get; set; }
        public string pm_OtherHandlingInstr { get; set; }
        public bool? pm_refrigerate { get; set; }
        public bool? pm_flammablestorageroom { get; set; }
        public bool? pm_freezablelist { get; set; }
        public bool? pm_refrigeratedlist { get; set; }
        public bool? isknownmaterial { get; set; }
        public string closelist { get; set; }
        public int? clientid { get; set; }
        public string clientname { get; set; }
        public List<SelectListItem> ListOfClients { get; set; }

        public string logofilename { get; set; }

        // ******* basic info
        [Display(Name = "BulkID")]
        public int bulkid { get; set; }

        [Display(Name = "Warehouse")]
        public string warehouse { get; set; }

        public List<SelectListItem> ListOfWareHouses { get; set; }

        [Display(Name = "Receive Date")]
        public DateTime? receivedate { get; set; }

        [Display(Name = "Carrier")]
        [StringLength(12, ErrorMessage = "[Max 12]")]
        public string carrier { get; set; }

        public List<SelectListItem> ListOfCarriers { get; set; }

        [Display(Name = "Received By")]
        [StringLength(10, ErrorMessage = "[Max 10]")]
        public string receivedby { get; set; }

        [Display(Name = "Entered By")]
        [StringLength(10, ErrorMessage = "[Max 10]")]
        public string enteredby { get; set; }

        [Display(Name = "Master Code")]
        public int? productmasterid { get; set; }

        public string MasterName { get; set; }
        public string MasterCode { get; set; }

        public List<SelectListItem> ListOfProductMasters { get; set; }

        [Display(Name = "Receive Weight")]
        public decimal? receiveweight { get; set; }

        // ******* batch and lot info
        [Display(Name = "Lot Number")]
        [StringLength(25, ErrorMessage = "Max 25)")]
        public string lotnumber { get; set; }

        [Display(Name = "Mfg Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? mfgdate { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime? expirationdate { get; set; }

        [Display(Name = "Cease Ship Date")]
        public DateTime? ceaseshipdate { get; set; }

        [Display(Name = "Bulk Status")]
        public string bulkstatus { get; set; }

        public List<SelectListItem> ListOfBulkStatusIDs { get; set; }

        public string qty { get; set; } // default=1, No user interface, remove from db?

        [Display(Name = "Unit Measure")]  // previous fieldname was Container
        [StringLength(20, ErrorMessage = "Max 20)")]
        public string um { get; set; }

        [Display(Name = "Container Color")]
        [StringLength(15, ErrorMessage = "Max 15)")]
        public string containercolor { get; set; }

        [Display(Name = "Bin")]
        [StringLength(25, ErrorMessage = "Max 25)")]
        public string bin { get; set; }

        [Display(Name = "Container Type")]
        [StringLength(1, ErrorMessage = "Max 1)")]
        public string containertype { get; set; }  // Steel, Plastic, Fiber, Other

        public List<SelectListItem> ListOfContainerTypeIDs { get; set; }

        // ******* documentation info
        [Display(Name = "COA Included?")]
        public bool? coaincluded { get; set; }

        [Display(Name = "MSDS Included?")]
        public bool? msdsincluded { get; set; }

        // ******* misc info

        // Display only ?
        [Display(Name = "Current Weight")]
        public decimal? currentweight { get; set; }

        [Display(Name = "QC Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? qcdate { get; set; }

        [Display(Name = "Return Location")]
        [StringLength(10, ErrorMessage = "Max 10)")]
        public string returnlocation { get; set; }

        [Display(Name = "Notice Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? noticedate { get; set; }

        [Display(Name = "Bulk Label Note")]
        [StringLength(100, ErrorMessage = "Max 100)")]
        public string bulklabelnote { get; set; }

        [Display(Name = "Received As Code")]
        [StringLength(20, ErrorMessage = "Max 20)")]
        public string receivedascode { get; set; }

        [Display(Name = "Received As Name")]
        [StringLength(25, ErrorMessage = "Max 25)")]
        public string receivedasname { get; set; }

        [Display(Name = "Container Notes")]
        [StringLength(500, ErrorMessage = "Max 500)")]
        public string containernotes { get; set; }

        [Display(Name = "Other Storage")]
        public string otherstorage { get; set; }

        // Some readonly properties from PM
        public bool? flammable { get; set; }

        public bool? freezer { get; set; }
        public bool? refrigerated { get; set; }

        [Display(Name = "PackOut on Recpt")]
        public bool? packout { get; set; }

        [Display(Name = "Waste Accum Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? wasteaccumstartdate { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}