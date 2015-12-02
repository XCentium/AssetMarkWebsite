using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml.Linq;



namespace Genworth.SitecoreExt.Services.Providers
{
    public class GenContentServiceProxy : ClientBase<IGenContentService>, IGenContentService, IDisposable
    {
        public GenContentServiceProxy()
        {
        }

        public List<ItemDefinitionContract> GetItemDefinitions(List<string> oItemsIds, string sDatabaseName)
        {
            return base.Channel.GetItemDefinitions(oItemsIds, sDatabaseName);
        }

        public ItemDefinitionContract GetItemDefinition(string sItemId, string sDatabaseName)
        {
            return base.Channel.GetItemDefinition(sItemId, sDatabaseName);
        }

        public List<FieldContract> GetItemFields(string sItemId, string sLanguageName, int iItemVersionNumber, string sDatabaseName)
        {
            return base.Channel.GetItemFields(sItemId, sLanguageName, iItemVersionNumber, sDatabaseName);
        }

        public Dictionary<string, List<FieldContract>> GetItemsFields(List<string> oItemsIds, string sDatabaseName)
        {
            return base.Channel.GetItemsFields(oItemsIds, sDatabaseName);
        }
        public List<VersionUriContract> GetItemVersions(string sItemId, string sDatabaseName)
        {
            return base.Channel.GetItemVersions(sItemId, sDatabaseName);
        }

        public List<Guid> GetChildIDs(string sItemId, string sDatabaseName)
        {
            return base.Channel.GetChildIDs(sItemId, sDatabaseName);
        }

        public Dictionary<Guid, List<Guid>> GetItemsChildIDs(List<string> oItemIds, string sDatabaseName)
        {
            return base.Channel.GetItemsChildIDs(oItemIds, sDatabaseName);
        }
        public Guid GetParentID(string sItemId, string sDatabaseName)
        {
            return base.Channel.GetParentID(sItemId, sDatabaseName);
        }

        public Guid GetRootID(string sDatabaseName)
        {
            return base.Channel.GetRootID(sDatabaseName);
        }

        public List<Guid> GetTemplateIDs(string sDatabaseName)
        {
            return base.Channel.GetTemplateIDs(sDatabaseName);
        }

        public Guid ResolvePath(string sItemPath, string sDatabaseName)
        {
            return base.Channel.ResolvePath(sItemPath, sDatabaseName);
        }

        public Guid SelectSingleID(string sQuery, string sDatabaseName)
        {
            return base.Channel.SelectSingleID(sQuery, sDatabaseName);
        }

        public List<Guid> SelectIDs(string sQuery, string sDatabaseName)
        {
            return base.Channel.SelectIDs(sQuery, sDatabaseName);
        }

        public List<LanguageContract> GetLanguages(string sDatabaseName)
        {
            return base.Channel.GetLanguages(sDatabaseName);
        }


        public System.IO.MemoryStream GetBlobStream(Guid oBlobId, string sDatabaseName)
        {
            return base.Channel.GetBlobStream(oBlobId, sDatabaseName);
        }

        public bool BlobStreamExists(Guid oBlobId, string sDatabaseName)
        {
            return base.Channel.BlobStreamExists(oBlobId, sDatabaseName);
        }

        /// <summary>
        /// IDisposable.Dispose implementation, calls Dispose(true).
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose worker method. Handles graceful shutdown of the
        /// client even if it is an faulted state.
        /// </summary>
        /// <param name="disposing">Are we disposing (alternative
        /// is to be finalizing)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (State != CommunicationState.Faulted)
                    {
                        Close();
                    }
                }
                finally
                {
                    if (State != CommunicationState.Closed)
                    {
                        Abort();
                    }
                }
            }
        }

    }
}
