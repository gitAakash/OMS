using OrderManagement.Web.Helper.Utilitties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{

    public interface IAccountRepository
    {
        UserAuthenticationResult Login(string Emailid, string password, bool persist);
        User GetUserByEmailId(string Emailid);
        User GetUserById(int id);
        IList<SelectOrg> GetCompanyLogo(string DomainName);

    }
    public class AccountRepository : IAccountRepository
    {
        private OrderMgntEntities db = null;
        public AccountRepository()
        {
            this.db = new OrderMgntEntities();
        }
        public AccountRepository(OrderMgntEntities db)
        {
            this.db = db;
        }


        public UserAuthenticationResult Login(string Emailid, string password, bool persist)
        {
            //Allow whitespaces before and after username like any standard site
            //  var users = db.Users.Where(c => c.EmailAddress == Emailid.Trim() && c.Password==password  && c.IsActive == true);

            string userspwdInfo = string.Empty;
            var userpwd = db.Users.SingleOrDefault(m => m.EmailAddress == Emailid.Trim() && m.IsActive == true);
            if (userpwd != null)
            {
                userspwdInfo = userpwd.Password;
                password = Cryptography.Encrypt(password);
            }
            //fromCalendar = OrderMangtDB.Calendars.SingleOrDefault(m => m.Row_Id == fromCalendarId).Name;

            var users = db.Users.Where(c => c.EmailAddress == Emailid.Trim() && userspwdInfo.Equals(password) && c.IsActive == true);

            if (users != null)
            {

                if (users.Count() == 0)
                {
                    return UserAuthenticationResult.UnSuccessful;
                }
                if (users.Count() > 1)
                {
                    return UserAuthenticationResult.DuplicateUser;
                }
                else
                {
                    return UserAuthenticationResult.Authenticated;
                }
            }
            else
                return UserAuthenticationResult.UnSuccessful;

        }
        public User GetUserByEmailId(string EmailId)
        {
            User omUser = null;
            if (!string.IsNullOrEmpty(EmailId))
            {
                //Allow whitespaces before and after username like any standard site
                IEnumerable<User> OmUserQuery = db.Users.Where(m => m.EmailAddress.Equals(EmailId.Trim()));
                if (OmUserQuery.Count() > 1)
                {
                    return null;
                }

                if (OmUserQuery.Count() == 0)
                    return null;
                omUser = OmUserQuery.First();
                return omUser;
            }
            else
            {
                return null;
            }
        }

        public virtual User GetUserById(int id)
        {
            return db.Users.Find(id);
        }


        public IList<SelectOrg> GetCompanyLogo(string DomainName)
        {
            return db.SelectOrg(DomainName).ToList();
        }



    }
}