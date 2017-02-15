using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OpenBulkOrderItems
    {
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
        public DateTime? receivedate { get; set; }

        [Display(Name = "Carrier")]
        [StringLength(12, ErrorMessage = "[Max 12]")]
        public string carrier { get; set; }

        public List<SelectListItem> ListOfCarriers { get; set; }

        [Display(Name = "Received By")]
        public string receivedby { get; set; }

        [Display(Name = "Entered By")]
        public string enteredby { get; set; }

        [Display(Name = "Master Code")]
        public int? productmasterid { get; set; }

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

        //shelf life (moved to tblProductMaster

        [Display(Name = "Cease Ship Date")]
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