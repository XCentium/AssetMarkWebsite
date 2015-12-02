using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DocumentEntities;
using System.IO;
using TestEntities;

namespace TestRCToolService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        // TODO: Add your service operations here
        /// <summary>
        /// BodySytle = WebMessageBodyStyle.Bare - we don't have multiple POST params so we don't need to wrap; file is a stream so there is no need to wrap respone either
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        //[ContentType("audio/x-ms-wmv")]
        [WebGet(UriTemplate = "get/{value}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Get(string value);

        [OperationContract]
        [WebGet(UriTemplate = "getfilemap/{value}", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml)]
        IList<Document> GetFileMapDefaultFormat(string value);

        [OperationContract]
        [WebGet(UriTemplate = "getfilemap/{value}/json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        IList<Document> GetFileMapJSONFormat(string value);

        [OperationContract]
        [WebGet(UriTemplate = "getfilemap/{value}/xml", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml)]
        IList<Document> GetFileMapXMLFormat(string value);

        [OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "reports/{value}")]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "reports/{value}")]
        Stream GetReport(string value);

        [OperationContract]
        //[WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST", UriTemplate = "postreports/{value}")]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "bigreports/{value}")]
        Stream GetPostReport(string value);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "postList")]
        List<PostResponse> GetDocumentList();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "checksum/{value}")]
        string GetCheckSum(string value);

        /// <summary>
        /// REST web method, receives CSV file in POST for partial sync of RC Tools.
        /// </summary>
        /// <param name="csvStream"></param>
        /// <returns>Updated CSV file</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "sync/partial")]
        Stream GetPartialSync(Stream csvStream);

        [OperationContract]
        [XmlSerializerFormat]
        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "write/nonsecure")]
        string WriteNonSecureXML();

        [OperationContract]
        [XmlSerializerFormat]
        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "write/secure")]
        string WriteSecureXML();

        [OperationContract]
        [XmlSerializerFormat]
        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "load/library")]
        string LoadRcLibrary();


    }
}
