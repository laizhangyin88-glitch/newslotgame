using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Popup")]
public class CheckPopupCount : ConditionTask<Blackboard> 
{
    public CompareMethod checkType = CompareMethod.EqualTo;
    public BBParameter<int> valueB;

    protected override string info
    {
        get { return "Popup count" + OperationUtils.GetCompareString(checkType) + valueB; }
    }

    protected override bool OnCheck() 
    {
        return OperationUtils.Compare(PopupManager.Instance.popupCount, valueB.value, checkType);
    }
}

}
