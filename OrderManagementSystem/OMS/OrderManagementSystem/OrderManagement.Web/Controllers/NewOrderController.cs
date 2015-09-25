using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;
using OrderManagement.Web.Helper.Utilitties;
using System.Text;
using System.Text.RegularExpressions;

namespace OrderManagement.Web.Controllers
{
    public class NewOrderController : Controller
    {
        private readonly ICompanyRepository _companyrepository;
        private readonly IOrderService _orderService;

        public NewOrderController()
        {
            _companyrepository = new CompanyRepository();
            var orderrapo = new OrderRepository();
            this._orderService = new OrderService(orderrapo);
        }


        public ActionResult Index()
        {

            var ordermodel = GetNewOrder();

            return PartialView("Controls/Order/_NewOrder", ordermodel);
        }


        public ActionResult ProductGroupContent(string id)
        {
            return PartialView("Controls/Order/_productGroupContent");
        }

        [HttpPost]
        public ActionResult AddOrder(NewOrderModel objNewOrderModel)
        {

            return PartialView("Controls/Order/_NewOrder", objNewOrderModel);
        }




        private NewOrderModel GetNewOrder()
        {
            NewOrderModel objNewOrderModel = new NewOrderModel();

            objNewOrderModel.OfficeContactName = string.Empty;
            objNewOrderModel.AgentEmail = string.Empty;


            var currentUser = UserManager.Current();
            var companylstModel = new List<CompanyModel>();
            if (currentUser != null && currentUser.OrgId != null)
            {
                int usertype = currentUser.UserType.HasValue ? currentUser.UserType.Value : default(int);
                objNewOrderModel.UserType = usertype;
                // if (usertype != 3)  // usertype 1= "internal" " 2 for Staff 3 = External user
                // {
                var companylist = _companyrepository.SelectAll().Where(cm => cm.Org_Id == currentUser.OrgId);
                companylstModel = Mapper.Map<IEnumerable<Company>, List<CompanyModel>>(companylist);
                objNewOrderModel.Companylist = _companyrepository.SelectAll().Where(cm => cm.Org_Id == currentUser.OrgId).ToList();
                //}
            }




            var AllOrderableProductsData = _orderService.SelectOrderableProducts().ToList();

            //...................Trilok..................................................

          List<PrimaryGroup> prodGrp = new List<PrimaryGroup>();

            var allOrderableProducts = AllOrderableProductsData.GroupBy(m => m.PrimaryProductGroup);
            
            foreach (var item in allOrderableProducts)
            {

              PrimaryGroup pg = new PrimaryGroup();
             
                var grpName = item.Key;

        // Bind Primary list
              pg.PrimaryProductGroupName = grpName;
          //pg.secondaryProductGroup = new List<SecondaryGroup>();

             var ItemGroup = item.Where(m => m.PrimaryProductGroup == grpName);

               
              var secondaryProdGrp = ItemGroup.GroupBy(m => m.SecondaryProductGroup);

              foreach (var secondProd in secondaryProdGrp)
              {
                  SecondaryGroup scondGrp = new SecondaryGroup();

                  if(!string.IsNullOrEmpty(secondProd.Key))
                      scondGrp.SecondaryProductGroup = secondProd.Key;
                  else
                      scondGrp.SecondaryProductGroup = "Test No value";
                        // Bind Secondry List
                  pg.secondaryProductGroup.Add(scondGrp);

                  var thirdgrp = secondProd.Where(m => m.SecondaryProductGroup == secondProd.Key);

                var thirdgrpitem = thirdgrp.GroupBy(m => m.WebName);

                foreach (var thrdgp in thirdgrpitem)
                {
                    ThirdGroup thrdGrp = new ThirdGroup();
                    
                    // Bind Third List
                    var trdName = thrdgp.Key;

                    thrdGrp.ThirdProductGroup = trdName;

                    scondGrp.thirdProductGroup.Add(thrdGrp);
                    foreach (var option in thrdgp)
                     {
                         thrdGrp.Row_Id= option.Row_Id;

                         if (!string.IsNullOrEmpty(option.WebOptions))
                        {
                         var optionArr = option.WebOptions.Split(';');

                         foreach (var arroptn in optionArr)
                          {
                              WebOption ObjOption = new WebOption();
                              ObjOption.Option = arroptn;
                             

                              thrdGrp.webOption.Add(ObjOption);

                          }
                        }

                        // thrdGrp.webOption = optionArr.ToList();
                     }

                  //  thrdGrp.

                    //if (!string.IsNullOrEmpty(thrdGrp.webOptions))
                    //    {
                    //      var webOptionlst= thrdGrp.webOptions

                    //    }

                }

              }

                prodGrp.Add(pg);
            }

            objNewOrderModel.primaryGroup = prodGrp;

            var list3 = prodGrp;

//............................End By Trilok...........................................


        var Primarygroups = from p in AllOrderableProductsData
                                group p by p.PrimaryProductGroup into g
                                select new { GroupName = g.Key, Members = g };


            var Secondarygroups = from s in AllOrderableProductsData
                                  group s by s.SecondaryProductGroup into sg
                                  select new { SGroupName = sg.Key, Members = sg };


            List<PrimaryProductGrp> lstPrimaryProductGrp = new List<PrimaryProductGrp>();
            List<SecondaryProductGroup> lstSecondryProductGrp = new List<SecondaryProductGroup>();

            int i = 11;
            foreach (var g in Primarygroups)
            {
                string strgrpName = g.GroupName.ToString();
                string datarelativediv = Regex.Replace(g.GroupName, "\\s", "").Replace("/", "");

                {
                    PrimaryProductGrp objPrimaryProductGrp = new PrimaryProductGrp();
                    objPrimaryProductGrp.Row_Id = i;
                    objPrimaryProductGrp.WebName = strgrpName;
                    objPrimaryProductGrp.ChkboxName = "";
                    objPrimaryProductGrp.datarelativediv = datarelativediv + i.ToString();
                    lstPrimaryProductGrp.Add(objPrimaryProductGrp);

                    var secondarygroups =
                                      (from osgroup in Secondarygroups
                                       where osgroup.SGroupName == strgrpName
                                       select osgroup).ToList();


                    foreach (var osgroup in secondarygroups)
                    {
                        string sgroupName = osgroup.SGroupName;
                        int j = 100;
                        SecondaryProductGroup objSecondaryProductGroup = new SecondaryProductGroup();
                        objPrimaryProductGrp.Row_Id = j;
                        objPrimaryProductGrp.WebName = strgrpName;
                        objPrimaryProductGrp.ChkboxName = "RD_" + strgrpName + i.ToString();
                        objPrimaryProductGrp.datarelativediv = datarelativediv + i.ToString();
                        lstSecondryProductGrp.Add(objSecondaryProductGroup);
                        j++;







                    }












                    i++;

                }

            }

            //            select  PrimaryProductGroup from #MySelectProdut 
            //GROUP BY PrimaryProductGroup
            //order by  PrimaryProductGroup 









            //var mainproductgroups =
            //      from productgroups in AllGalleryFoldersData
            //      group productgroups by productgroups.PrimaryProductGroup;


            //foreach (var maingroup in mainproductgroups)

            //    foreach (var primaryProductGroup in maingroup)
            //    {
            //        {
            //            // bind the Project requirements Section
            //            NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //            objOrderableProducts.Row_Id = primaryProductGroup.Row_Id;
            //            objOrderableProducts.WebName = primaryProductGroup.PrimaryProductGroup;
            //            objOrderableProducts.ChkboxName = "";
            //            lstOrderableProductsInfo.Add(objOrderableProducts);



            //            // now need to bind another


            //        }

            //    }




            //{
            //    NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //    objOrderableProducts.Row_Id = 11;
            //    objOrderableProducts.WebName = "Photography";
            //    objOrderableProducts.ChkboxName = "";
            //    lstOrderableProductsInfo.Add(objOrderableProducts);
            //}


            //foreach (var item in AllGalleryFoldersData)
            //{
            //    NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //    objOrderableProducts.Row_Id = item.Row_Id;
            //    objOrderableProducts.WebName = item.WebName;
            //    objOrderableProducts.ChkboxName = "";
            //    lstOrderableProductsInfo.Add(objOrderableProducts);
            //}





            //{
            //    NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //    objOrderableProducts.Row_Id = 11;
            //    objOrderableProducts.WebName = "Photography";
            //    objOrderableProducts.ChkboxName = "";
            //    lstOrderableProductsInfo.Add(objOrderableProducts);
            //}

            //{
            //    NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //    objOrderableProducts.Row_Id = 12;
            //    objOrderableProducts.WebName = "UAV Drone/aerial photography";
            //    objOrderableProducts.ChkboxName = "";
            //    lstOrderableProductsInfo.Add(objOrderableProducts);
            //}

            //{
            //    NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //    objOrderableProducts.Row_Id = 12;
            //    objOrderableProducts.WebName = "Floor plans/land-boxes";
            //    objOrderableProducts.ChkboxName = "";
            //    lstOrderableProductsInfo.Add(objOrderableProducts);
            //}

            //{
            //    NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //    objOrderableProducts.Row_Id = 13;
            //    objOrderableProducts.WebName = "Copy writing";
            //    objOrderableProducts.ChkboxName = "";
            //    lstOrderableProductsInfo.Add(objOrderableProducts);
            //}

            //{
            //    NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //    objOrderableProducts.Row_Id = 14;
            //    objOrderableProducts.WebName = "Video and image tours";
            //    objOrderableProducts.ChkboxName = "";
            //    lstOrderableProductsInfo.Add(objOrderableProducts);
            //}


            //for (int i = 0; i < 5; i++)
            //{
            //    NewSelectOrderableProducts objOrderableProducts = new NewSelectOrderableProducts();
            //    objOrderableProducts.Row_Id = i;
            //    objOrderableProducts.WebName = "Photography " + i.ToString();
            //    objOrderableProducts.ChkboxName = "";
            //    lstOrderableProductsInfo.Add(objOrderableProducts);
            //}

            //var lstPrimaryProductGrps = lstPrimaryProductGrp.Select(p => new
            // {
            //     Row_Id = p.Row_Id,
            //     WebName = p.WebName,
            //     checkStatus = p.CheckedStatus

            // });

            List<ContactTypes> lstContactTypeInfo = new List<ContactTypes>();
            {
                ContactTypes objContactType = new ContactTypes();
                objContactType.ContactTypeId = 200;
                objContactType.ContactType = "Agent";
                objContactType.ChkboxName = "chkAgent";
                objContactType.Relativediv = "divAgentContent";

                lstContactTypeInfo.Add(objContactType);
            }

            {
                ContactTypes objContactType = new ContactTypes();
                objContactType.ContactTypeId = 201;
                objContactType.ContactType = "Owner";
                objContactType.ChkboxName = "chkOwner";
                objContactType.Relativediv = "divOwnerContent";
                lstContactTypeInfo.Add(objContactType);
            }

            {
                ContactTypes objContactType = new ContactTypes();
                objContactType.ContactTypeId = 202;
                objContactType.ContactType = "Tenant";
                objContactType.ChkboxName = "chkTenant";
                objContactType.Relativediv = "divTenentContent";
                lstContactTypeInfo.Add(objContactType);
            }

            objNewOrderModel.contactTypes = lstContactTypeInfo.ToList();

            #region PhotoGraphy Section

            #region Region for bind List for Image Options 2

            List<OrderableProductsDayPhotographyOpt2> lstOrderableProductsDayPhotographyOpt2 = new List<OrderableProductsDayPhotographyOpt2>();

            OrderableProductsDayPhotographyOpt2 objOrderableProductsDayPhotographyOpt1 = new OrderableProductsDayPhotographyOpt2();
            objOrderableProductsDayPhotographyOpt1.Row_Id = 1;
            objOrderableProductsDayPhotographyOpt1.WebName = "Front external 1";
            objOrderableProductsDayPhotographyOpt1.ChkboxName = "";
            objOrderableProductsDayPhotographyOpt1.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt2.Add(objOrderableProductsDayPhotographyOpt1);

            OrderableProductsDayPhotographyOpt2 objOrderableProductsDayPhotographyOpt2 = new OrderableProductsDayPhotographyOpt2();
            objOrderableProductsDayPhotographyOpt2.Row_Id = 2;
            objOrderableProductsDayPhotographyOpt2.WebName = "Front external 2";
            objOrderableProductsDayPhotographyOpt2.ChkboxName = "";
            objOrderableProductsDayPhotographyOpt2.cssClass = "DayPhotographyOptions2 FE2";
            lstOrderableProductsDayPhotographyOpt2.Add(objOrderableProductsDayPhotographyOpt2);

            OrderableProductsDayPhotographyOpt2 objOrderableProductsDayPhotographyOpt3 = new OrderableProductsDayPhotographyOpt2();
            objOrderableProductsDayPhotographyOpt2.Row_Id = 3;
            objOrderableProductsDayPhotographyOpt2.WebName = "Rear external";
            objOrderableProductsDayPhotographyOpt2.ChkboxName = "";
            objOrderableProductsDayPhotographyOpt2.cssClass = "DayPhotographyOptions2 RE1";
            lstOrderableProductsDayPhotographyOpt2.Add(objOrderableProductsDayPhotographyOpt2);

            OrderableProductsDayPhotographyOpt2 objOrderableProductsDayPhotographyOpt4 = new OrderableProductsDayPhotographyOpt2();
            objOrderableProductsDayPhotographyOpt2.Row_Id = 4;
            objOrderableProductsDayPhotographyOpt2.WebName = "Rear external 2";
            objOrderableProductsDayPhotographyOpt2.ChkboxName = "";
            objOrderableProductsDayPhotographyOpt2.cssClass = "DayPhotographyOptions2 RE2";
            lstOrderableProductsDayPhotographyOpt2.Add(objOrderableProductsDayPhotographyOpt2);

            #endregion

            #region Region for bind List for Photography Image Options 5

            List<OrderableProductsDayPhotographyOpt5> lstOrderableProductsDayPhotographyOpt5 = new List<OrderableProductsDayPhotographyOpt5>();

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_1 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_1.Row_Id = 5;
            DayPhotographyOpt1_1.WebName = "Front external 1";
            DayPhotographyOpt1_1.ChkboxName = "chk_img_5_FR1";
            //  DayPhotographyOpt1_1.cssClass = "chkposition";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_1);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_2 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_2.Row_Id = 6;
            DayPhotographyOpt1_2.WebName = "Front external 2";
            DayPhotographyOpt1_2.ChkboxName = "chk_img_5_FR2";
            //  DayPhotographyOpt1_2.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_2);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_3 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_3.Row_Id = 7;
            DayPhotographyOpt1_3.WebName = "Living";
            DayPhotographyOpt1_3.ChkboxName = "chk_img_5_Living";
            //  DayPhotographyOpt1_3.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_3);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_4 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_4.Row_Id = 8;
            DayPhotographyOpt1_4.WebName = "Dining";
            DayPhotographyOpt1_4.ChkboxName = "chk_img_5_Dining";
            //  DayPhotographyOpt1_4.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_4);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_5 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_5.Row_Id = 9;
            DayPhotographyOpt1_5.WebName = "Family";
            DayPhotographyOpt1_5.ChkboxName = "chk_img_5_Family";
            // DayPhotographyOpt1_5.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_5);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_6 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_6.Row_Id = 10;
            DayPhotographyOpt1_6.WebName = "Kitchen";
            DayPhotographyOpt1_6.ChkboxName = "chk_img_5_Kitchen";
            // DayPhotographyOpt1_6.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_6);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_7 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_7.Row_Id = 11;
            DayPhotographyOpt1_7.WebName = "Bathroom";
            DayPhotographyOpt1_7.ChkboxName = "chk_img_5_Bathroom";
            //  DayPhotographyOpt1_7.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_7);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_8 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_8.Row_Id = 12;
            DayPhotographyOpt1_8.WebName = "Ensuite";
            DayPhotographyOpt1_8.ChkboxName = "chk_img_5_Ensuite";
            //  DayPhotographyOpt1_8.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_8);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_9 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_9.Row_Id = 13;
            DayPhotographyOpt1_9.WebName = "Master bed";
            DayPhotographyOpt1_9.ChkboxName = "chk_img_5_MasterBed";
            //  DayPhotographyOpt1_9.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_9);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_10 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_10.Row_Id = 14;
            DayPhotographyOpt1_10.WebName = "2nd bedroom";
            DayPhotographyOpt1_10.ChkboxName = "chk_img_5_2Bedroom";
            //   DayPhotographyOpt1_10.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_10);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_11 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_11.Row_Id = 15;
            DayPhotographyOpt1_11.WebName = "Rumpus";
            DayPhotographyOpt1_11.ChkboxName = "chk_img_5_Rumpus";
            //  DayPhotographyOpt1_11.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_11);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_12 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_12.Row_Id = 16;
            DayPhotographyOpt1_12.WebName = "Home theatre";
            DayPhotographyOpt1_12.ChkboxName = "chk_img_5_HomeTheatre";
            //  DayPhotographyOpt1_12.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_12);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_13 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_13.Row_Id = 17;
            DayPhotographyOpt1_13.WebName = "Pool";
            DayPhotographyOpt1_13.ChkboxName = "chk_img_5_Pool";
            //  DayPhotographyOpt1_13.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_13);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_14 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_14.Row_Id = 18;
            DayPhotographyOpt1_14.WebName = "Rear external 1";
            DayPhotographyOpt1_14.ChkboxName = "chk_img_5_RE1";
            //  DayPhotographyOpt1_14.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_14);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_15 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_15.Row_Id = 19;
            DayPhotographyOpt1_15.WebName = "Rear external 2";
            DayPhotographyOpt1_15.ChkboxName = "chk_img_5_RE2";
            //  DayPhotographyOpt1_15.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_15);

            OrderableProductsDayPhotographyOpt5 DayPhotographyOpt1_16 = new OrderableProductsDayPhotographyOpt5();
            DayPhotographyOpt1_16.Row_Id = 20;
            DayPhotographyOpt1_16.WebName = "Lifestyle";
            DayPhotographyOpt1_16.ChkboxName = "chk_img_5_Lifestyle";
            //  DayPhotographyOpt1_16.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt5.Add(DayPhotographyOpt1_16);

            #endregion

            #region Region for bind List for Photography Image Options 8

            List<OrderableProductsDayPhotographyOpt8> lstOrderableProductsDayPhotographyOpt8 = new List<OrderableProductsDayPhotographyOpt8>();

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_1 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_1.Row_Id = 21;
            DayPhotographyOpt8_1.WebName = "Front external 1";
            DayPhotographyOpt8_1.ChkboxName = "chk_img_8_FR1";
            //  DayPhotographyOpt1_1.cssClass = "chkposition";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_1);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_2 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_2.Row_Id = 22;
            DayPhotographyOpt8_2.WebName = "Front external 2";
            DayPhotographyOpt8_2.ChkboxName = "chk_img_8_FR2";
            //  DayPhotographyOpt1_2.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_2);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_3 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_3.Row_Id = 23;
            DayPhotographyOpt8_3.WebName = "Living";
            DayPhotographyOpt8_3.ChkboxName = "chk_img_8_Living";
            //  DayPhotographyOpt1_3.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_3);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_4 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_4.Row_Id = 24;
            DayPhotographyOpt8_4.WebName = "Dining";
            DayPhotographyOpt8_4.ChkboxName = "chk_img_8_Dining";
            //  DayPhotographyOpt1_4.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_4);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_5 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_5.Row_Id = 25;
            DayPhotographyOpt8_5.WebName = "Family";
            DayPhotographyOpt8_5.ChkboxName = "chk_img_8_Family";
            // DayPhotographyOpt1_5.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_5);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_6 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_6.Row_Id = 26;
            DayPhotographyOpt8_6.WebName = "Kitchen";
            DayPhotographyOpt8_6.ChkboxName = "chk_img_8_Kitchen";
            // DayPhotographyOpt1_6.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_6);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_7 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_7.Row_Id = 27;
            DayPhotographyOpt8_7.WebName = "Bathroom";
            DayPhotographyOpt8_7.ChkboxName = "chk_img_8_Bathroom";
            //  DayPhotographyOpt1_7.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_7);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_8 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_8.Row_Id = 28;
            DayPhotographyOpt8_8.WebName = "Ensuite";
            DayPhotographyOpt8_8.ChkboxName = "chk_img_8_Ensuite";
            //  DayPhotographyOpt1_8.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_8);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_9 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_9.Row_Id = 29;
            DayPhotographyOpt8_9.WebName = "Master bed";
            DayPhotographyOpt8_9.ChkboxName = "chk_img_8_MasterBed";
            //  DayPhotographyOpt1_9.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_9);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_10 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_10.Row_Id = 30;
            DayPhotographyOpt8_10.WebName = "2nd bedroom";
            DayPhotographyOpt8_10.ChkboxName = "chk_img_8_2Bedroom";
            //   DayPhotographyOpt1_10.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_10);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_11 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_11.Row_Id = 31;
            DayPhotographyOpt8_11.WebName = "Rumpus";
            DayPhotographyOpt8_11.ChkboxName = "chk_img_8_Rumpus";
            //  DayPhotographyOpt1_11.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_11);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_12 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_12.Row_Id = 32;
            DayPhotographyOpt8_12.WebName = "Home theatre";
            DayPhotographyOpt8_12.ChkboxName = "chk_img_8_HomeTheatre";
            //  DayPhotographyOpt1_12.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_12);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_13 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_13.Row_Id = 33;
            DayPhotographyOpt8_13.WebName = "Pool";
            DayPhotographyOpt8_13.ChkboxName = "chk_img_8_Pool";
            //  DayPhotographyOpt1_13.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_13);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_14 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_14.Row_Id = 34;
            DayPhotographyOpt8_14.WebName = "Rear external 1";
            DayPhotographyOpt8_14.ChkboxName = "chk_img_8_RE1";
            //  DayPhotographyOpt1_14.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_14);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_15 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_15.Row_Id = 35;
            DayPhotographyOpt8_15.WebName = "Rear external 2";
            DayPhotographyOpt8_15.ChkboxName = "chk_img_8_RE2";
            //  DayPhotographyOpt1_15.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_15);

            OrderableProductsDayPhotographyOpt8 DayPhotographyOpt8_16 = new OrderableProductsDayPhotographyOpt8();
            DayPhotographyOpt8_16.Row_Id = 36;
            DayPhotographyOpt8_16.WebName = "Lifestyle";
            DayPhotographyOpt8_16.ChkboxName = "chk_img_8_Lifestyle";
            //  DayPhotographyOpt1_16.cssClass = "DayPhotographyOptions2 FE1";
            lstOrderableProductsDayPhotographyOpt8.Add(DayPhotographyOpt8_16);

            #endregion

            #region Region for bind List for Dusk Photography Image Options

            List<OrderableProductsDuskPhotography> lstOrderableProductsduskPhotography = new List<OrderableProductsDuskPhotography>();

            OrderableProductsDuskPhotography duskPhotography = new OrderableProductsDuskPhotography();
            duskPhotography.Row_Id = 37;
            duskPhotography.WebName = "Upto 8 final Images";
            duskPhotography.ChkboxName = "chk_dusk_8final_img";
            lstOrderableProductsduskPhotography.Add(duskPhotography);

            #endregion

            #region Region for bind List for Dusk Photography Options 8

            List<OrderableProductsDuskPhotographyOpt8> lstOrderableProductsDuskPhotographyOpt8 = new List<OrderableProductsDuskPhotographyOpt8>();

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_1 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_1.Row_Id = 38;
            DuskPhotographyOpt8_1.WebName = "Front external 1";
            DuskPhotographyOpt8_1.ChkboxName = "chk_dusk_8final_FR1";
            //  DuskPhotographyOpt1_1.cssClass = "chkposition";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_1);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_2 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_2.Row_Id = 39;
            DuskPhotographyOpt8_2.WebName = "Front external 2";
            DuskPhotographyOpt8_2.ChkboxName = "chk_dusk_8final_FR2";
            //  DuskPhotographyOpt1_2.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_2);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_3 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_3.Row_Id = 40;
            DuskPhotographyOpt8_3.WebName = "Living";
            DuskPhotographyOpt8_3.ChkboxName = "chk_dusk_8final_Living";
            //  DuskPhotographyOpt1_3.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_3);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_4 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_4.Row_Id = 41;
            DuskPhotographyOpt8_4.WebName = "Dining";
            DuskPhotographyOpt8_4.ChkboxName = "chk_dusk_8final_Dining";
            //  DuskPhotographyOpt1_4.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_4);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_5 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_5.Row_Id = 42;
            DuskPhotographyOpt8_5.WebName = "Family";
            DuskPhotographyOpt8_5.ChkboxName = "chk_dusk_8final_Family";
            // DuskPhotographyOpt1_5.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_5);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_6 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_6.Row_Id = 43;
            DuskPhotographyOpt8_6.WebName = "Kitchen";
            DuskPhotographyOpt8_6.ChkboxName = "chk_dusk_8final_Kitchen";
            // DuskPhotographyOpt1_6.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_6);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_7 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_7.Row_Id = 44;
            DuskPhotographyOpt8_7.WebName = "Bathroom";
            DuskPhotographyOpt8_7.ChkboxName = "chk_dusk_8final_Bathroom";
            //  DuskPhotographyOpt1_7.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_7);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_8 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_8.Row_Id = 45;
            DuskPhotographyOpt8_8.WebName = "Ensuite";
            DuskPhotographyOpt8_8.ChkboxName = "chk_dusk_8final_Ensuite";
            //  DuskPhotographyOpt1_8.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_8);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_9 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_9.Row_Id = 46;
            DuskPhotographyOpt8_9.WebName = "Master bed";
            DuskPhotographyOpt8_9.ChkboxName = "chk_dusk_8final_MasterBed";
            //  DuskPhotographyOpt1_9.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_9);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_10 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_10.Row_Id = 47;
            DuskPhotographyOpt8_10.WebName = "2nd bedroom";
            DuskPhotographyOpt8_10.ChkboxName = "chk_dusk_8final_2Bedroom";
            //   DuskPhotographyOpt1_10.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_10);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_11 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_11.Row_Id = 48;
            DuskPhotographyOpt8_11.WebName = "Rumpus";
            DuskPhotographyOpt8_11.ChkboxName = "chk_dusk_8final_Rumpus";
            //  DuskPhotographyOpt1_11.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_11);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_12 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_12.Row_Id = 49;
            DuskPhotographyOpt8_12.WebName = "Home theatre";
            DuskPhotographyOpt8_12.ChkboxName = "chk_dusk_8final_HomeTheatre";
            //  DuskPhotographyOpt1_12.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_12);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_13 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_13.Row_Id = 50;
            DuskPhotographyOpt8_13.WebName = "Pool";
            DuskPhotographyOpt8_13.ChkboxName = "chk_dusk_8final_Pool";
            //  DuskPhotographyOpt1_13.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_13);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_14 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_14.Row_Id = 51;
            DuskPhotographyOpt8_14.WebName = "Rear external 1";
            DuskPhotographyOpt8_14.ChkboxName = "chk_dusk_8final_RE1";
            //  DuskPhotographyOpt1_14.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_14);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_15 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_15.Row_Id = 52;
            DuskPhotographyOpt8_15.WebName = "Rear external 2";
            DuskPhotographyOpt8_15.ChkboxName = "chk_dusk_8final_RE2";
            //  DuskPhotographyOpt1_15.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_15);

            OrderableProductsDuskPhotographyOpt8 DuskPhotographyOpt8_16 = new OrderableProductsDuskPhotographyOpt8();
            DuskPhotographyOpt8_16.Row_Id = 53;
            DuskPhotographyOpt8_16.WebName = "Lifestyle";
            DuskPhotographyOpt8_16.ChkboxName = "chk_dusk_8final_Lifestyle";
            //  DuskPhotographyOpt1_16.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsDuskPhotographyOpt8.Add(DuskPhotographyOpt8_16);

            #endregion

            #region Region for bind List for Prestige Day - Up to 12 Final Images

            List<OrderableProductsPrestigeDayPhoto12> lstOrderableProductsPrestigeDayPhoto12 = new List<OrderableProductsPrestigeDayPhoto12>();

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_1 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_1.Row_Id = 54;
            prestigeDayPhotographyOpt_1.WebName = "Front external 1";
            prestigeDayPhotographyOpt_1.ChkboxName = "chk_dusk_8final_FR1";
            //  DuskPhotographyOpt1_1.cssClass = "chkposition";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_1);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_2 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_2.Row_Id = 55;
            prestigeDayPhotographyOpt_2.WebName = "Front external 2";
            prestigeDayPhotographyOpt_2.ChkboxName = "chk_dusk_8final_FR2";
            //  DuskPhotographyOpt1_2.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_2);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_3 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_3.Row_Id = 56;
            prestigeDayPhotographyOpt_3.WebName = "Living";
            prestigeDayPhotographyOpt_3.ChkboxName = "chk_dusk_8final_Living";
            //  DuskPhotographyOpt1_3.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_3);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_4 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_4.Row_Id = 57;
            prestigeDayPhotographyOpt_4.WebName = "Dining";
            prestigeDayPhotographyOpt_4.ChkboxName = "chk_dusk_8final_Dining";
            //  DuskPhotographyOpt1_4.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_4);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_5 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_5.Row_Id = 58;
            prestigeDayPhotographyOpt_5.WebName = "Family";
            prestigeDayPhotographyOpt_5.ChkboxName = "chk_dusk_8final_Family";
            // DuskPhotographyOpt1_5.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_5);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_6 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_6.Row_Id = 59;
            prestigeDayPhotographyOpt_6.WebName = "Kitchen";
            prestigeDayPhotographyOpt_6.ChkboxName = "chk_dusk_8final_Kitchen";
            // DuskPhotographyOpt1_6.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_6);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_7 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_7.Row_Id = 60;
            prestigeDayPhotographyOpt_7.WebName = "Bathroom";
            prestigeDayPhotographyOpt_7.ChkboxName = "chk_dusk_8final_Bathroom";
            //  DuskPhotographyOpt1_7.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_7);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_8 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_8.Row_Id = 61;
            prestigeDayPhotographyOpt_8.WebName = "Ensuite";
            prestigeDayPhotographyOpt_8.ChkboxName = "chk_dusk_8final_Ensuite";
            //  DuskPhotographyOpt1_8.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_8);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_9 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_9.Row_Id = 62;
            prestigeDayPhotographyOpt_9.WebName = "Master bed";
            prestigeDayPhotographyOpt_9.ChkboxName = "chk_dusk_8final_MasterBed";
            //  DuskPhotographyOpt1_9.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_9);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_10 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_10.Row_Id = 63;
            prestigeDayPhotographyOpt_10.WebName = "2nd bedroom";
            prestigeDayPhotographyOpt_10.ChkboxName = "chk_dusk_8final_2Bedroom";
            //   DuskPhotographyOpt1_10.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_10);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_11 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_11.Row_Id = 64;
            prestigeDayPhotographyOpt_11.WebName = "Rumpus";
            prestigeDayPhotographyOpt_11.ChkboxName = "chk_dusk_8final_Rumpus";
            //  DuskPhotographyOpt1_11.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_11);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_12 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_12.Row_Id = 65;
            prestigeDayPhotographyOpt_12.WebName = "Home theatre";
            prestigeDayPhotographyOpt_12.ChkboxName = "chk_dusk_8final_HomeTheatre";
            //  DuskPhotographyOpt1_12.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_12);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_13 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_13.Row_Id = 66;
            prestigeDayPhotographyOpt_13.WebName = "Pool";
            prestigeDayPhotographyOpt_13.ChkboxName = "chk_dusk_8final_Pool";
            //  DuskPhotographyOpt1_13.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_13);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_14 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_14.Row_Id = 67;
            prestigeDayPhotographyOpt_14.WebName = "Rear external 1";
            prestigeDayPhotographyOpt_14.ChkboxName = "chk_dusk_8final_RE1";
            //  DuskPhotographyOpt1_14.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_14);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_15 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_15.Row_Id = 68;
            prestigeDayPhotographyOpt_15.WebName = "Rear external 2";
            prestigeDayPhotographyOpt_15.ChkboxName = "chk_dusk_8final_RE2";
            //  DuskPhotographyOpt1_15.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_15);

            OrderableProductsPrestigeDayPhoto12 prestigeDayPhotographyOpt_16 = new OrderableProductsPrestigeDayPhoto12();
            prestigeDayPhotographyOpt_16.Row_Id = 69;
            prestigeDayPhotographyOpt_16.WebName = "Lifestyle";
            prestigeDayPhotographyOpt_16.ChkboxName = "chk_dusk_8final_Lifestyle";
            //  DuskPhotographyOpt1_16.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDayPhoto12.Add(prestigeDayPhotographyOpt_16);

            #endregion

            #region Region for bind List for Prestige Dusk - Up to 12 Final Images

            List<OrderableProductsPrestigeDuskPhoto12> lstOrderableProductsPrestigeDuskPhoto12 = new List<OrderableProductsPrestigeDuskPhoto12>();

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_1 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_1.Row_Id = 54;
            prestigeDuskPhotographyOpt_1.WebName = "Front external 1";
            prestigeDuskPhotographyOpt_1.ChkboxName = "chk_dusk_8final_FR1";
            //  DuskPhotographyOpt1_1.cssClass = "chkposition";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_1);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_2 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_2.Row_Id = 55;
            prestigeDuskPhotographyOpt_2.WebName = "Front external 2";
            prestigeDuskPhotographyOpt_2.ChkboxName = "chk_dusk_8final_FR2";
            //  DuskPhotographyOpt1_2.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_2);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_3 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_3.Row_Id = 56;
            prestigeDuskPhotographyOpt_3.WebName = "Living";
            prestigeDuskPhotographyOpt_3.ChkboxName = "chk_dusk_8final_Living";
            //  DuskPhotographyOpt1_3.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_3);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_4 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_4.Row_Id = 57;
            prestigeDuskPhotographyOpt_4.WebName = "Dining";
            prestigeDuskPhotographyOpt_4.ChkboxName = "chk_dusk_8final_Dining";
            //  DuskPhotographyOpt1_4.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_4);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_5 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_5.Row_Id = 58;
            prestigeDuskPhotographyOpt_5.WebName = "Family";
            prestigeDuskPhotographyOpt_5.ChkboxName = "chk_dusk_8final_Family";
            // DuskPhotographyOpt1_5.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_5);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_6 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_6.Row_Id = 59;
            prestigeDuskPhotographyOpt_6.WebName = "Kitchen";
            prestigeDuskPhotographyOpt_6.ChkboxName = "chk_dusk_8final_Kitchen";
            // DuskPhotographyOpt1_6.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_6);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_7 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_7.Row_Id = 60;
            prestigeDuskPhotographyOpt_7.WebName = "Bathroom";
            prestigeDuskPhotographyOpt_7.ChkboxName = "chk_dusk_8final_Bathroom";
            //  DuskPhotographyOpt1_7.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_7);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_8 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_8.Row_Id = 61;
            prestigeDuskPhotographyOpt_8.WebName = "Ensuite";
            prestigeDuskPhotographyOpt_8.ChkboxName = "chk_dusk_8final_Ensuite";
            //  DuskPhotographyOpt1_8.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_8);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_9 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_9.Row_Id = 62;
            prestigeDuskPhotographyOpt_9.WebName = "Master bed";
            prestigeDuskPhotographyOpt_9.ChkboxName = "chk_dusk_8final_MasterBed";
            //  DuskPhotographyOpt1_9.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_9);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_10 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_10.Row_Id = 63;
            prestigeDuskPhotographyOpt_10.WebName = "2nd bedroom";
            prestigeDuskPhotographyOpt_10.ChkboxName = "chk_dusk_8final_2Bedroom";
            //   DuskPhotographyOpt1_10.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_10);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_11 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_11.Row_Id = 64;
            prestigeDuskPhotographyOpt_11.WebName = "Rumpus";
            prestigeDuskPhotographyOpt_11.ChkboxName = "chk_dusk_8final_Rumpus";
            //  DuskPhotographyOpt1_11.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_11);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_12 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_12.Row_Id = 65;
            prestigeDuskPhotographyOpt_12.WebName = "Home theatre";
            prestigeDuskPhotographyOpt_12.ChkboxName = "chk_dusk_8final_HomeTheatre";
            //  DuskPhotographyOpt1_12.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_12);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_13 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_13.Row_Id = 66;
            prestigeDuskPhotographyOpt_13.WebName = "Pool";
            prestigeDuskPhotographyOpt_13.ChkboxName = "chk_dusk_8final_Pool";
            //  DuskPhotographyOpt1_13.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_13);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_14 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_14.Row_Id = 67;
            prestigeDuskPhotographyOpt_14.WebName = "Rear external 1";
            prestigeDuskPhotographyOpt_14.ChkboxName = "chk_dusk_8final_RE1";
            //  DuskPhotographyOpt1_14.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_14);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_15 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_15.Row_Id = 68;
            prestigeDuskPhotographyOpt_15.WebName = "Rear external 2";
            prestigeDuskPhotographyOpt_15.ChkboxName = "chk_dusk_8final_RE2";
            //  DuskPhotographyOpt1_15.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_15);

            OrderableProductsPrestigeDuskPhoto12 prestigeDuskPhotographyOpt_16 = new OrderableProductsPrestigeDuskPhoto12();
            prestigeDuskPhotographyOpt_16.Row_Id = 69;
            prestigeDuskPhotographyOpt_16.WebName = "Lifestyle";
            prestigeDuskPhotographyOpt_16.ChkboxName = "chk_dusk_8final_Lifestyle";
            //  DuskPhotographyOpt1_16.cssClass = "DuskPhotographyOptions2 FE1";
            lstOrderableProductsPrestigeDuskPhoto12.Add(prestigeDuskPhotographyOpt_16);

            #endregion

            #region Region for bind List for Rental  PhotoGraphy - Option 5

            List<OrderableProductsRentalPhoto5> lstOrderableProductsRentalPhoto5 = new List<OrderableProductsRentalPhoto5>();

            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 70;
                prestigeRentalPhotographyOpt_1.WebName = "Front external 1";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_FR1";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }


            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 71;
                prestigeRentalPhotographyOpt_1.WebName = "Front external 2";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_FR2";
                //  DuskPhotographyOpt1_2.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 72;
                prestigeRentalPhotographyOpt_1.WebName = "Living";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Living";
                //  DuskPhotographyOpt1_3.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 73;
                prestigeRentalPhotographyOpt_1.WebName = "Dining";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Dining";
                //  DuskPhotographyOpt1_4.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 74;
                prestigeRentalPhotographyOpt_1.WebName = "Family";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Family";
                // DuskPhotographyOpt1_5.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 75;
                prestigeRentalPhotographyOpt_1.WebName = "Kitchen";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Kitchen";
                // DuskPhotographyOpt1_6.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 76;
                prestigeRentalPhotographyOpt_1.WebName = "Bathroom";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Bathroom";
                //  DuskPhotographyOpt1_7.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 77;
                prestigeRentalPhotographyOpt_1.WebName = "Ensuite";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Ensuite";
                //  DuskPhotographyOpt1_8.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 78;
                prestigeRentalPhotographyOpt_1.WebName = "Master bed";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_MasterBed";
                //  DuskPhotographyOpt1_9.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 79;
                prestigeRentalPhotographyOpt_1.WebName = "2nd bedroom";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_2Bedroom";
                //   DuskPhotographyOpt1_10.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 80;
                prestigeRentalPhotographyOpt_1.WebName = "Rumpus";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Rumpus";
                //  DuskPhotographyOpt1_11.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 81;
                prestigeRentalPhotographyOpt_1.WebName = "Home theatre";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_HomeTheatre";
                //  DuskPhotographyOpt1_12.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 82;
                prestigeRentalPhotographyOpt_1.WebName = "Pool";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Pool";
                //  DuskPhotographyOpt1_13.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 83;
                prestigeRentalPhotographyOpt_1.WebName = "Rear external 1";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_RE1";
                //  DuskPhotographyOpt1_14.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 84;
                prestigeRentalPhotographyOpt_1.WebName = "Rear external 2";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_RE2";
                //  DuskPhotographyOpt1_15.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto5 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto5();
                prestigeRentalPhotographyOpt_1.Row_Id = 85;
                prestigeRentalPhotographyOpt_1.WebName = "Lifestyle";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Lifestyle";
                //  DuskPhotographyOpt1_16.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto5.Add(prestigeRentalPhotographyOpt_1);
            }
            #endregion

