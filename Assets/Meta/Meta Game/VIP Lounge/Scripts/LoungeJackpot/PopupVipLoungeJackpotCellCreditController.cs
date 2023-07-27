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
    public class PopupVipLoungeJackpotCellCreditController : MonoBehaviour
    {
        private ContextElement textElement;

        private MetaIncreaseNumber contentJackpotCredit = null;

        public void SetJackpotCredit(long newPrev, long newTarget, float refreshTime)
        {
            if (textElement == null)
                textElement = gameObject.GetComponent<ContextElement>();

            GetMetaIncreaseNumber();
            contentJackpotCredit?.Reset(
                            textElement as IContextText,
                            "TEXT_COMMA_NUMBER",
                            newPrev,
                            newTarget,
                            refreshTime,
                            1,
                            false,
                            SetJackpotCreditText);
        }

        private MetaIncreaseNumber GetMetaIncreaseNumber()
        {
            contentJackpotCredit = textElement.GetComponent<MetaIncreaseNumber>();
            if (contentJackpotCredit == null)
                contentJackpotCredit = textElement.gameObject.AddComponent<MetaIncreaseNumber>();

            return contentJackpotCredit;
        }

        private void SetJackpotCreditText(long credit)
        {
            if (textElement == null)
                return;

            long baseCredit = credit;
            long _1Q = FormatUtility._1T * 1000;

            if (baseCredit < _1Q)
                MetaContextElementUtils.SetTextGlobal(textElement, "TEXT_COMMA_NUMBER", baseCredit);
            else if (baseCredit < (_1Q * 1000))
                MetaContextElementUtils.SetText(textElement, (baseCredit / FormatUtility._1K).ToString("#,000K"));
            else
                MetaContextElementUtils.SetText(textElement, (baseCredit / FormatUtility._1B).ToString("#,000B"));
        }
    }
}