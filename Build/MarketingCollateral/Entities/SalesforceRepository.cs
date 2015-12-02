using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Entities
{
    public class SalesforceRepository : BaseRepository
    {
        public string LibraryPath { get; set; }
        public string GroupSubjectId { get; set; }
        public string LibrarySubjectId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecurityToken { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string TokenRequestEndpointUrl { get; set; }

    }
}
