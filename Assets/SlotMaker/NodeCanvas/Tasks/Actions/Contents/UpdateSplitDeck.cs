using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateSplitDeck : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<List<int>> indices;
    public BBParameter<string> targetBB = "./spin/deck";
    [SerializeField] protected bool checkAll;
    [SerializeField] protected List<bool> masks;

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        var deck = (Deck)slotData.deck.Clone();
        deck.stripIndices = indices.value;

        ProcessUpdateSplitDeck(deck, checkAll, masks);

        var variable = BlackboardUtils.GetOrCreateVariable<Deck>(null, targetBB.value);
        variable.value = deck;

        slotData.deck = deck;

        EndAction();
    }

    private void ProcessUpdateSplitDeck(Deck deck, bool checkAll, List<bool> masks)
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        int totalColumn = slotData.column;
        int totalRow = slotData.row;

        deck.deck = new List<List<SymbolInfo>>();
        deck.hitMap = new List<List<bool>>();
        var strips = GlobalReelStrips.Instance.GetReelStrips();
        int stripIndex = 0;

        for (int column = 0; column < totalColumn; ++column)
        {
            var hitReel = new List<bool>();
            var reel = new List<SymbolInfo>();
            if (checkAll || masks[column])
            {
                for (int row = 0; row < totalRow; ++row)
                {
                    var strip = strips.GetReelStrip(stripIndex);
                    int idx = strip.CalcIndex(deck.stripIndices[stripIndex++]);
                    reel.Add(SlotUtils.GetSymbol(slotIndex.value, column, strip, idx));
                    hitReel.Add(false);
                }
            }
            else
            {
                var strip = strips.GetReelStrip(stripIndex);
                for (int row = 0; row < totalRow; ++row)
                {
                    int idx = strip.CalcIndex(deck.stripIndices[stripIndex] + row);
                    reel.Add(SlotUtils.GetSymbol(slotIndex.value, column, strip, idx));
                    hitReel.Add(false);
                }
                stripIndex++;
            }

            deck.deck.Add(reel);
            deck.hitMap.Add(hitReel);
        }
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
