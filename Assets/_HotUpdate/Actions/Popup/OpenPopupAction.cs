using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Collections;
using BagelCode;

namespace SlotMaker.Task.Actions
{
    public class OpenPopupAction : ActionTask<MonoBehaviour>
    {
        public BBParameter<ActionOpenPopupType> type;

        protected override string info
        {
            get { return "Open Popup Action: " + type.value; }
        }

        protected override void OnExecute()
        {
            StartCoroutine(OpenPopupCoroutine());
        }

        private IEnumerator OpenPopupCoroutine()
        {
            yield return StartCoroutine(IAMUtils.OpenPopupCoroutine(agent, type.value));

            EndAction();
        }
    }
}
