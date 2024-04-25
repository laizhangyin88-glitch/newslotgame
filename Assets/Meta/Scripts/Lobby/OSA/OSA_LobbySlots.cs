using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using frame8.Logic.Misc.Other;
using frame8.Logic.Misc.Other.Extensions;
using frame8.Logic.Misc.Visual.UI;
using Com.TheFallenGames.OSA.Core;
using System.IO;
using System.Text;
using SlotMaker.Json;

namespace BagelCode.OSA_Scroll
{
    public class OSA_LobbySlots : OSA<LobbySlotsParams, LobbySlotsItem>
    {
        private bool isInteractable = true;

        private int showBackButtonTargetSlotIndex = 8;
        private double showBackButtonTargetDelta = 0;

        private Variable<bool> isShowLobbyBackButton;

        /// MachineUse
        private bool initMod = true;

        //特别注意深坑!
        //当往后翻页时, 隐藏Item在左侧
        //当往前翻页时, 隐藏Item在右侧
        private int offsetIndex = 1;

        private int curPage = 0;
        private int curSelectIndex = 0;
        private int maxIndex = 0;
        private bool isMovingPage;
        private int hallScrollviewItemCount = 0;

        private GameFilter curShowGameType = GameFilter.UNKNOWN;

        public void Refresh()
        {
            ClearVisibleItems();
            // ClearCachedRecyclableItems();
            CreateSlotList();
            ResetCurSelect();
        }

        /* 旧版本
        public void SelectNextItem()
        {
            if (isMovingPage)
                return;
            curSelectIndex++;
            GSManager.Instance.GetHandler("UI_Button_Normal").Play();
            if (maxIndex > 0 &&
                ((initMod && curSelectIndex == maxIndex)
                || !initMod && curSelectIndex == maxIndex - 2)
            )
            {
                MachineMovePage(1);
                offsetIndex = 1;
                maxIndex = 0;
            }
            else
                ChangeCurSelect();
        }
         */
        public void SelectNextItem()
        {
            if (isMovingPage)
                return;
            curSelectIndex++;

            //bool isFirstPage =  0 == GetItemViewsHolder(0).ItemIndex;
            bool isLastPage = _Params.data.Count - 1 == GetItemViewsHolder(_VisibleItems.Count - 1).ItemIndex;

            if (isLastPage && curSelectIndex > maxIndex - 1)
                curSelectIndex = maxIndex - 1;

            Debug.LogWarning($"@#@ ++ curSelectIndex ={curSelectIndex} maxIndex={maxIndex}  isLastPage = {isLastPage} offsetIndex={offsetIndex}");
            //Debug.LogWarning($"@ firstColIndex {GetItemViewsHolder(0).ItemIndex}  lastColIndex {GetItemViewsHolder(_VisibleItems.Count -1).ItemIndex}  allColCount {_VisibleItems.Count} ");

            GSManager.Instance.GetHandler("UI_Button_Normal").Play();
            if (!isLastPage && maxIndex > 0 &&
            ((initMod && curSelectIndex == maxIndex) //最后一列时开始跳转
            || !initMod && curSelectIndex == maxIndex - 2)  //提前一列开始跳转
            )
            {
                //Debug.LogWarning($"@#@  ++ 跳转起点{GetItemViewsHolder(_VisibleItems.Count - 1).ItemIndex -1}");
                MachineMovePage(1);
                offsetIndex = 1;
                maxIndex = 0;
            }
            else
                ChangeCurSelect();
        }

        /* 旧版本
        public void SelectPreItem()
        {
            if (isMovingPage)
                return;
            GSManager.Instance.GetHandler("UI_Button_Normal").Play();
            if (curPage > 0)
                curSelectIndex--;
            else
                curSelectIndex = curSelectIndex > 0 ? curSelectIndex - 1 : 0;
            if (curSelectIndex == offsetIndex && curPage > 0)
            {
                MachineMovePage(-1);
                offsetIndex = -1;
                maxIndex = 0;
            }
            else
                ChangeCurSelect();
        } 
         */

