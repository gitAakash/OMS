using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {        
        
        var context = HttpContext.Current;
        string reqheaders = "reached";
        var actualRequest = context.Request;
        var reqKeys = context.Request.Headers.AllKeys;
        string calId = string.Empty;
        bool isValid = false;
        GoogleService.IGoogleNotificationService client1 = new GoogleService.GoogleNotificationServiceClient();
        client1.CheckAllCalendarEvents(825);//.CheckCalendarEvents(calId);
        //ServiceReference1.IGoogleNotificationService client1 = new ServiceReference1.GoogleNotificationServiceClient();
        //client1.CheckCalendarEvents("dpi.campaigntrack@gmail.com");
        var googlePushNotification = new GooglePushNotification();
        for (int i = 0; i < reqKeys.Count(); i++)
        {
            if (reqKeys[i] == "X-Goog-Resource-URI")
            {
                googlePushNotification.ResoourceURI = context.Request.Headers.Get(i).ToString();
                calId = context.Request.Headers.Get(i).Replace("https://www.googleapis.com/calendar/v3/calendars/", string.Empty);
                calId = calId.Replace("/events?alt=json", string.Empty);
                isValid = true;
            }
            else if (reqKeys[i] == "X-Goog-Channel-ID")
            {
                googlePushNotification.ChannelId = context.Request.Headers.Get(i).ToString();
            }
            else if (reqKeys[i] == "X-Goog-Channel-Expiration")
            {
                googlePushNotification.ChannelExpiration = DateTime.Parse(context.Request.Headers.Get(i).ToString().Replace("GMT",string.Empty).Trim());
            }
            else if (reqKeys[i] == "X-Goog-Resource-State")
            {
                googlePushNotification.ResourceState = context.Request.Headers.Get(i).ToString();
            }
            else if (reqKeys[i] == "X-Goog-Message-Number")
            {
                googlePushNotification.MessageNumber = long.Parse(context.Request.Headers.Get(i).ToString());
            }
            else if (reqKeys[i] == "X-Goog-Resource-ID")
            {
                googlePushNotification.ResourceId = context.Request.Headers.Get(i).ToString();
            }            
            reqheaders = reqheaders + Environment.NewLine + reqKeys[i] + " : " + context.Request.Headers.Get(i);
        }

        //bool validChannel = false;
        //try
        //{
        //    if (activeChannels.Keys.Contains(googlePushNotification.ChannelId) || !activeChannels.Values.Contains(googlePushNotification.ResourceId))
        //    {
        //        validChannel = true;
        //    }
        //}
        //catch
        //{ 
        //}

        try
        {
            //if(validChannel)
            File.WriteAllText(@"C:\testDev.txt", reqheaders);
        }
        catch
        { }
        //isValid = true;
        //calId = "dpi.campaigntrack@gmail.com";
        if (isValid)
        {
            try
            {            
                if (isValid)
                {
                    GoogleService.IGoogleNotificationService client = new GoogleService.GoogleNotificationServiceClient();
                    client.CheckCalendarEvents(calId);
                    client.InsertGooglePushNotification(googlePushNotification.ChannelId, "", googlePushNotification.ChannelExpiration, googlePushNotification.ResourceId, googlePushNotification.ResoourceURI, googlePushNotification.ResourceState, googlePushNotification.MessageNumber);
                    //context.Response.StatusCode = 200;
                    //context.Response.End();
                    //return;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(@"C:\Google.txt", ex.Message);
            }
        }
    }
}

