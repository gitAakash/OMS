using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderManagement.Web.Models.Repository;

namespace OrderManagement.Web.Models.ServiceRepository
{

    public interface ICalendarService
    {
        

    }

    public class CalendarService:ICalendarService
    {
          private ICalendarRepository _repository;
          public CalendarService(ICalendarRepository repository)
        {
            _repository = repository;
           
        }

    }
}