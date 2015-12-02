using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FileShareItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "sharingType")]
        public string SharingType { get; set; }
    }
}
