using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sitecore.Eventing;

namespace Genworth.SitecoreExt.Services.Eventing
{
	[DataContract]
	public class ProxyEventQueueQuery
	{
		[DataMember]
		private Type oEventType;
		[DataMember]
		private long? iFromTimestamp;
		[DataMember]
		private DateTime? dFromUtcDate;
		[DataMember]
		private Type oInstanceType;
		[DataMember]
		private string sSourceInstanceName;
		[DataMember]
		private string sTargetInstanceName;
		[DataMember]
		private long? iToTimestamp;
		[DataMember]
		private DateTime? dToUtcDate;
		[DataMember]
		private string sUsername;

		public ProxyEventQueueQuery(EventQueueQuery oEventQueueQuery)
		{
			oEventType = oEventQueueQuery.EventType;
			iFromTimestamp = oEventQueueQuery.FromTimestamp;
			dFromUtcDate = oEventQueueQuery.FromUtcDate;
			oInstanceType = oEventQueueQuery.InstanceType;
			sSourceInstanceName = oEventQueueQuery.SourceInstanceName;
			sTargetInstanceName = oEventQueueQuery.TargetInstanceName;
			iToTimestamp = oEventQueueQuery.ToTimestamp;
			dToUtcDate = oEventQueueQuery.ToUtcDate;
			sUsername = oEventQueueQuery.UserName;
		}

		public EventQueueQuery GetEventQueueQuery()
		{
			EventQueueQuery oEventQueueQuery;

			oEventQueueQuery = new EventQueueQuery();
			oEventQueueQuery.EventType = oEventType;
			oEventQueueQuery.FromTimestamp = iFromTimestamp;
			oEventQueueQuery.FromUtcDate = dFromUtcDate;
			oEventQueueQuery.InstanceType = oInstanceType;
			oEventQueueQuery.SourceInstanceName = sSourceInstanceName;
			oEventQueueQuery.TargetInstanceName = sTargetInstanceName;
			oEventQueueQuery.ToTimestamp = iToTimestamp;
			oEventQueueQuery.ToUtcDate = dToUtcDate;
			oEventQueueQuery.UserName = sUsername;

			return oEventQueueQuery;
		}
	}
}
