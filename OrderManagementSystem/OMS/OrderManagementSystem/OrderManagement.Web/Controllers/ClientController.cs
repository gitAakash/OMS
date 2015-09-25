using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderManagement.Web.Controllers
{
    public class ClientController : Controller
    {

        private readonly IUserService _userService;
        private readonly ICompanyRepository _repository;
        private readonly IProductGroupRepository _productGroupRepository;

        public ClientController()
        {
            var userrapo = new UserRepository();
            _userService = new UserService(userrapo);
            _repository = new CompanyRepository();
            _productGroupRepository = new ProductGroupRepository();

        }

        public ActionResult UserProfile()
        {
            var currentUser = UserManager.Current();
            var profile = new ProfileModel();
            if (currentUser != null)
            {
                profile = _userService.UserProfile(currentUser.Row_Id);
            }


            return PartialView("Controls/client/_UserProfile", profile);
        }

        public ActionResult Index()
        {

            return PartialView("Controls/Client/_Client");
        }

        public ActionResult EditProfile(string userid)
        {
            var model = new UserModel();

            if (!string.IsNullOrEmpty(userid))
            {

                model = _userService.GetUserByid(int.Parse(userid));

            }

            return PartialView("Controls/Client/_UpdateUser", model);
        }

        public ActionResult Update(UserModel model, HttpPostedFileBase theFile)
        {

            _userService.ClientStaffUpdate(model, theFile);

            var currentUser = UserManager.Current();
            var profile = new UserProfileModel();
            if (currentUser != null)
            {
               //profile profile = _userService.UserProfile(currentUser.Row_Id);
            }
            return PartialView("Controls/Client/_UserProfile", profile);
        }

        public ActionResult TrackOrder()
        {
            var ordertracklist = _userService.OrderTracking();
            return PartialView("Controls/client/_Ordertracking", ordertracklist);

            // return null;
        }

    }
}
