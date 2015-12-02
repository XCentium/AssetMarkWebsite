using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Eventing;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Eventing
{
	[DataContract]
	public class ProxyQueuedEvent
	{
		[DataMember]
		private DateTime dCreated;
		[DataMember]
		private string sEventTypeName;
		[DataMember]
		private string sInstanceData;
		[DataMember]
		private string sInstanceName;
		[DataMember]
		private string sInstanceTypeName;
		[DataMember]
		private long iTimestamp;
		[DataMember]
		private string sUsername;

		public ProxyQueuedEvent(QueuedEvent oQueuedEvent)
		{
			dCreated = oQueuedEvent.Created;
			sEventTypeName = oQueuedEvent.EventTypeName;
			sInstanceData = oQueuedEvent.InstanceData;
			sInstanceName = oQueuedEvent.InstanceName;
			sInstanceTypeName = oQueuedEvent.InstanceTypeName;
			iTimestamp = oQueuedEvent.Timestamp;
			sUsername = oQueuedEvent.UserName;
		}

		public QueuedEvent GetQueuedEvent()
		{
			return new QueuedEvent(sEventTypeName, sInstanceTypeName, sInstanceData, sInstanceName, sUsername, iTimestamp, dCreated);
		}
	}
}
