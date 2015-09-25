using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class GetJobAttachmentAndComment
    {
        public IList<SelectJobAttachmentTemplate_Result> JobAttachmentAndCommment { get; set; }
        public int CommentCount { get; set; }
        public string Company_Name { get; set; }
        public string Name { get; set; }
        public string FName2 { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Address { get; set; }
        public string package { get; set; }
        public string ContactNumber { get; set; }
        public string DAY_PHOTOGRAPHER { get; set; }
        public string DUSK_PHOTOGRAPHER { get; set; }
    }
}