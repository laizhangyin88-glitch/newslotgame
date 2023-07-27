using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class IDFAHelper : MonoWeakSingleton<IDFAHelper>
    {
        IIDFA delegator;
        string _bicontextID;

        public const string IDFA_CONSENT_RESULT = "idfaConsentResult";

        private void Awake()
        {
#if UNITY_IOS && !UNITY_EDITOR
            delegator = new IDFAIOS();
#endif
        }

        public void Initialize()
        {
            delegator.Initialize(gameObject.name);
        }

        public void RequestIDFA(string contextID)
        {
            _bicontextID = contextID;
            if(string.IsNullOrEmpty(_bicontextID))
                _bicontextID = BiEventUtils.GenerateContextID();

            delegator?.RequestIDFA();
        }

        public bool IsEligibleToSeeIDFAConsentPopup()
        {
            return delegator?.IsEligibleToSeeIDFAConsentPopup() ?? false;
        }

        public bool HasSeenIDFAConsentPopup()
        {
            return delegator?.HasSeenIDFAConsentPopup() ?? true;
        }

        public void SetHasSeenIDFAConsentPopup()
        {
            delegator?.SetHasSeenIDFAConsentPopup();
        }

        // This function call from native code.
        public void IDFARequestResult(string result)
        {
            Debug.Log( string.Format("IDFA OnIDFARequestResult : {0}", result) );

            // result types
            // ATTrackingManagerAuthorizationStatusAuthorized
            // ATTrackingManagerAuthorizationStatusRestricted
            // ATTrackingManagerAuthorizationStatusDenied
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), IDFA_CONSENT_RESULT, result);
            SetHasSeenIDFAConsentPopup();

            string IDFAAction;
            if (result == "ATTrackingManagerAuthorizationStatusAuthorized")
                IDFAAction = "allow";
            else
                IDFAAction = "disallow";

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["action"] = IDFAAction;
            customData["context_id"] = _bicontextID;
            Analytics.CustomEvent("client_idfa_allow", customData);
        }
    }
}
