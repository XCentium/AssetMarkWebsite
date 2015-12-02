using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;

using Sitecore.Data.Items;
using System.ServiceModel.Web;
using System.Web;
using Genworth.SitecoreExt.Helpers;
using Genworth.SitecoreExt.Marketing;

namespace Genworth.SitecoreExt.Services.Marketing
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MarketingService : IMarketingService
    {
        public MarketingDataItem GetHtmlFormData(string agentId, string actionUrl)
        {
            var collection = Controller.BuildStandardRegisterHtmlFormData(agentId, true, actionUrl);
            MarketingDataItem items = new MarketingDataItem()
            {
                Fields = collection.AllKeys.ToDictionary(k => k, k => System.Web.HttpUtility.HtmlEncode(collection[k])),
                ActionUrl = Controller.LogonURL
            };
            return items;
        }

        public Dictionary<string, string> GetUserRoles(string roleId)
        {
            int role;
            if (Int32.TryParse(roleId, out role))
            {
                return MarketingLogic.GetUserRoles(role);
            }
            return MarketingLogic.GetUserRoles();
        }

        public string GetMarcomCentralAuthenticationUrl(string agentId, string pagename, string productId)
        {
            var request = Controller.BuildMarcomCentralAuthenticationRequest(agentId, pagename, productId);
            return Controller.SendMarcomCentralAuthenticationRequest(request);
        }
    }
}
