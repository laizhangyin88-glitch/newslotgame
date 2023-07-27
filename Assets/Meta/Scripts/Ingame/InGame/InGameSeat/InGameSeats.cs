using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public class InGameSeats : MonoBehaviour
{
    public ContextList seatContextList;
    public List<InGameSeat> inGameSeatList;
    public ObjectPool likeTokenPool;

    private GameObject myProfile;
    private string[] userIds = new string[6];

    private Dictionary<string, MessageDispatcher.EventDelegate> metaUIDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();
    private Dictionary<string, MessageDispatcher.EventDelegate> contentDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

    private const string ON_LONG_POLL_EVENT = "OnLongPollEvent";
    private const string ON_META_UI_EVENT = "OnMetaUIEvent";
    private const string ON_SEAT_LIKE = "SeatLike";
    private const string ON_CLOSE_SHOP = "CloseShop";
    private const string ON_SPINBUTTON_EVENT = "OnSpinButtonEvent";
    private const string ON_CONTENT_EVENT = "OnContentEvent";
    private const string ON_END_TURN_EVENT = "EndTurn";
    private const string ON_CONTENT_UI_EVENT = "OnContentUIEvent";

    private const string ON_COMMUNITY_TICKET_EARN_EVENT = "OnCommunityTicketEarn";
    private const string ON_BOWL_TICKET_EARN_EVENT = "OnBowlTicketEarn";
    private const string ON_EMPTY_BOWL = "OnEmptyBowl";

    private const int SEAT_COUNT = 4;

    private bool isReady = false;

    public bool SeatsReady
    {
        get
        {
            if(isReady == false)
            {
                if(inGameSeatList == null || inGameSeatList.Count != SEAT_COUNT)
                {
                    return false;
                }

                isReady = true;
            }

            return true;
        }
    }

    private static readonly int ANIMATOR_TO_ME = Animator.StringToHash("To Me");

    public bool interactable
    {
        set
        {
            if(inGameSeatList != null)
            {
                for (int i = 0; i < inGameSeatList.Count; ++i)
                {
                    if (inGameSeatList[i] != null)
                    {
                        inGameSeatList[i].interactable = value;
                    }
                }
            }
        }
    }

    private void Awake()
    {
        metaUIDelegates[ON_SEAT_LIKE] = OnSeatLike;
        metaUIDelegates[ON_CLOSE_SHOP] = OnCloseShop;
        contentDelegates[ON_END_TURN_EVENT] = OnEndTurn;
    }

    private void OnDestroy()
    {
        MessageDispatcher.UnRegister(ON_LONG_POLL_EVENT, OnLongPollEvent);
        MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
        MessageDispatcher.UnRegister(ON_SPINBUTTON_EVENT, OnSpinButton);
        MessageDispatcher.UnRegister(ON_CONTENT_EVENT, OnContentEvent);
    }

    private void Start()
    {
        myProfile = BlackboardUtils.FindVariable<GameObject>(null, "/shortcut/profile").value;
        userIds[0] = BlackboardUtils.FindVariable<string>(null, "/me/userId").value;

        seatContextList.UpdateContext(true);
        for (int i = 0; i < SEAT_COUNT; ++i)
        {
            var inGameSeat = seatContextList.GetChildElement(i).GetComponent<InGameSeat>();
            inGameSeat.seatIndex = i;
            inGameSeatList.Add( inGameSeat );
        }

        MessageDispatcher.Register(ON_LONG_POLL_EVENT, OnLongPollEvent);
        MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
        MessageDispatcher.Register(ON_SPINBUTTON_EVENT, OnSpinButton);
        MessageDispatcher.Register(ON_CONTENT_EVENT, OnContentEvent);
    }

    private void OnLongPollEvent(EventData eventData)
    {
        if(!SeatsReady) return;

        var pollBB = ((EventData<Blackboard>)eventData).value;
        var pollType = pollBB.GetVariable<PollType>("__event__")?.value ?? PollType.UNKNOWN;
        SceneState currentSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>(MainBlackboard.Get(), "currentSceneState")?.value ?? SceneState.INGAME;
        if (pollType == PollType.LEVEL_UP ||
            pollType == PollType.TIER_UP ||
            pollType == PollType.WIN)
        {
            string userId = pollBB.GetValue<string>("userId");
            int seatIndex = BlackboardQueryUtils.GetSeatIndex(userId);
            if (seatIndex >= 0)
            {
                var seat = inGameSeatList[seatIndex];
                seat.PushSeatEvent(pollBB);
            }
        }
        else if (pollType == PollType.LIKE && currentSceneState == SceneState.INGAME)
        {
            UpdateUserIds();

            int from = FindUserIndex(pollBB.GetValue<string>("userId"));
            int to = FindUserIndex(pollBB.GetValue<string>("targetUserId"));

            if(from > -1 && to > -1)
            {
                var token = likeTokenPool.GetObject();
                var controller = token.GetComponent<DirectionalWeightPositionController>();

                controller.from = GetLikeTransform(from);
                controller.to = GetLikeTransform(to);
                token.transform.SetParent(controller.to, false);

                if (to == 0)// To Me
                    token.GetComponent<Animator>().SetBool(ANIMATOR_TO_ME, true);
            }
        }
        else if (pollType == PollType.COMMUNITY_GAME_TICKET_EARN && currentSceneState == SceneState.INGAME)
        {
            // int to = FindUserIndex(pollBB.GetValue<string>("userId"));
            // int to = FindUserIndex(pollBB.GetValue<string>("targetUserId"));
            string userId = pollBB.GetValue<string>("userId");
            int seatIndex = BlackboardQueryUtils.GetSeatIndex(userId);

            if (seatIndex >= 0)
            {
                var seat = inGameSeatList[seatIndex].transform;
                MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData<Transform>(ON_COMMUNITY_TICKET_EARN_EVENT, seat));
            }
        }
        else if (pollType == PollType.CUSTOM_USER_IN_GAME_ACTION && currentSceneState == SceneState.INGAME)
        {
            string userId = pollBB.GetValue<string>("userId");
            CustomUserInGameActionType customUserInGameActionType = pollBB.GetValue<CustomUserInGameActionType>("customUserInGameActionType");
            int seatIndex = BlackboardQueryUtils.GetSeatIndex(userId);

            if (seatIndex >= 0)
            {
                var seat = inGameSeatList[seatIndex].transform;
                if (customUserInGameActionType == CustomUserInGameActionType.ACCUMULATE_BOWL)
                {
                    MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData<Transform>(ON_BOWL_TICKET_EARN_EVENT, seat));
                }
                else if (customUserInGameActionType == CustomUserInGameActionType.EMPTY_OUT_BOWL)
                {
                    MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, new EventData<Transform>(ON_EMPTY_BOWL, seat));
                }
            }
        }
    }

    private void OnMetaUIEvent(EventData eventData)
    {
        MessageDispatcher.EventDelegate del;
        if (metaUIDelegates.TryGetValue(eventData.name, out del))
            del.Invoke(eventData);
    }

    private void OnSeatLike(EventData eventData)
    {
        if(!SeatsReady) return;

        UpdateUserIds();

        var userID = ((EventData<string>)eventData).value;

        int to = FindUserIndex(userID);
        if(to > -1)
        {
            var token = likeTokenPool.GetObject();
            var controller = token.GetComponent<DirectionalWeightPositionController>();

            controller.from = GetLikeTransform(0);
            controller.to = GetLikeTransform(to);

            token.transform.SetParent(controller.to, false);
        }
    }

    private void OnCloseShop(EventData eventData)
    {
        interactable = true;
    }

    private void OnSpinButton(EventData eventData)
    {
        interactable = false;
    }

    private void OnContentEvent(EventData eventData)
    {
        MessageDispatcher.EventDelegate del;
        if (contentDelegates.TryGetValue(eventData.name, out del))
            del.Invoke(eventData);
    }

    private void OnEndTurn(EventData eventData)
    {
        bool autoSpin = ContentBlackboard.Get().GetValue<bool>("autoSpin");
        if (!autoSpin)
            interactable = true;
    }

    private void UpdateUserIds()
    {
        for (int i = 0; i < SEAT_COUNT; ++i)
        {
            userIds[i + 1] = inGameSeatList[i].userId;
        }
    }

    private int FindUserIndex(string userId)
    {
    for (int i = 0; i < SEAT_COUNT+1; ++i)
        {
            if (string.Equals(userIds[i], userId))
                return i;
        }

        return -1;
    }

    private Transform GetLikeTransform(int userIndex)
    {
        if (userIndex < 0) return null;
        if (userIndex == 0)
            return myProfile.GetComponent<NavigationProfileController>().likeAnchor;

        return inGameSeatList[userIndex - 1].likeAnchor;
    }
}

}
