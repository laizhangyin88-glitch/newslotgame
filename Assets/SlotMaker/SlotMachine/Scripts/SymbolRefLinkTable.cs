using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{

[Serializable]
public class SymbolRefLinkTable
{
    public List<List<SymbolLink>> linkTable = new List<List<SymbolLink>>();

    public void Initialize(int linkTableCount)
    {
        linkTable.Clear();
        for (int i = 0; i < linkTableCount; ++i)
        {
            CreateSymbolRefLink(1);
        }
    }

    public void CreateSymbolRefLink(int rowCount)
    {
        List<SymbolLink> linkList = new List<SymbolLink>();
        for (int rowOffset = 0; rowOffset < rowCount; ++rowOffset)
        {
            SymbolLink newLink = new SymbolLink();
            newLink.columnCount = 1;
            newLink.columnOffset = 0;
            newLink.rowCount = rowCount;
            newLink.rowOffset = rowOffset;
            linkList.Add(newLink);
        }
        linkTable.Add(linkList);
    }

    public void InsertSymbolRefLink(int linkIndex, int rowCount)
    {
        List<SymbolLink> linkList = new List<SymbolLink>();
        for (int rowOffset = 0; rowOffset < rowCount; ++rowOffset)
        {
            SymbolLink newLink = new SymbolLink();
            newLink.columnCount = 1;
            newLink.columnOffset = 0;
            newLink.rowCount = rowCount;
            newLink.rowOffset = rowOffset;
            linkList.Add(newLink);
        }
        linkTable.Insert(linkIndex, linkList);
    }

    public int GetSymbolRefLinkCount(int linkIndex)
    {
        return linkTable[linkIndex].Count;
    }

    public SymbolLink GetSymbolRefLink(int linkIndex, int rowOffset)
    {
        return linkTable[linkIndex][rowOffset];
    }
}

}
