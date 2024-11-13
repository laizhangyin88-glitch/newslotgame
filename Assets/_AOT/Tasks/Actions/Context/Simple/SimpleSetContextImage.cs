using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextImage : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;    
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BBParameter<Sprite> sprite;

    protected override string info
    {
        get { return string.Format("{0}.sprite = {1}", elementName, sprite); }
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

            image.SetSprite(sprite.value);
            EndAction();
        }
    }
}

}