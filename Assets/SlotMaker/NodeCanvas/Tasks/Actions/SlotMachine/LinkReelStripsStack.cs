using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class LinkReelStripsStack : ActionTask
{
    public BBParameter<int> symbolIndex;
    public BBParameter<int> linkCount;
    public BBParameter<int> reelStripsIndex;

    [SerializeField] protected bool checkAll;
    [SerializeField] protected List<bool> masks;

    protected override string info
    {
        get { return string.Format("Link {0} Stacked Symbols in ReelStrips[{1}], Index {2}", linkCount.value, reelStripsIndex.value, symbolIndex.value); }
    }

    protected override void OnExecute()
    {
        //  이 코드는 ReelStrip이 정확한 정보를 가지고 있다고 가정하고 심볼을 Link해줍니다.
        var strips = GlobalReelStrips.Instance.stripsList[reelStripsIndex.value];

        for (int reelIndex = 0; reelIndex < strips.reelCount; ++reelIndex)
        {
            if (IsIgnoreColumn(reelIndex))  continue;

            var reelStrip = strips.GetReelStrip(reelIndex);
            for (int i = 0; i < reelStrip.stripCount; ++i)
            {
                var symbolInfo = reelStrip.GetSymbol(i);
                if (symbolInfo.symbol == symbolIndex.value)
                {
                    for (int j = 0; j < linkCount.value; ++j)
                    {
                        symbolInfo = reelStrip.GetSymbol(i+j);
                        SymbolLink replaceLink = (SymbolLink)symbolInfo.link.Clone();
                        replaceLink.rowCount = linkCount.value;
                        replaceLink.rowOffset = linkCount.value - j - 1;
                        symbolInfo.link = replaceLink;
                    }
                    i += linkCount.value - 1;
                }
            }
        }

        EndAction();
    }

    private bool IsIgnoreColumn(int col)
    {
        if (checkAll)
            return false;
        else
            return !masks[col];
    }

    #if UNITY_EDITOR
    protected override void OnTaskInspectorGUI()
    {
        DrawDefaultInspector();

        checkAll = UnityEditor.EditorGUILayout.Toggle("Check All", checkAll);
        if (!checkAll)
            masks = (List<bool>)EditorUtils.ReflectedFieldInspector("Masks", masks, typeof(List<bool>));
    }
    #endif
}

}
