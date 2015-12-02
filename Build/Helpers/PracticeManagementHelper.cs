using Genworth.SitecoreExt.Security;
using Genworth.SitecoreExt.Utilities.GridComponent;
using GFWM.Shared.ServiceRequest;
using GFWM.Shared.ServiceRequestFactory;
using ServerLogic.SitecoreExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Helpers
{
   
    public static class PracticeManagementHelper
    {
        private static readonly string InvestmentLevelImagesPath = "/sitecore/content/Shared Content/Practice-Management/Investment-Level-Images";
        
        public static Dictionary<string, GridTable> GetInvestmentLevelData()
        {
            Dictionary<string, GridTable> tables = new Dictionary<string, GridTable>();
            IServiceRequest oAUMService;
			try
			{
                var currentAuthorization = Authorization.CurrentAuthorization;
                oAUMService = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

                GFWM.Common.PracticeManagement.Entities.Request.PremierConsultantInvestmentLevelRequest request = new GFWM.Common.PracticeManagement.Entities.Request.PremierConsultantInvestmentLevelRequest();

                var response = oAUMService.Request<GFWM.Common.PracticeManagement.Entities.Request.PremierConsultantInvestmentLevelRequest, GFWM.Common.PracticeManagement.Entities.Response.PremierConsultantInvestmentLevelResponse>(request);
                      
                if (response != null) 
                        {
                            if(response.FirmBdas != null && response.FirmBdas.Count() > 0)
                            {
                                tables.Add(HtmlTableHelper.ILSFirmCODE, GetFirmTable(response.FirmBdas));
                            }

                            if (response.AdvisorBdas != null && response.AdvisorBdas.Count() > 0)
                            {
                                tables.Add(HtmlTableHelper.ILSAdvisorCODE, GetAdvisorTable(response.AdvisorBdas));
                            }
                        }

                return tables;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error executing GetInvestmentLevelData request", ex, typeof(PracticeManagementHelper));
            }
            return null;
        }

        private static GridTable GetFirmTable(IEnumerable<GFWM.Common.PracticeManagement.Entities.Data.FirmBda> firmBdas)
        {
            GridTable firmTable = new GridTable();
            firmTable.ID = HtmlTableHelper.ILSFirmCODE;

            firmTable.Header = new List<GridCell>(){
                                     new GridCell(){
                                           Value = "Firm/Office Name"
                                     },
                                     new GridCell(){
                                           Value = "Available BDA Balance"
                                     },
                                     new GridCell(){
                                           Value = "Maximum BDA Balance"
                                     },
                                     new GridCell(){
                                           Value = "Available as % of Max"
                                     }
                                 };
            firmTable.Rows = new List<GridRow>();
            foreach(var f in firmBdas)
            {
                GridRow row = new GridRow()
                {
                    Cells = new List<GridCell>(){
                                     new GridCell(){
                                           Value = f.FirmName
                                     },
                                     new GridCell(){
                                           Value = f.AvailableBalance.HasValue ? f.AvailableBalance.Value.ToString("C0") : "$0"
                                     },
                                     new GridCell(){
                                           Value = f.MaximumBalance.HasValue ? f.MaximumBalance.Value.ToString("C0") : "$0"
                                     },
                                     new GridCell(){
                                           Value = f.PercentageOfMax.HasValue ? f.PercentageOfMax.Value.ToString("N0") + " %"  : "0 %"
                                 }
                            }
                };
                firmTable.Rows.Add(row);
            }
            return firmTable;
        }

        private static GridTable GetAdvisorTable(IEnumerable<GFWM.Common.PracticeManagement.Entities.Data.AdvisorBda> advisorBdas)
        {
            GridTable advisorTable = new GridTable();
            DateTime? dt = advisorBdas.Where(a => a.EligibleAumDate.HasValue).Select(s=>s.EligibleAumDate).FirstOrDefault();
            advisorTable.ID = HtmlTableHelper.ILSAdvisorCODE;
            advisorTable.Header = new List<GridCell>(){
                                     new GridCell(){
                                           Value = "Advisor Name"
                                     },
                                     new GridCell(){
                                           Value = "Net Contributions for PC Status Level"
                                     },
                                     new GridCell(){
                                           Value = dt.HasValue ? string.Format("BDA Eligible AUM (as of {0})", dt.Value.ToString("MM/dd/yy")) : "BDA Eligible AUM"
                                     },
                                     new GridCell(){
                                           Value = "Available BDA Balance"
                                     },
                                      new GridCell(){
                                           Value = "Maximum BDA Balance"
                                     },
                                     new GridCell(){
                                           Value = "Available as % of Max"
                                     }
                                 };

            advisorTable.Rows = new List<GridRow>();
            foreach (var a in advisorBdas)
            {
                GridRow row = new GridRow()
                {
                    Cells = new List<GridCell>(){
                                     new GridCell(){
                                           Value = a.AdvisorName
                                     },
                                     new GridCell(){
                                           Value = a.NetContributions.HasValue ? a.NetContributions.Value.ToString("C0") : "$0"
                                     },
                                     new GridCell(){
                                           Value = a.EligibleAum.HasValue ? a.EligibleAum.Value.ToString("C0") : "$0"
                                     },
                                     new GridCell(){
                                           Value = a.AvailableBalance.HasValue ? a.AvailableBalance.Value.ToString("C0") : "$0"
                                     },
                                     new GridCell(){
                                           Value = a.MaximumBalance.HasValue ? a.MaximumBalance.Value.ToString("C0") : "$0"
                                     },
                                     new GridCell(){
                                           Value = a.PercentageOfMax.HasValue ? a.PercentageOfMax.Value.ToString("N0") + " %" : "0 %"
                                 }
                            },
                            Attributes = new Dictionary<string,string>(){
                                                       {"data-imageurl", GetInvestmentLevelImageUrl(a.PCStatus)}
                                                }

                };
                advisorTable.Rows.Add(row);
            }

            return advisorTable;
        }


        public static GridTable  GetBDAData()
        {
            GridTable table = null;
            IServiceRequest oAUMService;
            try
            {
                var currentAuthorization = Authorization.CurrentAuthorization;
                oAUMService = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);
                if(currentAuthorization.IsOsj)
                {
                    if (currentAuthorization.OsjIds != null && currentAuthorization.OsjIds.Count() > 0)
                    {
                        GFWM.Common.PracticeManagement.Entities.Request.PremierBusinessBuilderRequest request = new GFWM.Common.PracticeManagement.Entities.Request.PremierBusinessBuilderRequest();

                        var response = oAUMService.Request<GFWM.Common.PracticeManagement.Entities.Request.PremierBusinessBuilderRequest, GFWM.Common.PracticeManagement.Entities.Response.PremierBusinessBuilderResponse>(request);
                        if (response != null && response.OsjBda != null)
                        {
                            table = new GridTable();

                            DateTime? dt = response.OsjBda.EligibleAumDate;
                       
                            table.Header = new List<GridCell>(){
                             new GridCell(){
                                   Value = "ID"
                             },
                             new GridCell(){
                                   Value = "Name"
                             },
                             new GridCell(){
                                   Value = dt.HasValue ? string.Format("BDA Eligible AUM (as of {0})", dt.Value.ToString("MM/dd/yy")) : "BDA Eligible AUM"
                             },
                             new GridCell(){
                                   Value = "Available BDA Balance"
                             },
                              new GridCell(){
                                   Value = "Maximum BDA Balance"
                             },
                             new GridCell(){
                                   Value = "Available BDA % of Max"
                             },
                            };

                            var cells = new List<GridCell>(){
                                 new GridCell(){
                                       Value = response.OsjBda.OsjId                     
                                 },
                                 new GridCell(){
                                       Value = response.OsjBda.FirmName
                                 },
                                 new GridCell(){
                                       Value = response.OsjBda.EligibleAum.HasValue ? response.OsjBda.EligibleAum.Value.ToString("C0") : "$0"
                                 },
                                 new GridCell(){
                                       Value = response.OsjBda.AvailableBalance.HasValue ? response.OsjBda.AvailableBalance.Value.ToString("C0") : "$0"
                                 },
                                  new GridCell(){
                                       Value = response.OsjBda.MaximumBalance.HasValue ? response.OsjBda.MaximumBalance.Value.ToString("C0") : "$0"
                                 },
                                  new GridCell(){
                                       Value = response.OsjBda.PercentageOfMax.HasValue ? response.OsjBda.PercentageOfMax.Value.ToString("N0") + " %" : "0 %"
                                 },
                                
                          };
                            var rows = new List<GridRow>(){
                             new GridRow(){
                                   Cells = cells,
                                    }
                             };
                            table.Rows = rows;
                                }
                    }
                }

                return table;
            }
            catch(Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error executing GetBDAData request", ex, typeof(PracticeManagementHelper));
            }

            return null;
        }



        private static string GetInvestmentLevelImageUrl(string pcStatus)
        {
            var investmentLevelImagesFolder = ContextExtension.CurrentDatabase.GetItem(InvestmentLevelImagesPath);

            if (investmentLevelImagesFolder != null)
            {
                var imageItem = investmentLevelImagesFolder.Children.Where(c => c.GetText("PC Status") == pcStatus).FirstOrDefault();
                if (imageItem != null)
                {
                    string imgUrl = imageItem.GetImageURL("Image");
                    if(!string.IsNullOrEmpty(imgUrl))
                    {
                        return Sitecore.StringUtil.EnsurePrefix('/', imgUrl);
                    }
                }
            }

            return null;
        }
    }
}
