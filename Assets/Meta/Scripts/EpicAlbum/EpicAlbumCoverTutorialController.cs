using SlotMaker;
using UnityEngine;

namespace BagelCode.EpicAlbum
{
    public class EpicAlbumCoverTutorialController : MonoBehaviour
    {
        private ContextElement agent;
        private ContextElement tutorial1;
        private ContextElement tutorial2;
        
        private ContextElement tutorial1Anchor1;
        private ContextElement tutorial2Anchor1;
        
        private ContextElement speechBalloon1;
        private ContextElement speechBalloon2;
        
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
            string tutorial1Anchor1SpeechBalloonText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_COVER_TUTORIAL_1_SPEECH_BALLOON_1_TEXT");
            MetaContextElementUtils.SetText(tutorial1Anchor1SpeechBalloonTextElement, tutorial1Anchor1SpeechBalloonText);

            tutorial2 = ContextUtils.FindElement(agent, "Tutorial 2", ContextSearchingType.ChildrenSearch);
            tutorial2Anchor1 = ContextUtils.FindElement(tutorial2, "Speech Balloon Anchor 2", ContextSearchingType.ChildrenSearch);
            speechBalloon2 = ContextUtils.FindElement(tutorial2Anchor1, "Tutorial Speech Balloon Top", ContextSearchingType.ChildrenSearch);
            ContextElement tutorial2Anchor1SpeechBalloonTextElement = ContextUtils.FindElement(speechBalloon2, "Anchor/Text", ContextSearchingType.FullNameSearch);
            string tutorial2Anchor1SpeechBalloonText = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_COVER_TUTORIAL_2_SPEECH_BALLOON_1_TEXT");
            MetaContextElementUtils.SetText(tutorial2Anchor1SpeechBalloonTextElement, tutorial2Anchor1SpeechBalloonText);
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

        private void SetActive(int stage)
        {
            MetaContextElementUtils.SetActive(tutorial1, stage == 1);
            MetaContextElementUtils.SetActive(tutorial2, stage == 2);
        }
    }
}