using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Genworth.SitecoreExt.Services.Providers
{
    class GenSqlServerClientDataStoreServiceProxy : ClientBase<IGenSqlServerClientDataStoreService>, IGenSqlServerClientDataStoreService, IDisposable
    {

        public void CompactDataOperation()
        {
            base.Channel.CompactDataOperation();
        }

        public string LoadDataOperation(string sKey)
        {
            return base.Channel.LoadDataOperation(sKey);
        }

        public void RemoveDataOperation(string sKey)
        {
            base.Channel.RemoveDataOperation(sKey);
        }

        public void SaveDataOperation(string sKey, string sData)
        {
            base.Channel.SaveDataOperation(sKey, sData);
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
