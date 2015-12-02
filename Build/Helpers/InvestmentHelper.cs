using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genworth.SitecoreExt.Services.Investments;
using Sitecore.Data.Items;
using Genworth.SitecoreExt.Security;
using ServerLogic.SitecoreExt;
using GFWM.Shared.ServiceRequestFactory;
using GFWM.Common.AUM.Entities;
using GFWM.Common.Resources.Entities.Response;
using GFWM.Common.Resources.Entities.Request;
using GFWM.Shared.ServiceRequest;
using GFWM.Common.Resources.Entities.Data;
using System.Text.RegularExpressions;

namespace Genworth.SitecoreExt.Helpers
{
 
	public class InvestmentHelper
	{
        private const string isTaxManagedRegExp = @"(IsTaxmanaged=)(.)*(&IsGlobal)";
        private const string isGlobalRegExp = @"(IsGlobal=)(.)*(&IsMFC)";
        private const string isMFCRegExp = @"(IsMFC=)(.)*(&IsPrimary)";
        private const string isHedgedRegExp = @"(IsHedged=)(.)*";
        private const string omniParamValueRegExp = @"[0-9]+";     

		public static IInvestmentsSearch GetProvider(string sProviderName)
		{
			IInvestmentsSearch oProvider;
			switch (sProviderName.ToLower())
			{
				case ModelPortfolio.CODE:
					oProvider = ModelPortfolio.Current;
					break;
				case ResearchClientView.CODE:
					oProvider = new ResearchClientView();
					break;
				default:
					oProvider = new Research();//Research.Current;
					break;

			}
			return oProvider;
		}		

		#region MODEL PORTFOLIO

		/// <summary>
		/// Returns the automatically generated documents for the current user
		/// </summary>
		/// <param name="oCustodians"></param>
		/// <param name="oPrograms"></param>
		/// <returns></returns>
		public static List<ModelPortfolioResult> GetModelPortfolioDocuments(string[] oCustodians, string[] oPrograms)
		{
			ModelSetsRequest oModelSetsRequest;
			ModelSetsResponse oModelSetsResponse;
			Authorization oAuthorization;
			List<ModelPortfolioResult> oModelPortfolioResult;
			IServiceRequest oAUMService;
			try
			{

				oAUMService = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);
				oModelPortfolioResult = new List<ModelPortfolioResult>();
				oAuthorization = Authorization.CurrentAuthorization;
				Sitecore.Diagnostics.Log.Info("Model Portfolio Start External call", "");
				Sitecore.Diagnostics.Log.Info(string.Format("Model Portfolio oAuthorization != null [{0}] !oAuthorization.IsTestMode [{1}] oAuthorization.Claim != null [{2}] oAuthorization.Claim.Roles != null [{3}] oAuthorization.Claim.Roles.Count() > 0 [{4}]", oAuthorization != null, !oAuthorization.IsTestMode, oAuthorization.Claim != null, oAuthorization.Claim.Roles != null, oAuthorization.Claim.Roles.Count() > 0), "");
				//We can only obtain the documents if we are working on integrated mode
				if (oAuthorization != null && !oAuthorization.IsTestMode && oAuthorization.Claim != null && oAuthorization.Claim.Roles != null && oAuthorization.Claim.Roles.Count() > 0)
				{
					//We assume that we should get the documents for all the roles for the current user
					Sitecore.Diagnostics.Log.Info(string.Format("Model Portfolio External call NO SIEBELID,CustodianID: {0}, ProgramId: {1}, SiebelID: {2}", String.Concat(oCustodians), String.Concat(oPrograms), "String.Empty"), "");
					oModelSetsRequest = new GFWM.Common.Resources.Entities.Request.ModelSetsRequest()
					{
						CustodianId = oCustodians,
						ProgramId = oPrograms
					};

					oModelSetsResponse = oAUMService.Request<GFWM.Common.Resources.Entities.Request.ModelSetsRequest, GFWM.Common.Resources.Entities.Response.ModelSetsResponse>(oModelSetsRequest);

					if (oModelSetsResponse != null && oModelSetsResponse.ModelSets != null)
					{
						foreach (GFWM.Common.Resources.Entities.Data.ModelSetDocument documentData in oModelSetsResponse.ModelSets)
						{
                           oModelPortfolioResult.Add(new ModelPortfolioResult(
                                                                                documentData.DocumentTitle,
                                                                                documentData.DownloadLink,
                                                                                documentData.StrategistName,
                                                                                documentData.LastReallocationDT.HasValue ? documentData.LastReallocationDT.Value.ToString("MM/dd/yyyy") : DateTime.Today.ToString("MM/dd/yyyy"),
                                                                                documentData.CustodianId,
                                                                                documentData.ProgramId,
                                                                                GetModelPortfolioOmnitureParameter(documentData)
                                                                               )
                            );
						}
						Sitecore.Diagnostics.Log.Info(string.Format("Model Portfolio External call result: {0}", oModelSetsResponse.ModelSets.Count()), "");
					}
					else
						Sitecore.Diagnostics.Log.Info("Model Portfolio External call result: NULL", "");
				}
				Sitecore.Diagnostics.Log.Info("Model Portfolio Finish External call", "");
			}
			catch (Exception ex)
			{
				Sitecore.Diagnostics.Log.Error("Model Portfolio External call fail", ex, ex);
				return new List<ModelPortfolioResult>();
			}

