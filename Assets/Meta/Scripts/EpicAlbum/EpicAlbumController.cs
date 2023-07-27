using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BagelCode;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;
using CanvasGroup = UnityEngine.CanvasGroup;

namespace BagelCode.EpicAlbum
{
    public class EpicAlbumController : MonoBehaviour
    {
        public EpicAlbumAssets epicAlbumAssets;
        private string BUNDLE_NAME = "epicalbum";

        private ContextElement agent;
        private CanvasGroup anchorCanvasGroup;
        private Animator animator;

        private Transform categoryPageArea;
        private Transform frontPageArea;
        private Transform backPageArea;
        private Transform pageArrowsArea;

        private EpicAlbumCategoryController categoryPage;
        private EpicAlbumPageController frontPage;
        private EpicAlbumPageController backPage;
        private List<EpicAlbumTabController> tabs;

        private List<CategoryType> categoryList;

        private ContextElement buttonCloseElement;
        private ContextElement buttonLeftElement;
        private ContextElement buttonRightElement;

        private bool pageChangeFinished = false;

        private int PAGE_FORWARD_1 = 1;
        private int PAGE_FORWARD_2 = 2;
        private int PAGE_FORWARD_3 = 3;
        private int PAGE_BACKWARD_1 = 4;
        private int PAGE_BACKWARD_2 = 5;
        private int PAGE_BACKWARD_3 = 6;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private string ON_CLICK_CLOSE_EVENT = "OnClickClose";
        private string ON_CLICK_CLOSE_CATEGORY_EVENT = "OnClickCloseCategory";
        private string ON_CLICK_INFORMATION_EVENT = "OnClickInformation";
        private string ON_CLICK_TAB_EVENT = "OnClickTab";
        private string ON_CLICK_RIGHT_EVENT = "OnClickRight";
        private string ON_CLICK_LEFT_EVENT = "OnClickLeft";

