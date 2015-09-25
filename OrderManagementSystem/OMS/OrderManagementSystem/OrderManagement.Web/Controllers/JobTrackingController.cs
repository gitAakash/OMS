
using System.Linq;
using System.Web;
using System.Linq;
using System.Web.Mvc;

using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;
using System.Collections.Generic;
using System.IO;
using OrderManagement.Web.Helper.Utilitties;
using System;
using Kendo.Mvc.UI;
using System.Web.Script.Serialization;
using System.Threading;

namespace OrderManagement.Web.Controllers
{
    public class JobTrackingController : Controller
    {
        public string ErrorMsg = string.Empty;
        //
        // GET: /JobTracking/
        private readonly IJobTrackingService _repository;

        public JobTrackingController()
        {
            var trackjob = new JobTrackingRepository();
            _repository = new JobTrackingService(trackjob);
        }
       // [HttpGet]
        public ActionResult Index(string search)
        {
            var joblist = _repository.GetAllJobs(search);
            //  return View("About");

            //return PartialView(joblist);
           return PartialView("Controls/JobTracking/_JobList", joblist);
        }

        public ActionResult ProductPartial()
        {
            //    List<Product> model = Product.getProduct();
            return PartialView("_FileUploader");
        }

        public ActionResult ViewJobTracking(string jobid)
        {
            var joblist = _repository.GetAllJobs(string.Empty);
            if (joblist.Count > 0)
            {
                var job = joblist.Where(m => m.JobId == int.Parse(jobid)).FirstOrDefault();
                return PartialView("Controls/JobTracking/_ViewJobs", job);
            }
            return null;
        }


        public ActionResult DeleteJobs(string jobid)
        {
            if (!string.IsNullOrEmpty(jobid))
            {
                _repository.DeleteJob(int.Parse(jobid));
            }

            var joblist = _repository.GetAllJobs(string.Empty);
            return PartialView("Controls/JobTracking/_JobList", joblist);
        }


        public ActionResult ViewEvents(string jobid)
        {
            var joblist = _repository.GetAllJobsBySp().Where(m => m.Row_Id == int.Parse(jobid)).ToList();
            
            return PartialView("Controls/JobTracking/_Events", joblist);
        }

        public ActionResult ViewGallery(string jobid)
        {

            //var currentuser = UserManager.Current();
            //if (currentuser != null)
            //{
            //    int OrgID = Convert.ToInt32(currentuser.OrgId);
            //    int UserID = Convert.ToInt32(currentuser.Row_Id);
            //    int UserType = Convert.ToInt32(currentuser.UserType); //1= admin 2- staff, 3-- client
            //    int UserCompanyId = Convert.ToInt32(currentuser.CompanyId);
            //}

            GalleryModel objGalleryModel = new GalleryModel();
            objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders().ToList();

            int groupid = objGalleryModel.GalleryFolders.Take(1).FirstOrDefault().ROW_ID;
            var TagList = objGalleryModel.GalleryFolders.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.FOLDER });

            foreach (var item in TagList)
            {
                string myString = item.TAGS;
                string[] strArray = myString.Split(';');

                List<TagInfo> lstTagInfo = new List<TagInfo>();

                foreach (string obj in strArray)
                {
                    TagInfo objtagInfo = new TagInfo();

                    objtagInfo.Row_Id = item.ROW_ID;
                    objtagInfo.Folder = item.FOLDER;
                    objtagInfo.TagName = obj;
                    lstTagInfo.Add(objtagInfo);
                }

                objGalleryModel.TagList = lstTagInfo;

            }

            objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(groupid, Convert.ToInt16(jobid)); 

          //  var attch = _repository.SelectJobAttachment();
        
          //  int PageSize = 15;
          //  int page;
          //  page = 0;
          ////  objGalleryModel.GalJobAttachmentlst = attch.ToList();
          //  objGalleryModel.GalJobAttachmentlst = attch.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
          //  objGalleryModel.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)attch.Count() / PageSize));
          //  objGalleryModel.CurrentPage = page;

