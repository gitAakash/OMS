using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;
using OrderManagement.Web.Helper.Utilitties;
using System.Transactions;
using System.Text;

namespace OrderManagement.Web.Controllers
{
    public class OrderController : Controller
    {
       
        private readonly ICompanyRepository _companyrepository;
        private readonly IOrderService _orderService;

        public OrderController()
        {
            _companyrepository = new CompanyRepository();
            var orderrapo = new OrderRepository();
            this._orderService = new OrderService(orderrapo);
        }

        private string getPropertyNameById(int propertyId)
        {
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                var t = OrderMangtDB.Properties.ToList().Where(x => x.Row_Id.Equals(propertyId)).FirstOrDefault();

                if (t != null)
                {
                    return t.Name.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string getCompanyNameByPropertyId(int propertyId)
        {
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                int compnayId;

                var company = OrderMangtDB.Properties.ToList().Where(x => x.Row_Id.Equals(propertyId)).FirstOrDefault();
                if (company != null)
                {
                    compnayId = Convert.ToInt32(company.Company_Id.ToString());

                }
                else
                {
                    compnayId = 0;
                }
                var t = OrderMangtDB.Companies.Where(x => x.Row_Id.Equals(compnayId)).FirstOrDefault();

                if (t != null)
                {
                    return t.XeroName.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public ActionResult Index(string id)
        {


            var AllGalleryFoldersData = _orderService.SelectOrderableProducts().ToList();

            List<OrderableProducts> lstOrderableProductsInfo = new List<OrderableProducts>();

            for (int i = 0; i < 5; i++)
            {
                OrderableProducts objOrderableProducts = new OrderableProducts();
                objOrderableProducts.Row_Id = i;
                objOrderableProducts.WebName = "Photography " + i.ToString();
                lstOrderableProductsInfo.Add(objOrderableProducts);
            }

            var lstOrderableProd = lstOrderableProductsInfo.Select(p => new
            {
                Row_Id = p.Row_Id,
                productDes = p.WebName

            });




            using (var OrderMangtDB = new OrderMgntEntities())
            {
                var currentUser = UserManager.Current();
                int calendarUserId;
                if (currentUser != null && currentUser.UserType == 1)
                {
                    IList<OrderViewModel> order = _orderService.GetAll();
                    foreach (var item in order)
                    {
                        item.PropertyName = item.Property_Id != null ? getPropertyNameById((int)item.Property_Id) : "";
                        item.CompanyName = item.Property_Id != null ? getCompanyNameByPropertyId((int)item.Property_Id) : "";
                    }
                    return PartialView("Controls/Order/_OrderList", order);
                }
                else
                {
                    var calendarUser = OrderMangtDB.CalendarUsers.ToList().Where(x => x.UserId.Equals(currentUser.Row_Id)).FirstOrDefault();
                    calendarUserId = (int)calendarUser.CalendarId;
                    var orderIdList = OrderMangtDB.EventTrackings.ToList().Where(x => x.CalendarId.Equals(calendarUserId)).Select(x => x.OrderId).ToArray();
                    var orders = OrderMangtDB.Orders.ToList().Where(x => orderIdList.Contains(x.OrderId));
                    var orderlstModel = Mapper.Map<IEnumerable<Order>, List<OrderViewModel>>(orders);
                    foreach (var item in orderlstModel)
                    {
                        item.PropertyName = item.Property_Id != null ? getPropertyNameById((int)item.Property_Id) : "";
                        item.CompanyName = item.Property_Id != null ? getCompanyNameByPropertyId((int)item.Property_Id) : "";


                        //  item.RequiredDate.Value = item.RequiredDate.Value != null ? item.RequiredDate: "";//Convert.ToDateTime(item.RequiredDate) : "";
                        if (item.RequiredDate.Value != null)
                        {
                            item.RequiredDate = item.RequiredDate;
                        }
                    }

                    return PartialView("Controls/Order/_OrderList", orderlstModel);
                }
            }

        }

        public ActionResult GetOrderItemList(string id)
        {
            var orderitems = _companyrepository.GetAllOrderItemsByOrder(int.Parse(id)).ToList();
            if (orderitems != null)
            {
                OrderViewModel objorderviewModel = new OrderViewModel();
                objorderviewModel.OrderItems = orderitems;

                int ContactId = _companyrepository.GetOrderContactByOrderID(int.Parse(id)).OrderId.Value;
                var contact = _companyrepository.GetContactsById(3205);
                if (contact != null)
                {
                    objorderviewModel.OrderContact = contact;// new Contact { Row_Id = 123, Name = "Susan McGlashan", Value = "0417 554 224  9822 9999", ContactType = "Sales" };
                }
                return PartialView("Controls/Order/_OrderItemsList", objorderviewModel);
            }
            else
            {
                return PartialView("Controls/Order/_OrderItemsList", "");
            }
        }
     
        public ActionResult AddOrder()
        {

            NewOrderViewModel objNewOrderViewModel = new NewOrderViewModel();

            var currentUser = UserManager.Current();
            var companylstModel = new List<CompanyModel>();
            if (currentUser != null && currentUser.OrgId != null)
            {
                int usertype = currentUser.UserType.HasValue ? currentUser.UserType.Value : default(int);
                objNewOrderViewModel.UserType = usertype;
                if (usertype != 3)  // usertype 1= "internal" " 2 for Staff 3 = External user
                {
                    var companylist = _companyrepository.SelectAll().Where(cm => cm.Org_Id == currentUser.OrgId);
                    companylstModel = Mapper.Map<IEnumerable<Company>, List<CompanyModel>>(companylist);
                    objNewOrderViewModel.Companylist = _companyrepository.SelectAll().Where(cm => cm.Org_Id == currentUser.OrgId).ToList();
                }
            }

            return PartialView("Controls/Order/_AddOrder", objNewOrderViewModel);

        }

        [HttpPost]
        public ActionResult AddOrder(FormCollection frm)
        {
            NewOrderViewModel objNewOrderViewModel = new NewOrderViewModel();
          
            var currentUser = UserManager.Current();
            var companylstModel = new List<CompanyModel>();
            if (currentUser != null && currentUser.OrgId != null)
            {
                int usertype = currentUser.UserType.HasValue ? currentUser.UserType.Value : default(int);

                objNewOrderViewModel.UserType = usertype;
                if ((objNewOrderViewModel.UserType == 1) || (objNewOrderViewModel.UserType == 2))  // admin -1 , internal user/staff 2;  external User/Client 3 
                {
                    // var CurrentUserComName = currentUser.Company.ToString();
                    var companylist = _companyrepository.SelectAll().Where(cm => cm.Org_Id == currentUser.OrgId);
                    companylstModel = Mapper.Map<IEnumerable<Company>, List<CompanyModel>>(companylist);
                    objNewOrderViewModel.Companylist = _companyrepository.SelectAll().Where(cm => cm.Org_Id == currentUser.OrgId).ToList();
                }
                else if (objNewOrderViewModel.UserType == 3)
                {
                    objNewOrderViewModel.CompanyID = currentUser.CompanyId.HasValue ? currentUser.CompanyId.Value : default(int);
                    objNewOrderViewModel.CompanyName = _companyrepository.GetById(currentUser.CompanyId.ToString()).XeroName;
                }
            }

        //    objNewOrderViewModel.PropTypeID = Convert.ToInt32(frm["rd_proptype"].ToString());


            SaveProperties(objNewOrderViewModel, frm);

            //int value = objNewOrderViewModel.PropTypeID;
            //switch (value)
            //{
            //    case 1:
            //        SaveProperties(objNewOrderViewModel, frm);
            //        break;
            //    case 2:
            //        SaveProperties(objNewOrderViewModel, frm);
            //        break;
            //    case 3:
            //        SaveCommertialProperty(objNewOrderViewModel, frm);
            //        break;
            //}

           // return RedirectToAction("ReloadOrder");

          return PartialView("Controls/Order/_AddOrder", objNewOrderViewModel);
        }

        /// <summary>
      /// 
      /// </summary>
      /// <param name="objNewOrderViewModel"></param>
      /// <param name="frm"></param>
        private void SaveProperties(NewOrderViewModel objNewOrderViewModel,FormCollection frm)
        {
            StringBuilder sb = new StringBuilder();
            #region Saving Residential Property

            if ((objNewOrderViewModel.UserType == 1) || (objNewOrderViewModel.UserType == 2))
            {
                objNewOrderViewModel.CompanyID = Convert.ToInt32(frm["ddlCompany"].ToString());
            }


            objNewOrderViewModel.PropertyAddrs = (frm["PropertyAddress"].ToString());
            objNewOrderViewModel.SpecialInstruction = (frm["txt_SpecialInsruction"].ToString());

            DateTime dtPropReq;

            if (string.IsNullOrEmpty(frm["Dt_PropertyReady"].ToString()))
            {
                dtPropReq = Convert.ToDateTime(frm["Dt_PropertyReady"].ToString());
            }
            else
            {
                dtPropReq = DateTime.Now.AddMonths(1);
            }

            objNewOrderViewModel.PropertyReady = dtPropReq;


            #region Keys in safe/Office
            string strKeys = string.Empty;
            if (frm["chk_KeyinsafeProp"] != null)
            {
                string strUAVDrone = (frm["chk_KeyinsafeProp"].ToString());
                sb = null;
                if (frm["chk_KeyinsafeProp"] != null)
                {
                    strKeys = frm["chk_KeyinsafeProp"].ToString();
                    // sb.Append(frm["chk_KeyinsafeProp"].ToString());
                }
            }

            if (frm["chk_KeyinOffice"] != null)
            {
                strKeys = strKeys + frm["chk_KeyinOffice"].ToString();
                // sb.Append(frm["chk_KeyinOffice"].ToString());
            }
            // sb.ToString();



            #endregion

            using (var OrderMangtDB = new OrderMgntEntities())
            {
                using (var transaction = new TransactionScope())
                {
                    var EntityProperties = new Property
                    {
                        Company_Id = objNewOrderViewModel.CompanyID,
                        Name = objNewOrderViewModel.PropertyAddrs,
                        Created = DateTime.Now,
                    };

                    // Add the Properties entity
                    OrderMangtDB.Properties.Add(EntityProperties);

                    OrderMangtDB.SaveChanges();
                    // Update the entity in the database

                    // Get the Row_Id generated by the database
                    objNewOrderViewModel.PropRowID = EntityProperties.Row_Id;

                    var EnityOrder = new Order
                    {
                        Property_Id = objNewOrderViewModel.PropRowID,
                        SpecialInstructions = objNewOrderViewModel.SpecialInstruction,
                        OrderId = "DPI-" + DateTime.Now.ToString("yyyyMMddHHmmssf"),
                        Keys = strKeys.ToString(),
                        RequiredDate = objNewOrderViewModel.PropertyReady,
                        Created = DateTime.Now,
                    };

                    // Add the Orders entity
                    OrderMangtDB.Orders.Add(EnityOrder);

                    OrderMangtDB.SaveChanges();
                    // Update the entity in the database

                    // Get the Row_Id generated by the database
                    objNewOrderViewModel.OrderRowID = EnityOrder.Row_Id;


                    #region PhotoGrphy

                    if (frm["Chk_PR_photography"] != null)
                    {
                        string Photography = (frm["Chk_PR_photography"].ToString());

                        #region day PhotoGrphy  Section
                        if (frm["Rd_day_photography"] != null)
                        {
                            string DayphotographyType = frm["Rd_day_photography"].ToString();
                            StringBuilder sbPhotoGraphy = new StringBuilder();
                            sbPhotoGraphy.Append("Day Photography:");

                            switch (DayphotographyType)
                            {
                                case "2ExternalImages":

                                    if (frm["chk_img_2_FR1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_2_FR1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_2_FR2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_2_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_2_RE1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_2_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }

                                    if (frm["chk_img_2_RE2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_2_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    break;

                                case "5FinalImages":

                                    if (frm["chk_img_5_FR1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_FR1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_FR2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_Living"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Living"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_Dining"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Dining"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }

                                    /////////////
                                    if (frm["chk_img_5_Family"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Family"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_Kitchen"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Kitchen"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_Bathroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Bathroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_Ensuite"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Ensuite"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_img_5_MasterBed"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_MasterBed"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_2Bedroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_2Bedroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_Rumpus"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Rumpus"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_HomeTheatre"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_HomeTheatre"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_img_5_Pool"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Pool"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_RE1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_RE1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_RE2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_RE2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_5_Lifestyle"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_5_Lifestyle"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }

                                    break;

                                case "8FinalImages":

                                    if (frm["chk_img_8_FR1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_FR1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_FR2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_Living"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Living"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_Dining"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Dining"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }

                                    /////////////
                                    if (frm["chk_img_8_Family"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Family"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_Kitchen"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Kitchen"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_Bathroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Bathroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_Ensuite"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Ensuite"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_img_8_MasterBed"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_MasterBed"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_2Bedroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_2Bedroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_Rumpus"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Rumpus"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_HomeTheatre"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_HomeTheatre"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_img_8_Pool"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Pool"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_RE1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_RE1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_RE2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_RE2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_img_8_Lifestyle"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_img_8_Lifestyle"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }

                                    // remove last ,


                                    break;
                            }

                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbPhotoGraphy.ToString().Remove(sbPhotoGraphy.ToString().Length - 1), 2);

                        }
                        #endregion Day Photo

                        #region Dusk PhotoGrphy  Section
                        if (frm["chk_dusk_8final_img"] != null)
                        {
                            StringBuilder sbPhotoGraphy = new StringBuilder();
                            sbPhotoGraphy.Append(" Dusk Photography:");

                            if (frm["chk_dusk_8final_FR1"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_FR1"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_FR2"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_FR2"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_Living"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Living"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_Dining"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Dining"].ToString());
                                sbPhotoGraphy.Append(",");
                            }


                            if (frm["chk_dusk_8final_Family"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Family"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_Kitchen"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Kitchen"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_Bathroom"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Bathroom"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_Ensuite"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Ensuite"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            ///////////////////////

                            if (frm["chk_dusk_8final_MasterBed"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_MasterBed"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_2Bedroom"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_2Bedroom"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_Rumpus"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Rumpus"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_HomeTheatre"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_HomeTheatre"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            ///////////////////////

                            if (frm["chk_dusk_8final_Pool"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Pool"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_RE1"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_RE1"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_RE2"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_RE2"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            if (frm["chk_dusk_8final_Lifestyle"] != null)
                            {
                                sbPhotoGraphy.Append(frm["chk_dusk_8final_Lifestyle"].ToString());
                                sbPhotoGraphy.Append(",");
                            }
                            // remove last ,


                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbPhotoGraphy.ToString().Remove(sbPhotoGraphy.ToString().Length - 1), 2);

                        }
                        #endregion Dusk Photo

                        #region Prestige PhotoGrphy  Section

                        if (frm["Rd_day_presphotography"] != null)
                        {
                            string PrestigePhotographyType = frm["Rd_day_presphotography"].ToString();
                            StringBuilder sbPhotoGraphy = new StringBuilder();
                            sbPhotoGraphy.Append("Prestige Photography:");

                            switch (PrestigePhotographyType)
                            {

                                case "12DayFinalImages":

                                    if (frm["chk_Prestige_Day_FR1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_FR1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_FR2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_Living"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Living"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_Dining"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Dining"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }

                                    if (frm["chk_Prestige_Day_Family"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Family"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_Kitchen"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Kitchen"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_Bathroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Bathroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_Ensuite"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Ensuite"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_Prestige_Day_MasterBed"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_MasterBed"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_2Bedroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_2Bedroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_Rumpus"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Rumpus"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_dusk_8final_HomeTheatre"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_HomeTheatre"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_Prestige_Day_Pool"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Pool"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_RE1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_RE1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_RE2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_RE2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_Day_Lifestyle"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_Day_Lifestyle"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    // remove last ,

                                    sbPhotoGraphy.ToString().Remove(sbPhotoGraphy.ToString().Length - 1);

                                    ////////////////////////
                                    break;

                                case "Dusk12FinalImages":

                                    if (frm["chk_Prestige_dusk_FR1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_FR1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_FR2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_Living"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Living"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_Dining"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Dining"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }


                                    if (frm["chk_Prestige_dusk_Family"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Family"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_Kitchen"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Kitchen"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_Bathroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Bathroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_Ensuite"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Ensuite"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_Prestige_dusk_MasterBed"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_MasterBed"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_2Bedroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_2Bedroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_Rumpus"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Rumpus"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_HomeTheatre"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_HomeTheatre"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_Prestige_dusk_Pool"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Pool"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_RE1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_RE1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_RE2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_RE2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_Prestige_dusk_Lifestyle"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_Prestige_dusk_Lifestyle"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }

                                    break;
                            }

                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbPhotoGraphy.ToString().Remove(sbPhotoGraphy.ToString().Length - 1), 3);
                        }

                        #endregion Prestisge day Photo

                        #region  Rental Photography Section

                        if (frm["rd_day_Rental_photography"] != null)
                        {
                            string RentalPhotographyType = frm["rd_day_Rental_photography"].ToString();
                            StringBuilder sbPhotoGraphy = new StringBuilder();
                            sbPhotoGraphy.Append("Rental Photography:");

                            switch (RentalPhotographyType)
                            {

                                case "5FinalRentalPhotography":

                                    if (frm["chk_RentalOption5_FR1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_FR1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_FR2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_Living"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Living"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_Dining"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Dining"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }


                                    if (frm["chk_RentalOption5_Family"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Family"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_Kitchen"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Kitchen"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_Bathroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Bathroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_Ensuite"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Ensuite"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_RentalOption5_MasterBed"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_MasterBed"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_2Bedroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_2Bedroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_Rumpus"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Rumpus"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_HomeTheatre"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_HomeTheatre"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_RentalOption5_Pool"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Pool"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_RE1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_RE1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_RE2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_RE2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption5_Lifestyle"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption5_Lifestyle"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    // remove last ,

                                    sbPhotoGraphy.ToString().Remove(sbPhotoGraphy.ToString().Length - 1);

                                    ////////////////////////
                                    break;

                                case "10FinalRentalPhotography":

                                    if (frm["chk_RentalOption10_FR1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_FR1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_FR2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_FR2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_Living"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Living"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_Dining"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Dining"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }


                                    if (frm["chk_RentalOption10_Family"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Family"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_Kitchen"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Kitchen"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_Bathroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Bathroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_Ensuite"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Ensuite"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_RentalOption10_MasterBed"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_MasterBed"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_2Bedroom"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_2Bedroom"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_Rumpus"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Rumpus"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_HomeTheatre"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_HomeTheatre"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    ///////////////////////

                                    if (frm["chk_RentalOption10_Pool"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Pool"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_RE1"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_RE1"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_RE2"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_RE2"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    if (frm["chk_RentalOption10_Lifestyle"] != null)
                                    {
                                        sbPhotoGraphy.Append(frm["chk_RentalOption10_Lifestyle"].ToString());
                                        sbPhotoGraphy.Append(",");
                                    }
                                    // remove last ,

                                    sbPhotoGraphy.ToString().Remove(sbPhotoGraphy.ToString().Length - 1);

                                    break;
                            }

                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbPhotoGraphy.ToString().Remove(sbPhotoGraphy.ToString().Length - 1), 4);
                        }

                        #endregion Rental Photography
                    }
                    #endregion Photography

                    #region UAV Drone/Aerial Photography

                    if (frm["Chk_PR_UAVDrone"] != null)
                    {
                        string strUAVDrone = (frm["Chk_PR_UAVDrone"].ToString());

                        StringBuilder sbUAV_Drone = new StringBuilder();
                        string strHead = "UAV Drone/Aerial Photography:";

                        if (frm["chk_UAVDroneUP3"] != null)
                        {
                            sbUAV_Drone.Append(strHead);
                            sbUAV_Drone.Append(frm["chk_UAVDroneUP3"].ToString());
                            sbUAV_Drone.Append(",");
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbUAV_Drone.ToString().Remove(sbUAV_Drone.ToString().Length - 1), 3);
                            sbUAV_Drone.Length = 0;

                        }
                        if (frm["chk_UAVDroneUP5"] != null)
                        {
                            sbUAV_Drone.Append(strHead);
                            sbUAV_Drone.Append(frm["chk_UAVDroneUP5"].ToString());
                            sbUAV_Drone.Append(",");
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbUAV_Drone.ToString().Remove(sbUAV_Drone.ToString().Length - 1), 15);
                            sbUAV_Drone.Length = 0;
                        }

                        if (frm["chk_UAVDrone_Helicop"] != null)
                        {
                            sbUAV_Drone.Append(strHead);
                            sbUAV_Drone.Append(frm["chk_UAVDrone_Helicop"].ToString());
                            sbUAV_Drone.Append(",");
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbUAV_Drone.ToString().Remove(sbUAV_Drone.ToString().Length - 1), 16);
                            sbUAV_Drone.Length = 0;
                        }

                        if (frm["chk_UAVDrone_Elevated"] != null)
                        {
                            sbUAV_Drone.Append(strHead);
                            sbUAV_Drone.Append(frm["chk_UAVDrone_Elevated"].ToString());
                            sbUAV_Drone.Append(",");
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbUAV_Drone.ToString().Remove(sbUAV_Drone.ToString().Length - 1), 17);
                            sbUAV_Drone.Length = 0;
                        }
                    }
                    #endregion UAV Drone/Aerial Photography

                    #region Floor Plans/Land-boxes Photography

                    if (frm["Chk_PR_FloorPlan"] != null)
                    {
                        string strUAVDrone = (frm["Chk_PR_FloorPlan"].ToString());

                        StringBuilder sbFloorPlan = new StringBuilder();
                        string strHead = "Floor Plans/Land-boxes:";

                        if ((frm["Rd_day_Floorphotography"] != null))
                        {
                            string FloorphotographyType = frm["Rd_day_Floorphotography"].ToString();

                            switch (FloorphotographyType)
                            {

                                case "FloorPlan":
                                    if (frm["chk_OnsiteColor"] != null)
                                    {
                                        sbFloorPlan.Append(strHead);
                                        sbFloorPlan.Append(frm["chk_OnsiteColor"].ToString());
                                    }
                                    SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbFloorPlan.ToString(), 6);
                                    break;

                                case "FloorPlanandSite":
                                    if (frm["chk_OnsiteColor"] != null)
                                    {
                                        sbFloorPlan.Append(strHead);
                                        sbFloorPlan.Append(frm["chk_OnsiteColor"].ToString());
                                    }
                                    SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbFloorPlan.ToString(), 6);
                                    break;
                            }

                        }

                        ////////////////////////
                        if ((frm["RedrawFloorSiteRadio"] != null))
                        {
                            string FloorphotographyType = frm["RedrawFloorSiteRadio"].ToString();

                            switch (FloorphotographyType)
                            {
                                case "FloorPlan":
                                    if (frm["chk_RedrawColor"] != null)
                                    {
                                        sbFloorPlan.Append(strHead);
                                        sbFloorPlan.Append(frm["chk_RedrawColor"].ToString());
                                    }
                                    SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbFloorPlan.ToString(), 7);
                                    break;

                                case "FloorPlanSitePlan":
                                    if (frm["chk_RedrawColor"] != null)
                                    {
                                        sbFloorPlan.Append(strHead);
                                        sbFloorPlan.Append(frm["chk_RedrawColor"].ToString());
                                    }
                                    SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbFloorPlan.ToString(), 7);
                                    break;
                            }
                        }

                        //Saving Landbox value 
                        sbFloorPlan.Append(strHead + " Landbox:");
                        sbFloorPlan.Append(frm["ddlLandbox"].ToString());
                        SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sbFloorPlan.ToString(), 8);
                        ///////////////////
                    }

                    #endregion Floor Plans/Land-boxes Photography

                    #region Copy Writing

                    if (frm["Chk_PR_CopyWriting"] != null)
                    {
                        string strUAVDrone = (frm["Chk_PR_CopyWriting"].ToString());
                        sb = new StringBuilder();
                        string strHead = "Copy Writing:";

                        if (frm["chk_copywrite_Onsite"] != null)
                        {
                            sb.Append(strHead);
                            sb.Append(frm["chk_copywrite_Onsite"].ToString());
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sb.ToString(), 9);
                            sb.Length = 0;
                        }

                        if (frm["chk_copywrite_Offsite"] != null)
                        {
                            sb.Append(strHead);
                            sb.Append(frm["chk_copywrite_Offsite"].ToString());
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sb.ToString(), 10);
                            sb.Length = 0;
                        }

                        if (frm["chk_Rewrite_agent"] != null)
                        {
                            sb.Append(strHead);
                            sb.Append(frm["chk_Rewrite_agent"].ToString());
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sb.ToString(), 11);
                            sb.Length = 0;
                        }
                    }
                    #endregion Copy Writing


                    #region Video and Image tours

                    if (frm["Chk_PR_VideoImageTour"] != null)
                    {
                        string strUAVDrone = (frm["Chk_PR_VideoImageTour"].ToString());
                        sb = new StringBuilder();
                        string strHead = "Video and Image tours:";

                        if (frm["chk_video_propvideo"] != null)
                        {
                            sb.Append(strHead);
                            sb.Append(frm["chk_video_propvideo"].ToString());
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sb.ToString(), 12);
                            sb.Length = 0;
                        }

                        if (frm["chk_video_profile"] != null)
                        {
                            sb.Append(strHead);
                            sb.Append(frm["chk_video_profile"].ToString());
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sb.ToString(), 13);
                            sb.Length = 0;
                        }

                        if (frm["chk_ImageTours_stillImg"] != null)
                        {
                            sb.Append(strHead);
                            sb.Append(frm["chk_ImageTours_stillImg"].ToString());
                            SaveDatainOrderSubItem(OrderMangtDB, objNewOrderViewModel.OrderRowID, sb.ToString(), 14);
                            sb.Length = 0;
                        }
                    }
                    #endregion Copy Writing
                    // End Logic for Copy Writing selected items

                    #region Agent Check
                    if (frm["chkAgent"] != null)
                    {
                        string ContactType = (frm["chkAgent"].ToString());
                        objNewOrderViewModel.AgentName = (frm["txtAgentName"].ToString());

                        string AgentPhoneEmail = string.Empty;
                        if (!string.IsNullOrEmpty(frm["txtAgentPhone"]))
                        {
                            objNewOrderViewModel.AgentPhone = (frm["txtAgentPhone"].ToString());
                            AgentPhoneEmail = "Phone:" + objNewOrderViewModel.AgentPhone;
                        }

                        if (!string.IsNullOrEmpty(frm["txtAgentPhone"]))
                        {
                            objNewOrderViewModel.AgentEmail = (frm["txtAgentEmail"].ToString());
                            AgentPhoneEmail = AgentPhoneEmail + " E-Mail:" + objNewOrderViewModel.AgentEmail;
                        }

                        var EntityContacts = new Contact
                        {
                            CompanyId = objNewOrderViewModel.CompanyID,
                            Name = objNewOrderViewModel.AgentName,
                            Value = AgentPhoneEmail,
                            ContactType = ContactType,
                            Created = DateTime.Now,
                        };

                        OrderMangtDB.Contacts.Add(EntityContacts);
                        OrderMangtDB.SaveChanges();
                        objNewOrderViewModel.ContactRowID = EntityContacts.Row_Id;

                        var EntityOrderContacts = new OrderContact
                        {
                            ContactId = objNewOrderViewModel.ContactRowID,
                            OrderId = objNewOrderViewModel.OrderRowID,
                            Created = DateTime.Now,
                        };

                        OrderMangtDB.OrderContacts.Add(EntityOrderContacts);
                        OrderMangtDB.SaveChanges();
                        objNewOrderViewModel.OrderContactID = EntityOrderContacts.Row_Id;

                    }
                    #endregion Agent

                    #region Owner Section
                    if (frm["chkOwner"] != null)
                    {

                        string ContactType = (frm["chkOwner"].ToString());
                        objNewOrderViewModel.AgentName = (frm["txt_OwnerName"].ToString());

                        string OwnerPhoneEmail = string.Empty;
                        if (!string.IsNullOrEmpty(frm["txt_OwnerPhone"]))
                        {
                            objNewOrderViewModel.OwnerPhone = (frm["txt_OwnerPhone"].ToString());
                            OwnerPhoneEmail = "Phone:" + objNewOrderViewModel.OwnerPhone;
                        }


                        var EntityContacts = new Contact
                        {
                            CompanyId = objNewOrderViewModel.CompanyID,
                            Name = objNewOrderViewModel.OwnerName,
                            Value = OwnerPhoneEmail,
                            ContactType = ContactType,
                            Created = DateTime.Now,
                        };

                        OrderMangtDB.Contacts.Add(EntityContacts);
                        OrderMangtDB.SaveChanges();
                        objNewOrderViewModel.ContactRowID = EntityContacts.Row_Id;

                        var EntityOrderContacts = new OrderContact
                        {
                            ContactId = objNewOrderViewModel.ContactRowID,
                            OrderId = objNewOrderViewModel.OrderRowID,
                            Created = DateTime.Now,
                        };

                        OrderMangtDB.OrderContacts.Add(EntityOrderContacts);
                        OrderMangtDB.SaveChanges();
                        objNewOrderViewModel.OrderContactID = EntityOrderContacts.Row_Id;

                    }
                    #endregion Owner

                    #region Tenant Section
                    if (frm["chkTenant"] != null)
                    {

                        string ContactType = (frm["chkTenant"].ToString());

                        if (!string.IsNullOrEmpty(frm["txt_TenantName"]))
                        {
                            objNewOrderViewModel.TanantName = (frm["txt_TenantName"].ToString());
                        }

                        if (!string.IsNullOrEmpty(frm["txt_TenantPhone"]))
                        {
                            objNewOrderViewModel.TanantPhone = "Phone:" + (frm["txt_TenantPhone"].ToString());
                        }


                        var EntityContacts = new Contact
                        {
                            CompanyId = objNewOrderViewModel.CompanyID,
                            Name = objNewOrderViewModel.TanantName,
                            Value = objNewOrderViewModel.TanantPhone,
                            ContactType = ContactType,
                            Created = DateTime.Now,
                        };

                        OrderMangtDB.Contacts.Add(EntityContacts);
                        OrderMangtDB.SaveChanges();
                        objNewOrderViewModel.ContactRowID = EntityContacts.Row_Id;

                        var EntityOrderContacts = new OrderContact
                        {
                            ContactId = objNewOrderViewModel.ContactRowID,
                            OrderId = objNewOrderViewModel.OrderRowID,
                            Created = DateTime.Now,
                        };

                        OrderMangtDB.OrderContacts.Add(EntityOrderContacts);
                        OrderMangtDB.SaveChanges();
                        objNewOrderViewModel.OrderContactID = EntityOrderContacts.Row_Id;

                    }
                    #endregion Tenant

                    transaction.Complete();
                }

            }
            #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objNewOrderViewModel"></param>
        /// <param name="frm"></param>
        
        private void SaveCommertialProperty(NewOrderViewModel objNewOrderViewModel,FormCollection frm)
        {
            #region Saving Commercial Photography

                    if ((objNewOrderViewModel.UserType == 1) || (objNewOrderViewModel.UserType == 2))
                    {
                        objNewOrderViewModel.CompanyID = Convert.ToInt32(frm["ddlCompanyCom"].ToString());
                    }

                    objNewOrderViewModel.OfficeContactName = (frm["txtOfficeContactNameCom"].ToString());
                    objNewOrderViewModel.Phone = (frm["txtPhone"].ToString());
                    objNewOrderViewModel.Email = (frm["txtEmailIdCom"].ToString());
                    objNewOrderViewModel.ProjectAddress = (frm["txtProjectAddress"].ToString());
                    objNewOrderViewModel.DetailedBrief = (frm["txtDetailedBrief"].ToString());

                    using (var OrderMangtDB = new OrderMgntEntities())
                    {
                        using (var transaction = new TransactionScope())
                        {
                            var EntityProperties = new Property
                            {
                                Company_Id = objNewOrderViewModel.CompanyID,
                                Name = objNewOrderViewModel.ProjectAddress,
                                Created = DateTime.Now,
                            };

                            // Add the Properties entity
                            OrderMangtDB.Properties.Add(EntityProperties);

                            OrderMangtDB.SaveChanges();
                            // Update the entity in the database

                            // Get the Row_Id generated by the database
                            objNewOrderViewModel.PropRowID = EntityProperties.Row_Id;

                            var EnityOrder = new Order
                            {
                                Property_Id = objNewOrderViewModel.PropRowID,
                                Description = objNewOrderViewModel.DetailedBrief,
                                OrderId = "DPI-" + DateTime.Now.ToString("yyyyMMddHHmmssf"),
                                Created = DateTime.Now,
                            };

                            // Add the Orders entity
                            OrderMangtDB.Orders.Add(EnityOrder);

                            OrderMangtDB.SaveChanges();
                            // Update the entity in the database

                            // Get the Row_Id generated by the database
                            objNewOrderViewModel.OrderRowID = EnityOrder.Row_Id;

                            // Logic for PhotoGrphy selected items



                            // End Logic for PhotoGrphy selected items


                            var EntityOrderItems = new OrderItem
                            {
                                Order_Id = objNewOrderViewModel.OrderRowID,
                                //Name = "Options Premium Package 2.1  Photography up to 8 day/dusk images Property Video /Floorplan + Siteplan",
                                Created = DateTime.Now,
                            };
                            // Add the OrderItems entity
                            OrderMangtDB.OrderItems.Add(EntityOrderItems);
                            // Update the entity in the database
                            OrderMangtDB.SaveChanges();
                            // Get the Row_Id generated by the database
                            objNewOrderViewModel.OrderItemRowID = EntityOrderItems.Row_Id;

                            // add entry in the CompanyOrdertable

                            var EntityCompanyOrder = new CompanyOrder
                            {
                                CompanyId = objNewOrderViewModel.CompanyID,
                                OrderId = objNewOrderViewModel.OrderRowID,
                                Created = DateTime.Now,
                            };

                            // Add the CompanyOrders entity
                            OrderMangtDB.CompanyOrders.Add(EntityCompanyOrder);
                            // Update the entity in the database
                            OrderMangtDB.SaveChanges();
                            // Get the Row_Id generated by the database
                            objNewOrderViewModel.CompanyOrderRowID = EntityCompanyOrder.Row_Id;

                            transaction.Complete();
                        }
                    }
                    #endregion
        }

        /// <summary>
        ///  Function for saving data in orderitems and OrderSubItems table
        /// </summary>
        /// <param name="OrderMgntEntities"></param>
        /// <param name="Order_Id"></param>
        /// <param name="SelectedItemList"></param>
        /// <param name="ProductSubGroupId"></param>
        /// <returns></returns>
        private int SaveDatainOrderSubItem(OrderMgntEntities OrderMangtDB, int Order_Id, string SelectedItemList, int ProductSubGroupId)
        {
            int OrderItemRowID, OrderSubItemRowID;
            var EntityOrderItems = new OrderItem
            {
                Order_Id = Order_Id,
                Created = DateTime.Now,
            };
            // Add the OrderItems entity
            OrderMangtDB.OrderItems.Add(EntityOrderItems);
            // Update the entity in the database
            OrderMangtDB.SaveChanges();
            // Get the Row_Id generated by the database
            OrderItemRowID = EntityOrderItems.Row_Id;


            var EntityOrdersubItems = new OrderSubItem
            {
                OrderItemId = OrderItemRowID,
                ProductSubGroupId = ProductSubGroupId, // get "Rental Photography" value form the ProductSubGroups table
                OptionSelected = SelectedItemList.ToString(),
                Created = DateTime.Now,
            };

            // Add the OrderItems entity
            OrderMangtDB.OrderSubItems.Add(EntityOrdersubItems);
            // Update the entity in the database
            OrderMangtDB.SaveChanges();
            // Get the Row_Id generated by the database
            return OrderSubItemRowID = EntityOrderItems.Row_Id;
        }


        public JsonResult GetOrderableProductsDataJson(string groupid = "1")
        {

            // _orderService

            var AllGalleryFoldersData = _orderService.SelectOrderableProducts().ToList();
                
            List<OrderableProducts> lstOrderableProductsInfo = new List<OrderableProducts>();

                for (int i = 0; i < 5; i++)
                {
                    OrderableProducts objOrderableProducts = new OrderableProducts();
                    objOrderableProducts.Row_Id = i;
                    objOrderableProducts.WebName = "Photography " + i.ToString();
                    lstOrderableProductsInfo.Add(objOrderableProducts);
                }

                var lstOrderableProd = lstOrderableProductsInfo.Select(p => new
                {
                    Row_Id = p.Row_Id,
                    productDes = p.WebName

                });

            //var TagList = AllGalleryFoldersData.ToList().Where(m => m.ROW_ID == Convert.ToInt16(groupid)).Select(i => new { i.ROW_ID, i.TAGS, i.FOLDER });

            //List<TagInfo> TagInfo = new List<TagInfo>();

            //foreach (var item in TagList)
            //{
            //    string strTag = item.TAGS;
            //    List<TagInfo> lstTagInfo = new List<TagInfo>();
            //    if (!string.IsNullOrEmpty(strTag))
            //    {
            //        string[] strTagArray = strTag.Split(';');

            //        foreach (string obj in strTagArray)
            //        {
            //            TagInfo objtagInfo = new TagInfo();
            //            objtagInfo.Row_Id = item.ROW_ID;
            //            objtagInfo.Folder = item.FOLDER;
            //            objtagInfo.TagName = obj;
            //            lstTagInfo.Add(objtagInfo);
            //        }
            //    }
            //    TagInfo = lstTagInfo;
            //}

                return Json(lstOrderableProd, JsonRequestBehavior.AllowGet);
        }


    }

}

