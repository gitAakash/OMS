using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.UI;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace OrderManagement.Web.Controllers
{
    public class UserController : Controller
    {
      
        private readonly IUserService _userService;
        private readonly ICompanyRepository _repository;
        private readonly IProductGroupRepository _productGroupRepository;
        private readonly IProductScheduleService _repositoryschedule;
        public UserController()
        {
            var userrapo = new UserRepository();
            _userService = new UserService(userrapo);
            _repository = new CompanyRepository();
            _productGroupRepository = new ProductGroupRepository();
            var productschedrapo = new ProductScheduleRepository();
            _repositoryschedule = new ProductScheduleService(productschedrapo);
       
        }

        public ActionResult Index()
        {
            var userlistModel = _userService.GetAllUsersBySp().ToList();

         // var userlistModel=  _userService.GetAllUsers().ToList();
          return PartialView("Controls/User/_UserList", userlistModel);
          
        }

        public ActionResult NewUser(string userid)
        {
            var model = new UserModel();
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    model = _userService.GetUserByid(int.Parse(userid));
                }
                var companylst = _repository.SelectAll().Where(c => c.Org_Id == currentUser.OrgId);

          
                model.ProductGrouplist = _productGroupRepository.GetProductgroupBySp(currentUser.OrgId,null).ToList();
                model.Companylist = companylst.OrderBy(m=>m.XeroName).ToList();
                var userTypelist = _userService.GetUserTypes();
                if (userTypelist.Count > 0)
                {
                    model.UserTypelist = userTypelist.ToList();
                }
                model.Calendarlist = _userService.GetAllCalendars(int.Parse(currentUser.OrgId.ToString())).ToList();
                model.Colorlist = _repositoryschedule.GetAllColors();
            }
            if (!string.IsNullOrEmpty(userid))
            {
                return PartialView("Controls/User/_UpdateUser", model);
            }
            else
            {
                model.IsActive = true;
                  return PartialView("Controls/User/_CreateUser", model);
            }
          
        }

        [HttpPost]
        public ActionResult AddorUpdate(UserModel model, HttpPostedFileBase theFile)
        {
            _userService.AddOrUpdate(model, theFile);
          //  var userlistModel = _userService.GetAllUsers();
           // return PartialView("Controls/User/_UserList", userlistModel);

            var userlistModel = _userService.GetAllUsersBySp().ToList();
            // var userlistModel=  _userService.GetAllUsers().ToList();
            return PartialView("Controls/User/_UserList", userlistModel);
        }

        public JsonResult CheckUniqeEmail(string EmailAddress)
        {
            JsonResult result = new JsonResult();
            try
            {
                User user = _userService.RegistrantUser(EmailAddress);
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                result.Data = true;
                if (user != null)
                    result.Data = false;
            }
            catch (Exception ex)
            {
               
            }
            return result;
        }

        public ActionResult DeleteUser(string userid)
        {
            _userService.DeleteUser(userid);
           // var userlistModel = _userService.GetAllUsers();
           // return PartialView("Controls/User/_UserList", userlistModel);

            var userlistModel = _userService.GetAllUsersBySp().ToList();
            return PartialView("Controls/User/_UserList", userlistModel);
        }

        public ActionResult UserProfile(string userid)
        {
             var profile = new ProfileModel();
             if (!string.IsNullOrEmpty(userid))
            {

                profile = _userService.UserProfile(int.Parse(userid));
            }

            return PartialView("Controls/User/_UserProfile", profile);
        }

        public ActionResult UserActivities()
        {
           var activities = _userService.AlluserActivities().ToList();
           return PartialView("Controls/User/_UserAcivities", activities);

        }


        
        public ActionResult GetDatabaseImage(string id, int width=0, int height=0)
         {
             byte[] databyte = null;

             if (!string.IsNullOrEmpty(id))
             {
                 databyte = _userService.GetAttachmentByUserId(int.Parse(id));

             }
             if (databyte==null)
             {

                 //width = 100;
                 //height = 44;

                 width = 150;
                 height = 84;
                 var imageFile = Path.Combine(Server.MapPath("~/App_Data/uploads"), "DPIlogo.jpg");
                 using (var srcImage = Image.FromFile(imageFile))
                 using (var newImage = new Bitmap(width, height))
                 using (var graphics = Graphics.FromImage(newImage))
                 using (var stream = new MemoryStream())
                 {
                     graphics.SmoothingMode = SmoothingMode.AntiAlias;
                     graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                     graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                     graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height));
                     newImage.Save(stream, ImageFormat.Png);
                     return File(stream.ToArray(), "image/png");

                 }

             }
            
             // adjust content type appropriately
            return File(databyte, "image/png"); 
          
        }

        private static byte[] ImageToByteArraybyImageConverter(Image image)
         {
             var imageConverter = new ImageConverter();
             byte[] imageByte = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
             return imageByte;
         }

    }
}
