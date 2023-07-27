using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions {

    [Category("★ BagelCode/TimeUtils")]
    public class GetTimeZoneOffset : ActionTask {
        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override string info {
            get {
                return string.Format("Get Time Zone Offset as {0}", saveAs); 
            }
        }

        protected override void OnExecute() {
            saveAs.value = BagelCode.TimeUtils.GetTimeZoneOffset();
            EndAction();
        }
    }
}

