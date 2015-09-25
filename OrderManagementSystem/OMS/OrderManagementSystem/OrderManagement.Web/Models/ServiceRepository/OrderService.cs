using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Http.ModelBinding;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models.Repository;
using ModelState = System.Web.Mvc.ModelState;

namespace OrderManagement.Web.Models.ServiceRepository
{

    public interface IOrderService
    {
        OrderAttachment OrderAttachment(HttpPostedFileBase file, string orderid, string groupname);
        Order GetOrderById(int orderid);
        void UpdateOrder(Order obj);

        IList<OrderViewModel> GetAll();
        IList<OrderItemsModel> GetAllOrderItems(int orderid);
        IList<SelectOrderableProducts> SelectOrderableProducts();
        
        IList<InsertWebOrder> InsertWebOrder(int? ordertypeid, int? orderid, string address, int? property_Id, string comments, DateTime? requireTime, string description, int? mailId, string status, string key);
        int InsertWebProperty(string address, string property_Id, string latitude, string longitude, int company_id);
        void InsertWebOrderItem(int? orderid, int? productid, decimal? quantity, decimal? price, decimal? discountPercent, string options, int? userSelectedcompamyid);
        int InsertWebContact(int order_id, string first_name, string last_name, string email_address, string phone_number, string contact_type, int userSelectedcompamyid);
        int InsertJob(int? orderId, int? property_Id, DateTime? requiredDate, int? mail_Id, int? company_id, string status, string keys);
        int InsertJobEvent( int? job_Id, int? calendar_Id, int? event_Id, int? productGroup_Id, string status, string keys);

        IList<GetStagingCalenderUsers> GetStagingCalenderUsers();
        

    }

    public class OrderService : IOrderService
    {
        private IOrderRepository _repository;
        private IProductRepository _productrepository;
        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
            _productrepository = new ProductRepository();
        }


        public IList<OrderViewModel> GetAll()
        {
            try
            {
                var orderlist = _repository.SelectAll().ToList();
                var orderlstModel = Mapper.Map<IEnumerable<Order>, List<OrderViewModel>>(orderlist);
                return orderlstModel;
            }
            catch (Exception ex)
            {
                var error = ex.Message;

            }

            return null;
        }

