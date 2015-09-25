using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;
using OrderManagement.Web.Helper.Utilitties;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Kendo.Mvc;

//using Google.Apis.YouTube;

//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Services;
//using Google.Apis.Upload;
//using Google.Apis.Util.Store;
//using Google.Apis.YouTube.v3;
//using Google.Apis.YouTube.v3.Data;
//using Google.YouTube;
//using Google.GData.Client;
//using Google.GData.YouTube;
//using System.Threading;


using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Mvc;

using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.YouTube;
using Google.GData.Extensions.MediaRss;

using YouTubeVideo = Google.YouTube;
using Google.GData.Client;
using Google.GData.YouTube;
using System.Collections.Specialized;


namespace OrderManagement.Web.Controllers
{
    public class VideoController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly IProductScheduleService _repositoryschedule;


        public VideoController()
        {
            var videorepo = new VideoRepository();
            _videoService = new VideoService(videorepo);
            var productschedrapo = new ProductScheduleRepository();
            _repositoryschedule = new ProductScheduleService(productschedrapo);
        }


        public ActionResult Index()
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                var clients = _videoService.GetClients().ToList();
                return PartialView("Controls/Video/_ClientList", clients);
            }
            return null;
        }


        public ActionResult NewClient(string userid)
        {
            var model = new ClientModel();
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    var lstClient = _videoService.GetClients(string.Empty, int.Parse(userid)).ToList();

                    foreach (var item in lstClient)
                    {
                        model.Main_Email = item.Main_Email;
                        model.Name = item.Name;
                        model.Main_URL = item.Main_URL;
                        model.Main_Phone = item.Main_Phone;
                        model.Row_Id = item.Row_Id;
                    }
                }

            }
            return PartialView("Controls/Video/_CreateClient", model);

            //if (!string.IsNullOrEmpty(userid))
            //{
            //    return PartialView("Controls/Video/_CreateClient", model);
            //}
            //else
            //{
            //    //model.IsActive = true;
            //    return PartialView("Controls/Video/_CreateClient", model);
            //}
        }

        public JsonResult CheckUniqeEmail(string Main_Email)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = true;
            //try
            //{
            //    Video video = _userService.RegistrantUser(EmailAddress);
            //    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //    result.Data = true;
            //    if (video != null)
            //        result.Data = false;
            //}
            //catch (Exception ex)
            //{

            //}
            return result;
        }

        [HttpPost]
        public ActionResult AddorUpdate(ClientModel model)
        {
            _videoService.AddOrUpdate(model);
            var ClientlistModel = _videoService.GetClients().ToList();
            return PartialView("Controls/Video/_ClientList", ClientlistModel);
        }

        public ActionResult DeleteClient(string userid)
        {
            _videoService.DeleteUser(userid);
            var ClientlistModel = _videoService.GetClients().ToList();
            return PartialView("Controls/Video/_ClientList", ClientlistModel);
        }

        public ActionResult VideoList(string id)
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                var clients = _videoService.GetVideos(string.Empty, int.Parse(id)).ToList();
                return PartialView("Controls/Video/_VideoList", clients);
            }
            return null;
        }

        public JsonResult IsVideosAvailable(string id)
        {
            JsonVideo objJsonVideo = new JsonVideo();

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                var clients = _videoService.GetVideos(string.Empty, int.Parse(id)).ToList();

                if (clients.Count() > 0)
                {
                    objJsonVideo.IsDataAvail = true;
                }
            }

            return Json(objJsonVideo, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddNewVideos(string clientId, string videoid)
        {
            var model = new VideoModel();
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (!string.IsNullOrEmpty(videoid))
                {
                    var lstViedo = _videoService.GetVideos(string.Empty, int.Parse(videoid)).ToList();

                    //List<string> optionlist = new List<string>();
                    //List<string> optionlistval = new List<string>();

                    foreach (var item in lstViedo)
                    {
                        model.Row_Id = item.Row_Id;
                        model.ClientId = item.Client_Id.Value;
                        model.Reference = item.Reference;
                        model.HostPrimary = item.Host_Primary;
                        model.HostPrimaryLink = item.Host_Primary_Link;
                        model.HostSecondary = item.Host_Secondary;
                        model.HostSecondaryLink = item.Host_Secondary_Link;
                        model.Status = item.Status;
                        model.Comments = item.Comments;

                        //var options = item.Title.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        //optionlist.AddRange(options);


                        //var optionsval = item.Row_Id.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        //optionlistval.AddRange(optionsval);

                    }

                    //model.JobOptionslist = optionlist;
                    //model.JobOptionValue = optionlistval.ToArray();

                }

                else
                {
                    int client_id;
                    bool res = int.TryParse(clientId, out client_id);
                    if (res)
                    {
                        model.ClientId = client_id;
                    }
                }
            }
            return PartialView("Controls/Video/_AddVideo", model);
        }

        public ActionResult Save1(IEnumerable<HttpPostedFileBase> files, string ClientId, string Title, string Reference, string HostPrimary, string HostPrimaryLink, string HostSecondary, string HostSecondaryLink, string Comments, string Status, string JobId) //,string JobId, string groupType, string groupname)
        {
            //, string Title, string Reference, string HostPrimary, string HostPrimaryLink, string HostSecondary, string HostSecondaryLink, string Comments
            // string Title = ""; string Reference = ""; string HostPrimary = ""; string HostPrimaryLink = ""; string HostSecondary = ""; string HostSecondaryLink = ""; string Comments = "";

            // The Name of the Upload component is "files"

            int job_Id;
            bool res = int.TryParse(JobId, out job_Id);
            if (res)
            {
                job_Id = (Convert.ToInt32(JobId));
            }


            try
            {
                if (files != null && (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Status)))
                {
                    foreach (var file in files)
                    {
                        string strImgShowingRepositoryUrl = string.Empty;
                        string strFolder = string.Empty;

                        var fileName = Path.GetFileName(file.FileName);
                        strFolder = System.Configuration.ConfigurationManager.AppSettings["VideoDisplayLocation"];

                        if (Title.Contains("/"))
                        {
                            Title = Title.Replace("/", "-");
                        }

                        strFolder = strFolder + "/" + ClientId + "/" + Title + "/";
                        string FilePath = strFolder;
                        strImgShowingRepositoryUrl = System.Configuration.ConfigurationManager.AppSettings["ImgShowingRepositoryUrl"];


                        strFolder = strFolder + fileName.ToString();

                        if (System.IO.File.Exists(strFolder))
                        {
                            // rename file 

                            string strRandomFileName = Path.GetRandomFileName(); //This method returns a random file name of 11 characters
                            string FileExtension = System.IO.Path.GetExtension(file.FileName);
                            string FileName = System.IO.Path.GetFileName(file.FileName);
                            int fileSize = file.ContentLength;

                            strRandomFileName = strRandomFileName.Replace(".", "") + FileExtension;
                            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + ClientId + "/" + Title + "/" + strRandomFileName.ToString();

                            strFolder = string.Empty;
                            strFolder = FilePath + strRandomFileName;
                            file.SaveAs(strFolder);

                            //var clients = _videoService.GetVideos(string.Empty, int.Parse(id)).ToList();

                            //int client_Id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                            //    string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                            //    string status, bool isDeleted = false

                            _videoService.Insertvideo(Convert.ToInt32(ClientId), Title, FileName, FileExtension, fileSize, strImgShowingRepositoryUrl, Reference, HostPrimary, HostPrimaryLink, HostSecondary, HostSecondaryLink, Comments, Status, false, job_Id);

                            //_repository.InsertJobAttachment(JobId, groupType, groupname, strRandomFileName, FileExtension, fileSize, selectedTags, strImgShowingRepositoryUrl, FileName);

                            // this will rutund the exist file
                        }
                        else
                        {
                            DirectoryInfo diPath = new DirectoryInfo(FilePath);
                            diPath.Create();
                            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + ClientId + "/" + Title + "/" + fileName.ToString();

                            string FileExtension = System.IO.Path.GetExtension(file.FileName);
                            string FileName = System.IO.Path.GetFileName(file.FileName);
                            int fileSize = file.ContentLength;

                            file.SaveAs(strFolder);
                            //     _repository.InsertJobAttachment(JobId, groupType, groupname, file, selectedTags, strImgShowingRepositoryUrl);
                            _videoService.Insertvideo(Convert.ToInt32(ClientId), Title, FileName, FileExtension, fileSize, strImgShowingRepositoryUrl, Reference, HostPrimary, HostPrimaryLink, HostSecondary, HostSecondaryLink, Comments, Status, false, job_Id);

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

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                var clients = _videoService.GetVideos(string.Empty, int.Parse(ClientId)).ToList();
                return PartialView("Controls/Video/_VideoList", clients);



            }
            return null;

            //  return Content("");
        }


        public ActionResult Save(IEnumerable<HttpPostedFileBase> files, VideoModel video, string JobOptionValue) //,string JobId, string groupType, string groupname)
        {
            //, string Title, string Reference, string HostPrimary, string HostPrimaryLink, string HostSecondary, string HostSecondaryLink, string Comments
            // string Title = ""; string Reference = ""; string HostPrimary = ""; string HostPrimaryLink = ""; string HostSecondary = ""; string HostSecondaryLink = ""; string Comments = "";

            // The Name of the Upload component is "files"


            try
            {

                int job_Id;
                bool res = int.TryParse(JobOptionValue, out job_Id);
                if (res)
                {
                    job_Id = (Convert.ToInt32(JobOptionValue));
                }

                if (files != null && (!string.IsNullOrEmpty(video.Title) && !string.IsNullOrEmpty(video.Status)))
                {
                    foreach (var file in files)
                    {
                        string strImgShowingRepositoryUrl = string.Empty;
                        string strFolder = string.Empty;

                        var fileName = Path.GetFileName(file.FileName);
                        strFolder = System.Configuration.ConfigurationManager.AppSettings["VideoUplaodLocation"];

                        if (video.Title.Contains("/"))
                        {
                            video.Title = video.Title.Replace("/", "-");
                        }

                        strFolder = strFolder + "/" + video.ClientId + "/" + video.Title + "/";
                        string FilePath = strFolder;
                        strImgShowingRepositoryUrl = System.Configuration.ConfigurationManager.AppSettings["VideoDisplayLocation"];


                        strFolder = strFolder + fileName.ToString();

                        if (System.IO.File.Exists(strFolder))
                        {
                            // rename file 

                            string strRandomFileName = Path.GetRandomFileName(); //This method returns a random file name of 11 characters
                            string FileExtension = System.IO.Path.GetExtension(file.FileName);
                            string FileName = System.IO.Path.GetFileName(file.FileName);
                            int fileSize = file.ContentLength;

                            strRandomFileName = strRandomFileName.Replace(".", "") + FileExtension;
                            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + video.ClientId + "/" + video.Title + "/" + strRandomFileName.ToString();

                            strFolder = string.Empty;
                            strFolder = FilePath + strRandomFileName;
                            file.SaveAs(strFolder);

                            ////////////////////////////////////////////

                            //                         Google.YouTube.YouTubeRequestSettings settings =
                            //new Google.YouTube.YouTubeRequestSettings("Bobina Channel", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ");
                            //                         Google.YouTube.YouTubeRequest request = new Google.YouTube.YouTubeRequest(settings);


                            //                         Google.YouTube.Video newVideo = new Google.YouTube.Video();
                            //                         newVideo.Title = "test 1";
                            //                         //  newVideo.Tags.Add(new MediaCategory("Gaming", YouTubeNameTable.CategorySchema));
                            //                         newVideo.Keywords = "mstest1 , mstest2";

                            //                         newVideo.Description = "test 3 test 4";
                            //                         newVideo.YouTubeEntry.Private = false;
                            //                         Google.YouTube.Video createdVideo = request.Upload(newVideo);



                            ///////////////////////////////////////////


                            //---------------------------------------------------------------

                            YouTubeRequestSettings settings =
  new YouTubeRequestSettings("Zerofootprint", "532982290458-ua1mk31m7ke3pee5vas9rcr6rgfcmavf.apps.googleusercontent.com", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ");
                            YouTubeRequest request = new YouTubeRequest(settings);

                            //YouTubeRequestSettings settings = new YouTubeRequestSettings("Zerofootprint", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ", "dpi.campaigntrack@gmail.com", "dpi@Zf.2o15");
                            //YouTubeRequest request = new YouTubeRequest(settings);



                            Google.YouTube.Video newVideo = new Google.YouTube.Video();

                            newVideo.Title = "ms test";
                            newVideo.Tags.Add(new MediaCategory("Autos", Google.GData.YouTube.YouTubeNameTable.CategorySchema));
                            newVideo.Keywords = "mstest";
                            newVideo.Description = "mstest";
                            newVideo.YouTubeEntry.Private = false;
                            //newVideo.Tags.Add(new MediaCategory("mydevtag, anotherdevtag",
                            //  YouTubeNameTable.DeveloperTagSchema));

                            // newVideo.YouTubeEntry.Location = new GeoRssWhere(37, -122);
                            // alternatively, you could just specify a descriptive string
                            // newVideo.YouTubeEntry.setYouTubeExtension("location", "Mountain View, CA");

                            newVideo.YouTubeEntry.MediaSource = new Google.GData.Client.MediaFileSource(strFolder,
                              "video/quicktime");

                            Google.YouTube.Video createdVideo = request.Upload(newVideo);

                            //---------------------------------------------------------------



                            //var clients = _videoService.GetVideos(string.Empty, int.Parse(id)).ToList();

                            //int client_Id, string title, string fileName, string fileExtension, int fileSize, string files3location,
                            //    string reference, string host_Primary, string hostPrimaryLink, string hostSecondary, string hostSecondaryLink, string comments,
                            //    string status, bool isDeleted = false


                            if (video.Row_Id > 0)
                            {
                                _videoService.UpdateVideo(Convert.ToInt32(video.ClientId), video.Title, FileName, FileExtension, fileSize, strImgShowingRepositoryUrl,
                                    video.Reference, video.HostPrimary, video.HostPrimaryLink, video.HostSecondary, video.HostSecondaryLink, video.Comments, video.Status, video.Row_Id, job_Id);
                            }
                            else
                            {
                                _videoService.Insertvideo(Convert.ToInt32(video.ClientId), video.Title, FileName, FileExtension, fileSize, strImgShowingRepositoryUrl, video.Reference, video.HostPrimary, video.HostPrimaryLink, video.HostSecondary, video.HostSecondaryLink, video.Comments, video.Status, false, job_Id);
                            }
                            //_repository.InsertJobAttachment(JobId, groupType, groupname, strRandomFileName, FileExtension, fileSize, selectedTags, strImgShowingRepositoryUrl, FileName);

                            // this will rutund the exist file
                        }
                        else
                        {
                            DirectoryInfo diPath = new DirectoryInfo(FilePath);
                            diPath.Create();
                            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + video.ClientId + "/" + video.Title + "/" + fileName.ToString();

                            string FileExtension = System.IO.Path.GetExtension(file.FileName);
                            string FileName = System.IO.Path.GetFileName(file.FileName);
                            int fileSize = file.ContentLength;

                            file.SaveAs(strFolder);


                            /////////////////////////// working except upload////////////////

                            //YouTubeRequestSettings settings = new YouTubeRequestSettings("Zerofootprint", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ", "dpi.campaigntrack@gmail.com", "dpi@Zf.2o15");
                            //YouTubeRequest request = new YouTubeRequest(settings);

                            //Google.YouTube.Video newVideo = new Google.YouTube.Video();

                            //newVideo.Title = "ms test";
                            ////newVideo.Tags.Add(new MediaCategory("Autos", Google.GData.YouTube.YouTubeNameTable.CategorySchema));
                            //newVideo.Keywords = "mstest";
                            //newVideo.Description = "mstest";
                            //newVideo.YouTubeEntry.Private = false;
                            ////newVideo.Tags.Add(new MediaCategory("mydevtag, anotherdevtag",
                            ////  YouTubeNameTable.DeveloperTagSchema));

                            //// newVideo.YouTubeEntry.Location = new GeoRssWhere(37, -122);
                            //// alternatively, you could just specify a descriptive string
                            //// newVideo.YouTubeEntry.setYouTubeExtension("location", "Mountain View, CA");

                            //newVideo.YouTubeEntry.MediaSource = new Google.GData.Client.MediaFileSource(strFolder,
                            //  "video/quicktime");

                            //Google.YouTube.Video createdVideo = request.Upload(newVideo);


                            ////////////////////////////////////////////////

                            //Google.YouTube.YouTubeRequestSettings settings =
                            //new Google.YouTube.YouTubeRequestSettings("Bobina Channel", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ");
                            //YouTubeRequest request = new YouTubeRequest(settings);


                            //Google.YouTube.Video newVideo = new Google.YouTube.Video();
                            //newVideo.Title = "test 1";
                            ////  newVideo.Tags.Add(new MediaCategory("Gaming", YouTubeNameTable.CategorySchema));
                            //newVideo.Keywords = "mstest1 , mstest2";

                            //newVideo.Description = "test 3 test 4";
                            //newVideo.YouTubeEntry.Private = false;
                            //Google.YouTube.Video createdVideo = request.Upload(newVideo);

                            //////////////////////////////////////////////

                            //---------------------------------------------------------

                            //                            var settings1 = new YouTubeRequestSettings("Zerofootprint", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ", "dpi.campaigntrack@gmail.com"
                            //, "dpi@Zf.2o15");
                            //                            var youTubeRequest = new YouTubeRequest(settings1);
                            //                           Google.YouTube.Video  video1 = new Google.YouTube.Video();



                            //                            video1.Title = "First Test 1";
                            //                            video1.Keywords = "O2 Test";
                            //                           // MediaCategory category = new MediaCategory("Technology");
                            //                          //  category.Attributes["scheme"] = YouTubeService.DefaultCategory;
                            //                          //  video1.Tags.Add(category);
                            //                             video1.MediaSource = new Google.GData.Client.MediaFileSource(strFolder, "video/avi");
                            //                             youTubeRequest.Upload(video1);
                            //                            //--------------------------------------------------------------------------------------------




                            //                        YouTubeRequestSettings settings = new YouTubeRequestSettings(
                            //"whatwill come here ?",
                            //"AIzaSyBM80iRA-wuMScreXvI2TfIkTncdt9Mx2s",
                            //"dpi.campaigntrack@gmail.com",
                            //"dpi@Zf.2o15");
                            //                        YouTubeRequest request = new YouTubeRequest(settings);


                            //                        Google.YouTube.Video newVideo = new Google.YouTube.Video();
                            //                        newVideo.Title = "test 1";
                            //                        //  newVideo.Tags.Add(new MediaCategory("Gaming", YouTubeNameTable.CategorySchema));
                            //                        newVideo.Keywords = "mstest1 , mstest2";

                            //                        newVideo.Description = "test 3 test 4";
                            //                        newVideo.YouTubeEntry.Private = false;
                            //                        //newVideo.Tags.Add(new MediaCategory("tag 1, tag 2",
                            //                        //  Google.GData.YouTube.YouTubeNameTable.DeveloperTagSchema));

                            //                        //   newVideo.YouTubeEntry.Location = new GeoRssWhere(37, -122);


                            //                        newVideo.YouTubeEntry.MediaSource = new MediaFileSource(strFolder, "video/quicktime");



                            //                        Google.YouTube.Video createdVideo = request.Upload(newVideo);
                            ///////////////////////////////////////////


                         //   byte[] bytes = System.IO.File.ReadAllBytes(strFolder);


                            CreateYoutubeVideo(video.Title, video.Title, video.Comments, false, strFolder, file.FileName, "video/quicktime");


                            if (video.Row_Id > 0)
                            {
                                _videoService.UpdateVideo(Convert.ToInt32(video.ClientId), video.Title, FileName, FileExtension, fileSize, strImgShowingRepositoryUrl,
                                    video.Reference, video.HostPrimary, video.HostPrimaryLink, video.HostSecondary, video.HostSecondaryLink, video.Comments, video.Status, video.Row_Id, job_Id);
                            }
                            else
                            {
                                _videoService.Insertvideo(Convert.ToInt32(video.ClientId), video.Title, FileName, FileExtension, fileSize, strImgShowingRepositoryUrl, video.Reference, video.HostPrimary, video.HostPrimaryLink, video.HostSecondary, video.HostSecondaryLink, video.Comments, video.Status, false, job_Id);
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

            //var currentUser = UserManager.Current();
            //if (currentUser != null)
            //{
            //    var videomodel = _videoService.GetVideos(string.Empty, (video.ClientId)).ToList();
            //    return PartialView("Controls/Video/_VideoList", videomodel);



            //}
            //return null;

            return Content("");
        }


        public static YouTubeVideo.Video CreateYoutubeVideo(string title, string keywords, string description, bool isPrivate, byte[] content, string fileName, string contentType)
        {
            //YouTubeRequestSettings settings = new YouTubeRequestSettings("Logicum", "YouTubeDeveloperKey", "YoutubeUserName", "YoutubePassword");
      //      YouTubeRequestSettings settings = new YouTubeRequestSettings("Zerofootprint", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ", "dpi.campaigntrack@gmail.com", "dpi@Zf.2o15");
            YouTubeRequestSettings settings =
  new YouTubeRequestSettings("Zerofootprint", "532982290458-ua1mk31m7ke3pee5vas9rcr6rgfcmavf.apps.googleusercontent.com", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ");
            YouTubeRequest request = new YouTubeRequest(settings);

            
        

            YouTubeVideo.Video newVideo = new YouTubeVideo.Video();

            newVideo.Title = title;
            newVideo.Tags.Add(new MediaCategory("Autos", YouTubeNameTable.CategorySchema));
            newVideo.Keywords = keywords;
            newVideo.Description = description;
            newVideo.YouTubeEntry.Private = isPrivate;
            //newVideo.Tags.Add(new MediaCategory("mydevtag, anotherdevtag”, YouTubeNameTable.DeveloperTagSchema));

            // alternatively, you could just specify a descriptive string newVideo.YouTubeEntry.setYouTubeExtension(“location”, “Mountain View, CA”);
            //newVideo.YouTubeEntry.Location = new GeoRssWhere(37, -122);

            Stream stream = new MemoryStream(content);
            newVideo.YouTubeEntry.MediaSource = new MediaFileSource(stream, fileName, contentType);
            YouTubeVideo.Video createdVideo = request.Upload(newVideo);

            return createdVideo;
        }


        public static YouTubeVideo.Video CreateYoutubeVideo(string title, string keywords, string description, bool isPrivate, string FilePath, string fileName, string contentType)
        {
            //YouTubeRequestSettings settings = new YouTubeRequestSettings("Logicum", "YouTubeDeveloperKey", "YoutubeUserName", "YoutubePassword");
            //      YouTubeRequestSettings settings = new YouTubeRequestSettings("Zerofootprint", "AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ", "dpi.campaigntrack@gmail.com", "dpi@Zf.2o15");
            YouTubeRequestSettings settings =
                //new YouTubeRequestSettings("Zerofootprint", "532982290458-ua1mk31m7ke3pee5vas9rcr6rgfcmavf.apps.googleusercontent.com","AI39si5uAJcnGQWqT7bOooT00fTbkCsMjImXlYoyZpkArc49nQvQF-UhxIQDUpwoLxdvf85t97K3wUP2SDrdm1Q8IchJT5mYgQ");

    new YouTubeRequestSettings("mstest","AI39si7-2qSv3Kie2omUhQk4k_IvvOw6IjsBdXZCLCEv5ULvFPG3nVYG4hWNj_U-hLLksdJmWkBjvftq-sA50hgeBVlH_1k6KA","dpi.campaigntrack@gmail.com", "dpi@Zf.2o15");

            YouTubeRequest request = new YouTubeRequest(settings);
            YouTubeVideo.Video newVideo = new YouTubeVideo.Video();
            newVideo.Title = title;
            newVideo.Tags.Add(new MediaCategory("Autos", YouTubeNameTable.CategorySchema));
            newVideo.Keywords = keywords;
            newVideo.Description = description;
            newVideo.YouTubeEntry.Private = isPrivate;
            //newVideo.Tags.Add(new MediaCategory("mydevtag, anotherdevtag”, YouTubeNameTable.DeveloperTagSchema));
            // alternatively, you could just specify a descriptive string newVideo.YouTubeEntry.setYouTubeExtension(“location”, “Mountain View, CA”);
            //newVideo.YouTubeEntry.Location = new GeoRssWhere(37, -122);

            newVideo.YouTubeEntry.MediaSource = new MediaFileSource(FilePath,contentType);
           // newVideo.YouTubeEntry.MediaSource = new MediaFileSource(stream, fileName, contentType);
 
            YouTubeVideo.Video createdVideo = request.Upload(newVideo);

            return createdVideo;
        }



        public ActionResult DeleteVideos(string videoid, string clientId)
        {
            _videoService.DeleteVideos(int.Parse(videoid));
            var VideolistModel = _videoService.GetVideos("", int.Parse(clientId)).ToList();
            return PartialView("Controls/Video/_VideoList", VideolistModel);
        }


        public ActionResult EditVideo(string id)
        {
            var model = new VideoModel();
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var lstClient = _videoService.SelectVideoById(string.Empty, int.Parse(id)).ToList();

                    foreach (var item in lstClient)
                    {
                        model.Row_Id = item.Row_Id;
                        model.ClientId = item.Client_Id.Value;
                        model.Title = item.Title;
                        model.Reference = item.Reference;
                        model.HostPrimary = item.Host_Primary;
                        model.HostPrimaryLink = item.Host_Primary_Link;
                        model.HostSecondary = item.Host_Secondary;
                        model.HostSecondaryLink = item.Host_Secondary_Link;
                        model.Status = item.Status;
                        model.Comments = item.Comments;
                        model.FileName = item.FileName;
                        model.FileExtension = item.FileExtension;
                        model.FileSize = item.FileSize.Value;
                        model.Files3Location = item.File_s3_Location;

                        if (item.JobId != null)
                        {
                            model.jobId = Convert.ToInt32(item.JobId);
                        }
                        else
                        {
                            model.jobId = 0;
                        }

                        model.jobTitle = item.JobTitle;
                        //model.JobIdOptions=item.jo
                    }
                }

            }
            return PartialView("Controls/Video/_AddVideo", model);

        }


        [HttpPost]
        public ActionResult AddorUpdateVideos(VideoModel video)
        {

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                int jobId;
                bool res = int.TryParse(video.JobIdOptions, out jobId);
                if (res)
                {
                    jobId = (Convert.ToInt32(video.JobIdOptions));
                }

                if (video.Row_Id > 0)
                {
                    _videoService.UpdateVideo(Convert.ToInt32(video.ClientId), video.Title, video.FileName, video.FileExtension, video.FileSize, video.Files3Location,
                        video.Reference, video.HostPrimary, video.HostPrimaryLink, video.HostSecondary, video.HostSecondaryLink, video.Comments, video.Status, video.Row_Id, jobId);
                }
                else
                {
                    _videoService.Insertvideo(Convert.ToInt32(video.ClientId), video.Title, video.FileName, video.FileExtension, video.FileSize, video.Files3Location, video.Reference, video.HostPrimary, video.HostPrimaryLink, video.HostSecondary, video.HostSecondaryLink, video.Comments, video.Status, false, jobId);
                }
            }
            return null;
        }


        [HttpPost, FileDownload]
        public FilePathResult DownloadVideos(string VideoId, string clientId)
        {
            string groupname = "Videos"; ; string VideoTitle = string.Empty; string FileName = string.Empty;
            string strDestinationFolderPath = string.Empty;
            string strFolder = System.Configuration.ConfigurationManager.AppSettings["FileUploadPath"];
            string strImgShowingRepositoryUrl = System.Configuration.ConfigurationManager.AppSettings["VideoDisplayLocation"];

            //string strCreateFolder = "~/App_data/uploads/" + Jobid + "/" + groupname + "/";
            int Video_Id = Convert.ToInt16(VideoId);

            //Get Title of Video

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                var VideoList = _videoService.SelectVideoById(string.Empty, int.Parse(VideoId)).ToList();
                foreach (var item in VideoList)
                {
                    VideoTitle = item.Title;
                    FileName = item.FileName;

                }
            }

            if (System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] != null)
            {
                strDestinationFolderPath = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + groupname + "/" + clientId + "/"; ;
            }

            strFolder = strFolder + "/" + groupname + "/" + clientId + "/";

            strImgShowingRepositoryUrl = strImgShowingRepositoryUrl + clientId + "/" + groupname + "/" + VideoTitle.ToString();

            strFolder = strFolder + VideoTitle.ToString() + "/" + FileName.ToString();
            string strFileDestinationFolder;
            strFileDestinationFolder = string.Empty;
            strFileDestinationFolder = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + groupname + "/" + clientId + "/" + VideoTitle.ToString() + "/" + FileName.ToString();

            if (System.IO.File.Exists(strFileDestinationFolder))
            {
                System.IO.File.Delete(strFileDestinationFolder);

                // Copy file from source to app folder [destination folder]
                System.IO.File.Copy(strFolder, strFileDestinationFolder);
                return File(strFileDestinationFolder, "application/jpg", string.Format(FileName, 0));
                // this will returned the exist file
            }
            else
            {
                strFileDestinationFolder = string.Empty;
                strFileDestinationFolder = System.Configuration.ConfigurationManager.AppSettings["FileDestinationFolder"] + groupname + "/" + clientId + "/" + VideoTitle + "/";

                DirectoryInfo diPath = new DirectoryInfo(strFileDestinationFolder);
                diPath.Create();

                strFileDestinationFolder = strFileDestinationFolder + FileName.ToString();
                // Copy file from source to app folder [destination folder]
                System.IO.File.Copy(strFolder, strFileDestinationFolder);
                return File(strFileDestinationFolder, "application/jpg", string.Format(FileName, 0));
            }
        }


        public JsonResult GetJobDataJson1()
        {

            foreach (string key in Request.QueryString.Keys)
            {
               string strkey = key.ToString();
            }


            foreach (String key in Request.QueryString.AllKeys)
            {
                string strkey = "Key: " + key + " Value: " + Request.QueryString[key];
            }


            ////

            NameValueCollection nCollection = Request.QueryString;
            List<JobInfo> lstJobInfo = new List<JobInfo>();
            string filterVal = string.Empty;

            if (nCollection != null)
            {
                if (nCollection.Count > 1)
                {
                    string key = nCollection.GetKey(1);

                    if (key == "filter[filters][0][value]")
                    {
                        filterVal = nCollection.Get(1);


                       
                        var currentUser = UserManager.Current();
                        if (currentUser != null)
                        {
                            var jobs = _videoService.GetAllJobs().ToList();

                            foreach (var item in jobs)
                            {
                                JobInfo objJobInfo = new JobInfo();
                                objJobInfo.JobId = item.Row_Id;
                                objJobInfo.JobTitle = item.JobTitle;
                                lstJobInfo.Add(objJobInfo);
                            }
                        }
                       




                    }
                }
            }
            
            ///////

             return Json(lstJobInfo, JsonRequestBehavior.AllowGet);



        }

        public JsonResult GetJobDataJson()
        {
            NameValueCollection nCollection = Request.QueryString;
            List<JobInfo> lstJobInfo = new List<JobInfo>();
            string filterVal = string.Empty;

            if (nCollection != null)
            {
                if (nCollection.Count > 1)
                {
                    string key = nCollection.GetKey(1);

                    if (key == "filter[filters][0][value]")
                    {
                        filterVal = nCollection.Get(1);

                        var currentUser = UserManager.Current();
                        if (currentUser != null)
                        {
                            var jobs = _videoService.GetAllJobs().ToList();

                            foreach (var item in jobs)
                            {
                                JobInfo objJobInfo = new JobInfo();
                                objJobInfo.JobId = item.Row_Id;
                                objJobInfo.JobTitle = item.JobTitle;
                                lstJobInfo.Add(objJobInfo);
                            }
                        }
                    }
                }

                else
                {
                    var currentUser = UserManager.Current();
                    if (currentUser != null)
                    {
                        var jobs = _videoService.GetAllJobs().ToList();

                        foreach (var item in jobs)
                        {
                            JobInfo objJobInfo = new JobInfo();
                            objJobInfo.JobId = item.Row_Id;
                            objJobInfo.JobTitle = item.JobTitle;
                            lstJobInfo.Add(objJobInfo);
                        }
                    }
                }
            }

            return Json(lstJobInfo, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UplaodYouTube()
        {


            //YouTubeRequestSettings settings = new YouTubeRequestSettings(
            //     "whatwill come here ?",
            //     "my api key",
            //     "my youtube login email",
            //     "my youtube login password");
            //YouTubeRequest request = new YouTubeRequest(settings);


            return null;

        }


    }
}
