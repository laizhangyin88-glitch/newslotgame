using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using UnityEngine.EventSystems;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    public class DynamicScrollClubNewsFeedCreator : DynamicScrollItemCreator, IBeginDragHandler, IEndDragHandler
    {
        public Blackboard clubInfoBB;
        public ClubAuthority myAuthority;

        public List<Blackboard> feedList = null;

        public ObjectPool winBonusObjectPool;
        public ObjectPool messageObjectPool;
        public ObjectPool logObjectPool;
        public ObjectPool loadingObjectPool;
        public ObjectPool endObjectPool;

        public ObjectPool offerReceiveObjectPool;
        public ObjectPool challengeRewardObjectPool;
        public ObjectPool missionRewardObjectPool;

        public ObjectPool clubLeagueRewardObjectPool;
        public ObjectPool clubLeagueTierObjectPool;

        public ObjectPool clubNoticeObjectPool;
        public ObjectPool requestShareCellObjectPool;

        public ObjectPool bossRaidersResultObjectPool;

        public ObjectPool leaderPushObjectPool;

        public ObjectPool clubArenaResultObjectPool;
        public ObjectPool clubArenaHelpObjectPool;

        public ObjectPool hiddenObjectsFinderRequestObjectPool;

        public GameObject caller;
        public Blackboard agent;

        private bool dragging = false;
        private Vector2 startDragPoint = Vector2.zero;

        private long reqBlockSeq = 0;
        private long lastFeedID = 0;
        private bool isNextExistFeed = false;

        private int FIRST_FEED_COUNT = 50;
        private int REQUEST_FEED_COUNT = 10;

        private bool isLast = false;

        private string feedTabContextID;
        private string mgBundleName;
        private int mgEventID = 0;
        private EventInfoType mgEventType;
        private ClubFeedFilterType feedFilterType;

        public void SetClubInfo(Blackboard clubInfo, ClubAuthority authority, string currentBundleName, EventInfoType currentEventInfoType, int currentEventID)
        {
            clubInfoBB = clubInfo;
            myAuthority = authority;
            mgBundleName = currentBundleName;
            mgEventType = currentEventInfoType;
            mgEventID = currentEventID;
        }

        public void RefreshList(string contextID, ClubFeedFilterType currentfeedFilterType)
        {
            feedTabContextID = contextID;
            feedFilterType = currentfeedFilterType;

            reqBlockSeq = 0;
            lastFeedID = 0;
            isLast = false;
            isNextExistFeed = false;

            feedList = new List<Blackboard>();
            RemoveAll();

            // Notice. 
            feedList.Add(clubInfoBB);

            BlackboardUtils.DestroyBlackboardList(agent, "AdditionalFeedList");

            var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "clubNewsFeedResponse/feedList");

            isNextExistFeed = BlackboardUtils.FindVariable<bool>(agent, "clubNewsFeedResponse/nextExists").value;

            if (variable != null && variable.value.Count > 0)
            {
                feedList.AddRange(variable.value);

                if (isNextExistFeed)
                    lastFeedID = feedList[feedList.Count - 1].GetValue<long>("id");
                else
                    isLast = true;

                for (int i = 0; i < maxBufferingCount; ++i)
                {
                    if (!PushBack())
                        break;
                }
            }
            else
            {
                isLast = true;
            }

            RebuildContentBounds();
            dynamicScrollRect.verticalNormalizedPosition = 1f;
        }

        public void AppendList(List<Blackboard> additionalFeedList, bool nextExistFeed)
        {
            if (additionalFeedList.Count > 0)
            {
                bool isPushBack = false;

                if (backIndex > feedList.Count)
                {
                    PopBack();
                    isPushBack = true;
                }

                feedList.AddRange(additionalFeedList);

                if (nextExistFeed)
                    lastFeedID = feedList[feedList.Count - 1].GetValue<long>("id");
                else
                    isLast = true;


                if (isPushBack)
                {
                    PushBack();
                }
            }
            else
            {
                if (backIndex > feedList.Count)
                {
                    PopBack();
                    isLast = true;
                    PushBack();
                }
            }
        }

        public override void OnInitialize()
        {
            reqBlockSeq = 0;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            // Additional Load Cancel.. 
            lastFeedID = 0;
            reqBlockSeq = 0;
            isLast = false;
        }

        private void InitCell(int index, Blackboard bb)
        {
            bb.AddVariable("feedInfo", feedList[index]);
            bb.AddVariable("myAuthority", myAuthority);
            bb.AddVariable("caller", caller);
            bb.AddVariable("cellIndex", index);
            bb.AddVariable("_biContextID", feedTabContextID);

            BlackboardUtils.SetOrCreateValue<int>(bb, "mgEventID", mgEventID);
            BlackboardUtils.SetOrCreateValue<string>(bb, "mgBundleName", mgBundleName);
            BlackboardUtils.SetOrCreateValue<EventInfoType>(bb, "mgEventType", mgEventType);
        }

        private GameObject PushFront(ObjectPool pool)
        {
            if (pool == null) return null;

            var go = pool.GetObject(false).gameObject;
            go.transform.SetParent(content, false);
            go.transform.SetAsFirstSibling();
            return go;
        }

        protected override bool PushFront()
        {
            if (frontIndex == -1)
            {
                // Debug.LogError(string.Format("PushFront : {0}", dynamicScrollRect.normalizedPosition.y));
                return false;
            }
            if (feedList.Count <= frontIndex)
            {
                //Debug.LogError("PushFront : out of range exception");
                return false;
            }

            var go = PushFront(GetObjectPool(frontIndex));

            if (go != null)
            {
                InitCell(frontIndex, go.GetComponent<Blackboard>());
                go.SetActive(true);
            }

            frontIndex -= 1;
            return true;
        }

        private GameObject PushBack(ObjectPool pool)
        {
            if (pool == null) return null;

            var go = pool.GetObject(false).gameObject;
            go.transform.SetParent(content, false);
            go.transform.SetAsLastSibling();
            return go;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
            startDragPoint = dynamicScrollRect.normalizedPosition;
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                if (startDragPoint.y <= 0f && startDragPoint.y > dynamicScrollRect.normalizedPosition.y && dynamicScrollRect.normalizedPosition.y < 0.1f)
                {
                    RequestAdditionalFeedList();
                }
            }

            dragging = false;
        }

        protected override bool PushBack()
        {
            if (backIndex >= feedList.Count)
            {
                if (backIndex >= feedList.Count + 1)
                {
                    return false;
                }
                else
                {
                    if (isLast)
                    {
                        var endObject = endObjectPool.GetObject(false).gameObject;
                        endObject.transform.SetParent(content, false);
                        endObject.transform.SetAsLastSibling();
                        endObject.SetActive(true);
                        ++backIndex;
                        return true;
                    }
                    else
                    {
                        var endObject = loadingObjectPool.GetObject(false).gameObject;
                        endObject.transform.SetParent(content, false);
                        endObject.transform.SetAsLastSibling();
                        endObject.SetActive(true);
                        ++backIndex;

                        RequestAdditionalFeedList();
                        return true;
                    }
                }
            }

            var go = PushBack(GetObjectPool(backIndex));

            if (go != null)
            {
                InitCell(backIndex, go.GetComponent<Blackboard>());
                go.SetActive(true);
            }

            backIndex += 1;
            return true;
        }

        protected override bool PopFront()
        {
            frontIndex += 1;
            content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();

            return true;
        }

        protected override bool PopBack()
        {
            backIndex -= 1;
            content.GetChild(content.childCount - 1).GetComponent<PooledObject>().ReturnToPool();

            return true;
        }

        protected void RemoveAll()
        {
            while (content.childCount > 0)
            {
                content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
            }

            frontIndex = -1;
            backIndex = 0;
        }

        private ObjectPool GetObjectPool(int index)
        {
            if (index == 0) return clubNoticeObjectPool;

            ClubFeedType feedType = feedList[index].GetValue<ClubFeedType>("type");

            switch (feedType)
            {
                case ClubFeedType.COLEADER_PROMOTE:
                case ClubFeedType.COLEADER_DEMOTE:
                case ClubFeedType.JOIN:
                case ClubFeedType.LEVEL_UP:
                case ClubFeedType.LEADER_CHANGE:
                    return logObjectPool;
                case ClubFeedType.MESSAGE:
                    return messageObjectPool;
                case ClubFeedType.WIN_BONUS:
                    return winBonusObjectPool;
                case ClubFeedType.CLUB_OFFER_BONUS:
                    return offerReceiveObjectPool;
                case ClubFeedType.CLUB_MISSION_COMPLETE:
                    return missionRewardObjectPool;
                case ClubFeedType.CLUB_CHALLENGE_COMPLETE:
                    return challengeRewardObjectPool;
                case ClubFeedType.CLUB_LEAGUE_REWARD:
                    return clubLeagueRewardObjectPool;
                case ClubFeedType.CLUB_TIER_INFO:
                    return clubLeagueTierObjectPool;
                case ClubFeedType.COLLECTING_GAME_REQUEST:
                    return requestShareCellObjectPool;
                case ClubFeedType.BOSS_RAIDERS_ROUND_COMPLETE:
                case ClubFeedType.BOSS_RAIDERS_END_REWARD:
                    return bossRaidersResultObjectPool;
                case ClubFeedType.LEADER_PUSH:
                    return leaderPushObjectPool;
                case ClubFeedType.CLUB_ARENA_HELP:
                    return clubArenaHelpObjectPool;
                case ClubFeedType.CLUB_ARENA_END_REWARD:
                    return clubArenaResultObjectPool;
                case ClubFeedType.HIDDEN_UNIVERSE_FINDER_REQUEST:
                    return hiddenObjectsFinderRequestObjectPool;
                default:
                    Debug.LogWarning(string.Format("DynamicScrollClubNewsFeedCreator.GetObjectPool failure. {0} is undefined feedType.", feedType));
                    return null;
            }
        }

        private void RequestAdditionalFeedList()
        {
            if (isNextExistFeed && reqBlockSeq == 0 && lastFeedID > 0)
            {
                MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRequestFeedList"));

                BagelCode.BagelCodeClientAPI.RequestClubNewsFeedList(feedFilterType, lastFeedID, REQUEST_FEED_COUNT, mgEventID, out reqBlockSeq, false,
                (response) =>
                {
                    if (reqBlockSeq != 0 && agent != null && reqBlockSeq == response.common.blockseq)
                    {
                        var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "clubNewsFeedAppendResponse");

                        BlackboardUtils.ClearBlackboard(bb);
                        BagelCode.ClientAPI2Blackboard.Serialize(bb, response);

                        isNextExistFeed = response.nextExists;

                        var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "clubNewsFeedAppendResponse/feedList");
                        if (variable != null)
                        {
                            BlackboardUtils.GetOrCreateBlackboardList(agent, "AdditionalFeedList");
                            for (int i = 0; i < variable.value.Count; ++i)
                            {
                                BlackboardUtils.AddToBlackboardList(agent, "AdditionalFeedList", variable.value[i]);
                            }

                            lastFeedID = 0;
                            reqBlockSeq = 0;
                            AppendList(variable.value, isNextExistFeed);
                        }
                    }
                },
                (error) =>
                {
                    reqBlockSeq = 0;
                    MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnFailedFeedList"));
                });
            }
        }
    }
}
