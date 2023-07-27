using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Tutorial")]
    public class UpdateInGameTutorial : ActionTask<ContextElement>
    {
        public BBParameter<long> timeStamp;
        public BBParameter<int> isTimeStage;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;
        
        protected override string info
        {
            get { return "Update In Game Tutorial"; }
        }

        protected override void OnExecute()
        {
            var stage = BlackboardUtils.FindVariable<int>(null, "/tutorialInfo/stage");
            var count = BlackboardUtils.FindVariable<long>(null, "/tutorialInfo/count");
            var level = BlackboardUtils.FindVariable<int>(null, "/userSyncInfo/level");
            if (level == null) level = BlackboardUtils.FindVariable<int>(null, "/me/level");

            ContextElement progressBarElement = ContextUtils.FindElement(agent, "Progress Bar", ContextSearchingType.ChildrenSearch);
            ContextElement progressTextElement = ContextUtils.FindElement(agent, "Progress Text", ContextSearchingType.ChildrenSearch);
            ContextElement textElement = ContextUtils.FindElement(agent, "Text", ContextSearchingType.ChildrenSearch);

            if (stage.value == 0)
            {
                MetaContextElementUtils.SetSliderValue(progressBarElement, count.value / 7f);
                string progressText = StringTableUtils.GetString(tableType, "A_PER_B", count.value, 7);
                MetaContextElementUtils.SetText(progressTextElement, progressText);
                
                string questName = StringTableUtils.GetString(tableType, "TUTORIAL_PLAY_QUEST_TEXT_1");
                MetaContextElementUtils.SetText(textElement, questName);

                UpdateTimer(stage.value + 1);
            } 
            else if (stage.value == 1)
            {
                MetaContextElementUtils.SetSliderValue(progressBarElement, level.value / 4f);
                string progressText = StringTableUtils.GetString(tableType, "A_PER_B", level.value, 4);
                MetaContextElementUtils.SetText(progressTextElement, progressText);
                
                string questName = StringTableUtils.GetString(tableType, "TUTORIAL_PLAY_QUEST_TEXT_2");
                MetaContextElementUtils.SetText(textElement, questName);

                UpdateTimer(stage.value + 1);
            } 
            else if (stage.value == 2)
            {
                MetaContextElementUtils.SetSliderValue(progressBarElement, (level.value - 4) / 2f);
                string progressText = StringTableUtils.GetString(tableType, "A_PER_B", level.value - 4, 2);
                MetaContextElementUtils.SetText(progressTextElement, progressText);
                
                string questName = StringTableUtils.GetString(tableType, "TUTORIAL_PLAY_QUEST_TEXT_3");
                MetaContextElementUtils.SetText(textElement, questName);

                UpdateTimer(stage.value + 1);
            }
            else if (stage.value == 3)
            {
                MetaContextElementUtils.SetSliderValue(progressBarElement, 1f);
                string progressText = StringTableUtils.GetString(tableType, "A_PER_B", level.value - 4, 2);
                MetaContextElementUtils.SetText(progressTextElement, progressText);
            }
            
            EndAction();
        }

        private void UpdateTimer(int timeStage)
        {
            Animator animator = agent.GetComponent<Animator>();

            if (isTimeStage.value != timeStage)
            {
                ContextElement timerElement = ContextUtils.FindElement(agent, "Timer Anchor/Event Tag Without Text/Remaining Timer", ContextSearchingType.FullNameSearch);
                var timerBB = timerElement.GetComponent<Blackboard>();

                long timerTime = 1000 * (long)GetTutorialTime(timeStage - 1);
                if(timerTime != 0)
                {
                    animator.SetBool("IsTimer", true);

                    timeStamp.value = timerTime + TimeUtils.GetTimeStamp();

                    MetaContextElementUtils.SetCommonRemainingTimer(
                        timerElement,
                        timeStamp.value,
                        0,
                        "TIME_FORMAT_MMSS",
                        "",
                        "",
                        StringTableUtils.GetString(StringTable.StringTableType.Global, "META_GAME_ENDED"),
                        true,
                        agent.gameObject
                    );
                }
                else
                {
                    timeStamp.value = 0;
                    animator.SetBool("IsTimer", false);
                }

                isTimeStage.value = timeStage;
            }
            else if(timeStamp.value != 0 && timeStamp.value < TimeUtils.GetTimeStamp())
            {
                animator.SetBool("IsTimer", false);
            }
        }

        private int GetTutorialTime(int stageIndex)
        {
            if(stageIndex == 0)
                return BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/misc/TUTORIAL_TIMER_SEC/SPIN_SEVEN_TIMES").value;

            if(stageIndex == 1)
                return BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/misc/TUTORIAL_TIMER_SEC/REACH_LEVEL_FOUR").value;

            return BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/misc/TUTORIAL_TIMER_SEC/TWO_MORE_LEVEL").value;
        }
    }
}