using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Content
{
    public class ByteArrayContentBuilder : IContentBuilder
    {
        Models.MultipartFormDataObject multipartObject;

        public ByteArrayContentBuilder(Models.MultipartFormDataObject multipartObject)
        {
            this.multipartObject = multipartObject;
        }

        public void BuildHttpContent(System.Net.Http.MultipartFormDataContent multipartFormDataContent)
        {
            var content = new ByteArrayContent((byte[])multipartObject.InputObject);
            content.Headers.Clear();
            content.Headers.Add("Content-Type", multipartObject.MimeType); // can be: application/pdf
            content.Headers.Add("Content-Disposition", string.Format("form-data; name=\"{0}\"; filename=\"{1}\"", multipartObject.Name, multipartObject.FileName));
            multipartFormDataContent.Add(content, multipartObject.Name, multipartObject.FileName);
        }
    }
}
