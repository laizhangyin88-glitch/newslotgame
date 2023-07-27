using System.Collections.Generic;
using SlotMaker;
using BagelCode.Scratcher;
using NodeCanvas.Framework;
using System.Linq;

using static BagelCode.Scratcher.ScratcherCellPlayGroup;

using CellGroup = System.Collections.Generic.List<BagelCode.Scratcher.ScratcherCellController>;
using SymbolGroup = System.Collections.Generic.List<int>;

namespace BagelCode
{
    public static partial class PopupScratcherUtils
    {
        private static void InitScratcherPlayGroupsForKenoLine(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            bool isReward = scratcherInfo.GetVariable<bool>("_isReward")?.value ?? false;

            int COLUMN = 5;
            int ROW = 4;
            int MIN_HIT_FOR_WIN = 3;

            SymbolGroup accumulatedHitSymbols = new SymbolGroup();

            // Info
            int version = scratcherInfo.GetVariable<int>("version")?.value ?? 0;
            if (version == 1)
            {
                int maxMatches = 8;
                int ruleLines = 6;
                var ruleLineTextList = new List<string>();
                var prizeLineTextList = new List<string>();

                List<long> prizeList = scratcherInfo.GetValue<List<long>>("prizeList");
                prizeList.Reverse();

                for (int i = 0; i < ruleLines; ++i)
                {
                    long prize = prizeList[i];
                    if (isReward)
                    {
                        prize = MultiplierUtils.GetRewardMultiplierValue(prize, scratcherInfo, "scratcher");
                    }

                    int matches = maxMatches - i;

                    var ruleLineText = StringTableUtils.GetString(GLOBAL,
                        "COLLECTING_GAME_INFO_KENO_RULE_BIG_TEXT", matches, maxMatches);
                    ruleLineTextList.Add(ruleLineText);

                    var prizeLineText = StringTableUtils.GetString(GLOBAL,
                        "COLLECTING_GAME_INFO_KENO_LINE_PRIZE_BIG_TEXT", prize);
                    prizeLineTextList.Add(prizeLineText);
                }

                var infoTexts = new ContextElement[6];
                for (int i = 0; i < ruleLines; ++i)
                {
                    var infoArea = ContextUtils.FindElement(agent, SCRATCHER_INFO_AREA_NAME + " " + i, CHILDREN);
                    infoTexts[i] = ContextUtils.FindElement(infoArea, SCRATCHER_TEXT_INFO_TEXT, CHILDREN);

                    bool isRule = i % 2 == 0; // false is prize
                    int row = i / 2;
                    int column = 2;

                    var textList = isRule ? ruleLineTextList : prizeLineTextList;
                    string text = string.Join("\r\n", textList.GetRange(row * column, column));
                    MetaContextElementUtils.SetText(infoTexts[i], text);
                }
            }

            // Hit Symbol
            List<SymbolGroup> hitSymbolGroupList = new List<SymbolGroup>();
            int idx = 0;
            for (int i = 0; i < ROW; ++i)
            {
                SymbolGroup symbols = cellInstanceList.GetRange(idx, COLUMN).Select(c => c.SymbolId).ToList();
                hitSymbolGroupList.Add(symbols);
                idx += COLUMN;
            }
            CellGroup callerCellGroup = cellInstanceList.GetRange(0, idx);

            // Line
            int columnCount = scratcherInfo.GetValue<int>("columnCount");
            int length = scratcherInfo.GetValue<int>("columnLength");
            List<CellGroup> lineCellGroupList = new List<CellGroup>();
            for (int i = 0; i < columnCount; ++i)
            {
                CellGroup cells = cellInstanceList.GetRange(idx, length);
                idx += length;
                lineCellGroupList.Add(cells);
            }

            // Play Group Setting
            foreach (var hitSymbols in hitSymbolGroupList)
            {
                // Play Caller
                CellGroup callerHitCells = FindHitCell(hitSymbols, callerCellGroup);
                AddPlayGroup(scratcherPlayGroups, callerHitCells, DEFAULT_OPTION_SET);

                accumulatedHitSymbols.AddRange(hitSymbols);

                // Play Line
                ScratcherCellPlayGroup hitNumberPlayGroup = null;
                ScratcherCellPlayGroup winLinePlayGroup = null;
                for (int i = 0; i < columnCount; ++i)
                {
                    CellGroup lineCells = lineCellGroupList[i];
                    CellGroup lineHitCells = FindHitCell(hitSymbols, lineCells);

                    if (hitNumberPlayGroup == null)
                        hitNumberPlayGroup = AddPlayGroup(scratcherPlayGroups, lineHitCells, NON_HIGHLIGHT_OPTION_SET);
                    else
                        AppendPlayGroup(hitNumberPlayGroup, lineHitCells);

                    // Play Highlight
                    CellGroup accumulatedLineHitCells = FindHitCell(accumulatedHitSymbols, lineCells);
                    if (accumulatedLineHitCells.Count >= MIN_HIT_FOR_WIN)
                    {
                        AddHighlightToCells(lineCells);
                        if (winLinePlayGroup == null)
                            winLinePlayGroup = AddPlayGroup(scratcherPlayGroups, lineCells, ONLY_HIGHLIGHT_OPTION_SET);
                        else
                            AppendPlayGroup(winLinePlayGroup, lineCells);
                    }
                }
            }

            // Multiplier
            var multCell = cellInstanceList.Last();
            AddHighlightToCell(multCell);
            AddPlayGroup(scratcherPlayGroups, multCell, DEFAULT_OPTION_SET);
        }

