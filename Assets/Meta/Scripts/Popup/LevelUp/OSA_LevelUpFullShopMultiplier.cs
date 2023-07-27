using System;
using System.Collections.Generic;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using SlotMaker;

namespace BagelCode.OSA_Scroll
{
    public class OSA_LevelUpFullShopMultiplier : OSA<LevelUpFullShopMultiplierParams, LevelUpFullShopMultiplierViewHolder>
    {
        private ContextElement rootElement = null;
        private List<PopupLevelMultiplierItem> itemList;

        protected override void Start()
        {
            base.Start();
            CreateItemList();
        }

        public void CreateItemList()
        {
            if (rootElement == null) rootElement = gameObject.GetComponent<ContextElement>();
            if (itemList == null || itemList.Count == 0) return;

            _Params.data.Clear();
            _Params.data.AddRange(itemList);
            ResetItems(itemList.Count);
        }

        public void SetItems(List<PopupLevelMultiplierItem> items)
        {
            itemList = items;
        }

        protected override LevelUpFullShopMultiplierViewHolder CreateViewsHolder(int itemIndex)
        {
            LevelUpFullShopMultiplierViewHolder viewHolder = new LevelUpFullShopMultiplierViewHolder();

            viewHolder.Init(_Params.GetPrefab(transform), itemIndex);
            return viewHolder;
        }

        protected override void UpdateViewsHolder(LevelUpFullShopMultiplierViewHolder newOrRecycled)
        {
            newOrRecycled.UpdateView(_Params.data[newOrRecycled.ItemIndex]);

            //if (newOrRecycled.ContentSizeFitter.enabled)
            //    newOrRecycled.ContentSizeFitter.enabled = false;
            {
                newOrRecycled.MarkForRebuild();
                ScheduleComputeVisibilityTwinPass(true);
            }
        }
    }

    [Serializable]
    public class LevelUpFullShopMultiplierParams : BaseParams
    {
        public List<PopupLevelMultiplierItem> data = new List<PopupLevelMultiplierItem>();
        private GameObject prefab = null;

        public GameObject GetPrefab(Transform transform)
        {
            if (prefab == null)
            {
                prefab = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "LM Shop Reward Item Cell", transform, null, "Item Cell");
                prefab.SetActive(false);
            }

            return prefab;
        }
    }

    [Serializable]
    public class LevelUpFullShopMultiplierViewHolder : BaseItemViewsHolder
    {
        public PopupLevelMultiplierItemController controller;
        //public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(PopupLevelMultiplierItem levelMultiplierItem)
        {
            controller.OnInit(levelMultiplierItem);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<PopupLevelMultiplierItemController>();

            //ContentSizeFitter = root.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            //ContentSizeFitter.enabled = false;
        }
    }
}