using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Common")]
public class SetFlippingText : ActionTask<Blackboard>
{
    public BBParameter<ContextElement> element;
    public BBParameter<bool> isFlip;
    public BBParameter<string> defaultText;
    public BBParameter<List<string>> flippingTextList;
    public BBParameter<float> intervalTime;

    protected override string info
    {
        get { return string.Format("Set flipping text"); }
    }

    protected override void OnExecute()
    {
        Blackboard bb = element.value.GetComponent<Blackboard>();

        var defaultTextValue = BlackboardUtils.FindVariable<string>(agent, defaultText.value);
        string initText = null;
        if(defaultTextValue != null)
        {
            initText = defaultTextValue.value;
            // BlackboardUtils.SetOrCreateValue<string>(bb, "defaultText", defaultTextValue.value);
        }

        List<string> textList = new List<string>();

        if(flippingTextList != null && flippingTextList.value != null)
        {
            for(int i = 0; i < flippingTextList.value.Count; ++i)
            {
                var text = BlackboardUtils.FindVariable<string>(agent, flippingTextList.value[i]);
                if(text != null && text.value != null)
                {
                    textList.Add(text.value);
                }
            }

            // BlackboardUtils.SetOrCreateValue<List<string>>(bb, "flippingTextList", textList);
        }

        // BlackboardUtils.SetOrCreateValue<float>(bb, "intervalTime", intervalTime.value);
        // BlackboardUtils.SetOrCreateValue<bool>(bb, "isQuickStart", isFlip.value);
        // BlackboardUtils.SetOrCreateValue<bool>(bb, "isRefresh", true);

        // element.value.gameObject.SetActive(true);

        MetaContextElementUtils.SetFlippingText(element.value, initText, textList, isFlip.value, intervalTime.value);

        EndAction();
    }
}

}