        private static void InitScratcherPlayGroupsForKenoCube(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent, bool isBig)
        {
            bool isReward = scratcherInfo.GetVariable<bool>("_isReward")?.value ?? false;

            int COLUMN, ROW, BONUS_COUNT;
            if (isBig)
            {
                COLUMN = 5;
                ROW = 4;
                BONUS_COUNT = 4;
            }
            else
            {
                COLUMN = 10;
                ROW = 2;
                BONUS_COUNT = 2;
            }

            // Info
            int version = scratcherInfo.GetVariable<int>("version")?.value ?? 0;
            if (version == 1)
            {
                int maxMatches = 10;
                int ruleLines = isBig ? 6 : 7;
                List<string> ruleLineTextList = new List<string>();
                List<string> prizeLineTextList = new List<string>();
                List<long> prizeList = scratcherInfo.GetValue<List<long>>("prizeList");
                prizeList.Reverse();

                for (int i = 0; i < ruleLines; ++i)
                {
                    long prize = prizeList[i];
                    if (isReward)
                    {
                        prize = MultiplierUtils.GetRewardMultiplierValue(prize, scratcherInfo, "scratcher");
                    }

                    int matches = maxMatches - i;

                    var ruleLineText = StringTableUtils.GetString(GLOBAL,
                        isBig ? "COLLECTING_GAME_INFO_KENO_RULE_BIG_TEXT" : "COLLECTING_GAME_INFO_KENO_RULE_SMALL_TEXT",
                        matches, maxMatches);
                    ruleLineTextList.Add(ruleLineText);

                    var prizeLineText = StringTableUtils.GetString(GLOBAL,
                        isBig ? "COLLECTING_GAME_INFO_KENO_BONUS_PRIZE_BIG_TEXT" : "COLLECTING_GAME_INFO_KENO_BONUS_PRIZE_SMALL_TEXT",
                        prize);

                    prizeLineTextList.Add(prizeLineText);
                }

                if (isBig)
                {
                    ContextElement[] infoTexts = new ContextElement[4];
                    for (int i = 0; i < 4; ++i)
                    {
                        var infoArea = ContextUtils.FindElement(agent, SCRATCHER_INFO_AREA_NAME + " " + i, CHILDREN);
                        infoTexts[i] = ContextUtils.FindElement(infoArea, SCRATCHER_TEXT_INFO_TEXT, CHILDREN);
                    }

                    var rule1 = string.Join("\r\n", ruleLineTextList.GetRange(0, 3));
                    MetaContextElementUtils.SetText(infoTexts[0], rule1);
                    var prize1 = string.Join("\r\n", prizeLineTextList.GetRange(0, 3));
                    MetaContextElementUtils.SetText(infoTexts[1], prize1);
                    var rule2 = string.Join("\r\n", ruleLineTextList.GetRange(3, 3));
                    MetaContextElementUtils.SetText(infoTexts[2], rule2);
                    var prize2 = string.Join("\r\n", prizeLineTextList.GetRange(3, 3));
                    MetaContextElementUtils.SetText(infoTexts[3], prize2);
                }
                else
                {
                    ContextElement[] infoTexts = new ContextElement[2];
                    for (int i = 0; i < 2; ++i)
                    {
                        var infoArea = ContextUtils.FindElement(agent, SCRATCHER_INFO_AREA_NAME + " " + i, CHILDREN);
                        infoTexts[i] = ContextUtils.FindElement(infoArea, SCRATCHER_TEXT_INFO_TEXT, CHILDREN);
                    }

                    var rule1 = string.Join("\r\n\r\n", ruleLineTextList.GetRange(0, ruleLines));
                    MetaContextElementUtils.SetText(infoTexts[0], rule1);
                    var prize1 = string.Join("\r\n\r\n", prizeLineTextList.GetRange(0, ruleLines));
                    MetaContextElementUtils.SetText(infoTexts[1], prize1);
                }
            }

            SymbolGroup accumulatedHitSymbols = new SymbolGroup();

            // Hit Symbol
            List<SymbolGroup> hitSymbolGroupList = new List<SymbolGroup>();
            int idx = 0;
            for (int i = 0; i < ROW; ++i)
            {
                SymbolGroup symbols = cellInstanceList.GetRange(idx, COLUMN).Select(c => c.SymbolId).ToList();
                hitSymbolGroupList.Add(symbols);
                idx += COLUMN;
            }
            CellGroup callerCellGroup = cellInstanceList.GetRange(0, idx);

            // Cube
            int cubeNumberCount = scratcherInfo.GetValue<int>("numberCount");
            CellGroup cubeCellGroup = cellInstanceList.GetRange(idx, cubeNumberCount);
            idx += cubeNumberCount;

            // Bonus
            var bonusSymbolGroupBBList = scratcherInfo.GetValue<List<Blackboard>>("winningNumbersAroundCube");
            var bonusSymbolGroupList = new List<SymbolGroup>();
            foreach (var group in bonusSymbolGroupBBList)
                bonusSymbolGroupList.Add(group.GetValue<List<int>>("numbers"));
            CellGroup bonusPrizeCells = cellInstanceList.GetRange(idx, BONUS_COUNT);
            CellGroup playedBonusPrizeCells = new CellGroup();

            // Play Group Setting
            foreach (var hitSymbols in hitSymbolGroupList)
            {
                // Play Caller
                CellGroup callerHitCells = FindHitCell(hitSymbols, callerCellGroup);
                AddPlayGroup(scratcherPlayGroups, callerHitCells, DEFAULT_OPTION_SET);

                // Play Cube
                CellGroup cubeHitCells = FindHitCell(hitSymbols, cubeCellGroup);
                AddPlayGroup(scratcherPlayGroups, cubeHitCells, DEFAULT_OPTION_SET);

                accumulatedHitSymbols.AddRange(hitSymbols);

                // Check Bonus Number
                for (int i = 0; i < BONUS_COUNT; ++i)
                {
                    SymbolGroup bonusSymbols = bonusSymbolGroupList[i];
                    SymbolGroup hitBonusSymbols = FindHitSymbol(hitSymbols, bonusSymbols);

                    // Highlight Bonus
                    CellGroup hitBonusCells = FindHitCell(hitBonusSymbols, cubeCellGroup);
                    AddHighlightToCells(hitBonusCells);

                    // Check Winning Bonus
                    var bonusPrizeCell = bonusPrizeCells[i];
                    if (bonusSymbols.All(s => accumulatedHitSymbols.Contains(s)) &&
                        !playedBonusPrizeCells.Contains(bonusPrizeCell))
                    {
                        playedBonusPrizeCells.Add(bonusPrizeCell);
                        AddHighlightToCell(bonusPrizeCell);
                        AddPlayGroup(scratcherPlayGroups, bonusPrizeCell, DEFAULT_OPTION_SET);
                    }
                }
            }
        }

