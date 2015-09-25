using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace OrderManagement.Web.Models.Repository
{
    public interface IOrderRepository
    {
        IEnumerable<Order> SelectAll();
        IEnumerable<Property> GetAllProperty();
        IList<OrderItem> OrderItemsByOrderid(int orderid);
        Order GetOrderById(int orderid);
        Property GetPropertyById(int propertyid);
        OrderItem GetOrderItemById(int orderitemid);
        void SaveOrderAttachment(OrderAttachment obj);
        void UpdateOrder(Order obj);
        OrderItem GetOrderItemByOrderId(int orderid);
        IList<SelectOrderableProducts> SelectOrderableProducts(int orgid, int? userid, string userType, int? compamyid);

        int InsertWebProperty(int orgid, int? userid, string address, string property_Id, string latitude, string longitude, int? compamyid);
        IList<InsertWebOrder> InsertWebOrder(int orgid, int? ordertypeid, int? orderid, int? userid, string address, int? property_Id, string comments, string userType, DateTime? requireTime, string description, int? mailId, int? compamyid, string status, string key);
        void InsertWebOrderItem(int orgid, int? orderid, int? userid, int? productid, decimal? quantity, decimal? price, decimal? discountPercent, string options, string userType, int? compamyid);

        int InsertWebContact(int company_Id, int order_id, int user_id, string first_name, string last_name, string email_address, string phone_number, string contact_type);

        int InsertJob(int org_Id, int? orderId, int? user_id, int? property_Id, DateTime? requiredDate, int? mail_Id, int? company_id, string status, string keys, int? adminid);

        int InsertJobEvent(int org_Id, int? job_Id, int? user_id, int? calendar_Id, int? event_Id, int? productGroup_Id, string status, string keys);

        IList<GetStagingCalenderUsers> GetStagingCalenderUsers(int org_id);
    }
    public class OrderRepository : IOrderRepository
    {
        private OrderMgntEntities db = null;



        public OrderRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public OrderRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        public IEnumerable<Order> SelectAll()
        {
            return db.Orders.ToList();
        }

        public IEnumerable<Property> GetAllProperty()
        {
            return db.Properties.ToList();
        }

        public IList<OrderItem> OrderItemsByOrderid(int orderid)
        {
            return db.OrderItems.Where(m => m.Order_Id == orderid).ToList();
        }

        public Order GetOrderById(int orderid)
        {
            return db.Orders.FirstOrDefault(m => m.Row_Id == orderid);
        }

        public Property GetPropertyById(int propertyid)
        {
            return db.Properties.FirstOrDefault(m => m.Row_Id == propertyid);
        }

        public OrderItem GetOrderItemById(int orderitemid)
        {
            return db.OrderItems.FirstOrDefault(m => m.Row_Id == orderitemid);

        }

        public void SaveOrderAttachment(OrderAttachment obj)
        {
            db.OrderAttachments.Add(obj);
            db.SaveChanges();

        }

        public void UpdateOrder(Order obj)
        {
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }


        public OrderItem GetOrderItemByOrderId(int orderid)
        {
            return db.OrderItems.FirstOrDefault(m => m.Order_Id == orderid);

        }

        public virtual IList<SelectOrderableProducts> SelectOrderableProducts(int orgid, int? userid, string userType, int? compamyid)
        {
            return CachedOrderRepository.Instance.SelectOrderableProducts(orgid, userid, userType, compamyid).ToList();
           // return db.SelectOrderableProducts(orgid, userid, userType, compamyid).ToList();
        }


        public int InsertWebProperty(int orgid, int? userid, string address, string property_Id, string latitude, string longitude, int? compamyid)
        {
            return Convert.ToInt32((db.InsertWebProperty(orgid, userid, address, property_Id, latitude, longitude, 0)).FirstOrDefault().recidentity.Value);
        }

        public IList<InsertWebOrder> InsertWebOrder(int orgid, int? ordertypeid, int? orderid, int? userid, string address, int? property_Id, string comments, string userType, DateTime? requireTime, string description, int? mailId, int? compamyid, string status, string key)
        {
            return db.InsertWebOrder(orgid, ordertypeid, null, userid, property_Id, comments, null, userType, requireTime, description, null, compamyid, status, key).ToList();
        }




        public void InsertWebOrderItem(int orgid, int? orderid, int? userid, int? productid, decimal? quantity, decimal? price, decimal? discountPercent, string options, string userType, int? compamyid)
        {
            db.InsertWebOrderItem(orgid, orderid, userid, productid, quantity, price, discountPercent, null, options, userType, compamyid);

        }

        public int InsertWebContact(int company_Id, int order_id, int user_id, string first_name, string last_name, string email_address, string phone_number, string contact_type)
        {
            return db.InsertWebContact(company_Id, order_id, user_id, first_name, last_name, email_address, phone_number, contact_type);


        }

        public int InsertJob(int org_Id, int? orderId, int? user_id, int? property_Id, DateTime? requiredDate, int? mail_Id, int? company_id, string status, string keys, int? adminid)
        {
            return db.InsertJob(org_Id, orderId, user_id, property_Id, requiredDate, mail_Id, company_id, null, keys,adminid);


        }

        public int InsertJobEvent(int org_Id, int? job_Id, int? user_id, int? calendar_Id, int? event_Id, int? productGroup_Id, string status, string keys)
        {
            return db.InsertJobEvent(org_Id, job_Id, user_id, calendar_Id, event_Id, productGroup_Id, status, keys);
        }

        public IList<GetStagingCalenderUsers> GetStagingCalenderUsers(int org_id)
        {
            return db.GetStagingCalenderUsers(org_id).ToList();
        }



    }
}