using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{

    public interface ISchedulerRepository : IDisposable
    {
        IList<ColorMaster> GetAllColor();
        IList<Event> GetAllEventByUserIdAndDate(List<int> UserIds, DateTime StartDate, DateTime EndDate);
        Event GetEventByCalendarIdAndEventId(string Eventid, int CalendarId);
        Dictionary<string, string> GetEventExceptions();
        IList<EventException> GetAllEventException();
        void AddOrUpdateException(List<EventException> obj);
        void DeleteEvent(Event objEvent);
        IList<CalendarUser> GetAllCalendarUser();
        IList<Calendar> GetAllCalendars();
        IList<Event> GetUnScheduledjoblist(int CalendarId, DateTime StartDate);
        IList<User> GetAllUsersByOrgId(int OrgId);
        IList<UserProductGroup> GetAllUserProductGroups();
        IList<Event> GetAllEventByEventID(string EventID);
        IList<Calendar> GetCalendarsByRowId(int?  RowId);
        IList<Event> GetAllEventByCalendarIdAndEventId(string Eventid, int CalendarId);
        IList<GetCalenderUsers> GetCalenderUsers(int? OrgId);
        IList<GetUserProductCalendar> GetUserProductCalendar(int? OrgId);
        IList<GetGroupCalendarUsers> GetGroupCalendarUsers(int? OrgId, string UserIDs);
        IList<GetAllEventException> GetDistanctEventException();
        void UpdateJobEvents(string EventId, string CalendarName, string UpdatedByCalendarName, int OldEventRowId);
        IList<GetEventRowId> GetEventRowId(string EventId);
        int InsertEventLog(string Event_Id, string Event_Title, string Operation, int User_Id, string Source_calendar, string Destination_calendar);

       // IList<GetEventEmailORPrintData> GetEventEmailORPrintData(int orgid, int? userid, string userType, int? compamyid, string EventId);
        IList<GetEventDetails> GetEventDetails(int orgid, int? userid, string userType, int? compamyid, string EventId);
        IList<SelectCommunicationTemplates> GetCommunicationTemplate(int orgid, int? userid, string userType, int? compamyid, string TemplateType, bool? IsPrint,bool? IsEmail );
        int InsertEventMailToEmailInbox(string To_email, string from_email, string Cc_email, string Bcc_email, string Subject, string MessageBody, int orgid, int? userid, string userType, int? compamyid);

        IList<string> GetEmailAddress(int orgid, int? userid, string userType, int? compamyid, string SearchValue);

    }

    public class SchedulerRepository : ISchedulerRepository, IDisposable
    {
        private OrderMgntEntities db = null;

        public SchedulerRepository()
        {
            this.db = new OrderMgntEntities();
        }
        public SchedulerRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public IList<ColorMaster> GetAllColor()
        {
            return db.ColorMasters.ToList();

        }

        public IList<Event> GetAllEventByUserIdAndDate(List<int> UserIds, DateTime StartDate, DateTime EndDate)
        {
            var Eventlist = new List<Event>();
            Eventlist =
                          db.Events.Where(
                              x =>
                                  UserIds.Contains((int)x.CalendarId) && (x.EventId != null) &&
                                  (x.Status == "confirmed" || x.Status == "tentative") && ((x.StartDate >= StartDate && x.StartDate <= EndDate) || x.IsAllDay == true || !string.IsNullOrEmpty(x.Recurrence))).AsQueryable().ToList();




            var DistinctItems = Eventlist.GroupBy(x => new { x.EventId, x.CalendarId }).Select(y => y.First());

            var c = Eventlist.Count();
            var count = DistinctItems.Count();

            return DistinctItems.ToList();
        }

        public Dictionary<string, string> GetEventExceptions()
        {
            var lstEventException = from lst in db.GetAllEventException()
                                              select new
                                              {
                                                  EventId = lst.Eventid,
                                                  Exception = lst.Exception
                                              };
            return lstEventException.Select(x => new { x.EventId, x.Exception }).ToDictionary(x => x.EventId, x => x.Exception);
         //   return db.EventExceptions.Select(x => new { x.EventId, x.Exception }).ToDictionary(x => x.EventId, x => x.Exception);
        }


        public IList<EventException> GetAllEventException()
        {
            return db.EventExceptions.ToList();
        }

        public void AddOrUpdateException(List<EventException> obj)
        {
            foreach (var objItem in obj)
            {
                var exc = db.EventExceptions.FirstOrDefault(x => x.EventId == objItem.EventId);
                if (exc != null)
                {
                    exc.Exception = objItem.Exception;
                }
                else
                {
                    db.EventExceptions.Add(objItem);
                }

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }  

            }

        }


        public IList<Event> GetAllEventByCalendarIdAndEventId(string Eventid, int CalendarId)
        {
            return db.Events.Where(x => x.EventId == Eventid &&  x.CalendarId == CalendarId).AsQueryable().ToList();
        }



        public Event GetEventByCalendarIdAndEventId(string Eventid, int CalendarId)
        {
            Event objevent = db.Events.Single(e => e.EventId == Eventid && e.CalendarId == CalendarId);
            return objevent;
        }


        /// <summary>
        /// This method is used for updating the Event table
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void DeleteEvent(Event obj)
        {
            obj.Status = "cancelled";
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();

            //  User insertedUser = db.Users.SingleOrDefault(u => u.EmailAddress == obj.EmailAddress);
            // return insertedUser;
        }

        public IList<CalendarUser> GetAllCalendarUser()
        {
            return db.CalendarUsers.ToList();
        }


        public IList<Calendar> GetAllCalendars()
        {
            return db.Calendars.ToList();
        }

        public IList<Event> GetUnScheduledjoblist(int CalendarId, DateTime StartDate)
        {
            return db.Events.Where(x => x.CalendarId == CalendarId && x.EventId != null && x.Status == "confirmed" && x.StartDate > StartDate).ToList().OrderByDescending(x=>x.Updated).ToList();
        }

        public IList<User> GetAllUsersByOrgId(int OrgId)
        {
            return db.Users.Where(m => m.OrgId == OrgId && (m.UserType == 1 || m.UserType == 2) && m.IsActive == true && (m.IsDeleted != null && m.IsDeleted == false)).ToList();
        }


        public IList<UserProductGroup> GetAllUserProductGroups()
        {
            return db.UserProductGroups.ToList();
        }

        public IList<Event> GetAllEventByEventID(string EventID)
        {
            return db.Events.Where(x => x.EventId == EventID && x.Status == "confirmed") .AsQueryable().ToList();
        }


        public IList<Calendar> GetCalendarsByRowId(int? RowId)
        {
            return db.Calendars.Where(x => x.Row_Id == RowId).AsQueryable().ToList();
        }

        public IList<GetCalenderUsers> GetCalenderUsers(int? org_Id)
        {
            return db.GetCalenderUsers(org_Id).ToList();
        }

        public IList<GetUserProductCalendar> GetUserProductCalendar(int? OrgId)
        {
            return db.GetUserProductCalendar(OrgId).ToList();
        }

        public IList<GetGroupCalendarUsers> GetGroupCalendarUsers(int? OrgId, string UserIDs)
        {
            return db.GetGroupCalendarUsers(OrgId,UserIDs).ToList();
        }


        public IList<GetAllEventException> GetDistanctEventException()
        {
            return db.GetAllEventException().ToList();
        }


        public void UpdateJobEvents(string EventId, string CalendarName, string UpdatedByCalendarName, int OldEventRowId)
        {
            db.UpdateJobEvents(EventId, CalendarName, UpdatedByCalendarName, OldEventRowId);
        }

        public IList<GetEventRowId> GetEventRowId(string EventId)
        {
            return db.GetEventRowId(EventId).ToList();
           
        }

        public int InsertEventLog(string Event_Id, string Event_Title, string Operation, int User_Id, string Source_calendar, string Destination_calendar)
        {
            return 0;
           // return db.InsertEventslog(Event_Id, Event_Title, Operation, User_Id, Source_calendar, Destination_calendar);

        }



        public IList<string> GetEmailAddress(int orgid, int? userid, string userType, int? compamyid, string SearchValue)
        {
            return db.SelectEmailAddressAutoComplete(orgid, userid, userType, compamyid, 0, SearchValue).ToList();
        }

        //public IList<GetEventEmailORPrintData> GetEventEmailORPrintData(int orgid, int? userid, string userType, int? compamyid, string EventId)
        //{
        //    return db.GetEventEmailORPrintData(orgid, userid, userType, compamyid).ToList();
        //}


        public IList<GetEventDetails> GetEventDetails(int orgid, int? userid, string userType, int? compamyid, string EventId)
        {
            return db.GetEventDetails(orgid, userid, userType, compamyid, EventId).ToList();
        }


        public IList<SelectCommunicationTemplates> GetCommunicationTemplate(int orgid, int? userid, string userType, int? compamyid, string TemplateType, bool? IsPrint, bool? IsEmail)
        {
            return db.SelectCommunicationTemplates(orgid, userid, userType, compamyid, TemplateType, IsPrint, IsEmail).ToList();
        }

        public int InsertEventMailToEmailInbox(string To_email, string from_email, string Cc_email, string Bcc_email, string Subject, string MessageBody, int orgid, int? userid, string userType, int? compamyid)
        {
            return db.InsertEventMailToEmailInbox(To_email, from_email, Cc_email, Bcc_email, Subject, MessageBody, orgid, userid, userType, compamyid);
        }
    }


}


