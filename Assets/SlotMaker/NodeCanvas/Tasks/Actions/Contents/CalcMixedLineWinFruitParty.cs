using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using System.Runtime.CompilerServices;
using SimpleJSON;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class CalcMixedLineWinFruitParty : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<List<Blackboard>> payTable;
        public BBParameter<List<Blackboard>> payLine;
        public BBParameter<long> multiplier;
        public BBParameter<List<long>> symbolMultipliers;
        public OperationMethod MultiplierOperation = OperationMethod.Multiply;
        public BBParameter<bool> bidirectional;
        public BBParameter<bool> excludeMaxLine = false;

        public BBParameter<List<SymbolWin>> saveAs;

        protected List<MixedLineWinInfo> mixedLineWinInfos;
        protected List<SymbolWin> winList;
        private const int wildSymbolIndex = 0;
        private JSONNode res;
        private List<Dictionary<string, object>> dict;

        protected long GetSymbolMultiplier(int symbolIndex)
        {
            if (MultiplierOperation == OperationMethod.Add)
            {
                if (symbolMultipliers.isNone || symbolMultipliers.isNull || symbolMultipliers.value[symbolIndex] == 1)
                    return 0L;
            }
            if (symbolMultipliers.isNone || symbolMultipliers.isNull)
                return 1L;

            return symbolMultipliers.value[symbolIndex];
        }

        public List<int> GetPayLine(int lineIndex)
        {
            return payLine.value[lineIndex].GetValue<List<int>>("value");
        }

        private int GetPayLineIndex(List<int> line)
        {

            for (int i = 0; i < payLine.value.Count; i++)
            {
                var tempLine = GetPayLine(i);
                int count = 0;
                for (int j = 0; j < tempLine.Count; j++)
                {
                    if ((tempLine[j] == line[j]))
                    {
                        count++;
                        if (count >= 5)
                        {
                            return i;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return 0;
        }

        private long getLineCredit(int lineIndex)
        {
            long result = 0;
            if (dict == null)
            {
                dict = new List<Dictionary<string, object>>();
                for (int i = 0; i < res["result"]["win_line_reward_list"].Count; i++)
                {
                    var list = res["result"]["win_line_reward_list"][i];
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    if (list["index"] != null)
                    {
                        temp.Add("index", list["index"]);
                    }
                    if (list["credit"] != null)
                    {
                        temp.Add("credit", list["credit"]);
                    }
                    dict.Add(temp);
                }
            }
            for (int i = 0; i < dict.Count; i++)
            {
                var temp = dict[i];
                if (temp.TryGetValue("index", out object value))
                {

                    if (int.Parse(value.ToString()) == lineIndex)
                    {
                        if (temp.TryGetValue("credit", out object credit))
                        {
                            result = long.Parse(credit.ToString());
                            break;
                        }
                    }
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 转换牌型数据 3x5 转换为  5x3
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private int[,] TransposeArray(List<List<int>> input)
        {
            int rows = input.Count; //5
            int cols = input[0].Count;//3 
            var result = new int[cols, rows];
            for (int i = 0; i < input.Count; i++)
            {
                var temp = input[i];
                for (int j = 0; j < temp.Count; j++)
                {
                    result[j, i] = temp[j];
                }
            }
            return result;
        }

        private List<Cell> fillCell(List<int> line)
        {
            List<Cell> result = new List<Cell>();
            for (int i = 0; i < 5; i++)
            {
                Cell cell = new Cell(i, line[i]);
                result.Add(cell);
            }
            return result;
        }

        private long FillLineData(List<SymbolWin> winList, int dircetion)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = (Deck)slotData.deck.Clone();
            Variable<Blackboard> spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            string responseNew = spinBB.value.GetValue<string>("responseNew");
            res = JSONNode.Parse(responseNew);

            //Debug.LogError("拉霸下发数据......." + res.ToString());
            //Debug.LogError("拉霸结果....... + " + res["game_result"]["first_index_list"].ToString());

            //for (int i = 0; i < res["game_result"]["total_result"].Count; i++)
            //{
            //    var temp = res["game_result"]["total_result"][i];
            //    if(temp != null && temp["win_line"] != null)
            //    {
            //        Debug.LogError(temp["win_line"].ToString());
            //    }
            //}

            JSONNode lineResult = res["game_result"]["total_result"];
            var list = new List<List<int>>();
            for (int i = 0; i < lineResult.Count; i++)
            {
                var temp = lineResult[i];
                if (temp["win_line"] != null)
                {   ///保存连线的结果
                    var list1 = new List<int>();
                    for (int j = 0; j < temp["win_line"].Count; j++)
                    {
                        list1.Add(temp["win_line"][j]);
                    } 
                    list.Add(list1);
                }
            }
            var shuffling_list = res["game_result"]["shuffling_list"];
            var tempNewList = new List<List<int>>();
            for (int i = 0; i < shuffling_list.Count; i++)
            {
                var ttt = shuffling_list[i];
                var tList = new List<int>();
                for (global::System.Int32 j = 0; j < ttt.Count; j++)
                {
                    tList.Add(ttt[j]);
                }
                tempNewList.Add(tList);
            }
           
            var newList = TransposeArray(tempNewList);

            long totalEarnCredit = 0L;
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    List<int> line = list[i];
                    var cellLine = fillCell(line);
                    SymbolInfo temp = null;
                    int hitCount = 0;
                    SymbolWin symbolWin = new SymbolWin();
                    symbolWin.direction = dircetion;
                    int lineIndex = GetPayLineIndex(line);
                    symbolWin.lineIndex = lineIndex + 1;
                    for (int j = 0; j < cellLine.Count; j++)
                    {
                        var cell = cellLine[j];
                        if (temp == null)
                        {
                            temp = deck.deck[cell.column][cell.row];
                            hitCount++;
                            symbolWin.symbolIndex = temp.symbol;
                            symbolWin.cells.Add(new Cell(j, line[j]));
                        }
                        else
                        {
                            if (temp.symbol == deck.deck[cell.column][cell.row].symbol || newList[cell.column, cell.row] == 9)
                            {
                                symbolWin.cells.Add(new Cell(j, line[j]));
                                hitCount++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    long lineMultiplier = 1L;
                    lineMultiplier = (long)OperationTools.Operate(lineMultiplier, GetSymbolMultiplier(temp.symbol), MultiplierOperation);
                    if (MultiplierOperation == OperationMethod.Add && lineMultiplier != 1L)
                        --lineMultiplier;
                    symbolWin.hitCount = hitCount;
                    symbolWin.multiplier = multiplier.value * lineMultiplier;
                    //long earnCredit = FindEarnCredit(temp.symbol, hitCount);
                    symbolWin.earnCredit = getLineCredit(lineIndex); //earnCredit * betPerLine * symbolWin.multiplier;
                    totalEarnCredit += symbolWin.earnCredit;
                    winList.Add(symbolWin);
                }
            }
            return totalEarnCredit;
        }

        /// <summary>
        /// 查找铃铛得分
        /// </summary>
        /// <param name="list"></param>
        private void FindBell(List<SymbolWin> list)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = (Deck)slotData.deck.Clone();
            int count = 0;
            SymbolWin symbolWin = new SymbolWin();
            for (int i = 0; i < deck.deck.Count; i++)
            {
                List<SymbolInfo> infos = deck.deck[i];
                for (int j = 0; j < infos.Count; j++)
                {
                    SymbolInfo info = infos[j];
                    if (info.symbol == 9)///水果派对，9是免费游戏牌
                    {
                        count++;
                        symbolWin.symbolIndex = info.symbol;
                        symbolWin.hitCount = count;
                        symbolWin.cells.Add(new Cell(i, j));
                    }
                }
            }
            if (count == 2)//有两张以上的免费牌
            {
                Debug.LogError("摇到两个免费牌");
                long temp = res["game_result"]["free_game_credit"].AsLong;
                symbolWin.earnCredit = temp;
                symbolWin.multiplier = 1;
                symbolWin.lineIndex = 0;
                winList.Add(symbolWin);
            }
            if (count >= 3)
            {
                Debug.LogError("触发免费游戏了......");
                winList.Add(symbolWin);
                symbolWin.earnCredit = 0;
            }
        }
        /// <summary>
        /// 查找是不是触发小游戏
        /// </summary>
        protected void FindMiniGame(List<SymbolWin> list)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = (Deck)slotData.deck.Clone();
            int count = 0;
            SymbolWin symbolWin = new SymbolWin();
            for (int i = 0; i < deck.deck.Count; i++)
            {
                List<SymbolInfo> infos = deck.deck[i];
                for (int j = 0; j < infos.Count; j++)
                {
                    SymbolInfo info = infos[j];
                    if (info.symbol == 10)///水果派对，10是小游戏牌
                    {
                        count++;
                        symbolWin.symbolIndex = info.symbol;
                        symbolWin.hitCount = count;
                        symbolWin.multiplier = 1;
                        symbolWin.cells.Add(new Cell(i, j));
                    }
                }
            }
            if (count >= 3)
            {
                list.Add(symbolWin);
            }
        }

        private void InitData()
        {

        }
         
        protected override void OnExecute()
        {
            InitData();
            mixedLineWinInfos = ContentCustomData.GetSlotData(slotIndex.value).mixedLineWinInfos;
            long totalEarnCredit = 0L;
            winList = new List<SymbolWin>();

            totalEarnCredit += FillLineData(winList, 1);
            if (bidirectional.value)
                totalEarnCredit += FillLineData(winList, -1);

            FindBell(winList);
            FindMiniGame(winList);
            ///使用服务器下发的数据
            var total = res["game_result"]["earn_credit"].AsLong;
            Debug.LogError("服务器下发的赢钱数值....." + total);
            totalEarnCredit = total;

            winList.Sort();
            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "winList", winList);
            ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);
            Blackboard bb = ContentBlackboard.Get();
            //bb.SetValue("earnCredit", totalEarnCredit);
            //bb.GetVariable<long>("earnCredit").value = totalEarnCredit;
            BlackboardUtils.GetOrCreateVariable<long>(bb, "earnCredit").value = totalEarnCredit;
            saveAs.value = winList;
            if(winList.Count > 0)
            {
                EventSender.SendGlobalEvent(new EventData("Win")); 
            }
            EndAction();
        }
    }

}
