using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Sitecore.Data.Items;

using ServerLogic.SitecoreExt;
using Sitecore.Resources.Media;
using System.ServiceModel.Activation;

namespace Genworth.SitecoreExt.Services.Documents
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DocumentService : IDocumentService
	{
		List<Document> oDocuments = new List<Document>();

		public void SubmitGreeting(Document oDocument)
		{
			oDocuments.Add(oDocument);
		}

		public List<Document> GetGreetings()
		{
			return oDocuments;
		}

		public List<KeyValuePair<string, string>> GetItemById(string Ciid)
		{
			return GetItemFieldValues(ContextExtension.CurrentDatabase.GetItem(Ciid));
		}

		public List<KeyValuePair<string, string>> GetItemByPath(string Path)
		{
			return GetItemFieldValues(ContextExtension.CurrentDatabase.SelectSingleItem(Path));
		}

		private List<KeyValuePair<string, string>> GetItemFieldValues(Item oItem)
		{
			List<KeyValuePair<string, string>> oDocument;

			if (oItem != null)
			{
				//create the document as a new object
				oDocument = new List<KeyValuePair<string, string>>();

				//read all of the fields
				oItem.Fields.ReadAll();

				//now we can find the field we care about
				oItem.Fields.ToList().ForEach(oField =>
				{
					//add the field info to the document
					oDocument.Add(new KeyValuePair<string, string>(oField.Name, oField.Value));
				});
			}
			else
			{
				//intialize the new document as null
				oDocument = null;
			}

			//return the document
			return oDocument;
		}

		public Stream GetFileById(string Ciid)
		{
			return GetFileStream(ContextExtension.CurrentDatabase.GetItem(Ciid));
		}

		public Stream GetFileByPath(string Path)
		{
			return GetFileStream(ContextExtension.CurrentDatabase.SelectSingleItem(Path));
		}


        public Stream GetImageBySSO(string sKey)
        {
            StringBuilder oQueryBuilder;
            string sQuery;
            Item oItem;

            string sDecryptedSSO = Encoding.UTF8.GetString(Convert.FromBase64String(sKey));

            oQueryBuilder = new StringBuilder("fast:/sitecore/media library//*[@@name = '")
                                                                  .Append(sDecryptedSSO)
                                                                  .Append("']");

            sQuery = oQueryBuilder.ToString();


            oItem = ContextExtension.CurrentDatabase.SelectSingleItem(sQuery);

            return GetFileStream(oItem);
        }

        public String GetImageUriBySSO(string sKey, int iWidth, int iHeight)
        {
            StringBuilder oQueryBuilder;
            string sQuery;
            Item oItem;

//#if DEBUG
//            string sDecryptedSSO = sKey;
//#else
            string sDecryptedSSO = Encoding.UTF8.GetString(Convert.FromBase64String(sKey));
//#endif

            oQueryBuilder = new StringBuilder("fast:/sitecore/media library//*[@@name = '")
                                                                  .Append(sDecryptedSSO)
                                                                  .Append("']");

            sQuery = oQueryBuilder.ToString();


            oItem = ContextExtension.CurrentDatabase.SelectSingleItem(sQuery);

            return GetImageUri(oItem, iWidth, iHeight);
        }

        public Stream GetImageByPath(string sPath)
        {
            StringBuilder oQueryBuilder;
            string sQuery;
            Item oItem;

            //We need to decr
            string sDecryptedPath = Encoding.UTF8.GetString(Convert.FromBase64String(sPath));

            if (sPath.ElementAt(0) == '/')
            {
                oQueryBuilder = new StringBuilder("fast:").Append(sDecryptedPath);                                                                      
            }
            else
            {
                oQueryBuilder = new StringBuilder("fast:/").Append(sDecryptedPath);
            }

            sQuery = oQueryBuilder.ToString();


            oItem = ContextExtension.CurrentDatabase.SelectSingleItem(sQuery);

            return GetFileStream(oItem);
        }

		private Stream GetFileStream(Item oItem)
		{
			Stream oStream;
			MediaStream oMediaStream;

			oStream = null;

			if (oItem != null)
			{
				if ((oMediaStream = MediaManager.GetMedia(oItem).GetStream()) != null)
				{
					oStream = oMediaStream.Stream;
				}
			}

			return oStream;
		}

        private string GetImageUri(Item oItem, int iWidth, int iHeight)
        {            
            MediaUrlOptions oMediaUrlOptions;
            string sMediaUrl;

            sMediaUrl = string.Empty;            

            if (oItem != null)
            {
                if (iWidth > 0 && iHeight > 0)
                {
                    oMediaUrlOptions = new MediaUrlOptions();                    
                    oMediaUrlOptions.Width = iWidth;
                    oMediaUrlOptions.Height = iHeight;
                    oMediaUrlOptions.UseItemPath = false;
                    sMediaUrl = MediaManager.GetMediaUrl(oItem, oMediaUrlOptions);
                }
                else
                {
                    sMediaUrl = MediaManager.GetMediaUrl(oItem);
                }
            }

            return sMediaUrl;
        }
	}

}
