using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class ProductModel
    {
        public int Row_Id { get; set; }
        [Display(Name = "Xero Code")]
        public string XeroCode { get; set; }
        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }
        [Display(Name = "Xero Item Description")]
        public string XeroItemDescription { get; set; }
        [Display(Name = "Sales Unit Price")]
        public decimal? SalesUnitPrice { get; set; }
        public string SalesAccountCode { get; set; }
        public string SalesTaxType { get; set; }
        public int? ProductGroupId { get; set; }
        [Display(Name = "Product Group")]
        public string Group { get; set; }
        public string GroupType { get; set; }
        public List<ProductCategories> ProductGroupslist { get; set; }
        //..................Property for Web Orders....................

        public bool WebOrderable { get; set; }

          [Display(Name = " Web Name")]
        public string WebName { get; set; }
        public string WebDescription { get; set; }
          [Display(Name = "Web Type")]
        public string WebType { get; set; }
        public List<string> WebOptionslist { get; set; }
        public string[] WebOptionValue { get; set; }
         [Display(Name = " Web Options")]
        public string WebOptions { get; set; }
          [Display(Name = " Web Option Min")]
        public string WebOptionMin { get; set; }
        [Display(Name = " Web Option Max")]
        public string WebOptionMax { get; set; }
        public List<ProductCategories> ProductSubGrouplist { get; set; }
        public int ProductSubGroupId { get; set; }

    }



    public class CompanyProductModel
    {
        public List<CompanyModel> ProductCompanylist { get; set; }
        public ProductModel ProductModel { get; set; }

    }

    public class ProductSchedulesModel
    {

        public int Row_Id { get; set; }
        public string XeroCode { get; set; }
        [Display(Name = "Value")]
        public string Value { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        // [Display(Name = "Select Color")]
        public int? ColorId { get; set; }
        public string ColorCode { get; set; }
        public bool CreateEvent { get; set; }
        public bool SendEmail { get; set; }
        public string EmailAddress { get; set; }
        public IList<ColorMaster> Colorlist { get; set; }
        public string Productid { get; set; }
        public List<ProductCategories> ProductGroupslist { get; set; }

        [Display(Name = "Product Group")]
        public int ProductGroupId { get; set; }
        public string ProductGroup { get; set; }

        [Display(Name = "Product Qty")]
        public int WebOptionMax { get; set; }

    }




}