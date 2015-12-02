using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace Genworth.SitecoreExt.CustomDataProvider
{
	public class GenMembershipProvider: System.Web.Security.MembershipProvider
	{
		#region VARIABLES		

		#region CONSTANTS

		/// <summary>
		/// Web.config setting that specifies the default membership user to use for the web environment
		/// </summary>
		private const string DefaultUserSettingName = "GenMembershipProviderDefaultUser";

		/// <summary>
		/// Default membership to use if no other was defined in the web.config
		/// </summary>
		private const string DefaultMembershipUser = "extranet\\Anonymous";

		#endregion				


		#endregion

		#region PROPERTIES

		/// <summary>
		/// This is the Membership user that will be returned by default for this membership provider
		/// </summary>
		private string DefaultUser
		{
			get
			{
				return Sitecore.Configuration.Settings.GetSetting(DefaultUserSettingName, DefaultMembershipUser);
			}
		}

		#endregion

		public override string GetUserNameByEmail(string sEmail)
		{
			Sitecore.Diagnostics.Log.Info(string.Format("GetUserNameByEmail, email:{0}", sEmail), this);
			return DefaultUser;
		}

		public override bool ValidateUser(string sUsername, string sPassword)
		{
			Sitecore.Diagnostics.Log.Info(string.Format("ValidateUser, username:{0},password:{1}", sUsername, sPassword), this);
			return string.Equals(DefaultUser, sUsername);
		}

		public override string ApplicationName
		{
			get;
			set;
		}

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			throw new NotImplementedException();
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			throw new NotImplementedException();
		}

		public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
		{
			throw new NotImplementedException();
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			throw new NotImplementedException();
		}

		public override bool EnablePasswordReset
		{
			get { throw new NotImplementedException(); }
		}

		public override bool EnablePasswordRetrieval
		{
			get { throw new NotImplementedException(); }
		}

		public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override int GetNumberOfUsersOnline()
		{
			throw new NotImplementedException();
		}

		public override string GetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		public override System.Web.Security.MembershipUser GetUser(string sUsername, bool bUserIsOnline)
		{

			#region VARIABLES

			System.Web.Security.MembershipUser oMembershipUser;			

			#endregion

			oMembershipUser = null;
			Sitecore.Diagnostics.Log.Info(string.Format("GetUser, username:{0},userIsOnline:{1}", sUsername, bUserIsOnline), this);

			//Since this provider is only to be used in Web environment all the user will be authenticated as "extranet\\Anonymous"
			oMembershipUser = new System.Web.Security.MembershipUser(this.Name,
																		DefaultUser,
																		null,
																		null,
																		null,
																		null,
																		true,
																		false,
																		DateTime.MinValue,
																		DateTime.MinValue,
																		DateTime.MinValue,
																		DateTime.MinValue,
																		DateTime.MinValue
																	);

			if (oMembershipUser != null)
			{
				Sitecore.Diagnostics.Log.Info(string.Format("GetUser, MembershipUser:{0}", oMembershipUser.UserName), this);
			}
			else
			{
				Sitecore.Diagnostics.Log.Info("GetUser, Unable to get MembershipUser", this);
			}

			return oMembershipUser;
		}

		public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			throw new NotImplementedException();
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { throw new NotImplementedException(); }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { throw new NotImplementedException(); }
		}

		public override int MinRequiredPasswordLength
		{
			get { throw new NotImplementedException(); }
		}

		public override int PasswordAttemptWindow
		{
			get { throw new NotImplementedException(); }
		}

		public override System.Web.Security.MembershipPasswordFormat PasswordFormat
		{
			get { throw new NotImplementedException(); }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { throw new NotImplementedException(); }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { throw new NotImplementedException(); }
		}

		public override bool RequiresUniqueEmail
		{
			get { throw new NotImplementedException(); }
		}

		public override string ResetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		public override bool UnlockUser(string userName)
		{
			throw new NotImplementedException();
		}

		public override void UpdateUser(System.Web.Security.MembershipUser user)
		{
			throw new NotImplementedException();
		}
	}
}
