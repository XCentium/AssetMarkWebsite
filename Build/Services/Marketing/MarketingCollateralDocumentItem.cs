using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AssetMark.SitecoreExt.Services.Marketing
{
    [KnownType(typeof(NameValueCollection))]
    [DataContract]
    public class MarketingCollateralDocumentItem
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string FormNumber { get; set; }
        [DataMember]
        public string ComplianceNumber { get; set; }
        [DataMember]
        public Dictionary<string, string> RepositoryParameters { get; set; }
    }

    public enum RepositoryState
    {
        N, // no, not posted to the destination endpoint
        P, // pending, submitter indicated that document should be posted and it is pending a further workflow step from the destination endpoint owner (i.e., not connected via API).
        R, // requested, submitter indicated that document should be posted but destination endpoint is not integrated in a two-way fashion (i.e., ftp dropoff) so confirmation of delivery is not possible
        Y  // yes, posted to the destination endpoint and confirmation received from that system
    }
}
