using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class PublishedFiles
    {
        public string FileName { get; set; }
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsPublished { get; set; }
    }
}