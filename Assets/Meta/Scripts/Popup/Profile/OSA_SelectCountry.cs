using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using Com.TheFallenGames.OSA.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BagelCode.OSA_Scroll
{
    public class OSA_SelectCountry : OSA<SelectCountryParams, CountryViewHolder>
    {
        protected override void Start()
        {
            base.Start();

            if(_Params.data.Count > 0)
            {
                ResetItems(_Params.data.Count);

                if(_Params.selectCountryIndex > -1)
                    MoveToIndex(_Params.selectCountryIndex);
            }
        }

        public void InitList(List<string> countryCodeList, int selectCountryIndex)
        {
            _Params.data = countryCodeList;
            _Params.selectCountryIndex = selectCountryIndex;

            // if(IsInitialized)
            // {
            //     ResetItems(_Params.data.Count);

            //     _Params.selectCountryIndex = selectCountryIndex;
            //     if(_Params.selectCountryIndex > -1)
            //         MoveToIndex(_Params.selectCountryIndex);
            // }
            
        }

        public void UpdateSelectCountry(int selectCountryIndex)
        {
            _Params.selectCountryIndex = selectCountryIndex;
        }

        protected override CountryViewHolder CreateViewsHolder(int itemIndex)
        {
            CountryViewHolder viewHolder = new CountryViewHolder();
            viewHolder.Init( _Params.GetPrefab(transform), itemIndex);

            return viewHolder;
        }

        protected override void OnItemHeightChangedPreTwinPass(CountryViewHolder vh)
        {
            base.OnItemHeightChangedPreTwinPass(vh);

            // owner.chatDataList[vh.ItemIndex].changeSize = false;
            vh.ContentSizeFitter.enabled = false;
        }

        protected override void UpdateViewsHolder(CountryViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex], newOrRecycled.ItemIndex, _Params.selectCountryIndex);

            if (newOrRecycled.ContentSizeFitter.enabled)
                newOrRecycled.ContentSizeFitter.enabled = false;

            // if (chatData.changeSize)
            {
                // Height will be available before the next 'twin' pass, inside OnItemHeightChangedPreTwinPass() callback (see above)
                newOrRecycled.MarkForRebuild(); // will enable the content size fitter
                                                //newOrRecycled.contentSizeFitter.enabled = true;
                ScheduleComputeVisibilityTwinPass(true);
            }
        }

        protected override void OnBeforeRecycleOrDisableViewsHolder(CountryViewHolder inRecycleBinOrVisible, int newItemIndex)
        {
            base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }

        protected override bool IsRecyclable(CountryViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
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
    public class SelectCountryParams : BaseParams
    {
        public List<string> data = new List<string>();
        public GameObject prefab = null;
        public int selectCountryIndex = -1;

        public GameObject GetPrefab(Transform transform)
        {
            if(prefab == null)
                prefab = MetaObjectUtils.MakePrefab("Select Country Cell", transform);

            return prefab;
        }
    }

    [Serializable]
    public class CountryViewHolder : BaseItemViewsHolder
    {
        private PopupSelectCountryCellController controller;

        public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(string countryCode, int index, int selectIndex)
        {
            controller.Refresh(countryCode, index, selectIndex);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<PopupSelectCountryCellController>();

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
