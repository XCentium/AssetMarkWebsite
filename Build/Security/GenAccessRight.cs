using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Sitecore.Security.AccessControl; 

namespace Genworth.SitecoreExt.Security
{
	class GenAccessRight : AccessRight
	{
		private Authorization oAuthorization;

		public GenAccessRight(string sName): base(sName)
		{ }


		public override void Initialize(NameValueCollection config)
		{
			base.Initialize(config);

			oAuthorization = Authorization.CurrentAuthorization;
		}
	}
}
