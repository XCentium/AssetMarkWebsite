using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DocumentEntities;
using System.IO;

namespace DocumentProcessor.Helpers
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IRCToolService
    {
        /// <summary>
        /// REST Service Interface. Returns file mapping data. Default data format returned: XML
        /// </summary>
        /// <param name="value"></param>
        /// <returns>XML response format, using XMLSerializerFormat attribute</returns>
        [OperationContract]
        [XmlSerializerFormat]
        [WebGet(UriTemplate = "getfilemap/{value}", BodyStyle = WebMessageBodyStyle.Bare)]
        DocumentList GetFileMap(string value);

        /// <summary>
        /// REST Service Interface. Returns file mapping data. Data format returned as indicated in query: XML
        /// </summary>
        /// <param name="value"></param>
        /// <returns>XML response format, using XMLSerializerFormat attribute</returns>
        [OperationContract]
        [XmlSerializerFormat]
        [WebGet(UriTemplate = "getfilemap/{value}/xml", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml)]
        DocumentList GetFileMapXML(string value);

        /// <summary>
        /// REST Service Interface. Returns file mapping data. Data format returned as indicated in query: JSON
        /// </summary>
        /// <param name="value"></param>
        /// <returns>JSON formatted data</returns>
        [OperationContract]
        [WebGet(UriTemplate = "getfilemap/{value}/json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        DocumentList GetFileMapJSON(string value);

        /// <summary>
        /// REST Service Interface. Returns file mapping data in the specified format.
        /// *ISSUES with XML, since it does not use XmlSerializerFormat attribute*
        /// </summary>
        /// <param name="value"></param>
        /// <returns>JSON or XML formatted data</returns>
        [OperationContract]
        [WebGet(UriTemplate = "getfilemap/format/{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        DocumentList GetFileMapWithFormat(string format);

        /// <summary>
        /// REST Service Interface. Returns file mapping data as a Stream object and serialized in the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "getfilemap/stream/{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetFileMapStreamedWithFormat(string format);

    }
}