        public void SelectPreItem()
        {
            if (isMovingPage)
                return;

            // Debug.LogWarning($"@  first {GetItemViewsHolder(0).ItemIndex}  last {GetItemViewsHolder(_VisibleItems.Count - 1).ItemIndex} count {_VisibleItems.Count}  col {col} allcol {_Params.data.Count}");

            GSManager.Instance.GetHandler("UI_Button_Normal").Play();
            curSelectIndex--;
            bool isFirstPage = 0 == GetItemViewsHolder(0).ItemIndex;
            if (isFirstPage && curSelectIndex < 0)
                curSelectIndex = 0;



            Debug.LogWarning($"@#@ ++ curSelectIndex ={curSelectIndex} maxIndex={maxIndex}  isFirstPage = {isFirstPage} offsetIndex={offsetIndex}");
            //Debug.LogWarning($"@#@ -- curSelectIndex ={ curSelectIndex} maxIndex={maxIndex}");
            //Debug.LogWarning($"@ firstColIndex {GetItemViewsHolder(0).ItemIndex}  lastColIndex {GetItemViewsHolder(_VisibleItems.Count -1).ItemIndex}  allColCount {_VisibleItems.Count} ");

            if (!isFirstPage && curSelectIndex <= offsetIndex)
            {
                MachineMovePage(-1);
                offsetIndex = -1;
                maxIndex = 0;
            }
            else
                ChangeCurSelect();
        }

        public void OnChangeGameListShowMode()
        {
            GetCurShowGameType();
            if (curShowGameType == GameFilter.UNKNOWN)
            {
                //Refresh();


                var newModels = new List<LobbySlotsModel>();
                var slotInfoDict = new Dictionary<int, Blackboard>();
                var gameInfoDict = GetGameInfoDict();
                var bonusIAMBBDict = BlackboardQueryUtils.GetBonusIAMDict();
                ClearVisibleItems();
                CreateSortSlotList(newModels, slotInfoDict, gameInfoDict, bonusIAMBBDict);
                ResetCurSelect();
            }
            else
            {
                var newModels = new List<LobbySlotsModel>();
                var slotInfoDict = new Dictionary<int, Blackboard>();
                var gameInfoDict = GetGameInfoDict();
                var bonusIAMBBDict = BlackboardQueryUtils.GetBonusIAMDict();
                ClearVisibleItems();
                CreateSortSlotList(newModels, slotInfoDict, gameInfoDict, bonusIAMBBDict);
                ResetCurSelect();
            }
        }

        private void ResetCurSelect()
        {
            curSelectIndex = 0;
            ChangeCurSelect();
        }

        private new void Awake()
        {
            isShowLobbyBackButton = BlackboardUtils.GetOrCreateVariable<bool>("/isShowLobbyBackButton");
            isShowLobbyBackButton.value = false;

            task = () =>
            {
                MachineSelectManager.Instance.ReflashHallBtnRegion();
            };
        }

        System.Action task;

        private void Update()
        {
            if (task != null)
            {
                task();
                task = null;
            }
        }



        private void OnEnable()
        {
            isInteractable = PopupManager.Instance.popupCount == 0;
            ScrollPositionChanged += OnChangeScrollPosition;
        }

        private void OnDisable()
        {
            ScrollPositionChanged -= OnChangeScrollPosition;
        }



        public void OnChangeMetaPopupCount()
        {
            isInteractable = PopupManager.Instance.popupCount == 0;

            if (isInteractable == false)
            {
                StopMovement();
            }
        }

