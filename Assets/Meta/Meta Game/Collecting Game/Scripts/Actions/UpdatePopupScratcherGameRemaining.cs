using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class UpdatePopupScratcherGameRemaining : ActionTask<ContextElement>
    {
        public BBParameter<int> remainingCount;
        public BBParameter<bool> isAuto = false;
        public BBParameter<bool> isResult = false;
        
        protected override string info
        {
            get { return "Update Collecting Game Remaining"; }
        }

        protected override void OnExecute()
        {
            ContextElement scratcherElement = ContextUtils.FindElement(agent, "Scratcher", ContextSearchingType.ChildrenSearch);
            var scratcherBB = scratcherElement.GetComponent<Blackboard>();
            var bb = agent.GetComponent<Blackboard>();

            // High win to disable auto mode.
            var forceSetAutoDisableVar = (Variable<bool>)(bb.GetVariable<bool>("forceSetAutoDisable") ??
                bb.AddVariable("forceSetAutoDisable", true));

            var scratcherRewardResult = bb.GetValue<Blackboard>("_scratcherRewardResult");
            var winType = scratcherRewardResult.GetValue<ScratcherBigWinType>("bigWinType");
            if (forceSetAutoDisableVar.value && isResult.value &&
                (winType == ScratcherBigWinType.SUPER_MEGA || winType == ScratcherBigWinType.EPIC))
            {
                isAuto.value = false;
                forceSetAutoDisableVar.value = false;
            }
            
            scratcherBB.AddVariable("_isAuto", isAuto.value);

            if (isResult.value)
            {
                ContextElement buttonCollect = ContextUtils.FindElement(agent, "Collect Board/Collect", ContextSearchingType.FullNameSearch);
                ContextElement buttonContinue = ContextUtils.FindElement(agent, "Collect Board/Continue", ContextSearchingType.FullNameSearch);
                
                if (isAuto.value)
                {
                    MetaContextElementUtils.SetActive(buttonCollect, false);
                    MetaContextElementUtils.SetActive(buttonContinue, false);
                }
                else
                {
                    MetaContextElementUtils.SetActive(buttonCollect, true);
                    MetaContextElementUtils.SetActive(buttonContinue, remainingCount.value > 0);
                }
            }

            HandleRemainingArea();
            EndAction(true);
        }

        private void HandleRemainingArea()
        {
            ContextElement remainingAreaElement = ContextUtils.FindElement(agent, "Remaining Area", ContextSearchingType.ChildrenSearch);
            ContextElement textRemainingElement = ContextUtils.FindElement(remainingAreaElement, "Text Remaining", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetText(textRemainingElement, remainingCount.value.ToString());
    
            ContextElement autoElement = ContextUtils.FindElement(remainingAreaElement, "Button Auto Scratch/Auto", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetActive(autoElement, isAuto.value);
        }
    }
}
