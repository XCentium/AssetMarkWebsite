using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Entities
{
    public class SitecoreRepository : BaseRepository
    {
        public string SitecoreContentRootId { get; set; }

        public string MediaLibraryRootId { get; set; }

        public string ContentItemBaseTemplateName { get; set; }

        public string SelectableBaseTemplateId { get; set; }
    }

}
