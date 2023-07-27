using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Cards
{
	[Serializable]
	public class PokerWin
	{
		public int owner;
		public int paytableIndex;
		public List<bool> hitmap;
		public long earnCredit;
		public long multiplier;
	}
}
