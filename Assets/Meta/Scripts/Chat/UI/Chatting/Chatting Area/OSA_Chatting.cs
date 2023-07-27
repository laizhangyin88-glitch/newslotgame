using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using Com.TheFallenGames.OSA.Core;
using UnityEngine;
using UnityEngine.EventSystems;


namespace BagelCode.Chat
{
    public class OSA_Chatting : OSA<ChatPollParams, BalloonViewHolder>
    {
        public ChattingController owner;
        public Action<PointerEventData> onEndDragEvent;

        public List<BalloonViewHolder> updateViewHolderList = new List<BalloonViewHolder>();

        public void SubscribeEndDragEvent(Action<PointerEventData> action)
        {
            if(action == null) return;
            onEndDragEvent += action;
        }

        public void UnSubscribeEndDragEvent(Action<PointerEventData> action)
        {
            onEndDragEvent -= action;
        }

        protected override BalloonViewHolder CreateViewsHolder(int itemIndex)
        {
            // Debug.LogError(GetItemsCount());
            // Debug.LogError(string.Format("Create {0}", itemIndex));
            if (owner.ChatDataList.IsValidIndex(itemIndex))
            {
                var chatData = owner.ChatDataList[itemIndex];
                BalloonViewHolder viewHolder = new BalloonViewHolder();
                viewHolder.Init(FindPrefab(chatData.GetAssetName()), itemIndex);
                viewHolder.textBalloon.gameObject.name = chatData.GetAssetName();
                viewHolder.textBalloon.Init(owner);

                return viewHolder;
            }
            else
            {
                Debug.LogWarning("CreateViewsHolder failure. " + itemIndex + " is invalid index with 'owner.chatDataList'");
                return null;
            };
        }

        protected override void OnItemHeightChangedPreTwinPass(BalloonViewHolder vh)
        {
            base.OnItemHeightChangedPreTwinPass(vh);

            owner.ChatDataList[vh.ItemIndex].changeSize = false;
            vh.ContentSizeFitter.enabled = false;
        }

        protected override void UpdateViewsHolder(BalloonViewHolder newOrRecycled)
        {
            // Debug.LogError("UpdateViewsHolder");
            var chatData = owner.ChatDataList[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateView(chatData);

            if (newOrRecycled.ContentSizeFitter.enabled)
                newOrRecycled.ContentSizeFitter.enabled = false;

            // if (chatData.changeSize)
            {
                // Height will be available before the next 'twin' pass, inside OnItemHeightChangedPreTwinPass() callback (see above)
                newOrRecycled.MarkForRebuild(); // will enable the content size fitter
                                                //newOrRecycled.contentSizeFitter.enabled = true;
                ScheduleComputeVisibilityTwinPass(true);
            }
        }

        protected override void OnBeforeRecycleOrDisableViewsHolder(BalloonViewHolder inRecycleBinOrVisible, int newItemIndex)
        {
            base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override bool IsRecyclable(BalloonViewHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            // Debug.LogError(string.Format("IsRecyclable {0}", indexOfItemThatWillBecomeVisible >= owner.chatDataList.Count));
            if(indexOfItemThatWillBecomeVisible >= owner.ChatDataList.Count) return false;

            var chatData = owner.ChatDataList[indexOfItemThatWillBecomeVisible];
            if (chatData.isSystem)
            {
                return potentiallyRecyclable.textBalloon.name == "Text Balloon System";
            }

            // Debug.LogError(string.Format("{0}, {1}", potentiallyRecyclable.textBalloon.name, chatData.GetAssetName())); 
            return potentiallyRecyclable.textBalloon.name == chatData.GetAssetName();
        }

        public override void ResetItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            ClearVisibleItems();
            base.ResetItems(itemsCount, contentPanelEndEdgeStationary, keepVelocity);
        }

        public void InsertToFirstItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            int prevCount = GetItemsCount();

            base.RemoveItems(0, 1, contentPanelEndEdgeStationary, keepVelocity);
            base.InsertItems(0, itemsCount + 1, contentPanelEndEdgeStationary, keepVelocity);

            // Debug.LogError(prevCount);
            // Debug.LogError(GetItemsCount());
            int scrollToIndex = GetItemsCount() - prevCount;
            if(scrollToIndex < 0) scrollToIndex = 0;

            StopMovement();
            ScrollTo(scrollToIndex);
        }

        public void InsertItem(int insertIndex, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            // Debug.LogError(string.Format("Insert {0}/{1}", insertIndex, GetItemsCount()));
            if(insertIndex < 0 || insertIndex > GetItemsCount()) insertIndex = GetItemsCount();

            // Debug.LogError(insertIndex);
            // Debug.LogError(GetItemsCount());
            base.InsertItems(insertIndex, 1, contentPanelEndEdgeStationary, keepVelocity);

            // StopMovement();
            // ScrollTo(index + 1);
        }

        public void RemoveItem(int removeIndex, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
        {
            // Debug.LogError(string.Format("Remove {0}/{1}", removeIndex, GetItemsCount()));

            if(removeIndex >= GetItemsCount()) return;

            base.RemoveItems(removeIndex, 1, contentPanelEndEdgeStationary, keepVelocity);

            // StopMovement();
            // ScrollTo(index + 1);
        }

        public void MoveToLast(bool isSmooth = false)
        {
            StopMovement();
            ScrollTo(GetItemsCount() - 1);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if(onEndDragEvent != null)
                onEndDragEvent(eventData);
                
            base.OnEndDrag(eventData);
        }

        private GameObject FindPrefab(string assetName)
        {
            if(!_Params.prefabDict.ContainsKey(assetName))
            {
                var balloon = MetaObjectUtils.MakePrefab<TextBalloonBase>(assetName, transform);
                
                balloon.gameObject.name = assetName;
                balloon.gameObject.SetActive(false);
                _Params.prefabDict.Add(assetName, balloon.gameObject);
            }

            return _Params.prefabDict[assetName];
        }
    }

    [Serializable]
    public class ChatPollParams : BaseParams
    {
        public Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
    }

    [Serializable]
    public class BalloonViewHolder : BaseItemViewsHolder
    {
        public TextBalloonBase textBalloon;
        public UnityEngine.UI.ContentSizeFitter ContentSizeFitter { get; private set; }

        public void UpdateView(ChatMessageData chatData)
        {
            textBalloon.Refresh(chatData);
        }

        public override void CollectViews()
        {
            base.CollectViews();

            textBalloon = root.GetComponent<TextBalloonBase>();
            ContentSizeFitter = root.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            ContentSizeFitter.enabled = false;
        }

        public override void MarkForRebuild()
        {
            base.MarkForRebuild();
            if (ContentSizeFitter)
                ContentSizeFitter.enabled = true;
        }
    }
}
