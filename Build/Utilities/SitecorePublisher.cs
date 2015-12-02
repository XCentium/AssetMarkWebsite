using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecore.Publishing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.Utilities
{
    public static class SitecorePublisher
    {
        #region "Publish item"

        /// <summary>
        /// Publishes an item with default options, from master to web
        /// </summary>
        /// <param name="item">Item to be published</param>
        public static void PublishItem(this Item item)
        {
            PublishItem(item, false);
        }

        /// <summary>
        /// Publishes an item and its subitems, a SmartPublish with subitems
        /// </summary>
        /// <param name="item">Item to be published</param>
        /// <param name="includeSubitems">should subitems also be published</param>
        public static void PublishItem(this Item item, bool includeSubitems)
        {
            var sourceDatabase = Sitecore.Configuration.Factory.GetDatabase("master");
            var targetDatabase = Sitecore.Configuration.Factory.GetDatabase("web");
            PublishItem(item, includeSubitems, sourceDatabase, targetDatabase, false);
        }

        /// <summary>
        /// Publishes an item with default options, from master to web, in async mode
        /// </summary>
        /// <param name="item">Item to be published</param>
        /// <returns>a bool value which indicates whether item published successfully or not.</returns>
        public static bool PublishItemAsync(this Item item)
        {
            return PublishItemAsync(item, false);
        }

        /// <summary>
        /// Publishes an item and its subitems, a SmartPublish with subitems, in async mode
        /// </summary>
        /// <param name="item">Item to be published</param>
        /// <param name="includeSubitems">should subitems also be published</param>
        /// <returns>a bool value which indicates whether item published successfully or not.</returns>
        public static bool PublishItemAsync(this Item item, bool includeSubitems)
        {
            var sourceDatabase = Sitecore.Configuration.Factory.GetDatabase("master");
            var targetDatabase = Sitecore.Configuration.Factory.GetDatabase("web");
            return PublishItem(item, includeSubitems, sourceDatabase, targetDatabase, true);
        }

        /// <summary>
        /// Most general mothod for pubishing an item
        /// </summary>
        /// <param name="item">the item to publish</param>
        /// <param name="includeSubitems">should subitems also be published</param>
        /// <param name="sourceDatabase">from this database</param>
        /// <param name="targetDatabase">to this database</param>
        /// <param name="async">should the publish be async</param>
        /// <returns>a bool value which indicates whether item published successfully or not.</returns>
        public static bool PublishItem(this Item item, bool includeSubitems, Database sourceDatabase, Database targetDatabase, bool async)
        {
            bool bOk = false;
            try
            {
                if (item == null)
                {
                    Log.Info(String.Format("Genworth.SitecoreExt.Utilities.SitecorePublisher:PublishItem, item is null"), typeof(SitecorePublisher));
                }

                if (sourceDatabase == null)
                {
                    Log.Info(String.Format("Genworth.SitecoreExt.Utilities.SitecorePublisher:PublishItem, sourceDatabase is null"), typeof(SitecorePublisher));
                }

                if (targetDatabase == null)
                {
                    Log.Info(String.Format("Genworth.SitecoreExt.Utilities.SitecorePublisher:PublishItem, targetDatabase is null"), typeof(SitecorePublisher));
                }

                var options = new PublishOptions(sourceDatabase, targetDatabase, PublishMode.Full,
                                                 LanguageManager.DefaultLanguage, DateTime.Now);

                options.Deep = includeSubitems;

                options.CompareRevisions = true;
                options.RootItem = item;

                var publisher = new Publisher(options);
                if (!async)
                {
                    publisher.Publish();
                    bOk = true;
                }
                else
                {
                    publisher.PublishAsync();
                    bOk = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.SitecorePublisher:PublishItem, exception: {0}", ex.ToString()), ex, typeof(SitecorePublisher));
                bOk = false;
            }

            return bOk;
        }

        #endregion

        #region "Publish Site"
        /// <summary>
        /// This will use the Sitecore API to publish all pending changes in the content tree
        /// </summary>
        /// <returns></returns>
        public static bool PublishSite(Database sourceDatabase, Database targetDatabase)
        {
            bool bOk = false;

            try
            {
                Sitecore.Data.Database[] targetDatabases = { targetDatabase };
                Sitecore.Globalization.Language[] languages = sourceDatabase.Languages;

                Sitecore.Publishing.PublishManager.PublishIncremental(sourceDatabase, targetDatabases, languages);

                bOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.SitecorePublisher:PublishSite, exception: {0}", ex.ToString()), ex, typeof(SitecorePublisher));
            }

            return bOk;
        }
        #endregion

    }
}