        private void OnChangeScrollPosition(double delta)
        {
            if (isShowLobbyBackButton != null)
            {
                // if(showBackButtonTargetDelta > 0.0 && showBackButtonTargetDelta <= delta)
                if (delta > 0.0 && delta > showBackButtonTargetDelta)
                {
                    isShowLobbyBackButton.value = true;
                }
                else
                {
                    isShowLobbyBackButton.value = false;
                }
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (isInteractable)
            {
                base.OnDrag(eventData);
            }
        }

        protected override void Start()
        {
            FixPlatformScaler();

            base.Start();
            var newModels = new List<LobbySlotsModel>();
            var slotInfoDict = new Dictionary<int, Blackboard>();
            var gameInfoDict = GetGameInfoDict();
            var bonusIAMBBDict = BlackboardQueryUtils.GetBonusIAMDict();
            ClearVisibleItems();
            CreateSortSlotList(newModels, slotInfoDict, gameInfoDict, bonusIAMBBDict);
            ResetCurSelect();
            //CreateSlotList();
        }

        protected override LobbySlotsItem CreateViewsHolder(int itemIndex)
        {
            LobbySlotsItem item = null;
            LobbySlotsItemType itemType = _Params.data[itemIndex].itemType;

            switch (itemType)
            {
                case LobbySlotsItemType.Banner_Portrait:
                    item = new LobbySlotsItem_BannerPortrait();
                    break;
                // case LobbySlotsItemType.Banner_LSS:
                // case LobbySlotsItemType.Banner_SSL:
                // case LobbySlotsItemType.Banner_LL:
                case LobbySlotsItemType.FavoriteDouble:
                    item = new LobbySlotsItem_FavoriteDouble();
                    break;
                case LobbySlotsItemType.SlotSingle:
                    item = new LobbySlotsItem_SlotSingle();
                    break;
                case LobbySlotsItemType.SlotDouble:
                    item = new LobbySlotsItem_SlotDouble();
                    break;
            }

            if (item != null)
                item.Init(FindPrefab((int)itemType, itemIndex), itemIndex);

            return item;
        }

        protected override void UpdateViewsHolder(LobbySlotsItem newOrRecycled)
        {
            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateViews(model);
        }

        protected override bool IsRecyclable(LobbySlotsItem potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return potentiallyRecyclable.CanPresentModelType(_Params.data[indexOfItemThatWillBecomeVisible].itemType);
        }

        protected override bool ShouldDestroyRecyclableItem(LobbySlotsItem inRecycleBin, bool isInExcess)
        {
            return inRecycleBin.ShouldDestroyRecyclableItem();
        }

        void CreateSlotList()
        {
            var newModels = new List<LobbySlotsModel>();

            var slotInfoDict = new Dictionary<int, Blackboard>();
            var gameInfoDict = GetGameInfoDict();
            var bonusIAMBBDict = BlackboardQueryUtils.GetBonusIAMDict();

            int bannerCount = CreateSlotBanner(newModels);
            CreateSlotList(newModels, slotInfoDict, gameInfoDict, bonusIAMBBDict);
            InsertFavorites(newModels, slotInfoDict, gameInfoDict, bannerCount);

            _Params.data.Clear();
            _Params.data.AddRange(newModels);
            ResetItems(newModels.Count);

            // Set show back button target delta position
            if (showBackButtonTargetSlotIndex < _Params.data.Count)
            {
                double contentsSize = (double)GetContentSize();
                contentsSize -= GetViewportSize();

                double targetWidth = (double)showBackButtonTargetSlotIndex * (double)BaseParameters.DefaultItemSize;
                targetWidth += (double)((showBackButtonTargetSlotIndex - 1) * (double)BaseParameters.contentSpacing);
                targetWidth += (double)BaseParameters.contentPadding.left;

                // page over enable
                // showBackButtonTargetDelta = targetWidth/contentsSize;

                // android hotfix back button LobbyHomeScreenButtonController;
                showBackButtonTargetDelta = 0.0;
            }
        }


        /// <summary>刷新大厅游戏选择框</summary>
        private void ChangeCurSelect()
        {
            //Debug.LogError($"【test】 curSelectIndex = {curSelectIndex}");
            int index = 0;
            for (int i = 0; i < _VisibleItems.Count; i++)
            {
                LobbySlotsItem visibleItem = _VisibleItems[i];
                switch (_Params.data[visibleItem.ItemIndex].itemType)
                {
                    case LobbySlotsItemType.FavoriteDouble:
                        var element = visibleItem.root.GetComponent<ContextElement>();
                        element.UpdateContext(false);
                        for (int j = 0; j < 2; j++)
                        {
                            var child = ContextUtils.FindElement(element, string.Format("Slot Short {0}", (j + 1)), ContextSearchingType.ChildrenSearch);
                            var controller = child.GetComponent<LobbySlotFavoriteController>();
                            controller.Select(index == curSelectIndex);
                            index++;
                        }
                        break;
                    case LobbySlotsItemType.SlotSingle: // 大单个
                        var controller1 = visibleItem.root.GetComponent<LobbySlotController>();
                        controller1.Select(index == curSelectIndex);
                        index++;
                        break;
                    case LobbySlotsItemType.SlotDouble: // 小个
                        for (int j = 0; j < 2; ++j)
                        {
                            var child = visibleItem.root.GetChild(j);
                            var controller = child.GetComponent<LobbySlotController>();
                            controller.Select(index == curSelectIndex);
                            index++;
                        }
                        break;
                    default:
                        break;
                }
            }
            //Debug.LogWarning($"@#@ ++ curSelectIndex ={curSelectIndex} maxIndex={maxIndex}  index = {index}");
            /*if (maxIndex == 0)
                maxIndex = index;*/
            maxIndex = index;
        }


        public void SetCurSelect(int index)
        {
            curSelectIndex = index;
            ChangeCurSelect();
        }


        int CreateSlotBanner(List<LobbySlotsModel> newModels)
        {
            int bannerCount = 0;
            var slotBannerGroupList = MainBlackboard.Get().GetValue<List<Blackboard>>("slotBannerGroupList");
            for (int i = 0; i < slotBannerGroupList.Count; ++i)
            {
                var slotBannerGroup = slotBannerGroupList[i];
                var noticeList = BlackboardQueryUtils.GetValidSlotBannerNoticeList(slotBannerGroup);
                if (noticeList.Count == 0)
                    continue;

                var model = new LobbySlotsModel_Banner();
                model.noticeList = noticeList;

                var slotBannerGroupType = slotBannerGroup.GetValue<SlotBannerGroupType>("slotBannerGroupType");
                switch (slotBannerGroupType)
                {
                    case SlotBannerGroupType.PORTRAIT:
                        model.itemType = LobbySlotsItemType.Banner_Portrait;
                        newModels.Add(model);
                        ++bannerCount;
                        break;
                        // case SlotBannerGroupType.LANDSCAPE_LANDSCAPE:
                        // case SlotBannerGroupType.LANDSCAPE_SHORT_SHORT:
                        // case SlotBannerGroupType.SHORT_SHORT_LANDSCAPE:
                }
            }
            return bannerCount;
        }

        void CreateSlotList(
            List<LobbySlotsModel> newModels,
            Dictionary<int, Blackboard> slotInfoDict,
            Dictionary<int, Blackboard> gameInfoDict,
            Dictionary<int, Blackboard> bonusIAMBBDict
        )
        {
            var slotInfoList = MainBlackboard.Get().GetValue<List<Blackboard>>("slotList");
            int slotCount = slotInfoList.Count;
            for (int i = 0; i < slotCount; ++i)
            {
                Blackboard[] slotInfos = new Blackboard[2];
                slotInfos[0] = slotInfoList[i];
                slotInfos[1] = (i < (slotCount - 1)) ? slotInfoList[i + 1] : null;

                bool isLong1 = BlackboardUtils.FindValue<bool>(slotInfos[0], "flags/isLong");
                if (isLong1)
                {
                    var model = new LobbySlotsModel_SlotSingle();
                    model.itemType = LobbySlotsItemType.SlotSingle;
                    model.slotInfo = slotInfos[0];
                    int gameId = slotInfos[0].GetValue<int>("gameId");
                    if (!gameInfoDict.ContainsKey(gameId))
                        continue;
                    model.gameInfo = gameInfoDict[gameId];

                    model.bonusIAMBB = bonusIAMBBDict.ContainsKey(gameId) ? bonusIAMBBDict[gameId] : null;
                    BlackboardUtils.SetOrCreateValue<bool>(slotInfos[0], "isViewLong", true);

                    slotInfoDict[gameId] = slotInfos[0];
                    newModels.Add(model);
                }
                else
                {
                    var model = new LobbySlotsModel_SlotDouble();
                    model.itemType = LobbySlotsItemType.SlotDouble;

                    bool isLong2 = (slotInfos[1] != null) ? BlackboardUtils.FindValue<bool>(slotInfos[1], "flags/isLong") : true;
                    bool initFinish = false;
                    if (isLong2)
                    {
                        model.slotInfos[0] = slotInfos[0];
                        int gameId = slotInfos[0].GetValue<int>("gameId");
                        if (!gameInfoDict.ContainsKey(gameId))
                            continue;
                        model.gameInfos[0] = gameInfoDict[gameId];
                        model.bonusIAMBBs[0] = bonusIAMBBDict.ContainsKey(gameId) ? bonusIAMBBDict[gameId] : null;
                        BlackboardUtils.SetOrCreateValue<bool>(slotInfos[0], "isViewLong", false);

                        initFinish = true;
                        slotInfoDict[gameId] = slotInfos[0];
                    }
                    else
                    {
                        for (int j = 0; j < 2; ++j)
                        {
                            model.slotInfos[j] = slotInfos[j];
                            int gameId = slotInfos[j].GetValue<int>("gameId");
                            if (!gameInfoDict.ContainsKey(gameId))
                                continue;
                            model.gameInfos[j] = gameInfoDict[gameId];
                            model.bonusIAMBBs[j] = bonusIAMBBDict.ContainsKey(gameId) ? bonusIAMBBDict[gameId] : null;
                            BlackboardUtils.SetOrCreateValue<bool>(slotInfos[j], "isViewLong", false);

                            initFinish = true;
                            slotInfoDict[gameId] = slotInfos[j];
                        }
                        ++i;
                    }
                    if (initFinish)
                        newModels.Add(model);
                }
            }
        }

        /// <summary>配置大厅游戏列表排版</summary>
        void CreateSortSlotList(List<LobbySlotsModel> newModels,
            Dictionary<int, Blackboard> slotInfoDict,
            Dictionary<int, Blackboard> gameInfoDict,
            Dictionary<int, Blackboard> bonusIAMBBDict)
        {
            var slotInfoList = MainBlackboard.Get().GetValue<List<Blackboard>>("slotList");
            List<Blackboard> tempSlotInfoList = new List<Blackboard>();
            int slotCount = slotInfoList.Count;
            int gameId = 0;
            for (int i = 0; i < slotCount; ++i)
            {
                gameId = slotInfoList[i].GetValue<int>("gameId");
                if (!gameInfoDict.ContainsKey(gameId))
                    continue;
                tempSlotInfoList.Add(slotInfoList[i]);
            }
            slotCount = tempSlotInfoList.Count;
            hallScrollviewItemCount = tempSlotInfoList.Count;
            int startIndex = slotCount % 2 > 0 ? 1 : 0;
            if (slotCount % 2 > 0) // 有单个
            {
                var model = new LobbySlotsModel_SlotSingle();
                model.itemType = LobbySlotsItemType.SlotSingle;
                model.slotInfo = tempSlotInfoList[0];
                gameId = tempSlotInfoList[0].GetValue<int>("gameId");
                model.gameInfo = gameInfoDict[gameId];

                model.bonusIAMBB = bonusIAMBBDict.ContainsKey(gameId) ? bonusIAMBBDict[gameId] : null;
                BlackboardUtils.SetOrCreateValue<bool>(tempSlotInfoList[0], "isViewLong", true);

                slotInfoDict[gameId] = tempSlotInfoList[0];
                newModels.Add(model);
            }

            for (int i = startIndex; i < slotCount; ++i)
            {
                var modelDouble = new LobbySlotsModel_SlotDouble();
                modelDouble.itemType = LobbySlotsItemType.SlotDouble;
                Blackboard[] slotInfos = new Blackboard[2];
                slotInfos[0] = tempSlotInfoList[i];
                slotInfos[1] = (i < (slotCount - 1)) ? tempSlotInfoList[i + 1] : null;
                for (int j = 0; j < 2; ++j)
                {
                    modelDouble.slotInfos[j] = slotInfos[j];
                    gameId = slotInfos[j].GetValue<int>("gameId");
                    modelDouble.gameInfos[j] = gameInfoDict[gameId];
                    modelDouble.bonusIAMBBs[j] = bonusIAMBBDict.ContainsKey(gameId) ? bonusIAMBBDict[gameId] : null;
                    BlackboardUtils.SetOrCreateValue<bool>(slotInfos[j], "isViewLong", false);

                    slotInfoDict[gameId] = slotInfos[j];
                }
                ++i;
                newModels.Add(modelDouble);
            }

            _Params.data.Clear();
            _Params.data.AddRange(newModels);
            Debug.Log($"newModels.Count = {newModels.Count}");
            ResetItems(newModels.Count);
        }

        void InsertFavorites(
            List<LobbySlotsModel> newModels,
            Dictionary<int, Blackboard> slotInfoDict,
            Dictionary<int, Blackboard> gameInfoDict,
            int bannerCount
        )
        {
            int favoriteItemIndex = MainBlackboard.Get().GetValue<int>("favoriteSlotUiIndex");
            if (favoriteItemIndex >= 0)
            {
                var model = new LobbySlotsModel_FavoritesDouble();
                model.itemType = LobbySlotsItemType.FavoriteDouble;

                int found = 0;
                var favoriteList = MainBlackboard.Get().GetValue<List<int>>("favoriteSlotIdList");
                int count = favoriteList.Count;
                for (int i = 0; i < count; ++i)
                {
                    int gameId = favoriteList[i];

                    if (slotInfoDict.ContainsKey(gameId))
                    {
                        var slotInfo = slotInfoDict[gameId];
                        var flags = slotInfo.GetValue<Blackboard>("flags");
                        int status = flags.GetValue<int>("status");
                        // int tag = flags.GetValue<int>("tag");
                        if (status == 0)// && tag == 0)
                        {
                            model.slotInfos[found] = slotInfo;
                            model.gameInfos[found] = gameInfoDict[gameId];
                            if (++found == 2)
                            {
                                newModels.Insert(bannerCount + favoriteItemIndex, model);
                                break;
                            }
                        }
                    }
                }
            }
        }

        Dictionary<int, Blackboard> GetGameInfoDict()
        {
            var gameInfoList = MainBlackboard.Get().GetValue<List<Blackboard>>("gameInfoList");
            var result = new Dictionary<int, Blackboard>();
            int count = gameInfoList.Count;

            List<string> gameTitles = new List<string>();

            for (int i = 0; i < count; ++i)
            {
                gameTitles.Add(gameInfoList[i].GetValue<string>("gameTitle"));
                if (curShowGameType == GameFilter.UNKNOWN)
                {
                    if (!result.ContainsKey(gameInfoList[i].GetValue<int>("gameId")))
                        result[gameInfoList[i].GetValue<int>("gameId")] = gameInfoList[i];
                }
                else
                    if (!result.ContainsKey(gameInfoList[i].GetValue<int>("gameId")) && gameInfoList[i].GetValue<GameFilter>("gameFilter") == curShowGameType)
                    result[gameInfoList[i].GetValue<int>("gameId")] = gameInfoList[i];
            }
            ////生成SelectGames的json文件
            //string jsonStr = SlotSimpleJson.SerializeObject(gameTitles);
            //string jsonPath = Path.Combine(Application.streamingAssetsPath, "SelectGames");
            //if (Directory.Exists(jsonPath))
            //    Directory.Delete(jsonPath, true);
            //Directory.CreateDirectory(jsonPath);
            //jsonPath = Path.Combine(jsonPath, "AllGames.txt");
            //FileStream fileStream = new FileStream(jsonPath, FileMode.OpenOrCreate);
            //StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8, jsonStr.Length);
            //writer.WriteLine(jsonStr);
            //writer.Close();

            return result;
        }

        void GetCurShowGameType()
        {
            curShowGameType = MainBlackboard.Get().GetValue<GameFilter>("curShowGameType");
        }

        //        Dictionary<int, Blackboard> GetBuyABonusIAMDict()
        //        {
        //            var iamInfoList = BlackboardQueryUtils.GetIAMListFromType(InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP);
        //            var result = new Dictionary<int, Blackboard>();
        //            int count = iamInfoList.Count;
        //            for (int i = 0; i < count; ++i)
        //            {
        //                if(!result.ContainsKey(iamInfoList[i].GetValue<int>("gameId")))
        //                    result.Add(iamInfoList[i].GetValue<int>("gameId"), iamInfoList[i]);
        //            }
        //            return result;
        //        }

        GameObject FindPrefab(int id, int itemIndex)
        {
            if (_Params.prefabs[id] == null && _Params.sceneInfos[id] != null)
            {
                var go = SceneManager.LoadScene(transform, _Params.sceneInfos[id].GetSceneInfo());
                go.SetActive(false);
                _Params.prefabs[id] = go;
            }
            return _Params.prefabs[id];
        }

        void FixPlatformScaler()
        {
            var rectTransform = GetComponent<RectTransform>();
            float xScale = rectTransform.localScale.x;
            if (xScale != 0f && !Mathf.Approximately(xScale, 1f))
            {
                float screenWidth = rectTransform.root.GetComponent<RectTransform>().sizeDelta.x;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, screenWidth * (1f / xScale - 1f));
            }
        }

