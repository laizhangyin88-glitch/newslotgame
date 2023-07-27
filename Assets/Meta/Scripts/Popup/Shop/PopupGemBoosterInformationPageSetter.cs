using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class PopupGemBoosterInformationPageSetter : MonoBehaviour, IInformationPageSetter
    {
        public ContextElement baseTextElement;
        public ContextElement totalTextElement;
        public List<ContextElement> multiplierTextElements = new List<ContextElement>();

        public string multiplierTextKey = "";

        public void SetInformationPage()
        {
            InformationDataBase dataBase = gameObject.GetComponent<InformationDataBase>();
            if (dataBase != null)
            {
                long totalGem = dataBase.GetInformationData<long>(0);
                long multiplier = dataBase.GetInformationData<long>(1);
                long maxGem = dataBase.GetInformationData<long>(2);

                if (totalGem > 0)
                    MetaContextElementUtils.SetTextGlobal(baseTextElement, "POPUP_RESULT_COIN_MULTIPLIER_GEM_TEXT", totalGem);
                if (maxGem > 0)
                    MetaContextElementUtils.SetTextGlobal(totalTextElement, "POPUP_RESULT_COIN_MULTIPLIER_GEM_TEXT", maxGem);
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