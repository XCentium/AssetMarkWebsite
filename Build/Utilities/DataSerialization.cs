using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Genworth.SitecoreExt.Utilities
{
    public class DataSerialization
    {
        /// <summary>
        /// Receives any entity class and serializes it in XML format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Returns string object</returns>
        public static string ToXmlString<T>(T entity, string xmlNamespace = null, bool omitXmlDeclaration = false)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlNamespace);
            return ToXmlString<T>(serializer, entity, xmlNamespace, omitXmlDeclaration);
        }

        /// <summary>
        /// Receives any entity class and serializes it in XML format including additional types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="types"></param>
        /// <returns>Returns string object</returns>
        public static string ToXmlString<T>(T entity, Type[] types, string xmlNamespace = null, bool omitXmlDeclaration = false)
        {
            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            
            XmlSerializer serializer = new XmlSerializer(typeof(T), null, types, null, xmlNamespace);
            return ToXmlString<T>(serializer, entity, xmlNamespace, omitXmlDeclaration);
        }

        /// <summary>
        /// Receives initialized Xml Serializer object and entity to be serialized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer"></param>
        /// <param name="entity"></param>
        /// <returns>Returns string object</returns>
        private static string ToXmlString<T>(XmlSerializer serializer, T entity, string xmlNamespace = null, bool omitXmlDeclaration = false)
        {
            StringBuilder xmlStr = new StringBuilder();

            try
            {         
                XmlSerializerNamespaces xmlSerializerSettings = new XmlSerializerNamespaces();
                xmlSerializerSettings.Add("", xmlNamespace ?? "");

                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;
                XmlWriter writer = XmlTextWriter.Create(xmlStr, xmlWriterSettings);

                serializer.Serialize(writer, entity, xmlSerializerSettings);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while serializing an object to a string with XML format.", ex, typeof(DataSerialization));
            }

            return xmlStr.ToString();
        }
    }
}


