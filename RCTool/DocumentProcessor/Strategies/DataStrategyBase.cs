using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentEntities;

namespace DocumentProcessor.Strategies
{
    public abstract class DataStrategyBase
    {
        public DataStrategyBase()
        {
        }

        public abstract RcToolsData Get();
    }
}
