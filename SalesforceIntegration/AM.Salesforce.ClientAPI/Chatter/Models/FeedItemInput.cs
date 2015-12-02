using Newtonsoft.Json;

namespace Salesforce.Chatter.Models
{
    public class FeedItemInput
    {
        [JsonProperty(PropertyName = "attachment")]
        public Attachment Attachment { get; set; }

        [JsonProperty(PropertyName = "body")]
        public MessageBodyInput Body { get; set; }

        [JsonProperty(PropertyName = "subjectId")]
        public string SubjectId { get; set; }
    }
}