using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcPhoenix.Models
{
    public class GHSViewModel
    {
        public int GHSID { get; set; }

        // PD fields
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        // detail fields
        public int PHDetailID { get; set; }
        public int ProductDetailID { get; set; }
        public string PHNumber { get; set; }

        // source fields
        public int PHSourceID { get; set; }
        public string Language { get; set; }
        public string PHStatement { get; set; }

        public string GHSSignalWord { get; set; }
        public enum GHSSignalWordChoice 
        {
            None,
            Caution,
            Warning,
            Danger
        }

        public string GHSSymbol1 { get; set; }
        public string GHSSymbol2 { get; set; }
        public string GHSSymbol3 { get; set; }
        public string GHSSymbol4 { get; set; }
        public string GHSSymbol5 { get; set; }
        public enum GHSSymbolChoice
        {
            None,
            Explosive,
            Flammable,
            Oxidizing,
            Compressed_Gas,
            Corrosive,
            Toxic,
            Irritant,
            Health_Hazard,
            Environmental_Hazard
        }

        [StringLength(250, ErrorMessage = "Other Label Information cannot be longer than 250 characters.")]
        public string OtherLabelInfo { get; set; }
    }

    public class GHSDetail
    {
        public int PHDetailID { get; set; }
        public int ProductDetailID { get; set; }
        public bool? ExcludeFromLabel { get; set; }
        public string PHNumber { get; set; }
        public string Language { get; set; }
        public string PHStatement { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class GHSPHSource
    {
        public int PHSourceID { get; set; }
        public string PHNumber { get; set; }
        public string Language { get; set; }
        public string PHStatement { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}