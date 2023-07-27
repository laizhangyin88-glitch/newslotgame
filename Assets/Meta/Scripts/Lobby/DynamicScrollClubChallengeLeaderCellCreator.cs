using UnityEngine;
using System.Collections.Generic;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class DynamicScrollClubChallengeLeaderCellCreator : DynamicScrollItemCreator
    {
        public List<Blackboard> contributionInfoList = null;
        public ObjectPool objectPool;

        private const string ON_META_UI_EVENT = "OnMetaUIEvent";
        private const string ON_REFRESH_LIST = "OnRefreshList";

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
        }

        public void OnMetaUIEvent(EventData eventData)
        {
            if (eventData.name == ON_REFRESH_LIST)
            {
                RefreshList(eventData.value as Transform);
            }
        }

        public void RefreshList(Transform agent)
        {
            if (agent == null) return;

            var agentBB = agent.GetComponent<Blackboard>();
            if (agentBB == null) return;

            contributionInfoList = new List<Blackboard>();

            RemoveAll();

            var missionResponse = BlackboardUtils.FindVariable<Blackboard>(agentBB, "clubMissionResponse");
            if (missionResponse == null) return;

            var contributionList = BlackboardUtils.FindVariable<List<Blackboard>>(missionResponse.value, "missionContributionList");

            if (contributionList != null && contributionList.value.Count > 0)
            {
                contributionInfoList.AddRange(contributionList.value);

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
            bb.SetValue("contributionInfo", contributionInfoList[index]);
            bb.SetValue("cellIndex", index);
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
            if (backIndex == contributionInfoList.Count)
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
        }
    }
}