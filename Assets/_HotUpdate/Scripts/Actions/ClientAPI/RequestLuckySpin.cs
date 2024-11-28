using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestLuckySpin : ActionTask <Blackboard> 
{
    protected override string info
    { 
        get 
        { 
            return string.Format("Request Lucky Spin");
        } 
    }
    protected override void OnExecute()
    {
        BagelCodeClientAPI.LuckySpinBonus(
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(agent, response);

            var timebonusConsecutive = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/timebonusConsecutive");
            var lastCollectTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "lastCollectTimestamp");
            var timeBonusCooltime = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "timeBonusCooltime");

            timebonusConsecutive.value = response.timebonusConsecutive;
            timeBonusCooltime.value = response.timeBonusCooltime;
            lastCollectTimestamp.value = response.lastCollectTimestamp;

            BlackboardUtils.SetOrCreateValue<bool>(ContentBlackboard.Get(), "congratulationEffect", response.gameSpinResult.congratulationEffect);

            IBlackboard resultBB   = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "spin");
            var outputList   = BlackboardUtils.GetOrCreateVariable<List<int>>(resultBB, "outputList");
            outputList.value = new List<int>();
            outputList.value.Add(response.gameSpinResult.gameIdReelIndex);
            outputList.value.Add(response.gameSpinResult.spinCountReelIndex);
            outputList.value.Add(response.gameSpinResult.betScaleReelIndex);

            // Vip Lounge
            BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeInfo);

            ClientAPI2Blackboard.Serialize(ContentBlackboard.Get(), response);

            // BlackboardQueryUtils.AddGameSpinCount( response.gameSpinResult.gameId, response.gameSpinResult.addedSpinCount );

#if !UNITY_EDITOR
            // var pushInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "values/misc/LOCAL_PUSH/TIME_BONUS");
            // NativeHelper.Instance.DeleteLocalPush(pushInfoBB.value.GetValue<int>("ID"));
            // NativeHelper.Instance.SetLocalPush(pushInfoBB.value.GetValue<int>("ID"), pushInfoBB.value.GetValue<string>("TITLE"), pushInfoBB.value.GetValue<string>("TEXT"), pushInfoBB.value.GetValue<int>("SECONDS"), pushInfoBB.value.GetValue<string>("TYPE"), false);
#endif

            EndAction(true);
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
