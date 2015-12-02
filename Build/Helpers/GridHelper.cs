using Genworth.SitecoreExt.Providers;
namespace Genworth.SitecoreExt.Helpers
{
    public class GridHelper
    {
        public static IJsonCollectionProvider GetProvider(string sProviderName)
        {
            IJsonCollectionProvider oProvider;
            
            if ((oProvider = EventHelper.GetProvider(sProviderName)) == null)
            {
                oProvider = InvestmentHelper.GetProvider(sProviderName);
            }

            return oProvider;
        }
    }
}
