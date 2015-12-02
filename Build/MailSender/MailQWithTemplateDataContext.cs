using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MailSender
{
    public class MailQWithTemplateDataContext : DataContext
    {
        private static readonly string AM_STAGEQ = "AM_STAGEQ";

        public MailQWithTemplateDataContext(string conn) :
            base(conn, XmlMappingSource.FromStream(Assembly.GetExecutingAssembly()
               .GetManifestResourceStream("Genworth.SitecoreExt.MailSender.MailQWithTemplateMapping.xml")))
        {
            this.Log = System.Console.Out;
        }

        public MailQWithTemplateDataContext() : this(ConfigurationManager.ConnectionStrings[AM_STAGEQ].ToString()) { }

        [ResultType(typeof(ContentID))]
        [ResultType(typeof(MailQID))]
        [ResultType(typeof(ContactID))]
        [ResultType(typeof(ValueID))]
        public IMultipleResults SendEmailByTemplate(string fromAddress, string fromName, string bCCRecipients, string cCRecipients, string recipients, string subject, string body, System.Nullable<int> contentTypeCD, System.Nullable<int> templateId, string templateItems, System.Nullable<System.DateTime> sendAfterDate)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), fromAddress, fromName, bCCRecipients, cCRecipients, recipients, subject, body, contentTypeCD, templateId, templateItems, sendAfterDate);
            return ((IMultipleResults)(result.ReturnValue));
        }
    }
}
