using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Google.Apis.Util.Store;
using Google.Apis.Services;
namespace Calender_Test
{
    class Program
    {
        static IList<string> scopes = new List<string>();
        //' Calendar service.

        static CalendarService service;

        static void Main(string[] args)
        {
            string consoleapp = @"C:\Users\swapnil.gade\Desktop\Gun Freight\ConsoleApplication2\ConsoleApplication2\bin\Debug";


            Dictionary<string, string> param = new Dictionary<String, String>();
            SetCalendar();

            var calendarList = service.CalendarList.List().Execute().Items.ToList();
            
            foreach (var cal in calendarList)
            {
                Channel request = new Channel();
                request.Id = System.Guid.NewGuid().ToString();
                request.Type = "web_hook";
                request.Params = param;
                if (cal.Id == "dpi.campaigntrack@gmail.com")
                {
                    request.Address = "https://dev.zerofootprint.com.au/notification/Default.aspx";
                }
                else
                {
                    request.Address = "https://dev.zerofootprint.com.au/notification/ChildCalendar.aspx";
                }
                var changes = service.Events.Watch(request, cal.Id).Execute();//.CalendarList.Events.cal(request, "dpi@gmail.com").Execute();
            }
            //IList<CalendarListEntry> list = service.CalendarList.List().Execute().Items;
            //DisplayList(list);
            //foreach (Google.Apis.Calendar.v3.Data.CalendarListEntry calendar in list)
            //{
            //    // Display calendar's events
            //    DisplayFirstCalendarEvents(calendar);
            //}
            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
            //CreateEvent();

            //CheckCalendarEvents("ht642k01401dikgup7pk3j4l0s@group.calendar.google.com");
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

        /// <summary>Displays all calendars.</summary>
        private static void DisplayList(IList<CalendarListEntry> list)
        {
            Console.WriteLine("Lists of calendars:");
            foreach (CalendarListEntry item in list)
            {
                Console.WriteLine("ID : " + item.Id);
            }
        }

        public static void SetCalendar()
        {
            try
            {
                scopes.Add(CalendarService.Scope.Calendar);
                UserCredential credential = default(UserCredential);
                FileDataStore _fdsToken = new FileDataStore("CT");
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
                credential.Token.RefreshToken = "1/wU76ItFh-n64iqZLjIiXER0Vx4u-MxEnHtHFLgQpa7E";                                                                                                   
                credential.Token.ExpiresInSeconds = 500000;
                // Create the calendar service using an initializer instance
                BaseClientService.Initializer initializer = new BaseClientService.Initializer();
                initializer.HttpClientInitializer = credential;
                initializer.ApplicationName = "DPI Google Calendar";
                service = new CalendarService(initializer);
            }
            catch (Exception ex)
            {

            }

        }


        public static void CheckCalendarEvents(string calendarId)
        {

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
                  //      InsertOtherEvents(Calendar, EventId, RecurringEventId, ColorId, Attendes, AttendeesOmitted, AnyoneCanAddSelf, Locked, Title, Location, Description, Kind, StartDate, EndDate, MovedOn, Status, Recurrences, Organizer, Creator, Sequence, Transperancy, Updated, Visibility);


                    }


                }
            }
            catch (Exception ex)
            {
                //Email.Email eml = new Email.Email();
                string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
                //eml.SendMail("Error syncing google calendar", "Error :- " + strLog, "sachin@zerofootprint.com.au", "", false);


            };

        }

        public static string CreateEvent()//string Title, string Location, string SalesContact, string StartDate, string EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId)
        {
            SetCalendar();
            Event evnt = new Event();
            
            //if (ColorId == "")
            //{ 
                evnt.ColorId = "5"; 
            //}
            //if (ColorId != "")
            //{ evnt.ColorId = ColorId; }
            evnt.Start = new EventDateTime
            {
                DateTime = DateTime.Now.AddDays(1),// Convert.ToDateTime(StartDate),
                TimeZone = "Australia/Melbourne"
            };
            evnt.End = new EventDateTime
            {
                DateTime = DateTime.Now.AddDays(1).AddHours(3),// Convert.ToDateTime(EndDate),
                TimeZone = "Australia/Melbourne"
            };


            //evnt.Description = "SalesContact : " + SalesContact + Environment.NewLine + Environment.NewLine + "Required On :" + Convert.ToDateTime(RequiredDate).ToShortDateString() + Environment.NewLine + Environment.NewLine + "Product Description :" + ProductDescription + Environment.NewLine + Environment.NewLine + "Supplier Instructions :" + Comments.Replace("Supplier Instructions", "") + Environment.NewLine + Environment.NewLine + "Order Item (Please donot delete or update OrderItemId) :" + OrderId;
            evnt.Summary = "Test app";
            evnt.Location = "";
            service.Events.Insert(evnt, "ht642k01401dikgup7pk3j4l0s@group.calendar.google.com").Execute();
            //service.Events.Insert(evnt, "christopher.a.kerr@gmail.com").Execute();
            //  service.Events.Insert(evnt, "sac.nan@gmail.com").Execute();


            return "";

        }


        public static void ThreadWork()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }
    }
}
