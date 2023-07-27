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
    public class OSA_ClubArenaLeadersController : OSA<ClubArenaLeadersItemParams, ClubArenaLeadersItemViewHolder>
    {
        public ContextElement caller;

        protected override void Start()
        {
            base.Start();
            CreateItemList();
        }

        public void CreateItemList()
        {
            List<Blackboard> contributionList = ClubArenaUtils.ClubMemberContributionList;
            if (contributionList == null || contributionList.Count == 0) return;

            _Params.data.Clear();
            _Params.data.AddRange(contributionList);
            ResetItems(contributionList.Count);

            InitScrollPosition(contributionList);
        }

        private void InitScrollPosition(List<Blackboard> userList)
        {
            if (userList != null && userList.Count > 5)
            {
                string myUserId = BlackboardQueryUtils.GetMyUserId();
                if (myUserId == null)
                    MoveToIndex(0);
                else
                {
                    int myIndex = 0;
                    for (int i = 0; i < userList.Count; ++i)
                    {
                        string userId = BlackboardUtils.FindValue<string>(userList[i], "userId");
                        if (string.Equals(myUserId, userId))
                        {
                            myIndex = i;
                            break;
                        }
                    }
                    int scrollIndex = 0;
                    if (myIndex <= 3)
                        scrollIndex = 0;
                    else if (myIndex >= userList.Count - 3)
                        scrollIndex = userList.Count - 5;
                    else
                        scrollIndex = myIndex - 3;

                    MoveToIndex(scrollIndex);
                }
            }
        }

        public void OnRefresh()
        {
            ClearVisibleItems();
            List<Blackboard> contributionList = ClubArenaUtils.ClubMemberContributionList;

            if (contributionList == null || contributionList.Count == 0) return;

            _Params.data.Clear();
            _Params.data.AddRange(contributionList);
            ResetItems(contributionList.Count);

            MoveToIndex(0);
        }

        protected override ClubArenaLeadersItemViewHolder CreateViewsHolder(int itemIndex)
        {
            ClubArenaLeadersItemViewHolder viewHolder = new ClubArenaLeadersItemViewHolder();
            viewHolder.Init(_Params.GetPrefab(transform), itemIndex);
            return viewHolder;
        }

        protected override void OnItemHeightChangedPreTwinPass(ClubArenaLeadersItemViewHolder viewsHolder)
        {
            base.OnItemHeightChangedPreTwinPass(viewsHolder);
            viewsHolder.ContentSizeFitter.enabled = false;
        }

        protected override void UpdateViewsHolder(ClubArenaLeadersItemViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex], newOrRecycled.ItemIndex, caller);

            if (newOrRecycled.ContentSizeFitter.enabled)
                newOrRecycled.ContentSizeFitter.enabled = false;

            {
                newOrRecycled.MarkForRebuild();
                ScheduleComputeVisibilityTwinPass(true);
            }
        }

        protected override void OnBeforeRecycleOrDisableViewsHolder(ClubArenaLeadersItemViewHolder inRecycleBinOrVisible, int newItemIndex)
        {
            base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }

        protected override bool IsRecyclable(ClubArenaLeadersItemViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
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

        public void SetCaller(ContextElement _caller)
        {
            caller = _caller;
        }
    }

    [Serializable]
    public class ClubArenaLeadersItemParams : BaseParams
    {
        public List<Blackboard> data = new List<Blackboard>();
        public GameObject prefab = null;

        public GameObject GetPrefab(Transform transform)
        {
            if (prefab == null)
            {
                prefab = MetaObjectUtils.MakePrefab("mgclubarenacontents", "Arena Leaders Cell", transform);
                prefab.SetActive(false);
            }

            return prefab;
        }
    }

    [Serializable]
    public class ClubArenaLeadersItemViewHolder : BaseItemViewsHolder
    {
        private ClubArenaLeadersItemsController controller;

        public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(Blackboard contributionBB, int index, ContextElement caller)
        {
            controller.UpdateVariables(contributionBB, index, caller);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<ClubArenaLeadersItemsController>();

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