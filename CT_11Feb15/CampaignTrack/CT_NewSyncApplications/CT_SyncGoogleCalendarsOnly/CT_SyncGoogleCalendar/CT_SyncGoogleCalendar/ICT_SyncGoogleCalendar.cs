using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CT_SyncGoogleCalendar
{
    [ServiceContract]
    public interface ICT_SyncGoogleCalendar
    {

        /// <summary>
        /// This function is used for Sync Google calenders [new and old both  (not used for pushnotification)] in the Db as well as insert calendar event details in the event table.
        /// </summary>
        /// <param name="orgId"></param>
        /// 
        [OperationContract]
        void SyncGoogleCalendars(int orgId);

        [OperationContract]
        void SyncGoogleCalendar(string calendarId);
        
        /// <summary>
        ///  <para>This function is used for Subscribe Calendars Google calenders mainly it is used to start push notification from Google.</para> 
        ///  <para> when event not sync from google to our OMS application - </para> 
        ///  <para> It is also inserting channel expiration in the oms table.</para> 
        /// </summary>
        /// <param name="orgId"></param>
        /// 

        [OperationContract]
        void SubscribeCalendars(int orgId);

       
    }
}
