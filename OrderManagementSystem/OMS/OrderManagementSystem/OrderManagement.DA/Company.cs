//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderManagement.DA
{
    using System;
    using System.Collections.Generic;
    
    public partial class Company
    {
        public Company()
        {
            this.Contacts = new HashSet<Contact>();
            this.Properties = new HashSet<Property>();
        }
    
        public int Row_Id { get; set; }
        public Nullable<int> Org_Id { get; set; }
        public string CompanyCode { get; set; }
        public string XeroName { get; set; }
        public string ScrappedName { get; set; }
        public Nullable<bool> CreateEvent { get; set; }
        public Nullable<bool> CreateInvoice { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
    
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
    }
}
