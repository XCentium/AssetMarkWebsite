using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace Genworth.SitecoreExt.Utilities
{
    public class Maps
    {



        #region PROPERTIES

        public static string StaticMapBaseURL
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Integrations.Google.StaticMapBaseURL, string.Empty);
            }
        }

        public static string MapsClienSideBaseURL
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Integrations.Google.MapsClienSideBaseURL, string.Empty);
            }
        }

        #endregion

    }

}
