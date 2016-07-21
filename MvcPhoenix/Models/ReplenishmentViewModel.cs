using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcPhoenix.Models
{
    public class ReplenishmentViewModel
    {
    }

    public class BulkOrder
    {
        public string ResultsMessage { get; set; }
        public int itemcount { get; set; }
        public int opencount { get; set; }

        [Display(Name = "Bulk Order ID")]
        public int bulkorderid { get; set; }

        public int? clientid { get; set; }
        public string clientname { get; set; }
        public string logofilename { get; set; }

        [Display(Name = "Order Date")]
        public DateTime? orderdate { get; set; }

        [Display(Name = "Order Comments")]
        public string ordercomment { get; set; }

        [Display(Name = "SupplyID")]
        public string supplyid { get; set; }

        public List<SelectListItem> ListOfSupplyIDs { get; set; }

        [Display(Name = "Supplier Email")]
        public string bulksupplieremail { get; set; }

        [Display(Name = "Last Email Sent Date")]
        public string emailsent { get; set; }

        public List<BulkOrderItem> ListOfBulkOrderItem { get; set; }
    }

    public class BulkOrderItem
    {
        [Display(Name = "Item ID")]
        public int bulkorderitemid { get; set; }

        public int? bulkorderid { get; set; }

        [Display(Name = "Mastercode")]
        public int? productmasterid { get; set; }

        public string mastercode { get; set; }
        public string mastername { get; set; }
        
        public List<SelectListItem> ListOfProductMasters { get; set; }

        [Display(Name = "Weight")]
        public decimal? weight { get; set; }

        [Display(Name = "Status")]
        public string itemstatus { get; set; }

        public List<SelectListItem> ListOfItemStatusIDs { get; set; }

        [Display(Name = "ETA")]
        public DateTime? eta { get; set; }

        [Display(Name = "Date Received")]
        public DateTime? datereceived { get; set; }

        [Display(Name = "Item Notes")]
        public string itemnotes { get; set; }
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

    // pc 07/15/16 retired
    public class BulkContainerSearchResults
    {
        //public string searchname { get; set; }
        public int bulkid { get; set; }
        public string warehouse { get; set; }
        public DateTime? receivedate { get; set; }
        public DateTime? expirationdate { get; set; }
        public string carrier { get; set; }
        public int? productmasterid { get; set; }
        public string mastercode { get; set; }
        public string mastername { get; set; }
        public string lotnumber { get; set; }
        public string bulkstatus { get; set; }
        public string um { get; set; }
        public string bin { get; set; }
        public string containertype { get; set; }
        public string clientname { get; set; }
        public decimal? currentweight { get; set; }
    }
    
    public class SuggestedBulkOrderItem
    {
        // corresponds to a record in tblSuggestedBulk
        public int id { get; set; }

        public int? clientid { get; set; }
        
        public string clientname { get; set; }
        public string logofilename { get; set; }

        [Display(Name = "Mastercode")]
        public int? productmasterid { get; set; }

        //public int? masterdivisionid { get; set; }
        public int? divisionid { get; set; }

        public List<SelectListItem> ListOfProductMasters { get; set; }

        public string supplyid { get; set; }
        public List<SelectListItem> ListOfSupplyIDs { get; set; }

        [Display(Name = "Reorder Weight")]
        public int? reorderweight { get; set; }

        [Display(Name = "Item Notes")]
        public string reordernotes { get; set; }

        public string mysessionid { get; set; }
        public string username { get; set; }

        // add fields to reire above class
        public string mastercode { get; set; }

        //public int? masterdivisionid { get; set; }
        public string division { get; set; }

        public string mastername { get; set; }
        public Decimal? bulkshippedperday { get; set; }
        public Decimal? shelfshippedperday { get; set; }
        public int? usethisdaystilexpiration { get; set; }
        public Decimal? averageleadtime { get; set; }
    }
}