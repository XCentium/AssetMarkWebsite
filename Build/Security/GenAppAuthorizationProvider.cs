using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Security.AccessControl;
using System.Xml.Linq;

namespace Genworth.SitecoreExt.Security
{
	/// <summary>
	/// Provides methods for working with nested sitecore authorization.
	/// </summary>
	class GenAppAuthorizationProvider : AuthorizationProvider
	{

		#region VARIABLES

		#region CONSTANTS

		
		#endregion		


		#endregion

		#region PROPERTIES		

		#endregion


		protected override AccessResult GetAccessCore(ISecurable entity, Sitecore.Security.Accounts.Account oAccount, AccessRight oAccessRight)
		{			
			#region VARIABLES

			AccessResult oAccesResult;
			AccessExplanation oAccessExplanation;
			string sAccessExplanationText;
			
			#endregion

			//Sitecore.Diagnostics.Log.Info(string.Format("GenSqlServerAuthorizationProvider.GetAccessCore, entity{0}", entity.GetUniqueId()), this);

            switch (oAccessRight.Name)
            {
                case "item:read":
                case "field:read":
                case "language:read":
                case "site:enter":
                    oAccessExplanation = new AccessExplanation(string.Format("{0} acces right granted for entity {1}", oAccessRight.Name, entity.GetUniqueId()), new object[0]);
                    oAccesResult = new AccessResult(AccessPermission.Allow, oAccessExplanation);
                    break;
                default:
                    sAccessExplanationText = string.Format("Access right {0} is unavailable in this instance layer", oAccessRight.Name);
                    oAccessExplanation = new AccessExplanation(sAccessExplanationText, new object[0]);
                    oAccesResult = new AccessResult(AccessPermission.Deny, oAccessExplanation);
                    Sitecore.Diagnostics.Log.Debug(sAccessExplanationText, this);
                    break;
            }

            //Sitecore.Diagnostics.Log.Info(string.Format("GenSqlServerAuthorizationProvider.GetAccessCore, AccessResult:{0}", oAccesResult.Permission), this);

			return oAccesResult;
		}


		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="oEntity"></param>
		/// <returns></returns>
		public override AccessRuleCollection GetAccessRules(ISecurable oEntity)
		{
			throw new NotImplementedException("GenSqlServerAuthorizationProvider.SetAccessRules");
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="rules"></param>
		public override void SetAccessRules(ISecurable entity, AccessRuleCollection rules)
		{
			throw new NotImplementedException("GenSqlServerAuthorizationProvider.SetAccessRules");
		}
	}
}
