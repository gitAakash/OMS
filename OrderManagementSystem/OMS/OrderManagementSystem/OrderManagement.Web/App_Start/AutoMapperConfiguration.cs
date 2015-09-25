using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using OrderManagement.Web.Models;

namespace OrderManagement.Web.App_Start
{
    public static class AutoMapperConfiguration
    {

        public static void MapViewModelwithDomainClass()
        {
            Mapper.CreateMap<Company, CompanyModel>();
            Mapper.CreateMap<CompanyModel,Company>();
            Mapper.CreateMap<User, UserModel>();
            Mapper.CreateMap<UserModel, User>();
            Mapper.CreateMap<OrderStatus, OrderStatusModel>();
            Mapper.CreateMap<OrderStatusModel, OrderStatus>();
            Mapper.CreateMap<Product, ProductModel>();
            Mapper.CreateMap<ProductModel, Product>();
            Mapper.CreateMap<Contact, ContactModel>();
            Mapper.CreateMap<ContactModel, Contact>();
            Mapper.CreateMap<ProductSchedule, ProductSchedulesModel>();

            Mapper.CreateMap<OrderViewModel, Order>();
            Mapper.CreateMap<Order, OrderViewModel>();

            Mapper.CreateMap<OrderTrackingModel, Order>();
            Mapper.CreateMap<Order, OrderTrackingModel>();

            Mapper.CreateMap<OrderItemsModel, OrderItem>();
             Mapper.CreateMap<OrderItem, OrderItemsModel>();

            Mapper.CreateMap<OrderItem, OrderViewModel>();
            Mapper.CreateMap<OrderViewModel, OrderItem>();


            Mapper.CreateMap<OrderAttachment, OrderStatusModel1>();
            Mapper.CreateMap<OrderStatusModel1, OrderAttachment>();
        
        }


    }
}