        public void ScrollToNextPage()
        {
            MovePage(1);
        }

        public void ScrollToPrevPage()
        {
            MovePage(-1);
        }

        public void ScrollToFirstPage()
        {
            SmoothScrollTo(0, 0.3f, 0.5f);

            Analytics.CustomEvent("client_click_lobby_reset", null);
        }

        void MovePage(int pageCount)
        {
            float offset = 0.075f;
            int itemIndex = GetItemViewsHolder(0).ItemIndex + pageCount * (VisibleItemsCount - 1);
            if (itemIndex < 0)
                offset = 0.5f;
            itemIndex = Mathf.Clamp(itemIndex, 0, _Params.data.Count - 1);
            SmoothScrollTo(itemIndex, 0.3f, offset);
        }


        /* 旧版本
        void MachineMovePage(int pageCount)
        {
            float offset = 0.02f;
            int temp = initMod ? VisibleItemsCount : VisibleItemsCount - 1;
            if (pageCount < 0 && offsetIndex > 0)
                temp -= 2;
            initMod = false;
            int itemIndex = GetItemViewsHolder(0).ItemIndex + pageCount * temp;
            itemIndex = Mathf.Clamp(itemIndex, 0, _Params.data.Count);
            isMovingPage = true;
            SmoothScrollTo(itemIndex, 0.3f, offset, 0, (progress) =>
            {
                if (progress == 1)
                {
                    curSelectIndex = pageCount > 0 ? 2 : GetPageTrulyItemCount();
                    ChangeCurSelect();
                    curPage += pageCount;
                    isMovingPage = false;
                }
                return true;
            });
        }
        */

