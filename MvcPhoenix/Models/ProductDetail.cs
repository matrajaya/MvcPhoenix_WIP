using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// pc add
//using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MvcPhoenix.Models
{

    // need to add per CD 10/12/2015
    //SpecificGravity (used to calc Density) Density = SG * 8.34
    // 
    //AccuracyVerified



    // Maps to tblProductDetail
    public class ProductDetail
    {

        public ProductMaster PM { get; set; }

        [Display(Name = "ProductDetailID")]
        public int productdetailid { get; set; }

        [Display(Name = "ProductMasterID")]
        public int? productmasterid { get; set; }

        [Display(Name = "SGLegacyID")]
        public int? sglegacyid { get; set; }

        [Display(Name = "DivisionID")]
        public int? divisionid { get; set; }

        [Display(Name = "BusArea")]
        public string busarea { get; set; }

        [Display(Name = "ProductCode")]
        public string productcode { get; set; }

        [Display(Name = "ProductName")]
        public string productname { get; set; }

        [Display(Name = "CustCode")]
        public string custcode { get; set; }

        [Display(Name = "MultiLotReq")]
        public bool? multilotreq { get; set; }

        [Display(Name = "ExtendableExpDt")]
        public bool? extendableexpdt { get; set; }

        [Display(Name = "HarmonizedCode")]
        public string harmonizedcode { get; set; }

        [Display(Name = "EndUse")]
        public string enduse { get; set; }

        [Display(Name = "SGRevisionDate")]
        public DateTime? sgrevisiondate { get; set; }

        [Display(Name = "MSDSRevisionDate")]
        public DateTime? msdsrevisiondate { get; set; }

        [Display(Name = "MSDSRevisionNumber")]
        public string msdsrevisionnumber { get; set; }

        [Display(Name = "LabelRevisionDate")]
        public DateTime? labelrevisiondate { get; set; }

        [Display(Name = "LabelNumber")]
        public string labelnumber { get; set; }

        [Display(Name = "Checked")]
        // table field is checked, should be changed
        public bool? invchecked { get; set; }

        [Display(Name = "CheckedBy")]
        public string checkedby { get; set; }

        [Display(Name = "CheckedWhen")]
        public DateTime? checkedwhen { get; set; }

        [Display(Name = "EPABiocide")]
        public bool? epabiocide { get; set; }

        [Display(Name = "ShippingNotes")]
        public string shippingnotes { get; set; }

        [Display(Name = "OtherLabelNotes")]
        public string otherlabelnotes { get; set; }

        [Display(Name = "ProductDescription")]
        public string productdescription { get; set; }

        [Display(Name = "LabelInfo")]
        public string labelinfo { get; set; }

        [Display(Name = "GHSReady")]
        public bool? ghsready { get; set; }

        [Display(Name = "CustomsValue")]
        public decimal customsvalue { get; set; }

        [Display(Name = "CustomsValueUnit")]
        public string customsvalueunit { get; set; }

        [Display(Name = "GlobalProduct")]
        public bool? globalproduct { get; set; }

        [Display(Name = "Company_MDB")]
        public string company_mdb { get; set; }

        [Display(Name = "MasterCode_MDB")]
        public string mastercode_mdb { get; set; }

        [Display(Name = "Division_MDB")]
        public string division_mdb { get; set; }

    }
}
