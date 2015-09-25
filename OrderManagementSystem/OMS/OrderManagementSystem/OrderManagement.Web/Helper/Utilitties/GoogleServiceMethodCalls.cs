using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Helper.Utilitties
{
    public class GoogleServiceMethodCalls
    {

       //    TestEnvGoogelService.IGoogleNotificationService GoogleService; //this instance for http://test.zerofootprint.com.au/GoogleService/GoogleNotificationService.svc
        //LocalTestEnvServiceRef.IGoogleNotificationService GoogleService;
            ProdEnvGoogelService.IGoogleNotificationService GoogleService; // this instance for http://dpi.zerofootprint.com.au/GoogleService/GoogleNotificationService.svc

        public GoogleServiceMethodCalls()
        {

        //  GoogleService = new TestEnvGoogelService.GoogleNotificationServiceClient();
        //    GoogleService = new LocalTestEnvServiceRef.GoogleNotificationServiceClient();
             GoogleService = new ProdEnvGoogelService.GoogleNotificationServiceClient();
            //  GoogleService.CheckCalendarEvents("campaigntrack.dpi@gmail.com");
        }
        /// <summary>
        /// CreateEvent used for Create Event in the Google Cal
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Location"></param>
        /// <param name="Sales"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Comment"></param>
        /// <param name="ProductDescription"></param>
        /// <param name="Orderid"></param>
        /// <param name="RequiredDate"></param>
        /// <param name="ColorId"></param>
        public string CreateEvent(string Title, string Location, string Sales, DateTime StartDate, DateTime EndDate, string Comment, string ProductDescription, string Orderid, string RequiredDate, String ColorId, string UserCalendar, string Description, string ReccurrenceRule, bool isAllday)
        {
            string eventId = string.Empty;

            try
            {
                //if (isAllday)
                //{
                //    EndDate = EndDate.AddDays(1);
                //}
                eventId = GoogleService.CreateEvent(Title, Location, Sales, StartDate, EndDate, Comment, ProductDescription, Orderid, RequiredDate, ColorId, UserCalendar, Description, ReccurrenceRule, isAllday);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                //  File.AppendAllText("D:\\ClientGoogle.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }

            return eventId;
        }

        /// <summary>
        /// UpdateEvent used for Updating Event in the Google Cal
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="Creator"></param>
        /// <param name="Organizer"></param>
        /// <param name="Location"></param>
        /// <param name="Title"></param>
        /// <param name="EventID"></param>

        public string UpdateEvent(DateTime StartTime, DateTime EndTime, string Creator, string Organizer, string Location, string Title, string EventID, string UserCalendar, int sequence, string description, string ReccurrenceRule, bool isAllday, string colorId)
        {
            string eventId = string.Empty;
            try
            {
                //if (isAllday)
                //{
                //    EndTime = EndTime.AddDays(1);
                //}

                eventId = GoogleService.UpdateEvent(StartTime, EndTime, Creator, Organizer, Location, Title, EventID, UserCalendar, sequence, description, ReccurrenceRule, isAllday, colorId);
            }
            catch (Exception ex)
            {
                //  File.AppendAllText("D:\\ClientGoogle.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }

            return eventId;
        }

        public string UpdateEventInstance(DateTime StartTime, DateTime EndTime, string Creator, string Organizer, string Location, string Title, string EventID, string UserCalendar, int sequence, string description, string ReccurrenceRule, bool isAllday, string colorId)
        {
            string eventId = string.Empty;
            try
            {
                //if (isAllday)
                //{
                //    EndTime = EndTime.AddDays(1);
                //}

                eventId = GoogleService.UpdateEventInstance(StartTime, EndTime, Creator, Organizer, Location, Title, EventID, UserCalendar, sequence, description, ReccurrenceRule, isAllday, colorId);
            }
            catch (Exception ex)
            {
                //  File.AppendAllText("D:\\ClientGoogle.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }

            return eventId;
        }

        /// <summary>
        /// DeleteEvent used for Deleteing Event in the Google Cal
        /// </summary>
        /// <param name="EventID"></param>
        public void DeleteEvent(string EventID, string UserCalendar)
        {
            try
            {
                string statusMsg = GoogleService.DeleteEvent(EventID, UserCalendar);
            }
            catch (Exception ex)
            {
                //  File.AppendAllText("D:\\ClientGoogle.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
        }

        /// <summary>
        /// MovedEvent
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="Creator"></param>
        /// <param name="Organizer"></param>
        /// <param name="Location"></param>
        /// <param name="Title"></param>
        /// <param name="EventID"></param>

        public string MoveEvent(DateTime StartTime, DateTime EndTime, string Creator, string Organizer, string Location, string Title, string EventID, string DestinationCalendar, string UserCalendar, int sequence, string Description, string ReccurrenceRule, bool isAllday, string colorId)
        {
            string eventId = string.Empty;
            try
            {
                //if (isAllday)
                //{
                //    EndTime = EndTime.AddDays(1);
                //}

                eventId = GoogleService.MoveEvent(StartTime, EndTime, Creator, Organizer, Location, Title, EventID, DestinationCalendar, UserCalendar, sequence, Description, ReccurrenceRule, isAllday, colorId);
            }
            catch (Exception ex)
            {
                //  File.AppendAllText("D:\\ClientGoogle.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }

            return eventId;
        }


        public string DeleteEventInstance(string eventid, string calenderUser, DateTime start)
        {
            string eventId = string.Empty;
            try
            {
                eventId = GoogleService.DeleteEventInstance(eventid, calenderUser, start);
            }
            catch (Exception ex)
            {
                //  File.AppendAllText("D:\\ClientGoogle.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }

            return eventId;
        }


        public string GetLangitudeAndLongiutude(string PropAddress)
        {
            string coordinates = string.Empty;
            try
            {
            //     coordinates = GoogleService.GetCoordinates(PropAddress).Latitude + "," + GoogleService.GetCoordinates(PropAddress).Longitude; // for test site we need to uncomment this for new order form
            }
            catch (Exception ex)
            {
                  File.AppendAllText("D:\\ClientGoogle.txt", ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
            }
            return coordinates;
        }



        public string CheckCalendarEvents(string calid)
        {
            GoogleService.CheckCalendarEvents(calid);
            return "";
        }

    }

}
