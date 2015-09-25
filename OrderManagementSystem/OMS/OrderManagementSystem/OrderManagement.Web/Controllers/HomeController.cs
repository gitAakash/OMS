using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models;
using OrderManagement.Web.Models.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace OrderManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICompanyRepository _repository;
        private readonly IProductRepository _productRepository;
        private IProductCompanyRepository _productCompanyRepo;
        private readonly IProductGroupRepository _productGroupRepository;

        public HomeController()
        {
            this._repository = new CompanyRepository();
            _productRepository = new ProductRepository();
            _productCompanyRepo = new ProductCompanyRepository();
            _productGroupRepository = new ProductGroupRepository();
        }

        public ActionResult Index()
        {
            //Check if user is already validate or not
            if (User.Identity.IsAuthenticated)
            {
             //   return RedirectToAction("Index", "JobTracking");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Dashboard()
        {
            //Check if user is already validate or not
            //User currentUser = HttpContext.Cache["currentloggedinuser"] as User;
            //Mapper code for index view model
            IndexViewModel indexViewModel = new IndexViewModel();
            var currentUser = UserManager.Current();
            indexViewModel.FirstName = currentUser.FirstName;
            indexViewModel.LastName = currentUser.LastName;
            var companylstModel = new List<CompanyModel>();
            if (currentUser != null && currentUser.OrgId != null)
            {
                var companylst = _repository.SelectAll().Where(c => c.Org_Id == currentUser.OrgId);
                indexViewModel.ProductGrouplist = _productGroupRepository.GetAllProductgroup().ToList();
                indexViewModel.Companylist = companylst.ToList();
            }

            return PartialView("_DashBoard", indexViewModel);

        }

        public ActionResult Company()
        {
            return PartialView("_Company");
        }

        public ActionResult Products()
        {
            return PartialView("_Products");
        }

        public ActionResult GetChartData(DateTime fromDate, DateTime toDate, string productType, int companyCode)
        {
            return View();
        }

        public ActionResult GetSearchResult(int month, int year, int companyId, int productgrpId)
        {
            var properties = _repository.GetAllProperty().Where(m => m.Company_Id == companyId).Select(c => c.Row_Id).ToList();
            var orderIds = _repository.GetAllOrders().Where(o => properties.Contains((int)o.Property_Id)).Select(x => x.Row_Id).ToList();

            DateTime currentMonth = new DateTime(year, month, DateTime.Now.Day);
            var thisMonthStart = currentMonth.AddDays(1 - currentMonth.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            var orderItems = _repository.GetAllOrderItems().Where(o => orderIds.Contains((int)o.Order_Id)).ToList();

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.OrderQuantity = new object[thisMonthEnd.Day];
            dashboardViewModel.Days = new string[thisMonthEnd.Day];

            dashboardViewModel.DaysInMonth = thisMonthEnd.Day;
            for (int i = 0; i < thisMonthEnd.Day; i++)
            {
                dashboardViewModel.OrderQuantity[i] = (orderItems.Where(x => x.Created.Equals(thisMonthStart)).Count());
                dashboardViewModel.Days[i] = (i + 1).ToString();
                thisMonthStart.AddDays(1);
            }

            Highcharts chart = new Highcharts("chart")
                .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
                .SetTitle(new Title { Text = "Monthly Order" })
                .SetSubtitle(new Subtitle { Text = "" })
                .SetXAxis(new XAxis { Categories = dashboardViewModel.Days })
                .SetYAxis(new YAxis
                {
                    Min = 0,
                    Title = new YAxisTitle { Text = "Orders quantity" }
                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Left,
                    VerticalAlign = VerticalAligns.Top,
                    X = 100,
                    Y = 70,
                    Floating = true,
                    BackgroundColor = new BackColorOrGradient(ColorTranslator.FromHtml("#FFFFFF")),
                    Shadow = true
                })
                .SetTooltip(new Tooltip { Formatter = @"function() { return ''+ this.x +': '+ this.y ; }" })
                .SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        PointPadding = 0.2,
                        BorderWidth = 0
                    }
                })
                .SetSeries(new[]
                {
                    //new Series { Name = "Order", Data = new Data(dashboardViewModel.OrderQuantity) },
                    new Series { Name = "Order", Data = new Data(new object[] { 48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2,48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2,21.7 }) },
                    //new Series { Name = "New York", Data = new Data(new object[] { 83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3 }) },
                    //new Series { Name = "Berlin", Data = new Data(new object[] { 42.4, 33.2, 34.5, 39.7, 52.6, 75.5, 57.4, 60.4, 47.6, 39.1, 46.8, 51.1 }) }
                });

            return PartialView("GetSearchResult", chart);
            //return Json(dashboardViewModel, JsonRequestBehavior.AllowGet);
        }
    }

}