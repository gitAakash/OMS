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
    
    public partial class Eventlog
    {
        public int Row_Id { get; set; }
        public string Event_Id { get; set; }
        public string Event_Title { get; set; }
        public string Operation { get; set; }
        public Nullable<int> User_Id { get; set; }
        public string Source_calendar { get; set; }
        public string Destination_calendar { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
    }
}
