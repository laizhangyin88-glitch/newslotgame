using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersLobbyButtonController : MonoBehaviour
    {
        private ContextElement rootElement;
        private BossRaidersButtonIconController iconController;

        private GameObject gs_managerObj;

        private ContextElement iconAreaElement;

        private ContextElement timerAreaElement;
        private ContextElement remainingTimerElement;

        private ContextAnimator badgeAnimator;
        private ContextElement badgeAreaElement;
        private ContextElement badgeTextElement;

        private ContextElement lockedIconElement;
        private ContextElement lockedSpeechBalloonElement;
        private Animator lockedSpeechAnimator;

        private bool isInit = false;
        private bool ignoreSpeechBalloon = false;

        public string bundleName = "";

        private Coroutine levelLockedSpeechBalloonEnumerator = null;

        public void OnEnable()
        {
            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        public void OnDisable()
        {
            MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if(!isInit) return;
            // Refresh lock icon.
            if(eventData.name == MetaEventDefine.ON_CLUB_REQUEST_ACCEPTED)
            {
                iconController?.UpdateValues();
                iconController?.SetLocked(false);
            }
        }

        private void InitProperty()
        {
            if (isInit) return;

            var bb = GetComponent<Blackboard>();
            ignoreSpeechBalloon = bb.GetVariable<bool>("ignoreSpeechBalloon")?.value ?? false;

            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(false); // todo : event ID extraction
            bundleName = BlackboardQueryUtils.GetMetaBundleName(eventInfo, false);
            string sharedBundle = BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, false);
            if (gs_managerObj == null)
                gs_managerObj = MetaObjectUtils.MakePrefab(sharedBundle, "Boss Raiders Common Shared Sounds", transform);

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            iconAreaElement = ContextUtils.FindElement(rootElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            ContextElement iconControllerElement = ContextUtils.FindElement(iconAreaElement, "Boss Raiders Icon", ContextSearchingType.ChildrenSearch);
            iconController = iconControllerElement.GetComponent<BossRaidersButtonIconController>();
            iconController?.InitProperty();

            timerAreaElement = ContextUtils.FindElement(rootElement, "Event Timer Area", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(timerAreaElement, "Event Timer/Remaining Timer", ContextSearchingType.FullNameSearch);

            badgeAreaElement = ContextUtils.FindElement(rootElement, "Badge Area", ContextSearchingType.ChildrenSearch);

            var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform);
            badgeAnimator = badgeObj.GetComponent<ContextAnimator>();
            badgeAnimator.UpdateContext(false);
            badgeAnimator.isPreserve = true;
            badgeAnimator.propertyName = "value";

            badgeTextElement = ContextUtils.FindElement(badgeAnimator, "Text", ContextSearchingType.ChildrenSearch);
            badgeTextElement.gameObject.SetActive(false);

            ContextElement lockedAreaElement = ContextUtils.FindElement(rootElement, "Locked Area", ContextSearchingType.ChildrenSearch);
            lockedIconElement = ContextUtils.FindElement(lockedAreaElement, "Boss Raiders Locked Icon", ContextSearchingType.ChildrenSearch);

            if (!ignoreSpeechBalloon)
            {
                lockedSpeechBalloonElement = ContextUtils.FindElement(lockedAreaElement, "Boss Raiders Locked Info Speech Balloon", ContextSearchingType.ChildrenSearch);
                lockedSpeechAnimator = lockedSpeechBalloonElement.GetComponent<Animator>();
            }

            MetaContextElementUtils.SetClickable(
                rootElement,
                "OnEnterBossRaiders",
                rootElement,
                null
            );

            BlackboardUtils.SetOrCreateValue(remainingTimerElement.GetComponent<NodeCanvas.Framework.Blackboard>(), "caller", gameObject);

            BossRaidersUtils.InitBossRaiders();

            isInit = true;
        }

        private void InitText()
        {
            int level = MetaGameUtils.GetMetaUnlockedLevel();
            MetaContextElementUtils.SimpleSetText(lockedIconElement, "Text", level.ToString());

            if (!ignoreSpeechBalloon)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(lockedSpeechBalloonElement, "Text", "META_GAME_LEVEL_UNLOCKED_SPEECH_TEXT", ContextSearchingType.ChildrenSearch, level);
            }
        }

        public void OnInit()
        {
            InitProperty();
            InitText();

            iconController?.UpdateValues();
            iconController?.SetRewardCallback(() => { UpdateBadge(); });
            iconController?.SetLocked(false);
            UpdateTimer();
            UpdateBadge();
            UpdateUnlockedLevel();

            BossRaidersUtils.CheckMetaStart();
        }

        private void UpdateTimer()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.BOSS_RAIDERS);
            if(eventInfo != null)
                MetaGameUtils.UpdateMetaGameRemainingTimer(timerAreaElement, remainingTimerElement, eventInfo.endTimestamp);
        }

        private void UpdateBadge()
        {
            if (badgeAnimator == null) return;
            // Unclaimed count
            int totalCount = BossRaidersUtils.SpinPossibleCount > 0 ? 1 : 0;
            badgeAnimator.SetIntProperty(totalCount);

            if (totalCount > 99)
                MetaContextElementUtils.SetText(badgeTextElement, "99+");
            else
                MetaContextElementUtils.SetText(badgeTextElement, totalCount.ToString());
        }

        public void UpdateUnlockedLevel()
        {
            bool isLockedLevel = MetaGameUtils.IsMetaGameLevelLocked();

            iconAreaElement.gameObject.SetActive(!isLockedLevel);
            badgeAreaElement.gameObject.SetActive(!isLockedLevel);

            lockedIconElement.gameObject.SetActive(isLockedLevel);
            lockedSpeechBalloonElement?.gameObject.SetActive(isLockedLevel);
        }

        public bool GetClubber()
        {
            return ClubUtils.IsClubber();
        }

        public void ClearMetaBlackboard()
        {
            BossRaidersUtils.ClearBlackboard();
        }

        public void ReturnSceneToLobby()
        {
            UpdateBadge();
            iconController.UpdateExpGauge();
        }

        public bool CheckMetaGameData()
        {
            return BossRaidersUtils.BossRaidersInfo != null;
        }

        public bool CheckUnlockedLevel()
        {
            return MetaGameUtils.IsMetaGameLevelLocked();
        }

        public void ActiveLevelLockedSpeechBalloon()
        {
            if (ignoreSpeechBalloon) return;

            if (levelLockedSpeechBalloonEnumerator != null)
                StopCoroutine(levelLockedSpeechBalloonEnumerator);
            levelLockedSpeechBalloonEnumerator = StartCoroutine(LevelLockedSpeechBalloonEnumerator());
        }

        private IEnumerator LevelLockedSpeechBalloonEnumerator()
        {
            lockedSpeechAnimator?.SetBool("IsActive", true);
            yield return new WaitForSeconds(1.0f);
            lockedSpeechAnimator?.SetBool("IsActive", false);
        }

        public void SetMetaGameCrashReport()
        {
            BlackboardQueryUtils.MetaGameCrashReport(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_NAME"));
        }

        public void BIClientClickBossRaidersIcon(string contextId, string type)
        {
            BossRaidersUtils.BIClientClickBossRaidersIcon(contextId, type);
        }
    }
}
