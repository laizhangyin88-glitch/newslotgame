using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.UI;
using SlotMaker;
using ParadoxNotion.Design;

namespace BagelCode
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class InitCollectingGameLoadingBarHandle : ActionTask<Blackboard>
    {
        protected override void OnExecute()
        {
            var progressBar = agent.GetVariable<ContextElement>("_progressBar")?.value;
            var progressBarIcon = agent.GetVariable<ContextElement>("_progressBarIcon")?.value;
            if(progressBar != null && progressBarIcon != null)
            {
                progressBar.gameObject.GetComponent<Slider>().handleRect = progressBarIcon.gameObject.GetComponent<RectTransform>();
            }
            EndAction();
        }
    }
}
