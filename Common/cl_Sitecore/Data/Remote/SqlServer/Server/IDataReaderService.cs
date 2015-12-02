using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.ServiceModel.Activation;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server
{
	[ServiceContract(Namespace = "http://ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.IDataReaderService", SessionMode = SessionMode.Required)]
	public interface IDataReaderService
	{
		[OperationContract(IsOneWay = false, IsInitiating = true)]
		void Initialize(Guid oGuid);

		#region BASED ON IDATAREADER

		[OperationContract(IsInitiating = false)]
		void Close();

		[OperationContract(IsInitiating = false)]
		DataTable GetSchemaTable();

		[OperationContract(IsInitiating = false)]
		bool NextResult();

		[OperationContract(IsInitiating = false)]
		bool Read();

		int Depth
		{
			[OperationContract(IsInitiating = false)]
			get;
		}

		bool IsClosed
		{
			[OperationContract(IsInitiating = false)]
			get;
		}

		int RecordsAffected
		{
			[OperationContract(IsInitiating = false)]
			get;
		}

		#endregion

		#region BASED ON IDATARECORD

		object this[string name]
		{
			[OperationContract(Name = "GetItemByName")]
			get;
		}

		object this[int i]
		{
			[OperationContract(Name = "GetItemById")]
			get;
		}

		[OperationContract(IsInitiating = false)]
		bool GetBoolean(int i);

		[OperationContract(IsInitiating = false)]
		byte GetByte(int i);

		[OperationContract(IsInitiating = false)]
		long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);

		[OperationContract(IsInitiating = false)]
		char GetChar(int i);

		[OperationContract(IsInitiating = false)]
		long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length);

		[OperationContract(IsInitiating = false)]
		string GetDataTypeName(int i);

		[OperationContract(IsInitiating = false)]
		DateTime GetDateTime(int i);

		[OperationContract(IsInitiating = false)]
		decimal GetDecimal(int i);

		[OperationContract(IsInitiating = false)]
		double GetDouble(int i);

		[OperationContract(IsInitiating = false)]
		Type GetFieldType(int i);

		[OperationContract(IsInitiating = false)]
		float GetFloat(int i);

		[OperationContract(IsInitiating = false)]
		Guid GetGuid(int i);

		[OperationContract(IsInitiating = false)]
		short GetInt16(int i);

		[OperationContract(IsInitiating = false)]
		int GetInt32(int i);

		[OperationContract(IsInitiating = false)]
		long GetInt64(int i);

		[OperationContract(IsInitiating = false)]
		string GetName(int i);

		[OperationContract(IsInitiating = false)]
		int GetOrdinal(string name);

		[OperationContract(IsInitiating = false)]
		string GetString(int i);

		[OperationContract(IsInitiating = false)]
		object GetValue(int i);

		[OperationContract(IsInitiating = false)]
		int GetValues(object[] values);

		[OperationContract(IsInitiating = false)]
		bool IsDBNull(int i);

		int FieldCount
		{
			[OperationContract(IsInitiating = false)]
			get;
		}

		#endregion

		#region BASED ON IDISPOSABLE

		[OperationContract(IsInitiating = false)]
		void Dispose();

		#endregion
	}
}
