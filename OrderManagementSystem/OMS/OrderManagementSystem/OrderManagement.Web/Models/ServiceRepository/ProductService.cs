using System;
using System.Collections.Generic;
using System.Data;
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
    public interface IProductService
    {
        IEnumerable<SpGetAllProduct> GetAllProductBySp(int orgId);
        CompanyProductModel GetProductCompanyList(string id);
        IList<ProductModel> GetAll();
        CompanyProductModel Update(ProductModel model);
        List<ProductModel> GetAllProductsBygroup(string groupname);
        int EnableProductforCompany(string prodid, string companyid, bool ischecked);
        IList<ProductSchedulesModel> GetAllProductScheduleByProdId(int prodid);
        Product GetProductById(int id);
        IList<ProductCategories> GetProductgroupBySp(int? orgId, int? parentId);
        IList<SelectProductGroupOptions> GetProductGroupOptionBySp(int productid);
        void SaveWebOrderable(ProductModel model);
    }

    public class ProductService : IProductService
    {
        private IProductRepository _repository;
        private ICompanyRepository _companyRepository;
        private IProductCompanyRepository _productCompanyRepo;
        private IProductGroupRepository _productgroupRepo;
        private IProductScheduleRepository _productScheduleRepo;
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
            _companyRepository = new CompanyRepository();
            _productCompanyRepo = new ProductCompanyRepository();
            _productgroupRepo = new ProductGroupRepository();
            _productScheduleRepo = new ProductScheduleRepository();
        }

        public IEnumerable<SpGetAllProduct> GetAllProductBySp(int orgId)
        {
            return _repository.GetAllProductBySp(orgId);
        }


       public void SaveWebOrderable(ProductModel model)
        {
            try
            {

                var product = _repository.SelectById(model.Row_Id);
                if (product != null)
                {
                  ProductModel prodObject = new ProductModel();

                  if (model.WebOrderable == true)
                  {
                   
                    if (!string.IsNullOrEmpty(model.WebDescription))
                      product.WebDescription = model.WebDescription;
                    if (!string.IsNullOrEmpty(model.WebName))
                      product.WebName = model.WebName;
                    if (!string.IsNullOrEmpty(model.WebOptionMax))
                      product.WebOptionMax = int.Parse(model.WebOptionMax);
                    if (!string.IsNullOrEmpty(model.WebOptionMin))
                      product.WebOptionMin = int.Parse(model.WebOptionMin);
                    if (!string.IsNullOrEmpty(model.WebOptions))
                    {
                        product.WebOptions = model.WebOptions.Replace(",", ";");
                    }
                       
                    if (!string.IsNullOrEmpty(model.WebType))
                      product.WebType = model.WebType;
                   // if (!string.IsNullOrEmpty(model.w))
                    product.ProductSubGroupId = model.ProductSubGroupId;
                  }
                  else if (model.WebOrderable == false)
                  {
                      product.WebDescription = null;
                      product.WebName = null;
                      product.WebOptionMax = null;
                      product.WebOptionMin = null;
                      product.WebOptions = null;
                      product.WebType = null;
                      product.ProductSubGroupId =0;
                  }

                  product.WebOrderable = model.WebOrderable;

                  _repository.Update(product);
                }

            }
            catch (Exception ex)
            {


            }

         
        }





        public IList<SelectProductGroupOptions> GetProductGroupOptionBySp(int productid)
        {
            try
            {
                var currentUser = UserManager.Current();
                if (currentUser != null)
                {

                    int orgid = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    string userType = currentUser.UserType.ToString();
                    string userTypeName = currentUser.UserType1.Name;
                    int? compamyid = null;
                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    return _productgroupRepo.GetProductGroupOptionBySp(orgid, userid, userType, compamyid, productid);
                }


            }
            catch (Exception ex)
            {


            }

            return null;
        }

        public IList<ProductModel> GetAll()
        {
            try
            {
                var productlist = _repository.SelectAll().ToList();
                var productgroup = _productgroupRepo.GetAllProductgroup().ToList();
                IList<ProductModel> productlstModel = new List<ProductModel>();
                var prodmodellst = Mapper.Map<IList<Product>, List<ProductModel>>(productlist);
                foreach (var item in prodmodellst)
                {
                    // var grpItem = productgroup.FirstOrDefault(m => m.ProductId == item.ProductGroupId);
                    var grpItem = productgroup.FirstOrDefault(m => m.Row_Id == item.ProductGroupId);
                    if (grpItem != null)
                    {
                        item.GroupType = grpItem.Name;
                        productlstModel.Add(item);
                    }

                }
                return productlstModel;
            }
            catch (Exception ex)
            {
                var error = ex.Message;

            }

            return null;
        }
        public CompanyProductModel GetProductCompanyList(string id)
        {
            var conpanyProduct = new CompanyProductModel();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    conpanyProduct = ProductCompanyDetailList(id);
                }

            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return conpanyProduct;
        }

        public CompanyProductModel Update(ProductModel model)
        {
            var compProdObj = new CompanyProductModel();
            try
            {

                if (model != null)
                {
                    //  var productgroup = _productgroupRepo.GetAllProductgroup().FirstOrDefault(m => m.Row_Id == int.Parse(model.Group));

                    var productgroup = _productgroupRepo.GetProductGroupById(int.Parse(model.Group));

                    //  using (var transaction = new TransactionScope())
                    // {
                    var prodObject = _repository.SelectById(model.Row_Id);
                    if (prodObject != null)
                    {
                        prodObject.SalesUnitPrice = model.SalesUnitPrice;
                        prodObject.XeroItemDescription = model.XeroItemDescription;
                        prodObject.ProductDescription = model.ProductDescription;
                        if (productgroup != null)
                            prodObject.ProductGroupId = productgroup.Row_Id;
                        _repository.Update(prodObject);
                    }
                    //if (!string.IsNullOrEmpty(model.CompanyIds))
                    //{
                    //    var productcompanies = model.CompanyIds;
                    //    var productcompaniesId = productcompanies.Split(',');
                    //    DeleteCompamiesByProduct(model.Row_Id);
                    //    AddCompamiesByProduct(model.Row_Id, productcompaniesId);
                    //}

                    //if (!string.IsNullOrEmpty(model.Group))
                    //{
                    //    AddorUpdateProductGroup(model.Row_Id, model.Group);
                    //}
                    //if (model.ProductSchedule != null)
                    //{
                    //    AddorUpdateProductSchedule(model.ProductSchedule, model.Row_Id);

                    //}
                    //   transaction.Complete();
                    //  }

                    compProdObj = ProductCompanyDetailList(model.Row_Id.ToString());
                }




            }
            catch (Exception ex)
            {

                string error = ex.Message;
            }

            return compProdObj;
        }

        public int EnableProductforCompany(string prodid, string companyid, bool ischecked)
        {
            try
            {
                if (!string.IsNullOrEmpty(prodid) && !string.IsNullOrEmpty(companyid))
                {
                    var prodcomp = _productCompanyRepo.GetAllProductCompanylist()
                          .Where(p => p.ProductId == int.Parse(prodid)).SingleOrDefault(c => c.CompanyId == int.Parse(companyid));
                    if (prodcomp != null)
                    {
                        if (!ischecked)
                        {
                            // Delete
                            return _productCompanyRepo.Delete(prodcomp);
                        }
                    }
                    else
                    {
                        if (ischecked)
                        {
                            var obj = new ProductCompany();
                            obj.CompanyId = int.Parse(companyid);
                            obj.ProductId = int.Parse(prodid);
                            // obj.Enable = ischecked;
                            return _productCompanyRepo.Add(obj);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }
            return 0;
        }


        public List<ProductModel> GetAllProductsBygroup(string groupname)
        {
            var productlstModel = new List<ProductModel>();
            try
            {
                if (!string.IsNullOrEmpty(groupname))
                {
                    var productgroup = _productgroupRepo.GetProductGroupById(int.Parse(groupname));
                    if (productgroup != null)
                    {
                        var prodlst = _repository.SelectAll().Where(p => p.ProductGroupId == productgroup.Row_Id).ToList();
                        var prodmodellst = Mapper.Map<IList<Product>, List<ProductModel>>(prodlst);
                        foreach (var item in prodmodellst)
                        {
                            item.GroupType = productgroup.Name;
                            productlstModel.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                string message = ex.Message;
            }

            return productlstModel;
        }




        public IList<ProductSchedulesModel> GetAllProductScheduleByProdId(int prodid)
        {
            IList<ProductSchedulesModel> schedulesModelslst = new List<ProductSchedulesModel>();
            List<ProductSchedule> schedulelst = new List<ProductSchedule>();

            
            try
            {

                var currentUsr = UserManager.Current();
                if (currentUsr != null)
                {
                    if (prodid != 0)
                    {
                        var prdgrp = _repository.GetProductgroupBySp(currentUsr.OrgId, null).ToList();
                        var product = _repository.SelectById(prodid);
                        var productSubGroup = _repository.GetProductgroupBySp(currentUsr.OrgId, product.ProductGroupId).ToList();
                        if (product != null)
                        {
                            var schedulelist = _productScheduleRepo.GetAllProductSchedule()
                                    .Where(ps => ps.XeroCode == product.XeroCode).ToList();

                             var pg= prdgrp.Where(m => m.Row_Id == product.ProductGroupId).FirstOrDefault();

                              var ckeckGroup= productSubGroup.Any(m => m.Row_Id == product.ProductGroupId);
                              if (!ckeckGroup)
                                 {
                                      productSubGroup.Add(pg);
                                 }

                              /* below code is commented becoz some data from  ProductSchedules is not displaying due to ProductGroupId is not matched //
                               exec [dbo].[GetProductCategory] @Org_Id=825,@Parent_Id=7
                               Select * from ProductSchedules where XEROCODE = 'MWPACK5'
                              */
                             /*
                                        //if (schedulelist.Count >0)
                                        //{

                                        //    foreach (var subgrp in productSubGroup)
                                        //    {
                                        //      var prosch = schedulelist.Where(m => m.ProductGroupId == subgrp.Row_Id).ToList();
                                        //      if (prosch!= null)
                                        //        {
                                        //            schedulelst.AddRange(prosch);
                                        //        }
                                        //    }
                                        //   // schedulelist.Where(m=>m.ProductGroupId==);
                                        //}
                                        // schedulesModelslst = Mapper.Map<IEnumerable<ProductSchedule>, List<ProductSchedulesModel>>(schedulelst);
                             
                            */
                              if (schedulelist != null && schedulelist.Count > 0)
                              {
                                  schedulesModelslst = Mapper.Map<IEnumerable<ProductSchedule>, List<ProductSchedulesModel>>(schedulelist);
                              }

                           // schedulesModelslst =  Mapper.Map<IEnumerable<ProductSchedule>, List<ProductSchedulesModel>>(schedulelist);

                            foreach (var itemSchedule in schedulesModelslst)
                            {
                                var pgrpsub = productSubGroup.FirstOrDefault(m => m.Row_Id == itemSchedule.ProductGroupId);
                                if (pgrpsub != null)
                                {
                                    itemSchedule.ProductGroup = pgrpsub.Name;
                                }
                               // var pgrp = productSubGroup.FirstOrDefault(m => m.Row_Id == product.ProductGroupId);

                                //var pgrp = prdgrp.FirstOrDefault(m => m.Row_Id == product.ProductGroupId);
                                //if (pgrp != null)
                                //{
                                //    itemSchedule.ProductGroup = pgrp.Name;
                                //}

                            }



                        }
                    }
                }



            }
            catch (Exception ex)
            {


            }

            return schedulesModelslst;

        }


        public IList<ProductCategories> GetProductgroupBySp(int? orgId, int? parentId)
        {
            return _repository.GetProductgroupBySp(orgId, parentId);
        }

        public Product GetProductById(int id)
        {
            return _repository.SelectById(id);
        }
        //..................Helper Mehods........................


        private CompanyProductModel EditCompanyProduct(int id, int orgId)
        {

            var companyProduct = new CompanyProductModel();
            try
            {

                var product = _repository.SelectById(id);
                var productModel = Mapper.Map<Product, ProductModel>(product);
                // .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));
                productModel.ProductGroupslist = _repository.GetProductgroupBySp(orgId, null).ToList();
                productModel.ProductSubGrouplist = _repository.GetProductgroupBySp(orgId, product.ProductGroupId).ToList();
              //  _productService.GetProductgroupBySp(currentuser.OrgId.Value, int.Parse(id)).ToList();
                //  var group = _productgroupRepo.GetAllProductgroup().FirstOrDefault(pg => pg.ProductId == product.ProductGroupId);

                if (product != null && product.ProductGroupId != null)
                {
                    var group = _productgroupRepo.GetProductGroupById(product.ProductGroupId.Value);


                    if (group != null)
                    {
                        // productModel.Group = group.ProductId.ToString();
                        productModel.Group = group.Row_Id.ToString();
                    }
                }
                companyProduct.ProductModel = productModel;


            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return companyProduct;

        }

        private int DeleteCompamiesByProduct(int prodid)
        {
            int status = 0;
            if (prodid != null && prodid != 0)
            {
                var prodCompList = _productCompanyRepo.GetAllProductCompanylist().Where(pid => pid.ProductId == prodid).ToList();


                foreach (var deleteItem in prodCompList)
                {
                    if (deleteItem.ProductId != null)
                        _productCompanyRepo.Delete(deleteItem);
                }
            }
            return status;
        }

        private bool AddCompamiesByProduct(int prodId, string[] companyIds)
        {
            bool status = false;

            foreach (var cid in companyIds)
            {
                if (!string.IsNullOrEmpty(cid))
                {
                    var objProductCompany = new ProductCompany();
                    objProductCompany.ProductId = prodId;
                    objProductCompany.CompanyId = int.Parse(cid);
                    _productCompanyRepo.Add(objProductCompany);
                    status = true;

                }

            }
            return status;
        }

        private bool AddorUpdateProductGroup(int pid, string grpName)
        {
            bool status = false;
            if (!string.IsNullOrEmpty(grpName) && pid != 0)
            {
                var currentuser = UserManager.Current();

                //   var prodgroup = _productgroupRepo.GetProductGroupByProductId(pid);
                var prodgroup =
                    _productgroupRepo.GetAllProductgroup()
                        .Where((p => p.ProductId == pid && p.CreatedBy == currentuser.Row_Id))
                        .SingleOrDefault();
                //GetAllProductgroup
                if (prodgroup != null)
                {
                    prodgroup.Name = grpName.Trim();
                    prodgroup.UpdatedBy = currentuser.Row_Id;
                    prodgroup.Updated = DateTime.Now;
                    _productgroupRepo.Update(prodgroup);
                    status = true;
                }
                else
                {
                    var pg = new ProductGroup
                    {
                        ProductId = pid,
                        Name = grpName.Trim(),
                        Created = DateTime.Now,
                        CreatedBy = currentuser.Row_Id
                    };
                    _productgroupRepo.Add(pg);
                    status = true;
                }

            }
            return status;
        }




        private CompanyProductModel ProductCompanyDetailList(string id)
        {
            var conpanyProduct = new CompanyProductModel();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var currentuser = UserManager.Current();

                    if (currentuser != null && currentuser.OrgId != null)
                    {
                        conpanyProduct = EditCompanyProduct(int.Parse(id), currentuser.OrgId.Value);
                        var companylist = _companyRepository.SelectAll().Where(c => c.Org_Id == currentuser.OrgId && c.Active == true).ToList();
                        var compmodel = Mapper.Map<IEnumerable<Company>, List<CompanyModel>>(companylist);
                        var productcopmlist = _productCompanyRepo.GetAllProductCompanylist().Where(p => p.ProductId == int.Parse(id)).ToList();

                        if (productcopmlist.Count > 0 && companylist.Count > 0)
                        {
                            var prodlist =
                                productcopmlist.Where(
                                    x => companylist.Select(y => y.Row_Id.ToString()).Contains(x.CompanyId.ToString()))
                                    .ToList();

                            if (prodlist.Count > 0)
                            {
                                var finallist = CheckforCompanyEnable(prodlist, compmodel);
                                conpanyProduct.ProductCompanylist = finallist;

                            }
                            else
                            {
                                conpanyProduct.ProductCompanylist = compmodel;
                            }
                        }
                        else
                        {
                            conpanyProduct.ProductCompanylist = new List<CompanyModel>();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return conpanyProduct;
        }


        private List<CompanyModel> CheckforCompanyEnable(List<ProductCompany> productCompanylst, List<CompanyModel> cmplist)
        {

            //foreach (var item in cmplist)
            //{
            //    var compprod = productCompanylst.FirstOrDefault(cmp => cmp.CompanyId == item.Row_Id);
            //    if (compprod != null)
            //        if (compprod.Enable != null) 
            //        item.Enabled = compprod.Enable.Value;
            //}
            foreach (var item in cmplist)
            {
                var compprod = productCompanylst.FirstOrDefault(cmp => cmp.CompanyId == item.Row_Id);
                if (compprod != null)
                {
                    item.Enabled = true;
                }
                else
                {
                    item.Enabled = false;
                }

            }

            return cmplist;

        }

        private IList<ProductGroup> AllProductGroup()
        {
            var prdGrp = _repository.GetProductgroupBySp(825, null).ToList();

            foreach (var itm in prdGrp)
            {

            }

            return null;
        }

    }
}