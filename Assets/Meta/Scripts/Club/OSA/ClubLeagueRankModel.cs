using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using BagelCode;
using Com.ForbiddenByte.OSA.Core;

namespace BagelCode.OSA_Scroll
{
    public enum ClubLeagueRankCellType
    {
        Club = 0,
        Promote,
        Demote
    }

    [Serializable]
    public class ClubLeagueRankParams : BaseParams
    {
        public List<ClubLeagueRankModel> data = new List<ClubLeagueRankModel>();

        public GameObject[] prefabs = null;
    }

    [Serializable]
    public class ClubLeagueRankModel
    {
        public ClubLeagueRankCellType cellType;
        public GameObject caller;
    }

    [Serializable]
    public class ClubLeagueRankModel_Cell : ClubLeagueRankModel
    {
        public Blackboard infoBB;
        public int rank;
        public int indexPromote;
        public int indexDemote;
        public int maxOpenedTier;
        public bool isFromLeaguePopup;
    }

    [Serializable]
    public class ClubLeagueRankModel_Divider : ClubLeagueRankModel
    {
        public static int customHeightSize = 48;
        public int index;
    }



    [Serializable]
    public abstract class ClubLeagueRankItem : BaseItemViewsHolder
    {
        public ContentSizeFitter contentSizeFitter;

        public abstract bool CanPresentModelType(ClubLeagueRankCellType type);
        public virtual bool ShouldDestroyRecyclableItem() { return false; }
        public abstract void UpdateView(ClubLeagueRankModel model);
        public virtual void SetActiveCount(bool isActive) { }
    }


    [Serializable]
    public class ClubLeagueRankItem_Club : ClubLeagueRankItem
    {
        private ClubLeagueRankCellController controller;

        public override bool CanPresentModelType(ClubLeagueRankCellType type) { return type == ClubLeagueRankCellType.Club; }
        public override void UpdateView(ClubLeagueRankModel model)
        {
            var dividerModel = model as ClubLeagueRankModel_Cell;
            if(dividerModel != null)
            {
                controller.UpdateVariables(dividerModel);

                controller.GetComponent<GraphOwner>().StopBehaviour();
                controller.GetComponent<GraphOwner>().StartBehaviour();
            }
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<ClubLeagueRankCellController>();

            contentSizeFitter = root.GetComponent<ContentSizeFitter>();
            contentSizeFitter.enabled = false;
        }

        public override void MarkForRebuild()
        {
            base.MarkForRebuild();
            if (contentSizeFitter)
                contentSizeFitter.enabled = true;
        }
    }

    [Serializable]
    public class ClubLeagueRankItem_Divider : ClubLeagueRankItem
    {
        private ClubLeagueRankDividerController controller;

        public override bool CanPresentModelType(ClubLeagueRankCellType type) { return type != ClubLeagueRankCellType.Club; }
        public override void UpdateView(ClubLeagueRankModel model)
        {
            var dividerModel = model as ClubLeagueRankModel_Divider;
            if(dividerModel != null)
            {
                controller.UpdateVariables(dividerModel);
            }
        }

        public override void CollectViews()
        {
            base.CollectViews();

            controller = root.GetComponent<ClubLeagueRankDividerController>();

            contentSizeFitter = root.GetComponent<ContentSizeFitter>();
            contentSizeFitter.enabled = false;
        }

        public override void MarkForRebuild()
        {
            base.MarkForRebuild();
            if (contentSizeFitter)
                contentSizeFitter.enabled = true;
        }
    }
}
