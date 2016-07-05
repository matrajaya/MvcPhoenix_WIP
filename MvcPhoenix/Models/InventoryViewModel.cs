using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class Inventory
    {
    }

    public class BulkContainerViewModel
    {
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
        //[Required(ErrorMessage = "[Receive Date Required]")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? receivedate { get; set; }

        [Display(Name = "Carrier")]
        //[Required(ErrorMessage = "[Carrier Required]")]
        [StringLength(12, ErrorMessage = "[Max 12]")]
        public string carrier { get; set; }

        public List<SelectListItem> ListOfCarriers { get; set; }

        [Display(Name = "Received By")]
        //[Required(ErrorMessage = "[Received By Required]")]
        [StringLength(10, ErrorMessage = "[Max 10]")]
        public string receivedby { get; set; }

        [Display(Name = "Entered By")]
        //[Required(ErrorMessage = "[Entered By Required]")]
        [StringLength(10, ErrorMessage = "[Max 10]")]
        public string enteredby { get; set; }

        [Display(Name = "Master Code")]
        public int? productmasterid { get; set; }

        public string MasterName { get; set; }
        public string MasterCode { get; set; }

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
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? expirationdate { get; set; }

        //shelf life (moved to tblProductMaster

        [Display(Name = "Cease Ship Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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

		[Display(Name = "Other Storage")]
        public string otherstorage { get; set; }

        // Some readonly properties from PM
        public bool? flammable { get; set; }

        public bool? freezer { get; set; }
        public bool? refrigerated { get; set; }

        [Display(Name = "PackOut on Recpt")]
        public bool? packout { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}