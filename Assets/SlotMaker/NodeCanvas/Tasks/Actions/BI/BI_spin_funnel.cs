using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_spin_funnel : ActionTask
    {
        protected override void OnExecute()
        {
            int gameId = BlackboardUtils.FindValue<int>("./game/gameId");

            Analytics.spin_funnel(gameId);

            EndAction();
        }
    }
}
