using System;
using System.ComponentModel.DataAnnotations;

namespace MvcPhoenix.Models
{
    //SpecificGravity (used to calc Density) Density = SG * 8.34

    public class ProductDetail
    {
        public ProductMaster PM { get; set; }
        public int productdetailid { get; set; }
        public int? productmasterid { get; set; }
        public int? sglegacyid { get; set; }
        public int? divisionid { get; set; }
        public string busarea { get; set; }
        public string productcode { get; set; }
        public string productname { get; set; }
        public string custcode { get; set; }
        public bool? multilotreq { get; set; }
        public bool? extendableexpdt { get; set; }
        public string harmonizedcode { get; set; }
        public string enduse { get; set; }
        public DateTime? sgrevisiondate { get; set; }
        public DateTime? msdsrevisiondate { get; set; }
        public string msdsrevisionnumber { get; set; }
        public DateTime? labelrevisiondate { get; set; }
        public string labelnumber { get; set; }        
        public bool? invchecked { get; set; }                       // table field is checked, should be changed
        public string checkedby { get; set; }
        public DateTime? checkedwhen { get; set; }
        public bool? epabiocide { get; set; }
        public string shippingnotes { get; set; }
        public string otherlabelnotes { get; set; }
        public string productdescription { get; set; }
        public string labelinfo { get; set; }
        public bool? ghsready { get; set; }
        public decimal customsvalue { get; set; }
        public string customsvalueunit { get; set; }
        public bool? globalproduct { get; set; }
    }
}