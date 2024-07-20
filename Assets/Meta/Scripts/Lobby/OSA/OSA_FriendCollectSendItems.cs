using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using Com.ForbiddenByte.OSA.Core;
using frame8.Logic.Misc.Other.Extensions;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.OSA_Scroll
{
    public class OSA_FriendCollectSendItems : OSA<FriendCollectSendParams, FriendCollectSendItem>
    {
        public ObjectPool friendsCellPool;
        private int UIMode = 0;
        private int columnCount = 0;

        public void Refresh()
        {
            CreateItemList();
        }

        protected override FriendCollectSendItem CreateViewsHolder(int itemIndex)
        {
            FriendCollectSendItem item = null;
            FriendCollectSendItemType itemType = _Params.data[itemIndex].itemType;

            switch (itemType)
            {
                case FriendCollectSendItemType.List:
                    item = new FriendCollectSendItem_List();
                    break;
            }
            
            if (item != null)
                item.Init(FindPrefab((int)itemType), _Params.Content, itemIndex);

            return item;
        }
        
        protected override void UpdateViewsHolder(FriendCollectSendItem newOrRecycled)
        {
            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateViews(model);
        }
        
        protected override bool IsRecyclable(FriendCollectSendItem potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return potentiallyRecyclable.CanPresentModelType(_Params.data[indexOfItemThatWillBecomeVisible].itemType);
        }

        protected override bool ShouldDestroyRecyclableItem(FriendCollectSendItem inRecycleBin, bool isInExcess)
        {
            return inRecycleBin.ShouldDestroyRecyclableItem();
        }

        public void CreateItemList()
        {
            GameObject caller = GetComponent<Blackboard>().GetValue<GameObject>("caller");
            
            var newModels = new List<FriendCollectSendModel>();

            CreateItemList(newModels);
            
            ClearVisibleItems();
            ClearCachedRecyclableItems();
            
            _Params.data.Clear();
            _Params.data.AddRange(newModels);
            ResetItems(newModels.Count);
            ScrollTo(0);
            
            caller.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>().SendEvent("OnFinishCollectSendLoading");
        }
        
        void CreateItemList(List<FriendCollectSendModel> newModels)
        {
            UIMode = GetComponent<Blackboard>().GetValue<int>("UIMode");
            
            var grid = FindPrefab((int) FriendCollectSendItemType.List);
            columnCount = LayoutUtils.CalcGridLayoutGroupMaxColumn(Content, grid.GetComponent<GridLayoutGroup>());

            var friendList = BlackboardQueryUtils.GetFriendList();
            FriendCollectSendModel_List model = null;
            
            for (int i = 0; i < friendList.Count; i++)
            {
                bool valid = false;

                if (friendList[i].GetValue<bool>("accepted"))
                {
                    if (UIMode == 0)
                    {
                        int giftCount = friendList[i].GetValue<int>("receivedGiftCount");
    
                        if (giftCount > 0)
                        {
                            valid = true;
                        }
                    }
                    else if (UIMode == 1)
                    {
                        long lastSendGiftTimestamp = friendList[i].GetValue<long>("lastSendGiftTimestamp");
                        int sendGiftTimeInterval  = BlackboardUtils.FindVariable<int>(null, "/values/misc/FRIEND_GIFT_SEND_INTERVAL_SEC").value;
    
                        if (BagelCode.TimeUtils.GetTimeStamp() - lastSendGiftTimestamp > (long) sendGiftTimeInterval * 1000)
                        {
                            valid = true;
                        }
                    }
                }

                if (valid)
                {
                    if (model == null)
                    {
                        model = new FriendCollectSendModel_List(friendsCellPool);
                    }
                    model.friendInfoList.Add(friendList[i]);
                }
                
                if (model != null && (model.friendInfoList.Count >= columnCount || i == friendList.Count - 1))
                {
                    newModels.Add(model);
                        
                    model = new FriendCollectSendModel_List(friendsCellPool);
                }
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

        public List<GameObject> GetGameObjectList()
        {
            List<GameObject> objectList = new List<GameObject>();

            for (int i = 0; i < _Params.data.Count; i++)
            {

                for (int j = 0; j < _Params.data[i].friendCellList.Count; j++)
                {
                    objectList.Add(_Params.data[i].friendCellList[j].gameObject);
                }
            }
            
            return objectList;
        }

        public void InteractableTrigger(bool trigger)
        {
            _Params.InteractableTrigger(trigger);
        }
    }
    
    public enum FriendCollectSendItemType
    {
        List
    }
    
    [Serializable]
    public class FriendCollectSendParams : BaseParams
    {
        public SceneInfoObject[] sceneInfos;
        public GameObject[] prefabs;

        public List<FriendCollectSendModel> data = new List<FriendCollectSendModel>();

        public void InteractableTrigger(bool trigger)
        {
            DragEnabled = trigger;
            ScrollEnabled = trigger;
        }
    }
    
    [Serializable]
    public class FriendCollectSendModel
    {
        public FriendCollectSendItemType itemType;
        public List<Blackboard> friendInfoList;
        public List<PooledObject> friendCellList;
        public ObjectPool cellPool;
    }

    [Serializable]
    public class FriendCollectSendModel_List : FriendCollectSendModel
    {
        public FriendCollectSendModel_List(ObjectPool cellPool)
        {
            itemType = FriendCollectSendItemType.List;
            friendInfoList = new List<Blackboard>();
            friendCellList = new List<PooledObject>();
            this.cellPool = cellPool;
        }
    }
    
    public abstract class FriendCollectSendItem : BaseItemViewsHolder
    {
        public abstract bool CanPresentModelType(FriendCollectSendItemType itemType);
        public virtual bool ShouldDestroyRecyclableItem() { return false; }
        public abstract void UpdateViews(FriendCollectSendModel model);
    }
    
    public class FriendCollectSendItem_List : FriendCollectSendItem
    {
        public override bool CanPresentModelType(FriendCollectSendItemType itemType) { return itemType == FriendCollectSendItemType.List; }
        public override void UpdateViews(FriendCollectSendModel model)
        {
            root.gameObject.SetActive(true);
            var listModel = model as FriendCollectSendModel_List;

            var children = root.GetChildren();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].GetComponent<PooledObject>().ReturnToPool();
            }
            
            listModel.friendCellList.Clear();
            
            for (int i = 0; i < listModel.friendInfoList.Count; i++)
            {
                listModel.friendCellList.Add(listModel.cellPool.GetObject());
                
                var cell = listModel.friendCellList[i];
                cell.transform.SetParent(root.transform, false);
                cell.gameObject.SetActive(false);
                
                cell.gameObject.GetComponent<IContextBooleanProperty>().SetBooleanProperty(false);

                Blackboard cellBB = cell.transform.GetComponent<Blackboard>();

                cellBB.RemoveVariable("cellInfo");
                BlackboardUtils.SetOrCreateValue<Blackboard>(cellBB, "cellInfo", listModel.friendInfoList[i]);

                var friendType = listModel.friendInfoList[i].GetValue<FriendType>("type");
                if (friendType == FriendType.FACEBOOK)
                {
                    BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)DynamicScrollFriendCreator.FriendCellType.Facebook);
                    BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)DynamicScrollFriendCreator.BadgeType.Facebook);
            
                }
                else
                {
                    var myClubID = BlackboardUtils.FindVariable<long>(null, "/clubId");
                    var clubID = listModel.friendInfoList[i].GetValue<long>("clubId");

                    BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)DynamicScrollFriendCreator.FriendCellType.Normal);

                    if(myClubID.value > 0 && myClubID.value == clubID)
                    {
                        BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)DynamicScrollFriendCreator.BadgeType.Club);
                    }
                    else
                    {
                        BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)DynamicScrollFriendCreator.BadgeType.None);
                    }
                }

                BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useBottomButton", false);
                BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useXButtom", false);

                cell.gameObject.SetActive(true);
            }
        }
    }
}
