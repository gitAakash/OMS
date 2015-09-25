using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models.Repository;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Transactions;
namespace OrderManagement.Web.Models.ServiceRepository
{
    public interface IUserService
    {
        IList<SpGetAllUsers> GetAllUsersBySp();
        IList<UserModel> GetAllUsers();
        IList<User> AllUsers();
        IList<UserType> GetUserTypes();
        void DeleteUser(string userid);
        int AddOrUpdate(UserModel obj, HttpPostedFileBase theFile);
        User RegistrantUser(string email);
        UserModel GetUserByid(int id);
        byte[] GetAttachmentByUserId(int userid);
        void AddOrUpdateAttachment(UserProfileModel model, HttpPostedFileBase theFile);
        IList<Calendar> GetAllCalendars(int orgid);
        //UserProfileModel UserProfile(int userid);
           ProfileModel UserProfile(int userid);
        // IList<OrderAttachment> OrderAttachment();
        IList<OrderTrackingModel> JobEventTrackings(int userid);
        //IList<OrderStatusModel> AlluserActivities();
        void ClientStaffUpdate(UserModel obj, HttpPostedFileBase theFile);
        IList<GetAllUsersJobStatus> AlluserActivities();
        IList<OrderTrackingModel> OrderTracking();
        IEnumerable<OrderAttachment> GetAllOrderAttachment();
        IList<JobStatus> GetjobStatusByUserId(int userid,int orgid); 
        void Dispose();
    }

    public class UserService : IUserService
    {
        private IUserRepository _repository;
        private ICalendarRepository _calendarRepository;
        private ICompanyRepository _companyRepository;
        private IProductGroupRepository _productGroupRepository;
        private readonly IProductScheduleService _repositoryschedule;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
            _companyRepository = new CompanyRepository();
            _calendarRepository = new CalendarRepository();
            _productGroupRepository = new ProductGroupRepository();
            var productschedrapo = new ProductScheduleRepository();
            _repositoryschedule = new ProductScheduleService(productschedrapo);
        }

