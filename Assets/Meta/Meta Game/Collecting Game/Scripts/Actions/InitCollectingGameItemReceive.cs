using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameItemReceive : ActionTask<ContextElement>
    {
        public BBParameter<string> bundle;
        public BBParameter<string> sharedBundle;
        
        private const string ON_COLLECT_EVENT = "OnCollect";
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        
        protected override string info
        {
            get { return "Init Collecting Game Item Receive"; }
        }

        protected override void OnExecute()
        {
            ContextElement titleTextElement = ContextUtils.FindElement(agent, "Title Area/Text", ContextSearchingType.FullNameSearch);
            string titleText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_ITEM_RECEIVE_TITLE");
            MetaContextElementUtils.SetText(titleTextElement, titleText);
            
            ContextElement scratcherItemAreaElement = ContextUtils.FindElement(agent, "Scratcher Item Area", ContextSearchingType.ChildrenSearch);
            var sharePieceList = BlackboardUtils.GetOrCreateVariable<List<Blackboard>>(null, "/collectingGameInfo/sharePieceList");
            
            for (int i = 0; i < sharePieceList.value.Count; i++)
            {
                MetaObjectUtils.MakePrefab(sharedBundle.value, "Scratcher Item Position", scratcherItemAreaElement.transform, null, string.Format("Scratcher Item Position {0}", i));
            }
            
            scratcherItemAreaElement.UpdateContext(true);
            
            for (int i = 0; i < sharePieceList.value.Count; i++)
            {
                Blackboard itemInfo = sharePieceList.value[i];
                
                ContextElement anchorElement = ContextUtils.FindElement(scratcherItemAreaElement, string.Format("Scratcher Item Position {0}/Anchor", i), ContextSearchingType.FullNameSearch);
                MetaObjectUtils.MakePrefab(bundle.value, "Scratcher Item", anchorElement.transform);
                
                anchorElement.UpdateContext(true);
                
                ContextElement starElement = ContextUtils.FindElement(anchorElement, "Star", ContextSearchingType.ChildrenSearch);
                
                var rarity = itemInfo.GetValue<ScratcherPieceRarity>("rarity");
                MetaContextElementUtils.SetSprite(starElement, CollectingGameCustomData.Instance.collectingGameAssets.starAssets[(int)rarity - 1]);
                
                ContextElement ItemImageElement = ContextUtils.FindElement(anchorElement, "Image", ContextSearchingType.ChildrenSearch);
                
                int itemId = itemInfo.GetValue<int>("pieceId");
                Sprite itemAsset = BlackboardQueryUtils.GetAssetOfItem(itemId);
                MetaContextElementUtils.SetSprite(ItemImageElement, itemAsset);

                ContextElement badgeAreaElement = ContextUtils.FindElement(anchorElement, "Badge Area", ContextSearchingType.ChildrenSearch);
                int count = itemInfo.GetValue<int>("count");
                MetaObjectUtils.UpdateBadge(badgeAreaElement, false, count);
            }
            
            ContextElement contentTextElement = ContextUtils.FindElement(agent, "Text", ContextSearchingType.ChildrenSearch);
            string contentText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_ITEM_RECEIVE_CONTENT");
            MetaContextElementUtils.SetText(contentTextElement, contentText);
            
            ContextElement okayElement = ContextUtils.FindElement(agent, "Okay", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                okayElement,
                ON_COLLECT_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement okayTextElement = ContextUtils.FindElement(okayElement, "Text", ContextSearchingType.ChildrenSearch);
            string okayText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_COLLECT_BUTTON_TEXT");
            MetaContextElementUtils.SetText(okayTextElement, okayText);
            
            agent.GetComponent<Animator>().SetBool("Active", true);
            
            EndAction();
        }
    }
}
