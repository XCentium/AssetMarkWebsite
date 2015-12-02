using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Genworth.SitecoreExt.Services.Providers
{
	/// <summary>
	/// Contract that allows the serialization of System.Configuration.SettingsProperty
	/// </summary>
	[DataContract]
	public class ProfilePropertyContract
	{
		[DataMember]
		public string Name;
		[DataMember]
		public string Type;
		[DataMember]
		public bool IsReadOnly;
		[DataMember]
		public string DefaultValue;
		[DataMember]
		public List<ProfilePropertyAttributeContract> Attributes;
	}
}
