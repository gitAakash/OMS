using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models.Repository;


namespace OrderManagement.Web.Models.ServiceRepository
{
    public interface ISchedulerService
    {
      IList<ColorMaster> GetAllColor();
      IList<Event> GetAllEventByUserIdAndDate(List<int> UserIds, DateTime StartDate, DateTime EndDate);
      Dictionary<string, string> GetEventExceptions();
      IList<EventException> GetAllEventException();
      void AddOrUpdateException(List<EventException> obj);
      void DeleteEvent(Event objEvent);
      Event GetEventByCalendarIdAndEventId(string Eventid, int CalendarId);
      IList<CalendarUser> GetAllCalendarUser();
      IList<Calendar> GetAllCalendars();
      IList<Event> GetUnScheduledjoblist(int CalendarId, DateTime StartDate);
      IList<User> GetAllUsersByOrgId(int OrgId);
      IList<UserProductGroup> GetAllUserProductGroups();
      IList<Event> GetAllEventByEventID(string EventID);
      IList<Calendar> GetCalendarsByRowId(int? RowId);
      IList<Event> GetAllEventByCalendarIdAndEventId(string Eventid, int CalendarId);
      IList<GetCalenderUsers> GetCalenderUsers(int? OrgId);
      IList<GetUserProductCalendar> GetUserProductCalendar(int? OrgId);
      IList<GetGroupCalendarUsers> GetGroupCalendarUsers(int? OrgId, string UserIDs);
      IList<GetAllEventException> GetDistanctEventException();
      void UpdateJobEvents(string EventId, string CalendarName, string UpdatedByCalendarName,int OldEventRowId);
      IList<GetEventRowId> GetEventRowId(string EventId);
      int InsertEventLog(string Event_Id, string Event_Title, string Operation, int User_Id, string Source_calendar, string Destination_calendar);
      IList<SelectEventsBySearchValue> GetAllEventBySearch(string SearchText);
      IList<GetEventEmailORPrintData> GetEventEmailORPrintData(string Event_id);
      IList<GetEventDetails> GetEventDetails(string Event_id);
      IList<SelectCommunicationTemplates> GetEventCommunicationTemplate(string TemplateType, bool? IsPrint, bool? IsEmail);
      int InsertEventMailToEmailInbox(string To_email, string from_email, string Cc_email, string Bcc_email, string Subject, string MessageBody);//, int? userid, string userType, int? compamyid);
      IList<string> GetEmailAddress(string SearchValue);
      IList<GetEmailTemplateData_Result> GetEmailTemplateData(string JobId);

    }


    public class SchedulerService : ISchedulerService
    {
        private ISchedulerRepository _repository;
        public SchedulerService(ISchedulerRepository repository)
        {
            _repository = repository;
        }

        public IList<ColorMaster> GetAllColor()
        {
            return _repository.GetAllColor();
        }

        public IList<Event> GetAllEventByUserIdAndDate(List<int> UserIds, DateTime StartDate, DateTime EndDate)
        {
            return _repository.GetAllEventByUserIdAndDate(UserIds, StartDate, EndDate);

        }

        public IList<SelectEventsBySearchValue> GetAllEventBySearch(string SearchText)
        {

             var currentUser = UserManager.Current();
             if (currentUser != null)
             {
                 if (currentUser.OrgId != null)
                 {
                     int orgid = currentUser.OrgId.Value;
                     var userid = currentUser.Row_Id;
                     var userType = currentUser.UserType;
                     string userTypeName = currentUser.UserType1.Name;
                     int? compamyid = null;

                     if (currentUser.UserType == 3)
                     {
                         compamyid = currentUser.CompanyId;
                     }

                     if (string.IsNullOrEmpty(SearchText))
                         SearchText = string.Empty;

                     var Eventlist = _repository.GetAllEventBySearch(orgid, userid, userTypeName, compamyid, SearchText).ToList();
                     return Eventlist;
                 }
             }
             return null;
            
        }

        public Dictionary<string, string> GetEventExceptions()
        {
            return _repository.GetEventExceptions();
        }

        public IList<EventException> GetAllEventException()
        {
            return _repository.GetAllEventException();

        }

        public void AddOrUpdateException(List<EventException> obj)
        {
            _repository.AddOrUpdateException(obj);
        }

        public Event GetEventByCalendarIdAndEventId(string Eventid, int CalendarId)
        {
            return _repository.GetEventByCalendarIdAndEventId(Eventid, CalendarId);
        }

        public void DeleteEvent(Event objEvent)
        {
            _repository.DeleteEvent(objEvent);
        }

        public IList<CalendarUser> GetAllCalendarUser()
        {
            return _repository.GetAllCalendarUser();
        }

        public IList<Calendar> GetAllCalendars()
        {
            return _repository.GetAllCalendars();
        }

        public IList<Event> GetUnScheduledjoblist(int CalendarId, DateTime StartDate)
        {
            return _repository.GetUnScheduledjoblist(CalendarId,StartDate);
          
        }

        public IList<User> GetAllUsersByOrgId(int OrgId)
        {
            return _repository.GetAllUsersByOrgId(OrgId);
          
        }

        public IList<UserProductGroup> GetAllUserProductGroups()
        {
            return _repository.GetAllUserProductGroups();
        }

        public IList<Event> GetAllEventByEventID(string EventID)
        {
            return _repository.GetAllEventByEventID(EventID);
        }

