using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots.Strategy
{
	[Serializable]
	public abstract class SymbolSortingOrderStrategy : ScriptableObject
	{
	    public abstract int GetSortingOrder(int x, int y, int z);
	}
}