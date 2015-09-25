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

namespace OrderManagement.Web.Models.ServiceRepository
{
    public interface IJobTrackingService
    {
        IList<JobTrackingModel> GetAllJobs(string search, bool IsRefreshCache = false);
        IList<TrackingJobs> GetAllJobsBySp();
        IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int? job_Id);
        IList<GetJobAttachments> GetJobAttachments(int? groupType, int? job_Id, string Tags = "");
        IList<GetJobAttachments> GetJobAttachmentsForMail(int? groupType, int? job_Id, string Tags = "");
        IList<SelectJobCopyType> GetAllJobCopyTypes();
        IList<SelectJobCopy> GetAllJobCopy(int jobid);
        IList<GetJobCommentsById> GetJobCommentsById(int? Row_Id);
        IList<SelectJobAttachmentTypes> GetJobAttachmentTypes(int groupid);
        IList<GetFoldersAttachmentCount> GetFoldersAttachmentCount(int? Job_id);
        IList<GetUserCommentsbyJobId> GetUserCommentsbyJobId(int? Job_id);
        IList<GetUserCommentsbyJobId> GetUserCommentsbyJobIdForMail(int? Job_id);
        IList<GetJobAnnotationById> GetJobAnnotationbyFileId(int? FileId);
        IList<GetJobAnnotationById> GetJobAnnotationbyFileIdForMail(int? FileId);
        JobTrackingModel GetjobById(string jobid);
        void UpdateFolderLock(string jobId, int RowId);
        void InsertJobAttachment(string jobis, string groupType, string folderName, HttpPostedFileBase file, string tags, string filePath, string ThumbImgPath, int Selected=0, int Row_id = 0, int isDeleted = 0);
        void InsertJobAttachment(string jobid, string groupType, string folderName, string fileName, string fileExt, int fileSize, string tags, string filePath, string OriginalFileName, string ThumbImgPath, int Selected=0, int Row_id = 0, int isDeleted = 0);
        void InsertJobCopy(JobCopyModel model);
        void UpdateJobAttachment(string TagSelected, string FileName, string FilePath, string Job_id,int isSelected=0, int Row_id = 0, int isDeleted = 0);
        void UpdateJobCopy(JobCopyModel model);
        void UpdateJobAttachmentSelected(int rowId, bool isSelected);
        void UpdateJobAttachmentComments(int Row_Id, String Comments);
        void InsertJobAnnotation(int Row_Id, String AnnotationText);

