using System;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;

//SOURCE CODE FROM http://knab.ws/blog/index.php?/archives/3-CSV-file-parser-and-writer-in-C-Part-1.html

namespace ServerLogic.Parsing.Base
{
	public class DelimitedTextWriter
	{
		private char cDelimeter = ',';

		public DelimitedTextWriter(char cDelimeter)
		{
			this.cDelimeter = cDelimeter;
		}

		public string WriteToString(DataTable table, bool header, bool quoteall)
		{
			StringWriter writer = new StringWriter();
			WriteToStream(writer, table, header, quoteall);
			return writer.ToString();
		}

		public void WriteToStream(TextWriter stream, DataTable table, bool header, bool quoteall)
		{
			if (header)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					WriteItem(stream, table.Columns[i].Caption, quoteall);
					if (i < table.Columns.Count - 1)
						stream.Write(cDelimeter);
					else
						stream.Write('\n');
				}
			}
			foreach (DataRow row in table.Rows)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					WriteItem(stream, row[i], quoteall);
					if (i < table.Columns.Count - 1)
						stream.Write(cDelimeter);
					else
						stream.Write('\n');
				}
			}
		}

		private void WriteItem(TextWriter stream, object item, bool quoteall)
		{
			if (item == null)
				return;
			string s = item.ToString();
			if (quoteall || s.IndexOfAny("\",\x0A\x0D".ToCharArray()) > -1)
				stream.Write("\"" + s.Replace("\"", "\"\"") + "\"");
			else
				stream.Write(s);
		}
	}
}