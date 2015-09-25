using Kendo.Mvc.UI;
using OrderManagement.Web;
using OrderManagement.Web.Helper.Utilitties;
using SchedulerCustomEditor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TelerikMvcSchedulerPOC2.Models;

namespace TelerikMvcApp1.Models
{
    //public class EventViewModel : ISchedulerEvent
    //{
     
    //    public EventViewModel()
    //    {
    //        this.CalenderResources = OrganisationUsers();
    //    }

    //    public int Row_Id { get; set; }
    //    public Nullable<int> CalendarId { get; set; }
    //     public string EventId { get; set; }
    //    public string ColorId { get; set; }
    //    //public string Attendees { get; set; }
    //    public string GoogleEventid { get; set; }
    //    //public int RowId { get; set; }
    //    [Required]
    //    public string Title { get; set; }
    //    public string Location { get; set; }
    //    public string Description { get; set; }
    //    public Nullable<int> UserId { get; set; }

    //    public string UnScheduleToolTip { get; set; }

    //   [Display(Name="Unscheduled Jobs")]
    //    public string UnscheduledJobs { get; set; }
    //    public Nullable<System.DateTime> MovedOn { get; set; }
    //    public string Status { get; set; }
    //    public string Organizer { get; set; }
    //    public string Creator { get; set; }
    //    public Nullable<System.DateTime> Updated { get; set; }
    //    public string Visibility { get; set; }
    //    public IList<ScheduleResources> CalenderResources { get; set; }
    //    public IList<ScheduleResources> CalenderGroupResources { get; set; }
    //    //[Required]
    //    public int? CalenderUser { get; set; }
    //    public int? ProductGroup { get; set; }
    //    //Additional properties which is implemented from interface
    //    private DateTime start;
    //    [Required]
    //    public DateTime Start
    //    {
    //        get
    //        {
    //            return start;
    //        }
    //        set
    //        {
    //            start = value;//.ToUniversalTime();
    //        }
    //    }
    //    private DateTime end;
    //    [Required]
    //    [DateGreaterThan(OtherField = "Start")]
    //    public DateTime End
    //    {
    //        get
    //        {
    //            return end;
    //        }
    //        set
    //        {
    //            end = value;//.ToUniversalTime();
    //        }
    //    }

    //    public bool Isdrag { get; set; }

    //    public string StartTimezone { get; set; }
    //    public string EndTimezone { get; set; }
    //    public string RecurrenceRule { get; set; }
    //    public string RecurrenceID { get; set; }
    //    public string RecurrenceException { get; set; }
    //    public bool IsAllDay { get; set; }
    //    public string Timezone { get; set; }
    //    public int Sequence { get; set; }


    //    //private string GetUserNameById(int userId)
    //    //{
    //    //    using (var OrderMangtDB = new OrderMgntEntities())
    //    //    {
    //    //        var temp = OrderMangtDB.Users.ToList().Where(m => m.Row_Id == userId).FirstOrDefault();
    //    //        if (temp != null)
    //    //        {
    //    //            return temp.FirstName + " " + temp.LastName;
    //    //        }
    //    //        else
    //    //        {
    //    //            return string.Empty;
    //    //        }
    //    //    }
    //    //}


    //    private IList<ScheduleResources> OrganisationUsers()
    //    {
    //        IList<ScheduleResources> lstresources = new List<ScheduleResources>();
    //        try
    //        {
    //            using (var OrderMangtDB = new OrderMgntEntities())
    //            {
    //                var currentuser = UserManager.Current();
    //                if (currentuser != null)
    //                {
    //                    var calenderUsers =
    //                        OrderMangtDB.Users.Where(
    //                            m =>
    //                                m.OrgId == currentuser.OrgId && (m.UserType == 1 || m.UserType == 2) &&
    //                                m.IsActive == true)
    //                            .Join(OrderMangtDB.CalendarUsers, e => e.Row_Id, d => d.UserId,
    //                                (e, d) => new { e.FirstName, e.LastName, e.UserType, d.UserId, d.Color, d.CalendarId });

