using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ReturnBulkViewModel
    {
        public string vmMode { get; set; }
        public bool? markedforreturn { get; set; }
        public int? divisionid { get; set; }
        public string divisionname { get; set; }
        public int? clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }
        public int bulkid { get; set; }
        public string warehouse { get; set; }
        public int? productmasterid { get; set; }
        public string MasterName { get; set; }
        public string MasterCode { get; set; }
        public decimal? receiveweight { get; set; }
        public string lotnumber { get; set; }
        public DateTime? expirationdate { get; set; }
        public DateTime? ceaseshipdate { get; set; }
        public string bulkstatus { get; set; }
        public string qty { get; set; }                     // default=1, No user interface, remove from db?
        public string um { get; set; }
        public string bin { get; set; }
        public decimal? currentweight { get; set; }
        public DateTime? qcdate { get; set; }
    }

    public class ReturnStockViewModel
    {
        public string vmMode { get; set; }
        public bool? markedforreturn { get; set; }
        public int? ClientID { get; set; }
        public int? divisionid { get; set; }
        public string divisionname { get; set; }
        public int? StockID { get; set; }
        public int? BulkID { get; set; }
        public int? ShelfID { get; set; }
        public string Warehouse { get; set; }
        public int? QtyOnHand { get; set; }
        public string Bin { get; set; }
        public string ShelfStatus { get; set; }
        public int? ProductDetailID { get; set; }
        public string Size { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}