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
    
    public partial class tblCustomer
    {
        public int CustomerID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public string ShipToCompany { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public Nullable<bool> Restricted { get; set; }
        public string RestrictNote1 { get; set; }
        public string RestrictNote2 { get; set; }
        public string CustAcct { get; set; }
        public string Acode { get; set; }
        public Nullable<int> LegacyID { get; set; }
        public string Company_MDB { get; set; }
        public string Location_MDB { get; set; }
    }
}