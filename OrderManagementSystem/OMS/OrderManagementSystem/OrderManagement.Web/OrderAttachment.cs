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
    
    public partial class OrderAttachment
    {
        public int Row_Id { get; set; }
        public int Org_Id { get; set; }
        public int Order_Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public Nullable<int> FileSize { get; set; }
        public byte[] File { get; set; }
        public string GroupType { get; set; }
        public string Folder { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
