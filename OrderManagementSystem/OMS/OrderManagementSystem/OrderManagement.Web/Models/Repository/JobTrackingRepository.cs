using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{
    public interface IJobTrackingRepository
    {
        IList<TrackingJobs> GetAllJobs(int orgid, int? userid, string userType, int? compamyid, string search);
        IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int orgid, int? userid, string userType, int? compamyid);

        void InsertJobCopy(int orgid, int userid, string userType, int companyid, int jobid, int typeid, string title, int titleWordCount, string body, int bodyWordCount, string status);

        void InsertJobAttachment(int orgid, int userid, string userType, int? companyid, int jobid, string fileName, string fileExt, int fileSize, string filePath, string grouptype, string tags, string folder, int Row_id = 0, int isDeleted = 0);

        IList<GetJobAttachments> GetJobAttachments(int orgid, int? userid, string userType, int? compamyid, int? groupType, int? job_Id, string Tags = "");

        IList<SelectJobAttachmentTypes> GetJobAttachmentTypes(int orgid, int? userid, string userType, int? compamyid, int? groupid);

        IList<SelectJobCopyType> GetAllJobCopyTypes(int orgId, int userId, string userType, int companyId);
        IList<SelectJobCopy> GetAllJobCopy(int orgId, int userId, string userType, int companyId, int jobid);
        void UpdateJobCopy(int orgid, int userid, string userType, int companyid, int jobid, int typeid, string title, int titleWordCount, string body, int bodyWordCount, string status);
        void DeleteJobCopy(int orgid, int userid, string userType, int companyid, int jobCopyRowid);
        void UpdateJobAttachmentSelected(int orgid, int userid, string userType, int? compamyid, int rowid, bool isSelected);

        void DeleteJob(int orgid, int? userid, string userType, int? compamyid, int? jobid);
    }

    public class JobTrackingRepository:IJobTrackingRepository
    {
         private OrderMgntEntities db = null;

        public JobTrackingRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public JobTrackingRepository(OrderMgntEntities db)
        {
            this.db = db;
        }


        public IList<TrackingJobs> GetAllJobs(int orgid, int? userid, string userType, int? compamyid, string search)
       {
            //db.InsertJobAttachment()
           return db.SelectJobs(orgid,userid,userType,compamyid,search).ToList();
          
          
       }

        public void DeleteJob(int orgid, int? userid, string userType, int? compamyid, int? jobid)
        {

            db.DeleteJob(orgid, userid, userType, compamyid, jobid);
        }

       // public  IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int orgid, int? userid, string userType, int? compamyid)
       //{
       //     //db.InsertJobAttachment()
      
       //    return   db.SelectJobAttachmentFolders(orgid, userid, userType, compamyid).ToList();
       //}

        public virtual IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int orgid, int? userid, string userType, int? compamyid)
        {
            return CachedJobTrackingRepository.Instance.SelectJobAttachmentFolders(orgid, userid, userType, compamyid).ToList();
            //  return   db.SelectJobAttachmentFolders(orgid, userid, userType, compamyid).ToList();
        }

        public void InsertJobCopy(int orgid, int userid, string userType, int companyid, int jobid, int typeid, string title, int titleWordCount, string body, int bodyWordCount, string status)
        {
            db.InsertJobCopy(orgid, userid, userType, companyid, jobid, typeid, title, titleWordCount, body, bodyWordCount, status);
        }

       //public void InsertJobAttachment(int orgid, int userid, string userType, int? companyid, int jobid, string fileName, string fileExt, int fileSize, byte[] file, string grouptype, string tags, string folder)
       //{
       //    db.InsertJobAttachment(orgid, userid, userType, companyid, jobid, fileName, fileExt, fileSize, file, grouptype, tags, folder);
       //}

       public void InsertJobAttachment(int orgid, int userid, string userType, int? companyid, int jobid, string fileName, string fileExt, int fileSize, string filePath, string grouptype, string tags, string folder, int Row_id = 0, int isDeleted=0 )
       {
           db.InsertJobAttachment(orgid, userid, userType, companyid, jobid, fileName, fileExt, fileSize, grouptype, tags, folder, filePath,Row_id,isDeleted);
       }


       public IList<GetJobAttachments> GetJobAttachments(int orgid, int? userid, string userType, int? compamyid, int? groupType, int? job_Id, string Tags = "")
       {
           return db.GetJobAttachments(orgid, userid, userType, compamyid, groupType, job_Id,Tags).ToList();
       }

       //public IList<GetJobAttachments> SelectJobAttachment(int orgid, int? userid, string userType, int? compamyid, int pageNum, int pagesize)
       //{
       //    return db.GetJobAttachments(orgid, userid, userType, compamyid).ToList();
       //}

       public IList<SelectJobAttachmentTypes> GetJobAttachmentTypes(int orgid, int? userid, string userType, int? compamyid, int? groupid)
       {
           return db.SelectJobAttachmentTypes(orgid, userid, userType, compamyid, groupid).ToList();
       }

       public IList<SelectJobCopyType> GetAllJobCopyTypes(int orgId, int userId, string userType, int companyId)
       {

           return db.SelectJobCopyTypes(orgId, userId, userType, companyId).ToList();
       }


       public IList<SelectJobCopy> GetAllJobCopy(int orgId, int userId, string userType, int companyId, int jobid)
       {

           return db.SelectJobCopy(orgId, userId, userType, companyId, jobid).ToList();
          
       }

       public void UpdateJobCopy(int orgid, int userid, string userType, int companyid, int jobid, int typeid, string title, int titleWordCount, string body, int bodyWordCount, string status)
       {
           db.UpdateJobCopy(orgid, userid, userType, companyid, jobid, typeid, title, titleWordCount, body, bodyWordCount, status);

       }

        public void DeleteJobCopy(int orgid, int userid, string userType, int companyid,int jobCopyRowid)
        {
            db.DeleteJobCopy(orgid, userid, userType, companyid, jobCopyRowid);
        }


        public void UpdateJobAttachmentSelected(int orgid, int userid, string userType, int? compamyid, int rowid, bool isSelected)
         {
             db.UpdateJobAttachmentSelected(orgid, userid, userType, compamyid, rowid, isSelected);
        }


      
    }
}