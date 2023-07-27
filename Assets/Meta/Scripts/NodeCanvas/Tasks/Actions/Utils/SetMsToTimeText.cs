using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Blackboard")]
    public class SetMsToTimeText : ActionTask<Blackboard>
    {
        public BBParameter<long> milliSecond;

        public BBParameter<string> saveAs;

        protected override string info
        {
            get { return saveAs.name + " = ? hours ? minutes ? seconds"; }
        }

        protected override void OnExecute()
        {
            saveAs.value = BagelCode.TextDecoUtils.MsToTimeText(milliSecond.value);
            EndAction(true);
        }
    }
}