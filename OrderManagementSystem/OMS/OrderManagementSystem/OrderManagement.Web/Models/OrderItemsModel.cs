using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class OrderItemsModel
    {
        public int Row_Id { get; set; }
        public int Order_Id { get; set; }
        public int Xero_Id { get; set; }
        public string Name { get; set; }
        public string ClientPrice { get; set; }
        public string CostPrice { get; set; }
        public string Comments { get; set; }
        public DateTime Created { get; set; }
        public Order Order { get; set; }
        public Property Property { get; set; }

    }


    public class StatusModel
    {
        public bool OrderStatus { get; set; }
    }

    public class OrderableProducts
    {
        public int Row_Id { get; set; }
        public string PrimaryProductGroup { get; set; }
        public string CompanyName { get; set; }
        public string WebName { get; set; }
        public string WebDescription { get; set; }
        public string WebType { get; set; }
        public string WebOptions { get; set; }
    }
}