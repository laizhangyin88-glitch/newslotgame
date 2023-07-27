using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupChallengeCTAFriendSuggestChooseController : MonoBehaviour
    {
        public int maxSuggestCount = 0;
        public List<string> targetUserIdList = new List<string>();

        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement addButtonElement;
        private ContextElement scrollContentsElement;

        private bool isInit = false;

        private const int ACTIVE_BUTTON_COUNT = 0;
        private const int MAX_RECOMMENDATION_LIST = 50;

        public void OnInit()
        {
            if (isInit) return;

            InitProperty();
            InitData();

            isInit = true;
        }

        private void InitProperty()
        {
            rootAnimator = gameObject.GetComponent<Animator>();

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            ContextElement titleAreaElement = ContextUtils.FindElement(rootElement, "Title Area", ContextSearchingType.ChildrenSearch);

            ContextElement closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
            addButtonElement = ContextUtils.FindElement(rootElement, "Button Get", ContextSearchingType.ChildrenSearch);

            ContextElement scrollElement = ContextUtils.FindElement(rootElement, "Scroll", ContextSearchingType.ChildrenSearch);
            scrollContentsElement = ContextUtils.FindElement(scrollElement, "Contents", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SimpleSetTextGlobal(titleAreaElement, "Text", "POPUP_CHALLENGE_FRIEND_SUGGEST_CHOOSE_TITLE", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(addButtonElement, "Text", "POPUP_CHALLENGE_FRIEND_SUGGEST_CHOOSE_BUTTON", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                OnClickClose);
            MetaContextElementUtils.SetClickable(
                addButtonElement,
                "OnClickAddFriend",
                rootElement,
                null
            );

            MetaSystem.SubscribeBackButton(this.GetHashCode(), OnClickClose);
        }

        private void InitData()
        {
            List<Blackboard> challengeFriendRecommendationList = BlackboardQueryUtils.GetChallengeFriendRecommendationList();

            if (challengeFriendRecommendationList == null || challengeFriendRecommendationList.Count == 0)
                maxSuggestCount = 0;
            else
                maxSuggestCount = challengeFriendRecommendationList.Count;

            if (maxSuggestCount > 0)
                CreateFriendListItem(challengeFriendRecommendationList);
            SetActiveAddButton(maxSuggestCount > 0);
        }

        private void CreateFriendListItem(List<Blackboard> friendList)
        {
            string assetName = "Friend Suggest Choose Cell";
            string profileAssetName = "Profile Picture Normal";
            string profileName = "Profile Picture";
            int index = 0;
            foreach(Blackboard bb in friendList)
            {
                GameObject cellObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, scrollContentsElement.transform, null, string.Format("Cell {0:00}", index++));
                Transform profileArea = cellObject.transform.Find("Anchor").Find("Proifle Picture Area");
                MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, profileAssetName, profileArea, null, profileName);

                Blackboard cellBB = cellObject.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(cellBB, "caller", gameObject);
                BlackboardUtils.SetOrCreateValue(cellBB, "info", bb);
            }
        }

        private void SetActiveAddButton(bool isActive)
        {
            addButtonElement.gameObject.SetActive(isActive);
        }

        public void OnClickClose()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
            PopupManager.Instance.Close(gameObject);
            rootAnimator?.SetTrigger("Close");
        }

        public void OnToggleFriend()
        {
            SetActiveAddButton(targetUserIdList.Count > ACTIVE_BUTTON_COUNT);
        }
    }
}