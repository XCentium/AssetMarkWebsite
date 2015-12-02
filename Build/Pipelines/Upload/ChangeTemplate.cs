using Sitecore.Pipelines.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.Pipelines.Upload
{
    public class ChangeTemplate
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<ChangedMediaTemplate> Templates { get; set; }

        public ChangeTemplate()
        {
            Templates = new List<ChangedMediaTemplate>();
        }

        public void Process(UploadArgs args)
        {
            var db = Sitecore.Context.ContentDatabase;

            var uploadPath = db.GetItem(args.Folder).Paths.ContentPath;
            if (!uploadPath.StartsWith(Path))
            {
                // Not uploading to designated folder
                return;
            }

            foreach (var item in args.UploadedItems)
            {
                Sitecore.Diagnostics.Log.Info(string.Format("UPLOAD Pipeline :: Item for Upload, Name: {0}, Path: {1}", item.Name, item.Paths.FullPath), this);
                // Need to change template for this item?
                Sitecore.Resources.Media.Media media = Sitecore.Resources.Media.MediaManager.GetMedia(item);
                string mimeType = string.Empty;
                if (media != null)
                {
                    Sitecore.Diagnostics.Log.Info(string.Format("UPLOAD Pipeline :: Item has a mediaItem, extension: {0}, MIME type: {1}", media.Extension, media.MimeType), this);
                    mimeType = media.MimeType;
                }

                var changedTemplate = Templates.Where(t => t.Old.Equals(item.Template.FullName) && t.MimeType.ToLower() == mimeType.ToLower()).FirstOrDefault();
                
                if (changedTemplate != null)
                {
                    var newTemplate = db.Templates[changedTemplate.New];
                    try
                    {
                        item.ChangeTemplate(newTemplate);
                        Sitecore.Diagnostics.Log.Info(string.Format("UPLOAD Pipeline :: Changed Template applied to item {0}: from [{1}] to [{2}].", item.Name, item.Template.FullName, newTemplate.FullName), this);
                    }
                    catch (Exception ex)
                    {
                        Sitecore.Diagnostics.Log.Error(string.Format("UPLOAD Pipeline :: Configured ChangeTemplate was unable to be applied on item {0}: original template [{1}], new template [{2}].", item.Name, item.Template.FullName, newTemplate.FullName), ex, this);
                    }
                }
            }
        }
    }
}
