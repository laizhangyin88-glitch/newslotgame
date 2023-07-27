using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameCreateScratcher : ActionTask<ContextElement>
    {
        public BBParameter<Blackboard> scratcherRewardInfo;
        public BBParameter<int> scratcherId;
        public BBParameter<string> bundle;
        public BBParameter<bool> isLoaded;
        public BBParameter<long> remainingCount;
        
        protected override string info
        {
            get { return "Init Collecting Game Create Scratcher Scene"; }
        }

        protected override void OnExecute()
        {
            List<Blackboard> itemList = BlackboardQueryUtils.GetScratcher(scratcherId.value).GetValue<List<Blackboard>>("pieceList");

            for (int i = 0; i < itemList.Count; i++)
            {    
                Blackboard itemInfo = itemList[i];
                
                ContextElement itemAreaElement = ContextUtils.FindElement(agent, string.Format("Item 0{0}/Anchor", i + 1), ContextSearchingType.FullNameSearch);

                MetaObjectUtils.MakePrefab(bundle.value, "Scratcher Item", itemAreaElement.transform);
                itemAreaElement.UpdateContext(true);

                ContextElement starElement = ContextUtils.FindElement(itemAreaElement, "Star", ContextSearchingType.ChildrenSearch);
                
                var rarity = itemInfo.GetValue<ScratcherPieceRarity>("rarity");
                MetaContextElementUtils.SetSprite(starElement, CollectingGameCustomData.Instance.collectingGameAssets.starAssets[(int)rarity - 1]);
                
                ContextElement ItemImageElement = ContextUtils.FindElement(itemAreaElement, "Image", ContextSearchingType.ChildrenSearch);
                
                int itemId = itemInfo.GetValue<int>("pieceId");
                Sprite itemAsset = BlackboardQueryUtils.GetAssetOfItem(itemId);
                MetaContextElementUtils.SetSprite(ItemImageElement, itemAsset);
            }

            ContextElement scratcherAreaElement = ContextUtils.FindElement(agent, "Scratcher Area", ContextSearchingType.ChildrenSearch);
            MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Image Scratcher", scratcherAreaElement.transform);
            
            scratcherAreaElement.UpdateContext(true);
            ContextElement scratcherImageElement = ContextUtils.FindElement(scratcherAreaElement, "Base", ContextSearchingType.ChildrenSearch);
            string sampleImageUrl = scratcherRewardInfo.value.GetValue<string>("sampleImageUrl");
            MetaContextElementUtils.SetWebImage(
                scratcherImageElement,
                sampleImageUrl,
                CacheType.FileCache,
                false,
                () => { isLoaded.value = true; }
            );
            
            scratcherAreaElement.UpdateContext(true);
            
            remainingCount.value = BlackboardQueryUtils.GetMakableScratcherCount(scratcherId.value);
            
            EndAction();
        }
    }
}
