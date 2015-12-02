using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;

namespace Genworth.SitecoreExt.MailSender
{
    public class MailQProvider
    {

        private const string FROM_KEYNAME = @"From_KeyName";
        private const string FROM_ADDRESS_KEYNAME = @"From_Address_KeyName";
        private const int HTML_CONTENT = 40102;

        public MailQProvider(){
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="templateId"></param>
        /// <param name="templateValues"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public int SendEmailWithTemplate(string toAddress, int templateId, string templateValues, string subject)
        {
            int mailQId = 0;
            try
            {
                using (MailQDataContext mailqContextWithTemplate = new MailQDataContext())
                {
                    IMultipleResults results = mailqContextWithTemplate.SendEmailByTemplate(
                    Sitecore.Configuration.Settings.GetSetting(FROM_ADDRESS_KEYNAME)
                    ,Sitecore.Configuration.Settings.GetSetting(FROM_KEYNAME)
                    ,string.Empty
                    ,string.Empty
                    ,toAddress
                    ,subject
                    ,null
                    ,HTML_CONTENT
                    ,templateId
                    ,templateValues
                    ,null);

                    MailQID mailQ = results.GetResult<MailQID>().FirstOrDefault();

                    if (mailQ != null)
                    {
                        mailQId = mailQ.Value;
                    }

                }
            }
            catch(Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.MailSender: SendEmailWithTemplate failed due to {0}", ex.Message), this);
                throw;
            }

            return mailQId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="bccAddress"></param>
        /// <param name="ccAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public int SendEmailWithOutTemplate(string toAddress, string bccAddress, string ccAddress, string subject, string body)
        {
            int mailQId = SendEmailWithOutTemplate(Sitecore.Configuration.Settings.GetSetting(FROM_ADDRESS_KEYNAME),
                                                    Sitecore.Configuration.Settings.GetSetting(FROM_KEYNAME),
                                                    toAddress, bccAddress, ccAddress, subject, body);
            
            return mailQId;
        }

        public int SendEmailWithOutTemplate(string fromAddress, string fromAddressName, string toAddress, string bccAddress, string ccAddress, string subject, string body)
        {
            int mailQId = 0;
            try
            {

                using (MailQDataContext mailqContextWithOutTemplate = new MailQDataContext())
                {
                    IMultipleResults results = mailqContextWithOutTemplate.SendEmailWithOutTemplate(
                    Sitecore.Configuration.Settings.GetSetting(FROM_ADDRESS_KEYNAME)
                    , Sitecore.Configuration.Settings.GetSetting(FROM_KEYNAME)
                    , bccAddress
                    , ccAddress
                    , toAddress
                    , subject
                    , body
                    , HTML_CONTENT
                    );

                    MailQID mailQ = results.GetResult<MailQID>().FirstOrDefault();

                    if (mailQ != null)
                    {
                        mailQId = mailQ.Value;
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.MailSender: SendEmailWithOutTemplate failed due to {0}", ex.Message), this);
                throw;
            }

            return mailQId;
        }

    }
}
