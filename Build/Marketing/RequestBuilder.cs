using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Genworth.SitecoreExt.Marketing
{
    public class RequestBuilder 
    {
        public static NameValueCollection BuildFormFieldValues(string issuer, string xmlRequest, string token = null, string actionUrl = null, string actionData = null)
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("issuer", issuer);
            collection.Add("request", xmlRequest);

            if (!string.IsNullOrWhiteSpace(token))
            {
                collection.Add("token", token);
            }

            if (!string.IsNullOrWhiteSpace(actionUrl))
            {
                collection.Add("actionUrl", actionUrl);
            }

            if (!string.IsNullOrWhiteSpace(actionData))
            {
                collection.Add("actionData", actionData);
            }

            return collection;
        }
    }
}
