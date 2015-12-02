using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerLogic.WCF;
using ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server;
using System.ServiceModel;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client
{
    public class SqlServerDataProvider : Sitecore.Data.DataProviders.Sql.SqlDataProvider
    {

        private const int iDefaultMaxNumberOfAttempts = 3;

        private int MaxNumberOfAttempts
        {

            get
            {

                int iMaxNumberOfAttempts;

                if (!int.TryParse(Sitecore.Configuration.Settings.GetSetting("ServerLogic.SitecoreExtension.Data.Remote.MaxNumberOfConnectionsAttempts", String.Empty), out iMaxNumberOfAttempts))
                {

                    iMaxNumberOfAttempts = iDefaultMaxNumberOfAttempts;

                }

                return iMaxNumberOfAttempts;

            }

        }

        public SqlServerDataProvider(string sConnectionString)

            : base(new SqlServerDataApi(sConnectionString))
        {

            //create the blob factory

        }

        public override Stream GetBlobStream(Guid oGuid, Sitecore.Data.DataProviders.CallContext context)
        {

            Stream oStream;

            int iMaxNumberOfAttempts;

            int iCurrentAttempt;



            //ResilientChannelFactory<IBlobService> oBlobFactory;

            iMaxNumberOfAttempts = MaxNumberOfAttempts;

            iCurrentAttempt = 0;

            oStream = null;

            while (iCurrentAttempt < iMaxNumberOfAttempts)
            {

                try
                {

                    using (var oBlobFactory = new ResilientChannelFactory<IBlobService>("IBlobService"))
                    {
                        oStream = oBlobFactory.Service.GetBlobStream(oGuid);
                    }

                    break;

                }

                catch (Exception oGetBlobStreamException)
                {

                    iCurrentAttempt++;

                    if (iCurrentAttempt == iMaxNumberOfAttempts)
                    {

                        Sitecore.Diagnostics.Log.Error(string.Format("After {0} attempts the remote provider was unable to get BLOB with ID {1}", iMaxNumberOfAttempts, oGuid.ToString()), oGetBlobStreamException, this);

                    }

                    else
                    {

                        Sitecore.Diagnostics.Log.Error(string.Format("{0} Attempt the remote provider was unable to get BLOB with ID {1}", iCurrentAttempt, oGuid.ToString()), oGetBlobStreamException, this);

                    }

                }

            }

            return oStream;

        }

    }
}
