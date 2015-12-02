using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Entities
{
    public class RepositoryData
    {
        public string Name { get; set; }
        public string KeyName { get; set; }
        public bool IsSelected { get; set; }
        public string DownloadName { get; set; }
        public string OwnerEmail { get; set; }
        public string DocumentState { get; set; }
        public string RepositoryLookupId { get; set; }
    }
}
