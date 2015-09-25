using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Kendo.Mvc.Extensions;
using OrderManagement.Web.Helper;
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
using System.IO.Compression;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Globalization;
using Font = System.Drawing.Font;
using Image = iTextSharp.text.Image;
using Path = System.IO.Path;
using Rectangle = iTextSharp.text.Rectangle;
using pdfFont = iTextSharp.text.Font;
using System.Web.UI;
namespace OrderManagement.Web.Controllers
{
    public class JobTrackingController : Controller
    {
        public string ErrorMsg = string.Empty;
        //
        // GET: /JobTracking/
        private readonly IJobTrackingService _repository;
        private readonly IUserService _userService;
        private ISchedulerService _scheduler;
        private SchedulerRepository _schedulerrapo;
        public JobTrackingController()
        {
            var userrapo = new UserRepository();
            _userService = new UserService(userrapo);
            var trackjob = new JobTrackingRepository();
            _repository = new JobTrackingService(trackjob);
            _schedulerrapo = new SchedulerRepository();
            _scheduler = new SchedulerService(_schedulerrapo);
        }
        // [HttpGet]
        public ActionResult Index(string search)
        {
            var joblist = _repository.GetAllJobs(search);
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
                job.Email_Notify = new List<string>();
                if (!string.IsNullOrEmpty(job.Email_Notification))
                {
                    string[] Email = job.Email_Notification.Split(',');
                    for (int idex = 0; idex < Email.Count(); idex++)
                    {
                        if (!string.IsNullOrEmpty(Email[idex]))
                            job.Email_Notify.Add(Email[idex]);
                    }
                }
                return PartialView("Controls/JobTracking/_ViewJobs", job);
            }
            return null;
        }

        public ActionResult DeleteEmailNotifyResult(int jobId, string email_Notification)
        {
            try
            {
                _repository.UpdateJobEmail_Notification(jobId, email_Notification);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DeleteJobs(string jobid)
        {
            if (!string.IsNullOrEmpty(jobid))
            {
                _repository.DeleteJob(int.Parse(jobid));
            }

            var joblist = _repository.GetAllJobs(string.Empty,true);
            return PartialView("Controls/JobTracking/_JobList", joblist);
        }

        public ActionResult ViewEvents(string jobid)
        {

            var currentUser = UserManager.Current();
            if (currentUser.UserType == 3)
            {
                var job = _repository.GetAllJobs(string.Empty);
                var joblists = job.Where(m => m.JobId == int.Parse(jobid)).FirstOrDefault();
                return PartialView("Controls/JobTracking/_ClientEvent", joblists);
            }
            else
            {
                var joblist = _repository.GetAllJobsBySp().Where(m => m.Row_Id == int.Parse(jobid)).ToList();
                return PartialView("Controls/JobTracking/_Events", joblist);
            }

        }

        //public ActionResult EmailViewJobMail(int ImageId) {

        //    return View("JobImage");
        //}

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
            objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders(Convert.ToInt32(jobid)).ToList();
            var lst = Swap(objGalleryModel.GalleryFolders, 1, 2);
            objGalleryModel.GalleryFolders = lst;

            // var TagList = objGalleryModel.GalleryFolders.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.Folder });

            //foreach (var item in TagList)
            //{
            //    string myString = item.TAGS;

            //    if (item.TAGS != null)
            //    {
            //        string[] strArray = myString.Split(';');

            //        List<TagInfo> lstTagInfo = new List<TagInfo>();

            //        foreach (string obj in strArray)
            //        {
            //            TagInfo objtagInfo = new TagInfo();

            //            objtagInfo.Row_Id = item.ROW_ID;
            //            objtagInfo.Folder = item.Folder;
            //            objtagInfo.TagName = obj;
            //            lstTagInfo.Add(objtagInfo);
            //        }

            //        objGalleryModel.TagList = lstTagInfo;
            //    }
            //}

            int ImagesGroupid = objGalleryModel.GalleryFolders.ToArray()[0].ROW_ID;
            objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(ImagesGroupid, Convert.ToInt16(jobid));

            if (objGalleryModel.GalleryFolders != null && objGalleryModel.GalleryFolders.Count >= 1)
            {
                int floorPlanGroupid = objGalleryModel.GalleryFolders.ToArray()[1].ROW_ID;
                var floorPlanImages = _repository.GetJobAttachments(floorPlanGroupid, Convert.ToInt16(jobid));
                foreach (var a in floorPlanImages)
                {
                    objGalleryModel.JobAttachmentlist.Add(a);
                }
            }
            //if (objGalleryModel.JobAttachmentlist.Count > 0)
            //{
            //    ViewBag.ImgCount = objGalleryModel.JobAttachmentlist.Count;

            //    int SelectedImgCount = 0;
            //    foreach (var item in objGalleryModel.JobAttachmentlist)
            //    {
            //        if (item.Selected == true)
            //        {
            //            SelectedImgCount++;
            //        }

            //    }

            //    ViewBag.SelectedImgCount = SelectedImgCount;
            //}
            if (objGalleryModel.GalleryFolders != null && objGalleryModel.GalleryFolders.Count >= 2)
            {
                int finalImagesGroupid = objGalleryModel.GalleryFolders.ToArray()[2].ROW_ID;
                var finalImages = _repository.GetJobAttachments(finalImagesGroupid, Convert.ToInt16(jobid));
                foreach (var a in finalImages)
                {
                    objGalleryModel.JobAttachmentlist.Add(a);
                }
            }

            objGalleryModel.FolderRecordsCountList = _repository.GetFoldersAttachmentCount(Convert.ToInt16(jobid)).ToList();

            objGalleryModel.GetUserCommentsbyJobIdList = _repository.GetUserCommentsbyJobId(Convert.ToInt16(jobid)).ToList();



            //          List<FolderRecordsCounts> lstFolderRecordsCounts = new List<FolderRecordsCounts>();


            //         FolderRecordsCounts objFolderRecordsCounts = new FolderRecordsCounts();

            //            objFolderRecordsCounts.FolderName = "Copy Writing";
            //            objFolderRecordsCounts.Count=105;
            //            lstFolderRecordsCounts.Add(objFolderRecordsCounts);

            //            FolderRecordsCounts objFolderRecordsCounts1 = new FolderRecordsCounts();

            //            objFolderRecordsCounts1.FolderName = "Photography";
            //            objFolderRecordsCounts1.Count = 100;
            //            lstFolderRecordsCounts.Add(objFolderRecordsCounts1);
            //            //https://zerofootprint.atlassian.net/browse/OMSUIUX-13


            //objGalleryModel.FolderRecordsCountList = lstFolderRecordsCounts;


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

        ///
        /// 
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files, string JobId, string groupType, string groupname, string selectedTags) //,string JobId, string groupType, string groupname)
        {
            try
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        string strImgShowingRepositoryUrl = string.Empty;
                        string strFolder = string.Empty;
                        string strFolderThumb = string.Empty;

                        var fileName = Path.GetFileName(file.FileName);
                        strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
                        strFolderThumb = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
                        var tempFile = strFolder;
                        if (groupname.Contains("/"))
                        {
                            groupname = groupname.Replace("/", "-");
                        }

                        strFolder = strFolder + "/" + JobId + "/" + groupname + "/";

                        strFolderThumb = strFolderThumb + "/" + JobId + "/" + "thumb" + "/" + groupname + "/";

                        string FilePath = strFolder;
                        string FileThumbPath = strFolderThumb;

                        strImgShowingRepositoryUrl =
                            System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];

                        strFolder = strFolder + fileName.ToString();

                        strFolderThumb = strFolderThumb + "thumb-" + fileName.ToString();

                        if (System.IO.File.Exists(strFolder))
                        {
                            // rename file 

                            string strRandomFileName = Path.GetRandomFileName();
                            //This method returns a random file name of 11 characters
                            string FileExtension = System.IO.Path.GetExtension(file.FileName);
                            string FileName = System.IO.Path.GetFileName(file.FileName);
                            int fileSize = file.ContentLength;

                            strRandomFileName = strRandomFileName.Replace(".", "") + FileExtension;
                            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + JobId + "/" + groupname + "/" +
                                                         strRandomFileName.ToString();
                            strFolderThumb = string.Empty;
                            strFolderThumb = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
                            //strFolderThumb = strFolderThumb + "/" + JobId + "/" + "thumb" + "/" + groupname + "/" +
                            //                 strRandomFileName.ToString();

                            strFolderThumb = strFolderThumb + "/" + JobId + "/" + "thumb" + "/" + groupname + "/";
                            strFolderThumb = strFolderThumb + "thumb-" + strRandomFileName.ToString();


                            if (groupType == Convert.ToString((int)FolderEnum.FolderData.Image))
                            {
                                strFolder = string.Empty;
                                strFolder = FilePath + strRandomFileName;
                                tempFile += "temp-" + strRandomFileName;
                                file.SaveAs(tempFile);
                                CreatePrimaryImage(tempFile, strFolder);
                                CreateThumbImage(tempFile, strFolderThumb);
                                if (System.IO.File.Exists(tempFile))
                                {
                                    System.IO.File.Delete(tempFile);
                                }

                            }
                            else
                            {
                                strFolder = string.Empty;
                                strFolder = FilePath + strRandomFileName;
                                file.SaveAs(strFolder);
                                CreateThumbImage(strFolder, strFolderThumb);
                            }
                           

                            // Thumb File showing URL
                            string ThumbImgURL = string.Empty;
                            ThumbImgURL =
                                System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];
                            ThumbImgURL = ThumbImgURL + JobId + "/" + "thumb" + "/" + groupname + "/" +
                                          "thumb-"+strRandomFileName.ToString();
                            _repository.InsertJobAttachment(JobId, groupType, groupname, strRandomFileName,
                                FileExtension, fileSize, selectedTags, strImgShowingRepositoryUrl, FileName, ThumbImgURL);
                        }
                        else
                        {
                            DirectoryInfo diPath = new DirectoryInfo(FilePath);
                            diPath.Create();

                            DirectoryInfo diThumbPath = new DirectoryInfo(FileThumbPath);
                            diThumbPath.Create();

                            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + JobId + "/" + groupname + "/" +
                                                         fileName.ToString();
                            if (groupType == Convert.ToString((int)FolderEnum.FolderData.Image))
                            {
                                tempFile += "temp-" + fileName.ToString();
                                file.SaveAs(tempFile);

                                CreatePrimaryImage(tempFile, strFolder);
                                CreateThumbImage(tempFile, strFolderThumb);
                                if (System.IO.File.Exists(tempFile))
                                {
                                    System.IO.File.Delete(tempFile);
                                }
                            }
                            else
                            {
                                file.SaveAs(strFolder);
                                CreateThumbImage(strFolder, strFolderThumb);
                            }
                            ////////////////create a thumb image/////////////////////


                            string DisplayThumbImgPath =
                                System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];
                            // strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + JobId + "/" + groupname + "/" + "thumb-" + fileName.ToString();
                            DisplayThumbImgPath = DisplayThumbImgPath + JobId + "/" + "thumb" + "/" + groupname + "/" +
                                                  "thumb-" + fileName.ToString();

