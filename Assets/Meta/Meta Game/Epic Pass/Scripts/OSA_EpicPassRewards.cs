using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using BagelCode.EpicPass;
using NodeCanvas.Framework;
using Com.ForbiddenByte.OSA.Core;

namespace BagelCode.OSA_Scroll
{
    public class OSA_EpicPassRewards : OSA<EpicPassRewardItemParams, EpicPassRewardItemViewHolder>
    {
        protected override void Start()
        {
            base.Start();
            CreateItemList();
        }

        public void CreateItemList()
        {
            if(EpicPassUtils.EpicPassInfo == null) return;

            var rewardInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(EpicPassUtils.EpicPassInfo, "rewardInfoList");
            if(rewardInfoList == null || rewardInfoList.value == null) return;

            // Find first unclaimed item.

            _Params.data.Clear();
            _Params.data.AddRange(rewardInfoList.value);
            ResetItems(rewardInfoList.value.Count);

            if(EpicPassUtils.Level > 4 && rewardInfoList.value.Count > 4)
            {
                ScrollTo((EpicPassUtils.Level > rewardInfoList.value.Count ? rewardInfoList.value.Count : EpicPassUtils.Level)-4);
            }
        }

        public void OnRefresh()
        {
            ClearVisibleItems();

            if (EpicPassUtils.EpicPassInfo != null)
            {
                var rewardInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(EpicPassUtils.EpicPassInfo, "rewardInfoList");
                if (rewardInfoList != null && rewardInfoList.value != null)
                {
                    _Params.data.Clear();
                    _Params.data.AddRange(rewardInfoList.value);
                    ResetItems(rewardInfoList.value.Count);

                    MoveToIndex(0);
                }
            }
        }

        public void OnChangeMetaPopupCount()
        {
            var isInteractable = PopupManager.Instance.popupCount == 0;

            if (isInteractable == false)
            {
                StopMovement();
            }
        }


        protected override EpicPassRewardItemViewHolder CreateViewsHolder(int itemIndex)
        {
            EpicPassRewardItemViewHolder viewHolder = new EpicPassRewardItemViewHolder();
            viewHolder.Init( _Params.GetPrefab(transform), _Params.Content, itemIndex);

            return viewHolder;
        }

        protected override void OnItemHeightChangedPreTwinPass(EpicPassRewardItemViewHolder vh)
        {
            base.OnItemHeightChangedPreTwinPass(vh);

            vh.ContentSizeFitter.enabled = false;
        }

        protected override void UpdateViewsHolder(EpicPassRewardItemViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex], newOrRecycled.ItemIndex);

            if (newOrRecycled.ContentSizeFitter.enabled)
                newOrRecycled.ContentSizeFitter.enabled = false;

            {
                newOrRecycled.MarkForRebuild();
                ScheduleComputeVisibilityTwinPass(true);
            }
        }

        protected override void OnBeforeRecycleOrDisableViewsHolder(EpicPassRewardItemViewHolder inRecycleBinOrVisible, int newItemIndex)
        {
            base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }

        protected override bool IsRecyclable(EpicPassRewardItemViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
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

            if(index > GetItemsCount())
                index = GetItemsCount();

            ScrollTo(index);
        }
    }

    [Serializable]
    public class EpicPassRewardItemParams : BaseParams
    {
        public List<Blackboard> data = new List<Blackboard>();
        public GameObject prefab = null;

        public GameObject GetPrefab(Transform transform)
        {
            if(prefab == null)
            {
                prefab = MetaObjectUtils.MakePrefab("mgepicpasscontents", "Epic Pass Level Rewards", transform);
                prefab.SetActive(false);
            }

            return prefab;
        }
    }

    [Serializable]
    public class EpicPassRewardItemViewHolder : BaseItemViewsHolder
    {
        private EpicPassRewardItemsController controller;

        public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(Blackboard rewardsBB, int index)
        {
            controller.UpdateVariables(rewardsBB, index);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<EpicPassRewardItemsController>();

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
