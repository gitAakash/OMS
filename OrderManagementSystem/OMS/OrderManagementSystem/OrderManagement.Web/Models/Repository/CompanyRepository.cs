using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using OrderManagement.Web.Helper.Utilitties;

namespace OrderManagement.Web.Models.Repository
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> SelectAll();
        IEnumerable<Property> GetAllProperty();
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Contact> GetContactsByCompId(int id);
        Company GetById(string id);
        Contact GetContactsById(int id);
        int AddContacts(Contact obj);
        int DeleteContact(int id);
        void DeleteOrderContact(int id);
        int UpdateContacts(Contact obj);
        int SaveOrdercontact(OrderContact ordercontacts);
        void Add(Company obj);
        void Update(Company obj);
        void Delete(string id);
        void Save();
        IEnumerable<OrderItem> GetAllOrderItemsByOrder(int orderid);
        IEnumerable<OrderItem> GetAllOrderItems();
        OrderContact GetOrderContactByOrderID(int orderid);
      //  IList<UserProductGroup> GeUserProductGroupsbyUserId(int userid);
        IList<OrderContact> GetOrderContactsbyOrder(int orderid);
    }

    public class CompanyRepository : ICompanyRepository
    {

        private OrderMgntEntities db = null;

        public CompanyRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public CompanyRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        public IEnumerable<Company> SelectAll()
        {
            var currentOrg = UserManager.Current();
            if (currentOrg!=null)
                return db.Companies.Where(c => c.Active == true && c.Org_Id == currentOrg.OrgId).ToList();
            else
             return db.Companies.Where(c => c.Active == true).ToList();
        }

        public Company GetById(string id)
        {
            return db.Companies.Find(Convert.ToInt32(id));
        }

        public void Add(Company obj)
        {

            db.Companies.Add(obj);
        }

        public void Update(Company obj)
        {
            db.Entry(obj).State = EntityState.Modified;
        }

        public int AddContacts(Contact obj)
        {
            db.Contacts.Add(obj);
             db.SaveChanges();
             return obj.Row_Id;
            
        }
        public int UpdateContacts(Contact obj)
        {
            db.Entry(obj).State = EntityState.Modified;
            return db.SaveChanges();
        }

        public void Delete(string id)
        {

            Company existing = db.Companies.Find(Convert.ToInt32(id));
            db.Companies.Remove(existing);
        }

        public void Save()
        {
            db.SaveChanges();
           
        }

        public IEnumerable<Property> GetAllProperty()
        {
            return db.Properties.ToList();
        }
        public IEnumerable<Order> GetAllOrders()
        {
            return db.Orders.ToList();
        }

        public IEnumerable<Contact> GetContactsByCompId(int id)
        {
            return db.Contacts.Where(m => m.CompanyId == id).ToList();
        }
        public Contact GetContactsById(int id)
        {
            return db.Contacts.Find(id);
        }

        public int DeleteContact(int id)
        {
            Contact existing = db.Contacts.Find(id);
            db.Contacts.Remove(existing);
           return db.SaveChanges();
        }

        public IEnumerable<OrderItem> GetAllOrderItemsByOrder(int orderid)
        {
            return db.OrderItems.Where(m => m.Order_Id == orderid).ToList();

        }

        public IEnumerable<OrderItem> GetAllOrderItems()
        {
            return db.OrderItems.ToList();
        }


        public OrderContact GetOrderContactByOrderID(int orderid)
        {

            return db.OrderContacts.Where(m => m.OrderId == orderid).FirstOrDefault();


        }

        public IList<OrderContact> GetOrderContactsbyOrder(int orderid)
        {
            return db.OrderContacts.Where(m => m.OrderId == orderid).ToList();
        }

        public int SaveOrdercontact(OrderContact ordercontacts)
        {
            db.OrderContacts.Add(ordercontacts);
          return db.SaveChanges();

        }

        public void DeleteOrderContact(int id)
        {
            OrderContact existing = db.OrderContacts.FirstOrDefault(m => m.ContactId==id);
            db.OrderContacts.Remove(existing);
             db.SaveChanges();

        }
        //public IList<UserProductGroup> GeUserProductGroupsbyUserId(int userid)
        //{

        //    return db.UserProductGroups.Where(m => m.UserId == userid).ToList();
        //}

    }
}