using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static bool IsHideUI()
        {
            return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "hideUI")?.value ?? false;
        }

        public static bool IsAvailableDeleteAccount()
        {
#if DEV
            bool isSkip = PlayerPrefs.GetInt("DEBUG_SKIP_DELETE_ACCOUNT_EMAIL_CONFIRMATION", 0) == 1;
            if (isSkip) return true;
#endif
            return BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "isAccountRemovalPermitted")?.value ?? false;
        }
    }
}
