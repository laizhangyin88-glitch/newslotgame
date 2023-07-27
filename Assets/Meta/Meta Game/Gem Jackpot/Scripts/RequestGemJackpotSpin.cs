using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.GemJackpot
{
    [Category("★ BagelCode/Meta Games/Gem Jackpot")]
    public class RequestGemJackpotSpin : ActionTask
    {
        public BBParameter<bool> saveAsSuccess;

        protected override string info
        {
            get { return "Request Gem Jackpot Spin"; }
        }

        protected override void OnExecute()
        {
            saveAsSuccess.value = false;
#if DEV
            var metaBB = GemJackpotUtils.GemJackpotInfo;
            var index = BlackboardUtils.GetOrCreateVariable<int>(metaBB, "debugIndex");
            BagelCodeClientAPI.RequestGemJackpotDebugSpin(index.value, GemJackpotUtils.BISlotEnterContextID,
#else
            BagelCodeClientAPI.RequestGemJackpotSpin(GemJackpotUtils.BISlotEnterContextID,
#endif
            (response) =>
                {
                    var bb = GemJackpotUtils.GemJackpotInfo;
                    GemJackpotUtils.PrevProgress = GemJackpotUtils.NextProgress;
                    if (response.userSyncInfo.gem < 0L)
                    {
                        response.userSyncInfo.gem = 0L;
                        //Debug.Log("Gem < 0 !!!!!!!!!!!!!");
                    }
                    ClientAPI2Blackboard.Serialize(bb, response);

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();

                    List<int> slotReelSetResultList = GemJackpotUtils.SlotReelSetResultIndexList;
                    if (slotReelSetResultList != null)
                    {
                        List<int> outputList = new List<int>();
                        for(int i = 0; i < slotReelSetResultList.Count; ++i)
                        {
                            outputList.Add(slotReelSetResultList[i]);
                        }

                        BlackboardUtils.SetOrCreateValue<List<int>>(bb, "outputList", outputList);
                    }

                    GemJackpotUtils.UpdateJackpotInfo(GemJackpotUtils.PrevGrandJackpotMultiplyNumerator);

                    BlackboardUtils.DestroyBlackboard(bb, "winList");

                    saveAsSuccess.value = true;
#if DEV
                    var debugIndex = BlackboardUtils.GetOrCreateVariable<int>(bb, "debugIndex");
                    debugIndex.value = 0;
#endif
                    EndAction();
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        //case ClientModels.Error.INVALID_GEM_JACKPOT_REQUEST_ERROR:
                        //case ClientModels.Error.NOT_ENOUGH_GEM_ERROR:
                        //    saveAsSuccess.value = false;
                        //    EndAction(false);
                        //    break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });
        }
    }
}