using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class UserProfileModel
    {
        // Need to remove this call after staff and client development.
        // We are user ProfileModel class indteed of this.


        //public string UserId { get; set; }
        //public string AboutMe { get; set; }
        //public string FullName { get; set; }
        public User User { get; set; }
        public IList<EventTracking> EventTracking { get; set; }
        public IList<OrderAttachment> OrderAttachment { get; set; }
        public IList<OrderStatus> OrderStatuslst { get; set; }
        public IList<OrderTrackingModel> OrderTracking { get; set; }
        public IList<Order> Order { get; set; }


    }



    public  class ProfileModel
    {
      public User User { get; set; }
      public IList<JobStatus> JobStatus { get; set; }

    }

}