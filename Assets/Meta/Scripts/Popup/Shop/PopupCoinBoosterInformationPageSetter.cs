using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class PopupCoinBoosterInformationPageSetter : MonoBehaviour, IInformationPageSetter
    {
        public ContextElement creditTextElement;
        public ContextElement totalTextElement;
        public List<ContextElement> multiplierTextElements = new List<ContextElement>();

        public string multiplierTextKey = "";

        public void SetInformationPage()
        {
            InformationDataBase dataBase = gameObject.GetComponent<InformationDataBase>();
            if (dataBase != null)
            {
                long totalCredit = dataBase.GetInformationData<long>(0);
                long multiplier = dataBase.GetInformationData<long>(1);
                long maxCredit = dataBase.GetInformationData<long>(2);

                if (totalCredit > 0 && creditTextElement != null)
                    MetaContextElementUtils.SetTextGlobal(creditTextElement, "POPUP_RESULT_COIN_MULTIPLIER_COIN_TEXT", totalCredit);
                if (maxCredit > 0 && totalTextElement != null)
                    MetaContextElementUtils.SetTextGlobal(totalTextElement, "POPUP_RESULT_COIN_MULTIPLIER_COIN_TEXT", maxCredit);
                if (multiplier > 0 && multiplierTextElements != null && multiplierTextElements.Count > 0)
                {
                    for (int i = 0; i < multiplierTextElements.Count; ++i)
                    {
                        if (multiplierTextElements[i] != null)
                            MetaContextElementUtils.SetTextGlobal(multiplierTextElements[i], multiplierTextKey, NumberUtils.GetMultiplierFromNumerator(multiplier));
                    }
                }
            }
        }
    }
}