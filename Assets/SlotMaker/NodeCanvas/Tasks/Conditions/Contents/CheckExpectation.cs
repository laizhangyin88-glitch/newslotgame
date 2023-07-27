using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents 
{

[Category("★ BagelCode/Contents")]
public class CheckExpectation : ConditionTask 
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> reelIndex;

    protected override string info { get { return string.Format("CheckExpectation({0})", reelIndex); } }

    protected override bool OnCheck() 
    {  
        return ContentCustomData.GetSlotData(slotIndex.value).expectation.expectations[reelIndex.value];
    }
}

}
