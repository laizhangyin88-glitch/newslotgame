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
    public class GetBIInfoBuyBonus : ActionTask<Blackboard> 
    {
        public BBParameter<string> gameIDValue;

        public BBParameter<int> eventID;
        public BBParameter<bool> saveAsIsEvent;

        protected override string info
        {
            get{ return string.Format("Buy Bonus BI Info({0})", gameIDValue); }
        }

        protected override void OnExecute () 
        {
            saveAsIsEvent.value = false;

            var gameID = BlackboardUtils.FindVariable<int>(agent, gameIDValue.value);
            if(gameID != null)
            {
                EventInfo bonusEventInfo = PassiveEventManager.Instance.GetBonusEventInfo(gameID.value);
                if (bonusEventInfo != null)
                {
                    if (bonusEventInfo.type == EventInfoType.BONUS_SALE)
                    {
                        saveAsIsEvent.value = true;
                        eventID.value = bonusEventInfo.id;
                    }
                    else if (bonusEventInfo.type == EventInfoType.BONUS_MULTIPLY)
                    {
                        saveAsIsEvent.value = true;
                        eventID.value = bonusEventInfo.id;
                    }
                
                }
            } 

            EndAction();
        }
    }
}
