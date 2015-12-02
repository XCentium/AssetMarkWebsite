using Genworth.SitecoreExt.Providers;
using Genworth.SitecoreExt.Utilities.GridComponent;
using System.Collections.Generic;
namespace Genworth.SitecoreExt.Helpers
{
	public class HtmlTableHelper
	{
		public const string ILSFirmCODE = "ILSFirm";
		public const string ILSAdvisorCODE = "ILSAdvisor";
		public const string AumBDACODE = "aum-bda";

		public static GridTable GetProvider(string sProviderName)
		{
			GridTable oProvider;
			switch (sProviderName.ToLower())
			{
				case AumBDACODE:
					oProvider = PracticeManagementHelper.GetBDAData();
					break;
				default:
					oProvider = null;
					break;
			}
			return oProvider;
		}

		public static Dictionary<string, GridTable> GetInvestmentLevelStatusTables()
		{
			var tables = PracticeManagementHelper.GetInvestmentLevelData();
			return tables;
		}

		
	}
}
