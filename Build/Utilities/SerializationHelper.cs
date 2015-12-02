using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Utilities
{
    public class SerializationHelper
    {
        private static IDataFormatProvider provider = GetDefaultProvider();

        /// <summary>
        /// Serializes an entity class object using the default data format provider, returning it as a string object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Returns string object</returns>
        public static string Serialize<T>(T entity, string xmlNamespace = null, bool omitXmlDeclaration = false)
        {   
            string serializedData = string.Empty;

            try
            {
                if (entity != null)
                    serializedData = provider.SerializeToString<T>(entity, xmlNamespace, omitXmlDeclaration);
                else
                    throw new Exception("It was not possible to Serialize because the entity is null");
                
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while serializing an object using the SerializationHelper", ex, typeof(SerializationHelper));
            }
            
            return serializedData;
        }

        /// <summary>
        /// Serializes an entity class object using the default data format provider, and addtional object types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static string Serialize<T>(T entity, Type[] types, string xmlNamespace = null, bool omitXmlDeclaration = false)
        {
            string serializedData = string.Empty;

            try
            {
                if (entity != null)                
                    serializedData = provider.SerializeToString<T>(entity, types, xmlNamespace, omitXmlDeclaration);                
                else                
                    throw new Exception("It was not possible to Serialize because the entity is null");                
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while serializing an object using the SerializationHelper", ex, typeof(SerializationHelper));
            }
            
            return serializedData;
        }

        /// <summary>
        /// Deserializes an xml formatted string into an entity class object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string data)
        {
            T entity = provider.DeserializeFromString<T>(string.Empty);

            try
            {
                if (data != string.Empty)                                    
                    entity = provider.DeserializeFromString<T>(data);
                else
                    throw new Exception("It was not possible to Deserialize because the xml string is empty");                
            }
            catch(Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while serializing an object using the SerializationHelper", ex, typeof(SerializationHelper));
            }

            return entity;
        }

        /// <summary>
        /// Deserializes an xml formatted string into an entity class object, using additional object types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string data, Type[] types)
        {
            T entity = provider.DeserializeFromString<T>(data, types);

            try
            {
                if (data != string.Empty)
                    entity = provider.DeserializeFromString<T>(data);
                else
                    throw new Exception("It was not possible to Deserialize because the xml string is empty");         
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while serializing an object using the SerializationHelper", ex, typeof(SerializationHelper));
            }

            return entity;
        }

        /// <summary>
        /// Returns default data format provider for serialization tasks
        /// </summary>
        /// <returns></returns>
        private static IDataFormatProvider GetDefaultProvider()
        {            
            return new XmlDataFormatProvider();
        }
    }
}
