using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Utilities
{
    public class BundleHelper
    {
        public enum BundleType { Scripts, Styles };

        public static string GetBundle(string bundleName, BundleType bundleType)
        {
            string bundleUrl = string.Empty;

            try
            {
                Dictionary<string, string> bundlesCached = System.Web.HttpContext.Current.Application["Bundles"] as Dictionary<string, string>;

                if (bundlesCached == null)
                {
                    bundlesCached = new Dictionary<string, string>();
                }

                if (bundlesCached.ContainsKey(bundleName + "-" + bundleType) && !String.IsNullOrEmpty(bundlesCached[bundleName + "-" + bundleType]))
                {
                    bundleUrl = bundlesCached[bundleName + "-" + bundleType];
                }
                else
                {
                    string bundleEndpoint = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Integrations.Ewm.BundleURL);
                    string bundleEncoding = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Integrations.Ewm.Encoding);
                    string bundleTypeName = string.Empty;

                    if (bundleType == BundleType.Scripts)
                    {
                        bundleTypeName = Genworth.SitecoreExt.Constants.Settings.BundleTypes.Scripts;
                    }
                    else
                    {
                        bundleTypeName = Genworth.SitecoreExt.Constants.Settings.BundleTypes.Styles;
                    }

                    // get bundle url with cache key from remote system
                    bundleUrl = Genworth.SitecoreExt.Helpers.HTMLIntegrationLogic.GetHtmlFromUrlWithCookies(bundleEncoding, bundleEndpoint + "/" + bundleTypeName + "?key=" + bundleTypeName + "/" + bundleName);

                    bundlesCached.Add(bundleName + "-" + bundleType, bundleUrl);
                    System.Web.HttpContext.Current.Application["Bundles"] = bundlesCached;

                    Sitecore.Diagnostics.Log.Debug(string.Format("Remote Bundle Url: {0}", bundleUrl));
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(string.Format("Remote Bundle Url exception getting bundle name: {0}, type: {1}", bundleName, bundleType), ex);
            }

            return bundleUrl;
        }
    }
}
