using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Documents
{
	[DataContract]
	public class Document
	{
		[DataMember]
		public string Greeting;
	}
}
