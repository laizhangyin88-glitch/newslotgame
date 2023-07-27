using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UnlinkDeckSymbols : ActionTask
{
    public BBParameter<int> slotIndex = 0;

	protected override void OnExecute()
	{
        var deck = ContentCustomData.GetSlotData(slotIndex.value).deck.deck;
		int iCount = deck.Count;
		for (int i = 0; i < iCount; ++i)
		{
			int jCount = deck[i].Count;
			for (int j = 0; j < jCount; ++j)
			{
				deck[i][j].link.Reset();
			}
		}

		EndAction();
	}
}

}
