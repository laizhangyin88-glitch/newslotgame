using System;
using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot;
using ParadoxNotion;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using Random = UnityEngine.Random;

namespace GameStudio.Slot.CTC
{
    public class CTCPotController : FeatureModule
    {
        [SerializeField] private List<Animator> basePotList;
        [SerializeField] private List<GameObject> rocketListParentList;
        [SerializeField] private List<Animator> bonusPotList;

        [SerializeField] private ObjectPool rocketFlyingPool;
        [SerializeField] private float waitDelayPerRocket = 0.5f;
        [SerializeField] private float initialWaitDelayPerRocketAtTrigger = 0.25f;
        [SerializeField] private float minimumWaitDelayPerRocketAtTrigger = 0.1f;
        [SerializeField] private float waitDelayAccelationAtTrigger = 0.05f;
        [SerializeField] private float fireWorkSoundPlayTime = 0.35f;
        [SerializeField] private float fireCrackSoundPlayTime = 0.35f;

        [SerializeField] private float scatterStopEffectTime = 0.6f;
        [SerializeField] private float rocketFlyingTime = 1.05f;
        [SerializeField] private float rocketFlyingAnimTotalTime = 2f;
        [SerializeField] private float collectAnimWaitDelay = 1.08f;
        [SerializeField] private int maxFakeIgnitingCount = 3;
        [SerializeField] private int flyingDirectionDecisionStandardCol = 3;

        private bool hasInited = false;

        private Dictionary<long, List<int>> ignitedRocketCountPerPotPerBet;
        private List<List<Animator>> rocketListPerPot;
        private Variable<int> currentFiringRocketVar;
        private List<Coroutine> fireCrackerSoundIndexInitRoutineList;
        private List<Coroutine> fireWorkSoundIndexInitRoutineList;

        private int currentFireCrackerPlayIndex = -1;
        private int currentFireWorkPlayIndex = -1;

        public const string INIT_POTS_EVENT = "InitPots";
        public const string REFRESH_POT_STATUS_EVENT = "RefreshPotStatus";
        public const string COLLECT_POT_EVENT = "CollectPot";
        public const string COLLECT_POT_DONE_EVENT = "CollectPotDone";
        public const string START_TRIGGERED_POT_ANIM_EVENT = "StartTriggeredPotAnim";

        public const string ON_CREDIT_EVENT = "OnCreditEvent";
        public const string UPDATE_TOTAL_BET_EVENT = "UpdatedTotalBetCredit";

        public const int PAYUP_SYMBOL_INDEX = 11;
        public const int ROCKET_FIRE_ANIM_LAYER = 1;

