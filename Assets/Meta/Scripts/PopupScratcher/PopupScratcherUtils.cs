using UnityEngine;
using System.Collections.Generic;
using SlotMaker;
using BagelCode.Scratcher;
using BagelCode.ClientModels;
using System.Linq;
using NodeCanvas.Framework;
using System.Collections;

using static BagelCode.Scratcher.ScratcherCellPlayGroup;

using CellGroup = System.Collections.Generic.List<BagelCode.Scratcher.ScratcherCellController>;
using SymbolGroup = System.Collections.Generic.List<int>;

namespace BagelCode
{
    public static partial class PopupScratcherUtils
    {
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private const string SCRATCHER_INFO_AREA_NAME = "Scratcher Info Area";
        private const string SCRATCHER_TEXT_INFO_OUTLINE_TEXT = "Scratcher Text Info Outline";
        private const string SCRATCHER_TEXT_INFO_TEXT = "Scratcher Text Info";

        public enum ScratcherSceneType
        {
            NONE = 0,
            INSTANT_CASH,
            EXTREME_NUMBERS_1, // 10x
            EXTREME_NUMBERS_2, // 10x
            WINNING_NUMBERS, // 20x
            DICE,
            MATCH_THREE_ROW,
            MATCH_THREE_PRIZE,
            WINNING_NUMBERS_DYNAMIC_ROWS,
            WINNING_NUMBERS_DYNAMIC_TOWER,
            WINNING_NUMBERS_GLOBAL_MULTIPLIER,
            POKER_DEFAULT,
            POKER_MULTIPLIER,
            BINGO_DOUBLE,
            BINGO_QUADRUPLE,
            BINGO_QUADRUPLE_MULTIPLIER,
            KENO_STEPPED,
            KENO_STEPPED_MULTIPLIER,
            KENO_CUBE_SMALL,
            KENO_CUBE_BIG,
            KENO_GENERAL,
            LADDER_3_LINE,
            LADDER_3_LINE_BONUS,
            LADDER_4_LINE_BONUS,
            LADDER_5_LINE_MULTIPLIER,
            LADDER_8_LINE_MULTIPLIER,
        }

        public static string ScratcherSceneTypeToName(ScratcherSceneType sceneType)
        {
            if (sceneType == ScratcherSceneType.NONE) return string.Empty;

            string typeName = TextDecoUtils.EnumTypeToText<ScratcherSceneType>(
                (int)sceneType, TextDecoUtils.TextFormat.PASCAL_CASE, " ");

            return string.Format("Popup Scratcher Game {0} Scene", typeName);
        }

