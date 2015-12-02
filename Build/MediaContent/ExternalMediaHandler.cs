using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using System.IO;

namespace Genworth.SitecoreExt.MediaContent
{
    public class ExternalMediaHandler : IHttpHandler
    {

        private const string databaseName = "web";
        private const string externalContentPath = "/sitecore/media library/External Content/";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    string id = context.Request.QueryString["id"];
                    string newId = FormatId(id);

                    Database db = Factory.GetDatabase(databaseName);
                    Item item = db.GetItem(newId);

                    if (item != null)
                    {
                        if (item.Paths.FullPath.Contains(externalContentPath))
                        {
                            var media = MediaManager.GetMedia(item);
                            if (media != null)
                            {
                                MediaStream mediaStream = media.GetStream();

                                context.Response.ContentType = mediaStream.MimeType;
                                context.Response.Buffer = false;

                                long len = mediaStream.Length, bytes;
                                context.Response.AppendHeader("content-length", len.ToString());

                                byte[] buffer = new byte[1024];
                                Stream outStream = context.Response.OutputStream;

                                using (Stream stream = mediaStream.Stream)
                                {
                                    while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        outStream.Write(buffer, 0, (int)bytes);
                                        len -= bytes;
                                    }
                                }
                            }
                            else
                            {
                                Sitecore.Diagnostics.Log.Error("Unable to get External Media item because is not a media item.", this);
                            }
                        }
                        else
                        {
                            Sitecore.Diagnostics.Log.Error("Unable to get External Media item because is not at 'External Content' folder.", this);
                        }
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error("Unable to get External Media item because it does not exist.", this);
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get External Media item", ex, this);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string FormatId(string id)
        {
            string newId = string.Empty;

            if (id.Length > 0 && id.Length > 30)
            {
                string dash = "-";
                newId = id.Insert(8, dash).Insert(13, dash).Insert(18, dash).Insert(23, dash);
            }

            return newId;
        }

    }
}
