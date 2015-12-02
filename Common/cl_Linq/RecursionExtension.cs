using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerLogic.Linq
{
	/// <summary>
	/// This class represents a set of extension methods for working with recursive data structures.
	/// </summary>
	public static class RecursiveExtension
	{
		public static IEnumerable<Item> RecursiveToList<Item>(this Item oItem, Func<Item, IEnumerable<Item>> oRecursiveFunction)
		{
			//return the list with a maximum of 1000 items
			return oItem.RecursiveToList(oRecursiveFunction, 1000);
		}

		/// <summary>
		/// Since this method returns an enuermation, it is not ideal for it to be truly recursive. Rather, it uses a stack to simulate recursion and save memory.
		/// </summary>
		public static IEnumerable<Item> RecursiveToList<Item>(this Item oItem, Func<Item, IEnumerable<Item>> oRecursiveFunction, int iMaxItems)
		{
			Stack<Item> oStack;
			Item oCurrentItem;
			int iItemCount;

			//create the stack
			oStack = new Stack<Item>();

			//push the current item onto the stack
			oStack.Push(oItem);

			//set item count to zero;
			iItemCount = 0;

			//loop until the stack is empty
			while (oStack.Count != 0 && (iMaxItems == 0 || iItemCount < iMaxItems))
			{
				//pop the current item off
				oCurrentItem = oStack.Pop();

				//yield the current item
				yield return oCurrentItem;

				//get the children items
				foreach (Item oChildItem in oRecursiveFunction(oCurrentItem).Reverse())
				{
					//put the item in the stack
					oStack.Push(oChildItem);
				}

				//increment the item count
				iItemCount++;
			}
		}
	}
}