        public void Init()
        {
            // Remove Migration New Badge.
            BlackboardQueryUtils.SetEpicAlbumMigrationNewBadge(false);

            var epicAlbumBBList = BlackboardQueryUtils.GetEpicAlbumInfoList();
            categoryList = new List<CategoryType>();

            for (int i = 0; i < epicAlbumBBList.Count; i++)
            {
                if (BlackboardQueryUtils.IsEmptyAlbum(epicAlbumBBList[i]))
                    continue;
                categoryList.Add( BlackboardQueryUtils.GetCategoryType( epicAlbumBBList[i]) );
            }

            agent = GetComponent<ContextElement>();
            ContextElement anchor = ContextUtils.FindElement(agent, "Anchor", ContextSearchingType.ChildrenSearch);
            anchorCanvasGroup = anchor.GetComponent<CanvasGroup>();
            animator = GetComponent<Animator>();

            ContextElement categoryPageAreaElement = ContextUtils.FindElement(anchor, "Cover Page Area", ContextSearchingType.ChildrenSearch);
            categoryPageArea = categoryPageAreaElement.transform;

            ContextElement categoryPageElement = ContextUtils.FindElement(categoryPageAreaElement, "Epic Album Cover Page", ContextSearchingType.ChildrenSearch);
            categoryPage = categoryPageElement.GetComponent<EpicAlbumCategoryController>();
            categoryPage.Init(agent);

            ContextElement frontPageAreaElement = ContextUtils.FindElement(anchor, "Front Page Area", ContextSearchingType.ChildrenSearch);
            frontPageArea = frontPageAreaElement.transform;
            ContextElement frontPageElement = ContextUtils.FindElement(frontPageAreaElement, "Epic Album Page", ContextSearchingType.ChildrenSearch);
            frontPage = frontPageElement.GetComponent<EpicAlbumPageController>();
            frontPage.Init(agent);

            ContextElement backPageAreaElement = ContextUtils.FindElement(anchor, "Back Page Area", ContextSearchingType.ChildrenSearch);
            backPageArea = backPageAreaElement.transform;
            ContextElement backPageElement = ContextUtils.FindElement(backPageAreaElement, "Epic Album Page", ContextSearchingType.ChildrenSearch);
            backPage = backPageElement.GetComponent<EpicAlbumPageController>();
            backPage.Init(agent);

            // tabs = new List<EpicAlbumTabController>();
            // ContextElement scrollRectElement = ContextUtils.FindElement(anchor, "Category Tab/Settings Scroll Rect", ContextSearchingType.FullNameSearch);
            // tabScroll = scrollRectElement.GetComponent<ScrollRect>();
            // ContextElement viewportElement = ContextUtils.FindElement(scrollRectElement, "Viewport", ContextSearchingType.ChildrenSearch);
            // viewport = viewportElement.GetComponent<RectTransform>();
            // ContextElement tabContentsElement = ContextUtils.FindElement(viewportElement, "Contents", ContextSearchingType.ChildrenSearch);
            // tabContentsLayoutGroup = tabContentsElement.GetComponent<HorizontalLayoutGroup>();
            // tabRectTransform = tabContentsElement.GetComponent<RectTransform>();
            // for (int i = 0; i < categoryCount; i++)
            // {
            //     //Category Tabs Init
            //     CategoryType categoryType = (CategoryType)i + 1;
            //     var tab = MetaObjectUtils.MakePrefab(BUNDLE_NAME, "Epic Album Category Tab Button", tabContentsElement.transform, null);
            //     EpicAlbumTabController tabController = tab.GetComponent<EpicAlbumTabController>();
            //     tabController.Init((CategoryType) (i+1), epicAlbumAssets);
            //     tabs.Add(tabController);

            //     MetaContextElementUtils.SetClickable(
            //         tabController.GetComponent<ContextElement>(),
            //         ON_CLICK_TAB_EVENT,
            //         i,
            //         agent,
            //         null
            //     );
            //     var tabElement = tab.GetComponent<ContextElement>();
            //     int gameIdCount = BlackboardQueryUtils.GetGameIdList(categoryType).Count;
            //     int woeCount = BlackboardQueryUtils.GetWOEInfoList(categoryType).Select(x => x.GetValue<int>("gameId")).Count(x => BlackboardQueryUtils.GetEarlyAccessSlotInfo(x) == null);
            //     var textElement = ContextUtils.FindElement(tabElement, "Category Achievement/Text", ContextSearchingType.FullNameSearch);
            //     if (gameIdCount==woeCount && woeCount!=0) //If Complete
            //     {
            //         MetaContextElementUtils.SetActive(textElement, false);
            //         var completeElement = ContextUtils.FindElement(tabElement, "Category Achievement Complete", ContextSearchingType.ChildrenSearch);
            //         MetaContextElementUtils.SetActive(completeElement, true);
            //     } else
            //     {
            //         string numberFormat = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_CATEGORY_NUMBER", woeCount, gameIdCount);
            //         MetaContextElementUtils.SetText(textElement, numberFormat);
            //     }
            // }

            ContextElement buttonInformationElement = ContextUtils.FindElement(anchor, "Button Information", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                buttonInformationElement,
                ON_CLICK_INFORMATION_EVENT,
                agent,
                null
            );
            ContextElement buttonInformationTextElement = ContextUtils.FindElement(buttonInformationElement, "Text", ContextSearchingType.ChildrenSearch);
            string buttonInformationText = StringTableUtils.GetString(tableType, "EPIC_ALBUM_QUESTION_BUTTON_TEXT");
            MetaContextElementUtils.SetText(buttonInformationTextElement, buttonInformationText);

            ContextElement pageArrows = ContextUtils.FindElement(anchor, "Page Arrows", ContextSearchingType.ChildrenSearch);
            pageArrowsArea = pageArrows.transform;

            buttonRightElement = ContextUtils.FindElement(pageArrows, "Arrow Right", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                buttonRightElement,
                ON_CLICK_RIGHT_EVENT,
                agent,
                null
            );

            buttonLeftElement = ContextUtils.FindElement(pageArrows, "Arrow Left", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                buttonLeftElement,
                ON_CLICK_LEFT_EVENT,
                agent,
                null
            );

            buttonCloseElement = ContextUtils.FindElement(anchor, "Button Close", ContextSearchingType.ChildrenSearch);

            MetaObjectUtils.MakePrefab(BUNDLE_NAME, "Epic Album Sounds", agent.transform, null);

            InitCover();

            animator.SetBool("Active", true);
        }

