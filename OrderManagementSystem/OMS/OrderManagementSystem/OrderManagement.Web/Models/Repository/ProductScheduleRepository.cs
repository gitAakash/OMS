using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{
    public interface IProductScheduleRepository
    {
        ProductSchedule GetProductScheduleByXeroCode(string xeroCode);
        List<ProductSchedule> GetAllProductSchedule();
        ProductSchedule GetById(int id);
         int Add(ProductSchedule obj);
        int Update(ProductSchedule obj);
        int Delete(int id);
       // void Save();
        IList<ColorMaster> GetAllColors();
    }

    public class ProductScheduleRepository :IProductScheduleRepository
    {
          private OrderMgntEntities db = null;

        public ProductScheduleRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public ProductScheduleRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        public ProductSchedule GetProductScheduleByXeroCode(string xeroCode)
        {
            return db.ProductSchedules.SingleOrDefault(c => c.XeroCode == xeroCode.Trim());
        }
        public int Add(ProductSchedule obj)
        {

            db.ProductSchedules.Add(obj);
           return db.SaveChanges();
        }
        public int Update(ProductSchedule obj)
        {
            db.Entry(obj).State = EntityState.Modified;
           return db.SaveChanges();
        }
        public ProductSchedule GetById(int id)
        {
            return db.ProductSchedules.Find(id);
        }

        public IList<ColorMaster> GetAllColors()
        {
            return db.ColorMasters.ToList();
        }

        public List<ProductSchedule> GetAllProductSchedule()
        {
            return db.ProductSchedules.ToList();
        }

        public int Delete(int id)
        {
            ProductSchedule existing = db.ProductSchedules.Find(id);
            db.ProductSchedules.Remove(existing);
           return db.SaveChanges();
        }
    }
}