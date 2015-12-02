using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Utilities
{
    public interface IDataFormatProvider
    {
        string SerializeToString<T>(T entity, string xmlNamespace = null, bool omitXmlDeclaration = false);
        string SerializeToString<T>(T entity, Type[] types, string xmlNamespace = null, bool omitXmlDeclaration = false);
        T DeserializeFromString<T>(string data);
        T DeserializeFromString<T>(string data, Type[] types);
    }
}

