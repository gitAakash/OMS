using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Google.Apis.Calendar;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Management;
using Google.Apis.Util.Store;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Globalization;
using System.Web.Script.Serialization;

namespace NotificationService
{
    
    public class GoogleNotificationService : IGoogleNotificationService
    {
        public delegate void CanlendarHandler();
        public event CanlendarHandler UpdateCalendar;

        //' Calendar scopes which is initialized on the main method.

        static IList<string> scopes = new List<string>();
        //' Calendar service.

        static CalendarService service;
        public string userName = "";

        public string userPassword = "";
        //' Here's the feed to access events on the users' private/primar calendar:
        private string feedUri = "https://www.google.com/calendar/feeds/default/private/full?max-results=9999";
        //' Here's the feed that lists all calendars that this user has access to:
        private const string feedOwnCalendars = "https://www.google.com/calendar/feeds/default/owncalendars/full";
        //' This is a feed for non-primary calendars that requires an ID field, taken from enumerating the users' calendars...

        private const string CALENDAR_TEMPLATE = "https://www.google.com/calendar/feeds/{0}/private/full?max-results=9999";


        public int CheckedCalendarEvent = 0;

        public void SetCalendar()
        {
            try
            {
                string CTFolderPath = @"D:\Websites\dev.zerofootprint.com.au\GoogleService\CT";


                DirectoryInfo directory = new DirectoryInfo(CTFolderPath);
                string filename = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault().FullName;
                string jsonText = File.ReadAllText(filename);
                Dictionary<string, object> tokenOjbect = (Dictionary<string, object>)new JavaScriptSerializer().DeserializeObject(jsonText);
                string refreshToken = tokenOjbect["refresh_token"].ToString();


                scopes.Add(CalendarService.Scope.Calendar);
                UserCredential credential = default(UserCredential);
                FileDataStore _fdsToken = new FileDataStore(CTFolderPath);
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                  new ClientSecrets
                  {
                      ClientId = "532982290458-u189keaaki44sskvtki6p28rq5crjl6d.apps.googleusercontent.com",
                      ClientSecret = "DDO62gcKn5nYWm4XCXlJkngo",
                  }, scopes,
                  "Z",
                CancellationToken.None, _fdsToken).Result;
                credential.Token.ExpiresInSeconds = 500000;
                String token = credential.Token.RefreshToken;

                
                credential.Token.RefreshToken = refreshToken;
                
                //credential.Token.ExpiresInSeconds = 500000;
                // Create the calendar service using an initializer instance
                BaseClientService.Initializer initializer = new BaseClientService.Initializer();
                initializer.HttpClientInitializer = credential;
                initializer.ApplicationName = "DPI Google Calendar";
                service = new CalendarService(initializer);
            }
            catch (Exception ex)
            {
                string text = File.ReadAllText("C://Google.txt");
                text = text + Environment.NewLine + ex.Message;
                File.WriteAllText("C://Google.txt", text);
            }
        }

    //    }
    //    public void CheckCalendarEvents(string calendarId)
    //    {
    //        SetCalendar();
    //        int count = 0;
    //        var list = service.CalendarList.List().Execute().Items.ToList();//.Where(x => x.Id == calendarId).ToList();
    //        try
    //        {
    //            foreach (var cal in list)
    //            {
    //                var itemlistreq = service.Events.List(cal.Id);
    //                // itemlistreq.PageToken = token;
    //                itemlistreq.MaxResults = 100000;
    //                itemlistreq.ShowDeleted = true;
    //                itemlistreq.ShowHiddenInvitations = true;
    //                var itemlistex = itemlistreq.Execute();

    //                //itemlistex.Items[0].
    //                var token = itemlistex.NextPageToken;
    //                string[] Descs = null;
    //                count = itemlistex.Items.Count;
    //                for (int i = 0; i < itemlistex.Items.Count; i++)
    //                {
    //                    string Calendar = cal.Id;
    //                    string EventId = itemlistex.Items[i].Id;
    //                    string RecurringEventId = itemlistex.Items[i].RecurringEventId;
    //                    if (RecurringEventId == null) { RecurringEventId = ""; };
    //                    var Attendees = itemlistex.Items[i].Attendees;
    //                    string Attendes = "";
    //                    if (Attendees != null)
    //                    {

    //                        for (int j = 0; j < Attendees.Count; j++)
    //                        { Attendes = Attendes + "~" + Attendees[j]; };
    //                    }
    //                    string ColorId = itemlistex.Items[i].ColorId;
    //                    if (ColorId == null) { ColorId = ""; };
    //                    bool? AttendeesOmitted = itemlistex.Items[i].AttendeesOmitted;
    //                    if (AttendeesOmitted == null) { AttendeesOmitted = false; };
    //                    bool? AnyoneCanAddSelf = itemlistex.Items[i].AnyoneCanAddSelf;
    //                    if (AnyoneCanAddSelf == null) { AnyoneCanAddSelf = false; };
    //                    string Title = itemlistex.Items[i].Summary;
    //                    if (Title == null) { Title = ""; }
    //                    string Location = itemlistex.Items[i].Location;
    //                    if (Location == null) { Location = ""; }
    //                    string Description = itemlistex.Items[i].Description;
    //                    if (Description == null) { Description = ""; }
    //                    string Kind = itemlistex.Items[i].Kind;
    //                    if (Kind == null) { Kind = ""; }
    //                    DateTime StartDate = Convert.ToDateTime("01/01/1900");
    //                    if (itemlistex.Items[i].Start != null)
    //                    { StartDate = Convert.ToDateTime(itemlistex.Items[i].Start.DateTime); }
    //                    if (StartDate != null && StartDate.ToString().Contains("0001")) { StartDate = Convert.ToDateTime("01/01/1900"); }
    //                    DateTime EndDate = Convert.ToDateTime("01/01/1900");
    //                    if (itemlistex.Items[i].End != null)
    //                    { EndDate = Convert.ToDateTime(itemlistex.Items[i].End.DateTime); }
    //                    if (EndDate != null && EndDate.ToString().Contains("0001")) { EndDate = Convert.ToDateTime("01/01/1900"); }