        public void InitCover()
        {
            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                ON_CLICK_CLOSE_EVENT,
                agent,
                null
            );

            categoryPage.Activate(true);
            frontPage.Activate(false);
            backPage.Activate(false);
            pageArrowsArea.gameObject.SetActive(false);

            anchorCanvasGroup.interactable = true;
        }

        public void EnterCover()
        {
            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                ON_CLICK_CLOSE_EVENT,
                agent,
                null
            );

            anchorCanvasGroup.interactable = false;
            pageChangeFinished = false;
            categoryPage.Activate(true);
            categoryPage.Play(PAGE_BACKWARD_3);
            frontPage.TopLayerActive(false);
            backPage.TopLayerActive(false);
            GSManager.Instance.GetHandler("Epic_Album_Page_Three").Play();

            StartCoroutine(
                WaitUntilPageChangeFinished(
                    () =>
                    {
                        categoryPage.Activate(true);
                        frontPage.Activate(false);
                        backPage.Activate(false);
                        pageArrowsArea.gameObject.SetActive(false);
                    }
                )
            );
        }

        // use only bi event.
        public string GetCategoryBadgeStatus(int index)
        {
            return categoryPage.GetCategoryBadgeStatus((CategoryType)index);
        }

        public int EnterCategory(int pageIndex)
        {
            if(categoryList == null || categoryList.Count <= pageIndex) return (int) CategoryType.FIRST;

            anchorCanvasGroup.interactable = false;
            pageChangeFinished = false;
            frontPage.Activate(true);
            frontPage.Refresh(categoryList[pageIndex], pageIndex);
            frontPage.TopLayerActive(true);

            categoryPage.Play(PAGE_FORWARD_3);
            GSManager.Instance.GetHandler("Epic_Album_Page_Three").Play();

            StartCoroutine(
                WaitUntilPageChangeFinished(
                    () =>
                    {
                        categoryPage.Activate(false);
                        frontPage.Activate(true);

                        pageArrowsArea.gameObject.SetActive(true);

                        MetaContextElementUtils.SetClickable(
                            buttonCloseElement,
                            ON_CLICK_CLOSE_CATEGORY_EVENT,
                            agent,
                            null
                        );
                    }
                )
            );

            return (int) categoryList[pageIndex];
        }

        public int Activate()
        {
            frontPage.Activate(true);
            frontPage.Refresh();
            backPage.Activate(false);
            // RefreshTabs(frontPage.categoryType);

            return (int) frontPage.categoryType;
        }

        public int OnClickRight()
        {
            if(categoryList == null || categoryList.Count == 0) return (int) CategoryType.FIRST;

            int nextPageIndex = frontPage.pageIndex + 1;
            CategoryType targetCategoryType;
            int pageParameter;

            if (nextPageIndex >= categoryList.Count)
            {
                nextPageIndex = 0;
                targetCategoryType = categoryList[0];
                pageParameter = PAGE_BACKWARD_3;
            }
            else
            {
                targetCategoryType = categoryList[nextPageIndex];
                pageParameter = PAGE_FORWARD_1;
            }

            ChangePage(targetCategoryType, nextPageIndex, pageParameter);

            return (int) targetCategoryType;
        }

        public int OnClickLeft()
        {
            if(categoryList == null || categoryList.Count == 0) return (int) CategoryType.FIRST;

            int nextPageIndex = frontPage.pageIndex - 1;

            CategoryType targetCategoryType;
            int pageParameter;

            if (nextPageIndex < 0)
            {
                nextPageIndex = categoryList.Count -1;
                targetCategoryType = categoryList[nextPageIndex];
                pageParameter = PAGE_FORWARD_3;
            }
            else
            {
                targetCategoryType = categoryList[nextPageIndex];
                pageParameter = PAGE_BACKWARD_1;
            }
            ChangePage(targetCategoryType, nextPageIndex, pageParameter);

            return (int) targetCategoryType;
        }

        public int OnClickTab(int pageIndex)
        {
            if(categoryList == null || categoryList.Count <= pageIndex) return (int) CategoryType.FIRST;

            CategoryType selectedCategoryType = categoryList[pageIndex];

            CategoryType targetCategoryType;
            int pageParameter;

            if (pageIndex > frontPage.pageIndex)
            {
                if (pageIndex - frontPage.pageIndex == 1)
                    pageParameter = PAGE_FORWARD_1;
                else if (pageIndex - frontPage.pageIndex == 2)
                    pageParameter = PAGE_FORWARD_2;
                else
                    pageParameter = PAGE_FORWARD_3;
                targetCategoryType = categoryList[pageIndex];
            }
            else
            {
                if (frontPage.pageIndex - pageIndex == 1)
                    pageParameter = PAGE_BACKWARD_1;
                else if (frontPage.pageIndex - pageIndex == 2)
                    pageParameter = PAGE_BACKWARD_2;
                else
                    pageParameter = PAGE_BACKWARD_3;

                targetCategoryType = categoryList[pageIndex];
            }

            ChangePage(targetCategoryType, pageIndex, pageParameter);

            return (int) targetCategoryType;
        }

        private void ChangePage(CategoryType targetCategoryType, int pageIndex, int pageParameter)
        {
            anchorCanvasGroup.interactable = false;

            if (pageParameter >= PAGE_FORWARD_1 && pageParameter <= PAGE_FORWARD_3)
            {
                if (pageParameter == PAGE_FORWARD_1)
                    GSManager.Instance.GetHandler("Epic_Album_Page_One").Play();
                else if (pageParameter == PAGE_FORWARD_2)
                    GSManager.Instance.GetHandler("Epic_Album_Page_Two").Play();
                else
                    GSManager.Instance.GetHandler("Epic_Album_Page_Three").Play();

                backPage.Activate(true);
                backPage.Refresh(targetCategoryType, pageIndex);
                backPage.TopLayerActive(true);
                // RefreshTabs(targetCategoryType, true);

                frontPage.Play(pageParameter);
                frontPage.TopLayerActive(false);
                StartCoroutine(
                    WaitUntilPageChangeFinished(
                        () =>
                        {
                            SwapPage();
                            backPage.Activate(false);
                        }
                    )
                );
            }
            else if (pageParameter >= PAGE_BACKWARD_1 && pageParameter <= PAGE_BACKWARD_3)
            {
                if (pageParameter == PAGE_BACKWARD_1)
                    GSManager.Instance.GetHandler("Epic_Album_Page_One").Play();
                else if (pageParameter == PAGE_BACKWARD_2)
                    GSManager.Instance.GetHandler("Epic_Album_Page_Two").Play();
                else
                    GSManager.Instance.GetHandler("Epic_Album_Page_Three").Play();

                SwapPage();

                frontPage.Activate(true);
                frontPage.Refresh(targetCategoryType, pageIndex);
                frontPage.TopLayerActive(true);
                // RefreshTabs(targetCategoryType, true);

                frontPage.Play(pageParameter);
                // frontPage.TopLayerActive(true);
                backPage.TopLayerActive(false);

                StartCoroutine(
                    WaitUntilPageChangeFinished(
                        () =>
                        {
                            backPage.Activate(false);
                        }
                    )
                );
            }
        }

        private IEnumerator WaitUntilPageChangeFinished(Action callback)
        {
            yield return new WaitUntil(() => pageChangeFinished);

            callback();
            anchorCanvasGroup.interactable = true;

            // Unity Interactable Bug. ///////////////////////////////////////////
            MetaContextElementUtils.SetBooleanProperty(buttonCloseElement, false);
            MetaContextElementUtils.SetBooleanProperty(buttonLeftElement, false);
            MetaContextElementUtils.SetBooleanProperty(buttonRightElement, false);

            MetaContextElementUtils.SetBooleanProperty(buttonCloseElement, true);
            MetaContextElementUtils.SetBooleanProperty(buttonLeftElement, true);
            MetaContextElementUtils.SetBooleanProperty(buttonRightElement, true);
            //////////////////////////////////////////////////////////////////////

            pageChangeFinished = false;
        }

        public void PageChangeFinished()
        {
            pageChangeFinished = true;
        }

        private void SwapPage()
        {
            Vector3 backPagePosition = backPage.transform.localPosition;
            Vector3 frontPagePosition = frontPage.transform.localPosition;

            backPage.transform.SetParent(frontPageArea);
            frontPage.transform.SetParent(backPageArea);

            EpicAlbumPageController tempPage = frontPage;
            frontPage = backPage;
            backPage = tempPage;

            frontPage.transform.localPosition = frontPagePosition;
            backPage.transform.localPosition = backPagePosition;
        }

        // private void RefreshTabs(CategoryType categoryType, bool refreshScroll = false)
        // {
        //     for (int i = 0; i < categoryCount; i++)
        //     {
        //         tabs[i].Refresh(categoryType);
        //     }

        //     if (refreshScroll)
        //         RefreshTabScroll(categoryType);
        // }

        public void RefreshCell(int index)
        {
            frontPage.Refresh(index);

            // tabs[(int) frontPage.categoryType - 1].Refresh();
        }

        // private void RefreshTabScroll(CategoryType targetCategoryType)
        // {
        //     float marginRight = Math.Abs(viewport.offsetMax.x);
        //     float marginLeft = Math.Abs(viewport.offsetMin.x);
        //     float canvasWidth = mainCanvas.GetComponent<RectTransform>().sizeDelta.x - (marginLeft + marginRight);

        //     float tabScrollOffset = tabScroll.content.anchoredPosition.x;
        //     float tabWidth = tabs[0].GetComponent<RectTransform>().sizeDelta.x;

        //     int paddingLeft = tabContentsLayoutGroup.padding.left;
        //     int paddingRight = tabContentsLayoutGroup.padding.right;
        //     float spacing = tabContentsLayoutGroup.spacing;

        //     int index = (int) targetCategoryType - 1;
        //     int maxIndex = (int) maxCategoryType - 1;

        //     float tabLeftSideDistanceFromStart = paddingLeft + tabWidth * index + (index > 0 ? index * spacing : 0) - (index > 0 ? spacing : paddingLeft);
        //     float tabLeftSideOffset = tabScrollOffset + tabLeftSideDistanceFromStart;

        //     float tabRightSideDistanceFromStart = paddingLeft + tabWidth * (index + 1) + (index > 0 ? index * spacing : 0) + (index < maxIndex ? spacing : paddingRight);
        //     float tabRightSideOffset = tabScrollOffset + tabRightSideDistanceFromStart;

        //     if (tabLeftSideOffset >= marginLeft && tabRightSideOffset <= canvasWidth - marginRight)
        //         return;

        //     Vector3 targetPosition = tabRectTransform.anchoredPosition;
        //     float targetXPosition;

        //     if (tabLeftSideOffset < marginLeft)
        //         targetXPosition = -tabLeftSideDistanceFromStart + (targetCategoryType == CategoryType.FIRST ? 0 : tabWidth * 2 / 5);
        //     else
        //         targetXPosition = -tabRightSideDistanceFromStart + canvasWidth - (targetCategoryType == maxCategoryType ? 0 : tabWidth * 2 / 5);

        //     targetPosition.x = targetXPosition;

        //     StartCoroutine(StartMoveScroll(tabRectTransform.anchoredPosition, targetPosition, 0.5f));
        // }

        // private IEnumerator StartMoveScroll(Vector3 startPosition, Vector3 targetPosition, float targetTime)
        // {
        //     float passedTime = 0f;

        //     while (passedTime + Time.deltaTime < targetTime)
        //     {
        //         passedTime += Time.deltaTime;

        //         float x = passedTime / targetTime;
        //         float y = -x * x + 2 * x;
        //         tabRectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, y);

        //         yield return null;
        //     }

        //     tabRectTransform.anchoredPosition = targetPosition;
        // }

        public int GetCategoryType()
        {
            return (int) frontPage.categoryType;
        }

        public int GetGameId(int index)
        {
            List<int> gameIdList = BlackboardQueryUtils.GetGameIdList(frontPage.categoryType);
            int gameId = gameIdList.Count > index ? gameIdList[index] : -1;

            return gameId;
        }

        public void RefreshProgress()
        {
            frontPage.RefreshProgress();
        }
    }
}
