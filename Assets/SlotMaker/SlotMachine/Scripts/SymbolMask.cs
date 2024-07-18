using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{

[System.Flags]
public enum SymbolAttribute
{
    Wild      = (1 << 0),
    Bar       = (1 << 1),
    Seven     = (1 << 2),
    Scatter1  = (1 << 3),
    Scatter2  = (1 << 4),
    Scatter3  = (1 << 5),
    Scatter4  = (1 << 6),
    Scatter5  = (1 << 7),
    Scatter6  = (1 << 8),
    Scatter7  = (1 << 9),
    Scatter8  = (1 << 10),
    Scatter9  = (1 << 11),
    Scatter10 = (1 << 12),
    Blank     = (1 << 13),
    // Blending Option
    Overlay   = (1 << 14),
    Reject    = (1 << 15)
};

[Serializable]
public class SymbolMask
{
    public List<SymbolAttribute> mask;

    public SymbolAttribute GetMask(int symbolIndex)
    {
        if (symbolIndex < mask.Count)
            return mask[symbolIndex];

        return (SymbolAttribute)0;
    }

    public static bool HasWild(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Wild);
    }
    
    public static bool HasBar(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Bar);
    }
    
    public static bool HasSeven(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Seven);
    }

    public static bool HasScatter1(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter1);
    }

    public static bool HasScatter2(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter2);
    }

    public static bool HasScatter3(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter3);
    }

    public static bool HasScatter4(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter4);
    }

    public static bool HasScatter5(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter5);
    }

    public static bool HasScatter6(SymbolInfo symbolInfo)
    {
        return HasAttribute(symbolInfo, SymbolAttribute.Scatter6);
    }
    
    public static bool HasScatter7(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter7);
    }
    
    public static bool HasScatter8(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter8);
    }
    
    public static bool HasScatter9(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter9);
    }
    
    public static bool HasScatter10(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Scatter10);
    }
    
    public static bool HasBlank(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Blank);
    }
    
    public static bool HasOverlay(SymbolInfo symbolInfo)
    {
    	return HasAttribute(symbolInfo, SymbolAttribute.Overlay);
    }

    public static bool HasReject(SymbolInfo symbolInfo)
    {
        return HasAttribute(symbolInfo.mask, SymbolAttribute.Reject);
    }

    public static bool HasAttribute(SymbolInfo symbolInfo, SymbolAttribute attribute)
    {
    	if (HasReject(symbolInfo)) return false;

    	return HasAttribute(symbolInfo.mask, attribute);
    }

    public static bool HasAttribute(SymbolAttribute source, SymbolAttribute mask)
    {
        return (int)(source & mask) == (int)mask;
    }

    public static bool HasAnyAttribute(SymbolAttribute source, SymbolAttribute mask)
    {
        return (int)(source & mask) != 0;
    }

    public static bool HasAttribute(SymbolAttribute source, int mask)
    {
        return (int)((int)source & mask) == mask;
    }
}

}
