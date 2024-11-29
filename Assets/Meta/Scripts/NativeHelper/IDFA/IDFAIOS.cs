#if UNITY_IOS && !UNITY_EDITOR

using System.Runtime.InteropServices;

namespace BagelCode
{
    public class IDFAIOS : IIDFA
    {
        //[DllImport ("__Internal")]
        //private static extern void _initializeIDFA(string name);
        public void Initialize(string name)
        {
            //_initializeIDFA(name);
        }

        //[DllImport("__Internal")]
        //public static extern void _requestIDFA();
        public void RequestIDFA()
        {
            //_requestIDFA();
        }

        //[DllImport("__Internal")]
        //public static extern bool _isEligibleToSeeIDFAConsentPopup();
        public bool IsEligibleToSeeIDFAConsentPopup()
        {
            //return _isEligibleToSeeIDFAConsentPopup();
            return false;
        }

        //[DllImport("__Internal")]
        //public static extern bool _hasSeenIDFAConsentPopup();
        public bool HasSeenIDFAConsentPopup()
        {
            //return _hasSeenIDFAConsentPopup();
            return false;
        }

        //[DllImport("__Internal")]
        //public static extern void _setHasSeenIDFAConsentPopup();
        public void SetHasSeenIDFAConsentPopup()
        {
            //_setHasSeenIDFAConsentPopup();
        }
    }
}

#endif
