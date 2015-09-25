using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace OrderManagement.Web.Helper.Utilitties
{
    public static class OrderManagementHtmlHelper
    {

        //public static string TimeAgo(DateTime date)
        //{

        //    TimeSpan timeSince = DateTime.Now.Subtract(date);

        //    if (timeSince.TotalMilliseconds < 1)
        //        return "not yet";
        //    if (timeSince.TotalMinutes < 1)
        //        return "just now";
        //    if (timeSince.TotalMinutes < 2)
        //        return "1 minute ago";
        //    if (timeSince.TotalMinutes < 60)
        //        return string.Format("{0} minutes ago", timeSince.Minutes);
        //    if (timeSince.TotalMinutes < 120)
        //        return "1 hour ago";
        //    if (timeSince.TotalHours < 24)
        //        return string.Format("{0} hours ago", timeSince.Hours);
        //    if (timeSince.TotalDays == 1)
        //        return "yesterday";
        //    if (timeSince.TotalDays < 7)
        //        return string.Format("{0} days ago", timeSince.Days);
        //    if (timeSince.TotalDays < 14)
        //        return "last week";
        //    if (timeSince.TotalDays < 21)
        //        return "2 weeks ago";
        //    if (timeSince.TotalDays < 28)
        //        return "3 weeks ago";
        //    if (timeSince.TotalDays < 60)
        //        return "last month";
        //    if (timeSince.TotalDays < 365)
        //        return string.Format("{0} months ago", Math.Round(timeSince.TotalDays / 30));
        //    if (timeSince.TotalDays < 730)
        //        return "last year";

        //    return string.Format("{0} years ago", Math.Round(timeSince.TotalDays / 365));
        //}





        public static MvcHtmlString TimeAgo(this HtmlHelper helper, DateTime date)
        {
            TimeSpan timeSince = DateTime.Now.Subtract(date);

            string strDate = null;

            if (timeSince.TotalMilliseconds < 1)
              strDate= "not yet";
            if (timeSince.TotalMinutes < 1)
              strDate= "just now";
            if (timeSince.TotalMinutes >1 &&timeSince.TotalMinutes < 2)
                strDate = "1 minute ago";
            if (timeSince.TotalMinutes > 2 && timeSince.TotalMinutes < 60)
                strDate = string.Format("{0} minutes ago", timeSince.Minutes);

            if (timeSince.TotalMinutes > 60 && timeSince.TotalMinutes < 120)
                strDate = "1 hour ago";
            if (timeSince.TotalHours < 24)
                strDate = string.Format("{0} hours ago", timeSince.Hours);
            if (timeSince.TotalDays == 1)
                strDate = "yesterday";
            if (timeSince.TotalDays > 1 && timeSince.TotalDays < 7)
                strDate = string.Format("{0} days ago", timeSince.Days);
            if (timeSince.TotalDays > 7 && timeSince.TotalDays < 14)
                strDate = "last week";
            if (timeSince.TotalDays > 14 && timeSince.TotalDays < 21)
                strDate = "2 weeks ago";
            if (timeSince.TotalDays > 21 && timeSince.TotalDays < 28)
                strDate = "3 weeks ago";
            if (timeSince.TotalDays > 28 && timeSince.TotalDays < 60)
                strDate = "last month";
            if ( timeSince.TotalDays > 60 && timeSince.TotalDays < 365)
               strDate = string.Format("{0} months ago", Math.Round(timeSince.TotalDays / 30));

            if (timeSince.TotalDays > 365 && timeSince.TotalDays < 730)
              strDate = "last year";
            if (timeSince.TotalDays > 730)
               strDate = string.Format("{0} years ago", Math.Round(timeSince.TotalDays / 365));
            
            return MvcHtmlString.Create(strDate);
        }




         //using (var OrderMangtDB = new OrderMgntEntities()) 
         //   { 
         //       var currentUser = UserManager.Current(); 
         //       int calendarUserId; 
         //       if (currentUser != null && currentUser.UserType == 1) 
         //       { 
         //           IList<OrderViewModel> order = _orderService.GetAll(); 
         //           foreach (var item in order) 
         //           { 
         //               item.PropertyName = item.Property_Id != null ? getPropertyNameById((int)item.Property_Id) : ""; 
         //               item.CompanyName = item.Property_Id != null ? getCompanyNameByPropertyId((int)item.Property_Id) : "";  

         //           } 
         //           return PartialView("Controls/Order/_OrderList", order); 
         //       }



        public static MvcHtmlString GetFiles(this HtmlHelper helper,string orderid)
        {
            string fileCount = string.Empty;
            using (var orderMangtDb = new OrderMgntEntities())
            {
                var currentUser = UserManager.Current();

              var orderlst =  orderMangtDb.Orders.Where(m => m.OrderId == orderid).ToList();

              if (orderlst.Count > 0)
              {


                  foreach (var item in orderlst)
                  {
                      var orderAttach =
                          orderMangtDb.OrderAttachments.Where(
                              m => m.Org_Id == currentUser.OrgId && m.Order_Id == item.Row_Id).ToList();
                       fileCount = orderAttach.Count.ToString();
                  }
                  if (fileCount != "0" && fileCount!="")
                  return MvcHtmlString.Create(fileCount);

              }
            }

            return null;
            
        }


        public static string ValidatePassword(string password)
        {
            string strResult = "fail";
            try
            {
                bool result = false;
                bool isDigit = false;
                bool isLetter = false;
                bool isLowerChar = false;
                bool isUpperChar = false;
                bool isNonAlpha = false;

                foreach (char c in password)
                {
                    if (char.IsDigit(c))
                        isDigit = true;
                    if (char.IsLetter(c))
                    {
                        isLetter = true;
                        if (char.IsLower(c))
                            isLowerChar = true;
                        if (char.IsUpper(c))
                            isUpperChar = true;
                    }
                    Match m = Regex.Match(c.ToString(), @"\W|_");
                    if (m.Success)
                        isNonAlpha = true;
                }

                if (isDigit && isLetter && isLowerChar &&
                                       isUpperChar && isNonAlpha)
                    result = true;

                if (result)
                    strResult = "success";
            }
            catch
            {
                strResult = "fail";
            }
            return strResult;
        }


      




    }
}