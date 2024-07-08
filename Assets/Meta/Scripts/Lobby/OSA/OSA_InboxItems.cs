using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using Com.ForbiddenByte.OSA.Core;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using UnityEngine.EventSystems;

using InboxGroup = System.Collections.Generic.List<NodeCanvas.Framework.Blackboard>;

namespace BagelCode.OSA_Scroll
{
    public class OSA_InboxItems : OSA<InboxParams, InboxItem>
    {
        private class GroupInfo
        {
            public int count;
            public Blackboard firstInfo;
            public Blackboard lastAddedInfo;
        }

        protected override void Start()
        {   
            base.Start();
            CreateItemList();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (PopupManager.Instance.popupCount == 0)
            {
                base.OnDrag(eventData);
            }
        }

        public void Refresh()
        {
            ClearVisibleItems();
            CreateItemList();
        }

        public void CreateItemList()
        {
            CreateItemList(out List<InboxModel> newModels);

            _Params.data.Clear();
            _Params.data.AddRange(newModels);
            ResetItems(newModels.Count);
        }

        public void RemoveItemFromID(int targetId)
        {
            for (int i = 0; i < _Params.data.Count; i++)
            {
                var queue = _Params.data[i].inboxInfoList;
                var data = _Params.data[i].inboxInfoList.FirstOrDefault();

                int inboxId = data.GetVariable<int>("id")?.value ?? 0;
                if (inboxId == targetId)
                {
                    queue.PopFirst();

                    if (queue.Count == 0)
                    {
                        _Params.data.RemoveAt(i);
                        ResetItems(_Params.data.Count); // UpdateViewsHolder
                    }
                }
            }
        }

        public void RemoveItemFromGiftId(int targetGiftId)
        {
            for (int i = 0; i < _Params.data.Count; i++)
            {
                var queue = _Params.data[i].inboxInfoList;
                var data = _Params.data[i].inboxInfoList.FirstOrDefault();

                int giftId = data.GetVariable<int>("giftId")?.value ?? 0;
                if (giftId == targetGiftId)
                {
                    queue.PopFirst();

                    if (queue.Count == 0)
                    {
                        _Params.data.RemoveAt(i);
                        ResetItems(_Params.data.Count); // UpdateViewsHolder
                    }
                }
            }
        }
        //

        protected override InboxItem CreateViewsHolder(int itemIndex)
        {
            InboxItem item = null;
            InboxItemType itemType = _Params.data[itemIndex].itemType;

            switch (itemType)
            {
                case InboxItemType.NORMAL:
                    item = new InboxItem_Normal();
                    break;
            }
            
            if (item != null)
                item.Init(FindPrefab((int)itemType), _Params.Content, itemIndex);

            return item;
        }
        
        
        protected override void UpdateViewsHolder(InboxItem newOrRecycled)
        {
            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateViews(model);
        }
        
        protected override bool IsRecyclable(InboxItem potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return potentiallyRecyclable.CanPresentModelType(_Params.data[indexOfItemThatWillBecomeVisible].itemType);
        }

        protected override bool ShouldDestroyRecyclableItem(InboxItem inRecycleBin, bool isInExcess)
        {
            return inRecycleBin.ShouldDestroyRecyclableItem();
        }

        //

        private void AddNewModel(List<InboxModel> list, InboxGroup inboxGroup)
        {
            var model = new InboxModel_Normal();
            model.itemType = InboxItemType.NORMAL;
            model.inboxInfoList = inboxGroup;
            list.Add(model);
        }

        private void CreateItemList(out List<InboxModel> newModels)
        {
            newModels = new List<InboxModel>();

            // Banner
            var inboxBannerInfoList = BlackboardUtils.FindValue<List<Blackboard>>("/inboxBannerList");
            for (int i = 0; i < inboxBannerInfoList.Count; i++)
            {
                var info = inboxBannerInfoList[i];

                if (info.GetValue<InboxBannerTypes>("inboxBannerType") == InboxBannerTypes.NEWS &&
                    BlackboardQueryUtils.IsWatchedInboxBannerItem(info.GetValue<int>("id")))
                    continue;

                var inboxGroup = new InboxGroup();
                inboxGroup.Add(info);
                AddNewModel(newModels, inboxGroup);
            }

            // Bucks Gift Add
            var bucksGiftList = BlackboardQueryUtils.GetBucksGiftList();
            if (bucksGiftList != null && bucksGiftList.Count > 0)
            {
                for (int i = 0; i < bucksGiftList.Count; ++i)
                {
                    Blackboard bucksInfo = bucksGiftList[i];

                    var inboxGroup = new InboxGroup();
                    inboxGroup.Add(bucksInfo);
                    AddNewModel(newModels, inboxGroup);
                }
            }

            // Inbox
            var inboxInfoList = BlackboardUtils.FindValue<List<Blackboard>>("/inboxList");
            var inboxGroupList = InboxUtils.GroupingInboxInfos(inboxInfoList);

            inboxGroupList.Reverse();

            foreach (var group in inboxGroupList)
            {
                AddNewModel(newModels, group);
            }
        }
        
        private GameObject FindPrefab(int id)
        {
            if (_Params.prefabs[id] == null && _Params.sceneInfos[id] != null)
            {
                var go = SceneManager.LoadScene(transform, _Params.sceneInfos[id].GetSceneInfo());
                go.SetActive(false);
                _Params.prefabs[id] = go;
            }
            return _Params.prefabs[id];
        }
    }
}
