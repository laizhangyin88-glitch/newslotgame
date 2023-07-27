using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Condition
{

[Category("★ BagelCode/NativeHelper")]
public class CheckVideoAdsAvailable : ConditionTask
{   
    public BBParameter<string> placement;

    protected override string info
    {
        get { return string.Format("Check {0} ads is playable", placement); }
    }

    protected override bool OnCheck()
    {
        return VideoAdsController.Instance.IsVideoAdsAvailable(placement.value);
    }

}

}
