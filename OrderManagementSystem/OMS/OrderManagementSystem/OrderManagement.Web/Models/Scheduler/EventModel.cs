using Kendo.Mvc.UI;
using SchedulerCustomEditor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TelerikMvcSchedulerPOC2.Models;

namespace OrderManagement.Web.Models
{
    public class EventModel: ISchedulerEvent
    {
        public EventModel()
        {
            this.CalenderResources = new List<ScheduleResources>();
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                var calenderUsers = OrderMangtDB.CalendarUsers.ToList();

                List<ScheduleResources> lstresources = new List<ScheduleResources>();

                foreach (var item in calenderUsers)
                {
                    ScheduleResources scheduleResources = new ScheduleResources();
                    scheduleResources.Text = GetUserNameById((int)item.UserId);
                    scheduleResources.Value = item.CalendarId.ToString();
                    scheduleResources.Color = item.Color;
                    lstresources.Add(scheduleResources);
                }
                this.CalenderResources = lstresources;
            }
        }

        public int Row_Id { get; set; }
        public Nullable<int> CalendarId { get; set; }
         public string EventId { get; set; }
        public string RecurringEventId { get; set; }
        public string ColorId { get; set; }
        //public string Attendees { get; set; }
       // public int Attendees { get; set; }
        public Nullable<bool> AttendeesOmitted { get; set; }
        public Nullable<bool> AnyoneCanAddSelf { get; set; }
        public Nullable<bool> Locked { get; set; }
        [Required]
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Kind { get; set; }
        public Nullable<int> UserId { get; set; }
       
        public Nullable<System.DateTime> MovedOn { get; set; }
        public string Status { get; set; }
        public string Recurrence { get; set; }
        public string Organizer { get; set; }
        public string Creator { get; set; }
        public Nullable<int> Sequence { get; set; }
        public string Transperancy { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string Visibility { get; set; }
        public IEnumerable<SelectListItem>  DropDownValues { get; set; }
        public IList<ScheduleResources> CalenderResources { get; set; }
        //[Required]
        public int? CalenderUser { get; set; }
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
                start = value.ToUniversalTime();
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
                end = value.ToUniversalTime();
            }
        }

        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public bool IsAllDay { get; set; }
        public string Timezone { get; set; }


        private string GetUserNameById(int userId)
        {
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                var temp = OrderMangtDB.Users.ToList().Where(m => m.Row_Id == userId).FirstOrDefault();
                if (temp != null)
                {
                    return temp.FirstName;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}