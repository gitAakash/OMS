using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;
using System.Web.Util;

namespace OrderManagement.Web.CalenderHelper
{
    public static class KendoMvcExtensions
    {

        public static IHtmlString ToMvcClientTemplate(this MvcHtmlString mvcString)
        {
            if (HttpEncoder.Current.GetType() == typeof(AntiXssEncoder))
            {
                var initial = mvcString.ToHtmlString();
                var corrected = initial.Replace("\\u0026", "&").Replace("%23", "#").Replace("%3D", "=").Replace("&#32;", " ");
                return new HtmlString(corrected);
            }

            return mvcString;
        }
    }
}