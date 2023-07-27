using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class TimeBonusCollect : ActionTask <Blackboard> 
{
    protected override string info
	{ 
		get 
		{ 
			return string.Format("Time Bonus Collect");
		} 
	}
	protected override void OnExecute()
	{
        // PassiveEventManager.Instance.GetActiveEventID(EventInfoType.TIME_BONUS)
        int timeBonusEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "timeBonusEventID").value;

        BagelCodeClientAPI.TimeBonusCollect(timeBonusEventID,
        (response) =>
        {
            if(agent != null)
            {
                ClientAPI2Blackboard.Serialize(agent, response);

                var timeBonusCredit = BlackboardUtils.GetOrCreateVariable<long>(agent, "timeBonusCredit");
                timeBonusCredit.value = response.earnCredit;
                BlackboardQueryUtils.AddCoins(response.earnCredit);
                BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeInfo);

                var timebonusConsecutive = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/timebonusConsecutive");
                var lastCollectTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "lastCollectTimestamp");

                timebonusConsecutive.value = response.timebonusConsecutive;
                lastCollectTimestamp.value = response.lastCollectTimestamp;
                
#if !UNITY_EDITOR
                // var enabledLuckySpin = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_LUCKY_SPIN");

                // string localPushKey = "values/misc/LOCAL_PUSH/TIME_BONUS";

                // if(enabledLuckySpin != null && enabledLuckySpin.value)
                // {
                //     var luckySpinMaxCount = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/gameSpin/consecutiveMax");
                    
                //     if(timebonusConsecutive.value >= luckySpinMaxCount.value)
                //         localPushKey = "values/misc/LOCAL_PUSH/LUCKY_SPIN_BONUS";
                // }   

                // var pushInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), localPushKey);

                // NativeHelper.Instance.DeleteLocalPush(pushInfoBB.value.GetValue<int>("ID"));
                // NativeHelper.Instance.SetLocalPush(pushInfoBB.value.GetValue<int>("ID"), pushInfoBB.value.GetValue<string>("TITLE"), pushInfoBB.value.GetValue<string>("TEXT"), pushInfoBB.value.GetValue<int>("SECONDS"), pushInfoBB.value.GetValue<string>("TYPE"), false);
#endif

                EndAction(true);
            }
            
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.COOLTIME_NOT_YET_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();

                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_COOLTIME_NOT_YET", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                        ErrorPopupHandler.Instance.OpenError(info);

                        EndAction(false);
                    }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
	}
}

}
