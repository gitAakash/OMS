using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RestSharp;

namespace OrderManagement.Web.Models.Repository
{

    public interface IMailgunPushNotificationRepository : IDisposable
    {
        int InsertWebhookEmaildelivered(string Parameter_Name, string @event, string recipient, string domain, string ip, string Country, string region, string city, string user_agent, string device_type, string client_type, string client_name, string client_os, string campaign_id, string campaign_name, string tag, string url, string custom_variables, string token, string signature, string message_headers, string code, string error, string notification);

        int InsertWebhookEmailData(string Parameter_Name, string @event, string recipient, string domain, string ip, string Country, string region, string city, string user_agent, string device_type, string client_type, string client_name, string client_os, string campaign_id, string campaign_name, string tag, string url, string custom_variables, string token, string signature, string message_headers, string code, string error, string notification);


        

      //  IRestResponse GetOpenedEmailInfo();
    }

    public class MailgunPushNotificationRepository : IMailgunPushNotificationRepository,IDisposable
    {
        private OrderMgntEntities db = null;


        public MailgunPushNotificationRepository()
        {
            this.db = new OrderMgntEntities();
        }

        public MailgunPushNotificationRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public int InsertWebhookEmaildelivered(string Parameter_Name, string @event, string recipient, string domain, string ip, string Country, 
                                               string region, string city, string user_agent, string device_type, string client_type, string client_name, 
                                               string client_os, string campaign_id, string campaign_name, string tag, string url, string custom_variables, 
                                               string token, string signature, string message_headers, string code, string error, string notification)
        {
            return db.InsertEmailWebHooks(Parameter_Name,  @event,  recipient,  domain,  ip,  Country,  region,  city,  user_agent,  device_type,  client_type,  client_name,  client_os,  campaign_id,  campaign_name,  tag,  url,  custom_variables,  token,  signature,  message_headers,  code,  error,  notification);
        }


        public int InsertWebhookEmailData(string Parameter_Name, string @event, string recipient, string domain, string ip, string Country,
                                              string region, string city, string user_agent, string device_type, string client_type, string client_name,
                                              string client_os, string campaign_id, string campaign_name, string tag, string url, string custom_variables,
                                              string token, string signature, string message_headers, string code, string error, string notification)
        {
            return db.InsertEmailWebHooks(Parameter_Name, @event, recipient, domain, ip, Country, region, city, user_agent, device_type, client_type, client_name, client_os, campaign_id, campaign_name, tag, url, custom_variables, token, signature, message_headers, code, error, notification);
        }



      


    }
}