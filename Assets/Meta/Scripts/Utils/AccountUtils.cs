using UnityEngine;
using SlotMaker;
using System;
using System.Collections;
using NodeCanvas.Framework;

namespace BagelCode
{

public class AccountUtils
{
	public static bool HasAccount()
	{
        var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(null, "/ssoAccountInfo");
        if (ssoAccountInfo != null && ssoAccountInfo.value != null)
            return true;
        return false;
	}

	public static bool IsFacebookConnected()
	{
        var facebookId = BlackboardUtils.FindVariable<string>(null, "/me/facebookId");
        // var facebookId = BlackboardUtils.FindVariable<string>(null, "/ssoAccountInfo/facebookId");
        if (facebookId != null && facebookId.value != null && !string.IsNullOrEmpty(facebookId.value))
            return true;
        return false;
	}

    public static bool IsEmailConnected()
    {
        var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(null, "/ssoAccountInfo");
        if (ssoAccountInfo != null && ssoAccountInfo.value != null)
        {
            var email = BlackboardUtils.FindVariable<string>(null, "/ssoAccountInfo/email");
            if (email != null && email.value != null && !string.IsNullOrEmpty(email.value))
                return true;
        }

        return false;
    }
}

}
