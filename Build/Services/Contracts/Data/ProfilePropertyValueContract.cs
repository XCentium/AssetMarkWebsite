using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Providers
{
	/// <summary>
	/// Contract that allows the serialization of System.Configuration.SettingsPropertyValue
	/// </summary>
	[DataContract]
	public class ProfilePropertyValueContract
	{
		[DataMember]
		public string Value;
		[DataMember]
		public ProfilePropertyContract ProfileProperty;
	}
}
