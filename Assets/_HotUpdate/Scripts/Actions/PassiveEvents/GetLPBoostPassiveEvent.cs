using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions 
{

    [Category("★ BagelCode/PassiveEvents")]
    public class GetLPBoostPassiveEventBB : ActionTask<Blackboard> 
    {
        public BBParameter<int> id;
        public BBParameter<long> endTimestamp;

        protected override string info
        {
            get{ return string.Format("LP Boost Passive Event"); }
        }

        protected override void OnExecute () 
        {
            EventInfo lpBoostEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.LP_BOOST);
        
            if(lpBoostEventInfo != null)
            {
                id.value = lpBoostEventInfo.id;
                endTimestamp.value = lpBoostEventInfo.endTimestamp;
            }
            else
            {
                id.value = 0;
                endTimestamp.value = 0;
            }

            EndAction(true);
        }
    }

}
