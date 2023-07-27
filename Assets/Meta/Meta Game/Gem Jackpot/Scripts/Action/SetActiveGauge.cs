using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.GemJackpot;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/GemJackpot")]
    public class SetActiveGauge : ActionTask<Blackboard>
    {
        public BBParameter<List<ContextElement>> goldElementList;
        public BBParameter<List<GemJackpotWinRewardObjectController>> rewardControllerList;
        public BBParameter<float> delayTime;

        protected override string info
        {
            get
            {
                return string.Format("Set GemJackpot Active Gauge({0})", delayTime.value);
            }
        }

        protected override void OnExecute()
        {
            StartCoroutine(CheckActiveGauge());
        }

        protected IEnumerator CheckActiveGauge()
        {
            if (rewardControllerList != null && rewardControllerList.value != null && goldElementList != null && goldElementList.value != null)
            {
                int nextProgress = GemJackpotUtils.NextProgress;
                int prevPregress = GemJackpotUtils.PrevProgress;
                prevPregress = prevPregress - (prevPregress % 3);

                for (int i = prevPregress; i < nextProgress; ++i)
                {
                    if (i < goldElementList.value.Count)
                    {
                        if (!goldElementList.value[i].gameObject.activeSelf)
                        {
                            goldElementList.value[i].gameObject.SetActive(true);
                            GSManager.Instance.GetHandler("Meta_Gemjackpot_Gageup").Play();
                        }
                        if (i % 3 == 0)
                        {
                            if (i < nextProgress)
                            {
                                int rewardIndex = i / 3;

                                if (!rewardControllerList.value[rewardIndex].isActiveSelf)
                                {
                                    rewardControllerList.value[rewardIndex].SetWinRewardActive(true);
                                    if (rewardIndex > 0)
                                    {
                                        rewardControllerList.value[rewardIndex - 1].SetWinRewardActive(false);
                                    }
                                }
                            }
                        }
                        yield return new WaitForSeconds(delayTime.value);
                    }
                    else if (i == goldElementList.value.Count)
                    {
                        rewardControllerList.value[rewardControllerList.value.Count - 1].SetWinRewardActive(false);
                    }
                    else
                        break;
                }
            }
            EndAction();
        }
    }
}