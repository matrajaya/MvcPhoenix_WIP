using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// pc add
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace MvcPhoenix.Models
{
    //public class ReceivingViewModels
    //{

    //}
    
    public class OpenBulkOrderItems
    {
        // short class for use in _OpenOrderItems.cshtml
        public int bulkorderitemid { get; set; }
        public int? bulkorderid { get; set; }
        public int? productmasterid { get; set; }
        public decimal? weight { get; set; }
        public string status { get; set; }
        public DateTime? eta { get; set; }
        public string supplyid { get; set; }
        public string itemnotes { get; set; }
        public DateTime? orderdate { get; set; }
        public bool? ToBeClosed { get; set; }
    }


    public class ItemForPrePackViewModel
    {
        [Display(Name = "ShelfID")]
        public int shelfid { get; set; }

        //[Display(Name = "ProductDetailID")]
        //public int productdetailid { get; set; }

        // join to tblProductDetail
        //public string productcode { get; set; }
        //public string productname { get; set; }
               
        [Display(Name = "Size")]
        public string size { get; set; }

        [Display(Name = "Bin")]
        public string bin { get; set; }


    }
    public class PrePackViewModel
    {
        public List<ItemForPrePackViewModel> ListOfShelfMasters { get; set; }
        public int ItemsCount { get; set; }
        public bool? isknownmaterial { get; set; }
       
        public int? clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }

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

        public string bulkstatus { get; set; }

        public string qty { get; set; } // default=1, No user interface, remove from db?


        [Display(Name = "COA Included?")]
        public bool? coaincluded { get; set; }

        [Display(Name = "MSDS Included?")]
        public bool? msdsincluded { get; set; }

        [Display(Name = "QC Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? qcdate { get; set; }


        public string productcode { get; set; }
        public string productname { get; set; }

    }



}