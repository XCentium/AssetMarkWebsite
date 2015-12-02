using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;


namespace Genworth.SitecoreExt.Services.Content
{
	[ServiceContract]
	public interface IGenHelpContentService
	{
		[OperationContract]
		List<HelpTextContract> GetHelpContentByItemId(string itemId);

		[OperationContract]
		List<HelpTextContract> GetHelpContentByItemName (string itemName);

		[OperationContract]
		List<HelpTextContract> GetHelpContentByItemPath(string itemPath);

	}
}
