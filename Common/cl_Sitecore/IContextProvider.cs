using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System.Globalization;

namespace ServerLogic.SitecoreExt
{
	public interface IContextProvider
	{
		Database CurrentDatabase { get; }

		Item CurrentItem { get; }

		Item CurrentParentItem { get; }

		List<Item> CurrentParentItems { get; }

		CultureInfo CurrentCulture { get; }

		string CurrentLanguageCode { get; }
	}
}
