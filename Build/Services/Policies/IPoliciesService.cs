using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Genworth.SitecoreExt.Services.Policies
{
    [ServiceContract]
    public interface IPoliciesService
    {        
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetVersionedPolicy/{sPolicyId}/{sVersion}")]
        Policy GetVersionedPolicy(string sPolicyId, string sVersion);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetPolicy/{sPolicyId}")]
        Policy GetPolicy(string sPolicyId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetPolicyVersions/{sPolicyId}")]
        int[] GetPolicyVersions(string sPolicyId);
        
    }
    
}
