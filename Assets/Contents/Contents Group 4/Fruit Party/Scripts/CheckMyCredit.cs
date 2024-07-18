using BagelCode;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMyCredit
{
    public static bool CheckCredit()
    {
        int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;
        if (globalStore.IsHaveLineSelect.ContainsKey(gameId))
        {
            int line = BlackboardUtils.GetOrCreateVariable<int>(ContentBlackboard.Get(), "./game/lineCount").value;
            long betCredit = BlackboardUtils.FindVariable<long>("./betCredit").value;
            var meCredit = BlackboardUtils.FindVariable<long>(null, "/me/credit");
            if (line * betCredit > meCredit.value)
            {
                ErrorPopupInfo info = new ErrorPopupInfo();
                info.text = "<size=32>Sorry, your balance has been insufficient, please recharge</size>"; 
                info.type = ErrorPopupType.OK;
                info.buttonText1 = "OK";
                ErrorPopupHandler.Instance.OpenError(info);
                return false;
            }
        }
        return true;
    }
}
