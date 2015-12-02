using Salesforce.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Content
{
    public interface IContentBuilder
    {
        void BuildHttpContent(MultipartFormDataContent multipartFormDataContent);
    }
}
