using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.EpicAlbum
{
    public class EpicAlbumPageController : MonoBehaviour, IEpicAlbumPageElement
    {
        public CategoryType categoryType;
        public int pageIndex;
        private Blackboard epicAlbumInfoBB;
        private int albumEntryLimit;
        private long finalRewardGem;

        private ContextElement root;
        private ContextElement agent;
        private ContextElement anchor;
        private ContextElement categoryName;
        private ContextElement categoryIconAreaElement;
        private ContextElement categoryGaugeElement;
        private ContextButton  completeRewardElement;
        private ContextElement rewardProgressAreaElement;
        private ContextElement rewardCompleteAreaElement;
        private ContextElement rewardProgressTextElement;
        private ContextElement rewardCompleteTextElement;
        private ContextElement completeTextElement;
        private ContextElement middleRewardAreaElement;
        private ContextButton  middleRewardElement;
        private Animator middleRewardAnimator;
        private Animator completeRewardAnimator;
        private Animator animator;

        private GameObject categoryObj = null;
        private GameObject middleRewardObj = null;

        private List<EpicAlbumCellController> cells;
        private EpicAlbumController epicAlbum;

        private List<int> gameIDList;
        private int totalStarCount;
        private float gaugeElemtnWidth;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const string ON_CLICK_CELL_EVENT = "OnClickCell";
        private const string ON_COLLECT_CATEGORY_REWARD_EVENT = "OnCollectCategoryReward";
        private const string BUNDLE_NAME = "epicalbum";
        private const string MIDDLE_REWARD_PREFAB = "Epic Album Middle Reward Base";
        private const int STAR_COUNT = 3;

        public void Init(ContextElement parent)
        {
            root = parent;
            epicAlbum = parent.GetComponent<EpicAlbumController>();
            agent = GetComponent<ContextElement>();
            agent.UpdateContext();
            animator = GetComponent<Animator>();
            albumEntryLimit = BlackboardQueryUtils.GetEpicAlbumEntryLimit();
            finalRewardGem = BlackboardQueryUtils.GetEpicAlbumFianlRewardGem();

            categoryType = CategoryType.FIRST;

            anchor = ContextUtils.FindElement(agent, "Anchor", ContextSearchingType.ChildrenSearch);

            cells = new List<EpicAlbumCellController>();
            for (int i = 0; i < albumEntryLimit; i++)
            {
                ContextElement cell = ContextUtils.FindElement(anchor, string.Format("Cell 0{0}/Slot Cell", i + 1), ContextSearchingType.FullNameSearch);
                EpicAlbumCellController cellController = cell.GetComponent<EpicAlbumCellController>();
                cellController.Init();
                cells.Add(cellController);

                MetaContextElementUtils.SetClickable(
                    cell,
                    ON_CLICK_CELL_EVENT,
                    i,
                    parent,
                    null
                );
            }

            categoryName = ContextUtils.FindElement(anchor, "Text Category Name", ContextSearchingType.ChildrenSearch);
            categoryIconAreaElement = ContextUtils.FindElement(anchor, "Category Icon Area", ContextSearchingType.ChildrenSearch);

            var gaugeElement = ContextUtils.FindElement(anchor, "Gauge", ContextSearchingType.ChildrenSearch);
            gaugeElemtnWidth = gaugeElement.gameObject.GetComponent<RectTransform>().sizeDelta.x;

            categoryGaugeElement = ContextUtils.FindElement(anchor, "Gauge/Slider", ContextSearchingType.FullNameSearch);

            completeRewardElement = ContextUtils.FindElement(anchor, "Gauge/Epic Album Complete Reward Base", ContextSearchingType.FullNameSearch) as ContextButton;
            completeRewardAnimator = completeRewardElement.GetComponent<Animator>();

            rewardProgressAreaElement = ContextUtils.FindElement(anchor, "Gauge/Epic Album Complete Reward Base/Progress Base", ContextSearchingType.FullNameSearch);
            rewardProgressTextElement = ContextUtils.FindElement(rewardProgressAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            rewardCompleteAreaElement = ContextUtils.FindElement(anchor, "Gauge/Epic Album Complete Reward Base/Complete Base", ContextSearchingType.FullNameSearch);
            rewardCompleteTextElement = ContextUtils.FindElement(rewardCompleteAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            completeTextElement = ContextUtils.FindElement(anchor, "Gauge/Complete Text Base", ContextSearchingType.FullNameSearch);
            middleRewardAreaElement = ContextUtils.FindElement(anchor, "Gauge/Middle Reward Anchor", ContextSearchingType.FullNameSearch);

            middleRewardObj = MetaObjectUtils.MakePrefab(BUNDLE_NAME, MIDDLE_REWARD_PREFAB, middleRewardAreaElement.transform);
            middleRewardAnimator = middleRewardObj.GetComponent<Animator>();

            middleRewardElement = middleRewardObj.GetComponent<ContextElement>() as ContextButton;
            middleRewardElement.UpdateContext();

            MetaContextElementUtils.SetText(rewardProgressTextElement, StringTableUtils.GetString(tableType, "EPIC_ALBUM_FINAL_REWARD_VALUE_TEXT", finalRewardGem));
            MetaContextElementUtils.SetText(rewardCompleteTextElement, StringTableUtils.GetString(tableType, "EPIC_ALBUM_FINAL_REWARD_VALUE_COLLECT_TEXT", finalRewardGem));
        }

        // Interface
        public void PageChangeFinished()
        {
            if(epicAlbum != null)
                epicAlbum.PageChangeFinished();
        }

        public void Refresh()
        {
            Refresh(categoryType, pageIndex);
        }

        public void Refresh(CategoryType newCategoryType, int newPageIndex)
        {
            categoryType = newCategoryType;
            pageIndex = newPageIndex;

            epicAlbumInfoBB = BlackboardQueryUtils.GetEpicAlbumInfo(categoryType);
            totalStarCount = epicAlbumInfoBB.GetValue<int>("totalStarCount");
            gameIDList = BlackboardQueryUtils.GetGameIdList(categoryType);

            for (int i = 0; i < albumEntryLimit; i++)
            {
                cells[i].Refresh(categoryType, gameIDList.Count > i ? gameIDList[i] : -1);
            }

            RefreshCategory();
            RefreshProgress();
        }

        public void RefreshCategory()
        {
            if(categoryObj != null)
                GameObject.Destroy(categoryObj);

            var iconName = StringTableUtils.GetString(tableType, string.Format("EPIC_ALBUM_CATEGORY_NAME_{0}", (int)(categoryType-1)));
            categoryObj = MetaObjectUtils.MakePrefab(BUNDLE_NAME, iconName, categoryIconAreaElement.transform);

            int index = (int) categoryType - 1;
            string name = StringTableUtils.GetString(tableType, "EPIC_ALBUM_CATEGORY_NAME_" + index);

            int woeCount = 0;
            var woeInfoList = BlackboardQueryUtils.GetWOEInfoList(categoryType);
            if (woeInfoList != null)
            {
                for (int i = 0; i < woeInfoList.Count; i++)
                {
                    int gameId = woeInfoList[i].GetValue<int>("gameId");
                    Blackboard earlyAccessSlotInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameId);
                    if (earlyAccessSlotInfo == null)
                        woeCount++;
                }
            }
            string nameFormat = StringTableUtils.GetString(tableType, "EPIC_ALBUM_CATEGORY_NAME_FORMAT", name, woeCount, gameIDList.Count);

            MetaContextElementUtils.SetText(categoryName, nameFormat);
        }

        public void RefreshProgress()
        {
            int gameIdCount = gameIDList.Count;

            completeTextElement.gameObject.SetActive(false);
            middleRewardObj.SetActive(false);
            completeRewardElement.gameObject.SetActive(false);
            MetaContextElementUtils.SetFloatProperty(categoryGaugeElement, 0f);

            completeRewardElement.RemoveAllListener();
            middleRewardElement.RemoveAllListener();
            completeRewardElement.SetBooleanProperty(false);
            middleRewardElement.SetBooleanProperty(false);

            middleRewardAnimator.SetBool("Active", false);
            completeRewardAnimator.SetBool("Active", false);

            if(gameIdCount == 0)
            {
                // Comming Soon???
            }
            else
            {
                // Reward Stage.
                var collectedRewardStage = epicAlbumInfoBB.GetValue<int>("collectedRewardStage");
                if(collectedRewardStage < gameIdCount)
                {
                    int maxStarCount = gameIdCount * STAR_COUNT;
                    // Gauge.
                    MetaContextElementUtils.SetFloatProperty(categoryGaugeElement, totalStarCount == maxStarCount ? 1f : (float)totalStarCount/(float)maxStarCount);

                    if(albumEntryLimit == gameIdCount && collectedRewardStage + 1 == gameIdCount)
                    {
                        // Last Reward
                        rewardCompleteAreaElement.gameObject.SetActive(totalStarCount == maxStarCount);
                        rewardCompleteTextElement.gameObject.SetActive(totalStarCount == maxStarCount);

                        rewardProgressAreaElement.gameObject.SetActive(totalStarCount != maxStarCount);
                        rewardProgressTextElement.gameObject.SetActive(totalStarCount != maxStarCount);
                        completeRewardElement.SetBooleanProperty(totalStarCount == maxStarCount);
                        MetaContextElementUtils.SetClickable(
                            completeRewardElement,
                            ON_COLLECT_CATEGORY_REWARD_EVENT,
                            (int)categoryType,
                            root,
                            null
                        );
                        completeRewardElement.gameObject.SetActive(true);
                        completeRewardAnimator.SetBool("Active", totalStarCount == maxStarCount);
                    }
                    else
                    {
                        if(albumEntryLimit == gameIdCount)
                        {
                            rewardCompleteAreaElement.gameObject.SetActive(false);
                            rewardCompleteTextElement.gameObject.SetActive(false);
                            rewardProgressAreaElement.gameObject.SetActive(true);
                            rewardProgressTextElement.gameObject.SetActive(true);
                            completeRewardElement.gameObject.SetActive(true);
                        }

                        // Make Middle Reward.
                        float posX = Mathf.Min(gaugeElemtnWidth, gaugeElemtnWidth/(float)gameIdCount * ((float)collectedRewardStage + 1f));
                        middleRewardObj.transform.localPosition = new Vector3(posX, middleRewardObj.transform.localPosition.y, middleRewardObj.transform.localPosition.z);
                        middleRewardObj.SetActive(true);
                        var middleRewardStarCount = (collectedRewardStage + 1) * STAR_COUNT;

                        if(totalStarCount < middleRewardStarCount)
                        {
                            // Count
                            // MetaContextElementUtils.SimpleSetActive(middleRewardElement, "Progress Base", true);
                            // MetaContextElementUtils.SimpleSetActive(middleRewardElement, "Complete Base", false);
                            middleRewardAnimator.SetBool("Active", false);
                            var nextRequiredStarCount = epicAlbumInfoBB.GetValue<int>("nextRequiredStarCount");
                            var countText = StringTableUtils.GetString(tableType, "EPIC_ALBUM_CATEGORY_STAR_COUNT_TEXT", totalStarCount, nextRequiredStarCount);

                            MetaContextElementUtils.SimpleSetText(middleRewardElement, "Progress Base/Text", countText, ContextSearchingType.FullNameSearch);
                        }
                        else
                        {
                            // Collect
                            // MetaContextElementUtils.SimpleSetActive(middleRewardElement, "Progress Base", false);
                            // MetaContextElementUtils.SimpleSetActive(middleRewardElement, "Complete Base", true);
                            middleRewardAnimator.SetBool("Active", true);
                            middleRewardElement.SetBooleanProperty(true);
                            MetaContextElementUtils.SetClickable(
                                middleRewardElement,
                                ON_COLLECT_CATEGORY_REWARD_EVENT,
                                (int)categoryType,
                                root,
                                null
                            );
                        }
                    }
                }
                else
                {
                    MetaContextElementUtils.SetFloatProperty(categoryGaugeElement, 1f);
                    // Collect ALL..
                    if(gameIdCount == albumEntryLimit)
                    {
                        completeTextElement.gameObject.SetActive(true);
                    }
                    else
                    {
                        // Commin Soon?
                    }
                }
            }
        }

        public void Activate(bool active)
        {
            MetaContextElementUtils.SetActive(anchor, active);
        }

        public void Play(int pageParameter)
        {
            animator.SetInteger("Page", pageParameter);
        }

        public void TopLayerActive(bool active)
        {
            animator.SetBool("Content Active", active);
        }

        public void Refresh(int index)
        {
            cells[index].Refresh();
        }

        // public void CollectGem(int categoryIndex, long earnGem)
        // {
        //     if((int)categoryType == categoryIndex)
        //     {
        //         // Debug.LogError(categoryIndex);
        //         // Debug.LogError(earnGem);
        //         RefreshProgress();
        //     }
        // }
    }
}
