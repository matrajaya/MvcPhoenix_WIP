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
        public int shelfid { get; set; }
        public string size { get; set; }
        public string bin { get; set; }
    }

    public class PrePackViewModel
    {
        public List<ItemForPrePackViewModel> ListOfShelfMasters { get; set; }
        public int ItemsCount { get; set; }
        public bool? isknownmaterial { get; set; }
        public int? pm_ceaseshipdifferential { get; set; }
        public int? clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }
        public int bulkid { get; set; }
        public string warehouse { get; set; }
        public DateTime? receivedate { get; set; }
        [StringLength(12, ErrorMessage = "[Max 12]")]
        public string carrier { get; set; }
        public string receivedby { get; set; }
        public string enteredby { get; set; }
        public int? productmasterid { get; set; }
        public decimal? receiveweight { get; set; }

        // ******* batch and lot info
        [StringLength(25, ErrorMessage = "Max 25)")]
        public string lotnumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? mfgdate { get; set; }
        public DateTime? expirationdate { get; set; }

        //shelf life (moved to tblProductMaster)

        public DateTime? ceaseshipdate { get; set; }
        public string bulkstatus { get; set; }
        public string qty { get; set; } // default=1, No user interface, remove from db?
        public bool? coaincluded { get; set; }
        public bool? msdsincluded { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? qcdate { get; set; }
        public string productcode { get; set; }
        public string productname { get; set; }
    }
}