            return PartialView("Controls/JobTracking/_Gallery", objGalleryModel);
            //   return PartialView("Controls/JobTracking/_Uploadfiles");
        }

        //public ActionResult Save(IEnumerable<HttpPostedFileBase> files, string JobId, string groupType, string groupname, string selectedTags) 
        //{
        //   if (files != null)
        //        {
        //            foreach (var file in files)
        //            {
        //                var fileName = Path.GetFileName(file.FileName);
        //                string strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
        //                strFolder = strFolder + "/" + JobId + "/" + groupname + "/";
        //                DirectoryInfo diPath = new DirectoryInfo(strFolder);
        //                strFolder = strFolder + fileName.ToString();
        //                if (System.IO.File.Exists(strFolder))
        //                {
        //                    // this will rutund the exist file
        //                }
        //                else
        //                {
        //                    _repository.InsertJobAttachment(JobId, groupType, groupname, file, selectedTags, strFolder);
        //                    file.SaveAs(strFolder);
        //                    diPath.Create();
        //                }
        //            }
        //        }

        //    return Content("");
        //}
       
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files, string JobId, string groupType, string groupname,string selectedTags) //,string JobId, string groupType, string groupname)
        {
            // The Name of the Upload component is "files"
            //if (files != null)
            //{
            //    foreach (var file in files)
            //    {
            //        // Some browsers send file names with full path.
            //        // We are only interested in the file name.
            //          var fileName = Path.GetFileName(file.FileName);

            //        string strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];

            //          string strphysicalPath = "~/" + strFolder + "/" + JobId + "/" + groupname + "/";
                   
            //          var physicalPath = Path.Combine(Server.MapPath(strphysicalPath), fileName);

            //         // var path = Server.MapPath("~/Content/users/" + "manoj");

            //       // Attachments\12345\PhotoGraphy\Room.Jpg

            //          var path = Server.MapPath("~/" + strFolder + "/" + JobId + "/" + groupname + "/");

            //          Directory.CreateDirectory(path);
                      
            //        strphysicalPath = strphysicalPath + fileName;

            //          // Attachments\12345\PhotoGraphy\Room.Jpg
            //        _repository.InsertJobAttachment(JobId, groupType, groupname, file, selectedTags, strphysicalPath);                    
            //        //   The files are not actually saved in this demo
            //          file.SaveAs(physicalPath);
                     
            //    }
            //}

            try
            {


                if (files != null)
                {
                    foreach (var file in files)
                    {
                        string strImgShowingRepositoryUrl = string.Empty;
                        string strFolder= string.Empty;

                        var fileName = Path.GetFileName(file.FileName);
                        strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];

                        if (groupname.Contains("/"))
                        {
                            groupname = groupname.Replace("/", "-");
                        }
                        
                        strFolder = strFolder + "/" + JobId + "/" + groupname + "/";
                        string FilePath = strFolder;
                        strImgShowingRepositoryUrl= System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];
                       
                            
                        strFolder = strFolder + fileName.ToString();
                       
                        if (System.IO.File.Exists(strFolder))
                        {
                           // rename file 

                            string strRandomFileName = Path.GetRandomFileName(); //This method returns a random file name of 11 characters
                            string FileExtension= System.IO.Path.GetExtension(file.FileName);
                            string FileName = System.IO.Path.GetFileName(file.FileName);
                            int fileSize = file.ContentLength;
                         
                            strRandomFileName = strRandomFileName.Replace(".", "") + FileExtension ;
                            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + JobId + "/" + groupname + "/" + strRandomFileName.ToString();
                          
                            strFolder = string.Empty;
                            strFolder = FilePath + strRandomFileName;
                            file.SaveAs(strFolder);

                            _repository.InsertJobAttachment(JobId, groupType, groupname, strRandomFileName, FileExtension, fileSize, selectedTags, strImgShowingRepositoryUrl, FileName);
                           
                           // this will rutund the exist file
                        }
                        else
                        {
                            DirectoryInfo diPath = new DirectoryInfo(FilePath);
                             diPath.Create();
                             strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + JobId + "/" + groupname + "/" + fileName.ToString();
                             file.SaveAs(strFolder);
                             _repository.InsertJobAttachment(JobId, groupType, groupname, file, selectedTags, strImgShowingRepositoryUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region For Error

                string ErrorMsg = ex.Message;
                string Error = ErrorMsg;



                if (!string.IsNullOrEmpty(ErrorMsg))
                {

                    switch (Error)
                    {
                        case "FileExist":
                            return Json(new DataSourceResult
                            {
                                // Errors = "You cannot change the organizer of an instance."
                                Errors = "CustomError400"
                            });

                        case "CustomError401":
                            return Json(new DataSourceResult
                            {
                                //Errors = "You cannot turn an instance of a recurring event into a recurring event itself."
                                Errors = "CustomError401"
                            });


                        case "Null_Event":
                            return Json(new DataSourceResult
                            {
                                Errors = "Null_Event"
                            });

                        //default:
                        //         Logger(ex.InnerException.Message);
                        //         return Json(new DataSourceResult
                        //    {

                        //        Errors = string.Empty
                        //    });
                    }
                }
                else
                {
                    return Json(new DataSourceResult
                    {
                        Errors = ""
                    });
                }
                #endregion
            }
            return Content("");
        }
        
        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult QuickAssign()
        {
            return PartialView("Controls/JobTracking/_FileUploader");
        }

        public ActionResult Create()
        {
            //  Department objDept = new Department();
            //return PartialView("CreateDepartment", objDept);
            return PartialView("Controls/JobTracking/_FileUploader");

        }

        public ActionResult FolderContent(string folderName, string groupid, string job_Id)
        {
            try
            {
                GalleryModel objGalleryModel = new GalleryModel();
                
                objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders().ToList();
                var TagList = objGalleryModel.GalleryFolders.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.FOLDER });
                
                objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(Convert.ToInt32(groupid), Convert.ToInt32(job_Id));
                
                foreach (var item in TagList)
                {
                    string strTag = item.TAGS;
                    List<TagInfo> lstTagInfo = new List<TagInfo>();
                    if (!string.IsNullOrEmpty(strTag))
                    {
                        string[] strTagArray = strTag.Split(';');

                        foreach (string obj in strTagArray)
                        {
                            TagInfo objtagInfo = new TagInfo();
                            objtagInfo.Row_Id = item.ROW_ID;
                            objtagInfo.Folder = item.FOLDER;
                            objtagInfo.TagName = obj;
                            lstTagInfo.Add(objtagInfo);
                        }
                    }
                    //else
                    //{
                    //    TagInfo objtagInfo = new TagInfo();

                    //    objtagInfo.Row_Id = 0;
                    //    objtagInfo.Folder = "test";
                    //    objtagInfo.TagName = string.Empty;
                    //    lstTagInfo.Add(objtagInfo);
                    //}

                    objGalleryModel.TagList = lstTagInfo;

                  //  var serializer = new JavaScriptSerializer();
                 //   ViewBag.TagList = serializer.Serialize(objGalleryModel.TagList);
                }
                             
              //  var attch = _repository.SelectJobAttachment(1);

              //  int PageSize = 15;
              //  int page;
              //  page = 0;
              ////  objGalleryModel.GalJobAttachmentlst = attch.ToList();
              //     objGalleryModel.GalJobAttachmentlst = attch.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
              //  objGalleryModel.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)attch.Count() / PageSize));
              //  objGalleryModel.CurrentPage = page;

                return PartialView("Controls/JobTracking/_GalleryList", objGalleryModel);
            }

            catch (Exception)
            {
                return PartialView("Controls/JobTracking/_FileUploader");
            }
        }

        public ActionResult TagBasisSearchContent(string folderName, string groupid, string job_Id, bool ShowExtwiseGalleryFiles)
        {
            try
            {
                GalleryModel objGalleryModel = new GalleryModel();
              //  objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders().ToList();
                objGalleryModel.GalleryFolders = null;
                objGalleryModel.TagList = null;

                // Added "Extension" Keyword for showing the result when user Click on the Top Search link i.e All, Document, Audio Image"
                
                if (ShowExtwiseGalleryFiles)
                {
                    folderName = "Extension " + folderName; 
                }
                
                objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(Convert.ToInt32(groupid), Convert.ToInt32(job_Id), folderName);
                return PartialView("Controls/JobTracking/_GalleryList", objGalleryModel);
            }

            catch (Exception)
            {
               // return PartialView("Controls/JobTracking/_FileUploader");
                return PartialView("Controls/JobTracking/_GalleryList");
            }
        }
        
        public ActionResult ImageListing(int page)
        {
           // var attch = _repository.SelectJobAttachment(page);
            var JobAttachmentModel = new JobAttachmentModel();
            int PageSize = 15;
            // var obj = _userService.GetAllOrderAttachment();

            //JobAttachmentModel.JobAttachmentlst = attch.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
            //JobAttachmentModel.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)attch.Count() / PageSize));
            //JobAttachmentModel.CurrentPage = page;

            return PartialView("Controls/JobTracking/_GalleryList", JobAttachmentModel);
        }

        public JsonResult GetFileTypesDataJson(string groupid = "1")
        {


         //   var data1 = _repository.GetJobAttachmentTypes().ToList();

            var data = _repository.GetJobAttachmentTypes(Convert.ToInt32(groupid)).Select(p => new
            {
                AllowedFileExtensions = p.AllowedFileExtension,
                MaxFileSize = p.MaxFileSize
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTagInfoDataJson(string groupid = "1")
        {
            var AllGalleryFoldersData = _repository.SelectJobAttachmentFolders().ToList();
            var TagList = AllGalleryFoldersData.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.FOLDER });

            List<TagInfo> TagInfo = new List<TagInfo>();

            foreach (var item in TagList)
            {
                string strTag = item.TAGS;
                List<TagInfo> lstTagInfo = new List<TagInfo>();
                if (!string.IsNullOrEmpty(strTag))
                {
                    string[] strTagArray = strTag.Split(';');

                    foreach (string obj in strTagArray)
                    {
                        TagInfo objtagInfo = new TagInfo();
                        objtagInfo.Row_Id = item.ROW_ID;
                        objtagInfo.Folder = item.FOLDER;
                        objtagInfo.TagName = obj;
                        lstTagInfo.Add(objtagInfo);
                    }
                }
                TagInfo = lstTagInfo;
            }

            var TagListInfo = TagInfo.Select(p => new
            {
                
                TagName = p.TagName,
                GroupId=p.Row_Id
            });
            return Json(TagListInfo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateGallery(string Row_Id, string TagSelected, string NewFileName, string Job_id, string oldFileName, string groupname)
        {
            JsonInfo objJsonInfo = new JsonInfo();

            if ((System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"] != null) && (System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"] != null))
            {
                try
                {
                    string strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];

                    if (groupname.Contains("/"))
                    {
                        groupname = groupname.Replace("/", "-");
                    }

                    strFolder = strFolder + "/" + Job_id + "/" + groupname + "/";
                    DirectoryInfo diPath = new DirectoryInfo(strFolder);

                    string CdnURl = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];
                    string strImgShowingRepositoryUrl = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];

                    strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + Job_id + "/" + groupname + "/" + oldFileName.ToString();

                    string oldFileURL = strFolder + oldFileName.ToString();
                    string NewFileURL = strFolder + NewFileName.ToString();

                    string NewImgFilePathUrl = CdnURl + Job_id + "/" + groupname + "/" + NewFileName.ToString();

                    Logger("UpdateJobAttachment:" + Environment.NewLine + "TagSelected- " + TagSelected + Environment.NewLine + " oldFileURL-" + oldFileURL + Environment.NewLine + " NewFileURL-" + NewFileURL + Environment.NewLine + " Job_id-" + Job_id + Environment.NewLine + " Row_Id-" + Convert.ToInt16(Row_Id));

                    objJsonInfo.NewFileName = NewFileName;
                    objJsonInfo.OldFileName = oldFileName;

                    try
                    {
                        strFolder = strFolder + oldFileName.ToString();
                        if (System.IO.File.Exists(strFolder))
                        {
                            if (NewFileName.Equals(oldFileName))
                            {
                                _repository.UpdateJobAttachment(TagSelected, NewFileName, NewImgFilePathUrl, Job_id, Convert.ToInt16(Row_Id));
                                objJsonInfo.Save = true;
                            }
                            else
                            {
                                // this will rutund the exist file
                                if (!System.IO.File.Exists(NewFileURL))
                                {
                                    System.IO.File.Move(oldFileURL, NewFileURL); // Try to move
                                    _repository.UpdateJobAttachment(TagSelected, NewFileName, NewImgFilePathUrl, Job_id, Convert.ToInt16(Row_Id));
                                    objJsonInfo.Save = true;
                                    Logger("UpdateJobAttachment:End - " + NewFileName + " + file has been updated.");
                                }
                                else
                                {
                                    Logger("UpdateJobAttachment:End - " + NewFileName + " + File Name Exist.");
                                    objJsonInfo.FileNotSaveReason = "FileNameExist";
                                    return Json(objJsonInfo, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            throw new FileNotFoundException();

                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        Logger("Exception- FileNotFoundException:" + ex.Message);
                        objJsonInfo.FileNotSaveReason = ex.Message;
                        return Json(objJsonInfo, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    Logger("Exception- " + ex.Message);
                    objJsonInfo.FileNotSaveReason = ex.Message;
                    return Json(objJsonInfo, JsonRequestBehavior.AllowGet);
                }

                //string JsonReturn = "{folderName:'" + groupname + "',groupname:'" + groupname + "'}";
            }
            return Json(objJsonInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteGalleryFile(string Row_Id)
        {
          _repository.UpdateJobAttachment(string.Empty, string.Empty, string.Empty, string.Empty, Convert.ToInt16(Row_Id),1);
          return Json("File has been deleted", JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateJobAttachmentSelected(string rowId, bool isSelected)
        {
            _repository.UpdateJobAttachmentSelected(Convert.ToInt16(rowId), isSelected);
            return Json("SelectionUpdated", JsonRequestBehavior.AllowGet);
        }

        private void Logger(string logText)
        {
            using (StreamWriter tw = new StreamWriter("d:\\Logger.txt", true))
            {
                tw.WriteLine("----------------------------------------------------------------------------------------------");
                tw.WriteLine(logText);
                tw.WriteLine(DateTime.Now);
                tw.WriteLine("----------------------------------------------------------------------------------------------");
            }
        }


        [HttpPost]
        public ActionResult AddorUpdateCopyWriting(JobCopyModel model)
        {

            if (model.RowId != 0)
            {
                _repository.UpdateJobCopy(model);
            }
            else
            {
                _repository.InsertJobCopy(model);
            }

            if (model.Jobid!=0)
            {

                var jobCopylist = _repository.GetAllJobCopy(model.Jobid).ToList();
                return PartialView("Controls/JobTracking/_CopyWritingList", jobCopylist);
            }

            return null;
        }

        public ActionResult CopyWritingListing(string jobid)
        {
         
            if (!string.IsNullOrEmpty(jobid))
            {

                var jobCopylist = _repository.GetAllJobCopy(int.Parse(jobid)).ToList();
                return PartialView("Controls/JobTracking/_CopyWritingList", jobCopylist);
            }
            return null;
        }

        [HttpGet]
        public ActionResult ViewCopyWriting(string jobid, string jobCopyRowid)
        {
            var jobModel = new JobCopyModel();
            if (!string.IsNullOrEmpty(jobid))
            {

                var jobCopylist = _repository.GetAllJobCopy(int.Parse(jobid));
                if (jobCopylist.Count > 0)
                {
                    var jobCopy = jobCopylist.FirstOrDefault(m => m.Row_Id == int.Parse(jobCopyRowid));
                    if (jobCopy != null)
                    {
                        jobModel.Title = jobCopy.Title;
                        if (jobCopy.Job_Copy_Type_Id != null) jobModel.Type = jobCopy.Job_Copy_Type_Id.Value;
                        jobModel.JobBody = jobCopy.Body;
                        jobModel.RowId = jobCopy.Row_Id;
                        jobModel.TitleWordCount = jobCopy.TitleWordCount.ToString();
                        jobModel.BodyWordCount = jobCopy.BodyWordCount.ToString();

                    }
                }
            }

            jobModel.TypeList = _repository.GetAllJobCopyTypes().ToList();
            return PartialView("Controls/JobTracking/_CopyWriting", jobModel);



        }

        public ActionResult EditCopyWriting()
        {
             var jobModel = new JobCopyModel();
            jobModel.TypeList = _repository.GetAllJobCopyTypes().ToList();
            return PartialView("Controls/JobTracking/_CopyWriting", jobModel);
        }

        public ActionResult DeleteJobCopy(string jobid,string jobCopyRowid)
        {
            if (!string.IsNullOrEmpty(jobid) && !string.IsNullOrEmpty(jobCopyRowid))
            {
                _repository.DeleteJobCopy(int.Parse(jobCopyRowid));

                //var joblist = _repository.GetAllJobs(string.Empty);
                //if (joblist.Count > 0)
                //{

                //    var job = joblist.FirstOrDefault(m => m.JobId == int.Parse(jobid));
                //    return PartialView("Controls/JobTracking/_ViewJobs", job);
                //}

                if (!string.IsNullOrEmpty(jobid))
                {

                    var jobCopylist = _repository.GetAllJobCopy(int.Parse(jobid)).ToList();
                    return PartialView("Controls/JobTracking/_CopyWritingList", jobCopylist);
                }
            }


            return null;

        }

        public JsonResult CheckUniqeJobCopyType(string Type, string Jobid, string JobcopyRowid)
        {
            var result = new JsonResult();
            try
            {
                if (JobcopyRowid == "0" || string.IsNullOrEmpty(JobcopyRowid))
                {
                    if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Jobid))
                    {
                        int jobId = int.Parse(Jobid);
                        var jobCopylist = _repository.GetAllJobCopy(jobId).ToList();
                        if (jobCopylist.Count > 0)
                        {
                            var jobCopy =
                                jobCopylist.FirstOrDefault(
                                    m => m.Job_Id == jobId && m.Job_Copy_Type_Id == int.Parse(Type));
                            if (jobCopy != null)
                                result.Data = false;
                            else
                                result.Data = true;
                        }
                        else
                        {
                            result.Data = true;
                        }
                       
                    }
                }
                else
                {
                    result.Data = true;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        [HttpPost, FileDownload]
        public FilePathResult DownloadReport(string Row_Id, string JobId, string groupname, string FileName)
        {
            string strDestinationFolderPath = string.Empty;
            string strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
            string strImgShowingRepositoryUrl = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];

            //string strCreateFolder = "~/App_data/uploads/" + Jobid + "/" + groupname + "/";
            int Jobid = Convert.ToInt16(JobId);

            if (groupname.Contains("/"))
            {
                groupname = groupname.Replace("/", "-");
            }

            if (System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] != null)
            {
                strDestinationFolderPath = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + "/" + Jobid + "/" + groupname + "/";
            }
           
            strFolder = strFolder + "/" + Jobid + "/" + groupname + "/";
          
            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + Jobid + "/" + groupname + "/" + FileName.ToString();

            strFolder = strFolder + FileName.ToString();
            string strFileDestinationFolder;
            strFileDestinationFolder = string.Empty;
            strFileDestinationFolder = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + Jobid + "/" + groupname + "/" + FileName.ToString();

            if (System.IO.File.Exists(strFileDestinationFolder))
            {
                System.IO.File.Delete(strFileDestinationFolder);

                // Copy file from source to app folder [destination folder]
              
                System.IO.File.Copy(strFolder, strFileDestinationFolder);
                return File(strFileDestinationFolder, "application/jpg", string.Format(FileName, 0));
                // this will rutund the exist file
            }
            else
            {
                strFileDestinationFolder = string.Empty;
                strFileDestinationFolder = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + Jobid + "/" + groupname + "/";

                DirectoryInfo diPath = new DirectoryInfo(strFileDestinationFolder);
                diPath.Create();

                strFileDestinationFolder = strFileDestinationFolder + FileName.ToString();
                // Copy file from source to app folder [destination folder]
                System.IO.File.Copy(strFolder, strFileDestinationFolder);
                return File(strFileDestinationFolder, "application/jpg",string.Format(FileName,0));
            }
        }

        [HttpPost, FileDownload]
        public FilePathResult DownloadReport1(string Row_Id)
        {
            int Jobid = Convert.ToInt16(Row_Id);
            return GetReport(Jobid);
        }


        private FilePathResult GetReport(int id)
        {
            //simulate generating the report
            Thread.Sleep(3000);

            //only even file ids will work
            // if (id % 2 == 0)
            //the required cookie for jquery.fileDownload is written by the FileDownloadAttribute for all
            //result types that inherit from FileResult but could be done manually here if desired
            return File("~/App_data/uploads/mstestDesert.jpg", "application/jpg", string.Format("mstestDesert{0}.jpg", id));
            throw new Exception(string.Format("File Report{0}.pdf could not be found. \r\n\r\n NOTE: This is for demonstration purposes only, customErrors should always be enabled in a production environment.", id));
        }


        

    }

    public class FileExistException : ApplicationException
    {
        public FileExistException(string message)
            : base(message)
        {
        }
    }

}
