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
    
    public partial class JobCopy
    {
        public int Row_Id { get; set; }
        public Nullable<int> Org_Id { get; set; }
        public Nullable<int> Job_Id { get; set; }
        public Nullable<int> Job_Copy_Type_Id { get; set; }
        public string Title { get; set; }
        public Nullable<int> TitleWordCount { get; set; }
        public string Body { get; set; }
        public Nullable<int> BodyWordCount { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
