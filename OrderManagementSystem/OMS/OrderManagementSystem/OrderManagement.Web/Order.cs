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
    
    public partial class Order
    {
        public Order()
        {
            this.OrderEvents = new HashSet<OrderEvent>();
            this.OrderItems = new HashSet<OrderItem>();
            this.OrderAttachments = new HashSet<OrderAttachment>();
        }
    
        public int Row_Id { get; set; }
        public Nullable<int> OrderType_Id { get; set; }
        public Nullable<int> Property_Id { get; set; }
        public Nullable<int> Mail_Id { get; set; }
        public string OrderId { get; set; }
        public Nullable<System.DateTime> RequiredDate { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string Status { get; set; }
        public string Keys { get; set; }
        public string SpecialInstructions { get; set; }
    
        public virtual ICollection<OrderEvent> OrderEvents { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<OrderAttachment> OrderAttachments { get; set; }
    }
}