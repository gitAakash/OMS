using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models
{
    public class MailgunPushNotificationModel
    {
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

       // [JsonProperty("client-info")]
        public ClientInfo clientinfo { get; set; }


        public string address { get; set; }
        public string tag { get; set; }
        public string created_at { get; set; }

        public string error { get; set; }
        public int code { get; set; }
       



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
       [JsonProperty("client-os")]
        public string clientos { get; set; }
      //  [JsonProperty("device-type")]
        public string device_type { get; set; }

      //  [JsonProperty("client-name")]

        public string clientname { get; set; }
      //  [JsonProperty("client-name")]
        public string clienttype { get; set; }
      //  [JsonProperty("user-agent")]
        public string useragent { get; set; }
    }

    public class Recipients
    {

    }
    /////////////////////////////////////////////////////

    //public class Geolocation
    //{

    //    [JsonProperty("city")]
    //    public string City { get; set; }

    //    [JsonProperty("region")]
    //    public string Region { get; set; }

    //    [JsonProperty("country")]
    //    public string Country { get; set; }
    //}

    //public class UserVariables
    //{
    //}

    //public class ClientInfo
    //{

    //    [JsonProperty("client-os")]
    //    public string ClientOs { get; set; }

    //    [JsonProperty("device-type")]
    //    public string DeviceType { get; set; }

    //    [JsonProperty("client-name")]
    //    public string ClientName { get; set; }

    //    [JsonProperty("client-type")]
    //    public string ClientType { get; set; }

    //    [JsonProperty("user-agent")]
    //    public string UserAgent { get; set; }
    //}

    //public class Headers
    //{

    //    [JsonProperty("message-id")]
    //    public string MessageId { get; set; }
    //}

    //public class Message
    //{

    //    [JsonProperty("headers")]
    //    public Headers Headers { get; set; }
    //}

    //public class Item
    //{

    //    [JsonProperty("geolocation")]
    //    public Geolocation Geolocation { get; set; }

    //    [JsonProperty("tags")]
    //    public IList<object> Tags { get; set; }

    //    [JsonProperty("ip")]
    //    public string Ip { get; set; }

    //    [JsonProperty("log-level")]
    //    public string LogLevel { get; set; }

    //    [JsonProperty("id")]
    //    public string Id { get; set; }

    //    [JsonProperty("campaigns")]
    //    public IList<object> Campaigns { get; set; }

    //    [JsonProperty("user-variables")]
    //    public UserVariables UserVariables { get; set; }

    //    [JsonProperty("timestamp")]
    //    public double Timestamp { get; set; }

    //    [JsonProperty("client-info")]
    //    public ClientInfo ClientInfo { get; set; }

    //    [JsonProperty("message")]
    //    public Message Message { get; set; }

    //    [JsonProperty("recipient")]
    //    public string Recipient { get; set; }

    //    [JsonProperty("event")]
    //    public string Event { get; set; }
    //}

    //public class Paging
    //{

    //    [JsonProperty("next")]
    //    public string Next { get; set; }

    //    [JsonProperty("last")]
    //    public string Last { get; set; }

    //    [JsonProperty("first")]
    //    public string First { get; set; }

    //    [JsonProperty("previous")]
    //    public string Previous { get; set; }
    //}

    //public class Example
    //{

    //    [JsonProperty("items")]
    //    public IList<Item> Items { get; set; }

    //    [JsonProperty("paging")]
    //    public Paging Paging { get; set; }
    //}

  

 

}