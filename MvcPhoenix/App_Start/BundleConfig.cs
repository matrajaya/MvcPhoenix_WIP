using System.Web;
using System.Web.Optimization;

namespace MvcPhoenix
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-migrate-{version}.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery-barcode.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/site.js",
                        "~/Scripts/modernizr-*"
                        ));

            #region close comments
            // bundle all-in-one
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js",
            //            "~/Scripts/jquery-migrate-{version}.js",
            //            "~/Scripts/jquery.unobtrusive*",
            //            "~/Scripts/jquery.validate*",
            //            "~/Scripts/jquery-ui-{version}.js"
            //            ));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //            "~/Scripts/bootstrap.js",
            //            "~/Scripts/respond.js"));

            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));
            #endregion

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css",
                        "~/Content/font-awesome.css",
                        "~/Content/jquery-ui.css",
                        "~/Content/PagedList.css"));
        }
    }
}
