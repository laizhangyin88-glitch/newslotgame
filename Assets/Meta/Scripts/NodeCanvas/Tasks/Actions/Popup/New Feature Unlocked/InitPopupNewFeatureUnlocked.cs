using System.Collections.Generic;
using BagelCode.ClientModels;
using BagelCode.Tasks.Actions.BI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Popup/New Feature Unlocked")]
    public class InitPopupNewFeatureUnlocked : ActionTask<ContextElement> 
    {
        public BBParameter<LockedFeatureType> featureType;
        public BBParameter<BI_tutorial.TutorialType> tutorialType;
        
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        private string ON_CLOSE_EVENT = "OnClose";
        
        private string ON_CHALLENGE_EVENT = "OnChallenge";
        private string ON_TOURNAMENT_EVENT = "OnTournament";
        private string ON_CLUB_EVENT = "OnClub";
        private string ON_CLUB_CLOSE_EVENT = "OnClubClose";
        private string ON_EARLY_ACCESS_EVENT = "OnEarlyAccess";
        private string ON_EARLY_ACCESS_CLOSE_EVENT = "OnEarlyAccessClose";

        private ContextElement checkButtonElement;
        private ContextElement laterButtonElement;
        private ContextElement closeButtonElement;
        
        protected override string info
        {
            get { return string.Format("Init Popup New Feature Unlocked"); }
        }
        
        protected override void OnExecute()
        {
            ContextElement titleTextElement = ContextUtils.FindElement(agent, "Title Text", ContextSearchingType.ChildrenSearch);
            ContextElement infoTextElement = ContextUtils.FindElement(agent, "Info Text", ContextSearchingType.ChildrenSearch);
            checkButtonElement = ContextUtils.FindElement(agent, "Button Check", ContextSearchingType.ChildrenSearch);
            ContextElement checkButtonTextElement = ContextUtils.FindElement(checkButtonElement, "Text", ContextSearchingType.ChildrenSearch);
            laterButtonElement = ContextUtils.FindElement(agent, "Button Later", ContextSearchingType.ChildrenSearch);
            ContextElement laterButtonTextElement = ContextUtils.FindElement(laterButtonElement, "Text", ContextSearchingType.ChildrenSearch);
            closeButtonElement = ContextUtils.FindElement(agent, "Close", ContextSearchingType.ChildrenSearch);
            
            ContextElement iconEventButtonElement = ContextUtils.FindElement(agent, "Icon Event Button", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetActive(iconEventButtonElement, false);
            
            string checkButtonText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_CHECK_BUTTON");
            MetaContextElementUtils.SetText(checkButtonTextElement, checkButtonText);

            string laterButtonText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_LATER_BUTTON");
            MetaContextElementUtils.SetText(laterButtonTextElement, laterButtonText);            

            string titleText = "";
            string infoText = "";
            
            if (featureType.value == LockedFeatureType.CHALLENGE)
            {
                ContextElement iconChallengeElement = ContextUtils.FindElement(agent, "Icon Challenge", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(iconChallengeElement, true);
                
                titleText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_CHALLENGE_TITLE");
                infoText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_CHALLENGE_INFO");
                
                MetaContextElementUtils.SetActive(laterButtonElement, true);
                SetClickableButtons(ON_CHALLENGE_EVENT, ON_CLOSE_EVENT, ON_CLOSE_EVENT);
                tutorialType.value = BI_tutorial.TutorialType.CHALLENGE;
            } 
            else if (featureType.value == LockedFeatureType.TOURNAMENT)
            {
                ContextElement iconTournamentElement = ContextUtils.FindElement(agent, "Icon Tournament", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(iconTournamentElement, true);
                
                titleText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_TOURNAMENT_TITLE");
                infoText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_TOURNAMENT_INFO");
                
                MetaContextElementUtils.SetActive(laterButtonElement, false);
                SetClickableButtons(ON_TOURNAMENT_EVENT, null, ON_CLOSE_EVENT);
                
                tutorialType.value = BI_tutorial.TutorialType.TOURNAMENT;
            } 
            else if (featureType.value == LockedFeatureType.CLUB)
            {
                ContextElement iconClubElement = ContextUtils.FindElement(agent, "Icon Club", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(iconClubElement, true);
                
                titleText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_CLUB_TITLE");
                infoText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_CLUB_INFO");
                
                MetaContextElementUtils.SetActive(laterButtonElement, false);
                SetClickableButtons(ON_CLUB_EVENT, null, ON_CLUB_CLOSE_EVENT);
                
                tutorialType.value = BI_tutorial.TutorialType.CLUB;
            }
            else if (featureType.value == LockedFeatureType.EARLY_ACCESS)
            {
                ContextElement iconEarlyAccessElement = ContextUtils.FindElement(agent, "Icon Early Access", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetActive(iconEarlyAccessElement, true);

                titleText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_EARLY_ACCESS_TITLE");
                infoText = StringTableUtils.GetString(tableType, "POPUP_NEW_FEATURE_UNLOCKED_EARLY_ACCESS_INFO");

                MetaContextElementUtils.SetActive(laterButtonElement, false);
                SetClickableButtons(ON_EARLY_ACCESS_EVENT, null, ON_EARLY_ACCESS_CLOSE_EVENT);
                
                List<Blackboard> earlyAccessGameInfoList = BlackboardQueryUtils.GetEarlyAccessGameInfoList();
        
                if (earlyAccessGameInfoList != null && earlyAccessGameInfoList.Count >= 3)
                {
                    var gameTitle = BlackboardUtils.FindVariable<string>(earlyAccessGameInfoList[1], "gameTitle");
                    var slotThumbnailAnchorElement = ContextUtils.FindElement(iconEarlyAccessElement, "Slot Thumbnail Anchor", ContextSearchingType.ChildrenSearch);
                    MetaIconUtils.MakeSlotThumbnailIconObjectFromGameTitle(gameTitle.value, slotThumbnailAnchorElement.transform, null);
                }
                else
                {
                    var textElement = ContextUtils.FindElement(iconEarlyAccessElement, "Text", ContextSearchingType.ChildrenSearch);
                    MetaContextElementUtils.SetActive(textElement, true);
                }
                
                tutorialType.value = BI_tutorial.TutorialType.EARLY_ACCESS;
            }

            MetaContextElementUtils.SetText(titleTextElement, titleText);
            MetaContextElementUtils.SetText(infoTextElement, infoText);
            
            EndAction();
        }

        private void SetClickableButtons(string CHECK_BUTTON_EVENT, string LATER_BUTTON_EVENT, string CLOSE_BUTTON_EVENT)
        {
            if (CHECK_BUTTON_EVENT != null)
            {
                MetaContextElementUtils.SetClickable(
                    checkButtonElement,
                    CHECK_BUTTON_EVENT,
                    false,
                    false,
                    SendEvent,
                    ownerSystem
                );
            }

            if (LATER_BUTTON_EVENT != null)
            {
                MetaContextElementUtils.SetClickable(
                    laterButtonElement,
                    LATER_BUTTON_EVENT,
                    false,
                    false,
                    SendEvent,
                    ownerSystem
                );
            }

            if (CLOSE_BUTTON_EVENT != null)
            {
                MetaContextElementUtils.SetClickable(
                    closeButtonElement,
                    CLOSE_BUTTON_EVENT,
                    false,
                    false,
                    SendEvent,
                    ownerSystem
                );
            }
        }
    }
}