			return oModelPortfolioResult;

		}

        private static string GetModelPortfolioOmnitureParameter(ModelSetDocument doc)
        {
            StringBuilder result = new StringBuilder("CMS::ModelPortfolio::");
            Regex isGlobalRegex = new Regex(isGlobalRegExp);
            Regex isTaxManagedRegex = new Regex(isTaxManagedRegExp);
            Regex omniParamValueRegex = new Regex(omniParamValueRegExp);
            Regex isMFCRegex = new Regex(isMFCRegExp);
            Regex isHedgedRegex = new Regex(isHedgedRegExp);

            result.Append(doc.StrategistId.Trim());
            result.Append("_");
            result.Append(doc.ProgramId.Trim());
            result.Append("_");
            string productCategory = doc.ProductCategoryCD.HasValue ? doc.ProductCategoryCD.Value.ToString().Trim() : string.Empty;
            if (!string.IsNullOrEmpty(productCategory))
            {
                result.Append(productCategory);
            }

            Match isTaxManagedMatch1 =isTaxManagedRegex.Match(doc.DownloadLink);

            if (isTaxManagedMatch1.Success && !string.IsNullOrEmpty(isTaxManagedMatch1.Value))
            {
                string original = isTaxManagedMatch1.Value.Replace("|",string.Empty);
                Match match = omniParamValueRegex.Match(original);

                if (match.Success && !string.IsNullOrEmpty(match.Value))
                {
                    if (HasDocumentFlag(match.Value))
                    {
                        result.Append("_TaxManaged");
                    }
                }
            }

            Match isGlobalMatch1 = isGlobalRegex.Match(doc.DownloadLink);

            if (isGlobalMatch1.Success && !string.IsNullOrEmpty(isGlobalMatch1.Value))
            {
                string original2 = isGlobalMatch1.Value.Replace("|", string.Empty);
                Match isGlobalMatch2 = omniParamValueRegex.Match(original2);

                if (isGlobalMatch2.Success && !string.IsNullOrEmpty(isGlobalMatch2.Value))
                {
                    if (HasDocumentFlag(isGlobalMatch2.Value))
                    {
                        result.Append("_Global");
                    }
                }
            }

            Match isMFCMatch1 = isMFCRegex.Match(doc.DownloadLink);

            if (isMFCMatch1.Success && !string.IsNullOrEmpty(isMFCMatch1.Value))
            {
                string original3 = isMFCMatch1.Value.Replace("|", string.Empty);
                Match isMFCMatch2 = omniParamValueRegex.Match(original3);

                if (isMFCMatch2.Success && !string.IsNullOrEmpty(isMFCMatch2.Value))
                {
                    if (HasDocumentFlag(isMFCMatch2.Value))
                    {
                        result.Append("_MFC");
                    }
                }
            }

            Match isHedgedMatch1 = isHedgedRegex.Match(doc.DownloadLink);

            if (isHedgedMatch1.Success && !string.IsNullOrEmpty(isHedgedMatch1.Value))
            {
                string original4 = isHedgedMatch1.Value.Replace("|", string.Empty);
                Match isHedgedMatch2 = omniParamValueRegex.Match(original4);

                if (isHedgedMatch2.Success && !string.IsNullOrEmpty(isHedgedMatch2.Value))
                {
                    if (HasDocumentFlag(isHedgedMatch2.Value))
                    {
                        result.Append("_Hedged");
                    }
                }
            }

            return result.ToString();
        }

