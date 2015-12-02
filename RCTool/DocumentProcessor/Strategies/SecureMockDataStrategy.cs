using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentEntities;

namespace DocumentProcessor.Strategies
{
    public class SecureMockDataStrategy : DataStrategyBase
    {
        public SecureMockDataStrategy()
            : base()
        {
        }

        public override RcToolsData Get()
        {
            return new ContentData()
            {
                Category = new Category()
                {
                    SubCategories = new List<Category>()
                    {
                        new Category()
                        {
                            Id = "ad614b46-1cf0-44c1-a37c-3a38aaea8aea",
                            Title = "Genworth Materials",
                            Description = "Some category GNW description here",
                            //Image = new Image()
                            //{
                            //    File = new RCFile()
                            //    {
                            //        //Name = "75d21aae-3aa0-4a48-a51e-1071123img.gif",
                            //        Size = "1207"
                            //    }
                            //},
                            SubCategories = new List<Category>()
                            {
                                new Category()
                                {
                                    Id = "Events",
                                    Title = "East Coast",
                                    Description = "Some category description here",
                                    //Image = new Image()
                                    //{
                                    //    File = new RCFile()
                                    //    {
                                    //        //Name = "75d21aae-3aa0-4a48-a51e-1071791img.gif",
                                    //        Size = "1207"
                                    //    }
                                    //},
                                    Assets = new List<Asset>()
                                    {
                                        new Asset()
                                        {
                                            Id = "75d21aae-3aa0-4a48-a51e-107179e38e22",
                                            //Type = AssetType.Document,
                                            Title = "2012 East Coast Event Calendar - External Use",
                                            Description = "Some description here",
                                            ClientApproved = false,
                                            CanDistribute = true,
                                            //IsRotatable = true,
                                            //DefaultOrientation = Orientation.Portrait,
                                            URL = "http://www.yahoo.com/east_coast_events.pdf",
                                            Files = new List<RCFile>()
                                            {
                                                new RCFile()
                                                {
                                                    //Name = "75d21aae-3aa0-4a48-a51e-107179e38e22.pdf",
                                                    Size = "180781",
                                                    Checksum = "KNmc3YqRcCN15LcqlETNBWs4N/g="
                                                }
                                            }
                                        }
                                    }                                    
                                },
                                new Category()
                                {
                                    Id = "practiceMgmt",
                                    Title = "Practice Management",
                                    SubCategories = new List<Category>()
                                    {
                                        new Category()
                                        {
                                            Id = "75d21aae-3aa0-4a48-a51e-107179e38e79",
                                            Title = "Mastery Program",
                                            Description = "Some category description here",
                                            //Image = new Image()
                                            //{
                                            //    File = new RCFile()
                                            //    {
                                            //        //Name = "75d21aae-3aa0-4a48-a51e-1071791img.gif",
                                            //        Size = "1207"
                                            //    }
                                            //},
                                            Assets = new List<Asset>()
                                            {
                                                new Asset()
                                                {
                                                    Id = "75d21aae-3aa0-4a48-a51e-107179e38e11",
                                                    //Type = AssetType.Document,
                                                    Title = "2012 Event Calendar - External Use",
                                                    Description = "Some description here",
                                                    ClientApproved = false,
                                                    CanDistribute = true,
                                                    //IsRotatable = true,
                                                    //DefaultOrientation = Orientation.Portrait,
                                                    URL = "http://www.yahoo.com/some_document.pdf",
                                                    Files = new List<RCFile>()
                                                    {
                                                        new RCFile()
                                                        {
                                                            //Name = "75d21aae-3aa0-4a48-a51e-107179e38e12.pdf",
                                                            Size = "120781",
                                                            Checksum = "KNmc3YqRcCN15LcqlETNBWs4N/g="
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                View = new View()
                {
                    SubViews = new List<View>()
                    {
                        new View()
                        {
                            Id = "View Id"
                            //Title = "View Title",
                            //Description = "Some view description here",
                            //Links = new List<string>()
                            //{
                            //    "Link1",
                            //    "Link2",
                            //    "Link3"
                            //},
                            //    Categories = new List<string>()
                            //{
                            //    "Category Id 1",
                            //    "Category Id 2",
                            //    "Category Id 3"
                            //}
                        }
                    }
                }
            };
        }
    }
}
