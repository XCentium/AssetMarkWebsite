using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assert = Sitecore.Diagnostics.Assert;
using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;

using SC = Sitecore;
using Sitecore.Links;
using ServerLogic.SitecoreExt;

namespace Genworth.SitecoreExt.Search.ComputedFields
{
    public class GlossaryPrefix : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string prefixText = null;

            var termField = item.GetField(Constants.HelpCenter.Templates.GlossaryTerm.Sections.GlossaryTerm.Name, Constants.HelpCenter.Templates.GlossaryTerm.Sections.GlossaryTerm.Fields.TermFieldName);

            if (termField != null && !string.IsNullOrEmpty(termField.Value))
            {
                string sPrefix = termField.Value.Trim().ElementAtOrDefault(0).ToString();

                if (!string.IsNullOrEmpty(sPrefix))
                {
                    prefixText = string.Format("{0}{1}", Constants.HelpCenter.Indexes.GlossaryIndex.StartsWith, sPrefix.ToUpper());
                }
            }

            return prefixText;
        } 
    }
}



