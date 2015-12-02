using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class CapabilitiesContent
    {
        [JsonProperty(PropertyName = "contentDocumentId")]
        public string ContentDocumentId;
    }
}
