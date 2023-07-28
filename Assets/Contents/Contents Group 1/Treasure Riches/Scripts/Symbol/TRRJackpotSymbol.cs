
using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.Utillity;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Slots.TRR.Symbol
{
    public class TRRJackpotSymbol : SymbolBehaviour
    {
        private const int Stop = 0;
        private const int Win = 1;
        private const int Expectation = 2;
        private Coroutine PlayExpectationCoroutine;

        public override void StartBehaviour(SymbolEventHandler eventHandler)
        {
            base.StartBehaviour(eventHandler);
            PlayExpectationCoroutine = null;
        }

        public override void StopBehaviour()
        {
            base.StopBehaviour();
            if (PlayExpectationCoroutine != null)
            {
                StopCoroutine(PlayExpectationCoroutine);
                PlayExpectationCoroutine = null;
            }
        }

        public override void OnEntry()
        {
        }

        public override void OnPrepareStop()
        {
        }

        public override void OnSkip()
        {
            DisableAllCachedObjects();
            DisableJackpotExpectation();
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            DisableAllCachedObjects();
            GetCachedObject(Stop).SetActive(true);
            int jackpotCount = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./customData/jackpotCount");
            TRRUtillity.TrySetGlobalBlackBoardVariable<int>("./customData/jackpotCount", ++jackpotCount);
            if (jackpotCount == 2)
            {
                BaseSlotMachine slotMachine = TRRUtillity.TryGetGlobalBlackBoardVariable<GameObject>("./slotMachine").GetComponent<BaseSlotMachine>();
                List<bool> checkJackpotList = TRRUtillity.TryGetGlobalBlackBoardVariable<List<bool>>("./spin/response/checkJackpot");
                for (int colIndex = 0; colIndex < symbol.column; colIndex++)
                {
                    if (checkJackpotList[colIndex] == true)
                        for (int rowIndex = 0; rowIndex < 4; rowIndex++)
                            if (slotMachine.GetSymbol(colIndex, rowIndex).symbolIndex == 11) slotMachine.GetSymbol(colIndex, rowIndex).Play("PlayExpectationAnimation");
                }
                PlayExpectationAnimation();
            }
            else if (jackpotCount > 2)
            {
                PlayExpectationAnimation();
            }
            PlayAnimation("Inactive");
        }

        public override void OnWin()
        {
            DisableAllCachedObjects();
            DisableJackpotExpectation();
            GetCachedObject(Win).SetActive(true);

            int jackpotIndex = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/jackpotIndex");
            GetCachedObject(Win).GetComponentInChildren<Animator>().SetInteger("Jackpot", jackpotIndex + 1);
            TRRUtillity.SendEvent("OnSoundEvent", "JackpotSymbolChange");
            PlayAnimation("Inactive");
        }
        public void DisableJackpotExpectation()
        {
            if (PlayExpectationCoroutine != null)
            {
                StopCoroutine(PlayExpectationCoroutine);
                PlayExpectationCoroutine = null;
            }
            GetCachedObject(Expectation).SetActive(false);
            PlayAnimation("Idle");
        }
        public void PlayExpectationAnimation()
        {
            PlayExpectationCoroutine = StartCoroutine(_EnableExpectationCoroutineWithDelay());
        }

        private IEnumerator _EnableExpectationCoroutineWithDelay()
        {
            yield return new WaitForSeconds(0.75f);
            EnableExpectation();
        }

        private void EnableExpectation()
        {
            DisableAllCachedObjects();
            GetCachedObject(Expectation).SetActive(true);
            PlayAnimation("Inactive");
            PlayExpectationCoroutine = null;
        }
    }
}