        public static string GetScratcherSceneName(ScratcherName name)
        {
            ScratcherSceneType scratcherSceneType;
            switch (name)
            {
                case ScratcherName.VEGAS_SEVENS:
                case ScratcherName.GRAND_PRIZE_HOTEL:
                case ScratcherName.FRESH_SHRIMPS:
                case ScratcherName.SEAFOOD_STREET:
                case ScratcherName.BASEBALL_CHAMPIONS:
                case ScratcherName.HOCKEY_MATCH:
                    scratcherSceneType = ScratcherSceneType.INSTANT_CASH;
                    break;
                case ScratcherName.CASINO_NIGHT:
                case ScratcherName.FOOTBALL_FEVER:
                case ScratcherName.BIG_FRIED_CHICKEN:
                    scratcherSceneType = ScratcherSceneType.EXTREME_NUMBERS_1;
                    break;
                case ScratcherName.SUPER_DOGGY:
                case ScratcherName.EXTREME_10X:
                    scratcherSceneType = ScratcherSceneType.EXTREME_NUMBERS_2;
                    break;
                case ScratcherName.MASSIVE_20X:
                case ScratcherName.MASSIVE_20X_BALL:
                case ScratcherName.MASSIVE_20X_BBQ:
                    scratcherSceneType = ScratcherSceneType.WINNING_NUMBERS;
                    break;
                case ScratcherName.VEGAS_NIGHT:
                case ScratcherName.FINAL_TOUCHDOWN:
                case ScratcherName.TASTY_DISHES:
                    scratcherSceneType = ScratcherSceneType.DICE;
                    break;
                case ScratcherName.RICH_PALACE:
                case ScratcherName.LUCKY_PETS:
                    scratcherSceneType = ScratcherSceneType.MATCH_THREE_ROW;
                    break;
                case ScratcherName.MATCH_3_TRIPLER:
                case ScratcherName.CUTE_3_PAWS:
                    scratcherSceneType = ScratcherSceneType.MATCH_THREE_PRIZE;
                    break;
                case ScratcherName.CASH_DESERT:
                    scratcherSceneType = ScratcherSceneType.WINNING_NUMBERS_DYNAMIC_ROWS;
                    break;
                case ScratcherName.CATS_TOWER:
                    scratcherSceneType = ScratcherSceneType.WINNING_NUMBERS_DYNAMIC_TOWER;
                    break;
                case ScratcherName.WORLD_TOUR:
                case ScratcherName.TOP_DOG:
                    scratcherSceneType = ScratcherSceneType.WINNING_NUMBERS_GLOBAL_MULTIPLIER;
                    break;
                case ScratcherName.JOKERS_WILD_POKER:
                case ScratcherName.JOKERS_POKER_RED:
                case ScratcherName.JOKERS_POKER_GREEN:
                    scratcherSceneType = ScratcherSceneType.POKER_DEFAULT;
                    break;
                case ScratcherName.JOKERS_WILD_POKER_ADVANCE:
                case ScratcherName.JOKERS_10X_PURPLE:
                case ScratcherName.JOKERS_10X_GOLD:
                    scratcherSceneType = ScratcherSceneType.POKER_MULTIPLIER;
                    break;
                case ScratcherName.BINGO_RUSH:
                    scratcherSceneType = ScratcherSceneType.BINGO_DOUBLE;
                    break;
                case ScratcherName.BINGO_CRUISE:
                case ScratcherName.BINGO_LAND:
                    scratcherSceneType = ScratcherSceneType.BINGO_QUADRUPLE;
                    break;
                case ScratcherName.BINGO_5X_STOCK_KING:
                case ScratcherName.BINGO_10X_GOLD_VAULT:
                    scratcherSceneType = ScratcherSceneType.BINGO_QUADRUPLE_MULTIPLIER;
                    break;
                case ScratcherName.SCRATCH_KENO:
                    scratcherSceneType = ScratcherSceneType.KENO_STEPPED;
                    break;
                case ScratcherName.WHEEL_OF_KENO:
                    scratcherSceneType = ScratcherSceneType.KENO_STEPPED_MULTIPLIER;
                    break;
                case ScratcherName.KENO_CUBE:
                    scratcherSceneType = ScratcherSceneType.KENO_CUBE_SMALL;
                    break;
                case ScratcherName.KENO_4X:
                    scratcherSceneType = ScratcherSceneType.KENO_CUBE_BIG;
                    break;
                case ScratcherName.KENO_10X:
                    scratcherSceneType = ScratcherSceneType.KENO_GENERAL;
                    break;
                case ScratcherName.LADDER_HUSTLE:
                    scratcherSceneType = ScratcherSceneType.LADDER_3_LINE;
                    break;
                case ScratcherName.GOLDEN_CARNIVAL:
                    scratcherSceneType = ScratcherSceneType.LADDER_3_LINE_BONUS;
                    break;
                case ScratcherName.QUADRUPLE_PLEASURE:
                    scratcherSceneType = ScratcherSceneType.LADDER_4_LINE_BONUS;
                    break;
                case ScratcherName.FORTUNE_FIESTA_10X:
                    scratcherSceneType = ScratcherSceneType.LADDER_5_LINE_MULTIPLIER;
                    break;
                case ScratcherName.GRAND_CAROUSEL_20X:
                    scratcherSceneType = ScratcherSceneType.LADDER_8_LINE_MULTIPLIER;
                    break;
                default:
                    {
                        Debug.LogWarning("ScratcherUtils.GetScratcherSceneName failure. " + name.ToString() + " is undefined scratcherName.");
                    }
                    return string.Empty;
            }

            return ScratcherSceneTypeToName(scratcherSceneType);
        }

