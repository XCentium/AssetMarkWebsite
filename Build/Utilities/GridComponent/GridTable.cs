using System;
using System.Collections.Generic;
using System.Linq;

namespace Genworth.SitecoreExt.Utilities.GridComponent
{
	public class GridTable
	{
		public List<GridRow> Rows { get; set; }
		public List<GridCell> Header { get; set; }
		public string ID { get; set; }
	}
}