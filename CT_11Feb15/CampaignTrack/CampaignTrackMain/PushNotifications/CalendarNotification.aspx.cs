﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CalendarNotification : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var context = HttpContext.Current;
        string reqheaders = string.Empty;
        var actualRequest = context.Request;
        var reqKeys = context.Request.Headers.AllKeys;
        string calId = string.Empty;
        bool isValid = false;
        var googlePushNotification = new GooglePushNotification();
        for (int i = 0; i < reqKeys.Count(); i++)
        {
            //if (reqKeys[i] == "X-Goog-Resource-URI")
            //{
            //    googlePushNotification.ResoourceURI = context.Request.Headers.Get(i);
            //    calId = context.Request.Headers.Get(i).Replace("https://www.googleapis.com/calendar/v3/calendars/", string.Empty);
            //    calId = calId.Replace("/events?alt=json", string.Empty);
            //    isValid = true;
            //}
            //else if (reqKeys[i] == "X-Goog-Channel-ID")
            //{
            //    googlePushNotification.ChannelId = context.Request.Headers.Get(i).ToString();
            //}
            //else if (reqKeys[i] == "X-Goog-Channel-Expiration")
            //{
            //    googlePushNotification.ChannelExpiration = DateTime.Parse(context.Request.Headers.Get(i).ToString().Replace("GMT", string.Empty).Trim());
            //}
            //else if (reqKeys[i] == "X-Goog-Resource-State")
            //{
            //    googlePushNotification.ResourceState = context.Request.Headers.Get(i).ToString();
            //}
            //else if (reqKeys[i] == "X-Goog-Message-Number")
            //{
            //    googlePushNotification.MessageNumber = long.Parse(context.Request.Headers.Get(i).ToString());
            //}
            //else if (reqKeys[i] == "X-Goog-Resource-ID")
            //{
            //    googlePushNotification.ResourceId = context.Request.Headers.Get(i).ToString();
            //}
            reqheaders = reqheaders + Environment.NewLine + reqKeys[i] + " : " + context.Request.Headers.Get(i);
        }
        try
        {
            File.WriteAllText("C://Calendar.txt", reqheaders);
        }
        catch
        { }
        if (isValid)
        {
            try
            {
                //GoogleService.IGoogleNotificationService client = new GoogleService.GoogleNotificationServiceClient();
                //client.CheckCalendarEvents(calId);
                //client.InsertGooglePushNotification(googlePushNotification.ChannelId, "", googlePushNotification.ChannelExpiration, googlePushNotification.ResourceId, googlePushNotification.ResoourceURI, googlePushNotification.ResourceState, googlePushNotification.MessageNumber);
            }
            catch (Exception ex)
            {
                File.WriteAllText("C://Google.txt", ex.Message);
            }
        }
    }
}