using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;

namespace SlotMaker.AnimatorBehaviour
{

    public class CloseContentPopup : StateMachineBehaviour
    {
        public bool atExit;
        public string eventName;

    	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    	{
    		if (!atExit) ClosePopup(animator.gameObject);
    	}

    	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    	{
    		if (atExit) ClosePopup(animator.gameObject);
    	}

        private void ClosePopup(GameObject go)
        {
            PopupManager.Instance.Close();

            if (!string.IsNullOrEmpty(eventName))
            {
                var bb = go.GetComponent<Blackboard>();
                var caller = bb.GetValue<GameObject>("caller");
                caller.GetComponent<GraphOwner>().SendEvent(eventName);
            }

            Destroy(go);
        }
    }

}