            #region Region for bind List for Rental  PhotoGraphy - Option 10

            List<OrderableProductsRentalPhoto10> lstOrderableProductsRentalPhoto10 = new List<OrderableProductsRentalPhoto10>();

            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 86;
                prestigeRentalPhotographyOpt_1.WebName = "Front external 1";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_FR1";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }


            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeDuskPhotographyOpt_2.Row_Id = 87;
                prestigeDuskPhotographyOpt_2.WebName = "Front external 2";
                prestigeDuskPhotographyOpt_2.ChkboxName = "chk_dusk_8final_FR2";
                //  DuskPhotographyOpt1_2.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeDuskPhotographyOpt_3.Row_Id = 88;
                prestigeDuskPhotographyOpt_3.WebName = "Living";
                prestigeDuskPhotographyOpt_3.ChkboxName = "chk_dusk_8final_Living";
                //  DuskPhotographyOpt1_3.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeDuskPhotographyOpt_1.Row_Id = 89;
                prestigeDuskPhotographyOpt_1.WebName = "Dining";
                prestigeDuskPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Dining";
                //  DuskPhotographyOpt1_4.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 90;
                prestigeRentalPhotographyOpt_1.WebName = "Family";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Family";
                // DuskPhotographyOpt1_5.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 91;
                prestigeRentalPhotographyOpt_1.WebName = "Kitchen";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Kitchen";
                // DuskPhotographyOpt1_6.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 92;
                prestigeRentalPhotographyOpt_1.WebName = "Bathroom";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Bathroom";
                //  DuskPhotographyOpt1_7.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 93;
                prestigeRentalPhotographyOpt_1.WebName = "Ensuite";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Ensuite";
                //  DuskPhotographyOpt1_8.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 94;
                prestigeRentalPhotographyOpt_1.WebName = "Master bed";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_MasterBed";
                //  DuskPhotographyOpt1_9.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 95;
                prestigeRentalPhotographyOpt_1.WebName = "2nd bedroom";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_2Bedroom";
                //   DuskPhotographyOpt1_10.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 96;
                prestigeRentalPhotographyOpt_1.WebName = "Rumpus";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Rumpus";
                //  DuskPhotographyOpt1_11.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }

            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 97;
                prestigeRentalPhotographyOpt_1.WebName = "Home theatre";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_HomeTheatre";
                //  DuskPhotographyOpt1_12.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 98;
                prestigeRentalPhotographyOpt_1.WebName = "Pool";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Pool";
                //  DuskPhotographyOpt1_13.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 99;
                prestigeRentalPhotographyOpt_1.WebName = "Rear external 1";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_RE1";
                //  DuskPhotographyOpt1_14.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 100;
                prestigeRentalPhotographyOpt_1.WebName = "Rear External 2";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_RE2";
                //  DuskPhotographyOpt1_15.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            {
                OrderableProductsRentalPhoto10 prestigeRentalPhotographyOpt_1 = new OrderableProductsRentalPhoto10();
                prestigeRentalPhotographyOpt_1.Row_Id = 101;
                prestigeRentalPhotographyOpt_1.WebName = "Lifestyle";
                prestigeRentalPhotographyOpt_1.ChkboxName = "chk_dusk_8final_Lifestyle";
                //  DuskPhotographyOpt1_16.cssClass = "DuskPhotographyOptions2 FE1";
                lstOrderableProductsRentalPhoto10.Add(prestigeRentalPhotographyOpt_1);
            }
            #endregion

