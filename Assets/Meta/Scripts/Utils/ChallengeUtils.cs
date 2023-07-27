using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public static class ChallengeUtils
    {
        public enum MissionProgressType
        {
            NONE = 0,
            COUNT, // SPIN, WIN
            COIN,
            CURRENCY,
        }

        public static bool IsEventChallengePassiveEvent(EventInfo eventInfo)
        {
            if (eventInfo == null) return false;
            return eventInfo.type == EventInfoType.PERSONAL_EVENT_CHALLENGE || eventInfo.type == EventInfoType.CLUB_EVENT_CHALLENGE;
        }

        public static void SetAllChallengeCheckTimeCurrent()
        {
            var types = (MetaChallengeType[])System.Enum.GetValues(typeof(MetaChallengeType));

            for(int i = 0; i < types.Length; ++i)
                SetChallengeCheckTimeCurrent(types[i]);
        }

        public static void SetChallengeCheckTimeCurrent(MetaChallengeType challengeType)
        {
            long currentTimestamp = TimeUtils.GetTimeStamp();
            PlayerPrefsUtils.SetInt64("CHALLENGE_CHECK_TIME_" + challengeType.ToString(), currentTimestamp);
        }

        public static long GetChallengeCheckTime(MetaChallengeType challengeType)
        {
            return PlayerPrefsUtils.GetInt64("CHALLENGE_CHECK_TIME_" + challengeType.ToString(), 0L);
        }

        public static bool IsChallengeTypeEvent(MetaChallengeType challengeType)
        {
            return challengeType == MetaChallengeType.EVENT_CLUB ||
                challengeType == MetaChallengeType.EVENT_PERSONAL;
        }

        public static bool IsChallengeTypePersonal(MetaChallengeType challengeType)
        {
            return challengeType == MetaChallengeType.DAILY ||
                challengeType == MetaChallengeType.EXPERT ||
                challengeType == MetaChallengeType.MASTER ||
                challengeType == MetaChallengeType.EVENT_PERSONAL;
        }

        public static void MetaChallengeTypeToChallengeType(MetaChallengeType type, out ChallengeType personalType, out ClubChallengeType clubType)
        {
            personalType = ChallengeType.UNKNOWN;
            clubType = ClubChallengeType.UNKNOWN;

            switch (type)
            {
                case MetaChallengeType.DAILY:
                    personalType = ChallengeType.DAILY;
                    break;
                case MetaChallengeType.EXPERT:
                    personalType = ChallengeType.EXPERT;
                    break;
                case MetaChallengeType.MASTER:
                    personalType = ChallengeType.MASTER;
                    break;
                case MetaChallengeType.EVENT_PERSONAL:
                    personalType = ChallengeType.EVENT;
                    break;
                case MetaChallengeType.CLUB:
                    clubType = ClubChallengeType.NORMAL;
                    break;
                case MetaChallengeType.EVENT_CLUB:
                    clubType = ClubChallengeType.EVENT;
                    break;
            }
        }

        public static MetaChallengeType PersonalChallengeTypeToMetaChallengeType(ChallengeType type)
        {
            PersonalChallengeTypeToMetaChallengeType(type, out MetaChallengeType result);
            return result;
        }

        public static MetaChallengeType ClubChallengeTypeToMetaChallengeType(ClubChallengeType type)
        {
            ClubChallengeTypeToMetaChallengeType(type, out MetaChallengeType result);
            return result;
        }

        public static void PersonalChallengeTypeToMetaChallengeType(ChallengeType type, out MetaChallengeType challengeType)
        {
            switch (type)
            {
                case ChallengeType.DAILY:
                    challengeType = MetaChallengeType.DAILY;
                    break;
                case ChallengeType.EXPERT:
                    challengeType = MetaChallengeType.EXPERT;
                    break;
                case ChallengeType.MASTER:
                    challengeType = MetaChallengeType.MASTER;
                    break;
                case ChallengeType.EVENT:
                    challengeType = MetaChallengeType.EVENT_PERSONAL;
                    break;
                default:
                    challengeType = MetaChallengeType.NONE;
                    break;
            }
        }

        public static void ClubChallengeTypeToMetaChallengeType(ClubChallengeType type, out MetaChallengeType challengeType)
        {
            switch (type)
            {
                case ClubChallengeType.NORMAL:
                    challengeType = MetaChallengeType.CLUB;
                    break;
                case ClubChallengeType.EVENT:
                    challengeType = MetaChallengeType.EVENT_CLUB;
                    break;
                default:
                    challengeType = MetaChallengeType.NONE;
                    break;
            }
        }

        public static string GetChallengeMissionTitle(Blackboard missionInfo)
        {
            ChallengeMissionType type = missionInfo.GetValue<ChallengeMissionType>("missionType");

            var winTypeName = missionInfo.GetVariable<string>("winType")?.value ?? "";
            WinType winType = WinType.UNKNOWN;
            if (!string.IsNullOrEmpty(winTypeName))
            {
                winType = (WinType)System.Enum.Parse(typeof(WinType), winTypeName, true);
            }

            const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

            long completeCount = BlackboardUtils.GetOrCreateVariable<long>(missionInfo, "completeCount")?.value ?? 0L;

            switch (type)
            {
                case ChallengeMissionType.WIN_ANY:
                case ChallengeMissionType.WIN_TARGETED:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_WIN_COUNT");
                case ChallengeMissionType.SPIN_ANY:
                case ChallengeMissionType.SPIN_TARGETED:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_SPIN_COUNT");
                case ChallengeMissionType.WIN_BIG_WIN_ANY:
                case ChallengeMissionType.WIN_BIG_WIN_TARGETED:
                    return StringTableUtils.GetString(GLOBAL, string.Format("POPUP_CHALLENGE_MISSION_{0}_WIN", winType.ToString()));
                case ChallengeMissionType.ENTER_FREE_SPIN_ANY:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME");
                case ChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME");
                // case ChallengeMissionType.SPIN_WITH_MAX_BET_ANY:
                // case ChallengeMissionType.SPIN_WITH_MAX_BET_TARGETED:
                //     title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_MAX_BET_SPIN");
                //     break;
                case ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
                case ChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_SPIN_WIN");
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_TOTAL_WIN");
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                    return StringTableUtils.GetString(GLOBAL, string.Format("POPUP_CHALLENGE_MISSION_TOTAL_{0}_WIN", winType.ToString()));
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_TOTAL_FREE_GAME_WIN");
                case ChallengeMissionType.COLLECT_TIMEBONUS:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_COLLECT_TIME_BONUS");
                case ChallengeMissionType.ACHIEVE_TOTAL_WAGER_CREDIT_ANY:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_WAGER", completeCount);
                case ChallengeMissionType.ACHIEVE_TOTAL_WAGER_CREDIT_ANY_WITH_BET_LIMIT:
                    {
                        long betLimit = BlackboardUtils.GetOrCreateVariable<long>(missionInfo, "betLimit")?.value ?? 0L;
                        return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_LIMIT_WAGER", completeCount, betLimit);
                    }
                case ChallengeMissionType.SPIN_WITH_BET_LIMIT:
                    {
                        long betLimit = BlackboardUtils.GetOrCreateVariable<long>(missionInfo, "betLimit")?.value ?? 0L;
                        return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_LIMIT_SPIN", completeCount, betLimit);
                    }
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY_WITH_BET_LIMIT:
                    {
                        long betLimit = BlackboardUtils.GetOrCreateVariable<long>(missionInfo, "betLimit")?.value ?? 0L;
                        return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_LIMIT_WIN", completeCount, betLimit);
                    }
                case ChallengeMissionType.COLLECT_LUCKY_SPINS:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_LUCKY_SPIN");
                case ChallengeMissionType.PURCHASE_ANY:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PURCHASE_COUNT");
                case ChallengeMissionType.PURCHASE_PRICE_MORE_THAN:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PURCHASE_PAY");
                case ChallengeMissionType.ACHIEVE_TOTAL_PURCHASE_PRICE:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PURCHASE_TOTAL_PAY");
                case ChallengeMissionType.WATCH_VIDEO_ADS:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_WATCH_VIDEO_ADS");
                case ChallengeMissionType.CONNECT_FACEBOOK:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_CONNECT_FACEBOOK");
                case ChallengeMissionType.ADD_FRIENDS:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_ADD_FRIENDS");
                case ChallengeMissionType.JOIN_CLUB:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_JOIN_CLUB");
                case ChallengeMissionType.CONNECT_EMAIL:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_CONNECT_EMAIL");
                case ChallengeMissionType.VIP_CLUB:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_CONNECT_VIP");
                case ChallengeMissionType.USE_GEM_MORE_THAN:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_USE_GEM_MORE_THAN");
                case ChallengeMissionType.HIDDEN_UNIVERSE_COLLECT_FINDERS:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_GET_FINDERS", completeCount);
                case ChallengeMissionType.LUCKY_FIVE_COLLECT_CARDS:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_GET_CARDS", completeCount);
                case ChallengeMissionType.LUCKY_FIVE_COLLECT_WILD_CARDS:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_GET_WILD_CARDS", completeCount);
                case ChallengeMissionType.COLLECTING_GAME_SCRATCH_SCRATCHER:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_SCRATCH", completeCount);
                case ChallengeMissionType.COLLECTING_GAME_COLLECT_CHESTS:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_GET_CHEST", completeCount);
                case ChallengeMissionType.GEM_JACKPOT_SPIN:
                    return StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_SPIN_GOLD_TOWER", completeCount);
                default:
                    return "UNKNOWN";
            }
        }

        public static string ClubChallengeMissionTypeToText(Blackboard missionInfo)
        {
            ClubChallengeMissionType type = missionInfo.GetValue<ClubChallengeMissionType>("missionType");

            var winTypeName = missionInfo.GetVariable<string>("winType")?.value ?? "";
            WinType winType = WinType.UNKNOWN;
            if (!string.IsNullOrEmpty(winTypeName))
            {
                winType = (WinType)System.Enum.Parse(typeof(WinType), winTypeName, true);
            }

            switch (type)
            {
                case ClubChallengeMissionType.WIN_ANY:
                case ClubChallengeMissionType.WIN_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_WIN_COUNT");
                case ClubChallengeMissionType.SPIN_ANY:
                case ClubChallengeMissionType.SPIN_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_SPIN_COUNT");
                case ClubChallengeMissionType.WIN_BIG_WIN_ANY:
                case ClubChallengeMissionType.WIN_BIG_WIN_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, string.Format("POPUP_CHALLENGE_MISSION_{0}_WIN", winType.ToString()));
                case ClubChallengeMissionType.ENTER_FREE_SPIN_ANY:
                case ClubChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TRIGGER_FREE_GAME");
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_WIN");
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, string.Format("POPUP_CHALLENGE_MISSION_TOTAL_{0}_WIN", winType.ToString()));
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_FREE_GAME_WIN");
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_ANY");
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_TARGETED");
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_TOTAL_LP_FREE_GAMES");
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_TARGETED:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, string.Format("POPUP_CHALLENGE_MISSION_TOTAL_LP_{0}_WIN", winType.ToString()));
                default:
                    return "UNKNOWN";
            }
        }

        public static MissionProgressType ChallengeMissionTypeToProgressType(ChallengeMissionType type)
        {
            switch (type)
            {
                case ChallengeMissionType.WIN_ANY:
                case ChallengeMissionType.WIN_TARGETED:
                case ChallengeMissionType.SPIN_ANY:
                case ChallengeMissionType.SPIN_TARGETED:
                case ChallengeMissionType.WIN_BIG_WIN_ANY:
                case ChallengeMissionType.WIN_BIG_WIN_TARGETED:
                case ChallengeMissionType.ENTER_FREE_SPIN_ANY:
                case ChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                // case ChallengeMissionType.SPIN_WITH_MAX_BET_ANY:
                // case ChallengeMissionType.SPIN_WITH_MAX_BET_TARGETED:
                //     title.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_MISSION_MAX_BET_SPIN");
                //     break;
                case ChallengeMissionType.COLLECT_TIMEBONUS:
                case ChallengeMissionType.COLLECT_LUCKY_SPINS:
                case ChallengeMissionType.WATCH_VIDEO_ADS:
                case ChallengeMissionType.CONNECT_FACEBOOK:
                case ChallengeMissionType.ADD_FRIENDS:
                case ChallengeMissionType.JOIN_CLUB:
                case ChallengeMissionType.CONNECT_EMAIL:
                case ChallengeMissionType.VIP_CLUB:
                case ChallengeMissionType.PURCHASE_ANY:
                    return MissionProgressType.COUNT;
                case ChallengeMissionType.PURCHASE_PRICE_MORE_THAN:
                case ChallengeMissionType.ACHIEVE_TOTAL_PURCHASE_PRICE:
                    return MissionProgressType.CURRENCY;
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                case ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
                case ChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED:
                    return MissionProgressType.COIN;
                default:
                    return MissionProgressType.NONE;
            }
        }

        public static MissionProgressType ChallengeMissionTypeToProgressType(ClubChallengeMissionType type)
        {
            switch (type)
            {
                case ClubChallengeMissionType.WIN_ANY:
                case ClubChallengeMissionType.WIN_TARGETED:
                case ClubChallengeMissionType.SPIN_ANY:
                case ClubChallengeMissionType.SPIN_TARGETED:
                case ClubChallengeMissionType.WIN_BIG_WIN_ANY:
                case ClubChallengeMissionType.WIN_BIG_WIN_TARGETED:
                case ClubChallengeMissionType.ENTER_FREE_SPIN_ANY:
                case ClubChallengeMissionType.ENTER_FREE_SPIN_TARGETED:
                    return MissionProgressType.COUNT;
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_TARGETED:
                    return MissionProgressType.COIN;
                default:
                    return MissionProgressType.NONE;
            }
        }

        public static IEnumerator RequestChallengeClaim(Blackboard challengeInfo, Blackboard callerBB)
        {

            if(challengeInfo == null)
            {
                if (ApplicationSettings.LogTest())
                    Debug.Log("Failure ChallengeUtils.RequestChallengeClaim");
                yield break;
            }
            else
            {
                if (ApplicationSettings.LogTest())
                    Debug.Log("Try ChallengeUtils.RequestChallengeClaim");
            }

            var claimChallengetype = BlackboardUtils.FindVariable<ChallengeType>(challengeInfo, "challengeType");

            int multiplierEventID = 0;
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);
            if (eventInfo != null)
                multiplierEventID = eventInfo.id;

            bool isSuccess = false;
            bool isFail = false;
            BagelCodeClientAPI.ChallengeClaim(claimChallengetype.value, multiplierEventID,
            (response) =>
            {
                if (callerBB != null)
                {
                    var claimResponse = BlackboardUtils.GetOrCreateBlackboard(callerBB, "claimResponse");

                    ClientAPI2Blackboard.Serialize(claimResponse, response);
                    ClientAPI2Blackboard.Serialize(challengeInfo, response.nextChallengeInfo);
                    BlackboardUtils.SetOrCreateList(challengeInfo, "lastRewardResultList", response.rewardResultList, ClientAPI2Blackboard.Serialize);

                    BlackboardQueryUtils.UpdateChallengeSimpleInfo(challengeInfo);

                    PersonalChallengeTypeToMetaChallengeType(claimChallengetype.value, out MetaChallengeType challengeType);
                    SetChallengeCheckTimeCurrent(challengeType);
                }

                isSuccess = true;
            },
            (error) =>
            {
                Debug.LogError("RequestChallengeClaim failure: " + error.error);
                GlobalErrorHandler.GlobalError(error);

                isFail = true;
            });

            yield return new WaitUntil(() => isSuccess || isFail);

            if (callerBB != null)
                callerBB.AddVariable("isSuccessClaimRequest", isSuccess);

            if (ApplicationSettings.LogTest())
                Debug.Log("Done ChallengeUtils.RequestChallengeClaim");
        }

        public static IEnumerator RequestChallengeInfo(Blackboard bb, GameObject caller = null)
        {
            if(bb == null)
            {
                if (ApplicationSettings.LogTest())
                    Debug.Log("Failure ChallengeUtils.RequestChallengeInfo: bb is null");
                yield break;
            }
            else
            {
                if (ApplicationSettings.LogTest())
                    Debug.Log("Try ChallengeUtils.RequestChallengeInfo");
            }

            bool isSuccess = false;
            bool isFail = false;

            BagelCodeClientAPI.ChallengeInfo(
            (response) =>
            {
                if(bb != null)
                {
                    var infoBB = BlackboardUtils.GetOrCreateBlackboard(bb, "challengeInfoResponse");

                    ClientAPI2Blackboard.Serialize(infoBB, response);

                    // update simple info..
                    var challengeInfoList = infoBB.GetValue<List<Blackboard>>("challengeInfoList");

                    for (int i = 0; i < challengeInfoList.Count; ++i)
                    {
                        var info = challengeInfoList[i];
                        PersonalChallengeTypeToMetaChallengeType(info.GetValue<ChallengeType>("challengeType"), out MetaChallengeType challengeType);
                        BlackboardQueryUtils.UpdateChallengeSimpleInfo(info);
                    }

                    var clubEventChallengeInfo = infoBB.GetVariable<Blackboard>("clubEventChallengeInfo")?.value;
                    if(clubEventChallengeInfo != null)
                    {
                        BlackboardQueryUtils.UpdateChallengeSimpleInfo(clubEventChallengeInfo);
                    }

                    var clubChallengeInfo = infoBB.GetVariable<Blackboard>("clubChallengeInfo")?.value;
                    ClubUtils.SetIsClubber(clubChallengeInfo != null);

                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, ChallengeEventManager.ON_REFRESH_CHALLENGE_INFO);
                }

                isSuccess = true;
            },
            (error) =>
            {
                Debug.LogError("RequestChallengeInfo failure.");
                GlobalErrorHandler.GlobalError(error);
                isFail = true;
            });

            yield return new WaitUntil(() => isSuccess || isFail);

            if(caller != null)
            {
                var callerBB = caller.GetComponent<Blackboard>();
                if (callerBB != null)
                    callerBB.AddVariable("isChallengeInfoRequestSuccess", isSuccess);
            }

            if (ApplicationSettings.LogTest())
                Debug.Log("Done ChallengeUtils.RequestChallengeInfo");
        }

        public static EventInfo GetPreferredPassiveEventInfo()
        {
            EventInfo personalEventChallengeInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PERSONAL_EVENT_CHALLENGE);
            EventInfo clubEventChallengeInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CLUB_EVENT_CHALLENGE);

            bool isPersonalEventChallengeAvailable = BlackboardQueryUtils.IsAvailablePassiveEvent(personalEventChallengeInfo);
            bool isClubEventChallengeAvailable = BlackboardQueryUtils.IsAvailablePassiveEvent(clubEventChallengeInfo);

            bool isClubber = ClubUtils.IsClubber();

            if (isClubEventChallengeAvailable && isClubber)
            {
                return clubEventChallengeInfo;
            }
            else if (isPersonalEventChallengeAvailable && !isClubber)
            {
                return personalEventChallengeInfo;
            }
            else if(isPersonalEventChallengeAvailable && isClubber)
            {
                return personalEventChallengeInfo;
            }
            else if (isClubEventChallengeAvailable && !isClubber) // NonClubber
            {
                return clubEventChallengeInfo;
            }
            else
            {
                return PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);
            }
        }

        public static IEnumerator RequestClubMissionInfoCoroutine(Blackboard ownerBB, int missionDate, string missionId)
        {
            bool isDone = false;

            BagelCodeClientAPI.RequestClubMissionInfo(missionDate, missionId,
            (response) =>
            {
                if (ownerBB != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(ownerBB, "clubMissionResponse");

                    BlackboardUtils.ClearBlackboard(bb);
                    ClientAPI2Blackboard.Serialize(bb, response);
                    isDone = true;
                }
            },
            (error) =>
            {
                switch (error.errorCode)
                {
                    case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                        GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                        if (ownerBB != null)
                            isDone = true;
                        break;
                    case ClientModels.Error.NOT_IN_CLUB_ERROR:
                        {
                            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                            if (meClubID.value > 0)
                            {
                                bool stringError = false;
                                ErrorPopupInfo info = new ErrorPopupInfo();
                                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                                info.type = ErrorPopupType.OK;
                                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                                ErrorPopupHandler.Instance.OpenError(info);
                            }

                            meClubID.value = 0;
                            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                        }
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
            });

            yield return new WaitUntil(() => isDone);
        }

        public static IEnumerator RequestChallengeEpicPassInfo(GameObject caller = null)
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
            if (metaGameInfo == null)
                yield break;

            bool isSuccess = false;
            bool isFail = false;

            BagelCodeClientAPI.SeasonPassInfoRequestV2(metaGameInfo.id,
                (response) =>
                {
                    EpicPassUtilsV2.UpdateSeasonPassInfo(response);
                    isSuccess = true;
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.INVALID_SEASON_PASS_REQUEST_ERROR:
                            //EndAction(false);
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                    isFail = true;
                });
            yield return new WaitUntil(() => isSuccess || isFail);

            if (caller != null)
            {
                var callerBB = caller.GetComponent<Blackboard>();
                if (callerBB != null)
                    callerBB.AddVariable("isEpicPassInfoRequestSuccess", isSuccess);
            }
        }
    }
}
