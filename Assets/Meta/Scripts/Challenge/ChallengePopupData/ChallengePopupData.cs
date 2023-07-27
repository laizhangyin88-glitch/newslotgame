using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public abstract class ChallengePopupData
    {
        public GameObject owner;
        public PopupChallengeController controller;
        public ContextElement root;
        public Blackboard rootBB;
        public Animator rootAnim;

        public bool isUpdated = false;
        public bool isAuto = false;

        public static Dictionary<MetaChallengeType, ContextElement> tabElementDict = new Dictionary<MetaChallengeType, ContextElement>();
        public static Dictionary<MetaChallengeType, ContextElement> coverElementDict = new Dictionary<MetaChallengeType, ContextElement>();
        public static Dictionary<MetaChallengeType, ContextElement> completeElementDict = new Dictionary<MetaChallengeType, ContextElement>();
        public static Dictionary<MetaChallengeType, ContextElement> badgeElementDict = new Dictionary<MetaChallengeType, ContextElement>();
        public static Dictionary<MetaChallengeType, Animator> badgeAnimatorDict = new Dictionary<MetaChallengeType, Animator>();

        private bool isCompleteInit = false;
        protected static ContextElement challengeCompleteElement;
        protected static ContextElement challengeCompleteRewardTextElement;
        protected static ContextElement challengeCompleteTextElement;
        protected static ContextElement challengeCompleteNextTimerElement;

        private static ContextElement currentTabElement;
        private static ContextElement currentTabCoverElement;
        private static MetaChallengeType currentType = MetaChallengeType.NONE; 

        protected bool isMainTab = false;
        protected bool isBadge = false;
        public bool IsBadge { get { return isBadge; } }

        protected const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        //

        public ChallengePopupData(GameObject _owner)
        {
            owner = _owner;
            controller = owner.GetComponent<PopupChallengeController>();
            root = owner.GetComponent<ContextElement>();
            rootBB = owner.GetComponent<Blackboard>();
            rootAnim = owner.GetComponent<Animator>();

            root.UpdateContext(false);

            if (!isCompleteInit)
            {
                isCompleteInit = true;
                challengeCompleteElement = ContextUtils.FindElement(root, "Challenge Complete", CHILDREN);
                challengeCompleteRewardTextElement = ContextUtils.FindElement(challengeCompleteElement, "Text Reward", CHILDREN);
                challengeCompleteTextElement = ContextUtils.FindElement(challengeCompleteElement, "Text Complete", CHILDREN);
                challengeCompleteNextTimerElement = ContextUtils.FindElement(challengeCompleteElement, "Next Mission Remaining Time", CHILDREN);
            }
        }

        public abstract bool IsComplete(MetaChallengeType tabType);
        public abstract void InitProperty();
        public abstract void UpdateChallengeInfo();
        public abstract void OnUpdateChallengeInfo();
        public abstract void OnSelectTab(MetaChallengeType tabType);
        public abstract void UpdatePopupData();
        public abstract void UpdateBadge();

        //

        public virtual void ClearPopupData() { }

        //

        protected void UpdateGuageProgress(List<ContextElement> gaugeObjList, ContextElement countObj, int progress)
        {
            for (int i = 0; i < gaugeObjList.Count; ++i)
            {
                gaugeObjList[i].gameObject.SetActive(progress > i);
            }

            if (progress < gaugeObjList.Count)
            {
                UpdateCountPosition(countObj, gaugeObjList[progress].gameObject);
            }
            else
            {
                UpdateCountPosition(countObj, gaugeObjList[gaugeObjList.Count - 1].gameObject, false);
            }
        }

        protected void UpdateCountPosition(ContextElement countObj, GameObject targetObj, bool leftAnchor = true)
        {
            RectTransform targetRect = targetObj.GetComponent<RectTransform>();
            RectTransform countRect = countObj.GetComponent<RectTransform>();

            countRect.position = targetRect.position;

            if (leftAnchor)
                countRect.localPosition -= new Vector3(targetRect.rect.width / 2f, 0f, 0f);
            else
                countRect.localPosition += new Vector3(targetRect.rect.width / 2f, 0f, 0f);
        }

        protected void ChangeCurrentTab(ContextElement newCurrentTabElement, ContextElement newCurrentTabCoverElement, MetaChallengeType tabType)
        {
            if (currentTabElement != null)
                MetaContextElementUtils.SetBooleanProperty(currentTabElement, true);
            if (currentTabCoverElement != null)
                currentTabCoverElement.gameObject.SetActive(true);

            MetaContextElementUtils.SetBooleanProperty(newCurrentTabElement, false);
            newCurrentTabCoverElement.gameObject.SetActive(false);

            switch (tabType)
            {
                case MetaChallengeType.DAILY:
                case MetaChallengeType.EXPERT:
                case MetaChallengeType.MASTER:
                case MetaChallengeType.CLUB:
                case MetaChallengeType.EVENT_PERSONAL:
                case MetaChallengeType.EVENT_CLUB:
                    SetActiveTopTab(true);
                    break;
                case MetaChallengeType.NORMAL:
                    SetActiveTopTab(true);
                    break;
                default:
                    SetActiveTopTab(false);
                    break;
            }

            currentTabElement = newCurrentTabElement;
            currentTabCoverElement = newCurrentTabCoverElement;
            currentType = tabType;
        }

        protected void BI_ClientChallengeEnter(Blackboard challengeInfoBB, string challengeType, string enterType)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["enter_type"] = enterType;
            customData["challenge_type"] = challengeType; // "club" "daily" "expert" "master"

            if (challengeInfoBB != null)
            {
                var challengeID = challengeInfoBB.GetValue<string>("challengeId");
                customData["challenge_id"] = challengeID;
            }
            else
            {
                customData["challenge_id"] = "";
            }

            Analytics.CustomEvent("client_challenge_ui_enter", customData);

            isAuto = false;
        }

        protected void ChangeTab(MetaChallengeType tabType)
        {
            var tabElement = tabElementDict[tabType];
            var coverElement = coverElementDict[tabType];
            ChangeCurrentTab(tabElement, coverElement, tabType);

            if (tabType == MetaChallengeType.DAILY)
                rootAnim.SetInteger("Challenge", 0);
            else if (tabType == MetaChallengeType.EXPERT)
                rootAnim.SetInteger("Challenge", 1);
            else if (tabType == MetaChallengeType.MASTER)
                rootAnim.SetInteger("Challenge", 2);
            else if (tabType == MetaChallengeType.CLUB)
                rootAnim.SetInteger("Challenge", 3);
            else if (tabType == MetaChallengeType.EVENT_PERSONAL)
                rootAnim.SetInteger("Challenge", 4);
            else if (tabType == MetaChallengeType.EVENT_CLUB)
                rootAnim.SetInteger("Challenge", 5);
            rootAnim.SetBool("EpicPass", tabType == MetaChallengeType.EPIC_PASS);
        }

        protected void SetActiveTopTab(bool isActive)
        {
            tabElementDict[MetaChallengeType.DAILY]?.gameObject.SetActive(isActive);
            tabElementDict[MetaChallengeType.EXPERT]?.gameObject.SetActive(isActive);
            tabElementDict[MetaChallengeType.MASTER]?.gameObject.SetActive(isActive);
            tabElementDict[MetaChallengeType.CLUB]?.gameObject.SetActive(isActive);
            if (tabElementDict.ContainsKey(MetaChallengeType.EVENT_PERSONAL))
                tabElementDict[MetaChallengeType.EVENT_PERSONAL]?.gameObject.SetActive(isActive);
            if (tabElementDict.ContainsKey(MetaChallengeType.EVENT_CLUB))
                tabElementDict[MetaChallengeType.EVENT_CLUB]?.gameObject.SetActive(isActive);
            if (tabElementDict.ContainsKey(MetaChallengeType.NORMAL))
                MetaContextElementUtils.SetBooleanProperty(tabElementDict[MetaChallengeType.NORMAL], !isActive);
            if (coverElementDict.ContainsKey(MetaChallengeType.NORMAL))
                coverElementDict[MetaChallengeType.NORMAL]?.gameObject.SetActive(!isActive);
        }
    }
}
