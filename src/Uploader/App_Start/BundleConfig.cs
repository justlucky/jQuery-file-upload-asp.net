using System.Web;
using System.Web.Optimization;

namespace Uploader
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/bootstrap")
             .Include("~/Content/file-upload/css/bootstrap.min.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/Content/fontAwesome")
             .Include("~/Content/vendor/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-1.10.2.min.js"));
            


            bundles.Add(new StyleBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.min.js"));
        }
    }
}
