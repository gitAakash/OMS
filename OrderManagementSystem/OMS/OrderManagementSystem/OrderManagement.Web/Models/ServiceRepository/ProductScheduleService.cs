using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using AutoMapper;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models.Repository;

namespace OrderManagement.Web.Models.ServiceRepository
{
    public interface IProductScheduleService
    {
        int AddOrUpdate(ProductSchedulesModel model);
        IList<ColorMaster> GetAllColors();
        ProductSchedulesModel GetById(int id);
        int Delete(int id);

    }

    public class ProductScheduleService : IProductScheduleService
    {

        private IProductScheduleRepository _repository;
        private IProductRepository _productRepository;
        public ProductScheduleService(IProductScheduleRepository repository)
        {
            _repository = repository;
            _productRepository = new ProductRepository();

        }



        public int AddOrUpdate(ProductSchedulesModel model)
        {
            try
            {
                if (model != null)
                {
                    string strSubject = System.Configuration.ConfigurationManager.AppSettings["ProductSchedule"];
                    string strBody = "<p>Product has been scheduled.</p>";
                    var currentusr = UserManager.Current();
                    if (currentusr != null)
                    {
                        var product = _productRepository.SelectById(int.Parse(model.Productid));
                        if (product != null)
                        {
                            var objSchedule = new ProductSchedule();

                            if (model.Row_Id != 0 && model.Row_Id != null)
                            {
                                objSchedule = _repository.GetById(model.Row_Id);
                            }
                            var color = _repository.GetAllColors().SingleOrDefault(col => col.Color == model.ColorCode);
                            objSchedule.CreateEvent = model.CreateEvent;
                            objSchedule.Title = model.Title;
                            objSchedule.Value = model.Value;

                            objSchedule.WebOptionMax = model.WebOptionMax;


                            if (color!=null)
                              objSchedule.ColorId = color.Row_Id;

                            objSchedule.SendEmail = model.SendEmail;
                            objSchedule.EmailAddress = model.EmailAddress;

                            if (!string.IsNullOrEmpty(model.EmailAddress))
                            {

                                objSchedule.ProductGroupId = product.ProductGroupId;
                            }
                            else
                                objSchedule.ProductGroupId = model.ProductGroupId; 



                            if (!string.IsNullOrEmpty(product.XeroCode))
                            {
                                objSchedule.XeroCode = product.XeroCode.Trim();
                            }
                           


                            if (model.Row_Id == 0 || model.Row_Id == null)
                            {
                                objSchedule.Created = DateTime.Now;
                                objSchedule.CreatedBy = currentusr.Row_Id;
                              
                              
                              var status =  _repository.Add(objSchedule);
                              if (status == 1 && !string.IsNullOrEmpty(model.EmailAddress))
                                {
                                  //  string strBody = "You have scheduled product";

                                    bool hasEmailSent = Email.sendemail(model.EmailAddress, "", strBody, true, false, strSubject);
                                   
                                }
                              return status;
                            }
                            if (model.Row_Id != 0 && model.Row_Id != null)
                            {
                                objSchedule.Updated = DateTime.Now;
                                objSchedule.UpdatedBy = currentusr.Row_Id;
                                var status = _repository.Update(objSchedule);
                                if (status == 1 && !string.IsNullOrEmpty(model.EmailAddress))
                                {
                                  //  string strBody = "You have scheduled product";
                                    bool hasEmailSent = Email.sendemail(model.EmailAddress, "", strBody, true, false, strSubject);
                                   
                                }

                                return status;
                            }



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

        public IList<ColorMaster> GetAllColors()
        {
            return _repository.GetAllColors().OrderBy(m=>m.Row_Id).Take(11).ToList();
        }

        public ProductSchedulesModel GetById(int id)
        {
            var model = new ProductSchedulesModel();
            try
            {
                if (id != 0)
                {
                    var prodSchedule = _repository.GetById(id);
                    if (prodSchedule != null)
                    {
                        model = Mapper.Map<ProductSchedule, ProductSchedulesModel>(prodSchedule);
                        var color = _repository.GetAllColors().SingleOrDefault(c => c.Row_Id == model.ColorId).Color;
                        model.ColorCode = color;
                    }
                }

            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return model;
        }

        public int Delete(int id)
        {

            try
            {
                if (id != 0)
                {
                    return _repository.Delete(id);
                }

            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return 0;
        }
    }
}