using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Services.Investments
{
	/// <summary>
	/// An item cache simply keeps track of whether we have asked Sitecore for a particular item id, and 
	/// whether it returned an item. This allows us to determine if a user is allowed to access the content 
	/// without having to re-ask Sitecore multiple times for the same lucene document.
	/// </summary>
	public class ItemCache : Dictionary<string, bool> { }
}
