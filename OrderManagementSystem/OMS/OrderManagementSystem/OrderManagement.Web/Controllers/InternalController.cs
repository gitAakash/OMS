using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderManagement.Web.Models;

namespace OrderManagement.Web.Controllers
{
    public class InternalController : Controller
    {
        //
        // GET: /Internal/

        public ActionResult Index()
        {

            return PartialView("Controls/Staff/_JobList");
        }

    }
}
