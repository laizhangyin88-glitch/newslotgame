using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.IIP.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.IIP.Feature
{
    public class IIPCommunityGameTileAdmin : FeatureModule
    {
        public List<IIPCommunityGameTileVerticalGroup> tileVerticalGroupList = new List<IIPCommunityGameTileVerticalGroup>();

        private void Awake()
        {
            foreach (var verticalGroup in tileVerticalGroupList)
            {
                verticalGroup.allChildTiles.AddRange(verticalGroup.GetComponentsInChildren<IIPCommunityGameTile>());
            }
            RegisterEvent("IIPInitializeCommunityGame", InitialzieBonusGame);
        }
        IEnumerator _AppearHiddenPenguinsCoroutine()
        {
            for (int i = 0; i < tileVerticalGroupList.Count; i++)
            {
                for (int j = 0; j < tileVerticalGroupList[i].allChildTiles.Count; j++)
                {
                    //if tileMask is 0 ( Always unuse)
                    if (tileVerticalGroupList[i].tileMask[j] >= 1)
                    {
                        if (tileVerticalGroupList[i].allChildTiles[j].tileObjectStatus == TileObjectStatus.PENGUIN && tileVerticalGroupList[i].allChildTiles[j].penguin.isAppeared == false)
                        {
                            IIPUtility.PlaySound("Penguin Appear 1");
                            tileVerticalGroupList[i].allChildTiles[j].penguin.Appear();
                            yield return new WaitForSeconds(0.15f);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
        public Coroutine AppearHiddenPenguinsCoroutine() => StartCoroutine(_AppearHiddenPenguinsCoroutine());


        public void InitialzieBonusGame(EventData eventData)
        {
            List<Blackboard> tileData = BlackboardUtils.FindVariable<List<Blackboard>>("./bonus/response/initialGameBoard").value;

            for (int i = 0; i < tileVerticalGroupList.Count; i++)
            {
                if (i == 0 || i == 10)
                {
                    for (int j = 0; j < tileVerticalGroupList[i].allChildTiles.Count; j++)
                    {
                        if (tileVerticalGroupList[i].tileMask[j] == 0)
                            tileVerticalGroupList[i].allChildTiles[j].Initialize(TileObjectStatus.NONE, 3);
                    }
                    continue;
                }
                List<int> currentVertical = tileData[i - 1].GetVariable<List<int>>("value").value;
                int necessaryTileIndex = 0;
                for (int j = 0; j < tileVerticalGroupList[i].allChildTiles.Count; j++)
                {
                    //if tileMask is 0 ( Always unuse)
                    if (tileVerticalGroupList[i].tileMask[j] == 0)
                        tileVerticalGroupList[i].allChildTiles[j].Initialize(TileObjectStatus.NONE, 3);
                    else if (tileVerticalGroupList[i].tileMask[j] >= 1)
                    {
                        int level = tileVerticalGroupList[i].tileMask[j];

                        bool isPenguinTile = (TileKind)currentVertical[necessaryTileIndex] == TileKind.PENGUIN_SEA_WATER;
                        bool isTreasureTile = (TileKind)currentVertical[necessaryTileIndex] == TileKind.TREASURE_SEA_WATER;

                        tileVerticalGroupList[i].availableChildTiles.Add(tileVerticalGroupList[i].allChildTiles[j]);
                        tileVerticalGroupList[i].allChildTiles[j].Initialize(
                            isPenguinTile == true ? TileObjectStatus.PENGUIN : isTreasureTile == true ? TileObjectStatus.TREASURE : TileObjectStatus.NONE, level);

                        necessaryTileIndex++;
                    }
                }
            }
            GetTileByAvailablePos(4, 2).UpdateFloor(TileFloorStatus.ICE, true);
        }

        public void SetSecondePhase()
        {
            List<Blackboard> tileData = BlackboardUtils.FindVariable<List<Blackboard>>("./bonus/response/secondTileBoard").value;

            for (int i = 0; i < tileVerticalGroupList.Count; i++)
            {
                if (i == 0 || i == 10)
                    continue;

                List<int> currentVertical = tileData[i - 1].GetVariable<List<int>>("value").value;
                int necessaryTileIndex = 0;

                for (int j = 0; j < tileVerticalGroupList[i].allChildTiles.Count; j++)
                {
                    //if tileMask is 0 ( Always unuse)
                    if (tileVerticalGroupList[i].tileMask[j] == 0)
                        tileVerticalGroupList[i].allChildTiles[j].Initialize(TileObjectStatus.NONE, 3);
                    else if (tileVerticalGroupList[i].tileMask[j] == 2)
                    {
                        bool isPenguinTile = (TileKind)currentVertical[necessaryTileIndex] == TileKind.PENGUIN_SEA_WATER;
                        bool isTreasureTile = (TileKind)currentVertical[necessaryTileIndex] == TileKind.TREASURE_SEA_WATER;

                        tileVerticalGroupList[i].allChildTiles[j].Initialize(
                            isPenguinTile == true ? TileObjectStatus.PENGUIN : isTreasureTile == true ? TileObjectStatus.TREASURE : TileObjectStatus.NONE, 1);

                        necessaryTileIndex++;
                    }
                    else if (tileVerticalGroupList[i].tileMask[j] == 1)
                        necessaryTileIndex++;
                }
            }
        }

        public IIPCommunityGameTile GetTileByRealPos(int x, int y) => tileVerticalGroupList[x].allChildTiles[y];
        public IIPCommunityGameTile GetTileByAvailablePos(int x, int y) => tileVerticalGroupList[x + 1].availableChildTiles[y];
    }
}