        public IList<Calendar> GetCalendarsByRowId(int? RowId)
        {
            return _repository.GetCalendarsByRowId(RowId);
        }


        public IList<Event> GetAllEventByCalendarIdAndEventId(string Eventid, int CalendarId)
        {
            return _repository.GetAllEventByCalendarIdAndEventId(Eventid, CalendarId);
        }

        public IList<GetCalenderUsers> GetCalenderUsers(int? OrgId)
        {
             return _repository.GetCalenderUsers(OrgId);
        }

        public IList<GetUserProductCalendar> GetUserProductCalendar(int? OrgId)
        {
            return _repository.GetUserProductCalendar(OrgId);
        }


        public IList<GetGroupCalendarUsers> GetGroupCalendarUsers(int? OrgId, string UserIDs)
        {
            return _repository.GetGroupCalendarUsers(OrgId, UserIDs);
        }

        public IList<GetAllEventException> GetDistanctEventException()
        {
            return _repository.GetDistanctEventException();
        }

        public void UpdateJobEvents(string EventId, string CalendarName, string UpdatedByCalendarName,int OldEventRowId)
        {
            _repository.UpdateJobEvents(EventId, CalendarName, UpdatedByCalendarName,OldEventRowId);
        }

        public IList<GetEventRowId> GetEventRowId(string EventId)
        {
           return _repository.GetEventRowId(EventId);

          //  _repository
        }


        public int InsertEventLog(string Event_Id, string Event_Title, string Operation, int User_Id, string Source_calendar, string Destination_calendar)
        {
            return _repository.InsertEventLog(Event_Id, Event_Title, Operation, User_Id, Source_calendar, Destination_calendar);
        }

        public IList<GetEventEmailORPrintData> GetEventEmailORPrintData(string EventId)
        {

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (currentUser.OrgId != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    var userType = currentUser.UserType;
                    string userTypeName = currentUser.UserType1.Name;
                    int? compamyid = null;

                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    if (string.IsNullOrEmpty(EventId))
                        EventId = string.Empty;

                    var Eventlist = _repository.GetEventEmailORPrintData(orgid, userid, userTypeName, compamyid, EventId).ToList();

                    return Eventlist;
                }
            }
            return null;

         
        }

        public IList<GetEventDetails> GetEventDetails(string EventId)
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (currentUser.OrgId != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    var userType = currentUser.UserType;
                    string userTypeName = currentUser.UserType1.Name;
                    int? compamyid = null;

                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    if (string.IsNullOrEmpty(EventId))
                        EventId = string.Empty;

                    var Eventlist = _repository.GetEventDetails(orgid, userid, userTypeName, compamyid, EventId).ToList();
                    return Eventlist;
                }
            }
            return null;
        }

        public IList<SelectCommunicationTemplates> GetEventCommunicationTemplate(string TemplateType, bool? IsPrint, bool? IsEmail)
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (currentUser.OrgId != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    var userType = currentUser.UserType;
                    string userTypeName = currentUser.UserType1.Name;
                    int? compamyid = null;

                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    var Templatelist = _repository.GetCommunicationTemplate(orgid, userid, userTypeName, compamyid, TemplateType, IsPrint, IsEmail).ToList();

                    return Templatelist;
                }
            }
            return null;
        }

        //To Get Email Template for Job tracking images and Floor Plan
        public IList<GetEmailTemplateData_Result> GetEmailTemplateData(string JobId)
        {
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (currentUser.OrgId != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    var userType = currentUser.UserType;
                    string userTypeName = currentUser.UserType1.Name;

                    int? compamyid = null;

                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }          

                    var EmailTemplate = _repository.GetEmailTemplateData(orgid, userid, userTypeName, compamyid, JobId).ToList();
                    return EmailTemplate;
                }
            }
            return null;
        }


     //   IList<GetEventPrintData> GetEventPrintData(string Event_id);    



        public int InsertEventMailToEmailInbox(string To_email, string from_email, string Cc_email, string Bcc_email, string Subject, string MessageBody)
        {
        
            int identity = 0 ;
            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (currentUser.OrgId != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    var userType = currentUser.UserType;
                    string userTypeName = currentUser.UserType1.Name;
                    int? compamyid = null;

                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    identity = _repository.InsertEventMailToEmailInbox(To_email, from_email, Cc_email, Bcc_email, Subject, MessageBody, orgid, userid, userTypeName, compamyid);
                   // return null;
                    
                }
            }
            return identity;
        }

     //   int InsertEventMailToEmailInbox(string To_email, string from_email, string Cc_email, string Bcc_email, string Subject, string MessageBody, int orgid, int? userid, string userType, int? compamyid);


        public IList<string> GetEmailAddress(string SearchValue)
        {
            //SelectEmailAddressAutoComplete

            var currentUser = UserManager.Current();
            if (currentUser != null)
            {
                if (currentUser.OrgId != null)
                {
                    int orgid = currentUser.OrgId.Value;
                    var userid = currentUser.Row_Id;
                    var userType = currentUser.UserType;
                    string userTypeName = currentUser.UserType1.Name;
                    int? compamyid = null;

                    if (currentUser.UserType == 3)
                    {
                        compamyid = currentUser.CompanyId;
                    }

                    if (string.IsNullOrEmpty(SearchValue))
                        SearchValue = string.Empty;

                    var Emaillist = _repository.GetEmailAddress(orgid, userid, userTypeName, compamyid, SearchValue).ToList();
                    return Emaillist;
                }
            }
            return null;
        }


        

    }
}