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
    
    public partial class tblInvoice
    {
        public int InvoiceID { get; set; }
        public Nullable<int> InvoiceNumber { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<System.DateTime> ChargeDate { get; set; }
        public string ChargeMonth { get; set; }
        public Nullable<int> ChargeYear { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}