        public IEnumerable<OrderAttachment> GetAllOrderAttachment()
        {

            return _repository.GetAllOrderAttachment();
        }
        public IList<UserModel> GetAllUsers()
        {
            try
            {
                var usrModellist = new List<UserModel>();


                var currentuser = UserManager.Current();
                if (currentuser != null)
                {
                  //  var userlist = _repository.GetAll().Where(c => c.OrgId == currentuser.OrgId && c.IsDeleted == false).ToList();
                    var userlist =
                       _repository.GetAll()
                                  .Where(c => c.OrgId == currentuser.OrgId && c.IsDeleted == false)
                                  .OrderByDescending(m=>m.Created)
                                  .ToList();


                    var companylist = _companyRepository.SelectAll();
                    var getAllUserProductGroups = _repository.GetAllUserProductGroups();
                    var productGroupRepository =_productGroupRepository.GetAllProductgroup();
                    var getUserType = _repository.GetUserType();
                    foreach (var item in userlist)
                    {
                        var usrModel = new UserModel();
                        if (item.CompanyId != 0 && item.CompanyId != null)
                        {
                          //  var compName = _companyRepository.GetById(item.CompanyId.ToString()).XeroName;
                            if (companylist != null && companylist.ToList().Count>0)
                            {
                                usrModel.Company = companylist.FirstOrDefault(m => m.Row_Id == item.CompanyId).XeroName;
                            }
                           
                           // usrModel.Company = compName;

                        }

                      //  var usergrplist = _repository.GetAllUserProductGroups().Where(m => m.UserId == item.Row_Id).ToList();
                        var usergrplist = getAllUserProductGroups.Where(m => m.UserId == item.Row_Id).ToList();

                        // string grp = string.Join(", ", from itemgrp in usergrplist select itemgrp.);
                        string grpname = string.Empty;
                        if (usergrplist.Count > 0)
                        {
                            foreach (var usrgrp in usergrplist)
                            {
                              //  var groupobj = _productGroupRepository.GetAllProductgroup().FirstOrDefault(m => m.Row_Id == usrgrp.ProductGroupId);
                                if (productGroupRepository != null && productGroupRepository.ToList().Count >0)
                                {
                                      var groupobj = productGroupRepository.FirstOrDefault(m => m.Row_Id == usrgrp.ProductGroupId);
                                      if (groupobj != null)
                                      {
                                          grpname = grpname + ", " + groupobj.Name;
                                      }

                                }
                              

                               
                            }



                            if (!string.IsNullOrEmpty(grpname))
                            {

                                usrModel.InternalGroup = grpname.TrimStart(',');
                            }
                        }


                       // var userrole = _repository.GetUserType().FirstOrDefault(usr => usr.Row_Id == item.UserType);

                        if (getUserType != null && getUserType.ToList().Count>0)
                        {
                            var userrole = getUserType.FirstOrDefault(usr => usr.Row_Id == item.UserType);

                            if (userrole != null)
                            {
                                usrModel.Role = userrole.Name;
                            }
                        }
                      
                        usrModel.EmailAddress = item.EmailAddress;
                        usrModel.FullName = item.FirstName + " " + item.LastName;
                        usrModel.FirstName = item.FirstName;
                        usrModel.LastName = item.LastName;
                        usrModel.Row_Id = item.Row_Id;
                        if (item.IsActive.HasValue)
                        {
                            usrModel.IsActive = item.IsActive.Value;
                        }

                        usrModellist.Add(usrModel);
                    }



                    //  var userlistModel = Mapper.Map<IList<User>, List<UserModel>>(userlist);

                    return usrModellist;
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }



            return null;

        }

        public void Dispose()
        {
            _repository.Dispose();
        }
        public IList<UserType> GetUserTypes()
        {
            return _repository.GetUserType().ToList();

        }

        public int AddOrUpdate(UserModel obj, HttpPostedFileBase theFile)
        {
            try
            {
                string colorid = string.Empty;
                if (obj != null)
                {
                    var currentuser = UserManager.Current();
                    User usr = new User();
                    if (currentuser != null)
                    {
                       // using (var transaction = new TransactionScope())
                       // {
                            User userobj = new User();

                            if (obj.Row_Id > 0)
                            {
                                  userobj = _repository.GetById(obj.Row_Id);
                                string ExistingPwd = userobj.Password;

                                if (!ExistingPwd.Equals(obj.Password))
                                {
                                    obj.Password = Cryptography.Encrypt(obj.Password);
                                }
                            }
                            else
                            {
                                obj.Password = Cryptography.Encrypt(obj.Password);
                            }

                            obj.OrgId = int.Parse(currentuser.OrgId.ToString());

                            //if (!string.IsNullOrEmpty(obj.ColorCode))
                            //{

                            //    colorid = _repositoryschedule.GetAllColors().SingleOrDefault(col => col.Color == obj.ColorCode).Row_Id;
                            //}

                            obj.IsDeleted = false;
                            
                            if (obj.Row_Id != null && obj.Row_Id != 0)
                            {
                                obj.Updated = DateTime.Now;
                                if (userobj.Created != null) 
                                   obj.Created = userobj.Created.Value;
                                Removerecordbyusertype(obj, userobj);
                                var user = Mapper.Map<UserModel, User>(obj);
                                usr = _repository.Update(user);
                            }
                            else
                            {
                                obj.Created = DateTime.Now;
                                obj.Updated = DateTime.Now;
                                var user = Mapper.Map<UserModel, User>(obj);
                                usr = _repository.Add(user);
                            }
                            if (usr != null)
                            {
                                if (theFile != null)
                                    AddorUpdateprofileImage(usr.Row_Id, theFile);

                                if (!string.IsNullOrEmpty(obj.Group))
                                    ProductUsergroup(usr, obj.Group);

                                if (!string.IsNullOrEmpty(obj.Calendar))
                                {
                                    AddorUpdateUsercalender(usr.Row_Id, obj.Calendar, obj.ColorCode);
                                }
                            }
                         //   transaction.Complete();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }

            return 1;

        }


        public IList<SpGetAllUsers> GetAllUsersBySp()
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                var allusers = _repository.GetAllUsersBySp(currentUser.OrgId.Value).ToList();

                var usersGroupList = allusers.GroupBy(p => p.Row_Id).Select(g => new SpGetAllUsers
                {

                    Row_Id = g.Key,
                    FirstName = g.Select(p => p.FirstName).FirstOrDefault(),
                    LastName = g.Select(p => p.LastName).FirstOrDefault(),
                    EmailAddress = g.Select(p => p.EmailAddress).FirstOrDefault(),
                    UserRoll = g.Select(p => p.UserRoll).FirstOrDefault(),
                    XeroName = g.Select(p => p.XeroName).FirstOrDefault(),
                    CompanyId = g.Select(p => p.CompanyId).FirstOrDefault(),
                    IsActive = g.Select(p => p.IsActive).FirstOrDefault(),
                    GroupsName = string.Join(",", g.Select(p => p.GroupsName)),
                    IsDeleted = g.Select(p => p.IsDeleted).FirstOrDefault(),
                    ProductGroupId = g.Select(p => p.ProductGroupId).FirstOrDefault()
                }).ToList();



                //foreach (var itm in usersGroupList)
                //{
                //    itm.FirstName=s
                //

                return usersGroupList;
            }

            return null;
        }

        public User RegistrantUser(string email)
        {
            var user = new User();
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    user = _repository.GetAll().FirstOrDefault(e => e.EmailAddress == email);
                }
            }
            catch (Exception ex)
            {

            }
            return user;
        }

