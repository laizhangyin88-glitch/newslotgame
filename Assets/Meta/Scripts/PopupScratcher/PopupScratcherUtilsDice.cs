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
        public static void InitScratcherPlayGroupsForDice(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            bool isReward = scratcherInfo.GetVariable<bool>("_isReward")?.value ?? false;

            // Info
            int version = scratcherInfo.GetVariable<int>("version")?.value ?? 0;
            if (version == 1)
            {
                var infoArea = ContextUtils.FindElement(agent, SCRATCHER_INFO_AREA_NAME + " 0", CHILDREN);
                var infoText = ContextUtils.FindElement(infoArea, SCRATCHER_TEXT_INFO_OUTLINE_TEXT, CHILDREN);
                var instantPrize = scratcherInfo.GetValue<long>("instantWinCredit");
                if (isReward)
                {
                    instantPrize = MultiplierUtils.GetRewardMultiplierValue(instantPrize, scratcherInfo, "scratcher");
                }
                MetaContextElementUtils.SetTextGlobal(infoText, "COLLECTING_GAME_INFO_DICE_INSTANT_TEXT", instantPrize);
            }

            int instantWinSymbol = scratcherInfo.GetValue<int>("instantWinSymbol");

            int diceHitSum = scratcherInfo.GetValue<int>("diceHitSum");
            int diceTwiceSum = scratcherInfo.GetValue<int>("diceTwiceSum");

            int numInstantWin = scratcherInfo.GetValue<int>("numInstantWin");
            int numDiceTry = scratcherInfo.GetValue<int>("numDiceTry");

            ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();

            for (int i = 0; i < numInstantWin; i++)
            {
                if (cellInstanceList[i].SymbolId == instantWinSymbol)
                {
                    cellInstanceList[i].AddHighlightCellsSelf();
                }

                playGroup.AddCellInstance(cellInstanceList[i]);
            }

            ScratcherCellPlayGroup playGroup2 = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup playGroup3 = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup winGroup = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup loseGroup = new ScratcherCellPlayGroup(OPEN_ALL_OPTION_SET);

            for (int i = numInstantWin; i < numInstantWin + numDiceTry; i++)
            {
                int sum = int.Parse(cellInstanceList[i].SymbolValue) + int.Parse(cellInstanceList[i + numDiceTry].SymbolValue);

                if (sum == diceHitSum || sum == diceTwiceSum)
                {
                    cellInstanceList[i + numDiceTry].AddHighlightCells(cellInstanceList[i]);
                    cellInstanceList[i + numDiceTry].AddHighlightCellsSelf();
                    cellInstanceList[i + numDiceTry * 2].AddHighlightCellsSelf();

                    winGroup.AddCellInstance(cellInstanceList[i + numDiceTry * 2]);
                }
                else
                {
                    loseGroup.AddCellInstance(cellInstanceList[i + numDiceTry * 2]);
                }

                playGroup2.AddCellInstance(cellInstanceList[i]);
                playGroup3.AddCellInstance(cellInstanceList[i + numDiceTry]);
            }

            scratcherPlayGroups.Add(playGroup);
            scratcherPlayGroups.Add(playGroup2);
            scratcherPlayGroups.Add(playGroup3);

            if (!winGroup.IsCellEmpty())
                scratcherPlayGroups.Add(winGroup);

            if (!loseGroup.IsCellEmpty())
                scratcherPlayGroups.Add(loseGroup);
        }
    }
}
