using System.Collections;
using System.Collections.Generic;
using BagelCode.Slots.TRR.Utillity;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Slots.TRR.Symbol
{
    public class TRRScatterSymbol : SymbolBehaviour
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
            GetCachedObject(Stop).SetActive(false);
            GetCachedObject(Win).SetActive(false);
            GetCachedObject(Expectation).SetActive(false);
            if (PlayExpectationCoroutine != null)
            {
                StopCoroutine(PlayExpectationCoroutine);
                PlayExpectationCoroutine = null;
            }
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            GetCachedObject(Expectation).SetActive(false);
            GetCachedObject(Stop).SetActive(true);
            PlayAnimation("Inactive");

            if (symbol.row == 3)
            {
                TRRUtillity.PlaySound("Scatter Land");
                List<bool> checkScatterList = TRRUtillity.TryGetGlobalBlackBoardVariable<List<bool>>("./spin/response/checkScatterStack");
                if (checkScatterList[symbol.column] == true)
                {
                    int scatterStackCount = TRRUtillity.TryGetGlobalBlackBoardVariable<int>("./customData/scatterStackCount");
                    TRRUtillity.TrySetGlobalBlackBoardVariable<int>("./customData/scatterStackCount", ++scatterStackCount);

                    if (scatterStackCount == 2)
                    {
                        BaseSlotMachine slotMachine = TRRUtillity.TryGetGlobalBlackBoardVariable<GameObject>("./slotMachine").GetComponent<BaseSlotMachine>();
                        for (int colIndex = 0; colIndex < symbol.column + 1; colIndex++)
                        {
                            if (checkScatterList[colIndex] == true)
                                for (int rowIndex = 0; rowIndex < 4; rowIndex++)
                                    if (slotMachine.GetSymbol(colIndex, rowIndex).symbolIndex == 0) slotMachine.GetSymbol(colIndex, rowIndex).Play("PlayExpectationAnimation");

                        }
                    }
                    else if (scatterStackCount > 2)
                    {
                        BaseSlotMachine slotMachine = TRRUtillity.TryGetGlobalBlackBoardVariable<GameObject>("./slotMachine").GetComponent<BaseSlotMachine>();
                        for (int rowIndex = 0; rowIndex < 4; rowIndex++)
                            if (slotMachine.GetSymbol(symbol.column, rowIndex).symbolIndex == 0) slotMachine.GetSymbol(symbol.column, rowIndex).Play("PlayExpectationAnimation");

                    }
                }

            }
        }

        public override void OnWin()
        {
            GetCachedObject(Win).SetActive(true);
            GetCachedObject(Stop).SetActive(false);
            PlayAnimation("Inactive");
        }
        public void DisableScatterExpectation()
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
            yield return new WaitForSeconds(0.35f);
            EnableExpectation();
        }

        private void EnableExpectation()
        {
            GetCachedObject(Expectation).SetActive(true);
            GetCachedObject(Stop).SetActive(false);
            PlayAnimation("Inactive");
            PlayExpectationCoroutine = null;
        }
    }
}

