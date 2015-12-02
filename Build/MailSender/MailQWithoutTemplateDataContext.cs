using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Genworth.SitecoreExt.MailSender
{
    public class MailQWithOutTemplateDataContext : DataContext
    {
        private static readonly string AM_STAGEQ = "AM_STAGEQ";

        public MailQWithOutTemplateDataContext() : this(ConfigurationManager.ConnectionStrings[AM_STAGEQ].ToString()) { }

        public MailQWithOutTemplateDataContext(string conn) :
            base(conn, XmlMappingSource.FromStream(Assembly.GetExecutingAssembly()
               .GetManifestResourceStream("Genworth.SitecoreExt.MailSender.MailQWithOutTemplateMapping.xml")))
        {
            this.Log = System.Console.Out;
        }

        [ResultType(typeof(ContentID))]
        [ResultType(typeof(MailQID))]
        [ResultType(typeof(ContactID))]
        public IMultipleResults SendEmailWithOutTemplate(string fromAddress, string fromName, string bCCRecipients, string cCRecipients, string recipients, string subject, string body, System.Nullable<int> contentTypeCD)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), fromAddress, fromName, bCCRecipients, cCRecipients, recipients, subject, body, contentTypeCD);
            return ((IMultipleResults)(result.ReturnValue));
        }
    }
}
