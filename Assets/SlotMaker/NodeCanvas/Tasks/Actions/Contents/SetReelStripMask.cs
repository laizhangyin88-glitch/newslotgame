using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class SetReelStripMask : ActionTask
{
	public BBParameter<int> reelStripsIndex;
	public BBParameter<int> reelStripIndex;
	public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;
	public SymbolAttribute attribute;

	protected override void OnExecute()
	{
		var strip = ((ReelStrip)GlobalReelStrips.Instance.stripsList[reelStripsIndex.value].reelStrips[reelStripIndex.value]).strip;
		for (int i = 0; i < strip.Count; ++i)
		{
			var symbol = strip[i];
			symbol.mask = (SymbolAttribute)OperationUtils.Operate((int)symbol.mask, (int)attribute, Operation);
		}

		EndAction();
	}
}

}
