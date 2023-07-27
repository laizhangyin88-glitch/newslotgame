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

        private static void InitScratcherPlayGroupsForInstantCashDefault(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            int hitSymbol = scratcherInfo.GetValue<int>("hitSymbol");

            ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();
            playGroup.AddCellInstance(cellInstanceList);

            ScratcherCellPlayGroup winGroup = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup loseGroup = new ScratcherCellPlayGroup(OPEN_ALL_OPTION_SET);

            for (int i = 0; i < cellInstanceList.Count; i++)
            {
                if (cellInstanceList[i].SymbolId == hitSymbol)
                {
                    cellInstanceList[i].AddHighlightCellsSelf();

                    winGroup.AddCellInstance(cellInstanceList[i]);
                }
                else
                {
                    loseGroup.AddCellInstance(cellInstanceList[i]);
                }
            }

            scratcherPlayGroups.Add(playGroup);
            if (!winGroup.IsCellEmpty())
                scratcherPlayGroups.Add(winGroup);

            if (!loseGroup.IsCellEmpty())
                scratcherPlayGroups.Add(loseGroup);
        }

        private static void InitScratcherPlayGroupsForInstantCashWinItAll(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            int hitSymbol = scratcherInfo.GetValue<int>("hitSymbol");
            int winItAllSymbol = scratcherInfo.GetValue<int>("winItAllSymbol");

            bool isWinItAll = false;

            ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();
            playGroup.AddCellInstance(cellInstanceList);

            ScratcherCellPlayGroup winGroup = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup loseGroup = new ScratcherCellPlayGroup(OPEN_ALL_OPTION_SET);

            for (int i = 0; i < cellInstanceList.Count; i++)
            {
                if (isWinItAll)
                {
                    cellInstanceList[i].AddHighlightCellsSelf();
                    if (cellInstanceList[i].SymbolId != winItAllSymbol)
                    {
                        winGroup.AddCellInstance(cellInstanceList[i]);
                    }
                }
                else if (cellInstanceList[i].SymbolId == winItAllSymbol)
                {
                    isWinItAll = true;
                    cellInstanceList[i].AddHighlightCells(cellInstanceList.GetRange(0, i + 1));

                    loseGroup.ClearCellInstance();
                    winGroup.ClearCellInstance();
                    for (int j = 0; j < i; j++)
                    {
                        winGroup.AddCellInstance(cellInstanceList[j]);
                    }
                }
                else if (cellInstanceList[i].SymbolId == hitSymbol)
                {
                    cellInstanceList[i].AddHighlightCellsSelf();
                    winGroup.AddCellInstance(cellInstanceList[i]);
                }
                else
                {
                    loseGroup.AddCellInstance(cellInstanceList[i]);
                }
            }

            scratcherPlayGroups.Add(playGroup);

            if (!winGroup.IsCellEmpty())
                scratcherPlayGroups.Add(winGroup);

            if (!loseGroup.IsCellEmpty())
                scratcherPlayGroups.Add(loseGroup);
        }
    }
}
