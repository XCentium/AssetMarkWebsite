using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentProcessor.Utilities;

namespace DocumentProcessor.Format
{
    public class JsonProvider : IDataFormatProvider
    {
        public System.IO.Stream SerializeToStream<T>(T entity)
        {
            return DataSerialization.ToJsonStream<T>(entity);
        }

        public string SerializeToString<T>(T entity)
        {
            return DataSerialization.ToJsonString<T>(entity);
        }

        public System.IO.Stream SerializeToStream<T>(T entity, Type[] types)
        {
            return DataSerialization.ToJsonStream<T>(entity, types);
        }

        public string SerializeToString<T>(T entity, Type[] types)
        {
            return DataSerialization.ToJsonString<T>(entity, types);
        }
    }
}
