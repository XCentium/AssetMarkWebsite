using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;

using ServerLogic.Parsing.Base;

namespace ServerLogic.Parsing.Csv
{
	public class CsvParser
	{
		private static char cDelimeter = ',';

		public static DataTable Parse(string sData, bool bHasHeaders)
		{
			return new DelimetedTextParser(cDelimeter).Parse(sData, bHasHeaders);
		}

		public static DataTable Parse(string sData)
		{
			return new DelimetedTextParser(cDelimeter).Parse(sData);
		}

		public static DataTable Parse(Stream oStream)
		{
			return new DelimetedTextParser(cDelimeter).Parse(oStream);
		}

		public static DataTable Parse(Stream oStream, bool bHasHeaders)
		{
			return new DelimetedTextParser(cDelimeter).Parse(oStream, bHasHeaders);
		}

		public static DataTable Parse(TextReader oStream)
		{
			return new DelimetedTextParser(cDelimeter).Parse(oStream);
		}

		public static DataTable Parse(TextReader oStream, bool bHasHeaders)
		{
			return new DelimetedTextParser(cDelimeter).Parse(oStream, bHasHeaders);
		}
	}
}