        void MachineMovePage(int pageCount)
        {
            float offset = 0.02f;
            int temp = initMod ? VisibleItemsCount : VisibleItemsCount - 1; //要跳过的列数
            if (pageCount < 0 && offsetIndex > 0)
                temp -= 2;

            initMod = false;
            int itemIndex = GetItemViewsHolder(0).ItemIndex + pageCount * temp;
            itemIndex = Mathf.Clamp(itemIndex, 0, _Params.data.Count);
            isMovingPage = true;

            int lastEndIdx = GetItemViewsHolder(_VisibleItems.Count - 1).ItemIndex;
            int lastStartIdx = GetItemViewsHolder(0).ItemIndex;

            Debug.LogWarning($"@#@ curSelectIndex = {curSelectIndex}");
            SmoothScrollTo(itemIndex, 0.3f, offset, 0, (progress) =>
            {
                if (progress == 1)
                {
                    int endIdx = GetItemViewsHolder(_VisibleItems.Count - 1).ItemIndex;
                    int startIdx = GetItemViewsHolder(0).ItemIndex;

                    if (pageCount > 0)
                    {
                        curSelectIndex = (lastEndIdx - startIdx) * 2;
                        //Debug.LogWarning($"@#@ +++ lastStartIdx {lastStartIdx}  lastEndIdx {lastEndIdx}  move {itemIndex} Start {startIdx}  End {endIdx} count ={GetPageVisibleIconCount()}");
                    }
                    else
                    {
                        if (endIdx == lastStartIdx)
                        {
                            curSelectIndex = GetPageVisibleIconCount() - 2 - 1;
                        }
                        else
                        {
                            curSelectIndex = GetPageVisibleIconCount() - (endIdx - lastStartIdx) * 2 - 1;
                        }
                        //Debug.LogWarning($"@#@ --- lastStartIdx {lastStartIdx}  lastEndIdx {lastEndIdx}  move {itemIndex} Start {startIdx}  End {endIdx} count ={GetPageVisibleIconCount()}  - {1 + (endIdx - lastStartIdx) * 2}  {offsetIndex}");
                    }
                    //curSelectIndex = pageCount > 0 ? 2 : GetPageTrulyItemCount();
                    ChangeCurSelect();
                    //curPage += pageCount;
                    isMovingPage = false;
                }
                return true;
            });
        }


