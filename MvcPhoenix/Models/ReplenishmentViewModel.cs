using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class BulkOrder
    {
        public string ResultsMessage { get; set; }
        public int itemcount { get; set; }
        public int opencount { get; set; }
        public int bulkorderid { get; set; }
        public int? clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }
        public DateTime? orderdate { get; set; }
        public string ordercomment { get; set; }
        public string supplyid { get; set; }
        public string bulksupplieremail { get; set; }
        public string emailsent { get; set; }
        public List<BulkOrderItem> ListOfBulkOrderItem { get; set; }
    }

    public class BulkOrderItem
    {
        public int bulkorderitemid { get; set; }
        public int? bulkorderid { get; set; }
        public int? productmasterid { get; set; }
        public string mastercode { get; set; }
        public string mastername { get; set; }
        public List<SelectListItem> ListOfProductMasters { get; set; }
        public decimal? weight { get; set; }
        public string itemstatus { get; set; }
        public DateTime? eta { get; set; }
        public DateTime? datereceived { get; set; }
        public string itemnotes { get; set; }
        public bool? PrepackedBulk { get; set; }
    }

    public class BulkOrderEmailViewModel
    {
        public int bulkorderid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string ToAddressCC { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
    }
    
    public class SuggestedBulkOrderItem
    {
        public int id { get; set; }
        public int? clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }
        public int? productmasterid { get; set; }
        public int? divisionid { get; set; }
        public List<SelectListItem> ListOfProductMasters { get; set; }
        public string supplyid { get; set; }
        public decimal? reorderweight { get; set; }
        public string reordernotes { get; set; }
        public string mysessionid { get; set; }
        public string username { get; set; }
        public string mastercode { get; set; }
        public string division { get; set; }
        public string mastername { get; set; }
        public Decimal? bulkshippedperday { get; set; }
        public Decimal? shelfshippedperday { get; set; }
        public int? usethisdaystilexpiration { get; set; }
        public Decimal? averageleadtime { get; set; }
    }
}