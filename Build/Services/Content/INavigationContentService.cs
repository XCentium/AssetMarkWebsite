using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Genworth.SitecoreExt.Services.Content
{
    [ServiceContract]
    interface INavigationContentService
    {
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetPrimaryNavigationContent")]
        NavigationItem[] GetPrimaryNavigationContent();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAlerts/{itemID}")]
        IEnumerable<NotificationItem> GetAlerts(string itemID);
    }
}
