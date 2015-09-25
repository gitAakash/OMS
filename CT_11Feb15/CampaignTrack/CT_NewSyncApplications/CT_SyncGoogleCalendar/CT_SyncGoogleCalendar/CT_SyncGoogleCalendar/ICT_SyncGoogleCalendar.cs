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
        [OperationContract]
        void SyncGoogleCalendars();

        [OperationContract]
        void SyncGoogleCalendar(string calendarId);
    }
}
