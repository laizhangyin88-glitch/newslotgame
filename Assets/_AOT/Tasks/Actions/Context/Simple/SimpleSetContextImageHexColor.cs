using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextImageHexColor : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;    
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BBParameter<string> valueA;

    protected override string info
    {
        get { return string.Format("{0}.color = {1}", elementName, valueA); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);
        if (element == null)
        {
            Debug.LogError("[Context] " + elementName.value + " is not found");
            EndAction(false);
        }
        else 
        {
            IContextImage image = element as IContextImage;

            Blackboard bb = agent.GetComponent<Blackboard>();
            var hexColor = BlackboardUtils.FindVariable<string>(bb, valueA.value);
            image.SetColor(FormatUtility.GetColor(hexColor.value));
            EndAction();
        }
    }
}

}
