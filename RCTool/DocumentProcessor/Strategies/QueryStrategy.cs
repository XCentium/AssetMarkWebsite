using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentEntities;

namespace DocumentProcessor.Strategies
{
    /// <summary>
    /// Use a Sitecore Fast query to process request
    /// </summary>
    public class QueryStrategy : StrategyBase
    {
        public QueryStrategy()
            : base()
        {
        }

        //public override DocumentList Get()
        //{
        //    return null;
        //}
    }
}
