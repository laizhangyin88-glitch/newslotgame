using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BagelCode.ClientModels;

namespace BagelCode.VegasDreams
{
    public static class VegasDreamsErrorHandler
    {
        public static void OpenVegasDreamsEndPopup(System.Action callback)
        {
            ErrorPopupInfo info = new ErrorPopupInfo();
            info.type = ErrorPopupType.OK;
            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "VEGAS_DREAMS_END_ERROR_POPUP_TEXT");
            info.buttonText1 = "OK";

            info.callback1 = delegate
            {
                if (callback != null)
                {
                    callback();
                }
            };

            ErrorPopupHandler.Instance.OpenError(info);
        }
    }
}
