//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcPhoenix.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwShelfStockForReplenishment
    {
        public int StockID { get; set; }
        public Nullable<int> QtyOnHand { get; set; }
        public Nullable<decimal> UnitWeight { get; set; }
        public int ProductMasterID { get; set; }
        public Nullable<int> DivisionID { get; set; }
        public string ShelfStatus { get; set; }
    }
}