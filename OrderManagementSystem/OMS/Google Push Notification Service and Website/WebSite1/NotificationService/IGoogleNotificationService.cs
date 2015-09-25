using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NotificationService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IGoogleNotificationService
    {

        [OperationContract]
        void CheckCalendarEvents(string calendarId);
        [OperationContract]
        string CreateEvent(string Title, string Location, string SalesContact, DateTime StartDate, DateTime EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId, string UserCalendar, string description, string reccurrenceRule, bool isAllDay);
        [OperationContract]
        string DeleteEvent(string eventId, string UserCalendar);
        [OperationContract]
        string DeleteEventInstance(string eventId, string UserCalendar, DateTime Start);
        [OperationContract]
        string UpdateEvent(DateTime Start, DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay,string ColorId = "");//, string[] reccurrence)
        [OperationContract]
        string UpdateEventInstance(DateTime Start, DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId = "");//, string[] reccurrence)
        [OperationContract]
        string MoveEvent(DateTime Start, DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string destinationCalendar, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay,string ColorId = "");//, string[] reccurrence);
        [OperationContract]
        void SubscribeCalendars(int orgId);
        [OperationContract]
        void CheckCalendarMovements(string calendar,int orgId);
        [OperationContract]
        void InsertGooglePushNotification(string ChannelId, string ChannelToken, DateTime ChannelExpiration, string ResourceId, string ResoourceURI, string ResourceState, long MessageNumber);
        [OperationContract]
        void InsertGooglePushNotificationWatch(int Org_Id, int OAuthId, int CalendarId, Guid ChannelId, string Type, long Expiration, string ResourceId, string ResourceURI, bool Active);
        [OperationContract]
        Dictionary<string, string> GetChannelInformation();
        [OperationContract]
        Dictionary<string, string> GetCalendarInformation();
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
