using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Globalization;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Models.ServiceRepository;


namespace OrderManagement.Web.Controllers
{
    public class MailgunController : Controller
    {
        public string ErrorMsg = string.Empty;

        protected override void Dispose(bool disposing)
        {
            _mailgunPushNotificationRepository.Dispose();
            base.Dispose(disposing);
        }

        private IMailgunPushNotificationService _mailgunPushNotification;

        private MailgunPushNotificationRepository _mailgunPushNotificationRepository;

        public MailgunController()
        {
            _mailgunPushNotificationRepository = new MailgunPushNotificationRepository();
            _mailgunPushNotification = new MailgunPushNotificationService(_mailgunPushNotificationRepository);
        }

        public JsonResult GetClickEmail()
        {
            var objclickdata = GetClickEmailinfo();
            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(objclickdata.Content.ToString());
            return Json(objclickdata.Content.ToString(), JsonRequestBehavior.AllowGet); 
        }

        public JsonResult GetOpenedEmailInfo()
        {
           int count =  _mailgunPushNotification.InsertWebhookOpenedEmailInfo();
           return Json(count +  " Records  inserted successfully.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClickedEmailInfo()
        {
            int count = _mailgunPushNotification.InsertWebhookClickedEmailInfo();
            return Json(count + " Records  inserted successfully.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeliveredEmailInfo()
        {
            int count = _mailgunPushNotification.InsertWebhookDeliveredEmailInfo();
            return Json(count + " Records  inserted successfully.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDroppedEmailInfo()
        {
            int count = _mailgunPushNotification.InsertWebhookDroppedEmailInfo();
            return Json(count + " Records  inserted successfully.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUnSubscribeEmailInfo()
        {
            int count = _mailgunPushNotification.InsertWebhookUnsubscribesEmailInfo();
            return Json(count + " Records  inserted successfully.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComplaintsEmailInfo()
        {
            int count = _mailgunPushNotification.InsertWebhookComplaintsEmailInfo();
            return Json(count + " Records  inserted successfully.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHardBounceEmailInfo()
        {
            int count = _mailgunPushNotification.InsertWebhookHardBouncedEmailInfo();
            return Json(count + " Records  inserted successfully.", JsonRequestBehavior.AllowGet);
        }

        #region 



        public JsonResult EmailStack()
        {
            var objstackkdata = GetStatsData();
            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(objstackkdata.Content.ToString());

            foreach (var obj in jsonObj.items)
            {

            }


            return Json(objstackkdata.Content.ToString(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult Emaildelivered()
         {
             var objDeliverdEmail = GetDeliveredEmailInfo_test();


             var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(objDeliverdEmail.Content.ToString());



             foreach (var obj in jsonObj.items)
             {

                 int row_id = Convert.ToInt32(_mailgunPushNotification.InsertWebhookEmaildata("", obj.@event, "testmanoj", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""));

             }

             return Json(objDeliverdEmail.Content.ToString(), JsonRequestBehavior.AllowGet);
         }
        
        public JsonResult GetOpenedEmailInfo_old()
        {
            var objOpendEmail = GetOpenedEmail();

            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(objOpendEmail.Content.ToString());

            foreach (var obj in jsonObj.items)
            {
                String recipient = String.Empty;
                String @event = String.Empty;
                String domain = String.Empty;
                String ipAddress = String.Empty;
                String country = String.Empty;
                String region = String.Empty;
                String city = String.Empty;
                String user_agent = String.Empty;
                String device_type = String.Empty;
                String client_type = String.Empty;
                String client_name = String.Empty;
                String client_os = String.Empty;
                String campaign_id = String.Empty;
                String campaign_name = String.Empty;
                String tag = String.Empty;
                String Url = String.Empty;
                String mailing_list = String.Empty;
                String custom_variables = String.Empty;
                double timestamp ;
                String token = String.Empty;
                String signature = String.Empty;
                String message_headers = String.Empty;

                String code = String.Empty;
                String error = String.Empty;
                String notification = String.Empty;


                @event=obj.@event;
                recipient = obj.recipient;
               
                ipAddress = obj.ip;
                country = obj.geolocation.country;
                region = obj.geolocation.region;
                city = obj.geolocation.city;
                
                user_agent = obj.client_info.user_agent;
                device_type = obj.client_info.device_type;

                client_type = obj.client_info.client_type;
                client_name = obj.client_info.client_name;
                client_os = obj.client_info.client_os;

                timestamp = obj.timestamp;

                int row_id = Convert.ToInt32(_mailgunPushNotification.InsertWebhookEmaildata(
                                            "", @event, recipient, "", ipAddress, country, region, city, user_agent, device_type, client_type, client_name,
                                            client_os, campaign_id, campaign_name, tag, Url,custom_variables,token,signature,message_headers,code,error,notification));
            }

            return Json(objOpendEmail.Content.ToString(), JsonRequestBehavior.AllowGet);
        }

//        public JsonResult Index()
//        {
//            //http://json2csharp.com/#

//            var Stats = GetStats();
//       //  var Clicks = GetDomain();
//       //  var Click1s = AddDomain();


////            string myString = @"
////            {
////                ""total_count"": 95,  
////                ""items"": [
////                    {
////                        ""event"": ""sent"",
////                        ""total_count"": 16,
////                        ""created_at"": ""Tue, 14 Feb 2012 00:00:00 GMT"",
////                        ""id"": ""c56addaa3e1966714d""
////                    },
////                   
////                    {
////                        ""event"": ""sent"",
////                        ""total_count"": 20,
////                        ""created_at"": ""Tue, 14 Feb 2012 00:00:00 GMT"",
////                        ""id"": ""c56addaa3e1966714d""
////                    }
////            ]}";




//         var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(Stats.Content.ToString());

//         foreach (var obj in jsonObj.items)
//         {
         


//         }

//         int count = jsonObj.total_count;

//         return null;


//      //   return Json(Stats, JsonRequestBehavior.AllowGet);

////            return null;
//        }


        public static IRestResponse GetStatsData()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                       new HttpBasicAuthenticator("api",
                                                  "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/stats";
         //   request.AddParameter("event", "sent");
          //  request.AddParameter("event", "opened");
            request.AddParameter("skip", 1);
            request.AddParameter("limit", 3);
            return client.Execute(request);
        }

        public static IRestResponse GetStats1()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/stats";
            request.AddParameter("event", "sent");
            request.AddParameter("event", "opened");
            request.AddParameter("skip", 1);
            request.AddParameter("limit", 1000);
            return client.Execute(request);
        }

        public static IRestResponse GetStats()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "delivered");
          //  request.AddParameter("event", "opened");
          //  request.AddParameter("skip", 1);
            request.AddParameter("limit", 300);
            return client.Execute(request);
        }
        
        public static IRestResponse GetDomain()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "/domains/{domain}/webhooks/click";
            return client.Execute(request);
        }
        
        // for adding the domain dynamically 
        public static IRestResponse AddDomain() 
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2/");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.Resource = "domains/schedly.com.au/webhooks";
            request.AddParameter("id", "click");
            request.AddParameter("url", "http://bin.example.com/8de4a9c4");
            request.Method = Method.POST;
            return client.Execute(request);
        }
        
        public static IRestResponse GetDeliveredEmailInfo_test()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "delivered");
            //  request.AddParameter("event", "opened");
            //  request.AddParameter("skip", 1);
            request.AddParameter("limit", 300);
            return client.Execute(request);
        }

        public static IRestResponse GetOpenedEmail()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "opened");
            //  request.AddParameter("event", "opened");
            //  request.AddParameter("skip", 1);
            request.AddParameter("limit", 300);
            return client.Execute(request);
        }

        public static IRestResponse GetClickEmailinfo()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-91bb4d61267af48a42ee4e1020532c60");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "schedly.com.au", ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "clicked");
            //  request.AddParameter("event", "opened");
            //  request.AddParameter("skip", 1);
            request.AddParameter("limit", 1);
            return client.Execute(request);
        }

    }


    public class DeliveryStatus
    {
        public string message { get; set; }
        public int code { get; set; }
        public object description { get; set; }
        public double session_seconds { get; set; }
    }

    public class Envelope
    {
        public string transport { get; set; }
        public string sender { get; set; }
        public string sending_ip { get; set; }
        public string targets { get; set; }
    }

    public class UserVariables
    {
    }

    public class Flags
    {
        public object is_routed { get; set; }
        public bool is_authenticated { get; set; }
        public bool is_system_test { get; set; }
        public bool is_test_mode { get; set; }
    }

    public class Headers
    {
        public string to { get; set; }
        public string message_id { get; set; }
        public string from { get; set; }
        public string subject { get; set; }
    }

    public class Message
    {
        public Headers headers { get; set; }
        public List<object> attachments { get; set; }
        public List<string> recipients { get; set; }
        public int size { get; set; }
    }

    public class Item
    {
        public List<object> tags { get; set; }
        public DeliveryStatus delivery_status { get; set; }
        public Envelope envelope { get; set; }
        public string recipient_domain { get; set; }
        public string ip { get; set; }
        public List<object> campaigns { get; set; }
        public UserVariables user_variables { get; set; }
        public Flags flags { get; set; }
        public string log_level { get; set; }
        public double timestamp { get; set; }
        public Message message { get; set; }
        public string recipient { get; set; }
        public string @event { get; set; }

        public Geolocation geolocation { get; set; }
        public ClientInfo client_info { get; set; }


    }

    public class Paging
    {
        public string next { get; set; }
        public string last { get; set; }
        public string first { get; set; }
        public string previous { get; set; }
    }

    public class RootObject
    {
        public List<Item> items { get; set; }

        public Paging paging { get; set; }
    }

    public class Geolocation
    {
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
    }

    public class ClientInfo
{
    public string client_os { get; set; }
    public string device_type { get; set; }
    public string client_name { get; set; }
    public string client_type { get; set; }
    public string user_agent { get; set; }
}


    /// <summary>
    /// /////////////////////////////////////////////////////////////
    /// </summary>

    //public class Tags
    //{
    //}

    //public class Item
    //{
    //    public string @event { get; set; }
    //    public int total_count { get; set; }
    //    public string created_at { get; set; }
    //    public string id { get; set; }
    //    public Tags tags { get; set; }
    //}

    //public class RootObject
    //{
    //    public int total_count { get; set; }
    //    public List<Item> items { get; set; }
    //}

    ///////////////////////////////////////////////////////////////
        #endregion
}
