using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.FeatureAdmin;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Slots.TRR.Utillity
{
    public class TRRContentsStorePollManager : MonoWeakSingleton<TRRContentsStorePollManager>
    {
        public float syncDelay = 2.0f;
        public const string CONTENT_EVENT_ON_FINISH_CONTENTS_STORE_LOAD = "TRROnFinishContentsStoreLoad";

        private Dictionary<string, MessageDispatcher.EventDelegate> contentDelegateDictionary = new Dictionary<string, MessageDispatcher.EventDelegate>();
        private Dictionary<string, MessageDispatcher.EventDelegate> contentUIDelegateDictionary = new Dictionary<string, MessageDispatcher.EventDelegate>();

        private MessageDelegates contentUIMessageDelegates;
        private MessageDelegates contentMessageDelegates;

        public TRRSpotLeftWindow spotLeftWindow;
        public TRRTreasurePrizeAdmin treasurePrizeAdmin;

        public bool isLoading = false;

        public int lastCount;
        public int currentCount;
        public bool ignoreUpdate = false;
        public bool communityGameTriggered = false;
        public bool includePlayerIdInLatest = false;
        public bool enablePoll = false;

        public bool CheckIncludeAnyUserId(string userId, List<Blackboard> boardData)
        {
            for (int i = 0; i < 3; i++)
            {
                List<Blackboard> rowSpots = TRRUtillity.TryGetLocalBlackBoardVariable<List<Blackboard>>(boardData[i], "value");
                for (int j = 0; j < 8; j++)
                {
                    Blackboard colmnSpots = rowSpots[j];
                    bool collected = TRRUtillity.TryGetLocalBlackBoardVariable<bool>(colmnSpots, "collected");
                    if (collected)
                    {
                        string collectedUserID = TRRUtillity.TryGetLocalBlackBoardVariable<string>(colmnSpots, "userId");
                        if (String.Compare(userId, collectedUserID) == 0)
                            return true;
                    }
                }
            }
            return false;
        }

        public void UpdateContentStore(System.Action callback = null)
        {
            isLoading = true;
            MetaSystem.ApplyContentsStore(() =>
            {
                if (TRRUtillity.TryGetGlobalBlackBoardVariable<bool>("/inGame") == false || BlackboardUtils.FindVariable<bool>("./customData/duringTurn").value == true || enablePoll == false)
                {
                    if (enablePoll == false)
                        callback?.Invoke();

                    isLoading = false;
                    return;
                }
                if (communityGameTriggered == false)
                {
                    lastCount = currentCount;
                    currentCount = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./game/contentsStoreInfo/community_game_data/totalCollectCount");
                    string initalizedContentsStorePath = "./game/contentsStoreInfo/community_game_data/collectData";
                    string playerUserID = TRRUtillity.TryGetGlobalBlackBoardVariable<string>("/me/userId");

                    if (currentCount < lastCount)
                    {
                        //  Allow update if lastst updated data not contains user collect himself
                        if (includePlayerIdInLatest == false)
                        {
                            if (ignoreUpdate == false)
                            {
                                treasurePrizeAdmin.AddLoadTaskUserProfileFromBoardData(
                                    TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));

                                spotLeftWindow.UpdateSpotLeft(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));
                                includePlayerIdInLatest = CheckIncludeAnyUserId(playerUserID, TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));
                            }
                        }
                        else
                        {
                            if (ignoreUpdate == false)
                            {
                                communityGameTriggered = true;
                                ignoreUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        if (ignoreUpdate == false)
                        {
                            treasurePrizeAdmin.AddLoadTaskUserProfileFromBoardData(
                                TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));

                            spotLeftWindow.UpdateSpotLeft(TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));

                            includePlayerIdInLatest = CheckIncludeAnyUserId(playerUserID, TRRUtillity.TryGetGlobalBlackBoardVariable<List<Blackboard>>(initalizedContentsStorePath));
                        }
                    }
                }
                callback?.Invoke();
                isLoading = false;
            }, () => { isLoading = false; });
        }

        private void Awake()
        {
            contentDelegateDictionary.Add("TRREndSpin", OnSpinEnd);
            contentDelegateDictionary.Add("TRRWaitUntilFinishLoad", OnWaitUntilFinishLoad);
            contentUIDelegateDictionary.Add("OnBowlTicketEarn", RequestUpdateStoreData);
            contentUIDelegateDictionary.Add("OnEmptyBowl", RequestUpdateStoreData);
        }
        private void Start()
        {
            lastCount = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./game/contentsStoreInfo/community_game_data/totalCollectCount");
            currentCount = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./game/contentsStoreInfo/community_game_data/totalCollectCount");
            enablePoll = true;
        }

        private void OnEnable()
        {
            contentMessageDelegates = new MessageDelegates(contentDelegateDictionary);
            contentUIMessageDelegates = new MessageDelegates(contentUIDelegateDictionary);
            MessageDispatcher.Register(SendEvent.ON_CONTENT_EVENT, contentMessageDelegates.Delegate);
            MessageDispatcher.Register(SendEvent.ON_CONTENT_UI_EVENT, contentUIMessageDelegates.Delegate);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(SendEvent.ON_CONTENT_EVENT, contentMessageDelegates.Delegate);
            MessageDispatcher.UnRegister(SendEvent.ON_CONTENT_UI_EVENT, contentUIMessageDelegates.Delegate);
        }

        private void OnWaitUntilFinishLoad(EventData eventData)
        {
            communityGameTriggered = false;
            StartCoroutine(WaitUntilFinishLoadCoroutine());
        }

        private void OnSpinEnd(EventData eventData)
        {
            communityGameTriggered = false;
            enablePoll = true;
            ignoreUpdate = false;
            string playerUserID = TRRUtillity.TryGetGlobalBlackBoardVariable<string>("/me/userId");
            includePlayerIdInLatest = CheckIncludeAnyUserId(playerUserID, spotLeftWindow.spotLeftData);
            currentCount = 24 - spotLeftWindow.spotLeftCount;
            lastCount = currentCount;
        }

        private IEnumerator WaitUntilFinishLoadCoroutine()
        {
            yield return new WaitUntil(() => isLoading == false);
            TRRUtillity.SendEvent("OnContentEvent", CONTENT_EVENT_ON_FINISH_CONTENTS_STORE_LOAD);
        }

        public void Report()
        {
            Debug.LogError($"CommunityGame Triggered : {communityGameTriggered}\nCurrent Count : {currentCount}\nLast Count : {lastCount}\nSpots Count : {spotLeftWindow.spotLeftCount}\nSpots Count2 : {spotLeftWindow.leftCount}\n");
        }

        public void RequestUpdateStoreData(EventData data)
        {
            if (isLoading == false && enablePoll == true)
                UpdateContentStore();
        }
    }
}