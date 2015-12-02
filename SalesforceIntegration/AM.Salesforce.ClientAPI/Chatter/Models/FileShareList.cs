using Newtonsoft.Json;
using System.Collections.Generic;

namespace Salesforce.Chatter.Models
{
    public class FileShareList
    {
        [JsonProperty(PropertyName = "shares")]
        public List<FileShareItem> Shares { get; set; }
    }
}
