using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Context")]
public class SimpleSetContextSliderParameter : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;

    public BBParameter<bool> isUseRange;
    public BBParameter<float> minRange;
    public BBParameter<float> maxRange;

    protected override string info
    {
        get { return string.Format("{0}.SliderParameter", elementName); }
    }

    protected override void OnExecute()
    {
        ContextElement element = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType);

        ContextSlider sliderElement = element as ContextSlider;
        if (sliderElement == null)
        {
            Debug.LogError("[Context] " + agent.ContextName + " is not IContextIntsliderElement");
            EndAction(false);
        }
        else
        {
            sliderElement.SetSliderRangeParameter(isUseRange.value, minRange.value, maxRange.value);
            EndAction();
        }
    }
}

}
