using System;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;

//SOURCE CODE FROM http://knab.ws/blog/index.php?/archives/10-CSV-file-parser-and-writer-in-C-Part-2.html

namespace ServerLogic.Parsing.Base
{
	public class DelimetedTextParser
	{
		private char cDelimeter = ',';

		public DelimetedTextParser(char cDelimeter)
		{
			this.cDelimeter = cDelimeter;
		}

		public DataTable Parse(string data, bool headers)
		{
			return Parse(new StringReader(data), headers);
		}

		public DataTable Parse(string data)
		{
			return Parse(new StringReader(data));
		}

		public DataTable Parse(Stream oStream)
		{
			return Parse(new StreamReader(oStream), false);
		}

		public DataTable Parse(Stream oStream, bool bHeaders)
		{
			return Parse(new StreamReader(oStream), bHeaders);
		}

		public DataTable Parse(TextReader stream)
		{
			return Parse(stream, false);
		}

		public DataTable Parse(TextReader stream, bool headers)
		{
			DataTable table = new DataTable();
			DelimitedTextStream csv = new DelimitedTextStream(stream, cDelimeter);
			string[] row = csv.GetNextRow();
			if (row == null)
				return null;
			if (headers)
			{
				foreach (string header in row)
				{
					if (header != null && header.Length > 0 && !table.Columns.Contains(header))
						table.Columns.Add(header, typeof(string));
					else
						table.Columns.Add(GetNextColumnHeader(table), typeof(string));
				}
				row = csv.GetNextRow();
			}
			while (row != null)
			{
				while (row.Length > table.Columns.Count)
					table.Columns.Add(GetNextColumnHeader(table), typeof(string));
				table.Rows.Add(row);
				row = csv.GetNextRow();
			}
			return table;
		}

		private string GetNextColumnHeader(DataTable table)
		{
			int c = 1;
			while (true)
			{
				string h = "Column" + c++;
				if (!table.Columns.Contains(h))
					return h;
			}
		}
	}
}
