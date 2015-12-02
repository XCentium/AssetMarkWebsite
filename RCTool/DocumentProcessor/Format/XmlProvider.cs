using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentProcessor.Utilities;

namespace DocumentProcessor.Format
{
    public class XmlProvider : IDataFormatProvider
    {
        public System.IO.Stream SerializeToStream<T>(T entity)
        {
            return DataSerialization.ToXmlStream<T>(entity);
        }

        public string SerializeToString<T>(T entity)
        {
            return DataSerialization.ToXmlString<T>(entity);
        }

        public System.IO.Stream SerializeToStream<T>(T entity, Type[] types)
        {
            return DataSerialization.ToXmlStream<T>(entity, types);
        }

        public string SerializeToString<T>(T entity, Type[] types)
        {
            return DataSerialization.ToXmlString<T>(entity, types);
        }
    }
}
