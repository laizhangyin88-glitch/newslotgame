using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using System.Linq;

namespace BagelCode
{
    public class DynamicScrollOnlinePlayersCreator : DynamicScrollItemCreator
    {
        private enum CelebrityBadgeType
        {
            None = 0,
            Facebook,
            Friend,
            Club
        }

        public ObjectPool listPool;
        public ObjectPool cellPool;

        public List<Blackboard> celebInfoList = null;
        public List<Blackboard> onlineFriendList = null;
        public List<Blackboard> onlineClubMemberList = null;
        public List<Blackboard> totalCelebrityList = null;

        public int totalCelebCount = 0;
        public int backEndIndex = 0;

        private bool isConnectedFB = false;
        private bool createdBannerObject = false;

        private int RegionCount = 5;

        private void SetUpData()
        {
            onlineFriendList = BlackboardQueryUtils.GetMyOnlineFriendList();
            onlineClubMemberList = BlackboardQueryUtils.GetOnlineClubMemberList();
            celebInfoList = BlackboardQueryUtils.GetCelebrityList();
            isConnectedFB = AccountUtils.IsFacebookConnected();

            for (int i = 0; i < onlineFriendList.Count; ++i)
            {
                var friendType = onlineFriendList[i].GetValue<BagelCode.ClientModels.FriendType>("type");

                switch (friendType)
                {
                    case BagelCode.ClientModels.FriendType.FACEBOOK:
                        onlineFriendList[i].AddVariable("badgeType", (int)CelebrityBadgeType.Facebook);
                        break;
                    default:
                        onlineFriendList[i].AddVariable("badgeType", (int)CelebrityBadgeType.Friend);
                        break;
                }
            }

            for (int i = 0; i < onlineClubMemberList.Count; ++i)
            {
                onlineClubMemberList[i].AddVariable("badgeType", (int)CelebrityBadgeType.Club);
            }

            for (int i = 0; i < celebInfoList.Count; ++i)
            {
                celebInfoList[i].AddVariable("badgeType", (int)CelebrityBadgeType.None);
            }

            frontIndex = -1;
            backIndex = 0;

            totalCelebrityList = new List<Blackboard>();
            totalCelebrityList.AddRange(onlineFriendList);
            totalCelebrityList.AddRange(onlineClubMemberList);
            totalCelebrityList.AddRange(celebInfoList);

            // remove blocked users
            var blockedUserIDList = BlackboardQueryUtils.GetBlockedUserIDList();
            totalCelebrityList.RemoveAll(c => blockedUserIDList.Contains(c.GetValue<string>("userId")));

            totalCelebCount = totalCelebrityList.Count;
            int remainCount = totalCelebCount % RegionCount;

            backEndIndex = totalCelebCount / RegionCount + (remainCount > 0 ? 1 : 0);

            for (int i = 0; i < maxBufferingCount; ++i)
            {
                if (!PushBack())
                    break;
            }

            RebuildContentBounds();
            dynamicScrollRect.verticalNormalizedPosition = 1f;
        }

        public void Clear()
        {
            while (content.childCount > 0)
            {
                var list = content.GetChild(0);

                if (list.name.Equals("Celebrity List VIP Club"))
                {
                    list.GetComponent<PooledObject>().ReturnToPool();

                }
                else
                {
                    while (list.childCount > 0)
                    {
                        list.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
                    }
                    list.GetComponent<PooledObject>().ReturnToPool();
                }
            }

            celebInfoList = null;
            onlineFriendList = null;
            onlineClubMemberList = null;
            totalCelebrityList.Clear();

            createdBannerObject = false;
            frontIndex = -1;
            backIndex = 0;
            isConnectedFB = false;
        }

        public void UpdateList()
        {
            Refresh();
        }

        public override void OnInitialize()
        {
            SetUpData();
        }

        private Blackboard GetCellInfoBB(int index)
        {
            if (index < 0) return null;

            if (index < totalCelebrityList.Count)
                return totalCelebrityList[index];

            return null;
        }

        private void InitCellItem(Transform parent, Blackboard info)
        {
            var cell = cellPool.GetObject(false);
            cell.transform.SetParent(parent, false);

            var bb = cell.transform.GetComponent<Blackboard>();

            if (bb != null)
            {
                bb.SetValue("cellInfo", info);
                var badgeList = bb.GetValue<List<GameObject>>("badgeList");

                var badgeType = info.GetValue<int>("badgeType");
                for (int i = 0; i < badgeList.Count; ++i)
                {
                    if (badgeType == i + 1)
                        badgeList[i].SetActive(true);
                    else
                        badgeList[i].SetActive(false);
                }
            }

            cell.gameObject.SetActive(true);
        }

        private bool PushFront(Transform parent)
        {
            int insertCount = 0;

            int pivotIndex = frontIndex * RegionCount;

            while (insertCount < RegionCount)
            {
                Blackboard currentCellInfo = GetCellInfoBB(pivotIndex + insertCount);
                if (currentCellInfo != null)
                {
                    ++insertCount;
                    InitCellItem(parent, currentCellInfo);
                }
                else
                {
                    break;
                }
            }

            --frontIndex;
            return true;
        }

        protected override bool PushFront()
        {
            if (frontIndex < 0) return false;

            var go = listPool.GetObject().gameObject;
            go.transform.SetParent(content, false);
            go.transform.SetAsFirstSibling();

            return PushFront(go.transform);
        }

        private bool PushBack(Transform parent)
        {
            int insertCount = 0;

            int pivotIndex = backIndex * RegionCount;

            while (insertCount < RegionCount)
            {
                Blackboard currentCellInfo = GetCellInfoBB(pivotIndex + insertCount);

                if (currentCellInfo != null)
                {
                    ++insertCount;
                    InitCellItem(parent, currentCellInfo);
                }
                else
                {
                    break;
                }
            }

            ++backIndex;
            return true;
        }

        protected override bool PushBack()
        {
            if (backIndex >= backEndIndex) return false;

            var go = listPool.GetObject().gameObject;
            go.transform.SetParent(content, false);
            go.transform.SetAsLastSibling();

            return PushBack(go.transform);
        }

        protected override bool PopFront()
        {
            frontIndex++;

            var list = content.GetChild(0);

            while (list.childCount > 0)
            {
                list.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
            }

            list.GetComponent<PooledObject>().ReturnToPool();

            return true;
        }

        protected override bool PopBack()
        {
            backIndex--;

            var list = content.GetChild(content.childCount - 1);
            while (list.childCount > 0)
            {
                list.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
            }

            list.GetComponent<PooledObject>().ReturnToPool();

            return true;
        }

        public void Refresh()
        {
            dynamicScrollRect.enabled = false;
            Clear();
            SetUpData();
            dynamicScrollRect.enabled = true;
        }
    }
}
