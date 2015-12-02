using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AssetMark.SitecoreExt.Services.Marketing
{
    [KnownType(typeof(MarketingCollateralDocumentItem))]
    [KnownType(typeof(NameValueCollection))]
    [DataContract]
    public class MarketingCollateralRootDataItem
    {
        [DataMember(Name="data")]
        public MarketingCollateralDocumentItem[] Items { get; set; }
    }
}
