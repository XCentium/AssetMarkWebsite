using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerLogic.SitecoreExt.Async
{
    public interface IAsyncLayout
    {
        void RegisterSublayout(IAsyncSublayout oSublayout);
    }
}
