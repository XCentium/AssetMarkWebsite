using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentEntities;
using DocumentProcessor.Strategies;

namespace DocumentProcessor.Mapping
{
    public class DataMap : DocumentMapBase
    {
        public DataMap() { }
        public DataMap(DataStrategyBase strategy)
            : this()
        {
            this.Strategy = strategy;
        }

        public override RcToolsData Get()
        {
            return this.Strategy.Get();
        }
    }
}
