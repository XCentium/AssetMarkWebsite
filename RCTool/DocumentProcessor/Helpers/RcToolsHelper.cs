using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.Net.Mime;
using System.Text;
using DocumentEntities;
using System.Xml.Serialization;
using System.IO;
using DocumentProcessor.Format;

namespace DocumentProcessor.Helpers
{
    public class RcToolsHelper
    {
        /// <summary>
        /// Analyzes the incoming request by checking explicit or automatic response format
        /// </summary>
        public static void ApplyReturnFormat()
        {
            string formatQueryStringValue = GetCurrentFormat();

            // explicit format takes precedence
            if (!string.IsNullOrEmpty(formatQueryStringValue))
            {
                ApplyReturnFormat(formatQueryStringValue);
            }
            else
            {
                // if no format is specfied directly on the URI template, look for the accepting encoding headers
                string acceptEncodingFormat = GetAcceptingEncodingFromRequest();

                if (!string.IsNullOrEmpty(acceptEncodingFormat))
                {
                    ApplyReturnFormat(formatQueryStringValue);
                }
                else
                {
                    // Apply default, if no format is specified in the request
                    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;                    
                }
            }
        }

        /// <summary>
        /// Gets the current format sent to the service for specifying the returning data format
        /// </summary>
        /// <returns>string</returns>
        public static string GetCurrentFormat()
        {
            return WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BoundVariables["format"];
        }

        /// <summary>
        /// Retrieves from the request the accepting encoding headers, either for JSON or XML.
        /// </summary>
        /// <returns></returns>
        public static string GetAcceptingEncodingFromRequest()
        {
            string acceptEncodingValue = string.Empty;
            IList<ContentType> acceptHeaderElements = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();
            for (int i = 0; i < acceptHeaderElements.Count; i++)
            {
                string normalizedMediaType = acceptHeaderElements[i].MediaType.ToLowerInvariant();
                switch (normalizedMediaType)
                {
                    case "application/xml":
                    case "application/json":
                        acceptEncodingValue = normalizedMediaType;
                        break;
                }

                // accepting encoding found
                if (!string.IsNullOrWhiteSpace(acceptEncodingValue))
                    break;
            }

            return acceptEncodingValue;
        }

        /// <summary>
        /// Directly specifies the return response format. JSON or XML.
        /// </summary>
        /// <param name="format"></param>
        public static void ApplyReturnFormat(string format)
        {
            string applyingFormat = format != null ? format : string.Empty;

            switch (applyingFormat.ToUpper())
            {
                case "JSON":
                    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
                    break;
                case "XML":
                    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;
                    break;
                default:
                    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;
                    break;
            }
        }

        /// <summary>
        /// Returns a crafted/generic WCF Message for custom data formatting, with current incoming request data format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Custom formatted data</returns>
        public static System.ServiceModel.Channels.Message GetMessage<T>(T entity)
        {
            string format = GetCurrentFormat();
            return GetMessage<T>(format, entity);
        }

        /// <summary>
        /// Returns a crafted/generic WCF Message for custom data formatting, with specified data format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="documentList"></param>
        /// <returns>Custom formatted data</returns>
        public static System.ServiceModel.Channels.Message GetMessage<T>(string format, T entity)
        {
            System.ServiceModel.Channels.Message message = null;
            switch (format.ToLower())
            {
                case "xml":
                    message = WebOperationContext.Current.CreateXmlResponse<T>(entity, new XmlSerializer(typeof(T)));
                    break;
                case "json":
                    message = WebOperationContext.Current.CreateJsonResponse<T>(entity);
                    break;
            }
            return message;
        }

        /// <summary>
        /// Converts an entity object to the current data format from the incoming request
        /// </summary>
        /// <param name="documentList"></param>
        /// <returns>Entity object in a Stream object</returns>
        public static System.IO.Stream ConvertToStream<T>(T entity)
        {
            string format = GetCurrentFormat();
            return ConvertToStreamWithFormat(entity, format);
        }

        public static System.IO.Stream ConvertToStream<T>(T entity, Type[] types)
        {
            string format = GetCurrentFormat();
            return ConvertToStreamWithFormat(entity, format, types);
        }

        public static string ConvertToString<T>(T entity)
        {
            string format = GetCurrentFormat();
            return ConvertToStringWithFormat(entity, format);
        }

        /// <summary>
        /// Converts an entity object to the current data format from the incoming request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="format"></param>
        /// <returns>Entity object in a Stream object</returns>
        public static System.IO.Stream ConvertToStreamWithFormat<T>(T entity, string format)
        {
            IDataFormatProvider formatProvider = GetDataFormatProvider(format);
            return formatProvider.SerializeToStream<T>(entity);
        }

        public static System.IO.Stream ConvertToStreamWithFormat<T>(T entity, string format, Type[] types)
        {
            IDataFormatProvider formatProvider = GetDataFormatProvider(format);
            return formatProvider.SerializeToStream<T>(entity, types);
        }

        public static string ConvertToStringWithFormat<T>(T entity, string format)
        {
            IDataFormatProvider formatProvider = GetDataFormatProvider(format);
            return formatProvider.SerializeToString<T>(entity);
        }

        /// <summary>
        /// Retrieves the data format provider depending on the incoming format parameter
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static IDataFormatProvider GetDataFormatProvider(string format)
        {
            IDataFormatProvider formatProvider = null;
            switch (format.ToLower())
            {
                case "xml":
                    formatProvider = new XmlProvider();
                    break;
                case "json":
                    formatProvider = new JsonProvider();
                    break;
                default:
                    formatProvider = new XmlProvider();
                    break;
            }
            return formatProvider;
        }
        
    }
}