    //                    DateTime MovedOn = Convert.ToDateTime("01/01/1900");
    //                    if (itemlistex.Items[i].Created != null)
    //                    { MovedOn = Convert.ToDateTime(itemlistex.Items[i].Created); }
    //                    if (MovedOn != null && MovedOn.ToString().Contains("0001")) { MovedOn = Convert.ToDateTime("01/01/1900"); }
    //                    string Status = itemlistex.Items[i].Status;
    //                    var Recurrence = itemlistex.Items[i].Recurrence;
    //                    string Recurrences = "";
    //                    if (Recurrence != null)
    //                    {

    //                        for (int j = 0; j < Recurrence.Count; j++)
    //                        { Recurrences = Recurrences + "~" + Recurrence[j]; };
    //                    }
    //                    string Organizer = "";
    //                    if (itemlistex.Items[i].Organizer != null)
    //                    {
    //                        Organizer = itemlistex.Items[i].Organizer.DisplayName;
    //                    }
    //                    if (Organizer == null) { Organizer = ""; }
    //                    string Creator = "";
    //                    if (itemlistex.Items[i].Creator != null)
    //                    {
    //                        Creator = itemlistex.Items[i].Creator.DisplayName;
    //                    }
    //                    if (Creator == null) { Creator = ""; }
    //                    bool? Locked = false;
    //                    try
    //                    {
    //                        Locked = itemlistex.Items[i].Locked;
    //                    }
    //                    catch { };
    //                    Locked = false;
    //                    //if (Locked == null) { Locked = false; };
    //                    int Sequence = 0;
    //                    if (itemlistex.Items[i].Sequence != null)
    //                    { Sequence = (int)itemlistex.Items[i].Sequence; }
    //                    string Transperancy = itemlistex.Items[i].Transparency;
    //                    if (Transperancy == null) { Transperancy = ""; }
    //                    DateTime Updated = Convert.ToDateTime("01/01/1900");
    //                    if (itemlistex.Items[i].Updated != null)
    //                    { Updated = Convert.ToDateTime(itemlistex.Items[i].Updated); }
    //                    if (Updated != null && Updated.ToString().Contains("0001")) { Updated = Convert.ToDateTime("01/01/1900"); }
    //                    string Visibility = itemlistex.Items[i].Visibility;
    //                    if (Visibility == null) { Visibility = ""; }
    //                    //InsertOtherEvents(Calendar, EventId, RecurringEventId, ColorId, Attendes, AttendeesOmitted, AnyoneCanAddSelf, Locked, Title, Location, Description, Kind, StartDate, EndDate, MovedOn, Status, Recurrences, Organizer, Creator, Sequence, Transperancy, Updated, Visibility);
    //                    File.WriteAllText("C://test.txt", count.ToString());

    //                }


    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Email.Email eml = new Email.Email();
    //            string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
    //            eml.SendMail("Error syncing google calendar", "Error :- " + strLog, "sachin@zerofootprint.com.au", "", false);


    //        };

    //    }
    //    private void InsertOtherEvents(string Calendar, string EventId, string RecurringEventId, string ColorId, string Attendees, bool? AttendeesOmitted, bool? AnyoneCanAddSelf, bool? Locked, string Title, string Location, string Description, string Kind, DateTime StartDate, DateTime EndDate, DateTime MovedOn, string Status, string Recurrence, string Organizer, string Creator, int Sequence, string Transperancy, DateTime Updated, string Visibility)
    //    {
    //        try
    //        {
    //            string connectionString = null;
    //            connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
    //            using (SqlConnection con = new SqlConnection(connectionString))
    //            {
    //                using (SqlCommand cmd = new SqlCommand("InsertOtherEvents", con))
    //                {
    //                    cmd.CommandType = CommandType.StoredProcedure;
    //                    cmd.Parameters.Add("@Calendar", SqlDbType.VarChar).Value = Calendar;
    //                    cmd.Parameters.Add("@EventId", SqlDbType.VarChar).Value = EventId;
    //                    cmd.Parameters.Add("@RecurringEventId", SqlDbType.VarChar).Value = RecurringEventId;
    //                    cmd.Parameters.Add("@ColorId", SqlDbType.VarChar).Value = ColorId;
    //                    cmd.Parameters.Add("@Attendees", SqlDbType.VarChar).Value = Attendees;
    //                    cmd.Parameters.Add("@AttendeesOmitted", SqlDbType.Bit).Value = AttendeesOmitted;
    //                    cmd.Parameters.Add("@AnyoneCanAddSelf", SqlDbType.Bit).Value = AnyoneCanAddSelf;
    //                    cmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = Locked;
    //                    cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = Title;
    //                    cmd.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
    //                    cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
    //                    cmd.Parameters.Add("@Kind", SqlDbType.VarChar).Value = Kind;
    //                    cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
    //                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
    //                    cmd.Parameters.Add("@MovedOn", SqlDbType.Date).Value = MovedOn;
    //                    cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;
    //                    cmd.Parameters.Add("@Recurrence", SqlDbType.VarChar).Value = Recurrence;
    //                    cmd.Parameters.Add("@Organizer", SqlDbType.VarChar).Value = Organizer;
    //                    cmd.Parameters.Add("@Creator", SqlDbType.VarChar).Value = Creator;
    //                    cmd.Parameters.Add("@Sequence", SqlDbType.Int).Value = Sequence;
    //                    cmd.Parameters.Add("@Transperancy", SqlDbType.VarChar).Value = Transperancy;
    //                    cmd.Parameters.Add("@Updated", SqlDbType.DateTime).Value = Updated;
    //                    cmd.Parameters.Add("@Visibility", SqlDbType.VarChar).Value = Visibility;

    //                    con.Open();
    //                    cmd.ExecuteNonQuery();
    //                    con.Close();
    //                }
    //            }
    //        }
    //        catch (Exception ex) { throw ex; }

