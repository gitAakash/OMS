using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models.Repository;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using AutoMapper;

namespace OrderManagement.Web.Models.ServiceRepository
{
    public interface IVideoService
    {
        IEnumerable<SelectClients> GetClients(string search = "", int rowId = 0);
        int AddOrUpdate(ClientModel obj);
        void DeleteUser(string userid);
        IEnumerable<SelectVideo> GetVideos(string search = "", int rowId = 0);
        void Insertvideo( int client_Id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                                string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                                string status, bool isDeleted, int job_id);
        int DeleteVideos(int videoId = 0);

        IEnumerable<SelectVideoById> SelectVideoById(string search = "", int rowId = 0);

        void UpdateVideo(int client_Id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                       string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                       string status, int row_id, int job_id);

        IEnumerable<GetAllJobs> GetAllJobs();

    }

    public class VideoService : IVideoService
    {
        private IVideoRepository _repository;

        public VideoService(IVideoRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<SelectClients> GetClients(string search = "", int rowId = 0)
        {

            var currentUser = UserManager.Current();

            if (currentUser != null && currentUser.OrgId != null)
            {
                int orgid = currentUser.OrgId.Value;
                int userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                if (string.IsNullOrEmpty(search))
                    search = string.Empty;
                return _repository.GetClients(orgid, userid, userTypeName, compamyid, search, rowId);
            }
            else
                return null;

        }

        public int AddOrUpdate(ClientModel obj)
        {
            try
            {
                if (obj != null)
                {
                    var currentuser = UserManager.Current();

                    if (currentuser != null)
                    {
                        Client usr = new Client();
                        int orgid; int client_Id; int user_id; string name = string.Empty; string main_Phone = string.Empty; string main_Email = string.Empty; string main_URL = string.Empty; ;

                        // using (var transaction = new TransactionScope())
                        // {
                        client_Id = obj.Row_Id;
                        main_Phone = obj.Main_Phone;
                        obj.OrgId = int.Parse(currentuser.OrgId.ToString());

                        obj.IsDeleted = false;
                        _repository.UpdateClients(obj.OrgId, obj.Row_Id, currentuser.Row_Id, obj.Name, obj.Main_Phone, obj.Main_Email, obj.Main_URL);

                        //   transaction.Complete();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return 1;

        }

        public void DeleteUser(string userid)
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {

                    var currentuser = UserManager.Current();

                    if (currentuser != null)
                    {

                        var lstClient = GetClients(string.Empty, int.Parse(userid)).ToList();

                        if (lstClient != null)
                        {
                            var model = new ClientModel();
                            foreach (var item in lstClient)
                            {
                                model.OrgId = item.Org_Id.Value;
                                model.Main_Email = item.Main_Email;
                                model.Name = item.Name;
                                model.Main_URL = item.Main_URL;
                                model.Main_Phone = item.Main_Phone;
                                model.Row_Id = item.Row_Id;
                                model.IsDeleted = true;
                                _repository.UpdateClients(model.OrgId, model.Row_Id, currentuser.Row_Id, model.Name, model.Main_Phone, model.Main_Email, model.Main_URL, model.IsDeleted);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }


        public IEnumerable<SelectVideo> GetVideos(string search = "", int rowId = 0)
        {
            var currentUser = UserManager.Current();

            if (currentUser != null && currentUser.OrgId != null)
            {
                int orgid = currentUser.OrgId.Value;
                int userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                if (string.IsNullOrEmpty(search))
                    search = string.Empty;
                return _repository.GetVideos(orgid, userid, userTypeName, compamyid, search, rowId);
            }
            else
                return null;

        }

        public IEnumerable<SelectVideoById> SelectVideoById(string search = "", int rowId = 0)
        {
            var currentUser = UserManager.Current();

            if (currentUser != null && currentUser.OrgId != null)
            {
                int orgid = currentUser.OrgId.Value;
                int userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                if (string.IsNullOrEmpty(search))
                    search = string.Empty;
                return _repository.SelectVideoById(orgid, userid, userTypeName, compamyid, search, rowId);
            }
            else
                return null;

        }

        public IEnumerable<GetAllJobs> GetAllJobs()
        {
            var currentUser = UserManager.Current();

            if (currentUser != null && currentUser.OrgId != null)
            {
                int orgid = currentUser.OrgId.Value;
                int userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }
              
                return _repository.GetAllJobs(orgid, userid, userTypeName, compamyid);
            }
            else
                return null;

        }




        public void Insertvideo(int client_Id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                               string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                               string status, bool isDeleted, int job_id)
        {
            var currentUser = UserManager.Current();

            if (currentUser != null && currentUser.OrgId != null)
            {
                int orgid = currentUser.OrgId.Value;
                int userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                 _repository.Insertvideo(orgid, client_Id, userid, title, fileName, fileExtension, fileSize, files3location, reference, host_Primary, hostPrimaryLink, hostSecondary,
                          hostSecondaryLink, comments, status, isDeleted, job_id);

            }

        }


        public void UpdateVideo(int client_Id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                       string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                       string status, int row_id, int job_id)
        {
            var currentUser = UserManager.Current();

            if (currentUser != null && currentUser.OrgId != null)
            {
                int orgid = currentUser.OrgId.Value;
                int userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                _repository.UpdateVideo(orgid, client_Id, userid, title, fileName, fileExtension, fileSize, files3location, reference, host_Primary, hostPrimaryLink, hostSecondary,
                         hostSecondaryLink, comments, status, false,row_id, job_id);

            }

        }


        public int DeleteVideos(int videoId = 0)
        {

            var currentUser = UserManager.Current();

            if (currentUser != null && currentUser.OrgId != null)
            {
                int orgid = currentUser.OrgId.Value;
                int userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                return _repository.DeleteVideos(orgid, userid, userType.Value.ToString(), compamyid,videoId);
            }
            return 0;
        }



    }
}