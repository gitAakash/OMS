using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{

    public interface IVideoRepository
    {
        IEnumerable<SelectClients> GetClients(int orgid, int? userid, string userType, int? compamyid, string search, int rowId = 0);
        Client Add(Client obj);
        Client Update(Client obj);

        void UpdateClients(int orgid, int client_Id, int user_id, string name, string main_Phone, string main_Email, string main_URL, bool isDeleted = false);

        IEnumerable<SelectVideo> GetVideos(int orgid, int? userid, string userType, int? compamyid, string search = "", int rowId = 0);

        void Insertvideo(int orgid, int client_Id, int user_id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                                string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                                string status, bool isDeleted, int job_id);

        int DeleteVideos(int orgid, int? userid, string userType, int? compamyid, int videoId = 0);


        IEnumerable<SelectVideoById> SelectVideoById(int orgid, int? userid, string userType, int? compamyid, string search, int rowId = 0);

        void UpdateVideo(int orgid, int client_Id, int user_id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                                     string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                                     string status, bool isDeleted, int row_id, int job_id);


        IEnumerable<GetAllJobs> GetAllJobs(int orgid, int? userid, string userType, int? compamyid);


    }
    public class VideoRepository : IVideoRepository
    {
        private OrderMgntEntities db = null;

        public VideoRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public VideoRepository(OrderMgntEntities db)
        {
            this.db = db;
        }



        public IEnumerable<SelectClients> GetClients(int orgid, int? userid, string userType, int? compamyid, string search = "", int rowId = 0)
        {
            
            return db.SelectClients(orgid, userid, userType, search, rowId);

           
        }

        public Client Add(Client obj)
        {
            db.Clients.Add(obj);
            db.SaveChanges();
            Client insertedUser = db.Clients.SingleOrDefault(u => u.Main_Email == obj.Main_Email);
            return insertedUser;
        }


        /// <summary>
        /// This method is used for updating the Client table
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Client Update(Client obj)
        {
            //We query local context first to see if it's there.
            var UserEntity = db.Clients.Find(obj.Row_Id);

            //We have it in the context, need to update.
            if (UserEntity != null)
            {
                var attachedUserEntry = db.Entry(UserEntity);
                attachedUserEntry.CurrentValues.SetValues(obj);
            }
            db.SaveChanges();
            Client insertedUser = db.Clients.SingleOrDefault(u => u.Main_Email == obj.Main_Email);
            return insertedUser;
        }

        public void UpdateClients(int orgid, int client_Id, int user_id, string name, string main_Phone, string main_Email, string main_URL, bool isDeleted = false)
        {
            db.UpdateClients(orgid, client_Id, user_id, name, main_Phone, main_Email, main_URL, isDeleted);
        }


        public IEnumerable<SelectVideo> GetVideos(int orgid, int? userid, string userType, int? compamyid, string search = "", int rowId = 0)
        {
            return db.SelectVideo(orgid, userid, userType, compamyid, search, rowId);
        }

        public IEnumerable<SelectVideoById> SelectVideoById(int orgid, int? userid, string userType, int? compamyid, string search = "", int rowId = 0)
        {

            return db.SelectVideoById(orgid, userid, userType, compamyid, search, rowId);
        }


        public IEnumerable<GetAllJobs> GetAllJobs(int orgid, int? userid, string userType, int? compamyid)
        {
            return db.GetAllJobs(orgid, userid, userType, compamyid);
        }



        public void Insertvideo(int orgid, int client_Id, int user_id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                                string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                                string status, bool isDeleted,int job_id)
        {
            db.InsertVideo(orgid, client_Id, user_id, title, fileName, fileExtension, fileSize, files3location, reference, host_Primary, hostPrimaryLink, hostSecondary,
                          hostSecondaryLink, comments, status, isDeleted, job_id);
        }

        public void UpdateVideo(int orgid, int client_Id, int user_id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                                string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                                string status, bool isDeleted, int row_id, int job_id)
        {
            db.UpdateVideo(orgid, client_Id, user_id, title, fileName, fileExtension, fileSize, files3location, reference, host_Primary, hostPrimaryLink, hostSecondary,
                         hostSecondaryLink, comments, status, isDeleted,row_id,job_id);
        }


        public int DeleteVideos(int orgid, int? userid, string userType, int? compamyid, int videoId = 0)
        {
            return db.DeleteVideo(orgid, userid, userType, compamyid, videoId);
        }

    }
}