using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Genworth.SitecoreExt.Services.Cache
{
    [ServiceContract]	
    public interface IGenCacheManagerService
    {
        [OperationContract]
        void ClearAllCache();
    }
}
