using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Providers
{

	/// <summary>
	/// Contract that allows the serialization of the Sitecore Language type
	/// </summary>
	[DataContract]
	public class LanguageContract
	{
		[DataMember]
		public string Name;

		[DataMember]
		public string OriginItemId;
		
	}

}