        private int GetPageVisibleIconCount()
        {
            int count = 0;
            for (int i = 0; i < _VisibleItems.Count; i++)
            {
                LobbySlotsItem visibleItem = _VisibleItems[i];
                switch (_Params.data[visibleItem.ItemIndex].itemType)
                {
                    case LobbySlotsItemType.FavoriteDouble:
                        count += 2;
                        break;
                    case LobbySlotsItemType.SlotSingle:
                        count++;
                        break;
                    case LobbySlotsItemType.SlotDouble:
                        count += 2;
                        break;
                    default:
                        break;
                }
            }
            return count;
        }



        private int GetPageTrulyItemCount()
        {
            int count = 0;
            for (int i = 0; i < _VisibleItems.Count; i++)
            {
                LobbySlotsItem visibleItem = _VisibleItems[i];
                switch (_Params.data[visibleItem.ItemIndex].itemType)
                {
                    case LobbySlotsItemType.FavoriteDouble:
                        count += 2;
                        break;
                    case LobbySlotsItemType.SlotSingle:
                        count++;
                        break;
                    case LobbySlotsItemType.SlotDouble:
                        count += 2;
                        break;
                    default:
                        break;
                }
            }
            return count - 3;
        }
    }

    public enum LobbySlotsItemType
    {
        Banner_Portrait = 0,
        Banner_LSS, // Landscape, Short, Short
        Banner_SSL, // Short, Short, Landscape
        Banner_LL, // Landscape, Landscape
        FavoriteDouble,
        SlotSingle,
        SlotDouble
    }

