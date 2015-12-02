using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Sitecore.Eventing;

namespace Genworth.SitecoreExt.Services.Eventing
{
	[ServiceContract]
	public interface IGenEventingService
	{
		[OperationContract]
		void Cleanup(uint daysToKeep);

		[OperationContract]
		long GetQueuedEventCount();

		[OperationContract]
		IEnumerable<ProxyQueuedEvent> GetQueuedEvents(ProxyEventQueueQuery query);

		[OperationContract]
		ProxyTimeStamp GetTimestampForLastProcessing();

		[OperationContract]
		void QueueEvent(ProxyQueuedEvent oQueuedEvent, bool bAddToGlobalQueue, bool bAddToLocalQueue);

		[OperationContract]
		void RemoveQueuedEvents(ProxyEventQueueQuery query);

		[OperationContract]
		void SetTimestampForLastProcessing(ProxyTimeStamp oTimeStamp);
	}
}