using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using OrderManagement.Web.App_Start;
using OrderManagement.Web.Controllers;
using OrderManagement.Web.Helper.Utilitties;
using OrderManagement.Web.Models.Repository;

namespace OrderManagement.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            AutoMapperConfiguration.MapViewModelwithDomainClass();
            AuthenticateUserSession();
        }


        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }



        

        private void AuthenticateUserSession()
        {

            UserManager.SetUserTracker(new Func<User>(() =>
            {
                var getTicket = new Func<string>(() =>
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (cookie == null)
                        return null;
                    FormsAuthenticationTicket ticket = null;
                    try
                    {
                        if (cookie.Value.Length == 0)
                            return null;
                        ticket = FormsAuthentication.Decrypt(cookie.Value);
                    }
                    catch
                    {
                    }

                    if (ticket == null)
                        return null;

                    return ticket.Name;
                });

                var authToken = getTicket();
                int systemUserId;
                if (!int.TryParse(authToken, out systemUserId))
                    return null;

                return CachedAccountRepository.Instance.CurrentLoggedInUser(systemUserId,true);

                //  IAccountRepository _account;
                //_account = new AccountRepository();
               // return _account.GetUserById(systemUserId);
              //  return null; //Persister.Session.Get<SystemUser>(systemUserId);
            }));
        }


    }
}