        private static bool HasDocumentFlag(string sFlag)
        {
            int intValue;
            bool response = false;
            if (int.TryParse(sFlag, out intValue))
            {
                response = intValue != 0;
            }
            return response;
        }

		#endregion

        public class Strategist
        {
            public Item Item { get; private set; }
            private bool bIsStrategy;
            private HashSet<string> oSolutionsTypeCodes;
            private List<Solution> oSolutions;
            public List<Solution> Solutions { get { return oSolutions; } }
            private Lookup<string, Item> oStrategies;

            public Strategist(Item oStrategist)
            {
                Item = oStrategist;
                oSolutions = new List<Solution>();
                oSolutionsTypeCodes = new HashSet<string>();
                oStrategies = (Lookup<string, Item>)oStrategist.GetChildrenOfTemplate("Strategy").ToLookup(oStrategy => oStrategy.GetField("Allocation Approach").GetItem().GetText("Code"));
                bIsStrategy = !oStrategist.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.StrategistNoAllocation);
            }

            public void AddSolution(string sAllocationApproach, string sSolutionType)
            {
                List<Item> oParents = new List<Item>();
                Item oSolution;
                if (bIsStrategy)
                {
                    if (oStrategies.Contains(sAllocationApproach))
                    {
                        var strategies = oStrategies[sAllocationApproach];
                        oParents = strategies.ToList();
                    }
                }
                else
                {
                    if (oSolutionsTypeCodes.Contains(sSolutionType))
                    {
                        return;
                    }

                    oParents.Add(Item);
                }

                int addedSolutions = 0;
                if (oParents != null && oParents.Count > 0)
                {
                    foreach (var parent in oParents)
                    {
                        oSolution = parent.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution).Where(oItem => oItem.GetMultilistItems("Type").Any(oType => oType.GetText("Code") == sSolutionType)).FirstOrDefault();
                        if (oSolution != null)
                        {
                            addedSolutions++;
                            oSolutions.Add(new Solution(oSolution, parent, false));
                            if (!oSolutionsTypeCodes.Contains(sSolutionType))
                            {
                                oSolutionsTypeCodes.Add(sSolutionType);
                            }
                        }
                    }
                }

                if (addedSolutions <= 0)
                {
                    Sitecore.Diagnostics.Log.Info(string.Format("Client View allocationApproach not found allocationApproach[{0}],Solutiontype[{1}]", sAllocationApproach, sSolutionType), this);
                }
            }

        }

		public class Solution
		{
			public Item Item { get; private set; }
			private Item oParent;
			public bool UserOwn { get; private set; }
			public string SolutionName { get; private set; }
			public Solution(Item oSolution, Item oParent,bool bAppendAllocationApproach=true)
			{
				Item = oSolution;
				this.oParent = oParent;
                if (bAppendAllocationApproach)
                {
                    if (string.IsNullOrEmpty(SolutionName = oSolution.GetText(Genworth.SitecoreExt.Constants.Investments.Templates.SolutionTypeFields.Mandate)))
                        SolutionName = string.Format("{0} - {1}", oSolution.DisplayName, oParent.GetField("Allocation Approach").GetItem().GetText("Title"));
                }
                else
                    SolutionName = oSolution.DisplayName;

			}

			public Item AllocationApproach
			{
				get 
				{
					// find the Strategy related to this solution in order to gets its related Allocation Approach
					return (this.oParent.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy)) ? this.oParent.GetListItem("Allocation Approach") : null;	
				}
			}


		}




	}
}
