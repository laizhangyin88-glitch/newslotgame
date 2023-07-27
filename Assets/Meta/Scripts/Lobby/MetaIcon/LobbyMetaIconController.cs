using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class LobbyMetaIconController : EventMonoBehaviour
    {
        public DynamicScrollRect dynamicScrollRect = null;
        public GameObject scrollUpObject = null;
        public GameObject scrollDownObject = null;

        private ContextElement eventButtonAreaElement;

        [SerializeField] private List<LobbyMetaIcon> lobbyIconList;

        private int iconCount = -1;
        private bool isInit = false;

        private const int MAX_SCROLL_COUNT = 3;
        private const int MAX_CALCULATOR_COUNT = 7;

        private const string ON_PASSIVE_EVENT = "OnPassiveEvent";
        private const string ON_REFRESH_PASSIVE = "RefreshPassive";
        private const string ON_START_PASSIVE = "StartPassive";
        private const string ON_REFRESH_META_GAME = "OnRefreshMetaGame";
        private const string ON_REFRESH_ICON_CHECK = "OnRefreshIconCheck";

        public static bool IsCalledApiVipLoungeInfo { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnInit();
            InitRegister();
            OnStart();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            UnRegisterAll();
        }

        private void OnInit()
        {
            if (isInit) return;

            InitProperty();
            InitLobbyIconList();

            isInit = true;
        }

        private void InitProperty()
        {
            ContextElement rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            eventButtonAreaElement = ContextUtils.FindElement(rootElement, "Event Button Area", ContextSearchingType.ChildrenSearch);
        }

        private void InitLobbyIconList()
        {
            if (lobbyIconList == null)
                lobbyIconList = new List<LobbyMetaIcon>();
            if (lobbyIconList.Count > 0)
                lobbyIconList.Clear();

            // Make & Add icon list
            lobbyIconList.Add(new LobbyUpdateRecentIcon());
            lobbyIconList.Add(new LobbyVipDealIcon());
            lobbyIconList.Add(new LobbyVipDealV2PayIcon());
            lobbyIconList.Add(new LobbyVipDealV2FreeIcon());
            lobbyIconList.Add(new LobbyMetaGameIcon());
            lobbyIconList.Add(new LobbyOtherMetaGameIcon());
            lobbyIconList.Add(new LobbyEpicPassAlwaysIcon());
            lobbyIconList.Add(new LobbyHiddenObjectsIcon());
            lobbyIconList.Add(new LobbyVipLoungeIcon());
            lobbyIconList.Add(new LobbyVegasDreamsIcon());
            lobbyIconList.Add(new LobbyLevelUpDashIcon());

            // List OnInit
            for (int i = 0; i < lobbyIconList.Count; ++i)
                lobbyIconList[i].OnInit(eventButtonAreaElement);
        }

        private void InitRegister()
        {
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(ON_PASSIVE_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, ON_REFRESH_META_GAME, OnDispatchMetaUIEvent);
            Register(MetaEventDefine.ON_META_UI_EVENT, ON_REFRESH_ICON_CHECK, CheckListActive);
            Register(ON_PASSIVE_EVENT, ON_REFRESH_PASSIVE, OnDispatchPassiveEvent);
            Register(ON_PASSIVE_EVENT, ON_START_PASSIVE, OnDispatchStartPassiveEvent);
        }

        private void OnStart()
        {
            if (lobbyIconList != null && lobbyIconList.Count > 0)
            {
                for (int i = 0; i < lobbyIconList.Count; ++i)
                    lobbyIconList[i].OnStart();
            }
            CheckListActive();
        }

        private void OnEventRecv(string eventName, EventData eventData)
        {
            OnBeginEventRecv(eventName, eventData);

            if (lobbyIconList != null && lobbyIconList.Count > 0)
            {
                for (int i = 0; i < lobbyIconList.Count; ++i)
                    lobbyIconList[i].OnEventRecv(eventName, eventData);
            }

            OnEndEventRecv(eventName, eventData);
            CheckListActive();
        }

        private void OnBeginEventRecv(string eventName, EventData eventData)
        {
            CheckBeginVIPLoungeEvent(eventName, eventData);
        }

        private void OnEndEventRecv(string eventName, EventData eventData)
        {

        }

        private void CheckListActive()
        {
            int activeCount = 0;

            if (lobbyIconList != null && lobbyIconList.Count > 0)
            {
                for (int i = 0; i < lobbyIconList.Count; ++i)
                    activeCount += lobbyIconList[i].IsActive() ? 1 : 0;
            }
            if (ApplicationSettings.LogTest())
                Debug.Log("CheckListActive count : " + activeCount);

            if (iconCount != activeCount)
            {
                SetScrollActive(activeCount > MAX_SCROLL_COUNT);
                StartCoroutine(ScrollInitPosition());
            }
            iconCount = activeCount;
        }

        private void SetScrollActive(bool isActive)
        {
            if (dynamicScrollRect != null) dynamicScrollRect.draggable = isActive;
            if (scrollUpObject != null) scrollUpObject.SetActive(isActive);
            if (scrollDownObject != null) scrollDownObject.SetActive(isActive);
        }

        public IEnumerator ScrollInitPosition()
        {
            bool isOrigEnabled = dynamicScrollRect.enabled;
            dynamicScrollRect.enabled = true;
            dynamicScrollRect.verticalNormalizedPosition = 1.0f;
            yield return new WaitForEndOfFrame();
            dynamicScrollRect.enabled = isOrigEnabled;
        }

        private void OnDispatchMetaUIEvent(EventData eventData)
        {
            OnEventRecv(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        private void OnDispatchPassiveEvent(EventData eventData)
        {
            OnEventRecv(ON_PASSIVE_EVENT, eventData);
        }

        private void OnDispatchStartPassiveEvent(EventData eventData)
        {
            OnEventRecv(ON_PASSIVE_EVENT, eventData);
        }

        private float CalculatorPosition()
        {
            int calcCount = iconCount * 2 - MAX_CALCULATOR_COUNT;
            float verticalPosition = (float)Math.Round(1.0f / calcCount, 1);
            verticalPosition *= 2.0f;
            return verticalPosition;
        }

        public void ClickScrollUp()
        {
            dynamicScrollRect.verticalNormalizedPosition = Mathf.Lerp(0.0f, 1.0f, dynamicScrollRect.verticalNormalizedPosition + CalculatorPosition());
        }

        public void ClickScrollDown()
        {
            dynamicScrollRect.verticalNormalizedPosition = Mathf.Lerp(0.0f, 1.0f, dynamicScrollRect.verticalNormalizedPosition - CalculatorPosition());
        }
        // VIP Lounge + Build dream (API => RequestVegasDreamInfo)
        private void CheckBeginVIPLoungeEvent(string eventName, EventData eventData)
        {
            bool isUpdateVipLounge = false;
            if (eventName.Equals(ON_PASSIVE_EVENT))
            {
                if (eventData.name.Equals(ON_REFRESH_PASSIVE))
                    isUpdateVipLounge = IsVipLoungeEvent(eventData);
                else if (eventData.name.Equals(ON_START_PASSIVE))
                {
                    EventInfoType eventInfoType = ((EventData<EventInfoType>)eventData).value;
                    if (eventInfoType == EventInfoType.VIP_LOUNGE || eventInfoType == EventInfoType.BUILD_DREAM_SEASON)
                        isUpdateVipLounge = true;
                }
            }
            else if (eventName.Equals(MetaEventDefine.ON_META_UI_EVENT) && eventData.name.Equals(ON_REFRESH_META_GAME))
                isUpdateVipLounge = IsVipLoungeEvent(eventData);

            if (isUpdateVipLounge)
                StartCoroutine(RequestVegasDreamInfo());
        }

        private bool IsVipLoungeEvent(EventData eventData)
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetEventInfoFromID((int)eventData.value, true);
            return eventInfo != null && (eventInfo.type == EventInfoType.VIP_LOUNGE || eventInfo.type == EventInfoType.BUILD_DREAM_SEASON);
        }

        private IEnumerator RequestVegasDreamInfo()
        {
            if (IsCalledApiVipLoungeInfo)
                yield break;
            IsCalledApiVipLoungeInfo = true;

            bool isSuccess = false, isFail = false;
            BagelCodeClientAPI.RequestVegasDreamInfo(
                (response) =>
                {
                    BlackboardQueryUtils.UpdateRequestVegasDreamInfo(response);
                    isSuccess = true;
                },
                (error) =>
                {
                    isFail = true;
                });

            yield return new WaitUntil(() => isSuccess || isFail);
            IsCalledApiVipLoungeInfo = false;
        }
    }
}