        public IList<OrderItemsModel> GetAllOrderItems(int orderid)
        {
            IList<OrderItem> orderitemlst = new List<OrderItem>();
            IList<OrderItemsModel> orderitemmodellst = new List<OrderItemsModel>();
            var orderItemsModel = new OrderItemsModel();
            try
            {
                orderitemlst = _repository.OrderItemsByOrderid(orderid).ToList();
                if (orderitemlst.Count > 0)
                {
                    foreach (var orderitem in orderitemlst)
                    {
                        var order = _repository.GetOrderById(orderid);
                        if (order != null)
                        {
                            orderItemsModel = Mapper.Map<OrderItem, OrderItemsModel>(orderitem);
                            orderItemsModel.Order = order;
                            orderItemsModel.Property = _repository.GetPropertyById(int.Parse(order.Property_Id.ToString()));
                            orderitemmodellst.Add(orderItemsModel);
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return orderitemmodellst;

        }

        public OrderAttachment OrderAttachment(HttpPostedFileBase file, string orderid, string groupname)
        {

            try
            {
                var currentuser = UserManager.Current();
                var orderitems = _repository.GetOrderItemByOrderId(int.Parse(orderid));

                if (orderitems != null)
                {
                    // var product = _productrepository.SelectById(int.Parse(orderitems.Xero_Id.ToString()));

                    if (file != null && !string.IsNullOrEmpty(orderid))
                    {

                        byte[] data;
                        const int width = 300;
                        const int height = 300;
                        using (var srcImage = Image.FromStream(file.InputStream))
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
                        }
                        var objAttachment = new OrderAttachment();
                        objAttachment.Org_Id = int.Parse(currentuser.OrgId.ToString());
                        objAttachment.Order_Id = int.Parse(orderitems.Order_Id.ToString());
                        objAttachment.FileName = Path.GetFileName(file.FileName);
                        objAttachment.FileExtension = Path.GetExtension(file.FileName); ;
                        objAttachment.FileSize = file.ContentLength;
                        objAttachment.Folder = groupname;
                        objAttachment.Created = DateTime.Now;
                        objAttachment.CreatedBy = currentuser.UserType;
                        objAttachment.File = data;
                        //if (product!=null)
                        //{
                        //    objAttachment.GroupType = product.ProductGroupId.ToString();
                        //}
                        _repository.SaveOrderAttachment(objAttachment);
                    }
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return null;

        }

        public Order GetOrderById(int orderid)
        {
            return _repository.GetOrderById(orderid);

        }

        public void UpdateOrder(Order obj)
        {
            _repository.UpdateOrder(obj);

        }

        public IList<SelectOrderableProducts> SelectOrderableProducts()
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

                var JobFolders = _repository.SelectOrderableProducts(orgId, userid, userTypeName, compamyid).ToList();
                return JobFolders;

            }
            return null;

        }

        public int InsertWebProperty(string address, string property_Id, string latitude, string longitude, int company_id)
        {

            int id = 0;
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

                else
                {
                    compamyid = company_id;
                }

                id = _repository.InsertWebProperty(orgId, userid, address, property_Id, latitude, longitude, compamyid);
                return id;

            }
            return id;

        }

     


        public IList<InsertWebOrder> InsertWebOrder(int? ordertypeid, int? orderid, string address, int? property_Id, string comments, DateTime? requireTime, string description, int? mailId, string status, string key)
        {

            int id = 0;
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

                IList<InsertWebOrder> weborderitemlist = new List<InsertWebOrder>();
                weborderitemlist = _repository.InsertWebOrder(orgId, ordertypeid, orderid, userid, address, property_Id, comments, userType.ToString(), requireTime, description, mailId, compamyid, status, key);
                return weborderitemlist;

            }

            return null;
        }


        public void InsertWebOrderItem(int? orderid, int? productid, decimal? quantity, decimal? price, decimal? discountPercent, string options, int? userSelectedcompamyid)
        {

            int id = 0;
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

                
                //InsertJob
                
               _repository.InsertWebOrderItem(orgId, orderid, userid, productid, quantity, price, discountPercent, options, userType.ToString(), userSelectedcompamyid);
            }
        }

        public int InsertWebContact(int order_id, string first_name, string last_name, string email_address, string phone_number, string contact_type,int userSelectedcompamyid)
        {

            int id = 0;
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




                id = _repository.InsertWebContact(userSelectedcompamyid, order_id, currentUser.Row_Id, first_name, last_name, email_address, phone_number, contact_type);
                return id;

            }
            return id;

        }


           public int InsertJob(int? orderId, int? property_Id, DateTime? requiredDate, int? mail_Id, int? company_id, string status, string keys)
           {

            int id = 0;
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

                id = _repository.InsertJob(orgId, orderId, userid, property_Id, requiredDate, mail_Id, company_id, null, keys,null);
                return id;

            }
            return id;

        }

           public int InsertJobEvent(int? job_Id, int? calendar_Id, int? event_Id, int? productGroup_Id, string status, string keys)
           {

               int id = 0;
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

                   id = _repository.InsertJobEvent(orgId, job_Id, (int)userid, calendar_Id, event_Id, productGroup_Id, null, keys);
                   return id;

               }
               return id;

           }

       public IList<GetStagingCalenderUsers> GetStagingCalenderUsers()
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
                var StagingCalenderUsers = _repository.GetStagingCalenderUsers(orgId).ToList();
                return StagingCalenderUsers;
            }
            return null;

        }
        


          // int InsertJobEvent(int org_Id, int? job_Id, int? user_id, int? calendar_Id, int? event_Id, int? productGroup_Id, string status, string keys);


        


        

    }
}