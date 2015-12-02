using Salesforce.Common.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Models
{
    public class MultipartFormDataObject
    {
        public object InputObject { get; set; }
        public string MimeType { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }

        public IContentBuilder ContentBuilder { get; set; }
    }
}
