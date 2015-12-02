using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

namespace Genworth.SitecoreExt.Services.Investments
{
	[ServiceContract]
	public interface IManagerSearchService
	{
		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetStrategy/{sStrategyId}")]
		ManagerSearch.Strategy GetStrategy(string sStrategyId);
	}
}
