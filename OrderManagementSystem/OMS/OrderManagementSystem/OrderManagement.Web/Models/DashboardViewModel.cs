using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class DashboardViewModel
    {
        public int DaysInMonth { get; set; }
        public string[] Days { get; set; }
        public object[] OrderQuantity { get; set; }
    }
}