            // objNewOrderModel.ProductGoruplist = lstOrderableProductsInfo.ToList();

            objNewOrderModel.primaryProductGrp = lstPrimaryProductGrp.ToList();
            objNewOrderModel.secondaryProductGroup = lstSecondryProductGrp.ToList();

            objNewOrderModel.orderableProductsDayPhoto2 = lstOrderableProductsDayPhotographyOpt2.ToList();
            objNewOrderModel.orderableProductsDayPhoto5 = lstOrderableProductsDayPhotographyOpt5.ToList();
            objNewOrderModel.orderableProductsDayPhoto8 = lstOrderableProductsDayPhotographyOpt8.ToList();
            objNewOrderModel.orderableProductsduskPhoto = lstOrderableProductsduskPhotography.ToList();
            objNewOrderModel.orderableProductsDuskPhoto8 = lstOrderableProductsDuskPhotographyOpt8.ToList();
            objNewOrderModel.orderableProductsPrestigeDayPhoto12 = lstOrderableProductsPrestigeDayPhoto12.ToList();
            objNewOrderModel.orderableProductsPrestigeDuskPhoto12 = lstOrderableProductsPrestigeDuskPhoto12.ToList();
            objNewOrderModel.orderableProductsRentalPhoto5 = lstOrderableProductsRentalPhoto5.ToList();
            objNewOrderModel.orderableProductsRentalPhoto10 = lstOrderableProductsRentalPhoto10.ToList();

