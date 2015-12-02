using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Contracts.Data
{
	/// <summary>
	/// Contract that allows the serialization of the Lucene query clause
	/// </summary>
	[DataContract]
	public class BooleanClauseContract
	{
		[DataMember]
		public MultiFieldQueryContract MultiFieldQuery;

		[DataMember]
		public String Occur;
	}
}
