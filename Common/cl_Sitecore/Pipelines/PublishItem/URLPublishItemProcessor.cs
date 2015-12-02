using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Publishing.Pipelines.PublishItem;
using System.Net;

namespace ServerLogic.SitecoreExt.Pipelines.PublishItem
{
	public abstract class URLPublishItemProcessor : PublishItemProcessor
	{
		public abstract string URL { get; }

		public override void Process(PublishItemContext oPublishItemContext)
		{
			string sItemId;
			string sItemProcessorURL;

			//get the item id for the item being published
			sItemId = oPublishItemContext.ItemId.ToString();

			//get the item processor url
			sItemProcessorURL = string.Format(URL, sItemId);

			//log that we are removing the item from the cache
			Sitecore.Diagnostics.Log.Debug(string.Format("URL Publish Item Processor is processing item {0} with URL {1}.", sItemId, sItemProcessorURL));

			//call the url
			new WebClient().DownloadString(sItemProcessorURL);
		}
	}
}
