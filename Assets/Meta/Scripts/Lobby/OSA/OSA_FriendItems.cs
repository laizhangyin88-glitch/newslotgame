using System;
using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using Com.ForbiddenByte.OSA.Core;
using frame8.Logic.Misc.Other.Extensions;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.OSA_Scroll
{
    public class OSA_FriendItems : OSA<FriendParams, FriendItem>
    {
        public ObjectPool friendsCellPool;
        public ObjectPool friendsCoinPool;

        public int customTagSize = 70;

        private GameObject caller;
        private Blackboard friendsBB;
        private int tabMode;
        private int columnCount = 0;
        private bool isFacebookConnected = false;
        private long myClubID;

        public void Refresh()
        {
            CreateItemList();
        }

        public void RemoveItemFromID(string userId)
        {
            bool found = false;
            int row = 0;
            
            // find and remove corresponding item
            for (int i = 0; i < _Params.data.Count; i++)
            {
                if (_Params.data[i] is FriendModel_List)
                {
                    FriendModel_List listModel = _Params.data[i] as FriendModel_List;
                    for (int j = 0; j < listModel.friendInfoList.Count; j++)
                    {
                        var currentUserId = BlackboardUtils.FindVariable<string>(listModel.friendInfoList[j], "userId");
                        if(currentUserId != null && userId == currentUserId.value)
                        {
                            row = i;
                            found = true;
                            listModel.friendCellList[j].ReturnToPool();
                            listModel.friendCellList.RemoveAt(j);
                            listModel.friendInfoList.RemoveAt(j);
                            break;
                        }
                    }

                    if (found) break;
                }
            }

            if (found)
            {
                // Data Fix
                for (int i = 0 + 1; i < _Params.data.Count; i++)
                {
                    if (_Params.data[i] is FriendModel_List)
                    {
                        FriendModel_List currListModel =_Params.data[i] as FriendModel_List;

                        if(currListModel.transform == null && currListModel.friendCellList.Count > 0)
                        {
                            currListModel.friendCellList.Clear();
                        }
                    }
                }

                List<int> indexWillBeRemove = new List<int>();
                
                // shift items
                for (int i = row + 1; i < _Params.data.Count; i++)
                {
                    if (_Params.data[i] is FriendModel_Tag)
                        break;

                    FriendModel_List prevListModel =_Params.data[i-1] as FriendModel_List;
                    FriendModel_List currListModel =_Params.data[i] as FriendModel_List;
                    
                    prevListModel.friendInfoList.Add(currListModel.friendInfoList[0]);
                    currListModel.friendInfoList.RemoveAt(0);

                    if (currListModel.friendCellList.Count > 0)
                    {
                        currListModel.friendCellList[0].transform.SetParent(prevListModel.transform);
                        
                        prevListModel.friendCellList.Add(currListModel.friendCellList[0]);
                        currListModel.friendCellList.RemoveAt(0);
                    }
                    else if (currListModel.friendCellList.Count == 0 && prevListModel.friendCellList.Count > 0)
                    {
                        var cell = prevListModel.cellPool.GetComponent<ObjectPool>().GetObject();
                        cell.transform.SetParent(prevListModel.transform);
                
                        prevListModel.friendCellList.Add(cell);
                        
                        BlackboardUtils.SetOrCreateValue<GameObject>(prevListModel.friendInfoList.Last(), "object", cell.gameObject);
                        Blackboard cellBB = cell.transform.GetComponent<Blackboard>();
                        cellBB.RemoveVariable("cellInfo");
                
                        BlackboardUtils.SetOrCreateValue<Blackboard>(cellBB, "cellInfo", prevListModel.friendInfoList.Last());
                        BlackboardUtils.SetOrCreateValue<GameObject>(cellBB, "coinPool", prevListModel.coinPool);
                        BlackboardUtils.SetOrCreateValue<GameObject>(cellBB, "caller", prevListModel.caller);
                        BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)prevListModel.friendInfoList.Last().GetValue<CellType>("cellType"));
                        BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)prevListModel.friendInfoList.Last().GetValue<BadgeType>("cellBadgeType"));
                        BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useBottomButton", prevListModel.friendInfoList.Last().GetValue<bool>("useBottomButton"));
                        BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useXButton", prevListModel.friendInfoList.Last().GetValue<bool>("useXButton"));

                        cell.gameObject.SetActive(true);
                        cell.GetComponent<GraphOwner>().StopBehaviour();
                        cell.GetComponent<GraphOwner>().StartBehaviour();
                    }
                }

                // get index to remove 
                for (int i = row; i < _Params.data.Count; i++)
                {
                    if (_Params.data[i] is FriendModel_Tag)
                        break;
                    
                    FriendModel_List listModel =_Params.data[i] as FriendModel_List;
                    if (listModel.friendInfoList.Count == 0)
                    {
                        indexWillBeRemove.Add(i);

                        if (i == row && i - 1 > 0 && _Params.data[i - 1] is FriendModel_Tag)
                        {
                            indexWillBeRemove.Add(i - 1);
                        }
                        break;
                    }
                }

                if (indexWillBeRemove.Count > 0)
                {
                    for (int i = 0; i < indexWillBeRemove.Count; i++)
                    {
                        _Params.data.RemoveAt(indexWillBeRemove[i]);
                    }
                }
                
                ClearVisibleItems();
                ClearCachedRecyclableItems();
                ResetItems(_Params.data.Count);
                for (int i = 0; i < _Params.data.Count; i++)
                {
                    if (_Params.data[i] is FriendModel_Tag)
                        RequestChangeItemSizeAndUpdateLayout(i, customTagSize);
                }
            }
        }
        
        protected override FriendItem CreateViewsHolder(int itemIndex)
        {
            FriendItem item = null;
            FriendItemType itemType = _Params.data[itemIndex].itemType;

            switch (itemType)
            {
                case FriendItemType.Tag:
                    item = new FriendItem_Tag();
                    break;
                case FriendItemType.List:
                    item = new FriendItem_List();
                    break;
            }

            GameObject prefab = FindPrefab((int) itemType);
            
            if (item != null)
                item.Init(prefab, _Params.Content, itemIndex);

            item.ContentSizeFitter = prefab.GetComponent<ContentSizeFitter>();
            
            return item;
        }
        
        protected override void UpdateViewsHolder(FriendItem newOrRecycled)
        {
//            ScheduleComputeVisibilityTwinPass();
            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateViews(model);
            newOrRecycled.SetActiveCount(newOrRecycled.ItemIndex == 0);
        }
        
        protected override bool IsRecyclable(FriendItem potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return potentiallyRecyclable.CanPresentModelType(_Params.data[indexOfItemThatWillBecomeVisible].itemType);
        }

        protected override bool ShouldDestroyRecyclableItem(FriendItem inRecycleBin, bool isInExcess)
        {
            return inRecycleBin.ShouldDestroyRecyclableItem();
        }

        public void CreateItemList()
        {   
            caller = GetComponent<Blackboard>().GetValue<GameObject>("caller");
            
            var newModels = new List<FriendModel>();

            CreateItemList(newModels);

            ClearVisibleItems();
            _Params.data.Clear();
            
            _Params.data.AddRange(newModels);
            
            _Params.ContentPadding.bottom = tabMode == 0 ? 0 : 78;
            _Params.ContentSpacing = tabMode == 0 ? 0 : 20;
            
            ResetItems(newModels.Count);
            
            RebuildLayoutDueToScrollViewSizeChange();
            
            for (int i = 0; i < newModels.Count; i++)
            {
                if (newModels[i] is FriendModel_Tag)
                    RequestChangeItemSizeAndUpdateLayout(i, customTagSize);
            }

            SetNormalizedPosition(1);

            caller.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>().SendEvent("OnFinishLoading");
        }

        void CreateItemList(List<FriendModel> newModels)
        {   
            if (friendsBB == null)
                friendsBB = caller.GetComponent<Blackboard>();

            tabMode = BlackboardUtils.FindValue<int>(friendsBB, "_tabMode");
            
            var grid = FindPrefab((int) FriendItemType.List);
            columnCount = LayoutUtils.CalcGridLayoutGroupMaxColumn(Content, grid.GetComponent<GridLayoutGroup>());

            var facebookId = BlackboardUtils.FindVariable<string>(null, "/me/facebookId");
            if (string.IsNullOrEmpty(facebookId.value))
                isFacebookConnected = false;
            else
                isFacebookConnected = true;

            myClubID = BlackboardUtils.FindVariable<long>(null, "/clubId").value;

            List<Blackboard> addableInfoList = new List<Blackboard>();
            CreateAddableItemList(addableInfoList);
            
            if (tabMode == 0)
            {
                var recommendedFriends = BlackboardUtils.FindVariable<List<Blackboard>>(friendsBB, "_recommendedFriends");
                var onlineFriends = BlackboardUtils.FindVariable<List<Blackboard>>(friendsBB, "_onlineFriends");
                var offlineFriends = BlackboardUtils.FindVariable<List<Blackboard>>(friendsBB, "_offlineFriends");

                if (recommendedFriends != null)
                {   
                    if (onlineFriends != null && onlineFriends.value.Count > 0 || offlineFriends != null && offlineFriends.value.Count > 0)
                        CreateModelsByTag(ListTag.Suggest, newModels, recommendedFriends.value);
                    else
                        CreateModelsByTag(ListTag.Suggest, newModels, recommendedFriends.value, addableInfoList);
                }
    
                if (onlineFriends != null)
                    CreateModelsByTag(ListTag.Online, newModels, onlineFriends.value, addableInfoList);
    
                if (offlineFriends != null)
                {
                    var offlineFriendsContainer = new List<List<Blackboard>>();
                    
                    for (int i = 0; i < 5; i++)
                        offlineFriendsContainer.Add(new List<Blackboard>());
                    
                    foreach (Blackboard bb in offlineFriends.value)
                    {
                        long lastOnlineTimestamp = bb.GetVariable<long>("lastOnlineTimestamp").value;
                        int dayCount = BagelCode.TimeUtils.GetLastLoginDayCount(lastOnlineTimestamp);
    
                        if (dayCount < 1)
                            offlineFriendsContainer[0].Add(bb);
                        else if (dayCount < 3)
                            offlineFriendsContainer[1].Add(bb);
                        else if (dayCount < 5)
                            offlineFriendsContainer[2].Add(bb);
                        else if (dayCount < 10)
                            offlineFriendsContainer[3].Add(bb);
                        else
                            offlineFriendsContainer[4].Add(bb);
                    }
                    
                    CreateModelsByTag(ListTag.Today, newModels, offlineFriendsContainer[0], addableInfoList);
                    CreateModelsByTag(ListTag.Yesterday, newModels, offlineFriendsContainer[1], addableInfoList);
                    CreateModelsByTag(ListTag.ThreeDays, newModels, offlineFriendsContainer[2], addableInfoList);
                    CreateModelsByTag(ListTag.FiveDays, newModels, offlineFriendsContainer[3], addableInfoList);
                    CreateModelsByTag(ListTag.TenDays, newModels, offlineFriendsContainer[4], addableInfoList);
                }
                
                CreateModelsByTag(ListTag.None, newModels, addableInfoList);
            }
            else if (tabMode == 1)
            {
                var requestFriends = BlackboardUtils.FindVariable<List<Blackboard>>(friendsBB, "_requestedFriends");
                if (requestFriends != null)
                    CreateModelsByTag(ListTag.Request, newModels, requestFriends.value);
            }
        }

        private void CreateAddableItemList(List<Blackboard> addableInfoList)
        {
#if UNITY_IOS || (UNITY_ANDROID && !PLATFORM_AMAZON)
            var enableInviteLink = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_INVITE_INSTALL");
            if(enableInviteLink != null && enableInviteLink.value)
            {
                BlackboardUtils.DestroyBlackboard(friendsBB, "inviteLink");
                Blackboard inviteLinkBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(friendsBB, "inviteLink");

                SetCellInfo(inviteLinkBB, CellType.InviteLink, BadgeType.Plus, false, false);
                addableInfoList.Add(inviteLinkBB);
            }
#endif
            BlackboardUtils.DestroyBlackboard(friendsBB, "addFriendCode");
            Blackboard addFriendCodeBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(friendsBB, "addFriendCode");
            
            SetCellInfo(addFriendCodeBB, CellType.FriendCode, BadgeType.Plus, false, false);
            addableInfoList.Add(addFriendCodeBB);

            if (isFacebookConnected)
            {
                BlackboardUtils.DestroyBlackboard(friendsBB, "fbInvite");
                Blackboard fbInviteBB = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(friendsBB, "fbInvite");
                
                SetCellInfo(fbInviteBB, CellType.Invite, BadgeType.Plus, false, false);
                addableInfoList.Add(fbInviteBB);
            }
        }

        private void SetCellInfo(Blackboard cellBB, CellType cellType, BadgeType badgeType, bool useBottomButton, bool useXButton)
        {
            BlackboardUtils.SetOrCreateValue<CellType>(cellBB, "cellType", cellType);
            BlackboardUtils.SetOrCreateValue<BadgeType>(cellBB, "cellBadgeType", badgeType);
            BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useBottomButton", useBottomButton);
            BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useXButton", useXButton);
        }

        private void CreateModelsByTag(ListTag tag, List<FriendModel> newModels, List<Blackboard> cellInfoList, List<Blackboard> addableInfoList = null)
        {
            if (cellInfoList.Count > 0)
            {
                List<Blackboard> clonedList = new List<Blackboard>();
                clonedList.AddRange(cellInfoList);
                
                switch (tag)
                {
                    case ListTag.Suggest:
                    {
                        if (CheckSuggestFriendEnabled())
                        {
                            if (clonedList.Count > columnCount)
                                clonedList = clonedList.GetRange(0, columnCount);
                            
                            CreateFriendsItemList(tag, newModels, clonedList, addableInfoList, true, false, false, true);
                        }
                        break;
                    }
                    case ListTag.Online:
                    {
                        CreateFriendsItemList(tag, newModels, clonedList, addableInfoList, true, true, false);
                        break;
                    }
                    case ListTag.Today:
                    case ListTag.Yesterday:
                    case ListTag.ThreeDays:
                    case ListTag.FiveDays:
                    case ListTag.TenDays:
                    {
                        CreateFriendsItemList(tag, newModels, clonedList, addableInfoList, true, true, true);   
                        break;
                    }
                    case ListTag.Request:
                    {
                        CreateFriendsItemList(tag, newModels, clonedList, addableInfoList, false, true, false);   
                        break;
                    }
                    case ListTag.None:
                    {
                        CreateFriendsItemList(tag, newModels, new List<Blackboard>(), addableInfoList, false, false, false);
                        break;
                    }
                }
            }
            
        }

        private void CreateFriendsItemList(
            ListTag tag, 
            List<FriendModel> newModels, 
            List<Blackboard> cellInfoList, 
            List<Blackboard> addableInfoList,
            bool addTag,
            bool checkClub,
            bool checkDay,
            bool addableInfoToEnd = false)
        {
            if (addTag)
            {
                FriendModel_Tag tagModel = new FriendModel_Tag();
                tagModel.tag = tag;
                newModels.Add(tagModel);
            }
            
            FriendModel_List listModel = new FriendModel_List(friendsCoinPool.gameObject, friendsCellPool, gameObject);

            if (addableInfoList != null)
            {
                if (addableInfoToEnd)
                    cellInfoList.AddRange(addableInfoList);
                else
                    cellInfoList.InsertRange(0, addableInfoList);
            }
            
            CellType cellType = CellType.Normal;
            BadgeType badgeType = BadgeType.None;
            bool useBottomButton = false;
            bool useXButton = false;
            
            for (int i = 0; i < cellInfoList.Count; i++)
            {
                if (addableInfoList == null ||
                    addableInfoList != null && (addableInfoToEnd && i < cellInfoList.Count - addableInfoList.Count || !addableInfoToEnd && i >= addableInfoList.Count))
                {
                    if (checkClub)
                    {
                        long clubID = cellInfoList[i].GetValue<long>("clubId");
                        if (myClubID > 0 && myClubID == clubID)
                            badgeType = BadgeType.Club;
                        else
                            badgeType = BadgeType.None;
                    }
    
                    if (checkDay)
                    {
                        if (tag == ListTag.TenDays)
                        {
                            cellType = CellType.Unfriendable;
                            useXButton = true;
                        }
                        else
                        {
                            cellType = CellType.Normal;
                            useXButton = false;
                        }
                    }
    
                    var friendType = cellInfoList[i].GetVariable<FriendType>("type");
                    if (friendType != null && friendType.value == FriendType.FACEBOOK)
                    {
                        cellType = CellType.Facebook;
                        badgeType = BadgeType.Facebook;
                    }
    
                    if (tag == ListTag.Suggest)
                    {
                        cellType = CellType.Suggest;
                        useBottomButton = true;
                        useXButton = true;
                    }
    
                    if (tag == ListTag.Request)
                    {
                        cellType = CellType.Request;
                        useBottomButton = true;
                        useXButton = true;
                    }
    
                    SetCellInfo(cellInfoList[i], cellType, badgeType, useBottomButton, useXButton);
                }
                
                listModel.friendInfoList.Add(cellInfoList[i]);

                if (listModel.friendInfoList.Count >= columnCount || i == cellInfoList.Count - 1)
                {
                    newModels.Add(listModel);
                    listModel = new FriendModel_List(friendsCoinPool.gameObject, friendsCellPool, gameObject);
                }
            }
            
            if (addableInfoList != null)
                addableInfoList.Clear();
        }

        private bool CheckSuggestFriendEnabled()
        {
            int friendCount = 0;
            var friendList = BlackboardQueryUtils.GetFriendList();
            
            for (int i = 0; i < friendList.Count; ++i)
            {
                if (friendList[i].GetValue<bool>("accepted"))
                    friendCount++;

                if (friendCount > 6)
                    break;
            }

            return friendCount <= 6;
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
        
        
    }
    
    
    public enum FriendItemType
    {
        Tag,
        List
    }

    public enum ListTag
    {
        Suggest  = 0,
        Online,
        Today,
        Yesterday,
        ThreeDays,
        FiveDays,
        TenDays,
        Request,
        None
    }

    public enum CellType
    {
        Normal     = 0,
        Facebook,
        Invite,
        FriendCode,
        Suggest,
        Request,
        Unfriendable,
        InviteLink,
        Unknown
    }

    public enum BadgeType
    {
        None = 0,
        Plus,
        Facebook,
        Club
    }
    
    [Serializable]
    public class FriendParams : BaseParams
    {
        public SceneInfoObject[] sceneInfos;
        public GameObject[] prefabs;

        public List<FriendModel> data = new List<FriendModel>();
    }
    
    [Serializable]
    public class FriendModel
    {
        public FriendItemType itemType;
    }

    [Serializable]
    public class FriendModel_List : FriendModel
    {
        public List<Blackboard> friendInfoList;
        public List<PooledObject> friendCellList;
        public GameObject coinPool;
        public ObjectPool cellPool;
        public GameObject caller;
        public Transform transform;

        public FriendModel_List(GameObject coinPool, ObjectPool cellPool, GameObject caller)
        {
            itemType = FriendItemType.List;
            friendInfoList = new List<Blackboard>();
            friendCellList = new List<PooledObject>();
            this.coinPool = coinPool;
            this.cellPool = cellPool;
            this.caller = caller;
        }
    }

    [Serializable]
    public class FriendModel_Tag : FriendModel
    {
        public ListTag tag;

        public FriendModel_Tag()
        {
            itemType = FriendItemType.Tag;
        }
    }
    
    public abstract class FriendItem : BaseItemViewsHolder
    {
        public ContentSizeFitter ContentSizeFitter;
        
        public abstract bool CanPresentModelType(FriendItemType itemType);
        public virtual bool ShouldDestroyRecyclableItem() { return false; }
        public abstract void UpdateViews(FriendModel model);
        public virtual void SetActiveCount(bool isActive) { }
     }
    
    public class FriendItem_List : FriendItem
    {
        public override bool CanPresentModelType(FriendItemType itemType) { return itemType == FriendItemType.List; }
        public override void UpdateViews(FriendModel model)
        {    
            var listModel = model as FriendModel_List;
            listModel.transform = root.transform;
            root.gameObject.SetActive(true);

            Transform[] children = root.transform.GetChildren();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].GetComponent<PooledObject>().ReturnToPool();
            }
            
            listModel.friendCellList.Clear();
            
            for (int i = 0; i < listModel.friendInfoList.Count; i++)
            {
                var cell = listModel.cellPool.GetComponent<ObjectPool>().GetObject();
                
                listModel.friendCellList.Add(cell);
                
                cell.transform.SetParent(root.transform, false);

                BlackboardUtils.SetOrCreateValue<GameObject>(listModel.friendInfoList[i], "object", cell.gameObject);
                Blackboard cellBB = cell.transform.GetComponent<Blackboard>();
                cellBB.RemoveVariable("cellInfo");
                
                BlackboardUtils.SetOrCreateValue<Blackboard>(cellBB, "cellInfo", listModel.friendInfoList[i]);
                BlackboardUtils.SetOrCreateValue<GameObject>(cellBB, "coinPool", listModel.coinPool);
                BlackboardUtils.SetOrCreateValue<GameObject>(cellBB, "caller", listModel.caller);
                BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)listModel.friendInfoList[i].GetValue<CellType>("cellType"));
                BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)listModel.friendInfoList[i].GetValue<BadgeType>("cellBadgeType"));
                BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useBottomButton", listModel.friendInfoList[i].GetValue<bool>("useBottomButton"));
                BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useXButton", listModel.friendInfoList[i].GetValue<bool>("useXButton"));

                cell.gameObject.SetActive(true);
                cell.GetComponent<GraphOwner>().StopBehaviour();
                cell.GetComponent<GraphOwner>().StartBehaviour();
            }
        }
    }
    
    public class FriendItem_Tag : FriendItem
    {
        public override bool CanPresentModelType(FriendItemType itemType) { return itemType == FriendItemType.Tag; }
        public override void UpdateViews(FriendModel model)
        {
            var tagModel = model as FriendModel_Tag;
            BlackboardUtils.SetOrCreateValue<int>(root.gameObject.GetComponent<Blackboard>(), "pageNum", (int)tagModel.tag);
            
            root.gameObject.SetActive(true);
            root.GetComponent<GraphOwner>().StopBehaviour();
            root.GetComponent<GraphOwner>().StartBehaviour();
        }

        public override void SetActiveCount(bool isActive)
        {
            var friendListCount = root.transform.Find("Layout/Friends List");
            if (friendListCount != null && friendListCount.gameObject != null)
            {
                friendListCount.gameObject.SetActive(isActive);

                if (isActive)
                {
                    var friendCountMax = BlackboardUtils.FindVariable<int>(null, "/values/misc/FRIEND_COUNT_MAX");

                    List<Blackboard> myFriendList = BlackboardQueryUtils.GetFriendList(true);

                    ContextElement friendListCountElement = friendListCount.gameObject.GetComponent<ContextElement>();
                    ContextElement friendListCountTextElement = ContextUtils.FindElement(friendListCountElement, "Text Friends List", ContextSearchingType.ChildrenSearch);
                    MetaContextElementUtils.SetTextGlobal(friendListCountTextElement, "FRIENDS_LIST_COUNT", myFriendList.Count, friendCountMax.value);
                }
            }
        }
    }
}