        private static void InitScratcherPlayGroupsForKenoStepped(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            bool isReward = scratcherInfo.GetVariable<bool>("_isReward")?.value ?? false;

            int COLUMN = 10;
            int ROW = 2;

            SymbolGroup accumulatedHitSymbols = new SymbolGroup();
            CellGroup winPrizeCells = new CellGroup();

            // Info
            int version = scratcherInfo.GetVariable<int>("version")?.value ?? 0;
            if (version == 1)
            {
                var prizeList = scratcherInfo.GetValue<List<long>>("prizeList");
                int prizeCount = prizeList.Count;

                for (int i = 0; i < prizeCount; ++i)
                {
                    long prize = prizeList[i];
                    if (isReward)
                    {
                        prize = MultiplierUtils.GetRewardMultiplierValue(prize, scratcherInfo, "scratcher");
                    }

                    var infoArea = ContextUtils.FindElement(agent, SCRATCHER_INFO_AREA_NAME + " " + i, CHILDREN);
                    var infoText = ContextUtils.FindElement(infoArea, SCRATCHER_TEXT_INFO_OUTLINE_TEXT, CHILDREN);

                    MetaContextElementUtils.SetTextGlobal(infoText,
                        "COLLECTING_GAME_INFO_KENO_STEPPED_PRIZE_TEXT",
                        prize);
                }
            }

            // Hit Symbol
            List<SymbolGroup> hitSymbolGroupList = new List<SymbolGroup>();
            int idx = 0;
            for (int i = 0; i < ROW; ++i)
            {
                SymbolGroup symbols = cellInstanceList.GetRange(idx, COLUMN).Select(c => c.SymbolId).ToList();
                hitSymbolGroupList.Add(symbols);
                idx += COLUMN;
            }
            CellGroup callerCellGroup = cellInstanceList.GetRange(0, idx);

            // Row
            int rowCount = scratcherInfo.GetValue<int>("rowCount");
            List<CellGroup> rowCellGroupList = new List<CellGroup>();
            CellGroup prizeCells = new CellGroup();
            for (int i = 0; i < rowCount; ++i)
            {
                int cellCount = i + 1;
                CellGroup cells = cellInstanceList.GetRange(idx, cellCount);
                rowCellGroupList.Add(cells);
                idx += cellCount;

                prizeCells.AddRange(cellInstanceList.GetRange(idx++, 1));
            }

            // Play Group Setting
            foreach (var hitSymbols in hitSymbolGroupList)
            {
                // Play Caller
                CellGroup callerHitCells = FindHitCell(hitSymbols, callerCellGroup);
                AddPlayGroup(scratcherPlayGroups, callerHitCells, DEFAULT_OPTION_SET);

                accumulatedHitSymbols.AddRange(hitSymbols);

                // Row
                ScratcherCellPlayGroup rowPlayGroup = null;
                ScratcherCellPlayGroup winRowPlayGroup = null;
                for (int i = 0; i < rowCellGroupList.Count; ++i)
                {
                    CellGroup rowCells = rowCellGroupList[i];
                    CellGroup rowHitCells = FindHitCell(hitSymbols, rowCells);

                    // Play Row
                    if (rowPlayGroup == null)
                        rowPlayGroup = AddPlayGroup(scratcherPlayGroups, rowHitCells, NON_HIGHLIGHT_OPTION_SET);
                    else
                        AppendPlayGroup(rowPlayGroup, rowHitCells);

                    // Highlight Prize & Row
                    var prizeCell = prizeCells[i];
                    if (rowCells.All(c => accumulatedHitSymbols.Contains(c.SymbolId)) &&
                        !winPrizeCells.Contains(prizeCell))
                    {
                        winPrizeCells.Add(prizeCell);
                        AddHighlightToCells(rowCells);

                        if (winRowPlayGroup == null)
                            winRowPlayGroup = AddPlayGroup(scratcherPlayGroups, rowCells, ONLY_HIGHLIGHT_OPTION_SET);
                        else
                            AppendPlayGroup(winRowPlayGroup, rowCells);
                    }
                }
            }

            // Prize
            AddHighlightToCells(winPrizeCells);
            AddPlayGroup(scratcherPlayGroups, winPrizeCells, DEFAULT_OPTION_SET);
        }

        private static void InitScratcherPlayGroupsForKenoSteppedMultiplier(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            InitScratcherPlayGroupsForKenoStepped(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);

            var multCell = cellInstanceList.Last();
            AddHighlightToCell(multCell);
            AddPlayGroup(scratcherPlayGroups, multCell, DEFAULT_OPTION_SET);
        }
    }
}
