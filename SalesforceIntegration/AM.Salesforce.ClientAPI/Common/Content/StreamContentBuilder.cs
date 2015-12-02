using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Content
{
    public class StreamContentBuilder : IContentBuilder
    {
        Models.MultipartFormDataObject multipartObject;

        public StreamContentBuilder(Models.MultipartFormDataObject multipartObject)
        {
            this.multipartObject = multipartObject;
        }

        public void BuildHttpContent(System.Net.Http.MultipartFormDataContent multipartFormDataContent)
        {
            var content = new StreamContent((Stream)multipartObject.InputObject);
            content.Headers.Clear();
            content.Headers.Add("Content-Type", multipartObject.MimeType); // commonly used: application/octet-stream
            content.Headers.Add("Content-Disposition", string.Format("form-data; name=\"{0}\"; filename=\"{1}\"", multipartObject.Name, multipartObject.FileName));
            multipartFormDataContent.Add(content, multipartObject.Name, multipartObject.FileName);
        }
    }
}
