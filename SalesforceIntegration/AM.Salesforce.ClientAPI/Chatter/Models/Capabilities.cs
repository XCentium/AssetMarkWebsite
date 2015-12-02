using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class Capabilities
    {
        [JsonProperty(PropertyName = "content")]
        public CapabilitiesContent Content { get; set; }
    }
}
