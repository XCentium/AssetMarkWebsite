using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Entities
{
    public class BaseRepository
    {
        public RepositoryType RepositoryType { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public bool EnableEmailNotificationSection { get; set; }

        public string RepositoryOwnerEmail { get; set; }
    }
}
