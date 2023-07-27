using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Task.Condition
{
    [Category("★ BagelCode/Events")]
    [EventReceiver("OnMetaSlotSpinButtonEvent")]
    public class CheckMetaGameSlotMachineSpinButtonEvent : ConditionTask<GraphOwner>
	{
		public BBParameter<int> slotIndex = 0;

		protected override string info { get { return "★ [MetaGameSpinButton](" + slotIndex + ")"; } }
		protected override bool OnCheck() { return false; }
		public void OnMetaSlotSpinButtonEvent(EventData receivedEvent)
		{
			if (isActive && receivedEvent.id == slotIndex.value)
			{

#if UNITY_EDITOR
				if (NodeCanvas.Editor.Prefs.logEvents)
				{
					Debug.Log(string.Format("★ Event '{0}' Received from '{1}'", receivedEvent.name, agent.gameObject.name), agent);
				}
#endif

				YieldReturn(true);
			}
		}
	}
}