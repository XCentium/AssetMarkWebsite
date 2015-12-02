using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Workflows;

namespace Genworth.SitecoreExt.WorkFlows
{
    /*
     * AdvancedWorkflowProvider was take from an example of how to publish to a pre-production web database.
     * This code works together with: AdvancedWorkflow and PublishHelper.
     * Also it requires to add database settings and a publish target for the pre-production web environment.
     * Autor: Alex Shyba 
     * Blog: http://sitecoreblog.alexshyba.com/
     * Links: http://sitecoreblog.alexshyba.com/2010/09/publish-to-pre-production-web-database.html
     * Also see this document for adding a Publishing Target Database configuration:
     * http://www.sitecore.net/~/media/Products/Product%20Categories/Web%20Content%20Management/Content%20Management/Multisite%20Deployment/scaling_guide_sc6364usletter.ashx
     */
    public class AdvancedWorkflowProvider : Sitecore.Workflows.Simple.WorkflowProvider
    {
        public AdvancedWorkflowProvider(string databaseName, HistoryStore historyStore)
            : base(databaseName, historyStore)
        {
        }

        public override IWorkflow GetWorkflow(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            string workflowID = GetWorkflowID(item);

            if (workflowID.Length > 0)
            {
                // customization
                return new AdvancedWorkflow(workflowID, this);
                // customization
            }

            return null;
        }

        private static string GetWorkflowID(Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            WorkflowInfo workflowInfo = item.Database.DataManager.GetWorkflowInfo(item);

            if (workflowInfo != null)
            {

                return workflowInfo.WorkflowID;

            }
            return string.Empty;
        }

        public override IWorkflow GetWorkflow(string workflowID)
        {
            Assert.ArgumentNotNullOrEmpty(workflowID, "workflowID");

            Error.Assert(ID.IsID(workflowID), "The parameter 'workflowID' must be parseable to an ID");

            if (Database.Items[ID.Parse(workflowID)] != null)
            {
                // customization
                return new AdvancedWorkflow(workflowID, this);
                // customization
            }
            return null;
        }

        public override IWorkflow[] GetWorkflows()
        {
            Item item = this.Database.Items[ItemIDs.WorkflowRoot];

            if (item == null)
            {
                return new IWorkflow[0];
            }

            Item[] itemArray = item.Children.ToArray();

            IWorkflow[] workflowArray = new IWorkflow[itemArray.Length];

            for (int i = 0; i < itemArray.Length; i++)
            {
                // customization
                var wfId = itemArray[i].ID.ToString();
                workflowArray[i] = new AdvancedWorkflow(wfId, this);
                // customization
            }

            return workflowArray;
        }

    }

}
