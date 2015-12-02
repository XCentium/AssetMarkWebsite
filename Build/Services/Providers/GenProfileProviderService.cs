using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using System.Web.Profile;
using System.Xml.Linq;
using System.ServiceModel.Activation;

namespace Genworth.SitecoreExt.Services.Providers
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class GenProfileProviderService : IGenProfileProviderService
	{

		#region VARIABLES

		#region CONSTANTS

		/// <summary>
		/// Web.config setting that specifies the role provider that should be used to retrieve the roles information
		/// </summary>
		private static string sDefaultProviderSettingName = "DefaultProviderGenProfileProviderService";

		/// <summary>
		/// Default Role Provider to be used in case that no role provider is defined in Web.config settings
		/// </summary>
		private static string sDefaultProviderName = "sql";


		#endregion


		#endregion

		#region PROPERTIES

		/// <summary>
		/// Default provider that will be used to access profiles information. If not conofigured in web.config it will return "sql" provider as default
		/// </summary>
		private static string DefaultProviderName
		{
			get
			{
				return Sitecore.Configuration.Settings.GetSetting(sDefaultProviderSettingName, sDefaultProviderName);
			}
		}


		#endregion

		#region METHODS

		#region SERVICE OPERATIONS

		public System.Web.Profile.ProfileInfoCollection FindInactiveProfilesByUserName(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, string sUsernameToMatch, DateTime dUserInactiveSinceDate)
		{
			#region VARIBLES

			System.Web.Profile.ProfileProvider oSitecoreSqlProfileProvider;
			ProfileInfoCollection oInactiveProfiles;

			#endregion

			//Sitecore.Diagnostics.Log.Info("GenProfileProviderService.FindInactiveProfilesByUserName", this);


			oSitecoreSqlProfileProvider = ProfileManager.Providers[DefaultProviderName];
			oInactiveProfiles = null;
			iTotalRecords = 0;
			if (oSitecoreSqlProfileProvider != null)
			{
				oInactiveProfiles = oSitecoreSqlProfileProvider.FindInactiveProfilesByUserName(oAuthenticationOption, sUsernameToMatch, dUserInactiveSinceDate, iPageIndex, iPageSize, out iTotalRecords);
				if (oInactiveProfiles != null)
				{
					Sitecore.Diagnostics.Log.Info(string.Format("GenProfileProviderService.FindInactiveProfilesByUserName, InactiveProfiles:{0}", oInactiveProfiles.ToString()), this);
				}
			}
			else
			{
				Sitecore.Diagnostics.Log.Warn("GenProfileProviderService.FindInactiveProfilesByUserName, Unable to find SqlProfileProvider", this);
			}

			return oInactiveProfiles;
		}

		public System.Web.Profile.ProfileInfoCollection FindProfilesByUserName(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, string sUsernameToMatch)
		{
			#region VARIBLES

			System.Web.Profile.ProfileProvider oSitecoreSqlProfileProvider;
			ProfileInfoCollection oUserProfiles;

			#endregion

			Sitecore.Diagnostics.Log.Info("GenProfileProviderService.FindProfilesByUserName", this);


			oSitecoreSqlProfileProvider = ProfileManager.Providers[DefaultProviderName];
			oUserProfiles = null;
			iTotalRecords = 0;
			if (oSitecoreSqlProfileProvider != null)
			{
				oUserProfiles = oSitecoreSqlProfileProvider.FindProfilesByUserName(oAuthenticationOption, sUsernameToMatch, iPageIndex, iPageSize, out iTotalRecords);
				if (oUserProfiles != null)
				{
					Sitecore.Diagnostics.Log.Info(string.Format("GenProfileProviderService.FindProfilesByUserName, UserProfiles:{0}", oUserProfiles.Count), this);
				}
			}
			else
			{
				Sitecore.Diagnostics.Log.Warn("GenProfileProviderService.FindProfilesByUserName, Unable to find SqlProfileProvider", this);
			}

			return oUserProfiles;
		}

		public System.Web.Profile.ProfileInfoCollection GetAllInactiveProfiles(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, DateTime dUserInactiveSinceDate)
		{
			#region VARIBLES

			System.Web.Profile.ProfileProvider oSitecoreSqlProfileProvider;
			ProfileInfoCollection oInactiveProfiles;

			#endregion

			Sitecore.Diagnostics.Log.Info("GenProfileProviderService.GetAllInactiveProfiles", this);


			oSitecoreSqlProfileProvider = ProfileManager.Providers[DefaultProviderName];
			oInactiveProfiles = null;
			iTotalRecords = 0;
			if (oSitecoreSqlProfileProvider != null)
			{
				oInactiveProfiles = oSitecoreSqlProfileProvider.GetAllInactiveProfiles(oAuthenticationOption, dUserInactiveSinceDate, iPageIndex, iPageSize, out iTotalRecords);
				if (oInactiveProfiles != null)
				{
					Sitecore.Diagnostics.Log.Info(string.Format("GenProfileProviderService.GetAllInactiveProfiles, InactiveProfiles:{0}", oInactiveProfiles.Count), this);
				}
			}
			else
			{
				Sitecore.Diagnostics.Log.Warn("GenProfileProviderService.GetAllInactiveProfiles, Unable to find SqlProfileProvider", this);
			}

			return oInactiveProfiles;
		}

		public System.Web.Profile.ProfileInfoCollection GetAllProfiles(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption)
		{
			#region VARIBLES

			System.Web.Profile.ProfileProvider oSitecoreSqlProfileProvider;
			ProfileInfoCollection oProfiles;

			#endregion

			Sitecore.Diagnostics.Log.Info("GenProfileProviderService.GetAllProfiles", this);

			oSitecoreSqlProfileProvider = ProfileManager.Providers[DefaultProviderName];
			oProfiles = null;
			iTotalRecords = 0;
			if (oSitecoreSqlProfileProvider != null)
			{
				oProfiles = oSitecoreSqlProfileProvider.GetAllProfiles(oAuthenticationOption, iPageIndex, iPageSize, out iTotalRecords);
				if (oProfiles != null)
				{
					Sitecore.Diagnostics.Log.Info(string.Format("GenProfileProviderService.GetAllProfiles, Profiles:{0}", oProfiles.Count), this);
				}
			}
			else
			{
				Sitecore.Diagnostics.Log.Warn("GenProfileProviderService.GetAllProfiles, Unable to find SqlProfileProvider", this);
			}

			return oProfiles;
		}

		public int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, DateTime dUserInactiveSinceDate)
		{
			#region VARIBLES

			System.Web.Profile.ProfileProvider oSitecoreSqlProfileProvider;
			int iNumberOfInactiveProfiles;

			#endregion

			//Sitecore.Diagnostics.Log.Info("GenProfileProviderService.GetNumberOfInactiveProfiles", this);

			oSitecoreSqlProfileProvider = ProfileManager.Providers[DefaultProviderName];
			iNumberOfInactiveProfiles = 0;
			
			if (oSitecoreSqlProfileProvider != null)
			{
				iNumberOfInactiveProfiles = oSitecoreSqlProfileProvider.GetNumberOfInactiveProfiles(oAuthenticationOption, dUserInactiveSinceDate);				
				Sitecore.Diagnostics.Log.Info(string.Format("GenProfileProviderService.GetNumberOfInactiveProfiles, Profiles:{0}", iNumberOfInactiveProfiles), this);
				
			}
			else
			{
				Sitecore.Diagnostics.Log.Warn("GenProfileProviderService.GetNumberOfInactiveProfiles, Unable to find SqlProfileProvider", this);
			}

			return iNumberOfInactiveProfiles;
		}

		public List<ProfilePropertyValueContract> GetPropertyValues(System.Configuration.SettingsContext oContext, List<ProfilePropertyContract> oCollection)
		{
			#region VARIBLES

			System.Web.Profile.ProfileProvider oSitecoreSqlProfileProvider;
			System.Configuration.SettingsPropertyValueCollection oPropertyValues;
			System.Configuration.SettingsPropertyCollection oCollectionAsSettingPropertyCollection;
			List<ProfilePropertyValueContract> oPropertyValuesSerialized;


			#endregion

			//Sitecore.Diagnostics.Log.Info("GenProfileProviderService.GetPropertyValues", this);
			oPropertyValues = null;
			oSitecoreSqlProfileProvider = ProfileManager.Providers[DefaultProviderName];
			oPropertyValuesSerialized = null;

			if (oSitecoreSqlProfileProvider != null)
			{				
				oCollectionAsSettingPropertyCollection = Genworth.SitecoreExt.Utilities.ProfileServiceSerialization.DeserializeSettingsPropertyCollection(oCollection);

				oPropertyValues = oSitecoreSqlProfileProvider.GetPropertyValues(oContext, oCollectionAsSettingPropertyCollection);

				if (oPropertyValues != null)
				{
					Sitecore.Diagnostics.Log.Info(string.Format("GenProfileProviderService.GetPropertyValues, Properties Count:{0}", oPropertyValues.Count), this);

					oPropertyValuesSerialized = Genworth.SitecoreExt.Utilities.ProfileServiceSerialization.SerializeSettingsPropertyValueCollection(oPropertyValues);

					Sitecore.Diagnostics.Log.Info(string.Format("GenProfileProviderService.GetPropertyValues, Properties Returned Count:{0}", oPropertyValuesSerialized.Count), this);
				}

			}
			else
			{
				Sitecore.Diagnostics.Log.Warn("GenProfileProviderService.GetPropertyValues, Unable to find SqlProfileProvider", this);
			}

			return oPropertyValuesSerialized;
		}

		#endregion


		#endregion
	}
}
