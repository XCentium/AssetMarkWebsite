using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DataReaderService : IDataReaderService
	{
		private IDataReader oReader;
		private Guid oGuid;

		private IDataReader Reader
		{
			get
			{
				if (oReader == null)
				{
					oReader = SqlServerDataApiService.GetReader(oGuid);
					if (oReader == null)
					{
						throw new NullReferenceException(string.Format("Could not get reader for Guid {0}", oGuid.ToString()));
					}
				}
				return oReader;
			}
		}

		private void LogError(string sClass, string sOperation, Exception oException)
		{
			Sitecore.Diagnostics.Log.Error(
				string.Format("Exception in {0} with Guid {1} while executing: {2}", sClass, oGuid != null ? oGuid.ToString() : "null", sOperation)
				, oException, this);
		}

		public void Initialize(Guid oGuid)
		{
			this.oGuid = oGuid;
			Sitecore.Diagnostics.Log.Debug(string.Format("Initializing {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", oGuid.ToString()),this);
		}

		public void Close()
		{
			try
			{
				Reader.Close();
                Sitecore.Diagnostics.Log.Debug(string.Format("Closed {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", oGuid.ToString()), this);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "Close()", oException);
				throw oException;
			}
		}

		public System.Data.DataTable GetSchemaTable()
		{
			try
			{
				return Reader.GetSchemaTable();
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetSchemaTable()", oException);
				throw oException;
			}
		}

		public bool NextResult()
		{
			try
			{
				return Reader.NextResult();
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "NextResult()", oException);
				throw oException;
			}
		}

		public bool Read()
		{
			try
			{
				return Reader.Read();
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "Read()", oException);
				throw oException;
			}
		}

		public int Depth
		{
			get
			{
				try
				{
					return Reader.Depth;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "Depth", oException);
					throw oException;
				}
			}
		}

		public bool IsClosed
		{
			get
			{
				try
				{
					return Reader.IsClosed;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetSchemaTable()", oException);
					throw oException;
				}
			}
		}

		public int RecordsAffected
		{
			get
			{
				try
				{
					return Reader.RecordsAffected;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "RecordsAffected", oException);
					throw oException;
				}
			}
		}

		public object this[string name]
		{
			get
			{
				try
				{
					return oReader[name];
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "this[string name]", oException);
					throw oException;
				}
			}
		}

		public object this[int i]
		{
			get
			{
				try
				{
					return oReader[i];
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "this[int i]", oException);
					throw oException;
				}
			}
		}

		public bool GetBoolean(int i)
		{
			try
			{
				return Reader.GetBoolean(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetBoolean(int i)", oException);
				throw oException;
			}
		}

		public byte GetByte(int i)
		{
			try
			{
				return Reader.GetByte(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetSchemaTable()", oException);
				throw oException;
			}
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			try
			{
				return Reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)", oException);
				throw oException;
			}
		}

		public char GetChar(int i)
		{
			try
			{
				return Reader.GetChar(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetChar(int i)", oException);
				throw oException;
			}
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			try
			{
				return Reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)", oException);
				throw oException;
			}
		}

		public string GetDataTypeName(int i)
		{
			try
			{
				return Reader.GetDataTypeName(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetDataTypeName(int i)", oException);
				throw oException;
			}
		}

		public DateTime GetDateTime(int i)
		{
			try
			{
				return Reader.GetDateTime(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetDateTime(int i)", oException);
				throw oException;
			}
		}

		public decimal GetDecimal(int i)
		{
			try
			{
				return Reader.GetDecimal(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetDecimal(int i)", oException);
				throw oException;
			}
		}

		public double GetDouble(int i)
		{
			try
			{
				return Reader.GetDouble(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetDouble(int i)", oException);
				throw oException;
			}
		}

		public Type GetFieldType(int i)
		{
			try
			{
				return Reader.GetFieldType(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetFieldType(int i)", oException);
				throw oException;
			}
		}

		public float GetFloat(int i)
		{
			try
			{
				return Reader.GetFloat(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetFloat(int i)", oException);
				throw oException;
			}
		}

		public Guid GetGuid(int i)
		{
			try
			{
				return Reader.GetGuid(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetGuid(int i)", oException);
				throw oException;
			}
		}

		public short GetInt16(int i)
		{
			try
			{
				return Reader.GetInt16(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetInt16(int i)", oException);
				throw oException;
			}
		}

		public int GetInt32(int i)
		{
			try
			{
				return Reader.GetInt32(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetInt32(int i)", oException);
				throw oException;
			}
		}

		public long GetInt64(int i)
		{
			try
			{
				return Reader.GetInt64(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetInt64(int i)", oException);
				throw oException;
			}
		}

		public string GetName(int i)
		{
			try
			{
				return Reader.GetName(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetName(int i)", oException);
				throw oException;
			}
		}

		public int GetOrdinal(string name)
		{
			try
			{
				return Reader.GetOrdinal(name);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetOrdinal(string name)", oException);
				throw oException;
			}
		}

		public string GetString(int i)
		{
			try
			{
				return Reader.GetString(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetString(int i)", oException);
				throw oException;
			}
		}

		public object GetValue(int i)
		{
			try
			{
				return Reader.GetValue(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetValue(int i)", oException);
				throw oException;
			}
		}

		public int GetValues(object[] values)
		{
			try
			{
				return Reader.GetValues(values);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "GetValues(object[] values)", oException);
				throw oException;
			}
		}

		public bool IsDBNull(int i)
		{
			try
			{
				return Reader.IsDBNull(i);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "IsDBNull(int i)", oException);
				throw oException;
			}
		}

		public int FieldCount
		{
			get
			{
				try
				{
					return Reader.FieldCount;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "FieldCount", oException);
					throw oException;
				}
			}
		}

		public void Dispose()
		{
			try
			{
				Reader.Dispose();
				SqlServerDataApiService.RemoveReader(oGuid);
                Sitecore.Diagnostics.Log.Debug(string.Format("Dispose {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", oGuid.ToString()), this);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataReaderService", "Dispose()", oException);
				throw oException;
			}
		}
	}
}
