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
using Sitecore.Globalization;

namespace ServerLogic.SitecoreExt
{
	public static class ContextExtension
	{
		private static IContextProvider oContextProvider;

		public static IContextProvider ContextProvider
		{
			get
			{
				return oContextProvider ?? (oContextProvider = LoadConfiguration<IContextProvider>("ServerLogic.SitecoreExtension.ContextProvider"));
			}
		}

		public static Database CurrentDatabase
		{
			get
			{
				return ContextProvider.CurrentDatabase;
			}
		}

		public static Item CurrentItem
		{
			get
			{
				return ContextProvider.CurrentItem;
			}
		}

		public static Item CurrentParentItem
		{
			get
			{
				return ContextProvider.CurrentParentItem;
			}
		}

		public static string CurrentLanguageCode
		{
			get
			{
				return ContextProvider.CurrentLanguageCode;
			}
		}

		public static List<Item> CurrentParentItems
		{
			get
			{
				return ContextProvider.CurrentParentItems;
			}
		}

		public static ConfigurationProfile LoadConfiguration<ConfigurationProfile>(string sSetting)
		{
			ConfigurationProfile oConfigurationProfile;
			object oClassConfiguration;
			string sClassConfiguration;

			//get the configuration variable, throwing exception if not found
			if (string.IsNullOrEmpty(sClassConfiguration = Sitecore.Configuration.Settings.GetSetting(sSetting)))
			{
				//throw the exception
				throw new Exception("Configuration setting [" + sSetting + "] was not found.");
			}

			//create instance of object specified, check that configuration is of type proper type
			if ((oClassConfiguration = Activator.CreateInstance(Type.GetType(sClassConfiguration))) == null)
			{
				//throw the exception
				throw new Exception("Configuration setting [" + sSetting + "] refers to a null class.");
			}
			else if (!(oClassConfiguration is ConfigurationProfile))
			{
				//throw the exception
				throw new Exception("Configuration setting [" + sSetting + "] refers to a class of type [" + oClassConfiguration.GetType().ToString() + "] but should refer to a class of type [" + typeof(ConfigurationProfile).ToString() + "].");
			}
			else
			{
				//set the local setting
				oConfigurationProfile = (ConfigurationProfile)oClassConfiguration;
			}

			//return the class
			return oConfigurationProfile;
		}
	}
}
