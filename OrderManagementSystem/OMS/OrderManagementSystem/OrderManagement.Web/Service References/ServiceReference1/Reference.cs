﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderManagement.Web.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IGoogleNotificationService")]
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGoogleNotificationServiceChannel : OrderManagement.Web.ServiceReference1.IGoogleNotificationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GoogleNotificationServiceClient : System.ServiceModel.ClientBase<OrderManagement.Web.ServiceReference1.IGoogleNotificationService>, OrderManagement.Web.ServiceReference1.IGoogleNotificationService {
        
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
    }
}