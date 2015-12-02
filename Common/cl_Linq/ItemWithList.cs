using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerLogic.LinqExt
{
	public class ItemWithList<T, S> : List<S>
	{
		private T oItem;

		public T Item
		{
			get { return oItem; }
			set { oItem = value; }
		}

		public ItemWithList() : base() { }

		public ItemWithList(T oItem) : base()
		{
			this.oItem = oItem;
		}
	}
}
