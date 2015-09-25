using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class OrderViewModel
    {

        public int Row_Id { get; set; }
        public Nullable<int> OrderType_Id { get; set; }
        public Nullable<int> Property_Id { get; set; }
        public Nullable<int> Mail_Id { get; set; }
        public string OrderId { get; set; }
        public Nullable<System.DateTime> RequiredDate { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string Status { get; set; }
        public string Value { get; set; }

        //public int RowId { get; set; }
        //public int? OrderTypeId { get; set; }
        //public int? Property_Id { get; set; }
        //public string OrderId { get; set; }
        //public DateTime? RequiredDate { get; set; }
        //public string Description { get; set; }
        //public DateTime? Created { get; set; }
        //public string Status { get; set; }
      

        //extra prop
        public string PropertyName { get; set; }
        public string CompanyName { get; set; }
        public int Total { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public Contact OrderContact { get; set; }

        //public Nullable<int> Order_Id { get; set; }
        //public Nullable<int> Xero_Id { get; set; }
        //public string Name { get; set; }
        //public string ClientPrice { get; set; }
        //public string CostPrice { get; set; }
        //public string Comments { get; set; }
    


    }

   
}