    [Serializable]
    public class LobbySlotsParams : BaseParams
    {
        public SceneInfoObject[] sceneInfos;
        public GameObject[] prefabs;

        public List<LobbySlotsModel> data = new List<LobbySlotsModel>();
    }

    [Serializable]
    public class LobbySlotsModel
    {
        public LobbySlotsItemType itemType;
    }

    [Serializable]
    public class LobbySlotsModel_Banner : LobbySlotsModel
    {
        public List<Blackboard> noticeList;
    }

    [Serializable]
    public class LobbySlotsModel_FavoritesDouble : LobbySlotsModel_SlotDouble { }

    [Serializable]
    public class LobbySlotsModel_SlotSingle : LobbySlotsModel
    {
        public Blackboard slotInfo;
        public Blackboard gameInfo;
        public Blackboard bonusIAMBB;
    }

    [Serializable]
    public class LobbySlotsModel_SlotDouble : LobbySlotsModel
    {
        public Blackboard[] slotInfos = new Blackboard[2];
        public Blackboard[] gameInfos = new Blackboard[2];
        public Blackboard[] bonusIAMBBs = new Blackboard[2];
    }

    public abstract class LobbySlotsItem : BaseItemViewsHolder
    {
        public abstract bool CanPresentModelType(LobbySlotsItemType itemType);
        public virtual bool ShouldDestroyRecyclableItem() { return false; }
        public abstract void UpdateViews(LobbySlotsModel model);
    }

