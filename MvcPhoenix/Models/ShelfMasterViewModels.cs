using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ShelfMasterViewModel
    {
        public string clientum { get; set; }
        public int shelfid { get; set; }
        public int? productdetailid { get; set; }
        public int clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }
        public string productcode { get; set; }
        public string productname { get; set; }

        [Required]
        public string warehouse { get; set; }        
        [Required]
        public string size { get; set; }
        public string pkgmaterial { get; set; }
        public decimal? unitweight { get; set; }
        public decimal? reordermin { get; set; }
        public decimal? reordermax { get; set; }
        public decimal? reorderqty { get; set; }

        [Required]
        public string bin { get; set; }
        public bool? hazardsurcharge { get; set; }
        public bool? flammablesurcharge { get; set; }
        public bool? heatsurcharge { get; set; }
        public bool? refrigsurcharge { get; set; }
        public bool? freezersurcharge { get; set; }
        public bool? cleansurcharge { get; set; }
        public bool? blendsurcharge { get; set; }
        public bool? nalgenesurcharge { get; set; }
        public bool? nitrogensurcharge { get; set; }
        public bool? biocidesurcharge { get; set; }
        public bool? koshersurcharge { get; set; }
        public bool? labelsurcharge { get; set; }
        public bool? othersurcharge { get; set; }
        public decimal? othersurchargeamt { get; set; }
        public string othersurchargedescription { get; set; }
        public string newitem { get; set; }
        public bool? inactivesize { get; set; }
        public bool? weboeinclude { get; set; }
        public int? sortorder { get; set; }
        public int? packageid { get; set; }
        public string packagepartnumber { get; set; }
        public string notes { get; set; }
        public bool? discontinued { get; set; }
        public string alert { get; set; }
        public string custcode { get; set; }
    }
}