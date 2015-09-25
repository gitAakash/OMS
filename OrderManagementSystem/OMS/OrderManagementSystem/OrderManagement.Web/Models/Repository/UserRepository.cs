using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Microsoft.Data.Edm.Library;
using System.Data;


namespace OrderManagement.Web.Models.Repository
{
    public interface IUserRepository
    {
        UserPersistenceInfo CreateUser(string email, string password, string firstName, string lastName);
        UserPersistenceInfo PersistUser(User Credential);

        IEnumerable<SpGetAllUsers> GetAllUsersBySp(int orgid);
        IEnumerable<User> GetAll();
        IEnumerable<UserType> GetUserType();
        User GetById(int id);
        //IEnumerable<User> GetAllColor();
        User Add(User obj);
        User Update(User obj);
        void DeleteUserProductGroup(int id);
        void DeleteUsercalendar(int userid);
        //void Save();GetUserType
        void AddUserProductGroup(UserProductGroup obj);
        void UpdateUserProductGroup(UserProductGroup obj);
        IList<UserProductGroup> GetAllUserProductGroups();

        void AddAttachment(Attachment obj);
        Attachment GetAttachmentByUserId(int userid);
        void DeleteAttachment(int id);
        void UpdateAttachment(Attachment attachment);

        IList<OrderAttachment> OrderAttachment(int orgid);
        IList<OrderStatus> OrderStatus(int orgid);

        IEnumerable<OrderAttachment> GetAllOrderAttachment();
        IList<JobStatus> GetjobStatusByUserId(int userid,int orgid);
        IList<GetAllUsersJobStatus> AlluserActivities(int orgid);
        

        void Dispose();

    }

    public class UserRepository : IUserRepository
    {
        private OrderMgntEntities db = null;

        public UserRepository()
        {
            this.db = new OrderMgntEntities();
        }
        public UserRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        public void Dispose()
        {

            db.Dispose();
        }

        public IEnumerable<SpGetAllUsers> GetAllUsersBySp(int orgid)
        {

            return db.SelectAllUsers(orgid);
        }
        public IEnumerable<User> GetAll()
        {
            return db.Users.ToList();
        }

        public IEnumerable<OrderAttachment> GetAllOrderAttachment()
        {

            return db.OrderAttachments.ToList();
        }

        public IList<OrderStatus> OrderStatus(int orgid)
        {
            return db.OrderStatus1.Where(m => m.Org_Id == orgid).ToList();
        }

        public IEnumerable<UserType> GetUserType()
        {
            return db.UserTypes.ToList();
        }

        public UserPersistenceInfo CreateUser(string Email, string password, string firstName, string lastName)
        {

            RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];

            gen.GetNonZeroBytes(salt);

            //if (salt == null || salt.Count() == 0)
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

            passwordBytes.CopyTo(saltedPassword, 0);
            salt.CopyTo(saltedPassword, passwordBytes.Length);

            User Cred = new User();
            Cred.FirstName = firstName;
            Cred.LastName = lastName;
            //   Cred.Salt = salt;
            Cred.Password = Hash(saltedPassword);
            Cred.EmailAddress = Email;
            Cred.UserType = 1;
            Cred.IsActive = true;
            Cred.Updated = DateTime.Now;
            Cred.Created = DateTime.Now;

            return PersistUser(Cred);
        }
        public string CreatePassword()
        {
            return Path.GetRandomFileName().Replace(".", "");
        }

        private string Hash(byte[] passwordWithSalt)
        {
            SHA512Managed hashing = new SHA512Managed();
            byte[] hashedPasswordWithSalt = hashing.ComputeHash(passwordWithSalt);
            return Convert.ToBase64String(hashedPasswordWithSalt);
        }

        public UserPersistenceInfo PersistUser(User Credential)
        {
            UserPersistenceInfo userpersistence = new UserPersistenceInfo();
            if (db.Users.Any(U => U.EmailAddress.ToLower() == Credential.EmailAddress.ToLower()))
            {
                userpersistence.UserId = Credential.Row_Id;
                userpersistence.UserPersistenceResult = UserPersistenceResult.UsernameInUse;
                return userpersistence;
            }
            if (Credential.Row_Id == 0)
            {
                db.Users.Add(Credential);
                db.SaveChanges();

                userpersistence.UserId = Credential.Row_Id;
                userpersistence.UserPersistenceResult = UserPersistenceResult.Successful;
            }
            return userpersistence;
        }

        public User Add(User obj)
        {
            db.Users.Add(obj);
            db.SaveChanges();
            User insertedUser = db.Users.SingleOrDefault(u => u.EmailAddress == obj.EmailAddress);
            return insertedUser;
        }

        public void AddUserProductGroup(UserProductGroup obj)
        {
            db.UserProductGroups.Add(obj);
            db.SaveChanges();

        }

        public void UpdateUserProductGroup(UserProductGroup obj)
        {
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();

        }

        public User GetById(int id)
        {
            return db.Users.Find(id);

        }

        public IList<UserProductGroup> GetAllUserProductGroups()
        {
            return db.UserProductGroups.ToList();
        }

        public void AddAttachment(Attachment obj)
        {
            db.Attachments.Add(obj);
            db.SaveChanges();
        }

        public Attachment GetAttachmentByUserId(int userid)
        {
            return db.Attachments.Where(m => m.UserId == userid).FirstOrDefault();
        }

        public User UpdateOLD(User obj)
        {
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
            User insertedUser = db.Users.SingleOrDefault(u => u.EmailAddress == obj.EmailAddress);
            return insertedUser;
        }

        /// <summary>
        /// This method is used for updating the user table
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public User Update(User obj)
        {
            //We query local context first to see if it's there.
            var UserEntity = db.Users.Find(obj.Row_Id);

            //We have it in the context, need to update.
            if (UserEntity != null)
            {
                var attachedUserEntry = db.Entry(UserEntity);
                attachedUserEntry.CurrentValues.SetValues(obj);
            }
            db.SaveChanges();
            User insertedUser = db.Users.SingleOrDefault(u => u.EmailAddress == obj.EmailAddress);
            return insertedUser;
        }

        public void DeleteUserProductGroup(int id)
        {
            UserProductGroup existing = db.UserProductGroups.Find(id);
            db.UserProductGroups.Remove(existing);
            db.SaveChanges();

        }

        public void UpdateAttachment(Attachment attachment)
        {
            db.Entry(attachment).State = EntityState.Modified;
            db.SaveChanges();

        }

        public void DeleteAttachment(int id)
        {
            Attachment existing = db.Attachments.Find(id);
            db.Attachments.Remove(existing);
            db.SaveChanges();

        }

        public IList<OrderAttachment> OrderAttachment(int orgid)
        {
            return db.OrderAttachments.Where(m => m.Org_Id == orgid).ToList();
        }


      public void DeleteUsercalendar(int userid)
        {

            CalendarUser existing = db.CalendarUsers.FirstOrDefault(m => m.UserId == userid);
            db.CalendarUsers.Remove(existing);
            db.SaveChanges();

        }

        public IList<JobStatus> GetjobStatusByUserId(int userid,int orgid)
        {
           return db.JobStatus.Where(m => m.Org_Id == orgid && m.User_Id == userid).ToList();
        }

        public IList<GetAllUsersJobStatus> AlluserActivities(int orgid)
       {
           return db.GetAllUsersJobStatus(orgid).ToList();
       }


    }
}