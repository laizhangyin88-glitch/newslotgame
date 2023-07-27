using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{

    public class DynamicScrollClubMemberCreator : DynamicScrollItemCreator
    {
        public List<Blackboard> clubMemberList = null;
        public ObjectPool objectPool;
        public GameObject caller;
        public Blackboard agent;

        public long clubID;
        public ClubAuthority myAuthority;
        public string cellFromType;

        private bool enableShareItem;

        private int clubLevel = 1;
        private int rankOffset = 0;

        private const string ON_META_UI_EVENT = "OnMetaUIEvent";

        private const string ON_REMOVE_CLUB_MEMBER = "OnRemoveClubMember";

        private void Start()
        {
            MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnDestroy()
        {
            MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
        }

        public void InitList(bool enableShare)
        {
            enableShareItem = enableShare;
        }

        public void RefreshList(string fromType, bool showLeaderMenu)
        {
            if (agent == null) agent = gameObject.GetComponent<Blackboard>();

            RemoveAll();

            clubMemberList = new List<Blackboard>();
            myAuthority = ClubAuthority.UNKNOWN;
            cellFromType = fromType;

            var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "clubInfoResponse/clubMemberList");

            clubLevel = BlackboardUtils.FindVariable<int>(agent, "clubInfoResponse/clubInfo/level").value;

            if (variable != null && variable.value.Count > 0)
            {
                clubID = BlackboardUtils.FindVariable<long>(agent, "clubInfoResponse/clubInfo/id").value;

                if (showLeaderMenu)
                {
                    var myClubInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "clubInfoResponse/myClubInfo");
                    if (myClubInfo != null)
                        myAuthority = myClubInfo.value.GetValue<ClubAuthority>("authority");
                }

                clubMemberList = variable.value;
                for (int i = 0; i < clubMemberList.Count; ++i)
                {
                    if (clubMemberList[i].GetValue<ClubAuthority>("authority") == ClubAuthority.LEADER)
                        break;
                    rankOffset = i;
                }

                for (int i = 0; i < maxBufferingCount; ++i)
                {
                    if (!PushBack())
                        break;
                }
            }

            RebuildContentBounds();
            dynamicScrollRect.verticalNormalizedPosition = 1f;
        }

        public override void OnInitialize()
        {

        }

        private void InitSlotItem(int index, Blackboard bb)
        {
            if (!clubMemberList.IsValidIndex(index))
            {
                Debug.LogWarning("InitSlotItem failure. " + index + " is invalid index with 'clubMemberList'");
                return;
            }

            bb.SetValue("memberInfo", clubMemberList[index]);
            bb.SetValue("caller", caller);
            bb.SetValue("cellIndex", index);
            bb.SetValue("rankOffset", rankOffset);
            bb.SetValue("myAuthority", myAuthority);
            bb.SetValue("fromType", cellFromType);
            bb.SetValue("clubID", clubID);
            bb.SetValue("clubLevel", clubLevel);
            bb.SetValue("enableShareItem", enableShareItem);
        }

        private GameObject PushFront(ObjectPool pool)
        {
            var go = pool.GetObject(false).gameObject;
            go.transform.SetParent(content, false);
            go.transform.SetAsFirstSibling();
            return go;
        }

        protected override bool PushFront()
        {
            if (frontIndex < 0)
                return false;

            var go = PushFront(objectPool);
            InitSlotItem(frontIndex, go.GetComponent<Blackboard>());
            go.SetActive(true);
            frontIndex -= 1;
            return true;
        }

        private GameObject PushBack(ObjectPool pool)
        {
            var go = pool.GetObject(false).gameObject;
            go.transform.SetParent(content, false);
            go.transform.SetAsLastSibling();
            return go;
        }

        protected override bool PushBack()
        {
            if (backIndex >= clubMemberList.Count)
                return false;

            var go = PushBack(objectPool);
            InitSlotItem(backIndex, go.GetComponent<Blackboard>());
            go.SetActive(true);
            backIndex += 1;
            return true;
        }

        protected override bool PopFront()
        {
            frontIndex += 1;
            content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();

            return true;
        }

        protected override bool PopBack()
        {
            backIndex -= 1;
            content.GetChild(content.childCount - 1).GetComponent<PooledObject>().ReturnToPool();

            return true;
        }

        protected void RemoveAll()
        {
            while (content.childCount > 0)
            {
                content.GetChild(0).GetComponent<PooledObject>().ReturnToPool();
            }

            frontIndex = -1;
            backIndex = 0;
            rankOffset = 0;
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (eventData.name == ON_REMOVE_CLUB_MEMBER)
            {
                if (eventData.value != null && eventData.value is string)
                {
                    RemoveClubMember((string)eventData.value);
                }
            }
        }

        private void RemoveClubMember(string removeUserID)
        {
            if (clubMemberList == null) return;

            int removeIndex = -1;

            for (int i = 0; i < clubMemberList.Count; ++i)
            {
                var userID = BlackboardUtils.FindVariable<string>(clubMemberList[i], "userId");

                if (userID.value == removeUserID)
                {
                    GameObject.Destroy(clubMemberList[i].gameObject);

                    clubMemberList.RemoveAt(i);
                    removeIndex = i;
                    break;
                }
            }

            if (removeIndex != -1)
            {
                RemoveAt(removeIndex);
            }
        }

        protected void RemoveAt(int index)
        {
            if (index > backIndex) return;
            if (index < frontIndex)
            {
                frontIndex -= 1;
                backIndex -= 1;
                return;
            }

            Transform child = content.GetChild(index - (frontIndex + 1));
            child.GetComponent<PooledObject>().ReturnToPool();

            backIndex -= 1;

            PushBack();
        }
    }
}
