using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DocumentEntities;
using DocumentProcessor.Strategies;
using DocumentProcessor.Mapping;
using DocumentProcessor.Helpers;

namespace DocumentProcessor.Helpers
{
    public class RCTool : IRCToolService
    {
        /// <summary>
        /// Returns file mapping data. Default data format returned by the Service: XML
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DocumentList GetFileMap(string value)
        {
            RcToolsHelper.ApplyReturnFormat();
            return GetFileMapData(value);            
        }

        /// <summary>
        /// Returns file mapping data in the specified format.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public DocumentList GetFileMapWithFormat(string format)
        {
            /*
             * NOTE
             * Works OK with JSON. Unfortunately, it doesn't with XML entirely, since we are returning specific element names,
             * these aren't used during serialization, class's property names are used instead.
             * 
             * The GetFileMap method is defined in the Service's contract without using the XMLSerializerFormat attribute. This
             * doesn't trigger the usage of the XmlSerializer class instead of the default xml serializer. This way, xml element
             * names are serialized incorrectly.
             * 
             * An alternative is to use the GetMessage<T> method from the ServiceHelper class, but browsing the service gives an error,
             * although the service web methods work fine...
             */

            RcToolsHelper.ApplyReturnFormat();
            return GetFileMapData(format);
        }

        /// <summary>
        ///  Returns file mapping data. Data format returned as indicated from Service: XML
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DocumentList GetFileMapXML(string value)
        {
            return GetFileMapData(value);
        }

        /// <summary>
        /// Returns file mapping data. Data format returned as indicated from Service: JSON
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DocumentList GetFileMapJSON(string value)
        {
            return GetFileMapData(value);
        }

        /// <summary>
        /// Returns file mapping data. Data returned as a Stream object and serialized using the indicated format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public System.IO.Stream GetFileMapStreamedWithFormat(string format)
        {
            DocumentList documentList = GetFileMapData(format);
            return RcToolsHelper.ConvertToStream<DocumentList>(documentList);
        }

        /// <summary>
        /// Implementation of the GetFileMap logic
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DocumentList GetFileMapData(string value)
        {
            // Uncomment to use real implementation, instead of mock data
            // StrategyBase strategy = new IndexStrategy();

            StrategyBase strategy = new MockDataStrategy(); // use while in dev testing phase
            Map file = new Map(strategy);
            return file.Get();
        }




    }
}
