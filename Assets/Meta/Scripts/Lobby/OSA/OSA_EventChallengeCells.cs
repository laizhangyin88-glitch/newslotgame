using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using Com.ForbiddenByte.OSA.Core;

namespace BagelCode.OSA_Scroll
{
    public class OSA_EventChallengeCells : OSA<EventChallengeCellParams, EventChallengeCellViewHolder>
    {
        private GameObject caller;

        public void CreateItemList(GameObject _caller, List<Blackboard> missionList, bool isCompleteToUnlock, bool isPersonal)
        {
            caller = _caller;

            bool setUnlock = false;
            bool setArrow = false;
            int pos = 0;
            missionList.ForEach(m =>
            {
                bool isDone = m.GetValue<bool>("done");
                if (isDone) pos++;

                if(setUnlock && !setArrow)
                {
                    m.AddVariable("isArrowEnable", true);
                    setArrow = true;
                }

                if (isCompleteToUnlock && !isDone && !setUnlock)
                {
                    m.AddVariable("isUnlock", true);
                    setUnlock = true;
                }
            });

            _Params.isPersonal = isPersonal;
            _Params.isCompleteToUnlock = isCompleteToUnlock;
            _Params.data.Clear();
            _Params.data.AddRange(missionList);

            // Center Alignment
            if (missionList.Count == 3)
            {
                _Params.SetScrollEnabled(false);
                _Params.data.Add(null);
                _Params.data.Insert(0, null);
                ResetItems(5);
                ScrollTo(2, 0.5f, 0.5f);
                return;
            }

            ResetItems(missionList.Count);

            if (pos > 0)
            {
                if (isCompleteToUnlock)
                {
                    ScrollTo(pos, 0.5f, 0.5f);
                }
                else
                {
                    ScrollTo(pos, 0f, -0.25f);
                }
            }
            else
            {
                ScrollTo(0);
            }
        }

        protected override EventChallengeCellViewHolder CreateViewsHolder(int itemIndex)
        {
            var viewHolder = new EventChallengeCellViewHolder();
            viewHolder.Init(_Params.prefab, _Params.Content, itemIndex);

            return viewHolder;
        }

        protected override void UpdateViewsHolder(EventChallengeCellViewHolder newOrRecycled)
        {
            int index = newOrRecycled.ItemIndex;
            newOrRecycled.UpdateView(caller, _Params.data[index], _Params.isPersonal, _Params.isCompleteToUnlock);
        }

        protected override bool IsRecyclable(EventChallengeCellViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return true;
        }
    }

    [Serializable]
    public class EventChallengeCellParams : BaseParams
    {
        public List<Blackboard> data = new List<Blackboard>();
        public GameObject prefab = null;
        public bool isPersonal = true;
        public bool isCompleteToUnlock = false;

        public void SetScrollEnabled(bool isEnable)
        {
            ScrollEnabled = isEnable;
            DragEnabled = isEnable;
        }
    }

    [Serializable]
    public class EventChallengeCellViewHolder : BaseItemViewsHolder
    {
        private ChallengeEventMissionCellController controller;

        public void UpdateView(GameObject caller, Blackboard missionInfo, bool isPersonal, bool isCompleteToUnlock)
        {
            if (missionInfo != null)
            {
                Blackboard cellBB = controller.GetComponent<Blackboard>();
                cellBB.AddVariable("caller", caller);

                controller.isPersonal = isPersonal;
                controller.isCompleteToUnlock = isCompleteToUnlock;
                controller.isDummy = false;
            }
            else
            {
                controller.isDummy = true;
            }

            controller.UpdateView(missionInfo);
            controller.gameObject.SetActive(true);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<ChallengeEventMissionCellController>();
        }
    }
}
