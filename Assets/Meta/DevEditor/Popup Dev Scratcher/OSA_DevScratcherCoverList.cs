using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using Com.TheFallenGames.OSA.Core;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class OSA_DevScratcherCoverList : OSA<ScratcherCellParams, ScratcherCoverViewHolder>
    {
        public List<ScratcherDevCellInfo> symbolList = new List<ScratcherDevCellInfo>();
        public List<ScratcherDevCellInfo> coverList = new List<ScratcherDevCellInfo>();

        public void UpdateSymbolItemList()
        {
            if(symbolList.Count == 0)
            {
                symbolList = new List<ScratcherDevCellInfo>();

                int symbolCount = Scratcher.ScratcherCustomData.Instance.scratcherCellAssets.symbolAssets.Count;

                for(int i=0; i < symbolCount; ++i)
                {
                    var cellInfo = new ScratcherDevCellInfo();
                    cellInfo.symbol = new Scratcher.ScratcherCellSymbol(i);
                    cellInfo.index = i;
                    symbolList.Add(cellInfo);
                }
            }
            
            if(_Params.data == null)
                _Params.data = new List<ScratcherDevCellInfo>();
            else
                _Params.data.Clear();
                
            _Params.data.AddRange(symbolList);
            ResetItems(symbolList.Count);
        }

        public void UpdateCoverItemList()
        {
            if(coverList.Count == 0)
            {
                coverList = new List<ScratcherDevCellInfo>();

                int coverCount = Scratcher.ScratcherCustomData.Instance.scratcherCellAssets.customCoverAssets.Count;

                for(int i=0; i < coverCount; ++i)
                {
                    var cellInfo = new ScratcherDevCellInfo();
                    cellInfo.cover = new Scratcher.ScratcherCellCover(i);
                    cellInfo.index = i;
                    coverList.Add(cellInfo);
                }
            }
            
            if(_Params.data == null)
                _Params.data = new List<ScratcherDevCellInfo>();
            else
                _Params.data.Clear();

            _Params.data.AddRange(coverList);
            ResetItems(coverList.Count);
        }

        protected override ScratcherCoverViewHolder CreateViewsHolder(int itemIndex)
        {
            ScratcherCoverViewHolder viewHolder = new ScratcherCoverViewHolder();
            viewHolder.Init(FindPrefab(), itemIndex);

            return viewHolder;
        }

        protected override void OnItemHeightChangedPreTwinPass(ScratcherCoverViewHolder vh)
        {
            base.OnItemHeightChangedPreTwinPass(vh);

            vh.ContentSizeFitter.enabled = false;
        }

        protected override void UpdateViewsHolder(ScratcherCoverViewHolder newOrRecycled)
        {
            // Debug.LogError("UpdateViewsHolder");
            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateView(model);

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

        protected override bool IsRecyclable(ScratcherCoverViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return true;
        }

        private GameObject FindPrefab()
        {
            if(_Params.prefab == null)
            {
                _Params.prefab = MetaObjectUtils.MakePrefab("testsuite", _Params.prefabName, transform);
                _Params.prefab.name = _Params.prefabName;
                _Params.prefab.SetActive(false);
            }

            return _Params.prefab;
        }
    }

    public class ScratcherDevCellInfo
    {
        public Scratcher.ScratcherCellSymbol symbol;
        public Scratcher.ScratcherCellCover cover;

        public int index;
    }

    [Serializable]
    public class ScratcherCellParams : BaseParams
    {
        public List<ScratcherDevCellInfo> data;

        public string prefabName = "Dev Scratcher Cell";
        public GameObject prefab = null;
    }

    [Serializable]
    public class ScratcherCoverViewHolder : BaseItemViewsHolder
    {
        public DevScratcherCellController controller;
        public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(ScratcherDevCellInfo cellData)
        {
            controller.Refresh(cellData);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<DevScratcherCellController>();
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