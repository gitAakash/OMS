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
    
    public partial class Attachment
    {
        public int Row_Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public byte[] Buffer { get; set; }
    
        public virtual User User { get; set; }
    }
}
