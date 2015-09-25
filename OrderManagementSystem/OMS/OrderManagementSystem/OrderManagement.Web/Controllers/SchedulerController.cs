using System.Globalization;
using Newtonsoft.Json;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;
using OrderManagement.Web.Controllers;

namespace TelerikMvcSchedulerPOC2.Controllers
{
    using System.Web.Mvc;
    using TelerikMvcSchedulerPOC2.Models;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;
    using TelerikMvcApp1.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OrderManagement.Web;
    using OrderManagement.Web.Helper.Utilitties;
    using System.IO;
    using System.Web.UI;
    using System.Runtime.Serialization;

    public class SchedulerController : Controller
    {
        public string ErrorMsg = string.Empty;

        protected override void Dispose(bool disposing)
        {
            _schedulerrapo.Dispose();
            base.Dispose(disposing);
        }

        private ISchedulerService _scheduler;

        private SchedulerRepository _schedulerrapo;

        public SchedulerController()
        {
            _schedulerrapo = new SchedulerRepository();
            _scheduler = new SchedulerService(_schedulerrapo);
        }

        public ActionResult Index()
        {
             GoogleServiceMethodCalls objGoogleCal = new GoogleServiceMethodCalls();
            // objGoogleCal.CheckCalendarEvents("campaigntrack.dpi@gmail.com");
            // objGoogleCal.CheckCalendarEvents("dpi.secilcanbay@gmail.com");
            // objGoogleCal.CheckCalendarEvents("dpi.jeffyates@gmail.com");
            // objGoogleCal.CheckCalendarEvents("dpi.richardpugh@gmail.com");
            
            //if (itemlistex.Items[i].Id == "3784i2iqlb187phdp7d76g6et4")
            // {
            // }

            var schedulerViewModel = new Models.SchedulerViewModel();

            var currentuser = UserManager.Current();
            if (currentuser != null)
            {
                // var users = _scheduler.GetAllUsersByOrgId(Convert.ToInt32(currentuser.OrgId));
                //  var usercalenderlst = _scheduler.GetCalenderUsers(Convert.ToInt32(currentuser.OrgId));
                // var userProductGroups = _scheduler.GetUserProductCalendar(Convert.ToInt32(currentuser.OrgId));

                //var calenderUsers =
                //    users.Join(usercalenderlst, e => e.Row_Id, d => d.UserId, (e, d) => new { e.FirstName, e.LastName, e.UserType, d.UserId, d.Color, d.CalendarId }).ToList().OrderBy(x => x.FirstName).ToList(); ;
                var calenderUsers = _scheduler.GetCalenderUsers(Convert.ToInt32(currentuser.OrgId));

                var userProductCalendar = _scheduler.GetUserProductCalendar(Convert.ToInt32(currentuser.OrgId));

                //var userProductCalendar =
                //    calenderUsers.ToList()
                //        .Join(userProductGroups, e => e.UserId, f => f.UserId,
                //            (e, f) => new { e.UserId, e.UserType, e.FirstName, e.LastName, e.CalendarId, f.ProductGroupId });


                var userProductCalendarlst = new List<UserProductCalendar>();

                foreach (var itemgrp in userProductCalendar)
                {

                    if (itemgrp != null)
                    {
                        var grodgrp = new UserProductCalendar();
                        grodgrp.UserId = itemgrp.UserId.ToString();
                        grodgrp.CalendarId = itemgrp.CalendarId.ToString();
                        grodgrp.UserProductGroup = itemgrp.ProductGroupId.ToString();
                        grodgrp.UserName = itemgrp.FirstName + " " + itemgrp.LastName;
                        userProductCalendarlst.Add(grodgrp);
                    }
                }

                schedulerViewModel.UserProductCalendar = userProductCalendarlst;
                var lstresources = new List<ScheduleResources>();
                foreach (var item in calenderUsers)
                {
                    if (item != null)
                    {
                        ScheduleResources scheduleResources = new ScheduleResources();
                        scheduleResources.UserId = item.UserId.ToString();
                        scheduleResources.Text = item.FirstName + " " + item.LastName;
                        scheduleResources.Value = item.CalendarId.ToString();
                        scheduleResources.Color = item.Color;
                        lstresources.Add(scheduleResources);
                    }
                }
                schedulerViewModel.CalenderResources = lstresources.Take(1).ToList();
                schedulerViewModel.AllCalenderResources = lstresources;
                schedulerViewModel.AllUsers = lstresources;
                //Group resource code
            }

            return PartialView("_SchedulerIndex", schedulerViewModel);
        }

        public JsonResult GetUserList([DataSourceRequest] DataSourceRequest request)
        {
            List<ScheduleResources> lstresources = new List<ScheduleResources>();
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                var calenderUsers = OrderMangtDB.CalendarUsers.ToList();
                Controllers.SchedulerController scheduleCntrl = new Controllers.SchedulerController();
                foreach (var item in calenderUsers)
                {
                    ScheduleResources scheduleResources = new ScheduleResources();
                    scheduleResources.Text = scheduleCntrl.GetUserNameById((int)item.UserId);
                    scheduleResources.Value = item.CalendarId.ToString();
                    scheduleResources.Color = item.Color;
                    lstresources.Add(scheduleResources);
                }
                ;
            }
            JsonResult jsonResult = Json(lstresources.ToDataSourceResult(request));
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

