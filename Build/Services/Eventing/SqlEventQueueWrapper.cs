using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Eventing;
using Sitecore.Data;
using Sitecore.Data.SqlServer;
using System.Configuration;
using System.Runtime.Serialization;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Eventing;
using System.Data;

namespace Genworth.SitecoreExt.Services.Eventing
{
	public class SqlEventQueueWrapper : SqlEventQueue
	{
		public SqlEventQueueWrapper(Database oDatabase)
			: base(new SqlServerDataApi(ConfigurationManager.ConnectionStrings[oDatabase.ConnectionStringName].ConnectionString), oDatabase)
		{
		}

		public ProxyTimeStamp GetProxyTimestampForLastProcessing()
		{
			TimeStamp oTimeStamp;
			oTimeStamp = GetTimestampForLastProcessing();
			return new ProxyTimeStamp(oTimeStamp.Date, oTimeStamp.Sequence);
		}

		protected override TimeStamp GetTimestampForLastProcessing()
		{
			return base.GetTimestampForLastProcessing();
		}

		public void SetProxyTimestampForLastProcessing(ProxyTimeStamp dTimeStamp)
		{
			SetTimestampForLastProcessing(new TimeStamp(dTimeStamp.Date, dTimeStamp.Sequence));
		}

		protected override void SetTimestampForLastProcessing(TimeStamp dTimeStamp)
		{
			base.SetTimestampForLastProcessing(dTimeStamp);
		}

		public override IEnumerable<QueuedEvent> GetQueuedEvents(Sitecore.Eventing.EventQueueQuery query)
		{
			return base.GetQueuedEvents(query);
		}
	}
}
