using NodeCanvas.Framework;
using NodeCanvas.Tasks.Conditions;
using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using UnityEngine;

namespace SlotMaker.Tasks.Condition
{
    [Category("✫ Utility")]
    [EventReceiver("OnCustomEvent")]
    public class CheckMachineSelect : ConditionTask<GraphOwner>
    {
        //[RequiredField]
        //public BBParameter<string> eventName;
        //public BBParameter<int> index = 0;

        public BBParameter<string> path;

        protected override string info
        {
            // get { return string.Format("★ [{0}]({1})", eventName.value, slotIndex.value); }
            get { return "Machine Select Event"; }
        }
        protected override bool OnCheck() { return false; }
        public void OnCustomEvent(EventData receivedEvent)
        {
            if (isActive && receivedEvent.name == "MachineSelectEvent")
            {
                Transform msb = agent.gameObject.transform.Find(path.value ?? "Machine Select Border");

                if (msb != null)
                {
                    MachineSelectBorder gomp = msb.GetComponent<MachineSelectBorder>();

                    Debug.Log($" name = {receivedEvent.name}  value = {receivedEvent.value}   index = {gomp.index}");
                    if (gomp != null && gomp.index == (int)receivedEvent.value)
                    {
                        YieldReturn(true);
                    }
                }
            }
        }
    }

}
