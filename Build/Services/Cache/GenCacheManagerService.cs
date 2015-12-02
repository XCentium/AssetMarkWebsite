using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Caching;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Genworth.SitecoreExt.Services.Cache
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class GenCacheManagerService : IGenCacheManagerService
    {
        public void ClearAllCache()
        {
            foreach (Sitecore.Caching.Cache cache in CacheManager.GetAllCaches())
            {
                cache.Clear();
            }            
        }
    }
}
