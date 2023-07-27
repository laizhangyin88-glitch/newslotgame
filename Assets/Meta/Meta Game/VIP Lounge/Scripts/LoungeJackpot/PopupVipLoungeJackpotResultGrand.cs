using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.OSA_Scroll;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode.VipLounge
{
    public class PopupVipLoungeJackpotResultGrand : PopupVipLoungeJackpotResultBase
    {
        private MetaIncreaseNumber jackpotChase = null;
        public bool isEndAnimationStart = false;
        public bool isAnimationEnd = false;
        public int chaseTime = 2;

        protected override void InitProperty()
        {
            anim?.SetInteger("Jackpot", (int)winType);

            scoreTextElement = ContextUtils.FindElement(root, "Scale/Text", ContextSearchingType.FullNameSearch);
            jackpotChase = scoreTextElement.GetComponent<MetaIncreaseNumber>();
            if (jackpotChase == null)
                jackpotChase = scoreTextElement.gameObject.AddComponent<MetaIncreaseNumber>();
            jackpotChase.Reset(
                scoreTextElement as IContextText,
                "",
                VipLounge.Utils.GetLoungeJackpotMultiplierValue(winCredit, 90L),
                winCredit,
                chaseTime,
                1,
                false,
                null,
                EndChase);

            collectButtonElement = ContextUtils.FindElement(root, "Button Collect", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(collectButtonElement, "Text", "VIP_LOUNGE_POPUP_JACKPOT_COLLECT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(collectButtonElement, OnClickCollect);
            GSManager.Instance.GetHandler("UI_Coin_Riser_Loop").Play();
        }

        protected override void PlaySound()
        {
            string soundKey = VipLounge.Defines.LOUNGE_JACKPOT_RESULT_COIN;
            switch (winType)
            {
                case LoungeJackpotWinType.MINI:
                    soundKey = VipLounge.Defines.LOUNGE_JACKPOT_RESULT_MINI;
                    break;
                case LoungeJackpotWinType.MINOR:
                    soundKey = VipLounge.Defines.LOUNGE_JACKPOT_RESULT_MINOR;
                    break;
                case LoungeJackpotWinType.MAJOR:
                    soundKey = VipLounge.Defines.LOUNGE_JACKPOT_RESULT_MAJOR;
                    break;
                case LoungeJackpotWinType.GRAND:
                    soundKey = VipLounge.Defines.LOUNGE_JACKPOT_RESULT_GRAND;
                    break;
            }
            GSManager.Instance.GetHandler(soundKey).Play();
        }

        protected override IEnumerator OnCloseCoroutine()
        {
            PopupManager.Instance.Close(gameObject);
            GSManager.Instance.GetHandler("UI_Coin_Riser_Start").Play();
            anim.SetTrigger("Close");

            yield return new WaitForSeconds(1.5f);

            SendCallback();
        }

        private void OnClickCollect()
        {
            if (isAnimationEnd == false)
            {
                jackpotChase.Reset(scoreTextElement as IContextText, "", winCredit, winCredit, 0.0f, NumberUtils.GetGlobalDenominator(), false);
                MetaContextElementUtils.SetTextGlobal(scoreTextElement, "TEXT_COMMA_NUMBER", winCredit);
                anim.SetTrigger("End");
            }
            else
            {
                OnClose();
            }
        }

        private void EndChase()
        {
            anim.SetTrigger("End");
        }

        public void CallbackCoinBorderStart()
        {
            if (isEndAnimationStart)
                return;
            isEndAnimationStart = true;
            GSManager.Instance.GetHandler("UI_Coin_Riser_Loop").Stop();
            GSManager.Instance.GetHandler("UI_Coin_Riser_End").Play();
            GSManager.Instance.GetHandler(VipLounge.Defines.LOUNGE_JACKPOT_RESULT_END_CREDIT).Play();
        }

        public void CallbackCoinBorderEnd()
        {
            isAnimationEnd = true;
        }
    }
}