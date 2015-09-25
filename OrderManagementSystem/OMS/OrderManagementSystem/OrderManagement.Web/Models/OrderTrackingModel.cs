using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class OrderTrackingModel
    {

        public int Row_Id { get; set; }
        public int OrderType_Id { get; set; }
        public int Property_Id { get; set; }
        public int Mail_Id { get; set; }
        public string OrderId { get; set; }
        public DateTime RequiredDate { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; }
        public string Value { get; set; }

        public string PropertyName { get; set; }
        public string CompanyName { get; set; }
        public int Total { get; set; }
    }
}