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
    public class DynamicScrollCollectSendCreator : DynamicScrollItemCreator
    {
        public ObjectPool friendsCellPool;
        public ObjectPool friendsListPool;

        // TODO Get Tab Event
        // TODO how to modifying in created item

        private List<List<Blackboard>> collectableFriendList;
        private List<List<Blackboard>> sendableFriendList;

        public List<GameObject> objectList;

        private bool isInitialize = false;

        //public int backPageIndex  = -1;

        public int UIMode = 0;
        public int columnCount = 0;

        // todo : merge from SetFriendBB
        private void SetUpData()
        {
            var bb = gameObject.GetComponent<Blackboard>();
            UIMode = bb.GetValue<int>("UIMode");

            collectableFriendList = new List<List<Blackboard>>();
            sendableFriendList = new List<List<Blackboard>>();

            int collectIndex = 0;
            int sendIndex = 0;
            int collectColumnIndex = 0;
            int sendColumnIndex = 0;

            var friendList = BlackboardQueryUtils.GetFriendList();

            for (int i = 0; i < friendList.Count; i++)
            {
                if (friendList[i].GetValue<bool>("accepted"))
                {
                    long lastSendGiftTimestamp = friendList[i].GetValue<long>("lastSendGiftTimestamp");
                    int sendGiftTimeInterval = BlackboardUtils.FindVariable<int>(null, "/values/misc/FRIEND_GIFT_SEND_INTERVAL_SEC").value;

                    if (BagelCode.TimeUtils.GetTimeStamp() - lastSendGiftTimestamp > (long)sendGiftTimeInterval * 1000)
                    {
                        if (sendColumnIndex == 0)
                        {
                            sendableFriendList.Add(new List<Blackboard>());
                        }

                        sendableFriendList[sendIndex].Add(friendList[i]);
                        sendColumnIndex++;
                    }

                    if (sendColumnIndex >= columnCount)
                    {
                        sendColumnIndex = 0;
                        sendIndex++;
                    }

                    int giftCount = friendList[i].GetValue<int>("receivedGiftCount");
                    if (giftCount > 0)
                    {
                        if (collectColumnIndex == 0)
                        {
                            collectableFriendList.Add(new List<Blackboard>());
                        }

                        collectableFriendList[collectIndex].Add(friendList[i]);
                        collectColumnIndex++;

                        if (collectColumnIndex >= columnCount)
                        {
                            collectColumnIndex = 0;
                            collectIndex++;
                        }
                    }
                }
            }

            isInitialize = true;
        }

        public void Clear()
        {
            while (content.childCount > 0)
            {
                while (content.GetChild(0).childCount > 0)
                {
                    content.GetChild(0).GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                }
                content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
            }

            frontIndex = -1;
            backIndex = 0;
            //backPageIndex   = -1;
            //friendCount     = 0;
            // columnCount     = 0;
        }

        public override void OnInitialize()
        {
            var grid = friendsListPool.GetObject();
            columnCount = LayoutUtils.CalcGridLayoutGroupMaxColumn(content, grid.GetComponent<GridLayoutGroup>());
            grid.ReturnToPool();
            //RefreshCells();

            // dynamicScrollRect.horizontalNormalizedPosition = 0f;
            // dynamicScrollRect.verticalNormalizedPosition = 1f;       
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

        private bool InitCellItem(Blackboard info, Transform parent)
        {
            var cell = friendsCellPool.GetObject();
            cell.transform.SetParent(parent, false);
            cell.gameObject.SetActive(false);

            cell.gameObject.GetComponent<IContextBooleanProperty>().SetBooleanProperty(false);

            Blackboard cellBB = cell.transform.GetComponent<Blackboard>();

            cellBB.RemoveVariable("cellInfo");
            BlackboardUtils.SetOrCreateValue<Blackboard>(cellBB, "cellInfo", info);

            var friendType = info.GetValue<FriendType>("type");
            if (friendType == FriendType.FACEBOOK)
            {
                BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)DynamicScrollFriendCreator.FriendCellType.Facebook);
                BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellBadgeType", (int)DynamicScrollFriendCreator.BadgeType.Facebook);

            }
            // else if (friendType == FriendType.GOOGLE)
            // {
            //     // todo: GP icon
            //     BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)DynamicScrollFriendCreator.FriendCellType.Normal);
            // }
            else
            {
                var myClubID = BlackboardUtils.FindVariable<long>(null, "/clubId");
                var clubID = info.GetValue<long>("clubId");

                BlackboardUtils.SetOrCreateValue<int>(cellBB, "cellType", (int)DynamicScrollFriendCreator.FriendCellType.Normal);

                if (myClubID.value > 0 && myClubID.value == clubID)
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

            return true;

        }

        protected override bool PushFront()
        {
            if (!isInitialize) return false;
            if (frontIndex < 0) return false;

            var list = friendsListPool.GetObject();
            list.transform.SetParent(content, false);
            list.transform.SetAsFirstSibling();

            if (UIMode == 0)
            {
                for (int i = 0; i < collectableFriendList[frontIndex].Count; i++)
                {
                    InitCellItem(collectableFriendList[frontIndex][i], list.transform);
                }

                frontIndex--;

                return true;
            }
            else
            {
                for (int i = 0; i < sendableFriendList[frontIndex].Count; i++)
                {
                    InitCellItem(sendableFriendList[frontIndex][i], list.transform);
                }

                frontIndex--;

                return true;
            }
        }

        protected override bool PushBack()
        {
            if (!isInitialize) return false;
            if (UIMode == 0)
            {
                if (backIndex >= collectableFriendList.Count)
                {
                    return false;
                }
                else
                {
                    var list = friendsListPool.GetObject();
                    list.transform.SetParent(content, false);
                    list.transform.SetAsLastSibling();

                    for (int i = 0; i < collectableFriendList[backIndex].Count; i++)
                    {
                        InitCellItem(collectableFriendList[backIndex][i], list.transform);
                    }

                    backIndex++;
                }

                return true;
            }
            else
            {
                if (backIndex >= sendableFriendList.Count)
                {
                    return false;
                }
                else
                {
                    var list = friendsListPool.GetObject();
                    list.transform.SetParent(content, false);
                    list.transform.SetAsLastSibling();

                    for (int i = 0; i < sendableFriendList[backIndex].Count; i++)
                    {
                        InitCellItem(sendableFriendList[backIndex][i], list.transform);
                    }

                    backIndex++;
                }

                return true;
            }
        }

        protected override bool PopFront()
        {
            var list = content.GetChild(0);

            while (list.childCount > 0)
            {
                list.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                //frontIndex++;
            }

            list.GetComponent<PooledObject>().ReturnToPool();
            frontIndex++;

            return true;
        }

        protected override bool PopBack()
        {
            var list = content.GetChild(content.childCount - 1);

            while (list.childCount > 0)
            {
                list.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                //backIndex--;
            }

            list.GetComponent<PooledObject>().ReturnToPool();
            backIndex--;

            return true;
        }

        public void RemoveItem(string userId)
        {
            // todo : 
            Refresh();
        }

        public void Refresh()
        {
            GameObject caller = GetComponent<Blackboard>().GetValue<GameObject>("caller");

            InteractableTrigger(true);

            dynamicScrollRect.enabled = false;
            Clear();
            RefreshCells();
            dynamicScrollRect.enabled = true;

            // caller.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>().SendEvent("RefreshTap");
            caller.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>().SendEvent("OnFinishCollectSendLoading");
        }

        public List<GameObject> GetGameObjectList()
        {
            objectList = new List<GameObject>();

            // float contentY = content.transform.position.y;

            for (int i = 0; i < content.childCount; i++)
            {
                for (int j = 0; j < content.GetChild(i).childCount; j++)
                {
                    float positionY = -(content.GetChild(i).GetChild(j).transform.position.y);

                    if (-3f < positionY && positionY < 5f)
                    {
                        objectList.Add(content.GetChild(i).GetChild(j).gameObject);
                    }
                }
            }
            return objectList;
        }

        public void InteractableTrigger(bool trigger)
        {
            gameObject.GetComponent<DynamicScrollRect>().interactable = trigger;
        }

    }
}
