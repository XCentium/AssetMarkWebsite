using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.CustomDataProvider
{
	class GenRoleProvider : System.Web.Security.RoleProvider
	{


		#region VARIABLES

		


		#endregion

		#region PROPERTIES

		
		#endregion

		public override void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			throw new NotImplementedException("GenRoleProviderAddUsersToRoles");
		}

		public override string ApplicationName
		{
			get;
			set;
		}

		public override void CreateRole(string roleName)
		{
			throw new NotImplementedException("GenRoleProvider.CreateRole");
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
			throw new NotImplementedException("GenRoleProvider.DeleteRole");
		}

		/// <summary>
		/// Gets a list of users in a specified role where the user name contains the specified user name to match.
		/// </summary>
		/// <param name="sRoleName">The role to search in</param>
		/// <param name="sUsernameToMatch">The user name to search for</param>
		/// <returns>A string array containing the names of all the users whose user name matches usernameToMatch and who are members of the specified role.</returns>
		public override string[] FindUsersInRole(string sRoleName, string sUsernameToMatch)
		{						

			Sitecore.Diagnostics.Log.Info(string.Format("GenRoleProvider.FindUsersInRole, role name{0}, user to match:{1}", sRoleName, sUsernameToMatch), this);
			return null;
		}

		/// <summary>
		/// Gets a list of all the roles for the sitecore application
		/// ( Since no roles will be handled directly in sitecore this functionality is not required)
		/// </summary>
		/// <returns>A string array containing the names of all the roles stored in the data source for the application</returns>
		public override string[] GetAllRoles()
		{
			Sitecore.Diagnostics.Log.Info("GenRoleProvider.GetAllRoles", this);
			return null;
		}

		/// <summary>
		/// Gets a list of the roles that a user is in.
		/// ( Since no roles will be handled directly in sitecore this functionality is not required)
		/// </summary>
		/// <param name="sUsername">The user name to match</param>
		/// <returns></returns>
		public override string[] GetRolesForUser(string sUsername)
		{
			Sitecore.Diagnostics.Log.Info(string.Format("GenRoleProvider.GetRolesForUser, username{0}", sUsername), this);
			return null;
		}

		/// <summary>
		/// Gets a list of users in the specified role.
		/// ( Since no roles will be handled directly in sitecore this functionality is not required)
		/// </summary>
		/// <param name="oRoleName">The role to get the list of users for</param>
		/// <returns>A string array containing the names of all the users who are members of the specified role.</returns>
		public override string[] GetUsersInRole(string sRoleName)
		{
			Sitecore.Diagnostics.Log.Info(string.Format("GenRoleProvider.GetUsersInRole, role name:{0}", sRoleName), this);
			return null;
		}

		/// <summary>
		/// Gets a value indicating whether a user is in the specified role.
		/// ( Since no roles will be handled directly in sitecore this functionality is not required)
		/// </summary>
		/// <param name="sUsername">The user name to match</param>
		/// <param name="sRoleName">The role to check</param>
		/// <returns></returns>
		public override bool IsUserInRole(string sUsername, string sRoleName)
		{
			Sitecore.Diagnostics.Log.Info(string.Format("GenRoleProvider.IsUserInRole, username:{0}, role name:{1}", sUsername, sRoleName), this);
			//For the Gen Role Provider we will only be working with the extranet\Anonymous and the default\Anonymous which are not assigned to any role by default so no need to check
			return false;			
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			throw new NotImplementedException("GenRoleProvider.RemoveUsersFromRoles");
		}

		/// <summary>
		/// Gets a value indicating whether the specified role name already exists in the role data source.
		/// ( Since no roles will be handled directly in sitecore this functionality is not required)
		/// </summary>
		/// <param name="sRoleName">The role to check</param>
		/// <returns></returns>
		public override bool RoleExists(string sRoleName)
		{
			Sitecore.Diagnostics.Log.Info(string.Format("GenRoleProvider.RoleExists, role name:{0}", sRoleName), this);
			return false;
		}
	}
}
