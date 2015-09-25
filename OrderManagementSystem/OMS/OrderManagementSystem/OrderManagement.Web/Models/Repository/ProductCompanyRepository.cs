using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{
    public interface IProductCompanyRepository
    {
        IEnumerable<ProductCompany> GetAllProductCompanylist();
        int Add(ProductCompany obj);
        int Update(ProductCompany obj);
        int Delete(ProductCompany objProductCompany);
       // void Save();
    }

    public class ProductCompanyRepository : IProductCompanyRepository
    {
           private OrderMgntEntities db = null;

        public ProductCompanyRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public ProductCompanyRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        public IEnumerable<ProductCompany> GetAllProductCompanylist()
        {
            return db.ProductCompanies.ToList();
        }
        public int Add(ProductCompany obj)
        {

            db.ProductCompanies.Add(obj);
          return  db.SaveChanges();
        }

        public int Update(ProductCompany obj)
        {
            db.Entry(obj).State = EntityState.Modified;
           return db.SaveChanges();
        }

        public int Delete(ProductCompany objProductCompany)
        {
            
           // var existing = db.ProductCompanies.Find(id);
            db.ProductCompanies.Remove(objProductCompany);
           return db.SaveChanges();
        }

        //public void Save()
        //{
        //    db.SaveChanges();
        //}
    }
}