using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
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
    public class UserProfileController : Controller
    {

        private readonly IUserService _userService;
        private readonly ICompanyRepository _repository;
        private readonly IProductGroupRepository _productGroupRepository;

        public UserProfileController()
        {
            var userrapo = new UserRepository();
            _userService = new UserService(userrapo);
            _repository = new CompanyRepository();
            _productGroupRepository = new ProductGroupRepository();
        }

        [HttpGet]
        public ActionResult UserProfile()
        {
            UserProfileModel model = new UserProfileModel();
            return PartialView("Controls/User/_UserProfile", model);

        }

        [HttpPost]
        public ActionResult UserProfile(UserProfileModel model, HttpPostedFileBase theFile)
        {
            _userService.AddOrUpdateAttachment(model, theFile);

            return View();
        }
        
        [HttpPost]
        public ActionResult UserProfile1(UserProfileModel model, HttpPostedFileBase theFile)
        {
            if (theFile != null && theFile.ContentLength > 0)
            {

                byte[] data;
                using (Stream inputStream = theFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    // MemoryStream memoryStream = new MemoryStream();
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
            }


            if (theFile != null && theFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(theFile.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                theFile.SaveAs(path);
            }

            return PartialView("Controls/User/_UserProfile", model);
        }

        public ActionResult Thumbnail(int width, int height)
        {
            // TODO: the filename could be passed as argument of course
            var imageFile = Path.Combine(Server.MapPath("~/App_Data/uploads"), "GridCheckbox.png");
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

        public ActionResult ImageShow(int width, int height)
        {
            var imageFile = Path.Combine(Server.MapPath("~/App_Data/uploads"), "GridCheckbox.png");
            System.Drawing.Image image = System.Drawing.Image.FromFile(imageFile);
            byte[] imageByte = ImageToByteArraybyImageConverter(image);
            // return imageByte;
            return File(imageByte, "image/png"); // adjust content type appropriately
        }
        
        private static byte[] ImageToByteArraybyImageConverter(System.Drawing.Image image)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] imageByte = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
            return imageByte;
        }
        
        public ActionResult DatabaseImage(string id, int width, int height)
        {
            //    int userId = 1031;
            var imageByte = _userService.GetAttachmentByUserId(int.Parse(id));

            return File(imageByte, "image/png"); // adjust content type appropriately
        }
        
        public ActionResult ImageList()
        {


            var obj = _userService.GetAllOrderAttachment();


            var model = Mapper.Map<IEnumerable<OrderAttachment>, IEnumerable<OrderStatusModel1>>(obj);


            return View(model.Take(3));
        }

        public ActionResult Products_Read([DataSourceRequest] DataSourceRequest request)
        {

            var obj = _userService.GetAllOrderAttachment();


            var model = Mapper.Map<IEnumerable<OrderAttachment>, IEnumerable<OrderStatusModel1>>(obj);

            return Json(model.ToDataSourceResult(request));
        }

    }
}



