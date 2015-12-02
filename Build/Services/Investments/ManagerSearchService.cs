using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;

using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Services.Investments
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class ManagerSearchService : IManagerSearchService
	{
		public ManagerSearch.Strategy GetStrategy(string sStrategyId)
		{
			//get the item
			return new ManagerSearch.Strategy(ContextExtension.CurrentDatabase.GetItem(ItemPointer.Parse(sStrategyId).ItemID));
		}
	}
}
