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
        IList<JobTrackingModel> GetAllJobs(string search);
        IList<TrackingJobs> GetAllJobsBySp();
        JobTrackingModel GetjobById(string jobid);
        IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders();
        void InsertJobAttachment(string jobis, string groupType, string folderName, HttpPostedFileBase file, string tags, string filePath, int Row_id = 0, int isDeleted = 0);
        void InsertJobAttachment(string jobid, string groupType, string folderName, string fileName, string fileExt, int fileSize, string tags, string filePath, string OriginalFileName, int Row_id = 0, int isDeleted = 0);
        
        IList<SelectJobAttachmentTypes> GetJobAttachmentTypes(int groupid);

        void UpdateJobAttachment(string TagSelected, string FileName,string FilePath, string Job_id, int Row_id = 0, int isDeleted = 0);
        IList<GetJobAttachments> GetJobAttachments(int? groupType, int? job_Id, string Tags="");
      
        void InsertJobCopy(JobCopyModel model);
        void UpdateJobCopy(JobCopyModel model);

        IList<SelectJobCopyType> GetAllJobCopyTypes();
        IList<SelectJobCopy> GetAllJobCopy(int jobid);
        void DeleteJobCopy(int jobCopyRowid);

        void UpdateJobAttachmentSelected(int rowId, bool isSelected);

        void DeleteJob(int jobid);

    }




    public class JobTrackingService : IJobTrackingService
    {
        private IJobTrackingRepository _repository;
        public JobTrackingService(IJobTrackingRepository repository)
        {
            _repository = repository;

        }

        public IList<JobTrackingModel> GetAllJobs(string search)
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

                    var joblist = _repository.GetAllJobs(orgid, userid, userTypeName, compamyid, search).ToList();
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
                            if (joblistdistinct.Created != null) jobTrack.Created = joblistdistinct.Created.Value;
                            if (joblistdistinct.Updated != null) jobTrack.Updated = joblistdistinct.Updated.Value;
                            if (joblistdistinct.StartDate != null) jobTrack.StartDate = joblistdistinct.StartDate.Value;
                            if (joblistdistinct.EndDate != null) jobTrack.EndDate = joblistdistinct.EndDate.Value;
                            jobTrack.Description = joblistdistinct.Description;
                            jobTrack.JobEventStatus = joblistdistinct.JobEventStatus;
                          


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

            public bool Equals(TrackJobProductGroup x, TrackJobProductGroup y )
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

        public IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders()
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


                var JobFolders = _repository.SelectJobAttachmentFolders(orgId, userid, userTypeName, compamyid).ToList();
                return JobFolders;

            }
            return null;

        }

        public void InsertJobAttachment(string jobid, string groupType, string folderName, HttpPostedFileBase file, string tags, string filePath, int Row_id = 0, int isDeleted = 0)
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
                        string fileExt = Path.GetExtension(file.FileName); ;
                        int fileSize = file.ContentLength;
                        string folder = folderName;

                       //byte[] file = data;

                        //_repository.InsertJobAttachment(orgid, userid, userType, compamyid, jobId, fileName, fileExt, fileSize, data, groupType, tags, folderName);
                        _repository.InsertJobAttachment(orgid, userid, userType, compamyid, jobId, fileName, fileExt, fileSize, filePath, groupType, tags, folderName);


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


        public void InsertJobAttachment(string jobid, string groupType, string folderName, string fileName, string fileExt, int fileSize, string tags, string filePath, string OriginalFileName, int Row_id = 0, int isDeleted = 0)
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

                        _repository.InsertJobAttachment(orgid, userid, userType, compamyid, jobId, OriginalFileName, fileExt, fileSize, filePath, groupType, tags, folderName);
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


       
        public void UpdateJobAttachment(string TagSelected,string FileName,string FilePath, string Job_id, int Row_id = 0, int isDeleted = 0)
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
                            _repository.InsertJobAttachment(orgid, userid, userType, compamyid, jobId, FileName,"", 0, FilePath, "", TagSelected, string.Empty, Row_id);
                        }
                        else if (isDeleted > 0)
                        {
                            _repository.InsertJobAttachment(orgid, userid, userType, compamyid, 0, FileName, "", 0, FilePath, "", TagSelected, string.Empty, Row_id,isDeleted);
                        }
                    }
                }
            
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

           
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
                if (currentUser!=null)
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

    }
}