        protected override void OnEnable()
        {
            hasInited = false;
            base.OnEnable();
            fireCrackerSoundIndexInitRoutineList = new List<Coroutine>();
            fireWorkSoundIndexInitRoutineList = new List<Coroutine>();
            RegisterEvent(INIT_POTS_EVENT, (eventData) =>
                {
                    currentFireCrackerPlayIndex = -1;
                    currentFireWorkPlayIndex = -1;
                    currentFiringRocketVar = BlackboardUtils.GetOrCreateVariable<int>("./customData/currentFiringRocket");
                    hasInited = true;
                    rocketListPerPot = new List<List<Animator>>();
                    ignitedRocketCountPerPotPerBet = new Dictionary<long, List<int>>();
                    foreach (var parent in rocketListParentList)
                        rocketListPerPot.Add(new List<Animator>(parent.GetComponentsInChildren<Animator>()));

                    var rocketCountInfoList = BlackboardUtils.FindVariable<List<Blackboard>>("./game/rocketCountInfoList");
                    if (rocketCountInfoList == null) return;

                    foreach (var rocketCountInfoBB in rocketCountInfoList.value)
                    {
                        var betCredit = rocketCountInfoBB.GetValue<long>("betCredit");
                        var ignitedRocketCountPerPot = rocketCountInfoBB.GetValue<List<int>>("rocketCountPerPot");
                        ignitedRocketCountPerPotPerBet.Add(betCredit, ignitedRocketCountPerPot);

                        if (betCredit == BlackboardUtils.FindValue<long>(null, "./betCredit"))
                            for (int potIndex = 0; potIndex < basePotList.Count; potIndex++)
                            {
                                basePotList[potIndex].SetInteger("Collect Count", ignitedRocketCountPerPot[potIndex]);
                                for (int rocketIndex = 0; rocketIndex < rocketListPerPot[potIndex].Count; rocketIndex++)
                                    rocketListPerPot[potIndex][rocketIndex].SetInteger("Idle State", Random.Range(0, 3));

                                for (int rocketIndex = 0; rocketIndex < ignitedRocketCountPerPot[potIndex]; rocketIndex++)
                                {
                                    rocketListPerPot[potIndex][rocketIndex].SetBool("IsSuccess", true);
                                    rocketListPerPot[potIndex][rocketIndex].SetTrigger("Ignite");
                                }
                            }
                    }
                }
            );


            RegisterEvent(COLLECT_POT_EVENT, (data) =>
            {
            // prevent rocket logic flow skipped by adding firing count
            currentFiringRocketVar.value++;
                var flyingStartSymbol = BlackboardUtils.FindValue<BaseSymbol>(null, "./customData/_currentStoppingScatter");
                bool isINS = BlackboardUtils.FindVariable<Blackboard>(null, "./bonus") != null;
                if (isINS)
                    StartCoroutine(CollectPotProcessINS(flyingStartSymbol));
                else
                {
                    var newIgnitedRocketCountPerPot = BlackboardUtils.FindValue<List<int>>(null, "./spin/response/ignitedRocketCountPerPot");
                    StartCoroutine(CollectPotProcess(flyingStartSymbol, newIgnitedRocketCountPerPot, CTCPostSpinController.BonusGameResponseList));
                }
            });

            RegisterEvent(START_TRIGGERED_POT_ANIM_EVENT, (data) =>
            {
                GSManager.Instance.GetHandler("Scatter Win").Play();
                var triggeredFlagPerPot = CTCPostSpinController.BonusGameResponseList[0]
                    .GetValue<List<bool>>("flagPerBonusType");

                for (int i = 1; i < triggeredFlagPerPot.Count; i++)
                    if (triggeredFlagPerPot[i])
                    {
                        basePotList[i - 1].SetTrigger("Trigger");
                        bonusPotList[i - 1].SetBool("OnBonus", true);
                    }
            });


            RegisterEvent(REFRESH_POT_STATUS_EVENT, (data) =>
            {
                var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
                var igniteCountPerPot = ignitedRocketCountPerPotPerBet[betCredit];

                for (int potIndex = 0; potIndex < basePotList.Count; potIndex++)
                {
                    var igniteCount = igniteCountPerPot[potIndex];
                    basePotList[potIndex].SetInteger("Collect Count", igniteCount);
                    rocketListParentList[potIndex].gameObject.SetActive(false);
                    rocketListParentList[potIndex].gameObject.SetActive(true);

                    for (int rocketIndex = 0; rocketIndex < rocketListPerPot[potIndex].Count; rocketIndex++)
                        rocketListPerPot[potIndex][rocketIndex].SetInteger("Idle State", Random.Range(0, 3));

                    for (int i = 0; i < igniteCount; i++)
                    {
                        rocketListPerPot[potIndex][i].SetTrigger("Ignite");
                        rocketListPerPot[potIndex][i].SetBool("IsSuccess", true);
                    }

                    bonusPotList[potIndex].SetBool("OnBonus", false);
                }

            });

            MessageDispatcher.Register(ON_CREDIT_EVENT, HandleCreditEvent);
        }

        private void HandleCreditEvent(EventData eventData)
        {
            if (eventData.name == UPDATE_TOTAL_BET_EVENT)
            {
                if (hasInited == false) return;

                var betCredit = (long)eventData.value;
                if (ignitedRocketCountPerPotPerBet.ContainsKey(betCredit) == false)
                {
                    var newList = new List<int>();
                    foreach (var basePot in basePotList) newList.Add(0);
                    ignitedRocketCountPerPotPerBet.Add(betCredit, newList);
                }

                var ignitedCountPerPot = ignitedRocketCountPerPotPerBet[betCredit];
                for (int potIndex = 0; potIndex < basePotList.Count; potIndex++)
                {
                    var igniteCount = ignitedCountPerPot[potIndex];
                    basePotList[potIndex].SetInteger("Collect Count", igniteCount);
                    rocketListParentList[potIndex].gameObject.SetActive(false);
                    rocketListParentList[potIndex].gameObject.SetActive(true);

                    for (int rocketIndex = 0; rocketIndex < rocketListPerPot[potIndex].Count; rocketIndex++)
                        rocketListPerPot[potIndex][rocketIndex].SetInteger("Idle State", Random.Range(0, 3));

                    for (int i = 0; i < igniteCount; i++)
                    {
                        rocketListPerPot[potIndex][i].SetTrigger("Ignite");
                        rocketListPerPot[potIndex][i].SetBool("IsSuccess", true);
                    }
                }

            }
        }

