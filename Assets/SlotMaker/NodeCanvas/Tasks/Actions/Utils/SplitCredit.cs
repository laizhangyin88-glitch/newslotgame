using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Utility/Credit")]
public class SplitCredit : ActionTask
{
	public BBParameter<long> credit;

	public BBParameter<List<long>> saveAs;

	protected override void OnExecute()
	{
		saveAs.value = new List<long>();

		long curr = credit.value;
		while (curr > 0L)
		{
			saveAs.value.Add(curr % 10L);
			curr /= 10L;
		}

		EndAction();
	}
}

}
