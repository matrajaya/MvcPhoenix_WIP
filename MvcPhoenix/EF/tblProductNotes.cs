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
    
    public partial class tblProductNotes
    {
        public int ProductNoteID { get; set; }
        public Nullable<int> ProductDetailID { get; set; }
        public Nullable<System.DateTime> NoteDate { get; set; }
        public string Notes { get; set; }
        public string ReasonCode { get; set; }
        public string Company_MDB { get; set; }
        public string Division_MDB { get; set; }
        public string ProdCode_MDB { get; set; }
    }
}