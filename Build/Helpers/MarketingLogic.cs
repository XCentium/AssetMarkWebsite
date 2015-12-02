using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using Genworth.SitecoreExt.Security;
using Genworth.SitecoreExt.Constants;
using GFWM.Security.STS.Entity;
using System.Security.Cryptography;

namespace Genworth.SitecoreExt.Helpers
{
    public class MarketingLogic
    {

        #region ATTRIBUTES


        private static string sRef;

        private static string sRDDBaseURL;

        private static string sRDDPassword;

        private static string sRDDAccount;


        private static string sGlobalsoftBaseURL;
        #endregion


        #region PROPERTIES

       
        public static string Ref
        {
            get { 
                    if(string.IsNullOrEmpty(sRef))
                    {
                        sRef = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.Ref);

                        if (string.IsNullOrEmpty(sRef))
                        {
                            Sitecore.Diagnostics.Log.Error("Ref encryption array has not been configured in sitecore\\settings", typeof(MarketingLogic));
                        }
                    }

                    return sRef;
                }
        }


        public static string RDDBaseURL
        {

            get {
                    if (string.IsNullOrEmpty(sRDDBaseURL))
                    {
                        sRDDBaseURL = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.RDDBaseURL);
                        if (string.IsNullOrEmpty(sRDDBaseURL))
                        {
                            Sitecore.Diagnostics.Log.Error("RDD base url has not been configured in sitecore\\settings", typeof(MarketingLogic));
                        }
                    }

                    return sRDDBaseURL;
                }
        }

        public static string GlobalsoftBaseURL
        {

            get
            {
                if (string.IsNullOrEmpty(sGlobalsoftBaseURL))
                {
                    sGlobalsoftBaseURL = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.GlobalsoftBaseURL);
                    if (string.IsNullOrEmpty(sGlobalsoftBaseURL))
                    {
                        Sitecore.Diagnostics.Log.Error("Globalsoft base url has not been configured in sitecore\\settings", typeof(MarketingLogic));
                    }
                }

                return sGlobalsoftBaseURL;
            }
        }

        public static string RDDAccount
        {
            get {
                    if (string.IsNullOrEmpty(sRDDAccount))
                    { 
                        sRDDAccount = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.RDDAccount);
                        if (string.IsNullOrEmpty(sRDDAccount))
                        {
                            Sitecore.Diagnostics.Log.Error("RDD Account has not been configured in sitecore\\settings", typeof(MarketingLogic));
                        }
                    }

                    return sRDDAccount;
                }
        }


        public static string RDDPassword
        {
            get
            {
                if (string.IsNullOrEmpty(sRDDPassword))
                {
                    sRDDPassword = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.RDDPassword);
                    if (string.IsNullOrEmpty(sRDDPassword))
                    {
                        Sitecore.Diagnostics.Log.Error("RDD Password has not been configured in sitecore\\settings", typeof(MarketingLogic));
                    }
                }

                return sRDDPassword;
            }
        }

        private static string sTabId;
        public static string TabId
        {
            get
            {
                if (string.IsNullOrEmpty(sTabId))
                {
                    sTabId = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.TabId);

                    if (string.IsNullOrEmpty(sTabId))
                    {
                        Sitecore.Diagnostics.Log.Error("Marketing Tab Id has not been configured in sitecore\\settings", typeof(MarketingLogic));
                    }
                }

                return sTabId;
            }
        }

        private static string sMarcomCentralToken;
        public static string MarcomCentralToken
        {

            get
            {
                if (string.IsNullOrEmpty(sMarcomCentralToken))
                {
                    sMarcomCentralToken = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.MarcomCentral.PartnerCredentialsToken);
                    if (string.IsNullOrEmpty(sMarcomCentralToken))
                    {
                        Sitecore.Diagnostics.Log.Error("Marcom Central Partner Credential Token has not been configured in sitecore\\settings", typeof(MarketingLogic));
                    }
                }

                return sMarcomCentralToken;
            }
        }

        private static string sMarcomCentralImpersonatingSettingsPath;
        public static string MarcomCentralImpersonatingSettingsPath
        {

            get
            {
                if (string.IsNullOrEmpty(sMarcomCentralImpersonatingSettingsPath))
                {
                    sMarcomCentralImpersonatingSettingsPath = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.MarcomCentral.ImpersonatingSettingsPath);
                    if (string.IsNullOrEmpty(sMarcomCentralImpersonatingSettingsPath))
                    {
                        Sitecore.Diagnostics.Log.Error("Marcom Central Default Impersonating Settings Path has not been configured in sitecore\\settings", typeof(MarketingLogic));
                    }
                }

                return sMarcomCentralImpersonatingSettingsPath;
            }
        }

        #endregion



        #region METHODS

		public static string[] GetAdvisors()
		{
			Authorization oAuthorization;			
			string[] oAplids;
			

			oAuthorization = Authorization.CurrentAuthorization;
			oAplids = oAuthorization.GetAdvisors();
					
			return oAplids != null? oAplids : new string[0];
		}

        /// <summary>
        /// Get User Levels from Marketing Tab Security section
        /// </summary>
        /// <returns></returns>
        private static int[] GetMarketingTabUserLevels()
        {
           var marketingTab = ContextExtension.CurrentDatabase.GetItem(TabId);
           var levels = marketingTab.GetMultilistItems(Constants.Security.Templates.SecurityBase.Sections.Security.Name, Constants.Security.Templates.SecurityBase.Sections.Security.Fields.UserLevelsFieldName)
               .Select(s => int.Parse(s.GetText(Constants.Security.Templates.UserLevel.Sections.UserLevel.Name,
                                            Constants.Security.Templates.UserLevel.Sections.UserLevel.Fields.CodeFieldName))).ToArray<int>();
          
           return levels;
        }

        public static Dictionary<string, string> GetUserRoles(int? roleId = null)
        {
            Dictionary<string, string> dicUserRoles = new Dictionary<string, string>();

            string format = "{0,15}|{1,30}|{2,-14}";

            int[] authorizedRoles = GetMarketingTabUserLevels(); 

            Authorization oAuthorization = Authorization.CurrentAuthorization;

            var authRoles = oAuthorization.GetRoles();

            if (authRoles != null)
            {
                List<UserRole> filteredRoles = authRoles.Where(u => authorizedRoles.Contains(u.RoleId)).ToList();

                if (filteredRoles.Count > 0)
                {
                    List<UserRole> userRoles = roleId.HasValue ? filteredRoles.Where(u => u.RoleId == roleId.Value).ToList() : filteredRoles;

                    foreach (var u in userRoles)
                    {
                        dicUserRoles.Add(u.AplId, string.Format(format, u.AplId, u.SortName ?? u.UserName ?? string.Empty, (string.IsNullOrEmpty(u.Channel)) ? string.Empty : (u.Channel.ToUpper() == "ADG" ? "Advisor" : "Referrer")));
                    }
                }
            }

            return dicUserRoles;
        }

        public static string SHA1Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return string.Empty;
            }

            // apply this format to phone numbers: (999)999-9999 x999 
            string format = "({0}){1}-{2}";
            string formatExtension = "({0}){1}-{2} x{3}";
            string formattedPhone = string.Empty;

            StringBuilder digits = new StringBuilder();
            foreach (var digit in phone.ToCharArray())
            {
                if (Char.IsDigit(digit))
                {
                    digits.Append(digit);
                }
            }

            int totalDigits = digits.ToString().Count();
            string phoneNumber = digits.ToString();

            // check if valid phone number
            if (totalDigits < 10)
            {
                // return empty phone number
            }
            else if (totalDigits == 10)
            {
                formattedPhone = string.Format(format, phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6));
            }
            else
            {
                formattedPhone = string.Format(formatExtension, phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6, 4), phoneNumber.Substring(10));
            }

            return formattedPhone;
        }
        
        #region DONNELLY INTEGRATION

        /// <summary>
        ///  Function adapted from RRD JS function. used to encrypt RRD password for automatic login
        /// </summary>
        /// <param name="sPassword"></param>
        /// <param name="sSeed"></param>
        /// <returns></returns>
        public static string MWASCrypt(string sPassword, int sSeed)
        {
            int iCipherValue;
            string sPasswordEncrypted;
            char[] oPasswordChars;
            int iIndexConversionHelper;
            int iCipher;
            int iNewCipher;
            string sCharEncrypted;

            iCipherValue = sSeed;

            sPasswordEncrypted = string.Empty;
            if (!(iCipherValue < 1) || !(iCipherValue > 99999999))
            {
                oPasswordChars = sPassword.ToArray();
                if (oPasswordChars != null)
                {                    
                    string cPasswordCharacter = string.Empty;
                    for (int i = 0; i < sPassword.Length; i++)
                    {
                        cPasswordCharacter = sPassword.Substring(i, 1);                        
                        iIndexConversionHelper = Ref.IndexOf(cPasswordCharacter);
                        if (iIndexConversionHelper < 0)
                        {                            
                            Sitecore.Diagnostics.Log.Error("MWASCrypt: password has invalid characters", typeof(MarketingLogic));                            
                        }

                        iCipher = iIndexConversionHelper ^ iCipherValue;
                        iNewCipher = iCipher % Ref.Length;                        
                        sCharEncrypted = Ref.Substring(iNewCipher, 1);
                        sPasswordEncrypted = string.Format("{0}{1}", sPasswordEncrypted, sCharEncrypted);
                    }
                }
                else
                {
                    //Error getting password characters       
                    Sitecore.Diagnostics.Log.Error("MWASCrypt: Error getting password characters", typeof(MarketingLogic));                    
                }
            }
            else
            {
                //Seed needs to be a number   
                Sitecore.Diagnostics.Log.Error("MWASCrypt: seed is not a number or seed is out of range", typeof(MarketingLogic));                
            }

            return sPasswordEncrypted;
        }

        /// <summary>
        /// Builds the RDD  URL that will be used by donnelly and globalsoft integrations
        /// </summary>
        /// <param name="sOrderAgentId"></param>
        /// <returns></returns>
        public static string BuildDonnellyURL(string sOrderAgentId)
        {
            int oSeed;            
            string sPassword;
            string sRDDURL;           
            DateTime oDate;

            oDate = DateTime.Now;

            int currentDate = oDate.Month + oDate.Day + oDate.Year;
            int time = oDate.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            oSeed = currentDate + time;

            sPassword = MWASCrypt(RDDPassword, oSeed);
            sRDDURL = string.Format("{0}?Option=2&username={1}&password={2}&account={3}&datetime={4}", RDDBaseURL, sOrderAgentId, sPassword, RDDAccount, oSeed);

            return sRDDURL;
        }

        /// <summary>
        /// Creates a URL that will link the user to the proper vendor system
        /// </summary>
        /// <param name="sVendor"></param>
        /// <param name="sAdvisorId"></param>
        /// <returns></returns>
        public static string BuildOrderURL(string sVendor, string sAdvisorId)
        {                        
            string sURLRedirect;
            
            sURLRedirect  = string.Empty;

            switch (sVendor.ToLower())
            {
                case "donnelly":
                    sURLRedirect = MarketingLogic.BuildDonnellyURL(sAdvisorId);
                    break;
                case "globalsoft":
                    sURLRedirect = MarketingLogic.BuildGlobalsoftURL(sAdvisorId);
                    break;
            }

            return sURLRedirect;
        }

        #endregion


        #region GLOBALSOFT INTEGRATION

        /// <summary>
        /// Builds the Globalsoft URL that will be used by donnelly and globalsoft integrations
        /// </summary>
        /// <param name="sOrderAgentId"></param>
        /// <returns></returns>
        public static string BuildGlobalsoftURL(string sOrderAgentId)
        {

            string sGlobalsoftURL ;

            sGlobalsoftURL = string.Format("{0}?username={1}", GlobalsoftBaseURL, sOrderAgentId);

            return sGlobalsoftURL;
        }


        #endregion

        #endregion


    }
}
