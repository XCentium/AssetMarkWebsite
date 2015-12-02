using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Query;
using Sitecore.Configuration;
using Sitecore.Express;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using Sitecore.Sites;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace ServerLogic.SitecoreExt
{
	public static class StringExtension
	{
		private static int SITECORE_TEXT_CACHING_SECONDS;
		private const int SITECORE_TEXT_CACHING_SECONDS_DEFAULT = 120;
		private const int SITECORE_TEXT_CACHING_SECONDS_MINIMUM = 30;

		public static int SitecoreTextCachingSeconds
		{
			get
			{
				int iTemp;

				//if the existing value is less than 30 seconds, we need to get from Sitecore settings.
				if (SITECORE_TEXT_CACHING_SECONDS < SITECORE_TEXT_CACHING_SECONDS_MINIMUM)
				{
					if (int.TryParse(Sitecore.Configuration.Settings.GetSetting("Sitecore.Text.Caching.Seconds", SITECORE_TEXT_CACHING_SECONDS_DEFAULT.ToString()), out iTemp) && iTemp >= SITECORE_TEXT_CACHING_SECONDS_MINIMUM)
					{
						SITECORE_TEXT_CACHING_SECONDS = iTemp;
					}
					else
					{
						SITECORE_TEXT_CACHING_SECONDS = SITECORE_TEXT_CACHING_SECONDS_DEFAULT;
					}
				}

				//return the caching seconds
				return SITECORE_TEXT_CACHING_SECONDS;
			}
		}

		public static string GetSitecoreText(this string sKey)
		{
			//pass key as default text and use default library
			return sKey.GetSitecoreText(string.Empty, sKey);
		}

		public static string GetSitecoreText(this string sKey, string sLibrary)
		{
			//pass key as default text
			return sKey.GetSitecoreText(sLibrary, sKey);
		}

		public static string GetSitecoreText(this string sKey, string sLibrary, string sDefault)
		{
			StringBuilder sPath;
			Item oItem;
			string sCacheKey;
			object oObject;
			string sText;

			sPath = new StringBuilder();
			sPath.Append("fast:/sitecore/content/shared/#text lookup#");
			if (!string.IsNullOrEmpty(sLibrary))
			{
				sPath.Append("/#").Append(sLibrary).Append("#");
			}
			sPath.Append("//#").Append(sKey).Append("#");

			//construct the cache key
			sCacheKey = new StringBuilder(ContextExtension.CurrentLanguageCode).Append("-").Append(sDefault).Append("-").Append(sPath.ToString()).ToString();

			//Sitecore.Diagnostics.Log.Debug(string.Format("Text Lookup Cache Key: {0}", sCacheKey));

			if ((oObject = HttpContext.Current.Cache[sCacheKey]) != null && oObject is string)
			{
				//cast object into string
				sText = (string)oObject;
			}
			else
			{
				//Sitecore.Diagnostics.Log.Debug(string.Format("Text Lookup not found in cache: {0}, looking in Sitecore: {1}", sCacheKey, sPath));

				//get the item for the selected path
				oItem = ContextExtension.CurrentDatabase.SelectItems(sPath.ToString()).FirstOrDefault();

				//if the item is found, return it's text, otherwise return the default text
				sText = oItem != null ? oItem.GetText("", "text", sDefault) : sDefault;

				//store text in cache
				HttpContext.Current.Cache.Add(sCacheKey, sText, null, DateTime.Now.AddSeconds(SitecoreTextCachingSeconds), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
			}

			//return the text
			return sText;
		}

		public static string HtmlWrapify(this string sText)
		{
			//cause text longer than 8 characters to have "wrap points" inserted
			return sText.HtmlWrapify(8);
		}

		public static string HtmlWrapify(this string sText, int iLength)
		{
			return Regex.Replace(sText, "(\\S{" + iLength + "})", "$1&thinsp;", RegexOptions.IgnoreCase);
		}

		public static string SitecoreNamify(this string sText)
		{
			sText = System.Text.RegularExpressions.Regex.Replace(sText, @"[^\w\s\-\$]+", oMatch =>
			{
				switch (oMatch.Value.ToLower().Trim())
				{
					case "@":
						return "-at-";
					default:
						return "-";
				}
			}).Trim();

			//if the text starts with a dash, clean it up
			if ((sText = Regex.Replace(sText, "^[-]+", string.Empty, RegexOptions.IgnoreCase)).Length == 0)
			{
				//set the text
				sText = "unnamed";
			};

			//return the text
			return sText;
		}
	}
}