        public UserModel GetUserByid(int id)
        {
            var objUser = new User();
            var userModel = new UserModel();
            try
            {
                var user = _repository.GetById(id);

                if (user != null)
                {

                    userModel = Mapper.Map<User, UserModel>(user);
                    var usrprodgroup = _repository.GetAllUserProductGroups().Where(u => u.UserId == user.Row_Id).ToList();

                    var builder = new StringBuilder();
                    foreach (var prodgrp in usrprodgroup)
                    {
                        builder.Append(prodgrp.ProductGroupId).Append(",");
                    }
                    userModel.Group = builder.ToString().TrimEnd(new char[] { ',' });


                    var usercalender = _calendarRepository.GetCalendarUserByUserId(user.Row_Id);
                    if (usercalender != null)
                    {
                        userModel.Calendar = usercalender.CalendarId.ToString();
                        userModel.ColorCode = usercalender.Color;
                    }

                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return userModel;
        }

        public IList<Calendar> GetAllCalendars(int orgid)
        {
            return _calendarRepository.GetAll(orgid).ToList();

        }

        public void AddOrUpdateAttachment(UserProfileModel model, HttpPostedFileBase theFile)
        {
            try
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


                Attachment obj = new Attachment();
                obj.UserId = 1033;
                obj.Buffer = data;
                _repository.AddAttachment(obj);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        public void ClientStaffUpdate(UserModel obj, HttpPostedFileBase theFile)
        {
            User usr = new User();
            try
            {
                if (obj != null)
                {
                    if (obj.Row_Id != 0)
                    {
                        usr = _repository.GetById(obj.Row_Id);
                    }


                    using (var transaction = new TransactionScope())
                    {
                        usr.FirstName = obj.FirstName;
                        usr.LastName = obj.LastName;
                        usr.Password = obj.Password.Trim();
                        usr.AboutMe = obj.AboutMe;
                        usr = _repository.Update(usr);

                        if (usr != null)
                        {
                            if (theFile != null)
                                AddorUpdateprofileImage(usr.Row_Id, theFile);

                        }
                        transaction.Complete();
                    }






                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }

        }

        public IList<GetAllUsersJobStatus> AlluserActivities()
       {

           var activities = new List<GetAllUsersJobStatus>();
           try
           {
               var currentUser = UserManager.Current();
               if (currentUser != null)
               {
                activities =_repository.AlluserActivities(currentUser.OrgId.Value).ToList();
               }

           }
           catch (Exception ex)
           {

               string msg = ex.Message;
           }

           return activities;
       }

        //public IList<OrderStatusModel> AlluserActivities()
        //{
        //    var currentuser = UserManager.Current();
        //    var order = new List<Order>();
        //    var statusmodel = new OrderStatusModel();
        //    var orderstatuslst = new List<OrderStatusModel>();
        //    var orderAttachments = new List<OrderAttachment>();
        //    try
        //    {
        //        var alluser = _repository.GetAll().Where(m => m.OrgId == currentuser.OrgId).ToList();
        //        if (alluser.Count > 0)
        //        {
        //            foreach (var usr in alluser)
        //            {
        //                var ordstatus = _repository.OrderStatus(int.Parse(currentuser.OrgId.ToString())).FirstOrDefault(m => m.User_Id == usr.Row_Id);
        //                if (ordstatus != null)
        //                {
        //                    statusmodel = Mapper.Map<OrderStatus, OrderStatusModel>(ordstatus);
        //                    statusmodel.User = usr;
        //                    var usercalender = _calendarRepository.GetCalendarUserByUserId(usr.Row_Id);
        //                    if (usercalender != null)
        //                    {
        //                        var eventTracking = _calendarRepository.GetEventTrackinglist(int.Parse(usercalender.CalendarId.ToString()), usr.OrgId.Value).DistinctBy(m => m.OrderId).ToList();

        //                        foreach (var ord in eventTracking)
        //                        {
        //                            var orderlst = _companyRepository.GetAllOrders().Where(m => m.OrderId == ord.OrderId).ToList();
        //                            if (orderlst.Count > 0)
        //                            {
        //                                order.AddRange(orderlst);
        //                            }

        //                        }
        //                        if (order.Count > 0)
        //                        {
        //                            foreach (var orderitem in order)
        //                            {
        //                                var ordattach =
        //                                    _repository.OrderAttachment(usr.OrgId.Value)
        //                                        .Where(m => m.Order_Id == orderitem.Row_Id)
        //                                        .ToList();
        //                                if (ordattach.Count > 0)
        //                                {
        //                                    orderAttachments.AddRange(ordattach);
        //                                }

        //                            }
        //                        }
        //                        statusmodel.OrderAttachment = orderAttachments;

        //                    }

        //                }

        //            }

        //            orderstatuslst.Add(statusmodel);

        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        string msg = ex.Message;
        //    }

        //    return orderstatuslst;

        //}

        public void DeleteUser(string userid)
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    var user = _repository.GetById(int.Parse(userid));
                    if (user!=null)
                    {
                        user.IsDeleted = true;
                        user.Updated = DateTime.Now;
                        _repository.Update(user);
                    }

                }
              

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }


        }

        public ProfileModel UserProfile(int userid)
        {
            var profile = new ProfileModel();
            try
            {
                var currentUser = UserManager.Current();

                var user = _repository.GetById(userid);
                if (user != null)
                {
                    if (currentUser != null)
                    profile.JobStatus=  _repository.GetjobStatusByUserId(user.Row_Id, currentUser.OrgId.Value);
                    profile.User = user;

                }

            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return profile;
        }

        //Neet to remove
        //public UserProfileModel UserProfile1(int userid)
        //{
        //    var profile = new UserProfileModel();
        //    var order = new List<Order>();

        //    var orderstatus = new OrderStatus();
        //    try
        //    {
        //        var user = _repository.GetById(userid);
        //        if (user != null)
        //        {
        //            //profile.UserId = user.Row_Id.ToString();
        //            //profile.AboutMe = user.AboutMe;
        //            //profile.FullName = string.Format("{0}{1}{2}", user.FirstName, " ", user.LastName);
        //            profile.User = user;
        //            profile.OrderStatuslst = _repository.OrderStatus(user.OrgId.Value).Where(m => m.User_Id == user.Row_Id).ToList();

        //            //  var usercalender = _calendarRepository.GetCalendarUserByUserId(user.Row_Id);
        //            //  if (usercalender != null)
        //            //{
        //            //     var eventTracking = _calendarRepository.GetEventTrackinglist(int.Parse(usercalender.CalendarId.ToString()), user.OrgId.Value).ToList();
        //            //     var eventorderlst =   eventTracking.Select(m => m.OrderId).Distinct().ToList();

        //            //     if (eventorderlst.Count>0)
        //            //    {

        //            //        foreach (var ord in eventorderlst)
        //            //        {
        //            //            var orderlst = _companyRepository.GetAllOrders().Where(m => m.OrderId == ord).ToList();
        //            //            order.AddRange(orderlst);
        //            //        }
        //            //    }



        //            // profile.EventTracking = eventTracking.OrderByDescending(x => x.StartDate).ToList();

        //            //if (order.Count > 0)
        //            //{


        //            //    foreach (var item in order)
        //            //    {
        //            //        var orderAttach = _repository.OrderAttachment(user.OrgId.Value).Where(a=>a.Order_Id==item.Row_Id).ToList();

        //            //      //  var count = orderAttach.Count();
        //            //       // var Orderid = item.OrderId;
        //            //    }



        //            // }
        //            // profile.Order = order;
        //            //  }



        //            // var ordattach = _repository.OrderAttachment(int.Parse(user.OrgId.ToString()));



        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        string msg = ex.Message;
        //    }

        //    return profile;
        //}

        public byte[] GetAttachmentByUserId(int userid)
        {
            var attachment = _repository.GetAttachmentByUserId(userid);
            if (attachment != null)
            {
                return attachment.Buffer;
            }

            return null;

        }

        public IList<OrderTrackingModel> JobEventTrackings(int userid)
        {
            var eventTrackings = new List<EventTracking>();
            IList<OrderTrackingModel> trackinglst = new List<OrderTrackingModel>();

            var order = new List<Order>();
            try
            {
                var user = _repository.GetById(userid);
                if (user != null)
                {

                    var usercalender = _calendarRepository.GetCalendarUserByUserId(user.Row_Id);
                    if (usercalender != null)
                    {
                        eventTrackings = _calendarRepository.GetEventTrackinglist(int.Parse(usercalender.CalendarId.ToString()), user.OrgId.Value).ToList();
                        eventTrackings = eventTrackings.DistinctBy(m => m.OrderId).ToList();
                        foreach (var ord in eventTrackings)
                        {
                            var orderlst = _companyRepository.GetAllOrders().Where(m => m.OrderId == ord.OrderId).ToList();

                            foreach (var orderitem in orderlst)
                            {
                                var property =
                                    _companyRepository.GetAllProperty().FirstOrDefault(m => m.Row_Id == orderitem.Property_Id);
                                if (property != null)
                                {
                                    var orderlstModel = Mapper.Map<Order, OrderTrackingModel>(orderitem);
                                    orderlstModel.CompanyName = property.Company.XeroName;
                                    orderlstModel.PropertyName = property.Name;
                                    trackinglst.Add(orderlstModel);
                                }
                            }

                        }

                        trackinglst = trackinglst.OrderByDescending(m => m.Created).ToList();
                    }

                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return trackinglst;
        }

        public IList<JobStatus> GetjobStatusByUserId(int userid, int orgid)
        {
           return _repository.GetjobStatusByUserId(userid, orgid);
        }

        private void ProductUsergroup(User usr, string prodgrp)
        {
            if (!string.IsNullOrEmpty(prodgrp))
            {
                var lstprodgrp = _repository.GetAllUserProductGroups().Where(pg => pg.UserId == usr.Row_Id).ToList();
                if (lstprodgrp.Count > 0)
                {
                    foreach (var item in lstprodgrp)
                    {
                        if (item != null)
                        {
                            _repository.DeleteUserProductGroup(item.Row_Id);
                        }

                    }
                }
                string[] groups = prodgrp.Split(',');
                foreach (var grp in groups)
                {
                    if (!string.IsNullOrEmpty(grp))
                    {
                        var usrprodgrp = new UserProductGroup();
                        usrprodgrp.UserId = usr.Row_Id;
                        usrprodgrp.ProductGroupId = int.Parse(grp);
                        usrprodgrp.Created = DateTime.Now;
                        _repository.AddUserProductGroup(usrprodgrp);
                    }

                }
            }

        }

        private void AddorUpdateprofileImage(int userid, HttpPostedFileBase theFile)
        {

            if (theFile != null)
            {
                byte[] data;
                //using (Stream inputStream = theFile.InputStream)
                //{
                //    var memoryStream = inputStream as MemoryStream;
                //    if (memoryStream == null)
                //    {
                //        memoryStream = new MemoryStream();
                //        inputStream.CopyTo(memoryStream);
                //    }
                //    data = memoryStream.ToArray();
                //}

                int width = 100;
                int height = 44;

                // var imageFile = Path.Combine(Server.MapPath("~/App_Data/uploads"), "GridCheckbox.png");

                using (var srcImage = Image.FromStream(theFile.InputStream))

                //  var image = Image.FromStream(httpPostedFileBase.InputStream, true, true)

                using (var newImage = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(newImage))
                using (var stream = new MemoryStream())
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height));
                    newImage.Save(stream, ImageFormat.Png);
                    data = stream.ToArray();
                    // return File(stream.ToArray(), "image/png");
                }



                var attachment = _repository.GetAttachmentByUserId(userid);
                if (attachment != null)
                {
                    _repository.DeleteAttachment(attachment.Row_Id);
                    //   attachment.Buffer = data;
                    //_repository.UpdateAttachment(attachment);
                }
                var attmnt = new Attachment();
                attmnt.UserId = userid;
                attmnt.Buffer = data;
                _repository.AddAttachment(attmnt);


            }

        }

        private void AddorUpdateUsercalender(int userid, string claendarid,string color)
        {
            try
            {

                if (!string.IsNullOrEmpty(claendarid) && userid != 0)
                {

                    var usercalender = _calendarRepository.GetCalendarUserByUserId(userid);
                    if (usercalender != null)
                    {
                        usercalender.CalendarId = int.Parse(claendarid);
                        usercalender.Color = color;
                        _calendarRepository.UpdateCalendarUser(usercalender);
                    }
                    else
                    {
                        var objcal = new CalendarUser();
                        objcal.CalendarId = int.Parse(claendarid);
                        objcal.UserId = userid;
                        objcal.Color = color;
                        objcal.Created = DateTime.Now;
                        _calendarRepository.SaveCalendarUser(objcal);


                    }
                }


            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

        }

        public IList<OrderTrackingModel> OrderTracking()
        {
            IList<OrderTrackingModel> trackinglst = new List<OrderTrackingModel>();
            try
            {
                var tracking = new OrderTrackingModel();

                var currentuser = UserManager.Current();
                var company = _companyRepository.GetById(currentuser.CompanyId.ToString());
                var property = _companyRepository.GetAllProperty().Where(m => m.Company_Id == currentuser.CompanyId).ToList();
                if (property.Count > 0)
                {
                    foreach (var item in property)
                    {
                        var order = _companyRepository.GetAllOrders().Where(m => m.Property_Id == item.Row_Id).ToList();
                        foreach (var orderitem in order)
                        {
                            var orderlstModel = Mapper.Map<Order, OrderTrackingModel>(orderitem);
                            orderlstModel.CompanyName = company.XeroName;
                            orderlstModel.PropertyName = item.Name;
                            trackinglst.Add(orderlstModel);
                        }

                    }



                }

            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return trackinglst;

        }

        public IList<User> AllUsers()
        {

            var currentuser = UserManager.Current();
            if (currentuser != null)
            {
                var userlist = _repository.GetAll().Where(c => c.OrgId == currentuser.OrgId && c.IsActive == true).ToList();
                return userlist;
            }

            return null;
        }

        private void Removerecordbyusertype(UserModel obj, User usr)
        {
            string existinguserType = usr.UserType.ToString();
            string currentUserType = obj.UserType.ToString();

            if (existinguserType == "1" && (currentUserType=="3" ||currentUserType=="2"))
            {
                _repository.DeleteUsercalendar(obj.Row_Id);

            }
            else if (existinguserType == "3" && (currentUserType == "2" || currentUserType == "1"))
            {
                  usr.CompanyId = 0;
                _repository.Update(usr);
            }
            else if (existinguserType == "2" && (currentUserType == "3" || currentUserType == "1"))
            {
                _repository.DeleteUsercalendar(obj.Row_Id);
            var associateduserGroups=    _repository.GetAllUserProductGroups().Where(m => m.UserId == obj.Row_Id).ToList();
            if (associateduserGroups.Count>0)
                {
                    foreach (var item in associateduserGroups)
                    {
                        _repository.DeleteUserProductGroup(item.Row_Id);
                    }
                }
               
            }

        }



    }


}