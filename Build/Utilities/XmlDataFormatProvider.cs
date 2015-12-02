using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Utilities
{
    public class XmlDataFormatProvider : IDataFormatProvider
    {
        /// <summary>
        /// Serializes an entity class object to an Xml formatted string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string SerializeToString<T>(T entity, string xmlNamespace = null, bool omitXmlDeclaration = false)
        {
            return DataSerialization.ToXmlString<T>(entity, xmlNamespace, omitXmlDeclaration);
        }

        /// <summary>
        /// Serializes an entity class object to an Xml formatted string also using additional object types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public string SerializeToString<T>(T entity, Type[] types, string xmlNamespace = null, bool omitXmlDeclaration = false)
        {         
            return DataSerialization.ToXmlString<T>(entity, types, xmlNamespace, omitXmlDeclaration);
        }

        public T DeserializeFromString<T>(string data)
        {
            throw new NotImplementedException();
        }

        public T DeserializeFromString<T>(string data, Type[] types)
        {
            throw new NotImplementedException();
        }
    }
}