        private string GetUserNameById(int userId)
        {
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                var temp = OrderMangtDB.Users.ToList().Where(m => m.Row_Id == userId).FirstOrDefault();
                if (temp != null)
                {
                    return temp.FirstName + " " + temp.LastName;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// This function is used for loading Scheduler data.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="chkselected"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        /// 
        public virtual JsonResult Event_Read(DataSourceRequest request, string chkselected, DateTime start, DateTime end, string Search)
        {
            List<EventException> lstException = new List<EventException>();
            IList<CalEventViewModel> events = new List<CalEventViewModel>();

            if (!string.IsNullOrEmpty(chkselected))
            {
                if (chkselected.EndsWith(","))
                {
                    chkselected = chkselected.Remove(chkselected.Length - 1);
                }

                List<int> userIds = new List<int>(Array.ConvertAll(chkselected.Split(','), int.Parse));

                if (userIds != null && userIds.Count > 0)
                {
                    end = end.AddDays(1);

                    var tasks = _scheduler.GetAllEventByUserIdAndDate(userIds, start, end);
                    var preExceptionList = _scheduler.GetEventExceptions();
                    var getAllEventException = _scheduler.GetAllEventException();
                    var colmaster = _scheduler.GetAllColor();
                    var usercalenderlst = _scheduler.GetAllCalendarUser();
                    if (tasks.ToList().Count > 0)
                    {
                        var reccEvents = tasks.Where(x => !string.IsNullOrEmpty(x.Recurrence)).ToList();

                        Dictionary<string, List<string>> test = new Dictionary<string, List<string>>();
                        foreach (var rec in reccEvents)
                        {
                            var exceptionList = tasks.Where(x => x.EventId.Contains(rec.EventId + "_")).Select(y => y.EventId.Replace(rec.EventId + "_", "")).ToList();
                            test.Add(rec.EventId, exceptionList);
                        }

                        foreach (var item in tasks)
                        {
                            if (!string.IsNullOrEmpty(item.ColorId))
                            {
                                var coloritem = colmaster.Where(m => m.Row_Id == int.Parse(item.ColorId)).FirstOrDefault();
                                item.ColorId = coloritem.Row_Id.ToString();
                                ViewBag.Color = coloritem.Color;
                            }
                            else
                            {
                                item.ColorId = GetUsercalenderColor(item.CalendarId.Value, usercalenderlst, colmaster);
                                var coloritem = colmaster.Where(m => m.Row_Id == int.Parse(item.ColorId)).FirstOrDefault();
                                ViewBag.Color = coloritem.Color;

                            }
                            var eventObj = new CalEventViewModel();
                            if (item.StartDate.Value.Year.ToString() != "1900" && !item.Recurrence.StartsWith("~EXDATE"))
                            {
                                eventObj.EventId = item.EventId;
                                eventObj.Title = item.Title;


                                eventObj.Start = TimeZoneInfo.ConvertTimeToUtc((DateTime)item.StartDate);


                                //.ToUniversalTime();// DateTime.SpecifyKind(DateTime.Parse(item.StartDate.ToString()), DateTimeKind.Utc);
                                eventObj.End = TimeZoneInfo.ConvertTimeToUtc((DateTime)item.EndDate);

                                DateTime endTime = DateTime.Now.AddSeconds(75);
                                TimeSpan span = eventObj.End.Subtract(eventObj.Start);

                                //string timeSlot = string.Format("{0:D2} hours :{1:D2}minutes :{2:D2}seconds",
                                //span.Hours,
                                //span.Minutes,
                                //span.Seconds
                                //);

                                string timeSlot = string.Format("{0:D2} hours :{1:D2}minutes",
                                span.Hours,
                                span.Minutes

                                );

                                bool ChkISAllDay = false;

                                if (item.StartDate.Value.ToString("HH:mm:ss") == "00:00:00" && item.EndDate.Value.ToString("HH:mm:ss") == "00:00:00")
                                {
                                    ChkISAllDay = true;
                                }

                                DateTime dtstart = (DateTime)item.StartDate;
                                DateTime dtEnd = (DateTime)item.EndDate;

                                string strdtStart = dtstart.ToString("dd/MM/yyyy");
                                string strdtEnd = dtEnd.ToString("dd/MM/yyyy");

                                eventObj.TooltipDescription = "Start Date: " + dtstart.ToString() + "\nEnd Date: " + dtEnd.ToString() + "\nDuration: " + timeSlot;

                                //DateTime.Parse(item.StartDate.ToString()).ToUniversalTime();// DateTime.SpecifyKind(DateTime.Parse(item.EndDate.ToString()), DateTimeKind.Utc);
                                eventObj.Description = item.Description;

                                //if (item.StartDate.Value.ToString("HH:mm:ss") == "00:00:00"  && item.EndDate.Value.ToString("HH:mm:ss") == "00:00:00")
                                //{
                                //    eventObj.IsAllDay = true;
                                //    eventObj.End = eventObj.End.AddDays(-1);
                                //}
                                //else
                                //{
                                //    eventObj.IsAllDay = false;
                                //}

                                if (!string.IsNullOrEmpty(item.Recurrence))
                                {

                                    eventObj.RecurrenceRule = item.Recurrence.Replace("~RRULE:", string.Empty);
                                }

                                string exception = string.Empty;
                                if (preExceptionList.Keys.Contains(eventObj.EventId))
                                    exception = preExceptionList[eventObj.EventId];
                                if (test.Keys.Contains(eventObj.EventId))
                                {

                                    foreach (string exc in test[eventObj.EventId])
                                    {

                                        if (exception == string.Empty)
                                        {
                                            if (!exception.Contains(exc))
                                                exception = exc;
                                        }
                                        else
                                        {
                                            if (!exception.Contains(exc))
                                                exception = exception + ";" + exc;
                                        }
                                    }
                                    if (!preExceptionList.Keys.Contains(eventObj.EventId) && !string.IsNullOrEmpty(exception))
                                    {
                                        var EventException = new EventException { EventId = eventObj.EventId, Exception = exception };
                                        lstException.Add(EventException);

                                    }
                                    else if (preExceptionList.Keys.Contains(eventObj.EventId))
                                    {
                                        if (preExceptionList[eventObj.EventId] != exception)
                                        {
                                            var EventExceptionUpdate = getAllEventException.FirstOrDefault(x => x.EventId == eventObj.EventId);

                                            EventExceptionUpdate.Exception = exception;
                                            lstException.Add(EventExceptionUpdate);
                                            //  OrderMangtDB.EventExceptions.Add(EventExceptionUpdate);
                                        }
                                    }

                                    eventObj.RecurrenceException = exception;
                                }
                                eventObj.EventId = item.EventId;
                                eventObj.RecurrenceID = item.RecurrenceID;
                                eventObj.CalendarId = item.CalendarId;
                                eventObj.CalenderUser = item.CalendarId;
                                eventObj.Organizer = item.Organizer;
                                eventObj.Location = item.Location;
                                eventObj.Status = item.Status;
                                eventObj.Creator = item.Creator;
                                eventObj.ColorId = item.ColorId;
                                eventObj.EventColorid = int.Parse(item.ColorId);
                                eventObj.Color = ViewBag.Color;
                                eventObj.Description = item.Description;
                                eventObj.DeleteRecurrenceEvent = item.EventId;
                                if (item.IsAllDay != null && ChkISAllDay)
                                    eventObj.IsAllDay = item.IsAllDay.Value;

                                events.Add(eventObj);
                            }
                        }
                    }

                    lock ("EventException")
                    {
                        _scheduler.AddOrUpdateException(lstException);
                    }

                    var temp = events as IEnumerable<CalEventViewModel>;
                    JsonResult jsonResult = Json(temp.ToDataSourceResult(request));
                    jsonResult.MaxJsonLength = Int32.MaxValue;
                    return jsonResult;
                }
            }
            return null;
        }

        /// <summary>
        /// This method is used for Deleting the Event from the DB and google cal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="eventViewModel"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public virtual JsonResult Event_Destroy([DataSourceRequest] DataSourceRequest request, CalEventViewModel eventViewModel)
        {
            if (ModelState.IsValid)
            {
                //int fromCalendarId = 0;
                string fromCalendar = string.Empty;
                int fromCalendarId = (int)eventViewModel.CalenderUser;
                using (var OrderMangtDB = new OrderMgntEntities())
                {
                    //fromCalendarId = (int)OrderMangtDB.Events.First(m => m.Row_Id == eventRowId).CalendarId;
                    fromCalendar = OrderMangtDB.Calendars.SingleOrDefault(m => m.Row_Id == fromCalendarId).Name;
                }

                try
                {
                    if (!string.IsNullOrEmpty(eventViewModel.EventId))
                    {

                        string eventid = eventViewModel.EventId;

                        var objGoogleCal = new GoogleServiceMethodCalls();
                        objGoogleCal.DeleteEvent(eventid, fromCalendar);


                        using (var OrderMangtDB = new OrderMgntEntities())
                        {
                            var Events =
                                     (from oEvents in OrderMangtDB.Events
                                      where oEvents.EventId == eventid && oEvents.CalendarId == fromCalendarId
                                      select oEvents).ToList();

                            foreach (var Event in Events)
                            {
                                Event.Status = "cancelled";

                            }
                            OrderMangtDB.SaveChanges();

                            int LoggedInUserId = (int)UserManager.Current().Row_Id;
                            DBLogger(eventViewModel.EventId, eventViewModel.Title, "Cancelled Event", LoggedInUserId, fromCalendar, fromCalendar);

                        }


                        // Check if more than one rec is exist 
                        //  var AllEventByCalendarIdAndEventId = _scheduler.GetAllEventByCalendarIdAndEventId(eventid, CalendarId);
                        //    DeleteEvent(eventViewModel.EventId, fromCalendar, fromCalendarId);

                    }
                    else if (!string.IsNullOrEmpty(eventViewModel.GoogleEventid))
                    {
                        DeleteEvent(eventViewModel.GoogleEventid, fromCalendar, fromCalendarId);
                        int LoggedInUserId = (int)UserManager.Current().Row_Id;
                        DBLogger(eventViewModel.GoogleEventid, eventViewModel.Title, "Cancelled Event", LoggedInUserId, fromCalendar, fromCalendar);

                    }

                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                    Logger(msg);
                }

            }

            return Json(new[] { eventViewModel }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        ///  This method is used for Creating the Event in the DB and google cal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="eventViewModel"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult Event_Create([DataSourceRequest] DataSourceRequest request, CalEventViewModel eventViewModel, List<int> userIds)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    using (var OrderMangtDB = new OrderMgntEntities())
                    {
                        var entity = new Event
                        {
                            Title = eventViewModel.Title,
                            //StartDate = DateTime.SpecifyKind(eventViewModel.Start, DateTimeKind.Local),
                            //EndDate = DateTime.SpecifyKind(eventViewModel.End, DateTimeKind.Local), 
                            StartDate = eventViewModel.Start,//StartDateParam,
                            EndDate = eventViewModel.End,
                            Description = eventViewModel.Description,
                            Recurrence = eventViewModel.RecurrenceRule,
                            RecurrenceID = eventViewModel.RecurrenceID,
                            Kind = "calendar#event",
                            IsAllDay = eventViewModel.IsAllDay,
                            CalendarId = eventViewModel.CalenderUser,
                            Organizer = eventViewModel.Organizer,
                            Creator = eventViewModel.Creator,
                            Updated = DateTime.Today,
                            Location = eventViewModel.Location,
                            ColorId = eventViewModel.ColorId
                        };

                        var objGoogleCal = new GoogleServiceMethodCalls();
                        string returnedEventid = string.Empty;
                        int fromCalId;
                        string calName = string.Empty;
                        int orgId;
                        string StagingCalendar = string.Empty;
                        if (eventViewModel.Row_Id != 0)
                        {
                            fromCalId = (int)OrderMangtDB.Events.First(x => x.Row_Id == eventViewModel.Row_Id).CalendarId;
                            calName = OrderMangtDB.Calendars.FirstOrDefault(c => c.Row_Id == fromCalId).Name;
                            orgId = (int)UserManager.Current().OrgId;
                            StagingCalendar = OrderMangtDB.Calendars.FirstOrDefault(x => (x.Org_Id != null && x.Org_Id == orgId) && (x.StagingCalendar != null && x.StagingCalendar == true)).Name;
                        }
                        if (eventViewModel.Isdrag && calName != StagingCalendar && eventViewModel.Row_Id != 0)
                        {
                            string creator = string.Empty;
                            string organizer = string.Empty;
                            string eventId = eventViewModel.GoogleEventid;
                            int eventRowId = eventViewModel.Row_Id;
                            string destinationCalendar = eventViewModel.CalenderUser.ToString();
                            var calendars = OrderMangtDB.Calendars.SingleOrDefault(m => m.Row_Id == eventViewModel.CalenderUser);
                            var fromCalendarId = OrderMangtDB.Events.First(m => m.Row_Id == eventRowId).CalendarId;
                            var fromCalendar = OrderMangtDB.Calendars.SingleOrDefault(m => m.Row_Id == fromCalendarId);
                            if (calendars != null)
                            {
                                if (entity.StartDate != null && entity.EndDate != null)
                                {
                                    Logger("MoveEvent called within Create block Called" + " || " + entity.Title);


                                    int OldEventRowId = Convert.ToInt32(_scheduler.GetEventRowId(eventId).FirstOrDefault().EventRow_id);

                                    returnedEventid = objGoogleCal.MoveEvent(entity.StartDate.Value, entity.EndDate.Value, creator,
                                           organizer,
                                           entity.Location, entity.Title, eventId, calendars.Name, fromCalendar.Name, (int)entity.Sequence, entity.Description ?? string.Empty, entity.Recurrence ?? string.Empty, entity.IsAllDay ?? false, entity.ColorId ?? string.Empty);
                                    eventViewModel.EventId = returnedEventid;


                                    int LoggedInUserId = (int)UserManager.Current().Row_Id;
                                    DBLogger(returnedEventid, entity.Title, "MoveEvent", LoggedInUserId, fromCalendar.Name, calendars.Name);

                                    _scheduler.UpdateJobEvents(returnedEventid, calendars.Name, fromCalendar.Name, OldEventRowId);///UpdateJobEvents
                                    Logger("UpdateJobEvents Called - " + "eventId: " + returnedEventid + " Source calendar Name : " + fromCalendar.Name + " Destination calendar Name : " + calendars.Name);                                                           ///

                                    LoggedInUserId = (int)UserManager.Current().Row_Id;
                                    DBLogger(returnedEventid, entity.Title, "MoveEvent", LoggedInUserId, fromCalendar.Name, calendars.Name);

                                    ThrowCustomExc(returnedEventid);

                                    Logger("MoveEvent end within Create block Called");
                                }
                                // objGoogleCal.UpdateEvent(entity.StartDate.Value, entity.EndDate.Value, creator, organizer, entity.Location, entity.Title, eventId, calendars.Name);
                            }

                        }
                        else if (!string.IsNullOrEmpty(eventViewModel.RecurrenceID))
                        {
                            var evnt = OrderMangtDB.Events.First(x => x.EventId == eventViewModel.RecurrenceID && x.Status != "Cancelled");
                            var cal = OrderMangtDB.Calendars.First(x => x.Row_Id == evnt.CalendarId).Name;
                            string colorid = string.Empty;
                            if (eventViewModel.EventColorid.HasValue)
                            {
                                if (eventViewModel.EventColorid.Value <= 11)
                                    colorid = eventViewModel.EventColorid.ToString();
                            }
                            else
                            {
                                if (eventViewModel.CalenderUser != null)
                                {
                                    // colorid = GetUsercalenderColor(eventViewModel.CalenderUser.Value);
                                    colorid = string.Empty;
                                }
                            }
                            entity.EventId = eventViewModel.RecurrenceID;
                            // entity.Sequence = 0;

                            int Sequenceid = (int)OrderMangtDB.Events.First(x => x.EventId == eventViewModel.RecurrenceID).Sequence;
                            Sequenceid++;
                            entity.Sequence = Sequenceid; // != 0 ? Sequenceid: Sequenceid + 1;

                            Logger("UpdateEventInstance called within Create block Called" + " || " + entity.Title);

                            var returnedEventId = objGoogleCal.UpdateEventInstance((DateTime)entity.StartDate, (DateTime)entity.EndDate, string.Empty, entity.Organizer, entity.Location, entity.Title, entity.EventId, cal, (int)entity.Sequence, entity.Description ?? string.Empty, entity.Recurrence ?? string.Empty, entity.IsAllDay ?? false, colorid ?? entity.ColorId);

                            int LoggedInUserId = (int)UserManager.Current().Row_Id;
                            DBLogger(returnedEventId, entity.Title, "Update Event", LoggedInUserId, cal, cal);

                            // string retUpdateEventId = returnedEventId;

                            ThrowCustomExc(returnedEventId);

                            Logger("UpdateEventInstance end within Create block Called");
                        }
                        else
                        {
                            string colorid = string.Empty;
                            if (eventViewModel.EventColorid.HasValue)
                            {
                                if (eventViewModel.EventColorid.Value < 12) // we only need to pass top 11 clolor while create update 
                                {
                                    colorid = eventViewModel.EventColorid.ToString();
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(eventViewModel.Color))
                                {
                                    //  colorid = eventViewModel.ColorId,

                                    colorid = OrderMangtDB.ColorMasters.First(x => x.Color == eventViewModel.Color).Row_Id.ToString();
                                }
                                else
                                {
                                    colorid = string.Empty;
                                }

                                //if (eventViewModel.CalenderUser != null)
                                //{
                                //    colorid = GetUsercalenderColor(eventViewModel.CalenderUser.Value);
                                //} 
                                //colorid = string.Empty;
                            }
                            var calendars = OrderMangtDB.Calendars.SingleOrDefault(m => m.Row_Id == eventViewModel.CalenderUser);

                            Logger("UpdateEventInstance end within Create block Called" + " || " + entity.Title);

                            returnedEventid = objGoogleCal.CreateEvent(entity.Title, entity.Location, string.Empty,
                                entity.StartDate.Value, entity.EndDate.Value, string.Empty, entity.Description,
                                string.Empty, string.Empty, colorid, calendars.Name, entity.Description ?? string.Empty, entity.Recurrence ?? string.Empty, entity.IsAllDay ?? false);


                            string strreturnedEventid = returnedEventid;

                            string[] parts = strreturnedEventid.Split(',');

                            string EventidfrmGoogle = string.Empty; string seqId = string.Empty;
                            if ((parts[0] != null && parts[0].Length > 0))
                            {
                                EventidfrmGoogle = (parts[0].ToString());
                                seqId = (parts[1].ToString());
                            }

                            int LoggedInUserId = (int)UserManager.Current().Row_Id;
                            DBLogger(EventidfrmGoogle, entity.Title, "Create Event", LoggedInUserId, calendars.Name, calendars.Name);


                            Logger("CreateEvent end within Create block Called");

                            if (returnedEventid != string.Empty)
                            {
                                var retValues = returnedEventid.Split(',');
                                eventViewModel.EventId = retValues[0];
                                eventViewModel.Sequence = Convert.ToInt32(retValues[1]);
                                ThrowCustomExc(retValues[0]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region For Error

                string Error = ErrorMsg;

                if (!string.IsNullOrEmpty(ErrorMsg))
                {

                    switch (Error)
                    {
                        case "CustomError400":
                            return Json(new DataSourceResult
                      {
                          // Errors = "You cannot change the organizer of an instance."
                          Errors = "CustomError400"
                      });

                        case "CustomError401":
                            return Json(new DataSourceResult
                   {
                       //Errors = "You cannot turn an instance of a recurring event into a recurring event itself."
                       Errors = "CustomError401"
                   });


                        case "Null_Event":
                            return Json(new DataSourceResult
                   {
                       Errors = "Null_Event"
                   });

                        //default:
                        //         Logger(ex.InnerException.Message);
                        //         return Json(new DataSourceResult
                        //    {

                        //        Errors = string.Empty
                        //    });
                    }
                }
                else
                {
                    return Json(new DataSourceResult
                    {
                        Errors = ""
                    });
                }
                #endregion

            }

            // Return the inserted task. The scheduler needs the generated TaskID. Also return any validation errors.

            //List<int> userid = new List<int> { };
            //Event_Read(request, "", DateTime.Now, DateTime.Now);

            return Json(new[] { eventViewModel }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// This method is used for Updating the Event in the DB and google cal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="eventViewModel"></param>
        /// <returns></returns>
        public virtual JsonResult Event_Update([DataSourceRequest] DataSourceRequest request, CalEventViewModel eventViewModel)
        {
            try
            {
                string returnedEventId = string.Empty;
                if (string.IsNullOrEmpty(eventViewModel.RecurrenceException))
                {
                    if (ModelState.IsValid)
                    {
                        var tasksEventCalendar = _scheduler.GetAllEventByEventID(eventViewModel.EventId);
                        var CalendarsByRowId = _scheduler.GetCalendarsByRowId(eventViewModel.CalendarId);

                        //  var eventCalendar = OrderMangtDB.Events.First(x => x.EventId == eventViewModel.EventId).CalendarId;
                        var eventCalendar = tasksEventCalendar.First(x => x.EventId == eventViewModel.EventId).CalendarId;

                        var entity = tasksEventCalendar.FirstOrDefault(m => m.EventId == eventViewModel.EventId);

                        // Create a new Task entity and set its properties from the posted TaskViewModel
                        if (!string.IsNullOrEmpty(eventViewModel.EventId))
                        {
                            entity.EventId = eventViewModel.EventId;
                        }

                        entity.Title = eventViewModel.Title;
                        //Specify the DateTimeKind to be UTC 
                        entity.StartDate = eventViewModel.Start;
                        entity.EndDate = eventViewModel.End;
                        entity.Description = eventViewModel.Description;
                        entity.Recurrence = eventViewModel.RecurrenceRule;
                        entity.RecurrenceID = eventViewModel.RecurrenceID;
                        entity.CalendarId = eventViewModel.CalenderUser != null ? eventViewModel.CalenderUser : entity.CalendarId;
                        entity.Organizer = eventViewModel.Organizer;
                        entity.Kind = "calendar#event";
                        entity.Location = eventViewModel.Location;
                        entity.Updated = DateTime.Today;
                        entity.Sequence = eventViewModel.Sequence == 0 ? entity.Sequence + 1 : 0;
                        // entity.Sequence =  eventViewModel.Sequence;
                        entity.IsAllDay = eventViewModel.IsAllDay;

                        GoogleServiceMethodCalls objGoogleCal = new GoogleServiceMethodCalls();

                        var CalendarsUser = _scheduler.GetCalendarsByRowId(eventViewModel.CalenderUser);

                        var calendars = CalendarsUser.SingleOrDefault(m => m.Row_Id == eventViewModel.CalenderUser);
                        string DestinationCalName = CalendarsUser.SingleOrDefault(m => m.Row_Id == eventViewModel.CalenderUser).Name.ToString();

                        if (eventCalendar != eventViewModel.CalenderUser)
                        {
                            // var fromCalendarId = OrderMangtDB.Events.First(m => m.Row_Id == eventRowId).CalendarId;
                            var fromCalendar = CalendarsByRowId.SingleOrDefault(m => m.Row_Id == eventViewModel.CalendarId);
                            string frmCalName = fromCalendar.Name.ToString();

                            if (entity.StartDate != null && entity.EndDate != null)

                                Logger("MoveEvent within Update block Called" + " || " + entity.Title);

                            int OldEventRowId = Convert.ToInt32(_scheduler.GetEventRowId(eventViewModel.EventId).FirstOrDefault().EventRow_id);

                            returnedEventId = objGoogleCal.MoveEvent(entity.StartDate.Value, entity.EndDate.Value, string.Empty, string.Empty,
                                entity.Location, entity.Title, entity.EventId, DestinationCalName, frmCalName, (int)entity.Sequence, entity.Description ?? string.Empty, entity.Recurrence ?? string.Empty, entity.IsAllDay ?? false, entity.ColorId ?? string.Empty);
                            eventViewModel.EventId = returnedEventId;


                            int LoggedInUserId = (int)UserManager.Current().Row_Id;
                            DBLogger(returnedEventId, entity.Title, "Move Event", LoggedInUserId, frmCalName, DestinationCalName);

                            _scheduler.UpdateJobEvents(returnedEventId, DestinationCalName, frmCalName, OldEventRowId);///UpdateJobEvents
                            ///
                            Logger("UpdateJobEvents Called - " + "eventId: " + returnedEventId + " Source calendar Name : " + frmCalName + " Destination calendar Name : " + DestinationCalName);                                                           ///

                            LoggedInUserId = (int)UserManager.Current().Row_Id;

                            DBLogger(returnedEventId, entity.Title, "Move Event", LoggedInUserId, frmCalName, DestinationCalName);

                            ThrowCustomExc(returnedEventId);

                            Logger("MoveEvent end within Update block Called");
                        }
                        else
                        {
                            string colorid = string.Empty;
                            if (eventViewModel.EventColorid.HasValue)
                            {
                                if (eventViewModel.EventColorid.Value <= 11)
                                    colorid = eventViewModel.EventColorid.ToString();
                            }
                            else
                            {
                                if (eventViewModel.CalenderUser != null)
                                {
                                    // colorid = GetUsercalenderColor(eventViewModel.CalenderUser.Value);
                                    colorid = string.Empty;
                                }
                            }

                            Logger("UpdateEvent called within Update block " + " || " + entity.Title);

                            returnedEventId = objGoogleCal.UpdateEvent((DateTime)entity.StartDate, (DateTime)entity.EndDate, string.Empty, entity.Organizer, entity.Location, entity.Title, entity.EventId, calendars.Name, (int)entity.Sequence, entity.Description ?? string.Empty, entity.Recurrence ?? string.Empty, entity.IsAllDay ?? false, colorid ?? entity.ColorId);

                            int LoggedInUserId = (int)UserManager.Current().Row_Id;
                            DBLogger(returnedEventId, entity.Title, "Update Event", LoggedInUserId, calendars.Name, calendars.Name);

                            ThrowCustomExc(returnedEventId);

                            eventViewModel.EventId = returnedEventId;

                            Logger("UpdateEvent end within Update block ");
                        }
                    }
                }

                // Return the updated task. Also return any validation errors.
                return Json(new[] { eventViewModel }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                #region For Error

                string Error = ErrorMsg;

                if (!string.IsNullOrEmpty(ErrorMsg))
                {

                    switch (Error)
                    {
                        case "CustomError400":
                            return Json(new DataSourceResult
                            {
                                // Errors = "You cannot change the organizer of an instance."
                                Errors = "CustomError400"
                            });

                        case "CustomError401":
                            return Json(new DataSourceResult
                            {
                                //Errors = "You cannot turn an instance of a recurring event into a recurring event itself."
                                Errors = "CustomError401"
                            });

                        case "Null_Event":
                            return Json(new DataSourceResult
                            {
                                Errors = "Null_Event"
                            });

                        default:
                            return Json(new DataSourceResult
                            {
                                Errors = ""
                            });
                    }
                }
                else
                {
                    return Json(new DataSourceResult
                    {
                        Errors = ""
                    });
                }
                #endregion For Error

            }
        }

        /// <summary>
        /// This function is used for loading unscheduled job ine Grid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult Jobs_Read([DataSourceRequest] DataSourceRequest request)
        {
            var colormaster = _scheduler.GetAllColor();
            var usercalenderlst = _scheduler.GetAllCalendarUser();
            var calenderlist = _scheduler.GetAllCalendars();

            var orgId = UserManager.Current().OrgId;

            var StagingCalendar = calenderlist.FirstOrDefault(x => (x.Org_Id != null && x.Org_Id == orgId) && (x.StagingCalendar != null && x.StagingCalendar == true));

            if (StagingCalendar != null)
            {
                DateTime dtbeforeOneMon = DateTime.Now.AddDays(-31);
                var unScheduledjoblist = _scheduler.GetUnScheduledjoblist(StagingCalendar.Row_Id, dtbeforeOneMon);

                if (unScheduledjoblist.Count > 0)
                {
                    IList<CalEventViewModel> events = new List<CalEventViewModel>();

                    foreach (var item in unScheduledjoblist)
                    {
                        if (!string.IsNullOrEmpty(item.ColorId))
                        {
                            var coloritem = colormaster.Where(m => m.Row_Id == int.Parse(item.ColorId)).FirstOrDefault();
                            item.ColorId = coloritem.Row_Id.ToString();
                            ViewBag.Color = coloritem.Color;
                        }
                        else
                        {
                            item.ColorId = GetUsercalenderColor(item.CalendarId.Value, usercalenderlst, colormaster);
                            var coloritem = colormaster.Where(m => m.Row_Id == int.Parse(item.ColorId)).FirstOrDefault();
                            ViewBag.Color = coloritem.Color;

                        }

                        var eventObj = new CalEventViewModel();
                        eventObj.Row_Id = item.Row_Id;
                        eventObj.EventId = item.EventId;
                        eventObj.Title = item.Title;
                        //Specify the DateTimeKind to be UTC
                        eventObj.Start = DateTime.Today;
                        eventObj.End = DateTime.Today.AddHours(1);
                        eventObj.Description = item.Description;
                        eventObj.IsAllDay = false;
                        eventObj.RecurrenceException = null;
                        eventObj.RecurrenceID = null;
                        eventObj.UnscheduledJobs = item.Title;
                        eventObj.Location = item.Location;
                        eventObj.ColorId = item.ColorId;
                        eventObj.Color = ViewBag.Color; // ColorMasterColor.FirstOrDefault(cm => cm.Row_Id == Convert.ToInt32(item.ColorId == "" ? GetUsercalenderColor(item.CalendarId.Value) : item.ColorId)).Color; // Setting default color code 0  //"#DC2127";
                        //eventObj.UnScheduleToolTip = "Title: "+ item.Title + "\nDescription: " + item.Description + "\nRequired Date: " + item.StartDate.Value.ToShortDateString() + "\nLocation: " + item.Location;
                        eventObj.UnScheduleToolTip = "Title: " + item.Title + "\nLocation: " + item.Location + "\nDescription: " + item.Description;
                        events.Add(eventObj);
                    }

                    var temp = events as IEnumerable<CalEventViewModel>;
                    JsonResult jsonResult = Json(temp.ToDataSourceResult(request));
                    jsonResult.MaxJsonLength = Int32.MaxValue;
                    return jsonResult;
                }
            }
            //  }

            return null;
        }

        /// <summary>
        /// This function is used for deleting the unscheduled job from the grid
        /// </summary>
        /// <param name="request"></param>
        /// <param name="eventViewModel"></param>
        /// <returns></returns>
        public ActionResult Jobs_Destroy([DataSourceRequest] DataSourceRequest request, CalEventViewModel eventViewModel)
        {
            int fromCalendarId = 0;
            string fromCalendar = string.Empty;
            int eventRowId = eventViewModel.Row_Id;
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                fromCalendarId = (int)OrderMangtDB.Events.First(m => m.Row_Id == eventRowId).CalendarId;
                fromCalendar = OrderMangtDB.Calendars.SingleOrDefault(m => m.Row_Id == fromCalendarId).Name;
            }
            try
            {
                if (!string.IsNullOrEmpty(eventViewModel.EventId))
                {
                    DeleteEvent(eventViewModel.EventId, fromCalendar, fromCalendarId);
                }
                else if (!string.IsNullOrEmpty(eventViewModel.GoogleEventid))
                {
                    DeleteEvent(eventViewModel.GoogleEventid, fromCalendar, fromCalendarId);
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return Json(new[] { eventViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult GroupCalendar(IEnumerable<int> userIds)
        {
            var schedulerViewModel = new Models.SchedulerViewModel();

            var currentuser = UserManager.Current();

            string struserids = userIds.Select(x => x.ToString()).Aggregate("", (a, b) => a + "" + b + ",").ToString();
            struserids = struserids.Remove(struserids.Length - 1, 1);

            var calendarUser = _scheduler.GetGroupCalendarUsers(currentuser.OrgId, struserids);

            //var calendarUser2 = OrderMangtDB.CalendarUsers.Where(x => userIds.Contains((int)x.CalendarId))
            // .Distinct()
            // .Join(
            //     OrderMangtDB.Users.Where(m => m.OrgId == currentuser.OrgId && (m.UserType == 1 || m.UserType == 2) && m.IsActive == true && m.IsDeleted == false), cu => cu.UserId, u => u.Row_Id, (cu, u) => new { u.FirstName, u.LastName, u.UserType, cu.UserId, cu.Color, cu.CalendarId }).OrderBy(x => x.FirstName).ToList();


            List<ScheduleResources> lstresources = new List<ScheduleResources>();
            if (calendarUser != null)
            {
                foreach (var item in calendarUser)
                {
                    ScheduleResources scheduleResources = new ScheduleResources();
                    scheduleResources.Text = item.FirstName + " " + item.LastName; // GetUserNameById((int)item.UserId);
                    scheduleResources.Value = item.CalendarId.ToString();
                    scheduleResources.Color = item.Color;
                    lstresources.Add(scheduleResources);
                }
            }

            schedulerViewModel.CalenderResources = lstresources;

            return PartialView("_Scheduler", schedulerViewModel);
        }

        /// <summary>
        /// This function is used for updating the event as Cancelled in the Event table
        /// </summary>
        /// <param name="eventid"></param>
        /// <param name="calendar"></param>
        /// <param name="CalendarId"></param>
        /// 
        private void DeleteEvent(string eventid, string calendar, int CalendarId)
        {
            if (!string.IsNullOrEmpty(eventid))
            {
                var objGoogleCal = new GoogleServiceMethodCalls();
                objGoogleCal.DeleteEvent(eventid, calendar);
                // Check if more than one rec is exist 
                //  var AllEventByCalendarIdAndEventId = _scheduler.GetAllEventByCalendarIdAndEventId(eventid, CalendarId);

                // if (AllEventByCalendarIdAndEventId.Count > 1)
                //  {
                using (var OrderMangtDB = new OrderMgntEntities())
                {
                    var Events =
                             (from oEvents in OrderMangtDB.Events
                              where oEvents.EventId == eventid && oEvents.CalendarId == CalendarId
                              select oEvents).ToList();

                    foreach (var Event in Events)
                    {
                        Event.Status = "cancelled";

                    }
                    OrderMangtDB.SaveChanges();
                }
                //  }
                //else
                //{
                //    Event objevent1 = _scheduler.GetEventByCalendarIdAndEventId(eventid, CalendarId);
                //    _scheduler.DeleteEvent(objevent1);
                //}

                //using (var OrderMangtDB = new OrderMgntEntities())
                //{
                //    Event objevent = OrderMangtDB.Events.Single(e => e.EventId == eventid && e.CalendarId == CalendarId);
                //    objevent.Status = "cancelled";
                //    OrderMangtDB.SaveChanges();
                //}

            }

        }

        public JsonResult GetCalenderUser()
        {
            IList<ScheduleResources> lstresources = new List<ScheduleResources>();
            try
            {
                // using (var OrderMangtDB = new OrderMgntEntities())
                // {
                var currentuser = UserManager.Current();
                if (currentuser != null)
                {

                    var users = _scheduler.GetAllUsersByOrgId(currentuser.OrgId.Value);
                    var calendarUserlst = _scheduler.GetAllCalendarUser();
                    var calenderUsers =
                        users.Where(
                            m => (m.UserType == 1 || m.UserType == 2) && m.IsActive == true)
                            .Join(calendarUserlst, e => e.Row_Id, d => d.UserId,
                                (e, d) => new { e.FirstName, e.LastName, e.UserType, d.UserId, d.Color, d.CalendarId }).OrderBy(e => e.FirstName);

                    if (calenderUsers != null && calenderUsers.ToList().Count > 0)
                    {
                        foreach (var item in calenderUsers.ToList())
                        {
                            var scheduleResources = new ScheduleResources();
                            scheduleResources.Text = item.FirstName + " " + item.LastName;
                            scheduleResources.Value = item.CalendarId.ToString();
                            if (item != null && item.Color != null)
                            {
                                scheduleResources.Color = item.Color.Replace("#", "\\#");
                            }
                            else
                            {
                                scheduleResources.Color = "";
                            }
                            lstresources.Add(scheduleResources);
                        }
                    }
                }
                //  }


            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }

            return Json(lstresources, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventColor()
        {
            IList<EventColor> lsteventColor = new List<EventColor>();
            try
            {
                var colorlist = _scheduler.GetAllColor().OrderBy(m => m.Row_Id).Take(11).ToList().OrderBy(m => m.ColorName);

                foreach (var item in colorlist)
                {
                    var eventColor = new EventColor();
                    eventColor.Text = item.ColorName;
                    eventColor.Value = item.Row_Id.ToString();
                    if (item != null && item.Color != null)
                    {
                        eventColor.Color = item.Color.Replace("#", "\\#");
                    }

                    lsteventColor.Add(eventColor);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Logger(msg);
            }

            return Json(lsteventColor, JsonRequestBehavior.AllowGet);
        }

        private static string GetUsercalenderColor(int calendarid, IList<CalendarUser> calendarUser, IList<ColorMaster> colorlist)
        {
            string colorid = string.Empty;
            try
            {
                if (calendarid != 0)
                {

                    if (calendarUser.ToList().Count > 0 && colorlist.ToList().Count > 0)
                    {
                        var firstOrDefault = calendarUser.FirstOrDefault(m => m.CalendarId == calendarid);
                        if (firstOrDefault != null)
                        {
                            var colorMaster = colorlist.FirstOrDefault(c => c.Color == firstOrDefault.Color);
                            if (colorMaster != null)
                                colorid = colorMaster.Row_Id.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                string msg = ex.Message;

            }

            return colorid;
        }

        public ActionResult DeleteRecurrence(string eventid, int calendarId, string calenderUser, DateTime end, DateTime start)
        {

            string calenderName = string.Empty;
            string rtneventid = string.Empty;
            string exceptionfrmDB = string.Empty;
            string strNewDate = string.Empty;

            var objGoogleCal = new GoogleServiceMethodCalls();
            using (var OrderMangtDB = new OrderMgntEntities())
            {
                calenderName = OrderMangtDB.Calendars.FirstOrDefault(m => m.Row_Id == calendarId).Name;

                if (!string.IsNullOrEmpty(calenderName))
                {

                    rtneventid = objGoogleCal.DeleteEventInstance(eventid, calenderName, start);

                    if (!rtneventid.Equals("Instance not available"))
                    {
                        string Currexception = rtneventid.Split('_').Last();
                        //var datepart = Currexception.Split('T').First();
                        //var newDate = DateTime.ParseExact(datepart, "yyyyMMdd", CultureInfo.InvariantCulture).AddDays(1);
                        //strNewDate = newDate.Year.ToString() + newDate.Month.ToString() + newDate.Day.ToString();
                        //Currexception = Currexception.Replace(datepart, strNewDate);

                        string streventid = eventid;

                        var exceptionEntry = OrderMangtDB.EventExceptions.FirstOrDefault(c => c.EventId == streventid);

                        if (exceptionEntry != null)
                        {
                            exceptionfrmDB = exceptionEntry.Exception;
                        }

                        if (exceptionfrmDB == string.Empty)
                        {
                            if (!exceptionfrmDB.Contains(Currexception))
                                exceptionfrmDB = Currexception;
                        }
                        else
                        {
                            exceptionfrmDB = exceptionfrmDB + ";" + Currexception;
                        }

                        if (exceptionEntry == null)
                        {
                            var EventException = new EventException { EventId = eventid, Exception = exceptionfrmDB };
                            OrderMangtDB.EventExceptions.Add(EventException);
                        }
                        else
                        {
                            exceptionEntry.Exception = exceptionfrmDB;
                        }

                        OrderMangtDB.SaveChanges();
                    }
                    else
                    {
                        using (var OrderMangtDB2 = new OrderMgntEntities())
                        {
                            var Events =
                                     (from oEvents in OrderMangtDB2.Events
                                      where oEvents.EventId == eventid && oEvents.CalendarId == calendarId
                                      select oEvents).ToList();

                            foreach (var Event in Events)
                            {
                                Event.Status = "cancelled";

                            }
                            OrderMangtDB2.SaveChanges();
                        }
                    }
                }
            }
            return Content(rtneventid);
        }

        private void Logger(string logText)
        {
            using (StreamWriter tw = new StreamWriter("d:\\Google.txt", true))
            {
                tw.WriteLine(logText);
                tw.WriteLine(DateTime.Now);
                tw.WriteLine("--------------------");
            }
        }

        private void DBLogger(string Event_Id, string Event_Title, string Operation, int User_Id, string Source_calendar, string Destination_calendar)
        {
            string EventId = Event_Id.Split(',')[0];
            int row_id = Convert.ToInt32(_scheduler.InsertEventLog(EventId, Event_Title, Operation, User_Id, Source_calendar, Destination_calendar));

        }

        private void ThrowCustomExc(string returnedEventIdFromGoogle)
        {
            switch (returnedEventIdFromGoogle)
            {
                case "CustomError400":
                    ErrorMsg = "CustomError400";
                    Logger(ErrorMsg + " You cannot change the organizer of an instance.");
                    throw new CustomException("you cannot change the organizer of an instance.");

                case "CustomError401":
                    Logger(ErrorMsg + " You cannot turn an instance of a recurring event into a recurring event itself.");
                    ErrorMsg = "CustomError401";
                    throw new CustomException("You cannot turn an instance of a recurring event into a recurring event itself.");

                case "Null_Event":
                    ErrorMsg = "Null_Event";
                    Logger(ErrorMsg + " You cannot change the organizer of an instance - Null Event.");
                    throw new CustomException("you cannot change the organizer of an instance.");
            }
        }

        /// <summary>
        /// This Funnction is used for Printing Event Printing Option
        /// </summary>
        /// <param name="event_Id"></param>
        /// <returns></returns>
        public JsonResult Event_Print(string event_Id)
        {

            string EventTitle = string.Empty;
            string EventLocation = string.Empty;
            string EventDescrption = string.Empty;
            string Organizer = string.Empty;
            string Startdate = string.Empty;
            string Enddate = string.Empty;

            var eventDetils = _scheduler.GetEventDetails(event_Id);

            if (eventDetils != null && eventDetils.Count > 0)
            {
                foreach (var item in eventDetils)
                {
                    EventTitle = item.Title;
                    EventLocation = item.Location;
                    EventDescrption = item.Description;
                    Startdate = String.Format("{0:f}", item.StartDate.Value);
                    Enddate = String.Format("{0:f}", item.EndDate.Value);
                    Organizer = item.Organizer;
                }


                string TemplateType = "Booking Confirmation";

                var EventPrintDataNew = _scheduler.GetEventCommunicationTemplate(TemplateType, true, false);


                string PrintContent = string.Empty;

                foreach (var item in EventPrintDataNew)
                {

                    if (string.IsNullOrEmpty(PrintContent))
                    {
                        PrintContent = item.TEMPLATE;
                    }

                    if ("[$Location$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        PrintContent = PrintContent.Replace("[$Location$]", EventLocation);
                    }

                    if ("[$Start$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        PrintContent = PrintContent.Replace("[$Start$]", Startdate.ToString());
                    }

                    if ("[$End$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        PrintContent = PrintContent.Replace("[$End$]", Enddate);
                    }


                    if ("[$Description$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        PrintContent = PrintContent.Replace("[$Description$]", EventDescrption);
                    }

                    if ("[$Title$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        PrintContent = PrintContent.Replace("[$Title$]", EventTitle);
                    }

                    if ("[$Calendar$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        PrintContent = PrintContent.Replace("[$Calendar$]", string.Empty);
                    }

                    PrintContent = PrintContent.Replace("[$PrintOrEmail$]", "print");

                    item.TEMPLATE = PrintContent;
                }

                List<string> Print = new List<string>();
                Print.Add(PrintContent);
                return Json(Print, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// This function is used forload event data into Email Popup
        /// </summary>
        /// <param name="event_Id"></param>
        /// <returns></returns>
        public JsonResult LoadMailContent(string event_Id)
        {
            string EventTitle = string.Empty;
            string EventLocation = string.Empty;
            string EventDescrption = string.Empty;
            string Organizer = string.Empty;
            string Startdate = string.Empty;
            string Enddate = string.Empty;
            string EmailContent = string.Empty;
            string FromEmail = string.Empty;
            string EmailSubject = string.Empty;
            string EmailFromDisplay = string.Empty;
            string EmailCC = string.Empty;
            string EmailBCC = string.Empty;

            var eventDetils = _scheduler.GetEventDetails(event_Id);

            if (eventDetils != null && eventDetils.Count > 0)
            {
                foreach (var item in eventDetils)
                {
                    EventTitle = item.Title;
                    EventLocation = item.Location;
                    EventDescrption = item.Description;
                    Startdate = String.Format("{0:f}", item.StartDate.Value);
                    Enddate = String.Format("{0:f}", item.EndDate.Value);
                    Organizer = item.Organizer;
                }

                string TemplateType = "Booking Confirmation";

                var EventPrintDataNew = _scheduler.GetEventCommunicationTemplate(TemplateType, false, true);

                foreach (var item in EventPrintDataNew)
                {
                    if (string.IsNullOrEmpty(EmailContent))
                    {
                        EmailContent = item.TEMPLATE;
                    }

                    if (string.IsNullOrEmpty(FromEmail))
                    {
                        FromEmail = item.FROM;
                    }

                    if (string.IsNullOrEmpty(EmailCC))
                    {
                        EmailCC = item.CC_TO;
                    }

                    if (string.IsNullOrEmpty(EmailBCC))
                    {
                        EmailBCC = item.BCC_TO;
                    }

                    if (string.IsNullOrEmpty(EmailFromDisplay))
                    {
                        if (!string.IsNullOrEmpty(item.FROM_DISPLAY_AS))
                        {
                            EmailFromDisplay = item.FROM_DISPLAY_AS;
                        }
                        else
                        {
                            EmailFromDisplay = "Schedly";
                        }
                    }

                    if (string.IsNullOrEmpty(EmailSubject))
                    {
                        EmailSubject = item.EMAIL_SUBJECT;
                        EmailSubject = EmailSubject.Replace("[$Location$]", EventLocation);
                    }

                    if ("[$Location$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$Location$]", EventLocation);
                    }

                    if ("[$StartTime$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$StartTime$]", Startdate);
                    }

                    if ("[$StartTime$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$StartTime$]", Startdate);
                    }

                    if ("[$EndTime$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$EndTime$]", Enddate);
                    }

                    if ("[$Description$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$Description$]", EventDescrption);
                    }

                    if ("[$Title$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$Title$]", EventTitle);
                    }

                    if ("[$Calendar$]".Equals(item.MERGE_ATTRIBUTE.ToString().Trim()))
                    {
                        EmailContent = EmailContent.Replace("[$Calendar$]", string.Empty);
                    }

                    EmailContent = EmailContent.Replace("[$PrintOrEmail$]", "print");

                    item.TEMPLATE = EmailContent;
                }

                List<string> lstEmailContent = new List<string>();
                lstEmailContent.Add(EmailContent);

                List<EventData> lstEmailData = new List<EventData>();
                EventData objEmailData = new EventData();

                objEmailData.EmailContent = EmailContent;
                objEmailData.EmailFrom = FromEmail;
                objEmailData.EmailSubject = EmailSubject;
                objEmailData.EmailFromDisplay = EmailFromDisplay;
                objEmailData.EmailCC = EmailCC;
                objEmailData.EmailBCC = EmailBCC;

                lstEmailData.Add(objEmailData);

                //var data = _repository.GetJobAttachmentTypes(Convert.ToInt32(groupid)).Select(p => new
                //{
                //    AllowedFileExtensions = p.AllowedFileExtension,
                //    MaxFileSize = p.MaxFileSize
                //});
                // return Json(data, JsonRequestBehavior.AllowGet);

                return Json(lstEmailData, JsonRequestBehavior.AllowGet);

                // return Json(EventPrintDataNew, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// This function is used fro sending Event email to users
        /// </summary>
        /// <param name="Emailmodel"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult Event_MailSent(SchedulerEmailModel Emailmodel)
        {

            List<EmailTo> EmailTo = new List<EmailTo>();
            List<EmailCC> EmailCC = new List<EmailCC>();
            List<EmailBCC> EmailBCC = new List<EmailBCC>();

            String strSubject = Emailmodel.MailSubject;
            String strMailFrom = Emailmodel.MailFrom;
            String strMailFromDisplay = Emailmodel.MailFromDisplay;

            String strEmailCC = string.Empty;
            String strEmailBCC = string.Empty;

            String strEmailTo = Emailmodel.MailTo;
            string[] EmailIdTos = strEmailTo.Split(',');
            foreach (string Emailid in EmailIdTos)
            {
                EmailTo objEmailTo = new EmailTo();
                objEmailTo.Email_Id = Emailid;
                EmailTo.Add(objEmailTo);
            }

            if (!string.IsNullOrEmpty(Emailmodel.MailCC))
            {
                strEmailCC = Emailmodel.MailCC;
                string[] EmailIdCCs = strEmailCC.Split(',');
                foreach (string Emailid in EmailIdCCs)
                {
                    EmailCC objEmailCC = new EmailCC();
                    objEmailCC.Email_Id = Emailid;
                    EmailCC.Add(objEmailCC);
                }
            }

            if (!string.IsNullOrEmpty(Emailmodel.MailBCC))
            {
                strEmailBCC = Emailmodel.MailBCC;
                string[] EmailIdBCCs = strEmailBCC.Split(',');
                foreach (string Emailid in EmailIdBCCs)
                {
                    EmailBCC objEmailBCC = new EmailBCC();
                    objEmailBCC.Email_Id = Emailid;
                    EmailBCC.Add(objEmailBCC);
                }
            }

            var resposne = Email.SendEmailFromMailGunServer(strSubject, Emailmodel.MailBody, EmailTo, EmailCC, EmailBCC, strMailFrom, strMailFromDisplay);
            string resposecode = resposne.StatusCode.ToString();

            // saving mail content in the Emailinbox table  
            if (resposecode.Equals("OK"))
            {
                int ret = _scheduler.InsertEventMailToEmailInbox(strEmailTo, strMailFrom, strEmailCC, strEmailBCC, strSubject, Emailmodel.MailBody);
                return Json("200", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("100", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This function is used for Load Auto Complete Email textboxes 
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public JsonResult LoadAutoCompleteEMails(string Search)
        {
            var emailList = _scheduler.GetEmailAddress("");

            List<EmailTo> EmailTo = new List<EmailTo>();

            foreach (var item in emailList)
            {
                EmailTo objEmail = new EmailTo();
                if (!string.IsNullOrEmpty(item))
                {
                    objEmail.Email_Id = item;
                    EmailTo.Add(objEmail);
                }
            }

            //string strEmailCC = "manojsoni80@gmail.com;manojkumar.soni@e-zest.in;christopher.kerr@zerofootprint.com.au;test@test.com;dpi@zerofootprint.com";
            //string[] EmailIdCCs = strEmailCC.Split(';');
            //foreach (string Emailid in EmailIdCCs)
            //{
            //    EmailTo objEmailCC = new EmailTo();
            //    objEmailCC.Email_Id = Emailid;
            //    EmailTo.Add(objEmailCC);
            //}

            List<string> Emaillist = new List<string>();
            foreach (var item in EmailTo)
            {
                Emaillist.Add(item.Email_Id);
            }

            var result = Emaillist.ToArray();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }



    /// <summary>
    /// This Class is used for CustomException
    /// </summary>
    [Serializable]
    public class CustomException : Exception
    {
        public CustomException()
            : base() { }

        public CustomException(string message)
            : base(message) { }

        public CustomException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public CustomException(string message, Exception innerException)
            : base(message, innerException) { }

        public CustomException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected CustomException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

}