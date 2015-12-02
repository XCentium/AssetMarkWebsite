using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Eventing
{
	[DataContract]
	public class ProxyTimeStamp
	{
		[DataMember]
		private DateTime oDate;
		[DataMember]
		private long iSequence;

		public DateTime Date
		{
			get
			{
				return oDate;
			}
		}

		public long Sequence
		{
			get
			{
				return iSequence;
			}
		}

		private static ProxyTimeStamp oEmpty = new ProxyTimeStamp();
		public static ProxyTimeStamp Empty { get { return oEmpty; } }

		private ProxyTimeStamp() : this(DateTime.MinValue, long.MinValue) { }

		public ProxyTimeStamp(DateTime oDate, long iSequence)
		{
			this.oDate = oDate;
			this.iSequence = iSequence;
		}
	}
}
