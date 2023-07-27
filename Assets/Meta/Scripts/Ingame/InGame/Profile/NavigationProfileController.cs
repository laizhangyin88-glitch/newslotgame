using UnityEngine;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.ClientModels;

namespace BagelCode
{
    // note
    // 인게임에서는 팝업 열려도 profile inactive 안됨
    public class NavigationProfileController : EventMonoBehaviour
    {
        private List<ProfileEventGroup> profileEventGroupList = new List<ProfileEventGroup>();

        private int displayIndex = -1;

        private ContextElement root;
        private Blackboard bb;

        private PIDButton profileButton;
        private PIDButton mysteryGiftButton;
        private ContextElement mysteryGiftElement;

        public Transform likeAnchor;

        private Variable<bool> hideTagVariable;
        private Variable<bool> disableClickVariable;

        private Coroutine eventTagCoroutine = null;

        private bool isInit = false;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            profileButton = GetComponent<PIDButton>();

            root.UpdateContext(false);

            hideTagVariable = BlackboardUtils.GetOrCreateVariable<bool>(bb, "hideTags");
            disableClickVariable = BlackboardUtils.GetOrCreateVariable<bool>(bb, "disableClick");

            // Tag Groups
            profileEventGroupList.Add(new ProfileEventGroupEventMultiplier(root));
            profileEventGroupList.Add(new ProfileEventGroupVIP(root));
            profileEventGroupList.Add(new ProfileEventGroupLevelUpDash(root));

            mysteryGiftElement = ContextUtils.FindElement(root, "Mystery Gift", ContextSearchingType.ChildrenSearch);
            mysteryGiftButton = mysteryGiftElement.GetComponent<PIDButton>();

            eventTagCoroutine = StartCoroutine(DisplayEventTagCoroutine());

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_PASSIVE_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, LevelUpDash.LevelUpDash.Events.NOTIFY_LEVEL_UP_DASH_ENDED, CheckLevelUpDashEnded);
            Register(MetaEventDefine.ON_PASSIVE_EVENT, MetaEventDefine.START_PASSIVE_EVENT, OnStartPassive);
            Register(MetaEventDefine.ON_PASSIVE_EVENT, MetaEventDefine.REFRESH_PASSIVE_EVENT, OnRefreshPassive);

            isInit = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateEventTags();

            CheckLevelUpDashEnded();

            if (isInit && eventTagCoroutine == null)
                eventTagCoroutine = StartCoroutine(DisplayEventTagCoroutine());
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (eventTagCoroutine != null)
            {
                DisappearAllTags();
                StopCoroutine(eventTagCoroutine);
                eventTagCoroutine = null;
            }
        }

        private void Update()
        {
            if(profileButton != null)
            {
                profileButton.interactable = !disableClickVariable?.value ?? true;
            }
            if(mysteryGiftButton != null)
            {
                mysteryGiftButton.interactable = !disableClickVariable?.value ?? true;
            }
        }

        private void CheckLevelUpDashEnded()
        {
            var levelDashSystemObj = LevelUpDash.MetaSystemLevelUpDashController.Instance.gameObject;
            var levelDashSystemBB = levelDashSystemObj.GetComponent<Blackboard>();
            var isReservedLevelDashClosedNoti = BlackboardUtils.GetOrCreateVariable<bool>(levelDashSystemBB, "isReservedLevelDashClosedNoti");

            if (isReservedLevelDashClosedNoti != null && isReservedLevelDashClosedNoti.value)
            {
                EventSender.SendEvent(levelDashSystemObj, LevelUpDash.LevelUpDash.Events.ON_DISPLAY_LEVEL_DASH_CLOSED);
                StartCoroutine(EndLevelUpDashCoroutine());
            }
        }

        private IEnumerator EndLevelUpDashCoroutine()
        {
            // Wait popup closed & ShowUI
            // reward가 남은 경우 level dash 팝업 닫힌 이후에.
            var levelDashSystemObj = LevelUpDash.MetaSystemLevelUpDashController.Instance.gameObject;
            var levelDashSystemBB = levelDashSystemObj.GetComponent<Blackboard>();
            var isLevelUpDashOpenedVar = BlackboardUtils.FindVariable<bool>(levelDashSystemBB, "isLevelUpDashOpened");
            var hideUIVariable = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "hideUI");

            yield return new WaitUntil(() =>
                PopupManager.Instance.popupCount == 0 && !hideUIVariable.value &&
                LevelUpDash.LevelUpDash.Utils.GetCurrentMissionRewardResultList() == null &&
                isLevelUpDashOpenedVar.value == false);

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Speech Balloon Level Up Dash Scene";
            Transform parent = ContextUtils.FindElement(root, "Event Tag Area", ContextSearchingType.ChildrenSearch).transform;

            MetaObjectUtils.MakeScene(bundle, asset, parent);
        }

        private void DisappearAllTags()
        {
            profileEventGroupList.ForEach(g => g.Disappear());
        }

        private void DisappearAnotherTags(ProfileEventGroup group)
        {
            profileEventGroupList.ForEach((g) =>
            {
                if (g != group) g.Disappear();
            });
        }

        private void UpdateEventTags()
        {
            profileEventGroupList.ForEach(p => p.UpdateTagState());
        }

        private IEnumerator DisplayEventTagCoroutine()
        {
            while (true)
            {
                yield return new WaitWhile(() => hideTagVariable?.value ?? false);

                // Select Next Item
                ++displayIndex;
                var currentDisplayItem = profileEventGroupList.CircularIndexing(ref displayIndex);

                // Display
                if (currentDisplayItem != null)
                {
                    currentDisplayItem.Appear();
                    if (currentDisplayItem.isDisplay) DisappearAnotherTags(currentDisplayItem);

                    yield return new WaitWhile(() => currentDisplayItem.isDisplay);
                }
                else
                {
                    // exception.
                    DisappearAllTags();
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        private void OnStartPassive(EventData eventData)
        {
            if ((EventInfoType)eventData.value == EventInfoType.EXP_MULTIPLY ||
                (EventInfoType)eventData.value == EventInfoType.EXP_MULTIPLY_EXTENDABLE)
            {
                UpdateEventTags();
            }
        }

        private void OnRefreshPassive(EventData eventData)
        {
            var eventInfo = PassiveEventManager.Instance.GetEventInfoFromID((int)eventData.value, true);

            if (eventInfo != null)
            {
                if(eventInfo.type == EventInfoType.EXP_MULTIPLY ||
                    eventInfo.type == EventInfoType.EXP_MULTIPLY_EXTENDABLE)
                {
                    UpdateEventTags();
                }
            }
        }
    }
}
