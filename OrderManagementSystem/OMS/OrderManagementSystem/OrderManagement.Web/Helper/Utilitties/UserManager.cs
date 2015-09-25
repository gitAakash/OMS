using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderManagement.Web.Helper.Utilitties
{
    public class UserManager
    {
        static Func<User> GetCurrentUser;

        public static void SetUserTracker(Func<User> getCurrentUser)
        {
            GetCurrentUser = getCurrentUser;
        }

        public static T Current<T>() where T : User
        {
            return GetCurrentUser() as T;
        }

        public static User Current()
        {
            return GetCurrentUser();
        }
    }
}