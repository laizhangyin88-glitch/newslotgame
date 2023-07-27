using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Com.TheFallenGames.OSA.Core;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using BagelCode.ClubArena;
using NodeCanvas.Framework;

namespace BagelCode.OSA_Scroll
{
    public class OSA_ClubArenaRevengeController : OSA<ClubArenaRevengeItemParams, ClubArenaRevengeItemViewHolder>
    {
        public GameObject caller;

        protected override void Start()
        {
            base.Start();
            CreateItemList();
        }

        public void CreateItemList()
        {
            List<Blackboard> revengeList = ClubArenaUtils.RevengeList;
            if (revengeList == null || revengeList.Count == 0) return;

            _Params.data.Clear();
            _Params.data.AddRange(revengeList);
            ResetItems(revengeList.Count);

            if (revengeList.Count > 5)
                ScrollTo(0);
        }

        public void OnRefresh()
        {
            ClearVisibleItems();
            List<Blackboard> revengeList = ClubArenaUtils.RevengeList;

            if (revengeList == null || revengeList.Count == 0) return;

            _Params.data.Clear();
            _Params.data.AddRange(revengeList);
            ResetItems(revengeList.Count);

            MoveToIndex(0);
        }

        protected override ClubArenaRevengeItemViewHolder CreateViewsHolder(int itemIndex)
        {
            ClubArenaRevengeItemViewHolder viewHolder = new ClubArenaRevengeItemViewHolder();
            viewHolder.Init(_Params.GetPrefab(transform), itemIndex);
            return viewHolder;
        }

        protected override void OnItemHeightChangedPreTwinPass(ClubArenaRevengeItemViewHolder viewsHolder)
        {
            base.OnItemHeightChangedPreTwinPass(viewsHolder);
            viewsHolder.ContentSizeFitter.enabled = false;
        }

        protected override void UpdateViewsHolder(ClubArenaRevengeItemViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex], newOrRecycled.ItemIndex, caller);

            if (newOrRecycled.ContentSizeFitter.enabled)
                newOrRecycled.ContentSizeFitter.enabled = false;

            {
                newOrRecycled.MarkForRebuild();
                ScheduleComputeVisibilityTwinPass(true);
            }
        }

        protected override void OnBeforeRecycleOrDisableViewsHolder(ClubArenaRevengeItemViewHolder inRecycleBinOrVisible, int newItemIndex)
        {
            base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }

        protected override bool IsRecyclable(ClubArenaRevengeItemViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return true;
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
    }

    [Serializable]
    public class ClubArenaRevengeItemParams : BaseParams
    {
        public List<Blackboard> data = new List<Blackboard>();
        public GameObject prefab = null;

        public GameObject GetPrefab(Transform transform)
        {
            if (prefab == null)
            {
                prefab = MetaObjectUtils.MakePrefab("mgclubarenacontents", "Revenge List Cell", transform);
                prefab.SetActive(false);
            }

            return prefab;
        }
    }

    [Serializable]
    public class ClubArenaRevengeItemViewHolder : BaseItemViewsHolder
    {
        private ClubArenaRevengeItemsController controller;

        public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(Blackboard rewardsBB, int index, GameObject caller)
        {
            controller.UpdateVariables(rewardsBB, index, caller);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<ClubArenaRevengeItemsController>();

            ContentSizeFitter = root.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            ContentSizeFitter.enabled = false;
        }

        public override void MarkForRebuild()
        {
            base.MarkForRebuild();
            if (ContentSizeFitter)
                ContentSizeFitter.enabled = true;
        }
    }
}