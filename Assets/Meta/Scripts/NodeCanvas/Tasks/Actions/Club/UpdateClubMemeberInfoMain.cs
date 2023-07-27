using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
 {
     
     [Category("★ BagelCode/Club")]
     public class UpdateClubMemberInfoMain : ActionTask<ContextElement>
     {
         public BBParameter<bool> enableShare;

         public BBParameter<string> donationText;
         public BBParameter<string> contributionText;
         public BBParameter<string> leaguePointText;
         public BBParameter<string> noticeText;

         public BBParameter<string> informationBalloonText;
         public BBParameter<string> donationBalloonText;
         public BBParameter<string> contributionBalloonText;
         public BBParameter<string> leaguePointBalloonText;
         public BBParameter<string> giftsSentBalloonText;
         
         public BBParameter<string> leaguePointBalloonText1;
         public BBParameter<string> leaguePointBalloonText2;
         public BBParameter<bool> leaguePointBalloonChart1Enabled;
         public BBParameter<bool> leaguePointBalloonChart1Applicabled;
         public BBParameter<string> leaguePointBalloonChart1Text;
         public BBParameter<string> leaguePointBalloonChart1Percent;
         public BBParameter<bool> leaguePointBalloonChart2Enabled;
         public BBParameter<bool> leaguePointBalloonChart2Applicabled;
         public BBParameter<string> leaguePointBalloonChart2Text;
         public BBParameter<string> leaguePointBalloonChart2Percent;
         public BBParameter<bool> leaguePointBalloonChart3Enabled;
         public BBParameter<bool> leaguePointBalloonChart3Applicabled;
         public BBParameter<string> leaguePointBalloonChart3Text;
         public BBParameter<string> leaguePointBalloonChart3Percent;
         public BBParameter<string> leaguePointBalloonChart3CoverDisabledText;
         
         protected override string info
         {
             get { return "Update Club Member Info Main"; }
         }
 
         protected override void OnExecute()
         {
             var donationTextElement = ContextUtils.FindElement(agent, "Member/Text Donation", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(donationTextElement, donationText.value);
             
             var contributionTextElement = ContextUtils.FindElement(agent, "Member/Text Contribution", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(contributionTextElement, contributionText.value);
             
             var leaguePointTextElement = ContextUtils.FindElement(agent, "Member/Text League Point", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointTextElement, leaguePointText.value);
             
             var noticeTextElement = ContextUtils.FindElement(agent, "News Feed/Text News Feed Club Notice", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(noticeTextElement, noticeText.value);
             
             var informationBalloonTextElement = ContextUtils.FindElement(agent, "Member/Club Information Area/Club Information Speech Bubble/Text", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(informationBalloonTextElement, informationBalloonText.value);
             
             var donationBalloonTextElement = ContextUtils.FindElement(agent, "Member/Donation Area/Donation Speech Bubble/Text", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(donationBalloonTextElement, donationBalloonText.value);
             
             var contributionBalloonTextElement = ContextUtils.FindElement(agent, "Member/Contribution Area/Contribution Speech Bubble/Text", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(contributionBalloonTextElement, contributionBalloonText.value);
             
             var leaguePointBalloonTextElement = ContextUtils.FindElement(agent, "Member/League Point Area/League Points Speech Bubble/Text", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonTextElement, leaguePointBalloonText.value);

             var giftsSendElement = ContextUtils.FindElement(agent, "Member/Gifts Sent", ContextSearchingType.FullNameSearch);
             giftsSendElement.gameObject.SetActive(enableShare.value);
             var giftsSentBalloonTextElement = ContextUtils.FindElement(giftsSendElement, "Gifts Sent Area/Gifts Sent Speech Bubble/Text", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(giftsSentBalloonTextElement, giftsSentBalloonText.value);
             
             var leaguePointBalloonText1Element = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Text 1", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonText1Element, leaguePointBalloonText1.value);
             
             var leaguePointBalloonText2Element = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Text 2", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonText2Element, leaguePointBalloonText2.value);
             
             var leaguePointBalloonChart1Text1Element = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 1/Text 01", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonChart1Text1Element, leaguePointBalloonChart1Text.value);
             
             var leaguePointBalloonChart1Text2Element = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 1/Text 02", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonChart1Text2Element, leaguePointBalloonChart1Percent.value);

             var leaguePointBalloonChart1CoverElement = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 1/Cover 01", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart1CoverElement, !leaguePointBalloonChart1Enabled.value);
             
             var leaguePointBalloonChart1CheckBoxElement = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 1/Check Box", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart1CheckBoxElement, leaguePointBalloonChart1Enabled.value);
             
             var leaguePointBalloonChart2Text1Element = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 2/Text 01", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonChart2Text1Element, leaguePointBalloonChart2Text.value);
             
             var leaguePointBalloonChart2Text2Element = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 2/Text 02", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonChart2Text2Element, leaguePointBalloonChart2Percent.value);
             
             var leaguePointBalloonChart2CoverElement = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 2/Cover 01", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart2CoverElement, !leaguePointBalloonChart2Enabled.value);
             
             var leaguePointBalloonChart2CheckBoxElement = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 2/Check Box", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart2CheckBoxElement, leaguePointBalloonChart2Enabled.value);
             
             var leaguePointBalloonChart3Text1Element = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 3/Text 01", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonChart3Text1Element, leaguePointBalloonChart3Text.value);
             
             var leaguePointBalloonChart3Text2Element = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 3/Text 02", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonChart3Text2Element, leaguePointBalloonChart3Percent.value);
             
             var leaguePointBalloonChart3CoverElement = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 3/Cover 01", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart3CoverElement, !leaguePointBalloonChart3Enabled.value);
             
             var leaguePointBalloonChart3CheckBoxElement = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 3/Check Box", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart3CheckBoxElement, leaguePointBalloonChart3Enabled.value);
             
             var leaguePointBalloonChart3CoverDisabledElement = ContextUtils.FindElement(agent, "Member/League Point Table Area/Club Speech Balloon League Points/Event Chart 3/Cover Disabled", ContextSearchingType.FullNameSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart3CoverDisabledElement, leaguePointBalloonChart3Enabled.value && !leaguePointBalloonChart3Applicabled.value);

             var leaguePointBalloonChart3CoverDisabledTextElement = ContextUtils.FindElement(leaguePointBalloonChart3CoverDisabledElement, "Text", ContextSearchingType.ChildrenSearch);
             MetaContextElementUtils.SetText(leaguePointBalloonChart3CoverDisabledTextElement, leaguePointBalloonChart3CoverDisabledText.value);
             
             
             
             var leaguePointBalloonChart1CheckBoxActiveElement = ContextUtils.FindElement(leaguePointBalloonChart1CheckBoxElement, "Active", ContextSearchingType.ChildrenSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart1CheckBoxActiveElement, leaguePointBalloonChart1Applicabled.value);
             
             var leaguePointBalloonChart1CheckBoxInActiveElement = ContextUtils.FindElement(leaguePointBalloonChart1CheckBoxElement, "InActive", ContextSearchingType.ChildrenSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart1CheckBoxInActiveElement, !leaguePointBalloonChart1Applicabled.value);
             
             var leaguePointBalloonChart2CheckBoxActiveElement = ContextUtils.FindElement(leaguePointBalloonChart2CheckBoxElement, "Active", ContextSearchingType.ChildrenSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart2CheckBoxActiveElement, leaguePointBalloonChart2Applicabled.value);
             
             var leaguePointBalloonChart2CheckBoxInActiveElement = ContextUtils.FindElement(leaguePointBalloonChart2CheckBoxElement, "InActive", ContextSearchingType.ChildrenSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart2CheckBoxInActiveElement, !leaguePointBalloonChart2Applicabled.value);
             
             var leaguePointBalloonChart3CheckBoxActiveElement = ContextUtils.FindElement(leaguePointBalloonChart3CheckBoxElement, "Active", ContextSearchingType.ChildrenSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart3CheckBoxActiveElement, leaguePointBalloonChart3Applicabled.value);
             
             var leaguePointBalloonChart3CheckBoxInActiveElement = ContextUtils.FindElement(leaguePointBalloonChart3CheckBoxElement, "InActive", ContextSearchingType.ChildrenSearch);
             MetaContextElementUtils.SetActive(leaguePointBalloonChart3CheckBoxInActiveElement, !leaguePointBalloonChart3Applicabled.value);
             
             EndAction();
         }
     }
 }