using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentProcessor.Strategies;
using DocumentEntities;

namespace DocumentProcessor.Mapping
{
    /// <summary>
    /// This class returns just a documents object containg Document Entity. The ResponseFormat of the Web Service will define the rest - JSON, XML..
    /// </summary>
    public class Map : MapBase
    {
        //public Map() { }
        //public Map(StrategyBase strategy)
        //    : this()
        //{
        //    this.Strategy = strategy;
        //}

        //public override DocumentList Get()
        //{
        //    return this.Strategy.Get();
        //}
    }
}
