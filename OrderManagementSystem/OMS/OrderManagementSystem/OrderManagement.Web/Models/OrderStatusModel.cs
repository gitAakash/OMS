using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class OrderStatusModel
    {
        public int Row_Id { get; set; }
        public int Org_Id { get; set; }
        public string Order_Id { get; set; }
        public int User_Id { get; set; }
        public string Status { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public User User { get; set; }
        public IList<OrderAttachment> OrderAttachment { get; set; }
       
    }


    public class OrderStatusModel1
    {

        public int Row_Id { get; set; }
        public int Org_Id { get; set; }
        public int Order_Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public Nullable<int> FileSize { get; set; }
        public byte[] File { get; set; }
       // public string Image64 { get; set; }
        public string GroupType { get; set; }
        public string Folder { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }

    }

    public class ImageModel
    {
        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }
        public IList<OrderStatusModel1> OrderStatusModel1 { get; set; }
    }
}