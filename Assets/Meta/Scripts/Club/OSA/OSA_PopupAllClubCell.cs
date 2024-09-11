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
    public class OSA_PopupAllClubCell : OSA<PopupAllClubCellItemParams, PopupAllClubCellItemViewHolder>
    {
        public Action<double> onEndDragEvent;

        private ContextElement caller;

        public void SubscribeEndDragEvent(Action<double> action)
        {
            if (action == null)
                return;
            onEndDragEvent += action;
        }

        public void UnSubscribeEndDragEvent(Action<double> action)
        {
            onEndDragEvent -= action;
        }

        public void CreateItemList(List<Blackboard> clubInfoList)
        {
            ClearVisibleItems();
            if (clubInfoList == null || clubInfoList.Count == 0) return;

            _Params.data.Clear();
            _Params.data.AddRange(clubInfoList);
            ResetItems(clubInfoList.Count);
        }

        public void RebuildLayout()
        {
            RebuildLayoutDueToScrollViewSizeChange();
        }

        protected override PopupAllClubCellItemViewHolder CreateViewsHolder(int itemIndex)
        {
            PopupAllClubCellItemViewHolder viewHolder = new PopupAllClubCellItemViewHolder();
            viewHolder.Init(_Params.GetPrefab(transform), _Params.Content, itemIndex);
            return viewHolder;
        }

        protected override void UpdateViewsHolder(PopupAllClubCellItemViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex], newOrRecycled.ItemIndex, caller);

            newOrRecycled.MarkForRebuild();
            ScheduleComputeVisibilityTwinPass(true);
        }

        public override void ResetItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            ClearVisibleItems();
            base.ResetItems(itemsCount, contentPanelEndEdgeStationary, keepVelocity);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (onEndDragEvent != null)
                onEndDragEvent(GetNormalizedPosition());
        }

        public override void OnScroll(PointerEventData eventData)
        {
            base.OnScroll(eventData);
            if (onEndDragEvent != null)
                onEndDragEvent(GetNormalizedPosition());
        }

        public void SetCaller(ContextElement _caller)
        {
            caller = _caller;
        }

        public List<Blackboard> GetDataBB()
        {
            List<Blackboard> infoListBB = new List<Blackboard>();
            for (int i = 0; i < VisibleItemsCount; ++i)
                infoListBB.Add(_VisibleItems[i].infoBB);
            return infoListBB;
        }

        public void InsertToFirstItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            int prevCount = GetItemsCount();

            base.RemoveItems(0, 1, contentPanelEndEdgeStationary, keepVelocity);
            base.InsertItems(0, itemsCount + 1, contentPanelEndEdgeStationary, keepVelocity);

            int scrollToIndex = GetItemsCount() - prevCount;
            if (scrollToIndex < 0) scrollToIndex = 0;

            StopMovement();
            ScrollTo(scrollToIndex);
        }

        public void InsertItem(int insertIndex, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            if (insertIndex < 0 || insertIndex > GetItemsCount()) insertIndex = GetItemsCount();

            base.InsertItems(insertIndex, 1, contentPanelEndEdgeStationary, keepVelocity);
        }

        public void ResetParams(List<Blackboard> newData = null)
        {
            if (newData == null)
                newData = new List<Blackboard>();
            _Params.data.Clear();
            _Params.data.AddRange(newData);
        }
    }
    [Serializable]
    public class PopupAllClubCellItemParams : BaseParams
    {
        public List<Blackboard> data = new List<Blackboard>();
        public GameObject prefab = null;

        public GameObject GetPrefab(Transform transform)
        {
            if (prefab == null)
            {
                prefab = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Club League Calculate Cell", transform);
                prefab.SetActive(false);
            }

            return prefab;
        }
    }

    [Serializable]
    public class PopupAllClubCellItemViewHolder : BaseItemViewsHolder
    {
        public Blackboard infoBB = null;
        private PopupAllClubCellItem controller;

        public void UpdateView(Blackboard clubInfoBB, int index, ContextElement caller)
        {
            infoBB = clubInfoBB;
            controller.UpdateVariables(clubInfoBB, caller);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<PopupAllClubCellItem>();
        }
    }
}
