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
using Sitecore.Data;
using Sitecore.Resources.Media;
using Genworth.SitecoreExt.Helpers;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;

namespace Genworth.SitecoreExt.Search.ComputedFields
{
    public class FileMIMEType : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string mimeType = null;

            Sitecore.Data.Fields.FileField fileField = item.GetField(Constants.Documents.Templates.DocumentBase.Sections.Document.Name, Constants.Documents.Templates.DocumentBase.Sections.Document.Fields.File);

            if (fileField != null && fileField.MediaItem != null) 
            {
                Media mediaItem = MediaManager.GetMedia(fileField.MediaItem);
                mimeType = mediaItem != null ? mediaItem.MimeType : null;
            }

            return mimeType;
        } 
    }
}




