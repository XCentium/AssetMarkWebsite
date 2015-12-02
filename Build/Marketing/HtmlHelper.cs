using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Genworth.SitecoreExt.Marketing
{
    public class HtmlHelper
    {
        public static string DefaultTarget = "_blank";

        public static string PreparePOSTForm(string url, NameValueCollection data, bool scriptSubmit)
        {
            //Set a name for the form
            string formID = "defaultPostForm";

            return PreparePOSTForm(formID, url, data, scriptSubmit, DefaultTarget, false);
        }

        public static string PreparePOSTForm(string formID, string url, NameValueCollection data, bool scriptSubmit, string target, bool autoSubmit)
        {
            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.AppendLine(string.Format("<form id='postForm{0}' name='{1}' action='{2}' target='{3}' method='POST'>", formID, formID, url, target ?? DefaultTarget));

            foreach (string key in data)
            {
                strForm.AppendLine(string.Format("<input type='hidden' name='{0}' value='{1}' />", key, System.Web.HttpUtility.HtmlEncode(data[key])));
            }

            if (!scriptSubmit)
            {
                strForm.AppendLine("<span class='button' style='display:none;'><input type='Submit' name='Order' value='Order' /></span>");
            }

            strForm.AppendLine("</form>");
            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();

            if (autoSubmit)
            {
                strScript.AppendLine("<script language='javascript'>");
                strScript.AppendLine(string.Format("var vPostForm{0} = document.postForm{1};", formID, formID));
                strScript.AppendLine(string.Format("vPostForm{0}.submit();", formID));
                strScript.AppendLine("</script>");
            }

            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }
    }
}
