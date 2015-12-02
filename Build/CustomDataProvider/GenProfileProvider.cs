using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Configuration;
using System.Xml.Linq;
using System.Collections;
using Genworth.SitecoreExt.Services.Providers;
using System.ServiceModel;

namespace Genworth.SitecoreExt.CustomDataProvider
{
    /// <summary>
    /// Provides access to the settings associated to a user
    /// </summary>
    class GenProfileProvider : System.Web.Profile.ProfileProvider
    {
        #region VARIABLES




        #endregion

        #region PROPERTIES


        #endregion

        public override int DeleteInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(string[] usernames)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(System.Web.Profile.ProfileInfoCollection profiles)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Profile.ProfileInfoCollection FindInactiveProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, string sUsernameToMatch, DateTime dUserInactiveSinceDate, int iPageIndex, int iPageSize, out int iTotalRecords)
        {
            System.Web.Profile.ProfileInfoCollection oProfileInfoCollection;

            Sitecore.Diagnostics.Log.Info("GenProfileProvider.FindUsersInRole", this);

            try
            {
                using (var oGenProfileProviderService = new GenProfileProviderServiceProxy())
                {
                    oProfileInfoCollection = oGenProfileProviderService.FindInactiveProfilesByUserName(out iTotalRecords, iPageIndex, iPageSize, oAuthenticationOption, sUsernameToMatch, dUserInactiveSinceDate);
                } 
            }
            catch (Exception oGenProfileProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Profile Provider Error", oGenProfileProviderException, this);
                throw;
            }
            return oProfileInfoCollection;
        }

        public override System.Web.Profile.ProfileInfoCollection FindProfilesByUserName(System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, string sUsernameToMatch, int iPageIndex, int iPageSize, out int iTotalRecords)
        {
            System.Web.Profile.ProfileInfoCollection oProfileInfoCollection;

            Sitecore.Diagnostics.Log.Info("GenProfileProvider.FindProfilesByUserName", this);

            try
            {
                using (var oGenProfileProviderService = new GenProfileProviderServiceProxy())
                {
                    oProfileInfoCollection = oGenProfileProviderService.FindProfilesByUserName(out iTotalRecords, iPageIndex, iPageSize, oAuthenticationOption, sUsernameToMatch);
                }
            }
            catch (Exception oGenProfileProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Profile Provider Error", oGenProfileProviderException, this);
                throw;
            }
            return oProfileInfoCollection;
        }

        public override System.Web.Profile.ProfileInfoCollection GetAllInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, DateTime dUserInactiveSinceDate, int iPageIndex, int iPageSize, out int iTotalRecords)
        {
            System.Web.Profile.ProfileInfoCollection oProfileInfoCollection;

            Sitecore.Diagnostics.Log.Info("GenProfileProvider.GetAllInactiveProfiles", this);

            try
            {
                using (var oGenProfileProviderService = new GenProfileProviderServiceProxy())
                {
                    oProfileInfoCollection = oGenProfileProviderService.GetAllInactiveProfiles(out iTotalRecords, iPageIndex, iPageSize, oAuthenticationOption, dUserInactiveSinceDate);
                }
            }
            catch (Exception oGenProfileProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Profile Provider Error", oGenProfileProviderException, this);
                throw;
            }
            return oProfileInfoCollection;
        }

        public override System.Web.Profile.ProfileInfoCollection GetAllProfiles(System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, int iPageIndex, int iPageSize, out int iTotalRecords)
        {
            System.Web.Profile.ProfileInfoCollection oProfileInfoCollection;

            Sitecore.Diagnostics.Log.Info("GenProfileProvider.GetAllProfiles", this);

            try
            {
                using (var oGenProfileProviderService = new GenProfileProviderServiceProxy())
                {
                    oProfileInfoCollection = oGenProfileProviderService.GetAllProfiles(out iTotalRecords, iPageIndex, iPageSize, oAuthenticationOption);
                }
            }
            catch (Exception oGenProfileProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Profile Provider Error", oGenProfileProviderException, this);
                throw;
            }
            return oProfileInfoCollection;
        }

        public override int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, DateTime dUserInactiveSinceDate)
        {
            int iNumberOfInactiveProfiles;

            Sitecore.Diagnostics.Log.Info("GenProfileProvider.GetNumberOfInactiveProfiles", this);

            try
            {
                using (var oGenProfileProviderService = new GenProfileProviderServiceProxy())
                {
                    iNumberOfInactiveProfiles = oGenProfileProviderService.GetNumberOfInactiveProfiles(oAuthenticationOption, dUserInactiveSinceDate);
                }
            }
            catch (Exception oGenProfileProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Profile Provider Error", oGenProfileProviderException, this);
                throw;
            }

            return iNumberOfInactiveProfiles;
        }

        public override string ApplicationName
        {
            get;
            set;
        }


        /// <summary>
        /// Fired whenever a value gets assigned to the Profile property.
        /// </summary>
        /// <param name="oContext"></param>
        /// <param name="oCollection"></param>
        /// <returns></returns>
        public override SettingsPropertyValueCollection GetPropertyValues(System.Configuration.SettingsContext oContext, SettingsPropertyCollection oCollection)
        {

            #region VARIABLES

            System.Configuration.SettingsPropertyValueCollection oSettingsPropertyValueCollection;
            List<ProfilePropertyValueContract> oSettingsPropertyValueCollectionSerialized;
            List<ProfilePropertyContract> oCollectionSerialized;

            #endregion

            Sitecore.Diagnostics.Log.Info("GenProfileProvider.GetPropertyValues", this);

            oCollectionSerialized = Genworth.SitecoreExt.Utilities.ProfileServiceSerialization.SerializeSettingsPropertyCollection(oCollection);

            Sitecore.Diagnostics.Log.Info(string.Format("GenProfileProvider.GetPropertyValues, Properties Count:{0}", oCollectionSerialized.Count), this);

            try
            {
                using (var oGenProfileProviderService = new GenProfileProviderServiceProxy())
                {
                    oSettingsPropertyValueCollectionSerialized = oGenProfileProviderService.GetPropertyValues(oContext, oCollectionSerialized);
                }
            }
            catch (Exception oGenProfileProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Profile Provider Error", oGenProfileProviderException, this);
                throw;
            }
            oSettingsPropertyValueCollection = Genworth.SitecoreExt.Utilities.ProfileServiceSerialization.DeserializeSettingsPropertyValueCollection(oSettingsPropertyValueCollectionSerialized);

            return oSettingsPropertyValueCollection;


        }

        public override void SetPropertyValues(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyValueCollection collection)
        {
            throw new NotImplementedException();

        }
    }
}
