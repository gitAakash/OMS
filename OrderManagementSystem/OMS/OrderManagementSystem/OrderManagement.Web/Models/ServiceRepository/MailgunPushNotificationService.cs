using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using OrderManagement.Web.Models.Repository;
using OrderManagement.Web.Helper.Utilitties;
using RestSharp;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OrderManagement.Web.Models.ServiceRepository
{

    public interface IMailgunPushNotificationService
    {
        int InsertWebhookEmaildata(string Parameter_Name, string @event, string recipient, string domain, string ip, string Country, string region, string city, string user_agent, string device_type, string client_type, string client_name, string client_os, string campaign_id, string campaign_name, string tag, string url, string custom_variables, string token, string signature, string message_headers, string code, string error, string notification);
        int InsertWebhookDeliveredEmailInfo();
        int InsertWebhookDroppedEmailInfo();
        int InsertWebhookHardBouncedEmailInfo();
        int InsertWebhookComplaintsEmailInfo();
        int InsertWebhookUnsubscribesEmailInfo();
        int InsertWebhookClickedEmailInfo();
        int InsertWebhookOpenedEmailInfo();
    }
    public class MailgunPushNotificationService : IMailgunPushNotificationService
    {
        #region Global Variables

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
        double timestamp;
        String token = String.Empty;
        String signature = String.Empty;
        String message_headers = String.Empty;

        String code = String.Empty;
        String error = String.Empty;
        String notification = String.Empty;

        #endregion

        private IMailgunPushNotificationRepository _repository;

        public MailgunPushNotificationService(IMailgunPushNotificationRepository repository)
        {
            _repository = repository;
        }

        public int InsertWebhookDeliveredEmailInfo()
        {
            var DeliveredEmailInfo = GetDeliveredEmail();

            CommonUsedUtilites.LoggerInTextFile("InsertWebhookDeliveredEmailInfo called - " + DateTime.Now, "d:\\MailgunError.txt");

            int row_id = 0;
            int count = 0;

            string strDeliveredEmailInfo = DeliveredEmailInfo.Content.ToString().Replace("client-info", "clientinfo")
                                                                          .Replace("client-os", "clientos")
                                                                          .Replace("device-type", "device_type")
                                                                          .Replace("client-name", "clientname")
                                                                          .Replace("client-type", "clienttype")
                                                                          .Replace("user-agent", "useragent");

            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(strDeliveredEmailInfo);


            if (jsonObj != null && jsonObj.items != null)
            {
                foreach (var obj in jsonObj.items)
                {
                    @event = obj.@event;

                    //recipient = obj.recipient;

                    if (obj.message.recipients != null)
                    {
                        foreach (var item in obj.message.recipients)
                        {
                            recipient = recipient + item.ToString() + ",";
                        }
                    }

                    if (recipient.EndsWith(","))
                        recipient = recipient.Substring(0, recipient.Length - 1);

                    ipAddress = obj.ip;

                    if (obj.geolocation != null)
                    {
                        country = obj.geolocation.country;
                        region = obj.geolocation.region;
                        city = obj.geolocation.city;
                    }

                    if (obj.clientinfo != null)
                    {
                        user_agent = obj.clientinfo.useragent;
                        device_type = obj.clientinfo.device_type;

                        client_type = obj.clientinfo.clienttype;
                        client_name = obj.clientinfo.clientname;
                        client_os = obj.clientinfo.clientos;
                    }

                    timestamp = obj.timestamp;

                    try
                    {
                        row_id = Convert.ToInt32(_repository.InsertWebhookEmailData(
                                             "", @event, recipient, "", ipAddress, country, region, city, user_agent, device_type, client_type, client_name,
                                             client_os, campaign_id, campaign_name, tag, Url, custom_variables, token, signature, message_headers, code, error, notification));

                        count++;
                    }
                    catch (Exception ex)
                    {
                        CommonUsedUtilites.LoggerInTextFile(ex.InnerException.ToString(), "d:\\MailgunError.txt");
                    }
                }
            }

            CommonUsedUtilites.LoggerInTextFile(count + " Records  inserted successfully." + DateTime.Now, "d:\\MailgunError.txt");

            return count;
        }

        public int InsertWebhookDroppedEmailInfo()
        {
            var DroppedEmailInfo = GetDroppedEmail();

            CommonUsedUtilites.LoggerInTextFile("InsertWebhookDroppedEmailInfo called - " + DateTime.Now, "d:\\MailgunError.txt");

            int row_id = 0;
            int count = 0;

            string strDroppedEmailInfo = DroppedEmailInfo.Content.ToString().Replace("client-info", "clientinfo")
                                                                          .Replace("client-os", "clientos")
                                                                          .Replace("device-type", "device_type")
                                                                          .Replace("client-name", "clientname")
                                                                          .Replace("client-type", "clienttype")
                                                                          .Replace("user-agent", "useragent");

            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(strDroppedEmailInfo);


            if (jsonObj != null && jsonObj.items != null)
            {
                foreach (var obj in jsonObj.items)
                {
                    @event = obj.@event;

                    //recipient = obj.recipient;

                    if (obj.message.recipients != null)
                    {
                        foreach (var item in obj.message.recipients)
                        {
                            recipient = recipient + item.ToString() + ",";
                        }
                    }

                    if (recipient.EndsWith(","))
                        recipient = recipient.Substring(0, recipient.Length - 1);

                    ipAddress = obj.ip;

                    if (obj.geolocation != null)
                    {
                        country = obj.geolocation.country;
                        region = obj.geolocation.region;
                        city = obj.geolocation.city;
                    }

                    if (obj.clientinfo != null)
                    {
                        user_agent = obj.clientinfo.useragent;
                        device_type = obj.clientinfo.device_type;

                        client_type = obj.clientinfo.clienttype;
                        client_name = obj.clientinfo.clientname;
                        client_os = obj.clientinfo.clientos;
                    }

                    timestamp = obj.timestamp;

                    try
                    {
                        row_id = Convert.ToInt32(_repository.InsertWebhookEmailData(
                                             "", @event, recipient, "", ipAddress, country, region, city, user_agent, device_type, client_type, client_name,
                                             client_os, campaign_id, campaign_name, tag, Url, custom_variables, token, signature, message_headers, code, error, notification));

                        count++;
                    }
                    catch (Exception ex)
                    {
                        CommonUsedUtilites.LoggerInTextFile(ex.InnerException.ToString(), "d:\\MailgunError.txt");
                    }
                }
            }

            CommonUsedUtilites.LoggerInTextFile(count + " Records  inserted successfully." + DateTime.Now, "d:\\MailgunError.txt");

            return count;
        }

        public int InsertWebhookHardBouncedEmailInfo()
        {
            var ComplaintsEmailInfo = GetHardBouncedEmail();

            CommonUsedUtilites.LoggerInTextFile("InsertWebhookComplaintsEmailInfo called - " + DateTime.Now, "d:\\MailgunError.txt");

            int row_id = 0;
            int count = 0;

            string strComplaintsEmailInfo = ComplaintsEmailInfo.Content.ToString().Replace("client-info", "clientinfo")
                                                                          .Replace("client-os", "clientos")
                                                                          .Replace("device-type", "device_type")
                                                                          .Replace("client-name", "clientname")
                                                                          .Replace("client-type", "clienttype")
                                                                          .Replace("user-agent", "useragent");

            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(strComplaintsEmailInfo);


            if (jsonObj != null && jsonObj.items != null)
            {
                foreach (var obj in jsonObj.items)
                {
                    @event = "bounced";

                    //recipient = obj.recipient;

                    code = obj.code.ToString();
                    error = obj.error;
                    recipient = obj.address;

                    try
                    {
                        row_id = Convert.ToInt32(_repository.InsertWebhookEmailData(
                                             "", @event, recipient, "", ipAddress, country, region, city, user_agent, device_type, client_type, client_name,
                                             client_os, campaign_id, campaign_name, tag, Url, custom_variables, token, signature, message_headers, code, error, notification));

                        count++;
                    }
                    catch (Exception ex)
                    {
                        CommonUsedUtilites.LoggerInTextFile(ex.InnerException.ToString(), "d:\\MailgunError.txt");
                    }
                }
            }

            CommonUsedUtilites.LoggerInTextFile(count + " Records  inserted successfully." + DateTime.Now, "d:\\MailgunError.txt");

            return count;
        }

        public int InsertWebhookComplaintsEmailInfo()
        {
            var ComplaintsEmailInfo = GetComplaintsEmail();

            CommonUsedUtilites.LoggerInTextFile("InsertWebhookComplaintsEmailInfo called - " + DateTime.Now, "d:\\MailgunError.txt");

            int row_id = 0;
            int count = 0;

            string strComplaintsEmailInfo = ComplaintsEmailInfo.Content.ToString().Replace("client-info", "clientinfo")
                                                                          .Replace("client-os", "clientos")
                                                                          .Replace("device-type", "device_type")
                                                                          .Replace("client-name", "clientname")
                                                                          .Replace("client-type", "clienttype")
                                                                          .Replace("user-agent", "useragent");

            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(strComplaintsEmailInfo);


            if (jsonObj != null && jsonObj.items != null)
            {
                foreach (var obj in jsonObj.items)
                {
                    @event = "Complaints";

                    //recipient = obj.recipient;


                    recipient = obj.address;
                   // tag = obj.tag;

                    try
                    {
                        row_id = Convert.ToInt32(_repository.InsertWebhookEmailData(
                                             "", @event, recipient, "", ipAddress, country, region, city, user_agent, device_type, client_type, client_name,
                                             client_os, campaign_id, campaign_name, tag, Url, custom_variables, token, signature, message_headers, code, error, notification));

                        count++;
                    }
                    catch (Exception ex)
                    {
                        CommonUsedUtilites.LoggerInTextFile(ex.InnerException.ToString(), "d:\\MailgunError.txt");
                    }
                }
            }

            CommonUsedUtilites.LoggerInTextFile(count + " Records  inserted successfully." + DateTime.Now, "d:\\MailgunError.txt");

            return count;
        }

        public int InsertWebhookUnsubscribesEmailInfo()
        {
            var UnsubscribeEmailInfo = GetUnsubscribesEmail();


            //                        string myString = @"
            //                        {
            //                            ""total_count"": 95,  
            //                            ""items"": [
            //                                {
            //                                    ""address"": ""sent@test.com"",
            //                                    ""tag"": ""*"",
            //                                    ""created_at"": ""Tue, 14 Feb 2012 00:00:00 GMT""
            //                                    
            //                                },
            //                               
            //                                {
            //                                    ""address"": ""sent@test.com"",
            //                                    ""tag"": ""*"",
            //                                    ""created_at"": ""Tue, 14 Feb 2012 00:00:00 GMT""
            //                                    
            //                                }
            //                        ]}";




            CommonUsedUtilites.LoggerInTextFile("InsertWebhookUnsubscribesEmailInfo called - " + DateTime.Now, "d:\\MailgunError.txt");

            int row_id = 0;
            int count = 0;

            string strDroppedEmailInfo = UnsubscribeEmailInfo.Content.ToString().Replace("client-info", "clientinfo")
                                                                          .Replace("client-os", "clientos")
                                                                          .Replace("device-type", "device_type")
                                                                          .Replace("client-name", "clientname")
                                                                          .Replace("client-type", "clienttype")
                                                                          .Replace("user-agent", "useragent");

            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(strDroppedEmailInfo);


            if (jsonObj != null && jsonObj.items != null)
            {
                foreach (var obj in jsonObj.items)
                {
                    @event = "Unsubscribes";

                    //recipient = obj.recipient;


                    recipient = obj.address;
                    tag = obj.tag;

                    try
                    {
                        row_id = Convert.ToInt32(_repository.InsertWebhookEmailData(
                                             "", @event, recipient, "", ipAddress, country, region, city, user_agent, device_type, client_type, client_name,
                                             client_os, campaign_id, campaign_name, tag, Url, custom_variables, token, signature, message_headers, code, error, notification));

                        count++;
                    }
                    catch (Exception ex)
                    {
                        CommonUsedUtilites.LoggerInTextFile(ex.InnerException.ToString(), "d:\\MailgunError.txt");
                    }
                }
            }

            CommonUsedUtilites.LoggerInTextFile(count + " Records  inserted successfully." + DateTime.Now, "d:\\MailgunError.txt");

            return count;
        }

        public int InsertWebhookClickedEmailInfo()
        {
            var OpenedEmailInfo = GetClickedEmail();

            CommonUsedUtilites.LoggerInTextFile("InsertWebhookClickedEmailInfo called - " + DateTime.Now, "d:\\MailgunError.txt");
            //delivered
            int row_id = 0;
            int count = 0;

            string strOpenedEmailInfo = OpenedEmailInfo.Content.ToString().Replace("client-info", "clientinfo")
                                                                          .Replace("client-os", "clientos")
                                                                          .Replace("device-type", "device_type")
                                                                          .Replace("client-name", "clientname")
                                                                          .Replace("client-type", "clienttype")
                                                                          .Replace("user-agent", "useragent");

            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(strOpenedEmailInfo);


            if (jsonObj != null && jsonObj.items != null)
            {
                foreach (var obj in jsonObj.items)
                {
                    @event = obj.@event;
                    recipient = obj.recipient;

                    ipAddress = obj.ip;

                    if (obj.geolocation != null)
                    {
                        country = obj.geolocation.country;
                        region = obj.geolocation.region;
                        city = obj.geolocation.city;
                    }

                    if (obj.clientinfo != null)
                    {
                        user_agent = obj.clientinfo.useragent;
                        device_type = obj.clientinfo.device_type;

                        client_type = obj.clientinfo.clienttype;
                        client_name = obj.clientinfo.clientname;
                        client_os = obj.clientinfo.clientos;
                    }

                    timestamp = obj.timestamp;

                    try
                    {
                        row_id = Convert.ToInt32(_repository.InsertWebhookEmailData(
                                             "", @event, recipient, "", ipAddress, country, region, city, user_agent, device_type, client_type, client_name,
                                             client_os, campaign_id, campaign_name, tag, Url, custom_variables, token, signature, message_headers, code, error, notification));

                        count++;
                    }
                    catch (Exception ex)
                    {
                        CommonUsedUtilites.LoggerInTextFile(ex.InnerException.ToString(), "d:\\MailgunError.txt");
                    }
                }
            }

            CommonUsedUtilites.LoggerInTextFile(count + " Records  inserted successfully." + DateTime.Now, "d:\\MailgunError.txt");

            return count;
        }

        public int InsertWebhookOpenedEmailInfo()
        {
            var OpenedEmailInfo = GetOpenedEmail();

            CommonUsedUtilites.LoggerInTextFile("InsertWebhookOpenedEmailInfo called - " + DateTime.Now, "d:\\MailgunError.txt");

            int row_id = 0;
            int count = 0;

            string strOpenedEmailInfo = OpenedEmailInfo.Content.ToString().Replace("client-info", "clientinfo")
                                                                          .Replace("client-os", "clientos")
                                                                          .Replace("device-type", "device_type")
                                                                          .Replace("client-name", "clientname")
                                                                          .Replace("client-type", "clienttype")
                                                                          .Replace("user-agent", "useragent");

            var jsonObj = new JavaScriptSerializer().Deserialize<RootObject>(strOpenedEmailInfo);


            if (jsonObj != null && jsonObj.items != null)
            {
                foreach (var obj in jsonObj.items)
                {
                    @event = obj.@event;
                    recipient = obj.recipient;

                    ipAddress = obj.ip;

                    if (obj.geolocation != null)
                    {
                        country = obj.geolocation.country;
                        region = obj.geolocation.region;
                        city = obj.geolocation.city;
                    }

                    if (obj.clientinfo != null)
                    {
                        user_agent = obj.clientinfo.useragent;
                        device_type = obj.clientinfo.device_type;

                        client_type = obj.clientinfo.clienttype;
                        client_name = obj.clientinfo.clientname;
                        client_os = obj.clientinfo.clientos;
                    }

                    timestamp = obj.timestamp;

                    try
                    {
                        row_id = Convert.ToInt32(_repository.InsertWebhookEmailData(
                                             "", @event, recipient, "", ipAddress, country, region, city, user_agent, device_type, client_type, client_name,
                                             client_os, campaign_id, campaign_name, tag, Url, custom_variables, token, signature, message_headers, code, error, notification));

                        count++;
                    }
                    catch (Exception ex)
                    {
                        CommonUsedUtilites.LoggerInTextFile(ex.InnerException.ToString(), "d:\\MailgunError.txt");
                    }
                }
            }

            CommonUsedUtilites.LoggerInTextFile(count + " Records  inserted successfully." + DateTime.Now, "d:\\MailgunError.txt");

            return count;
        }

        /// <summary>
        /// This function is used for getting Delivered Email info form MailGun
        /// </summary>
        /// <returns></returns>
        public static IRestResponse GetDeliveredEmail()
        {
            string strMailGunAPIKey = string.Empty;
            string strMailGunDomain = string.Empty;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"]))
            {
                strMailGunAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"];
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"]))
            {
                strMailGunDomain = System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"];
            }

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", strMailGunAPIKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", strMailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "delivered");
            //  request.AddParameter("skip", 1);
            request.AddParameter("limit", 1);
            return client.Execute(request);
        }

        public static IRestResponse GetDroppedEmail()
        {
            string strMailGunAPIKey = string.Empty;
            string strMailGunDomain = string.Empty;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"]))
            {
                strMailGunAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"];
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"]))
            {
                strMailGunDomain = System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"];
            }

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", strMailGunAPIKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", strMailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "dropped");
          //  request.AddParameter("limit", 1);
            return client.Execute(request);
        }

        public static IRestResponse GetHardBouncedEmail()
        {
            string strMailGunAPIKey = string.Empty;
            string strMailGunDomain = string.Empty;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"]))
            {
                strMailGunAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"];
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"]))
            {
                strMailGunDomain = System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"];
            }

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", strMailGunAPIKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", strMailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/bounces";
            //request.AddParameter("limit", 1);
            return client.Execute(request);
        }

        public static IRestResponse GetComplaintsEmail()
        {
            string strMailGunAPIKey = string.Empty;
            string strMailGunDomain = string.Empty;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"]))
            {
                strMailGunAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"];
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"]))
            {
                strMailGunDomain = System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"];
            }

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", strMailGunAPIKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", strMailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/complaints";
          //  request.AddParameter("limit", 1);
            return client.Execute(request);
        }

        public static IRestResponse GetUnsubscribesEmail()
        {
            string strMailGunAPIKey = string.Empty;
            string strMailGunDomain = string.Empty;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"]))
            {
                strMailGunAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"];
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"]))
            {
                strMailGunDomain = System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"];
            }

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", strMailGunAPIKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", strMailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/unsubscribes";
            // request.AddParameter("event", "unsubscribes");
            //  request.AddParameter("skip", 1);
          //  request.AddParameter("limit", 1);
            return client.Execute(request);
        }

        /// <summary>
        /// This function is used for getting clicked Email info form MailGun
        /// </summary>
        /// <returns></returns>
        public static IRestResponse GetClickedEmail()
        {

            string strMailGunAPIKey = string.Empty;
            string strMailGunDomain = string.Empty;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"]))
            {
                strMailGunAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"];
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"]))
            {
                strMailGunDomain = System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"];
            }

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", strMailGunAPIKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", strMailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "clicked");
            //  request.AddParameter("skip", 1);
            request.AddParameter("limit", 1);
            return client.Execute(request);
        }

        /// <summary>
        /// This function is used for getting opened Email info form MailGun
        /// </summary>
        /// <returns></returns>

        public static IRestResponse GetOpenedEmail()
        {

            string strMailGunAPIKey = string.Empty;
            string strMailGunDomain = string.Empty;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"]))
            {
                strMailGunAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"];
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"]))
            {
                strMailGunDomain = System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"];
            }

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", strMailGunAPIKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", strMailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "opened");
            //  request.AddParameter("skip", 1);
            request.AddParameter("limit", 1);
            return client.Execute(request);
        }

        /// <summary>
        /// This function is used for getting Delivered Email info form MailGun
        /// </summary>
        /// <returns></returns>
        public static IRestResponse GetFailedEmail()
        {
            string strMailGunAPIKey = string.Empty;
            string strMailGunDomain = string.Empty;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"]))
            {
                strMailGunAPIKey = System.Configuration.ConfigurationManager.AppSettings["MailGunAPIKey"];
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"]))
            {
                strMailGunDomain = System.Configuration.ConfigurationManager.AppSettings["MailGunDomain"];
            }

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator = new HttpBasicAuthenticator("api", strMailGunAPIKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", strMailGunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("event", "failed");
            //  request.AddParameter("skip", 1);
            request.AddParameter("limit", 1);
            return client.Execute(request);
        }

        public int InsertWebhookEmaildata(string Parameter_Name, string @event, string recipient, string domain, string ip, string Country, string region, string city, string user_agent, string device_type, string client_type, string client_name, string client_os, string campaign_id, string campaign_name, string tag, string url, string custom_variables, string token, string signature, string message_headers, string code, string error, string notification)
        {
            int identity = 0;

            var test =
            identity = _repository.InsertWebhookEmailData(Parameter_Name, @event, recipient, domain, ip, Country, region, city, user_agent, device_type, client_type, client_name, client_os, campaign_id, campaign_name, tag, url, custom_variables, token, signature, message_headers, code, error, notification);
            return identity;
        }
        
    }
}