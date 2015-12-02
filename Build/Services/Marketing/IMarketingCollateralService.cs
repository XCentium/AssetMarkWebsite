using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Collections.Specialized;

namespace AssetMark.SitecoreExt.Services.Marketing
{
    [ServiceContract]
    [ServiceKnownType(typeof(MarketingCollateralDocumentItem))]
    [ServiceKnownType(typeof(MarketingCollateralRootDataItem))]
    [ServiceKnownType(typeof(NameValueCollection))]
    public interface IMarketingCollateralService
    {
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDocumentList?templateIds={templateIds}")]
        MarketingCollateralRootDataItem GetDocumentList(string templateIds);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDocumentEntry/{itemId}")]
        MarketingCollateralRootDataItem GetDocumentEntry(string itemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateRepositoryState/{itemId}/{repositoryName}/{value}")]
        bool UpdateRepositoryState(string itemId, string repositoryName, string value);
    }
}

