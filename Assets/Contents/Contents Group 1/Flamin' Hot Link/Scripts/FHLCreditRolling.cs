using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.FHL
{
    public class FHLCreditRolling : MonoBehaviour
    {
        [SerializeField] private float time;
        public IContextText textElement;
        private long currentCredit;
        public long beforeCredit;
        public long finalCredit;

        public void OnCreditRolling()
        {

            long diffCredit = finalCredit - beforeCredit;
            currentCredit = beforeCredit;
            StartCoroutine(CreditRolling(diffCredit, finalCredit));
        }

        private IEnumerator CreditRolling(long diffCredit, long finalCredit)
        {
            ContextTextMeshProUGUI coinText = GetComponent<FHLSymbolCoinController>().coinText;
            while (currentCredit < finalCredit)
            {
                yield return null;
                currentCredit += (long)((float)diffCredit * Time.deltaTime / time);
                if (currentCredit > finalCredit) currentCredit = finalCredit;
                coinText.SetText(string.Format(StringTableUtils.customProvider, "{0:SimpleNumber}", currentCredit));
            }
        }
    }
}