        // Init Scratcher Play Group
        public static void InitScratcherPlayGroups(RewardScratcherRule scratcherRule, List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            switch (scratcherRule)
            {
                case RewardScratcherRule.INSTANT_CASH_DEFAULT:
                    // ScratcherName.VEGAS_SEVENS
                    InitScratcherPlayGroupsForInstantCashDefault(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.INSTANT_CASH_WIN_IT_ALL:
                    // ScratcherName.GRAND_PRIZE_HOTEL
                    InitScratcherPlayGroupsForInstantCashWinItAll(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.WINNING_NUMBERS_DEFAULT:
                    // ScratcherName.CASINO_NIGHT
                    InitScratcherPlayGroupsForWinningNumbersDefault(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.WINNING_NUMBERS_MULTIPLIER:
                    // ScratcherName.MASSIVE_20X
                    // ScratcherName.EXTREME_10X
                    InitScratcherPlayGroupsForWinningNumbersMultiplier(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.DICE:
                    // ScratcherName.VEGAS_NIGHT
                    InitScratcherPlayGroupsForDice(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.POKER_DEFAULT:
                    // ScratcherName.JOKERS_WILD_POKER
                    // ScratcherName.JOKERS_POKER_RED
                    // ScratcherName.JOKERS_POKER_GREEN
                    InitScratcherPlayGroupsForPokerDefault(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.POKER_MULTIPLIER:
                    // ScratcherName.JOKERS_WILD_POKER_ADVANCE
                    // ScratcherName.JOKERS_10X_PURPLE
                    // ScratcherName.JOKERS_10X_GOLD
                    InitScratcherPlayGroupsForPokerMultiplier(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.MATCH_THREE_ROW:
                    // ScratcherName.RICH_PALACE
                    InitScratcherPlayGroupsForMatchThreeRow(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.MATCH_THREE_PRIZE:
                    // ScratcherName.MATCH_3_TRIPLER
                    InitScratcherPlayGroupsForMatchThreePrize(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.WINNING_NUMBERS_DYNAMIC_ROWS:
                    // ScratcherName.CASH_DESERT
                    InitScratcherPlayGroupsForWinningNumbersDynamicRows(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.WINNING_NUMBERS_GLOBAL_MULTIPLIER:
                    // ScratcherName.WORLD_TOUR
                    InitScratcherPlayGroupsForWinningNumbersGlobalMultiplier(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.BINGO_DEFAULT:
                    // ScratcherName.BINGO_CRUISE
                    // ScratcherName.BINGO_LAND
                    // ScratcherName.BINGO_RUSH
                    InitScratcherPlayGroupsForBingoDefault(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.BINGO_MULTIPLIER:
                    // ScratcherName.BINGO_5X_STOCK_KING
                    // ScratcherName.BINGO_10X_GOLD_VAULT
                    InitScratcherPlayGroupsForBingoMultiplier(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.KENO_STEPPED:
                    // ScratcherName.KENO_SCRATCHER_KENO
                    InitScratcherPlayGroupsForKenoStepped(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.KENO_STEPPED_MULTIPLIER:
                    // ScratcherName.KENO_WHEEL_OF_KENO
                    InitScratcherPlayGroupsForKenoSteppedMultiplier(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.KENO_CUBE_BIG:
                    // ScratcherName.KENO_4X
                    InitScratcherPlayGroupsForKenoCube(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent, true);
                    break;
                case RewardScratcherRule.KENO_CUBE_SMALL:
                    // ScratcherName.KENO_CUBE
                    InitScratcherPlayGroupsForKenoCube(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent, false);
                    break;
                case RewardScratcherRule.KENO_GENERAL:
                    // ScratcherName.KENO_10X_KENO
                    InitScratcherPlayGroupsForKenoLine(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);
                    break;
                case RewardScratcherRule.LADDER:
                    // ScratcherName.LADDER_HUSTLE
                    InitScratcherPlayGroupsForLadder(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent, false, false);
                    break;
                case RewardScratcherRule.LADDER_BONUS:
                    // ScratcherName.GOLDEN_CANIVAL
                    // ScratcherName.QUADRUPLE_PLEASURE
                    InitScratcherPlayGroupsForLadder(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent, true, false);
                    break;
                case RewardScratcherRule.LADDER_MULTIPLIER:
                    // ScratcherName.FORTUNE_FIESTA_10X
                    // ScratcherName.GRAND_CAROUSEL_20X
                    InitScratcherPlayGroupsForLadder(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent, false, true);
                    break;
            }
        }

        private static ScratcherAsyncActionPlayGroup AddPlayGroup(List<ScratcherPlayGroup> scratcherPlayGroups, IEnumerator action)
        {
            var playGroup = new ScratcherAsyncActionPlayGroup(action);
            scratcherPlayGroups.Add(playGroup);
            return playGroup;
        }

        private static ScratcherActionPlayGroup AddPlayGroup(List<ScratcherPlayGroup> scratcherPlayGroups, System.Action action, float playTime)
        {
            var playGroup = new ScratcherActionPlayGroup(action, playTime);
            scratcherPlayGroups.Add(playGroup);
            return playGroup;
        }

        private static ScratcherCellPlayGroup AddPlayGroup(List<ScratcherPlayGroup> scratcherPlayGroups, PlayOption[] options)
        {
            var playGroup = new ScratcherCellPlayGroup(options);
            scratcherPlayGroups.Add(playGroup);
            return playGroup;
        }

        private static ScratcherCellPlayGroup AddPlayGroup(List<ScratcherPlayGroup> scratcherPlayGroups, CellGroup cells, PlayOption[] options)
        {
            if (cells == null || cells.Count == 0) return null;

            var playGroup = new ScratcherCellPlayGroup(options);
            playGroup.AddCellInstance(cells);
            scratcherPlayGroups.Add(playGroup);

            return playGroup;
        }

        private static ScratcherCellPlayGroup AddPlayGroup(List<ScratcherPlayGroup> scratcherPlayGroups, ScratcherCellController cell, PlayOption[] options)
        {
            if (cell == null) return null;

            var playGroup = new ScratcherCellPlayGroup(options);
            playGroup.AddCellInstance(cell);
            scratcherPlayGroups.Add(playGroup);
            return playGroup;
        }

        private static ScratcherCellPlayGroup AppendPlayGroup(ScratcherCellPlayGroup playGroup, CellGroup cells)
        {
            if (cells == null || cells.Count == 0) return null;

            playGroup.AddCellInstance(cells);
            return playGroup;
        }

        private static ScratcherCellPlayGroup AppendPlayGroup(ScratcherCellPlayGroup playGroup, ScratcherCellController cell)
        {
            if (cell == null) return null;

            playGroup.AddCellInstance(cell);
            return playGroup;
        }

        private static int FindCellIndex(CellGroup cells, ScratcherCellController cell)
        {
            return cells.IndexOf(cell);
        }

        private static CellGroup FindHitCell(SymbolGroup hitSymbols, CellGroup cells)
        {
            return cells.Where(c => hitSymbols.Contains(c.SymbolId)).ToList();
        }

        private static SymbolGroup FindHitSymbol(SymbolGroup hitSymbols, SymbolGroup symbols)
        {
            return symbols.Where(s => hitSymbols.Contains(s)).ToList();
        }

        private static bool HasOverlapCell(CellGroup a, CellGroup b)
        {
            return a.Any(c => b.Contains(c));
        }

        private static void AddHighlightToCells(CellGroup cells)
        {
            cells.ForEach(c => c.AddHighlightCellsSelf());
        }

        private static void AddHighlightToCell(ScratcherCellController cell)
        {
            cell.AddHighlightCellsSelf();
        }
    }
}
