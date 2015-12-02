using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerLogic.SitecoreExt.Pipelines.PublishItem.Implementations
{
	class ClearCacheItemProcessor : URLPublishItemProcessor
	{
		public string sURL;

		public override string URL
		{
			get
			{
				//do we need to load the URL from the config?
				if (string.IsNullOrEmpty(sURL))
				{
					//get the URL
					sURL = Sitecore.Configuration.Settings.GetSetting("pipelines:publish:processors:clearcachitem:url", string.Empty);
				}

				//return the url
				return sURL;
			}
		}
	}
}
