using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Com.TheFallenGames.OSA.Core;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using BagelCode.EpicPass;
using NodeCanvas.Framework;

namespace BagelCode.OSA_Scroll
{
    public class OSA_PopupEpicPassV2Rewards : OSA<PopupEpicPassV2RewardItemParams, PopupEpicPassV2RewardItemViewHolder>
    {
        public bool isFree = false;

        private bool isLocked = false;

        private List<Blackboard> dataList = null;

        protected override void Start()
        {
            base.Start();
            CreateItemList();
        }

        public void OnInit()
        {
            CreateItemList();
        }

        public void InitData(List<Blackboard> _dataList, bool _isLocked = false)
        {
            dataList = _dataList;
            isLocked = _isLocked;
        }

        private void CreateItemList()
        {
            if (dataList == null || dataList.Count == 0) return;

            // Find first unclaimed item.
            _Params.data.Clear();
            _Params.data.AddRange(dataList);
            ResetItems(dataList.Count);

            ScrollTo(0);
        }

        protected override PopupEpicPassV2RewardItemViewHolder CreateViewsHolder(int itemIndex)
        {
            PopupEpicPassV2RewardItemViewHolder viewHolder = new PopupEpicPassV2RewardItemViewHolder();
            viewHolder.Init(_Params.GetPrefab(transform), itemIndex);

            return viewHolder;
        }

        protected override void OnItemWidthChangedPreTwinPass(PopupEpicPassV2RewardItemViewHolder viewsHolder)
        {
            base.OnItemWidthChangedPreTwinPass(viewsHolder);
            viewsHolder.ContentSizeFitter.enabled = false;
        }

        protected override void UpdateViewsHolder(PopupEpicPassV2RewardItemViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex], isLocked);

            if (newOrRecycled.ContentSizeFitter.enabled)
                newOrRecycled.ContentSizeFitter.enabled = false;

            newOrRecycled.MarkForRebuild();
            ScheduleComputeVisibilityTwinPass(true);
        }

        public void SetDragEnabled(bool isEnabled)
        {
            _Params.SetDragEnabled(isEnabled);
        }

        public int GetScrollItemIndex()
        {
            return _Params.Snapper.GetMiddleVH(out _).ItemIndex;
        }
    }

    [Serializable]
    public class PopupEpicPassV2RewardItemParams : BaseParams
    {
        public List<Blackboard> data = new List<Blackboard>();
        public GameObject prefab = null;

        public GameObject GetPrefab(Transform transform)
        {
            if (prefab == null)
            {
                prefab = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Epic Pass Rewards Cell", transform);
                prefab.SetActive(false);
            }

            return prefab;
        }

        public void SetDragEnabled(bool isEnabled)
        {
            DragEnabled = isEnabled;
        }
    }

    [Serializable]
    public class PopupEpicPassV2RewardItemViewHolder : BaseItemViewsHolder
    {
        private PopupEpicPassV2RewardCellController controller;

        public ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(Blackboard rewardsBB, bool isLocked)
        {
            controller.OnInit(rewardsBB, isLocked);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<PopupEpicPassV2RewardCellController>();

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