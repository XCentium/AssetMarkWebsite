using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Diagnostics;

namespace Genworth.SitecoreExt.Helpers
{
	public class PostPublishCacheClearer
	{
		public void ClearCache(object sender, EventArgs e)
		{
			Log.Info("PostPublishCacheClearer clearing all caches.", this);
			Sitecore.Caching.CacheManager.ClearAllCaches();
			Log.Info("PostPublishCacheClearer done.", this);
		}
	}
}
