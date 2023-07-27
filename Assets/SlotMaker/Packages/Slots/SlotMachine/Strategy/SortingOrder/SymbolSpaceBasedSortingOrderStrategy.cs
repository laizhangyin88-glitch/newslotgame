using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SlotMaker.Slots.Strategy
{
	[CreateAssetMenu(fileName="New SortingOrder Strategy", menuName="SlotMaker2/Slot/Strategy/Symbol/SortingOrder")]
	public class SymbolSpaceBasedSortingOrderStrategy : SymbolSortingOrderStrategy
	{
		public int startingOrder;
		public int xSpace;
		public int ySpace;
		public int zSpace;

		public override int GetSortingOrder(int x, int y, int z)
	    {
	    	return startingOrder + x * xSpace + y * ySpace + z * zSpace;
	    }
	}
}