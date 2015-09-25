using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Models.Repository
{
    public interface ICalendarRepository
    {
        IList<Calendar> GetAll(int orgid);

        void SaveCalendarUser(CalendarUser obj);
        void UpdateCalendarUser(CalendarUser obj);
        CalendarUser GetCalendarUserByUserId(int usrid);
        IList<EventTracking> GetEventTrackinglist(int calendarid, int orgid);
    }

    public class CalendarRepository : ICalendarRepository
    {

         private OrderMgntEntities db = null;
        
        public CalendarRepository()
        {
            this.db = new OrderMgntEntities();
        }
        public CalendarRepository(OrderMgntEntities db)
        {
            this.db = db;
        }

        public IList<Calendar> GetAll(int orgid)
        {
            return db.Calendars.Where(cal => cal.Org_Id == orgid).ToList();
        }

        public void SaveCalendarUser(CalendarUser obj)
        {
            db.CalendarUsers.Add(obj);
            db.SaveChanges();
        }

        public void UpdateCalendarUser(CalendarUser obj)
        {
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }

        public CalendarUser GetCalendarUserByUserId(int usrid)
        {
             return db.CalendarUsers.SingleOrDefault(cal => cal.UserId ==usrid);
        }

        public IList<EventTracking> GetEventTrackinglist(int calendarid,int orgid)
        {

            return db.EventTrackings.Where(e => e.CalendarId == calendarid && e.Org_Id == orgid).ToList();
        }
    }
}