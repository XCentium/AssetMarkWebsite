using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using ServerLogic.Core.Web.Utilities;

namespace Genworth.SitecoreExt.Utilities
{
    public class Web
    {
		[Obsolete("Use GenworthQueryString instead.")]
        public static string MergeQuerystring(string sURL, Dictionary<string, string> oQueryStringParametersToAppend, bool bAppendCurrentRequestParameters)
        {
            Uri oUri;
            string sQueryParameter;
            NameValueCollection oQueryParameters;
            string sResultURL;
            bool bFirstParameterSet;
            QueryString oQueryString;

			sResultURL = sURL;

            if (!string.IsNullOrEmpty(sURL) && Uri.TryCreate(sURL, UriKind.RelativeOrAbsolute, out oUri) && oQueryStringParametersToAppend != null)
            {
                // Valid URL

                if (oUri.IsAbsoluteUri && !string.IsNullOrEmpty(sQueryParameter = oUri.Query))
                {
                    //It has parameters
                    oQueryParameters = HttpUtility.ParseQueryString(sQueryParameter);
                }
                else
                {
					if ((oQueryParameters = System.Web.HttpUtility.ParseQueryString(sURL)) == null)
					{
						//No query string parameters
						oQueryParameters = new NameValueCollection();
					}
                }

				//adding the current parameters we have to avoid loosing information
				if (bAppendCurrentRequestParameters)
				{
					oQueryString = QueryString.Current;
					foreach (string oCurrentRequestParameterKey in oQueryString.Keys)
					{
						oQueryParameters.Set(oCurrentRequestParameterKey, oQueryString.Get(oCurrentRequestParameterKey));
					}
				}

				//Add new parameters, if the parameter already exists it will replaced
                foreach (var oParameter in oQueryStringParametersToAppend)
                {
                    if (!string.IsNullOrEmpty(oParameter.Key))
                    {
                        oQueryParameters.Set(oParameter.Key, oParameter.Value);
                    }
                }                

				if (oUri.IsAbsoluteUri)
				{
					sResultURL = oUri.AbsoluteUri;
				}
				else
				{
					sResultURL = (sURL.IndexOf('?') > -1) ? (sURL.Substring(0, sURL.IndexOf('?'))) : (sURL);
				}

				if (oQueryParameters.Count > 0)
				{
					sResultURL = string.Format("{0}?", sResultURL);
				}
                
                bFirstParameterSet = false;
                foreach (string oQParameterKey in oQueryParameters.Keys)
                {
					if (!string.IsNullOrEmpty(oQParameterKey))
					{
						if (bFirstParameterSet)
						{
							sResultURL = string.Format("{0}&{1}={2}", sResultURL, oQParameterKey, oQueryParameters.Get(oQParameterKey));
						}
						else
						{
							sResultURL = string.Format("{0}{1}={2}", sResultURL, oQParameterKey, oQueryParameters.Get(oQParameterKey));
							bFirstParameterSet = true;
						}
					}
                }



            }

            return sResultURL;
        }
    
    }
}
