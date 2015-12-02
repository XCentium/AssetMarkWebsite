using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Security.Accounts;
using System.Xml.Linq;



namespace Genworth.SitecoreExt.CustomDataProvider
{
	/// <summary>
	/// Provides methods for working with nested sitecore roles.
	/// </summary>
	class GenSqlServerRolesInRolesProvider : RolesInRolesProvider
	{
		#region VARIABLES

		


		#endregion

		#region PROPERTIES

		

		#endregion



		#region METHODS

		/// <summary>
		/// Nested sitecore role provider, for reference review: SqlServerRolesInRolesProvider.FindRolesInRole
		/// ( Since no roles will be handled directly in sitecore this functionality is not required)
		/// </summary>
		/// <param name="sTargetRoleName"></param>
		/// <param name="sRoleNameToMatch"></param>
		/// <returns></returns>
		protected override IEnumerable<Role> FindRolesInRole(string sTargetRoleName, string sRoleNameToMatch)
		{
			Sitecore.Diagnostics.Log.Info("GenSqlServerRolesInRolesProvider.FindRolesInRole", this);
			return new List<Role>();
		}

		/// <summary>
		/// Nested sitecore role provider, for reference review: SqlServerRolesInRolesProvider.GetRolesForRole
		/// </summary>
		/// <param name="sTargetRoleName"></param>
		/// <param name="sRoleNameToMatch"></param>
		/// <returns></returns>
		protected override IEnumerable<Role> GetRolesForRole(string sMemberRoleName)
		{
			throw new NotImplementedException("GenSqlServerRolesInRolesProvider.GetRolesForRole");
		}

		/// <summary>
		/// Nested sitecore role provider, for reference, review: SqlServerRolesInRolesProvider.GetRolesInRole
		/// ( Since no roles will be handled directly in sitecore this functionality is not required)
		/// </summary>
		/// <param name="sTargetRoleName"></param>
		/// <param name="sRoleNameToMatch"></param>
		/// <returns></returns>
		protected override IEnumerable<Role> GetRolesInRole(string sTargetRoleName)
		{					
			return new List<Role>();
		}

		/// <summary>
		/// Nested sitecore role provider, for reference, review: SqlServerRolesInRolesProvider.IsRoleInRole 
		/// ( Since no roles will be handled directly in sitecore this functionality is not required)
		/// </summary>
		/// <param name="sTargetRoleName"></param>
		/// <param name="sRoleNameToMatch"></param>
		/// <returns></returns>
		protected override bool IsRoleInRole(string sMemberRoleName, string sTargetRoleName)
		{
			Sitecore.Diagnostics.Log.Info(string.Format("GenSqlServerRolesInRolesProvider.IsRoleInRole, member role name:{0}, target role name{1}", sMemberRoleName, sTargetRoleName), this);

			return false;
		}

		/// <summary>
		/// Not available
		/// </summary>
		/// <param name="sMemberRoles"></param>
		/// <param name="sTargetRoles"></param>
		public override void RemoveRoleRelations(string sRoleName)
		{
			throw new NotImplementedException("GenSqlServerRolesInRolesProvider.RemoveRoleRelations");
		}

		/// <summary>
		/// Not available
		/// </summary>
		/// <param name="sMemberRoles"></param>
		/// <param name="sTargetRoles"></param>
		public override void RemoveRolesFromRoles(IEnumerable<Role> sMemberRoles, IEnumerable<Role> sTargetRoles)
		{
			throw new NotImplementedException("GenSqlServerRolesInRolesProvider.RemoveRolesFromRoles");
		}


		/// <summary>
		/// Not available
		/// </summary>
		/// <param name="sMemberRoles"></param>
		/// <param name="sTargetRoles"></param>
		public override void AddRolesToRoles(IEnumerable<Role> sMemberRoles, IEnumerable<Role> sTargetRoles)
		{
			throw new NotImplementedException("GenSqlServerRolesInRolesProvider.AddRolesToRoles");
		}

		#endregion
	}
}
