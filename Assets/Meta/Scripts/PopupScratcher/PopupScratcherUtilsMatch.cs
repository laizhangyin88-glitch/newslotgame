using System.Collections.Generic;
using SlotMaker;
using BagelCode.Scratcher;
using NodeCanvas.Framework;

using static BagelCode.Scratcher.ScratcherCellPlayGroup;

using CellGroup = System.Collections.Generic.List<BagelCode.Scratcher.ScratcherCellController>;

namespace BagelCode
{
    public static partial class PopupScratcherUtils
    {
        private static void InitScratcherPlayGroupsForMatchThreeRow(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            int numRow = scratcherInfo.GetValue<int>("numRow");

            ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();
            playGroup.AddCellInstance(cellInstanceList.GetRange(0, numRow * 3));

            ScratcherCellPlayGroup winGroup = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup loseGroup = new ScratcherCellPlayGroup(OPEN_ALL_OPTION_SET);

            for (int i = 0; i < numRow; i++)
            {
                if (cellInstanceList[i * 3].SymbolId == cellInstanceList[i * 3 + 1].SymbolId && cellInstanceList[i * 3 + 1].SymbolId == cellInstanceList[i * 3 + 2].SymbolId)
                {
                    cellInstanceList[i * 3 + 2].AddHighlightCellsSelf();
                    cellInstanceList[i * 3 + 2].AddHighlightCells(cellInstanceList[i * 3]);
                    cellInstanceList[i * 3 + 2].AddHighlightCells(cellInstanceList[i * 3 + 1]);
                    cellInstanceList[numRow * 3 + i].AddHighlightCellsSelf();
                    winGroup.AddCellInstance(cellInstanceList[numRow * 3 + i]);
                }
                else
                {
                    loseGroup.AddCellInstance(cellInstanceList[numRow * 3 + i]);
                }
            }

            scratcherPlayGroups.Add(playGroup);
            if (!winGroup.IsCellEmpty())
                scratcherPlayGroups.Add(winGroup);

            if (!loseGroup.IsCellEmpty())
                scratcherPlayGroups.Add(loseGroup);
        }

        private static void InitScratcherPlayGroupsForMatchThreePrize(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            bool isReward = scratcherInfo.GetVariable<bool>("_isReward")?.value ?? false;

            int numScratchArea = scratcherInfo.GetValue<int>("numScratchArea");

            ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();
            playGroup.AddCellInstance(cellInstanceList.GetRange(0, numScratchArea));

            ScratcherCellPlayGroup multiplierGroup = new ScratcherCellPlayGroup();
            multiplierGroup.AddCellInstance(cellInstanceList[cellInstanceList.Count - 1]);

            Dictionary<long, int> matchInfo = new Dictionary<long, int>();

            for (int i = 0; i < numScratchArea; i++)
            {
                long prize = cellInstanceList[i].SymbolPrize;
                if (isReward)
                {
                    prize = MultiplierUtils.GetRewardMultiplierValue(prize, scratcherInfo, "scratcher");
                }

                if (matchInfo.ContainsKey(prize))
                {
                    matchInfo[prize] = matchInfo[prize] + 1;
                    if (matchInfo[prize] == 3)
                    {
                        List<int> indexToHighlight = new List<int>();

                        for (int j = 0; j < numScratchArea; j++)
                        {
                            if (cellInstanceList[j].SymbolPrize == prize)
                                indexToHighlight.Add(j);
                        }

                        if (indexToHighlight.Count == 3)
                        {
                            cellInstanceList[indexToHighlight[2]].AddHighlightCells(cellInstanceList[indexToHighlight[0]]);
                            cellInstanceList[indexToHighlight[2]].AddHighlightCells(cellInstanceList[indexToHighlight[1]]);
                            cellInstanceList[indexToHighlight[2]].AddHighlightCellsSelf();
                            cellInstanceList[cellInstanceList.Count - 1].AddHighlightCellsSelf();
                        }

                        break;
                    }
                }
                else
                {
                    matchInfo[prize] = 1;
                }
            }

            scratcherPlayGroups.Add(playGroup);
            scratcherPlayGroups.Add(multiplierGroup);
        }
    }
}
