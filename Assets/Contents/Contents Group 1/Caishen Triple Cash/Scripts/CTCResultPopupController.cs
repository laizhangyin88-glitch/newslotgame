using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using TMPro;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.CTC
{
    public class CTCResultPopupController : FeatureModule
    {
        [SerializeField] private float creditRollingTime = 0.5f;
        [SerializeField] private TextMeshProUGUI creditText;
        [SerializeField] private GameObject popup;
        [SerializeField] private Animator popUpAnimator;

        [SerializeField] private List<SlotMachine> slotMachineList;
        [SerializeField] private ObjectPool flyingCreditPool;
        [SerializeField] private float flyingTime = 1.1f;

        private int jackpotResponseIndex;
        private int bonusGameIndex;
        private int reelIndex;
        private int targetSlotIndex;

        private long lastCredit;
        private long betCredit;

        private List<Blackboard> bonusGameResponseList;
        private List<Blackboard> jackpotResponseList;
        private List<Coroutine> creditRollingCoroutines = new List<Coroutine>();

        public const string EARN_NEXT_CELL_CREDIT_EVENT = "EarnNextCellCredit";
        public const string INIT_RESULT_POPUP_EVENT = "InitResultPopup";
        public const string APPLY_GRAND_JACKPOT_TO_RESULT_POPUP_EVENT = "ApplyGrandJackpotToResultPopup";
        public const string MOVE_WIN_PROCESS_TO_NEXT_SLOT_EVENT = "MoveWinProcessToNextSlot";
        public const int JACKPOT_BONUS_ID = 21801;

        public static List<Blackboard> JackpotResponseList
        {
            get
            {
                var bonusList = BlackboardUtils.FindValue<List<Blackboard>>(null, "./turn/spin/response/bonusResult");
                List<Blackboard> targetBonusRsponseList = new List<Blackboard>();
                foreach (var bonus in bonusList)
                {
                    if (bonus.GetValue<int>("bonusId") == JACKPOT_BONUS_ID)
                        targetBonusRsponseList.Add(bonus);
                }

                return targetBonusRsponseList;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterEvent(INIT_RESULT_POPUP_EVENT, (eventData) =>
            {
                creditText.gameObject.SetActive(false);
                bonusGameResponseList = CTCPostSpinController.BonusGameResponseList;
                jackpotResponseList = JackpotResponseList;
                jackpotResponseIndex = 0;
                bonusGameIndex = 0;
                creditText.text = 0.ToString();
                lastCredit = 0;
                reelIndex = 0;
                betCredit = BlackboardUtils.FindValue<long>(null, "./betCredit");

                popup.SetActive(true);
                bool isDoubleSpin = (bonusGameResponseList[0].GetValue<int>("bonusCombination") & (int)EFreeGameBonusType.DoubleSpin) != 0;
                popUpAnimator.SetBool("IsDoubleSpin", isDoubleSpin);
                targetSlotIndex = isDoubleSpin ? 0 : 1;
            });

            RegisterEvent(EARN_NEXT_CELL_CREDIT_EVENT, (eventData) =>
            {
                var multiplierPerReel =
                    bonusGameResponseList[bonusGameIndex].GetValue<List<double>>("finalMultiplierPerReel");
                var jackpotIndexPerReel =
                    bonusGameResponseList[bonusGameIndex].GetValue<List<int>>("finalJackpotIndexPerReel");

                int targetReelIndex = reelIndex;
                bool hasFoundMultiplier = false;
                for (; targetReelIndex < multiplierPerReel.Count; targetReelIndex++)
                    if (multiplierPerReel[targetReelIndex] > 0 || jackpotIndexPerReel[targetReelIndex] >= 0)
                    {
                        hasFoundMultiplier = true;
                        break;
                    }

                double multiplier = 0;
                int jackpotIndex = -1;

                if (hasFoundMultiplier)
                {
                    multiplier = multiplierPerReel[targetReelIndex];
                    jackpotIndex = jackpotIndexPerReel[targetReelIndex];
                    reelIndex = targetReelIndex + 1;
                }
                else
                    return;

                long addedCredit = 0L;
                if (multiplier > 0) addedCredit += betCredit * Convert.ToInt64(multiplier * 100) / 100;
                if (jackpotIndex >= 0) addedCredit += jackpotResponseList[jackpotResponseIndex++].GetValue<long>("earnCredit");

                GSManager.Instance.GetHandler("LB Cash Symbol Fly").Play();
                var flyingObj = flyingCreditPool.GetObject();
                var fromToController = flyingObj.GetComponentInChildren<DirectionalWeightPositionController>(true);
                fromToController.from = slotMachineList[targetSlotIndex].GetOverlaySymbol(new Cell(targetReelIndex, 0).GetHashCode()).transform;
                fromToController.to = creditText.transform;
                flyingObj.gameObject.SetActive(true);
                long startCredit = lastCredit;
                StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                {
                    creditText.gameObject.SetActive(true);
                    GSManager.Instance.GetHandler("LB Cash Symbol Collect").Play();
                    foreach (var coroutine in creditRollingCoroutines) StopCoroutine(coroutine);
                    creditRollingCoroutines.Clear();
                    creditRollingCoroutines.Add(StartCoroutine(UpdateCreditText(startCredit, addedCredit)));
                    StartCoroutine(CTCUtill.CallActionAfterDelay(() =>
                    {
                        flyingObj.ReturnToPool();
                    }, 0.7f));
                }, flyingTime));
                lastCredit = lastCredit + addedCredit;
            });

            RegisterEvent(APPLY_GRAND_JACKPOT_TO_RESULT_POPUP_EVENT, (eventData) =>
            {
                var jackpotIndexPerReel =
                    bonusGameResponseList[bonusGameIndex].GetValue<List<int>>("finalJackpotIndexPerReel");

                long addedCredit = jackpotResponseList[jackpotResponseIndex++].GetValue<long>("earnCredit");
                creditText.gameObject.SetActive(true);
                foreach (var coroutine in creditRollingCoroutines) StopCoroutine(coroutine);
                creditRollingCoroutines.Clear();
                creditRollingCoroutines.Add(StartCoroutine(UpdateCreditText(lastCredit, addedCredit)));
                lastCredit = lastCredit + addedCredit;
            });

            RegisterEvent(MOVE_WIN_PROCESS_TO_NEXT_SLOT_EVENT, (eventData) =>
            {
                bonusGameIndex++;
                reelIndex = 0;
                popUpAnimator.SetTrigger("Next");
                targetSlotIndex++;
                return;
            });
        }

        private IEnumerator UpdateCreditText(long startCredit, long addedCredit)
        {
            float elapsedTime = 0;
            GSManager.Instance.GetHandler("Result Count Up").Play();
            while (elapsedTime < creditRollingTime)
            {
                creditText.text = FormatUtility.CommaNumberFormat(startCredit + Convert.ToInt64(elapsedTime / creditRollingTime * 100) * addedCredit / 100);
                yield return null;
                elapsedTime += Time.deltaTime;
            }
            GSManager.Instance.GetHandler("Result Count Up").Stop();
            GSManager.Instance.GetHandler("Result Count End").Play();
            creditText.text = FormatUtility.CommaNumberFormat(startCredit + addedCredit);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterEvent(INIT_RESULT_POPUP_EVENT);
            UnRegisterEvent(EARN_NEXT_CELL_CREDIT_EVENT);
            UnRegisterEvent(MOVE_WIN_PROCESS_TO_NEXT_SLOT_EVENT);
            UnRegisterEvent(APPLY_GRAND_JACKPOT_TO_RESULT_POPUP_EVENT);
        }

        public void DeactivePopupAfetDelay(float delay)
        {
            StartCoroutine(CTCUtill.CallActionAfterDelay(() => { popup.SetActive(false); }, delay));
        }
    }
}