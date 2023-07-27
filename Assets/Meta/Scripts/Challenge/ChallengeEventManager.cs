using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public class ChallengeEventManager : EventMonoBehaviour
    {
        public const string ON_OPEN_LEADERS_POPUP = "OnOpenLeadersPopup";
        public const string OPEN_CHALLENGE_POPUP = "OpenChallengePopup";
        public const string ON_OPEN_CHALLENGE_POPUP = "OnOpenChallengePopup";
        public const string ON_REMOVED_CLUB = "OnRemovedClub";
        public const string ON_CLUB = "OnClub";
        public const string ON_GO_TO_CLUB = "OnGoToClub";
        public const string ON_CLOSE = "OnClose";
        public const string ON_REFRESH_CHALLENGE_INFO = "OnRefreshChallengeInfo";
        public const string ON_END_TIMER = "OnEndTimer";
        public const string ON_UPDATE_CHALLENGE_POPUP = "OnUpdateChallengePopup";
        public const string CLOSE_CHALLENGE_POPUP = "CloseChallengePopup";

        public const string challengeEventListName = "challengeEventList";
        public const string shootChallengeEventListName = "shootChallengeEventList";

        public const string ON_CHECK_COMPLETE_MISSION = "OnCheckCompleteMission";
        public const string ON_SHOOT_CHALLENGE_MISSION = "OnShootChallengeMission";
        public const string ON_COMPLETE_CHALLENGE_MISSION_EFFECT = "OnCompleteChallengeMissionEffect";
        public const string ON_CLEAR_CHALLENGE_LIST = "OnClearChallengeList";

        public const string ON_COMPLETE_CHALLENGE_MISSION = "OnCompleteChallengeMission";
        public const string ON_CLUB_COMPLETE_CHALLENGE_MISSION = "OnCompleteClubChallengeMission";
        public const string ON_RECEIVE_COMPLETE_MISSION = "OnReceiveCompleteMission";

        public const string ON_START_COMPLETE_CHALLENGE_POPUP = "OnStartCompleteChallengePopup";
        public const string ON_END_COMPLETE_CHALLENGE_POPUP = "OnEndCompleteChallengePopup";

        public const string ON_START_COMPLETE_CLUB_CHALLENGE_POPUP = "OnStartCompleteClubChallengePopup";

        public const string ON_ENABLE_EVENT_CHALLENGE = "OnEnableEventChallenge";

        private static List<MetaChallengeType> challengeTypeList;
        public static List<MetaChallengeType> ChallengeTypeList
        {
            get
            {
                if (challengeTypeList == null)
                    InitChallengeTypeList();

                return challengeTypeList;
            }
        }

        public void OnRefreshPassiveEventChallenge()
        {
            InitChallengeTypeList();
        }

        public static void InitChallengeTypeList(Blackboard challengeResponse = null)
        {
            challengeTypeList = new List<MetaChallengeType>();

            // Check Event Challenge
            EventInfo targetEventInfo = ChallengeUtils.GetPreferredPassiveEventInfo();
            if (targetEventInfo != null)
            {
                if (challengeResponse != null)
                {
                    if (targetEventInfo.type == EventInfoType.PERSONAL_EVENT_CHALLENGE)
                    {
                        var personalChallengeInfo = BlackboardQueryUtils.GetEventChallengeInfoBB(challengeResponse, true);
                        if (personalChallengeInfo != null)
                            challengeTypeList.Add(MetaChallengeType.EVENT_PERSONAL);
                    }
                    else if (targetEventInfo.type == EventInfoType.CLUB_EVENT_CHALLENGE)
                    {
                        bool isClubber = ClubUtils.IsClubber();
                        var clubChallengeInfo = BlackboardQueryUtils.GetEventChallengeInfoBB(challengeResponse, false);
                        if (clubChallengeInfo != null || !isClubber)
                            challengeTypeList.Add(MetaChallengeType.EVENT_CLUB);
                    }
                }
                else
                {
                    if (targetEventInfo.type == EventInfoType.PERSONAL_EVENT_CHALLENGE)
                    {
                        challengeTypeList.Add(MetaChallengeType.EVENT_PERSONAL);
                    }
                    else if (targetEventInfo.type == EventInfoType.CLUB_EVENT_CHALLENGE)
                    {
                        challengeTypeList.Add(MetaChallengeType.EVENT_CLUB);
                    }
                }
            }

            challengeTypeList.Add(MetaChallengeType.DAILY);
            challengeTypeList.Add(MetaChallengeType.EXPERT);
            challengeTypeList.Add(MetaChallengeType.MASTER);
            challengeTypeList.Add(MetaChallengeType.CLUB);
            if (MetaGameUtils.IsEpicPassAlwayslLock() && !MetaGameUtils.IsMetaGameLevelLocked())
            {
                challengeTypeList.Add(MetaChallengeType.EPIC_PASS);
                challengeTypeList.Add(MetaChallengeType.NORMAL);
            }

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "ON_CHANGE_CHALLENGE_TYPE_LIST");
        }

        protected override void Awake()
        {
            base.Awake();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_LONG_POLL_EVENT);

            // Check Complete Mission
            Register(MetaEventDefine.ON_META_UI_EVENT, ON_CHECK_COMPLETE_MISSION, OnCheckCompleteMission);

            // On Shoot Mission
            Register(MetaEventDefine.ON_META_UI_EVENT, ON_SHOOT_CHALLENGE_MISSION, OnShootChallengeMission);

            // On Complete Mission Effect
            Register(MetaEventDefine.ON_META_UI_EVENT, ON_COMPLETE_CHALLENGE_MISSION_EFFECT, OnCompleteChallengeMissionEffect);

            // Clear Events
            Register(MetaEventDefine.ON_META_UI_EVENT, ON_CLEAR_CHALLENGE_LIST, OnClearChallengeList);

            // Long Poll
            Register(MetaEventDefine.ON_LONG_POLL_EVENT, MetaEventDefine.ANY_EVENT_NAME, OnLongPollEvent);
        }

        private void ClearChallengeEventList(string eventListName)
        {
            var challengeBB = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), eventListName);

            for (int i = 0; i < challengeBB.Count; ++i)
            {
                BlackboardUtils.ClearBlackboard(challengeBB[i]);
                Destroy(challengeBB[i].gameObject);
            }

            challengeBB.Clear();
        }

        private void OnLongPollEvent(EventData eventData)
        {
            var pollBB = ((EventData<Blackboard>)eventData).value;
            var pollType = pollBB.GetVariable<PollType>("__event__")?.value ?? PollType.UNKNOWN;
            if (pollType == PollType.CHALLENGE_MISSION_COMPLETE ||
                pollType == PollType.CLUB_CHALLENGE_MISSION_COMPLETE)
            {
                var simpleChallengeInfo = BlackboardUtils.FindVariable<Blackboard>(pollBB, "challenge");
                if (simpleChallengeInfo != null)
                    BlackboardQueryUtils.UpdateChallengeSimpleInfo(simpleChallengeInfo.value);

                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, ON_RECEIVE_COMPLETE_MISSION);
            }
        }

        private void OnClearChallengeList(EventData eventData)
        {
            ClearChallengeEventList(challengeEventListName);
            ClearChallengeEventList(shootChallengeEventListName);
        }

        private void OnCheckCompleteMission(EventData eventData)
        {
            CheckCompleteMission();
        }

        private void OnShootChallengeMission(EventData eventData)
        {
            if (eventData.value != null)
            {
                Blackboard eventBB = eventData.value as Blackboard;

                if (eventBB != null)
                {
                    var pollID = BlackboardUtils.FindVariable<long>(eventBB, "id");
                    RemoveChallengeMission(pollID.value);
                    BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), shootChallengeEventListName, eventBB);
                }
            }
        }

        private void OnCompleteChallengeMissionEffect(EventData eventData)
        {
            if (eventData.value != null)
            {
                Blackboard eventBB = eventData.value as Blackboard;

                if (eventBB != null)
                {
                    var pollID = BlackboardUtils.FindVariable<long>(eventBB, "id");
                    var challengeBBList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), shootChallengeEventListName);

                    for (int i = 0; i < challengeBBList.Count; ++i)
                    {
                        var id = BlackboardUtils.FindVariable<long>(challengeBBList[i], "id");

                        if (id?.value == pollID?.value)
                        {
                            challengeBBList.RemoveAt(i);
                            break;
                        }
                    }

                    BlackboardUtils.ClearBlackboard(eventBB);
                    Destroy(eventBB.gameObject);
                }
            }
        }

        private void CheckCompleteMission()
        {
            // popup blackboard
            var completeInfoBB = GetCompleteMission();
            if (completeInfoBB != null)
            {
                var pollType = completeInfoBB.GetVariable<PollType>("__event__")?.value ?? PollType.UNKNOWN;
                switch (pollType)
                {
                    case PollType.CHALLENGE_MISSION_COMPLETE:
                        var eventData = new EventData<Blackboard>(ON_COMPLETE_CHALLENGE_MISSION, completeInfoBB);
                        EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
                        break;
                    case PollType.CLUB_CHALLENGE_MISSION_COMPLETE:
                        eventData = new EventData<Blackboard>(ON_CLUB_COMPLETE_CHALLENGE_MISSION, completeInfoBB);
                        EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
                        break;
                    case PollType.UNKNOWN:
                        if (ApplicationSettings.LogTest())
                            Debug.LogWarning("Polling complete mission failure. \"completeInfoBB\\__event__\" is unknown");
                        break;
                }
            }
        }

        private Blackboard GetCompleteMission()
        {
            var challengeBBList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), challengeEventListName);

            if (challengeBBList.Count > 0)
            {
                return challengeBBList[0];
            }
            return null;
        }

        private void RemoveChallengeMission(long pollID)
        {
            var challengeBBList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), challengeEventListName);

            for (int i = 0; i < challengeBBList.Count; ++i)
            {
                var id = BlackboardUtils.FindVariable<long>(challengeBBList[i], "id");

                if (id.value == pollID)
                {
                    challengeBBList.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