                            // if Folder name 'Floor Plan' we need to select thet image


                            if (groupType.Equals("4") && groupname.Trim().Equals("Floor Plan"))
                            {
                            _repository.InsertJobAttachment(JobId, groupType, groupname, file, selectedTags,
                              strImgShowingRepositoryUrl, DisplayThumbImgPath,1);
                            }
                            else
                            {
                            _repository.InsertJobAttachment(JobId, groupType, groupname, file, selectedTags,
                                strImgShowingRepositoryUrl, DisplayThumbImgPath);
                            }
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

        public JsonResult UpdateFolderLock(string JobId, string groupType)
        {
            if (groupType == "1")
            {
                _repository.UpdateFolderLock(JobId, 1);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else if (groupType == "4") { _repository.UpdateFolderLock(JobId, 2);
            return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

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

        //public ActionResult Save(IEnumerable<HttpPostedFileBase> files, string JobId, string groupType, string groupname, string selectedTags) //,string JobId, string groupType, string groupname)
        //{
        //    // The Name of the Upload component is "files"
        //    //if (files != null)
        //    //{
        //    //    foreach (var file in files)
        //    //    {
        //    //        // Some browsers send file names with full path.
        //    //        // We are only interested in the file name.
        //    //          var fileName = Path.GetFileName(file.FileName);

        //    //        string strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];

        //    //          string strphysicalPath = "~/" + strFolder + "/" + JobId + "/" + groupname + "/";

        //    //          var physicalPath = Path.Combine(Server.MapPath(strphysicalPath), fileName);

        //    //         // var path = Server.MapPath("~/Content/users/" + "manoj");

        //    //       // Attachments\12345\PhotoGraphy\Room.Jpg

        //    //          var path = Server.MapPath("~/" + strFolder + "/" + JobId + "/" + groupname + "/");

        //    //          Directory.CreateDirectory(path);

        //    //        strphysicalPath = strphysicalPath + fileName;

        //    //          // Attachments\12345\PhotoGraphy\Room.Jpg
        //    //        _repository.InsertJobAttachment(JobId, groupType, groupname, file, selectedTags, strphysicalPath);                    
        //    //        //   The files are not actually saved in this demo
        //    //          file.SaveAs(physicalPath);

        //    //    }
        //    //}

        //    try
        //    {
        //        if (files != null)
        //        {


        //            foreach (var file in files)
        //            {
        //                string strImgShowingRepositoryUrl = string.Empty;
        //                string strFolder = string.Empty;
        //                string strFolderThumb = string.Empty;

        //                var fileName = Path.GetFileName(file.FileName);
        //                strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
        //                strFolderThumb = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];

        //                if (groupname.Contains("/"))
        //                {
        //                    groupname = groupname.Replace("/", "-");
        //                }

        //                strFolder = strFolder + "/" + JobId + "/" + groupname + "/";

        //                strFolderThumb = strFolderThumb + "/" + JobId + "/" + "thumb" + "/" + groupname + "/";

        //                string FilePath = strFolder;
        //                string FileThumbPath = strFolderThumb;

        //                strImgShowingRepositoryUrl = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];

        //                strFolder = strFolder + fileName.ToString();

        //                strFolderThumb = strFolderThumb + "thumb-" + fileName.ToString();

        //                if (System.IO.File.Exists(strFolder))
        //                {
        //                    // rename file 

        //                    string strRandomFileName = Path.GetRandomFileName(); //This method returns a random file name of 11 characters
        //                    string FileExtension = System.IO.Path.GetExtension(file.FileName);
        //                    string FileName = System.IO.Path.GetFileName(file.FileName);
        //                    int fileSize = file.ContentLength;

        //                    strRandomFileName = strRandomFileName.Replace(".", "") + FileExtension;
        //                    strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + JobId + "/" + groupname + "/" + strRandomFileName.ToString();

        //                    strFolder = string.Empty;
        //                    strFolder = FilePath + strRandomFileName;
        //                    file.SaveAs(strFolder);

        //                    strFolderThumb = string.Empty;
        //                    strFolderThumb = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
        //                    strFolderThumb = strFolderThumb + "/" + JobId + "/" + "thumb" + "/" + groupname + "/" + strRandomFileName.ToString();

        //                    CreateThumbImage(strFolder, strFolderThumb);

        //                    // Thumb File showing URL
        //                    string ThumbImgURL = string.Empty;

        //                    ThumbImgURL = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];
        //                    ThumbImgURL = ThumbImgURL + JobId + "/" + "thumb" + "/" + groupname + "/" + strRandomFileName.ToString();
        //                    _repository.InsertJobAttachment(JobId, groupType, groupname, strRandomFileName, FileExtension, fileSize, selectedTags, strImgShowingRepositoryUrl, FileName, ThumbImgURL);
        //                }
        //                else
        //                {
        //                    DirectoryInfo diPath = new DirectoryInfo(FilePath);
        //                    diPath.Create();

        //                    DirectoryInfo diThumbPath = new DirectoryInfo(FileThumbPath);
        //                    diThumbPath.Create();

        //                    strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + JobId + "/" + groupname + "/" + fileName.ToString();
        //                    file.SaveAs(strFolder);

        //                    ////////////////create a thumb image/////////////////////

        //                    CreateThumbImage(strFolder, strFolderThumb);

        //                    string DisplayThumbImgPath = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];
        //                    // strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + JobId + "/" + groupname + "/" + "thumb-" + fileName.ToString();
        //                    DisplayThumbImgPath = DisplayThumbImgPath + JobId + "/" + "thumb" + "/" + groupname + "/" + "thumb-" + fileName.ToString();

        //                    _repository.InsertJobAttachment(JobId, groupType, groupname, file, selectedTags, strImgShowingRepositoryUrl, DisplayThumbImgPath);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        #region For Error

        //        string ErrorMsg = ex.Message;
        //        string Error = ErrorMsg;



        //        if (!string.IsNullOrEmpty(ErrorMsg))
        //        {

        //            switch (Error)
        //            {
        //                case "FileExist":
        //                    return Json(new DataSourceResult
        //                    {
        //                        // Errors = "You cannot change the organizer of an instance."
        //                        Errors = "CustomError400"
        //                    });

        //                case "CustomError401":
        //                    return Json(new DataSourceResult
        //                    {
        //                        //Errors = "You cannot turn an instance of a recurring event into a recurring event itself."
        //                        Errors = "CustomError401"
        //                    });


        //                case "Null_Event":
        //                    return Json(new DataSourceResult
        //                    {
        //                        Errors = "Null_Event"
        //                    });

        //                //default:
        //                //         Logger(ex.InnerException.Message);
        //                //         return Json(new DataSourceResult
        //                //    {

        //                //        Errors = string.Empty
        //                //    });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new DataSourceResult
        //            {
        //                Errors = ""
        //            });
        //        }
        //        #endregion
        //    }
        //    return Content("");
        //}

        /// <summary>   This function is used for creating a thumb size image
        ///  ////////////////This function is used for creating a thumb size image/////////////////////
        /// </summary>
        /// <param name="SourceThumbFileName"></param>
        /// <param name="DestinationThumbImg"></param>
        private void CreateThumbImage(string SourceThumbFileName, string DestinationThumbImg)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(SourceThumbFileName);
            int srcWidth = image.Width;
            int srcHeight = image.Height;

            int resizeWidth = 180;

            Decimal sizeRatio = ((Decimal)srcHeight / srcWidth);
            int thumbHeight = Decimal.ToInt32(sizeRatio * resizeWidth);

            Bitmap bmp = new Bitmap(resizeWidth, thumbHeight);

            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);

            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, resizeWidth, thumbHeight);
            gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);
            bmp.Save(DestinationThumbImg, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
        }

