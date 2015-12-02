using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Providers
{
	/// <summary>
	/// Contract that allows the serialization of System.Configuration.SettingsAttribute
	/// </summary>
	[DataContract]
	public class ProfilePropertyAttributeContract
	{
		[DataMember]
		public string Key;
		[DataMember]
		public string Value;
	}
}
