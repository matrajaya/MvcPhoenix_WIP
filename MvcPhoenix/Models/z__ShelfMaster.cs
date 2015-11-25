﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcPhoenix.Models
{
    // Maps to tblShelfMaster

    public class ShelfMaster
    {
        [Display(Name = "ShelfID")]
        public int shelfid { get; set; }

        [Display(Name = "ProductDetailID")]
        public int productdetailid { get; set; }

        [Display(Name = "Warehouse")]
        public string warehouse { get; set; }

        [Display(Name = "Size")]
        public string size { get; set; }

        [Display(Name = "Unit Weight")]
        public decimal? unitweight { get; set; }

        [Display(Name = "Reorder Min")]
        public decimal? reordermin { get; set; }

        [Display(Name = "Reorder Max")]
        public decimal? reordermax { get; set; }

        [Display(Name = "Reorder Qty")]
        public decimal? reorderqty { get; set; }

        [Display(Name = "Bin")]
        public string bin { get; set; }

        [Display(Name = "Hazard Surcharge")]
        public bool? hazardsurcharge { get; set; }

        [Display(Name = "Flammable Surcharge")]
        public bool? flammablesurcharge { get; set; }

        [Display(Name = "Heat Surcharge")]
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

        [Display(Name = "Package")]
        public string package { get; set; }

        [Display(Name = "Package PartNumber")]
        public string packagepartnumber { get; set; }

        [Display(Name = "Package Weight")]
        public decimal? packageweight { get; set; }

        [Display(Name = "DOTSpec")]
        public string dotspec { get; set; }

        [Display(Name = "UNSpec")]
        public string unspec { get; set; }

        [Display(Name = "Bus Area")]
        public string busarea { get; set; }

        [Display(Name = "Mnemonic")]
        public string mnemonic { get; set; }

        [Display(Name = "Ground Hazard")]
        public string groundhazard { get; set; }

        [Display(Name = "Air Hazard")]
        public string airhazard { get; set; }

        [Display(Name = "Notes")]
        public string notes { get; set; }

        [Display(Name = "Discontinued")]
        public bool? discontinued { get; set; }

        [Display(Name = "Alert")]
        public string alert { get; set; }

        [Display(Name = "CustCode")]
        public string custcode { get; set; }
    }
}