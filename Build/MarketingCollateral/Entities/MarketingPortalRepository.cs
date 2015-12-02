using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Entities
{
    public class MarketingPortalRepository : BaseRepository
    {
        public string FTPHost { get; set; }
        public string FTPUsername { get; set; }
        public string FTPPassword { get; set; }
        public string InitialFolder { get; set; }
        public bool EnableSsl { get; set; }

    }
}
