using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Genworth.SitecoreExt.Services.Eventing
{
    public class GenEventingServiceProxy : ClientBase<IGenEventingService>, IGenEventingService, IDisposable
    {
        public void Cleanup(uint daysToKeep)
        {
            base.Channel.Cleanup(daysToKeep);
        }

        public long GetQueuedEventCount()
        {
            return base.Channel.GetQueuedEventCount();
        }

        public IEnumerable<ProxyQueuedEvent> GetQueuedEvents(ProxyEventQueueQuery query)
        {
            return base.Channel.GetQueuedEvents(query);
        }

        public ProxyTimeStamp GetTimestampForLastProcessing()
        {
            return base.Channel.GetTimestampForLastProcessing();
        }

        public void QueueEvent(ProxyQueuedEvent oQueuedEvent, bool bAddToGlobalQueue, bool bAddToLocalQueue) 
        {
            base.Channel.QueueEvent(oQueuedEvent, bAddToGlobalQueue, bAddToLocalQueue);
        }

        public void RemoveQueuedEvents(ProxyEventQueueQuery query)
        {
            base.Channel.RemoveQueuedEvents(query);
        }

        public void SetTimestampForLastProcessing(ProxyTimeStamp oTimeStamp)
        {
            base.Channel.SetTimestampForLastProcessing(oTimeStamp);
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