            #endregion

            #region Region for bind List for UAV Drone/Aerial Photography

            List<OrderableProductsUAVDrone> lstOrderableProductsUAVDrone = new List<OrderableProductsUAVDrone>();

            {
                OrderableProductsUAVDrone ProductsUAVDrone = new OrderableProductsUAVDrone();
                ProductsUAVDrone.Row_Id = 102;
                ProductsUAVDrone.WebName = "Up to 3 final images and one landbox / Text overlay Images from ground level to 400 feet - from any safe position onsite. Complimentary Image Proofing";
                ProductsUAVDrone.ChkboxName = "chk_UAVDroneUP3";
                ProductsUAVDrone.LabelText = "Aerial Photography - UAV Drone";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsUAVDrone.Add(ProductsUAVDrone);
            }

            {
                OrderableProductsUAVDrone ProductsUAVDrone = new OrderableProductsUAVDrone();
                ProductsUAVDrone.Row_Id = 103;
                ProductsUAVDrone.WebName = "Up to 5 final images and one landbox / Text overlay Images from ground level to 400 feet - from any safe position onsite. Complimentary Image Proofing";
                ProductsUAVDrone.ChkboxName = "chk_UAVDroneUP5";
                ProductsUAVDrone.LabelText = "Aerial Photography - UAV Drone";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsUAVDrone.Add(ProductsUAVDrone);
            }

