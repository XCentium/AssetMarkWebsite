using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

namespace Genworth.SitecoreExt.Services.Providers
{	
	[ServiceContract]
	[ServiceKnownType(typeof(System.Type))]
	public interface IGenProfileProviderService
	{
		[OperationContract]
		System.Web.Profile.ProfileInfoCollection FindInactiveProfilesByUserName(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, string sUsernameToMatch, DateTime dUserInactiveSinceDate);

		[OperationContract]
		System.Web.Profile.ProfileInfoCollection FindProfilesByUserName(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, string sUsernameToMatch);

		[OperationContract]
		System.Web.Profile.ProfileInfoCollection GetAllInactiveProfiles(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, DateTime dUserInactiveSinceDate);

		[OperationContract]
		System.Web.Profile.ProfileInfoCollection GetAllProfiles(out int iTotalRecords, int iPageIndex, int iPageSize, System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption);

		[OperationContract]
		int GetNumberOfInactiveProfiles(System.Web.Profile.ProfileAuthenticationOption oAuthenticationOption, DateTime dUserInactiveSinceDate);

		[OperationContract]
		List<ProfilePropertyValueContract> GetPropertyValues(System.Configuration.SettingsContext oContext, List<ProfilePropertyContract> oCollection);
	}
}
