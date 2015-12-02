using Newtonsoft.Json;
using Salesforce.Common.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Content
{
    public class StringContentBuilder : IContentBuilder
    {
        Salesforce.Common.Models.MultipartFormDataObject multipartObject;

        public StringContentBuilder(Salesforce.Common.Models.MultipartFormDataObject multipartObject)
        {
            this.multipartObject = multipartObject;
        }

        public void BuildHttpContent(MultipartFormDataContent multipartFormDataContent)
        {
            var json = JsonConvert.SerializeObject(multipartObject.InputObject,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CreateableContractResolver()
                    });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("Content-Disposition", string.Format("form-data; name=\"{0}\"", multipartObject.Name));
            multipartFormDataContent.Add(content, multipartObject.Name);
        }
    }
}
