using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Canvas")]
    public class SetCanvasGroupBlocksRaycasts : ActionTask<Transform>
    {
        public BBParameter<bool> on;

        protected override void OnExecute()
        {
            var canvasGroup = agent.GetComponent<UnityEngine.CanvasGroup>();
            canvasGroup.blocksRaycasts = on.value;
            EndAction();
        }
    }

}
