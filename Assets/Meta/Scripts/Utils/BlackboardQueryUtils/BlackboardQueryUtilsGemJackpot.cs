using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static EventInfo GetOtherMetaGameEventInfo(bool allowInactive = false)
        {
            EventInfo eventInfo = GetMetaGameEventInfo(EventInfoType.GEM_JACKPOT, allowInactive);
            if (eventInfo != null)
                return eventInfo;

            return null;
        }

        public static string GetOtherMetaGameEventName()
        {
            return GetMetaGameEventName(GetOtherMetaGameEventInfo(true));
        }

        public static bool CheckGemJackPotNoDealOpen()
        {
            Blackboard bb = GemJackpotUtils.GemJackpotInfo;
            if (bb != null)
            {
                int freeSpinCount = bb.GetValue<int>("freeSpinCount");
                int nextProgress = bb.GetValue<int>("nextProgress");
                if (freeSpinCount <= 0 && nextProgress <= 0)
                    return false;

                return true;
            }
            return false;
        }

        public static bool CheckGemJackPotNoDealClose()
        {
            Blackboard bb = GemJackpotUtils.GemJackpotInfo;
            if (bb != null)
            {
                var noDealAction = bb.GetVariable<int>("noDealAction");
                if (noDealAction != null && noDealAction.value != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckGemJackPotNoDealReward()
        {
            Blackboard bb = GemJackpotUtils.GemJackpotInfo;
            if (bb != null)
            {
                var noDealAction = bb.GetVariable<int>("noDealAction");

                if (noDealAction != null)
                {
                    return (noDealAction.value == 2);
                }
            }
            return false;
        }

        public static List<SlotMaker.TestSuite.DebugSpin> GetMetaDebugSpinList()
        {
            List<SlotMaker.TestSuite.DebugSpin> ret = null;
            Blackboard bb = GemJackpotUtils.GemJackpotInfo;
            if (bb != null)
            {
                ret = BlackboardUtils.FindValue<List<SlotMaker.TestSuite.DebugSpin>>(bb, "debugSpins/list");
            }
            return ret;
        }

        public static void SetBGMPlay(bool isPlay)
        {
            GameObject mainFSMObject = SlotMaker.ScreenCapture.Instance.gameObject;
            if (mainFSMObject == null) return;

            MetaStreamingSoundController streamingSoundController = mainFSMObject.GetComponentInChildren<MetaStreamingSoundController>();
            if (streamingSoundController == null) return;

            streamingSoundController.PlayBGM(isPlay);
        }

        public static void UpdateOtherMetaGameEnterInfo(MetaGameEnterInfoV1 metaGameEnterInfo)
        {
            if (metaGameEnterInfo != null && metaGameEnterInfo.gemJackpotEnterInfo != null)
            {
                var metaGameEnterInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "metaGameEnterInfo");
                var otherMetaGameEnterInfoBB = BlackboardUtils.GetOrCreateBlackboard(metaGameEnterInfoBB, "gemJackpotEnterInfo");
                ClientAPI2Blackboard.Serialize(otherMetaGameEnterInfoBB, metaGameEnterInfo.gemJackpotEnterInfo);
            }
        }
    }
}
