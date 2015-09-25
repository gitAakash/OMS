using System;
using System.Web;
using System.Web.Optimization;

namespace OrderManagement.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles )
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(

                "~/Content/bootstrap.min.css",
                "~/Content/font-awesome.css",
                "~/Content/morris-0.4.3.min.css",
                "~/Content/jquery.gritter.css",
                "~/Content/animate.css",
                "~/Content/style.css",
                "~/Content/Site.css",
                "~/Content/switchery.css"
                ));

            bundles.Add(new StyleBundle("~/Content/kendo/2014.2.716/KendoCss").Include(
                "~/Content/kendo/2014.2.716/kendo.common.min.css",
                "~/Content/kendo/2014.2.716/kendo.dataviz.min.css",
                "~/Content/kendo/2014.2.716/kendo.default.min.css",
                "~/Content/kendo/2014.2.716/kendo.dataviz.default.min.css",
                "~/Content/kendo/2014.2.716/kendo.dataviz.flat.min.css",
                "~/Content/kendo/2014.2.716/kendo.flat.min.css"
                ));


            bundles.Add(new StyleBundle("~/Content/themes/base/ThemesCss").Include(
                  "~/Content/themes/base/jquery-ui.css"
                 ));


             bundles.Add(new ScriptBundle("~/Scripts/kendo/2014.2.716/kendoScripts").Include(

              "~/Scripts/kendo/2014.2.716/jquery.min.js",
             "~/Scripts/kendo/2014.2.716/kendo.all.min.js",
             "~/Scripts/kendo/2014.2.716/kendo.aspnetmvc.min.js",
            // "~/Scripts/kendo/2014.2.716/kendo.timezones.min.js",
             "~/Scripts/kendo/2014.2.716/kendo.web.min.js"

           ));


             bundles.Add(new ScriptBundle("~/Scripts/Theme_Js/Themejquery").Include(
          
                 "~/Scripts/Theme_Js/jquery-ui.min.js",
                 "~/Scripts/Theme_Js/bootstrap.min.js",
                 "~/Scripts/Theme_Js/jquery.metisMenu.js",

                 //< !--Sparkline-- >
                 "~/Scripts/Theme_Js/jquery.sparkline.min.js",
                 //<!-- Custom and plugin javascript -->
                 "~/Scripts/Theme_Js/inspinia.js",
                 "~/Scripts/Theme_Js/pace.min.js",
                 "~/Scripts/Theme_Js/jquery.flot.js",
                 "~/Scripts/Theme_Js/jquery.flot.tooltip.min.js",
                 "~/Scripts/Theme_Js/jquery.flot.spline.js",
                 "~/Scripts/Theme_Js/jquery.flot.resize.js",
                 "~/Scripts/Theme_Js/jquery.flot.pie.js",
                 "~/Scripts/Theme_Js/jquery.flot.symbol.js"

          //<!-- EayPIE -->

          //"~/Scripts/Theme_Js/jquery.easypiechart.js",
          //"~/Scripts/Theme_Js/jquery.flot.pie.js",
          //"~/Scripts/Theme_Js/jquery.flot.symbol.js"

               ));

             bundles.Add(new ScriptBundle("~/Scripts/CommonJS").Include(
             "~/Scripts/switchery.js",
             "~/Scripts/icheck.js",
             "~/Scripts/jquery-1.10.2.js"

            ));




             bundles.Add(new ScriptBundle("~/Scripts/kendo/2014.2.716/GalkendoScripts").Include(

            "~/Scripts/kendo/2014.2.716/jquery.min.js",
           "~/Scripts/kendo/2014.2.716/kendo.web.min.js"
         ));

             BundleTable.EnableOptimizations = true;
            
        }
    }
}