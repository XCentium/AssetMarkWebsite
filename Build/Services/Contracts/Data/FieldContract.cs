using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Providers
{
	/// <summary>
	/// Contract that allows the serialization of Item Fields
	/// </summary>
	[DataContract]
	public class FieldContract
	{
		[DataMember]
		public string Id;
		[DataMember]
		public string Value;
        [DataMember]
        public string Language;
        [DataMember]
        public int Version;
	}
}
