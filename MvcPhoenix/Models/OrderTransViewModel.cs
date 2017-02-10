using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OrderTrans
    {
        public int ordertransid { get; set; }

        [Display(Name = "Mode")]
        public string pagemode { get; set; }

        [Display(Name = "ClientID")]
        public int? clientid { get; set; }

        [Display(Name = "OrderID")]
        public int? orderid { get; set; }

        [Display(Name = "Order Item")]
        public int? orderitemid { get; set; }

        public string productcode { get; set; }

        [Required(ErrorMessage = "Select Transaction Type")]
        [Display(Name = "Type")]
        public string transtype { get; set; }
        
        public enum OrderTransTypesChoice
        {
            MEMO,
            MISC_G,
            MISC_S,
            MISC_C,
            MISC_CF,
            MISC_D,
            MISC_EM,
            MISC_FSC,
            MISC_FSD,
            MISC_FSM,
            MISC_H,
            MISC_I,
            MISC_IN,
            MISC_M,
            MISC_P,
            MISC_PM,
            MISC_R,
            MISC_SPS,
            MISC_TDI,
            MISC_U,            
            OTHER
        }

        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime? transdate { get; set; }

        [Display(Name = "Qty")]
        public int? transqty { get; set; }

        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal? transamount { get; set; }

        [Display(Name = "Charge Date")]
        [DataType(DataType.DateTime)]
        public DateTime? chargedate { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        public DateTime? createdate { get; set; }
        public string createuser { get; set; }
        public DateTime? updatedate { get; set; }
        public string updateuser { get; set; }
    }
}