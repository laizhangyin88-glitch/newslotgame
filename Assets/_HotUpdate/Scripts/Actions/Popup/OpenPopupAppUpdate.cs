using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class OpenPopupAppUpdate : ActionTask<ContextElement>
{
    public BBParameter<string> elementName;
    public ContextSearchingType searchingType = ContextSearchingType.ChildrenSearch;
    public BBParameter<bool> ignoreReset;
    
    protected override string info
    {
        get { return "Regist Open Popup App"; }
    }

    protected override void OnExecute()
    {
        IContextClickable clickableElement = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), elementName.value, searchingType) as IContextClickable;
        if (clickableElement != null)
        {
            if (ignoreReset.value == false)
                clickableElement.RemoveAllListener();

            clickableElement.AddListenerOnClick( (ContextElement sender) => { OpenPopup(); } );

            EndAction();
        }
        else
        {
            Debug.LogError("[Context] " + elementName.value + " is not exist or not IContextClickable");
            EndAction(false);
        }
    }
    
    private void OpenPopup()
    {
        string appDownloadURL = BlackboardUtils.FindVariable<string>(null, "/appDownloadUrl").value;

        var rewardCoins = BlackboardUtils.FindVariable<long>(null, "/values/misc/VERSION_UPDATE_REWARD_CREDIT");

        bool isError = false;

        ErrorPopupInfo info = new ErrorPopupInfo();

        info.type = ErrorPopupType.OkWithTitle;
        info.title = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_TITLE", out isError);
        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_REWARD", out isError, rewardCoins.value);

        info.useXButton = true;

        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out isError);
        info.buttonAutoClose1 = false;

        info.callback1 = delegate
        {
            Application.OpenURL(appDownloadURL);
        };

        info.callbackX = delegate
        {
        };

        ErrorPopupHandler.Instance.OpenError(info);
    }
}

}