        private IEnumerator CollectPotProcess(BaseSymbol flyingStartSymbol, List<int> newIgnitedRocketCountPerPot, List<Blackboard> bonusResponseList)
        {
            yield return new WaitForSeconds(scatterStopEffectTime);
            int potIndex = flyingStartSymbol.symbolIndex - PAYUP_SYMBOL_INDEX;
            var targetPot = basePotList[potIndex];
            flyingStartSymbol.Play("Win");

            var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
            if (ignitedRocketCountPerPotPerBet.ContainsKey(betCredit) == false)
            {
                var newList = new List<int>();
                foreach (var pot in basePotList) newList.Add(0);
                ignitedRocketCountPerPotPerBet.Add(betCredit, newList);
            }


            var thisSpinIgnitingRocketCountPerPot = new List<int>(ignitedRocketCountPerPotPerBet[betCredit]);
            for (int i = 0; i < basePotList.Count; i++)
            {
                ignitedRocketCountPerPotPerBet[betCredit][i] = newIgnitedRocketCountPerPot[i];
                thisSpinIgnitingRocketCountPerPot[i] = newIgnitedRocketCountPerPot[i] - thisSpinIgnitingRocketCountPerPot[i];
            }

            GSManager.Instance.GetHandler("Cash Symbol Fly").Play();
            var flyingObj = rocketFlyingPool.GetObject();
            var fromToController = flyingObj.GetComponentInChildren<DirectionalWeightPositionController>(true);
            GameObject destAnchorObj = new GameObject();
            destAnchorObj.transform.position = flyingStartSymbol.transform.position;
            fromToController.from = destAnchorObj.transform;
            fromToController.to = targetPot.transform;
            flyingObj.gameObject.SetActive(true);

            var flyingAnimator = flyingObj.GetComponentInChildren<Animator>();
            flyingAnimator.SetInteger("Symbol Index", flyingStartSymbol.symbolIndex);
            flyingAnimator.SetInteger("Reel Index", flyingStartSymbol.column);
            bool thisPotTriggered = bonusResponseList.Count > 0 ? bonusResponseList[0].GetValue<List<bool>>("flagPerBonusType")[potIndex + 1] : false;

            StartCoroutine(CTCUtill.CallActionAfterDelay(() => { flyingObj.ReturnToPool(); destAnchorObj.DestroyThis(); }, rocketFlyingAnimTotalTime));
            StartCoroutine(CTCUtill.CallActionAfterDelay(() => { targetPot.SetTrigger("Collect"); }, collectAnimWaitDelay));
            yield return new WaitForSeconds(rocketFlyingTime);
            GSManager.Instance.GetHandler("Cash Symbol Collect").Play();

            int realIgnitingCount = thisPotTriggered ? rocketListPerPot[potIndex].Count : newIgnitedRocketCountPerPot[potIndex];
            var ignitingWaitdelay = thisPotTriggered ? initialWaitDelayPerRocketAtTrigger : waitDelayPerRocket;
            float ignitedCount = 0;
            for (int rocketIndex = 0; rocketIndex < realIgnitingCount; rocketIndex++)
            {
                if (rocketListPerPot[potIndex][rocketIndex].GetBool("IsSuccess")) continue;

                if (currentFireCrackerPlayIndex < rocketIndex)
                {
                    foreach (var coroutine in fireCrackerSoundIndexInitRoutineList) StopCoroutine(coroutine);

                    GSManager.Instance.GetHandler(string.Format("Fire Cracker {0}", rocketIndex + 1)).Play();
                    GSManager.Instance.GetHandler("Fire Work").Play();
                    currentFireCrackerPlayIndex = rocketIndex;
                    var newCoroutine = StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                    {
                        currentFireCrackerPlayIndex = -1;
                    }, fireCrackSoundPlayTime));
                    fireCrackerSoundIndexInitRoutineList.Add(newCoroutine);
                }

