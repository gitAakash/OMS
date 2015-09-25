using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{
    public interface IJobTrackingRepository
    {
        IList<TrackingJobs> GetAllJobs(int orgid, int? userid, string userType, int? compamyid, string search, bool IsRefreshCache = false);
        IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int orgid, int? userid, string userType, int? compamyid, int? jobId);

        void InsertJobCopy(int orgid, int userid, string userType, int companyid, int jobid, int typeid, string title, int titleWordCount, string body, int bodyWordCount, string status);

        void InsertJobAttachment(int orgid, int userid, string userType, int? companyid, int jobid, string fileName, string fileExt, int fileSize, string filePath, string grouptype, string tags, string folder, string ThumbImgPath="", int Selected=0, int Row_id = 0, int isDeleted = 0);

        IList<GetJobAttachments> GetJobAttachments(int orgid, int? userid, string userType, int? compamyid, int? groupType, int? job_Id, string Tags = "");

        IList<SelectJobAttachmentTypes> GetJobAttachmentTypes(int orgid, int? userid, string userType, int? compamyid, int? groupid);

        IList<SelectJobCopyType> GetAllJobCopyTypes(int orgId, int userId, string userType, int companyId);
        IList<SelectJobCopy> GetAllJobCopy(int orgId, int userId, string userType, int companyId, int jobid);
        void UpdateJobCopy(int orgid, int userid, string userType, int companyid, int jobid, int typeid, string title, int titleWordCount, string body, int bodyWordCount, string status);
        void DeleteJobCopy(int orgid, int userid, string userType, int companyid, int jobCopyRowid);
        void UpdateJobAttachmentSelected(int orgid, int userid, string userType, int? compamyid, int rowid, bool isSelected);

        void DeleteJob(int orgid, int? userid, string userType, int? compamyid, int? jobid);

        void UpdateJobAttachmentComments(int orgid, int userid, string userType, int? compamyid, int rowid, string Comments);
        void InsertJobAnnotation(int orgid, int userid, string userType, int? compamyid, int rowid, string AnnotationText);
        void UpdateJobEmail_Notification(int JobId, string EmailId);
        int UpdateJobstatus(int JobId, int orgId, int UpdatedBy);
        void UpdateFolderLock(int JobId, int RowId);

    //    IList<GetJobAttachments> GetJobAttachments(int orgid, int? userid, string userType, int? compamyid, int? groupType, int? job_Id, string Tags = "");

        IList<GetJobCommentsById> GetJobCommentsById(int orgid, int? userid, string userType, int? compamyid, int? Row_id);

        IList<GetFoldersAttachmentCount> GetFoldersAttachmentCount(int? Job_id, int orgid, int userid, string userType, int? compamyid);
        IList<GetUserCommentsbyJobId> GetUserCommentsbyJobId(int? Job_id, int orgid, int userid, string userType, int? compamyid);

        IList<GetJobAnnotationById> GetJobAnnotationbyFileId(int? File_id, int orgid, int userid, string userType, int? compamyid);
         void UpdateJobAttachmenTags(string TagSelected, int? Row_id, int? OrgId, int? Job_id);
        void UpdateJobAttachmentUploaded(int orgid, int userid, string userType, int? compamyid, int rowid, string uploadStatus);
        IList<SelectJobAttachmentTemplate_Result> SelectJobAttachmentTemplate(int orgid, int? userid, string userType, int? compamyid, int? job_Id, string folder);
        void UpdateConfirmImageSelected(string RowId, int jobId);
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

        public void UpdateJobAttachmentUploaded(int orgid, int userid, string userType, int? compamyid, int rowid, string uploadStatus)
        {
            db.UpdateJobAttachmentUploaded(orgid, userid, userType, compamyid, rowid, uploadStatus);
        }

        public virtual IList<TrackingJobs> GetAllJobs(int orgid, int? userid, string userType, int? compamyid, string search, bool IsRefreshCache=false)
       {
           if (search.Length <= 0)
           {
               return CachedJobTrackingRepository.Instance.GetAllJobs(orgid, userid, userType, compamyid, search, IsRefreshCache).ToList();
           }
           else
           {
               return CachedJobTrackingRepository.Instance.GetAllJobs(orgid, userid, userType, compamyid, search, IsRefreshCache).ToList().Where(s => s.EventTitle.Contains(search)).ToList();
               

           }
       //    //return db.SelectJobs(orgid,userid,userType,compamyid,search).ToList();
       }

        //public IList<TrackingJobs> GetAllJobs(int orgid, int? userid, string userType, int? compamyid, string search)
        //{
        //    return db.SelectJobs(orgid, userid, userType, compamyid, search).ToList();
        //}

        public void DeleteJob(int orgid, int? userid, string userType, int? compamyid, int? jobid)
        {

            db.DeleteJob(orgid, userid, userType, compamyid, jobid);
        }

       // public  IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int orgid, int? userid, string userType, int? compamyid)
       //{
       //     //db.InsertJobAttachment()
      
       //    return   db.SelectJobAttachmentFolders(orgid, userid, userType, compamyid).ToList();
       //}

        public virtual IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int orgid, int? userid, string userType, int? compamyid,int? job_Id)
        {
            return CachedJobTrackingRepository.Instance.SelectJobAttachmentFolders(orgid, userid, userType, compamyid,job_Id).ToList();
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

        public void InsertJobAttachment(int orgid, int userid, string userType, int? companyid, int jobid, string fileName, string fileExt, int fileSize, string filePath, string grouptype, string tags, string folder, string ThumbImgPath="",int Selected=0, int Row_id = 0, int isDeleted = 0)
       {
           db.InsertJobAttachment(orgid, userid, userType, companyid, jobid, fileName, fileExt, fileSize, grouptype, tags, folder, filePath, ThumbImgPath,Selected, Row_id, isDeleted);
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

        public void UpdateJobAttachmentComments(int orgid, int userid, string userType, int? compamyid, int rowid, string Comments)
        {
            //db.UpdateJobAttachmentComments(orgid, userid, userType, compamyid, rowid, Comments);
            db.UpdateJobAttachmentComments(orgid,userid,userType,compamyid,rowid,Comments,false,0);
        }

        public IList<GetJobCommentsById> GetJobCommentsById(int orgid, int? userid, string userType, int? compamyid, int? Row_id)
        {
            return db.GetJobCommentsById(orgid, userid, userType, compamyid, Row_id).ToList();
        }

        public IList<GetFoldersAttachmentCount> GetFoldersAttachmentCount(int? Job_id, int orgid, int userid, string userType, int? compamyid)
        {
            return db.GetFoldersAttachmentCount(Job_id,orgid, userid, userType, compamyid).ToList();
        }

        public IList<GetUserCommentsbyJobId> GetUserCommentsbyJobId(int? Job_id, int orgid, int userid, string userType, int? compamyid)
        {
            return db.GetUserCommentsbyJobId(Job_id, orgid, userid, userType, compamyid).ToList();
        }

        public IList<GetJobAnnotationById> GetJobAnnotationbyFileId(int? File_id, int orgid, int userid, string userType, int? compamyid)
        {
            return db.GetJobAnnotationById(orgid, userid, userType, compamyid, File_id).ToList();
        }


        public IList<SelectJobAttachmentTemplate_Result> SelectJobAttachmentTemplate(int orgid, int? userid, string userType, int? compamyid, int? job_Id, string folder)
        {
            return db.SelectJobAttachmentTemplate(orgid, userid, userType, compamyid, job_Id,folder).ToList();
        }

        public void UpdateJobEmail_Notification(int JobId, string EmailId)
        {
            db.UpdateJobEmail_Notification(JobId, EmailId);
        }
        public int UpdateJobstatus(int jobId, int orgId, int updatedBy)
        {
           int i= db.UpdateJobstatus(jobId,updatedBy,orgId);
            return i;
        }

        public void InsertJobAnnotation(int orgid, int userid, string userType, int? compamyid, int rowid, string AnnotationText)
        {
            db.InsertJobAnnotation(orgid, userid, userType, compamyid, rowid, AnnotationText, false);
        }
        public void UpdateJobAttachmenTags(string TagSelected, int? Row_id, int? OrgId, int? Job_id)
        {
            db.UpdateJobAttachmentTags(TagSelected, Row_id, OrgId, Job_id);
        }
        public void UpdateConfirmImageSelected(string RowId,int jobId) {
            db.UpdateImageSelection(RowId,jobId);
        }
        public void UpdateFolderLock(int jobId,int RowId)
        {
            db.UpdateFolderLock(jobId, RowId);
        }
    }
}