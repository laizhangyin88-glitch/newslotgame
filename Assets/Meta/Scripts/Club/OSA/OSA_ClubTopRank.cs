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
    public class OSA_ClubTopRank : OSA<ClubLeagueRankParams, ClubLeagueRankItem>
    {
        private GameObject root;
        private Blackboard rootBB;
        private GameObject caller;

        protected override void Start()
        {
            base.Start();

            root = gameObject;
            if(rootBB == null) rootBB = root.GetComponent<Blackboard>();

            caller = rootBB.GetValue<GameObject>("caller");
            Refresh();
        }

        public void Refresh()
        {
            if(root == null) return;
            ClearVisibleItems();
            CreateItemList();
        }

        public void CreateItemList()
        {
            CreateItemList(out List<ClubLeagueRankModel> newModels);

            _Params.data.Clear();
            _Params.data.AddRange(newModels);
            ResetItems(newModels.Count);
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
            var clubTopListResponse = BlackboardUtils.FindVariable<Blackboard>(rootBB, "clubTopListResponse");

            if (clubTopListResponse != null)
            {
                var clubListBB = BlackboardUtils.FindVariable<List<Blackboard>>(clubTopListResponse.value, "topClubList");

                if (clubListBB != null && clubListBB.value.Count > 0)
                {
                    for (int i = 0; i < clubListBB.value.Count; ++i)
                    {
                        ClubLeagueRankModel_Cell cellInfo = new ClubLeagueRankModel_Cell();
                        cellInfo.cellType = ClubLeagueRankCellType.Club;
                        cellInfo.caller = caller;;
                        cellInfo.infoBB = clubListBB.value[i];
                        cellInfo.rank = i;
                        cellInfo.indexPromote = -1;
                        cellInfo.indexDemote = 999;
                        cellInfo.maxOpenedTier = 0;
                        cellInfo.isFromLeaguePopup = false;

                        newModels.Add(cellInfo);
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
