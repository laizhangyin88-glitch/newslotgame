using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using SimpleJSON;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/SlotMachine")]
    public class FindScatterSymbolAtWinLineOnceNew : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public SymbolAttribute symbolMask;

        public BBParameter<int> symbol;

        public bool ignoreOverlay = false;

        public BBParameter<SymbolWin> saveAs;

        protected override string info
        {
            get { return string.Format("Find Scatter({0}) at win line <new>", symbolMask); }
        }

        protected override void OnExecute()
        {
            /*
            Blackboard cb = ContentBlackboard.Get();

            Dictionary<int, int> changeCode = cb.GetValue<Blackboard>("gameNew").GetValue<Dictionary<int, int>>("changeCode");

            int value = symbol.value;
            foreach (var item in changeCode)
            {
                if (item.Value == symbol.value)
                {
                    value = item.Key;
                }
            }

            int lineIndex = -1;
            //Variable<Blackboard> current = cb.GetVariable<Blackboard>("current");
            Variable<Blackboard> spin = cb.GetVariable<Blackboard>("spin");
            string responseNew = spin.value.GetValue<string>("responseNew");
            JSONNode node = JSONNode.Parse(responseNew);
            JSONNode totalResultNode = node["game_result"]["total_result"];
            for (int i = 0; i < totalResultNode.Count; i++)
            {
                JSONNode temp = totalResultNode[i];
                if (temp.HasKey("value") && (int)temp["value"] == value)
                {
                    lineIndex = (int)temp["index"];
                    break;
                }
            }*/


           int lineIndex = BlizzUtils.Utils.new_game_GetLineIndexOnce(symbol.value);

            if (lineIndex == -1)
            {
                Debug.LogError($"can not find symbol {symbol.value} win line");
                EndAction(false);
                return;
            }

            Debug.Log($"==@  find symbol {symbol.value} win line index = {lineIndex} ");


            var payLinesBB = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/payLines").value;
           // int column1 = BlackboardUtils.FindVariable<int>(null, "./customData/slotDataList/0/column").value;
           //  int row2 = BlackboardUtils.FindVariable<int>(null, "./customData/slotDataList/0/row").value;

            List<Blackboard> payLineBBList = payLinesBB[0].GetValue<List<Blackboard>>("value");
            List<int> payLine = payLineBBList[lineIndex].GetValue<List<int>>("value");


            var symbolWin = new SymbolWin();
            //saveAs.value = new List<Cell>();
            Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;

            for (int reelInde = 0; reelInde< payLine.Count; reelInde++)
            {
                int row = payLine[reelInde];
                int column = reelInde;
                var symbolInfo = deck.GetSymbol(column, row);
                if (ignoreOverlay && SymbolMask.HasAttribute(symbolInfo, SymbolAttribute.Overlay)) continue;
                if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
                {
                    //saveAs.value.Add(new Cell(column, row));
                    symbolWin.cells.Add(new Cell(column, row));
                }
            }
            saveAs.value = symbolWin;
            EndAction();
        }

    }

}


/*
private void OnWin(SymbolWin win)
{
    if (win.lineIndex <= 0) return;

    var payLinesBB = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/payLines").value;
    int column = BlackboardUtils.FindVariable<int>(null, "./customData/slotDataList/0/column").value;
    int row = BlackboardUtils.FindVariable<int>(null, "./customData/slotDataList/0/row").value;

    int index = 0;
    int lineIndex = (int)win.lineIndex - 1;
    List<Blackboard> payLineBBList = payLinesBB[0].GetValue<List<Blackboard>>("value");
    List<int> payLine = payLineBBList[lineIndex].GetValue<List<int>>("value");

    int count = column + 1;
    for (int j = 0; j < count; ++j)
    {
        int anchoredPolicy = anchorPolicies[j];
        index = payLine[anchoredPolicy] + row * j;
        payLines[index].SetBool("enable", true);
    }
}
*/
