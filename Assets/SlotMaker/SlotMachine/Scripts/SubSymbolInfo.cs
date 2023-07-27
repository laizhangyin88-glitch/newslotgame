using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
	[Serializable]
	public class SubSymbolInfo : ICloneable
	{
	    public int symbol;
	    
	    public object Clone()
	    {
	        var newSubSymbol = new SubSymbolInfo();
	        newSubSymbol.symbol = this.symbol;
	        return newSubSymbol;
	    }
	}
}
