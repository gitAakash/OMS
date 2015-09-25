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

public partial class ChildCalendar : System.Web.UI.Page
{
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
            File.WriteAllText("C://Child.txt", headerstring);
        }
        catch
        { }
        //GoogleService.IGoogleNotificationService client = new GoogleService.GoogleNotificationServiceClient();
        //client.CheckCalendarEvents(calId);
        headerstring = headerstring + "DB Entry done.";
        try
        {
            File.WriteAllText("C://Child.txt", headerstring);
        }
        catch
        { }
    }
}