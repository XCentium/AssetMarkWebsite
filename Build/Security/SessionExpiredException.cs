using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Security
{
	public class SessionExpiredException: Exception
	{
		public SessionExpiredException(string oMsg) :base(oMsg)	{	}
		public SessionExpiredException(string oMsg, Exception oInnerException):base(oMsg,oInnerException){	}
		public SessionExpiredException() : base() { }
	}
}
