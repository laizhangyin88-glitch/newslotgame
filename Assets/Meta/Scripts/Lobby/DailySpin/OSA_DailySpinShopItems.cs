using System;
using System.Collections.Generic;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;
using System.Linq;

namespace BagelCode
{
    public class OSA_DailySpinShopItems : OSA<DailySpinShopItemParams, DailySpinShopItemViewHolder>
    {
        private GameObject caller;

        public void CreateItemList(GameObject _caller, ContextElement shopElement, List<Blackboard> productGroupList, bool fromLogin, string contextId)
        {
            if (productGroupList.Count <= 0) return;

            caller = _caller;

            _Params.data.Clear();
            _Params.data.AddRange(productGroupList);
            _Params.contextId = contextId;
            _Params.fromLogin = fromLogin;

            if (productGroupList.Count <= 3)
            {
                _Params.SetScrollEnabled(false);

                var rectElement = ContextUtils.FindElement(shopElement, "Scroll Rect", ContextSearchingType.FullNameSearch);
                var rectRect = rectElement.GetComponent<RectTransform>();
                var pos = rectRect.anchoredPosition;
                pos.x += 30;
                rectRect.anchoredPosition = pos;

                var scrollBarElement = ContextUtils.FindElement(shopElement, "Scroll Bar Area", ContextSearchingType.FullNameSearch);
                scrollBarElement.gameObject.SetActive(false);

                ResetItems(productGroupList.Count);
                ScrollTo(productGroupList.Count / 2, 0.5f, 0.5f);
            }
            else
            {
                ResetItems(productGroupList.Count);

                int index = MainBlackboard.Get().GetVariable<int>("dailySpinPricePointsIndex")?.value ?? 0;
                ScrollTo(index, 0f, 0f);
            }
        }

        public void ScrollToRight()
        {
            MoveTo(1);
        }

        public void ScrollToLeft()
        {
            MoveTo(-1);
        }

        void MoveTo(int index)
        {
            int itemIndex = GetItemViewsHolder(0).ItemIndex + index;
            itemIndex = Mathf.Clamp(itemIndex, 0, _Params.data.Count - 1);
            SmoothScrollTo(itemIndex, 0.3f, 0f, 0f);
        }

        protected override DailySpinShopItemViewHolder CreateViewsHolder(int itemIndex)
        {
            var viewHolder = new DailySpinShopItemViewHolder();
            viewHolder.Init(_Params.prefab, itemIndex);

            return viewHolder;
        }

        protected override void UpdateViewsHolder(DailySpinShopItemViewHolder newOrRecycled)
        {
            int index = newOrRecycled.ItemIndex;
            newOrRecycled.UpdateView(caller, _Params.data[index], _Params.contextId, _Params.fromLogin, index);
        }

        protected override bool IsRecyclable(DailySpinShopItemViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return true;
        }
    }

    [Serializable]
    public class DailySpinShopItemParams : BaseParams
    {
        public List<Blackboard> data = new List<Blackboard>();
        public GameObject prefab = null;
        public string contextId = "";
        public bool fromLogin = false;

        public void SetScrollEnabled(bool isEnable)
        {
            ScrollEnabled = isEnable;
            DragEnabled = isEnable;
        }
    }

    [Serializable]
    public class DailySpinShopItemViewHolder : BaseItemViewsHolder
    {
        public void UpdateView(GameObject caller, Blackboard productGroup, string contextId, bool fromLogin, int idx)
        {
            Blackboard cellBB = root.GetComponent<Blackboard>();
            List<Blackboard> productList = BlackboardQueryUtils.GetProductList(productGroup);

            cellBB.AddVariable("caller", caller);
            cellBB.AddVariable("productList", productList);
            cellBB.AddVariable("_biContextID", contextId);
            cellBB.AddVariable("cellIndex", idx);
            cellBB.AddVariable("itemType", ItemType.DAILY_BONUS_WHEEL);
            cellBB.AddVariable("fromLogin", fromLogin);

            var owner = root.GetComponent<GraphOwner>();
            owner.StopBehaviour();
            owner.StartBehaviour();

            root.gameObject.SetActive(true);
        }
    }
}