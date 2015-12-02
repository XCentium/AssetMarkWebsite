using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;

using Sitecore.Data.Items;
using System.ServiceModel.Web;
using System.Web;
using Genworth.SitecoreExt.Helpers;

namespace Genworth.SitecoreExt.Services.Investments
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ResearchService : IResearchService
	{
		public string GetCurrentTime()
		{
			return DateTime.Now.ToString("yyyyMMdd-HHmmss");
		}


		public void SetValue(string sValue)
		{
		}

		public string Value
		{
			get
			{
				return (string)System.Web.HttpContext.Current.Session["Genworth.SitecoreExt.Services.Investments.ResearchService.Value"];
			}
			set
			{
				System.Web.HttpContext.Current.Session["Genworth.SitecoreExt.Services.Investments.ResearchService.Value"] = value;
			}
		}

		public InvestmentsSearchBase Research(string sType)
		{
			
				SetNoChaching();

				return (InvestmentsSearchBase)InvestmentHelper.GetProvider(sType);
			
		}

		public InvestmentsSearchBase SetFilterOption(string sType, string sFilter, string sOption, string sFiltered)
		{
			SetNoChaching();
			InvestmentsSearchBase oResearch;

			//we are using a local variable to skip cycles when we return
			(oResearch = Research(sType)).SetFilterOption(sFilter, sOption, sFiltered);

			//return the research
			return oResearch;
		}

		public ResultBase[] GetResults(string sType)
		{

			SetNoChaching();


			//use the research to get hte documents
			return ((InvestmentsSearchBase)Research(sType)).GetResults();
		}

		public InvestmentsSearchBase SetSort(string sType, string sField)
		{
			SetNoChaching();
			InvestmentsSearchBase oResearch;

			//we are using a local variable to skip cycles when we return
			(oResearch = Research(sType)).Sort(sField);

			//return the research
			return oResearch;
		}

		public InvestmentsSearchBase SetResultsPerPage(string sType, string sResultsPerPage)
		{
			SetNoChaching();
			InvestmentsSearchBase oResearch;
			int iResultsPerPage;

			//parse the results per page
			int.TryParse(sResultsPerPage, out iResultsPerPage);

			//set the results per page
			(oResearch = Research(sType)).ResultsPerPage = iResultsPerPage;

			//return the research
			return oResearch;
		}

		public InvestmentsSearchBase SetPage(string sType, string sPage)
		{
			SetNoChaching();
			InvestmentsSearchBase oResearch;
			int iPage;

			//parse the page
			int.TryParse(sPage, out iPage);

			//set the page
			(oResearch = Research(sType)).Page = iPage;

			//return the research
			return oResearch;
		}


		public InvestmentsSearchBase SetDateRange(string sType, string sFromDate, string sToDate)
		{
			SetNoChaching();
			InvestmentsSearchBase oResearch;
			
			//set the dates
			(oResearch = Research(sType)).Months = -2;
			oResearch.FromDate = sFromDate;
			oResearch.ToDate = sToDate;
			
			//return the research
			return oResearch;
		}


		public InvestmentsSearchBase SetDateLookback(string sType, string sMonths)
		{
			SetNoChaching();
			int iMonths;
			InvestmentsSearchBase oResearch;

			//try to parse the months
			if (!int.TryParse(sMonths, out iMonths))
			{
				//we could not parse, set to default range
				iMonths = Constants.Investments.DefaultMonthRange;
			}

			//set the months
			(oResearch = Research(sType)).Months = iMonths;

			//return the research
			return oResearch;
		}

		public InvestmentsSearchBase SetKeywords(string sType, string sKeywords)
		{
			SetNoChaching();
			InvestmentsSearchBase oResearch;

			//set the keywords
			(oResearch = Research(sType)).SetKeywords(sKeywords);

			//return the research
			return oResearch;
		}


		public InvestmentsSearchBase Reset(string sType)
		{
			SetNoChaching();
			InvestmentsSearchBase oResearch;

			//set the keywords
			(oResearch = Research(sType)).Reset(true);

			//return the research
			return oResearch;
		}
		internal void SetNoChaching()
		{
			HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));

			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			HttpContext.Current.Response.Cache.SetNoStore();
		}
	}
}
