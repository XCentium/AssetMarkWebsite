using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;

namespace Genworth.SitecoreExt.WorkFlows
{
    /*
     * PublishActionHelper was take from an example of how to publish to a pre-production web database.
     * This code works together with: AdvancedWorkflow and AdvancedWorflowProvider.
     * Also it requires to add database settings and a publish target for the pre-production web environment.
     * Autor: Alex Shyba 
     * Blog: http://sitecoreblog.alexshyba.com/
     * Link: http://sitecoreblog.alexshyba.com/2010/09/publish-to-pre-production-web-database.html
     * Also see this document for adding a Publishing Target Database configuration:
     * http://www.sitecore.net/~/media/Products/Product%20Categories/Web%20Content%20Management/Content%20Management/Multisite%20Deployment/scaling_guide_sc6364usletter.ashx
     */
    public class PublishActionHelper
    {
        public static Database Db
        {
            get { return Context.ContentDatabase ?? Context.Database ?? Factory.GetDatabase("master"); }
        }

        public static string GetFieldValue(Item item, string fieldName)
        {
            return item[fieldName] ?? String.Empty;
        }

        public static Item GetItemById(string id)
        {
            if (ID.IsID(id))
            {
                return Db.GetItem(ID.Parse(id));
            }

            return null;
        }

        public static string GetTargetDatabaseName(string targetId)
        {
            var publishingTarget = Db.SelectSingleItem(targetId);
            return publishingTarget["Target database"] ?? String.Empty;
        }

        public static IEnumerable<Item> GetItemsFromMultilist(Item carrier, string fieldName)
        {
            var multilistField = carrier.Fields[fieldName];

            if (FieldTypeManager.GetField(multilistField) is MultilistField)
            {
                return ((MultilistField)multilistField).GetItems();
            }

            return new Item[0];
        }

    }
}
