using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;


namespace BagelCode.OSA_Scroll
{
    public enum InboxItemType
    {
        NORMAL,
    }

    public abstract class InboxItem : BaseItemViewsHolder
    {
        public abstract bool CanPresentModelType(InboxItemType itemType);
        public virtual bool ShouldDestroyRecyclableItem() { return false; }
        public abstract void UpdateViews(InboxModel model);
    }

    [Serializable]
    public class InboxParams : BaseParams
    {
        public SceneInfoObject[] sceneInfos;
        public GameObject[] prefabs;

        public List<InboxModel> data = new List<InboxModel>();
    }

    [Serializable]
    public class InboxModel
    {
        public InboxItemType itemType;
        public List<Blackboard> inboxInfoList;
    }

    [Serializable]
    public class InboxModel_Normal : InboxModel { }

    public class InboxItem_Normal : InboxItem
    {
        public override bool CanPresentModelType(InboxItemType itemType) { return itemType == InboxItemType.NORMAL; }
        public override void UpdateViews(InboxModel model)
        {
            root.gameObject.SetActive(true);

            var normalModel = model as InboxModel_Normal;

            var controller = root.GetComponent<InboxCellController>();
            controller.UpdateInboxInfoGroup(normalModel.inboxInfoList);
        }
    }
}
