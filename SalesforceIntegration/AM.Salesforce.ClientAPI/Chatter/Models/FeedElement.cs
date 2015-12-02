using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FeedElement
    {
        [JsonProperty(PropertyName = "feedElementType")]
        public string FeedElementType { get; set; }

        [JsonProperty(PropertyName = "body")]
        public MessageBodyInput Body { get; set; }

        [JsonProperty(PropertyName = "subjectId")]
        public string SubjectId { get; set; }

        [JsonProperty(PropertyName = "capabilities")]
        public Capabilities Capabilities { get; set; }

    }
}
