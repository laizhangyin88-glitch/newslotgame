using SlotMaker;
using UnityEngine;

namespace BagelCode.EpicAlbum
{
    public class EpicAlbumTutorialController : MonoBehaviour
    {
        private ContextElement agent;
        private ContextElement tutorial1;
        private ContextElement tutorial2;
        private ContextElement tutorial3;
        private ContextElement tutorial4;
        
        private ContextElement tutorial1Anchor1;
        private ContextElement tutorial2Anchor1;
        private ContextElement tutorial3Anchor1;
        private ContextElement tutorial4Anchor1;
        
        private ContextElement speechBalloon1;
        private ContextElement speechBalloon2;
        private ContextElement speechBalloon3;
        private ContextElement speechBalloon4;
        
        private string ON_NEXT_EVENT = "OnNext";
        
        public void OnInit()
        {
            agent = GetComponent<ContextElement>();

            ContextElement touchAreaFullButton = ContextUtils.FindElement(agent, "Touch Area Full", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                touchAreaFullButton,
                ON_NEXT_EVENT,
                agent,
                null
            );

            tutorial1 = ContextUtils.FindElement(agent, "Tutorial 1", ContextSearchingType.ChildrenSearch);
            tutorial1Anchor1 = ContextUtils.FindElement(tutorial1, "Speech Balloon Anchor 1", ContextSearchingType.ChildrenSearch);
            speechBalloon1 = ContextUtils.FindElement(tutorial1Anchor1, "Tutorial Speech Balloon Top", ContextSearchingType.ChildrenSearch);
            ContextElement tutorial1Anchor1SpeechBalloonTextElement = ContextUtils.FindElement(speechBalloon1, "Anchor/Text", ContextSearchingType.FullNameSearch);
            string tutorial1Anchor1SpeechBalloonText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_TUTORIAL_1_SPEECH_BALLOON_1_TEXT");
            MetaContextElementUtils.SetText(tutorial1Anchor1SpeechBalloonTextElement, tutorial1Anchor1SpeechBalloonText);
            
            tutorial2 = ContextUtils.FindElement(agent, "Tutorial 2", ContextSearchingType.ChildrenSearch);
            tutorial2Anchor1 = ContextUtils.FindElement(tutorial2, "Speech Balloon Anchor 2", ContextSearchingType.ChildrenSearch);
            speechBalloon2 = ContextUtils.FindElement(tutorial2Anchor1, "Tutorial Speech Balloon Right Bottom", ContextSearchingType.ChildrenSearch);
            ContextElement tutorial2Anchor1SpeechBalloonTextElement = ContextUtils.FindElement(speechBalloon2, "Anchor/Text", ContextSearchingType.FullNameSearch);
            string tutorial2Anchor1SpeechBalloonText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_TUTORIAL_2_SPEECH_BALLOON_1_TEXT");
            MetaContextElementUtils.SetText(tutorial2Anchor1SpeechBalloonTextElement, tutorial2Anchor1SpeechBalloonText);
            
            tutorial3 = ContextUtils.FindElement(agent, "Tutorial 3", ContextSearchingType.ChildrenSearch);
            tutorial3Anchor1 = ContextUtils.FindElement(tutorial3, "Speech Balloon Anchor 3", ContextSearchingType.ChildrenSearch);
            speechBalloon3 = ContextUtils.FindElement(tutorial3Anchor1, "Tutorial Speech Balloon Left Side Bottom", ContextSearchingType.ChildrenSearch);
            ContextElement tutorial3Anchor1SpeechBalloonTextElement = ContextUtils.FindElement(speechBalloon3, "Anchor/Text", ContextSearchingType.FullNameSearch);
            string tutorial3Anchor1SpeechBalloonText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_TUTORIAL_3_SPEECH_BALLOON_1_TEXT");
            MetaContextElementUtils.SetText(tutorial3Anchor1SpeechBalloonTextElement, tutorial3Anchor1SpeechBalloonText);

            tutorial4 = ContextUtils.FindElement(agent, "Tutorial 4", ContextSearchingType.ChildrenSearch);
            tutorial4Anchor1 = ContextUtils.FindElement(tutorial4, "Speech Balloon Anchor 4", ContextSearchingType.ChildrenSearch);
            speechBalloon4 = ContextUtils.FindElement(tutorial4Anchor1, "Tutorial Speech Balloon Left Bottom", ContextSearchingType.ChildrenSearch);
            ContextElement tutorial4Anchor1SpeechBalloonTextElement = ContextUtils.FindElement(speechBalloon4, "Anchor/Text", ContextSearchingType.FullNameSearch);
            string tutorial4Anchor1SpeechBalloonText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_TUTORIAL_4_SPEECH_BALLOON_1_TEXT");
            MetaContextElementUtils.SetText(tutorial4Anchor1SpeechBalloonTextElement, tutorial4Anchor1SpeechBalloonText);
        }

        public void FirstStep()
        {
            SetActive(1);
            speechBalloon1.GetComponent<Animator>().SetTrigger("Appear");
        }
        
        public void SecondStep()
        {
            SetActive(2);
            
            speechBalloon2.GetComponent<Animator>().SetTrigger("Appear");
        }
        
        public void ThirdStep()
        {
            SetActive(3);
            
            speechBalloon3.GetComponent<Animator>().SetTrigger("Appear");
        }

        public void FourStep()
        {
            SetActive(4);
            
            speechBalloon4.GetComponent<Animator>().SetTrigger("Appear");
        }

        private void SetActive(int stage)
        {
            MetaContextElementUtils.SetActive(tutorial1, stage == 1);
            MetaContextElementUtils.SetActive(tutorial2, stage == 2);
            MetaContextElementUtils.SetActive(tutorial3, stage == 3);
            MetaContextElementUtils.SetActive(tutorial4, stage == 4);
        }
    }
}