using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TimeUtils")]
    public class GetNextDayTimeStamp : ActionTask
    {
        public BBParameter<bool> isLocal;
        public BBParameter<int> offsetHours;
        [BlackboardOnly]
        public BBParameter<long> saveAs;

        protected override string info
        {
            get { return "Get Next Day Time Stamp"; }
        }

        protected override void OnExecute()
        {
            saveAs.value = TimeUtils.GetNextDayTimestamp(isLocal.value, offsetHours.value);
            EndAction();
        }
    }
}

