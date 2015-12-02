using System.Collections.Generic;
namespace Genworth.SitecoreExt.Providers
{
    public interface IColumnsProvider
    {
        List<KeyValuePair<string, string>> Columns { get; } // Add here the name and code of the grid columns.
    }
}
