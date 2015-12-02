using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Web;
using System.Net.Mail;
using GNW.Email.Entity;
using GNW.Email.Service;
using System.Text.RegularExpressions;
using Genworth.SitecoreExt.Security;
using GFWM.Shared.Entity.Data;
using GFWM.Shared.ServiceRequest;
using GFWM.Shared.ServiceRequestFactory;

namespace Genworth.SitecoreExt.Utilities
{
	public class Email
	{

        /// <summary>
        /// Regular expression, which is used to validate an E-Mail address.
        /// </summary>
        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
           + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";


        private const int iUnableToSendEmail = -1;

        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="sEmail">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        public static bool IsEmailValid(string sEmail)
        {
            bool bIsValidEmail;

            if (!string.IsNullOrEmpty(sEmail))
            {
                bIsValidEmail = Regex.IsMatch(sEmail, MatchEmailPattern);
            }
            else
            {
                bIsValidEmail = false;
            }

            return bIsValidEmail;
        }

		private static string SMTPServer
		{
			get {
				return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Email.SMTPServer, string.Empty);
				}
		}

        static public Dictionary<string, string> ParseSemicolonDelimitedEmailList(string sSemicolonDelimitedEmaillist)
        {
            #region VARIABLES

            Dictionary<string, string> oEmails;
            string[] oListSplited;
            string sCurrentEmail;
            string sCurrentEmailName;
            int iEmailBeginIndex;
            int iEmailEndIndex;
            int iEmailNameIndexEnd;
            int iEmailLength;
            string sTrimedEmail;

            #endregion
            
            sCurrentEmail = string.Empty;
            sCurrentEmailName = string.Empty;
            oEmails = new Dictionary<string, string>();

            oListSplited = sSemicolonDelimitedEmaillist.Split(';');

            if (oListSplited != null && oListSplited.Length > 0)
            {
                foreach (string sEmail in oListSplited)
                {
                    

                    if (((iEmailBeginIndex = sEmail.IndexOf('<')) > -1) && ((iEmailEndIndex = sEmail.IndexOf('>')) > -1))
                    {
                        iEmailNameIndexEnd = iEmailBeginIndex - 1;

                        if(iEmailNameIndexEnd > 0)
                        {
                            sCurrentEmailName = sEmail.Substring(0, iEmailNameIndexEnd).Trim();
                        }

                        iEmailLength =  iEmailEndIndex - iEmailBeginIndex -1;
                           
                        if(iEmailLength > 0)
                        {
                            sCurrentEmail = sEmail.Substring(iEmailBeginIndex + 1, iEmailLength).Trim();
                        }                        
                    }
                    else
                    {
                        sTrimedEmail = sEmail.Trim();
                        if (IsEmailValid(sTrimedEmail))
                        {
                            sCurrentEmail = sTrimedEmail;
                        }
                    }

                    if (!string.IsNullOrEmpty(sCurrentEmail) && IsEmailValid(sCurrentEmail) && !oEmails.Keys.Contains(sCurrentEmail))
                    { 
                        if(!string.IsNullOrEmpty(sCurrentEmailName))
                        {
                            oEmails.Add(sCurrentEmail, sCurrentEmailName);
                        }
                        else
                        {
                            oEmails.Add(sCurrentEmail, sCurrentEmail);                        
                        }
                    }                    

                    sCurrentEmail = string.Empty;
                    sCurrentEmailName = string.Empty;
                }
            }

            return oEmails;
        }

        
		/// <summary>
		/// Sends an email by using the XSLT template file. The template XML is transform over a IDictionary object
		/// to an XHTML document.
		/// </summary>
		/// <param name="sEmailTo">To email address</param>
		/// <param name="oXsltEmailTemplate">Xml Reader from XSLT template file </param>
		/// <param name="oParameters">Dictonary objects containing data to be inserted in the transformed doc.</param>
		static public void SendEmail(Dictionary<string, string> oToEmails, Dictionary<string, string> oCCEmails, string sEmailFrom, string sEmailFromName, string sEmailSubject, XmlReader oXsltEmailTemplate, IDictionary oParameters)
		{
			XslCompiledTransform oXslt;
			XmlDocument oXmlDoc;
			XPathNavigator oXpathNavigator;
			XsltArgumentList oXslArguments;
			StringBuilder oEmailBuilder;
			XmlTextWriter oXmlWriter;			
			string sEmailBody;            
            string oListOfContacts;
            string oListOfCCContacts;                        
            Authorization oAuthorization;
            RoleClaim oCurrentRoleClaim;

			if (oXsltEmailTemplate != null)
			{
				oXslt = new XslCompiledTransform();

				oXslt.Load(oXsltEmailTemplate);

				oXmlDoc = new XmlDocument();
				oXmlDoc.AppendChild(oXmlDoc.CreateElement("DocumentRoot"));
				
				oXpathNavigator = oXmlDoc.CreateNavigator();


				oXslArguments = new XsltArgumentList();
				if (oParameters != null)
				{
					foreach (DictionaryEntry entry in oParameters)
					{
						oXslArguments.AddParam(entry.Key.ToString(), string.Empty, entry.Value);
					}
				}

				oEmailBuilder = new StringBuilder();

				oXmlWriter = new XmlTextWriter(new System.IO.StringWriter(oEmailBuilder));

				oXslt.Transform(oXpathNavigator, oXslArguments, oXmlWriter, null);				

				if (oEmailBuilder.Length > 0)
				{
                    oAuthorization = Authorization.CurrentAuthorization;
                                        
                    if (oAuthorization != null && !oAuthorization.IsTestMode)
                    {
                        
                        sEmailBody = HttpUtility.HtmlDecode(oEmailBuilder.ToString());

                        oListOfContacts = EmailsDictionaryToCommaDelimitedString(oToEmails);

                        oListOfCCContacts = EmailsDictionaryToCommaDelimitedString(oCCEmails);

                        if (oListOfContacts.Length > 0)
                        {                            
                            SendEmail(sEmailFrom, sEmailFromName, oListOfContacts, oListOfCCContacts, sEmailSubject, sEmailBody);
                        }
                        else
                        {
                            Sitecore.Diagnostics.Log.Error("SendEmail - Email could not be sent because it has no destination email addresses", typeof(Utilities.Email));
                        }
                    }
				}
			}

		}

