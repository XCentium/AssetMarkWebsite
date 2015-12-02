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
	[ServiceKnownType(typeof(ModelPortfolio))]
	[ServiceKnownType(typeof(ResearchClientView))]
	[ServiceKnownType(typeof(Research))]
	[ServiceKnownType(typeof(Result))]
	[ServiceKnownType(typeof(Result[]))]
	[ServiceKnownType(typeof(ModelPortfolioResult))]
	[ServiceKnownType(typeof(ModelPortfolioResult[]))]
	public interface IResearchService
	{
		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetCurrentTime")]
		string GetCurrentTime();

		string Value
		{
			[OperationContract]
			[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetValue/{value}")]
			set;
			[OperationContract]
			[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetValue")]
			get;
		}

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetResearch/{sType}")]
		InvestmentsSearchBase Research(string sType);
		

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetFilterOption/{sType}/{sFilter}/{sOption}/{sFiltered}")]
		InvestmentsSearchBase SetFilterOption(string sType,string sFilter, string sOption, string sFiltered);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetDateRange/{sType}/{sFromDate}/{sToDate}")]
		InvestmentsSearchBase SetDateRange(string sType, string sFromDate, string sToDate);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetDateLookback/{sType}/{sMonths}")]
		InvestmentsSearchBase SetDateLookback(string sType, string sMonths);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetResults/{sType}")]
		ResultBase[] GetResults(string sType);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetSort/{sType}/{sField}")]
		InvestmentsSearchBase SetSort(string sType,string sField);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetResultsPerPage/{sType}/{sResultsPerPage}")]
		InvestmentsSearchBase SetResultsPerPage(string sType,string sResultsPerPage);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetPage/{sType}/{sPage}")]
		InvestmentsSearchBase SetPage(string sType,string sPage);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetKeywords?Keywords={sKeywords}&Type={sType}")]
		InvestmentsSearchBase SetKeywords(string sType,string sKeywords);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "Reset/{sType}")]
		InvestmentsSearchBase Reset(string sType);
	}
}