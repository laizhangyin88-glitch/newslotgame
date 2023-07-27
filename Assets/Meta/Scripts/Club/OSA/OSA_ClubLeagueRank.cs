using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using Com.TheFallenGames.OSA.Core;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.EventSystems;
using BagelCode;

namespace BagelCode.OSA_Scroll
{
    public class OSA_ClubLeagueRank : OSA<ClubLeagueRankParams, ClubLeagueRankItem>
    {
        public GameObject root;
        public Blackboard rootBB;

        private int beginIndex = 0;

        protected override void Start()
        {
            base.Start();
            // CreateItemList();
        }

        public void Refresh()
        {
            ClearVisibleItems();
            CreateItemList();
        }

        public void CreateItemList()
        {
            CreateItemList(out List<ClubLeagueRankModel> newModels);

            _Params.data.Clear();
            _Params.data.AddRange(newModels);
            ResetItems(newModels.Count);
            for (int i = 0; i < _Params.data.Count; i++)
            {
                if (_Params.data[i] is ClubLeagueRankModel_Divider)
                    RequestChangeItemSizeAndUpdateLayout(i, ClubLeagueRankModel_Divider.customHeightSize);
            }

            if(beginIndex < 0)
                beginIndex = 0;
            else if(beginIndex > 0 && beginIndex >= _Params.data.Count)
                beginIndex = _Params.data.Count -1;

            if(_Params.data.Count > 0)
                ScrollTo(beginIndex);
        }

        protected override ClubLeagueRankItem CreateViewsHolder(int itemIndex)
        {
            ClubLeagueRankItem item = null;
            ClubLeagueRankCellType itemType = _Params.data[itemIndex].cellType;

            switch (itemType)
            {
                case ClubLeagueRankCellType.Club:
                    item = new ClubLeagueRankItem_Club();
                    break;
                case ClubLeagueRankCellType.Promote:
                case ClubLeagueRankCellType.Demote:
                    item = new ClubLeagueRankItem_Divider();
                    break;
            }

            if (item != null)
                item.Init(FindPrefab(itemType), itemIndex);

            return item;
        }


        protected override void UpdateViewsHolder(ClubLeagueRankItem newOrRecycled)
        {
            if(_Params.data.Count == 0) return;

            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateView(model);
        }

        protected override bool IsRecyclable(ClubLeagueRankItem potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return potentiallyRecyclable.CanPresentModelType(_Params.data[indexOfItemThatWillBecomeVisible].cellType);
        }

        protected override bool ShouldDestroyRecyclableItem(ClubLeagueRankItem inRecycleBin, bool isInExcess)
        {
            return inRecycleBin.ShouldDestroyRecyclableItem();
        }

        private void CreateItemList(out List<ClubLeagueRankModel> newModels)
        {
            newModels = new List<ClubLeagueRankModel>();

            bool isLeaguePopup = rootBB.GetVariable<bool>("isLeaguePopup")?.value ?? false;

            var leagueResponse = BlackboardUtils.FindVariable<Blackboard>(rootBB, "leagueResponse");

            if (leagueResponse != null)
            {
                int indexPromote = leagueResponse.value.GetValue<int>("indexPromote");
                int indexDemote = leagueResponse.value.GetValue<int>("indexDemote");
                int maxOpenedTier = leagueResponse.value.GetValue<int>("maxOpenedLeagueTier");

                var clubTierInfoBB = BlackboardUtils.FindVariable<Blackboard>(leagueResponse.value, "tierInfo");
                int clubTier = clubTierInfoBB.value.GetValue<int>("leagueTier");
                var myClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");

                var clubListBB = BlackboardUtils.FindVariable<List<Blackboard>>(leagueResponse.value, "clubList");

                if (clubListBB != null && clubListBB.value.Count > 0)
                {
                    var leagueMaxTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/LEAGUE/MAX_TIER");

                    bool usePromote = false;
                    bool useDemote = false;

                    if (clubTier == 0)
                    {
                        usePromote = true;
                    }
                    else if (clubTier == leagueMaxTier.value || clubTier >= maxOpenedTier)
                    {
                        useDemote = true;
                    }
                    else
                    {
                        usePromote = true;
                        useDemote = true;
                    }

                    int realIndex = 0;

                    for (int i = 0; i < clubListBB.value.Count; ++i)
                    {
                        ClubLeagueRankModel_Cell cellInfo = new ClubLeagueRankModel_Cell();
                        cellInfo.cellType = ClubLeagueRankCellType.Club;
                        cellInfo.caller = root;
                        cellInfo.infoBB = clubListBB.value[i];
                        cellInfo.rank = i;
                        cellInfo.indexPromote = indexPromote;
                        cellInfo.indexDemote = indexDemote;
                        cellInfo.maxOpenedTier = maxOpenedTier;
                        cellInfo.isFromLeaguePopup = isLeaguePopup;

                        var clubID = clubListBB.value[i].GetValue<long>("id");
                        if(clubID == myClubID.value)
                        {
                            beginIndex = realIndex-2;
                        }

                        newModels.Add(cellInfo);
                        ++realIndex;

                        if (usePromote && i == indexPromote)
                        {

                            ClubLeagueRankModel_Divider promoteCell = new ClubLeagueRankModel_Divider();
                            promoteCell.cellType = ClubLeagueRankCellType.Promote;
                            promoteCell.index = i;
                            promoteCell.caller = root;
                            newModels.Add(promoteCell);
                            ++realIndex;
                        }
                        else if (useDemote && i == indexDemote - 1)
                        {
                            ClubLeagueRankModel_Divider demoteCell = new ClubLeagueRankModel_Divider();
                            demoteCell.cellType = ClubLeagueRankCellType.Demote;
                            demoteCell.index = i;
                            demoteCell.caller = root;
                            newModels.Add(demoteCell);
                            ++realIndex;
                        }
                    }
                }
            }
        }

        private GameObject FindPrefab(ClubLeagueRankCellType type)
        {
            if (_Params.prefabs[(int)type] == null)
            {
                string prefabName = type == ClubLeagueRankCellType.Club ? "Club League Cell" : "Club League Divider Cell";

                GameObject go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, prefabName, transform);
                go.SetActive(false);
                _Params.prefabs[(int)type] = go;
            }

            return _Params.prefabs[(int)type];
        }
    }
}
