using UnityEngine;
using System;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Common")]
public class UpdateFlippingText : ActionTask<Blackboard>
{
    public BBParameter<ContextElement> textElement;

    public BBParameter<string> defaultText;
    public BBParameter<List<string>> flipTextList;
    public BBParameter<int> currentIndex;

    public BBParameter<bool> saveAsDefault;

    protected override string info
    {
        get { return string.Format("Update flipping text"); }
    }

    protected override void OnExecute()
    {
        if(flipTextList.value.Count < 2)
        {
            currentIndex.value = 0;
            saveAsDefault.value = true;
        }
        else
        {
            if(currentIndex.value >= flipTextList.value.Count)
            {
                currentIndex.value = 0;
            }

            MetaContextElementUtils.SetText(textElement.value, flipTextList.value[currentIndex.value]);


            currentIndex.value += 1;
            currentIndex.value = currentIndex.value%flipTextList.value.Count;

            saveAsDefault.value = false;
        }

        EndAction();
    }
}

}
