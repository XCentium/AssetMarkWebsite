using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetMark.SitecoreExt
{
    public static class MarketingCollateralLog
    {
        private static ILog log;

        public static ILog Log
        {
            get
            {
                return log ?? (log = Sitecore.Diagnostics.LoggerFactory.GetLogger("Sitecore.Diagnostics.MarketingCollateral"));
            }
        }
      }
}
