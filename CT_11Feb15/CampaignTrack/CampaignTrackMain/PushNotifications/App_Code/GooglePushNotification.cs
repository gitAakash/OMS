using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GooglePushNotification
/// </summary>
public class GooglePushNotification
{
    public string ChannelId { get; set; }
    public string ChannelToken { get; set; }
    public DateTime ChannelExpiration { get; set; }
    public string ResourceId { get; set; }
    public string ResoourceURI { get; set; }
    public string ResourceState { get; set; }
    public long MessageNumber { get; set; }
}
public static class Repository
{
    

}