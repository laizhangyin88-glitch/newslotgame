using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{

[Description("Find element that has Animator and set Int parameter. You can either use a parameter name OR hashID. Leave the parameter name empty or none to use hashID instead.")]
[Category("★ SlotMaker/Context")]
public class SimpleSetAnimatorParameterInt : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BBParameter<string> parameter;
    public BBParameter<int> parameterHashID;
    public BBParameter<int> setTo;

    protected override string info
    {
        get { return string.Format("{0}.animator.SetInt {1} to {2}", elementName, string.IsNullOrEmpty(parameter.value)? parameterHashID.ToString() : parameter.ToString(), setTo ); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

        if (element != null)
        {
            Animator animator = element.gameObject.GetComponent<Animator>();

            if (animator != null)
            {
                if (!string.IsNullOrEmpty(parameter.value))
                {
                    element.transform.GetComponent<Animator>().SetInteger(parameter.value, setTo.value);
                } 
                else 
                {
                    element.transform.GetComponent<Animator>().SetInteger(parameterHashID.value, setTo.value);
                }
            }
            else
            {
                Debug.LogError(elementName.value + " Doesn't have animator");
            }
        }
        else
        {
            Debug.LogError("[Context] " + elementName.value + " is not exist");
        }

        EndAction();
    }
}

}
