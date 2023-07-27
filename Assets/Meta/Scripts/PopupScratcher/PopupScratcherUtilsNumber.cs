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
        private static ScratcherCellPlayGroup InitScratcherPlayGroupsForWinningNumbersDefault(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            var winningNumberCount = scratcherInfo.GetValue<int>("winningNumbers");
            var myNumberCount = scratcherInfo.GetValue<int>("myNumbers");

            CellGroup winningNumbers = cellInstanceList.GetRange(0, winningNumberCount);
            CellGroup myNumbers = cellInstanceList.GetRange(winningNumberCount, myNumberCount);

            ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();
            playGroup.AddCellInstance(winningNumbers);

            ScratcherCellPlayGroup playGroup2 = new ScratcherCellPlayGroup();
            playGroup2.AddCellInstance(myNumbers);

            ScratcherCellPlayGroup winGroup = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup loseGroup = new ScratcherCellPlayGroup(OPEN_ALL_OPTION_SET);

            bool found;
            for (int i = 0; i < myNumbers.Count; i++)
            {
                found = false;

                for (int j = 0; j < winningNumbers.Count; j++)
                {
                    if (myNumbers[i].SymbolId == winningNumbers[j].SymbolId)
                    {
                        myNumbers[i].AddHighlightCellsSelf();
                        myNumbers[i].AddHighlightCells(winningNumbers[j]);
                        winGroup.AddCellInstance(myNumbers[i]);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    loseGroup.AddCellInstance(myNumbers[i]);
                }
            }

            scratcherPlayGroups.Add(playGroup);
            scratcherPlayGroups.Add(playGroup2);

            if (!winGroup.IsCellEmpty())
                scratcherPlayGroups.Add(winGroup);

            if (!loseGroup.IsCellEmpty())
                scratcherPlayGroups.Add(loseGroup);

            return winGroup;
        }

        private static void InitScratcherPlayGroupsForWinningNumbersMultiplier(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            var winningNumberCount = scratcherInfo.GetValue<int>("winningNumbers");
            var myNumberCount = scratcherInfo.GetValue<int>("myNumbers");

            var winningNumbersMultiplierSymbolList = scratcherInfo.GetValue<List<int>>("winningNumbersMultiplierSymbolList");

            CellGroup winningNumbers = cellInstanceList.GetRange(0, winningNumberCount);
            CellGroup myNumbers = cellInstanceList.GetRange(winningNumberCount, myNumberCount);

            ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();
            playGroup.AddCellInstance(winningNumbers);

            ScratcherCellPlayGroup playGroup2 = new ScratcherCellPlayGroup();
            playGroup2.AddCellInstance(myNumbers);

            ScratcherCellPlayGroup winGroup = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup loseGroup = new ScratcherCellPlayGroup(OPEN_ALL_OPTION_SET);

            bool isMultiplier, found;

            for (int i = 0; i < myNumbers.Count; i++)
            {
                isMultiplier = false;

                for (int j = 0; j < winningNumbersMultiplierSymbolList.Count; j++)
                {
                    if (myNumbers[i].SymbolId == winningNumbersMultiplierSymbolList[j])
                    {
                        myNumbers[i].AddHighlightCellsSelf();
                        isMultiplier = true;
                        winGroup.AddCellInstance(myNumbers[i]);
                        break;
                    }
                }

                if (isMultiplier) continue;

                found = false;

                for (int j = 0; j < winningNumbers.Count; j++)
                {
                    if (myNumbers[i].SymbolId == winningNumbers[j].SymbolId)
                    {
                        myNumbers[i].AddHighlightCellsSelf();
                        myNumbers[i].AddHighlightCells(winningNumbers[j]);
                        winGroup.AddCellInstance(myNumbers[i]);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    loseGroup.AddCellInstance(myNumbers[i]);
                }
            }

            scratcherPlayGroups.Add(playGroup);
            scratcherPlayGroups.Add(playGroup2);

            if (!winGroup.IsCellEmpty())
                scratcherPlayGroups.Add(winGroup);

            if (!loseGroup.IsCellEmpty())
                scratcherPlayGroups.Add(loseGroup);
        }

        private static void InitScratcherPlayGroupsForWinningNumbersDynamicRows(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            var winningNumberCount = scratcherInfo.GetValue<int>("winningNumbers");
            var myNumberCount = scratcherInfo.GetValue<int>("myNumbers");
            var rowNumList = scratcherInfo.GetValue<List<int>>("rowNumList");

            CellGroup winningNumbers = cellInstanceList.GetRange(0, winningNumberCount);
            CellGroup myNumbers = cellInstanceList.GetRange(winningNumberCount, myNumberCount);

            ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();
            playGroup.AddCellInstance(winningNumbers);

            ScratcherCellPlayGroup playGroup2 = new ScratcherCellPlayGroup();
            playGroup2.AddCellInstance(myNumbers);

            ScratcherCellPlayGroup winGroup = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup loseGroup = new ScratcherCellPlayGroup(OPEN_ALL_OPTION_SET);

            bool found;
            int startIndex = 3;

            for (int i = 0; i < rowNumList.Count; i++)
            {
                found = false;

                for (int j = 0; j < rowNumList[i]; j++)
                {
                    for (int k = 0; k < winningNumberCount; k++)
                    {
                        if (cellInstanceList[startIndex + j].SymbolId == cellInstanceList[k].SymbolId)
                        {
                            cellInstanceList[startIndex + j].AddHighlightCells(cellInstanceList[k]);
                            cellInstanceList[startIndex + j].AddHighlightCellsSelf();
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    winGroup.AddCellInstance(cellInstanceList[winningNumberCount + myNumberCount + i]);
                    cellInstanceList[winningNumberCount + myNumberCount + i].AddHighlightCellsSelf();
                }
                else
                    loseGroup.AddCellInstance(cellInstanceList[winningNumberCount + myNumberCount + i]);

                startIndex += rowNumList[i];
            }

            scratcherPlayGroups.Add(playGroup);
            scratcherPlayGroups.Add(playGroup2);

            if (!winGroup.IsCellEmpty())
                scratcherPlayGroups.Add(winGroup);

            if (!loseGroup.IsCellEmpty())
                scratcherPlayGroups.Add(loseGroup);
        }

        private static void InitScratcherPlayGroupsForWinningNumbersGlobalMultiplier(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            ScratcherCellPlayGroup winGroup = InitScratcherPlayGroupsForWinningNumbersDefault(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);

            ScratcherCellPlayGroup multiplierGroup = new ScratcherCellPlayGroup();
            multiplierGroup.AddCellInstance(cellInstanceList[cellInstanceList.Count - 1]);

            scratcherPlayGroups.Add(multiplierGroup);

            if (!winGroup.IsCellEmpty())
                cellInstanceList[cellInstanceList.Count - 1].AddHighlightCellsSelf();
        }
    }
}
