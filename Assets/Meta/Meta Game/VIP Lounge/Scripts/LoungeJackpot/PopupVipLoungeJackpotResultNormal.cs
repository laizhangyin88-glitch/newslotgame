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
    public class PopupVipLoungeJackpotResultNormal : PopupVipLoungeJackpotResultBase
    {
        protected override void InitProperty()
        {
            scoreTextElement = ContextUtils.FindElement(root, "Text Coin", ContextSearchingType.ChildrenSearch);
            SetScoreText(StringTableUtils.GetString(StringTable.StringTableType.Global, "COMMA_COIN", winCredit));

            collectButtonElement = ContextUtils.FindElement(root, "Button Collect", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(collectButtonElement, "Text", "VIP_LOUNGE_POPUP_JACKPOT_COLLECT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(collectButtonElement, OnClickClose);

            anim.SetBool("Active", true);
        }

        protected override void PlaySound()
        {
            GSManager.Instance.GetHandler(VipLounge.Defines.LOUNGE_JACKPOT_RESULT_COIN).Play();
        }

        protected override IEnumerator OnCloseCoroutine()
        {
            GSManager.Instance.GetHandler("UI_Coin_Riser_Start").Play();
            anim.SetBool("Active", false);
            yield return new WaitForSeconds(1.5f);

            SendCallback();
            PopupManager.Instance.Close(gameObject);

            anim.SetTrigger("Close");
        }

        private void OnClickClose()
        {
            OnClose();
        }
    }
}