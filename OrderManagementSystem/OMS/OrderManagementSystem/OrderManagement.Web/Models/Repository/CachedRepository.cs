using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;


namespace OrderManagement.Web.Models.Repository
{
    /// <summary>
    ///  CachedJobTrackingRepository Class is used for Cache the JobTrackingRepository Class
    /// </summary>
    public  sealed class CachedJobTrackingRepository : JobTrackingRepository
    {
        private OrderMgntEntities db = null;

        private static CachedJobTrackingRepository instance;

        public static CachedJobTrackingRepository Instance
        {
            get
            {
                if (instance == null)
                    instance = new CachedJobTrackingRepository();
                return instance;
            }
        }

        private CachedJobTrackingRepository()
        {
            this.db = new OrderMgntEntities();
        }

        private static readonly object CacheLockObject = new object();

        public override IList<SelectJobAttachmentFolders> SelectJobAttachmentFolders(int orgid, int? userid, string userType, int? compamyid,int?jobId)
        {
          
            string cacheKey = "SelectJobAttachmentFolders";
            var result = HttpRuntime.Cache[cacheKey] as List<SelectJobAttachmentFolders>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cacheKey] as List<SelectJobAttachmentFolders>;
                    if (result == null)
                    {
                        result = db.SelectJobAttachmentFolders(orgid,userid,userType,compamyid,jobId).ToList();
                        HttpRuntime.Cache.Insert(cacheKey, result, null,
                            DateTime.Now.AddSeconds(300), TimeSpan.Zero);
                    }
                }
            }
            return result;
        }

        public override IList<TrackingJobs> GetAllJobs(int orgid, int? userid, string userType, int? compamyid, string search, bool IsRefreshCache = false)
        {
            string cacheKey = "SelectJob";
            var result = HttpRuntime.Cache[cacheKey] as List<TrackingJobs>;

            if (IsRefreshCache == true)
            {
                result = null;
                {
                    lock (CacheLockObject)
                    {
                            result = db.SelectJobs(orgid, userid, userType, compamyid, search).ToList();
                            HttpRuntime.Cache.Insert(cacheKey, result, null,
                                DateTime.Now.AddSeconds(180), TimeSpan.Zero);
                    }
                }
            }
            else
            {
                if (result == null)
                {
                    lock (CacheLockObject)
                    {
                        result = HttpRuntime.Cache[cacheKey] as List<TrackingJobs>;
                        if (result == null)
                        {
                            result = db.SelectJobs(orgid, userid, userType, compamyid, search).ToList();
                            HttpRuntime.Cache.Insert(cacheKey, result, null,
                                DateTime.Now.AddSeconds(180), TimeSpan.Zero);
                        }
                    }
                }
            }
            return result;
        }

    }

    /// <summary>
    ///  CachedAccountRepository Class is used for Cache the JobTrackingRepository Class
    /// </summary>
    public sealed class CachedAccountRepository : AccountRepository
    {
        private OrderMgntEntities db = null;

        private static CachedAccountRepository instance;

        public static CachedAccountRepository Instance
        {
            get
            {
                if (instance == null)
                    instance = new CachedAccountRepository();
                return instance;
            }
        }

        private CachedAccountRepository()
        {
            this.db = new OrderMgntEntities();
        }

        private static readonly object CacheLockObject = new object();

        public User CurrentLoggedInUser(int userId, bool NewInstance = false)
        {


            System.Diagnostics.Debug.Print("CurrentLoggedInUser:CurrentLoggedInUser");
            string cacheKey = "CurrentLoggedInUser";
            var result = HttpRuntime.Cache[cacheKey] as User;
            if (NewInstance)
            {
                result = null;
                IAccountRepository _account;
                _account = new AccountRepository();
                result = _account.GetUserById(userId);
                HttpRuntime.Cache.Insert(cacheKey, result, null,
                DateTime.Now.AddSeconds(900), TimeSpan.Zero);
            }
            else
            {
                if (result == null)
                {
                    lock (CacheLockObject)
                    {
                        result = HttpRuntime.Cache[cacheKey] as User;
                        if (result == null)
                        {
                            IAccountRepository _account;
                            _account = new AccountRepository();
                            result = _account.GetUserById(userId);
                            HttpRuntime.Cache.Insert(cacheKey, result, null,
                            DateTime.Now.AddSeconds(900), TimeSpan.Zero);
                        }
                    }
                }
            }
            return result;
        }



        public static string Version(string rootRelativePath)
        {
            if (HttpRuntime.Cache[rootRelativePath] == null)
            {
                var absolutePath = HostingEnvironment.MapPath(rootRelativePath);
                var lastChangedDateTime = File.GetLastWriteTime(absolutePath);

                if (rootRelativePath.StartsWith("~"))
                {
                    rootRelativePath = rootRelativePath.Substring(1);
                }

                var versionedUrl = rootRelativePath + "?v=" + lastChangedDateTime.Ticks;

                HttpRuntime.Cache.Insert(rootRelativePath, versionedUrl, new CacheDependency(absolutePath));
            }

            return HttpRuntime.Cache[rootRelativePath] as string;
        }
    }


    /// <summary>
    ///  CachedJobTrackingRepository Class is used for Cache the JobTrackingRepository Class
    /// </summary>
    public sealed class CachedOrderRepository : OrderRepository
    {
        private OrderMgntEntities db = null;

        private static CachedOrderRepository instance;

        public static CachedOrderRepository Instance
        {
            get
            {
                if (instance == null)
                    instance = new CachedOrderRepository();
                return instance;
            }
        }

        private CachedOrderRepository()
        {
            this.db = new OrderMgntEntities();
        }

        private static readonly object CacheLockObject = new object();

        public override IList<SelectOrderableProducts> SelectOrderableProducts(int orgid, int? userid, string userType, int? compamyid)
        {
            string cacheKey = "SelectOrderableProducts";
            var result = HttpRuntime.Cache[cacheKey] as List<SelectOrderableProducts>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cacheKey] as List<SelectOrderableProducts>;
                    if (result == null)
                    {
                        result = db.SelectOrderableProducts(orgid, userid, userType, compamyid).ToList();
                        HttpRuntime.Cache.Insert(cacheKey, result, null,
                            DateTime.Now.AddSeconds(300), TimeSpan.Zero);
                    }
                }
            }
            return result;
        }

      
    }


    /// <summary>
    ///  CachedJobTrackingRepository Class is used for Cache the JobTrackingRepository Class
    /// </summary>
    public sealed class CachedSchedulerRepository : SchedulerRepository
    {
        private OrderMgntEntities db = null;

        private static CachedSchedulerRepository instance;

        public static CachedSchedulerRepository Instance
        {
            get
            {
                if (instance == null)
                    instance = new CachedSchedulerRepository();
                return instance;
            }
        }

        private CachedSchedulerRepository()
        {
            this.db = new OrderMgntEntities();
        }

        private static readonly object CacheLockObject = new object();

        public override IList<string> GetEmailAddress(int orgid, int? userid, string userType, int? compamyid, string SearchValue)
        {
            string cacheKey = "GetEmailAddress";
            var result = HttpRuntime.Cache[cacheKey] as List<string>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cacheKey] as List<string>;
                    if (result == null)
                    {
                        result = db.SelectEmailAddressAutoComplete(orgid, userid, userType, compamyid, 0, SearchValue).ToList();
                        HttpRuntime.Cache.Insert(cacheKey, result, null,
                            DateTime.Now.AddSeconds(300), TimeSpan.Zero);
                    }
                }
            }
            return result;
        }
    }

   


}
 
namespace BundlingSample.Extensions
{
    public static class StaticFile
    {
        public static string Version(string rootRelativePath)
        {
            if (HttpRuntime.Cache[rootRelativePath] == null)
            {
                var absolutePath = HostingEnvironment.MapPath(rootRelativePath);
                var lastChangedDateTime = File.GetLastWriteTime(absolutePath);
 
                if (rootRelativePath.StartsWith("~"))
                {
                    rootRelativePath = rootRelativePath.Substring(1);
                }
 
                var versionedUrl = rootRelativePath + "?v=" + lastChangedDateTime.Ticks;
 
                HttpRuntime.Cache.Insert(rootRelativePath, versionedUrl, new CacheDependency(absolutePath));
            }
 
            return HttpRuntime.Cache[rootRelativePath] as string;
        }
    }
}
