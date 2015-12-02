using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Content
{
	/// <summary>
	/// Contract that allows the serialization of Help Items 
	/// </summary>
	[DataContract]
	public class HelpTextContract
	{
		/// <summary>
		/// Help text title
		/// </summary>
		[DataMember]
		public string Title;
		/// <summary>
		/// the actual help text
		/// </summary>
		[DataMember]
		public string Text;
		/// <summary>
		/// URL to get the help text icon
		/// </summary>
		[DataMember]
		public string IconURL;
		/// <summary>
		/// This selector contains the control id that will be used to set the text in the appropiated control on the UI side
		/// </summary>
		[DataMember]
		public string FieldSelector;
	}
}
