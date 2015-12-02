using System;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.Workflows;
using Sitecore.Workflows.Simple;
using Version = Sitecore.Data.Version;

namespace Genworth.SitecoreExt.WorkFlows
{
    /*
     * AdvancedWorkflow was take from an example of how to publish to a pre-production web database.
     * This code works together with: AdvancedWorflowProvider and PublishHelper.
     * Also it requires to add database settings and a publish target for the pre-production web environment.
     * Autor: Alex Shyba 
     * Blog: http://sitecoreblog.alexshyba.com/
     * Link: http://sitecoreblog.alexshyba.com/2010/09/publish-to-pre-production-web-database.html
     * Also see this document for adding a Publishing Target Database configuration:
     * http://www.sitecore.net/~/media/Products/Product%20Categories/Web%20Content%20Management/Content%20Management/Multisite%20Deployment/scaling_guide_sc6364usletter.ashx
     */
    public class AdvancedWorkflow : Sitecore.Workflows.Simple.Workflow
    {
        private readonly AdvancedWorkflowProvider _owner;

        public AdvancedWorkflow(string workflowID, AdvancedWorkflowProvider owner)
            : base(workflowID, owner)
        {
            _owner = owner;
        }

        private Database Database
        {
            get
            {
                return _owner.Database;
            }
        }

        public override bool IsApproved(Item item)
        {
            var result = base.IsApproved(item);

            if (!result && Context.Site.Name.Equals("publisher", StringComparison.InvariantCultureIgnoreCase))
            {
                var stateItem = GetStateItem(item);
                if (stateItem != null && MatchTargetDatabase(stateItem) && IgnoreWorkflow(stateItem))
                {
                    result = true;
                }
            }

            return result;
        }

        protected virtual bool MatchTargetDatabase(Item stateItem)
        {
            if (Context.Job != null && !String.IsNullOrEmpty(Context.Job.Name))
            {
                var target = TargetDatabase(stateItem);

                return Context.Job.Name.Equals(String.Format("Publish to '{0}'", target), StringComparison.InvariantCultureIgnoreCase);
            }

            return false;

        }

        protected virtual string TargetDatabase(Item stateItem)
        {
            var publishTargetId = stateItem["Semi-Final Target Database"];
            var publishTargetItem = PublishActionHelper.GetItemById(publishTargetId);

            if (publishTargetItem != null)
            {
                return PublishActionHelper.GetFieldValue(publishTargetItem, "Target database");
            }

            return String.Empty;

        }

        protected virtual bool IgnoreWorkflow(Item stateItem)
        {
            return stateItem["Semi-Final"] == "1";
        }

        private Item GetStateItem(Item item)
        {
            string stateID = GetStateID(item);

            if (stateID.Length > 0)
            {
                return GetStateItem(stateID);
            }

            return null;
        }

        private Item GetStateItem(ID stateId)
        {
            return ItemManager.GetItem(stateId, Language.Current, Version.Latest, Database, SecurityCheck.Disable);
        }

        private Item GetStateItem(string stateId)
        {
            ID id = MainUtil.GetID(stateId, null);

            return id == (ID)null ? null : this.GetStateItem(id);
        }

        private string GetStateID(Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            WorkflowInfo workflowInfo = item.Database.DataManager.GetWorkflowInfo(item);

            if (workflowInfo != null)
            {
                return workflowInfo.StateID;
            }
            return string.Empty;
        }

    }

}
