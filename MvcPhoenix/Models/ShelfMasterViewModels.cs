using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ShelfMasterViewModel
    {
        [Display(Name = "ShelfID")]
        public int shelfid { get; set; }

        [Display(Name = "ProductDetailID")]
        public int? productdetailid { get; set; }

        public int clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }
        public string productcode { get; set; }
        public string productname { get; set; }

        [Required]
        [Display(Name = "Warehouse")]
        public string warehouse { get; set; }

        public List<SelectListItem> ListOfWareHouses { get; set; }

        [Required]
        [Display(Name = "Size")]
        public string size { get; set; }

        //-------------------------------------------------Iffy added 06/10/2016
        [Display(Name = "Package Material")]
        public string pkgmaterial { get; set; }

        public List<SelectListItem> ListOfTierSizes { get; set; }

        [Display(Name = "Unit Weight")]
        public decimal? unitweight { get; set; }

        [Display(Name = "Reorder Min")]
        public decimal? reordermin { get; set; }

        [Display(Name = "Reorder Max")]
        public decimal? reordermax { get; set; }

        [Display(Name = "Reorder Qty")]
        public decimal? reorderqty { get; set; }

        [Required]
        [Display(Name = "Bin")]
        public string bin { get; set; }

        [Display(Name = "Hazard Surcharge")]
        public bool? hazardsurcharge { get; set; }

        [Display(Name = "Flammable Surcharge")]
        public bool? flammablesurcharge { get; set; }

        [Display(Name = "Oven Surcharge")]
        public bool? heatsurcharge { get; set; }

        [Display(Name = "Refrig Surcharge")]
        public bool? refrigsurcharge { get; set; }

        [Display(Name = "Freezer Surcharge")]
        public bool? freezersurcharge { get; set; }

        [Display(Name = "Clean Surcharge")]
        public bool? cleansurcharge { get; set; }

        [Display(Name = "Blend Surcharge")]
        public bool? blendsurcharge { get; set; }

        [Display(Name = "Nalgene Surcharge")]
        public bool? nalgenesurcharge { get; set; }

        [Display(Name = "Nitrogen Surcharge")]
        public bool? nitrogensurcharge { get; set; }

        [Display(Name = "Biocide Surcharge")]
        public bool? biocidesurcharge { get; set; }

        [Display(Name = "Kosher Surcharge")]
        public bool? koshersurcharge { get; set; }

        [Display(Name = "Label Surcharge")]
        public bool? labelsurcharge { get; set; }

        [Display(Name = "Other Surcharge")]
        public bool? othersurcharge { get; set; }

        [Display(Name = "Other SurchargeAmt")]
        public decimal? othersurchargeamt { get; set; }

        [Display(Name = "New Item")]
        public string newitem { get; set; }

        [Display(Name = "Inactive Size")]
        public bool? inactivesize { get; set; }

        [Display(Name = "WebOE Include")]
        public bool? weboeinclude { get; set; }

        [Display(Name = "Sort Order")]
        public int? sortorder { get; set; }

        [Display(Name = "PackageID")]
        public int? packageid { get; set; }

        public List<SelectListItem> ListOfPackages { get; set; }

        public string packagepartnumber { get; set; }

        //[Display(Name = "Bus Area")]
        // public string busarea { get; set; }

        //[Display(Name = "Mnemonic")]
        //public string mnemonic { get; set; }

        //[Display(Name = "Ground Hazard")]
        //public string groundhazard { get; set; }

        //[Display(Name = "Air Hazard")]
        //public string airhazard { get; set; }

        [Display(Name = "Notes")]
        public string notes { get; set; }

        [Display(Name = "Discontinued")]
        public bool? discontinued { get; set; }

        [Display(Name = "Alert")]
        public string alert { get; set; }

        [Display(Name = "CustCode")]
        public string custcode { get; set; }

        //[Display(Name = "MigrationNotes")]
        //public string migrationnotes { get; set; }

        //public string datastate { get; set; }
        //public bool? IsValidItem { get; set; }
    }
}