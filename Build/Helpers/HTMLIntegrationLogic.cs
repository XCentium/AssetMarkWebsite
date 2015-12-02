using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

namespace Genworth.SitecoreExt.Helpers
{
    public static class HTMLIntegrationLogic
    {

        public static string GetHtmlFromUrlWithCookies(string sEncoding, string sHtmlURL)
        {

            Uri oHtmlUri;
            HttpWebRequest oHttpWebRequest;
            System.Web.HttpCookieCollection oCurrentCookies;            
            HttpWebResponse oHttpWebResponse;
            Stream oResponseStream;
            StreamReader oStreamReader;
            string sHtmlToRetun;
            Encoding oEncoding;

            sHtmlToRetun = string.Empty;

            if (Uri.TryCreate(sHtmlURL, UriKind.RelativeOrAbsolute, out oHtmlUri))
            {

                try
                {
                    oHttpWebRequest = WebRequest.Create(oHtmlUri) as HttpWebRequest;
                    oHttpWebRequest.Timeout = 1000 * 1000;
                    oEncoding = ParseEncoding(sEncoding);
                    oCurrentCookies = HttpContext.Current.Request.Cookies;

					if (oCurrentCookies != null && HttpContext.Current.Request.Cookies[Genworth.SitecoreExt.Constants.Security.SWT.GFWMSessionId] != null)
					{
						oHttpWebRequest.Headers.Add(
													HttpContext.Current.Request.Cookies[Genworth.SitecoreExt.Constants.Security.SWT.GFWMSessionId].Name, 
													HttpUtility.UrlEncode(HttpContext.Current.Request.Cookies[Genworth.SitecoreExt.Constants.Security.SWT.GFWMSessionId].Value, oEncoding)
												   );
					}
					else
					{
						Sitecore.Diagnostics.Log.Error("Unable to read GFWMSessionId", oHttpWebRequest);
					}

                    if (oHttpWebRequest != null)
                    {
                        
                        oHttpWebResponse = oHttpWebRequest.GetResponse() as HttpWebResponse;

                        if (oHttpWebResponse != null)
                        {
                            
                            if (oHttpWebResponse.StatusCode == HttpStatusCode.OK)
                            {

                                using (oResponseStream = oHttpWebResponse.GetResponseStream())
                                {

                                    oStreamReader = new StreamReader(oResponseStream);

                                    sHtmlToRetun = oStreamReader.ReadToEnd();

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error("Unable to get HTML", ex, ex);
                }
            }
            return sHtmlToRetun;
        }

        public static System.Net.Cookie CookieToNetCookie(HttpCookie oCookie, System.Text.Encoding oEncoding)
        {
            System.Net.Cookie oNetCookie;

            oNetCookie = new Cookie();

            oNetCookie.Name = oCookie.Name;
            oNetCookie.Value = HttpUtility.UrlEncode(oCookie.Value, oEncoding);

            if (String.IsNullOrEmpty(oCookie.Domain))
            {
                oNetCookie.Domain = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
            }
            else
            {
                oNetCookie.Domain = oCookie.Domain;
            }
            
            oNetCookie.Expires = oCookie.Expires;            
            oNetCookie.HttpOnly = oCookie.HttpOnly;
            oNetCookie.Path = oCookie.Path;
            oNetCookie.Secure = oCookie.Secure;

           
            return oNetCookie;

        }


        public static Encoding ParseEncoding(string sEncoding)
        {
            Encoding oParsedEncoding;

            switch (sEncoding)
            {
                case "unicode":
                    oParsedEncoding = Encoding.Unicode;
                    break;
                case "ascii":
                    oParsedEncoding = Encoding.ASCII;
                    break;
                case "utf-32":
                    oParsedEncoding = Encoding.UTF32;
                    break;
                case "utf-7":
                    oParsedEncoding = Encoding.UTF7;
                    break;
                case "utf-8":
                    oParsedEncoding = Encoding.UTF8;
                    break;
                case "default":
                default:
                    oParsedEncoding = Encoding.Default;
                    break;
            }

            return oParsedEncoding;
        }

        public static string GetHtmlFromUrl(string sEncoding, string sHtmlURL)
        {
            WebClient oClient;
            string sHtmlToRetun;

            
            //create a web client
            oClient = new WebClient();
            sHtmlToRetun = string.Empty;
            //based on the encoding chosen, tell the web client how to download
            switch (sEncoding)
            {
                case "unicode":
                    oClient.Encoding = Encoding.Unicode;
                    break;
                case "ascii":
                    oClient.Encoding = Encoding.ASCII;
                    break;
                case "utf-32":
                    oClient.Encoding = Encoding.UTF32;
                    break;
                case "utf-7":
                    oClient.Encoding = Encoding.UTF7;
                    break;
                case "utf-8":
                    oClient.Encoding = Encoding.UTF8;
                    break;
                case "default":
                default:
                    oClient.Encoding = Encoding.Default;
                    break;
            }

            //get HTML from remote system
            sHtmlToRetun = oClient.DownloadString(sHtmlURL).Trim();

            return sHtmlToRetun;
        }
    }
}
