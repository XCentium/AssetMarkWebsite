using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Web.UI.WebControls;

namespace ServerLogic.SitecoreExt
{
	public static class SublayoutExtension
	{
		public static string GetParameter(this Control oControl, string sParameter)
		{
			return oControl.GetParameter(sParameter, string.Empty);
		}

		public static string GetParameter(this Control oControl, string sParameter, string sDefault)
		{
			return (oControl != null && oControl.Parent != null && oControl.Parent is Sublayout) ? ((Sublayout)oControl.Parent).GetParameter(sParameter, sDefault) : sDefault;
		}

		public static string GetParameter(this Sublayout oSublayout, string sParameter)
		{
			return oSublayout.GetParameter(sParameter, string.Empty);
		}

		public static string GetParameter(this Sublayout oSublayout, string sParameter, string sDefault)
		{
			string sValue;

			//initialize the parameter to lower case and add an equal sign for comparison
			sParameter = sParameter.ToLower() + "=";

			//initialize the value to default
			sValue = sDefault;

			//be sure that the sublayout is not null before we start looping over it
			if (oSublayout != null)
			{
				foreach (string sItem in oSublayout.Parameters.Split('&'))
				{
					if (sItem.ToLower().StartsWith(sParameter))
					{
						sValue = HttpUtility.UrlDecode(sItem.Split('=').LastOrDefault() ?? sDefault).Replace("+", " ");
						break;
					}
				}
			}

			return sValue;
		}
	}
}