        private void CreatePrimaryImage(string SourceThumbFileName, string DestinationThumbImg)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(SourceThumbFileName);
            int srcWidth = image.Width;
            int srcHeight = image.Height;

            int resizeWidth = 760;

            Decimal sizeRatio = ((Decimal)srcHeight / srcWidth);
            int thumbHeight = Decimal.ToInt32(sizeRatio * resizeWidth);

            Bitmap bmp = new Bitmap(resizeWidth, thumbHeight);

            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
            bmp.SetResolution(100, 100);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, resizeWidth, thumbHeight);
            gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);
            bmp.Save(DestinationThumbImg, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
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
        public static IList<T> Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }
        public ActionResult FolderContent(string folderName, string groupid, string job_Id)
        {
            try
            {
                ViewBag.GroupId = groupid;
                GalleryModel objGalleryModel = new GalleryModel();

                objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders(Convert.ToInt32(job_Id)).ToList();
                var lst = Swap(objGalleryModel.GalleryFolders, 1, 2);
                objGalleryModel.GalleryFolders = lst;

                var TagList = objGalleryModel.GalleryFolders.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.Folder });

                objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(Convert.ToInt32(groupid), Convert.ToInt32(job_Id));

                TempData["ImgCount"] = null;
                TempData["ImgCount"] = objGalleryModel.JobAttachmentlist.Count;
                if (objGalleryModel.JobAttachmentlist.Count > 0)
                {
                    ViewBag.ImgCount = objGalleryModel.JobAttachmentlist.Count;
                    
                    int SelectedImgCount = 0;
                    foreach (var item in objGalleryModel.JobAttachmentlist)
                    {
                        if (item.Selected == true)
                        {
                            SelectedImgCount++;
                        }

                    }

                    ViewBag.SelectedImgCount = SelectedImgCount;
                }

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
                            objtagInfo.Folder = item.Folder;
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
                    TempData["lstTagInfo"] = objGalleryModel.TagList;

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

        public JsonResult GetFolderCount()
        {
            return Json(TempData["ImgCount"], JsonRequestBehavior.AllowGet);
        }

        public ActionResult FiltersFolderSelectedItems(string folderName, string groupid, string job_Id)
        {
            try
            {
                GalleryModel objGalleryModel = new GalleryModel();

                objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders(Convert.ToInt32(job_Id)).ToList();
                var TagList = objGalleryModel.GalleryFolders.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.Folder });

                objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(Convert.ToInt32(groupid), Convert.ToInt32(job_Id)).Where(s => s.Selected == true).ToList();

                //  objGalleryModel.JobAttachmentlist = objGalleryModel.JobAttachmentlist.Where(s => s.Selected == true).ToList();

                if (objGalleryModel.JobAttachmentlist.Count > 0)
                {
                    ViewBag.ImgCount = objGalleryModel.JobAttachmentlist.Count;

                    int SelectedImgCount = 0;
                    foreach (var item in objGalleryModel.JobAttachmentlist)
                    {
                        if (item.Selected == true)
                        {
                            SelectedImgCount++;
                        }
                    }
                    ViewBag.SelectedImgCount = SelectedImgCount;
                }

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
                            objtagInfo.Folder = item.Folder;
                            objtagInfo.TagName = obj;
                            lstTagInfo.Add(objtagInfo);
                        }
                    }

                    objGalleryModel.TagList = lstTagInfo;
                }

                return PartialView("Controls/JobTracking/_GalleryList", objGalleryModel);
            }

            catch (Exception)
            {
                return PartialView("Controls/JobTracking/_FileUploader");
            }
        }

        public ActionResult TagBasisSearchContent(string folderName, string groupid, string job_Id, bool ShowExtwiseGalleryFiles)
        {
            if (!folderName.Equals("Selected"))
            {
                try
                {
                    GalleryModel objGalleryModel = new GalleryModel();
                    objGalleryModel.GalleryFolders = null;
                    objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders(Convert.ToInt32(job_Id)).ToList();

                    objGalleryModel.TagList = null;

                    // Added "Extension" Keyword for showing the result when user Click on the Top Search link i.e All, Document, Audio Image"

                    if (ShowExtwiseGalleryFiles)
                    {
                        folderName = "Extension " + folderName;
                    }

                    var TagList = objGalleryModel.GalleryFolders.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.Folder });

                    objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(Convert.ToInt32(groupid), Convert.ToInt32(job_Id), folderName);

                    if (objGalleryModel.JobAttachmentlist.Count > 0)
                    {
                        ViewBag.ImgCount = objGalleryModel.JobAttachmentlist.Count;

                        TempData["ImgCount"] = objGalleryModel.JobAttachmentlist.Count;
                        TempData.Keep("ImgCount");

                        int SelectedImgCount = 0;
                        foreach (var item in objGalleryModel.JobAttachmentlist)
                        {
                            if (item.Selected == true)
                            {
                                SelectedImgCount++;
                            }
                        }
                        ViewBag.SelectedImgCount = SelectedImgCount;
                    }

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
                                objtagInfo.Folder = item.Folder;
                                objtagInfo.TagName = obj;
                                lstTagInfo.Add(objtagInfo);
                            }
                        }

                        objGalleryModel.TagList = lstTagInfo;
                    }

                    return PartialView("Controls/JobTracking/_GalleryList", objGalleryModel);
                }

                catch (Exception)
                {
                    // return PartialView("Controls/JobTracking/_FileUploader");
                    return PartialView("Controls/JobTracking/_GalleryList");
                }
            }
            else
            {
                try
                {
                    GalleryModel objGalleryModel = new GalleryModel();

                    objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders(Convert.ToInt32(job_Id)).ToList();
                    var TagList = objGalleryModel.GalleryFolders.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.Folder });

                    objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(Convert.ToInt32(groupid), Convert.ToInt32(job_Id)).Where(s => s.Selected == true).ToList();

                    //  objGalleryModel.JobAttachmentlist = objGalleryModel.JobAttachmentlist.Where(s => s.Selected == true).ToList();

                    if (objGalleryModel.JobAttachmentlist.Count > 0)
                    {
                        TempData["ImgCount"] = objGalleryModel.JobAttachmentlist.Count;
                        TempData.Keep("ImgCount");
                        ViewBag.ImgCount = objGalleryModel.JobAttachmentlist.Count;


                        int SelectedImgCount = 0;
                        foreach (var item in objGalleryModel.JobAttachmentlist)
                        {
                            if (item.Selected == true)
                            {
                                SelectedImgCount++;
                            }
                        }
                        ViewBag.SelectedImgCount = SelectedImgCount;
                    }

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
                                objtagInfo.Folder = item.Folder;
                                objtagInfo.TagName = obj;
                                lstTagInfo.Add(objtagInfo);
                            }
                        }

                        objGalleryModel.TagList = lstTagInfo;
                    }

                    return PartialView("Controls/JobTracking/_GalleryList", objGalleryModel);
                }

                catch (Exception)
                {
                    return PartialView("Controls/JobTracking/_FileUploader");
                }
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
            var data = _repository.GetJobAttachmentTypes(Convert.ToInt32(groupid)).Select(p => new
            {
                AllowedFileExtensions = p.AllowedFileExtension,
                MaxFileSize = p.MaxFileSize
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTagInfoDataJson(string groupid = "1")
        {

            List<TagInfo> TagListInfo = new List<TagInfo>();
            if (TempData["lstTagInfo"] != null)
            {
                TagListInfo = (List<TagInfo>)TempData["lstTagInfo"];

            }
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

                    bool exists = System.IO.Directory.Exists(strFolder);

                    if (!exists)
                        System.IO.Directory.CreateDirectory(strFolder);

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
                                objJsonInfo.IsTagUpdated = true;
                                objJsonInfo.Tags = TagSelected.TrimEnd(',');
                            }
                            else
                            {
                                // this will rutund the exist file
                                if (!System.IO.File.Exists(NewFileURL))
                                {
                                    System.IO.File.Move(oldFileURL, NewFileURL); // Try to move
                                    _repository.UpdateJobAttachment(TagSelected, NewFileName, NewImgFilePathUrl, Job_id, Convert.ToInt16(Row_Id));
                                    objJsonInfo.Save = true;

                                    objJsonInfo.IsTagUpdated = true;
                                    objJsonInfo.IsFileNameUpdated = true;
                                    objJsonInfo.Tags = TagSelected.TrimEnd(',');
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
                            //objJsonInfo.Save = true;
                            //objJsonInfo.IsTagUpdated = true;
                            //objJsonInfo.Tags = TagSelected.TrimEnd(',');

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

        public ActionResult DeleteGalleryFile(string Row_Id, string groupname, string jobid, string FileName)
        {
            string strDestinationFolderPath = string.Empty;
            string strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
            string strImgShowingRepositoryUrl = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];
            string strFolderFilePath = string.Empty;

            if (groupname.Contains("/"))
            {
                groupname = groupname.Replace("/", "-");
            }

            strFolderFilePath = strFolder + jobid + "/" + groupname + "/" + FileName.ToString();

            if (System.IO.File.Exists(strFolderFilePath))
            {
                System.IO.File.Delete(strFolderFilePath);
            }
            _repository.UpdateJobAttachment(string.Empty, string.Empty, string.Empty, string.Empty,0, Convert.ToInt16(Row_Id), 1);
            return Json("File has been deleted", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateJobAttachmentSelected(string rowId, bool isSelected, int groupid, int job_Id)
        {

            List<ImgCountInfo> lstImgCountInfo = new List<ImgCountInfo>();
            _repository.UpdateJobAttachmentSelected(Convert.ToInt16(rowId), isSelected);
            var GetAlldata = _repository.GetJobAttachments(Convert.ToInt32(groupid), Convert.ToInt32(job_Id)).ToList();
            int ImgCount = GetAlldata.Count;
            int SelectedImg = GetAlldata.Where(s => s.Selected == true).ToList().Count;// = _repository.GetJobAttachments(Convert.ToInt32(groupid), Convert.ToInt32(job_Id)).Where(s => s.Selected == true).ToList();

            ImgCountInfo objImgCountInfo = new ImgCountInfo();
            objImgCountInfo.SelectedImgCount = SelectedImg;

            if (TempData["ImgCount"] != null)
            {
                objImgCountInfo.TotalImgCount = Convert.ToInt32(TempData["ImgCount"]);
                TempData.Keep("ImgCount");
            }
            else
            {
                objImgCountInfo.TotalImgCount = ImgCount;
            }

            lstImgCountInfo.Add(objImgCountInfo);

            return Json(lstImgCountInfo, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult SaveComments(string Row_Id, string Comments)
        {
            _repository.UpdateJobAttachmentComments(Convert.ToInt16(Row_Id), Comments);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetComments(string Row_Id)
        //{
        //    var data = _repository.GetJobCommentsById(Convert.ToInt32(Row_Id)).Select(p => new
        //    {
        //        Comments = p.Comments
        //    });
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetComments(string Row_Id)
        {

            //var data = _repository.GetJobCommentsById(Convert.ToInt32(Row_Id)).Select(p => new
            //{
            //    Comments = p.Comments
            //});

            //byte[] databyte = null;

            //int  width = 100;
            //int  height = 100;

            // var imageFile = Path.Combine(Server.MapPath("~/App_Data/uploads"), "DPIlogo.jpg");
            // using (var srcImage = Image.FromFile(imageFile))
            // using (var newImage = new Bitmap(width, height))
            // using (var graphics = Graphics.FromImage(newImage))
            // using (var stream = new MemoryStream())
            // {
            //     graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //     graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //     graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //     graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height));
            //     newImage.Save(stream, ImageFormat.Png);
            //     databyte= stream.ToArray();
            // }


            var data = _repository.GetUserCommentsbyJobId(Convert.ToInt32(Row_Id)).Select(p => new
            {

                FirstName = p.FirstName,
                LastName = p.LastName,
                Row_id = p.Row_Id,
                TimeOfComment = OrderManagementHtmlHelper.TimeAgo(Convert.ToDateTime(p.Created)),
                Comments = p.Comments,
                FilePath = p.FilePath,
                Tags = p.Tags,
                //Userimage =  string.Format("data:image/png;base64,{0}", Convert.ToBase64String(p.Buffer)),
                //defaultImg = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(databyte)),
                Userimage = string.Empty,
                defaultImg = string.Empty,
                Annotaion = "{ x: 0.62, y: 0.7, text: 'Manoj' },{ x: 0.51, y: 0.5, text: 'Vinit' },{ x: 0.40, y: 0.3, text: 'Chris, obviously' }"
            });

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCommentsForMail(string Row_Id)
        {

            //var data = _repository.GetJobCommentsById(Convert.ToInt32(Row_Id)).Select(p => new
            //{
            //    Comments = p.Comments
            //});

            //byte[] databyte = null;

            //int  width = 100;
            //int  height = 100;

            // var imageFile = Path.Combine(Server.MapPath("~/App_Data/uploads"), "DPIlogo.jpg");
            // using (var srcImage = Image.FromFile(imageFile))
            // using (var newImage = new Bitmap(width, height))
            // using (var graphics = Graphics.FromImage(newImage))
            // using (var stream = new MemoryStream())
            // {
            //     graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //     graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //     graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //     graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height));
            //     newImage.Save(stream, ImageFormat.Png);
            //     databyte= stream.ToArray();
            // }


            var data = _repository.GetUserCommentsbyJobIdForMail(Convert.ToInt32(Row_Id)).Select(p => new
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                Row_id = p.Row_Id,
                TimeOfComment = OrderManagementHtmlHelper.TimeAgo(Convert.ToDateTime(p.Created)),
                Comments = p.Comments,
                FilePath = p.FilePath,
                Tags = p.Tags,
                //Userimage =  string.Format("data:image/png;base64,{0}", Convert.ToBase64String(p.Buffer)),
                //defaultImg = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(databyte)),
                Userimage = string.Empty,
                defaultImg = string.Empty,
                Annotaion = "{ x: 0.62, y: 0.7, text: 'Manoj' },{ x: 0.51, y: 0.5, text: 'Vinit' },{ x: 0.40, y: 0.3, text: 'Chris, obviously' }"
            });

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public string GetTemplateAsString(GetJobAttachmentAndComment imageList, string groupid, string job_Id, string Template)
        {
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(stringWriter);

            string[] headers = { "IMAGE THUMBNAIL", "IMAGE NAME", "CLIENT COMMENTS" };

            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "10px 10px");
            writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
            // close thead
            var imagesListData = new List<SelectJobAttachmentTemplate_Result>();
            //if (Template == "Image Confirmation")
            //{
            //    imagesListData = imageList.JobAttachmentAndCommment.Where(a => a.ConfirmSelectionStatus == true).ToList();
            //}
            //else
            //{
                imagesListData = imageList.JobAttachmentAndCommment.Where(a => a.Selected == true).ToList();
            //}
            if (imagesListData.Any())
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Thead);
                foreach (var header in headers)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Open Sans, Georgia");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "12px");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "1px solid #7e7e7e");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "#7e7e7e");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write(header);
                    writer.RenderEndTag();      // close th
                }
                writer.RenderEndTag();
                foreach (var imageData in imagesListData)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                    writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Open Sans, Georgia");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "12px");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "#7e7e7e");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);

                    var urlValue = "";
                    if (ConfigurationManager.AppSettings["UseTestSiteUrl"].ToString() == "true")
                    {
                        urlValue = ConfigurationManager.AppSettings["TestSiteUrl"].ToString() + "JobTracking/JobImage?groupid=" + groupid + "&job_Id=" + job_Id + "&image_Id=" + imageData.Row_Id;
                    }
                    else
                    {
                        urlValue = ConfigurationManager.AppSettings["LocalUrl"].ToString() + "JobTracking/JobImage?groupid=" + groupid + "&job_Id=" + job_Id + "&image_Id=" + imageData.Row_Id;
                    }


                    // anchor tag
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, urlValue);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);

                    // image tag
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, imageData.Thumbnail);
                    writer.AddAttribute(HtmlTextWriterAttribute.Width, "50px");
                    writer.AddAttribute(HtmlTextWriterAttribute.Height, "50px");
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "taggd");
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, "taggd_" + imageData.Row_Id);
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);       // Begin Image tag
                    writer.RenderEndTag();                              // close Image tag
                    writer.RenderEndTag();
                    writer.RenderEndTag();// close td 1

                    writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Open Sans, Georgia");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "12px");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "#7e7e7e");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(imageData.FileName);
                    writer.RenderEndTag();                              // close td 2

                    writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Open Sans, Georgia");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "12px");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "#7e7e7e");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    if (imageData.CommentCount > 0)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, urlValue);
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.Write("Yes");
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                    }
                    else { writer.Write("NO"); }
                    writer.RenderEndTag();                              // close td 3
                    writer.RenderEndTag();                              // close tr
                }
            }
            else
            {
                writer.Write("NO Images Selected..");
            }
            writer.RenderEndTag();                                  // close tbody
            writer.RenderEndTag();                                  // close table

            return stringWriter.ToString();
        }

        public ActionResult GetJobAttachmentAndCommentTemplateResult(string groupid, string job_Id, string MailTemplate)
        {
            GetJobAttachmentAndComment objGalleryModel = new GetJobAttachmentAndComment();
            try
            {
                objGalleryModel.JobAttachmentAndCommment = _repository.SelectJobAttachmentTemplate(Convert.ToInt32(groupid), Convert.ToInt32(job_Id));
                var EventPrintDataNew = _scheduler.GetEventCommunicationTemplate(MailTemplate, false, true);
                var list = objGalleryModel.JobAttachmentAndCommment;//.Where(a => a.GroupType == groupid).ToList();
                objGalleryModel.JobAttachmentAndCommment = list;
                var selectJobAttachmentCommentResult = objGalleryModel.JobAttachmentAndCommment.FirstOrDefault();
                string GetImageDetails = null;
                if (MailTemplate == "Job Attachments Feedback" || MailTemplate == "Upload Confirmation Images" || MailTemplate == "Image Confirmation")
                {
                    GetImageDetails = GetTemplateAsString(objGalleryModel, groupid, job_Id, MailTemplate);
                }
                if (selectJobAttachmentCommentResult != null)
                {
                    objGalleryModel.Company_Name = selectJobAttachmentCommentResult.Company_Name;
                    objGalleryModel.Address = selectJobAttachmentCommentResult.Name;
                    objGalleryModel.StartDate = selectJobAttachmentCommentResult.StartDate;
                    objGalleryModel.DueDate = selectJobAttachmentCommentResult.DueDate;
                    objGalleryModel.Name = selectJobAttachmentCommentResult.FirstName;
                    objGalleryModel.package = selectJobAttachmentCommentResult.PackageNumber;
                    objGalleryModel.ContactNumber = selectJobAttachmentCommentResult.MobileNumber;
                    objGalleryModel.FName2 = selectJobAttachmentCommentResult.Fname2;
                    objGalleryModel.DAY_PHOTOGRAPHER = selectJobAttachmentCommentResult.DAY_PHOTOGRAPHER;
                    objGalleryModel.DUSK_PHOTOGRAPHER = selectJobAttachmentCommentResult.DUSK_PHOTOGRAPHER;
                }
                string EmailContent = string.Empty;
                string FromEmail = string.Empty;
                string EmailSubject = string.Empty;
                string EmailCC = string.Empty;
                string EmailBCC = string.Empty;
                string EmailTo = string.Empty;
                 string DisplayFrom = string.Empty;
                foreach (var item in EventPrintDataNew)
                {
                    if (string.IsNullOrEmpty(EmailContent))
                    {
                        EmailContent = item.TEMPLATE;
                    }

                    if (string.IsNullOrEmpty(FromEmail))
                    {
                        FromEmail = item.FROM;
                    }
                    if (string.IsNullOrEmpty(EmailSubject))
                    {
                        EmailSubject = item.EMAIL_SUBJECT;

                    }
                    if (string.IsNullOrEmpty(EmailCC))
                    {
                        EmailCC = item.CC_TO;
                    }
                    if (string.IsNullOrEmpty(EmailTo))
                    {
                        EmailTo = item.TO;
                    }

                    if (string.IsNullOrEmpty(EmailBCC))
                    {
                        EmailBCC = item.BCC_TO;
                    }

                    if (string.IsNullOrEmpty(EmailContent))
                    {
                        EmailContent = item.TEMPLATE;
                    }

                    if (string.IsNullOrEmpty(FromEmail))
                    {
                        FromEmail = item.FROM;
                    }
                    if (string.IsNullOrEmpty(DisplayFrom))
                    {
                        DisplayFrom = item.FROM_DISPLAY_AS;
                    }

                    if ("[$Company_Name$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$Company_Name$]", objGalleryModel.Company_Name);

                    }
                    if ("[$Address$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$Address$]", objGalleryModel.Address);
                    }

                    if ("[$BookingContract$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$BookingContract$]", objGalleryModel.ContactNumber);
                    }

                    if ("[$StartTime$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$StartTime$]", Convert.ToString(objGalleryModel.StartDate));
                    }

                    if ("[$DueDate$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$DueDate$]", Convert.ToString(objGalleryModel.DueDate));
                    }

                    if (MailTemplate != "Upload Confirmation Images" && MailTemplate != "Image Confirmation")
                    {
                        if ("[$Fname$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                        {
                            EmailContent = EmailContent.Replace("[$Fname$]", objGalleryModel.Name);
                        }
                    }

                    if ("[$PackageNo$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$PackageNo$]", objGalleryModel.package);
                    }

                    if ("[$Location$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$Location$]", objGalleryModel.Address);
                    }

                    
                    if (MailTemplate == "Job Attachments Feedback" || MailTemplate == "Upload Confirmation Images" || MailTemplate == "Image Confirmation")
                    {

                        if ("[$ImageTable$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                        {
                            if (GetImageDetails.Length > 0)
                            {
                                EmailContent = EmailContent.Replace("[$ImageTable$]", GetImageDetails);
                            }

                        }
                    }

                    if (MailTemplate == "Upload Confirmation Images" || MailTemplate == "Image Confirmation")
                    {
                        if ("[$Fname$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                        {
                            EmailContent = EmailContent.Replace("[$Fname$]", objGalleryModel.DAY_PHOTOGRAPHER);
                        }
                        if ("[$Fname2$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                        {
                            EmailContent = EmailContent.Replace("[$Fname2$]", objGalleryModel.DUSK_PHOTOGRAPHER);
                        }
                    }
                    EmailContent = EmailContent.Replace("[$PrintOrEmail$]", "print");


                    item.TEMPLATE = EmailContent;
                }

                List<string> lstEmailContent = new List<string>();
                lstEmailContent.Add(EmailContent);


                List<EventData> lstEmailData = new List<EventData>();

                EventData objEmailData = new EventData();

                objEmailData.EmailContent = EmailContent;
                objEmailData.EmailFrom = FromEmail;
                objEmailData.EmailFromDisplay=DisplayFrom;
                if (selectJobAttachmentCommentResult != null)
                {
                    objEmailData.EmailSubject = EmailSubject + ": " + selectJobAttachmentCommentResult.Name;
                }
                else
                {
                    objEmailData.EmailSubject = EmailSubject;
                }

                objEmailData.EmailCC = EmailCC;
                objEmailData.EmailBCC = EmailBCC;
                objEmailData.TO = EmailTo;
                lstEmailData.Add(objEmailData);

                return Json(lstEmailData, JsonRequestBehavior.AllowGet);
                //if (MailTemplate == "0")
                //{
                //    return PartialView("Controls/JobTracking/_JobAttachmentEmailTemplate", objGalleryModel);

                //}
                //else
                //{
                //    return PartialView("Controls/JobTracking/_JobAttachmentEmailTemplateapprove", objGalleryModel);
                //}
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public ActionResult GetUserEmailAddressResult()
        {
            var userlistModel = _userService.GetAllUsersBySp().ToList();
            return Json(userlistModel, JsonRequestBehavior.AllowGet);
        }

        private void Logger(string logText)
        {
            using (StreamWriter tw = new StreamWriter(System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"] + "Logger.txt", true))
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

            if (model.Jobid != 0)
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

        public ActionResult DeleteJobCopy(string jobid, string jobCopyRowid)
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
                return File(strFileDestinationFolder, "application/jpg", string.Format(FileName, 0));
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

        [HttpPost, FileDownload]
        public FilePathResult DownloadAllFilesInZip(string groupname, string jobid , string Title )
        {
            string strDestinationFolderPath = string.Empty;
            string strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
            string strImgShowingRepositoryUrl = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];
            string FileName = Title + " " +  groupname + ".zip";


            if (groupname.Contains("/"))
            {
                groupname = groupname.Replace("/", "-");
            }

            if (System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] != null)
            {
                strDestinationFolderPath = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + jobid + "/" + groupname + "/";
            }

            strFolder = strFolder + jobid + "/" + groupname + "/";

            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + jobid + "/" + groupname + "/" + FileName.ToString();


            string strFileDestinationFolder;
            strFileDestinationFolder = string.Empty;
            strFileDestinationFolder = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + jobid + "/" + groupname + "/" + FileName.ToString();

            if (System.IO.File.Exists(strFileDestinationFolder))
            {
                System.IO.File.Delete(strFileDestinationFolder);

                // Copy file from source to app folder [destination folder]


                ZipFile.CreateFromDirectory(strFolder, strFileDestinationFolder);

                //  System.IO.File.Copy(strFolder, strFileDestinationFolder);

                return File(strFileDestinationFolder, "application/jpg", string.Format(FileName, 0));
                // this will rutund the exist file
            }
            else
            {
                strFileDestinationFolder = string.Empty;

                strFileDestinationFolder = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + jobid + "/" + groupname + "/";

                DirectoryInfo diPath = new DirectoryInfo(strFileDestinationFolder);
                diPath.Create();

                strFileDestinationFolder = strFileDestinationFolder + FileName.ToString();



                ZipFile.CreateFromDirectory(strFolder, strFileDestinationFolder);

                // Copy file from source to app folder [destination folder]
                //   System.IO.File.Copy(strFolder, strFileDestinationFolder);


                return File(strFileDestinationFolder, "application/jpg", string.Format(FileName, 0));
            }
        }


        //[HttpPost]
        // TODO : Remove Later 
        public FileResult DowloadAllFilesAsPdf1(string groupId, string jobid, string Title, string groupPDF)
        {
            try
            {

                String PdfFileName = Title + " " + groupPDF;

                GalleryModel objGalleryModel = new GalleryModel();
                objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(Convert.ToInt16(groupId), Convert.ToInt16(jobid));

                if (objGalleryModel.JobAttachmentlist.Any())
                {
                    var document = new Document();
                    MemoryStream stream = new MemoryStream();

                    PdfWriter.GetInstance(document, stream);

                    document.Open();
                    document.AddTitle(Title);
                    PdfPTable table = new PdfPTable(2);

                    table.DefaultCell.Border = Rectangle.NO_BORDER;

                    int count = 0;

                    var font = new pdfFont(iTextSharp.text.Font.FontFamily.TIMES_ROMAN,
                       11, iTextSharp.text.Font.NORMAL);


                    if (Convert.ToInt32(groupId) == 1)
                    {
                        foreach (GetJobAttachments result in objGalleryModel.JobAttachmentlist)
                        {
                            try
                            {

                                var Path = string.Empty;

                                if (result.Thumbnail != null && (result.Thumbnail.Length > 0))
                                {
                                    Path = result.Thumbnail;
                                }
                                else
                                {
                                    Path = result.FilePath;
                                }
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Path, false);
                                img.ScaleAbsoluteHeight(250f);
                                img.ScaleAbsoluteWidth(200f);

                                var imgCell = new PdfPCell()
                                {
                                    PaddingBottom = 5,
                                    PaddingTop = 5
                                };
                                string fileName = result.FileName;
                                string fileName1 = fileName.Length > 38
                                    ? fileName.Substring(0, 38) + "\n" + fileName.Substring(38, fileName.Length - 38)
                                    : fileName;

                                var paragraph = new Paragraph(fileName1, font);
                                if (((objGalleryModel.JobAttachmentlist.Count) % 2 != 0) &&
                                    count == (objGalleryModel.JobAttachmentlist.Count - 1))
                                {
                                    imgCell.Colspan = 2;
                                }
                                imgCell.AddElement(img);
                                imgCell.AddElement(paragraph);
                                imgCell.Border = 0;
                                imgCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.AddCell(imgCell);
                            }
                            catch (WebException ex)
                            {
                            }
                            finally
                            {
                                count++;
                            }
                        }
                    }
                    else
                    {
                        int indentation = 0;

                        foreach (GetJobAttachments result in objGalleryModel.JobAttachmentlist)
                        {
                            try
                            {

                                var Path = string.Empty;

                                if (result.Thumbnail != null && (result.Thumbnail.Length > 0))
                                {
                                    Path = result.Thumbnail;
                                }
                                else
                                {
                                    Path = result.FilePath;
                                }
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Path, false);
                                var imgCell = new PdfPCell()
                                {
                                    PaddingBottom = 5,
                                    PaddingTop = 5
                                };

                                string fileName = result.FileName;
                                string fileName1 = fileName.Length > 38
                                    ? fileName.Substring(0, 38) + "\n" + fileName.Substring(38, fileName.Length - 38)
                                    : fileName;
                                var paragraph = new Paragraph(fileName1, font);
                                img.ScaleAbsolute(500f, 300f);
                                img.SetAbsolutePosition(document.PageSize.Width - 50f, document.PageSize.Height - 50f);
                                img.Alignment = iTextSharp.text.Image.TEXTWRAP | iTextSharp.text.Image.ALIGN_CENTER;
                                img.IndentationLeft = 10f;
                                img.IndentationRight = 10f;
                                img.SpacingAfter = 9f;
                                img.SpacingBefore = 9f;
                                imgCell.Colspan = 2;
                                imgCell.AddElement(img);
                                imgCell.AddElement(paragraph);
                                imgCell.FixedHeight = 700f;
                                imgCell.Border = Rectangle.NO_BORDER;
                                table.AddCell(imgCell);
                            }
                            catch (WebException ex)
                            {
                            }
                            finally
                            {
                                count++;
                            }
                        }

                    }
                    Paragraph heading = new Paragraph(Title);
                    heading.SpacingAfter = 18f;
                    heading.Alignment = Element.ALIGN_CENTER;
                    document.Add(heading);
                    document.Add(table);
                    document.Close();
                    byte[] bytes = stream.ToArray();
                    stream.Close();
                    return File(bytes, "pdf", PdfFileName + ".pdf");
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public FileResult DowloadAllFilesAsPdf(string groupId, string jobid, string Title, string groupPDF)
        {
            try
            {

                String PdfFileName = Title + " " + groupPDF;

                GalleryModel objGalleryModel = new GalleryModel();
                objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(Convert.ToInt16(groupId), Convert.ToInt16(jobid));

                if (objGalleryModel.JobAttachmentlist.Any())
                {
                    var document = new Document();
                    MemoryStream stream = new MemoryStream();

                    PdfWriter.GetInstance(document, stream);

                    document.Open();
                    document.AddTitle(Title);
                    PdfPTable table = new PdfPTable(2);

                    table.DefaultCell.Border = Rectangle.NO_BORDER;

                    int count = 0;

                    var font = new pdfFont(iTextSharp.text.Font.FontFamily.TIMES_ROMAN,
                       11, iTextSharp.text.Font.NORMAL);


                    if (Convert.ToInt32(groupId) == 1)
                    {
                        foreach (GetJobAttachments result in objGalleryModel.JobAttachmentlist)
                        {
                            try
                            {

                                var Path = string.Empty;
                                Path = result.FilePath;
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Path, false);
                                img.ScaleAbsoluteHeight(250f);
                                img.ScaleAbsoluteWidth(200f);

                                var imgCell = new PdfPCell()
                                {
                                    PaddingBottom = 5,
                                    PaddingTop = 5
                                };
                                string fileName = result.FileName;
                                string fileName1 = fileName.Length > 38
                                    ? fileName.Substring(0, 38) + "\n" + fileName.Substring(38, fileName.Length - 38)
                                    : fileName;

                                var paragraph = new Paragraph(fileName1, font);
                                if (((objGalleryModel.JobAttachmentlist.Count) % 2 != 0) &&
                                    count == (objGalleryModel.JobAttachmentlist.Count - 1))
                                {
                                    imgCell.Colspan = 2;
                                }
                                imgCell.AddElement(img);
                                imgCell.AddElement(paragraph);
                                imgCell.Border = 0;
                                imgCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.AddCell(imgCell);
                            }
                            catch (WebException ex)
                            {
                            }
                            finally
                            {
                                count++;
                            }
                        }
                    }
                    else
                    {
                        int indentation = 0;

                        foreach (GetJobAttachments result in objGalleryModel.JobAttachmentlist)
                        {
                            try
                            {

                                var Path = string.Empty;
                                 Path = result.FilePath;
                              
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Path, false);
                                var imgCell = new PdfPCell()
                                {
                                    PaddingBottom = 5,
                                    PaddingTop = 5
                                };

                                string fileName1 = result.FileName;
                               var paragraph = new Paragraph(fileName1, font);
                                paragraph.Alignment=(Element.ALIGN_CENTER);
                                paragraph.SpacingBefore = 2f;
                                img.Alignment = iTextSharp.text.Image.TEXTWRAP | iTextSharp.text.Image.ALIGN_CENTER;
                                img.IndentationLeft = 10f;
                                img.IndentationRight = 10f;
                                img.SpacingAfter = 100f;
                                img.SpacingBefore = 200f;
                                imgCell.Colspan = 2;
                                imgCell.AddElement(paragraph);
                                imgCell.AddElement(img);
                                imgCell.Border = Rectangle.NO_BORDER;
                                table.AddCell(imgCell);
                            }
                            catch (WebException ex)
                            {
                            }
                            finally
                            {
                                count++;
                            }
                        }

                    }
                    Paragraph heading = new Paragraph(Title);
                    heading.SpacingAfter = 10f;
                    heading.Alignment = Element.ALIGN_CENTER;
                    document.Add(heading);
                    document.Add(table);
                    document.Close();
                    byte[] bytes = stream.ToArray();
                    stream.Close();
                    return File(bytes, "pdf", PdfFileName + ".pdf");
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult ViewGiftframe(string jobid)
        {

            GalleryModel objGalleryModel = new GalleryModel();
            //objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders().ToList();

            //int groupid = objGalleryModel.GalleryFolders.Take(1).FirstOrDefault().ROW_ID;
            //var TagList = objGalleryModel.GalleryFolders.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.FOLDER });

            //foreach (var item in TagList)
            //{
            //    string myString = item.TAGS;

            //    if (item.TAGS != null)
            //    {
            //        string[] strArray = myString.Split(';');

            //        List<TagInfo> lstTagInfo = new List<TagInfo>();

            //        foreach (string obj in strArray)
            //        {
            //            TagInfo objtagInfo = new TagInfo();

            //            objtagInfo.Row_Id = item.ROW_ID;
            //            objtagInfo.Folder = item.FOLDER;
            //            objtagInfo.TagName = obj;
            //            lstTagInfo.Add(objtagInfo);
            //        }

            //        objGalleryModel.TagList = lstTagInfo;
            //    }
            //}

            //objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(groupid, Convert.ToInt16(jobid));

            //  var attch = _repository.SelectJobAttachment();

            //  int PageSize = 15;
            //  int page;
            //  page = 0;
            ////  objGalleryModel.GalJobAttachmentlst = attch.ToList();
            //  objGalleryModel.GalJobAttachmentlst = attch.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
            //  objGalleryModel.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)attch.Count() / PageSize));
            //  objGalleryModel.CurrentPage = page;

            return PartialView("Controls/JobTracking/_Giftframe", objGalleryModel);
            //   return PartialView("Controls/JobTracking/_Uploadfiles");
        }

        public ActionResult UpdateJobStatus(string jobid)
        {
            try
            {
                if (!string.IsNullOrEmpty(jobid))
                    _repository.UpdateJobStatus(Convert.ToInt32(jobid));
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
                throw;
            }

        }

        #region PushSelectedImages

        /// <summary>
        /// Push images from OMS serv to campaign track server
        /// </summary>
        /// <param name="jobid">To set all images related to job</param>
        /// <returns></returns>
        public ActionResult PushSelectedImages(string jobid)
        {
            List<PublishedFiles> publishedFile = new List<PublishedFiles>();
            try
            {
               // throw new InvalidCastException();
                GalleryModel objGalleryModel = new GalleryModel();
                objGalleryModel.GalleryFolders = _repository.SelectJobAttachmentFolders(Convert.ToInt32(jobid)).ToList();

                var selectJobAttachmentFolders = objGalleryModel.GalleryFolders.Take(2).LastOrDefault();
                if (selectJobAttachmentFolders != null)
                {
                    int groupid = selectJobAttachmentFolders.ROW_ID;

                    //Get selected and yet not uploaded images
                    objGalleryModel.JobAttachmentlist = _repository.GetJobAttachments(groupid, Convert.ToInt16(jobid)).Where(j => j.Selected == true && j.UPLOAD_BY == null).ToList();
                }
                if (!objGalleryModel.JobAttachmentlist.Any())
                {
                    PublishedFiles filespublished = new PublishedFiles();
                    filespublished.Status = false;
                    filespublished.IsPublished = true;
                    filespublished.ErrorMessage = "Files selected are already published";
                    publishedFile.Add(filespublished);
                    return Json(publishedFile, JsonRequestBehavior.AllowGet);
                    /*PublishedFiles filespublished = new PublishedFiles();
                    filespublished.FileName = "No files To Upload";
                    filespublished.Status = false;
                    publishedFile.Add(filespublished);
                    return Json(publishedFile, JsonRequestBehavior.AllowGet);*/
                }
                string campaignTrackAPIURL = System.Configuration.ConfigurationManager.AppSettings["campaignTrackAPIURL"];
                string campaignTrackAPIUsername = System.Configuration.ConfigurationManager.AppSettings["campaignTrackAPIUsername"];
                string campaignTrackAPIPassword = System.Configuration.ConfigurationManager.AppSettings["campaignTrackAPIPassword"];

                dynamic objKey = sendWebRequest(campaignTrackAPIURL + "?id=1&username=" + campaignTrackAPIUsername + "&password=" + campaignTrackAPIPassword);
                dynamic objUploadUrl = sendWebRequest(campaignTrackAPIURL + "?id=56&key=" + objKey.key.ToString() + "&propertyno=" + objGalleryModel.JobAttachmentlist[0].Property_Id);

                List<OrderManagement.Web.Models.UploadFile> files = new List<OrderManagement.Web.Models.UploadFile>();
                //GetJobAttachments jobAttach = objGalleryModel.JobAttachmentlist.FirstOrDefault();
                foreach (GetJobAttachments jobAttach in objGalleryModel.JobAttachmentlist)
                {
                    PublishedFiles filespublished = new PublishedFiles();
                    Stream stream;
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            //  jobAttach.FilePath = "http://cdn1.zerofootprint.com.au.s3-us-west-2.amazonaws.com/ASSETS/11078/Photography/10-Bowman-Street-Aspendale-VIC-3195-Real-Estate-photo-1-medium-8564406.jpg";
                            byte[] data = webClient.DownloadData(jobAttach.FilePath);
                            stream = new MemoryStream(data);
                        }

                        OrderManagement.Web.Models.UploadFile objUploadFile =
                            new OrderManagement.Web.Models.UploadFile();
                        objUploadFile.Name = "file";
                        objUploadFile.Filename = jobAttach.FileName;
                        objUploadFile.ContentType = jobAttach.FileExtension;
                        objUploadFile.Stream = stream;

                        files.Add(objUploadFile);

                        var values = new NameValueCollection
                        {
                            {"key1", "value1"}
                        };

                        string result = UploadFiles(objUploadUrl.uploadurl.ToString(), files, values);
                        _repository.UpdateJobAttachmentUploaded(jobAttach.Row_Id, result);
                        filespublished.FileName = objUploadFile.Filename;
                        filespublished.Status = true;
                        publishedFile.Add(filespublished);
                    }
                    catch (Exception e)
                    {
                       
                        filespublished.FileName = jobAttach.FileName;
                        filespublished.Status = false;
                        publishedFile.Add(filespublished);
                    }
                    finally
                    {

                    }
                }
                return Json(publishedFile, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
               // ErrorLogGeneration.CreateErrorMessage(ex);
                PublishedFiles filespublished = new PublishedFiles();
                filespublished.Status = false;
                filespublished.IsPublished = true;
                filespublished.ErrorMessage = "Error while publishing files to CT.";//+ex.ToString();
                publishedFile.Add(filespublished);
                return Json(publishedFile, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }


        private dynamic sendWebRequest(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            String ver = response.ProtocolVersion.ToString();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonString = reader.ReadLine();
            return JsonConvert.DeserializeObject(jsonString);
        }

        //public byte[] UploadFiles(string address, IEnumerable<UploadFile> files, NameValueCollection values)
        public string UploadFiles(string address, IEnumerable<OrderManagement.Web.Models.UploadFile> files, NameValueCollection values)
        {
            var request = WebRequest.Create(address);
            request.Method = "POST";
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (var requestStream = request.GetRequestStream())
            {
                //// Write the values
                //foreach (string name in values.Keys)
                //{
                //    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                //    requestStream.Write(buffer, 0, buffer.Length);
                //    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                //    requestStream.Write(buffer, 0, buffer.Length);
                //    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                //    requestStream.Write(buffer, 0, buffer.Length);
                //}

                // Write the files
                foreach (var file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    //buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", "application/octet-stream", Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    file.Stream.CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            var response = request.GetResponse();
            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            string resultStatus = httpWebResponse.StatusCode.ToString() + " - " + httpWebResponse.StatusDescription;//HttpStatusCode.OK for Success
            return resultStatus;
            //using (var response = request.GetResponse())
            //using (var responseStream = response.GetResponseStream())
            //using (var stream = new MemoryStream())
            //{
            //    responseStream.CopyTo(stream);
            //    return stream.ToArray();
            //}
        }

        #endregion

        [ValidateInput(false)]
        public JsonResult Event_MailSent(string MailTo, string MailCC, string MailBCC, string MailSubject,
            string MailBody, string MailFrom, string FromEmailDisplayAsText)
        {
            List<EmailTo> EmailTo = new List<EmailTo>();
            List<EmailCC> EmailCC = new List<EmailCC>();
            List<EmailBCC> EmailBCC = new List<EmailBCC>();
            //if (string.IsNullOrEmpty(MailTo))
            //{
            //    MailTo = ConfigurationManager.AppSettings["campaignTrackEmailTo"];
            //}
            string Mailfrm = "";

            if (!string.IsNullOrEmpty(MailFrom))
            {
                Mailfrm = MailFrom;
            }
            else
            {
                Mailfrm=  "dpi.campaigntrack@gmail.com";

            }

            if (string.IsNullOrEmpty(FromEmailDisplayAsText))
            {
                FromEmailDisplayAsText = "Digital Photography Inhouse";
            }
         
            string strEmailTo = MailTo;//"manojsoni80@gmail.com;manojkumar.soni@e-zest.in;manojkumar.soni@e-zest.in;manojkumar.soni@e-zest.in";
            string[] EmailIdTos = strEmailTo.Split(',');
            foreach (string Emailid in EmailIdTos)
            {
                EmailTo objEmailTo = new EmailTo();
                objEmailTo.Email_Id = Emailid;
                EmailTo.Add(objEmailTo);
            }
            if (string.IsNullOrEmpty(MailCC))
            {
                MailCC = ConfigurationManager.AppSettings["campaignTrackEmailCC"];
            }

            string strEmailCC = MailCC;//"manojsoni80@gmail.com;manojkumar.soni@e-zest.in;manojkumar.soni@e-zest.in;manojkumar.soni@e-zest.in";
            if (!string.IsNullOrEmpty(strEmailCC))
            {
                string[] EmailIdCCs = strEmailCC.Split(',');

                foreach (string Emailid in EmailIdCCs)
                {
                    EmailCC objEmailCC = new EmailCC();
                    objEmailCC.Email_Id = Emailid;
                    EmailCC.Add(objEmailCC);
                }
            }
            if (string.IsNullOrEmpty(MailBCC))
            {
                MailBCC = ConfigurationManager.AppSettings["campaignTrackEmailBCC"];
            }

            if (!string.IsNullOrEmpty(MailBCC))
            {
                string strEmailBCC = MailBCC;
                //"manojsoni80@gmail.com;manojkumar.soni@e-zest.in;manojkumar.soni@e-zest.in;manojkumar.soni@e-zest.in";
                string[] EmailIdBCCs = strEmailBCC.Split(',');
                foreach (string Emailid in EmailIdBCCs)
                {
                    EmailBCC objEmailBCC = new EmailBCC();
                    objEmailBCC.Email_Id = Emailid;
                    EmailBCC.Add(objEmailBCC);
                }
            }
            try
            {
                var resposne2 = Email.SendEmailFromMailGunServer(MailSubject, MailBody, EmailTo, EmailCC, EmailBCC, Mailfrm, FromEmailDisplayAsText);
                string resposecode = resposne2.StatusCode.ToString();

                // saving mail content in the Emailinbox table  
                if (resposecode.Equals("OK"))
                {
                    SaveEmail.InsertEmailsToEmailInbox(MailTo, MailFrom, MailCC, MailBCC, MailSubject, MailBody, "sent");
                    return Json("200", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("100", JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception e)
            {
                throw;
            }
            //////////////////////////////////////////////////////////////////////////////////////////
            //return Json("200", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult JobImage(string groupid, string job_Id, string image_Id)
        {
            //if (UserManager.Current() != null)
            //{
            GalleryModel objGalleryModel = new GalleryModel();

            var JobAttachmentlist = _repository.GetJobAttachmentsForMail(Convert.ToInt32(groupid), Convert.ToInt32(job_Id)).ToList();
            int TotalImageCount = JobAttachmentlist.Count();
            int SelectedImageCount = JobAttachmentlist.Where(c => c.Selected == true).Count();

            objGalleryModel.JobAttachmentlist = JobAttachmentlist.Where(m => m.Row_Id == Convert.ToInt32(image_Id)).ToList();

            JobImage objJobImage = new JobImage();
            if (objGalleryModel.JobAttachmentlist.Count > 0)
            {
                objJobImage.ImageURL = objGalleryModel.JobAttachmentlist[0].FilePath;
                objJobImage.Job_Id = Convert.ToInt32(job_Id);
                objJobImage.Group_Id = Convert.ToInt32(groupid);
                objJobImage.Image_Id = Convert.ToInt32(objGalleryModel.JobAttachmentlist[0].Row_Id);
                objJobImage.TotalImageCount = TotalImageCount;
                objJobImage.SelectedImageCount = SelectedImageCount;
                return View(objJobImage);
            }
            return View("PageNotFound");
            //}
            //else {
            //    return RedirectToAction("Login", "Account", Request.Url);
            //}
        }

        public JsonResult GetJobAnnotation(string Row_Id)
        {
            var data = _repository.GetJobAnnotationbyFileId(Convert.ToInt32(Row_Id)).Select(p => new
            {

                Row_id = p.Row_Id,
                MyAnnotaion = p.Annotation,
                Annotaion = "{ x: 0.62, y: 0.7, text: 'Manoj' },{ x: 0.51, y: 0.5, text: 'Vinit' },{ x: 0.40, y: 0.3, text: 'Chris, obviously' }"
            });

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetJobAnnotationForMail(string Row_Id)
        {
            var data = _repository.GetJobAnnotationbyFileIdForMail(Convert.ToInt32(Row_Id)).Select(p => new
            {

                Row_id = p.Row_Id,
                MyAnnotaion = p.Annotation,
                Annotaion = "{ x: 0.62, y: 0.7, text: 'Manoj' },{ x: 0.51, y: 0.5, text: 'Vinit' },{ x: 0.40, y: 0.3, text: 'Chris, obviously' }"
            });

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SaveAnnotation(string Row_Id, string AnnotationText)
        {
            _repository.InsertJobAnnotation(Convert.ToInt16(Row_Id), AnnotationText);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateJobAttachmentTags(string TagSelected, string jobid, string ImageId)
        {
            _repository.UpdateJobAttachmentTags(TagSelected, Convert.ToInt32(ImageId), Convert.ToInt32(jobid));
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateConfirmedImageSelected(string Rowid, string jobId)
        {
            try
            {
                _repository.UpdateSelectedImageConfirmed(Rowid, Convert.ToInt32(jobId));
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { throw ex; }
        }

        public JsonResult LoadAutoCompleteEMails(string Search)
        {
            var emailList = _scheduler.GetEmailAddress("");

            List<EmailTo> EmailTo = new List<EmailTo>();

            foreach (var item in emailList)
            {
                EmailTo objEmail = new EmailTo();
                if (!string.IsNullOrEmpty(item))
                {
                    objEmail.Email_Id = item;
                    EmailTo.Add(objEmail);
                }
            }

            List<string> Emaillist = new List<string>();
            foreach (var item in EmailTo)
            {
                Emaillist.Add(item.Email_Id);
            }

            var result = Emaillist.ToArray();
            return Json(result, JsonRequestBehavior.AllowGet);
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
