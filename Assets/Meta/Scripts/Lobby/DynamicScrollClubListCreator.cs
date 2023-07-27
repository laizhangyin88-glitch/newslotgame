using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public class DynamicScrollClubListCreator : DynamicScrollItemCreator
    {
        public List<Blackboard> clubList = null;
        public ObjectPool objectPool;
        public GameObject caller;
        public Blackboard agent;

        public GameObject emptyInvitesObj;

        public string biListType;
        public string biContextID;
        public string biOpenType;

        private int tabState = 0;

        public void SetInviteList()
        {
            tabState = 1;
            biListType = "invites";
            biContextID = "";
            biOpenType = "";
            clubList = new List<Blackboard>();
            RemoveAll();

            var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "invitedClubList");

            if (variable != null && variable.value.Count > 0)
            {
                emptyInvitesObj.SetActive(false);
                clubList.AddRange(variable.value);

                for (int i = 0; i < maxBufferingCount; ++i)
                {
                    if (!PushBack())
                        break;
                }
            }
            else
            {
                emptyInvitesObj.SetActive(true);
            }

            RebuildContentBounds();
            dynamicScrollRect.verticalNormalizedPosition = 1f;
        }

        public void SetJoinClubList(string contextID, string openType)
        {
            tabState = 0;
            biListType = "join_club";
            biContextID = contextID;
            biOpenType = openType;
            clubList = new List<Blackboard>();
            RemoveAll();

            var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "response/clubList");

            if (variable != null && variable.value.Count > 0)
            {
                emptyInvitesObj.SetActive(false);

                clubList.AddRange(variable.value);

                for (int i = 0; i < maxBufferingCount; ++i)
                {
                    if (!PushBack())
                        break;
                }
            }
            else
            {
                emptyInvitesObj.SetActive(true);
            }

            RebuildContentBounds();
            dynamicScrollRect.verticalNormalizedPosition = 1f;
        }

        public override void OnInitialize()
        {
        }

        private void InitSlotItem(int index, Blackboard bb)
        {
            if (clubList.IsValidIndex(index))
            {
                BlackboardUtils.SetOrCreateValue(bb, "clubInfo", clubList[index]);
            }
            else
            {
                Debug.LogError(string.Format("DynamicScrollClubListCreator.InitSlotItem failure. {0} is invalid index in clubList.", index));
            }

            BlackboardUtils.SetOrCreateValue(bb, "caller", caller);
            BlackboardUtils.SetOrCreateValue(bb, "cellType", tabState);
            BlackboardUtils.SetOrCreateValue(bb, "cellIndex", index);
            BlackboardUtils.SetOrCreateValue(bb, "biListType", biListType);
            BlackboardUtils.SetOrCreateValue(bb, "biContextID", biContextID);
            BlackboardUtils.SetOrCreateValue(bb, "biOpenType", biOpenType);
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
            if (backIndex >= clubList.Count)
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