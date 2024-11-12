using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TimeUtils")]
    public class GetLastPacketResponseTime : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<long> timestamp;

        protected override string info
        {
            get { return string.Format("{0} = Get Last Packet Response Time", timestamp); }
        }

        protected override void OnExecute()
        {
            timestamp.value = BagelCode.Internal.BagelCodeHTTP.lastPacketTime;
            EndAction();
        }
    }
}

