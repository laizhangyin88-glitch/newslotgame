using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.Tasks.Actions.ClientAPI;
using BagelCode.Tasks.Actions.Contents;
using GameStudio.Slot.EDM.Feature;
using GameStudio.Slot.EDM.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.Slot.EDM.Popup
{
    public class EDMBonusGameResultBoardPopup : EDMPopup
    {
        public Animator animator;
        public Animator bonusGameSlotFrameAnimator;
        public Blackboard fullScreenJackpotBlackboard;
        public Animator fullScreenJackpotAnimator;
        public EDMRespinAdmin respinAdmin;
        public SlotMachine bonusSlotMachine;
        public Button button;

        public ObjectPool creditFlyingObjectPool;
        public Transform creditFlyingParentTransform;
        public Transform creditFlyingTargetTransform;

        public ObjectPool jackpotFlyingObjectPool;
        public Transform jackpotFlyingParentTransform;
        public Transform jackpotFlyingTargetTransform;

        public TextMeshProUGUI multiplierText;
        public TextMeshProUGUI creditText;
        private long accCredit = 0;
        public override IEnumerator ActiveCoroutine()
        {
            Variable<Blackboard> fullScreenJackpotBonus = BlackboardUtils.FindVariable<Blackboard>("./bonus/response/fullScreenJackpotBonus");
            accCredit = 0;
            creditText.text = "";

            if (fullScreenJackpotBonus != null)
            {
                Blackboard fullScreenJackpotBonusBB = fullScreenJackpotBonus.GetValue();
                bonusGameSlotFrameAnimator.SetBool("Full", true);

                int jackpotIndex = fullScreenJackpotBonusBB.GetValue<int>("jackpotIndex");
                long jackpotAwardAmount = fullScreenJackpotBonusBB.GetValue<long>("jackpotAwardAmount");
                bool isEligible = fullScreenJackpotBonusBB.GetValue<bool>("isEligible");

                yield return new WaitUntil(() => fullScreenJackpotBlackboard.gameObject.activeInHierarchy == true);
                EDMUtility.PlaySound("High Jackpot Chest Open");
                fullScreenJackpotAnimator.SetInteger("jackpotIndex", jackpotIndex);

                fullScreenJackpotBlackboard.SetValue("_jackpotEarnCredit", jackpotAwardAmount);
                fullScreenJackpotBlackboard.SetValue("_isEligible", isEligible);
                fullScreenJackpotBlackboard.SetValue("_jpIndex", jackpotIndex);
                yield return new WaitForSeconds(2.75f);


                bool jackpotEnd = false;
                void OnFullScreenJackpotEnd(EventData eventData)
                {
                    if (eventData.name == "EDM_FULLSCREEN_JACKPOT_END")
                        jackpotEnd = true;
                }
                ContentEvent.Register(OnFullScreenJackpotEnd);

                yield return new WaitForSeconds(0.33f);
                ContentEvent.SendEvent("EDM_FULLSCREEN_JACKPOT");



                yield return new WaitUntil(() => jackpotEnd);
                ContentEvent.UnRegister(OnFullScreenJackpotEnd);
                yield return new WaitForSeconds(0.5f);
                animator.SetBool("Board", true);
                yield return new WaitForSeconds(1f);

                bonusGameSlotFrameAnimator.SetBool("Full", false);

                bool jackpotApply = false;
                void OnFullScreenJackpotApply(EventData eventData)
                {
                    if (eventData.name == "EDM_FULLSCREEN_JACKPOT_APPLY")
                        jackpotApply = true;
                }
                ContentEvent.Register(OnFullScreenJackpotApply);
                yield return new WaitUntil(() => jackpotApply);
                ContentEvent.UnRegister(OnFullScreenJackpotApply);
                OnArriveFly(true, jackpotAwardAmount);
                yield return new WaitForSeconds(0.5f);

            }
            animator.SetBool("Board", true);
            yield return new WaitForSeconds(1f);

            int currentActoveRow = respinAdmin.currentActiveRow;

            List<Vector2Int> creditCollectCellList = respinAdmin.creditCollectCellList;
            List<Vector2Int> jackpotCollectCellList = respinAdmin.jackpotCollectCellList;

            for (int reelIndex = (6 - currentActoveRow) * 5; reelIndex < 30; reelIndex++)
            {
                BaseSymbol symbol = bonusSlotMachine.reels[reelIndex].symbols[1];
                if (symbol.symbolIndex == 12 || symbol.symbolIndex == 13 || symbol.symbolIndex == 14)

                    creditCollectCellList.Add(new Vector2Int(symbol.column, symbol.row));
                if (symbol.symbolIndex == 15 || symbol.symbolIndex == 16 || symbol.symbolIndex == 17)
                    jackpotCollectCellList.Add(new Vector2Int(symbol.column, symbol.row));
            }

            MessageDispatcher.Register("OnSpinButtonEvent", OnSpinButtonEvent);

            float creditFlyingDelay = 0.8f;
            float jackpotFlyingDelay = 1.25f;

            void OnSpinButtonEvent(EventData eventData)
            {
                creditFlyingDelay = 0.1f;
                jackpotFlyingDelay = 0.1f;
            }
            foreach (var cell in creditCollectCellList)
            {
                BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, cell);
                GameObject collectFly = creditFlyingObjectPool.GetObject().gameObject;
                symbol.Play("InActiveFinalCollect");
                collectFly.GetComponent<EDMBonusCalculateCreditFly>().Initialize(this, symbol.transform, creditFlyingTargetTransform,
                Convert.ToInt64(symbol.symbolInfo.customData["value"]), symbol.symbolIndex);
                collectFly.transform.SetParent(creditFlyingParentTransform);

                yield return new WaitForSeconds(creditFlyingDelay);
            }

            foreach (var cell in jackpotCollectCellList)
            {
                BaseSymbol symbol = EDMUtility.GetSymbolFromSpotSlotMachine(bonusSlotMachine, cell);
                long value = Convert.ToInt64(symbol.symbolInfo.customData["value"]);
                if (value > 0)
                {
                    int jackpotIndex = Convert.ToInt32(symbol.symbolInfo.customData["jackpotIndex"]);
                    symbol.Play("InActiveFinalCollect");
                    GameObject collectFly = jackpotFlyingObjectPool.GetObject().gameObject;
                    collectFly.GetComponent<EDMBonusCalculateJackpotFly>().Initialize(this, symbol.transform, jackpotFlyingTargetTransform,
                    value, jackpotIndex);
                    collectFly.transform.SetParent(jackpotFlyingParentTransform);
                    yield return new WaitForSeconds(jackpotFlyingDelay);
                }
            }
            MessageDispatcher.UnRegister("OnSpinButtonEvent", OnSpinButtonEvent);


            yield return new WaitUntil(() => creditFlyingParentTransform.transform.childCount == 0 || jackpotFlyingParentTransform.transform.childCount == 0);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(1f);

            animator.SetBool("Board", false);
            bool isClicked = false;
            void OnClickButton()
            {
                EDMUtility.PlaySound("Button");
                isClicked = true;
            }
            ClaimBonus claimBonus22302 = new ClaimBonus() { bonusId = 22302, selectedIndex = 0 };
            EDMUtility.ExecuteAction(claimBonus22302, this);
            yield return new WaitUntil(() => claimBonus22302.isRunning == false);
            EDMUtility.ExecuteAction(new EndBonus(), this);
            EDMUtility.ExecuteAction(new BeginBonus() { bonusId = 22303 }, this);
            EDMUtility.ExecuteAction<Blackboard>(new AddBonusEarnCredit() { earnCredit = "./bonus/response/earnCredit" }, BlackboardUtils.FindValue<GameObject>("./gameContent").GetComponent<Blackboard>());
            ClaimBonus claimBonus22303 = new ClaimBonus() { bonusId = 22303, selectedIndex = 0 };
            EDMUtility.ExecuteAction(claimBonus22303, this);
            yield return new WaitUntil(() => claimBonus22303.isRunning == false);
            EDMUtility.ExecuteAction(new EndBonus(), this);

            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateTurnCredit", false));
            button.onClick.AddListener(OnClickButton);
            yield return new WaitUntil(() => isClicked == true);
            button.onClick.RemoveListener(OnClickButton);


            animator.SetTrigger("Disappear");
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateTurnCredit", true));
            yield return new WaitForSeconds(1.5f);

        }



        public void OnArriveFly(bool isJackpot, long credit)
        {
            if (isJackpot) animator.SetTrigger("Jackpot");
            else animator.SetTrigger("Win");
            accCredit += credit;
            creditText.text = FormatUtility.CommaNumberFormat(accCredit);
        }
    }
}