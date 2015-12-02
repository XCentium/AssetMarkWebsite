using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

namespace Genworth.SitecoreExt.Services.Marketing
{
    [ServiceContract]
    [ServiceKnownType(typeof(MarketingDataItem))]
    public interface IMarketingService
    {
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFormData/{agentId}/?url={actionUrl}")]
        MarketingDataItem GetHtmlFormData(string agentId, string actionUrl);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetUserRoles/{roleId}")]
        Dictionary<string, string> GetUserRoles(string roleId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetRequestUrl/{agentId}/?page={pageName}&product={productId}")]
        string GetMarcomCentralAuthenticationUrl(string agentId, string pagename, string productId);
    }
}

