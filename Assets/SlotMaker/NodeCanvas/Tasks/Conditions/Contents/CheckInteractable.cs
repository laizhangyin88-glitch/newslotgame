using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Conditions.Contents
{
[Category("★ BagelCode/Contents")]
public class CheckInteractable : ConditionTask<UnityEngine.UI.Selectable>
{
    protected override string info 
    {
        get
        {
            return string.Format("Check buton interacble");
        }
    }

    protected override bool OnCheck()
    {
        return agent.IsInteractable();
    }
}

}
