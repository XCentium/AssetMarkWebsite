using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.MarketingCollateral.Entities
{
    public class MarketingCollateralMessage
    {
        public MarketingCollateralMessage(string value, ResponseStatus status)
        {
            this.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.Value = value;
            this.Status = status;
        }

        public string TimeStamp { get; set; }

        public string Value { get; set; }

        public ResponseStatus Status { get; set; }
    }

    public enum ResponseStatus
    {
        Success,
        Warning,
        Error
    }
}
