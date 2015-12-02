using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Eventing;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Eventing;
using Sitecore.Data.DataProviders;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.SqlServer;
using System.Configuration;

namespace Genworth.SitecoreExt.Services.Eventing
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class GenEventingService : IGenEventingService
	{
		private EventQueue oQueue;
		public EventQueue Queue
		{
			get
			{
				EventQueue oTemp;
				//if we do not have a queue yet, find it
				if (oQueue == null)
				{
					//search through database providers for queue
					foreach (Database oDatabase in Factory.GetDatabases())
					{
						foreach (DataProvider oProvider in oDatabase.GetDataProviders())
						{
							if ((oTemp = oProvider.GetEventQueue()) != null && oTemp is SqlEventQueue)
							{
								oQueue = new SqlEventQueueWrapper(oDatabase);
								break;
							}
						}
						//we only need one sql event queue wrapper
						if (oQueue != null)
						{
							break;
						}
					}
				}
				return oQueue ?? (oQueue = NullEventQueue.Instance);
			}
		}

		public void Cleanup(uint daysToKeep)
		{
			Queue.Cleanup(daysToKeep);
		}

		public long GetQueuedEventCount()
		{
			return Queue.GetQueuedEventCount();
		}

		public IEnumerable<ProxyQueuedEvent> GetQueuedEvents(ProxyEventQueueQuery query)
		{
			return Queue.GetQueuedEvents(query.GetEventQueueQuery()).Select(oQueuedEvent => new ProxyQueuedEvent(oQueuedEvent));
		}

		public ProxyTimeStamp GetTimestampForLastProcessing()
		{
			EventQueue oProxyQueue;
			ProxyTimeStamp oTimeStamp;

			if ((oProxyQueue = Queue) != null && oProxyQueue is SqlEventQueueWrapper)
			{
				oTimeStamp = ((SqlEventQueueWrapper)oProxyQueue).GetProxyTimestampForLastProcessing();
			}
			else
			{
				oTimeStamp = ProxyTimeStamp.Empty;
			}

			//return the time stamp
			return oTimeStamp;
		}

		public void QueueEvent(ProxyQueuedEvent oQueuedEvent, bool bAddToGlobalQueue, bool bAddToLocalQueue)
		{
			Queue.QueueEvent(oQueuedEvent, bAddToGlobalQueue, bAddToLocalQueue);
		}

		public void RemoveQueuedEvents(ProxyEventQueueQuery query)
		{
			Queue.RemoveQueuedEvents(query.GetEventQueueQuery());
		}

		public void SetTimestampForLastProcessing(ProxyTimeStamp oTimeStamp)
		{
			EventQueue oProxyQueue;

			if ((oProxyQueue = Queue) != null && oProxyQueue is SqlEventQueueWrapper)
			{
				((SqlEventQueueWrapper)oProxyQueue).SetProxyTimestampForLastProcessing(oTimeStamp);
			}
		}
	}
}
