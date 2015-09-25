using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{//Product
    
    public interface IProductRepository
    {

        IEnumerable<SpGetAllProduct> GetAllProductBySp(int orgId);
        IEnumerable<Product> SelectAll();
        Product SelectById(int id);
      //  IEnumerable<ProductCompany>GetAllProductCompanylist();
       // IEnumerable<ProductGroup> GetAllProductgroup();
        IEnumerable<ColorMaster> GetAllColor();
      //  IEnumerable<ProductSchedule> GetAllProductSchedule();
       // ProductSchedule GetProductScheduleByXeroCode(string xeroCode);
       
        void Insert(Product obj);
        void Update(Product obj);
        void Delete(string id);
        void Save();
        IList<ProductCategories> GetProductgroupBySp(int? orgId, int? parentId);
    }

    public class ProductRepository : IProductRepository
    {

        private OrderMgntEntities db = null;

        public ProductRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public ProductRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        public IEnumerable<Product> SelectAll()
        {
            return db.Products.ToList();
        }

        public Product SelectById(int id)
        {
            return db.Products.Find(id);
        }

        public void Insert(Product obj)
        {

            db.Products.Add(obj);
            db.SaveChanges();
        }

        public void Update(Product obj)
        {
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(string id)
        {

            var existing = db.Products.Find(Convert.ToInt32(id));
            db.Products.Remove(existing);
        }

        public void Save()
        {
            db.SaveChanges();
        }
       //public IEnumerable<ProductGroup> GetAllProductgroup()
       // {
       //     return db.ProductGroups.ToList();
       // }

       public IEnumerable<ColorMaster> GetAllColor()
        {

            return db.ColorMasters.ToList();
        }

       public IList<ProductCategories> GetProductgroupBySp(int? orgId, int? parentId)
       {
           return db.GetProductCategory(orgId, parentId).ToList();
       }

       public IEnumerable<SpGetAllProduct> GetAllProductBySp(int orgId )
       {
           return db.SelectAllProduct(orgId);
       }
    }
}