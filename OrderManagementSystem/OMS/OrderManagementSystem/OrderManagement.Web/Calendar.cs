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
    
    public partial class Calendar
    {
        public int Row_Id { get; set; }
        public Nullable<int> Org_Id { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<bool> StagingCalendar { get; set; }
        public string TimeZone { get; set; }
        public string Kind { get; set; }
        public string eTag { get; set; }
        public Nullable<int> ColorId { get; set; }
        public string Background { get; set; }
        public string Foreground { get; set; }
        public string AccessRole { get; set; }
        public Nullable<bool> IsSelected { get; set; }
    }
}