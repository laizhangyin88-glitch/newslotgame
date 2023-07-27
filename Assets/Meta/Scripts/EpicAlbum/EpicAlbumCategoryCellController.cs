using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.EpicAlbum
{
    public class EpicAlbumCategoryCellController : MonoBehaviour
    {
        public CategoryType categoryType;
        public int pageIndex;
        private Blackboard categoryInfoBB;
        private Blackboard epicAlbumInfoBB;
        private PIDButton button;

        private ContextElement agent;
        private ContextElement iconAreaElement;
        private ContextElement badgeAreaElement;
        private ContextElement starCountTextElement;
        private ContextElement completeElement;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private const string BUNDLE_NAME = "epicalbum";

        private const string ON_CLICK_CATEGORY_EVENT = "OnSelectCategory";
        private const int STAR_COUNT = 3;

        private bool isComplete;
        private bool isNew;

        private int badgeCount = 0;

        public void Init(Blackboard newEpicAlbumInfoBB, int newPageIndex, ContextElement root)
        {
            epicAlbumInfoBB = newEpicAlbumInfoBB;
            categoryInfoBB = epicAlbumInfoBB.GetValue<Blackboard>("categoryInfo");
            var newCategoryType = categoryInfoBB.GetValue<CategoryType>("categoryType");

            categoryType = newCategoryType;
            pageIndex = newPageIndex;
            agent = GetComponent<ContextElement>();
            agent.UpdateContext();

            iconAreaElement = ContextUtils.FindElement(agent, "Club Icon Area", ContextSearchingType.ChildrenSearch);
            starCountTextElement = ContextUtils.FindElement(agent, "Star Base/Text Slot Name", ContextSearchingType.FullNameSearch);
            completeElement = ContextUtils.FindElement(agent, "Complete Base", ContextSearchingType.ChildrenSearch);
            badgeAreaElement = ContextUtils.FindElement(agent, "Badge Area", ContextSearchingType.ChildrenSearch);

            var buttonElement = ContextUtils.FindElement(agent, "Button Cell", ContextSearchingType.ChildrenSearch);
            button = buttonElement.gameObject.GetComponent<PIDButton>();

            MetaContextElementUtils.SetClickable(
                buttonElement,
                ON_CLICK_CATEGORY_EVENT,
                pageIndex,
                root,
                null
            );

            UpdateStaticValues();
            // UpdateDynamicValues();
        }

        private void UpdateStaticValues()
        {
            var iconName = StringTableUtils.GetString(tableType, string.Format("EPIC_ALBUM_CATEGORY_NAME_{0}", (int)(categoryType-1)));
            var iconObj = MetaObjectUtils.MakePrefab(BUNDLE_NAME, iconName, iconAreaElement.transform);

            int gameIdCount = categoryInfoBB.GetValue<List<int>>("gameIdList").Count;
            int totalStarCount = epicAlbumInfoBB.GetValue<int>("totalStarCount");

            isComplete = false;

            if(gameIdCount == 0)
            {
                // Comming Soon???
                string countText = StringTableUtils.GetString(tableType, "EPIC_ALBUM_CATEGORY_STAR_COUNT_TEXT", 0, 0);
                MetaContextElementUtils.SetText(starCountTextElement, countText);
                starCountTextElement.gameObject.SetActive(true);
                completeElement.gameObject.SetActive(false);
            }
            else
            {
                int albumEntryLimit = BlackboardQueryUtils.GetEpicAlbumEntryLimit();
                int maxStarCount = gameIdCount * STAR_COUNT;

                if(totalStarCount == maxStarCount && gameIdCount == albumEntryLimit)
                {
                    starCountTextElement.gameObject.SetActive(false);
                    completeElement.gameObject.SetActive(true);

                    isComplete = true;
                }
                else
                {
                    string countText = StringTableUtils.GetString(tableType, "EPIC_ALBUM_CATEGORY_STAR_COUNT_TEXT", totalStarCount, maxStarCount);
                    MetaContextElementUtils.SetText(starCountTextElement, countText);
                    starCountTextElement.gameObject.SetActive(true);
                    completeElement.gameObject.SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            UpdateDynamicValues();
        }

        private void UpdateDynamicValues()
        {
            if(agent == null) return;

            badgeCount = BlackboardQueryUtils.GetEpicAlbumRewardCount(categoryType);
            bool isShowBadge = badgeCount > 0 ? true : false;

            if(!isShowBadge)
                isShowBadge = BlackboardQueryUtils.CheckIfNewExistInCategory(categoryType);

            MetaContextElementUtils.SetActive(badgeAreaElement, isShowBadge);

            isNew = isShowBadge;

            if(isShowBadge)
            {
                if(badgeCount > 0)
                {
                    MetaObjectUtils.UpdateBadge(badgeAreaElement, false, badgeCount);
                }
                else
                {
                    badgeCount = 1;
                    MetaObjectUtils.UpdateBadge(badgeAreaElement, true);
                }

                ContextElement badgeElement = ContextUtils.FindElement(badgeAreaElement, "Badge", ContextSearchingType.ChildrenSearch);
                StartCoroutine(SetBadgeValue(badgeElement as ContextAnimator));
            }
        }

        // use only bi event.
        public string GetCategoryBadgeStatus()
        {
            if(isComplete) return "Complete";
            if(isNew) return "New";
            return "Default";
        }

        private IEnumerator SetBadgeValue(ContextAnimator contextAnimator)
        {
            yield return null;
            contextAnimator.SetIntProperty(badgeCount);
        }
    }
}
