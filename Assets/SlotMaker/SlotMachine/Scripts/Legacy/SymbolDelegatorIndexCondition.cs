using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas;

namespace SlotMaker
{
	public class SymbolDelegatorIndexCondition : SymbolDelegatorCondition
	{
		public List<int> indices;
		public ActionListPlayer player;

		private HashSet<int> indexSet;

		private void Awake()
		{
			indexSet = new HashSet<int>();

			int count = indices.Count;
			for (int i = 0; i < indices.Count; ++i)
			{
				indexSet.Add(indices[i]);
			}
		}

		public override ActionListPlayer FindPlayer(BaseSymbol symbol, string animationName)
		{
			if (indexSet.Contains(symbol.symbolInfo.symbol))
				return player;

			return null;
		}
	}
}
