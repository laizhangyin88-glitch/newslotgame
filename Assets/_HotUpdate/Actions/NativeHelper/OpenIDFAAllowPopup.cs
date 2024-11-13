using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/NativeHelper")]
    public class OpenIDFAAllowPopup : ActionTask
    {
        public BBParameter<string> contextID;

        protected override string info
        {
            get
            {
                return string.Format("Open IDFA Allow({0})", contextID);
            }
        }

        protected override void OnExecute()
        {
            if( IDFAHelper.Instance.IsEligibleToSeeIDFAConsentPopup() && !IDFAHelper.Instance.HasSeenIDFAConsentPopup() )
            {
                string idfaContextID = "";
                if(contextID != null)
                    idfaContextID = contextID.value;

                IDFAHelper.Instance.RequestIDFA(idfaContextID);
            }

            EndAction();
        }
    }
}