            {
                OrderableProductsUAVDrone ProductsUAVDrone = new OrderableProductsUAVDrone();
                ProductsUAVDrone.Row_Id = 104;
                ProductsUAVDrone.WebName = "Up to 5 final images with 1 landbox / text overlay";
                ProductsUAVDrone.ChkboxName = "chk_UAVDrone_Helicop";
                ProductsUAVDrone.LabelText = "Aerial Photography - Helicopter";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsUAVDrone.Add(ProductsUAVDrone);
            }

            {
                OrderableProductsUAVDrone ProductsUAVDrone = new OrderableProductsUAVDrone();
                ProductsUAVDrone.Row_Id = 105;
                ProductsUAVDrone.WebName = "Up to 3 final images with 1 landbox / text overlay";
                ProductsUAVDrone.ChkboxName = "chk_UAVDrone_Elevated";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                ProductsUAVDrone.LabelText = "Elevated Pole Photography";
                lstOrderableProductsUAVDrone.Add(ProductsUAVDrone);
            }

            objNewOrderModel.orderableProductsUAVDrone = lstOrderableProductsUAVDrone.ToList();

            #endregion

            #region Region for bind List for Copy Writing

            List<OrderableProductsCopyWriting> lstOrderableProductsCopyWriting = new List<OrderableProductsCopyWriting>();

