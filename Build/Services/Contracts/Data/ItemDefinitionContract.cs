using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Providers
{
	[DataContract]
	public class ItemDefinitionContract
	{
		[DataMember]
		public Guid Id;
		[DataMember]
		public string Name;
		[DataMember]
		public string TemplateId;
		[DataMember]
		public string BranchId;
		[DataMember]
		public string ParentId;
	}
}
