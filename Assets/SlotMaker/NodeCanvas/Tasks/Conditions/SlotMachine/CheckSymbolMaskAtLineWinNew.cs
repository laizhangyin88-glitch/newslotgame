using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SimpleJSON;
using SlotMaker;
using BlizzUtils;
using UnityEngine;

namespace SlotMaker.Tasks.Conditions
{

    [Category("★ SlotMaker/SlotMachine")]
    public class CheckSymbolMaskAtLineWinNew : ConditionTask<Transform>
    {
        public BBParameter<int> symbolMask;
        public SymbolAttribute valueB;
        public BBParameter<int> symbol;

        protected override string info { get { return string.Format("CheckSymbolMask({0}, {1}) && in line win (new)", symbolMask, valueB); } }

        protected override bool OnCheck()
        {
            var temp = SymbolMask.HasAttribute((SymbolAttribute)symbolMask.value, valueB);
            Cell cell = null;
            int lineIndex = -1;
            if (temp)
            {
                lineIndex =  Utils.new_game_GetLineIndexOnce(symbol.value);

                if (lineIndex != -1)
                {
                    var payLinesBB = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/payLines").value;
                    List<Blackboard> payLineBBList = payLinesBB[0].GetValue<List<Blackboard>>("value");
                    List<int> payLine = payLineBBList[lineIndex].GetValue<List<int>>("value");

                    cell = Utils.GetVisibleSymbolColumnRow(agent);

                    temp = payLine[cell.column] == cell.row;
                }
                else
                {
                    temp = false;
                }
            }

            //if (cell != null)
            //{
            //    debug.logerror($"cell.c ={cell.column} cell.r={cell.row}  lineindex = {lineindex}");
            //}

            return temp;
        }



    }




}