        IList<SelectJobAttachmentTemplate_Result> SelectJobAttachmentTemplate(int? groupType, int? job_Id);
        void DeleteJobCopy(int jobCopyRowid);
        void DeleteJob(int jobid);
        void UpdateJobAttachmentUploaded(int rowid, string uploadStatus);
        /// <summary>
        /// Update email Notification of Job
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="Email"></param>
        void UpdateJobEmail_Notification(int JobId, string Email);
        void UpdateJobStatus(int JobId);
        void UpdateJobAttachmentTags(string TagSelected, int? Row_id, int? Job_id);
        void UpdateSelectedImageConfirmed(string RowId, int jobId);

    }

    public class JobTrackingService : IJobTrackingService
    {
        private IJobTrackingRepository _repository;
        public JobTrackingService(IJobTrackingRepository repository)
        {
            _repository = repository;

        }

        public void UpdateJobAttachmentUploaded(int rowid, string uploadStatus)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    int userid = currentUser.Row_Id;
                    string userType = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    _repository.UpdateJobAttachmentUploaded(orgid, userid, userType, compamyid, rowid, uploadStatus);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

        }

        public IList<JobTrackingModel> GetAllJobs(string search, bool IsRefreshCache = false)
        {

            IList<JobTrackingModel> joblst = new List<JobTrackingModel>();

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (currentUser.OrgId != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    var userType = currentUser.UserType;
                    string userTypeName = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    if (string.IsNullOrEmpty(search))
                        search = string.Empty;

                    var joblist = _repository.GetAllJobs(orgid, userid, userTypeName, compamyid, search, IsRefreshCache).ToList();
                    var joblistgroup = joblist.GroupBy(m => m.Row_Id).ToList();

                    foreach (var group in joblistgroup)
                    {
                        // Display key for group.
                        var jobTrack = new JobTrackingModel();
                        var joblistdistinct = joblist.Where(m => m.Row_Id == @group.Key).Distinct().FirstOrDefault();
                        if (joblistdistinct != null)
                        {
                            jobTrack.JobId = joblistdistinct.Row_Id;
                            jobTrack.Active = joblistdistinct.Status;
                            jobTrack.Completion = joblistdistinct.Completion_Percent.ToString();
                            jobTrack.JobTitle = joblistdistinct.JobTitle;
                            jobTrack.StatusColour = joblistdistinct.Status_Colour;

                            if (joblistdistinct.Required_By_Date != null)
                                jobTrack.RequireDate = joblistdistinct.Required_By_Date.Value;

                            jobTrack.CompanyName = joblistdistinct.Company_Name;

                            if (joblistdistinct.Company_Id != null) jobTrack.CompanyId = joblistdistinct.Company_Id.Value;
                            jobTrack.CreatedByName = joblistdistinct.Created_ByName;
                            jobTrack.Resource = joblistdistinct.Resource;
                            jobTrack.EventTitle = joblistdistinct.EventTitle;
                            //   jobTrack.ProductId = joblistdistinct.ProductId;
                            // if(joblistdistinct.WebOptionMax!=null)
                            //     jobTrack.Product_Qty = joblistdistinct.WebOptionMax;
                            if (joblistdistinct.Created != null) jobTrack.Created = joblistdistinct.Created.Value;
                            if (joblistdistinct.Updated != null) jobTrack.Updated = joblistdistinct.Updated.Value;
                            if (joblistdistinct.StartDate != null) jobTrack.StartDate = joblistdistinct.StartDate.Value;
                            if (joblistdistinct.EndDate != null) jobTrack.EndDate = joblistdistinct.EndDate.Value;
                            jobTrack.Description = joblistdistinct.Description;
                            jobTrack.JobEventStatus = joblistdistinct.JobEventStatus;
                            if (!string.IsNullOrEmpty(joblistdistinct.Email_Notification))
                                jobTrack.Email_Notification = joblistdistinct.Email_Notification;
                            if (!string.IsNullOrEmpty(joblistdistinct.Package))
                                jobTrack.Package = joblistdistinct.Package;
                            if (joblistdistinct.Package_Qty != null)
                            {
                                jobTrack.Product_Qty = joblistdistinct.Package_Qty;
                            }
                            else
                                jobTrack.Product_Qty = 0;


                        }
                        IList<TrackJobProductGroup> grplist = new List<TrackJobProductGroup>();
                        foreach (var value in @group)
                        {
                            var productGroup = new TrackJobProductGroup();
                            productGroup.ProductGroupName = value.Product_Group;
                            productGroup.Status = value.Status;

                            if (value.Required_By_Date != null)
                                productGroup.BookingDate = value.Required_By_Date.Value;

                            if (value.StartDate != null)
                                productGroup.StartDate = value.StartDate.Value;

                            if (value.EndDate != null)
                                productGroup.EndDate = value.EndDate.Value;

                            productGroup.Status = value.JobEventStatus;
                            // productGroup.Status = value.Status;

                            if (value.Created != null)
                                productGroup.Created = value.Created.Value;

                            productGroup.JobEventStatusColour = value.JobEventStatusColour;

                            productGroup.CssClass = ProductGroupColor(value.Product_Group);
                            grplist.Add(productGroup);
                        }

                        var distinctrowItems = grplist.Distinct(new DistinctItemComparer());

                        jobTrack.ProductGroup = distinctrowItems.ToList();

                        joblst.Add(jobTrack);
                    }
                }
            }
            return joblst;

        }

        /// <summary>
        /// below Code is used for removing duplicate rows from the list 
        /// var distinctrowItems = grplist.Distinct(new DistinctItemComparer());
        /// </summary>
        class DistinctItemComparer : IEqualityComparer<TrackJobProductGroup>
        {

            public bool Equals(TrackJobProductGroup x, TrackJobProductGroup y)
            {
                return x.StartDate == y.StartDate &&
                    x.EndDate == y.EndDate &&
                    x.ProductGroupName == y.ProductGroupName;
            }

            public int GetHashCode(TrackJobProductGroup obj)
            {
                return obj.StartDate.GetHashCode() ^
                    obj.EndDate.GetHashCode() ^
                    obj.ProductGroupName.GetHashCode();

            }
        }

        public void DeleteJob(int jobid)
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                int orgId = currentUser.OrgId.Value;
                var userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                _repository.DeleteJob(orgId, userid, userTypeName, compamyid, jobid);
            }
        }
        public JobTrackingModel GetjobById(string jobid)
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                int orgId = currentUser.OrgId.Value;
                var userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                var joblist = _repository.GetAllJobs(orgId, userid, userTypeName, compamyid, string.Empty).ToList();
                var joblistgroup = joblist.GroupBy(m => m.Row_Id).ToList();

                var lst = joblistgroup.Where(m => m.Key == int.Parse(jobid)).FirstOrDefault();

            }
            return null;
        }

        public IList<TrackingJobs> GetAllJobsBySp()
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (currentUser.OrgId != null)
                {
                    int orgId = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    var userType = currentUser.UserType;
                    string userTypeName = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    var joblist = _repository.GetAllJobs(orgId, userid, userTypeName, compamyid, string.Empty).ToList();
                    return joblist;
                }
            }

            return null;

        }

        private string ProductGroupColor(string productGroup)
        {

            string color = string.Empty;
            switch (productGroup)
            {
                case "Day Photography":
                    color = "label-warning";
                    break;
                case "Dusk Photography":
                    color = "label-info";
                    break;
                case "Video and Image tours":
                    color = "label-success";
                    break;
                case "Floor Plans/Land-boxes":
                    color = "label-floor";
                    break;
                case "UAV Drone/Aerial Photography":
                    color = "abel-warning";
                    break;
                case "Package":
                    color = "label-info";
                    break;
                case "Copy Writing":
                    color = "label-success";
                    break;

            }

            return color;
        }

        public IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int? job_Id)
        {

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                int orgId = currentUser.OrgId.Value;
                var userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }


                var JobFolders = _repository.SelectJobAttachmentFolders(orgId, userid, userTypeName, compamyid, job_Id).ToList();
                return JobFolders;

            }
            return null;

        }

        public void InsertJobAttachment(string jobid, string groupType, string folderName, HttpPostedFileBase file, string tags, string filePath, string ThumbImgPath, int Selected=0, int Row_id = 0, int isDeleted = 0)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    if (file != null && !string.IsNullOrEmpty(jobid))
                    {
                        int orgid = currentUser.OrgId.Value;
                        int userid = currentUser.Row_Id;
                        string userType = currentUser.UserType1.Name;
                        int? compamyid = null;
                        if (currentUser.UserType == 3)
                        {
                            compamyid = currentUser.CompanyId;
                        }
                        int jobId = int.Parse(jobid);


                        // byte[] data;
                        //const int width = 300;
                        //const int height = 300;
                        //using (var srcImage = System.Drawing.Image.FromStream(file.InputStream))
                        //using (var newImage = new Bitmap(width, height))
                        //using (var graphics = Graphics.FromImage(newImage))
                        //using (var stream = new MemoryStream())
                        //{
                        //    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        //    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        //    graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height));
                        //    newImage.Save(stream, ImageFormat.Png);
                        //    data = stream.ToArray();
                        //}


                        string fileName = Path.GetFileName(file.FileName);
                        string fileExt = Path.GetExtension(file.FileName);
                        int fileSize = file.ContentLength;
                        string folder = folderName;

                        //byte[] file = data;

                        //_repository.InsertJobAttachment(orgid, userid, userType, compamyid, jobId, fileName, fileExt, fileSize, data, groupType, tags, folderName);
                        _repository.InsertJobAttachment(orgid, userid, userType, compamyid, jobId, fileName, fileExt, fileSize, filePath, groupType, tags, folderName, ThumbImgPath, Selected, Row_id, isDeleted);


                        //InsertJobAttachment(int orgid, int userid, string userType, int companyid, int jobid, string fileName, string fileExt, int fileSize, byte[] file, string grouptype, string tags, string folder);


                        // _repository.InsertJobAttachment();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }

            //void InsertJobAttachment(int orgid, int userid, string userType, int companyid, int jobid, string fileName, string fileExt, int fileSize, byte file,string grouptype,string tags,string folder);
            //_repository.InsertJobAttachment();
        }

        public void InsertJobAttachment(string jobid, string groupType, string folderName, string fileName, string fileExt, int fileSize, string tags, string filePath, string OriginalFileName, string ThumbImgPath, int Row_id = 0, int isDeleted = 0, int Selected=0)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    if (!string.IsNullOrEmpty(jobid))
                    {
                        int orgid = currentUser.OrgId.Value;
                        int userid = currentUser.Row_Id;
                        string userType = currentUser.UserType1.Name;
                        int? compamyid = null;
                        if (currentUser.UserType == 3)
                        {
                            compamyid = currentUser.CompanyId;
                        }
                        int jobId = int.Parse(jobid);

                        _repository.InsertJobAttachment(orgid, userid, userType, compamyid, jobId, OriginalFileName, fileExt, fileSize, filePath, groupType, tags, folderName, ThumbImgPath, Selected);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }

            //void InsertJobAttachment(int orgid, int userid, string userType, int companyid, int jobid, string fileName, string fileExt, int fileSize, byte file,string grouptype,string tags,string folder);
            //_repository.InsertJobAttachment();
        }

        public void UpdateJobAttachment(string TagSelected, string FileName, string FilePath, string Job_id,int isSelected=0 ,int Row_id = 0, int isDeleted = 0)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {

                    int orgid = currentUser.OrgId.Value;
                    int userid = currentUser.Row_Id;
                    string userType = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }
                    if (!string.IsNullOrEmpty(Job_id))
                    {

                        int jobId = int.Parse(Job_id);
                        _repository.InsertJobAttachment(orgid, userid, userType, compamyid, jobId, FileName, "", 0, FilePath, "", TagSelected, string.Empty, string.Empty,isSelected, Row_id);
                    }
                    else if (isDeleted > 0)
                    {
                        _repository.InsertJobAttachment(orgid, userid, userType, compamyid, 0, FileName, "", 0, FilePath, "", TagSelected, string.Empty, string.Empty,isSelected, Row_id, isDeleted);
                    }
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public IList<GetJobAttachments> GetJobAttachmentsForMail(int? groupType, int? job_Id, string Tags = "")
        {
            //var currentUser = UserManager.Current();
            //if (currentUser != null)
            //{
            int pagesize = 15; // we are setting page size 15 
            int orgId = 825;
            var userid = 1111;
            var userType = "Super Admin";
            string userTypeName = "";
            int? compamyid = null;
            //if (currentUser.UserType == 3)
            //{
            //    compamyid = currentUser.CompanyId;
            //}


            var JobJobAttachment = _repository.GetJobAttachments(orgId, userid, userTypeName, compamyid, groupType, job_Id, Tags).ToList();
            return JobJobAttachment;

            // }
            // return null;
        }

        public IList<GetJobAttachments> GetJobAttachments(int? groupType, int? job_Id, string Tags = "")
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                int pagesize = 15; // we are setting page size 15 
                int orgId = currentUser.OrgId.Value;
                var userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }


                var JobJobAttachment = _repository.GetJobAttachments(orgId, userid, userTypeName, compamyid, groupType, job_Id, Tags).ToList();
                return JobJobAttachment;

            }
            return null;
        }

        public IList<SelectJobAttachmentTypes> GetJobAttachmentTypes(int groupid)
        {

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                int pagesize = 15; // we are setting page size 15 
                int orgId = currentUser.OrgId.Value;
                var userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }

                var JobAttachmentTypes = _repository.GetJobAttachmentTypes(orgId, userid, userTypeName, compamyid, groupid).ToList();
                return JobAttachmentTypes;

            }
            return null;
        }

        public void InsertJobCopy(JobCopyModel model)
        {

            var currentUser = UserManager.Current();
            try
            {

                if (currentUser != null)
                {

                    int titleWordCount = int.Parse(model.TitleWordCount);
                    int bodyWordCount = int.Parse(model.BodyWordCount);
                    string status = model.Status;

                    _repository.InsertJobCopy(currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value, model.Jobid, model.Type, model.Title, titleWordCount, model.JobBody, bodyWordCount, string.Empty);
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }


        }

        public IList<SelectJobCopyType> GetAllJobCopyTypes()
        {
            try
            {

                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    return _repository.GetAllJobCopyTypes(currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value);

                }



            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return null;
        }

        public IList<SelectJobCopy> GetAllJobCopy(int jobid)
        {

            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    return _repository.GetAllJobCopy(currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value, jobid);

                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        public void UpdateJobCopy(JobCopyModel model)
        {
            var currentUser = UserManager.Current();
            try
            {

                if (currentUser != null)
                {

                    int titleWordCount = int.Parse(model.TitleWordCount);
                    int bodyWordCount = int.Parse(model.BodyWordCount);
                    string status = model.Status;

                    _repository.UpdateJobCopy(currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value, model.RowId, model.Type, model.Title, titleWordCount, model.JobBody, bodyWordCount, string.Empty);
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

        }

        public void DeleteJobCopy(int jobCopyRowid)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    _repository.DeleteJobCopy(currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value, jobCopyRowid);

                }

            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

        }

        public void UpdateJobAttachmentSelected(int rowid, bool isSelected)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    int userid = currentUser.Row_Id;
                    string userType = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    _repository.UpdateJobAttachmentSelected(orgid, userid, userType, compamyid, rowid, isSelected);
                    //void UpdateJobAttachmentSelected(int orgid, int userid, string userType, int companyid, int jobRowid, bool IsSelected);
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
            }

        }

        public void UpdateJobAttachmentComments(int Row_Id, string Comments)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {

                    int orgid = currentUser.OrgId.Value;
                    int userid = currentUser.Row_Id;
                    string userType = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }
                    _repository.UpdateJobAttachmentComments(orgid, userid, userType, compamyid, Row_Id, Comments);
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }
        public void UpdateFolderLock(string jobId,int RowId)
        {
            try
            {
                _repository.UpdateFolderLock(Convert.ToInt32(jobId),RowId);
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public IList<GetJobCommentsById> GetJobCommentsById(int? Row_Id)
        {

            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    return _repository.GetJobCommentsById(currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value, Row_Id.Value);

                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        public IList<GetFoldersAttachmentCount> GetFoldersAttachmentCount(int? Job_id)
        {

            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    return _repository.GetFoldersAttachmentCount(Job_id, currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value);

                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        public IList<GetUserCommentsbyJobId> GetUserCommentsbyJobId(int? Job_id)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    return _repository.GetUserCommentsbyJobId(Job_id, currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value);
                }

            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public IList<GetUserCommentsbyJobId> GetUserCommentsbyJobIdForMail(int? Job_id)
        {
            try
            {
                return _repository.GetUserCommentsbyJobId(Job_id, 825, 1111, "Super Admin", 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //====To get Job Attachment and comment
        public IList<SelectJobAttachmentTemplate_Result> SelectJobAttachmentTemplate(int? groupType, int? job_Id)
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                int pagesize = 15; // we are setting page size 15 
                int orgId = currentUser.OrgId.Value;
                var userid = currentUser.Row_Id;
                var userType = currentUser.UserType;
                string userTypeName = currentUser.UserType1.Name;
                int? compamyid = null;
                if (currentUser.UserType == 3)
                {
                    compamyid = currentUser.CompanyId;
                }


                var JobJobAttachment = _repository.SelectJobAttachmentTemplate(orgId, userid, userTypeName, compamyid, job_Id,Convert.ToString(groupType)).ToList();
                return JobJobAttachment;

            }
            return null;
        }
        public void UpdateJobEmail_Notification(int JobId, string Email)
        {
            try
            {
                _repository.UpdateJobEmail_Notification(JobId, Email);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

        }

        public void UpdateJobStatus(int jobId)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {

                    int orgid = currentUser.OrgId.Value;
                    int userid = currentUser.Row_Id;
                    string userType = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }
                    _repository.UpdateJobstatus(jobId, orgid, userid);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

        }

        public void InsertJobAnnotation(int Row_Id, string AnnotationText)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {

                    int orgid = currentUser.OrgId.Value;
                    int userid = currentUser.Row_Id;
                    string userType = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }
                    _repository.InsertJobAnnotation(orgid, userid, userType, compamyid, Row_Id, AnnotationText);
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public IList<GetJobAnnotationById> GetJobAnnotationbyFileId(int? FileId)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {
                    return _repository.GetJobAnnotationbyFileId(FileId, currentUser.OrgId.Value, currentUser.Row_Id, currentUser.UserType.Value.ToString(), currentUser.CompanyId.Value);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
        public IList<GetJobAnnotationById> GetJobAnnotationbyFileIdForMail(int? FileId)
        {
            try
            {

                return _repository.GetJobAnnotationbyFileId(FileId, 825, 1111, "Super Admin", 0);

            }
            catch (Exception)
            {
                throw;
            }

            //   return null;
        }
        public void UpdateJobAttachmentTags(string TagSelected, int? Row_id, int? Job_id)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {

                    int orgid = currentUser.OrgId.Value;
                    int userid = currentUser.Row_Id;
                    string userType = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }
                    _repository.UpdateJobAttachmenTags(TagSelected, Row_id, orgid, Job_id);
             
                }
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public void UpdateSelectedImageConfirmed(string RowId, int jobId)
        {
            try
            {
                _repository.UpdateConfirmImageSelected(RowId,jobId);
            }

            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

    }
}