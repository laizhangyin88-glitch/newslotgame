using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Vip Deal")]
    public class VipDealSelectFreeDirecting : ActionTask<Blackboard>
    {
        public BBParameter<List<GameObject>> selectItemList;
        public BBParameter<int> targetIndex;

        public BBParameter<float> totalTimeSec;
        public BBParameter<float> fixDelaySec;
        public BBParameter<float> startDelaySec;
        public BBParameter<float> endDelaySec;

        private const string ANI_STATE_NAME = "FreeSelectOn";

        private List<Animator> selectItemsAniList = null;
        private int selectTargetIndex = -1;
        private Animator prevAnimator = null;
        private int currentIndex = -1;
        private float waitDelta = 0f;

        protected override string info
        {
            get { return "Directing select items."; }
        }

        protected override void OnExecute()
        {
            List<GameObject> sortedSelectItems = new List<GameObject>();
            sortedSelectItems.AddRange(selectItemList.value);
            sortedSelectItems.Sort(
                        delegate (GameObject source, GameObject dest)
                        {
                            var sourceIndex = source.GetComponent<Blackboard>().GetValue<int>("index");
                            var destIndex = dest.GetComponent<Blackboard>().GetValue<int>("index");

                            return sourceIndex.CompareTo(destIndex);
                        }
                    );

            selectItemsAniList = new List<Animator>();
            for(int i=0; i<sortedSelectItems.Count; ++i)
            {
                if(selectItemList.value[targetIndex.value] == sortedSelectItems[i])
                    selectTargetIndex = i;

                selectItemsAniList.Add( sortedSelectItems[i].GetComponent<Animator>());
            }

            currentIndex = 0;
        }

        protected override void OnUpdate()
        {
            if(currentIndex > -1)
            {
                waitDelta -= Time.deltaTime;

                if( waitDelta <= 0f)
                {
                    if(prevAnimator != null) prevAnimator.SetBool(ANI_STATE_NAME, false);
                    prevAnimator = selectItemsAniList[currentIndex];
                    prevAnimator.SetBool(ANI_STATE_NAME, true);

                    if(elapsedTime > totalTimeSec.value && currentIndex == selectTargetIndex)
                    {
                        EndAction();
                    }

                    ++currentIndex;
                    currentIndex = currentIndex%selectItemsAniList.Count;

                    if(elapsedTime < fixDelaySec.value)
                    {
                        waitDelta = startDelaySec.value;
                    }
                    else
                    {
                        waitDelta = Mathf.Lerp(startDelaySec.value, endDelaySec.value, elapsedTime/(totalTimeSec.value - fixDelaySec.value));
                    }
                    
                }
            }
        }
    }
}