    public class LobbySlotsItem_BannerPortrait : LobbySlotsItem
    {
        public override bool CanPresentModelType(LobbySlotsItemType itemType) { return itemType == LobbySlotsItemType.Banner_Portrait; }
        public override bool ShouldDestroyRecyclableItem() { return true; }

        public override void UpdateViews(LobbySlotsModel model)
        {
            var bannerModel = model as LobbySlotsModel_Banner;

            // var bb = root.GetComponent<Blackboard>();
            // if (!bb.GetValue<bool>("initialized"))
            // {
            //     bb.SetValue("noticeList", bannerModel.noticeList);
            //     root.GetComponent<GraphOwner>().StartBehaviour();
            // }
            var controller = root.GetComponent<LobbyBannerGroupController>();
            if (!controller.initialized)
                controller.UpdateBannerInfo(bannerModel.noticeList);
        }
    }

    public class LobbySlotsItem_FavoriteDouble : LobbySlotsItem
    {
        public override bool CanPresentModelType(LobbySlotsItemType itemType) { return itemType == LobbySlotsItemType.FavoriteDouble; }

        public override void UpdateViews(LobbySlotsModel model)
        {
            var favoritesModel = model as LobbySlotsModel_FavoritesDouble;

            var element = root.GetComponent<ContextElement>();
            element.UpdateContext(false);
            // var children = new ContextElement[2];
            for (int i = 0; i < 2; ++i)
            {
                var child = ContextUtils.FindElement(element, string.Format("Slot Short {0}", (i + 1)), ContextSearchingType.ChildrenSearch);

                var controller = child.GetComponent<LobbySlotFavoriteController>();
                controller.UpdateSlotInfo(favoritesModel.slotInfos[i], favoritesModel.gameInfos[i]);
                // var bb = child.GetComponent<Blackboard>();
                // bb.SetValue("slotInfo", favoritesModel.slotInfos[i]);
                // bb.SetValue("gameInfo", favoritesModel.gameInfos[i]);
                // bb.SetValue("bonusIAMBB", favoritesModel.bonusIAMBBs[i]);

                // child.GetComponent<GraphOwner>().StopBehaviour();
                // child.GetComponent<GraphOwner>().StartBehaviour();
            }
        }
    }

    public class LobbySlotsItem_SlotSingle : LobbySlotsItem
    {
        public override bool CanPresentModelType(LobbySlotsItemType itemType) { return itemType == LobbySlotsItemType.SlotSingle; }

        public override void UpdateViews(LobbySlotsModel model)
        {
            var singleModel = model as LobbySlotsModel_SlotSingle;

            root.gameObject.SetActive(true);

            var controller = root.GetComponent<LobbySlotController>();
            controller.UpdateSlotInfo(singleModel.slotInfo, singleModel.gameInfo, singleModel.bonusIAMBB);
        }
    }

    public class LobbySlotsItem_SlotDouble : LobbySlotsItem
    {
        public override bool CanPresentModelType(LobbySlotsItemType itemType) { return itemType == LobbySlotsItemType.SlotDouble; }

        public override void UpdateViews(LobbySlotsModel model)
        {
            var doubleModel = model as LobbySlotsModel_SlotDouble;

            root.gameObject.SetActive(true);
            for (int i = 0; i < 2; ++i)
            {
                var child = root.GetChild(i);
                var controller = child.GetComponent<LobbySlotController>();
                controller.UpdateSlotInfo(doubleModel.slotInfos[i], doubleModel.gameInfos[i], doubleModel.bonusIAMBBs[i]);
            }
        }
    }
}
