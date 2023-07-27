using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	public class SymbolAnimator : MonoBehaviour
	{
	    public virtual void Apply(BaseSymbol symbol, Animator animator, int additionalSortingOrder)
	    {
	        if (symbol.symbolInfo.link.isPivot)
	        {
	            animator.SetInteger("order",ContentCustomData.Instance.symbolSortingOrder.orders[symbol.symbolInfo.symbol] + additionalSortingOrder);
	            animator.SetInteger("symbolIndex", symbol.symbolIndex);
	        }
	    }
	}
}
