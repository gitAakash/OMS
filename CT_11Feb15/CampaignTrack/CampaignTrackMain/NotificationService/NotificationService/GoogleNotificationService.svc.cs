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
using System.Net;

namespace NotificationService
{

    public class GoogleNotificationService : IGoogleNotificationService
    {
        //public delegate void CanlendarHandler();
        //public event CanlendarHandler UpdateCalendar;

        //' Calendar scopes which is initialized on the main method.

        static IList<string> scopes = new List<string>();
        //' Calendar service.

        static CalendarService service;


        public class GoogleToken
        {
            public string access_token;
            public string token_type;
            public string expires_in;
            public string refresh_token;
            public string Issued;
        }

        //Dev SetCalendar
        public void SetCalendar(int orgId, bool isSubscription = false)
        {
            string clientKey = string.Empty;
            string secretKey = string.Empty;
            string refreshToken = string.Empty;
            string name = "GoogleCalendar";
            if (isSubscription)
            {
                name = "SubscribeGoogleCalendar";

            }
            try
            {
                var dt = SelectClientCredentials(name, orgId);
                if (dt != null)
                {
                    if (dt.Rows[0][0] != null)
                        clientKey = dt.Rows[0][0].ToString();
                    if (dt.Rows[0][1] != null)
                        secretKey = dt.Rows[0][1].ToString();
                    if (dt.Rows[0][2] != null)
                        refreshToken = dt.Rows[0][2].ToString();
                }

                scopes.Add(CalendarService.Scope.Calendar);
                UserCredential credential = default(UserCredential);
                DatabaseDataStore _fdsToken;
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    StoredResponse storedResponse = new StoredResponse(refreshToken);
                    _fdsToken = new DatabaseDataStore(storedResponse, orgId);
                }
                else
                {
                    _fdsToken = new DatabaseDataStore(orgId);
                }
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                  new ClientSecrets
                  {
                      ClientId = clientKey,// "532982290458-u189keaaki44sskvtki6p28rq5crjl6d.apps.googleusercontent.com",
                      ClientSecret = secretKey,//"DDO62gcKn5nYWm4XCXlJkngo",
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// production set calendar
        /// </summary>
        //public void SetCalendar(int orgId)
        //{
        //    string clientKey = string.Empty;
        //    string secretKey = string.Empty;
        //    string refreshToken = string.Empty;
        //    try
        //    {
        //        var dt = SelectClientCredentials("GoogleCalendar", orgId);
        //        if (dt != null)
        //        {
        //            if (dt.Rows[0][0] != null)
        //                clientKey = dt.Rows[0][0].ToString();
        //            if (dt.Rows[0][1] != null)
        //                secretKey = dt.Rows[0][1].ToString();
        //            if (dt.Rows[0][2] != null)
        //                refreshToken = dt.Rows[0][2].ToString();
        //        }

        //        scopes.Add(CalendarService.Scope.Calendar);
        //        UserCredential credential = default(UserCredential);
        //        DatabaseDataStore _fdsToken;
        //        if (!string.IsNullOrEmpty(refreshToken))
        //        {
        //            StoredResponse storedResponse = new StoredResponse(refreshToken);
        //            _fdsToken = new DatabaseDataStore(storedResponse, orgId);
        //        }
        //        else
        //        {
        //            _fdsToken = new DatabaseDataStore(orgId);
        //        }

        //        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //          new ClientSecrets
        //          {
        //              ClientId = "466513012782-ubgcnh2c3f25iprbhbmisb2l4i8965lf.apps.googleusercontent.com",
        //              ClientSecret = "JQT2RdjIoMjHuevYwfyFnefx",
        //          }, scopes,
        //          "Z",
        //        CancellationToken.None, _fdsToken).Result;
        //        credential.Token.ExpiresInSeconds = 500000;
        //        String token = credential.Token.RefreshToken;


        //        credential.Token.RefreshToken = refreshToken;


        //        BaseClientService.Initializer initializer = new BaseClientService.Initializer();
        //        initializer.HttpClientInitializer = credential;
        //        initializer.ApplicationName = "DPI Google Calendar";
        //        service = new CalendarService(initializer);
        //    }
        //    catch (Exception ex)
        //    {
        //        //string text = File.ReadAllText("C://Google.txt");
        //        //text = text + Environment.NewLine + ex.Message;
        //        //File.WriteAllText("C://Google.txt", text);
        //    }
        //}

        //private string _clientKey = string.Empty;
        //private string _clientSecretKey = string.Empty;
        //private string _refreshToken = string.Empty;
        //private void GetCalendarCredentials(int p)
        //{

        //}

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


        public string CreateEvent(string Title, string Location, string SalesContact, DateTime StartDate, DateTime EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId, string UserCalendar, string description, string reccurrenceRule, bool isAllDay)
        {
            string eventIdAndSequence = string.Empty;
            try
            {
                int orgId = 0;
                orgId = GetOrgId(UserCalendar, orgId);
                if (orgId == 0)
                {
                    return eventIdAndSequence;
                }
                SetCalendar(orgId);
                Event evnt = new Event();
                if (ColorId != "")
                { evnt.ColorId = ColorId; }

                if (isAllDay)
                {
                    evnt.Start = new EventDateTime
                    {

                        Date = StartDate.ToString("yyyy-MM-dd"),
                        TimeZone = "Australia/Melbourne"
                    };
                    evnt.End = new EventDateTime
                    {
                        Date = EndDate.AddDays(1).ToString("yyyy-MM-dd"),
                        TimeZone = "Australia/Melbourne"//TimeZoneInfo.Local.Id//"Etc/UTC"
                    };
                    evnt.Transparency = "transparent";
                }
                else
                {
                    //string startFormat = StartDate;
                    evnt.Start = new EventDateTime
                    {

                        DateTime = StartDate,
                        TimeZone = "Australia/Melbourne"
                    };

                    evnt.End = new EventDateTime
                    {
                        DateTime = EndDate,
                        TimeZone = "Australia/Melbourne"
                    };
                }



                evnt.Description = description;
                evnt.Summary = Title;
                evnt.Location = Location;

                if (reccurrenceRule.Trim() != string.Empty)
                {
                    string recc = "RRULE:" + reccurrenceRule;
                    List<string> reccurrence = new List<string>();
                    reccurrence.Add(recc);
                    evnt.Recurrence = reccurrence;
                }

                var returnedEventDetailes = service.Events.Insert(evnt, UserCalendar).Execute();
                eventIdAndSequence = returnedEventDetailes.Id + "," + returnedEventDetailes.Sequence;
                InsertReturnedEventDetailsInDB(UserCalendar, returnedEventDetailes);

            }
            catch (Exception ex)
            {
                File.AppendAllText("C:\\Google.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
            return eventIdAndSequence;

        }

        private void InsertReturnedEventDetailsInDB(string UserCalendar, Event returnedEventDetailes, bool isAllDay = false)
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
            {
                if (returnedEventDetailes.Start.DateTime != null)
                    StartDate = Convert.ToDateTime(returnedEventDetailes.Start.DateTime);
                else
                {
                    StartDate = Convert.ToDateTime(returnedEventDetailes.Start.Date);
                }
            }

            //{ 
            //    StartDate = Convert.ToDateTime(returnedEventDetailes.Start.DateTime); 

            //}
            if (StartDate != null && StartDate.ToString().Contains("0001")) { StartDate = Convert.ToDateTime("01/01/1900"); }
            DateTime EndDate = Convert.ToDateTime("01/01/1900");
            if (returnedEventDetailes.End != null)
            {
                if (returnedEventDetailes.End.DateTime != null)
                    EndDate = Convert.ToDateTime(returnedEventDetailes.End.DateTime);
                else
                {
                    EndDate = Convert.ToDateTime(returnedEventDetailes.End.Date);
                }

            }
            if (EndDate != null && EndDate.ToString().Contains("0001")) { EndDate = Convert.ToDateTime("01/01/1900"); }

            if (isAllDay)
            {
                EndDate = EndDate.AddDays(-1);
            }

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
            InsertOtherEvents(Calendar, EventId, RecurringEventId, ColorId, Attendes, AttendeesOmitted, AnyoneCanAddSelf, Locked, Title, Location, Description, Kind, StartDate, EndDate, MovedOn, Status, Recurrences, Organizer, Creator, Sequence, Transperancy, Updated, Visibility, isAllDay);
        }

        public void CheckCalendarMovements(string calendar, int orgId)
        {
            SetCalendar(orgId);
            //CheckCalendarEvents(calendar);

            string a;
            var list = service.CalendarList.List().Execute().Items;//.Where(x=>x.Id==calendar);
            try
            {

                foreach (var cal in list)
                {

                    // if (cal.Id != "campaigntrack.dpi@gmail.com") { continue; }
                    int res = InsertCalendars(cal.Id, 1, cal.TimeZone);
                    //Subscribe(cal);
                    //var itemlistreq = service.Events.List(cal.Id);
                    //// itemlistreq.PageToken = token;
                    //itemlistreq.MaxResults = 100000;
                    //itemlistreq.ShowDeleted = true;
                    //itemlistreq.ShowHiddenInvitations = true;
                    //var itemlistex = itemlistreq.Execute();

                    ////itemlistex.Items[0].
                    //var token = itemlistex.NextPageToken;
                    //for (int i = 0; i < itemlistex.Items.Count; i++)
                    //{
                    //    if (itemlistex.Items[i].Description != null)
                    //    {

                    //        if (Convert.ToDateTime(itemlistex.Items[i].Created).Date == DateTime.Now.Date || Convert.ToDateTime(itemlistex.Items[i].Created).Date == DateTime.Now.Date.AddDays(-1) || Convert.ToDateTime(itemlistex.Items[i].Created).Date == DateTime.Now.Date.AddDays(-2))
                    //        {
                    //            if (itemlistex.Items[i].Description.Contains("Order Item (Please donot delete or update OrderItemId) :") == true)
                    //            {
                    //                string Desc = itemlistex.Items[i].Description;

                    //                string[] Descs = Desc.Split(new string[] { "Order Item (Please donot delete or update OrderItemId) :" }, StringSplitOptions.None);
                    //                string OrderItemId = Descs[1];
                    //                string Title = itemlistex.Items[i].Summary;
                    //                string Calendar = cal.Id;
                    //                DateTime StartDate = Convert.ToDateTime(itemlistex.Items[i].Start.DateTime);
                    //                DateTime EndDate = Convert.ToDateTime(itemlistex.Items[i].End.DateTime);
                    //                DateTime MovedOn = Convert.ToDateTime(itemlistex.Items[i].Created);
                    //                string Status = itemlistex.Items[i].Status;
                    //                //InsertCalendarEvents(Calendar, OrderItemId, Title, StartDate, EndDate, MovedOn, Status);

                    //            }

                    //        }
                    //    }
                    //}

                    //Here, itemlistreq.PageToken == null;
                }
                //UpdateCalendar();
            }
            catch (Exception ex)
            {
                Email.Email eml = new Email.Email();
                string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                eml.SendMail("Error syncing google calendar", "Error :- " + strLog, "sachin@zerofootprint.com.au", "", false);


            };

        }

        void Subscribe(CalendarListEntry cal, int orgId)
        {
            SetCalendar(orgId, true);
            Dictionary<string, string> param = new Dictionary<String, String>();
            Channel request = new Channel();
            request.Id = System.Guid.NewGuid().ToString();
            request.Type = "web_hook";
            request.Params = param;
            if (cal.Id == "campaigntrack.dpi@gmail.com")
            {
                request.Address = "https://dev.zerofootprint.com.au/notification/Default.aspx";
            }
            else
            {
                request.Address = "https://dev.zerofootprint.com.au/notification/ChildCalendar.aspx";
            }
            try
            {
                var changes = service.Events.Watch(request, cal.Id).Execute();//.CalendarList.Events.cal(request, "dpi@gmail.com").Execute();
            }
            catch
            { }
        }

        //void SetupCalendarForSubsription()
        //{
        //    try
        //    {
        //        scopes.Add(CalendarService.Scope.Calendar);
        //        UserCredential credential = default(UserCredential);
        //        FileDataStore _fdsToken = new FileDataStore("CT");
        //        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //          new ClientSecrets
        //          {
        //              ClientId = "532982290458-u189keaaki44sskvtki6p28rq5crjl6d.apps.googleusercontent.com",
        //              ClientSecret = "DDO62gcKn5nYWm4XCXlJkngo",
        //          }, scopes,
        //          "Z",
        //        CancellationToken.None, _fdsToken).Result;
        //        credential.Token.ExpiresInSeconds = 500000;
        //        String token = credential.Token.RefreshToken;
        //        credential.Token.RefreshToken = "1/wU76ItFh-n64iqZLjIiXER0Vx4u-MxEnHtHFLgQpa7E";
        //        credential.Token.ExpiresInSeconds = 500000;
        //        // Create the calendar service using an initializer instance
        //        BaseClientService.Initializer initializer = new BaseClientService.Initializer();
        //        initializer.HttpClientInitializer = credential;
        //        initializer.ApplicationName = "DPI Google Calendar";
        //        service = new CalendarService(initializer);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public void CheckAllCalendarEvents(int orgId)
        {
            SetCalendar(orgId);
            bool IsAllDay = false;
            var list = service.CalendarList.List().Execute().Items;
            try
            {

                foreach (var cal in list)
                {
                    InsertGoogleSyncTime(cal.Id, DateTime.Now);
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
                        IsAllDay = false;
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
                            if (itemlistex.Items[i].End.Date != null)
                            {
                                EndDate = Convert.ToDateTime(itemlistex.Items[i].End.Date);
                                IsAllDay = true;// here this is all day event
                                EndDate = EndDate.AddDays(-1);

                                //text = File.ReadAllText("C://Google.txt");
                                //text = text + Environment.NewLine + " End Date- " + EndDate.ToString();
                                //File.WriteAllText("C://Google.txt", text);

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

                        InsertOtherEvents(Calendar, EventId, RecurringEventId, ColorId, Attendes, AttendeesOmitted, AnyoneCanAddSelf, Locked, Title, Location, Description, Kind, StartDate, EndDate, MovedOn, Status, Recurrences, Organizer, Creator, Sequence, Transperancy, Updated, Visibility, IsAllDay);


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
            bool IsAllDay = false;
            int orgId = 0;
            orgId = GetOrgId(calendarId, orgId);
            if (orgId != 0)
            {
                SetCalendar(orgId);
                List<CalendarListEntry> list = new List<CalendarListEntry>();
                //var abc = service.CalendarList.List().Execute().Items.ToList();
                list = service.CalendarList.List().Execute().Items.Where(x => x.Id == calendarId).ToList();
                try
                {
                    foreach (var cal in list)
                    {
                        IsAllDay = false;
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

                        if (DateTime.Now.Subtract(DateTime.Parse(lastSync.ToString())).TotalSeconds > 2 || calendarId != "campaigntrack.dpi@gmail.com")
                        {
                            DateTime nextSyncTime = DateTime.Now;
                            var itemlistex = itemlistreq.Execute();
                            if (itemlistex.Items.Count > 0)
                            {
                                InsertGoogleSyncTime(calendarId, nextSyncTime);
                            }
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
                                    if (itemlistex.Items[i].End.Date != null)
                                    {
                                        EndDate = Convert.ToDateTime(itemlistex.Items[i].End.Date);
                                        IsAllDay = true;// here this is all day event
                                        EndDate = EndDate.AddDays(-1);

                                        //text = File.ReadAllText("C://Google.txt");
                                        //text = text + Environment.NewLine + " End Date- " + EndDate.ToString();
                                        //File.WriteAllText("C://Google.txt", text);

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

                                InsertOtherEvents(Calendar, EventId, RecurringEventId, ColorId, Attendes, AttendeesOmitted, AnyoneCanAddSelf, Locked, Title, Location, Description, Kind, StartDate, EndDate, MovedOn, Status, Recurrences, Organizer, Creator, Sequence, Transperancy, Updated, Visibility, IsAllDay);

                            }
                        }
                    }
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
        }

        private int GetOrgId(string calendarId, int orgId)
        {
            if (GetCalendarInformation().Keys.Contains(calendarId))
            {
                orgId = Convert.ToInt16(GetCalendarInformation()[calendarId]);
            }
            else
            {
                CalendarInformation = null;
                if (GetCalendarInformation().Keys.Contains(calendarId))
                {
                    orgId = Convert.ToInt16(GetCalendarInformation()[calendarId]);
                }
            }
            return orgId;
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

        public void InsertOtherEvents(string Calendar, string EventId, string RecurringEventId, string ColorId, string Attendees, bool? AttendeesOmitted, bool? AnyoneCanAddSelf, bool? Locked, string Title, string Location, string Description, string Kind, DateTime StartDate, DateTime EndDate, DateTime MovedOn, string Status, string Recurrence, string Organizer, string Creator, int Sequence, string Transperancy, DateTime Updated, string Visibility, bool IsAllDay = false)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
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
                            cmd.Parameters.Add("@IsAllDay", SqlDbType.Bit).Value = IsAllDay;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        SqlConnection.ClearPool(con);
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }

                    }
                }
            }
            catch (Exception ex) { throw ex; }

        }

        public void UpdateRefreshToken(int orgId, string refreshToken)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("UpdateCalendarRefreshToken", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.Add("@clientKey", SqlDbType.VarChar).Value = clientKey;
                            //cmd.Parameters.Add("@clientSecretKey", SqlDbType.VarChar).Value = clientSecretKey;
                            cmd.Parameters.Add("@refreshToken", SqlDbType.VarChar).Value = refreshToken;
                            cmd.Parameters.Add("@orgId", SqlDbType.Int).Value = orgId;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    catch (Exception ex)
                    { }
                    finally
                    {
                        SqlConnection.ClearPool(con);
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }

                    }

                }
            }
            catch (Exception ex) { throw ex; }

        }

        /// <summary>
        /// This function is used for updating the event and set status as cancelled
        /// </summary>
        /// <param name="Event_id"></param>
        /// <param name="OldCalendarUse"></param>
        public void UpdateEventInDB(string Event_id, string OldCalendarUser)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("UpdateEvent", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Event_id", SqlDbType.VarChar).Value = Event_id;
                            cmd.Parameters.Add("@OldCalendarUser", SqlDbType.VarChar).Value = OldCalendarUser;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    catch (Exception ex)
                    { }
                    finally
                    {
                        SqlConnection.ClearPool(con);
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }

                    }

                }
            }
            catch (Exception ex) { throw ex; }

        }

        public DataTable SelectClientCredentials(string name, int orgId)
        {
            DataTable dt = new DataTable();
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("SelectClientCredentials", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
                            cmd.Parameters.Add("@orgId", SqlDbType.Int).Value = orgId;

                            SqlDataAdapter da = new SqlDataAdapter(cmd);

                            da.Fill(dt);
                        }
                    }

                    catch (Exception ex)
                    { }
                    finally
                    {
                        SqlConnection.ClearPool(con);
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }

                    }

                }
            }
            catch (Exception ex) { throw ex; }
            return dt;
        }

        public void InsertGoogleSyncTime(string calendar, DateTime syncTime)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    try
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
                    catch (Exception ex)
                    { }
                    finally
                    {
                        SqlConnection.ClearPool(con);
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }

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
                    try
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

                    catch (Exception ex)
                    { }
                    finally
                    {
                        SqlConnection.ClearPool(con);
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }

                    }

                }
            }
            catch (Exception ex) { throw ex; }
            return lastsync;
        }

        public int InsertCalendars(string Calendar, int Org_Id, string TimeZone)
        {
            int res;
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
                        cmd.Parameters.Add("@TimeZone", SqlDbType.VarChar).Value = TimeZone;
                        con.Open();
                        res = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            return res;

        }

        public string DeleteEvent(string eventId, string UserCalendar)
        {


            string retVal = "Entry already deleted.";
            try
            {
                int orgId = 0;
                orgId = GetOrgId(UserCalendar, orgId);
                if (orgId != 0)
                {
                    SetCalendar(orgId);

                    retVal = service.Events.Delete(UserCalendar, eventId.Trim()).Execute();
                }

                //InsertReturnedEventDetailsInDB(UserCalendar, returnEvent);
            }
            catch (Exception ex)
            {


            }
            return retVal;
        }

        public string DeleteEventInstance(string eventId, string UserCalendar, DateTime Start)
        {
            string retVal = "Instance not available";
            try
            {
                int orgId = 0;
                orgId = GetOrgId(UserCalendar, orgId);
                if (orgId != 0)
                {
                    SetCalendar(orgId);

                    Events instances = service.Events.Instances(UserCalendar, eventId).Execute();

                    Event instance = instances.Items.FirstOrDefault(x => Convert.ToDateTime(x.Start.DateTime).Date == Start.Date);
                    //instance.Status = "cancelled";

                    var evnt = service.Events.Delete(UserCalendar, instance.Id).Execute();//.Update(instance,UserCalendar, eventId.Trim()).Execute();

                    retVal = instance.Id;
                }
            }
            catch (Exception ex)
            {


            }
            return retVal;
        }

        public string UpdateEvent(DateTime Start, DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId = "")//, string[] reccurrence)
        {

            string updatedEventId = string.Empty;
            try
            {
                int orgId = 0;
                orgId = GetOrgId(UserCalendar, orgId);
                if (orgId == 0)
                {
                    return updatedEventId;
                }
                //if (service == null)
                //{
                SetCalendar(orgId);
                //}

                // CheckCalendarEvents("dpi.campaigntrack@gmail.com");

                Event evnt = new Event();

                //if (ColorId == "")
                //{ evnt.ColorId = "5"; }
                if (ColorId != "")
                { evnt.ColorId = ColorId; }

                if (isAllDay)
                {
                    evnt.Start = new EventDateTime
                    {

                        Date = Start.ToString("yyyy-MM-dd"),
                        TimeZone = "Australia/Melbourne"//"Etc/UTC"
                    };

                    evnt.End = new EventDateTime
                    {
                        Date = End.AddDays(1).ToString("yyyy-MM-dd"),
                        // Date = End.ToString("yyyy-MM-dd"), here we are adding one day becoz at google take less one day in all day event so we explicity adding one day
                        TimeZone = "Australia/Melbourne"//"Etc/UTC"
                    };

                    evnt.Transparency = "transparent";
                }
                else
                {
                    evnt.Start = new EventDateTime
                    {
                        DateTime = Start,
                        TimeZone = "Australia/Melbourne"//"Etc/UTC"
                    };
                    evnt.End = new EventDateTime
                    {
                        DateTime = End,
                        TimeZone = "Australia/Melbourne"//"Etc/UTC"
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
                InsertReturnedEventDetailsInDB(UserCalendar, updatedEvent, isAllDay);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot turn an instance of a recurring event into a recurring event itself. [400]"))
                {
                    updatedEventId = "CustomError401";
                }

                File.AppendAllText("C:\\Google.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
            return updatedEventId;
        }

        public string UpdateEventInstance(DateTime Start, DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId = "")//, string[] reccurrence)
        {

            string updatedEventId = string.Empty;
            try
            {
                int orgId = 0;
                orgId = GetOrgId(UserCalendar, orgId);
                if (orgId == 0)
                {
                    return updatedEventId;
                }
                SetCalendar(orgId);

                var instances = service.Events.Instances(UserCalendar, eventId).Execute();

                Event evnt = instances.Items.FirstOrDefault(x => Convert.ToDateTime(x.Start.DateTime).Date == Start.Date);

                if (evnt != null)
                {
                    if (ColorId != "")
                    { evnt.ColorId = ColorId; }

                    if (isAllDay)
                    {
                        evnt.Start = new EventDateTime
                        {

                            Date = Start.ToString("yyyy-MM-dd"),
                            TimeZone = "Australia/Melbourne"//"Etc/UTC"
                        };
                        evnt.End = new EventDateTime
                        {
                            Date = End.ToString("yyyy-MM-dd"),
                            TimeZone = "Australia/Melbourne"//"Etc/UTC"
                        };
                        evnt.Transparency = "transparent";
                    }
                    else
                    {
                        evnt.Start = new EventDateTime
                        {
                            DateTime = Start,
                            TimeZone = "Australia/Melbourne"//"Etc/UTC"
                        };
                        evnt.End = new EventDateTime
                        {
                            DateTime = End,
                            TimeZone = "Australia/Melbourne"//"Etc/UTC"
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



                    evnt.Description = description;
                    evnt.Summary = Title;
                    evnt.Location = Location;
                    int seq = 0;
                    if (evnt.Sequence != null)
                    {
                        seq = sequence + 1;
                    }
                    evnt.Sequence = seq;
                    //if (reccurrenceRule.Trim() != string.Empty)
                    //{
                    //    string recc = "RRULE:" + reccurrenceRule;
                    //    List<string> reccurrence = new List<string>();
                    //    reccurrence.Add(recc);
                    //    evnt.Recurrence = reccurrence;
                    //}                

                    var updatedEvent = service.Events.Update(evnt, UserCalendar, evnt.Id).Execute();
                    updatedEventId = updatedEvent.Id;
                    eventId = updatedEvent.Id;
                    InsertReturnedEventDetailsInDB(UserCalendar, updatedEvent);
                }
                else
                {
                    File.AppendAllText("C:\\Google.txt", "Null Event : Update instannce");
                    updatedEventId = "Null_Event";
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("C:\\Google.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
            return updatedEventId;
        }

        public string MoveEvent(DateTime Start, DateTime End, string Creator, string Organizer, string Location, string Title, string eventId, string destinationCalendar, string UserCalendar, int sequence, string description, string reccurrenceRule, bool isAllDay, string ColorId = "")//, string[] reccurrence)
        {
            string movedEventId = string.Empty;
            try
            {
                int orgId = 0;
                orgId = GetOrgId(UserCalendar, orgId);
                if (orgId == 0)
                {
                    return movedEventId;
                }
                //if (service == null)
                //{
                SetCalendar(orgId);
                //}

                Event evnt = new Event();

                //if (ColorId == "")
                //{ evnt.ColorId = "5"; }
                if (ColorId != "")
                { evnt.ColorId = ColorId; }

                if (isAllDay)
                {
                    evnt.Start = new EventDateTime
                    {

                        Date = Start.ToString("yyyy-MM-dd"),
                        TimeZone = "Australia/Melbourne"//"Etc/UTC"
                    };
                    evnt.End = new EventDateTime
                    {
                        Date = End.ToString("yyyy-MM-dd"),
                        TimeZone = "Australia/Melbourne"//"Etc/UTC"
                    };
                    evnt.Transparency = "transparent";
                }
                else
                {
                    evnt.Start = new EventDateTime
                    {
                        DateTime = Start,
                        TimeZone = "Australia/Melbourne"//"Etc/UTC"
                    };
                    evnt.End = new EventDateTime
                    {
                        DateTime = End,
                        TimeZone = "Australia/Melbourne"//"Etc/UTC"
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
                var test = service.Events.Move(UserCalendar, eventId, destinationCalendar).Execute();
                var updatedEvent = service.Events.Update(evnt, destinationCalendar, eventId).Execute();
                movedEventId = updatedEvent.Id;
                UpdateEventInDB(eventId, UserCalendar);
                InsertReturnedEventDetailsInDB(destinationCalendar, updatedEvent);

            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("Cannot change the organizer of an instance. [400]"))
                {
                    movedEventId = "CustomError400";
                }

                File.AppendAllText("C:\\Google.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
            return movedEventId;
        }

        public void SubscribeCalendars(int orgId)
        {
            Dictionary<string, string> param = new Dictionary<String, String>();
            SetCalendar(orgId,true);

            var calendarList = service.CalendarList.List().Execute().Items.ToList();
            int count = 1;
            foreach (var cal in calendarList)
            {
                Channel request = new Channel();
                request.Id = System.Guid.NewGuid().ToString();
                request.Type = "web_hook";
                request.Params = param;
                if (cal.Id == "campaigntrack.dpi@gmail.com")
                {
                    request.Address = "https://dev.zerofootprint.com.au/notification/Default.aspx";
                }
                else
                {
                    request.Address = "https://dev.zerofootprint.com.au/notification/ChildCalendar.aspx";
                }
                try
                {
                    var changes = service.Events.Watch(request, cal.Id).Execute();//.CalendarList.Events.cal(request, "dpi@gmail.com").Execute();
                }
                catch
                { }
                count++;
            }
        }

        public void InsertGooglePushNotification(string ChannelId, string ChannelToken, DateTime ChannelExpiration, string ResourceId, string ResoourceURI, string ResourceState, long MessageNumber)
        {
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertGooglePushNotification", con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ChannelId", SqlDbType.VarChar).Value = ChannelId;
                        cmd.Parameters.Add("@ChannelToken", SqlDbType.VarChar).Value = ChannelToken;
                        cmd.Parameters.Add("@ChannelExpiration", SqlDbType.DateTime).Value = ChannelExpiration;
                        cmd.Parameters.Add("@ResourceId", SqlDbType.VarChar).Value = ResourceId;
                        cmd.Parameters.Add("@ResourceURI", SqlDbType.VarChar).Value = ResoourceURI;
                        cmd.Parameters.Add("@ResourceState", SqlDbType.VarChar).Value = ResourceState;
                        cmd.Parameters.Add("@MessageNumber", SqlDbType.BigInt).Value = MessageNumber;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public void InsertGooglePushNotificationWatch(int Org_Id, int OAuthId, int CalendarId, Guid ChannelId, string Type, long Expiration, string ResourceId, string ResourceURI, bool Active)
        {
            dic = null;
            try
            {
                string connectionString = null;
                connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertGooglePushNotificationWatch", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Org_Id", SqlDbType.Int).Value = Org_Id;
                        cmd.Parameters.Add("@OAuthId", SqlDbType.Int).Value = OAuthId;
                        cmd.Parameters.Add("@CalendarId", SqlDbType.Int).Value = CalendarId;
                        cmd.Parameters.Add("@ChannelId", SqlDbType.UniqueIdentifier).Value = ChannelId;
                        cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                        cmd.Parameters.Add("@Expiration", SqlDbType.BigInt).Value = Expiration;
                        cmd.Parameters.Add("@ResourceId", SqlDbType.VarChar).Value = ResourceId;
                        cmd.Parameters.Add("@ResourceURI", SqlDbType.VarChar).Value = ResourceURI;
                        cmd.Parameters.Add("@Active", SqlDbType.Bit).Value = Active;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static Dictionary<string, string> dic;
        public Dictionary<string, string> GetChannelInformation()
        {
            if (dic == null)
            {
                dic = new Dictionary<string, string>();
                try
                {
                    string connectionString = null;
                    connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "SELECT resourceID, channelID FROM [CampaignTrack_OMS_Prod].[dbo].[GooglePushNotification]";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            //cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                dic.Add(row[1].ToString(), row[0].ToString());
                            }

                        }
                    }
                }
                catch (Exception ex)
                { }
            }
            return dic;
        }

        public static Dictionary<string, string> CalendarInformation;
        public Dictionary<string, string> GetCalendarInformation()
        {
            if (CalendarInformation == null)
            {
                CalendarInformation = new Dictionary<string, string>();
                try
                {
                    string connectionString = null;
                    connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConn"];
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "SELECT Org_Id, Name FROM [CampaignTrack_OMS_Prod].[dbo].[Calendars]";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            //cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                CalendarInformation.Add(row[1].ToString(), row[0].ToString());
                            }

                        }
                    }
                }
                catch (Exception ex)
                { }

            }
            return CalendarInformation;

        }

        public Coordinates GetCoordinates(string location)
        {
            Coordinates coordinates = new Coordinates();
            GeoCoding.IGeoCoder googleCoder = new GeoCoding.Google.GoogleGeoCoder();
            var address = googleCoder.GeoCode(location);
            if (address != null)
            {
                var firstOrDefaultEntry = address.FirstOrDefault();
                if (firstOrDefaultEntry != null)
                {
                    var coordinateEntry = firstOrDefaultEntry.Coordinates;
                    if (coordinateEntry != null)
                    {
                        coordinates.Latitude = coordinateEntry.Latitude.ToString();
                        coordinates.Longitude = coordinateEntry.Longitude.ToString();// string.Format("{0},{1}", coordinateEntry.Latitude.ToString(), coordinateEntry.Longitude.ToString());
                    }
                }
            }
            return coordinates;
        }

        public void SyncGoogleCalendars(int orgId)
        {
            SetCalendar(orgId);
            //CheckCalendarEvents();

            string a;
            var list = service.CalendarList.List().Execute().Items;
            try
            {

                foreach (var cal in list)
                {
                    // if (cal.Id != "campaigntrack.dpi@gmail.com") { continue; }

                    int result = InsertCalendars(cal.Id, 825, cal.TimeZone, cal.Kind, cal.ETag, int.Parse(cal.ColorId), cal.BackgroundColor, cal.ForegroundColor, cal.AccessRole, cal.Selected ?? false);
                    if (result == 2)
                    {
                        Subscribe(cal, orgId);
                    }
                    var itemlistreq = service.Events.List(cal.Id);
                    // itemlistreq.PageToken = token;
                    itemlistreq.MaxResults = 100000;
                    itemlistreq.ShowDeleted = true;
                    itemlistreq.ShowHiddenInvitations = true;
                    try
                    {
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
                                        InsertCalendarEvents(Calendar, OrderItemId, Title, StartDate, EndDate, MovedOn, Status);

                                    }

                                }
                            }
                        }
                    }
                    catch
                    { }
                    //Here, itemlistreq.PageToken == null;
                }
            }
            catch (Exception ex)
            {
                Email.Email eml = new Email.Email();
                string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                eml.SendMail("Error syncing google calendars Only (Inserting newly added calendar in db)", "Error :- " + strLog, "sachin@zerofootprint.com.au", "", false);


            };

        }

        private int InsertCalendars(string Calendar, int Org_Id, string TimeZone, string Kind, string eTag, int ColorId, string Background, string Foreground, string AccessRole, bool IsSelected)
        {
            int res = 0;
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
                        cmd.Parameters.Add("@TimeZone", SqlDbType.VarChar).Value = TimeZone;
                        cmd.Parameters.Add("@Kind", SqlDbType.VarChar).Value = Kind;
                        cmd.Parameters.Add("@eTag", SqlDbType.VarChar).Value = eTag;
                        cmd.Parameters.Add("@ColorId", SqlDbType.Int).Value = ColorId;
                        cmd.Parameters.Add("@Background", SqlDbType.VarChar).Value = Background;
                        cmd.Parameters.Add("@Foreground", SqlDbType.VarChar).Value = Foreground;
                        cmd.Parameters.Add("@AccessRole", SqlDbType.VarChar).Value = AccessRole;
                        cmd.Parameters.Add("@IsSelected", SqlDbType.Bit).Value = IsSelected;
                        con.Open();
                        res = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex; 
            }
            return res;
        }
        public void AddWildCardDomain(string website, string port, string newDomain, string websiteName)
        {
            WildCardDomain.WildCardDomain wildCardDomain = new WildCardDomain.WildCardDomain(website,port,newDomain,websiteName);
        }


        public Coordinates GetCoordinatesUsingGoogle(string location)
        {
            Coordinates test = new Coordinates();
            string url = "http://maps.google.com/maps/api/geocode/xml?address=" + location + "&sensor=false";
            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    DataSet dsResult = new DataSet();
                    dsResult.ReadXml(reader);
                    foreach (DataRow row in dsResult.Tables["result"].Rows)
                    {
                        string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
                        DataRow locationTable = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
                        test.Latitude = locationTable["lat"].ToString();
                        test.Longitude = locationTable["lng"].ToString();
                    }
                }
            }
            return test;
        }
    }

}