    //    }
    //    private void InsertCalendarEvents(string Calendar, string OrderId, string Title, DateTime StartDate, DateTime EndDate, DateTime MovedOn, string Status)
    //    {
    //        try
    //        {
    //            string connectionString = null;
    //            connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
    //            using (SqlConnection con = new SqlConnection(connectionString))
    //            {
    //                using (SqlCommand cmd = new SqlCommand("InsertEvents", con))
    //                {
    //                    cmd.CommandType = CommandType.StoredProcedure;
    //                    cmd.Parameters.Add("@Calendar", SqlDbType.VarChar).Value = Calendar;
    //                    cmd.Parameters.Add("@OrderId", SqlDbType.VarChar).Value = OrderId;
    //                    cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = Title;
    //                    cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
    //                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
    //                    cmd.Parameters.Add("@MovedOn", SqlDbType.DateTime).Value = MovedOn;
    //                    cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;
    //                    con.Open();
    //                    cmd.ExecuteNonQuery();
    //                    con.Close();
    //                }
    //            }
    //        }
    //        catch (Exception ex) { throw ex; }

    //    }

        //' Calendar scopes which is initialized on the main method.

        private static void DisplayList(IList<CalendarListEntry> list)
        {
            Console.WriteLine("Lists of calendars:");
            foreach (CalendarListEntry item in list)
            {
                Console.WriteLine(item.Summary + ". Location: " + item.Location + ", TimeZone: " + item.TimeZone);
            }
        }

        /// <summary>Displays the calendar's events.</summary>
        private static void DisplayFirstCalendarEvents(CalendarListEntry list)
        {
            Console.WriteLine(Environment.NewLine + "Maximum 5 first events from {0}:", list.Summary);
            Google.Apis.Calendar.v3.EventsResource.ListRequest requeust = service.Events.List(list.Id);
            // Set MaxResults and TimeMin with sample values
            requeust.MaxResults = 5;
            requeust.TimeMin = new DateTime(2013, 10, 1, 20, 0, 0);
            // Fetch the list of events
            foreach (Google.Apis.Calendar.v3.Data.Event calendarEvent in requeust.Execute().Items)
            {
                string startDate = "Unspecified";
                if (((calendarEvent.Start != null)))
                {
                    if (((calendarEvent.Start.Date != null)))
                    {
                        startDate = calendarEvent.Start.Date.ToString();
                    }
                }

                Console.WriteLine(calendarEvent.Summary + ". Start at: " + startDate);
            }
        }


