using Salesforce.Common.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Content
{
    public class UrlEncodedContentBuilder : IContentBuilder
    {
        Salesforce.Common.Models.MultipartFormDataObject multipartObject;

        public UrlEncodedContentBuilder(Salesforce.Common.Models.MultipartFormDataObject multipartObject)
        {
            this.multipartObject = multipartObject;
        }

        public void BuildHttpContent(System.Net.Http.MultipartFormDataContent multipartFormDataContent)
        {
            var content = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)multipartObject.InputObject);
            multipartFormDataContent.Add(content);
        }
    }
}
