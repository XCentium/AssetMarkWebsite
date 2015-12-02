using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server;
using System.ServiceModel;
using ServerLogic.WCF;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client
{
	public class DataReader : IDataReader
	{
		private ResilientChannelFactory<IDataReaderService> oFactory;

		public DataReader(Guid oGuid)
		{
			//create a factory to generate a channel to the service
			oFactory = new ResilientChannelFactory<IDataReaderService>("IDataReaderService", delegate(IDataReaderService oService) { oService.Initialize(oGuid); });
		}

		public void Close()
		{
			oFactory.Service.Close();
		}

		public int Depth
		{
			get { return oFactory.Service.Depth; }
		}

		public DataTable GetSchemaTable()
		{
			return oFactory.Service.GetSchemaTable();
		}

		public bool IsClosed
		{
			get { return oFactory.Service.IsClosed; }
		}

		public bool NextResult()
		{
			return oFactory.Service.NextResult();
		}

		public bool Read()
		{
			return oFactory.Service.Read();
		}

		public int RecordsAffected
		{
			get { return oFactory.Service.RecordsAffected; }
		}

		public void Dispose()
		{
			oFactory.Service.Dispose();
		}

		public int FieldCount
		{
			get { return oFactory.Service.FieldCount; }
		}

		public bool GetBoolean(int i)
		{
			return oFactory.Service.GetBoolean(i);
		}

		public byte GetByte(int i)
		{
			return oFactory.Service.GetByte(i);
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return oFactory.Service.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public char GetChar(int i)
		{
			return oFactory.Service.GetChar(i);
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public IDataReader GetData(int i)
		{
			throw new NotImplementedException();
		}

		public string GetDataTypeName(int i)
		{
			return oFactory.Service.GetDataTypeName(i);
		}

		public DateTime GetDateTime(int i)
		{
			return oFactory.Service.GetDateTime(i);
		}

		public decimal GetDecimal(int i)
		{
			return oFactory.Service.GetDecimal(i);
		}

		public double GetDouble(int i)
		{
			return oFactory.Service.GetDouble(i);
		}

		public Type GetFieldType(int i)
		{
			return oFactory.Service.GetFieldType(i);
		}

		public float GetFloat(int i)
		{
			return oFactory.Service.GetFloat(i);
		}

		public Guid GetGuid(int i)
		{
			return oFactory.Service.GetGuid(i);
		}

		public short GetInt16(int i)
		{
			return oFactory.Service.GetInt16(i);
		}

		public int GetInt32(int i)
		{
			return oFactory.Service.GetInt32(i);
		}

		public long GetInt64(int i)
		{
			return oFactory.Service.GetInt64(i);
		}

		public string GetName(int i)
		{
			return oFactory.Service.GetName(i);
		}

		public int GetOrdinal(string name)
		{
			return oFactory.Service.GetOrdinal(name);
		}

		public string GetString(int i)
		{
			return oFactory.Service.GetString(i);
		}

		public object GetValue(int i)
		{
			return oFactory.Service.GetValue(i);
		}

		public int GetValues(object[] values)
		{
			return oFactory.Service.GetValues(values);
		}

		public bool IsDBNull(int i)
		{
			return oFactory.Service.IsDBNull(i);
		}

		public object this[string name]
		{
			get { return oFactory.Service[name]; }
		}

		public object this[int i]
		{
			get { return oFactory.Service[i]; }
		}
	}
}
