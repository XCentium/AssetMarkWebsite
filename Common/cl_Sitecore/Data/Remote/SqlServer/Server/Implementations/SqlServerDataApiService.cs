using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Diagnostics;
using System.Data;
using Sitecore.Data.DataProviders.Sql;
using System.Threading;
using ServerLogic.LinqExt;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SqlServerDataApiService : Sitecore.Data.SqlServer.SqlServerDataApi, ISqlServerDataApiService
    {
        private static LockableSortedDictionary<Guid, GenericTimeableObject<IDbConnection>> oConnections = new LockableSortedDictionary<Guid, GenericTimeableObject<IDbConnection>>();
        private static LockableSortedDictionary<Guid, GenericTimeableObject<DataProviderCommand>> oCommands = new LockableSortedDictionary<Guid, GenericTimeableObject<DataProviderCommand>>();
        private static LockableSortedDictionary<Guid, GenericTimeableObject<IDataReader>> oReaders = new LockableSortedDictionary<Guid, GenericTimeableObject<IDataReader>>();
        private static LockableSortedDictionary<Guid, GenericTimeableObject<DataProviderTransaction>> oTransactions = new LockableSortedDictionary<Guid, GenericTimeableObject<DataProviderTransaction>>();
        private static object oStaticLogOwner = new object();
        private static Regex oConnectionStringRegex = new Regex(".*?data source=([^;]+);.*?", RegexOptions.IgnoreCase);
        private static long lMaxMillsec;
        private const long LDefaultMaxMillsec = 120000; //1 min
        private const int iRecyclingTimeConstant = 60000; //1 min
        private static Timer oTimer;

        #region LOCK TIME MANAGEMENT

        private static readonly int DEFAULT_LOCKABLE_LIST_TIMEOUT_MILLISECONDS = 1000;
        private static int iLockableListTimeoutMilliseconds;
        private static int LockableListTimeoutMilliseconds
        {
            get
            {
                if (iLockableListTimeoutMilliseconds == 0)
                {
                    if (!int.TryParse(Sitecore.Configuration.Settings.GetSetting("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.LockableListTimeoutMilliseconds", string.Empty), out iLockableListTimeoutMilliseconds) || iLockableListTimeoutMilliseconds == 0)
                    {
                        iLockableListTimeoutMilliseconds = DEFAULT_LOCKABLE_LIST_TIMEOUT_MILLISECONDS;
                    }
                }
                return iLockableListTimeoutMilliseconds;
            }
        }

        #endregion
        static SqlServerDataApiService()
        {
            int iRecyclingTime;
            if (!long.TryParse(Sitecore.Configuration.Settings.GetSetting("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.ConectionExpirationTime", string.Empty), out lMaxMillsec))
            {
                lMaxMillsec = LDefaultMaxMillsec;
            }
            if (!int.TryParse(Sitecore.Configuration.Settings.GetSetting("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.RecyclingTime", string.Empty), out iRecyclingTime))
            {
                iRecyclingTime = iRecyclingTimeConstant;
            }

            oTimer = new Timer(new TimerCallback(ReleasedTimedOutObjects), null, 0, iRecyclingTime);


        }
        

        public static void ReleasedTimedOutObjects(object oObj)
        {

            LockableSortedDictionary<Guid, GenericTimeableObject<IDbConnection>> oDisposeConnections = new LockableSortedDictionary<Guid, GenericTimeableObject<IDbConnection>>();
            LockableSortedDictionary<Guid, GenericTimeableObject<DataProviderCommand>> oDisposeCommands = new LockableSortedDictionary<Guid, GenericTimeableObject<DataProviderCommand>>();
            LockableSortedDictionary<Guid, GenericTimeableObject<IDataReader>> oDisposeReaders = new LockableSortedDictionary<Guid, GenericTimeableObject<IDataReader>>();
            LockableSortedDictionary<Guid, GenericTimeableObject<DataProviderTransaction>> oDisposeTransactions = new LockableSortedDictionary<Guid, GenericTimeableObject<DataProviderTransaction>>();

            //IMPORTANT NOTE: Order used to check for timed out connections matters

            //Review Readers
            try
            {
                oReaders.AcquireWriterLock(); //Ensure nobody access the pool while we check
                foreach (KeyValuePair<Guid, GenericTimeableObject<IDataReader>> reader in oReaders)
                {
                    if (reader.Value.IsExpired(lMaxMillsec))
                    {
                        oDisposeReaders.Add(reader.Key, reader.Value);
                    }
                }
                foreach (KeyValuePair<Guid, GenericTimeableObject<IDataReader>> disposeReader in oDisposeReaders)
                {
                    disposeReader.Value.Data.Close();
                    disposeReader.Value.Data.Dispose();
                    Sitecore.Diagnostics.Log.Debug(string.Format("Scavenger Reclaimed {0} with Guid {1}", "IDataReader", disposeReader.Key.ToString()), typeof(SqlServerDataApiService));
                    oReaders.Remove(disposeReader.Key);
                }
                oDisposeReaders.Clear();
                oDisposeReaders = null;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SqlSeverDataApiService scavenger was unable to check reader objects for resource releasing", ex, typeof(SqlServerDataApiService));
            }
            finally
            {
                oReaders.ReleaseWriterLock();
            }

            // Review transactions
            try
            {
                oTransactions.AcquireWriterLock(); //Ensure nobody access the pool while we check
                foreach (KeyValuePair<Guid, GenericTimeableObject<DataProviderTransaction>> transaction in oTransactions)
                {
                    if (transaction.Value.IsExpired(lMaxMillsec))
                    {
                        oDisposeTransactions.Add(transaction.Key, transaction.Value);
                    }
                }
                foreach (KeyValuePair<Guid, GenericTimeableObject<DataProviderTransaction>> disposeTransaction in oDisposeTransactions)
                {
                    disposeTransaction.Value.Data.Dispose();
                    Sitecore.Diagnostics.Log.Debug(string.Format("Scavenger Reclaimed {0} with Guid {1}", "DataProviderTransaction", disposeTransaction.Key.ToString()), typeof(SqlServerDataApiService));
                    oTransactions.Remove(disposeTransaction.Key);
                }
                oDisposeTransactions.Clear();
                oDisposeTransactions = null;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SqlSeverDataApiService scavenger was unable to check transaction objects for resource releasing", ex, typeof(SqlServerDataApiService));
            }
            finally
            {
                oTransactions.ReleaseWriterLock(); // Release writer lock no matter what
            }

            //Review Commands
            try
            {
                oCommands.AcquireWriterLock(); //Ensure nobody access the pool while we check for resources to release
                foreach (KeyValuePair<Guid, GenericTimeableObject<DataProviderCommand>> command in oCommands)
                {
                    if (command.Value.IsExpired(lMaxMillsec))
                    {
                        oDisposeCommands.Add(command.Key, command.Value);
                    }
                }
                foreach (KeyValuePair<Guid, GenericTimeableObject<DataProviderCommand>> disposeCommand in oDisposeCommands)
                {
                    disposeCommand.Value.Data.Dispose();
                    Sitecore.Diagnostics.Log.Debug(string.Format("Scavenger Reclaimed {0} with Guid {1}", "DataProviderCommand", disposeCommand.Key.ToString()), typeof(SqlServerDataApiService));
                    oCommands.Remove(disposeCommand.Key);
                }
                oDisposeCommands.Clear();
                oDisposeCommands = null;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SqlSeverDataApiService scavenger was unable to check command objects for resource releasing", ex, typeof(SqlServerDataApiService));
            }
            finally
            {
                oCommands.ReleaseWriterLock(); //Release writer lock no matter what
            }

            //Review Connections
            try
            {
                oConnections.AcquireWriterLock();//Ensure nobody access the pool while we check for resources to release
                foreach (KeyValuePair<Guid, GenericTimeableObject<IDbConnection>> connection in oConnections)
                {
                    if (connection.Value.IsExpired(lMaxMillsec))
                    {
                        oDisposeConnections.Add(connection.Key, connection.Value);
                    }
                }
                foreach (KeyValuePair<Guid, GenericTimeableObject<IDbConnection>> disposeConnection in oDisposeConnections)
                {
                    disposeConnection.Value.Data.Close();
                    disposeConnection.Value.Data.Dispose();
                    Sitecore.Diagnostics.Log.Debug(string.Format("Scavenger Reclaimed {0} with Guid {1}", "IDbConnection", disposeConnection.Key.ToString()), typeof(SqlServerDataApiService));
                    oConnections.Remove(disposeConnection.Key);
                }
                oDisposeConnections.Clear();
                oDisposeConnections = null;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("SqlSeverDataApiService scavenger was unable to check connection objects for resource releasing", ex, typeof(SqlServerDataApiService));
            }
            finally
            {
                oConnections.ReleaseWriterLock(); // Release writer lock no matter what
            }

        }
        public static string GetConnectionTimeLife(Guid oGuid)
        {
            return GetTimeLife(oGuid, oConnections);
        }
        public static string GetCommandTimeLife(Guid oGuid)
        {
            return GetTimeLife(oGuid, oCommands);
        }
        public static string GetDataReaderTimeLife(Guid oGuid)
        {
            return GetTimeLife(oGuid, oReaders);
        }
        public static string GetTransactionTimeLife(Guid oGuid)
        {
            return GetTimeLife(oGuid, oTransactions);
        }
        private static string GetTimeLife<T>(Guid oGuid, LockableSortedDictionary<Guid, T> oItems) where T : class
        {
            T oItem;

            //Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService Getting Item {0}.", oGuid), oStaticLogOwner);

            try
            {
                //get a reader lock
                oItems.AcquireReaderLock(LockableListTimeoutMilliseconds);

                //do we have the item?
                oItem = oItems.ContainsKey(oGuid) ? oItems[oGuid] : null;
            }
            finally
            {
                //release the lock
                oItems.ReleaseReaderLock();
            }

            //Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService Getting Item {0}. Done.", oGuid), oStaticLogOwner);

            //return the item
            return oItem.ToString();
        }
        public void Initialize(string sConnectionString)
        {
            MatchCollection oMatches;
            try
            {
                Assert.ArgumentNotNullOrEmpty(sConnectionString, "sConnectionString");

                //is this a regex match?
                if ((oMatches = oConnectionStringRegex.Matches(sConnectionString)) != null && oMatches.Count > 0)
                {
                    ConnectionString = Sitecore.Configuration.Settings.GetConnectionString(oMatches[0].Groups[1].Value);
                }
                else
                {
                    ConnectionString = sConnectionString;
                }
            }
            catch (Exception oException)
            {
                Sitecore.Diagnostics.Log.Error("Exception during ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService.Initialize(string sConnectionString)", oException, this);
                throw oException;
            }
        }

        public new Guid CreateConnection()
        {
            try
            {
                //create and store the connection
                return PutItem(new GenericTimeableObject<IDbConnection>(base.CreateConnection()), oConnections);
            }
            catch (Exception oException)
            {
                Sitecore.Diagnostics.Log.Error("Exception during ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService.CreateConnection()", oException, this);
                throw oException;
            }
        }

        public new Guid CreateCommand(string sSql, object[] oParameters)
        {
            try
            {
                //output that we are trying to run a query
                //Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService.CreateCommand: [{0}]", sSql), this);

                //create and store the command
                return PutItem(new GenericTimeableObject<DataProviderCommand>(base.CreateCommand(sSql, oParameters)), oCommands);
            }
            catch (Exception oException)
            {
                Sitecore.Diagnostics.Log.Error("Exception during ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService.CreateCommand(string sSql, object[] oParameters)", oException, this);
                throw oException;
            }
        }

        public new Guid CreateTransaction()
        {
            try
            {
                //create and store the transaction
                return PutItem(new GenericTimeableObject<DataProviderTransaction>(base.CreateTransaction()), oTransactions);
            }
            catch (Exception oException)
            {
                Sitecore.Diagnostics.Log.Error("Exception during ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService.CreateTransaction()", oException, this);
                throw oException;
            }
        }

        public override int Execute(string sSql, object[] parameters)
        {
            try
            {
                //output that we are trying to run a query
                //Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService.Execute: [{0}]", sSql), this);

                //have the base implementation execute the query
                return base.Execute(sSql, parameters);
            }
            catch (Exception oException)
            {
                Sitecore.Diagnostics.Log.Error("Exception during ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService.Execute(string sSql, object[] parameters)", oException, this);
                throw oException;
            }
        }

        public static IDbConnection GetConnection(Guid oGuid)
        {
            return GetItem(oGuid, oConnections).Data;
        }

        public static DataProviderCommand GetCommand(Guid oGuid)
        {
            return GetItem(oGuid, oCommands);
        }

        public static IDataReader GetReader(Guid oGuid)
        {
            return GetItem(oGuid, oReaders).Data;
        }

        public static DataProviderTransaction GetTransaction(Guid oGuid)
        {
            return GetItem(oGuid, oTransactions);
        }

        private static T GetItem<T>(Guid oGuid, LockableSortedDictionary<Guid, T> oItems) where T : class
        {
            T oReturnItem;

            //Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService Getting Item {0}.", oGuid), oStaticLogOwner);

            try
            {
                //get a reader lock
                oItems.AcquireReaderLock(LockableListTimeoutMilliseconds);

                //do we have the item?
                oReturnItem = oItems.ContainsKey(oGuid) ? oItems[oGuid] : null;
            }
            finally
            {
                //release the lock
                oItems.ReleaseReaderLock();
            }

            //Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService Getting Item {0}. Done.", oGuid), oStaticLogOwner);

            //return the item
            return oReturnItem;
        }

        public static Guid[] GetConnectionKeys()
        {
            return GetKeys(oConnections);
        }

        public static Guid[] GetCommandKeys()
        {
            return GetKeys(oCommands);
        }

        public static Guid[] GetReaderKeys()
        {
            return GetKeys(oReaders);
        }

        private static Guid[] GetKeys<T>(LockableSortedDictionary<Guid, T> oItems) where T : class
        {
            Guid[] oGuids;

            try
            {
                //get a reader lock
                oItems.AcquireReaderLock(LockableListTimeoutMilliseconds);

                //get the keys as an array
                oGuids = oItems.Keys.ToArray();
            }
            finally
            {
                //release the lock
                oItems.ReleaseReaderLock();
            }

            return oGuids;
        }

        public static Guid PutCommand(DataProviderCommand oCommand)
        {
            return PutItem(new GenericTimeableObject<DataProviderCommand>(oCommand), oCommands);
        }

        public static Guid PutReader(IDataReader oReader)
        {
            return PutItem(new GenericTimeableObject<IDataReader>(oReader), oReaders);
        }

        public static Guid PutTransaction(DataProviderTransaction oTransaction)
        {
            return PutItem(new GenericTimeableObject<DataProviderTransaction>(oTransaction), oTransactions);
        }

        private static Guid PutItem<T>(T oItem, LockableSortedDictionary<Guid, T> oItems) where T : class
        {
            Guid oGuid;

            //create guid
            oGuid = Guid.NewGuid();

            Sitecore.Diagnostics.Log.Debug(string.Format("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService Putting Item {0}.", oGuid), oStaticLogOwner);

            //create a guid and store the item
            PutItem(oGuid, oItem, oItems);

            Sitecore.Diagnostics.Log.Debug(string.Format("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.SqlServerDataApiService Putting Item {0}. Done.", oGuid), oStaticLogOwner);

            //return the guid
            return oGuid;
        }

        private static void PutItem<T>(Guid oGuid, T oItem, LockableSortedDictionary<Guid, T> oItems) where T : class
        {
            try
            {
                //acquire a reader lock
                oItems.AcquireWriterLock(LockableListTimeoutMilliseconds);

                //store the item
                oItems.Add(oGuid, oItem);
            }
            finally
            {
                //release the writer lock
                oItems.ReleaseWriterLock();
            }
        }

        public static void RemoveConnection(Guid oGuid)
        {
            RemoveItem(oGuid, oConnections);
        }

        public static void RemoveCommand(Guid oGuid)
        {
            RemoveItem(oGuid, oCommands);
        }

        public static void RemoveReader(Guid oGuid)
        {
            RemoveItem(oGuid, oReaders);
        }

        private static void RemoveItem<T>(Guid oGuid, LockableSortedDictionary<Guid, T> oItems) where T : class
        {
            LockCookie oLockCookie;

            try
            {
                //before we start reading from the list, acquire a reader lock on it
                oItems.AcquireReaderLock(LockableListTimeoutMilliseconds);

                //do we have this item?
                if (oItems.ContainsKey(oGuid))
                {
                    //acquire a reader lock
                    oLockCookie = oItems.UpgradeToWriterLock(LockableListTimeoutMilliseconds);

                    try
                    {
                        //remove the item
                        oItems.Remove(oGuid);
                    }
                    finally
                    {
                        //release the writer lock
                        oItems.DowngradeFromWriterLock(ref oLockCookie);
                    }
                }
            }
            finally
            {
                //release the writer lock
                oItems.ReleaseReaderLock();
            }
        }
    }
}
