using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Entities
{
    public class MarketingCollateralResponse
    {
        public List<MarketingCollateralMessage> values { get; private set; }
        public string DownloadFileUrl { get; set; }

        public void Add(string value, ResponseStatus status)
        {
            if (values == null)
            {
                values = new List<MarketingCollateralMessage>();
            }

            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            values.Add(new MarketingCollateralMessage(value, status));
        }
    }
}
