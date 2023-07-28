
using System.Collections;
using GameStudio.Slot.TRL.Utillity;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.TRL.Symbol
{
    public class TRLJackpotSymbol : SymbolBehaviour
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
            GetCachedObject(Expectation).SetActive(false);
            CancelInvoke("EnableExpectation");
            PlayAnimation("Idle");
        }

        public override void OnStopEffect()
        {
            DisableAllCachedObjects();
            GetCachedObject(Stop).SetActive(true);
            int jackpotCount = TRLUtillity.TryGetGlobalBlackBoardVariable<int>("./customData/jackpotCount");
            TRLUtillity.TrySetGlobalBlackBoardVariable<int>("./customData/jackpotCount", ++jackpotCount);
            if (jackpotCount == 1)
            {
                PlayExpectationAnimation();
            }
            PlayAnimation("Inactive");
        }

        public override void OnWin()
        {
            DisableAllCachedObjects();

            int jackpotIndex = TRLUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/jackpotIndex");
            GetCachedObject(3 + jackpotIndex).SetActive(true);
            PlayAnimation("Inactive");
        }

        public void Change()
        {
            DisableAllCachedObjects();
            GetCachedObject(Win).SetActive(true);

            int jackpotIndex = TRLUtillity.TryGetGlobalBlackBoardVariable<int>("./bonus/response/jackpotIndex");
            GetCachedObject(Win).GetComponentInChildren<Animator>().SetInteger("Jackpot", jackpotIndex + 1);
            TRLUtillity.SendEvent("OnSoundEvent", "JackpotSymbolChange");
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
