using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{

    public class ProperyModel
    {
        public  string PropertyName { get; set; }
        public string PropertyId { get; set; }
       // public Property Property { get; set; }
        public    List<Order> Order { get; set; } 
    }


}