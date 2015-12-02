using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentEntities;

namespace DocumentProcessor.Strategies
{
    public class NonSecureMockDataStrategy : DataStrategyBase
    {
        public NonSecureMockDataStrategy()
            : base()
        {
        }

        public override RcToolsData Get()
        {
            return new CheckData()
            {
                Checksum = "14fde92e3629116bef9cb462e2301ba0ec90c3b9"
            };
        }
    }
}
