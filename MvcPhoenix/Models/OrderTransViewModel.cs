using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class OrderTrans
    {
        public int OrderTransID { get; set; }
        public string PageMode { get; set; }
        public int? ClientId { get; set; }
        public int? DivisionId { get; set; }
        public int? OrderId { get; set; }
        public int? OrderItemId { get; set; }
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "Select Transaction Type")]
        public string TransType { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? TransDate { get; set; }
        public int? TransQty { get; set; }
        [DataType(DataType.Currency)]
        public decimal? TransRate { get; set; }
        [DataType(DataType.Currency)]
        public decimal? TransAmount { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? ChargeDate { get; set; }
        public int? BillingTier { get; set; }
        public decimal? BillingRate { get; set; }
        public decimal? BillingCharge { get; set; }
        public string Comments { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}