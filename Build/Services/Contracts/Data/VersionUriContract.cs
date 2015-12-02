using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Providers
{
	/// <summary>
	/// Contract that allows the serialization of VersionUri Sitecore Type
	/// </summary>
	[DataContract]
	public class VersionUriContract
	{
		[DataMember]
		public VersionContract Version;
		[DataMember]
		public LanguageContract Language;
	}
}
