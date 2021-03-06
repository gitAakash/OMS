﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CT_DPIService.GoogleService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Coordinates", Namespace="http://schemas.datacontract.org/2004/07/NotificationService")]
    [System.SerializableAttribute()]
    public partial class Coordinates : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LatitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LongitudeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Latitude {
            get {
                return this.LatitudeField;
            }
            set {
                if ((object.ReferenceEquals(this.LatitudeField, value) != true)) {
                    this.LatitudeField = value;
                    this.RaisePropertyChanged("Latitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Longitude {
            get {
                return this.LongitudeField;
            }
            set {
                if ((object.ReferenceEquals(this.LongitudeField, value) != true)) {
                    this.LongitudeField = value;
                    this.RaisePropertyChanged("Longitude");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GoogleService.IGoogleNotificationService")]
    public interface IGoogleNotificationService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CheckAllCalendarEvents", ReplyAction="http://tempuri.org/IGoogleNotificationService/CheckAllCalendarEventsResponse")]
        void CheckAllCalendarEvents(int orgId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CheckAllCalendarEvents", ReplyAction="http://tempuri.org/IGoogleNotificationService/CheckAllCalendarEventsResponse")]
        System.Threading.Tasks.Task CheckAllCalendarEventsAsync(int orgId);
        
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
        void SubscribeCalendars(int orgId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/SubscribeCalendars", ReplyAction="http://tempuri.org/IGoogleNotificationService/SubscribeCalendarsResponse")]
        System.Threading.Tasks.Task SubscribeCalendarsAsync(int orgId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CheckCalendarMovements", ReplyAction="http://tempuri.org/IGoogleNotificationService/CheckCalendarMovementsResponse")]
        void CheckCalendarMovements(string calendar, int orgId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/CheckCalendarMovements", ReplyAction="http://tempuri.org/IGoogleNotificationService/CheckCalendarMovementsResponse")]
        System.Threading.Tasks.Task CheckCalendarMovementsAsync(string calendar, int orgId);
        
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
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/GetCalendarInformation", ReplyAction="http://tempuri.org/IGoogleNotificationService/GetCalendarInformationResponse")]
        System.Collections.Generic.Dictionary<string, string> GetCalendarInformation();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/GetCalendarInformation", ReplyAction="http://tempuri.org/IGoogleNotificationService/GetCalendarInformationResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string>> GetCalendarInformationAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/GetCoordinates", ReplyAction="http://tempuri.org/IGoogleNotificationService/GetCoordinatesResponse")]
        CT_DPIService.GoogleService.Coordinates GetCoordinates(string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/GetCoordinates", ReplyAction="http://tempuri.org/IGoogleNotificationService/GetCoordinatesResponse")]
        System.Threading.Tasks.Task<CT_DPIService.GoogleService.Coordinates> GetCoordinatesAsync(string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/SyncGoogleCalendars", ReplyAction="http://tempuri.org/IGoogleNotificationService/SyncGoogleCalendarsResponse")]
        void SyncGoogleCalendars(int orgId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/SyncGoogleCalendars", ReplyAction="http://tempuri.org/IGoogleNotificationService/SyncGoogleCalendarsResponse")]
        System.Threading.Tasks.Task SyncGoogleCalendarsAsync(int orgId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/AddWildCardDomain", ReplyAction="http://tempuri.org/IGoogleNotificationService/AddWildCardDomainResponse")]
        void AddWildCardDomain(string website, string port, string newDomain, string websiteName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/AddWildCardDomain", ReplyAction="http://tempuri.org/IGoogleNotificationService/AddWildCardDomainResponse")]
        System.Threading.Tasks.Task AddWildCardDomainAsync(string website, string port, string newDomain, string websiteName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/GetCoordinatesUsingGoogle", ReplyAction="http://tempuri.org/IGoogleNotificationService/GetCoordinatesUsingGoogleResponse")]
        CT_DPIService.GoogleService.Coordinates GetCoordinatesUsingGoogle(string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGoogleNotificationService/GetCoordinatesUsingGoogle", ReplyAction="http://tempuri.org/IGoogleNotificationService/GetCoordinatesUsingGoogleResponse")]
        System.Threading.Tasks.Task<CT_DPIService.GoogleService.Coordinates> GetCoordinatesUsingGoogleAsync(string location);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGoogleNotificationServiceChannel : CT_DPIService.GoogleService.IGoogleNotificationService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GoogleNotificationServiceClient : System.ServiceModel.ClientBase<CT_DPIService.GoogleService.IGoogleNotificationService>, CT_DPIService.GoogleService.IGoogleNotificationService {
        
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
        
        public void CheckAllCalendarEvents(int orgId) {
            base.Channel.CheckAllCalendarEvents(orgId);
        }
        
        public System.Threading.Tasks.Task CheckAllCalendarEventsAsync(int orgId) {
            return base.Channel.CheckAllCalendarEventsAsync(orgId);
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
        
        public void SubscribeCalendars(int orgId) {
            base.Channel.SubscribeCalendars(orgId);
        }
        
        public System.Threading.Tasks.Task SubscribeCalendarsAsync(int orgId) {
            return base.Channel.SubscribeCalendarsAsync(orgId);
        }
        
        public void CheckCalendarMovements(string calendar, int orgId) {
            base.Channel.CheckCalendarMovements(calendar, orgId);
        }
        
        public System.Threading.Tasks.Task CheckCalendarMovementsAsync(string calendar, int orgId) {
            return base.Channel.CheckCalendarMovementsAsync(calendar, orgId);
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
        
        public System.Collections.Generic.Dictionary<string, string> GetCalendarInformation() {
            return base.Channel.GetCalendarInformation();
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, string>> GetCalendarInformationAsync() {
            return base.Channel.GetCalendarInformationAsync();
        }
        
        public CT_DPIService.GoogleService.Coordinates GetCoordinates(string location) {
            return base.Channel.GetCoordinates(location);
        }
        
        public System.Threading.Tasks.Task<CT_DPIService.GoogleService.Coordinates> GetCoordinatesAsync(string location) {
            return base.Channel.GetCoordinatesAsync(location);
        }
        
        public void SyncGoogleCalendars(int orgId) {
            base.Channel.SyncGoogleCalendars(orgId);
        }
        
        public System.Threading.Tasks.Task SyncGoogleCalendarsAsync(int orgId) {
            return base.Channel.SyncGoogleCalendarsAsync(orgId);
        }
        
        public void AddWildCardDomain(string website, string port, string newDomain, string websiteName) {
            base.Channel.AddWildCardDomain(website, port, newDomain, websiteName);
        }
        
        public System.Threading.Tasks.Task AddWildCardDomainAsync(string website, string port, string newDomain, string websiteName) {
            return base.Channel.AddWildCardDomainAsync(website, port, newDomain, websiteName);
        }
        
        public CT_DPIService.GoogleService.Coordinates GetCoordinatesUsingGoogle(string location) {
            return base.Channel.GetCoordinatesUsingGoogle(location);
        }
        
        public System.Threading.Tasks.Task<CT_DPIService.GoogleService.Coordinates> GetCoordinatesUsingGoogleAsync(string location) {
            return base.Channel.GetCoordinatesUsingGoogleAsync(location);
        }
    }
}