            {
                OrderableProductsCopyWriting ProductsCopyWriting = new OrderableProductsCopyWriting();
                ProductsCopyWriting.Row_Id = 103;
                ProductsCopyWriting.WebName = "Yes";
                ProductsCopyWriting.ChkboxName = "chk_copywrite_Onsite";
                ProductsCopyWriting.LabelText = "Onsite Copy";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsCopyWriting.Add(ProductsCopyWriting);
            }

            {
                OrderableProductsCopyWriting ProductsCopyWriting = new OrderableProductsCopyWriting();
                ProductsCopyWriting.Row_Id = 104;
                ProductsCopyWriting.WebName = "Yes";
                ProductsCopyWriting.ChkboxName = "chk_copywrite_Offsite";
                ProductsCopyWriting.LabelText = "Offsite from photography and floorplans";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsCopyWriting.Add(ProductsCopyWriting);
            }

            {
                OrderableProductsCopyWriting ProductsCopyWriting = new OrderableProductsCopyWriting();
                ProductsCopyWriting.Row_Id = 105;
                ProductsCopyWriting.WebName = "Yes";
                ProductsCopyWriting.ChkboxName = "chk_Rewrite_agent";
                ProductsCopyWriting.LabelText = "Re-write from agents own";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsCopyWriting.Add(ProductsCopyWriting);
            }

