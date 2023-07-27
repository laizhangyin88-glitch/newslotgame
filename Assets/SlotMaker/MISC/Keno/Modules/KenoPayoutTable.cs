using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    public class KenoPayoutTable : MonoBehaviour
    {
        public int hitCounts = 0;
        public ContextElement pickedCountText;
        public ContextElement hitCountText;
        public KenoMediator mediator;
        public List<Payout> payoutList;
        public KenoPaytable KenoPaytable;
        public int leastMatchCount;
        public List<KenoPay> paytable;
        
        protected virtual void Awake()
        {
            paytable = KenoPaytable.paytable;
        }

        public void CreatePayoutTable()
        {
            leastMatchCount = 0;
            int pickCount = mediator.pickCount;
            IContextText textElement = pickedCountText as IContextText;
            if(textElement != null)
                textElement.SetText(pickCount.ToString());
            for (int i = 0; i < payoutList.Count; i++)
            {
                payoutList[i].gameObject.SetActive(false);
            }
            int payTableTextIndex = 0;
            for (int hitCount = 0; hitCount < pickCount; hitCount++)
            {
                if (paytable[pickCount - 1].GetPay(hitCount) == 0) continue;
                payoutList[payTableTextIndex].SetText(hitCount + 1, paytable[pickCount - 1].GetPay(hitCount));
                payoutList[payTableTextIndex].gameObject.SetActive(true);
                payoutList[payTableTextIndex].DisplayNormal();
                payTableTextIndex++;
                if (leastMatchCount == 0)
                {
                    leastMatchCount = hitCount + 1;
                }
            }
        }

        public void DisplayMaxCount()
        {
            leastMatchCount = 0;
            IContextText textElement = pickedCountText as IContextText;
            if(textElement != null)
                textElement.SetText(mediator.maxPickCount.ToString());
            for (int i = 0; i < payoutList.Count; i++)
            {
                payoutList[i].gameObject.SetActive(false);
            }
            int payTableTextIndex = 0;
            for (int hitCount = 0; hitCount < mediator.maxPickCount; hitCount++)
            {
                if (paytable[mediator.maxPickCount - 1].GetPay(hitCount) == 0) continue;
                payoutList[payTableTextIndex].SetText(hitCount + 1, paytable[mediator.maxPickCount - 1].GetPay(hitCount));
                payoutList[payTableTextIndex].gameObject.SetActive(true);
                payoutList[payTableTextIndex].DisplayNormal();
                payTableTextIndex++;
                if (leastMatchCount == 0)
                {
                    leastMatchCount = hitCount + 1;
                }
            }
        }

        public void Catch(SpotInstance spot, GameObject gameObject)
        {
            if (!spot.IsHit)
                return;

            hitCounts++;
            IContextText textElement = hitCountText as IContextText;
            if(textElement != null)
                textElement.SetText(hitCounts.ToString());
            if (hitCounts >= leastMatchCount)
            {
                for (int i = 0; i < mediator.pickCount - leastMatchCount + 1; i++)
                {
                    if (i == hitCounts - leastMatchCount)
                    {
                        payoutList[i].DisplayWin();
                    }
                    else
                    {
                        payoutList[i].DisplayNormal();
                    }
                }
            }
        }

        public void ResetWin()
        {
            hitCounts = 0;
            for (int i = 0; i < mediator.pickCount - leastMatchCount + 1; i++)
            {
                payoutList[i].DisplayNormal();
            }
            IContextText textElement = hitCountText as IContextText;
            if(textElement != null)
                textElement.SetText(hitCounts.ToString());
        }

        public void UpdatePayoutTable()
        {
            var spots = mediator.KenoInstance.ticket.spots;
            var hitCounts = 0;
            foreach (SpotInstance spot in spots)
            {
                if (spot.catchState == CatchState.Catch && spot.markState == MarkState.Mark) {
                    hitCounts++;
                }
            }

            IContextText textElement = hitCountText as IContextText;
            if(textElement != null)
                textElement.SetText(hitCounts.ToString());
            if (hitCounts >= leastMatchCount)
            {
                for (int i = 0; i < mediator.pickCount - leastMatchCount + 1; i++)
                {
                    if (i == hitCounts - leastMatchCount)
                    {
                        payoutList[i].DisplayWin();
                    }
                    else
                    {
                        payoutList[i].DisplayNormal();
                    }
                }
            }
        }
    }
}
