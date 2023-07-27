using UnityEngine;
using System.Collections;

namespace SlotMaker
{
    public class ApplySymbolAnimator : MonoBehaviour
    {
        public Animator animator;
        public int additionalSortingOrder;

        public void Apply(BaseSymbol symbol)
        {
            ContentCustomData.Instance.GetComponent<SymbolAnimator>().Apply(symbol, animator, additionalSortingOrder);
        }
    }
}
