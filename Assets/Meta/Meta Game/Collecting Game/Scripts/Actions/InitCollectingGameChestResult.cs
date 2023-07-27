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
    public class InitCollectingGameChestResult : ActionTask<ContextElement>
    {
        public BBParameter<string> bundle;
        public BBParameter<string> sharedBundle;

        private string ON_COLLECT_EVENT = "OnCollect";
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        
        protected override string info
        {
            get { return "Init Collecting Game Chest Result Scene"; }
        }

        protected override void OnExecute()
        {
            List<Blackboard> packOpenResultList = BlackboardQueryUtils.GetResultPieceList();

            ContextElement scratcherItemAreaElement = ContextUtils.FindElement(agent, "Scroll Area/Scratcher Item Area", ContextSearchingType.FullNameSearch);
            
            for (int i = 0; i < packOpenResultList.Count; i++)
            {
                MetaObjectUtils.MakePrefab(sharedBundle.value, "Scratcher Item Position", scratcherItemAreaElement.transform, null, string.Format("Scratcher Item Position {0}", i));
            }
            
            scratcherItemAreaElement.UpdateContext(true); 
            
            for (int i = 0; i < packOpenResultList.Count; i++)
            {
                Blackboard itemInfo = packOpenResultList[i];
                
                ContextElement anchorElement = ContextUtils.FindElement(scratcherItemAreaElement, string.Format("Scratcher Item Position {0}/Anchor", i), ContextSearchingType.FullNameSearch);
                MetaObjectUtils.MakePrefab(bundle.value, "Scratcher Result Item", anchorElement.transform, null, "Scratcher Item");
                
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

                ContextElement effectResultLightElement = ContextUtils.FindElement(anchorElement, "Effect Result Light", ContextSearchingType.ChildrenSearch);

                if ((int) rarity >= 4)
                {
                    MetaContextElementUtils.SetActive(effectResultLightElement, true);

                    effectResultLightElement.GetComponent<OverrideRelativeParticleSortingLayer>().UpdateSortingLayer();
                    
                    OverrideRelativeParticleSortingLayer[] overriders = effectResultLightElement.GetComponentsInChildren<OverrideRelativeParticleSortingLayer>();
                    for (int j = 0; j < overriders.Length; j++)
                        overriders[j].UpdateSortingLayer();
                }
            }

            ContextElement collectButtonElement = ContextUtils.FindElement(agent, "Button Okay", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                collectButtonElement,
                ON_COLLECT_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );

            ContextElement collectButtonTextElement = ContextUtils.FindElement(collectButtonElement, "Text", ContextSearchingType.ChildrenSearch);
            string collectText = StringTableUtils.GetString(tableType, "BUTTON_COLLECT");
            MetaContextElementUtils.SetText(collectButtonTextElement, collectText);
            
            agent.GetComponent<Animator>().SetBool("Active", true);
            
            EndAction(true);
        }
    }
}
