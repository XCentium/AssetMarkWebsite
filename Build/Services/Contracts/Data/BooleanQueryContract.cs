using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Contracts.Data
{
	/// <summary>
	/// Contract that allows the serialization of the Lucene MultiFieldQuery
	/// </summary>
	[DataContract]
	public class BooleanQueryContract
	{
		[DataMember]
		public List<BooleanClauseContract> Clauses;
	}
}
