using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Entities
{
    public class MarketingCollateralItem
    {

        public string SubmitterName { get; set; }

        public string Department { get; set; }

        public string FormNumber { get; set; }

        public string ComplianceNumber { get; set; }

        public string DownloadName { get; set; }

        public string UploadPath { get; set; }

        public Stream File { get; set; }

        public string FileName { get; set; }

        public Dictionary<string, RepositoryData> RepositoryData { get; set; }

        public string Comment { get; set; }

        public string EWMRepositoryAdditionalInfo { get; set; }

        public string AssetMarkRepositoryAdditionalInfo { get; set; }

        public string MarketingPortalRepositoryAdditionalInfo { get; set; }

        public bool SendNotification { get; set; }

        public string Emails { get; set; }

        public string EmailBody { get; set; }

        public string ChatterMessage { get; set; }

        public string SalesforceDocumentId { get; set; }

        public string SalesforceFeedItemId { get; set; }

        public bool IsReplacing { get; set; }
    }
}
