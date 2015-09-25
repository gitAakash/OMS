using System.Collections;
using System.Web.Caching;
using AutoMapper;
using Kendo.Mvc.Extensions;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderManagement.Web.Controllers
{
    public class CompanyController : Controller
    {

        private readonly ICompanyRepository _repository;
        private readonly IProductRepository _productRepository;
        private IProductCompanyRepository _productCompanyRepo;
        public CompanyController()
        {
            this._repository = new CompanyRepository();
            _productRepository = new ProductRepository();
            _productCompanyRepo = new ProductCompanyRepository();
        }
        public CompanyController(ICompanyRepository repository, IProductRepository productRepository)
        {
            this._repository = repository;
            _productRepository = productRepository;
        }
        ////
        //// GET: /Company/
        public ActionResult Company()
        {
            return PartialView("../Company/_Company");

        }
        public ActionResult Index()
        {
            //  var currentUser = UserManager.Current();
            var companylstModel = new List<CompanyModel>();
            var companylist = _repository.SelectAll().OrderByDescending(c => c.Created);
            companylstModel = Mapper.Map<IEnumerable<Company>, List<CompanyModel>>(companylist);

            return PartialView("Controls/Company/_CompanyList", companylstModel);
        }

        public ActionResult Companylist()
        {
           
            return View();
        }

        public ActionResult CompanyContacts(string id)
        {
            var contactlist = new List<Contact>();
            var companylstModel = new List<ContactModel>();

            var ordercontacts = _repository.GetOrderContactsbyOrder(int.Parse(id));

            if (ordercontacts.Count > 0)
            {
                foreach (var item in ordercontacts)
                {
                    if (item != null)
                    {
                        if (item.ContactId != null)
                        {
                            var contact = _repository.GetContactsById(item.ContactId.Value);
                            if (contact != null)
                            {
                                contactlist.Add(contact);
                            }

                        }
                    }

                }
                companylstModel = Mapper.Map<IEnumerable<Contact>, List<ContactModel>>(contactlist);
            }

            return PartialView("Controls/Company/_CompanyContacts", companylstModel);

        }

        public ActionResult CompanyDetails(string id)
        {
            //var objModel = new CompanyModel(); 
            var company = _repository.GetById(id);
            var compModel = Mapper.Map<Company, CompanyModel>(company);
            var allOrders = _repository.GetAllOrders().ToList();
            compModel.ProperyOrderModel = CompanyOrderHistory(company.Properties.ToList(), allOrders);
            return PartialView("Controls/Company/_CompanyDetails", compModel);

        }

        [HttpPost]
        public ActionResult AddOrUpdate(CompanyModel model)
        {

            var compObj = Mapper.Map<CompanyModel, Company>(model);
            _repository.Update(compObj);
            _repository.Save();
            return null;
        }

        public ActionResult EditCompanyContacts(string id)
        {
            var contact = _repository.GetContactsById(int.Parse(id));
            var contactModel = Mapper.Map<Contact, ContactModel>(contact);
            return PartialView("Controls/Company/_EditContacts", contactModel);
        }
        public ActionResult AddNewContact(string orderid, string cid)
        {
            var contactModel = new ContactModel();
            if (!string.IsNullOrEmpty(orderid) && !string.IsNullOrEmpty(cid))
            {
                contactModel.CompanyId = int.Parse(cid);
                contactModel.Orderid = int.Parse(orderid);
            }

            return PartialView("Controls/Company/_EditContacts", contactModel);
        }

        public ActionResult AddOrUpdateContacts(ContactModel model)
        {
            var contactObj = Mapper.Map<ContactModel, Contact>(model);
            var status = 0;
            if (model.Row_Id != null && model.Row_Id != 0)
                status = _repository.UpdateContacts(contactObj);
            else
            {
                contactObj.Created = DateTime.Now;
                var contactid = _repository.AddContacts(contactObj);
                if (contactid != 0)
                {
                    var ordercontact = new OrderContact();
                    ordercontact.OrderId = model.Orderid;
                    ordercontact.ContactId = contactid;
                    ordercontact.Created = DateTime.Now;
                    status = _repository.SaveOrdercontact(ordercontact);
                }

            }


            if (status == 1)
            {
                var contactlist = new List<Contact>();
                var companylstModel = new List<ContactModel>();

                var ordercontacts = _repository.GetOrderContactsbyOrder(model.Orderid);

                if (ordercontacts.Count > 0)
                {
                    foreach (var item in ordercontacts)
                    {
                        if (item != null)
                        {
                            if (item.ContactId != null)
                            {
                                var contact = _repository.GetContactsById(item.ContactId.Value);
                                if (contact != null)
                                {
                                    contactlist.Add(contact);
                                }

                            }
                        }

                    }

                    companylstModel = Mapper.Map<IEnumerable<Contact>, List<ContactModel>>(contactlist);

                }

                return PartialView("Controls/Company/_CompanyContacts", companylstModel);
            }
            return null;
        }


        public ActionResult DeleteCompanyContacts(string id, string orderid)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(orderid))
            {
                var deleted = _repository.DeleteContact(int.Parse(id));
                if (deleted == 1)
                {
                    _repository.DeleteOrderContact(int.Parse(id));

                    var contactlist = new List<Contact>();
                    var companylstModel = new List<ContactModel>();

                    var ordercontacts = _repository.GetOrderContactsbyOrder(int.Parse(orderid));

                    if (ordercontacts.Count > 0)
                    {
                        foreach (var item in ordercontacts)
                        {
                            if (item != null)
                            {
                                if (item.ContactId != null)
                                {
                                    var contact = _repository.GetContactsById(item.ContactId.Value);
                                    if (contact != null)
                                    {
                                        contactlist.Add(contact);
                                    }
                                }
                            }

                        }

                        companylstModel = Mapper.Map<IEnumerable<Contact>, List<ContactModel>>(contactlist);

                    }



                    return PartialView("Controls/Company/_CompanyContacts", companylstModel);


                }



                //var companyContactlist = _repository.GetContactsByCompId(int.Parse(cid));
                //var companylstModel = Mapper.Map<IEnumerable<Contact>, List<ContactModel>>(companyContactlist);
                //return PartialView("Controls/Company/_CompanyContacts", companylstModel);
            }
            return null;

        }


        public ActionResult CompanyproductsbyOder(string id, string orderid)
        {
            IList<Product> prodlst = new List<Product>();
            List<ProductModel> productlstModel = new List<ProductModel>();
            try
            {
                if (string.IsNullOrEmpty(orderid) && !string.IsNullOrEmpty(id))
                {
                    var compProduct =
                             _productCompanyRepo.GetAllProductCompanylist().Where(pc => pc.CompanyId == int.Parse(id)).ToList();


                    if (compProduct.Count > 0)
                    {
                        foreach (var prod in compProduct)
                        {
                            var prodObj = _productRepository.SelectById(int.Parse(prod.ProductId.ToString()));
                            var prodmodel = Mapper.Map<Product, ProductModel>(prodObj);
                            productlstModel.Add(prodmodel);
                        }
                    }
                    //  var productList = _productRepository.SelectAll().Where(p => compProductIds.Contains(p.Row_Id)).ToList();



                    // productlstModel = Mapper.Map<IEnumerable<Product>, List<ProductModel>>(productList);

                    //  compModel.Product = productlstModel;

                }
                else
                {
                    if (!string.IsNullOrEmpty(orderid) && string.IsNullOrEmpty(id))
                    {
                        var orderitems = _repository.GetAllOrderItemsByOrder(int.Parse(orderid)).ToList();


                        foreach (var item in orderitems)
                        {
                            if (item.Xero_Id != null)
                            {
                                var prod = _productRepository.SelectById(item.Xero_Id.Value);
                                if (prod != null)
                                {
                                    prodlst.Add(prod);

                                }
                            }
                        }

                        productlstModel = Mapper.Map<IEnumerable<Product>, List<ProductModel>>(prodlst);
                    }

                }


            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }

            return PartialView("Controls/Company/_CompanyProductList", productlstModel);
        }

        public static List<PropertyOrderModel> CompanyOrderHistory(List<Property> property, List<Order> orderList)
        {

            var orderhistorylist = new List<PropertyOrderModel>();
            try
            {
                foreach (var item in property)
                {
                    var lst = orderList.Where(o => o.Property_Id == item.Row_Id).ToList();

                    if (lst.Count > 0)
                    {
                        foreach (var orderItem in lst)
                        {
                            var objProperty = new PropertyOrderModel();
                            objProperty.PropertyName = item.Name;
                            objProperty.PropertyId = item.Row_Id.ToString();
                            objProperty.OrderRowId = orderItem.Row_Id.ToString();
                            if (orderItem.RequiredDate.HasValue)
                            {
                                objProperty.RequiredDate = orderItem.RequiredDate.Value.ToString("MM/dd/yyyy");
                            }
                            objProperty.Status = orderItem.Status;
                            objProperty.Value = OrderValue(orderItem.OrderItems.ToList());

                            orderhistorylist.Add(objProperty);
                        }


                    }

                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }
            return orderhistorylist;
        }

        public static string OrderValue(List<OrderItem> companyorderlist)
        {
            // var clientPrice = 0.00;
            IList<Decimal> clientPrice = new List<decimal>();
            string totalClientCost = string.Empty;
            try
            {
                if (companyorderlist.Count > 0)
                {

                    //totalClientCost = companyorderlist.Sum(s => s.ClientPrice != null  ? decimal.Parse(s.ClientPrice) : 0).ToString();

                    foreach (var item in companyorderlist)
                    {

                        //  string text1 = "x";
                        decimal num1;
                        bool res = decimal.TryParse(item.ClientPrice, out num1);
                        if (res)
                        {
                            clientPrice.Add(decimal.Parse(item.ClientPrice));
                            // String is not a number.
                        }
                    }
                    totalClientCost = clientPrice.Sum(s => s).ToString();
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }


            return totalClientCost;
        }
    }
}

