using System.Collections.Generic;
using System.Collections.Specialized;
namespace Genworth.SitecoreExt.Providers
{
    public interface IJsonProvider
    {
        bool SetURLFilters(NameValueCollection oQueryString); // This is necessary to redirect to another URL.
        string URL { get; } // Redirect URL.
        string Code { get; } // This code is to differentiate the different grids.
        string JsonServiceUrl { get; } // The url of the service to deal with the grid component.
    }
}
