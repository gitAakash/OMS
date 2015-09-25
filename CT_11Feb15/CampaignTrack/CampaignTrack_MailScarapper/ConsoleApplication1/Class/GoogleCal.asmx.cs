using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Linq;

using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;


using Google.Apis.Calendar;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Management;
using Google.Apis.Util.Store;
using System.Threading;

[System.Web.Services.WebService(Namespace = "http://localhost/GoogleCalendarInVBNET/")]
[System.Web.Services.WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ToolboxItem(false)]
public class GoogleCalSvc : System.Web.Services.WebService
{

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






    public string CreateEvent(string Title, string Location, string SalesContact, string StartDate, string EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId)
    {


        // Add the calendar specific scope to the scopes list.
        scopes.Add(CalendarService.Scope.Calendar);

        UserCredential credential = default(UserCredential);

        using (FileStream stream = new FileStream(System.Configuration.ConfigurationSettings.AppSettings["Json"] + "client_secrets.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, scopes, "ZZZZZZZZZZZZ", CancellationToken.None, null).Result;
        }
        String token = credential.Token.AccessToken;
        credential.Token.RefreshToken = token;
        // Create the calendar service using an initializer instance
        BaseClientService.Initializer initializer = new BaseClientService.Initializer();

        initializer.HttpClientInitializer = credential;
        initializer.ApplicationName = "DPI Google Calendar";
        service = new CalendarService(initializer);

        Event evnt = new Event();
        if (ColorId == "")
        { evnt.ColorId = "5"; }
        if (ColorId != "")
        { evnt.ColorId = ColorId; }
        evnt.Start = new EventDateTime
        {
            DateTime = Convert.ToDateTime(StartDate),
            TimeZone = "Australia/Melbourne"
        };
        evnt.End = new EventDateTime
        {
            DateTime = Convert.ToDateTime(EndDate),
            TimeZone = "Australia/Melbourne"
        };

        evnt.Description = "SalesContact : " + SalesContact + Environment.NewLine + Environment.NewLine + "Required On :" + Convert.ToDateTime(RequiredDate).ToShortDateString() + Environment.NewLine + Environment.NewLine + "Product Description :" + ProductDescription + Environment.NewLine + Environment.NewLine + "Supplier Instructions :" + Comments + Environment.NewLine + Environment.NewLine + "Order Item :" + OrderId;
        evnt.Summary = Title;
        evnt.Location = Location;
        service.Events.Insert(evnt, "campaigntrack.dpi@gmail.com").Execute();
        //service.Events.Insert(evnt, "christopher.a.kerr@gmail.com").Execute();
        //  service.Events.Insert(evnt, "sac.nan@gmail.com").Execute();

        return "";

    }


}

