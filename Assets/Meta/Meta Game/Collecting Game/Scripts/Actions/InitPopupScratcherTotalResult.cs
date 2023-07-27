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
    public class InitPopupScratcherTotalResult : ActionTask<ContextElement>
    {
        public BBParameter<List<long>> prizeList;
        public BBParameter<List<int>> scratcherIdList;
        public BBParameter<List<int>> winTypeList;
        public BBParameter<float> waitTime;
        public BBParameter<bool> isBetterLuck;

        private string ON_CLICK_BUTTON_EVENT = "OnClickButton";
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        
        protected override string info
        {
            get { return "Init Popup Scratcher Total Result Scene"; }
        }

        protected override void OnExecute()
        {
            ContextElement titleTextElement = ContextUtils.FindElement(agent, "Title Area Full/Text", ContextSearchingType.FullNameSearch);
            string titleText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_TOTAL_RESULT_TITLE");
            MetaContextElementUtils.SetText(titleTextElement, titleText);
            
            ContextElement resultInfoElement = ContextUtils.FindElement(agent, "Text Result", ContextSearchingType.ChildrenSearch);
            string resultInfoText = StringTableUtils.GetString(tableType, "COLLECTING_GAME_TOTAL_RESULT_INFO", prizeList.value.Count);
            MetaContextElementUtils.SetText(resultInfoElement, resultInfoText);
            
            ContextElement buttonElement = ContextUtils.FindElement(agent, "Button Yellow", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                buttonElement,
                ON_CLICK_BUTTON_EVENT,
                false,
                false,
                SendEvent,
                ownerSystem
            );
            
            ContextElement contentsElement = ContextUtils.FindElement(agent, "Scroll Rect/Contents", ContextSearchingType.FullNameSearch);

            long totalPrize = 0;

            for (int i = 0; i < prizeList.value.Count; i++)
            {
                var cell = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Contents Total Result Cell", contentsElement.transform);

                ContextElement cellElement = cell.GetComponent<ContextElement>();
                cellElement.UpdateContext(true);

                ContextElement textNameElement = ContextUtils.FindElement(cellElement, "Text Name", ContextSearchingType.ChildrenSearch);
                ContextElement textResultElement = ContextUtils.FindElement(cellElement, "Text Result", ContextSearchingType.ChildrenSearch);
                ContextElement textWinElement = ContextUtils.FindElement(cellElement, "Text Win", ContextSearchingType.ChildrenSearch);

                Blackboard scratcherRewardInfo = BlackboardQueryUtils.GetScratcher(scratcherIdList.value[i]).GetValue<Blackboard>("reward");    
                string scratcherName = BlackboardQueryUtils.GetScratcherName(scratcherRewardInfo.GetValue<ScratcherName>("scratcherName")).ToUpper();
                string textName = StringTableUtils.GetString(tableType, "COLLECTING_GAME_TOTAL_RESULT_NAME", scratcherName);
                MetaContextElementUtils.SetText(textNameElement, textName);

                string prize;
                if (prizeList.value[i] > 0)
                    prize = StringTableUtils.GetString(tableType, "COMMA_STYLE_COIN", prizeList.value[i]);
                else
                    prize = StringTableUtils.GetString(tableType, "COLLECTING_GAME_LOSE_TEXT");
                MetaContextElementUtils.SetText(textResultElement, prize);
                
                if ((ScratcherBigWinType) winTypeList.value[i] >= ScratcherBigWinType.BIG)
                {
                    string textWin = StringTableUtils.GetString(tableType, string.Format("COLLECTING_GAME_RESULT_{0}_WIN", ((ScratcherBigWinType) winTypeList.value[i]).ToString()));
                    MetaContextElementUtils.SetText(textWinElement, textWin);
                } 

                totalPrize += prizeList.value[i];
            }
            
            ContextElement buttonTextElement = ContextUtils.FindElement(buttonElement, "Text", ContextSearchingType.ChildrenSearch);
            string buttonText = totalPrize > 0 
                ? StringTableUtils.GetString(tableType, "COLLECTING_GAME_COLLECT_BUTTON_TEXT") 
                : StringTableUtils.GetString(tableType, "COLLECTING_GAME_OKAY_BUTTON_TEXT");
            MetaContextElementUtils.SetText(buttonTextElement, buttonText);

            if (totalPrize == 0)
            {
                ContextElement totalResultElement = ContextUtils.FindElement(agent, "Total Result", ContextSearchingType.ChildrenSearch);
                totalResultElement.GetComponent<Animator>().enabled = false;

                ContextElement textElement = ContextUtils.FindElement(totalResultElement, "Text", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(textElement, false);
                
                ContextElement textCoinElement = ContextUtils.FindElement(totalResultElement, "Text Coin", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(textCoinElement, false);
                
                ContextElement textBetterLuckElement = ContextUtils.FindElement(totalResultElement, "Text Better Luck", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(textBetterLuckElement, true);

                isBetterLuck.value = true;
            }
            else
            {
                ContextElement textCoinElement = ContextUtils.FindElement(agent, "Total Result/Text Coin", ContextSearchingType.FullNameSearch);

                string textCoin = StringTableUtils.GetString(tableType, "COMMA_STYLE_COIN", totalPrize);
                MetaContextElementUtils.SetText(textCoinElement, textCoin);
                
                isBetterLuck.value = false;
            }
        }

        protected override void OnUpdate()
        {
            if ( elapsedTime >= waitTime.value ) {
                ContextElement rectElement = ContextUtils.FindElement(agent, "Scroll Rect", ContextSearchingType.ChildrenSearch);
                rectElement.GetComponent<ScrollRect>().enabled = true;
                EndAction();
            }
        }
    }
}
