using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Utilities.GridComponent
{
	public class GridRow
	{
		public Dictionary<string, string> Attributes { get; set; }
		public List<GridCell> Cells { get; set; }
	}
}
