using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.OSA_Scroll;

namespace BagelCode.VipLounge
{
    public class VipLoungeJackpotItemCellController : MonoBehaviour
    {
        public ContextElement textElement;
        public RectTransform rectTrans;
        public float height => rectTrans.sizeDelta.y;
        public float posY => rectTrans.anchoredPosition.y;

        public int index = 0;

        private System.Action animationCallback = null;

        public void SetText(string message = "")
        {
            if (textElement != null)
                MetaContextElementUtils.SetText(textElement, message);
        }

        public void AppearAnimationExtraPoint(float animationTime, long minValue, long maxValue)
        {
            if (textElement == null)
                return;

            MetaIncreaseNumber jackpotCredit = textElement.GetComponent<MetaIncreaseNumber>();
            if (jackpotCredit == null)
                jackpotCredit = textElement.gameObject.AddComponent<MetaIncreaseNumber>();

            jackpotCredit.Reset(textElement as IContextText, "COMMA_COIN", minValue, maxValue, animationTime, 1, false, SetCellNumberText);
        }

        public void SetTriggerAnimation(string trigger)
        {
            Animator anim = gameObject.GetComponent<Animator>();
            anim?.SetTrigger(trigger);
        }

        public void SetCallback(System.Action itemCallback)
        {
            animationCallback = itemCallback;
        }

        public void CallbackActiveEnd()
        {
            if (animationCallback != null)
                animationCallback();
        }

        public void SetCellNumberText(long credit)
        {
            long baseCredit = credit;
            if (baseCredit < FormatUtility._1B)
                MetaContextElementUtils.SetTextGlobal(textElement, "COMMA_COIN", baseCredit);
            else if (baseCredit < FormatUtility._1T)
                MetaContextElementUtils.SetText(textElement, (baseCredit / FormatUtility._1K).ToString("<sprite name=Coin>#,000K"));
            else
                MetaContextElementUtils.SetText(textElement, (baseCredit / FormatUtility._1B).ToString("<sprite name=Coin>#,000B"));
        }
    }
}