using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.EpicPass
{
    public class EpicPassLobbyButtonController : MetaGameEventButtonController
    {
        private ContextElement rootElement;

        private EpicPassButtonIconController iconController;

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

        private Coroutine levelLockedSpeechBalloonEnumerator = null;

        public override void InitProperty()
        {
            if(isInit) return;

            base.InitProperty();

            string bundle = BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS);
            if (gs_managerObj == null)
                gs_managerObj = MetaObjectUtils.MakePrefab(bundle, "Epic Pass Common Sounds", transform);

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            iconAreaElement = ContextUtils.FindElement(rootElement, "Icon Area", ContextSearchingType.ChildrenSearch);

            var iconControllerElement = ContextUtils.FindElement(iconAreaElement, "Epic Pass Icon", ContextSearchingType.ChildrenSearch);
            iconController = iconControllerElement.gameObject.GetComponent<EpicPassButtonIconController>();
            iconController.InitProperty();

            timerAreaElement = ContextUtils.FindElement(rootElement, "Event Timer Area", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(timerAreaElement, "Event Timer/Remaining Timer", ContextSearchingType.FullNameSearch);

            badgeAreaElement = ContextUtils.FindElement(rootElement, "Badge Area", ContextSearchingType.ChildrenSearch);

            var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform);
            badgeAnimator = badgeObj.GetComponent<ContextAnimator>();
            badgeAnimator.UpdateContext(false);
            badgeAnimator.isPreserve = true;
            badgeAnimator.propertyName = "value";

            badgeTextElement = ContextUtils.FindElement(badgeAnimator, "Text", ContextSearchingType.ChildrenSearch);

            ContextElement lockedAreaElement = ContextUtils.FindElement(rootElement, "Locked Area", ContextSearchingType.ChildrenSearch);
            lockedIconElement = ContextUtils.FindElement(lockedAreaElement, "Epic Pass Icon Locked", ContextSearchingType.ChildrenSearch);

            if (!ignoreSpeechBalloon)
            {
                lockedSpeechBalloonElement = ContextUtils.FindElement(lockedAreaElement, "Epic Pass Locked Info Speech Balloon", ContextSearchingType.ChildrenSearch);
                lockedSpeechAnimator = lockedSpeechBalloonElement.GetComponent<Animator>();
            }

            MetaContextElementUtils.SetClickable(
                rootElement,
                "OnEnterSeasonPass",
                rootElement,
                null
            );


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

            iconController.UpdateValues();
            iconController.SetRewardCallback(()=>{UpdateBadge();});

            UpdateBadge();
            UpdateTimer();
            UpdateUnlockedLevel();
        }

        private void UpdateTimer()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.SEASON_PASS);
            if(eventInfo != null)
                MetaGameUtils.UpdateMetaGameRemainingTimer(timerAreaElement, remainingTimerElement, eventInfo.endTimestamp);
        }

        private void UpdateBadge()
        {
            if(badgeAnimator == null) return;
            // Unclaimed count
            int totalCount = EpicPassUtils.UnclaimedRewardCount;
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
            BlackboardQueryUtils.MetaGameCrashReport(StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_NAME"));
        }
    }
}
