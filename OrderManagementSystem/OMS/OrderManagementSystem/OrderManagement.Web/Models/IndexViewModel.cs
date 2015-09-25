using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OrderManagement.Web.Models.Repository;

namespace OrderManagement.Web.Models
{
    public class IndexViewModel : LoginModel
    {
        private readonly ICompanyRepository _repository;
        private readonly IProductRepository _productRepository;
        private IProductCompanyRepository _productCompanyRepo;
        OrderMgntEntities _db = null;

        public IndexViewModel() {
            this._repository = new CompanyRepository();
            this._productRepository = new ProductRepository();
            this._productCompanyRepo = new ProductCompanyRepository();
            this._db = new OrderMgntEntities();
            this.Companylist = new List<Company>();
            this.ProductGrouplist = new List<ProductGroup>();
        }

        public SelectList GetCompaniesList()
        {
           
            IEnumerable<SelectListItem> stateList = _repository.SelectAll().Select(m => new SelectListItem() { Text = m.XeroName, Value = m.Row_Id.ToString() }); //(from m in _db.Companies select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.XeroName, Value = m.CompanyCode.ToString() });
            return new SelectList(stateList, "Value", "Text");
        }
        public List<Company> Companylist { get; set; }
        public List<ProductGroup> ProductGrouplist { get; set; }
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}")]
        public DateTime EndDate { get; set; }

    }
}