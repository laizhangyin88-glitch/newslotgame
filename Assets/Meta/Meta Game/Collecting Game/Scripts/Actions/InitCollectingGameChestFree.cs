using System;
using System.Collections.Generic;
using System.Text;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameChestFree : ActionTask<ContextElement>
    {
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private string ON_OPEN_CHEST_EVENT = "OnOpenChest";
        private string ON_COLLECT_EVENT = "OnCollect";
        
        protected override string info
        {
            get { return "Init Collecting Game Chest Free Scene"; }
        }

        protected override void OnExecute()
        {
            ContextElement chestBaseElement = ContextUtils.FindElement(agent, "Anchor/Base", ContextSearchingType.FullNameSearch);
            var freePackInfo = BlackboardUtils.GetOrCreateVariable<ScratcherPackInfo>(null, "/collectingGameInfo/freePackInfo");
            int packId = freePackInfo.value.packId;
            Sprite chestSprite = CollectingGameChestData.Instance.chestAssets.assets[BlackboardQueryUtils.GetChestIndex(packId)];
            MetaContextElementUtils.SetSprite(chestBaseElement, chestSprite);
            
            ContextElement fullCoverButtonElement = ContextUtils.FindElement(agent, "Full Cover Button", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                fullCoverButtonElement,
                ON_OPEN_CHEST_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );

            ContextElement buttonCollectElement = ContextUtils.FindElement(agent, "Button Collect", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                buttonCollectElement,
                ON_COLLECT_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement buttonCollectTextElement = ContextUtils.FindElement(buttonCollectElement, "Text", ContextSearchingType.ChildrenSearch);
            string buttonCollectText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_COLLECT_BUTTON_TEXT");
            MetaContextElementUtils.SetText(buttonCollectTextElement, buttonCollectText);

            ContextElement chestTextElement = ContextUtils.FindElement(agent, "Text", ContextSearchingType.ChildrenSearch);
            string chestText = BlackboardQueryUtils.GetChestName(packId);
            MetaContextElementUtils.SetText(chestTextElement, chestText);
            
            agent.GetComponent<Animator>().SetBool("Active", true);
            agent.GetComponent<Animator>().SetBool("TabActive", true);
            
            EndAction(true);
        }
    }
}
