using System;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.EpicPass;
using NodeCanvas.Framework;
using Com.ForbiddenByte.OSA.Core;

namespace BagelCode.OSA_Scroll
{
    public class OSA_EpicPassV2Rewards : OSA<EpicPassV2RewardItemParams, EpicPassV2RewardItemViewHolder>
    {
        private int checkCount = 4;

        protected override void Start()
        {
            base.Start();
            CreateItemList();
        }

        public void CreateItemList()
        {
            if (EpicPassUtilsV2.EpicPassInfo == null) return;

            var rewardInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(EpicPassUtilsV2.EpicPassInfo, "rewardInfoList");
            if (rewardInfoList == null || rewardInfoList.value == null) return;

            // Find first unclaimed item.
            _Params.data.Clear();
            _Params.data.AddRange(rewardInfoList.value);
            ResetItems(rewardInfoList.value.Count);

            if (EpicPassUtilsV2.Level > checkCount && rewardInfoList.value.Count > checkCount)
                ScrollTo((EpicPassUtilsV2.Level > rewardInfoList.value.Count ? rewardInfoList.value.Count : EpicPassUtilsV2.Level) - checkCount);
        }

        public void OnRefresh()
        {
            ClearVisibleItems();

            if (EpicPassUtilsV2.EpicPassInfo != null)
            {
                var rewardInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(EpicPassUtilsV2.EpicPassInfo, "rewardInfoList");
                if (rewardInfoList != null && rewardInfoList.value != null)
                {
                    _Params.data.Clear();
                    _Params.data.AddRange(rewardInfoList.value);
                    ResetItems(rewardInfoList.value.Count);

                    MoveToIndex(0);
                }
            }
        }

        protected override EpicPassV2RewardItemViewHolder CreateViewsHolder(int itemIndex)
        {
            EpicPassV2RewardItemViewHolder viewHolder = new EpicPassV2RewardItemViewHolder();
            viewHolder.Init(_Params.GetPrefab(transform), _Params.Content, itemIndex);

            return viewHolder;
        }

        protected override void OnItemHeightChangedPreTwinPass(EpicPassV2RewardItemViewHolder viewsHolder)
        {
            base.OnItemHeightChangedPreTwinPass(viewsHolder);
            viewsHolder.ContentSizeFitter.enabled = false;
        }

        protected override void UpdateViewsHolder(EpicPassV2RewardItemViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex], newOrRecycled.ItemIndex);

            if (newOrRecycled.ContentSizeFitter.enabled)
                newOrRecycled.ContentSizeFitter.enabled = false;

            newOrRecycled.MarkForRebuild();
            ScheduleComputeVisibilityTwinPass(true);
        }

        protected override bool IsRecyclable(EpicPassV2RewardItemViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
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

        public List<EpicPassV2RewardItemsController> GetControllers()
        {
            List<EpicPassV2RewardItemsController> controllers = new List<EpicPassV2RewardItemsController>();
            for (int i = 0; i < VisibleItemsCount; ++i)
                controllers.Add(_VisibleItems[i].Controller);
            return controllers;
        }
    }

    [Serializable]
    public class EpicPassV2RewardItemParams : BaseParams
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
    }

    [Serializable]
    public class EpicPassV2RewardItemViewHolder : BaseItemViewsHolder
    {
        public EpicPassV2RewardItemsController Controller { get { return controller; } }
        private EpicPassV2RewardItemsController controller;

        public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(Blackboard rewardsBB, int index)
        {
            controller.UpdateVariables(rewardsBB, index);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<EpicPassV2RewardItemsController>();

            ContentSizeFitter = root.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            ContentSizeFitter.enabled = false;
        }

        public override void MarkForRebuild()
        {
            base.MarkForRebuild();
            //if (ContentSizeFitter)
            //    ContentSizeFitter.enabled = true;
        }
    }
}
