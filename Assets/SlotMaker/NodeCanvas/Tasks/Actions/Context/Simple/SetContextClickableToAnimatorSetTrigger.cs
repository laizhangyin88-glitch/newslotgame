using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextClickableToAnimatorSetTrigger : ActionTask<ContextElement>
{
	public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BBParameter<string> targetElementName;
	public ContextSearchingType targetSearchingType = ContextSearchingType.ChildrenSearch;
    public BBParameter<string> parameter;
    public BBParameter<int> parameterHashID;
    public BBParameter<bool> ignoreReset;

	protected override string info
	{
        get
        {
            if(ignoreReset.value == true)
            {
                return string.Format("(Additive) {0}.onClick += {1}.SetTrigger({2})", elementName, targetElementName, string.IsNullOrEmpty(parameter.value)? parameterHashID.ToString() : parameter.ToString());
            }

            return string.Format("(Reset) {0}.onClick += {1}.SetTrigger({2})", elementName, targetElementName, string.IsNullOrEmpty(parameter.value)? parameterHashID.ToString() : parameter.ToString());
        }
	}

	protected override void OnExecute()
	{
		ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        ContextElement targetElement = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), targetElementName.value, targetSearchingType);

		IContextClickable clickableElement = element as IContextClickable;
		if (clickableElement != null)
		{
            if (ignoreReset.value == false)
                clickableElement.RemoveAllListener();

			clickableElement.AddListenerOnClick( (ContextElement sender) => { SetTrigger(targetElement.GetComponent<Animator>()); } );

			EndAction();
		}
		else
		{
			Debug.LogError("[Context] " + elementName.value + " is not exist or not IContextClickable");
			EndAction(false);
		}
	}
    
    private void SetTrigger(Animator animator)
    {
        if (!string.IsNullOrEmpty(parameter.value)){
            animator.SetTrigger(parameter.value);
        } else {
            animator.SetTrigger(parameterHashID.value);
        }
    }
}

}
