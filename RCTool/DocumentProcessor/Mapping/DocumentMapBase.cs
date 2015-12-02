using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentProcessor.Strategies;
using DocumentEntities;

namespace DocumentProcessor.Mapping
{
    public abstract class DocumentMapBase
    {
        protected RcToolsData Map;

        protected DataStrategyBase Strategy;

        public DocumentMapBase() { }

        public DocumentMapBase(DataStrategyBase strategy)
            : this()
        {
            this.Strategy = strategy;
        }

        public abstract RcToolsData Get();
    }
}
