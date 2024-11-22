using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    public class BI_sso_connect : ActionTask<Blackboard>
    {
        public BBParameter<SSOConnectType> type;

        protected override string info
        {
            get
            {
                return string.Format("adjust_sso_connect({0})", type);
            }
        }

        public enum SSOConnectType
        {
            EMAIL = 0,
            FACEBOOK,
            APPLE
        }

        protected override void OnExecute()
        {
            switch(type.value)
            {
                case SSOConnectType.EMAIL:
                    AdjustManager.Instance.SendEvent("email_connect");
                    break;
                case SSOConnectType.FACEBOOK:
                    AdjustManager.Instance.SendEvent("fb_connect");
                    break;
                case SSOConnectType.APPLE:
                    AdjustManager.Instance.SendEvent("apple_connect");
                    break;
            }

            EndAction();
        }
    }

}
