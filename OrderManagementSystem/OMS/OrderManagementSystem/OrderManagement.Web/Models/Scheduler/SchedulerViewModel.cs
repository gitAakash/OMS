﻿using OrderManagement.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TelerikMvcApp1.Models;

namespace TelerikMvcSchedulerPOC2.Models
{
    public class SchedulerViewModel
    {
        //public SchedulerViewModel()
        //{
        //    this.CalenderResources = new List<ScheduleResources>();
        //}
        public IList<Order> NewJobs { get; set; }
        public IList<ScheduleResources> CalenderResources { get; set; }
        public IList<ScheduleResources> AllCalenderResources { get; set; }
        public IEnumerable<CalEventViewModel> UnScheduledJobs { get; set; }
        public IList<ScheduleResources> AllUsers { get; set; }
        public IList<UserProductCalendar> UserProductCalendar { get; set; }
        

    }

    public class ScheduleResources
    {
        public string UserId { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
    }

    public class UserProductCalendar
    {
        public string UserId { get; set; }
        public string UserProductGroup { get; set; }
        public string CalendarId { get; set; }
        public string UserName { get; set; }

    }

    public class EventColor
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
    }

    public class ProductGroups
    {
        public int Row_Id { get; set; }
        public string Name { get; set; }
    }

    public class SchedulerEmailModel
    {
        public string MailTo { get; set; }
        public string MailCC { get; set; }
        public string MailBCC { get; set; }
        public string MailSubject { get; set; }
        [AllowHtml]
        public string MailBody { get; set; }
        public string MailFrom { get; set; }
        public string MailFromDisplay { get; set; }
    }



}