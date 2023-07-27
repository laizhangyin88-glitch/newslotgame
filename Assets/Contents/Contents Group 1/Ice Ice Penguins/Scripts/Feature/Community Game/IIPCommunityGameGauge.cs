using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.IIP.Utility;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.Slot.IIP.Feature
{
    public class IIPCommunityGameGauge : FeatureModule
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Animator currentMutliplierAnimator;
        [Space(20)]
        [SerializeField] private Slider gaugeSlider;
        [Space(20)]
        [SerializeField] private List<Animator> goalPointAnimator = new List<Animator>();
        [SerializeField] private List<float> _goalPointCondition = new List<float>();
        public List<float> goalPointCondition { get { return _goalPointCondition; } }
        [SerializeField] private List<int> goalMultiplier = new List<int>();
        [Space(20)]
        [SerializeField] private Transform applyTargetAnchor;
        [SerializeField] private Transform applyFlyingStartAnchor;
        [SerializeField] private DirectionalWeightPositionController mainDirectionController;

        public int currentLevelIndex { get; private set; }
        public float targetAmount { get; private set; }
        public bool isCharging { get; private set; }

        public void Initialize()
        {
            isCharging = false;
            currentLevelIndex = 0;
            goalPointAnimator[0].SetTrigger("Arrive Soon");
            SetGaugeAmount(0f);
        }

        public int GetCuurrentMultiplier()
        {
            return goalMultiplier[currentLevelIndex];
        }

        public Coroutine ApplyFinalMultiplierCoroutine() => StartCoroutine(_ApplyFinalMultiplierCoroutine());
        private IEnumerator _ApplyFinalMultiplierCoroutine()
        {
            bool isArrived = false;
            void OnMultiplierFlyingArrived(EventData eventData)
            {
                isArrived = true;
            }
            RegisterEvent("OnMutliplierFlyingArrive", OnMultiplierFlyingArrived);
            if (currentLevelIndex == goalPointCondition.Count)
            {
                animator.SetTrigger("Apply");
                mainDirectionController.from = applyFlyingStartAnchor;
                mainDirectionController.to = applyTargetAnchor;
            }
            else
            {
                goalPointAnimator[currentLevelIndex - 1].SetTrigger("Apply");
                goalPointAnimator[currentLevelIndex - 1].GetComponentInChildren<DirectionalWeightPositionController>().from = goalPointAnimator[currentLevelIndex - 1].transform;
                goalPointAnimator[currentLevelIndex - 1].GetComponentInChildren<DirectionalWeightPositionController>().to = applyTargetAnchor;
            }
            yield return new WaitUntil(() => isArrived == true);
            UnRegisterEvent("OnMutliplierFlyingArrive");
        }

        public void SetGaugeAmount(float amount)
        {
            targetAmount = gaugeSlider.value = amount;
        }

        public void ChargeGaugeAmount(float amount)
        {
            targetAmount += amount;
            isCharging = true;
        }

        void Update()
        {
            if (isCharging == true && currentLevelIndex < goalPointCondition.Count)
            {
                if (gaugeSlider.value < targetAmount)
                {
                    animator.SetBool("Charging", true);
                    gaugeSlider.value += Time.deltaTime * 2f;
                }
                else
                {
                    isCharging = false;
                    animator.SetBool("Charging", false);
                    gaugeSlider.value = targetAmount;
                }

                if (gaugeSlider.value >= goalPointCondition[currentLevelIndex])
                {
                    StartCoroutine(_OnReachedGoalPointCoroutine(++currentLevelIndex));
                    //OnReachedGoalPoint(++currentLevelIndex);
                }
            }
        }

        public void DisappearCurrentMultiplier()
        {
            if (currentLevelIndex < goalPointCondition.Count)
            {
                currentMutliplierAnimator.SetInteger("Index", currentLevelIndex - 1);
                currentMutliplierAnimator.SetTrigger("Disappear");
            }
        }

        private IEnumerator _OnReachedGoalPointCoroutine(int index)
        {
            if (index == goalPointCondition.Count)
            {
                currentMutliplierAnimator.SetInteger("Index", index - 2);
                currentMutliplierAnimator.SetTrigger("Disappear");
                yield return new WaitForSeconds(1f);

                IIPUtility.PlaySound("Community Game Multiplier Up 6");
                animator.SetTrigger("EndPoint");
                animator.SetBool("Charging", false);
                animator.SetTrigger("Middle");
                IIPUtility.PlaySound("Multiplier Whoosh 1");
                yield return new WaitForSeconds(1f);
                animator.SetTrigger("Apply");
                IIPUtility.PlaySound("Multiplier Whoosh 2");
                yield return new WaitForSeconds(1f);
            }
            else
            {
                IIPUtility.PlaySound($"Community Game Multiplier Up {index}");
                goalPointAnimator[index - 1].SetTrigger("Arrive");
                if (index > 1)
                {
                    currentMutliplierAnimator.SetInteger("Index", index - 2);
                    currentMutliplierAnimator.SetTrigger("Disappear");
                }
                yield return new WaitForSeconds(2f);
                currentMutliplierAnimator.SetInteger("Index", index - 1);
                currentMutliplierAnimator.SetTrigger("Middle");
                IIPUtility.PlaySound("Multiplier Whoosh 1");

                if (index < goalPointCondition.Count - 1)
                    goalPointAnimator[index].SetTrigger("Arrive Soon");
                yield return new WaitForSeconds(2f);
                currentMutliplierAnimator.SetTrigger("Left");
                IIPUtility.PlaySound("Multiplier Whoosh 2");
                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }
    }
}