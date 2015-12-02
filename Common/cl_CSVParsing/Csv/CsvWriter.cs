using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using ServerLogic.Parsing.Base;

namespace ServerLogic.Parsing.Csv
{
	public class CsvWriter
	{
		public static string WriteToString(DataTable oTable, bool bIncludeHeaders, bool bQuoteAll)
		{
			return new DelimitedTextWriter(',').WriteToString(oTable, bIncludeHeaders, bQuoteAll);
		}

		public static void WriteToStream(TextWriter oStream, DataTable oTable, bool bIncludeHeaders, bool bQuoteAll)
		{
			new DelimitedTextWriter(',').WriteToStream(oStream, oTable, bIncludeHeaders, bQuoteAll);
		}
	}
}
