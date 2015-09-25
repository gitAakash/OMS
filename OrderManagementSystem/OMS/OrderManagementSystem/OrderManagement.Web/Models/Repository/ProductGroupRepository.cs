using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{
    public interface IProductGroupRepository
    {
        IEnumerable<ProductGroup> GetAllProductgroup();
       // ProductGroup GetProductGroupByProductId(int pid);
        ProductGroup GetProductGroupById(int id);
        IList<ProductCategories> GetProductgroupBySp(int? orgId, int? parentId);
        void Add(ProductGroup obj);
        void Update(ProductGroup obj);
        // Nullable<int> oRG_ID, Nullable<int> uSER_ID, string uSER_TYPE, Nullable<int> uSER_COMPANY_ID, Nullable<int> pRODUCT_ROW_ID
        IList<SelectProductGroupOptions> GetProductGroupOptionBySp(int orgid,int userid, string userType, int? companyid,int productRowid);
    }

    public class ProductGroupRepository : IProductGroupRepository
    {

          private OrderMgntEntities db = null;

        public ProductGroupRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public IEnumerable<ProductGroup> GetAllProductgroup()
        {
            return db.ProductGroups.ToList();
        }

        public ProductGroupRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        //public ProductGroup GetProductGroupByProductId(int pid)
        //{
        //    return db.ProductGroups.SingleOrDefault(c => c.ProductId ==pid);
        //}
        public ProductGroup GetProductGroupById(int id)
        {
            return db.ProductGroups.SingleOrDefault(c => c.Row_Id == id);
        }
        public void Add(ProductGroup obj)
        {
            db.ProductGroups.Add(obj);
            db.SaveChanges();

        }

        public void Update(ProductGroup obj)
        {
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IList<ProductCategories> GetProductgroupBySp(int? orgId, int? parentId)
        {
            return db.GetProductCategory(orgId, parentId).ToList();
        }

        public IList<SelectProductGroupOptions> GetProductGroupOptionBySp(int orgid,int userid,string userType,int? companyid,int productRowid)
        {
            return db.SelectProductGroupOptions(orgid,userid,userType,companyid,productRowid).ToList();
        }
    }
}