        static private string EmailsDictionaryToCommaDelimitedString(Dictionary<string, string> oEmailsDictionary)
        {
            string CommaDelimitedEmails;

            CommaDelimitedEmails = string.Empty;

            foreach (var oEmail in oEmailsDictionary)
            {
                if (!string.IsNullOrEmpty(CommaDelimitedEmails))
                {
                    CommaDelimitedEmails = string.Format("{0},{1}", CommaDelimitedEmails, oEmail.Key);
                }
                else
                {
                    CommaDelimitedEmails = oEmail.Key;
                }
            }

            return CommaDelimitedEmails;
        }

        static private int SendEmail(string sEmailFrom, string sEmailFromName, string oListOfContacts, string oListOfCCContacts, string sEmailSubject, string sEmailBody)
        {
            int iSuccess;
            IServiceRequest oServiceProxy;
            GFWM.Common.AUM.Entities.Response.Client.EventSendEmailResponse oEmailResponse;
            GFWM.Common.AUM.Entities.Request.Client.EventSendEmailRequest oRequestEmail;

            try
            {
                oServiceProxy = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

                oEmailResponse = null;
                oRequestEmail = new GFWM.Common.AUM.Entities.Request.Client.EventSendEmailRequest();

                oRequestEmail.SendByUserName = sEmailFromName;
                oRequestEmail.SendByUserEmail = sEmailFrom;
                oRequestEmail.Recipient = oListOfContacts;
                oRequestEmail.CCRecipients = oListOfCCContacts;
                oRequestEmail.Subject = sEmailSubject;
                oRequestEmail.Body = sEmailBody;

                oEmailResponse = oServiceProxy.Request<GFWM.Common.AUM.Entities.Request.Client.EventSendEmailRequest, GFWM.Common.AUM.Entities.Response.Client.EventSendEmailResponse>(oRequestEmail);
                iSuccess = (oEmailResponse == null) ? iUnableToSendEmail : oEmailResponse.Result;
            }
            catch (Exception oEmailSendingException)
            {
                Sitecore.Diagnostics.Log.Error("Error while trying send email", oEmailSendingException, typeof(Email));
                throw oEmailSendingException;
            }

            return iSuccess;
        }


        static private void SendEmail(GenericEmail email)
        {
            //Ivoke Email Service
            EmailRequest requestEmail = new EmailRequest();
            requestEmail.EmailContent = email;

            //Send fax by email proxy

            EmailResponse emailResponse = (EmailResponse)GenericEmailProcessor.SendEMail(requestEmail);

            if (emailResponse == null)
            {
                Sitecore.Diagnostics.Log.Error("SendEmail - Unable to send email", typeof(Utilities.Email));
            }
            else
            {
                if (emailResponse.MailQId == 0 || emailResponse.MailQId == -1)
                {
                    Sitecore.Diagnostics.Log.Error(string.Format("SendEmail - Unable to send email. Fault: {0}", emailResponse.Fault), typeof(Utilities.Email));
                }               
            }


        }


		static private void SendEmail(MailAddressCollection oEmailTo, MailAddress oEmailFrom, string sEmailSubject, string sEmailBody)
		{
			string sSMTPServer = SMTPServer;
			SmtpClient oSMTPClient;
			if (oEmailTo != null && oEmailTo.Count > 0 && oEmailFrom != null && !string.IsNullOrEmpty(sEmailSubject) && !string.IsNullOrEmpty(sEmailBody))
			{
				MailMessage mailmessage = new MailMessage();
				mailmessage.From = oEmailFrom;
				foreach(MailAddress oMailAddress in oEmailTo)
				{
					mailmessage.To.Add(oMailAddress);
				}
				mailmessage.Subject = sEmailSubject;
				mailmessage.Body = sEmailBody;
				mailmessage.BodyEncoding = Encoding.UTF8;
				mailmessage.IsBodyHtml= true;

				oSMTPClient = new SmtpClient(sSMTPServer);
				
				try
				{
					oSMTPClient.Send(mailmessage);
				}
				catch(Exception oEmailException)
				{
					Sitecore.Diagnostics.Log.Error("Error trying to send email", oEmailException, typeof(Email));
					throw;
				}
				finally
				{
					mailmessage.Dispose();
				}
			}
		}
		
	}
}
