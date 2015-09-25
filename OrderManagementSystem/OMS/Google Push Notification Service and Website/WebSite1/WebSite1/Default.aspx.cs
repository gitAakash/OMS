using DataLayer;
using System;
using System.Collections.Generic;
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
    private static TcpListener _listener;
    private static TcpClient _client;
    private static NetworkStream _stream;

    private static bool? _handshakeEstablished = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        string headerstring = DateTime.Now.ToString() + Environment.NewLine;
        if (WebOperationContext.Current != null)
        {
            var request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;

            headerstring = "reading Headers" + Environment.NewLine + Environment.NewLine;
            foreach (string headerName in headers.AllKeys)
            {

                headerstring = headerstring + Environment.NewLine + headerName + ": " + headers[headerName];
            }
            headerstring = headerstring + Environment.NewLine + Environment.NewLine + "End reading Headers";
        }
        else
        {
            headerstring = headerstring + "-------------------------------------------------------" + Environment.NewLine + Environment.NewLine;
            headerstring = headerstring + "No Incoming request";
            headerstring = headerstring + Environment.NewLine + Environment.NewLine + "-------------------------------------------------------";
        }

        if (Request.Form.Keys != null)
        {
            string keyString = "Key Count : " + Request.Form.Keys.Count;
            var keys = Request.Form.AllKeys;//.Keys;
            for (int i = 0; i < keys.Length; i++)
            {
                keyString = keyString + Environment.NewLine + keys[i];
                //Response.Write("Form: " + keys[i] + "<br>");
            }
            //foreach (var key in keys)
            //{

            //    keyString = keyString + Environment.NewLine + key;
            //}
            headerstring = headerstring + Environment.NewLine + keyString;
        }
        else
        {
            headerstring = headerstring + Environment.NewLine + "No Key Found";
        }

        var oSR = new StreamReader(Request.InputStream);
        string sContent = oSR.ReadToEnd();

        headerstring = headerstring + Environment.NewLine + "Content Stream : " + sContent;

        var c = HttpContext.Current;


        string opstedData = c.Request.Headers.AllKeys.Count().ToString();//["AP"];
        headerstring = headerstring + Environment.NewLine + "Header count : " + opstedData;

        string reqheaders = string.Empty;
        var actualRequest = c.Request;
        var reqKeys = c.Request.Headers.AllKeys;
        string calId = string.Empty;
        for (int i = 0; i < reqKeys.Count(); i++)
        {
            if (reqKeys[i] == "X-Goog-Resource-URI")
            {
                calId = c.Request.Headers.Get(i).Replace("https://www.googleapis.com/calendar/v3/calendars/", string.Empty);
                calId = calId.Replace("/events?alt=json", string.Empty);
            }
            reqheaders = reqheaders + Environment.NewLine + reqKeys[i] + " : " + c.Request.Headers.Get(i);//actualRequest.ServerVariables[reqKeys[i]];
        }

        headerstring = headerstring + Environment.NewLine + "Headers : " + reqheaders + Environment.NewLine + "Calendar Id : " + calId;
        try
        {
            File.WriteAllText("C://test.txt", headerstring);
        }
        catch
        { }

        ServiceReference1.IGoogleNotificationService client = new ServiceReference1.GoogleNotificationServiceClient();
        client.CheckCalendarEvents(calId);
        //"Test Return Evnt Id 3453454", "", "", 65k7ohoi932gq7jolftmsje4r4 "FREQ=DAILY;COUNT=6" "9-26-2014 08:30:00"
        //client.DeleteEvent("1cneitqak06kpq9502mnirs8v8", "dpi.campaigntrack@gmail.com");//.CreateEvent("All Day ABC", "", "",DateTime.Now.ToString() , "9-26-2014 09:30:00", "", "", "", "", "", "dpi.campaigntrack@gmail.com", "Desc", "",false);//"FREQ=DAILY;COUNT=1");//.DeleteEvent("65k7ohoi932gq7jolftmsje4r4", "dpi.campaigntrack@gmail.com");//(DateTime.Now.ToString(), DateTime.Now.AddHours(1).ToString(), "Sac nan", "Sac nan", "", "Test Return Evnt Id 3453454 fgdfgdgdfg fsfsdf", "65k7ohoi932gq7jolftmsje4r4", "dpi.campaigntrack@gmail.com");//,new List<string>().ToArray());//.CheckCalendarEvents("dpi.campaigntrack@gmail.com");//ht642k01401dikgup7pk3j4l0s@group.calendar.google.com
        headerstring = headerstring + "DB Entry done.";
        try
        {
            File.WriteAllText("C://test.txt", headerstring);
        }
        catch
        { }
    }    
}