using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{

[Serializable]
public class SymbolLink : ICloneable
{
	public int columnCount  = 1;
	public int columnOffset = 0;
	public int rowCount     = 1;
	public int rowOffset    = 0;

	public bool isPivot { get { return (columnOffset == 0) && (rowOffset == 0); } }
	public bool unitSymbol { get { return (columnCount + rowCount) == 2; } }

    public object Clone()
    {
        var newLink = new SymbolLink();
        newLink.columnCount = this.columnCount;
        newLink.columnOffset = this.columnOffset;
        newLink.rowCount = this.rowCount;
		newLink.rowOffset = this.rowOffset;
        return newLink;
    }

	public void Reset()
	{
		columnCount  = 1;
		columnOffset = 0;
		rowCount     = 1;
		rowOffset    = 0;
	}
}

}
