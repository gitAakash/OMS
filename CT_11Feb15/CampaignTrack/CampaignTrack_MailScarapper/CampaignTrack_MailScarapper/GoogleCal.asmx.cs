﻿using Microsoft.VisualBasic;
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


    public int CheckedCalendarEvent = 0;


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


    public static void SetCalendar()
    {
        scopes.Add(CalendarService.Scope.Calendar);
        UserCredential credential = default(UserCredential);
        FileDataStore _fdsToken = new FileDataStore("CT");
        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
          new ClientSecrets
          {
              ClientId = "466513012782-ubgcnh2c3f25iprbhbmisb2l4i8965lf.apps.googleusercontent.com",
              ClientSecret = "JQT2RdjIoMjHuevYwfyFnefx",

          }, scopes,
          "ZZZZZZZZZZZZZZZZZZZ",
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



    public string CreateEvent(string Title, string Location, string SalesContact, string StartDate, string EndDate, string Comments, string ProductDescription, string OrderId, string RequiredDate, string ColorId)
    {
        SetCalendar();
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


        evnt.Description = "SalesContact : " + SalesContact + Environment.NewLine + Environment.NewLine + "Required On :" + Convert.ToDateTime(RequiredDate).ToShortDateString() + Environment.NewLine + Environment.NewLine + "Product Description :" + ProductDescription + Environment.NewLine + Environment.NewLine + "Supplier Instructions :" + Comments.Replace("Supplier Instructions", "") + Environment.NewLine + Environment.NewLine + "Order Item (Please donot delete or update OrderItemId) :" + OrderId;
        evnt.Summary = Title;
        evnt.Location = Location;
        service.Events.Insert(evnt, "campaigntrack.dpi@gmail.com").Execute();
        //service.Events.Insert(evnt, "christopher.a.kerr@gmail.com").Execute();
        //  service.Events.Insert(evnt, "sac.nan@gmail.com").Execute();


        return "";

    }

    public static void CheckCalendarMovements()
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
                InsertCalendars(cal.Id);
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
                                InsertCalendarEvents(Calendar, OrderItemId, Title, StartDate, EndDate, MovedOn, Status);

                            }

                        }
                    }
                }

                //Here, itemlistreq.PageToken == null;
            }
        }
        catch (Exception ex)
        {
            Email.Email eml = new Email.Email();
            string strLog = ex.StackTrace.ToString() + "~~~~~~~~~~~~Error~~~~~~~~~~~:" + ex.Message.ToString();
            eml.SendMail("Error syncing google calendar", "Error :- " + strLog, "sachin@zerofootprint.com.au", "", false);


        };

    }

    public static void CheckCalendarEvents()
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

    public String[] Splits(string values, char[] character)
    {


        string[] strSplitArr = values.Split(character);
        return strSplitArr;

    }

    public static void InsertCalendarEvents(string Calendar, string OrderId, string Title, DateTime StartDate, DateTime EndDate, DateTime MovedOn, string Status)
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

    public static void InsertOtherEvents(string Calendar, string EventId, string RecurringEventId, string ColorId, string Attendees, bool? AttendeesOmitted, bool? AnyoneCanAddSelf, bool? Locked, string Title, string Location, string Description, string Kind, DateTime StartDate, DateTime EndDate, DateTime MovedOn, string Status, string Recurrence, string Organizer, string Creator, int Sequence, string Transperancy, DateTime Updated, string Visibility)
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

    public static void InsertCalendars(string Calendar)
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
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        catch (Exception ex) { throw ex; }

    }




}

