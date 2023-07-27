using System;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using BagelCode.VegasDreams;
using Com.TheFallenGames.OSA.Core;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.EventSystems;


namespace BagelCode.OSA_Scroll
{
    public class OSA_ExhibitionItems : OSA<ExhibitionParams, ExhibitionItem>
    {
        private Blackboard bb;

        protected override void Start()
        {   
            base.Start();
        }

        public void Refresh()
        {
            ClearVisibleItems();
        }

        public void CreateItemList(Blackboard exhibition, int themeId)
        {
            CreateItemList(out List<ExhibitionModel> newModels, exhibition, themeId);

            _Params.data.Clear();
            _Params.data.AddRange(newModels);
            ResetItems(newModels.Count);

            ScrollTo(0, 1);
        }

        public void ScrollToNext()
        {
            var index = _Params.Snapper.GetMiddleVH(out _).ItemIndex + 1;
            if (index >= GetItemsCount()) return;
            
            ScrollTo(index, 0.5f, 0.5f);
        }

        public void ScrollToPrev()
        {
            var index = _Params.Snapper.GetMiddleVH(out _).ItemIndex - 1;
            if (index < 0) return;

            ScrollTo(index, 0.5f, 0.5f);
        }

        protected override ExhibitionItem CreateViewsHolder(int itemIndex)
        {
            ExhibitionItem item = new Exhibition_Normal();
            
            if (item != null)
                item.Init(_Params.prefabs[0], itemIndex);

            return item;
        }
        
        protected override void UpdateViewsHolder(ExhibitionItem newOrRecycled)
        {
            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateViews(model);
        }

        protected override bool ShouldDestroyRecyclableItem(ExhibitionItem inRecycleBin, bool isInExcess)
        {
            return inRecycleBin.ShouldDestroyRecyclableItem();
        }

        private void CreateItemList(out List<ExhibitionModel> newModels, Blackboard bb, int themeId)
        {
            newModels = new List<ExhibitionModel>();
            var season = bb.GetValue<Blackboard>("season");
            var buildingList = bb.GetValue<List<Blackboard>>("buildingList");
            var buildingPresetList = BlackboardUtils.FindVariable<List<Blackboard>>(season, "preset/buildingPresetList").value;

            for (int i = 0; i < buildingList.Count; i++)
            {
                ExhibitionModel model = new ExhibitionModel();
                model.building = buildingList[i];
                model.buildingPreset = buildingPresetList[i];
                model.themeId = themeId;
                newModels.Add(model);
            }
        }
        
        private GameObject FindPrefab(int id)
        {
            if (_Params.prefabs[id] == null && _Params.sceneInfos[id] != null)
            {
                var go = SceneManager.LoadScene(transform, _Params.sceneInfos[id].GetSceneInfo());
                go.SetActive(false);
                _Params.prefabs[id] = go;
            }
            return _Params.prefabs[id];
        }
    }
    
    public abstract class ExhibitionItem : BaseItemViewsHolder
    {
        public virtual bool ShouldDestroyRecyclableItem() { return false; }
        public abstract void UpdateViews(ExhibitionModel model);
    }

    [Serializable]
    public class ExhibitionParams : BaseParams
    {
        public SceneInfoObject[] sceneInfos;
        public GameObject[] prefabs;

        public List<ExhibitionModel> data = new List<ExhibitionModel>();
    }

    [Serializable]
    public class ExhibitionModel
    {
        public Blackboard building;
        public Blackboard buildingPreset;
        public int themeId = 0;
    }

    public class Exhibition_Normal : ExhibitionItem
    {
        public override void UpdateViews(ExhibitionModel model)
        {
            root.gameObject.SetActive(true);
            var normalModel = model as ExhibitionModel;

            var controller = root.GetComponent<VegasDreamsExhibitionCellController>();
            controller.UpdateView(model);
        }
    }
}
