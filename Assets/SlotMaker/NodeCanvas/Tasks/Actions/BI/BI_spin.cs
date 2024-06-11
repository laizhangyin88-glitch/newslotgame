using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_spin : ActionTask<Blackboard>
    {
        public BBParameter<string> spinType;

        protected override void OnExecute()
        {
            int gameId = BlackboardUtils.FindValue<int>("./game/gameId");
            long betCredit = BlackboardUtils.FindValue<long>("./turn/totalBetCredit");
            long earnCredit = BlackboardUtils.FindValue<long>("./spin/response/result/earnCredit");
            var autoSpin = BlackboardUtils.FindValue<bool>("./autoSpin");

            var spinTypeValue = BlackboardUtils.FindVariable<int>(agent, spinType.value);
            bool freeSpin = false;
            if (spinTypeValue != null)
                freeSpin = System.Convert.ToBoolean(spinTypeValue.value);

            Analytics.spin(gameId, betCredit, earnCredit, freeSpin, autoSpin);

            EndAction();
        }
    }
}
