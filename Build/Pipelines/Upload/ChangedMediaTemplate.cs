using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.Pipelines.Upload
{
    public class ChangedMediaTemplate
    {
        public string MimeType { get; set; }
        public string Old { get; set; }
        public string New { get; set; }
    }
}
