using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OrderTrans
    {
        public int ordertransid { get; set; }
        public string pagemode { get; set; }
        public int? clientid { get; set; }
        public int? orderid { get; set; }
        public int? orderitemid { get; set; }
        public string productcode { get; set; }
        [Required(ErrorMessage = "Select Transaction Type")]
        public string transtype { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? transdate { get; set; }
        public int? transqty { get; set; }
        [DataType(DataType.Currency)]
        public decimal? transrate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? chargedate { get; set; }
        public string comments { get; set; }
        public DateTime? createdate { get; set; }
        public string createuser { get; set; }
        public DateTime? updatedate { get; set; }
        public string updateuser { get; set; }
    }
}