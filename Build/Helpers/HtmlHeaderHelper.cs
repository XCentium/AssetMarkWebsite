using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Genworth.SitecoreExt.Helpers
{
    public static class HtmlHeaderHelper
    {
        public enum IncludeType { Script, Style }

        public static void AddJsToPage(System.Web.UI.Page page, string url, string receivingControlId = "pResources")
        {
            AddJsToPage(page, url, new NameValueCollection(), receivingControlId);
        }

        public static void AddJsToPage(System.Web.UI.Page page, string url, NameValueCollection nvc, string receivingControlId = "pResources")
        {
            AddIncludeToPage(page, url, nvc, receivingControlId, IncludeType.Script);
        }

        public static void AddCssToPage(System.Web.UI.Page page, string url, string receivingControlId = "pResources")
        {
            AddCssToPage(page, url, new NameValueCollection(), receivingControlId);
        }

        public static void AddCssToPage(System.Web.UI.Page page, string url, NameValueCollection nvc, string receivingControlId = "pResources")
        {
            AddIncludeToPage(page, url, nvc, receivingControlId, IncludeType.Style);
        }

        private static void AddIncludeToPage(System.Web.UI.Page page, string url, NameValueCollection nvc, string receivingControlId = "pResources", IncludeType includeType = IncludeType.Script)
        {
            if (page == null || String.IsNullOrEmpty(url) || nvc == null || String.IsNullOrEmpty(receivingControlId)) return;

            var attributeBuilder = new StringBuilder();
            foreach (var key in nvc.AllKeys)
            {
                attributeBuilder.AppendFormat(" {0}={1}", key, nvc[key]);
            }

            Control pResources = page.FindControl(receivingControlId);
            if (pResources != null)
            {
                Literal litInclude = new Literal();

                if (includeType == IncludeType.Script)
                {
                    litInclude.Text = "<script src='" + url + "' " + attributeBuilder.ToString() + " type='text/javascript'></script>";
                }
                else
                {
                    litInclude.Text = "<link href='" + url + "' " + attributeBuilder.ToString() + " rel='stylesheet' type='text/css' />";
                }

                pResources.Controls.Add(litInclude);
            }
        }
    }
}
