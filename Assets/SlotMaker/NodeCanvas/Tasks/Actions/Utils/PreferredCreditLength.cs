using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Utility/Credit")]
public class PreferredCreditLength : ActionTask
{
	public BBParameter<long> credit;
	public BBParameter<long> preferredCredit;

	public BBParameter<long> saveAs;
	public BBParameter<long> saveUnit;

	protected override void OnExecute()
	{
		saveAs.value = credit.value;
		saveUnit.value = 1L;

		while (saveAs.value > preferredCredit.value)
		{
			saveAs.value /= 1000L;
			saveUnit.value *= 1000L;
		}

		EndAction();
	}
}

}
