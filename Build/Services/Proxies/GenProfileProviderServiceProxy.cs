using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml.Linq;

namespace Genworth.SitecoreExt.Services.Providers
{
    class GenProfileProviderServiceProxy : ClientBase<IGenProfileProviderService>, IGenProfileProviderService, IDisposable
    {
        public System.Web.Profile.ProfileInfoCollection FindInactiveProfilesByUserName(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, string sUsernameToMatch, DateTime dUserInactiveSinceDate)
        {
            return base.Channel.FindInactiveProfilesByUserName(out iTotalRecords, iPageIndex, iPageSize, oAuthenticationOption, sUsernameToMatch, dUserInactiveSinceDate);
        }

        public System.Web.Profile.ProfileInfoCollection FindProfilesByUserName(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, string sUsernameToMatch)
        {
            return base.Channel.FindProfilesByUserName(out iTotalRecords, iPageIndex, iPageSize, oAuthenticationOption, sUsernameToMatch);
        }

        public System.Web.Profile.ProfileInfoCollection GetAllInactiveProfiles(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, DateTime dUserInactiveSinceDate)
        {
            return base.Channel.GetAllInactiveProfiles(out iTotalRecords, iPageIndex, iPageSize, oAuthenticationOption, dUserInactiveSinceDate);
        }

        public System.Web.Profile.ProfileInfoCollection GetAllProfiles(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption)
        {
            return base.Channel.GetAllProfiles(out iTotalRecords, iPageIndex, iPageSize, oAuthenticationOption);
        }

        public int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, DateTime dUserInactiveSinceDate)
        {
            return base.Channel.GetNumberOfInactiveProfiles(oAuthenticationOption, dUserInactiveSinceDate);
        }

        public List<ProfilePropertyValueContract> GetPropertyValues(System.Configuration.SettingsContext oContext, List<ProfilePropertyContract> oCollection)
        {
            return base.Channel.GetPropertyValues(oContext, oCollection);
        }

        /// <summary>
        /// IDisposable.Dispose implementation, calls Dispose(true).
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose worker method. Handles graceful shutdown of the
        /// client even if it is an faulted state.
        /// </summary>
        /// <param name="disposing">Are we disposing (alternative
        /// is to be finalizing)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (State != CommunicationState.Faulted)
                    {
                        Close();
                    }
                }
                finally
                {
                    if (State != CommunicationState.Closed)
                    {
                        Abort();
                    }
                }
            }
        }

    }
}
