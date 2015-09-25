using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{
    public interface IContactRepository
    {

        IEnumerable<Contact> GetAll();
        //void Add(Contact contact);
        //void Update(Contact contact);
        //int Delete(Contact contact);
    }



    public class ContactRepository : IContactRepository
    {
          private OrderMgntEntities db = null;

        public ContactRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public ContactRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        public IEnumerable<Contact> GetAll()
        {
            return db.Contacts.ToList();
        }
    }
}