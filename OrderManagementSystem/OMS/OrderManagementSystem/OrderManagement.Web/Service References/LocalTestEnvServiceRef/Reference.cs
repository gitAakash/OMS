﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderManagement.Web.LocalTestEnvServiceRef {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LocalTestEnvServiceRef.IGoogleNotificationService")]
    public interface IGoogleNotificationService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CheckCalendarEvents", ReplyAction="http://tempuri.org/IGoogleNotificationService/CheckCalendarEventsResponse")]
        void CheckCalendarEvents(string calendarId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CheckCalendarEvents", ReplyAction="http://tempuri.org/IGoogleNotificationService/CheckCalendarEventsResponse")]
        System.Threading.Tasks.Task CheckCalendarEventsAsync(string calendarId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CreateEvent", ReplyAction="http://tempuri.org/IGoogleNotificationService/CreateEventResponse")]
        string CreateEvent(string Title, string Location, string SalesContact, System.DateTime StartDate, System.DateTime EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId, string UserCalendar, string description, string reccurrenceRule, bool isAllDay);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CreateEvent", ReplyAction="http://tempuri.org/IGoogleNotificationService/CreateEventResponse")]
        System.Threading.Tasks.Task<string> CreateEventAsync(string Title, string Location, string SalesContact, System.DateTime StartDate, System.DateTime EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId, string UserCalendar, string description, string reccurrenceRule, bool isAllDay);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/DeleteEvent", ReplyAction="http://tempuri.org/IGoogleNotificationService/DeleteEventResponse")]
        string DeleteEvent(string eventId, string UserCalendar);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/DeleteEvent", ReplyAction="http://tempuri.org/IGoogleNotificationService/DeleteEventResponse")]
        System.Threading.Tasks.Task<string> DeleteEventAsync(string eventId, string UserCalendar);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/DeleteEventInstance", ReplyAction="http://tempuri.org/IGoogleNotificationService/DeleteEventInstanceResponse")]
        string DeleteEventInstance(string eventId, string UserCalendar, System.DateTime Start);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/DeleteEventInstance", ReplyAction="http://tempuri.org/IGoogleNotificationService/DeleteEventInstanceResponse")]
        System.Threading.Tasks.Task<string> DeleteEventInstanceAsync(string eventId, string UserCalendar, System.DateTime Start);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/UpdateEvent", ReplyAction="http://tempuri.org/IGoogleNotificationService/UpdateEventResponse")]
        string UpdateEvent(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/UpdateEvent", ReplyAction="http://tempuri.org/IGoogleNotificationService/UpdateEventResponse")]
        System.Threading.Tasks.Task<string> UpdateEventAsync(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/UpdateEventInstance", ReplyAction="http://tempuri.org/IGoogleNotificationService/UpdateEventInstanceResponse")]
        string UpdateEventInstance(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/UpdateEventInstance", ReplyAction="http://tempuri.org/IGoogleNotificationService/UpdateEventInstanceResponse")]
        System.Threading.Tasks.Task<string> UpdateEventInstanceAsync(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/MoveEvent", ReplyAction="http://tempuri.org/IGoogleNotificationService/MoveEventResponse")]
        string MoveEvent(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string destinationCalendar, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/MoveEvent", ReplyAction="http://tempuri.org/IGoogleNotificationService/MoveEventResponse")]
        System.Threading.Tasks.Task<string> MoveEventAsync(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string destinationCalendar, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/SubscribeCalendars", ReplyAction="http://tempuri.org/IGoogleNotificationService/SubscribeCalendarsResponse")]
        void SubscribeCalendars();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/SubscribeCalendars", ReplyAction="http://tempuri.org/IGoogleNotificationService/SubscribeCalendarsResponse")]
        System.Threading.Tasks.Task SubscribeCalendarsAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CheckCalendarMovements", ReplyAction="http://tempuri.org/IGoogleNotificationService/CheckCalendarMovementsResponse")]
        void CheckCalendarMovements(string calendar);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CheckCalendarMovements", ReplyAction="http://tempuri.org/IGoogleNotificationService/CheckCalendarMovementsResponse")]
        System.Threading.Tasks.Task CheckCalendarMovementsAsync(string calendar);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/InsertGooglePushNotification", ReplyAction="http://tempuri.org/IGoogleNotificationService/InsertGooglePushNotificationRespons" +
            "e")]
        void InsertGooglePushNotification(string ChannelId, string ChannelToken, System.DateTime ChannelExpiration, string ResourceId, string ResoourceURI, string ResourceState, long MessageNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/InsertGooglePushNotification", ReplyAction="http://tempuri.org/IGoogleNotificationService/InsertGooglePushNotificationRespons" +
            "e")]
        System.Threading.Tasks.Task InsertGooglePushNotificationAsync(string ChannelId, string ChannelToken, System.DateTime ChannelExpiration, string ResourceId, string ResoourceURI, string ResourceState, long MessageNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/InsertGooglePushNotificationWatch", ReplyAction="http://tempuri.org/IGoogleNotificationService/InsertGooglePushNotificationWatchRe" +
            "sponse")]
        void InsertGooglePushNotificationWatch(int Org_Id, int OAuthId, int CalendarId, System.Guid ChannelId, string Type, long Expiration, string ResourceId, string ResourceURI, bool Active);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/InsertGooglePushNotificationWatch", ReplyAction="http://tempuri.org/IGoogleNotificationService/InsertGooglePushNotificationWatchRe" +
            "sponse")]
        System.Threading.Tasks.Task InsertGooglePushNotificationWatchAsync(int Org_Id, int OAuthId, int CalendarId, System.Guid ChannelId, string Type, long Expiration, string ResourceId, string ResourceURI, bool Active);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/GetChannelInformation", ReplyAction="http://tempuri.org/IGoogleNotificationService/GetChannelInformationResponse")]
        System.Collections.Generic.Dictionary<string, string> GetChannelInformation();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/GetChannelInformation", ReplyAction="http://tempuri.org/IGoogleNotificationService/GetChannelInformationResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string>> GetChannelInformationAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGoogleNotificationServiceChannel : OrderManagement.Web.LocalTestEnvServiceRef.IGoogleNotificationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GoogleNotificationServiceClient : System.ServiceModel.ClientBase<OrderManagement.Web.LocalTestEnvServiceRef.IGoogleNotificationService>, OrderManagement.Web.LocalTestEnvServiceRef.IGoogleNotificationService {
        
        public GoogleNotificationServiceClient() {
        }
        
        public GoogleNotificationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GoogleNotificationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GoogleNotificationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GoogleNotificationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void CheckCalendarEvents(string calendarId) {
            base.Channel.CheckCalendarEvents(calendarId);
        }
        
        public System.Threading.Tasks.Task CheckCalendarEventsAsync(string calendarId) {
            return base.Channel.CheckCalendarEventsAsync(calendarId);
        }
        
        public string CreateEvent(string Title, string Location, string SalesContact, System.DateTime StartDate, System.DateTime EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId, string UserCalendar, string description, string reccurrenceRule, bool isAllDay) {
            return base.Channel.CreateEvent(Title, Location, SalesContact, StartDate, EndDate, Comments, ProductDescription, OrderId, RequiredDate, ColorId, UserCalendar, description, reccurrenceRule, isAllDay);
        }
        
        public System.Threading.Tasks.Task<string> CreateEventAsync(string Title, string Location, string SalesContact, System.DateTime StartDate, System.DateTime EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId, string UserCalendar, string description, string reccurrenceRule, bool isAllDay) {
            return base.Channel.CreateEventAsync(Title, Location, SalesContact, StartDate, EndDate, Comments, ProductDescription, OrderId, RequiredDate, ColorId, UserCalendar, description, reccurrenceRule, isAllDay);
        }
        
        public string DeleteEvent(string eventId, string UserCalendar) {
            return base.Channel.DeleteEvent(eventId, UserCalendar);
        }
        
        public System.Threading.Tasks.Task<string> DeleteEventAsync(string eventId, string UserCalendar) {
            return base.Channel.DeleteEventAsync(eventId, UserCalendar);
        }
        
        public string DeleteEventInstance(string eventId, string UserCalendar, System.DateTime Start) {
            return base.Channel.DeleteEventInstance(eventId, UserCalendar, Start);
        }
        
        public System.Threading.Tasks.Task<string> DeleteEventInstanceAsync(string eventId, string UserCalendar, System.DateTime Start) {
            return base.Channel.DeleteEventInstanceAsync(eventId, UserCalendar, Start);
        }
        
        public string UpdateEvent(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId) {
            return base.Channel.UpdateEvent(Start, End, Creator, Organizer, Location, Title, eventId, UserCalendar, sequence, description, reccurrenceRule, isAllDay, ColorId);
        }
        
        public System.Threading.Tasks.Task<string> UpdateEventAsync(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId) {
            return base.Channel.UpdateEventAsync(Start, End, Creator, Organizer, Location, Title, eventId, UserCalendar, sequence, description, reccurrenceRule, isAllDay, ColorId);
        }
        
        public string UpdateEventInstance(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId) {
            return base.Channel.UpdateEventInstance(Start, End, Creator, Organizer, Location, Title, eventId, UserCalendar, sequence, description, reccurrenceRule, isAllDay, ColorId);
        }
        
        public System.Threading.Tasks.Task<string> UpdateEventInstanceAsync(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId) {
            return base.Channel.UpdateEventInstanceAsync(Start, End, Creator, Organizer, Location, Title, eventId, UserCalendar, sequence, description, reccurrenceRule, isAllDay, ColorId);
        }
        
        public string MoveEvent(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string destinationCalendar, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId) {
            return base.Channel.MoveEvent(Start, End, Creator, Organizer, Location, Title, eventId, destinationCalendar, UserCalendar, sequence, description, reccurrenceRule, isAllDay, ColorId);
        }
        
        public System.Threading.Tasks.Task<string> MoveEventAsync(System.DateTime Start, System.DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string destinationCalendar, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId) {
            return base.Channel.MoveEventAsync(Start, End, Creator, Organizer, Location, Title, eventId, destinationCalendar, UserCalendar, sequence, description, reccurrenceRule, isAllDay, ColorId);
        }
        
        public void SubscribeCalendars() {
            base.Channel.SubscribeCalendars();
        }
        
        public System.Threading.Tasks.Task SubscribeCalendarsAsync() {
            return base.Channel.SubscribeCalendarsAsync();
        }
        
        public void CheckCalendarMovements(string calendar) {
            base.Channel.CheckCalendarMovements(calendar);
        }
        
        public System.Threading.Tasks.Task CheckCalendarMovementsAsync(string calendar) {
            return base.Channel.CheckCalendarMovementsAsync(calendar);
        }
        
        public void InsertGooglePushNotification(string ChannelId, string ChannelToken, System.DateTime ChannelExpiration, string ResourceId, string ResoourceURI, string ResourceState, long MessageNumber) {
            base.Channel.InsertGooglePushNotification(ChannelId, ChannelToken, ChannelExpiration, ResourceId, ResoourceURI, ResourceState, MessageNumber);
        }
        
        public System.Threading.Tasks.Task InsertGooglePushNotificationAsync(string ChannelId, string ChannelToken, System.DateTime ChannelExpiration, string ResourceId, string ResoourceURI, string ResourceState, long MessageNumber) {
            return base.Channel.InsertGooglePushNotificationAsync(ChannelId, ChannelToken, ChannelExpiration, ResourceId, ResoourceURI, ResourceState, MessageNumber);
        }
        
        public void InsertGooglePushNotificationWatch(int Org_Id, int OAuthId, int CalendarId, System.Guid ChannelId, string Type, long Expiration, string ResourceId, string ResourceURI, bool Active) {
            base.Channel.InsertGooglePushNotificationWatch(Org_Id, OAuthId, CalendarId, ChannelId, Type, Expiration, ResourceId, ResourceURI, Active);
        }
        
        public System.Threading.Tasks.Task InsertGooglePushNotificationWatchAsync(int Org_Id, int OAuthId, int CalendarId, System.Guid ChannelId, string Type, long Expiration, string ResourceId, string ResourceURI, bool Active) {
            return base.Channel.InsertGooglePushNotificationWatchAsync(Org_Id, OAuthId, CalendarId, ChannelId, Type, Expiration, ResourceId, ResourceURI, Active);
        }
        
        public System.Collections.Generic.Dictionary<string, string> GetChannelInformation() {
            return base.Channel.GetChannelInformation();
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string>> GetChannelInformationAsync() {
            return base.Channel.GetChannelInformationAsync();
        }
    }
}