using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Providers
{
	/// <summary>
	/// Contract that allows the serialization of Item Version (This contract allows the serialization of the Version Sitecore Type)
	/// </summary>
	[DataContract]
	public class VersionContract
	{
		[DataMember]
		public int Number;		
	}	

}
