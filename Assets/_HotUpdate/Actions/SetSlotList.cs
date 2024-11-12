using UnityEngine;

using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Task.Actions
{
[Category("★ BagelCode/SlotList")]
public class SetSlotList : ActionTask 
{
	[RequiredField] [BlackboardOnly]
    public BBParameter<IList<Blackboard>> targetList;

	protected override string info
	{
		get { return "Set size of slot themnails"; }
	}

	protected override void OnExecute()
	{
		IList<Blackboard> slots = targetList.value;

		if (slots == null) EndAction(false);
		
		int count = 0;
		for (int i = 0; i < slots.Count; ++i)
		{
			Blackboard flags = slots[i].GetValue<Blackboard>("flags");
			bool isLong 	= flags.GetValue<bool>("isLong");
			if (!isLong)
			{
				++count;
			}
			else
			{
				if (count % 2 != 0)
				{
					flags.SetValue("isLong", false);
				}
				count = 0;
			}
		}

		if (slots.Count > 1 && count % 2 != 0)
		{
			Blackboard flags = slots[slots.Count - 2].GetValue<Blackboard>("flags");
			flags.SetValue("isLong", false);
		}

		EndAction();
	}
}

}