            objNewOrderModel.orderableProductsCopyWriting = lstOrderableProductsCopyWriting.ToList();

            #endregion

            #region Region for bind List for Video and Image tours

            List<OrderableProductsVideoAndImage> lstOrderableProductsVideoAndImage = new List<OrderableProductsVideoAndImage>();

            {
                OrderableProductsVideoAndImage ProductsVideoAndImage = new OrderableProductsVideoAndImage();
                ProductsVideoAndImage.Row_Id = 106;
                ProductsVideoAndImage.WebName = "Yes";
                ProductsVideoAndImage.ChkboxName = "chk_video_propvideo";
                ProductsVideoAndImage.LabelText = "Property Video";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsVideoAndImage.Add(ProductsVideoAndImage);
            }

            {
                OrderableProductsVideoAndImage ProductsVideoAndImage = new OrderableProductsVideoAndImage();
                ProductsVideoAndImage.Row_Id = 107;
                ProductsVideoAndImage.WebName = "Yes";
                ProductsVideoAndImage.ChkboxName = "chk_video_profile";
                ProductsVideoAndImage.LabelText = "Corporate / Profile Videos";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsVideoAndImage.Add(ProductsVideoAndImage);
            }

            {
                OrderableProductsVideoAndImage ProductsVideoAndImage = new OrderableProductsVideoAndImage();
                ProductsVideoAndImage.Row_Id = 108;
                ProductsVideoAndImage.WebName = "Yes";
                ProductsVideoAndImage.ChkboxName = "chk_ImageTours_stillImg";
                ProductsVideoAndImage.LabelText = "Image Tours";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstOrderableProductsVideoAndImage.Add(ProductsVideoAndImage);
            }

            objNewOrderModel.orderableProductsVideoAndImage = lstOrderableProductsVideoAndImage.ToList();

            #endregion

            #region Region for bind List for Keys available

            List<Keys> lstKeysAvailable = new List<Keys>();
            {
                Keys KeysAvailable = new Keys();
                KeysAvailable.Row_Id = 250;
                KeysAvailable.WebName = "Yes";
                KeysAvailable.ChkboxName = "chk_KeyinsafeProp";
                KeysAvailable.LabelText = "Keys in safe at property:";
                //  DuskPhotographyOpt1_1.cssClass = "chkposition";
                lstKeysAvailable.Add(KeysAvailable);
            }

            {
                Keys KeysAvailable = new Keys();
                KeysAvailable.Row_Id = 251;
                KeysAvailable.WebName = "Yes";
                KeysAvailable.ChkboxName = "chk_KeyinOffice";
                KeysAvailable.LabelText = "Keys in office:";
                lstKeysAvailable.Add(KeysAvailable);
            }

            objNewOrderModel.keys = lstKeysAvailable.ToList();

            #endregion

            return objNewOrderModel;
        }
    }
}

