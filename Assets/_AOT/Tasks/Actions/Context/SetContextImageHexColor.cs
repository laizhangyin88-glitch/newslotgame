using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SetContextImageHexColor : ActionTask<Blackboard>
{
    public BBParameter<ContextElement> element;
    public BBParameter<string> valueA;

    protected override string info
    {
        get { return string.Format("{0}.color = {1}", element, valueA); }
    }

    protected override void OnExecute()
    {
        IContextImage image = element.value as IContextImage;
        if (image == null)
        {
            Debug.LogError("[Context] " + element.value.ContextName + " is not IContextImage");
            EndAction(false);
        }
        else 
        {
            var hexColor = BlackboardUtils.FindVariable<string>(agent, valueA.value);
            image.SetColor(FormatUtility.GetColor(hexColor.value));
            EndAction();
        }
    }
}

}
