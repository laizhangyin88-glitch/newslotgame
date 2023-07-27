using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{

	[Category("★ BagelCode/SlotMachine")]
	public class WaitLeastOneFrame : ActionTask {

		public BBParameter<float> waitTime = new BBParameter<float>{value = 1};
		public CompactStatus finishStatus = CompactStatus.Success;

        private bool firstFrame;

		protected override string info{
			get {return "Wait " + waitTime + " sec.";}
		}

        protected override void OnExecute()
        {
            firstFrame = true;
        }

		protected override void OnUpdate(){
			if (!firstFrame && elapsedTime >= waitTime.value){
				EndAction(finishStatus == CompactStatus.Success? true : false);
			}

            firstFrame = false;
		}
	}
}
