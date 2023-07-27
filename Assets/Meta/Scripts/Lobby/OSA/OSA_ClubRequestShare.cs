using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using Com.TheFallenGames.OSA.Core;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode.OSA_Scroll
{
    public class OSA_ClubRequestShare : OSA<ClubRequestShreItemParams, ClubRequestShareItem>
    {
        public GameObject owner;
        public List<Blackboard> itemList;

        // protected override void Start()
        // {   
        //     base.Start();
        //     // CreateItemList();
        // }

        public void SetOwner(GameObject owner)
        {
            this.owner = owner;
            CreateItemList();
        }

        public void Refresh()
        {
            ClearVisibleItems();
            CreateItemList();
        }
        
        protected override ClubRequestShareItem CreateViewsHolder(int itemIndex)
        {
            ClubRequestShareItem item = null;
            ClubRequestShareItemType itemType = _Params.data[itemIndex].itemType;

            switch (itemType)
            {
                case ClubRequestShareItemType.REQUEST:
                    item = new ClubRequestShareItem_Normal();
                    break;
            }
            
            if (item != null)
                item.Init(FindPrefab((int)itemType), itemIndex);

            return item;
        }

        protected override void UpdateViewsHolder(ClubRequestShareItem newOrRecycled)
        {
            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateViews(model);
        }
        
        protected override bool IsRecyclable(ClubRequestShareItem potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return potentiallyRecyclable.CanPresentModelType(_Params.data[indexOfItemThatWillBecomeVisible].itemType);
        }

        protected override bool ShouldDestroyRecyclableItem(ClubRequestShareItem inRecycleBin, bool isInExcess)
        {
            return inRecycleBin.ShouldDestroyRecyclableItem();
        }
        
        public void CreateItemList()
        {
            var newModels = new List<ClubRequestShareModel>();

            CreateItemList(newModels);

            _Params.data.Clear();
            _Params.data.AddRange(newModels);
            ResetItems(newModels.Count);
        }

        void CreateItemList(List<ClubRequestShareModel> newModels)
        {
            if(owner == null) return;

            Blackboard ownerBB = owner.GetComponent<Blackboard>();
            if(ownerBB == null) return;

            var eventInfoType = BlackboardUtils.FindVariable<EventInfoType>(ownerBB, "response/clubShareInfo/type").value;
            var mgEventID     = ownerBB.GetValue<int>("mgEventID");
            var mgBundleName  = ownerBB.GetValue<string>("mgBundleName");
            var clubID        = ownerBB.GetValue<long>("clubID");

            itemList = BlackboardUtils.FindVariable<List<Blackboard>>(ownerBB, "response/clubShareInfo/pieceList").value;

            for (int i = 0; i < itemList.Count; i++)
            {
                var model = new ClubRequestShareModel_Normal();
                model.cellIndex = i;
                model.itemType = ClubRequestShareItemType.REQUEST;
                model.eventInfoType = eventInfoType;
                model.shareItemInfo = itemList[i];
                model.caller = owner;
                model.mgEventID = mgEventID;
                model.mgBundleName = mgBundleName;
                model.clubID = clubID;
                
                newModels.Add(model);
            }
        }
        
        GameObject FindPrefab(int id)
        {
            if (_Params.prefabs[id] == null && _Params.sceneInfos[id] != null)
            {
                var go = SceneManager.LoadScene(transform, _Params.sceneInfos[id].GetSceneInfo());
                go.SetActive(false);
                _Params.prefabs[id] = go;
            }
            return _Params.prefabs[id];
        }
        
        public void RemoveItemFromID(int removeItemID)
        {
            for (int i = 0; i < _Params.data.Count; i++)
            {
                if (_Params.data[i].shareItemInfo.GetValue<int>("id") == removeItemID)
                {
                    _Params.data.RemoveAt(i);
                    break;
                }
            }
            ResetItems(_Params.data.Count);
        }
    }
    
    public enum ClubRequestShareItemType
    {
        REQUEST = 0,
    }
    
    [Serializable]
    public class ClubRequestShreItemParams : BaseParams
    {
        public SceneInfoObject[] sceneInfos;
        public GameObject[] prefabs;

        public List<ClubRequestShareModel> data = new List<ClubRequestShareModel>();
    }
    
    [Serializable]
    public class ClubRequestShareModel
    {
        public int cellIndex;
        public ClubRequestShareItemType itemType;
        public EventInfoType eventInfoType;
        public Blackboard shareItemInfo;
        public GameObject caller;
        public int mgEventID;
        public string mgBundleName;
        public long clubID;
    }

    [Serializable]
    public class ClubRequestShareModel_Normal : ClubRequestShareModel {}
    
    public abstract class ClubRequestShareItem : BaseItemViewsHolder
    {
        public abstract bool CanPresentModelType(ClubRequestShareItemType itemType);
        public virtual bool ShouldDestroyRecyclableItem() { return false; }
        public abstract void UpdateViews(ClubRequestShareModel model);
    }
    
    public class ClubRequestShareItem_Normal : ClubRequestShareItem
    {
        public override bool CanPresentModelType(ClubRequestShareItemType itemType) { return itemType == ClubRequestShareItemType.REQUEST; }
        public override void UpdateViews(ClubRequestShareModel model)
        {
            var normalModel = model as ClubRequestShareModel_Normal;
            
            var rootBB = root.GetComponent<Blackboard>();
            rootBB.SetValue("cellIndex",    normalModel.cellIndex);
            rootBB.SetValue("eventInfoType",normalModel.eventInfoType);
            rootBB.SetValue("shareItemInfo",normalModel.shareItemInfo);
            rootBB.SetValue("caller",       normalModel.caller);
            rootBB.SetValue("mgEventID",    normalModel.mgEventID);
            rootBB.SetValue("mgBundleName", normalModel.mgBundleName);
            rootBB.SetValue("clubID",       normalModel.clubID);
            
            root.gameObject.SetActive(true);
            root.GetComponent<GraphOwner>().StopBehaviour();
            root.GetComponent<GraphOwner>().StartBehaviour();
        }
    }
}
