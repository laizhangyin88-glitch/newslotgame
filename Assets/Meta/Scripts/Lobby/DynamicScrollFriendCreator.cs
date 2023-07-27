using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    // suggest // online / today / yesterday / 3 days ago / 5 days ago / +10d ago
    public class DynamicScrollFriendCreator : DynamicScrollItemCreator
    {
        private enum ListTag
        {
            Suggest = 0,
            Online,
            Today,
            Yesterday,
            ThreeDays,
            FiveDays,
            TenDays,
            Friends
        };

        public enum FriendCellType
        {
            Normal = 0,
            Facebook,
            Invite,
            FriendCode,
            Suggest,
            Request,
            Unfriendable,
            Unknown
        }

        public enum BadgeType
        {
            None = 0,
            Plus,
            Facebook,
            Club
        }

        public ObjectPool friendsTagPool;
        public ObjectPool friendsListPool;
        public ObjectPool friendsCellPool;
        public ObjectPool friendsCoinPool;

        // TODO Get Tab Event
        // TODO how to modifying in created item

        private Blackboard bbFriends;
        private GameObject caller;
        private List<Blackboard> suggestedFriendList;
        private List<KeyValuePair<int, ListTag>> tagIndex = new List<KeyValuePair<int, ListTag>>();
        private List<List<Blackboard>> offlineFriendlist;

        private List<List<Blackboard>> requestFriendList;
        private List<List<Blackboard>> reorganizedList;
        private List<KeyValuePair<ListTag, GameObject>> requestTagList = new List<KeyValuePair<ListTag, GameObject>>();
        private List<KeyValuePair<ListTag, GameObject>> tagList = new List<KeyValuePair<ListTag, GameObject>>();

        private List<List<Blackboard>> refreshList;
        private List<KeyValuePair<ListTag, GameObject>> refreshTagList;

        private bool isFacebookConnected = false;
        private bool isInitialize = false;

        // private int requestFriendListCount = 0;

        public int backPageIndex = -1;

        public int tabMode = 0;
        public int columnCount = 0;
        public int columnIndex = 0;

        private void Awake()
        {
            caller = GetComponent<Blackboard>().GetValue<GameObject>("caller");
        }

        private void SetFirstFriendCellType(List<Blackboard> bbList, bool isHeadType = true)
        {
            BlackboardUtils.DestroyBlackboard(bbFriends, "addFriendCode");
            Blackboard addFriendCode = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(bbFriends, "addFriendCode");

            BlackboardUtils.SetOrCreateValue<FriendCellType>(addFriendCode, "cellType", FriendCellType.FriendCode);
            BlackboardUtils.SetOrCreateValue<BadgeType>(addFriendCode, "cellBadgeType", BadgeType.Plus);
            BlackboardUtils.SetOrCreateValue<bool>(addFriendCode, "useBottomButton", false);
            BlackboardUtils.SetOrCreateValue<bool>(addFriendCode, "useXButtom", false);

            if (isHeadType)
            {
                bbList.Insert(0, addFriendCode);
            }
            else
            {
                bbList.Add(addFriendCode);
            }
            columnIndex++;

            if (isFacebookConnected)
            {
                BlackboardUtils.DestroyBlackboard(bbFriends, "fbInvite");
                Blackboard fbInvite = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(bbFriends, "fbInvite");

                BlackboardUtils.SetOrCreateValue<FriendCellType>(fbInvite, "cellType", FriendCellType.Invite);
                BlackboardUtils.SetOrCreateValue<BadgeType>(fbInvite, "cellBadgeType", BadgeType.Facebook);
                BlackboardUtils.SetOrCreateValue<bool>(fbInvite, "useBottomButton", false);
                BlackboardUtils.SetOrCreateValue<bool>(fbInvite, "useXButtom", false);

                if (isHeadType)
                {
                    bbList.Insert(1, fbInvite);
                }
                else
                {
                    bbList.Add(fbInvite);
                }
                columnIndex++;
            }
        }

        private void SetTagToList(int index, ListTag tag)
        {
            tagIndex.Add(new KeyValuePair<int, ListTag>(index, tag));
            return;
        }

        // todo : merge from SetFriendBB
        private void SetUpData()
        {
            bool isFirstList = true;
            int listIndex = 0;
            columnIndex = 0;

            tagIndex = new List<KeyValuePair<int, ListTag>>();
            requestTagList = new List<KeyValuePair<ListTag, GameObject>>();
            tagList = new List<KeyValuePair<ListTag, GameObject>>();
            reorganizedList = new List<List<Blackboard>>();
            var myClubID = BlackboardUtils.FindVariable<long>(null, "/clubId");

            var variableA = BlackboardUtils.FindVariable<string>(null, "/me/facebookId");

            if (string.IsNullOrEmpty(variableA.value))
            {
                isFacebookConnected = false;
            }
            else
            {
                isFacebookConnected = true;
            }

            {
                var variable = BlackboardUtils.FindVariable<int>(bbFriends, "_tabMode");
                if (variable != null)
                {
                    tabMode = variable.value;
                }
            }

            int friendCount = 0;
            var friendList = BlackboardQueryUtils.GetFriendList();
            for (int i = 0; i < friendList.Count; ++i)
            {
                if (friendList[i].GetValue<bool>("accepted"))
                    friendCount++;
            }

            {
                var variable = BlackboardUtils.FindVariable<List<Blackboard>>(bbFriends, "_recommendedFriends");
                if (variable != null && friendCount <= 6)
                {
                    // int columnLimits = 0;
                    if (variable.value.Count > 0)
                    {
                        reorganizedList.Add(new List<Blackboard>());
                        SetTagToList(listIndex, ListTag.Suggest);
                        listIndex++;
                    }

                    for (int i = 0; i < variable.value.Count; ++i)
                    {
                        BlackboardUtils.SetOrCreateValue<FriendCellType>(variable.value[i], "cellType", FriendCellType.Suggest);
                        BlackboardUtils.SetOrCreateValue<BadgeType>(variable.value[i], "cellBadgeType", BadgeType.None);
                        BlackboardUtils.SetOrCreateValue<bool>(variable.value[i], "useBottomButton", true);
                        BlackboardUtils.SetOrCreateValue<bool>(variable.value[i], "useXButtom", true);

                        if (columnIndex == 0)
                        {
                            reorganizedList.Add(new List<Blackboard>());
                        }
                        reorganizedList[listIndex].Add(variable.value[i]);

                        if (++columnIndex >= columnCount || i == variable.value.Count - 1)
                        {
                            listIndex++;
                            columnIndex = 0;
                            break;
                        }
                    }

                    suggestedFriendList = new List<Blackboard>();

                    for (int i = columnCount; i < variable.value.Count; i++)
                    {
                        BlackboardUtils.SetOrCreateValue<FriendCellType>(variable.value[i], "cellType", FriendCellType.Suggest);
                        BlackboardUtils.SetOrCreateValue<BadgeType>(variable.value[i], "cellBadgeType", BadgeType.None);
                        BlackboardUtils.SetOrCreateValue<bool>(variable.value[i], "useBottomButton", true);
                        BlackboardUtils.SetOrCreateValue<bool>(variable.value[i], "useXButtom", true);
                        suggestedFriendList.Add(variable.value[i]);
                    }

                }
            }

            {
                var variable = BlackboardUtils.FindVariable<List<Blackboard>>(bbFriends, "_onlineFriends");
                if (variable != null)
                {
                    if (variable.value.Count > 0)
                    {
                        reorganizedList.Add(new List<Blackboard>());
                        SetTagToList(listIndex, ListTag.Online);
                        listIndex++;

                        if (isFirstList)
                        {
                            reorganizedList.Add(new List<Blackboard>());
                            SetFirstFriendCellType(reorganizedList[listIndex]);
                            isFirstList = false;
                        }
                    }

                    for (int i = 0; i < variable.value.Count; ++i)
                    {
                        var clubID = variable.value[i].GetValue<long>("clubId");
                        BlackboardUtils.SetOrCreateValue<FriendCellType>(variable.value[i], "cellType", FriendCellType.Normal);
                        //Refactoring : Add BB to reorganizedList
                        if (myClubID.value > 0 && myClubID.value == clubID)
                        {
                            BlackboardUtils.SetOrCreateValue<BadgeType>(variable.value[i], "cellBadgeType", BadgeType.Club);
                        }
                        else
                        {
                            BlackboardUtils.SetOrCreateValue<BadgeType>(variable.value[i], "cellBadgeType", BadgeType.None);
                        }

                        BlackboardUtils.SetOrCreateValue<bool>(variable.value[i], "useBottomButton", false);
                        BlackboardUtils.SetOrCreateValue<bool>(variable.value[i], "useXButtom", false);

                        if (columnIndex == 0)
                        {
                            reorganizedList.Add(new List<Blackboard>());
                        }
                        reorganizedList[listIndex].Add(variable.value[i]);

                        if (++columnIndex >= columnCount || i == variable.value.Count - 1)
                        {
                            listIndex++;
                            columnIndex = 0;
                        }
                    }
                }
            }

            {
                offlineFriendlist = new List<List<Blackboard>>();

                for (int i = 0; i < 5; ++i)
                {
                    offlineFriendlist.Add(new List<Blackboard>());
                }

                var variable = BlackboardUtils.FindVariable<List<Blackboard>>(bbFriends, "_offlineFriends");
                if (variable != null)
                {
                    foreach (Blackboard bb in variable.value)
                    {
                        long lastOnlineTimestamp = bb.GetVariable<long>("lastOnlineTimestamp").value;
                        int dayCount = BagelCode.TimeUtils.GetLastLoginDayCount(lastOnlineTimestamp);

                        if (dayCount < 1)
                        {
                            offlineFriendlist[0].Add(bb);
                        }
                        else if (dayCount < 3)
                        {
                            offlineFriendlist[1].Add(bb);
                        }
                        else if (dayCount < 5)
                        {
                            offlineFriendlist[2].Add(bb);
                        }
                        else if (dayCount < 10)
                        {
                            offlineFriendlist[3].Add(bb);
                        }
                        else
                        {
                            offlineFriendlist[4].Add(bb);
                        }
                    }
                }
            }

            //Refactoring : Add offlineList to reorganizedList
            for (int i = 0; i < offlineFriendlist.Count; i++)
            {
                if (offlineFriendlist[i].Count > 0)
                {
                    reorganizedList.Add(new List<Blackboard>());
                    SetTagToList(listIndex, (ListTag)(i + 2));
                    listIndex++;

                    if (isFirstList)
                    {
                        reorganizedList.Add(new List<Blackboard>());
                        SetFirstFriendCellType(reorganizedList[listIndex]);
                        isFirstList = false;
                    }
                }
                for (int j = 0; j < offlineFriendlist[i].Count; j++)
                {
                    if (i == 4)
                    {
                        BlackboardUtils.SetOrCreateValue<FriendCellType>(offlineFriendlist[i][j], "cellType", FriendCellType.Unfriendable);
                        BlackboardUtils.SetOrCreateValue<bool>(offlineFriendlist[i][j], "useBottomButton", false);
                        BlackboardUtils.SetOrCreateValue<bool>(offlineFriendlist[i][j], "useXButtom", true);
                    }
                    else
                    {
                        BlackboardUtils.SetOrCreateValue<FriendCellType>(offlineFriendlist[i][j], "cellType", FriendCellType.Normal);
                        BlackboardUtils.SetOrCreateValue<bool>(offlineFriendlist[i][j], "useBottomButton", false);
                        BlackboardUtils.SetOrCreateValue<bool>(offlineFriendlist[i][j], "useXButtom", false);
                    }

                    var clubID = offlineFriendlist[i][j].GetValue<long>("clubId");
                    if (myClubID.value > 0 && myClubID.value == clubID)
                    {
                        BlackboardUtils.SetOrCreateValue<BadgeType>(offlineFriendlist[i][j], "cellBadgeType", BadgeType.Club);
                    }
                    else
                    {
                        BlackboardUtils.SetOrCreateValue<BadgeType>(offlineFriendlist[i][j], "cellBadgeType", BadgeType.None);
                    }

                    if (columnIndex == 0)
                    {
                        reorganizedList.Add(new List<Blackboard>());
                    }

                    reorganizedList[listIndex].Add(offlineFriendlist[i][j]);

                    if (++columnIndex >= columnCount || j == offlineFriendlist[i].Count - 1)
                    {
                        listIndex++;
                        columnIndex = 0;
                    }
                }
            }

            if (isFirstList)
            {
                isFirstList = false;

                if (listIndex == 0)
                {
                    reorganizedList.Add(new List<Blackboard>());
                    SetFirstFriendCellType(reorganizedList[listIndex], false);
                }
                else
                {
                    if (reorganizedList[listIndex - 1].Count < columnCount)
                    {
                        SetFirstFriendCellType(reorganizedList[listIndex - 1], false);
                    }
                    else
                    {
                        reorganizedList.Add(new List<Blackboard>());
                        SetFirstFriendCellType(reorganizedList[listIndex], false);
                    }
                }
                columnIndex = 0;
            }

            {
                int requestListIndex = 0;
                requestFriendList = new List<List<Blackboard>>();
                var variable = BlackboardUtils.FindVariable<List<Blackboard>>(bbFriends, "_requestedFriends");
                if (variable != null)
                {
                    for (int i = 0; i < variable.value.Count; ++i)
                    {
                        //Refactoring : Add BB to reorganizedList
                        BlackboardUtils.SetOrCreateValue<FriendCellType>(variable.value[i], "cellType", FriendCellType.Request);
                        BlackboardUtils.SetOrCreateValue<bool>(variable.value[i], "useBottomButton", true);
                        BlackboardUtils.SetOrCreateValue<bool>(variable.value[i], "useXButtom", true);

                        var clubID = variable.value[i].GetValue<long>("clubId");
                        if (myClubID.value > 0 && myClubID.value == clubID)
                        {
                            BlackboardUtils.SetOrCreateValue<BadgeType>(variable.value[i], "cellBadgeType", BadgeType.Club);
                        }
                        else
                        {
                            BlackboardUtils.SetOrCreateValue<BadgeType>(variable.value[i], "cellBadgeType", BadgeType.None);
                        }

                        if (columnIndex == 0)
                        {
                            requestFriendList.Add(new List<Blackboard>());
                        }
                        requestFriendList[requestListIndex].Add(variable.value[i]);

                        if (++columnIndex >= columnCount || i == variable.value.Count - 1)
                        {
                            requestListIndex++;
                            columnIndex = 0;
                        }
                    }
                }
            }

            int index = 0;
            for (int i = 0; i < reorganizedList.Count; i++)
            {
                if (index < tagIndex.Count && tagIndex[index].Key == i)
                {
                    tagList.Add(new KeyValuePair<ListTag, GameObject>(tagIndex[index].Value, null));
                    index++;
                }
                else
                {
                    tagList.Add(new KeyValuePair<ListTag, GameObject>(ListTag.Friends, null));
                }
            }
            for (int i = 0; i < requestFriendList.Count; i++)
            {
                requestTagList.Add(new KeyValuePair<ListTag, GameObject>(ListTag.Friends, null));
            }

            isInitialize = true;
        }

        public override void OnInitialize()
        {
            var grid = friendsListPool.GetObject();
            columnCount = LayoutUtils.CalcGridLayoutGroupMaxColumn(content, grid.GetComponent<GridLayoutGroup>());
            grid.ReturnToPool();
        }

        public void Clear()
        {
            while (content.childCount > 0)
            {
                if (content.GetChild(0).name.Equals("Friends List Tag"))
                {
                    content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                }
                else
                {
                    while (content.GetChild(0).childCount > 0)
                    {
                        content.GetChild(0).GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                    }
                    content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                }
            }

            frontIndex = -1;
            backIndex = 0;
            backPageIndex = -1;
        }

        private void InitTagItem(ListTag page, bool isFront)
        {
            var tag = friendsTagPool.GetObject(false);
            tag.transform.SetParent(content, false);
            Blackboard tagBB = tag.transform.GetComponent<Blackboard>();

            if (isFront)
            {
                tag.transform.SetAsFirstSibling();
                ListTag tagKey = tagList[frontIndex].Key;
                tagList.RemoveAt(frontIndex);
                tagList.Insert(frontIndex, new KeyValuePair<ListTag, GameObject>(tagKey, tag.gameObject));
            }
            else
            {
                ListTag tagKey = tagList[backIndex].Key;
                tagList.RemoveAt(backIndex);
                tagList.Insert(backIndex, new KeyValuePair<ListTag, GameObject>(tagKey, tag.gameObject));
            }

            BlackboardUtils.SetOrCreateValue<int>(tagBB, "pageNum", (int)page);
            tag.gameObject.SetActive(true);
            // tag.transform.GetComponent<NodeCanvas.StateMachines.FSMOwner>().SendEvent("InitFriendTag");
        }

        private void InitCellItem(Transform parent, Blackboard info)
        {
            FriendCellType type = BlackboardUtils.FindVariable<FriendCellType>(info, "cellType").value;
            BadgeType badgeType = BlackboardUtils.FindVariable<BadgeType>(info, "cellBadgeType").value;
            bool useBottomButton = BlackboardUtils.FindVariable<bool>(info, "useBottomButton").value;
            bool useXButtom = BlackboardUtils.FindVariable<bool>(info, "useXButtom").value;

            var cell = friendsCellPool.GetObject(false);
            cell.transform.SetParent(parent, false);

            BlackboardUtils.SetOrCreateValue<GameObject>(info, "object", cell.gameObject);
            Blackboard cellBB = cell.transform.GetComponent<Blackboard>();
            cellBB.RemoveVariable("cellInfo");
            BlackboardUtils.SetOrCreateValue<Blackboard>(cellBB, "cellInfo", info);

            // Check type
            var friendType = BlackboardUtils.FindVariable<FriendType>(info, "type");
            if (friendType != null)
            {
                if (friendType.value == FriendType.FACEBOOK)
                {
                    BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)FriendCellType.Facebook);
                    BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)BadgeType.Facebook);
                    BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useBottomButton", false);
                    BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useXButtom", false);
                }
                // else if (friendType.value == FriendType.GOOGLE)
                // {
                //     // todo: GP icon
                //     BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)type);
                //     BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)badgeType);
                //     BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useBottomButton", useBottomButton);
                //     BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useXButtom", useXButtom);
                // }
                else
                {
                    BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)type);
                    BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)badgeType);
                    BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useBottomButton", useBottomButton);
                    BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useXButtom", useXButtom);
                }
            }
            // Add friend, Invite
            else
            {
                BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)type);
                BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)badgeType);
                BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useBottomButton", useBottomButton);
                BlackboardUtils.SetOrCreateValue<bool>(cellBB, "useXButtom", useXButtom);
            }
            BlackboardUtils.SetOrCreateValue<GameObject>(cellBB, "coinPool", friendsCoinPool.gameObject);

            // Init Cell
            var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(cellBB, "caller");
            variable.value = gameObject;
            cell.gameObject.SetActive(true);
        }

        private bool PushFriendsFront()
        {
            if (tagList[frontIndex].Key != ListTag.Friends)
            {
                InitTagItem(tagList[frontIndex].Key, true);
            }
            else
            {
                var list = friendsListPool.GetObject();

                for (int i = 0; i < reorganizedList[frontIndex].Count; i++)
                {
                    InitCellItem(list.transform, reorganizedList[frontIndex][i]);
                }

                list.transform.SetParent(content, false);
                list.transform.SetAsFirstSibling();
                tagList.RemoveAt(frontIndex);
                tagList.Insert(frontIndex, new KeyValuePair<ListTag, GameObject>(ListTag.Friends, list.gameObject));
            }
            frontIndex--;
            return true;
        }

        private bool PushRequestsFront()
        {
            var list = friendsListPool.GetObject();

            for (int i = 0; i < requestFriendList[frontIndex].Count; ++i)
            {
                InitCellItem(list.transform, requestFriendList[frontIndex][i]);
            }

            list.transform.SetParent(content, false);
            list.transform.SetAsFirstSibling();
            requestTagList.RemoveAt(frontIndex);
            requestTagList.Insert(frontIndex, new KeyValuePair<ListTag, GameObject>(ListTag.Friends, list.gameObject));

            frontIndex--;
            return true;
        }

        protected override bool PushFront()
        {
            if (!isInitialize) return false;
            if (frontIndex < 0) return false;

            if (tabMode == 0)
            {
                return PushFriendsFront();
            }
            else
            {
                return PushRequestsFront();
            }
        }

        private bool PushFriendsBack()
        {
            if (tagList[backIndex].Key != ListTag.Friends)
            {
                InitTagItem(tagList[backIndex].Key, false);
            }
            else
            {
                var list = friendsListPool.GetObject();

                for (int i = 0; i < reorganizedList[backIndex].Count; i++)
                {
                    InitCellItem(list.transform, reorganizedList[backIndex][i]);
                }

                list.transform.SetParent(content, false);
                list.transform.SetAsLastSibling();
                tagList.RemoveAt(backIndex);
                tagList.Insert(backIndex, new KeyValuePair<ListTag, GameObject>(ListTag.Friends, list.gameObject));
            }
            backIndex++;
            return true;
        }

        private bool PushRequestsBack()
        {
            var list = friendsListPool.GetObject();

            for (int i = 0; i < requestFriendList[backIndex].Count; ++i)
            {
                InitCellItem(list.transform, requestFriendList[backIndex][i]);
            }

            list.transform.SetParent(content, false);
            list.transform.SetAsLastSibling();
            requestTagList.RemoveAt(backIndex);
            requestTagList.Insert(backIndex, new KeyValuePair<ListTag, GameObject>(ListTag.Friends, list.gameObject));

            backIndex++;
            return true;
        }

        protected override bool PushBack()
        {
            if (!isInitialize) return false;
            if (tabMode == 0)
            {
                if (backIndex >= reorganizedList.Count)
                {
                    return false;
                }

                return PushFriendsBack();
            }
            else
            {
                if (backIndex >= requestFriendList.Count)
                {
                    return false;
                }

                return PushRequestsBack();
            }
        }

        protected override bool PopFront()
        {
            frontIndex++;

            var list = content.GetChild(0);

            if (list.name.Equals("Friends List Tag"))
            {
                list.GetComponent<PooledObject>().ReturnToPool();
            }
            else
            {
                int cellIndex = list.childCount - 1;
                while (cellIndex >= 0)
                {
                    reorganizedList[frontIndex][cellIndex].RemoveVariable("object");
                    list.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                    cellIndex--;
                }
                list.GetComponent<PooledObject>().ReturnToPool();
            }
            return true;
        }

        protected override bool PopBack()
        {
            backIndex--;

            var list = content.GetChild(content.childCount - 1);

            if (list.name.Equals("Friends List Tag"))
            {
                list.GetComponent<PooledObject>().ReturnToPool();
            }
            else
            {
                int cellIndex = list.childCount - 1;
                while (cellIndex >= 0)
                {
                    reorganizedList[backIndex][cellIndex].RemoveVariable("object");
                    list.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                    cellIndex--;
                }
                list.GetComponent<PooledObject>().ReturnToPool();
            }
            return true;
        }

        public void RemoveItem(string userId)
        {
            // todo : 
            Refresh();
        }

        private void RefreshCells()
        {
            SetUpData();

            for (int i = 0; i < maxBufferingCount; ++i)
            {
                if (!PushBack())
                    break;
            }

            RebuildContentBounds();
            dynamicScrollRect.horizontalNormalizedPosition = 0f;
            dynamicScrollRect.verticalNormalizedPosition = 1f;
        }

        private Blackboard DragUpCell(int index, bool isPooled = true)
        {
            if (isPooled)
            {
                GameObject cell = refreshList[index][0].GetValue<GameObject>("object");
                cell.transform.SetParent(refreshTagList[index - 1].Value.transform, false);
            }
            refreshList[index - 1].Add(refreshList[index][0]);
            refreshList[index].RemoveAt(0);
            return refreshList[index - 1][columnCount - 1];
        }

        private void PopCell(int row, int column)
        {
            refreshList[row][column].GetValue<GameObject>("object").GetComponent<PooledObject>().ReturnToPool();
            refreshList[row][column].RemoveVariable("object");
            refreshList[row].RemoveAt(column);
        }

        private void PopTag(int index, bool isPooled = true, bool hasTap = true)
        {
            if (isPooled)
            {
                refreshTagList[index].Value.GetComponent<PooledObject>().ReturnToPool();
                backIndex--;
            }
            refreshList.RemoveAt(index);
            refreshTagList.RemoveAt(index);

            if (!hasTap)
                return;

            if (refreshTagList[index - 1].Key != ListTag.Friends)
            {
                PopTag(index - 1, true, false);
            }
            return;
        }

        private FriendCellType FindFriendCell(string userId, ref int row, ref int column)
        {
            if (tabMode == 0)
            {
                refreshList = reorganizedList;
                refreshTagList = tagList;
            }
            else
            {
                refreshList = requestFriendList;
                refreshTagList = requestTagList;
            }

            for (int i = frontIndex + 1; i < backIndex; i++)
            {
                for (int j = 0; j < refreshList[i].Count; j++)
                {
                    var currentUserId = BlackboardUtils.FindVariable<string>(refreshList[i][j], "userId");
                    if (currentUserId != null && string.Equals(userId, currentUserId.value))
                    {
                        row = i;
                        column = j;
                        return refreshList[i][j].GetValue<FriendCellType>("cellType");
                    }
                }
            }
            return FriendCellType.Unknown;
        }

        private void RefreshHead(int column)
        {
            PopCell(1, column);

            if (suggestedFriendList.Count != 0)
            {
                InitCellItem(refreshTagList[1].Value.transform, suggestedFriendList[0]);
                refreshList[1].Add(suggestedFriendList[0]);
                suggestedFriendList.RemoveAt(0);
            }
            else if (refreshTagList.Count > 2 && refreshTagList[2].Key == ListTag.Friends)
            {
                DragUpCell(2);
                if (refreshList[2].Count == 0)
                {
                    PopTag(2);
                    return;
                }
            }
            if (refreshList[1].Count == 0)
            {
                PopTag(1);
            }
        }

        private void RefreshBody(int row, int column, bool hasTap = true)
        {
            PopCell(row, column);
            if (refreshList[row].Count == 0)
            {
                PopTag(row, true, hasTap);
                return;
            }

            for (int i = row + 1; i < refreshList.Count; i++)
            {
                if (i < backIndex)
                    DragUpCell(i);
                else if (i == backIndex)
                    InitCellItem(refreshTagList[i - 1].Value.transform, DragUpCell(i, false));
                else
                    DragUpCell(i, false);

                if (refreshList[i].Count == 0)
                {
                    if (i < backIndex)
                        PopTag(i);
                    else
                        PopTag(i, false);
                    return;
                }
            }
        }

        public void Refresh()
        {
            if (bbFriends == null)
            {
                bbFriends = caller.GetComponent<Blackboard>();
            }

            dynamicScrollRect.enabled = false;
            Clear();
            RefreshCells();
            dynamicScrollRect.enabled = true;

            caller.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>().SendEvent("OnFinishLoading");
        }

        public void ReFresh(string userId)
        {
            int row = 0;
            int column = 0;
            FriendCellType cellType = FindFriendCell(userId, ref row, ref column);

            switch (cellType)
            {
                case FriendCellType.Suggest:
                    RefreshHead(column);
                    break;
                case FriendCellType.Unfriendable:
                    RefreshBody(row, column);
                    break;
                case FriendCellType.Request:
                    RefreshBody(row, column, false);
                    break;
            }
            return;
        }

    }
}
