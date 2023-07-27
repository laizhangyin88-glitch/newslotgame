using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitPopupScratcherGame : ActionTask<ContextElement>
    {
        public BBParameter<Blackboard> scratcherRewardResult;
        public BBParameter<string> booleanTrigger;
        public BBParameter<bool> isLoaded;
        public BBParameter<Transform> fullCoverButton;
        public BBParameter<ContextElement> scratcherElement;
        public BBParameter<string> winSound;
        public BBParameter<bool> playCoinSound;
        public BBParameter<int> scratcherId;

        private Blackboard bb;
        private bool isReward;

        private const StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const string ON_SCRATCHER_NEXT_STEP_EVENT = "OnScratcherNextStep";
        private const string ON_CLICK_COLLECT = "OnClickCollect";
        private const string ON_CLICK_CONTINUE = "OnClickContinue";
        private const string ON_TOGGLE_AUTO = "OnToggleAuto";

        protected override string info
        {
            get { return "Init Popup Scratcher Game Scene"; }
        }

        protected override void OnExecute()
        {
            bb = agent.GetComponent<Blackboard>();
            isReward = bb.GetVariable<bool>("_isReward")?.value ?? false;

            isLoaded.value = false;

            ContextElement scratcherElement = ContextUtils.FindElement(agent, "Scratcher", ContextSearchingType.ChildrenSearch);
            this.scratcherElement.value = scratcherElement;

            scratcherElement.GetComponent<Blackboard>().AddVariable("_popupScratcher", agent);

            if (isReward)
            {
                var rewardInfo = BlackboardUtils.FindVariable<Blackboard>(scratcherRewardResult.value, "rewardInfo")?.value;
                if(rewardInfo != null)
                {
                    BlackboardUtils.CopyBlackboardVariables(rewardInfo, scratcherRewardResult.value);
                }
            }

            var scratcherController = scratcherElement.GetComponent<ScratcherPlayController>();
            scratcherController.scratcherInfo = scratcherRewardResult.value;
            scratcherController.cellInstanceList = InitCells(scratcherElement);

            InitWinHighlight();
            SetScratcherNotFinished();
            SendBIEvent();

            ContextElement scratcherBackgroundElement = ContextUtils.FindElement(scratcherElement, "Base", ContextSearchingType.ChildrenSearch);

            string backgroundImageUrl = scratcherRewardResult.value.GetValue<string>("backgroundImageUrl");

            MetaContextElementUtils.SetWebImage(
                scratcherBackgroundElement,
                backgroundImageUrl,
                CacheType.FileCache,
                false,
                () => { isLoaded.value = true; }
            );

            ContextElement buttonCollectElement = ContextUtils.FindElement(agent, "Collect Board/Collect", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetClickable(
                buttonCollectElement,
                ON_CLICK_COLLECT,
                false,
                false,
                scratcherElement.GetComponent<BehaviourTreeOwner>().SendEvent,
                ownerSystem
            );

            ContextElement buttonCollectTextElement = ContextUtils.FindElement(buttonCollectElement, "Text", ContextSearchingType.ChildrenSearch);
            long credit = scratcherRewardResult.value.GetValue<long>("credit");

            string buttonCollectText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_COLLECT_BUTTON_TEXT");
            string buttonOkayText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_OKAY_BUTTON_TEXT");
            if (credit > 0)
                MetaContextElementUtils.SetText(buttonCollectTextElement, buttonCollectText);
            else
                MetaContextElementUtils.SetText(buttonCollectTextElement, buttonOkayText);

            ContextElement prizeAreaElement = ContextUtils.FindElement(scratcherElement, "Scratcher Top Prize Area", ContextSearchingType.FullNameSearch);
            ContextElement prizeElement = ContextUtils.FindElement(prizeAreaElement, "Scratcher Text Top Prize", ContextSearchingType.ChildrenSearch);

            var rewardBB = scratcherRewardResult.value.GetValue<Blackboard>("rewardInfo");
            long maxPrize = rewardBB.GetVariable<long>("maxWinCredit")?.value ?? 0L;

            if (isReward)
            {
                maxPrize = MultiplierUtils.GetRewardMultiplierValue(maxPrize, rewardBB, "scratcher");
            }

            if (maxPrize == 0L)
            {
                prizeAreaElement.gameObject.SetActive(false);
            }
            else
            {
                prizeAreaElement.gameObject.SetActive(true);
                MetaContextElementUtils.SetTextGlobal(prizeElement, "COLLECTING_GAME_SIMPLE_IMAGE_PRIZE_TEXT", maxPrize);
            }

            ContextElement buttonContinueElement = ContextUtils.FindElement(agent, "Collect Board/Continue", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetClickable(
                buttonContinueElement,
                ON_CLICK_CONTINUE,
                false,
                false,
                scratcherElement.GetComponent<BehaviourTreeOwner>().SendEvent,
                ownerSystem
            );

            ContextElement buttonContinueTextElement = ContextUtils.FindElement(buttonContinueElement, "Text", ContextSearchingType.ChildrenSearch);
            string buttonContinueText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_SCRATCH_NEXT_BUTTON_TEXT");
            MetaContextElementUtils.SetText(buttonContinueTextElement, buttonContinueText);

            var remainingCountVar = agent.gameObject.GetComponent<Blackboard>().GetVariable<int>("_remainingCount");
            if(remainingCountVar != null)
            {
                ContextElement remainingAreaElement = ContextUtils.FindElement(agent, "Remaining Area", ContextSearchingType.ChildrenSearch);
                ContextElement textRemainingElement = ContextUtils.FindElement(remainingAreaElement, "Text Remaining", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetText(textRemainingElement, remainingCountVar.value.ToString());
            }

            ContextElement buttonCancelElement = ContextUtils.FindElement(agent, "Remaining Area/Button Auto Scratch", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetClickable(
                buttonCancelElement,
                ON_TOGGLE_AUTO,
                false,
                false,
                scratcherElement.GetComponent<BehaviourTreeOwner>().SendEvent,
                ownerSystem
            );
            buttonCancelElement.GetComponent<PIDButton>().interactable = false;
            ContextElement wonTextElement = ContextUtils.FindElement(agent, "Collect Board/Text", ContextSearchingType.FullNameSearch);
            ContextElement buttonTextElement = ContextUtils.FindElement(agent, "Collect Board/Collect/Text", ContextSearchingType.FullNameSearch);

            string wonText;
            string buttonText;
            if (credit > 0)
            {
                wonText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_WON_TEXT", credit);
                buttonText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_COLLECT_BUTTON_TEXT");
            }
            else
            {
                wonText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_LOSE_TEXT");
                buttonText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_OKAY_BUTTON_TEXT");
            }

            MetaContextElementUtils.SetText(wonTextElement, wonText);
            MetaContextElementUtils.SetText(buttonTextElement, buttonText);

            ContextElement fullCoverButtonElement = ContextUtils.FindElement(scratcherElement, "Full Cover Button", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                fullCoverButtonElement,
                ON_SCRATCHER_NEXT_STEP_EVENT,
                false,
                true,
                SendEvent,
                ownerSystem
            );
            fullCoverButtonElement.GetComponent<PIDButton>().interactable = false;
            fullCoverButton.value = fullCoverButtonElement.transform;

            ContextElement buttonScratchElement = ContextUtils.FindElement(agent, "Button Scratch", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                buttonScratchElement,
                ON_SCRATCHER_NEXT_STEP_EVENT,
                false,
                true,
                SendEvent,
                ownerSystem
            );

            ContextElement buttonScratchTextElement = ContextUtils.FindElement(buttonScratchElement, "Text", ContextSearchingType.ChildrenSearch);
            string buttonScratchText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_SCRATCH_BUTTON_TEXT");
            MetaContextElementUtils.SetText(buttonScratchTextElement, buttonScratchText);

            EndAction();
        }

        private List<ScratcherCellController> InitCells(ContextElement scratcher)
        {
            List<ScratcherCellController> cellInstanceList = new List<ScratcherCellController>();
            List<Blackboard> symbolList = scratcherRewardResult.value.GetValue<List<Blackboard>>("symbolList");

            for (int i = 0; i < symbolList.Count; i++)
            {
                cellInstanceList.Add(InitCell(scratcher, symbolList[i], i));
            }

            return cellInstanceList;
        }

        private ScratcherCellController InitCell(ContextElement scratcherElement, Blackboard symbolBB, int index)
        {
            ContextElement areaElement = ContextUtils.FindElement(scratcherElement, String.Format("Scratcher Area {0}", index), ContextSearchingType.ChildrenSearch);
            ContextElement cellElement = ContextUtils.FindElement(areaElement, "Scratcher Cell", ContextSearchingType.ChildrenSearch);

            ScratcherCellSymbol cellSymbol = new ScratcherCellSymbol(bb, symbolBB);
            ScratcherCellCover cellCover = new ScratcherCellCover(symbolBB);
            ScratcherCellInfo cellInfo = new ScratcherCellInfo(cellSymbol, cellCover, symbolBB);

            var cellInstance = cellElement.GetComponent<ScratcherCellController>();
            cellInstance.cellInfo = cellInfo;
            cellInstance.Init();

            return cellInstance;
        }

        private void InitWinHighlight()
        {
            playCoinSound.value = false;
            winSound.value = "";
            ScratcherBigWinType winType = scratcherRewardResult.value.GetValue<ScratcherBigWinType>("bigWinType");
            ContextElement winAreaElement = ContextUtils.FindElement(agent, "Win Area", ContextSearchingType.ChildrenSearch);

            GameObject winEffect = null;

            switch (winType)
            {
                case ScratcherBigWinType.DEFAULT:
                    booleanTrigger.value = "End";
                    break;
                case ScratcherBigWinType.BIG:
                case ScratcherBigWinType.SUPER_BIG:
                {
                    booleanTrigger.value = "BigWin";
                    winSound.value = "Collecting_Game_Scratcher_Mini_Win";
                    playCoinSound.value = true;

                    if (winType == ScratcherBigWinType.BIG)
                        winEffect = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Scratcher Text Event Big Win", winAreaElement.transform);
                    else
                        winEffect = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Scratcher Text Event Super Big Win", winAreaElement.transform);
                    break;
                }
                case ScratcherBigWinType.MEGA:
                case ScratcherBigWinType.SUPER_MEGA:
                {
                    booleanTrigger.value = "MegaWin";
                    winSound.value = "Collecting_Game_Scratcher_Major_Win";
                    playCoinSound.value = true;

                    if (winType == ScratcherBigWinType.MEGA)
                        winEffect = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Scratcher Text Event Mega Win", winAreaElement.transform);
                    else
                        winEffect = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Scratcher Text Event Super Mega Win", winAreaElement.transform);
                    break;
                }
                case ScratcherBigWinType.EPIC:
                {
                    booleanTrigger.value = "EpicWin";
                    winSound.value = "Collecting_Game_Scratcher_Mega_Win";
                    playCoinSound.value = true;
                    winEffect = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Scratcher Text Event Epic Win", winAreaElement.transform);
                    break;
                }
            }

            if (winEffect != null)
            {
                OverrideRelativeParticleSortingLayer[] winEffectSortingLayers = winEffect.GetComponentsInChildren<OverrideRelativeParticleSortingLayer>();
                for (int i = 0; i < winEffectSortingLayers.Length; i++)
                {
                    winEffectSortingLayers[i].UpdateSortingLayer();
                }
            }
        }

        private void SetScratcherNotFinished()
        {
            ScratcherName scratcherName = scratcherRewardResult.value.GetValue<ScratcherName>("scratcherName");
            PlayerPrefs.SetString("NOT_FINISHED_SCRATCHER", BlackboardQueryUtils.GetScratcherName(scratcherName));
        }

        private void SendBIEvent()
        {
            var eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);

            string eventName = "client_scratcher_enter";
            var eventData = new Dictionary<string, object>();

            if (eventInfo != null)
            {
                eventData["collecting_game_event_id"] = eventInfo.id;
                eventData["collecting_game_id"] = ((EventDataCollectingGame)eventInfo.constraints).collectingGameId;
            }
            eventData["scratcher_preset_id"] = scratcherRewardResult.value.GetValue<int>("scratcherPresetId");

            if (scratcherId != null)
            {
                eventData["scratcher_id"] = scratcherId.value;
            }

            Variable scratcherType = scratcherRewardResult.value.GetVariable<RewardScratcherRule>("scratcherRule");
            if (scratcherType != null && scratcherType.value != null)
            {
                eventData["scratcher_type"] = scratcherRewardResult.value.GetValue<RewardScratcherRule>("scratcherRule").ToString();
            }

            var backgroundImageUrl = scratcherRewardResult.value.GetVariable<string>("backgroundImageUrl");
            eventData["image_url"] = backgroundImageUrl == null ? "" : backgroundImageUrl.value;

            Analytics.CustomEvent(eventName, eventData);
        }
    }
}
