using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

namespace Genworth.SitecoreExt.Services.Events
{
	[ServiceContract]
    [ServiceKnownType(typeof(EventsAll))]
    [ServiceKnownType(typeof(EventsWebinar))]
    [ServiceKnownType(typeof(EventsInPerson))]
    [ServiceKnownType(typeof(EventsArchive))]
    [ServiceKnownType(typeof(EventDataItem[]))]
	public interface IEventsService
	{
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
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetSessionObject/{sType}")]
        EventsBase Reset(string sType);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetResults/{sType}")]
        EventDataItem[] GetResults(string sType);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetDate/{sType}/{sDate}")]
        EventsBase SetDate(string sType, string sDate);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetSort/{sType}/{sField}")]
        EventsBase SetSort(string sType, string sField);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetResultsPerPage/{sType}/{sResultsPerPage}")]
        EventsBase SetResultsPerPage(string sType, string sResultsPerPage);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetPage/{sType}/{sPage}")]
        EventsBase SetPage(string sType, string sPage);

		[OperationContract]
		[WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetKeywords?Keywords={sKeywords}&Type={sType}")]
        EventsBase SetKeywords(string sType, string sKeywords);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "SetLocation?type={sType}&ratio={sRatio}&zipCode={sZipCode}")]
        EventsBase SetLocation(string sType, string sRatio, string sZipCode);
	}
}
