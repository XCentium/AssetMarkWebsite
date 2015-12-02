using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Eventing;
using Genworth.SitecoreExt.Services.Eventing;

namespace Genworth.SitecoreExt.CustomDataProvider
{
    public class RemoteClientEventQueue : EventQueue
    {
        public RemoteClientEventQueue() : base(new Sitecore.Common.Serializer()) { }

        public override void Cleanup(uint daysToKeep)
        {
            using (var proxy = new GenEventingServiceProxy())
            {
                proxy.Cleanup(daysToKeep);
            }
        }

        public override long GetQueuedEventCount()
        {
            long count = 0;
            using (var proxy = new GenEventingServiceProxy())
            {
                count = proxy.GetQueuedEventCount();
            }
            return count;
        }

        public override IEnumerable<QueuedEvent> GetQueuedEvents(EventQueueQuery query)
        {
            IEnumerable<QueuedEvent> oQueuedEvents;
            using (var proxy = new GenEventingServiceProxy())
            {
                oQueuedEvents = proxy.GetQueuedEvents(new ProxyEventQueueQuery(query)).Select(oProxyQueuedEvent => oProxyQueuedEvent.GetQueuedEvent());
            }
            Sitecore.Diagnostics.Log.Debug(string.Format("Genworth.SitecoreExt.CustomDataProvider.RemoteClientEventQueue Received [{0}] Events.", oQueuedEvents.Count()), this);
            return oQueuedEvents;
        }

        protected override EventQueue.TimeStamp GetTimestampForLastProcessing()
        {
            ProxyTimeStamp oTimeStamp;
            using (var proxy = new GenEventingServiceProxy())
            {
                oTimeStamp = proxy.GetTimestampForLastProcessing();
            }
            return new EventQueue.TimeStamp(oTimeStamp.Date, oTimeStamp.Sequence);
        }

        protected override void QueueEvent(QueuedEvent queuedEvent, bool addToGlobalQueue, bool addToLocalQueue)
        {
            using (var proxy = new GenEventingServiceProxy())
            {
                proxy.QueueEvent(new ProxyQueuedEvent(queuedEvent), addToGlobalQueue, addToLocalQueue);
            }
        }

        public override void RemoveQueuedEvents(EventQueueQuery query)
        {
            using (var proxy = new GenEventingServiceProxy())
            {
                proxy.RemoveQueuedEvents(new ProxyEventQueueQuery(query));
            }
        }

        protected override void SetTimestampForLastProcessing(EventQueue.TimeStamp timestamp)
        {
            using (var proxy = new GenEventingServiceProxy())
            {
                proxy.SetTimestampForLastProcessing(new ProxyTimeStamp(timestamp.Date, timestamp.Sequence));
            }
        }

        public override QueuedEvent GetLastEvent()
        {
            // adding per abstract class update in newer Sitecore.Kernel dll
            // not required for Sitecore 7.2 upgrade, just for making the project compilable
            throw new NotImplementedException();
        }
    }
}
