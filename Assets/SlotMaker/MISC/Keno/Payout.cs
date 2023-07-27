using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    public class Payout : MonoBehaviour
    {
        public GameObject normal;
        public GameObject win;

        public List<ContextElement> hitCountTextList;
        public List<ContextElement> payTextList;

        public void SetText(int hitCounts, long payAmout)
        {
            for (int i = 0; i < hitCountTextList.Count; i++)
            {
                IContextText textElement = hitCountTextList[i] as IContextText;
                if(textElement != null)
                    textElement.SetText(hitCounts.ToString());
            }
            for (int i = 0; i < payTextList.Count; i++)
            {
                IContextText textElement = payTextList[i] as IContextText;
                if(textElement != null)
                    textElement.SetText(payAmout.ToString());
            }
        }

        public void DisplayWin()
        {
            win.SetActive(true);
            normal.SetActive(false);
        }
        public void DisplayNormal()
        {
            win.SetActive(false);
            normal.SetActive(true);
        }
    }
}
