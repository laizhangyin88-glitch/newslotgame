using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.LevelUpDash
{
    public static class LevelUpDash
    {
        public static class Utils
        {
            public static Blackboard InfoBB =>
                BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "levelUpDashInfo")?.value;

            public static Blackboard MissionBB =>
                (InfoBB != null) ? BlackboardUtils.FindVariable<Blackboard>(InfoBB, "missionExtendedPersonal")?.value : null;

            public static long EndTimestamp =>
                (MissionBB != null) ? (BlackboardUtils.FindVariable<long>(MissionBB, "missionEndTimestamp")?.value ?? 0L) : 0L;

            public static int EventID =>
                (MissionBB != null) ? (BlackboardUtils.FindVariable<int>(MissionBB, "eventId"))?.value ?? 0 : 0;

            public static List<Blackboard> StageList =>
                (MissionBB != null) ? (BlackboardUtils.FindVariable<List<Blackboard>>(MissionBB, "stageList")?.value) : null;

            public static int StageCount => StageList?.Count ?? 0;

            public static Blackboard PurchaseBoosterBB =>
                (InfoBB != null) ? BlackboardUtils.FindVariable<Blackboard>(InfoBB, "anyPurchaseBoosterPersonal")?.value : null;

            public static long PurchaseBoosterEndTimestamp =>
                (PurchaseBoosterBB != null) ? (BlackboardUtils.FindVariable<long>(PurchaseBoosterBB, "endTimestamp")?.value ?? 0L) : 0L;

            public static long PurchaseBoosterMultiplierNumerator =>
                (PurchaseBoosterBB != null) ? (BlackboardUtils.FindVariable<long>(PurchaseBoosterBB, "expMultiplyNumerator")?.value ?? 100L) : 100L;

            //

            public static int GetMissionStartLevel()
            {
                var missionBB = MissionBB;
                if (missionBB != null)
                {
                    var variable = BlackboardUtils.GetOrCreateVariable<int>(missionBB, "missionStartLevel");
                    if (variable != null)
                    {
                        if (variable.value == 0)
                        {
                            variable.value = BlackboardQueryUtils.GetMyLevel();
                        }
                        return variable.value;
                    }
                }

                return BlackboardQueryUtils.GetMyLevel();
            }

            public static int GetCurrentStageIndex()
            {
                if (IsActiveLevelUpDash())
                {
                    var stageList = StageList;
                    for (int i = 0; i < stageList.Count; ++i)
                    {
                        int level = BlackboardQueryUtils.GetMyLevel();
                        int targetLevel = BlackboardUtils.FindVariable<int>(stageList[i], "targetLevel")?.value ?? 0;

                        if (level < targetLevel) return i;
                    }
                }

                return -1;
            }

            // stageIndex -1 : return nearest target level
            public static int GetTargetMissionLevel(int stageIndex = -1)
            {
                int level = BlackboardQueryUtils.GetMyLevel();
                var stageList = StageList;
                if (stageList != null)
                {
                    if (stageList.IsValidIndex(stageIndex))
                    {
                        return BlackboardUtils.FindVariable<int>(stageList[stageIndex], "targetLevel")?.value ?? 0;
                    }
                    else
                    {
                        for (int i = 0; i < stageList.Count; ++i)
                        {
                            int targetLevel = BlackboardUtils.FindVariable<int>(stageList[i], "targetLevel")?.value ?? 0;
                            if (level < targetLevel) return targetLevel;
                        }
                    }
                }

                return -1;
            }

            public static bool IsEnabledPurchaseBooster()
            {
                if (IsActiveLevelUpDash())
                {
                    long multiplierNumerator = PurchaseBoosterMultiplierNumerator;
                    bool enabled = BlackboardUtils.FindVariable<bool>(MissionBB, "isAnyPurchaseBoosterEnabled")?.value ?? false;
                    return multiplierNumerator > 100L && enabled;
                }

                return false;
            }

            public static bool IsLevelRemainingLessThan(int remaining)
            {
                int level = BlackboardQueryUtils.GetMyLevel();
                int targetLevel = GetTargetMissionLevel();
                return targetLevel >= 0 && targetLevel - level < remaining;
            }

            public static bool IsActivePurchaseBooster()
            {
                return PurchaseBoosterEndTimestamp > TimeUtils.GetTimeStamp();
            }

            public static bool IsActiveLevelUpDashCampaign()
            {
                EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.LEVEL_UP_DASH);
                return eventInfo != null;
            }

            public static bool IsActiveLevelUpDash()
            {
                EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.LEVEL_UP_DASH_MISSION);
                long remaining = EndTimestamp - TimeUtils.GetTimeStamp();
                return eventInfo != null && remaining > 0L;
            }

            public static void UpdateLevelUpDashInfo(LevelUpDashInfoOnSpin info)
            {
                if (info == null) return;

                bool isNew = false;
                if (MissionBB == null) isNew = true;

                Blackboard infoBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "levelUpDashInfo");

                // Mission
                if (info.missionExtendedPersonal != null)
                {
                    Blackboard missionBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(infoBB, "missionExtendedPersonal");
                    ClientAPI2Blackboard.Serialize(missionBB, info.missionExtendedPersonal);
                }

                // Purchase Booster
                if (info.anyPurchaseBoosterPersonal != null)
                {
                    Blackboard purchaseBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(infoBB, "anyPurchaseBoosterPersonal");
                    ClientAPI2Blackboard.Serialize(purchaseBB,info.anyPurchaseBoosterPersonal);
                }

                // Reward
                if (info.missionStageRewardResultList != null && info.missionStageRewardResultList.Count > 0)
                {
                    // todo use MetaBlackboardUtils.SerializeList instead of this
                    BlackboardUtils.GetOrCreateBlackboardList(infoBB, "missionStageRewardResults");
                    foreach (var rewardResult in info.missionStageRewardResultList)
                    {
                        var rewardBB = BlackboardUtils.CreateBlackboard("reward");
                        ClientAPI2Blackboard.Serialize(rewardBB, rewardResult);
                        BlackboardUtils.AddToBlackboardList(infoBB, "missionStageRewardResults", rewardBB);
                    }
                }

                if (isNew)
                {
                    BlackboardUtils.SetOrCreateValue(InfoBB, "isNewOpen", true);
                    UpdateLevelUpDashState();
                }

                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, Events.ON_UPDATE_LEVEL_UP_DASH);
            }

            public static void ClearMissionRewardResultList()
            {
                BlackboardUtils.DestroyBlackboardList(InfoBB, "missionStageRewardResults");
            }

            public static List<Blackboard> GetCurrentMissionRewardResultList()
            {
                var infoBB = InfoBB;
                if (infoBB != null)
                    return BlackboardUtils.FindVariable<List<Blackboard>>(infoBB, "missionStageRewardResults")?.value;

                return null;
            }

            public static void UpdateExpBoosterEndTimestamp(long endTimestamp)
            {
                if(PurchaseBoosterEndTimestamp < endTimestamp &&
                    PurchaseBoosterBB != null)
                {
                    BlackboardUtils.SetOrCreateValue(PurchaseBoosterBB, "endTimestamp", endTimestamp);
                    EventSender.SendGlobalMetaEvent(Events.ON_UPDATE_LEVEL_UP_DASH);
                }
            }

            // todo:meta system에서 하도록 수정
            // lobby->ingame 로딩 중 level dash 끝난 경우 처리가 안될 듯..
            public static void FinishLevelUpDash() 
            {
                // Reward 가 남아있는 경우 무시
                if (GetCurrentMissionRewardResultList() != null) return;

                BlackboardUtils.DestroyBlackboard(InfoBB, "missionExtendedPersonal");

                EventSender.SendGlobalMetaEvent(Events.ON_UPDATE_LEVEL_UP_DASH);
            }

            public static void UpdateLevelUpDashState()
            {
                var stageList = StageList;
                if (stageList != null)
                {
                    for (int i = 0; i < stageList.Count; ++i)
                    {
                        int targetLevel = BlackboardUtils.FindVariable<int>(stageList[i], "targetLevel")?.value ?? 0;
                        int level = BlackboardQueryUtils.GetMyLevel();
                        BlackboardUtils.SetOrCreateValue(stageList[i], "isClaimedReward", targetLevel <= level);
                    }
                }

                GetMissionStartLevel();
            }
        }

        public static class Defines
        {
            // Player Prefs
            public const string PLAYER_PREFS_IS_FIRST_ENTER_FORMAT = "LEVEL_UP_DASH_IS_FIRST_ENTER_{0}"; // eventID

            // Sounds
            public const string SOUND_BGM = "Meta_Level_Up_Dash_BGM";
            public const string SOUND_UNLOCK = "Meta_Level_Up_Dash_Unlock";
            public const string SOUND_COLLECT_REWARD = "Meta_Level_Up_Dash_Collect";
            public const string SOUND_COLLECT_FINAL_REWARD = "Meta_Level_Up_Dash_Collect_Final";
            public const string SOUND_CHECK_REWARD_BOX = "Meta_Level_Up_Dash_Check_Reward_Box";
            public const string SOUND_GAUGE_INCREASE = "Meta_Level_Up_Dash_Gauge_Increase";

            // Values
            public const int MAX_REMAINING_LEVEL_FOR_DISPLAYING_LEFT_LEVEL = 3;
        }

        public static class Events
        {
            // MetaUIEvent
            public const string ON_UPDATE_LEVEL_UP_DASH = "OnUpdateLevelUpDash";
            public const string ACTIVE_LEVEL_UP_DASH = "ActiveLevelUpDash";
            public const string DEACTIVE_LEVEL_UP_DASH = "DeactiveLevelUpDash";
            public const string OPEN_LEVEL_BOOST_IAM = "OpenLevelBoostIam";
            public const string ON_CLOSE_LEVEL_BOOST_IAM = "OnCloseLevelBoostIam";
            public const string ON_CANCEL_LEVEL_BOOST_IAM = "OnCancelLevelBoostIam";
            public const string OPEN_LEVEL_UP_DASH = "OpenLevelUpDash";
            public const string CHECK_LEVEL_UP_DASH_REWARD = "CheckLevelUpDashReward";
            public const string NOTIFY_LEVEL_UP_DASH_ENDED = "NotifyLevelUpDashEnded";
            public const string FINISH_CHECK_REWARD = "FinishCheckReward";

            // Custom
            public const string ON_END_PURCHASE_BOOSTER = "OnEndPurchaseBooster";
            public const string ON_DISPLAY_LEVEL_DASH_CLOSED = "OnDisplayLevelDashClosed";
        }
    }
}
