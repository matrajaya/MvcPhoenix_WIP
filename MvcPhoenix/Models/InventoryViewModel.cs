using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class Inventory
    {
        public ProductProfile ProductProfile { get; set; }

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
        public DateTime? OrderDate { get; set; }
        public string SupplyID { get; set; }
        public string OrderStatus { get; set; }
        public string OrderComment { get; set; }
    }

    public class StockViewModel
    {
        public List<SelectListItem> ListOfShelfMasterIDs { get; set; }

        public int? StockID { get; set; }
        public int? BulkID { get; set; }
        public int? ShelfID { get; set; }
        public string Warehouse { get; set; }
        public int? QtyOnHand { get; set; }
        public int? QtyAvailable { get; set; }
        public int? QtyAllocated { get; set; }

        [StringLength(15, ErrorMessage = "[Max 15]")]
        public string Bin { get; set; }

        public string ShelfStatus { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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
        public DateTime? CeaseShipDate { get; set; }
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
        public decimal? pm_sumofcurrentweight { get; set; }                         // hold sum(CurrentWeight) for ProductMasterID
        public string pm_OtherHandlingInstr { get; set; }
        public bool? pm_refrigerate { get; set; }
        public bool? pm_flammablestorageroom { get; set; }
        public bool? pm_freezerstorage { get; set; }
        public string pm_otherstorage { get; set; }
        public bool? pm_cleanroomgmp { get; set; }
        public string pm_alertnotesreceiving { get; set; }
        public decimal? pm_restrictedtoamount { get; set; }
        public bool? pm_tempraturecontrolledstorage { get; set; }
        public decimal? pm_shelflife { get; set; }
        public bool? pm_packoutonreceipt { get; set; }
        public int? pm_ceaseshipdifferential { get; set; }
        public string pd_groundunnum { get; set; }
        public string pd_groundpackinggrp { get; set; }
        public string pd_groundhazardclass { get; set; }
        public string pd_groundhazardsubclass { get; set; }
        public bool? pd_epabiocide { get; set; }
        public string ghs_signalword { get; set; }
        public string ghs_symbol1 { get; set; }
        public string ghs_symbol2 { get; set; }
        public string ghs_symbol3 { get; set; }
        public string ghs_symbol4 { get; set; }
        public string ghs_symbol5 { get; set; }
        public bool? isknownmaterial { get; set; }
        public string closelist { get; set; }
        public int? clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }

        // ******* basic info
        public int bulkid { get; set; }

        public string warehouse { get; set; }
        public DateTime? receivedate { get; set; }

        [StringLength(12, ErrorMessage = "[Max 12]")]
        public string carrier { get; set; }

        public string receivedby { get; set; }
        public string enteredby { get; set; }
        public int? productmasterid { get; set; }
        public string MasterName { get; set; }
        public string MasterCode { get; set; }
        public decimal? receiveweight { get; set; }

        // ******* batch and lot info
        [StringLength(25, ErrorMessage = "Max 25)")]
        public string lotnumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? mfgdate { get; set; }

        public DateTime? expirationdate { get; set; }
        public DateTime? ceaseshipdate { get; set; }
        public string bulkstatus { get; set; }
        public string qty { get; set; }                                         // default=1, No user interface, remove from db?

        [StringLength(20, ErrorMessage = "Max 20)")]
        public string um { get; set; }                                          // previous fieldname was Container

        [StringLength(15, ErrorMessage = "Max 15)")]
        public string containercolor { get; set; }

        [StringLength(25, ErrorMessage = "Max 25)")]
        public string bin { get; set; }

        public string containertype { get; set; }                               // Steel, Plastic, Fiber, Other
        public bool? coaincluded { get; set; }
        public bool? msdsincluded { get; set; }
        public decimal? currentweight { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? qcdate { get; set; }

        [StringLength(10, ErrorMessage = "Max 10)")]
        public string returnlocation { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? noticedate { get; set; }

        [StringLength(100, ErrorMessage = "Max 100)")]
        public string bulklabelnote { get; set; }

        [StringLength(20, ErrorMessage = "Max 20)")]
        public string receivedascode { get; set; }

        [StringLength(25, ErrorMessage = "Max 25)")]
        public string receivedasname { get; set; }

        [StringLength(500, ErrorMessage = "Max 500)")]
        public string containernotes { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? wasteaccumstartdate { get; set; }

        public bool? markedforreturn { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class InventoryLogNote
    {
        public int ProductNoteId { get; set; }
        public int? ProductMasterId { get; set; }
        public DateTime? NoteDate { get; set; }
        public string Notes { get; set; }
        public string ReasonCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class InventoryLog
    {
        [StringLength(6)]
        public string LogType { get; set; }

        public int? BulkId { get; set; }
        public int? StockId { get; set; }
        public int? ProductMasterId { get; set; }
        public int? ProductDetailId { get; set; }
        public int? LogQty { get; set; }
        public decimal? LogAmount { get; set; }
        public string UM { get; set; }
        public string LogNotes { get; set; }

        [Required]
        public int? ClientId { get; set; }

        public string ClientName { get; set; }
        public string Status { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string MasterCode { get; set; }
        public string MasterName { get; set; }
        public string Warehouse { get; set; }
        public string Size { get; set; }
        public string BulkBin { get; set; }
        public string ShelfBin { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ShipDate { get; set; }
        public int? OrderNumber { get; set; }
        public int? CurrentQtyAvailable { get; set; }
        public decimal? CurrentWeightAvailable { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CeaseShipDate { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? QCDate { get; set; }
        public int? PackOutId { get; set; }
    }
}