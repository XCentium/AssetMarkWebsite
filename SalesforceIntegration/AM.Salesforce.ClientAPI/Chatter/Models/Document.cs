using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Document
    {
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Keywords")]
        public string Keywords { get; set; }

        [JsonProperty(PropertyName = "FolderId")]
        public string FolderId { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        //[JsonProperty(PropertyName = "success")]
        //public string Success { get; set; }
    }
}
