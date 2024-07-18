using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Task.Condition
{

    [Category("★ BagelCode/Utility")]
    [EventReceiver("OnSymbolEvent")]
    public class CheckSymbolReelIndex : ConditionTask<Transform>
    {
        [RequiredField]
        public BBParameter<int> reelIndex;

        protected override string info { get { return "symbol in reel " + reelIndex.value; } }
        protected override bool OnCheck() { return false; }
        public void OnSymbolEvent(EventData receivedEvent)
        {

            int idx = -1;
            Transform gb = agent;

            for (int i=0; i<10; i++)
            {
                if (gb.parent == null)
                {
                    YieldReturn(false);
                    return;
                }
                if (gb.parent.name == "Reels")
                {
                    idx = gb.GetSiblingIndex();
                    break;
                }
                else
                {
                    gb = gb.parent;
                }
            }

            YieldReturn(reelIndex.value == idx);
            //YieldReturn(true);
        }
    }
}
