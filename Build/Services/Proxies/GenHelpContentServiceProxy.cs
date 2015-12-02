using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Genworth.SitecoreExt.Services.Contracts.Data;

namespace Genworth.SitecoreExt.Services.Content
{
    public class GenHelpContentServiceProxy : ClientBase<IGenHelpContentService>, IGenHelpContentService, IDisposable
    {
        public List<HelpTextContract> GetHelpContentByItemId(string itemId)
        {
            return base.Channel.GetHelpContentByItemId(itemId);
        }

        public List<HelpTextContract> GetHelpContentByItemName(string itemName)
        {
            return base.Channel.GetHelpContentByItemName(itemName);
        }


        public List<HelpTextContract> GetHelpContentByItemPath(string itemPath)
        {
            return base.Channel.GetHelpContentByItemPath(itemPath);
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
