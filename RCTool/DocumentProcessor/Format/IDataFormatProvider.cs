using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DocumentProcessor.Format
{
    public interface IDataFormatProvider
    {
        Stream SerializeToStream<T>(T entity);
        Stream SerializeToStream<T>(T entity, Type[] types);
        string SerializeToString<T>(T entity);
        string SerializeToString<T>(T entity, Type[] types);
    }
}