                if (currentFireWorkPlayIndex < rocketIndex)
                {
                    foreach (var coroutine in fireWorkSoundIndexInitRoutineList) StopCoroutine(coroutine);

                    GSManager.Instance.GetHandler("Fire Work").Play();
                    currentFireWorkPlayIndex = rocketIndex;
                    var newCoroutine = StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                    {
                        currentFireWorkPlayIndex = -1;
                    }, fireWorkSoundPlayTime));
                    fireWorkSoundIndexInitRoutineList.Add(newCoroutine);
                }

                rocketListPerPot[potIndex][rocketIndex].SetTrigger("Shake");
                rocketListPerPot[potIndex][rocketIndex].SetTrigger("Ignite");
                rocketListPerPot[potIndex][rocketIndex].SetBool("IsSuccess", true);

                currentFiringRocketVar.value++;
                float delay = Math.Max(minimumWaitDelayPerRocketAtTrigger, initialWaitDelayPerRocketAtTrigger - waitDelayAccelationAtTrigger * ignitedCount);
                ignitedCount++;
                yield return new WaitForSeconds(delay);

                basePotList[potIndex].SetInteger("Collect Count", rocketIndex);
                currentFiringRocketVar.value--;
            }

            int remainRocketCount = rocketListPerPot[potIndex].Count - realIgnitingCount;
            int maxPossibleFailRocketCount = Math.Min(maxFakeIgnitingCount - thisSpinIgnitingRocketCountPerPot[potIndex], remainRocketCount);
            int failedIgnitingCount = maxPossibleFailRocketCount < 1 ? 0 : Random.Range(1,
               maxPossibleFailRocketCount + 1);
            int ignitedRocketCount = 0;
            var waitTimePerRocket = new WaitForSeconds(ignitingWaitdelay);
            for (int rocketIndex = 0; ignitedRocketCount < failedIgnitingCount && rocketIndex < rocketListPerPot[potIndex].Count; rocketIndex++)
            {
                if (rocketListPerPot[potIndex][rocketIndex].GetBool("IsSuccess")/* || rocketListPerPot[potIndex][rocketIndex].GetCurrentAnimatorStateInfo(ROCKET_FIRE_ANIM_LAYER).IsName("Fire Off Idle") == false */) continue;

                if (currentFireWorkPlayIndex < rocketIndex)
                {
                    foreach (var coroutine in fireWorkSoundIndexInitRoutineList) StopCoroutine(coroutine);

                    GSManager.Instance.GetHandler("Fire Work").Play();
                    currentFireWorkPlayIndex = rocketIndex;
                    var newCoroutine = StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                    {
                        currentFireWorkPlayIndex = -1;
                    }, fireWorkSoundPlayTime));
                    fireWorkSoundIndexInitRoutineList.Add(newCoroutine);
                }


                rocketListPerPot[potIndex][rocketIndex].SetTrigger("Ignite");
                rocketListPerPot[potIndex][rocketIndex].SetBool("IsSuccess", false);
                ignitedRocketCount++;
                currentFiringRocketVar.value++;
                yield return waitTimePerRocket;

                currentFiringRocketVar.value--;
            }

            // substract 1 firing count which is added before start of coroutine
            currentFiringRocketVar.value--;
        }

        private IEnumerator CollectPotProcessINS(BaseSymbol flyingStartSymbol)
        {
            yield return new WaitForSeconds(scatterStopEffectTime);
            int potIndex = flyingStartSymbol.symbolIndex - PAYUP_SYMBOL_INDEX;
            var targetPot = basePotList[potIndex];
            flyingStartSymbol.Play("Win");

            var betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");
            if (ignitedRocketCountPerPotPerBet.ContainsKey(betCredit) == false)
            {
                var newList = new List<int>();
                foreach (var pot in basePotList) newList.Add(0);
                ignitedRocketCountPerPotPerBet.Add(betCredit, newList);
            }

            GSManager.Instance.GetHandler("Cash Symbol Fly").Play();
            var flyingObj = rocketFlyingPool.GetObject();
            var fromToController = flyingObj.GetComponentInChildren<DirectionalWeightPositionController>(true);
            fromToController.from = flyingStartSymbol.transform;
            fromToController.to = targetPot.transform;
            flyingObj.gameObject.SetActive(true);

            var flyingAnimator = flyingObj.GetComponentInChildren<Animator>();
            flyingAnimator.SetInteger("Symbol Index", flyingStartSymbol.symbolIndex);
            var bonusResponseList = CTCPostSpinController.BonusGameResponseList;
            bool thisPotTriggered = bonusResponseList.Count > 0 ? bonusResponseList[0].GetValue<List<bool>>("flagPerBonusType")[potIndex + 1] : false;

            StartCoroutine(CTCUtill.CallActionAfterDelay(() => { flyingObj.ReturnToPool(); }, rocketFlyingAnimTotalTime));
            yield return new WaitForSeconds(rocketFlyingTime);
            targetPot.SetTrigger("Collect");
            GSManager.Instance.GetHandler("Cash Symbol Collect").Play();

            int realIgnitingCount = thisPotTriggered ? rocketListPerPot[potIndex].Count : 0;
            var ignitingWaitdelay = thisPotTriggered ? initialWaitDelayPerRocketAtTrigger : waitDelayPerRocket;

            if (thisPotTriggered)
            {
                float ignitedCount = 0;
                for (int rocketIndex = 0; rocketIndex < realIgnitingCount; rocketIndex++)
                {
                    if (rocketListPerPot[potIndex][rocketIndex].GetBool("IsSuccess")) continue;

                    if (currentFireCrackerPlayIndex < rocketIndex)
                    {
                        foreach (var coroutine in fireCrackerSoundIndexInitRoutineList) StopCoroutine(coroutine);

                        GSManager.Instance.GetHandler(string.Format("Fire Cracker {0}", rocketIndex + 1)).Play();
                        GSManager.Instance.GetHandler("Fire Work").Play();
                        currentFireCrackerPlayIndex = rocketIndex;
                        var newCoroutine = StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                        {
                            currentFireCrackerPlayIndex = -1;
                        }, fireCrackSoundPlayTime));
                        fireCrackerSoundIndexInitRoutineList.Add(newCoroutine);
                    }

                    if (currentFireWorkPlayIndex < rocketIndex)
                    {
                        foreach (var coroutine in fireWorkSoundIndexInitRoutineList) StopCoroutine(coroutine);

                        GSManager.Instance.GetHandler("Fire Work").Play();
                        currentFireWorkPlayIndex = rocketIndex;
                        var newCoroutine = StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                        {
                            currentFireWorkPlayIndex = -1;
                        }, fireWorkSoundPlayTime));
                        fireWorkSoundIndexInitRoutineList.Add(newCoroutine);
                    }

                    rocketListPerPot[potIndex][rocketIndex].SetTrigger("Shake");
                    rocketListPerPot[potIndex][rocketIndex].SetTrigger("Ignite");
                    rocketListPerPot[potIndex][rocketIndex].SetBool("IsSuccess", true);

                    currentFiringRocketVar.value++;
                    float delay = Math.Max(minimumWaitDelayPerRocketAtTrigger, initialWaitDelayPerRocketAtTrigger - waitDelayAccelationAtTrigger * ignitedCount);
                    ignitedCount++;
                    yield return new WaitForSeconds(delay);


                    basePotList[potIndex].SetInteger("Collect Count", rocketIndex);
                    currentFiringRocketVar.value--;
                }
            }
            else
            {
                var waitTimePerRocket = new WaitForSeconds(ignitingWaitdelay);
                int remainRocketCount = rocketListPerPot[potIndex].Count - realIgnitingCount;
                int maxPossibleFailRocketCount = Math.Min(maxFakeIgnitingCount, remainRocketCount);
                int failedIgnitingCount = maxPossibleFailRocketCount < 1 ? 0 : Random.Range(1,
                   maxPossibleFailRocketCount + 1);
                int ignitedRocketCount = 0;
                for (int rocketIndex = 0; ignitedRocketCount < failedIgnitingCount && rocketIndex < rocketListPerPot[potIndex].Count; rocketIndex++)
                {
                    if (rocketListPerPot[potIndex][rocketIndex].GetBool("IsSuccess") || rocketListPerPot[potIndex][rocketIndex].GetCurrentAnimatorStateInfo(ROCKET_FIRE_ANIM_LAYER).IsName("Fire Off Idle") == false) continue;

                    if (currentFireWorkPlayIndex < rocketIndex)
                    {
                        foreach (var coroutine in fireWorkSoundIndexInitRoutineList) StopCoroutine(coroutine);

                        GSManager.Instance.GetHandler("Fire Work").Play();
                        currentFireWorkPlayIndex = rocketIndex;
                        var newCoroutine = StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                        {
                            currentFireWorkPlayIndex = -1;
                        }, fireWorkSoundPlayTime));
                        fireWorkSoundIndexInitRoutineList.Add(newCoroutine);
                    }

                    rocketListPerPot[potIndex][rocketIndex].SetTrigger("Ignite");
                    rocketListPerPot[potIndex][rocketIndex].SetBool("IsSuccess", false);
                    ignitedRocketCount++;
                    currentFiringRocketVar.value++;
                    yield return waitTimePerRocket;

                    currentFiringRocketVar.value--;
                }
            }

            // substract 1 firing count which is added before start of coroutine
            currentFiringRocketVar.value--;
        }

        public void SetFeatureAnimOfPot(int potIndex, bool onAnim)
        {
            var pot = bonusPotList[potIndex];
            pot.SetBool("OnFeature", onAnim);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(INIT_POTS_EVENT);
            UnRegisterEvent(COLLECT_POT_EVENT);
            UnRegisterEvent(START_TRIGGERED_POT_ANIM_EVENT);
            UnRegisterEvent(REFRESH_POT_STATUS_EVENT);
            MessageDispatcher.UnRegister(ON_CREDIT_EVENT, HandleCreditEvent);
        }
    }
}