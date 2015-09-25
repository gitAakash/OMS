using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class CompanyModel
    {
        public int Row_Id { get; set; }
        public int? Org_Id { get; set; }
         [Display(Name = "Company Code")]
        public string CompanyCode { get; set; }
         [Display(Name = "Company Name")]
        public string XeroName { get; set; }

        public string ScrappedName { get; set; }

         [Display(Name = "Create Event")]
        public bool CreateEvent { get; set; }

         [Display(Name = "Create Invoice")]
        public bool CreateInvoice { get; set; }


         [Display(Name = "Active")]
         public bool Active { get; set; }

         [Display(Name = "Web Orders")]
         public bool WebOrders { get; set; }

         [Display(Name = "Created Date")]
        public DateTime? Created { get; set; }
         public List<PropertyOrderModel> ProperyOrderModel { get; set; }
         public List<ProductModel> Product { get; set; }
         public bool Enabled { get; set; }
    }

    // Have to Remove
    //public class ProperyOrderModel
    //{
    //    public string PropertyName { get; set; }
    //    public string PropertyId { get; set; }
    //    public string Value { get; set; }
    //   // public List<Order> Order { get; set; }
    //    public List<OrderModel> OrderModel { get; set; }
    //}


    public class PropertyOrderModel
    {
        public string PropertyName { get; set; }
        public string PropertyId { get; set; }
        public string Value { get; set; }
        public string OrderRowId { get; set; }
        public string RequiredDate { get; set; }
        public string Description { get; set; }
     //   public DateTime Created { get; set; }
        public string Status { get; set; }




    }

    public class ContactModel
    {
        public int Row_Id { get; set; }
        public int CompanyId { get; set; }
        public int Orderid { get; set; }
        [Display(Name = "Contact Name")]
        public string Name { get; set; }
        public string Value { get; set; }
          [Display(Name = "Contact Type")]
        public string ContactType { get; set; }
        public DateTime? Created { get; set; }

    }

    public class OrderModel
    {
        public int RowId { get; set; }
        public int? OrderTypeId { get; set; }
        public int? PropertyId { get; set; }
        public string OrderId { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string Description { get; set; }
        public DateTime? Created { get; set; }
        public string Status { get; set; }
        public string Value { get; set; }

    }
}