    //                    if (calenderUsers != null && calenderUsers.ToList().Count > 0)
    //                    {
                     
    //                        foreach (var item in calenderUsers.ToList())
    //                        {
    //                            var scheduleResources = new ScheduleResources();
    //                            scheduleResources.Text = item.FirstName+" "+item.LastName;
    //                            scheduleResources.Value = item.CalendarId.ToString();
    //                            if (item != null && item.Color != null)
    //                            {
    //                                scheduleResources.Color = item.Color.Replace("#", "\\#");
    //                            }
    //                            else
    //                            {
    //                                scheduleResources.Color = "";
    //                            }
    //                            lstresources.Add(scheduleResources);
    //                        }
    //                    }

    //                }
    //            }


    //        }
    //        catch (Exception ex)
    //        {

    //            string msg = ex.Message;
    //        }

    //        return lstresources;
    //    }


    //}


    public class CalEventViewModel : ISchedulerEvent
    {
        //private IList<ScheduleResources> OrganisationUsers()
        //{
        //    IList<ScheduleResources> lstresources = new List<ScheduleResources>();
        //    try
        //    {
        //        using (var OrderMangtDB = new OrderMgntEntities())
        //        {
        //            var currentuser = UserManager.Current();
        //            if (currentuser != null)
        //            {
        //                var calenderUsers =
        //                    OrderMangtDB.Users.Where(
        //                        m =>
        //                            m.OrgId == currentuser.OrgId && (m.UserType == 1 || m.UserType == 2) &&
        //                            m.IsActive == true)
        //                        .Join(OrderMangtDB.CalendarUsers, e => e.Row_Id, d => d.UserId,
        //                            (e, d) => new { e.FirstName, e.LastName, e.UserType, d.UserId, d.Color, d.CalendarId });

        //                if (calenderUsers != null && calenderUsers.ToList().Count > 0)
        //                {

        //                    foreach (var item in calenderUsers.ToList())
        //                    {
        //                        var scheduleResources = new ScheduleResources();
        //                        scheduleResources.Text = item.FirstName + " " + item.LastName;
        //                        scheduleResources.Value = item.CalendarId.ToString();
        //                        if (item != null && item.Color != null)
        //                        {
        //                            scheduleResources.Color = item.Color.Replace("#", "\\#");
        //                        }
        //                        else
        //                        {
        //                            scheduleResources.Color = "";
        //                        }
        //                        lstresources.Add(scheduleResources);
        //                    }
        //                }

        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        string msg = ex.Message;
        //    }

        //    return lstresources;
        //}

        //public CalEventViewModel()
        //{
        //    this.CalenderResources = OrganisationUsers();
        //}
        public int Row_Id { get; set; }
        public Nullable<int> CalendarId { get; set; }
        public string EventId { get; set; }
        public string ColorId { get; set; }
        public string Color { get; set; }
        public int? EventColorid { get; set; }
        public int EventColorList { get; set; }
        [Required]
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public Nullable<int> UserId { get; set; }

        public string UnScheduleToolTip { get; set; }
        public Nullable<System.DateTime> MovedOn { get; set; }
        public string Status { get; set; }
        public string Organizer { get; set; }
        public string Creator { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string Visibility { get; set; }
        public IList<ScheduleResources> CalenderResources { get; set; }
        public IList<ScheduleResources> CalenderGroupResources { get; set; }
        public int? CalenderUser { get; set; }
        public int? ProductGroup { get; set; }
        //Additional properties which is implemented from interface
        private DateTime start;
        [Required]
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;//.ToUniversalTime();
            }
        }
        private DateTime end;
        [Required]
        [DateGreaterThan(OtherField = "Start")]
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;//.ToUniversalTime();
            }
        }

        public string TooltipDescription { get; set; }

        [Display(Name = "Unscheduled Jobs")]
        public string UnscheduledJobs { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public bool IsAllDay { get; set; }
        public string Timezone { get; set; }
        public int Sequence { get; set; }
        public string GoogleEventid { get; set; }
        public bool Isdrag { get; set; }
        public string DeleteRecurrenceEvent { get; set; }

    }




   
    
}