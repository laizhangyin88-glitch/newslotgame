using UnityEngine;
using System.Collections.Generic;
using SlotMaker;
using BagelCode.Scratcher;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

using static BagelCode.Scratcher.ScratcherCellPlayGroup;

using CellGroup = System.Collections.Generic.List<BagelCode.Scratcher.ScratcherCellController>;

namespace BagelCode
{
    public static partial class PopupScratcherUtils
    {

        private static void InitScratcherPlayGroupsForPokerDefault(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            List<PokerScratcherWinType> pokerWinTypeList = scratcherInfo.GetValue<List<PokerScratcherWinType>>("pokerWinTypeList");
            List<bool> pokerWinList = scratcherInfo.GetValue<List<bool>>("pokerWinList");

            ScratcherCellPlayGroup winGroup = new ScratcherCellPlayGroup();
            ScratcherCellPlayGroup loseGroup = new ScratcherCellPlayGroup(OPEN_ALL_OPTION_SET);

            Color[] handResultColors =
            {
                new Color(0.149f, 0f, 0.22f),
                new Color(0.149f, 0f, 0.22f),
                new Color(0.149f, 0f, 0.22f),
                new Color(0.898f, 0.4f, 0f),
                new Color(0.898f, 0f, 0f),
                new Color(0.486f, 0f, 0.898f),
                new Color(0.231f, 0f, 0.898f),
                new Color(0.0f, 0.4f, 0.898f),
                new Color(0.0f, 0.4f, 0.898f),
                new Color(0.0f, 0.4f, 0.898f),
                Color.gray,
            };

            for (int i = 0; i < pokerWinTypeList.Count; i++)
            {
                ScratcherCellPlayGroup playGroup = new ScratcherCellPlayGroup();

                ContextElement resultAreaElement = ContextUtils.FindElement(agent, string.Format("Result Area {0}", i), FULL);
                ContextElement pokerHandResultElement = ContextUtils.FindElement(resultAreaElement, "Poker Hand Result", CHILDREN);
                ContextElement pokerHandBaseElement = ContextUtils.FindElement(pokerHandResultElement, "Base", CHILDREN);

                if ((int)pokerWinTypeList[i] >= 0 && (int)pokerWinTypeList[i] <= 9)
                {
                    pokerHandBaseElement.GetComponent<CanvasRendererProperty>().color = handResultColors[(int)pokerWinTypeList[i]];

                    ContextElement pokerHandTextElement = ContextUtils.FindElement(pokerHandBaseElement, "Text", CHILDREN);

                    string pokerHandText = StringTableUtils.GetString(GLOBAL, string.Format("COLLECTING_GAME_POKER_RULE_{0}", pokerWinTypeList[i].ToString()));
                    MetaContextElementUtils.SetText(pokerHandTextElement, pokerHandText);

                    cellInstanceList[i * 5 + 4].AddGameObjectsToActivate(pokerHandResultElement.gameObject);
                }


                if (i == 0 || (i != 0 && pokerWinList[i]))
                {
                    for (int j = i * 5; j < i * 5 + 5; j++)
                    {
                        if (cellInstanceList[j].IsHit)
                        {
                            cellInstanceList[i * 5 + 4].AddHighlightCells(cellInstanceList[j]);
                        }
                    }

                    if (i != 0)
                    {
                        ContextElement groupHighlightElement = ContextUtils.FindElement(agent, string.Format("Group Highlight {0}", i), ContextSearchingType.ChildrenSearch);
                        cellInstanceList[i * 5 + 4].AddGameObjectsToActivate(groupHighlightElement.gameObject);

                        cellInstanceList[5 * 9 + i - 1].AddHighlightCellsSelf();

                        winGroup.AddCellInstance(cellInstanceList[5 * 9 + i - 1]);
                    }
                }
                else if (i != 0 && !pokerWinList[i])
                {
                    loseGroup.AddCellInstance(cellInstanceList[5 * 9 + i - 1]);
                }

                for (int j = i * 5; j < i * 5 + 5; j++)
                {
                    playGroup.AddCellInstance(cellInstanceList[j]);
                }

                scratcherPlayGroups.Add(playGroup);
            }

            if (!winGroup.IsCellEmpty())
                scratcherPlayGroups.Add(winGroup);

            if (!loseGroup.IsCellEmpty())
                scratcherPlayGroups.Add(loseGroup);
        }

        private static void InitScratcherPlayGroupsForPokerMultiplier(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent)
        {
            InitScratcherPlayGroupsForPokerDefault(scratcherPlayGroups, scratcherInfo, cellInstanceList, agent);

            List<bool> pokerWinList = scratcherInfo.GetValue<List<bool>>("pokerWinList");
            bool winExist = false;
            for (int i = 1; i < pokerWinList.Count; i++)
            {
                if (pokerWinList[i])
                {
                    winExist = true;
                    break;
                }
            }

            if (winExist)
                cellInstanceList[cellInstanceList.Count - 1].AddHighlightCellsSelf();

            ScratcherCellPlayGroup multiplierGroup = new ScratcherCellPlayGroup();
            multiplierGroup.AddCellInstance(cellInstanceList[cellInstanceList.Count - 1]);
            scratcherPlayGroups.Add(multiplierGroup);
        }
    }
}