        public string CreateEvent(string Title, string Location, string SalesContact, DateTime StartDate, DateTime EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId, string UserCalendar,string description, string reccurrenceRule,bool isAllDay)
        {
            string eventId = string.Empty;
            try
            {
                string text = File.ReadAllText("C://Google.txt");
                text = text + Environment.NewLine + "Set Calendar Called";
                File.WriteAllText("C://Google.txt", text);
                SetCalendar();
                text = text + Environment.NewLine + "Set Calendar Passed";
                File.WriteAllText("C://Google.txt", text);
                Event evnt = new Event();
                if (ColorId == "")
                { evnt.ColorId = "5"; }
                if (ColorId != "")
                { evnt.ColorId = ColorId; }

                if (isAllDay)
                {
                    evnt.Start = new EventDateTime
                    {

                        Date = StartDate.ToString("yyyy-MM-dd"),
                        TimeZone = "Etc/UTC"
                    };
                    evnt.End = new EventDateTime
                    {
                        Date = EndDate.ToString("yyyy-MM-dd"),
                        TimeZone = "Etc/UTC"
                    };
                    evnt.Transparency = "transparent";
                }
                else
                {
                    //string startFormat = StartDate;
                    evnt.Start = new EventDateTime
                    {
                        
                        DateTime = StartDate,
                        TimeZone = "Etc/UTC"
                    };
                    
                    evnt.End = new EventDateTime
                    {
                        DateTime = EndDate,
                        TimeZone = "Etc/UTC"
                    };
                }



                evnt.Description = description;// "SalesContact : " + SalesContact + Environment.NewLine + Environment.NewLine + "Required On :" + Environment.NewLine + Environment.NewLine + "Product Description :" + ProductDescription + Environment.NewLine + Environment.NewLine + "Supplier Instructions :" + Comments.Replace("Supplier Instructions", "") + Environment.NewLine + Environment.NewLine + "Order Item (Please donot delete or update OrderItemId) :" + OrderId;
                evnt.Summary = Title;
                evnt.Location = Location;

                if (reccurrenceRule.Trim() != string.Empty)
                {
                    string recc = "RRULE:" + reccurrenceRule;// +"\r\n";//"DTSTART;VALUE=DATE:" + DateTime.Parse(StartDate).ToString("yyyyMMdd").Replace("-","") + "\r\n" + "DTEND;VALUE=DATE:" + DateTime.Parse(EndDate).ToString("yyyyMMdd").Replace("-", "") + "\r\n" +
                    List<string> reccurrence = new List<string>();
                    reccurrence.Add(recc);
                    evnt.Recurrence = reccurrence;
                    //evnt.RecurringEventId = "1234";
                    //evnt.OriginalStartTime = evnt.Start;
                }

                var returnedEventDetailes = service.Events.Insert(evnt, UserCalendar).Execute();
                eventId = returnedEventDetailes.Id;
                InsertReturnedEventDetailsInDB(UserCalendar, returnedEventDetailes);
                File.WriteAllText("C://Google.txt", "Event added");
                //service.Events.Insert(evnt, "christopher.a.kerr@gmail.com").Execute();
                //  service.Events.Insert(evnt, "sac.nan@gmail.com").Execute();

            }
            catch (Exception ex)
            {
                File.AppendAllText("C:\\Google.txt",ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
            return eventId;            

        }

        private void InsertReturnedEventDetailsInDB(string UserCalendar, Event returnedEventDetailes)
        {
            string Calendar = UserCalendar;
            string EventId = returnedEventDetailes.Id;
            string RecurringEventId = returnedEventDetailes.RecurringEventId;
            if (RecurringEventId == null) { RecurringEventId = ""; };
            var Attendees = returnedEventDetailes.Attendees;
            string Attendes = "";
            if (Attendees != null)
            {

                for (int j = 0; j < Attendees.Count; j++)
                { Attendes = Attendes + "~" + Attendees[j]; };
            }
            string ColorId = returnedEventDetailes.ColorId;
            if (ColorId == null) { ColorId = ""; };
            bool? AttendeesOmitted = returnedEventDetailes.AttendeesOmitted;
            if (AttendeesOmitted == null) { AttendeesOmitted = false; };
            bool? AnyoneCanAddSelf = returnedEventDetailes.AnyoneCanAddSelf;
            if (AnyoneCanAddSelf == null) { AnyoneCanAddSelf = false; };
            string Title = returnedEventDetailes.Summary;
            if (Title == null) { Title = ""; }
            string Location = returnedEventDetailes.Location;
            if (Location == null) { Location = ""; }
            string Description = returnedEventDetailes.Description;
            if (Description == null) { Description = ""; }
            string Kind = returnedEventDetailes.Kind;
            if (Kind == null) { Kind = ""; }
            DateTime StartDate = Convert.ToDateTime("01/01/1900");
            if (returnedEventDetailes.Start != null)
            { StartDate = Convert.ToDateTime(returnedEventDetailes.Start.DateTime); }
            if (StartDate != null && StartDate.ToString().Contains("0001")) { StartDate = Convert.ToDateTime("01/01/1900"); }
            DateTime EndDate = Convert.ToDateTime("01/01/1900");
            if (returnedEventDetailes.End != null)
            { EndDate = Convert.ToDateTime(returnedEventDetailes.End.DateTime); }
            if (EndDate != null && EndDate.ToString().Contains("0001")) { EndDate = Convert.ToDateTime("01/01/1900"); }

            DateTime MovedOn = Convert.ToDateTime("01/01/1900");
            if (returnedEventDetailes.Created != null)
            { MovedOn = Convert.ToDateTime(returnedEventDetailes.Created); }
            if (MovedOn != null && MovedOn.ToString().Contains("0001")) { MovedOn = Convert.ToDateTime("01/01/1900"); }
            string Status = returnedEventDetailes.Status;
            var Recurrence = returnedEventDetailes.Recurrence;
            string Recurrences = "";
            if (Recurrence != null)
            {

                for (int j = 0; j < Recurrence.Count; j++)
                { Recurrences = Recurrences + "~" + Recurrence[j]; };
            }
            string Organizer = "";
            if (returnedEventDetailes.Organizer != null)
            {
                Organizer = returnedEventDetailes.Organizer.DisplayName;
            }
            if (Organizer == null) { Organizer = ""; }
            string Creator = "";
            if (returnedEventDetailes.Creator != null)
            {
                Creator = returnedEventDetailes.Creator.DisplayName;
            }
            if (Creator == null) { Creator = ""; }
            bool? Locked = false;
            try
            {
                Locked = returnedEventDetailes.Locked;
            }
            catch { };
            Locked = false;
            //if (Locked == null) { Locked = false; };
            int Sequence = 0;
            if (returnedEventDetailes.Sequence != null)
            { Sequence = (int)returnedEventDetailes.Sequence; }
            string Transperancy = returnedEventDetailes.Transparency;
            if (Transperancy == null) { Transperancy = ""; }
            DateTime Updated = Convert.ToDateTime("01/01/1900");
            if (returnedEventDetailes.Updated != null)
            { Updated = Convert.ToDateTime(returnedEventDetailes.Updated); }
            if (Updated != null && Updated.ToString().Contains("0001")) { Updated = Convert.ToDateTime("01/01/1900"); }
            string Visibility = returnedEventDetailes.Visibility;
            if (Visibility == null) { Visibility = ""; }
            InsertOtherEvents(Calendar, EventId, RecurringEventId, ColorId, Attendes, AttendeesOmitted, AnyoneCanAddSelf, Locked, Title, Location, Description, Kind, StartDate, EndDate, MovedOn, Status, Recurrences, Organizer, Creator, Sequence, Transperancy, Updated, Visibility);
        }

        public void CheckCalendarMovements()
        {
            SetCalendar();
            CheckCalendarEvents();

            string a;
            var list = service.CalendarList.List().Execute().Items;
            try
            {

                foreach (var cal in list)
                {
                    // if (cal.Id != "campaigntrack.dpi@gmail.com") { continue; }
                    InsertCalendars(cal.Id, 1);
                    var itemlistreq = service.Events.List(cal.Id);
                    // itemlistreq.PageToken = token;
                    itemlistreq.MaxResults = 100000;
                    itemlistreq.ShowDeleted = true;
                    itemlistreq.ShowHiddenInvitations = true;
                    var itemlistex = itemlistreq.Execute();

                    //itemlistex.Items[0].
                    var token = itemlistex.NextPageToken;
                    for (int i = 0; i < itemlistex.Items.Count; i++)
                    {
                        if (itemlistex.Items[i].Description != null)
                        {

                            if (Convert.ToDateTime(itemlistex.Items[i].Created).Date == DateTime.Now.Date || Convert.ToDateTime(itemlistex.Items[i].Created).Date == DateTime.Now.Date.AddDays(-1) || Convert.ToDateTime(itemlistex.Items[i].Created).Date == DateTime.Now.Date.AddDays(-2))
                            {
                                if (itemlistex.Items[i].Description.Contains("Order Item (Please donot delete or update OrderItemId) :") == true)
                                {
                                    string Desc = itemlistex.Items[i].Description;
                                    string[] Descs = Desc.Split(new string[] { "Order Item (Please donot delete or update OrderItemId) :" }, StringSplitOptions.None);
                                    string OrderItemId = Descs[1];
                                    string Title = itemlistex.Items[i].Summary;
                                    string Calendar = cal.Id;
                                    DateTime StartDate = Convert.ToDateTime(itemlistex.Items[i].Start.DateTime);
                                    DateTime EndDate = Convert.ToDateTime(itemlistex.Items[i].End.DateTime);
                                    DateTime MovedOn = Convert.ToDateTime(itemlistex.Items[i].Created);
                                    string Status = itemlistex.Items[i].Status;
                                    //InsertCalendarEvents(Calendar, OrderItemId, Title, StartDate, EndDate, MovedOn, Status);

                                }

                            }
                        }
                    }

                    //Here, itemlistreq.PageToken == null;
                }
                UpdateCalendar();
            }
            catch (Exception ex)
            {
                Email.Email eml = new Email.Email();
                string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                eml.SendMail("Error syncing google calendar", "Error :- " + strLog, "sachin@zerofootprint.com.au", "", false);


            };

        }

        public void CheckCalendarEvents()
        {

            var list = service.CalendarList.List().Execute().Items;
            try
            {

                foreach (var cal in list)
                {
                    var itemlistreq = service.Events.List(cal.Id);
                    // itemlistreq.PageToken = token;
                    itemlistreq.MaxResults = 100000;
                    itemlistreq.ShowDeleted = true;
                    itemlistreq.ShowHiddenInvitations = true;
                    var itemlistex = itemlistreq.Execute();

                    //itemlistex.Items[0].
                    var token = itemlistex.NextPageToken;
                    string[] Descs = null;
                    for (int i = 0; i < itemlistex.Items.Count; i++)
                    {
                        string Calendar = cal.Id;
                        string EventId = itemlistex.Items[i].Id;
                        string RecurringEventId = itemlistex.Items[i].RecurringEventId;
                        if (RecurringEventId == null) { RecurringEventId = ""; };
                        var Attendees = itemlistex.Items[i].Attendees;
                        string Attendes = "";
                        if (Attendees != null)
                        {

                            for (int j = 0; j < Attendees.Count; j++)
                            { Attendes = Attendes + "~" + Attendees[j]; };
                        }
                        string ColorId = itemlistex.Items[i].ColorId;
                        if (ColorId == null) { ColorId = ""; };
                        bool? AttendeesOmitted = itemlistex.Items[i].AttendeesOmitted;
                        if (AttendeesOmitted == null) { AttendeesOmitted = false; };
                        bool? AnyoneCanAddSelf = itemlistex.Items[i].AnyoneCanAddSelf;
                        if (AnyoneCanAddSelf == null) { AnyoneCanAddSelf = false; };
                        string Title = itemlistex.Items[i].Summary;
                        if (Title == null) { Title = ""; }
                        string Location = itemlistex.Items[i].Location;
                        if (Location == null) { Location = ""; }
                        string Description = itemlistex.Items[i].Description;
                        if (Description == null) { Description = ""; }
                        string Kind = itemlistex.Items[i].Kind;
                        if (Kind == null) { Kind = ""; }
                        DateTime StartDate = Convert.ToDateTime("01/01/1900");
                        if (itemlistex.Items[i].Start != null)
                        { StartDate = Convert.ToDateTime(itemlistex.Items[i].Start.DateTime); }
                        if (StartDate != null && StartDate.ToString().Contains("0001")) { StartDate = Convert.ToDateTime("01/01/1900"); }
                        DateTime EndDate = Convert.ToDateTime("01/01/1900");
                        if (itemlistex.Items[i].End != null)
                        { EndDate = Convert.ToDateTime(itemlistex.Items[i].End.DateTime); }
                        if (EndDate != null && EndDate.ToString().Contains("0001")) { EndDate = Convert.ToDateTime("01/01/1900"); }

                        DateTime MovedOn = Convert.ToDateTime("01/01/1900");
                        if (itemlistex.Items[i].Created != null)
                        { MovedOn = Convert.ToDateTime(itemlistex.Items[i].Created); }
                        if (MovedOn != null && MovedOn.ToString().Contains("0001")) { MovedOn = Convert.ToDateTime("01/01/1900"); }
                        string Status = itemlistex.Items[i].Status;
                        var Recurrence = itemlistex.Items[i].Recurrence;
                        string Recurrences = "";
                        if (Recurrence != null)
                        {

                            for (int j = 0; j < Recurrence.Count; j++)
                            { Recurrences = Recurrences + "~" + Recurrence[j]; };
                        }
                        string Organizer = "";
                        if (itemlistex.Items[i].Organizer != null)
                        {
                            Organizer = itemlistex.Items[i].Organizer.DisplayName;
                        }
                        if (Organizer == null) { Organizer = ""; }
                        string Creator = "";
                        if (itemlistex.Items[i].Creator != null)
                        {
                            Creator = itemlistex.Items[i].Creator.DisplayName;
                        }
                        if (Creator == null) { Creator = ""; }
                        bool? Locked = false;
                        try
                        {
                            Locked = itemlistex.Items[i].Locked;
                        }
                        catch { };
                        Locked = false;
                        //if (Locked == null) { Locked = false; };
                        int Sequence = 0;
                        if (itemlistex.Items[i].Sequence != null)
                        { Sequence = (int)itemlistex.Items[i].Sequence; }
                        string Transperancy = itemlistex.Items[i].Transparency;
                        if (Transperancy == null) { Transperancy = ""; }
                        DateTime Updated = Convert.ToDateTime("01/01/1900");
                        if (itemlistex.Items[i].Updated != null)
                        { Updated = Convert.ToDateTime(itemlistex.Items[i].Updated); }
                        if (Updated != null && Updated.ToString().Contains("0001")) { Updated = Convert.ToDateTime("01/01/1900"); }
                        string Visibility = itemlistex.Items[i].Visibility;
                        if (Visibility == null) { Visibility = ""; }
                        InsertOtherEvents(Calendar, EventId, RecurringEventId, ColorId, Attendes, AttendeesOmitted, AnyoneCanAddSelf, Locked, Title, Location, Description, Kind, StartDate, EndDate, MovedOn, Status, Recurrences, Organizer, Creator, Sequence, Transperancy, Updated, Visibility);


                    }


                }
            }
            catch (Exception ex)
            {
                Email.Email eml = new Email.Email();
                string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                eml.SendMail("Error syncing google calendar", "Error :- " + strLog, "sachin@zerofootprint.com.au", "", false);


            };

        }

        public void CheckCalendarEvents(string calendarId)
        {
            string text = string.Empty;
            try
            {
                text = File.ReadAllText("C://Google.txt");
                text = text + Environment.NewLine + "Set Calendar Called - CheckCalendarEvents(string CalendarID) method";
                File.WriteAllText("C://Google.txt", text);
            }
            catch
            { }
            SetCalendar();
            try
            {
                text = File.ReadAllText("C://Google.txt");
                text = text + Environment.NewLine + "Set Calendar Passed  - CheckCalendarEvents(string CalendarID) method";
                File.WriteAllText("C://Google.txt", text);
            }
            catch
            { }
            //calendarId = "ht642k01401dikgup7pk3j4l0s@group.calendar.google.com";
            var list = service.CalendarList.List().Execute().Items.Where(x => x.Id == calendarId).ToList();
            try
            {
                foreach (var cal in list)
                {                    
                    var itemlistreq = service.Events.List(cal.Id);
                    // itemlistreq.PageToken = token;
                    itemlistreq.MaxResults = 100000;
                    itemlistreq.ShowDeleted = true;
                    itemlistreq.ShowHiddenInvitations = true;
                    DateTime lastSync = SelectGoogleSyncTime(cal.Id);
                    if (lastSync != DateTime.Parse("01-01-0001 00:00:00"))
                    {
                        itemlistreq.UpdatedMin = lastSync; // here is the change
                    }
                    var itemlistex = itemlistreq.Execute();
                    InsertGoogleSyncTime(calendarId, DateTime.Now);
                    //itemlistex.Items[0].
                    var token = itemlistex.NextPageToken;
                    string[] Descs = null;
                    for (int i = 0; i < itemlistex.Items.Count; i++)
                    {
                        //if (Convert.ToDateTime(itemlistex.Items[i].Start.DateTime) > DateTime.Now.AddMonths(-1) && Convert.ToDateTime(itemlistex.Items[i].Start.DateTime) < DateTime.Now.AddMonths(2))
                        //{
                            string Calendar = cal.Id;
                            string EventId = itemlistex.Items[i].Id;
                            string RecurringEventId = itemlistex.Items[i].RecurringEventId;
                            if (RecurringEventId == null) { RecurringEventId = ""; };
                            var Attendees = itemlistex.Items[i].Attendees;
                            string Attendes = "";
                            if (Attendees != null)
                            {

                                for (int j = 0; j < Attendees.Count; j++)
                                { Attendes = Attendes + "~" + Attendees[j]; };
                            }
                            string ColorId = itemlistex.Items[i].ColorId;
                            if (ColorId == null) { ColorId = ""; };
                            bool? AttendeesOmitted = itemlistex.Items[i].AttendeesOmitted;
                            if (AttendeesOmitted == null) { AttendeesOmitted = false; };
                            bool? AnyoneCanAddSelf = itemlistex.Items[i].AnyoneCanAddSelf;
                            if (AnyoneCanAddSelf == null) { AnyoneCanAddSelf = false; };
                            string Title = itemlistex.Items[i].Summary;
                            if (Title == null) { Title = ""; }
                            string Location = itemlistex.Items[i].Location;
                            if (Location == null) { Location = ""; }
                            string Description = itemlistex.Items[i].Description;
                            if (Description == null) { Description = ""; }
                            string Kind = itemlistex.Items[i].Kind;
                            if (Kind == null) { Kind = ""; }
                            DateTime StartDate = Convert.ToDateTime("01/01/1900");
                            if (itemlistex.Items[i].Start != null)
                            {
                                if (itemlistex.Items[i].Start.Date != null)
                                {
                                    StartDate = Convert.ToDateTime(itemlistex.Items[i].Start.Date);
                                }
                                if (StartDate == null || StartDate == Convert.ToDateTime("01/01/1900"))
                                {
                                    { StartDate = Convert.ToDateTime(itemlistex.Items[i].Start.DateTime); }
                                    if (StartDate != null && StartDate.ToString().Contains("0001")) { StartDate = Convert.ToDateTime("01/01/1900"); }
                                }
                            }
                            DateTime EndDate = Convert.ToDateTime("01/01/1900");
                            if (itemlistex.Items[i].End != null)
                            {
                                if (itemlistex.Items[i].Start.Date != null)
                                {
                                    EndDate = Convert.ToDateTime(itemlistex.Items[i].Start.Date);
                                }
                                if (EndDate == null || EndDate == Convert.ToDateTime("01/01/1900"))
                                {
                                    { EndDate = Convert.ToDateTime(itemlistex.Items[i].End.DateTime); }
                                    if (EndDate != null && EndDate.ToString().Contains("0001")) { EndDate = Convert.ToDateTime("01/01/1900"); }
                                }
                            }
                            DateTime MovedOn = Convert.ToDateTime("01/01/1900");
                            if (itemlistex.Items[i].Created != null)
                            { MovedOn = Convert.ToDateTime(itemlistex.Items[i].Created); }
                            if (MovedOn != null && MovedOn.ToString().Contains("0001")) { MovedOn = Convert.ToDateTime("01/01/1900"); }
                            string Status = itemlistex.Items[i].Status;
                            var Recurrence = itemlistex.Items[i].Recurrence;
                            string Recurrences = "";
                            if (Recurrence != null)
                            {

                                for (int j = 0; j < Recurrence.Count; j++)
                                { Recurrences = Recurrences + "~" + Recurrence[j]; };
                            }
                            string Organizer = "";
                            if (itemlistex.Items[i].Organizer != null)
                            {
                                Organizer = itemlistex.Items[i].Organizer.DisplayName;
                            }
                            if (Organizer == null) { Organizer = ""; }
                            string Creator = "";
                            if (itemlistex.Items[i].Creator != null)
                            {
                                Creator = itemlistex.Items[i].Creator.DisplayName;
                            }
                            if (Creator == null) { Creator = ""; }
                            bool? Locked = false;
                            try
                            {
                                Locked = itemlistex.Items[i].Locked;
                            }
                            catch { };
                            Locked = false;
                            //if (Locked == null) { Locked = false; };
                            int Sequence = 0;
                            if (itemlistex.Items[i].Sequence != null)
                            { Sequence = (int)itemlistex.Items[i].Sequence; }
                            string Transperancy = itemlistex.Items[i].Transparency;
                            if (Transperancy == null) { Transperancy = ""; }
                            DateTime Updated = Convert.ToDateTime("01/01/1900");
                            if (itemlistex.Items[i].Updated != null)
                            { Updated = Convert.ToDateTime(itemlistex.Items[i].Updated); }
                            if (Updated != null && Updated.ToString().Contains("0001")) { Updated = Convert.ToDateTime("01/01/1900"); }
                            string Visibility = itemlistex.Items[i].Visibility;
                            if (Visibility == null) { Visibility = ""; }
                            InsertOtherEvents(Calendar, EventId, RecurringEventId, ColorId, Attendes, AttendeesOmitted, AnyoneCanAddSelf, Locked, Title, Location, Description, Kind, StartDate, EndDate, MovedOn, Status, Recurrences, Organizer, Creator, Sequence, Transperancy, Updated, Visibility);
                        //}
                    }
                }
                try
                {
                    text = File.ReadAllText("C://Google.txt");
                    text = text + Environment.NewLine + "Added in DB  - CheckCalendarEvents(string CalendarID) method";
                    File.WriteAllText("C://Google.txt", text);
                }
                catch
                { }
                //UpdateCalendar();
            }
            catch (Exception ex)
            {
                text = File.ReadAllText("C://Google.txt");
                text = text + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace;
                File.WriteAllText("C://Google.txt", text);
                //Email.Email eml = new Email.Email();
                //string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                //eml.SendMail("Error syncing google calendar", "Error :- " + strLog, "sachin@zerofootprint.com.au", "", false);


            };

        }

        public String[] Splits(string values, char[] character)
        {


            string[] strSplitArr = values.Split(character);
            return strSplitArr;

        }

        public void InsertCalendarEvents(string Calendar, string OrderId, string Title, DateTime StartDate, DateTime EndDate, DateTime MovedOn, string Status)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertEvents", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Calendar", SqlDbType.VarChar).Value = Calendar;
                        cmd.Parameters.Add("@OrderId", SqlDbType.VarChar).Value = OrderId;
                        cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = Title;
                        cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        cmd.Parameters.Add("@MovedOn", SqlDbType.DateTime).Value = MovedOn;
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public void InsertOtherEvents(string Calendar, string EventId, string RecurringEventId, string ColorId, string Attendees, bool? AttendeesOmitted, bool? AnyoneCanAddSelf, bool? Locked, string Title, string Location, string Description, string Kind, DateTime StartDate, DateTime EndDate, DateTime MovedOn, string Status, string Recurrence, string Organizer, string Creator, int Sequence, string Transperancy, DateTime Updated, string Visibility)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertOtherEvents", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Calendar", SqlDbType.VarChar).Value = Calendar;
                        cmd.Parameters.Add("@EventId", SqlDbType.VarChar).Value = EventId;
                        cmd.Parameters.Add("@RecurringEventId", SqlDbType.VarChar).Value = RecurringEventId;
                        cmd.Parameters.Add("@ColorId", SqlDbType.VarChar).Value = ColorId;
                        cmd.Parameters.Add("@Attendees", SqlDbType.VarChar).Value = Attendees;
                        cmd.Parameters.Add("@AttendeesOmitted", SqlDbType.Bit).Value = AttendeesOmitted;
                        cmd.Parameters.Add("@AnyoneCanAddSelf", SqlDbType.Bit).Value = AnyoneCanAddSelf;
                        cmd.Parameters.Add("@Locked", SqlDbType.Bit).Value = Locked;
                        cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = Title;
                        cmd.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
                        cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                        cmd.Parameters.Add("@Kind", SqlDbType.VarChar).Value = Kind;
                        cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                        cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                        cmd.Parameters.Add("@MovedOn", SqlDbType.Date).Value = MovedOn;
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;
                        cmd.Parameters.Add("@Recurrence", SqlDbType.VarChar).Value = Recurrence;
                        cmd.Parameters.Add("@Organizer", SqlDbType.VarChar).Value = Organizer;
                        cmd.Parameters.Add("@Creator", SqlDbType.VarChar).Value = Creator;
                        cmd.Parameters.Add("@Sequence", SqlDbType.Int).Value = Sequence;
                        cmd.Parameters.Add("@Transperancy", SqlDbType.VarChar).Value = Transperancy;
                        cmd.Parameters.Add("@Updated", SqlDbType.DateTime).Value = Updated;
                        cmd.Parameters.Add("@Visibility", SqlDbType.VarChar).Value = Visibility;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public void InsertGoogleSyncTime(string calendar,DateTime syncTime)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertGoogleSyncTime", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Calendar", SqlDbType.VarChar).Value = calendar;
                        cmd.Parameters.Add("@LastSync", SqlDbType.DateTime).Value = syncTime;                       
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public DateTime SelectGoogleSyncTime(string calendar)
        {            
            DateTime lastsync = new DateTime(); 
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SelectLastSyncForCalendar", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Calendar", SqlDbType.VarChar).Value = calendar;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count == 1)
                        {
                            lastsync = DateTime.Parse(dt.Rows[0][0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            return lastsync;
        }

        public void InsertCalendars(string Calendar, int Org_Id)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertCalendars", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Calendar", SqlDbType.VarChar).Value = Calendar;
                        cmd.Parameters.Add("@Org_Id", SqlDbType.Int).Value = Org_Id;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public void DeleteEvent(string eventId, string UserCalendar)
        {
            SetCalendar();
            service.Events.Delete(UserCalendar, eventId.Trim()).Execute();
            //InsertReturnedEventDetailsInDB(UserCalendar, returnEvent);
        }

        public string UpdateEvent(DateTime Start, DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule,bool isAllDay,string ColorId="")//, string[] reccurrence)
        {
            string updatedEventId = string.Empty;
            SetCalendar();

            Event evnt = new Event();

            if (ColorId == "")
            { evnt.ColorId = "5"; }
            if (ColorId != "")
            { evnt.ColorId = ColorId; }

            if (isAllDay)
            {
                evnt.Start = new EventDateTime
                {

                    Date = Start.ToString("yyyy-MM-dd"),
                    TimeZone = "Etc/UTC"
                };
                evnt.End = new EventDateTime
                {
                    Date = End.ToString("yyyy-MM-dd"),
                    TimeZone = "Etc/UTC"
                };
                evnt.Transparency = "transparent";
            }
            else
            {
                evnt.Start = new EventDateTime
                {
                    DateTime = Start,
                    TimeZone = "Etc/UTC"
                };
                evnt.End = new EventDateTime
                {
                    DateTime = End,
                    TimeZone = "Etc/UTC"
                };
            }

            evnt.Created = DateTime.Today;
            evnt.Creator = new Event.CreatorData
            {
                DisplayName = Creator
            };

            evnt.Organizer = new Event.OrganizerData
            {
                DisplayName = Organizer,
            };
            //evnt.Location = Location;


            evnt.Description = description;//"SalesContact : " + SalesContact + Environment.NewLine + Environment.NewLine + "Supplier Instructions :" ;
            evnt.Summary = Title;
            //evnt.Description = Title + "Desc";
            evnt.Id = eventId;
            evnt.Location = Location;
            evnt.Sequence = sequence;
            if (reccurrenceRule.Trim() != string.Empty)
            {
                string recc = "RRULE:" + reccurrenceRule;// +"\r\n";//"DTSTART;VALUE=DATE:" + DateTime.Parse(StartDate).ToString("yyyyMMdd").Replace("-","") + "\r\n" + "DTEND;VALUE=DATE:" + DateTime.Parse(EndDate).ToString("yyyyMMdd").Replace("-", "") + "\r\n" +
                List<string> reccurrence = new List<string>();
                reccurrence.Add(recc);
                evnt.Recurrence = reccurrence;
                //evnt.RecurringEventId = "1234";
                //evnt.OriginalStartTime = evnt.Start;
            }
            var updatedEvent = service.Events.Update(evnt, UserCalendar, eventId).Execute();
            eventId = updatedEvent.Id;
            InsertReturnedEventDetailsInDB(UserCalendar, updatedEvent);
            return updatedEventId;
        }

        public string MoveEvent(DateTime Start, DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string destinationCalendar, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay,string ColorId="")//, string[] reccurrence)
        {
            string movedEventId = string.Empty;
            SetCalendar();

            Event evnt = new Event();

            if (ColorId == "")
            { evnt.ColorId = "5"; }
            if (ColorId != "")
            { evnt.ColorId = ColorId; }

            if (isAllDay)
            {
                evnt.Start = new EventDateTime
                {

                    Date = Start.ToString("yyyy-MM-dd"),
                    TimeZone = "Etc/UTC"
                };
                evnt.End = new EventDateTime
                {
                    Date = End.ToString("yyyy-MM-dd"),
                    TimeZone = "Etc/UTC"
                };
                evnt.Transparency = "transparent";
            }
            else
            {
                evnt.Start = new EventDateTime
                {
                    DateTime = Start,
                    TimeZone = "Etc/UTC"
                };
                evnt.End = new EventDateTime
                {
                    DateTime = End,
                    TimeZone = "Etc/UTC"
                };
            }

            evnt.Created = DateTime.Today;
            evnt.Creator = new Event.CreatorData
            {
                DisplayName = Creator
            };

            evnt.Organizer = new Event.OrganizerData
            {
                DisplayName = Organizer,
            };
            //evnt.Location = Location;

            
            //evnt.Description = "SalesContact : " + SalesContact + Environment.NewLine + Environment.NewLine + "Required On :" + Convert.ToDateTime(RequiredDate).ToShortDateString() + Environment.NewLine + Environment.NewLine + "Product Description :" + ProductDescription + Environment.NewLine + Environment.NewLine + "Supplier Instructions :" + Comments.Replace("Supplier Instructions", "") + Environment.NewLine + Environment.NewLine + "Order Item (Please donot delete or update OrderItemId) :" + OrderId;
            evnt.Summary = Title;
            evnt.Description = description;
            //evnt.Id = eventViewModel.EventId;
            evnt.Location = Location;
            evnt.Sequence = sequence;
            if (reccurrenceRule.Trim() != string.Empty)
            {
                string recc = "RRULE:" + reccurrenceRule;// +"\r\n";//"DTSTART;VALUE=DATE:" + DateTime.Parse(StartDate).ToString("yyyyMMdd").Replace("-","") + "\r\n" + "DTEND;VALUE=DATE:" + DateTime.Parse(EndDate).ToString("yyyyMMdd").Replace("-", "") + "\r\n" +
                List<string> reccurrence = new List<string>();
                reccurrence.Add(recc);
                evnt.Recurrence = reccurrence;
                //evnt.RecurringEventId = "1234";
                //evnt.OriginalStartTime = evnt.Start;
            }
            service.Events.Move(UserCalendar, eventId,destinationCalendar).Execute();
            var updatedEvent = service.Events.Update(evnt,destinationCalendar, eventId).Execute();
            movedEventId = updatedEvent.Id;
            InsertReturnedEventDetailsInDB(destinationCalendar, updatedEvent);
            return movedEventId;
        }

    }
}
