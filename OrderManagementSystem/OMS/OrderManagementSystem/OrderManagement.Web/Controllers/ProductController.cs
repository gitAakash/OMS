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
namespace OrderManagement.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductScheduleService _repositoryschedule;

        public ProductController()
        {
            var productrapo = new ProductRepository();
            _productService = new ProductService(productrapo);
            var productschedrapo = new ProductScheduleRepository();
            _repositoryschedule = new ProductScheduleService(productschedrapo);
        }

        //public ActionResult Product_filter(string id)
        //{
        //    var productmodellist = new List<SpGetAllProduct>();
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        productmodellist = _productService.GetAllProductBySp().ToList();
        //    }
        //    else if (!string.IsNullOrEmpty(id))
        //    {
        //        //productmodellist = _productService.GetAllProductsBygroup(id);

        //        productmodellist = _productService.GetAllProductBySp().Where(m => m.ProductGroupId.ToString() == id).ToList();
        //    }
        //    return Json(productmodellist, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult Products_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    var currentUser = UserManager.Current();
        //    if (currentUser != null)
        //    {
        //        var products = _productService.GetAllProductBySp(currentUser.OrgId.Value).ToList();

        //        if (request.Filters.Count > 0)
        //        {
        //            if (request.Filters.Any())
        //            {
        //                foreach (var filter in request.Filters)
        //                {
        //                    var descriptor = filter as FilterDescriptor;
        //                    if (descriptor != null && descriptor.Member == "ProductGroupId")
        //                    {
        //                        var prodlist = products.Where(m => m.ProductGroupId == int.Parse(descriptor.Value.ToString())).ToList();
        //                       // request.Filters = new List<IFilterDescriptor>();
        //                        request.Filters.Clear();
        //                        return Json(prodlist.ToDataSourceResult(request));
        //                    }
        //                }
        //            }

        //        }

        //        return Json(products.ToDataSourceResult(request));

        //    }

        //    return null;
        //}

        public ActionResult Index(string id)
        {
               var currentUser = UserManager.Current();
               if (currentUser != null)
               {
                   var products = _productService.GetAllProductBySp(currentUser.OrgId.Value).ToList();
                   return PartialView("Controls/Product/_ProductList", products);
               }
               return null;
        }

        public ActionResult ProductCompanyList(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
              var currentuser = UserManager.Current();
                var productCompanylist = _productService.GetProductCompanyList(id);

                var productGroupOption = _productService.GetProductGroupOptionBySp(int.Parse(id));

              //  productCompanylist.ProductModel.WebOptionslist = productGroupOption.ToList();

              List<string> optionlist = new List<string>();
                foreach (var item in productGroupOption)
                {
                  var options=  item.Options.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                  optionlist.AddRange(options);

                }

              productCompanylist.ProductModel.WebOptionslist = optionlist;


              if (!string.IsNullOrEmpty(productCompanylist.ProductModel.WebOptions))
              {
                productCompanylist.ProductModel.WebOptionValue =
                  productCompanylist.ProductModel.WebOptions.Split(new char[] {';'},
                    StringSplitOptions.RemoveEmptyEntries);
              }
              else
              {
                productCompanylist.ProductModel.WebOptionValue  = optionlist.ToArray();
              }
              
              productCompanylist.ProductModel.ProductSubGroupId = productCompanylist.ProductModel.ProductSubGroupId;

              //  productCompanylist.ProductModel.ProductSubGrouplist = _productService.GetProductgroupBySp(currentuser.OrgId.Value, int.Parse(id)).ToList();

                // The total values and summary values are displayed in two partial views
                // We can't normally return two partial views from an action, but we don't want to have another server
                // call to get the second one, so we render the two partial views into HTML strings and package them into an
                // an anonymous object, which we then serialize into a JSON object for sending to the client
                // the client side script will then load these two partial views into the relevant page elements


                var productCompanyPartialView = RenderRazorViewToString(this.ControllerContext, "Controls/Product/_ProductCompany", productCompanylist);

                var productWebOrderPartialView = RenderRazorViewToString(this.ControllerContext, "Controls/Product/_WebOrderable", productCompanylist.ProductModel);

                var json = Json(new { productCompanyPartialView, productWebOrderPartialView });

                return Json(json, JsonRequestBehavior.AllowGet);
            }

          return null;
        }

        public static String RenderRazorViewToString(ControllerContext controllerContext, String viewName, Object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        public ActionResult AddorUpdate(ProductModel model)
        {
            var productCompanymodel = _productService.Update(model);
            // var  productmodellist = _productService.GetAll().ToList();
            // return PartialView("Controls/Product/_ProductList", productmodellist);
            // var products = _productService.GetAllProductBySp().ToList();
            return PartialView("Controls/Product/_ProductList");
        }

        public ActionResult SaveWebOrder(ProductModel model)
        {
            _productService.SaveWebOrderable(model);
            var prodModel = GetProduct(model.Row_Id.ToString());
           return PartialView("Controls/Product/_WebOrderable", prodModel);
        }

        //public ActionResult SaveNoWebOrder(ProductModel model)
        //{
        //    _productService.SaveWebOrderable(model);
        //    var prodModel = GetProduct(model.Row_Id.ToString());
        //    return PartialView("Controls/Product/_WebOrderable", prodModel);
        //}

        public ActionResult EnableProductCompany(string id, string companyId, string ischecked)
        {
            int status = _productService.EnableProductforCompany(id, companyId, bool.Parse(ischecked));
            return Content(status.ToString());
        }

        public ActionResult ProductGroup(string id)
        {
            var productgrplist = _productService.GetAllProductScheduleByProdId(int.Parse(id));
            return PartialView("Controls/Product/_ProductGroup", productgrplist);

        }

        public ActionResult NewProductSchedule(string id, string prodid)
        {
            var currentuser = UserManager.Current();
            var schmodel = new ProductSchedulesModel();
            if (!string.IsNullOrEmpty(id))
            {
                schmodel = _repositoryschedule.GetById(int.Parse(id));
            }
            schmodel.Colorlist = _repositoryschedule.GetAllColors();
            if (currentuser != null)
            {
                if (!string.IsNullOrEmpty(prodid))
                {
                    var product = _productService.GetProductById(int.Parse(prodid));
                    schmodel.ProductGroupslist = _productService.GetProductgroupBySp(currentuser.OrgId.Value, product.ProductGroupId).ToList();
                }

            }
            return PartialView("Controls/Product/_ProductSchedule", schmodel);
        }

        public ActionResult AddOrUpdateProductSchedule(ProductSchedulesModel model)
        {
            int status = _repositoryschedule.AddOrUpdate(model);
            if (status == 1)
            {
                var productgrplist = _productService.GetAllProductScheduleByProdId(int.Parse(model.Productid));
                return PartialView("Controls/Product/_ProductGroup", productgrplist);
            }
            return null;
        }

        public ActionResult DeleteProductSchedule(string id, string prodid)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(prodid))
            {
                _repositoryschedule.Delete(int.Parse(id));
                var productgrplist = _productService.GetAllProductScheduleByProdId(int.Parse(prodid));
                return PartialView("Controls/Product/_ProductGroup", productgrplist);
            }

            return View();
        }

        public ActionResult CancelWebOrder(string prodid)
        {
            var prodModel = GetProduct(prodid);
            return PartialView("Controls/Product/_WebOrderable", prodModel);

        }


        private ProductModel GetProduct(string prodid)
        {
            try
            {
                var productCompanylist = _productService.GetProductCompanyList(prodid);
                var productGroupOption = _productService.GetProductGroupOptionBySp(int.Parse(prodid));
                productCompanylist.ProductModel.ProductSubGroupId = productCompanylist.ProductModel.ProductSubGroupId;
                List<string> optionlist = new List<string>();
                foreach (var item in productGroupOption)
                {
                    var options = item.Options.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    optionlist.AddRange(options);
                }

                productCompanylist.ProductModel.WebOptionslist = optionlist;

                if (!string.IsNullOrEmpty(productCompanylist.ProductModel.WebOptions))
                {
                    productCompanylist.ProductModel.WebOptionValue = productCompanylist.ProductModel.WebOptions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    productCompanylist.ProductModel.WebOptionValue = optionlist.ToArray();
                }
                return productCompanylist.ProductModel;

            }
            catch (Exception ex)
            {


            }

            return null;

        }


    }
}
