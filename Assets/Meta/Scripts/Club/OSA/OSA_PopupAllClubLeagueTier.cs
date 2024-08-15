using System;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.EventSystems;
using BagelCode;
using Com.ForbiddenByte.OSA.Core;

namespace BagelCode.OSA_Scroll
{
    public class OSA_PopupAllClubLeagueTier : OSA<PopupAllClubLeagueTierItemParams, PopupAllClubLeagueTierItemViewHolder>
    {
        private ContextElement caller;
        private int myLeagueTier;
        private int currentTier;

        public void CreateItemList(int maxLeagueTier)
        {
            ClearVisibleItems();
            List<int> clubLeagueTierList = new List<int>();
            for (int i = maxLeagueTier; i >= 0; --i)
                clubLeagueTierList.Add(i);

            _Params.data.Clear();
            _Params.data.AddRange(clubLeagueTierList);
            ResetItems(clubLeagueTierList.Count);

            InitScrollPosition(clubLeagueTierList);
        }

        public void InitScrollPosition(List<int> clubLeagueTierList)
        {
            int pageItemsCount = 8;
            if (clubLeagueTierList != null && clubLeagueTierList.Count > pageItemsCount)
            {
                int myIndex = 0;
                for (int i = 0; i < clubLeagueTierList.Count; ++i)
                {
                    if (clubLeagueTierList[i] == myLeagueTier)
                    {
                        myIndex = i;
                        break;
                    }
                }
                int halfPageItemsCount = pageItemsCount / 2;
                if (myIndex <= halfPageItemsCount)
                    MoveToIndex(0);
                else if (myIndex >= clubLeagueTierList.Count - halfPageItemsCount)
                    MoveToIndex(clubLeagueTierList.Count - pageItemsCount);
                else
                    MoveToIndex(myIndex - halfPageItemsCount);

            }
        }

        protected override PopupAllClubLeagueTierItemViewHolder CreateViewsHolder(int itemIndex)
        {
            PopupAllClubLeagueTierItemViewHolder viewHolder = new PopupAllClubLeagueTierItemViewHolder();
            viewHolder.Init(_Params.GetPrefab(transform), _Params.Content, itemIndex);
            return viewHolder;
        }

        protected override void UpdateViewsHolder(PopupAllClubLeagueTierItemViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(myLeagueTier, _Params.data[newOrRecycled.ItemIndex], currentTier, newOrRecycled.ItemIndex, caller);

            newOrRecycled.MarkForRebuild();
            ScheduleComputeVisibilityTwinPass(true);
        }

        public override void ResetItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            ClearVisibleItems();
            base.ResetItems(itemsCount, contentPanelEndEdgeStationary, keepVelocity);
        }

        public void MoveToIndex(int index, bool isSmooth = false)
        {
            StopMovement();
            if (index > GetItemsCount())
                index = GetItemsCount();

            ScrollTo(index);
        }

        public void SetCaller(ContextElement _caller)
        {
            caller = _caller;
        }

        public void SetMyLeagueTier(int _myLeagueTier)
        {
            myLeagueTier = _myLeagueTier;
        }

        public void SetSelectTier(int selectTier)
        {
            currentTier = selectTier;
        }

        public void SetTierCover(int selectTier)
        {
            SetSelectTier(selectTier);
            if (_VisibleItems != null && _VisibleItems.Count > 0)
            {
                for (int i = 0; i < _VisibleItems.Count; ++i)
                    _VisibleItems[i].SetCover(selectTier);
            }
        }

        public void RefreshLeagueTier(int refreshTier)
        {
            SetSelectTier(refreshTier);
            SetTierCover(refreshTier);
            InitScrollPosition(_Params.data);

            if (_VisibleItems != null && _VisibleItems.Count > 0)
            {
                for (int i = 0; i < _VisibleItems.Count; ++i)
                    _VisibleItems[i].RefreshMyClubTier(myLeagueTier);
            }
        }
    }

    [Serializable]
    public class PopupAllClubLeagueTierItemParams : BaseParams
    {
        public List<int> data = new List<int>();
        public GameObject prefab = null;

        public GameObject GetPrefab(Transform transform)
        {
            if (prefab == null)
            {
                prefab = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Club League Button", transform);
                prefab.SetActive(false);
            }

            return prefab;
        }
    }

    [Serializable]
    public class PopupAllClubLeagueTierItemViewHolder : BaseItemViewsHolder
    {
        private PopupAllClubLeagueTierItem controller;

        public void UpdateView(int myLeagueTier, int clubLeagueTier, int currentTier, int index, ContextElement caller)
        {
            controller.UpdateVariables(myLeagueTier, clubLeagueTier, currentTier, index, caller);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<PopupAllClubLeagueTierItem>();
        }

        public void SetCover(int selectTier)
        {
            controller.SetCover(selectTier);
        }

        public void RefreshMyClubTier(int myLeagueTier)
        {
            controller.SetMyClubTier(myLeagueTier);
        }
    }
}
