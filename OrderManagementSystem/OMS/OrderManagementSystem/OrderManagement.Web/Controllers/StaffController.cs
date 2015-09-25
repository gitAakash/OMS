using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;

namespace OrderManagement.Web.Controllers
{
    public class StaffController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICompanyRepository _repository;
        private readonly IProductGroupRepository _productGroupRepository;
        private readonly IOrderService _orderService;

        public StaffController()
        {
            var userrapo = new UserRepository();
            _userService = new UserService(userrapo);
            _repository = new CompanyRepository();
            _productGroupRepository = new ProductGroupRepository();
            var orderrapo = new OrderRepository();
            _orderService = new OrderService(orderrapo);
        }

        public ActionResult Index()
        {
            return PartialView("Controls/Staff/_Staff");
        }

        public ActionResult Jobtracking()
        {
            IList<OrderTrackingModel> eventTrackings = new List<OrderTrackingModel>();
            var currentuser = UserManager.Current();
            if (currentuser != null)
            {
                eventTrackings = _userService.JobEventTrackings(currentuser.Row_Id);
            }
            return PartialView("Controls/Staff/_Ordertracking", eventTrackings);
        }

        public ActionResult StaffProfile()
        {
            var currentUser = UserManager.Current();
            var profile = new UserProfileModel();
            if (currentUser != null)
            {
               // profile = _userService.UserProfile(currentUser.Row_Id);
                DateTime dt = DateTime.Now;
                profile.OrderTracking = _userService.JobEventTrackings(currentUser.Row_Id).Where(m => m.Created.Date <= dt.Date && m.Created > dt.AddDays(-7)).ToList();
            }

            return PartialView("Controls/Staff/_StaffProfile", profile);
        }

        public ActionResult EditProfile(string userid)
        {
            var model = new UserModel();

            if (!string.IsNullOrEmpty(userid))
            {

                model = _userService.GetUserByid(int.Parse(userid));
            }

            return PartialView("Controls/Staff/_UpdateUser", model);
        }

        public ActionResult Update(UserModel model, HttpPostedFileBase theFile)
        {
            _userService.ClientStaffUpdate(model, theFile);

            var currentUser = UserManager.Current();
            var profile = new UserProfileModel();
            if (currentUser != null)
            {
                DateTime dt = DateTime.Now;

                // User Prodile Model insteed of UserprofileModel
             //   profile = _userService.UserProfile(currentUser.Row_Id);
                profile.OrderTracking = _userService.JobEventTrackings(currentUser.Row_Id).Where(m => m.Created.Date <= dt.Date && m.Created > dt.AddDays(-7)).ToList();
            }
            return PartialView("Controls/Staff/_StaffProfile", profile);
        }

        public ActionResult OrderItems(string orderid)
        {
            var itemlist = new List<OrderItemsModel>();
            if (!string.IsNullOrEmpty(orderid))
            {
                itemlist = _orderService.GetAllOrderItems(int.Parse(orderid)).ToList();
            }
            return PartialView("Controls/Staff/_orderitems", itemlist);
        }

        public ActionResult UploadFiles(string orderid, string status)
        {
            var model = new StatusModel();
            var order = new Order();
            if (!string.IsNullOrEmpty(orderid))
            {
                order = _orderService.GetOrderById(int.Parse(orderid));

                if (!string.IsNullOrEmpty(status))
                {
                    order.Status = status;
                    _orderService.UpdateOrder(order);

                    order = _orderService.GetOrderById(int.Parse(orderid));
                }


                if (order.Status == "Complete")
                    model.OrderStatus = true;
                else
                    model.OrderStatus = false;
            }

            var obj = _userService.GetAllOrderAttachment();

            var model1 = Mapper.Map<IEnumerable<OrderAttachment>, IEnumerable<OrderStatusModel1>>(obj);

            return PartialView("Controls/Staff/_Imagelist", model1);
        }

        [HttpPost]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files, string orderid, string groupname)
        {
            if (!string.IsNullOrEmpty(orderid) && !string.IsNullOrEmpty(groupname))
            {
                foreach (var file in files)
                {
                    _orderService.OrderAttachment(file, orderid, groupname);
                    string gm = file.FileName;
                }
            }
            // The Name of the Upload component is "attachments"

            // Return an empty string to signify success
            return Json(new { status = "OK" }, "text/plain");
        }

        public ActionResult ImageList()
        {
            var obj = _userService.GetAllOrderAttachment();
            var model = Mapper.Map<IEnumerable<OrderAttachment>, IEnumerable<OrderStatusModel1>>(obj);
            return PartialView("Controls/Staff/_Imagelist", model);
        }

        public ActionResult ImageListing(int page)
        {

            var imgmodle = new ImageModel();
            int PageSize = 1;
            var obj = _userService.GetAllOrderAttachment();

            var modelImagelist = Mapper.Map<IEnumerable<OrderAttachment>, IEnumerable<OrderStatusModel1>>(obj).ToList();
            imgmodle.OrderStatusModel1 = modelImagelist.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
            imgmodle.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)modelImagelist.Count() / PageSize));
            imgmodle.CurrentPage = page;
            
            return PartialView("Controls/Staff/_Imagelist", imgmodle);
        }

    }
}
