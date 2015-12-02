using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sitecore.Data.DataProviders;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Sitecore.Data.DataProviders.SqlServer;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class BlobService : IBlobService
	{
		public MemoryStream GetBlobStream(Guid oGuid)
		{
			Stream oStream;
			MemoryStream oMemoryStream;

			//initialize memory stream to null
			oMemoryStream = null;

			//is the guid non-null?
			if (oGuid != null)
			{
				//log that we are getting the blob stream
				Sitecore.Diagnostics.Log.Debug(string.Format("Getting Blob Stream for Guid {0}", oGuid.ToString()));

				try
				{
					//get the stream
					oStream = ServerLogic.SitecoreExt.ContextExtension.CurrentDatabase.GetDataProviders().FirstOrDefault().GetBlobStream(oGuid, new CallContext(null, 0));

					//create a reader
					oMemoryStream = new MemoryStream(new BinaryReader(oStream).ReadBytes((int)oStream.Length));
				}
				catch (Exception oException)
				{
					//log the exception locally before rethrowing
					Sitecore.Diagnostics.Log.Error(string.Format("Error Getting Blob Stream with Guid {0}", oGuid.ToString()), oException, this);
					throw oException;
				}
			}
			else
			{
				//log that we received no guid for the blob
				Sitecore.Diagnostics.Log.Error("Error Getting Blob Stream. Guid was null.", this);
			}

			return oMemoryStream;
		}
	}
}
