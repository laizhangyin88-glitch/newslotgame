using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.ClubArena
{
    public class ClubArenaButtonBaseController : MetaGameEventButtonController
    {
        protected ClubArenaButtonIconController iconController;

        protected ContextElement iconAreaElement;
        protected ContextElement timerAreaElement;
        protected ContextElement remainingTimerElement;

        protected ContextAnimator badgeAnimator;
        protected ContextElement badgeAreaElement;
        protected ContextElement badgeTextElement;

        protected ContextElement lockedIconElement;
        protected ContextElement lockedSpeechBalloonElement;
        private Animator lockedSpeechAnimator;

        protected GameObject gs_managerObj;

        private Coroutine levelLockedSpeechBalloonEnumerator = null;

        public override void InitProperty()
        {
            base.InitProperty();

            ClubArenaUtils.InitClubArena();
        }

        protected void InitProperty(ContextElement baseElement)
        {
            string bundle = BlackboardQueryUtils.GetMetaBundleName(EventInfoType.CLUB_ARENA);
            if (gs_managerObj == null)
                gs_managerObj = MetaObjectUtils.MakePrefab(bundle, "Club Arena Common Sounds", transform);

            iconAreaElement = ContextUtils.FindElement(baseElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            ContextElement iconControllerElement = ContextUtils.FindElement(iconAreaElement, "Club Arena Icon", ContextSearchingType.ChildrenSearch);
            iconController = iconControllerElement.GetComponent<ClubArenaButtonIconController>();
            iconController?.InitProperty();

            timerAreaElement = ContextUtils.FindElement(baseElement, "Event Timer Area", ContextSearchingType.ChildrenSearch);
            remainingTimerElement = ContextUtils.FindElement(timerAreaElement, "Event Timer/Remaining Timer", ContextSearchingType.FullNameSearch);

            badgeAreaElement = ContextUtils.FindElement(baseElement, "Badge Area", ContextSearchingType.ChildrenSearch);

            var badgeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform);
            badgeAnimator = badgeObj.GetComponent<ContextAnimator>();
            badgeAnimator.UpdateContext(false);
            badgeAnimator.isPreserve = true;
            badgeAnimator.propertyName = "value";

            badgeTextElement = ContextUtils.FindElement(badgeAnimator, "Text", ContextSearchingType.ChildrenSearch);
            badgeTextElement.gameObject.SetActive(false);

            ContextElement lockedAreaElement = ContextUtils.FindElement(baseElement, "Locked Area", ContextSearchingType.ChildrenSearch);
            lockedIconElement = ContextUtils.FindElement(lockedAreaElement, "Club Arena Locked Icon", ContextSearchingType.ChildrenSearch);

            if (!ignoreSpeechBalloon)
            {
                lockedSpeechBalloonElement = ContextUtils.FindElement(lockedAreaElement, "Club Arena Locked Info Speech Balloon", ContextSearchingType.ChildrenSearch);
                lockedSpeechAnimator = lockedSpeechBalloonElement.GetComponent<Animator>();
            }

            BlackboardUtils.SetOrCreateValue(remainingTimerElement.GetComponent<NodeCanvas.Framework.Blackboard>(), "caller", gameObject);
        }

        protected virtual void InitText()
        {
            int level = MetaGameUtils.GetMetaUnlockedLevel();
            MetaContextElementUtils.SimpleSetText(lockedIconElement, "Text", level.ToString());

            if (!ignoreSpeechBalloon)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(lockedSpeechBalloonElement, "Text", "META_GAME_LEVEL_UNLOCKED_SPEECH_TEXT", ContextSearchingType.ChildrenSearch, level);
            }
        }

        protected virtual void InitIconController()
        {
            iconController?.UpdateValues();
            iconController?.SetRewardCallback(() => { UpdateBadge(); });
        }

        public virtual void OnInit()
        {
            if (!isInit)
            {
                InitProperty();
                InitText();
            }
            InitIconController();
            UpdateTimer();
            UpdateBadge();
            UpdateUnlockedLevel();

            ClubArenaUtils.CheckMetaStart();

            isInit = true;
        }

        protected void UpdateTimer()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CLUB_ARENA);
            if(eventInfo != null)
                MetaGameUtils.UpdateMetaGameRemainingTimer(timerAreaElement, remainingTimerElement, eventInfo.endTimestamp);
        }

        protected virtual void UpdateBadge()
        {
            if (badgeAnimator == null) return;
            // Unclaimed count
            int totalCount = ClubArenaUtils.SpinPossibleCount > 0 ? 1 : 0;
            badgeAnimator.SetIntProperty(totalCount);

            if (totalCount > 99)
                MetaContextElementUtils.SetText(badgeTextElement, "99+");
            else
                MetaContextElementUtils.SetText(badgeTextElement, totalCount.ToString());
        }

        public virtual void UpdateUnlockedLevel()
        {
            bool isLockedLevel = MetaGameUtils.IsMetaGameLevelLocked();

            iconAreaElement.gameObject.SetActive(!isLockedLevel);
            badgeAreaElement.gameObject.SetActive(!isLockedLevel);

            lockedIconElement.gameObject.SetActive(isLockedLevel);
            lockedSpeechBalloonElement?.gameObject.SetActive(isLockedLevel);
        }

        public bool GetClubber()
        {
            return ClubArenaUtils.IsClubber();
        }

        public virtual void ClearMetaBlackboard()
        {
            ClubArenaUtils.ClearBlackboard();
        }

        public bool CheckMetaGameData()
        {
            return ClubArenaUtils.ClubArenaInfo != null;
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
            BlackboardQueryUtils.MetaGameCrashReport(StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_NAME"));
        }

        public void BIClientClickClubArenaIcon(string contextId, string type)
        {
            ClubArenaUtils.BIClientClickClubArenaIcon(contextId, type);
        }
    }
}
