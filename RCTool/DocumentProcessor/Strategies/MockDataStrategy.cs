using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentEntities;

namespace DocumentProcessor.Strategies
{
    public class MockDataStrategy : StrategyBase
    {
        public MockDataStrategy()
            : base()
        {
        }

        //public override DocumentList Get()
        //{
        //    return new DocumentList() { Documents = new List<Document>() 
        //                            { 
        //                                  new Document() { Id = "jpm", Title = "JPM Quarterly Review", 
        //                                                    Path = "/InvestmentManagement/Step4PortfolioStrategistSelection/JPM/",
        //                                                    CanDistribute = false, ClientApproved = true, 
        //                                                    Category1 = "InvestmentManagement" , 
        //                                                    Category2 = "Step4PortfolioStrategistSelection", 
        //                                                    Category3 = "JPM",
        //                                                    URL = "http://www.ewealthmanager.com/en/~media/hbaldgbasjdnqy898.ashx",
        //                                                    Files = new List<RCFile>() 
        //                                                        {  
        //                                                            new RCFile() { Size = "123456", Checksum= "14fde92e3629116bef9cb462e2301ba0ec90c3b9"},
        //                                                            new RCFile() { Size = "234567", Checksum= "14fde92e3629116bef9cb462e2301ba0ec90c3b9"} 
        //                                                        }
        //                                                    },
        //                                  new Document() { Id = "callan", Title = "Callan Quarterly Review", 
        //                                                    Path = "/InvestmentsReviews" }
        //                            }
        //    };
        //}
    }
}
