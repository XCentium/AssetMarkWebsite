using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using ServerLogic.Parsing.Base;

namespace ServerLogic.Parsing.Tab
{
	public class TabWriter
	{
		public static string WriteToString(DataTable oTable, bool bIncludeHeaders, bool bQuoteAll)
		{
			return new DelimitedTextWriter('\t').WriteToString(oTable, bIncludeHeaders, bQuoteAll);
		}

		public static void WriteToStream(TextWriter oStream, DataTable oTable, bool bIncludeHeaders, bool bQuoteAll)
		{
			new DelimitedTextWriter('\t').WriteToStream(oStream, oTable, bIncludeHeaders, bQuoteAll);
		}
	}
}
