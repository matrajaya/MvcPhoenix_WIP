using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ReplenishmentViewModel
    {
    }

    public class BulkContainer
    {
        public bool? isknownmaterial { get; set; }
        public string closelist { get; set; }

        public int? clientid { get; set; }

        public string clientname { get; set; }
        public List<SelectListItem> ListOfClients { get; set; }

        // ******* basic info
        [Display(Name = "BulkID")]
        public int bulkid { get; set; }

        [Display(Name = "Warehouse")]
        //[Required(ErrorMessage="Required")]
        [StringLength(2, ErrorMessage = "Max 2)")]
        public string warehouse { get; set; }

        public List<SelectListItem> ListOfWareHouses { get; set; }

        [Display(Name = "Receive Date")]
        //[Required(ErrorMessage = "[Receive Date Required]")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? receivedate { get; set; }

        [Display(Name = "Carrier")]
        //[Required(ErrorMessage = "[Carrier Required]")]
        [StringLength(12, ErrorMessage = "[Max 12]")]
        public string carrier { get; set; }

        [Display(Name = "Received By")]
        //[Required(ErrorMessage = "[Received By Required]")]
        [StringLength(10, ErrorMessage = "[Max 10]")]
        public string receivedby { get; set; }

        [Display(Name = "Entered By")]
        //[Required(ErrorMessage = "[Entered By Required]")]
        [StringLength(10, ErrorMessage = "[Max 10]")]
        public string enteredby { get; set; }

        // ******* product info
        [Display(Name = "Master Code")]
        //[Required]
        public int? productmasterid { get; set; }

        public List<SelectListItem> ListOfProductMasters { get; set; }

        [Display(Name = "Receive Weight")]
        //[Required(ErrorMessage = "Required")]
        public decimal? receiveweight { get; set; }

        // ******* batch and lot info
        [Display(Name = "Lot Number")]
        //[Required(ErrorMessage = "Required")]
        [StringLength(25, ErrorMessage = "Max 25)")]
        public string lotnumber { get; set; }

        [Display(Name = "Mfg Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? mfgdate { get; set; }

        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? expirationdate { get; set; }

        //shelf life (moved to tblProductMaster

        [Display(Name = "Cease Ship Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ceaseshipdate { get; set; }

        [Display(Name = "Bulk Status")]
        //[StringLength(6, ErrorMessage = "Max 6)")]
        public string bulkstatus { get; set; }

        public List<SelectListItem> ListOfBulkStatusIDs { get; set; }

        // ******* container info
        public string qty { get; set; } // default=1, No user interface, remove from db?

        [Display(Name = "Unit Measure")]  // previous fieldname was Container
        [StringLength(20, ErrorMessage = "Max 20)")]
        public string um { get; set; }

        [Display(Name = "Container Color")]
        [StringLength(15, ErrorMessage = "Max 15)")]
        public string containercolor { get; set; }

        [Display(Name = "Bin")]
        //[Required(ErrorMessage = "Required")]
        [StringLength(25, ErrorMessage = "Max 25)")]
        public string bin { get; set; }

        // Steel, Plastic, Fiber, Other
        [Display(Name = "Container Type")]
        [StringLength(1, ErrorMessage = "Max 1)")]
        public string containertype { get; set; }

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

        [Display(Name = "PackOut on Recpt")]
        public bool? packout { get; set; }

        //public List<OpenBulkOrderItems> ListOfOpenBulkOrderItems { get; set; }
    }

    public class OpenBulkOrderItems
    {
        // simple class for use in receive bulk partial view open items
        public int bulkorderitemid { get; set; }
        public int? bulkorderid { get; set; }
        public int? productmasterid { get; set; }
        public decimal? weight { get; set; }
        public string status { get; set; }
        public DateTime? eta { get; set; }
        public string supplyid { get; set; }
        public string itemnotes { get; set; }
        public DateTime? orderdate { get; set; }
    }

    public class BulkContainerSearchResults
    {
        // simple class for use in partial grid
        public string searchname { get; set; }
        public int bulkid { get; set; }
        public string warehouse { get; set; }
        public DateTime? receivedate { get; set; }
        public DateTime? expirationdate { get; set; }
        public string carrier { get; set; }
        public int? productmasterid { get; set; }
        public string mastercode { get; set; }
        public string mastername { get; set; }
        public string lotnumber { get; set; }
        public string bulkstatus { get; set; }
        public string um { get; set; }
        public string bin { get; set; }
        public string containertype { get; set; }
        public string clientname { get; set; }
        public decimal? currentweight { get; set; }

    }

    public class SuggestedBulkOrder
    {
        // DTO obj used to pass the ClientID and DivisionID to the CreateSuggested Order action
        // Not really necessary, could just post
        public int? clientid { get; set; }
        public int? divisionid { get; set; }
        public List<SelectListItem> ListOfClients { get; set; }
        //public List<SelectListItem> ListOfDivisions { get; set; }

        // 10/06
        //public List<SuggestedBulkOrderItems> ListOfOrderItems { get; set; }
    }

    public class SuggestedBulkOrderItems
    {
        // 10/06/2015 for partial list view, included in object above
        public int id { get; set; }
        public int? productmasterid { get; set; }
        public string mastercode { get; set; }
        public int? masterdivisionid { get; set; }
        public string division { get; set; }
        public string mastername { get; set; }
        public string supplyid { get; set; }
        public Decimal? bulkshippedperday { get; set; }
        public Decimal? shelfshippedperday { get; set; }
        public int? usethisdaystilexpiration { get; set; }
        public Decimal? averageleadtime { get; set; }
        public Decimal? reorderweight { get; set; }
        public string reordernotes { get; set; }
    }

    public class SuggestedBulkOrderItem
    {
        // corresponds to a record in tblSuggestedBulk
        public int id { get; set; }
        public int? clientid { get; set; }

        [Display(Name = "Mastercode")]
        public int productmasterid { get; set; }

        public List<SelectListItem> ListOfProductMasters { get; set; }
        public string supplyid { get; set; }

        [Display(Name = "Reorder Weight")]
        public int? reorderweight { get; set; }

        [Display(Name = "Item Notes")]
        public string notes { get; set; }

        public string mysessionid { get; set; }

        // later public string updateresult { get; set; }
    }

    public class BulkOrderSearchResults
    {
        // Filled in the controller using LINQ, pushed to partial viewmodel
        public int bulkorderid { get; set; }
        public int? clientid { get; set; }
        public string supplyid { get; set; }
        public DateTime? orderdate { get; set; }
        //public string status { get; set; }
        public string comment { get; set; }
        public string clientname { get; set; }
        public string bulksupplieremail { get; set; }
        public string emailsent { get; set; }   // leave as string, push date as string intoit
        public int itemcount { get; set; }
    }

    public class BulkOrderHeader
    {
        // ************  Add tblBulkOrder fields
        [Display(Name = "Bulk Order ID")]
        public int bulkorderid { get; set; }

        public int? clientid { get; set; }
        public string clientname { get; set; }

        [Display(Name = "Order Date")]
        public DateTime? orderdate { get; set; }

        [Display(Name = "Order Comments")]
        public string ordercomment { get; set; }

        [Display(Name = "SupplyID")]
        public string supplyid { get; set; }
        public List<SelectListItem> ListOfOrderHeaderSupplyIDs { get; set; }

        //[Required(ErrorMessage="Email is required")]
        [Display(Name = "Supplier Email")]
        //[DataType(DataType.EmailAddress)]
        [StringLength(150, ErrorMessage = "Max 150 chars for email")]
        public string bulksupplieremail { get; set; }

        [Display(Name = "Last Email Sent Date")]
        public string emailsent { get; set; }

        // 10/05 add list
        public List<BulkOrderItemsForDisplay> ListOfOrderItemsForDisplay { get; set; }
    }

    public class BulkOrderItemsForDisplay
    {
        // For grid in _BulkOrderItems.cshtml
        public int bulkorderitemid { get; set; }
        public int? bulkorderid { get; set; }
        public int? productmasterid { get; set; }
        public decimal? weight { get; set; }
        public DateTime? eta { get; set; }
        public string itemnotes { get; set; }
        public string itemstatus { get; set; }
        public DateTime? datereceived { get; set; }
        // fields used in Join
        public string mastercode { get; set; }
        public string mastername { get; set; }
    }

    public class BulkOrderItem
    {
        // tblBulkOrderItem fields for edit
        [Display(Name = "Item ID")]
        public int bulkorderitemid { get; set; }

        public int? bulkorderid { get; set; }

        [Display(Name = "Mastercode")]
        public int? productmasterid { get; set; }

        public List<SelectListItem> ListOfProductMasters { get; set; }

        [Display(Name = "Weight")]
        public decimal? weight { get; set; }

        [Display(Name = "Status")]
        public string itemstatus { get; set; }
        public List<SelectListItem> ListOfItemStatusIDs { get; set; }

        [Display(Name = "ETA")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString="{0:dd:MM:yyyy}",ApplyFormatInEditMode=true)]
        public DateTime? eta { get; set; }

        [Display(Name = "Date Received")]
        public DateTime? datereceived { get; set; }

        [Display(Name = "Item Notes")]
        public string itemnotes { get; set; }

        // later public string updateresult { get; set; }
    }
}