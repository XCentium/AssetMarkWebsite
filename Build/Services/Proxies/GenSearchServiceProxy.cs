using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Genworth.SitecoreExt.Services.Contracts.Data;

namespace Genworth.SitecoreExt.Services.Search
{
    class GenSearchServiceProxy : ClientBase<IGenSearchService>, IGenSearchService, IDisposable
    {

        public List<Lucene.Net.Documents.Document> ExecuteQuery(Lucene.Net.Search.Query query, string indexName)
        {
            return base.Channel.ExecuteQuery(query, indexName);
        }


        public List<Lucene.Net.Documents.Document> ExecuteBooleanQuery(BooleanQueryContract query, string indexName)
        {
            return base.Channel.ExecuteBooleanQuery(query, indexName);
        }

        public Dictionary<string, int> ExecuteGroupCountQuery(Lucene.Net.Search.Query query, string indexName, string sfield)
        {
            return base.Channel.ExecuteGroupCountQuery(query, indexName, sfield);
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
