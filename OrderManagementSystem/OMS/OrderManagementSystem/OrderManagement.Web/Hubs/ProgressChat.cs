using System;
using Microsoft.AspNet.SignalR.Hubs;

namespace OrderManagement.Web.Hubs
{
    public class ProgressChat : Microsoft.AspNet.SignalR.Hub
    {
        public static IHubCallerConnectionContext<dynamic> GroupClients;


        public void RegisterUser(string userId)
        {
            GroupClients = Clients;
            Groups.Add(Context.ConnectionId, userId);
        }

        public void SendNotification(string name, string message, int userId)
        {
            try
            {
                if (GroupClients == null)
                    GroupClients = Clients;
                GroupClients.Group(Convert.ToString(userId)).broadcastMessage(name, message);
            }
            catch (Exception ex)
            {
            }
        }
    }
}