using System.Collections.Generic;
using SlotMaker;
using BagelCode.Scratcher;
using NodeCanvas.Framework;
using System.Linq;
using BSS.Utils;

using static BagelCode.Scratcher.ScratcherCellPlayGroup.PlayOption;
using static BagelCode.Scratcher.ScratcherCellPlayGroup;

using CellGroup = System.Collections.Generic.List<BagelCode.Scratcher.ScratcherCellController>;
using SymbolGroup = System.Collections.Generic.List<int>;
using BingoPattern = System.Collections.Generic.List<int>;
using BoardState = System.Collections.Generic.List<bool>;

namespace BagelCode
{
    public static partial class PopupScratcherUtils
    {
        private const float PATTERN_PLAY_TERM = 0.4f;
        private static CellGroup GetPatternInBoard(BingoPattern pattern, CellGroup board)
        {
            return pattern.Select(idx => board[idx]).ToList();
        }

        private static bool CheckMadePattern(BingoPattern pattern, BoardState boardState)
        {
            return pattern.All(idx => boardState[idx]);
        }

        private static void InitScratcherPlayGroupsForBingoDefault(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            bool isReward = scratcherInfo.GetVariable<bool>("_isReward")?.value ?? false;

            // Hit Symbol
            List<SymbolGroup> hitSymbolGroupList = null;
            hitSymbolGroupList = scratcherInfo.GetValue<List<Blackboard>>("bingoCallersNumberList").Select(bb => bb.GetValue<List<int>>("row")).ToList();

            int hitCellTotal = hitSymbolGroupList.Sum(l => l.Count);
            CellGroup callerCellGroup = cellInstanceList.GetRange(0, hitCellTotal);

            // Board
            int boardCount = scratcherInfo.GetValue<int>("numBingoCard");

            int boardCellTotal = 5 * 5; // columns x rows
            var boardList = new List<CellGroup>();
            var playedPatternsList = new List<List<BingoPattern>>();
            for (int i = 0; i < boardCount; ++i)
            {
                int idx = hitCellTotal + boardCellTotal * i;
                CellGroup board = cellInstanceList.GetRange(idx, boardCellTotal);
                boardList.Add(board);
                playedPatternsList.Add(new List<BingoPattern>());
            }
            List<BoardState> boardStateList = boardList.Select(b => b.Select(cell => false).ToList()).ToList();

            // Info
            int version = scratcherInfo.GetVariable<int>("version")?.value ?? 0;
            if (version == 1)
            {
                var linePrizeList = scratcherInfo.GetValue<List<long>>("linePrizeList");
                var cornersPrizeList = scratcherInfo.GetValue<List<long>>("fourCornersPrizeList");
                var xPrizeList = scratcherInfo.GetValue<List<long>>("xPrizeList");
                for (int i = 0; i < boardCount; ++i)
                {
                    var infoArea = ContextUtils.FindElement(agent, SCRATCHER_INFO_AREA_NAME + " " + i, CHILDREN);
                    var infoText = ContextUtils.FindElement(infoArea, SCRATCHER_TEXT_INFO_TEXT, CHILDREN);

                    var linePrize = linePrizeList[i];
                    var cornersPrize = cornersPrizeList[i];
                    var xPrize = xPrizeList[i];
                    if (isReward)
                    {
                        linePrize = MultiplierUtils.GetRewardMultiplierValue(linePrize, scratcherInfo, "scratcher");
                        cornersPrize = MultiplierUtils.GetRewardMultiplierValue(cornersPrize, scratcherInfo, "scratcher");
                        xPrize = MultiplierUtils.GetRewardMultiplierValue(xPrize, scratcherInfo, "scratcher");
                    }

                    MetaContextElementUtils.SetTextGlobal(infoText, "COLLECTING_GAME_INFO_BINGO_RULE_TEXT", linePrize, cornersPrize, xPrize);
                }
            }

            // Pattern
            List<BingoPattern> patternList = scratcherInfo.GetValue<List<Blackboard>>("bingoWinPatternIndexList").Select(bb => bb.GetValue<List<int>>("row")).ToList();

            // Play Group Setting
            for (int i = 0; i < hitSymbolGroupList.Count; ++i)
            {
                SymbolGroup hitSymbols = hitSymbolGroupList[i];

                // Play Caller
                CellGroup callerHitCells = FindHitCell(hitSymbols, callerCellGroup);
                AddPlayGroup(scratcherPlayGroups, callerHitCells, new PlayOption[] { ACTIVATE_PARTICLE, PLAY_HIGHLIGHT, OPEN_CELL });

                for (int j = 0; j < boardCount; ++j)
                {
                    CellGroup board = boardList[j];
                    BoardState boardState = boardStateList[j];
                    List<BingoPattern> playedPatterns = playedPatternsList[j];
                    CellGroup boardHitCells = new CellGroup();

                    // Add "FREE" cell to hit list
                    if (i == 0) boardHitCells.AddRange(board.Where(c => c.SymbolText == "FREE"));

                    // Add hit list in board
                    boardHitCells.AddRange(FindHitCell(hitSymbols, board));

                    // Play Hit Cells
                    if (boardHitCells.Count > 0)
                    {
                        boardHitCells.ForEach(cell => boardState[FindCellIndex(board, cell)] = true);
                        AddPlayGroup(scratcherPlayGroups, boardHitCells, new PlayOption[] { ACTIVATE_PARTICLE, OPEN_CELL });
                    }

                    // Play Made Pattern Highlight
                    var madePatternList = patternList.Where(p => CheckMadePattern(p, boardState));
                    madePatternList
                        .Where(p => !playedPatterns.Contains(p))
                        .ForEach(p =>
                        {
                            playedPatterns.Add(p);

                            CellGroup madeCells = GetPatternInBoard(p, board);
                            AddHighlightToCells(madeCells);

                            // Check if there are two or more overlapping patterns
                            bool hasOverlapPattern = madePatternList.Where(pp => pp.Intersect(p).Count() > 0).Count() > 1;
                            if (hasOverlapPattern)
                            {
                                var patternPlayGroup = AddPlayGroup(scratcherPlayGroups, madeCells, new PlayOption[] { DISABLE_HIGHLIGHT, WAIT_BETWEEN, PLAY_HIGHLIGHT });
                                patternPlayGroup.isPlayNextInstantly = true;
                                patternPlayGroup.instantOpenDelay = PATTERN_PLAY_TERM;
                            }
                            else
                            {
                                var patternPlayGroup = AddPlayGroup(scratcherPlayGroups, madeCells, new PlayOption[] { PLAY_HIGHLIGHT });
                                patternPlayGroup.isPlayNextInstantly = true;
                                patternPlayGroup.instantOpenDelay = PATTERN_PLAY_TERM;
                            }
                        });
                }
            }
        }

        private static void InitScratcherPlayGroupsForBingoMultiplier(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            InitScratcherPlayGroupsForBingoDefault(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);

            var mult = cellInstanceList[cellInstanceList.Count - 1];
            AddHighlightToCell(mult);
            AddPlayGroup(scratcherPlayGroups, mult, DEFAULT_OPTION_SET);
        }
    }
}
