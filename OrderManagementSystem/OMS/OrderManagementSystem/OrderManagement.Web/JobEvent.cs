//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderManagement.Web
{
    using System;
    using System.Collections.Generic;
    
    public partial class JobEvent
    {
        public int Row_Id { get; set; }
        public Nullable<int> Org_Id { get; set; }
        public Nullable<int> Job_Id { get; set; }
        public Nullable<int> CalendarId { get; set; }
        public string EventId { get; set; }
        public Nullable<int> ProductGroupId { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
