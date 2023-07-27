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
    public class InitCollectingGame : ActionTask<ContextElement>
    {
        public BBParameter<int> currentScratcherId;
        public BBParameter<string> bundle;
        public BBParameter<string> sharedBundle;
        public BBParameter<int> shareItemCount;
        
        private const string ON_SELECT_BOTTOM_SCRATCHER_EVENT = "OnSelectBottomScratcher";
        private const string ON_CLOSE_EVENT = "OnClose";
        private const string ON_CLICK_INFORMATION_EVENT = "OnClickInformation";
        private const string ON_ZOOM_IN_SCRATCHER_EVENT = "OnZoomInScratcher";
        private const string ON_ZOOM_OUT_SCRATCHER_EVENT = "OnZoomOutScratcher";
        private const string ON_SCRATCH_EVENT = "OnScratch";
        private const string ON_OPEN_CHEST_EVENT = "OnOpenChest";
        private const string ON_OPEN_FREE_CHEST_EVENT = "OnOpenFreeChest";
        private const string ON_FREE_CHEST_VIDEO_AD_EVENT = "OnFreeChestVideoAD";
        private const string ON_BUY_CHEST_EVENT = "OnBuyChest";
        private const string ON_REQUEST_ITEM_EVENT = "OnRequestItem";

        private const StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        
        protected override string info
        {
            get { return "Init Collecting Game Scene"; }
        }

        protected override void OnExecute()
        {
            List<Blackboard> scratcherList = BlackboardQueryUtils.GetScratcherList();
            
            int scratcherCount = scratcherList.Count;
            currentScratcherId.value = scratcherList[0].GetValue<int>("scratcherId");

            ContextElement tabBaseAreaElement = ContextUtils.FindElement(agent, "Tab Base Area", ContextSearchingType.ChildrenSearch);
            
            for (int i = 0; i < scratcherCount; i++)
            {
                int scratcherId = scratcherList[i].GetValue<int>("scratcherId");
                
                GameObject tabScratcher = MetaObjectUtils.MakePrefab(sharedBundle.value, "Tab Scratcher", tabBaseAreaElement.transform, null, string.Format("Tab Scratcher {0}", i + 1));
                ContextElement tabScratcherElement = tabScratcher.GetComponent<ContextElement>();
                tabScratcherElement.UpdateContext();
                
                MetaContextElementUtils.SetClickable(
                    tabScratcherElement,
                    ON_SELECT_BOTTOM_SCRATCHER_EVENT,
                    scratcherId,
                    false,
                    false,
                    SendEvent,
                    ownerSystem
                );

                ContextElement tabScratcherTextElement = ContextUtils.FindElement(tabScratcherElement, "Text", ContextSearchingType.ChildrenSearch);

                string scratcherName = BlackboardQueryUtils.GetScratcherName(scratcherList[i].GetValue<Blackboard>("reward").GetValue<ScratcherName>("scratcherName"));
                MetaContextElementUtils.SetText(tabScratcherTextElement, scratcherName.ToUpper());
            }
            tabBaseAreaElement.UpdateContext();

            ContextElement closeButtonElement = ContextUtils.FindElement(agent, "Close", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                ON_CLOSE_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            ContextElement questionButtonElement = ContextUtils.FindElement(agent, "Button Collecting Game Question", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                questionButtonElement,
                ON_CLICK_INFORMATION_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            ContextElement questionButtonTextElement = ContextUtils.FindElement(questionButtonElement, "Text", ContextSearchingType.ChildrenSearch);
            string questionButtonText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_QUESTION_BUTTON_TEXT");
            MetaContextElementUtils.SetText(questionButtonTextElement, questionButtonText);

            ContextElement scratcherAreaElement = ContextUtils.FindElement(agent, "Base/Scratcher Area", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetClickable(
                scratcherAreaElement,
                ON_ZOOM_IN_SCRATCHER_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement magnifierButtonElement = ContextUtils.FindElement(agent, "Base/Button Magnifier", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetClickable(
                magnifierButtonElement,
                ON_ZOOM_IN_SCRATCHER_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement scratcherZoomInElement = ContextUtils.FindElement(agent, "Scratcher Zoom In", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                scratcherZoomInElement,
                ON_ZOOM_OUT_SCRATCHER_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement scratchButtonElement = ContextUtils.FindElement(agent, "Base/Button Scratch", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetClickable(
                scratchButtonElement,
                ON_SCRATCH_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            ContextElement scratchButtonTextElement = ContextUtils.FindElement(scratchButtonElement, "Text", ContextSearchingType.ChildrenSearch);
            string scratchButtonText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_SCRATCH_BUTTON_TEXT");
            MetaContextElementUtils.SetText(scratchButtonTextElement, scratchButtonText);

            MetaObjectUtils.MakePrefab(sharedBundle.value, "Collecting Game Sounds", agent.transform);
            MetaObjectUtils.MakePrefab(bundle.value, "Data", agent.transform);
            
            List<Blackboard> chestList = BlackboardQueryUtils.GetChestList();
            for (int i = 0; i < chestList.Count; i++)
            {
                Blackboard chestInfo = chestList[i];
                
                ContextElement chestElement = ContextUtils.FindElement(agent, string.Format("Chest 0{0}", i + 1), ContextSearchingType.ChildrenSearch);

                int chestId = chestInfo.GetValue<int>("packId");

                MetaContextElementUtils.SetClickable(
                    chestElement,
                    ON_OPEN_CHEST_EVENT,
                    chestId,
                    false,
                    false,
                    SendEvent,
                    ownerSystem
                );
            }

            ContextElement chestFreeElement = ContextUtils.FindElement(agent, "Chest Free", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                chestFreeElement,
                ON_OPEN_FREE_CHEST_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );

            ContextElement chestBonusADElement = ContextUtils.FindElement(agent, "Chest Bonus AD", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                chestBonusADElement,
                ON_FREE_CHEST_VIDEO_AD_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement chestBuyElement = ContextUtils.FindElement(agent, "Chest Buy", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                chestBuyElement,
                ON_BUY_CHEST_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );

            var sharePieceList = BlackboardUtils.GetOrCreateVariable<List<Blackboard>>(null, "/collectingGameInfo/sharePieceList");
            shareItemCount.value = sharePieceList != null ? sharePieceList.value.Count : 0;

            ContextElement requestButtonElement = ContextUtils.FindElement(agent, "Button Request", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                requestButtonElement,
                ON_REQUEST_ITEM_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            EndAction();
        }
    }
}
