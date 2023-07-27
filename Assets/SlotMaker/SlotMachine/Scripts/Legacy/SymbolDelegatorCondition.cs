using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas;

namespace SlotMaker
{
    public class SymbolDelegatorCondition : MonoBehaviour
    {
    	public virtual ActionListPlayer FindPlayer(BaseSymbol symbol, string animationName)
    	{
    		return null;
    	}
    }
}
