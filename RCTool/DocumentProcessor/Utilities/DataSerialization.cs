using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace DocumentProcessor.Utilities
{
    public class DataSerialization
    {
        /// <summary>
        /// Receives any entity class and serializes in JSON format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Returns Stream object</returns>
        public static Stream ToJsonStream<T>(T entity)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, entity);
            ms.Position = 0;
            return ms;
        }

        public static Stream ToJsonStream<T>(T entity, Type[] types)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), types);
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, entity);
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Receives any entity class and serializes in JSON format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Returns string object</returns>
        public static string ToJsonString<T>(T entity)
        {
            Stream stream = ToJsonStream<T>(entity);
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        public static string ToJsonString<T>(T entity, Type[] types)
        {
            Stream stream = ToJsonStream<T>(entity, types);
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Receives any entity class and serializes in XML format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Returns Stream object</returns>
        public static Stream ToXmlStream<T>(T entity)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return ToXmlStream(entity, serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Stream ToXmlStream<T>(T entity, Type[] types)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), types);
            return ToXmlStream(entity, serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        private static Stream ToXmlStream<T>(T entity, XmlSerializer serializer)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xmlWriterSettings = GetDefaultXmlWriterSettings();
            XmlWriter writer = XmlTextWriter.Create(ms, xmlWriterSettings);

            XmlSerializerNamespaces xmlSerializerSettings = new XmlSerializerNamespaces();
            xmlSerializerSettings.Add("", "");

            serializer.Serialize(writer, entity, xmlSerializerSettings);
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Receives any entity class and serializes in XML format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Returns string object</returns>
        public static string ToXmlString<T>(T entity)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return ToXmlString(entity, serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static string ToXmlString<T>(T entity, Type[] types)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), types);
            return ToXmlString(entity, serializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        private static string ToXmlString<T>(T entity, XmlSerializer serializer)
        {
            StringBuilder xmlStr = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = GetDefaultXmlWriterSettings();
            XmlWriter writer = XmlTextWriter.Create(xmlStr, xmlWriterSettings);

            XmlSerializerNamespaces xmlSerializerSettings = new XmlSerializerNamespaces();
            xmlSerializerSettings.Add("", "");

            serializer.Serialize(writer, entity, xmlSerializerSettings);

            return xmlStr.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static XmlWriterSettings GetDefaultXmlWriterSettings()
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = false;
            xmlWriterSettings.Encoding = Encoding.UTF8;
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.NewLineChars = Environment.NewLine;
            xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
            xmlWriterSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            return xmlWriterSettings;
        }
    }
}
