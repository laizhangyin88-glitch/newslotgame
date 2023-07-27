using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public interface IIDFA
    {
        void Initialize(string name);
        void RequestIDFA();
        bool IsEligibleToSeeIDFAConsentPopup();
        bool HasSeenIDFAConsentPopup();
        void SetHasSeenIDFAConsentPopup();
    }
}
